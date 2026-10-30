using System.Text;
using TeaQL.Core;
using TeaQL.Core.Storage;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.Sql;

namespace TeaQL.BlobReference;

public interface IBlobMaintenanceAuthorization
{
    string Scope(UserContext actor);
    Task<bool> CanMaintainAsync(UserContext actor, string scope);
}

public sealed record BlobMaintenanceCursor(string Scope, string After)
{
    public override string ToString() => "BlobMaintenanceCursor[masked]";
}
public sealed record BlobMaintenanceOutcome(BlobRef Reference, string Status, string? Error = null);
public sealed record BlobMaintenanceBatch(IReadOnlyList<BlobMaintenanceOutcome> Outcomes, BlobMaintenanceCursor? Next);

/// <summary>Explicit, host-scheduled fenced cleanup of expired or retained server-stream objects.</summary>
public sealed class SqliteBlobMaintenanceService
{
    private readonly SqliteBlobCatalog _catalog;
    internal SqliteBlobCatalog Catalog => _catalog;
    private readonly ISqlTransactionTransport _transport;
    private readonly IBlobMaintenanceAuthorization _authorization;
    private readonly int _batchSize;
    private readonly Func<DateTimeOffset> _now;

    private sealed record Row(string Key, string ObjectKey, string Version,
        string WrittenETag, string Status, string UploadState, long ExpiresMs,
        long? CompletedAtMs, long? LastUnboundAtMs, string? PolicyProfile,
        string? UploadMode, long? UnboundRetentionSeconds,
        long? EffectiveUnrefSeconds, long? RetentionFloorUntilMs);

