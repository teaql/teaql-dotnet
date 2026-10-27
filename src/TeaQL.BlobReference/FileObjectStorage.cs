using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TeaQL.Core.Storage;
using TeaQL.Runtime.Storage;

namespace TeaQL.BlobReference;

/// <summary>
/// A server-owned local store. The root and its parent must be protected from untrusted writers.
/// Old immutable data generations require a separate collector; deleting a key removes its manifest.
/// Start explicitly enables the loopback-only HTTP read gateway.
/// </summary>
public sealed class FileObjectStorage : IObjectStorageProvider, IAsyncDisposable
{
    private sealed record Manifest(string Version, long Size, string ContentType, string Sha256,
        DateTimeOffset Modified, Dictionary<string, string> Metadata);
    private sealed record Ticket(string Hash, string Version, long ExpiresUnixMs, string? Disposition);

    private readonly string _root;
    private readonly byte[] _secret;
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly object _gatewayLock = new();
    private HttpListener? _listener;
    private Task? _worker;
    private string? _baseUrl;

    public FileObjectStorage(string root, byte[] signingKey)
    {
        if (string.IsNullOrWhiteSpace(root) || !Path.IsPathFullyQualified(root) || signingKey?.Length != 32)
            throw new StorageException(StorageErrorCode.InvalidInput);
        _root = Path.GetFullPath(root);
        _secret = signingKey.ToArray();
    }

