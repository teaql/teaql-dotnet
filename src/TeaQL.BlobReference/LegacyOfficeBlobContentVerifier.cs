using System.Buffers.Binary;
using System.Diagnostics;
using System.Security.Cryptography;

namespace TeaQL.BlobReference;

/// <summary>Opt-in Linux DOC/XLS verifier using sandboxed, host-managed LibreOffice.</summary>
public sealed class LegacyOfficeBlobContentVerifier : IBlobContentVerifier, IBlobVerifierReadiness
{
    private const UnixFileMode PrivateDirectory = UnixFileMode.UserRead |
        UnixFileMode.UserWrite | UnixFileMode.UserExecute;
    private readonly string _sofficePath;
    private readonly string _bwrapPath;
    private readonly string _limitPath;
    private readonly string _scratchRoot;
    private readonly int _maximumInputBytes;
    private readonly int _maximumConvertedBytes;
    private readonly int _concurrentVerifications;
    private readonly OfficeBlobContentVerifier _packageVerifier;
    private int _active;

    public LegacyOfficeBlobContentVerifier(string sofficePath, string bwrapPath, string limitPath,
        string scratchRoot, int maximumInputBytes = 32 * 1024 * 1024,
        int maximumConvertedBytes = 10 * 1024 * 1024, int concurrentVerifications = 2)
    {
        if (!Path.IsPathFullyQualified(sofficePath) || !Path.IsPathFullyQualified(bwrapPath) ||
            !Path.IsPathFullyQualified(limitPath) || !Path.IsPathFullyQualified(scratchRoot) ||
            Path.GetFullPath(scratchRoot) != scratchRoot ||
            maximumInputBytes is < 1 or > 100 * 1024 * 1024 ||
            maximumConvertedBytes is < 1 or > 10 * 1024 * 1024 ||
            concurrentVerifications is < 1 or > 32)
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        _sofficePath = sofficePath;
        _bwrapPath = bwrapPath;
        _limitPath = limitPath;
        _scratchRoot = scratchRoot;
        _maximumInputBytes = maximumInputBytes;
        _maximumConvertedBytes = maximumConvertedBytes;
        _concurrentVerifications = concurrentVerifications;
        _packageVerifier = new OfficeBlobContentVerifier(maximumConvertedBytes, 100 * 1024 * 1024,
            10_000, 2_000_000, concurrentVerifications);
    }

