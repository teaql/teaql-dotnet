using System.Security.Cryptography;
using TeaQL.BlobReference;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public sealed class BlobCompositeVerifierTests
{
    private sealed class UnreadableStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() => throw new NotSupportedException();
        public override int Read(byte[] buffer, int offset, int count) => throw new InvalidOperationException("source read");
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("source read");
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    [Fact]
    public async Task RealImageAndMediaShareOneTrustedUploadVerifier()
    {
        if (!OperatingSystem.IsLinux() ||
            new[] { "/usr/bin/ffprobe", "/usr/bin/ffmpeg", "/usr/bin/prlimit" }.Any(path => !File.Exists(path)))
            return;
        var image = new StbImageBlobContentVerifier();
        var media = new FfmpegBlobContentVerifier("/usr/bin/ffprobe", "/usr/bin/ffmpeg", "/usr/bin/prlimit");
        var routes = new Dictionary<string, IBlobContentVerifier> {
            ["jpg"] = image, ["jpeg"] = image, ["png"] = image,
            ["mp3"] = media, ["mp4"] = media
        };
        var verifier = new CompositeBlobContentVerifier(routes);
        routes.Clear();
        Assert.Equal(["jpeg", "jpg", "mp3", "mp4", "png"], verifier.SupportedExtensions);
        await verifier.ValidateReadyAsync();
        foreach (var (extension, mime) in new[] { ("jpg", "image/jpeg"), ("mp3", "audio/mpeg") })
        {
            var bytes = await File.ReadAllBytesAsync(Path.Combine(AppContext.BaseDirectory,
                "Fixtures", "sample." + extension));
            await using var source = new MemoryStream(bytes, writable: false);
            var actual = await verifier.VerifyAsync(source, extension, bytes.Length);
            Assert.Equal(bytes.Length, actual.SizeBytes);
            Assert.Equal(Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(), actual.Sha256);
            Assert.Equal(mime, actual.ContentType);
        }
        await using var unreadable = new UnreadableStream();
        var unknown = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await verifier.VerifyAsync(unreadable, "docx", 1024));
        Assert.Equal("BLOB_FORMAT_NOT_ALLOWED", unknown.Message);
        Assert.Throws<InvalidOperationException>(() =>
            new CompositeBlobContentVerifier(new Dictionary<string, IBlobContentVerifier> { ["PDF"] = image }));
    }
}
