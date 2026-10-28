using System.Buffers.Binary;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TeaQL.BlobReference;

/// <summary>Opt-in Linux MP3/MP4 verifier using host-managed FFmpeg executables.</summary>
public sealed class FfmpegBlobContentVerifier : IBlobContentVerifier, IBlobVerifierReadiness
{
    private readonly string _probePath;
    private readonly string _decodePath;
    private readonly string _limitPath;
    private readonly int _maximumInputBytes;
    private readonly int _maximumFrames;
    private readonly long _maximumVideoPixels;
    private readonly int _concurrency;
    private int _active;

    public FfmpegBlobContentVerifier(string probePath, string decodePath, string limitPath,
        int maximumInputBytes = 32 * 1024 * 1024, int maximumFrames = 100_000,
        long maximumVideoPixels = 16_000_000, int concurrentVerifications = 2)
    {
        if (!Path.IsPathFullyQualified(probePath) || !Path.IsPathFullyQualified(decodePath) ||
            !Path.IsPathFullyQualified(limitPath) || maximumInputBytes is < 1 or > 100 * 1024 * 1024 ||
            maximumFrames is < 1 or > 1_000_000 || maximumVideoPixels is < 1 or > 64_000_000 ||
            concurrentVerifications is < 1 or > 32)
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        _probePath = probePath;
        _decodePath = decodePath;
        _limitPath = limitPath;
        _maximumInputBytes = maximumInputBytes;
        _maximumFrames = maximumFrames;
        _maximumVideoPixels = maximumVideoPixels;
        _concurrency = concurrentVerifications;
    }

    public async Task ValidateReadyAsync()
    {
        if (!OperatingSystem.IsLinux() ||
            new[] { _probePath, _decodePath, _limitPath }.Any(path => !File.Exists(path)))
            throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        try
        {
            var probe = await ExecuteAsync(_probePath, ["-version"], [], TimeSpan.FromSeconds(3));
            var decode = await ExecuteAsync(_decodePath, ["-version"], [], TimeSpan.FromSeconds(3));
            if (!Encoding.ASCII.GetString(probe).StartsWith("ffprobe version ", StringComparison.Ordinal) ||
                !Encoding.ASCII.GetString(decode).StartsWith("ffmpeg version ", StringComparison.Ordinal))
                throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        }
        catch { throw new InvalidOperationException("BLOB_CAPABILITY_MISSING"); }
    }