    public Task ValidateReadyAsync()
    {
        if (!OperatingSystem.IsLinux()) throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        try
        {
            foreach (var path in new[] { _sofficePath, _bwrapPath, _limitPath })
            {
                var executable = UnixFileMode.UserExecute | UnixFileMode.GroupExecute |
                    UnixFileMode.OtherExecute;
                if (!File.Exists(path) || (File.GetUnixFileMode(path) & executable) == 0)
                    throw new InvalidOperationException();
            }
            var root = new DirectoryInfo(_scratchRoot);
            if (!root.Exists || root.LinkTarget != null ||
                File.GetUnixFileMode(_scratchRoot) != PrivateDirectory)
                throw new InvalidOperationException();
            var probe = Path.Combine(_scratchRoot, "ready-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(probe, PrivateDirectory);
            Directory.Delete(probe);
            return Task.CompletedTask;
        }
        catch { throw new InvalidOperationException("BLOB_CAPABILITY_MISSING"); }
    }

    private static bool ValidContainer(ReadOnlySpan<byte> data)
    {
        ReadOnlySpan<byte> magic = [0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1];
        if (data.Length < 1024 || !data[..8].SequenceEqual(magic) ||
            BinaryPrimitives.ReadUInt16LittleEndian(data[28..]) != 0xfffe ||
            BinaryPrimitives.ReadUInt16LittleEndian(data[32..]) != 6) return false;
        var major = BinaryPrimitives.ReadUInt16LittleEndian(data[26..]);
        var shift = BinaryPrimitives.ReadUInt16LittleEndian(data[30..]);
        if (!((major == 3 && shift == 9) || (major == 4 && shift == 12))) return false;
        var sectorSize = 1 << shift;
        if (data.Length % sectorSize != 0 || data.Length < 2 * sectorSize) return false;
        var sectors = (uint)(data.Length / sectorSize - 1);
        var fat = BinaryPrimitives.ReadUInt32LittleEndian(data[44..]);
        var directory = BinaryPrimitives.ReadUInt32LittleEndian(data[48..]);
        var miniFat = BinaryPrimitives.ReadUInt32LittleEndian(data[60..]);
        var miniCount = BinaryPrimitives.ReadUInt32LittleEndian(data[64..]);
        var difat = BinaryPrimitives.ReadUInt32LittleEndian(data[68..]);
        var difatCount = BinaryPrimitives.ReadUInt32LittleEndian(data[72..]);
        return fat > 0 && fat <= sectors && directory < sectors &&
            BinaryPrimitives.ReadUInt32LittleEndian(data[56..]) == 4096 &&
            miniCount <= sectors && (miniCount == 0 || miniFat < sectors) &&
            difatCount <= sectors && (difatCount == 0 || difat < sectors);
    }

    private async Task<byte[]> ConvertAsync(byte[] data, string extension)
    {
        if (!OperatingSystem.IsLinux())
            throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        var job = Path.Combine(_scratchRoot, "legacy-office-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(job, PrivateDirectory);
            var output = Path.Combine(job, "output");
            var profile = Path.Combine(job, "profile");
            Directory.CreateDirectory(output, PrivateDirectory);
            Directory.CreateDirectory(profile, PrivateDirectory);
            var source = Path.Combine(job, "source." + extension);
            await using (var file = new FileStream(source, FileMode.CreateNew, FileAccess.Write,
                FileShare.None)) await file.WriteAsync(data);
            File.SetUnixFileMode(source, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            var start = new ProcessStartInfo(_limitPath) {
                UseShellExecute = false, RedirectStandardInput = true,
                RedirectStandardOutput = true, RedirectStandardError = true,
                WorkingDirectory = "/"
            };
            start.Environment.Clear();
            start.Environment["LC_ALL"] = "C";
            start.Environment["PATH"] = "/usr/bin:/bin";
            foreach (var item in new[] { "--cpu=20:20", "--as=1073741824:1073741824",
                         "--fsize=10485760:10485760", "--", _bwrapPath,
                         "--unshare-net", "--unshare-pid", "--die-with-parent",
                         "--ro-bind", "/usr", "/usr", "--ro-bind", "/lib", "/lib",
                         "--ro-bind", "/lib64", "/lib64", "--symlink", "usr/bin", "/bin",
                         "--ro-bind", "/etc", "/etc", "--dev", "/dev", "--proc", "/proc",
                         "--dir", "/tmp", "--bind", job, job, "--clearenv",
                         "--setenv", "LC_ALL", "C", "--setenv", "PATH", "/usr/bin:/bin",
                         "--setenv", "HOME", job, "--setenv", "TMPDIR", job,
                         "--", _sofficePath, "-env:UserInstallation=" + new Uri(profile + "/").AbsoluteUri,
                         "--headless", "--nodefault", "--norestore", "--nolockcheck",
                         "--convert-to", extension + "x", "--outdir", output, source })
                start.ArgumentList.Add(item);
            using var process = new Process { StartInfo = start };
            var started = false;
            try
            {
                if (!process.Start()) throw new InvalidOperationException();
                started = true;
                process.StandardInput.Close();
                var stdout = process.StandardOutput.BaseStream.CopyToAsync(Stream.Null);
                var stderr = process.StandardError.BaseStream.CopyToAsync(Stream.Null);
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                await process.WaitForExitAsync(timeout.Token);
                await Task.WhenAll(stdout, stderr);
                if (process.ExitCode != 0) throw new InvalidOperationException();
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
            var files = Directory.GetFileSystemEntries(output);
            var expected = Path.Combine(output, "source." + extension + "x");
            if (files.Length != 1 || files[0] != expected || new FileInfo(expected).LinkTarget != null)
                throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            var info = new FileInfo(expected);
            if (!info.Exists || info.Length is < 1 || info.Length > _maximumConvertedBytes)
                throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            var converted = await File.ReadAllBytesAsync(expected);
            if (converted.LongLength != info.Length)
                throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            return converted;
        }
        catch (InvalidOperationException) { throw; }
        catch { throw new InvalidOperationException("BLOB_STORE_FAILED"); }
        finally { if (Directory.Exists(job)) Directory.Delete(job, recursive: true); }
    }

    public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
        long maximumBytes)
    {
        var mime = extension switch {
            "doc" => "application/msword", "xls" => "application/vnd.ms-excel",
            _ => throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED")
        };
        if (content == null || maximumBytes < 1)
            throw new InvalidOperationException("BLOB_POLICY_INVALID");
        if (!OperatingSystem.IsLinux())
            throw new InvalidOperationException("BLOB_CAPABILITY_MISSING");
        if (Interlocked.Increment(ref _active) > _concurrentVerifications)
        {
            Interlocked.Decrement(ref _active);
            throw new InvalidOperationException("BLOB_VERIFICATION_BUSY");
        }
        try
        {
            await ValidateReadyAsync();
            var bound = Math.Min(maximumBytes, _maximumInputBytes);
            using var output = new MemoryStream();
            var buffer = new byte[64 * 1024];
            try
            {
                while (true)
                {
                    var count = await content.ReadAsync(buffer);
                    if (count == 0) break;
                    if (count > bound - output.Length)
                        throw new InvalidOperationException("BLOB_SIZE_EXCEEDED");
                    output.Write(buffer, 0, count);
                }
            }
            catch (InvalidOperationException error) when (error.Message == "BLOB_SIZE_EXCEEDED") { throw; }
            catch { throw new InvalidOperationException("BLOB_STORE_FAILED"); }
            var data = output.ToArray();
            if (!ValidContainer(data))
                throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED");
            var converted = await ConvertAsync(data, extension);
            try { await _packageVerifier.VerifyAsync(new MemoryStream(converted), extension + "x",
                converted.Length); }
            catch { throw new InvalidOperationException("BLOB_FORMAT_NOT_ALLOWED"); }
            return new BlobVerifiedContent(data.LongLength,
                Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant(), mime);
        }
        finally { Interlocked.Decrement(ref _active); }
    }
}
