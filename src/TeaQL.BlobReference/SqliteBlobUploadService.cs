using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using TeaQL.Core;
using TeaQL.Core.Storage;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.Sql;

namespace TeaQL.BlobReference;

public sealed record BlobUploadTarget(string Entity, string EntityId, string Field);
public sealed record BlobUploadInput(string FileName, long DeclaredSize, string? DeclaredContentType = null);
public sealed record BlobVerifiedContent(long SizeBytes, string Sha256, string ContentType);
public sealed record BlobUploadSession(string UploadId, BlobRef Reference, DateTimeOffset ExpiresAt)
{
    public override string ToString() => "BlobUploadSession[masked]";
}

/// <summary>Resolve the current model field, tenant scope and action permission.</summary>
public interface IBlobUploadAuthorization
{
    string Scope(UserContext actor);
    BlobFieldSpec Field(UserContext actor, BlobUploadTarget target);
    Task<bool> AuthorizeAsync(UserContext actor, BlobUploadTarget target, string action,
        BlobUploadInput input);
}

/// <summary>Host-supplied trusted parser; it must consume every byte and report actual format.</summary>
public interface IBlobContentVerifier
{
    Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension, long maximumBytes);
}

/// <summary>Explicit SQLite upload journal. Host migration and parser installation are separate.</summary>
public sealed class SqliteBlobUploadService
{
    private readonly SqliteBlobCatalog _catalog;
    internal SqliteBlobCatalog Catalog => _catalog;
    private readonly ISqlTransactionTransport _transport;
    private readonly IBlobUploadAuthorization _authorization;
    private readonly IBlobContentVerifier _verifier;
    private readonly long _maximumBytes;
    private readonly TimeSpan _lease;
    private readonly Func<DateTimeOffset> _now;

    private sealed record UploadRow(string Id, string Scope, string Key, string Entity,
        string EntityId, string Field, string FileName, string Extension, long DeclaredSize,
        long ExpiresMs, string State, string? WrittenSha256, string? WrittenETag);

