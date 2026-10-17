using Amazon.S3;
using Amazon.S3.Model;
using TeaQL.Core.Storage;
using TeaQL.Runtime.Storage;

namespace TeaQL.ObjectStorage.S3;

public sealed class S3ObjectStorage : IObjectStorageProvider, IDisposable
{
    private readonly IAmazonS3 _client;
    private readonly string _bucket;
    public S3ObjectStorage(IAmazonS3 client, string bucket)
    {
        if (client is null || string.IsNullOrWhiteSpace(bucket)) throw new StorageException(StorageErrorCode.InvalidInput);
        _client = client; _bucket = bucket;
    }
    private static StorageException Translate(Exception error)
    {
        var depth = 0;
        for (var cause = error; cause is not null && depth++ < 8; cause = cause.InnerException)
            if (cause is TimeoutException or TaskCanceledException) return new(StorageErrorCode.Timeout);
        if (error is StorageException storage) return new(storage.Code);
        if (error is AmazonS3Exception s3)
        {
            if (s3.ErrorCode == "NoSuchBucket") return new(StorageErrorCode.Unavailable);
            return new((int)s3.StatusCode switch
            {
                401 or 403 => StorageErrorCode.AccessDenied, 404 => StorageErrorCode.NotFound,
                409 or 412 => StorageErrorCode.Conflict, 408 or 504 => StorageErrorCode.Timeout,
                413 => StorageErrorCode.TooLarge, 501 => StorageErrorCode.Unsupported,
                400 or 422 => StorageErrorCode.InvalidInput, _ => StorageErrorCode.Unavailable
            });
        }
        return new(StorageErrorCode.Unavailable);
    }
    public async Task<ObjectMetadata> PutAsync(StorageInvocation invocation, ObjectKey key, Stream content, ObjectPutOptions options)
    {
        try
        {
            var fileOptions = new FileStreamOptions { Mode = FileMode.CreateNew, Access = FileAccess.ReadWrite, Share = FileShare.None, Options = FileOptions.Asynchronous | FileOptions.DeleteOnClose };
            if (!OperatingSystem.IsWindows()) fileOptions.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
            await using var staging = new FileStream(Path.Combine(Path.GetTempPath(), "teaql-object-" + Guid.NewGuid().ToString("N") + ".upload"), fileOptions);
            var buffer = new byte[65536]; long total = 0; int count;
            while ((count = await content.ReadAsync(buffer, invocation.CancellationToken).ConfigureAwait(false)) != 0)
            {
                total += count; if (total > options.Size) throw new StorageException(StorageErrorCode.InvalidInput);
                await staging.WriteAsync(buffer.AsMemory(0, count), invocation.CancellationToken).ConfigureAwait(false);
            }
            if (total != options.Size) throw new StorageException(StorageErrorCode.InvalidInput);
            staging.Position = 0;
            var request = new PutObjectRequest { BucketName = _bucket, Key = key.Value, InputStream = staging, ContentType = options.ContentType, AutoCloseStream = false, AutoResetStreamPosition = true, UseChunkEncoding = false };
            request.Headers.ContentLength = total;
            if (options.Mode == ObjectWriteMode.CreateOnly) request.IfNoneMatch = "*";
            foreach (var pair in options.Metadata) request.Metadata.Add(pair.Key, pair.Value);
            var response = await _client.PutObjectAsync(request, invocation.CancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(response.ETag)) throw new StorageException(StorageErrorCode.Unavailable);
            return new(key, total, options.ContentType, response.ETag, metadata: options.Metadata);
        }
        catch (OperationCanceledException) when (invocation.CancellationToken.IsCancellationRequested) { throw new OperationCanceledException(invocation.CancellationToken); }
        catch (IOException) { throw new StorageException(StorageErrorCode.Io); }
        catch (Exception error) { throw Translate(error); }
    }
    public async Task<ObjectContent> GetAsync(StorageInvocation invocation, ObjectKey key)
    {
        try
        {
            var response = await _client.GetObjectAsync(new GetObjectRequest { BucketName = _bucket, Key = key.Value }, invocation.CancellationToken).ConfigureAwait(false);
            try
            {
                var metadata = Metadata(key, response.ContentLength, response.Headers.ContentType, response.ETag, response.LastModified, response.Metadata);
                return new(metadata, response.ResponseStream);
            }
            catch { response.Dispose(); throw; }
        }
        catch (OperationCanceledException) when (invocation.CancellationToken.IsCancellationRequested) { throw new OperationCanceledException(invocation.CancellationToken); }
        catch (Exception error) { throw Translate(error); }
    }
    private static ObjectMetadata Metadata(ObjectKey key, long length, string? contentType, string? etag, DateTime? modified, MetadataCollection values)
    {
        if (string.IsNullOrWhiteSpace(etag)) throw new StorageException(StorageErrorCode.Unavailable);
        var metadata = values.Keys.ToDictionary(k => k.StartsWith("x-amz-meta-", StringComparison.OrdinalIgnoreCase) ? k[11..] : k, k => values[k], StringComparer.Ordinal);
        return new(key, length, contentType, etag, modified is null ? null : new DateTimeOffset(DateTime.SpecifyKind(modified.Value, DateTimeKind.Utc)), metadata);
    }
    public async Task<ObjectMetadata?> HeadAsync(StorageInvocation invocation, ObjectKey key)
    {
        try
        {
            var response = await _client.GetObjectMetadataAsync(new GetObjectMetadataRequest { BucketName = _bucket, Key = key.Value }, invocation.CancellationToken).ConfigureAwait(false);
            return Metadata(key, response.ContentLength, response.Headers.ContentType, response.ETag, response.LastModified, response.Metadata);
        }
        catch (OperationCanceledException) when (invocation.CancellationToken.IsCancellationRequested) { throw new OperationCanceledException(invocation.CancellationToken); }
        catch (Exception error) { var safe = Translate(error); if (safe.Code != StorageErrorCode.NotFound) throw safe; }
        try { await _client.HeadBucketAsync(new HeadBucketRequest { BucketName = _bucket }, invocation.CancellationToken).ConfigureAwait(false); return null; }
        catch (OperationCanceledException) when (invocation.CancellationToken.IsCancellationRequested) { throw new OperationCanceledException(invocation.CancellationToken); }
        catch (Exception error) { var safe = Translate(error); throw safe.Code == StorageErrorCode.NotFound ? new StorageException(StorageErrorCode.Unavailable) : safe; }
    }
    public async Task DeleteAsync(StorageInvocation invocation, ObjectKey key)
    {
        try { await _client.DeleteObjectAsync(new DeleteObjectRequest { BucketName = _bucket, Key = key.Value }, invocation.CancellationToken).ConfigureAwait(false); }
        catch (OperationCanceledException) when (invocation.CancellationToken.IsCancellationRequested) { throw new OperationCanceledException(invocation.CancellationToken); }
        catch (Exception error) { throw Translate(error); }
    }
    public async Task<ObjectReadUrl> GetReadUrlAsync(StorageInvocation invocation, ObjectKey key, ObjectReadUrlOptions options)
    {
        invocation.CancellationToken.ThrowIfCancellationRequested();
        var expires = DateTimeOffset.UtcNow + options.Lifetime;
        try
        {
            var request = new GetPreSignedUrlRequest { BucketName = _bucket, Key = key.Value, Verb = HttpVerb.GET, Expires = expires.UtcDateTime };
            if (Uri.TryCreate(_client.Config.ServiceURL, UriKind.Absolute, out var endpoint) && endpoint.Scheme == Uri.UriSchemeHttp)
                request.Protocol = Protocol.HTTP;
            if (options.ContentDisposition is not null) request.ResponseHeaderOverrides.ContentDisposition = options.ContentDisposition;
            var url = await _client.GetPreSignedURLAsync(request).ConfigureAwait(false);
            return new(new Uri(url), expires);
        }
        catch (Exception error) { throw Translate(error); }
    }
    public void Dispose() => _client.Dispose();
    public override string ToString() => "S3ObjectStorage[configuration=masked]";
}