    private static InvalidOperationException Error(string code) => new(code);
    private static CompiledQuery Query(string sql, params Value[] values) => new(sql, values.ToList());
    private static Value.TextValue Text(string value) => new(value);
    private static Value.I64Value Number(long value) => new(value);
    private static bool ValidScope(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value != value.Trim() || value.Any(char.IsControl))
            return false;
        try { return new UTF8Encoding(false, true).GetByteCount(value) <= 256; }
        catch (EncoderFallbackException) { return false; }
    }
    private static bool ValidVersion(string? value) => !string.IsNullOrEmpty(value) &&
        value != "*" && value.Length <= 256 && value == value.Trim() && !value.Any(char.IsControl);

    public SqliteBlobMaintenanceService(SqliteBlobCatalog catalog,
        IBlobMaintenanceAuthorization authorization, int batchSize = 100,
        Func<DateTimeOffset>? now = null)
    {
        _catalog = catalog ?? throw Error("BLOB_POLICY_INVALID");
        _transport = catalog.Transport as ISqlTransactionTransport ?? throw Error("BLOB_CAPABILITY_MISSING");
        _authorization = authorization ?? throw Error("BLOB_POLICY_INVALID");
        if (batchSize is < 1 or > 1000) throw Error("BLOB_POLICY_INVALID");
        _batchSize = batchSize;
        _now = now ?? (() => DateTimeOffset.UtcNow);
    }

    private async Task<string> AuthorizeAsync(UserContext? actor, string? expected = null)
    {
        if (actor == null) throw Error("BLOB_MAINTENANCE_DENIED");
        string scope;
        bool allowed;
        try
        {
            scope = _authorization.Scope(actor);
            allowed = await _authorization.CanMaintainAsync(actor, scope).ConfigureAwait(false);
        }
        catch { throw Error("BLOB_MAINTENANCE_DENIED"); }
        if (!ValidScope(scope) || expected is not null && scope != expected || !allowed)
            throw Error("BLOB_MAINTENANCE_DENIED");
        return scope;
    }

    private long NowMs()
    {
        var value = _now().ToUnixTimeMilliseconds();
        if (!BlobRetentionPolicy.ValidTime(value)) throw Error("BLOB_POLICY_INVALID");
        return value;
    }

    private async Task<T> Transaction<T>(Func<ISqlTransaction, Task<T>> work)
    {
        ISqlTransaction tx;
        try { tx = await _transport.BeginSqlAsync().ConfigureAwait(false); }
        catch { throw Error("BLOB_CATALOG_FAILED"); }
        using (tx)
        {
            try
            {
                var result = await work(tx).ConfigureAwait(false);
                try { await tx.CommitSqlAsync().ConfigureAwait(false); }
                catch { throw Error("BLOB_CATALOG_FAILED"); }
                return result;
            }
            catch
            {
                try { await tx.RollbackSqlAsync().ConfigureAwait(false); } catch { }
                throw;
            }
        }
    }

    private static async Task<Row?> ReadRowAsync(ISqlTransport sql, string scope, string key)
    {
        List<Record> rows;
        try
        {
            rows = await sql.FetchAllSqlAsync(Query("""
                SELECT c.object_key,c.content_version,c.status,c.completed_at_ms,
                  c.last_unbound_at_ms,c.policy_profile,c.upload_mode,
                  c.unbound_retention_seconds,c.effective_unref_seconds,
                  c.retention_floor_until_ms,u.state,u.expires_ms,u.written_etag
                FROM teaql_blob_catalog c JOIN teaql_blob_upload u
                  ON u.key=c.key AND u.scope=c.scope WHERE c.scope=@p0 AND c.key=@p1
                """, Text(scope), Text(key))).ConfigureAwait(false);
        }
        catch { throw Error("BLOB_CATALOG_FAILED"); }
        if (rows.Count == 0) return null;
        if (rows.Count != 1) throw Error("BLOB_CATALOG_FAILED");
        var row = rows[0];
        var objectKey = row.GetValueOrDefault("object_key")?.TryText();
        var version = row.GetValueOrDefault("content_version")?.TryText();
        var status = row.GetValueOrDefault("status")?.TryText();
        var state = row.GetValueOrDefault("state")?.TryText();
        var expires = row.GetValueOrDefault("expires_ms")?.TryI64();
        var writtenValue = row.GetValueOrDefault("written_etag");
        var written = writtenValue is null or Value.NullValue ? "" : writtenValue.TryText();
        if (string.IsNullOrEmpty(objectKey) || version == null || version.Length > 0 && !ValidVersion(version) ||
            status == null || state == null || expires == null || written == null ||
            written.Length > 0 && !ValidVersion(written) || state == "written" && written.Length == 0)
            throw Error("BLOB_CATALOG_FAILED");
        return new Row(key, objectKey, version, written, status, state, expires.Value,
            row.GetValueOrDefault("completed_at_ms")?.TryI64(),
            row.GetValueOrDefault("last_unbound_at_ms")?.TryI64(),
            row.GetValueOrDefault("policy_profile")?.TryText(),
            row.GetValueOrDefault("upload_mode")?.TryText(),
            row.GetValueOrDefault("unbound_retention_seconds")?.TryI64(),
            row.GetValueOrDefault("effective_unref_seconds")?.TryI64(),
            row.GetValueOrDefault("retention_floor_until_ms")?.TryI64());
    }

    private static bool Eligible(Row? row, long now)
    {
        if (row == null) return false;
        if (row.UploadState is "initiated" or "written")
            return (row.UploadMode is null or "server_stream") &&
                row.ExpiresMs <= now && (row.Status is "initiated" or "deleting");
        if (row.UploadState != "completed" ||
            row.Status is not ("uploaded_unbound" or "available" or "deleting") ||
            !ValidScope(row.PolicyProfile) || row.UploadMode != "server_stream" ||
            row.CompletedAtMs is null || !BlobRetentionPolicy.ValidTime(row.CompletedAtMs.Value) ||
            row.UnboundRetentionSeconds is null ||
            !BlobRetentionPolicy.ValidSeconds(row.UnboundRetentionSeconds.Value) ||
            row.EffectiveUnrefSeconds is null ||
            !BlobRetentionPolicy.ValidSeconds(row.EffectiveUnrefSeconds.Value) ||
            !ValidVersion(row.Version)) return false;
        try
        {
            long deadline;
            if (row.LastUnboundAtMs is null)
            {
                if (row.Status == "available") return false;
                deadline = BlobRetentionPolicy.Deadline(row.CompletedAtMs.Value,
                    row.UnboundRetentionSeconds.Value);
            }
            else
            {
                if (!BlobRetentionPolicy.ValidTime(row.LastUnboundAtMs.Value) ||
                    row.RetentionFloorUntilMs is not null &&
                    !BlobRetentionPolicy.ValidTime(row.RetentionFloorUntilMs.Value)) return false;
                deadline = BlobRetentionPolicy.UnreferencedEligibleAt(
                    row.LastUnboundAtMs.Value, row.EffectiveUnrefSeconds.Value,
                    row.RetentionFloorUntilMs);
            }
            return now >= deadline;
        }
        catch (InvalidOperationException) { return false; }
    }

    private static async Task<long> CountAsync(ISqlTransport sql, string statement, string value)
    {
        List<Record> rows;
        try { rows = await sql.FetchAllSqlAsync(Query(statement, Text(value))).ConfigureAwait(false); }
        catch { throw Error("BLOB_CATALOG_FAILED"); }
        var count = rows.Count == 1 ? rows[0].GetValueOrDefault("n")?.TryI64() : null;
        if (count == null || count < 0) throw Error("BLOB_CATALOG_FAILED");
        return count.Value;
    }

    private Task<Row?> ClaimAsync(string scope, string key, long now) => Transaction(async tx =>
    {
        var row = await ReadRowAsync(tx, scope, key).ConfigureAwait(false);
        if (!Eligible(row, now)) return null;
        if (await CountAsync(tx, "SELECT count(*) AS n FROM teaql_blob_binding WHERE key=@p0", key).ConfigureAwait(false) != 0)
            return null;
        if (await CountAsync(tx, "SELECT count(*) AS n FROM teaql_blob_catalog WHERE object_key=@p0", row!.ObjectKey).ConfigureAwait(false) != 1)
            throw Error("BLOB_CATALOG_FAILED");
        if (row.Status != "deleting")
        {
            ulong affected;
            try { affected = await tx.ExecuteSqlAsync(Query(
                "UPDATE teaql_blob_catalog SET status='deleting' WHERE key=@p0 AND scope=@p1 AND status=@p2",
                Text(key), Text(scope), Text(row.Status))).ConfigureAwait(false); }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
            if (affected != 1) throw Error("BLOB_CATALOG_FAILED");
        }
        return row;
    });

    private Task<bool> CaptureAsync(string scope, Row claim, string version, long now)
    {
        if (!ValidVersion(version)) throw Error("BLOB_IDEMPOTENCY_CONFLICT");
        return Transaction(async tx =>
        {
            var row = await ReadRowAsync(tx, scope, claim.Key).ConfigureAwait(false);
            var bound = await CountAsync(tx, "SELECT count(*) AS n FROM teaql_blob_binding WHERE key=@p0", claim.Key).ConfigureAwait(false);
            if (!Eligible(row, now) || row!.Status != "deleting" || row.ObjectKey != claim.ObjectKey || bound != 0 ||
                row.WrittenETag.Length > 0 && row.WrittenETag != version ||
                row.Version.Length > 0 && row.Version != version)
                throw Error("BLOB_IDEMPOTENCY_CONFLICT");
            if (row.Version.Length == 0)
            {
                ulong affected;
                try { affected = await tx.ExecuteSqlAsync(Query(
                    "UPDATE teaql_blob_catalog SET content_version=@p0 WHERE key=@p1 AND scope=@p2 AND status='deleting' AND content_version=''",
                    Text(version), Text(claim.Key), Text(scope))).ConfigureAwait(false); }
                catch { throw Error("BLOB_CATALOG_FAILED"); }
                if (affected != 1) throw Error("BLOB_IDEMPOTENCY_CONFLICT");
            }
            return true;
        });
    }

    private Task<bool> FinishAsync(string scope, Row claim, string version, long now) => Transaction(async tx =>
    {
        var row = await ReadRowAsync(tx, scope, claim.Key).ConfigureAwait(false);
        if (row == null || row.ObjectKey != claim.ObjectKey || row.Version != version)
            throw Error("BLOB_IDEMPOTENCY_CONFLICT");
        if (row.Status == "deleted") return true;
        var bound = await CountAsync(tx, "SELECT count(*) AS n FROM teaql_blob_binding WHERE key=@p0", claim.Key).ConfigureAwait(false);
        if (!Eligible(row, now) || row.Status != "deleting" || bound != 0)
            throw Error("BLOB_IDEMPOTENCY_CONFLICT");
        ulong affected;
        try { affected = await tx.ExecuteSqlAsync(Query(
            "UPDATE teaql_blob_catalog SET status='deleted' WHERE key=@p0 AND scope=@p1 AND status='deleting' AND content_version=@p2",
            Text(claim.Key), Text(scope), Text(version))).ConfigureAwait(false); }
        catch { throw Error("BLOB_CATALOG_FAILED"); }
        if (affected != 1) throw Error("BLOB_CATALOG_FAILED");
        return true;
    });

    private async Task<BlobMaintenanceOutcome> CollectAsync(UserContext actor, string scope,
        string key, ObjectStorage storage)
    {
        var reference = new BlobRef(key);
        try
        {
            await AuthorizeAsync(actor, scope).ConfigureAwait(false);
            var claim = await ClaimAsync(scope, key, NowMs()).ConfigureAwait(false);
            if (claim == null) return new(reference, "not_eligible");
            await AuthorizeAsync(actor, scope).ConfigureAwait(false);
            ObjectKey objectKey;
            try { objectKey = new ObjectKey(claim.ObjectKey); }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
            var version = claim.Version;
            if (version.Length > 0 && claim.WrittenETag.Length > 0 && version != claim.WrittenETag)
                throw Error("BLOB_IDEMPOTENCY_CONFLICT");
            if (version.Length == 0)
            {
                ObjectMetadata? observed;
                try { observed = await storage.HeadAsync(objectKey).ConfigureAwait(false); }
                catch { throw Error("BLOB_STORE_FAILED"); }
                if (observed != null)
                {
                    if (!ValidVersion(observed.ETag) ||
                        claim.WrittenETag.Length > 0 && observed.ETag != claim.WrittenETag)
                        throw Error("BLOB_IDEMPOTENCY_CONFLICT");
                    version = observed.ETag!;
                    await AuthorizeAsync(actor, scope).ConfigureAwait(false);
                    await CaptureAsync(scope, claim, version, NowMs()).ConfigureAwait(false);
                }
            }
            await AuthorizeAsync(actor, scope).ConfigureAwait(false);
            var result = "already_missing";
            if (version.Length > 0)
            {
                ObjectConditionalDeleteResult deleted;
                try { deleted = await storage.DeleteIfVersionAsync(objectKey, version).ConfigureAwait(false); }
                catch { throw Error("BLOB_STORE_FAILED"); }
                if (deleted == ObjectConditionalDeleteResult.VersionMismatch)
                    throw Error("BLOB_IDEMPOTENCY_CONFLICT");
                result = deleted == ObjectConditionalDeleteResult.Deleted ? "deleted" : "already_missing";
            }
            await AuthorizeAsync(actor, scope).ConfigureAwait(false);
            await FinishAsync(scope, claim, version, NowMs()).ConfigureAwait(false);
            return new(reference, result);
        }
        catch (InvalidOperationException error)
        {
            if (error.Message is "BLOB_MAINTENANCE_DENIED" or "BLOB_TRANSACTION_REQUIRED") throw;
            if (error.Message == "BLOB_IDEMPOTENCY_CONFLICT")
                return new(reference, "version_conflict", "BLOB_IDEMPOTENCY_CONFLICT");
            var code = error.Message == "BLOB_CATALOG_FAILED" ? "BLOB_CATALOG_FAILED" : "BLOB_STORE_FAILED";
            return new(reference, "failed", code);
        }
    }

    public async Task<BlobMaintenanceBatch> RunBatchAsync(UserContext actor,
        BlobMaintenanceCursor? cursor = null)
    {
        var scope = await AuthorizeAsync(actor).ConfigureAwait(false);
        if (cursor != null && cursor.Scope != scope) throw Error("BLOB_MAINTENANCE_DENIED");
        actor.RequireNoActiveGraphSql();
        ObjectStorage storage;
        try { storage = actor.ObjectStorage("collect expired Blob upload"); }
        catch { throw Error("BLOB_CAPABILITY_MISSING"); }
        if (!storage.SupportsVersionConditionalDelete) throw Error("BLOB_CAPABILITY_MISSING");
        await _catalog.ValidateSchemaAsync().ConfigureAwait(false);
        var now = NowMs();
        List<Record> rows;
        try
        {
            rows = await _transport.FetchAllSqlAsync(Query("""
                SELECT DISTINCT c.key FROM teaql_blob_catalog c JOIN teaql_blob_upload u
                  ON u.key=c.key AND u.scope=c.scope
                WHERE c.scope=@p0 AND c.key>@p1 AND (
                  (u.expires_ms<=@p2 AND u.state IN ('initiated','written')
                    AND c.status IN ('initiated','deleting')
                    AND (c.upload_mode IS NULL OR c.upload_mode='server_stream')) OR
                  (u.state='completed' AND c.upload_mode='server_stream'
                    AND c.policy_profile IS NOT NULL AND c.completed_at_ms IS NOT NULL
                    AND c.content_version<>'' AND (c.status='deleting' OR
                      (c.status='uploaded_unbound' AND c.last_unbound_at_ms IS NULL
                        AND c.completed_at_ms+c.unbound_retention_seconds*1000<=@p2) OR
                      (c.status='available' AND c.last_unbound_at_ms IS NOT NULL
                        AND c.last_unbound_at_ms+c.effective_unref_seconds*1000<=@p2
                        AND (c.retention_floor_until_ms IS NULL OR c.retention_floor_until_ms<=@p2)))))
                  AND NOT EXISTS (SELECT 1 FROM teaql_blob_binding b WHERE b.key=c.key)
                ORDER BY c.key LIMIT @p3
                """, Text(scope), Text(cursor?.After ?? ""), Number(now), Number(_batchSize))).ConfigureAwait(false);
        }
        catch { throw Error("BLOB_CATALOG_FAILED"); }
        var keys = new List<string>();
        foreach (var row in rows)
        {
            var key = row.GetValueOrDefault("key")?.TryText();
            try { _ = new BlobRef(key!); }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
            keys.Add(key!);
        }
        var outcomes = new List<BlobMaintenanceOutcome>();
        foreach (var key in keys)
            outcomes.Add(await CollectAsync(actor, scope, key, storage).ConfigureAwait(false));
        var next = keys.Count == _batchSize ? new BlobMaintenanceCursor(scope, keys[^1]) : null;
        return new BlobMaintenanceBatch(outcomes, next);
    }
}