    private async Task<byte[]> ExecuteAsync(string binary, string[] arguments, byte[] input,
        TimeSpan timeout)
    {
        var start = new ProcessStartInfo(_limitPath) {
            UseShellExecute = false, RedirectStandardInput = true,
            RedirectStandardOutput = true, RedirectStandardError = true
        };
        start.Environment.Clear();
        start.Environment["LC_ALL"] = "C";
        foreach (var item in new[] { "--cpu=10:10", "--as=536870912:536870912", "--", binary })
            start.ArgumentList.Add(item);
        foreach (var item in arguments) start.ArgumentList.Add(item);
        using var process = new Process { StartInfo = start };
        var started = false;
        try
        {
            if (!process.Start()) throw new InvalidOperationException();
            started = true;
            using var cancellation = new CancellationTokenSource(timeout);
            var token = cancellation.Token;
            var inputTask = Task.Run(async () => {
                if (input.Length > 0) await process.StandardInput.BaseStream.WriteAsync(input, token);
                process.StandardInput.Close();
            }, token);
            var errorTask = process.StandardError.BaseStream.CopyToAsync(Stream.Null, token);
            using var output = new MemoryStream();
            var buffer = new byte[8192];
            while (true)
            {
                var count = await process.StandardOutput.BaseStream.ReadAsync(buffer, token);
                if (count == 0) break;
                if (count > 65_536 - output.Length) throw new InvalidOperationException();
                output.Write(buffer, 0, count);
            }
            await Task.WhenAll(inputTask, errorTask, process.WaitForExitAsync(token));
            if (process.ExitCode != 0) throw new InvalidOperationException();
            return output.ToArray();
        }
        catch { throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED"); }
        finally
        {
            if (started && !process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
            }
        }
    }

    public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension, long maximumBytes)
    {
        if (extension is not ("mp3" or "mp4")) throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
        if (content == null || maximumBytes < 1) throw new InvalidOperationException("BLOB_POLICY_INVALID");
        if (Interlocked.Increment(ref _active) > _concurrency)
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
                    var count = await content.ReadAsync(buffer);
                    if (count == 0) break;
                    if (count > bound - output.Length) throw new InvalidOperationException("BLOB_SIZE_EXCEEDED");
                    output.Write(buffer, 0, count);
                }
            }
            catch (InvalidOperationException error) when (error.Message == "BLOB_SIZE_EXCEEDED") { throw; }
            catch { throw new InvalidOperationException("BLOB_STORE_FAILED"); }
            var data = output.ToArray();
            if (!(extension == "mp3" ? CompleteMp3(data, _maximumFrames) : CompleteMp4(data)))
                throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            await ValidateReadyAsync();
            var demuxer = extension == "mp3" ? "mp3" : "mov";
            var probe = await ExecuteAsync(_probePath,
                ["-v", "error", "-protocol_whitelist", "pipe", "-f", demuxer,
                    "-count_frames", "-show_entries",
                    "stream=codec_type,codec_name,nb_read_frames,width,height,sample_rate:stream_disposition=attached_pic:format=format_name",
                    "-of", "json", "-i", "pipe:0"], data, TimeSpan.FromSeconds(15));
            try
            {
                using var document = JsonDocument.Parse(probe);
                var root = document.RootElement;
                var format = root.GetProperty("format").GetProperty("format_name").GetString() ?? "";
                var streams = root.GetProperty("streams").EnumerateArray().ToArray();
                if (streams.Length is < 1 or > 8) throw new InvalidOperationException();
                if (extension == "mp3")
                {
                    var stream = streams[0];
                    if (format != "mp3" || streams.Length != 1 ||
                        Get(stream, "codec_type") != "audio" || Get(stream, "codec_name") != "mp3" ||
                        !int.TryParse(Get(stream, "sample_rate"), out var rate) || rate is < 8000 or > 192000 ||
                        !int.TryParse(Get(stream, "nb_read_frames"), out var frames) ||
                        frames < 1 || frames > _maximumFrames) throw new InvalidOperationException();
                }
                else
                {
                    if (!format.Contains("mp4", StringComparison.Ordinal)) throw new InvalidOperationException();
                    var videos = 0;
                    foreach (var stream in streams)
                    {
                        var type = Get(stream, "codec_type");
                        if (type is not ("audio" or "video")) throw new InvalidOperationException();
                        if (type != "video") continue;
                        videos++;
                        var width = stream.GetProperty("width").GetInt32();
                        var height = stream.GetProperty("height").GetInt32();
                        if (stream.GetProperty("disposition").GetProperty("attached_pic").GetInt32() != 0 ||
                            width < 1 || height < 1 || (long)width * height > _maximumVideoPixels ||
                            !int.TryParse(Get(stream, "nb_read_frames"), out var frames) ||
                            frames < 1 || frames > _maximumFrames) throw new InvalidOperationException();
                    }
                    if (videos is < 1 or > 4) throw new InvalidOperationException();
                }
            }
            catch { throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED"); }
            var args = new List<string> { "-nostdin", "-v", "error", "-xerror", "-err_detect", "explode",
                "-threads", "1", "-protocol_whitelist", "pipe", "-f", demuxer, "-i", "pipe:0" };
            args.AddRange(extension == "mp3" ? ["-map", "0:a:0"] : ["-map", "0:v", "-map", "0:a?"]);
            args.AddRange(["-f", "null", "-"]);
            await ExecuteAsync(_decodePath, [.. args], data, TimeSpan.FromSeconds(15));
            return new BlobVerifiedContent(data.LongLength,
                Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant(),
                extension == "mp3" ? "audio/mpeg" : "video/mp4");
        }
        finally { Interlocked.Decrement(ref _active); }
    }

    private static string Get(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) ? value.GetString() ?? "" : "";

    private static bool CompleteMp3(byte[] data, int maximumFrames)
    {
        var offset = 0;
        if (data.AsSpan().StartsWith("ID3"u8))
        {
            if (data.Length < 10 || data[3] is < 2 or > 4 || data[4] == 255 ||
                data.AsSpan(6, 4).ToArray().Any(value => (value & 0x80) != 0)) return false;
            offset = 10 + (data[6] << 21 | data[7] << 14 | data[8] << 7 | data[9]);
            if (data[3] == 4 && (data[5] & 0x10) != 0) offset += 10;
            if (offset > data.Length) return false;
        }
        var end = data.Length;
        if (end >= 128 && data.AsSpan(end - 128, 3).SequenceEqual("TAG"u8)) end -= 128;
        var frames = 0;
        var firstRate = 0;
        while (offset < end)
        {
            if (end - offset < 4 || frames >= maximumFrames) return false;
            var header = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(offset, 4));
            var version = (int)(header >> 19 & 3);
            var layer = (int)(header >> 17 & 3);
            var bitrateIndex = (int)(header >> 12 & 15);
            var sampleIndex = (int)(header >> 10 & 3);
            if (header >> 21 != 0x7ff || version == 1 || layer != 1 ||
                bitrateIndex is 0 or 15 || sampleIndex == 3) return false;
            var rate = new[] { 44100, 48000, 32000 }[sampleIndex];
            if (version == 2) rate /= 2;
            else if (version == 0) rate /= 4;
            if (firstRate != 0 && firstRate != rate) return false;
            firstRate = rate;
            int[] bitrates = version == 3
                ? [0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0]
                : [0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0];
            var length = (version == 3 ? 144000 : 72000) * bitrates[bitrateIndex] / rate +
                (int)(header >> 9 & 1);
            if (length < 4 || length > end - offset) return false;
            offset += length;
            frames++;
        }
        return frames > 0 && offset == end;
    }

    private static bool CompleteMp4(byte[] data)
    {
        var brands = new HashSet<string> { "isom", "iso2", "iso5", "iso6", "mp41", "mp42",
            "avc1", "M4V ", "M4V2", "dash" };
        var kinds = new HashSet<string>();
        var offset = 0;
        var count = 0;
        while (offset < data.Length)
        {
            if (data.Length - offset < 8 || count++ >= 1000) return false;
            var size = (ulong)BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(offset, 4));
            var kind = Encoding.ASCII.GetString(data, offset + 4, 4);
            var header = 8;
            if (size == 1)
            {
                if (data.Length - offset < 16) return false;
                size = BinaryPrimitives.ReadUInt64BigEndian(data.AsSpan(offset + 8, 8));
                header = 16;
            }
            else if (size == 0)
            {
                if (kind != "mdat") return false;
                size = (ulong)(data.Length - offset);
            }
            if (size < (ulong)header || size > (ulong)(data.Length - offset)) return false;
            if (count == 1 && (kind != "ftyp" || size < (ulong)(header + 8) ||
                !brands.Contains(Encoding.ASCII.GetString(data, offset + header, 4)))) return false;
            if ((kind is "ftyp" or "moov") && kinds.Contains(kind)) return false;
            kinds.Add(kind);
            offset += (int)size;
        }
        return kinds.Contains("ftyp") && kinds.Contains("moov") && kinds.Contains("mdat") && offset == data.Length;
    }
}
