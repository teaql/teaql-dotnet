using System.Text;
using TeaQL.Core;
using TeaQL.Core.Storage;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.Sql;

namespace TeaQL.BlobReference;

public sealed class BlobFileGenerationSweepCursor
{
    internal BlobFileGenerationSweepCursor(SqliteBlobFileGenerationSweepService owner, string scope, string after)
    { Owner = owner; Scope = scope; After = after; }
    internal SqliteBlobFileGenerationSweepService Owner { get; }
    internal string Scope { get; }
    internal string After { get; }
    public override string ToString() => "BlobFileGenerationSweepCursor[masked]";
}

public sealed record BlobFileGenerationSweepOutcome(BlobRef Reference, string Status,
    int RemovedGenerations = 0, string? Error = null);
public sealed record BlobFileGenerationSweepBatch(
    IReadOnlyList<BlobFileGenerationSweepOutcome> Outcomes, BlobFileGenerationSweepCursor? Next);

/// <summary>Explicit host-scheduled cleanup of inaccessible fenced File data generations.</summary>
public sealed class SqliteBlobFileGenerationSweepService
{
    private readonly SqliteBlobCatalog _catalog;
    private readonly ISqlTransactionTransport _transport;
    private readonly IBlobMaintenanceAuthorization _authorization;
    private readonly int _batchSize;

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

    public SqliteBlobFileGenerationSweepService(SqliteBlobCatalog catalog,
        IBlobMaintenanceAuthorization authorization, bool allWritersUpgraded, int batchSize = 100)
    {
        _catalog = catalog ?? throw Error("BLOB_POLICY_INVALID");
        _transport = catalog.Transport as ISqlTransactionTransport ?? throw Error("BLOB_CAPABILITY_MISSING");
        _authorization = authorization ?? throw Error("BLOB_POLICY_INVALID");
        if (!allWritersUpgraded || batchSize is < 1 or > 1000) throw Error("BLOB_POLICY_INVALID");
        _batchSize = batchSize;
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

    public async Task<BlobFileGenerationSweepBatch> RunBatchAsync(UserContext actor,
        FileObjectStorage provider, BlobFileGenerationSweepCursor? cursor = null,
        CancellationToken cancellationToken = default)
    {
        var scope = await AuthorizeAsync(actor).ConfigureAwait(false);
        if (cursor is not null && (!ReferenceEquals(cursor.Owner, this) || cursor.Scope != scope))
            throw Error("BLOB_MAINTENANCE_DENIED");
        actor.RequireNoActiveGraphSql();
        if (provider is null || !ReferenceEquals(actor.GetResource<IObjectStorageProvider>(), provider))
            throw Error("BLOB_CAPABILITY_MISSING");
        await _catalog.ValidateSchemaAsync().ConfigureAwait(false);
        List<Record> rows;
        try
        {
            rows = await _transport.FetchAllSqlAsync(Query("""
                SELECT key,object_key FROM teaql_blob_catalog
                WHERE scope=@p0 AND key>@p1 AND upload_mode='server_stream'
                ORDER BY key LIMIT @p2
                """, Text(scope), Text(cursor?.After ?? ""), Number(_batchSize))).ConfigureAwait(false);
        }
        catch { throw Error("BLOB_CATALOG_FAILED"); }
        var candidates = new List<(BlobRef Reference, ObjectKey ObjectKey)>();
        foreach (var row in rows)
        {
            try
            {
                var reference = new BlobRef(row.GetValueOrDefault("key")?.TryText()!);
                var key = new ObjectKey(row.GetValueOrDefault("object_key")?.TryText()!);
                candidates.Add((reference, key));
            }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
        }
        var outcomes = new List<BlobFileGenerationSweepOutcome>();
        foreach (var (reference, key) in candidates)
        {
            await AuthorizeAsync(actor, scope).ConfigureAwait(false);
            try
            {
                var removed = await provider.SweepOldGenerationsAsync(key, cancellationToken).ConfigureAwait(false);
                if (removed < 0) throw Error("BLOB_STORE_FAILED");
                outcomes.Add(new(reference, "swept", removed));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch { outcomes.Add(new(reference, "failed", 0, "BLOB_STORE_FAILED")); }
        }
        var next = candidates.Count == _batchSize
            ? new BlobFileGenerationSweepCursor(this, scope, candidates[^1].Reference.Key) : null;
        return new(outcomes, next);
    }
}
