using System.Diagnostics;
using TeaQL.Core.Storage;

namespace TeaQL.Runtime.Storage;

public sealed record StorageInvocation(UserContext Context, string Comment, CancellationToken CancellationToken)
{
    public override string ToString() => "StorageInvocation[context=masked]";
}
public interface IObjectStorageProvider
{
    Task<ObjectMetadata> PutAsync(StorageInvocation invocation, ObjectKey key, Stream content, ObjectPutOptions options);
    Task<ObjectContent> GetAsync(StorageInvocation invocation, ObjectKey key);
    Task<ObjectMetadata?> HeadAsync(StorageInvocation invocation, ObjectKey key);
    Task DeleteAsync(StorageInvocation invocation, ObjectKey key);
    Task<ObjectReadUrl> GetReadUrlAsync(StorageInvocation invocation, ObjectKey key, ObjectReadUrlOptions options);
}
public enum ObjectConditionalDeleteResult { Deleted, AlreadyMissing, VersionMismatch }
public interface IObjectStorageConditionalDeleter
{
    Task<ObjectConditionalDeleteResult> DeleteIfVersionAsync(StorageInvocation invocation, ObjectKey key, string contentVersion);
}
public sealed record ObjectStorageEvent(string Operation, string Comment, string TraceId, string Outcome, double ElapsedMs);
public interface IObjectStorageObserver { void Record(ObjectStorageEvent observation); }
public static class ObjectStorageContextExtensions
{
    public static UserContext WithObjectStorage(this UserContext context, IObjectStorageProvider provider)
    { context.InsertResource<IObjectStorageProvider>(provider); return context; }
    public static UserContext WithObjectStorageObserver(this UserContext context, IObjectStorageObserver observer)
    { context.InsertResource<IObjectStorageObserver>(observer); return context; }
    public static ObjectStorage ObjectStorage(this UserContext context, string comment) => new(context, comment);
}
internal sealed class StorageReceipt
{
    private readonly UserContext _context;
    private readonly string _operation, _comment;
    private readonly long _started = Stopwatch.GetTimestamp();
    private readonly IRuntimeTelemetryScope _scope;
    private int _ended;
    public StorageReceipt(UserContext context, string operation, string comment)
    { _context = context; _operation = operation; _comment = comment; _scope = context.RuntimeTelemetry.StartSafely(RuntimeOperation.Create("object_storage", operation)); }
    public void Finish(string outcome)
    {
        if (Interlocked.Exchange(ref _ended, 1) != 0) return;
        if (outcome == "success") _scope.Success();
        else _scope.Failure(outcome == "cancelled" ? new OperationCanceledException() : new StorageException(Enum.Parse<StorageErrorCode>(outcome)));
        static string Safe(string? value) => new((value ?? "").Select(c => char.IsControl(c) ? ' ' : c).ToArray());
        var observation = new ObjectStorageEvent(_operation, Safe(_comment), Safe(_context.TraceId), outcome, Stopwatch.GetElapsedTime(_started).TotalMilliseconds);
        try
        {
            var observer = _context.GetResource<IObjectStorageObserver>();
            if (observer is not null) observer.Record(observation);
            else Console.Error.WriteLine($"ObjectStorage operation={observation.Operation} comment={observation.Comment} trace={observation.TraceId} outcome={observation.Outcome} elapsedMs={observation.ElapsedMs:F3}");
        }
        catch { /* Observability is fail open. */ }
    }
}
public sealed class ObjectStorage
{
    private readonly UserContext _context;
    private readonly string _comment;
    private readonly IObjectStorageProvider _provider;
    internal ObjectStorage(UserContext context, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment)) throw new StorageException(StorageErrorCode.InvalidInput);
        _context = context; _comment = comment;
        _provider = context.GetResource<IObjectStorageProvider>() ?? throw new StorageException(StorageErrorCode.NotConfigured);
    }
    private StorageInvocation Invocation(CancellationToken token) => new(_context, _comment, token);
    private async Task<T> Execute<T>(string operation, Func<Task<T>> work)
    {
        var receipt = new StorageReceipt(_context, operation, _comment);
        try { var result = await work().ConfigureAwait(false); receipt.Finish("success"); return result; }
        catch (StorageException error) { receipt.Finish(error.Code.ToString()); throw new StorageException(error.Code); }
        catch (OperationCanceledException) { receipt.Finish("cancelled"); throw new OperationCanceledException(); }
        catch { receipt.Finish("Unavailable"); throw new StorageException(StorageErrorCode.Unavailable); }
    }
    public Task<ObjectMetadata> PutAsync(ObjectKey key, Stream content, ObjectPutOptions options, CancellationToken token = default) => Execute("put", async () =>
    {
        ObjectMetadata? result = null; Exception? failure = null;
        try { result = await _provider.PutAsync(Invocation(token), key, content, options).ConfigureAwait(false); }
        catch (Exception error) { failure = error; }
        try { await content.DisposeAsync().ConfigureAwait(false); }
        catch { failure ??= new StorageException(StorageErrorCode.Io); }
        if (failure is StorageException storage) throw new StorageException(storage.Code);
        if (failure is OperationCanceledException) throw new OperationCanceledException(token);
        if (failure is not null) throw new StorageException(StorageErrorCode.Unavailable);
        return result!;
    });
    public Task<ObjectMetadata> PutBytesAsync(ObjectKey key, byte[] content, ObjectPutOptions options, CancellationToken token = default) => PutAsync(key, new MemoryStream(content, false), options, token);
    public async Task<ObjectContent> GetAsync(ObjectKey key, CancellationToken token = default)
    {
        var receipt = new StorageReceipt(_context, "get", _comment);
        try
        {
            var response = await _provider.GetAsync(Invocation(token), key).ConfigureAwait(false);
            return new ObjectContent(response.Metadata, new ObservedObjectStream(response.Stream, response.Metadata.Size, receipt));
        }
        catch (StorageException error) { receipt.Finish(error.Code.ToString()); throw new StorageException(error.Code); }
        catch (OperationCanceledException) { receipt.Finish("cancelled"); throw new OperationCanceledException(token); }
        catch { receipt.Finish("Unavailable"); throw new StorageException(StorageErrorCode.Unavailable); }
    }
    public async Task<byte[]> GetBytesAsync(ObjectKey key, int maximumBytes, CancellationToken token = default)
    {
        if (maximumBytes < 0) throw new StorageException(StorageErrorCode.InvalidInput);
        await using var response = await GetAsync(key, token).ConfigureAwait(false);
        if (response.Metadata.Size > maximumBytes) throw new StorageException(StorageErrorCode.TooLarge);
        using var output = new MemoryStream(); var buffer = new byte[65536]; int count;
        while ((count = await response.Stream.ReadAsync(buffer, token).ConfigureAwait(false)) != 0)
        {
            if (output.Length + count > maximumBytes) throw new StorageException(StorageErrorCode.TooLarge);
            await output.WriteAsync(buffer.AsMemory(0, count), token).ConfigureAwait(false);
        }
        return output.ToArray();
    }
    public Task<ObjectMetadata?> HeadAsync(ObjectKey key, CancellationToken token = default) => Execute("head", () => _provider.HeadAsync(Invocation(token), key));
    public async Task<bool> ExistsAsync(ObjectKey key, CancellationToken token = default) => await HeadAsync(key, token).ConfigureAwait(false) is not null;
    public Task DeleteAsync(ObjectKey key, CancellationToken token = default) => Execute("delete", async () => { await _provider.DeleteAsync(Invocation(token), key).ConfigureAwait(false); return true; });
    public Task<ObjectConditionalDeleteResult> DeleteIfVersionAsync(ObjectKey key, string contentVersion, CancellationToken token = default) => Execute("delete_if_version", async () =>
    {
        if (string.IsNullOrEmpty(contentVersion) || contentVersion.Length > 256 || contentVersion == "*" ||
            contentVersion.Trim() != contentVersion || contentVersion.Any(char.IsControl))
            throw new StorageException(StorageErrorCode.InvalidInput);
        if (_provider is not IObjectStorageConditionalDeleter conditional)
            throw new StorageException(StorageErrorCode.Unsupported);
        var result = await conditional.DeleteIfVersionAsync(Invocation(token), key, contentVersion).ConfigureAwait(false);
        if (!Enum.IsDefined(result)) throw new StorageException(StorageErrorCode.Unavailable);
        return result;
    });
    public Task<ObjectReadUrl> GetReadUrlAsync(ObjectKey key, ObjectReadUrlOptions? options = null, CancellationToken token = default) => Execute("read_url", () => _provider.GetReadUrlAsync(Invocation(token), key, options ?? new()));
}
internal sealed class ObservedObjectStream(Stream inner, long remaining, StorageReceipt receipt) : Stream
{
    private bool _closed;
    public override bool CanRead => !_closed;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private int Observed(int count, int requested)
    {
        if (requested == 0) return count;
        if (count > remaining || count == 0 && remaining != 0) throw new StorageException(StorageErrorCode.Io);
        remaining -= count; if (count == 0) receipt.Finish("success"); return count;
    }
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_closed) throw new StorageException(StorageErrorCode.Io);
        try { return Observed(inner.Read(buffer, offset, count), count); }
        catch { receipt.Finish("Io"); try { Dispose(); } catch { } throw new StorageException(StorageErrorCode.Io); }
    }
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default)
    {
        if (_closed) throw new StorageException(StorageErrorCode.Io);
        try { return Observed(await inner.ReadAsync(buffer, token).ConfigureAwait(false), buffer.Length); }
        catch (OperationCanceledException) { receipt.Finish("cancelled"); try { await DisposeAsync(); } catch { } throw new OperationCanceledException(token); }
        catch { receipt.Finish("Io"); try { await DisposeAsync(); } catch { } throw new StorageException(StorageErrorCode.Io); }
    }
    protected override void Dispose(bool disposing)
    {
        if (_closed || !disposing) return; _closed = true;
        try { inner.Dispose(); receipt.Finish("cancelled"); }
        catch { receipt.Finish("Io"); throw new StorageException(StorageErrorCode.Io); }
        base.Dispose(disposing);
    }
    public override async ValueTask DisposeAsync()
    {
        if (_closed) return; _closed = true;
        try { await inner.DisposeAsync().ConfigureAwait(false); receipt.Finish("cancelled"); }
        catch { receipt.Finish("Io"); throw new StorageException(StorageErrorCode.Io); }
        GC.SuppressFinalize(this);
    }
    public override void Flush() => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