    private static string Hash(ObjectKey key) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key.Value))).ToLowerInvariant();
    private string DirectoryFor(string hash) => Path.Combine(_root, hash[..2], hash);
    private static string DataPath(string directory, string version) => Path.Combine(directory, "data-" + version);
    private static string Version() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(24)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    private static bool ValidVersion(string? version) => version is { Length: 32 } &&
        version.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_');
    private static bool ValidHash(string? hash) => hash is { Length: 64 } && hash.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f');

    private void EnsureRoot(bool create)
    {
        try
        {
            if (create)
            {
                if (OperatingSystem.IsWindows()) Directory.CreateDirectory(_root);
                else Directory.CreateDirectory(_root, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }
            var info = new DirectoryInfo(_root);
            if (!info.Exists || info.LinkTarget is not null ||
                !OperatingSystem.IsWindows() && (File.GetUnixFileMode(_root) & (UnixFileMode.GroupRead | UnixFileMode.GroupWrite |
                    UnixFileMode.GroupExecute | UnixFileMode.OtherRead | UnixFileMode.OtherWrite | UnixFileMode.OtherExecute)) != 0)
                throw new StorageException(StorageErrorCode.Unavailable);
        }
        catch (StorageException) { throw; }
        catch { throw new StorageException(StorageErrorCode.Unavailable); }
    }

    private Manifest? ReadManifest(string hash)
    {
        var path = Path.Combine(DirectoryFor(hash), "manifest.json");
        try
        {
            if (!File.Exists(path)) return null;
            var info = new FileInfo(path);
            if (info.LinkTarget is not null || info.Length > 16 * 1024) throw new StorageException(StorageErrorCode.Unavailable);
            var value = JsonSerializer.Deserialize<Manifest>(File.ReadAllBytes(path));
            if (value is null || !ValidVersion(value.Version) || value.Size < 0 || value.Size > ObjectPutOptions.MaximumSize ||
                string.IsNullOrEmpty(value.ContentType) || value.ContentType.Any(c => c < 32 || c > 126) ||
                !ValidHash(value.Sha256) || value.Modified == default || value.Metadata is null)
                throw new StorageException(StorageErrorCode.Unavailable);
            return value;
        }
        catch (StorageException) { throw; }
        catch { throw new StorageException(StorageErrorCode.Unavailable); }
    }

    private static ObjectMetadata Metadata(ObjectKey key, Manifest value) =>
        new(key, value.Size, value.ContentType, value.Sha256 + "." + value.Version, value.Modified, value.Metadata);

    private FileStream OpenData(string hash, Manifest value)
    {
        try
        {
            var path = DataPath(DirectoryFor(hash), value.Version);
            var info = new FileInfo(path);
            if (!info.Exists || info.LinkTarget is not null || info.Length != value.Size)
                throw new StorageException(StorageErrorCode.Unavailable);
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, FileOptions.Asynchronous);
        }
        catch (StorageException) { throw; }
        catch { throw new StorageException(StorageErrorCode.Unavailable); }
    }

    public async Task<ObjectMetadata> PutAsync(StorageInvocation invocation, ObjectKey key, Stream content, ObjectPutOptions options)
    {
        if (content is null) throw new StorageException(StorageErrorCode.InvalidInput);
        EnsureRoot(true);
        await _writeLock.WaitAsync(invocation.CancellationToken).ConfigureAwait(false);
        try
        {
            var hash = Hash(key);
            var directory = DirectoryFor(hash);
            if (OperatingSystem.IsWindows()) Directory.CreateDirectory(directory);
            else Directory.CreateDirectory(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            var version = Version();
            var dataPath = DataPath(directory, version);
            var tempPath = Path.Combine(directory, "manifest-" + version);
            var installed = false;
            try
            {
                using var digest = SHA256.Create();
                var dataOptions = new FileStreamOptions
                { Mode = FileMode.CreateNew, Access = FileAccess.Write, Share = FileShare.None, Options = FileOptions.Asynchronous };
                if (!OperatingSystem.IsWindows()) dataOptions.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
                await using (var output = new FileStream(dataPath, dataOptions))
                {
                    var remaining = options.Size;
                    var buffer = new byte[65536];
                    while (remaining > 0)
                    {
                        var count = await content.ReadAsync(buffer.AsMemory(0, (int)Math.Min(remaining, buffer.Length)), invocation.CancellationToken).ConfigureAwait(false);
                        if (count == 0) throw new StorageException(StorageErrorCode.InvalidInput);
                        await output.WriteAsync(buffer.AsMemory(0, count), invocation.CancellationToken).ConfigureAwait(false);
                        digest.TransformBlock(buffer, 0, count, null, 0);
                        remaining -= count;
                    }
                    if (await content.ReadAsync(buffer.AsMemory(0, 1), invocation.CancellationToken).ConfigureAwait(false) != 0)
                        throw new StorageException(StorageErrorCode.InvalidInput);
                    output.Flush(true);
                }
                digest.TransformFinalBlock([], 0, 0);
                var manifest = new Manifest(version, options.Size, options.ContentType,
                    Convert.ToHexString(digest.Hash!).ToLowerInvariant(), DateTimeOffset.UtcNow,
                    new Dictionary<string, string>(options.Metadata, StringComparer.Ordinal));
                var manifestOptions = new FileStreamOptions
                { Mode = FileMode.CreateNew, Access = FileAccess.Write, Share = FileShare.None };
                if (!OperatingSystem.IsWindows()) manifestOptions.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
                await using (var output = new FileStream(tempPath, manifestOptions))
                {
                    await JsonSerializer.SerializeAsync(output, manifest, cancellationToken: invocation.CancellationToken).ConfigureAwait(false);
                    output.Flush(true);
                }
                try { File.Move(tempPath, Path.Combine(directory, "manifest.json"), options.Mode == ObjectWriteMode.Replace); }
                catch (IOException) when (options.Mode == ObjectWriteMode.CreateOnly && File.Exists(Path.Combine(directory, "manifest.json")))
                { throw new StorageException(StorageErrorCode.Conflict); }
                installed = true;
                return Metadata(key, manifest);
            }
            finally
            {
                if (!installed)
                {
                    try { File.Delete(dataPath); } catch { }
                    try { File.Delete(tempPath); } catch { }
                }
            }
        }
        catch (StorageException) { throw; }
        catch (OperationCanceledException) { throw; }
        catch (IOException) { throw new StorageException(StorageErrorCode.Io); }
        catch { throw new StorageException(StorageErrorCode.Unavailable); }
        finally { _writeLock.Release(); }
    }

    public Task<ObjectContent> GetAsync(StorageInvocation invocation, ObjectKey key)
    {
        invocation.CancellationToken.ThrowIfCancellationRequested();
        EnsureRoot(false);
        var hash = Hash(key);
        var value = ReadManifest(hash) ?? throw new StorageException(StorageErrorCode.NotFound);
        return Task.FromResult(new ObjectContent(Metadata(key, value), OpenData(hash, value)));
    }

    public Task<ObjectMetadata?> HeadAsync(StorageInvocation invocation, ObjectKey key)
    {
        invocation.CancellationToken.ThrowIfCancellationRequested();
        EnsureRoot(false);
        var hash = Hash(key);
        var value = ReadManifest(hash);
        if (value is null) return Task.FromResult<ObjectMetadata?>(null);
        using var data = OpenData(hash, value);
        return Task.FromResult<ObjectMetadata?>(Metadata(key, value));
    }

    public async Task DeleteAsync(StorageInvocation invocation, ObjectKey key)
    {
        EnsureRoot(false);
        await _writeLock.WaitAsync(invocation.CancellationToken).ConfigureAwait(false);
        try { File.Delete(Path.Combine(DirectoryFor(Hash(key)), "manifest.json")); }
        catch { throw new StorageException(StorageErrorCode.Io); }
        finally { _writeLock.Release(); }
    }

    private string Seal(Ticket ticket)
    {
        var plain = JsonSerializer.SerializeToUtf8Bytes(ticket);
        var nonce = RandomNumberGenerator.GetBytes(12);
        var output = new byte[nonce.Length + plain.Length + 16];
        nonce.CopyTo(output, 0);
        using (var aes = new AesGcm(_secret, 16))
            aes.Encrypt(nonce, plain, output.AsSpan(12, plain.Length), output.AsSpan(12 + plain.Length, 16));
        return Convert.ToBase64String(output).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private Ticket? OpenTicket(string token)
    {
        try
        {
            if (token.Length is < 40 or > 4096 || token.Any(c => !char.IsAsciiLetterOrDigit(c) && c is not '-' and not '_')) return null;
            var padded = token.Replace('-', '+').Replace('_', '/').PadRight((token.Length + 3) / 4 * 4, '=');
            var bytes = Convert.FromBase64String(padded);
            if (Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_') != token || bytes.Length < 29) return null;
            var plain = new byte[bytes.Length - 28];
            using (var aes = new AesGcm(_secret, 16))
                aes.Decrypt(bytes.AsSpan(0, 12), bytes.AsSpan(12, plain.Length), bytes.AsSpan(12 + plain.Length, 16), plain);
            var ticket = JsonSerializer.Deserialize<Ticket>(plain);
            return ticket is not null && ValidHash(ticket.Hash) && ValidVersion(ticket.Version) &&
                ticket.ExpiresUnixMs > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() &&
                (ticket.Disposition is null || !ticket.Disposition.Any(c => c < 32 || c > 126)) ? ticket : null;
        }
        catch { return null; }
    }

    public Task<ObjectReadUrl> GetReadUrlAsync(StorageInvocation invocation, ObjectKey key, ObjectReadUrlOptions options)
    {
        invocation.CancellationToken.ThrowIfCancellationRequested();
        string? baseUrl;
        lock (_gatewayLock) baseUrl = _baseUrl;
        if (baseUrl is null) throw new StorageException(StorageErrorCode.NotConfigured);
        EnsureRoot(false);
        var hash = Hash(key);
        var value = ReadManifest(hash) ?? throw new StorageException(StorageErrorCode.NotFound);
        using var data = OpenData(hash, value);
        var expires = DateTimeOffset.UtcNow.Add(options.Lifetime);
        var token = Seal(new Ticket(hash, value.Version, expires.ToUnixTimeMilliseconds(), options.ContentDisposition));
        return Task.FromResult(new ObjectReadUrl(new Uri(baseUrl + "/objects/" + token), expires));
    }

    public Uri Start()
    {
        EnsureRoot(true);
        lock (_gatewayLock)
        {
            if (_listener is not null) throw new StorageException(StorageErrorCode.Conflict);
            var port = new TcpListener(IPAddress.Loopback, 0);
            port.Start(); var number = ((IPEndPoint)port.LocalEndpoint).Port; port.Stop();
            var baseUrl = $"http://127.0.0.1:{number}";
            var listener = new HttpListener();
            listener.Prefixes.Add(baseUrl + "/");
            try { listener.Start(); }
            catch { listener.Close(); throw new StorageException(StorageErrorCode.Unavailable); }
            _listener = listener; _baseUrl = baseUrl;
            _worker = Task.Run(async () =>
            {
                while (listener.IsListening)
                {
                    HttpListenerContext context;
                    try { context = await listener.GetContextAsync().ConfigureAwait(false); }
                    catch { break; }
                    _ = Task.Run(() => ServeAsync(context));
                }
            });
            return new Uri(baseUrl);
        }
    }

    private async Task ServeAsync(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        try
        {
            if (request.HttpMethod is not ("GET" or "HEAD")) { response.StatusCode = 405; return; }
            var path = request.Url?.AbsolutePath;
            if (path is null || !path.StartsWith("/objects/", StringComparison.Ordinal) || path.LastIndexOf('/') != 8)
            { response.StatusCode = 404; return; }
            var ticket = OpenTicket(path[9..]);
            if (ticket is null) { response.StatusCode = 404; return; }
            EnsureRoot(false);
            var manifest = ReadManifest(ticket.Hash);
            if (manifest is null || manifest.Version != ticket.Version) { response.StatusCode = 404; return; }
            await using var file = OpenData(ticket.Hash, manifest);
            response.ContentType = manifest.ContentType;
            response.Headers["Cache-Control"] = "private, no-store";
            response.Headers["X-Content-Type-Options"] = "nosniff";
            response.Headers["Accept-Ranges"] = "bytes";
            response.Headers["ETag"] = "\"" + manifest.Sha256 + "." + manifest.Version + "\"";
            if (ticket.Disposition is not null) response.Headers["Content-Disposition"] = ticket.Disposition;
            var start = 0L; var end = manifest.Size - 1;
            var range = request.Headers["Range"];
            if (range is not null)
            {
                var pieces = range.StartsWith("bytes=", StringComparison.Ordinal) && !range[6..].Contains(',')
                    ? range[6..].Split('-') : [];
                var valid = pieces is [var first, var last] && manifest.Size > 0;
                if (valid)
                {
                    if (pieces[0].Length == 0)
                    {
                        valid = long.TryParse(pieces[1], out var suffix) && suffix > 0;
                        if (valid) start = Math.Max(0, manifest.Size - suffix);
                    }
                    else
                    {
                        valid = long.TryParse(pieces[0], out start) && start >= 0 && start < manifest.Size &&
                            (pieces[1].Length == 0 || long.TryParse(pieces[1], out end) && end >= start);
                    }
                }
                if (!valid)
                {
                    response.StatusCode = 416; response.Headers["Content-Range"] = $"bytes */{manifest.Size}"; return;
                }
                end = Math.Min(end, manifest.Size - 1);
                response.StatusCode = 206;
                response.Headers["Content-Range"] = $"bytes {start}-{end}/{manifest.Size}";
            }
            response.ContentLength64 = range is null ? manifest.Size : end - start + 1;
            if (request.HttpMethod == "HEAD") return;
            file.Position = start;
            var remaining = response.ContentLength64;
            var buffer = new byte[65536];
            while (remaining > 0)
            {
                var count = await file.ReadAsync(buffer.AsMemory(0, (int)Math.Min(remaining, buffer.Length))).ConfigureAwait(false);
                if (count == 0) return;
                await response.OutputStream.WriteAsync(buffer.AsMemory(0, count)).ConfigureAwait(false);
                remaining -= count;
            }
        }
        catch { try { response.StatusCode = 404; } catch { } }
        finally { try { response.Close(); } catch { } }
    }

    public async ValueTask DisposeAsync()
    {
        Task? worker;
        lock (_gatewayLock)
        {
            _listener?.Close(); _listener = null; _baseUrl = null;
            worker = _worker; _worker = null;
        }
        if (worker is not null) await worker.ConfigureAwait(false);
    }

    public override string ToString() => "FileObjectStorage[configuration=masked]";
}
