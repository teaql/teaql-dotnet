using System.Buffers.Binary;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using StbImageSharp;

namespace TeaQL.BlobReference;

/// <summary>Opt-in Linux PDF verifier using patched, host-managed Poppler executables.</summary>
public sealed class PopplerBlobContentVerifier : IBlobContentVerifier, IBlobVerifierReadiness
{
    private readonly string _infoPath;
    private readonly string _renderPath;
    private readonly string _limitPath;
    private readonly int _maximumInputBytes;
    private readonly int _maximumPages;
    private readonly int _maximumRenderedBytes;
    private readonly int _concurrentVerifications;
    private int _active;

    public PopplerBlobContentVerifier(string infoPath, string renderPath, string limitPath,
        int maximumInputBytes = 32 * 1024 * 1024, int maximumPages = 100,
        int maximumRenderedBytes = 32 * 1024 * 1024, int concurrentVerifications = 2)
    {
        if (!Path.IsPathFullyQualified(infoPath) || !Path.IsPathFullyQualified(renderPath) ||
            !Path.IsPathFullyQualified(limitPath) || maximumInputBytes is < 1 or > 100 * 1024 * 1024 ||
            maximumPages is < 1 or > 200 || maximumRenderedBytes is < 1 or > 100 * 1024 * 1024 ||
            concurrentVerifications is < 1 or > 32)
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        _infoPath = infoPath;
        _renderPath = renderPath;
        _limitPath = limitPath;
        _maximumInputBytes = maximumInputBytes;
        _maximumPages = maximumPages;
        _maximumRenderedBytes = maximumRenderedBytes;
        _concurrentVerifications = concurrentVerifications;
    }

    public async Task ValidateReadyAsync()
    {
        if (!OperatingSystem.IsLinux() ||
            new[] { _infoPath, _renderPath, _limitPath }.Any(path => !File.Exists(path)))
            throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        try
        {
            foreach (var entry in new[] { (_infoPath, "pdfinfo version "),
                         (_renderPath, "pdftoppm version ") })
            {
                var (stdout, stderr) = await ExecuteAsync(entry.Item1, ["-v"], [], 4096,
                    TimeSpan.FromSeconds(3));
                if (!Encoding.ASCII.GetString(stderr.Length > 0 ? stderr : stdout)
                    .StartsWith(entry.Item2, StringComparison.Ordinal))
                    throw new InvalidOperationException();
            }
        }
        catch { throw new InvalidOperationException("BLOB_CAPABILITY_MISSING"); }
    }

    private static async Task<byte[]> ReadBoundedAsync(Stream source, int maximum,
        CancellationToken cancellation)
    {
        using var output = new MemoryStream();
        var buffer = new byte[8192];
        while (true)
        {
            var count = await source.ReadAsync(buffer, cancellation);
            if (count == 0) return output.ToArray();
            if (count > maximum - output.Length) throw new InvalidOperationException();
            output.Write(buffer, 0, count);
        }
    }

    private async Task<(byte[] Stdout, byte[] Stderr)> ExecuteAsync(string binary,
        string[] arguments, byte[] input, int maximumOutput, TimeSpan timeout)
    {
        var start = new ProcessStartInfo(_limitPath) {
            UseShellExecute = false, RedirectStandardInput = true,
            RedirectStandardOutput = true, RedirectStandardError = true
        };
        start.Environment.Clear();
        start.Environment["LC_ALL"] = "C";
        foreach (var item in new[] { "--cpu=10:10", "--as=536870912:536870912",
                     "--fsize=1048576:1048576", "--", binary }) start.ArgumentList.Add(item);
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
            var stdoutTask = ReadBoundedAsync(process.StandardOutput.BaseStream, maximumOutput, token);
            var stderrTask = ReadBoundedAsync(process.StandardError.BaseStream, 4096, token);
            var stdout = await stdoutTask;
            var stderr = await stderrTask;
            await Task.WhenAll(inputTask, process.WaitForExitAsync(token));
            if (process.ExitCode != 0) throw new InvalidOperationException();
            return (stdout, stderr);
        }
        catch { throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED"); }
        finally
        {
            if (started && !process.HasExited)
            {
                try { process.Kill(entireProcessTree: true); }
                catch (InvalidOperationException) { }
                await process.WaitForExitAsync();
            }
        }
    }

