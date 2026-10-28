using System.Collections.ObjectModel;
using System.Text;

namespace TeaQL.Core.Storage;

public enum StorageErrorCode { InvalidInput, NotConfigured, NotFound, AccessDenied, Conflict, Timeout, Unavailable, Unsupported, TooLarge, Io }
public sealed class StorageException : Exception
{
    public StorageErrorCode Code { get; }
    public StorageException(StorageErrorCode code) : base($"Object storage: {code}") => Code = code;
}
public sealed record ObjectKey
{
    public string Value { get; }
    public ObjectKey(string value)
    {
        var valid = !string.IsNullOrEmpty(value) && !value.Any(char.IsControl);
        try { valid = valid && new UTF8Encoding(false, true).GetByteCount(value) <= 1024; }
        catch (EncoderFallbackException) { valid = false; }
        if (!valid) throw new StorageException(StorageErrorCode.InvalidInput);
        Value = value;
    }
    public override string ToString() => "ObjectKey[masked]";
}
public enum ObjectWriteMode { CreateOnly, Replace }
public sealed record ObjectPutOptions
{
    public const long MaximumSize = 5L * 1024 * 1024 * 1024;
    public long Size { get; }
    public ObjectWriteMode Mode { get; }
    public string ContentType { get; }
    public IReadOnlyDictionary<string, string> Metadata { get; }
    internal static string Header(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Any(c => c < 32 || c > 126)) throw new StorageException(StorageErrorCode.InvalidInput);
        return value;
    }
    public ObjectPutOptions(long size, ObjectWriteMode mode, string contentType = "application/octet-stream", IReadOnlyDictionary<string, string>? metadata = null)
    {
        if (size < 0 || !Enum.IsDefined(mode)) throw new StorageException(StorageErrorCode.InvalidInput);
        if (size > MaximumSize) throw new StorageException(StorageErrorCode.TooLarge);
        Size = size; Mode = mode; ContentType = Header(contentType);
        var copy = new Dictionary<string, string>(metadata ?? new Dictionary<string, string>(), StringComparer.Ordinal);
        foreach (var pair in copy)
        {
            if (string.IsNullOrEmpty(pair.Key) || pair.Key[0] == '-' || pair.Key.Any(c => !(c is >= 'a' and <= 'z' or >= '0' and <= '9' or '-')))
                throw new StorageException(StorageErrorCode.InvalidInput);
            Header(pair.Value);
        }
        Metadata = new ReadOnlyDictionary<string, string>(copy);
    }
    public override string ToString() => $"ObjectPutOptions[size={Size}, mode={Mode}, metadata=masked]";
}
public sealed record ObjectMetadata
{
    public ObjectKey Key { get; }
    public long Size { get; }
    public string? ContentType { get; }
    public string? ETag { get; }
    public DateTimeOffset? LastModified { get; }
    public IReadOnlyDictionary<string, string> CustomMetadata { get; }
    public ObjectMetadata(ObjectKey key, long size, string? contentType = null, string? etag = null, DateTimeOffset? lastModified = null, IReadOnlyDictionary<string, string>? metadata = null)
    {
        if (size < 0) throw new StorageException(StorageErrorCode.Unavailable);
        Key = key; Size = size; ContentType = contentType; ETag = etag; LastModified = lastModified;
        CustomMetadata = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(metadata ?? new Dictionary<string, string>(), StringComparer.Ordinal));
    }
    public override string ToString() => $"ObjectMetadata[size={Size}, details=masked]";
}
public sealed record ObjectReadUrlOptions
{
    public TimeSpan Lifetime { get; }
    public string? ContentDisposition { get; }
    public ObjectReadUrlOptions(TimeSpan? lifetime = null, string? disposition = null, string? filename = null)
    {
        Lifetime = lifetime ?? TimeSpan.FromMinutes(5);
        if (Lifetime <= TimeSpan.Zero || Lifetime > TimeSpan.FromDays(7) || Lifetime.Ticks % TimeSpan.TicksPerSecond != 0 || disposition is not (null or "inline" or "attachment"))
            throw new StorageException(StorageErrorCode.InvalidInput);
        if (filename is not null)
        {
            var valid = disposition is not null && !string.IsNullOrEmpty(filename) &&
                !filename.Any(c => char.IsControl(c) || c is '/' or '\\' or '"');
            try { valid = valid && new UTF8Encoding(false, true).GetByteCount(filename) <= 1024; }
            catch (EncoderFallbackException) { valid = false; }
            if (!valid) throw new StorageException(StorageErrorCode.InvalidInput);
        }
        if (disposition is null) ContentDisposition = null;
        else if (filename is null) ContentDisposition = disposition;
        else if (filename.All(c => c <= 126))
            ContentDisposition = $"{disposition}; filename=\"{filename}\"";
        else
        {
            var fallback = new string(filename.Select(c => c <= 126 ? c : '_').ToArray());
            ContentDisposition = $"{disposition}; filename=\"{fallback}\"; " +
                $"filename*=UTF-8''{Uri.EscapeDataString(filename)}";
        }
    }
    public override string ToString() => "ObjectReadUrlOptions[details=masked]";
}
public sealed record ObjectReadUrl(Uri Url, DateTimeOffset ExpiresAt)
{
    public override string ToString() => "ObjectReadUrl[url=masked]";
}
public sealed class ObjectContent(ObjectMetadata metadata, Stream stream) : IDisposable, IAsyncDisposable
{
    public ObjectMetadata Metadata { get; } = metadata;
    public Stream Stream { get; } = stream;
    public void Dispose() => Stream.Dispose();
    public ValueTask DisposeAsync() => Stream.DisposeAsync();
    public override string ToString() => "ObjectContent[masked]";
}