    private static InvalidOperationException Error(string code) => new(code);
    private static CompiledQuery Query(string sql, params Value[] values) => new(sql, values.ToList());
    private static Value.TextValue Text(string value) => new(value);
    private static Value.I64Value Number(long value) => new(value);
    private static string Token() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(24))
        .TrimEnd('=').Replace('+', '-').Replace('/', '_');
    private static bool ValidText(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value) || value != value.Trim() || value.Any(char.IsControl))
            return false;
        try { return new UTF8Encoding(false, true).GetByteCount(value) <= max; }
        catch (EncoderFallbackException) { return false; }
    }
    private static bool EntityId(string? value) => value == "" ||
        ulong.TryParse(value, out var id) && id > 0 && id.ToString() == value;

    public SqliteBlobUploadService(SqliteBlobCatalog catalog, IBlobUploadAuthorization authorization,
        IBlobContentVerifier verifier, long maximumBytes, TimeSpan? lease = null,
        Func<DateTimeOffset>? now = null)
    {
        _catalog = catalog ?? throw Error("BLOB_POLICY_INVALID");
        _transport = catalog.Transport as ISqlTransactionTransport ?? throw Error("BLOB_TRANSACTION_REQUIRED");
        _authorization = authorization ?? throw Error("BLOB_POLICY_INVALID");
        _verifier = verifier ?? throw Error("BLOB_POLICY_INVALID");
        _maximumBytes = maximumBytes;
        _lease = lease ?? TimeSpan.FromMinutes(15);
        _now = now ?? (() => DateTimeOffset.UtcNow);
        if (maximumBytes is < 1 or > ObjectPutOptions.MaximumSize ||
            _lease < TimeSpan.FromSeconds(1) || _lease > TimeSpan.FromHours(1) ||
            _lease.Ticks % TimeSpan.TicksPerSecond != 0)
            throw Error("BLOB_POLICY_INVALID");
    }

    private BlobUploadInput Normalize(BlobUploadInput input)
    {
        if (input == null || input.DeclaredSize < 0 || input.DeclaredSize > _maximumBytes)
            throw Error("BLOB_SIZE_EXCEEDED");
        if (!ValidText(input.FileName, 1024)) throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var fileName = input.FileName.Replace('\\', '/').Split('/').Last();
        if (!ValidText(fileName, 512) || BlobFieldSpec.MimeForExtension(
                fileName.Split('.').Last().ToLowerInvariant()) == null)
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var declared = input.DeclaredContentType?.Trim().ToLowerInvariant();
        if (declared != null && declared.Length > 0 && !ValidText(declared, 128))
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        return new BlobUploadInput(fileName, input.DeclaredSize, declared);
    }

    private async Task<string> Current(UserContext actor, BlobUploadTarget target, string action,
        BlobUploadInput input, string? expectedScope = null)
    {
        if (actor == null || target == null || !ValidText(target.Entity, 256) ||
            !ValidText(target.Field, 256) || target.Entity.EnumerateRunes().Count() > 128 ||
            target.Field.EnumerateRunes().Count() > 128 || !EntityId(target.EntityId))
            throw Error("BLOB_REFERENCE_INVALID");
        string scope;
        BlobFieldSpec field;
        bool allowed;
        try
        {
            scope = _authorization.Scope(actor);
            field = _authorization.Field(actor, target);
            allowed = await _authorization.AuthorizeAsync(actor, target, action, input);
        }
        catch { throw Error("BLOB_UPLOAD_DENIED"); }
        if (!ValidText(scope, 256) || expectedScope != null && scope != expectedScope ||
            field == null || field.Entity != target.Entity || field.Field != target.Field || !allowed)
            throw Error("BLOB_UPLOAD_DENIED");
        var extension = input.FileName.Split('.').Last().ToLowerInvariant();
        if (!field.AllowedExtensions.Contains(extension, StringComparer.Ordinal))
            throw Error("BLOB_FORMAT_NOT_ALLOWED");
        return scope;
    }

    private async Task<T> Transaction<T>(Func<ISqlTransaction, Task<T>> work)
    {
        ISqlTransaction tx;
        try { tx = await _transport.BeginSqlAsync(); }
        catch { throw Error("BLOB_CATALOG_FAILED"); }
        using (tx)
        {
            try
            {
                var result = await work(tx);
                try { await tx.CommitSqlAsync(); }
                catch { throw Error("BLOB_CATALOG_FAILED"); }
                return result;
            }
            catch
            {
                try { await tx.RollbackSqlAsync(); } catch { }
                throw;
            }
        }
    }

    public async Task<BlobUploadSession> BeginAsync(UserContext actor, BlobUploadTarget target,
        BlobUploadInput input, string replayKey)
    {
        var normalized = Normalize(input);
        if (!ValidText(replayKey, 256)) throw Error("BLOB_REFERENCE_INVALID");
        var scope = await Current(actor, target, "Begin", normalized);
        var extension = normalized.FileName.Split('.').Last().ToLowerInvariant();
        var request = JsonSerializer.Serialize(new object?[] { target.Entity, target.EntityId,
            target.Field, normalized.FileName, normalized.DeclaredSize, normalized.DeclaredContentType });
        var requestHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request))).ToLowerInvariant();
        return await Transaction(async tx =>
        {
            List<Record> rows;
            try { rows = await tx.FetchAllSqlAsync(Query(
                "SELECT upload_id,key,request_hash,expires_ms FROM teaql_blob_upload WHERE scope=@p0 AND replay_key=@p1",
                Text(scope), Text(replayKey))); }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
            if (rows.Count > 1) throw Error("BLOB_CATALOG_FAILED");
            if (rows.Count == 1)
            {
                var old = rows[0];
                if (old.GetValueOrDefault("request_hash")?.TryText() != requestHash)
                    throw Error("BLOB_IDEMPOTENCY_CONFLICT");
                await Current(actor, target, "Begin", normalized, scope);
                try { return new BlobUploadSession(old["upload_id"].TryText()!,
                    new BlobRef(old["key"].TryText()!),
                    DateTimeOffset.FromUnixTimeMilliseconds(old["expires_ms"].TryI64()!.Value)); }
                catch { throw Error("BLOB_CATALOG_FAILED"); }
            }
            var reference = new BlobRef("blob_" + Token());
            var uploadId = "bu_" + Token();
            var objectKey = "blob/" + Token();
            var expiresMs = _now().Add(_lease).ToUnixTimeMilliseconds();
            await Current(actor, target, "Begin", normalized, scope);
            try
            {
                await tx.ExecuteSqlAsync(Query("""
                    INSERT INTO teaql_blob_catalog
                    (key,scope,object_key,content_version,file_name,content_type,size_bytes,status)
                    VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7)
                    """, Text(reference.Key), Text(scope), Text(objectKey), Text(""),
                    Text(normalized.FileName), Text(""), Number(0), Text("initiated")));
                await tx.ExecuteSqlAsync(Query("""
                    INSERT INTO teaql_blob_upload
                    (upload_id,scope,replay_key,request_hash,key,entity,entity_id,field,file_name,
                     extension,declared_size,expires_ms,state)
                    VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12)
                    """, Text(uploadId), Text(scope), Text(replayKey), Text(requestHash),
                    Text(reference.Key), Text(target.Entity), Text(target.EntityId), Text(target.Field),
                    Text(normalized.FileName), Text(extension), Number(normalized.DeclaredSize),
                    Number(expiresMs), Text("initiated")));
            }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
            return new BlobUploadSession(uploadId, reference,
                DateTimeOffset.FromUnixTimeMilliseconds(expiresMs));
        });
    }

    private static string Required(Record row, string column)
    {
        var value = row.GetValueOrDefault(column)?.TryText();
        return value ?? throw Error("BLOB_CATALOG_FAILED");
    }

    private async Task<UploadRow> Row(UserContext actor, string uploadId, string action)
    {
        if (actor == null || !Regex.IsMatch(uploadId ?? "", @"\Abu_[A-Za-z0-9_-]{32}\z"))
            throw Error("BLOB_UPLOAD_DENIED");
        string scope;
        try { scope = _authorization.Scope(actor); }
        catch { throw Error("BLOB_UPLOAD_DENIED"); }
        if (!ValidText(scope, 256)) throw Error("BLOB_UPLOAD_DENIED");
        List<Record> rows;
        try { rows = await _catalog.Transport.FetchAllSqlAsync(Query("""
            SELECT upload_id,scope,key,entity,entity_id,field,file_name,extension,
              declared_size,expires_ms,state,written_sha256,written_etag
            FROM teaql_blob_upload WHERE scope=@p0 AND upload_id=@p1
            """, Text(scope), Text(uploadId!))); }
        catch { throw Error("BLOB_CATALOG_FAILED"); }
        if (rows.Count != 1) throw Error("BLOB_UPLOAD_DENIED");
        var r = rows[0];
        var size = r.GetValueOrDefault("declared_size")?.TryI64();
        var expiry = r.GetValueOrDefault("expires_ms")?.TryI64();
        if (size == null || expiry == null) throw Error("BLOB_CATALOG_FAILED");
        var row = new UploadRow(Required(r, "upload_id"), Required(r, "scope"), Required(r, "key"),
            Required(r, "entity"), Required(r, "entity_id"), Required(r, "field"),
            Required(r, "file_name"), Required(r, "extension"), size.Value, expiry.Value,
            Required(r, "state"), r.GetValueOrDefault("written_sha256")?.TryText(),
            r.GetValueOrDefault("written_etag")?.TryText());
        await Current(actor, new BlobUploadTarget(row.Entity, row.EntityId, row.Field), action,
            new BlobUploadInput(row.FileName, row.DeclaredSize), scope);
        if (_now().ToUnixTimeMilliseconds() >= row.ExpiresMs) throw Error("BLOB_UPLOAD_EXPIRED");
        if (row.DeclaredSize < 0 || row.DeclaredSize > _maximumBytes) throw Error("BLOB_SIZE_EXCEEDED");
        if (row.Extension != row.FileName.Split('.').Last().ToLowerInvariant())
            throw Error("BLOB_CATALOG_FAILED");
        return row;
    }

    private async Task<ObjectKey> ObjectKey(UploadRow row)
    {
        try
        {
            var rows = await _catalog.Transport.FetchAllSqlAsync(Query(
                "SELECT object_key FROM teaql_blob_catalog WHERE key=@p0 AND scope=@p1",
                Text(row.Key), Text(row.Scope)));
            if (rows.Count == 1) return new ObjectKey(Required(rows[0], "object_key"));
        }
        catch { /* Stable catalog failure below. */ }
        throw Error("BLOB_CATALOG_FAILED");
    }

    public async Task<long> WriteAsync(UserContext actor, string uploadId, Stream source)
    {
        if (source == null) throw Error("BLOB_UPLOAD_INCOMPLETE");
        await using var sourceLifetime = source;
        var row = await Row(actor, uploadId, "Write");
        if (row.State != "initiated") throw Error("BLOB_IDEMPOTENCY_CONFLICT");
        var key = await ObjectKey(row);
        var mime = BlobFieldSpec.MimeForExtension(row.Extension) ?? throw Error("BLOB_FORMAT_NOT_ALLOWED");
        var reader = new HashingReadStream(source, row.DeclaredSize);
        ObjectMetadata metadata;
        try { metadata = await actor.ObjectStorage("write Blob upload").PutAsync(key, reader,
            new ObjectPutOptions(row.DeclaredSize, ObjectWriteMode.CreateOnly, mime)); }
        catch { reader.Release(); throw Error("BLOB_STORE_FAILED"); }
        try
        {
            await Current(actor, new BlobUploadTarget(row.Entity, row.EntityId, row.Field), "Write",
                new BlobUploadInput(row.FileName, row.DeclaredSize), row.Scope);
            if (_now().ToUnixTimeMilliseconds() >= row.ExpiresMs) throw Error("BLOB_UPLOAD_EXPIRED");
            if (!reader.Eof || reader.Count != row.DeclaredSize || metadata.Size != reader.Count ||
                string.IsNullOrEmpty(metadata.ETag)) throw Error("BLOB_UPLOAD_INCOMPLETE");
            var digest = reader.Digest();
            return await Transaction(async tx =>
            {
                ulong affected;
                try { affected = await tx.ExecuteSqlAsync(Query("""
                    UPDATE teaql_blob_upload SET state='written',written_sha256=@p0,written_etag=@p1
                    WHERE upload_id=@p2 AND scope=@p3 AND state='initiated'
                    """, Text(digest), Text(metadata.ETag), Text(uploadId), Text(row.Scope))); }
                catch { throw Error("BLOB_CATALOG_FAILED"); }
                if (affected != 1) throw Error("BLOB_IDEMPOTENCY_CONFLICT");
                return reader.Count;
            });
        }
        finally { reader.Release(); }
    }

    public async Task<BlobRef> CompleteAsync(UserContext actor, string uploadId)
    {
        var row = await Row(actor, uploadId, "Confirm");
        if (row.State == "completed")
        {
            if (row.WrittenSha256 == null || !Regex.IsMatch(row.WrittenSha256, "\\A[0-9a-f]{64}\\z") ||
                string.IsNullOrEmpty(row.WrittenETag)) throw Error("BLOB_UPLOAD_INCOMPLETE");
            List<Record> records;
            try { records = await _catalog.Transport.FetchAllSqlAsync(Query("""
                SELECT object_key,content_version,content_type,size_bytes,status
                FROM teaql_blob_catalog WHERE key=@p0 AND scope=@p1
                """, Text(row.Key), Text(row.Scope))); }
            catch { throw Error("BLOB_UPLOAD_INCOMPLETE"); }
            if (records.Count != 1) throw Error("BLOB_UPLOAD_INCOMPLETE");
            var catalog = records[0];
            var location = catalog.GetValueOrDefault("object_key")?.TryText();
            var version = catalog.GetValueOrDefault("content_version")?.TryText();
            var contentType = catalog.GetValueOrDefault("content_type")?.TryText();
            var size = catalog.GetValueOrDefault("size_bytes")?.TryI64();
            var status = catalog.GetValueOrDefault("status")?.TryText();
            var mime = BlobFieldSpec.MimeForExtension(row.Extension);
            if (location == null || version != row.WrittenETag || contentType != mime ||
                size != row.DeclaredSize || status is not ("uploaded_unbound" or "available"))
                throw Error("BLOB_UPLOAD_INCOMPLETE");
            ObjectKey replayKey;
            try { replayKey = new ObjectKey(location); }
            catch { throw Error("BLOB_UPLOAD_INCOMPLETE"); }
            ObjectStorage replayStore;
            try { replayStore = actor.ObjectStorage("recheck completed Blob upload"); }
            catch { throw Error("BLOB_STORE_FAILED"); }
            ObjectMetadata? metadata;
            try { metadata = await replayStore.HeadAsync(replayKey); }
            catch { throw Error("BLOB_UPLOAD_INCOMPLETE"); }
            if (metadata == null || metadata.Size != row.DeclaredSize || metadata.ETag != version)
                throw Error("BLOB_UPLOAD_INCOMPLETE");
            await Current(actor, new BlobUploadTarget(row.Entity, row.EntityId, row.Field), "Confirm",
                new BlobUploadInput(row.FileName, row.DeclaredSize, mime), row.Scope);
            if (_now().ToUnixTimeMilliseconds() >= row.ExpiresMs) throw Error("BLOB_UPLOAD_EXPIRED");
            return new BlobRef(row.Key);
        }
        if (row.State != "written" || string.IsNullOrEmpty(row.WrittenSha256) ||
            string.IsNullOrEmpty(row.WrittenETag)) throw Error("BLOB_UPLOAD_INCOMPLETE");
        var key = await ObjectKey(row);
        ObjectStorage store;
        try { store = actor.ObjectStorage("verify Blob upload"); }
        catch { throw Error("BLOB_STORE_FAILED"); }
        ObjectMetadata? before;
        try { before = await store.HeadAsync(key); }
        catch { throw Error("BLOB_UPLOAD_INCOMPLETE"); }
        if (before == null || before.Size != row.DeclaredSize || before.ETag != row.WrittenETag)
            throw Error("BLOB_UPLOAD_INCOMPLETE");
        BlobVerifiedContent verified;
        try
        {
            await using var content = await store.GetAsync(key);
            if (content.Metadata.Size != before.Size || content.Metadata.ETag != before.ETag)
                throw Error("BLOB_UPLOAD_INCOMPLETE");
            var reader = new HashingReadStream(content.Stream, _maximumBytes, leaveOpen: true);
            try
            {
                verified = await _verifier.VerifyAsync(reader, row.Extension, _maximumBytes);
                var digest = reader.Digest();
                if (!reader.Eof || reader.Count != before.Size || verified == null ||
                    verified.SizeBytes != reader.Count || verified.Sha256 != digest ||
                    verified.Sha256 != row.WrittenSha256 ||
                    verified.ContentType != BlobFieldSpec.MimeForExtension(row.Extension))
                    throw Error("BLOB_UPLOAD_INCOMPLETE");
            }
            finally { reader.Release(); }
        }
        catch { throw Error("BLOB_UPLOAD_INCOMPLETE"); }
        ObjectMetadata? after;
        try { after = await store.HeadAsync(key); }
        catch { throw Error("BLOB_UPLOAD_INCOMPLETE"); }
        if (after == null || after.Size != before.Size || after.ETag != before.ETag)
            throw Error("BLOB_UPLOAD_INCOMPLETE");
        await Current(actor, new BlobUploadTarget(row.Entity, row.EntityId, row.Field), "Confirm",
            new BlobUploadInput(row.FileName, row.DeclaredSize, verified.ContentType), row.Scope);
        if (_now().ToUnixTimeMilliseconds() >= row.ExpiresMs) throw Error("BLOB_UPLOAD_EXPIRED");
        await Transaction(async tx =>
        {
            ulong affected;
            try { affected = await tx.ExecuteSqlAsync(Query("""
                UPDATE teaql_blob_upload SET state='completed' WHERE upload_id=@p0 AND scope=@p1
                  AND state='written' AND written_sha256=@p2 AND written_etag=@p3
                """, Text(uploadId), Text(row.Scope), Text(row.WrittenSha256!), Text(row.WrittenETag!))); }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
            if (affected != 1) throw Error("BLOB_IDEMPOTENCY_CONFLICT");
            try { affected = await tx.ExecuteSqlAsync(Query("""
                UPDATE teaql_blob_catalog SET content_version=@p0,content_type=@p1,size_bytes=@p2,
                  status='uploaded_unbound' WHERE key=@p3 AND scope=@p4 AND status='initiated'
                """, Text(row.WrittenETag!), Text(verified.ContentType), Number(verified.SizeBytes),
                Text(row.Key), Text(row.Scope))); }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
            if (affected != 1) throw Error("BLOB_IDEMPOTENCY_CONFLICT");
            return true;
        });
        return new BlobRef(row.Key);
    }

    private sealed class HashingReadStream(Stream inner, long maximum, bool leaveOpen = false) : Stream
    {
        private readonly IncrementalHash _hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        private bool _released;
        public long Count { get; private set; }
        public bool Eof { get; private set; }
        public override bool CanRead => inner.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        private int Observe(ReadOnlySpan<byte> block, int requested)
        {
            Count += block.Length;
            if (Count > maximum) throw Error("BLOB_SIZE_EXCEEDED");
            if (block.Length > 0) _hash.AppendData(block);
            else if (requested > 0) Eof = true;
            return block.Length;
        }
        public string Digest() => Convert.ToHexString(_hash.GetHashAndReset()).ToLowerInvariant();
        public void Release() { if (_released) return; _released = true; _hash.Dispose(); }
        public override int Read(byte[] buffer, int offset, int count) =>
            Observe(buffer.AsSpan(offset, inner.Read(buffer, offset, count)), count);
        public override int Read(Span<byte> buffer) => Observe(buffer[..inner.Read(buffer)], buffer.Length);
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default)
        {
            var count = await inner.ReadAsync(buffer, token);
            return Observe(buffer.Span[..count], buffer.Length);
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token) =>
            ReadAsync(buffer.AsMemory(offset, count), token).AsTask();
        protected override void Dispose(bool disposing)
        {
            if (disposing && !leaveOpen) inner.Dispose();
            base.Dispose(disposing);
        }
        public override async ValueTask DisposeAsync()
        {
            if (!leaveOpen) await inner.DisposeAsync();
            GC.SuppressFinalize(this);
        }
        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