    private static bool CompletePdf(byte[] data)
    {
        if (data.Length < 32 || !data.AsSpan().StartsWith("%PDF-"u8) ||
            data[5] is not ((byte)'1' or (byte)'2') || data[6] != '.' ||
            data[7] is < (byte)'0' or > (byte)'9') return false;
        var end = data.Length;
        while (end > 0 && data[end - 1] is (byte)' ' or (byte)'\t' or (byte)'\r' or
                   (byte)'\n' or (byte)'\f') end--;
        return end >= 5 && data.AsSpan(end - 5, 5).SequenceEqual("%%EOF"u8) &&
            data.AsSpan(0, end - 5).IndexOf("startxref"u8) >= 0;
    }

    private static int PageCount(byte[] output, int maximum)
    {
        var pages = 0;
        string? encrypted = null;
        foreach (var line in Encoding.ASCII.GetString(output).Split('\n'))
        {
            if (line.StartsWith("Pages:", StringComparison.Ordinal))
            {
                if (pages != 0 || !int.TryParse(line[6..].Trim(), out pages) ||
                    pages < 1 || pages > maximum) return 0;
            }
            else if (line.StartsWith("Encrypted:", StringComparison.Ordinal))
            {
                if (encrypted != null) return 0;
                encrypted = line[10..].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();
            }
        }
        return encrypted == "no" ? pages : 0;
    }

    private static bool RenderedPages(byte[] output, int expected)
    {
        ReadOnlySpan<byte> signature = [137, 80, 78, 71, 13, 10, 26, 10];
        var offset = 0;
        var pages = 0;
        while (offset < output.Length)
        {
            if (pages >= expected || output.Length - offset < 33 ||
                !output.AsSpan(offset, 8).SequenceEqual(signature) ||
                BinaryPrimitives.ReadUInt32BigEndian(output.AsSpan(offset + 8, 4)) != 13 ||
                !output.AsSpan(offset + 12, 4).SequenceEqual("IHDR"u8)) return false;
            var width = BinaryPrimitives.ReadUInt32BigEndian(output.AsSpan(offset + 16, 4));
            var height = BinaryPrimitives.ReadUInt32BigEndian(output.AsSpan(offset + 20, 4));
            if (width is < 1 or > 64 || height is < 1 or > 64) return false;
            var position = offset + 8;
            var chunks = 0;
            while (true)
            {
                if (chunks++ >= 1000 || output.Length - position < 12) return false;
                var length = BinaryPrimitives.ReadUInt32BigEndian(output.AsSpan(position, 4));
                if (length > output.Length - position - 12) return false;
                var kind = output.AsSpan(position + 4, 4);
                position += checked((int)length + 12);
                if (!kind.SequenceEqual("IEND"u8)) continue;
                if (length != 0) return false;
                try
                {
                    var image = ImageResult.FromMemory(output.AsSpan(offset, position - offset).ToArray(),
                        ColorComponents.RedGreenBlueAlpha);
                    if (image.Width != width || image.Height != height ||
                        image.Data.Length != (int)(width * height * 4)) return false;
                }
                catch { return false; }
                offset = position;
                pages++;
                break;
            }
        }
        return pages == expected && offset == output.Length;
    }

    public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
        long maximumBytes)
    {
        if (extension != "pdf") throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
        if (content == null || maximumBytes < 1) throw new InvalidOperationException("BLOB_POLICY_INVALID");
        if (!OperatingSystem.IsLinux()) throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
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
                    var count = await content.ReadAsync(buffer);
                    if (count == 0) break;
                    if (count > bound - output.Length) throw new InvalidOperationException("BLOB_SIZE_EXCEEDED");
                    output.Write(buffer, 0, count);
                }
            }
            catch (InvalidOperationException error) when (error.Message == "BLOB_SIZE_EXCEEDED") { throw; }
            catch { throw new InvalidOperationException("BLOB_STORE_FAILED"); }
            var data = output.ToArray();
            if (!CompletePdf(data)) throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            await ValidateReadyAsync();
            // Each child has its own CPU/address-space limit; the parent bounds both outputs.
            var (info, _) = await ExecuteAsync(_infoPath, ["-"], data, 65_536,
                TimeSpan.FromSeconds(10));
            var pages = PageCount(info, _maximumPages);
            if (pages == 0) throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            var (rendered, _) = await ExecuteAsync(_renderPath,
                ["-f", "1", "-l", pages.ToString(), "-scale-to", "64", "-png", "-"],
                data, _maximumRenderedBytes, TimeSpan.FromSeconds(10));
            if (!RenderedPages(rendered, pages)) throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            return new BlobVerifiedContent(data.LongLength,
                Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant(), "application/pdf");
        }
        finally { Interlocked.Decrement(ref _active); }
    }
}
