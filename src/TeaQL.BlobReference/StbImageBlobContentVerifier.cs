using System.Buffers.Binary;
using System.Security.Cryptography;
using StbImageSharp;

namespace TeaQL.BlobReference;

/// <summary>Bounded full-pixel JPEG/PNG verifier for hosts that opt into image uploads.</summary>
public sealed class StbImageBlobContentVerifier : IBlobContentVerifier
{
    private readonly int _maximumInputBytes;
    private readonly int _maximumDecodedBytes;
    private readonly int _concurrentVerifications;
    private int _active;

    public StbImageBlobContentVerifier(int maximumInputBytes = 32 * 1024 * 1024,
        int maximumDecodedBytes = 100 * 1024 * 1024, int concurrentVerifications = 2)
    {
        if (maximumInputBytes is < 1 or > 100 * 1024 * 1024 ||
            maximumDecodedBytes is < 4 or > 512 * 1024 * 1024 ||
            concurrentVerifications is < 1 or > 32)
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        _maximumInputBytes = maximumInputBytes;
        _maximumDecodedBytes = maximumDecodedBytes;
        _concurrentVerifications = concurrentVerifications;
    }

    public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
        long maximumBytes)
    {
        var mime = extension switch
        {
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            _ => throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED")
        };
        if (content == null || maximumBytes < 1)
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        if (Interlocked.Increment(ref _active) > _concurrentVerifications)
        {
            Interlocked.Decrement(ref _active);
            throw new InvalidOperationException("BLOB_VERIFICATION_BUSY");
        }
        try
        {
            var bound = Math.Min(maximumBytes, _maximumInputBytes);
            using var output = new MemoryStream();
            var buffer = new byte[64 * 1024];
            try
            {
                while (true)
                {
                    var count = await content.ReadAsync(buffer).ConfigureAwait(false);
                    if (count == 0) break;
                    if (count > bound - output.Length)
                        throw new InvalidOperationException("BLOB_SIZE_EXCEEDED");
                    output.Write(buffer, 0, count);
                }
            }
            catch (InvalidOperationException error) when (error.Message == "BLOB_SIZE_EXCEEDED")
            { throw; }
            catch { throw new InvalidOperationException("BLOB_STORE_FAILED"); }
            var bytes = output.ToArray();
            if (bytes.Length == 0 || !(extension == "png" ? CompletePng(bytes) : CompleteJpeg(bytes)))
                throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            try
            {
                using var header = new MemoryStream(bytes, writable: false);
                var info = ImageInfo.FromStream(header);
                if (info == null || info.Value.Width < 1 || info.Value.Height < 1 ||
                    (long)info.Value.Width * info.Value.Height * 4 > _maximumDecodedBytes)
                    throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
                var image = ImageResult.FromMemory(bytes, ColorComponents.RedGreenBlueAlpha);
                if (image.Width != info.Value.Width || image.Height != info.Value.Height ||
                    image.Data.LongLength != (long)image.Width * image.Height * 4 ||
                    image.Data.LongLength > _maximumDecodedBytes)
                    throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            }
            catch { throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED"); }
            return new BlobVerifiedContent(bytes.LongLength,
                Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(), mime);
        }
        finally { Interlocked.Decrement(ref _active); }
    }

    private static bool CompleteJpeg(byte[] bytes) => bytes.Length >= 4 &&
        bytes[0] == 0xff && bytes[1] == 0xd8 &&
        bytes[^2] == 0xff && bytes[^1] == 0xd9;

    private static bool CompletePng(byte[] bytes)
    {
        ReadOnlySpan<byte> signature = [137, 80, 78, 71, 13, 10, 26, 10];
        if (bytes.Length < 45 || !bytes.AsSpan(0, 8).SequenceEqual(signature)) return false;
        var offset = 8;
        var chunks = 0;
        var imageData = false;
        while (offset <= bytes.Length - 12)
        {
            var length = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(offset, 4));
            if (length > bytes.Length - offset - 12) return false;
            var kind = bytes.AsSpan(offset + 4, 4);
            if (!kind.ToArray().All(value => value is >= (byte)'A' and <= (byte)'Z' or
                >= (byte)'a' and <= (byte)'z')) return false;
            if (chunks == 0 && (length != 13 || !kind.SequenceEqual("IHDR"u8))) return false;
            uint crc = 0xffffffff;
            foreach (var value in bytes.AsSpan(offset + 4, checked((int)length + 4)))
            {
                crc ^= value;
                for (var bit = 0; bit < 8; bit++)
                    crc = (crc & 1) != 0 ? 0xedb88320u ^ (crc >> 1) : crc >> 1;
            }
            if ((crc ^ 0xffffffffu) != BinaryPrimitives.ReadUInt32BigEndian(
                    bytes.AsSpan(offset + 8 + (int)length, 4))) return false;
            offset += checked((int)length + 12);
            chunks++;
            if (kind.SequenceEqual("IDAT"u8)) imageData = true;
            if (kind.SequenceEqual("IEND"u8))
                return length == 0 && imageData && offset == bytes.Length;
        }
        return false;
    }
}
