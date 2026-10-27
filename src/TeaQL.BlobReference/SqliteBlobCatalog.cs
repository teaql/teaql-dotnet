using System.Text;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;

namespace TeaQL.BlobReference;

public sealed record BlobBindingTarget(string Entity, string EntityId, string Field);
public sealed record BlobBindingChange(BlobBindingTarget Target, BlobRef? Next,
    long ExpectedVersion, long NextVersion, string Action);
public sealed record BlobCatalogRead(string ObjectKey, string ContentVersion,
    string FileName, string ContentType, long SizeBytes);

public interface IBlobBindingAuthorization
{
    string Scope(UserContext context);
    Task<bool> AuthorizeAsync(UserContext context, BlobBindingTarget target,
        BlobRef? previous, BlobRef? next, string action);
}

/// <summary>SQLite-only authoritative catalog. Schema installation is an explicit host action.</summary>
public sealed class SqliteBlobCatalog(ISqlTransport transport)
{
    private static InvalidOperationException Error(string code) => new(code);
    private static CompiledQuery Query(string sql, params Value[] values) => new(sql, values.ToList());

    public async Task EnsureSchemaAsync()
    {
        foreach (var sql in new[] {
            """
            CREATE TABLE IF NOT EXISTS teaql_blob_catalog (
              key TEXT PRIMARY KEY, scope TEXT NOT NULL, object_key TEXT NOT NULL,
              content_version TEXT NOT NULL, file_name TEXT NOT NULL,
              content_type TEXT NOT NULL, size_bytes BIGINT NOT NULL,
              status TEXT NOT NULL CHECK (status IN
                ('initiated','uploaded_unbound','available','deleting','deleted')))
            """,
            """
            CREATE TABLE IF NOT EXISTS teaql_blob_binding (
              scope TEXT NOT NULL, entity TEXT NOT NULL, entity_id TEXT NOT NULL,
              field TEXT NOT NULL, key TEXT, version BIGINT NOT NULL,
              PRIMARY KEY (scope,entity,entity_id,field),
              FOREIGN KEY (key) REFERENCES teaql_blob_catalog(key))
            """,
            "CREATE INDEX IF NOT EXISTS teaql_blob_binding_key ON teaql_blob_binding(key)"
        })
            try { await transport.ExecuteSqlAsync(Query(sql)); }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
    }

    public async Task ValidateSchemaAsync()
    {
        try
        {
            await transport.FetchAllSqlAsync(Query("SELECT key,scope,object_key,content_version,file_name,content_type,size_bytes,status FROM teaql_blob_catalog WHERE 1 = 0"));
            await transport.FetchAllSqlAsync(Query("SELECT scope,entity,entity_id,field,key,version FROM teaql_blob_binding WHERE 1 = 0"));
            var columns = await transport.FetchAllSqlAsync(Query("PRAGMA table_info(teaql_blob_binding)"));
            var key = columns.SingleOrDefault(row => row.GetValueOrDefault("name")?.TryText() == "key");
            var version = columns.SingleOrDefault(row => row.GetValueOrDefault("name")?.TryText() == "version");
            if (key?.GetValueOrDefault("notnull")?.TryI64() != 0 ||
                version?.GetValueOrDefault("notnull")?.TryI64() != 1)
                throw Error("BLOB_SCHEMA_MISSING");
        }
        catch { throw Error("BLOB_SCHEMA_MISSING"); }
    }

    /// <summary>Recheck the exact current binding; a stable Key grants no read authority.</summary>
    public async Task<BlobCatalogRead> CurrentReadAsync(AuthorizedBlobSlot slot)
    {
        if (slot == null || !Scope(slot.Scope) || !Name(slot.Entity) ||
            !EntityId(slot.EntityId) || !Name(slot.Field) || slot.BindingVersion < 1 ||
            slot.Reference == null)
            throw Error("BLOB_READ_DENIED");
        List<Record> rows;
        try
        {
            rows = await transport.FetchAllSqlAsync(Query("""
                SELECT c.object_key, c.content_version, c.file_name,
                  c.content_type, c.size_bytes FROM teaql_blob_binding b
                JOIN teaql_blob_catalog c ON c.key = b.key
                WHERE b.scope = @p0 AND b.entity = @p1 AND b.entity_id = @p2
                  AND b.field = @p3 AND b.key = @p4 AND b.version = @p5
                  AND c.scope = b.scope AND c.status = 'available'
                """, new Value.TextValue(slot.Scope), new Value.TextValue(slot.Entity),
                new Value.TextValue(slot.EntityId), new Value.TextValue(slot.Field),
                new Value.TextValue(slot.Reference.Key), new Value.I64Value(slot.BindingVersion)));
        }
        catch { throw Error("BLOB_CATALOG_FAILED"); }
        if (rows.Count != 1) throw Error("BLOB_READ_DENIED");
        var row = rows[0];
        var objectKey = row.GetValueOrDefault("object_key")?.TryText();
        var version = row.GetValueOrDefault("content_version")?.TryText();
        var fileName = row.GetValueOrDefault("file_name")?.TryText();
        var contentType = row.GetValueOrDefault("content_type")?.TryText();
        var size = row.GetValueOrDefault("size_bytes")?.TryI64();
        if (string.IsNullOrWhiteSpace(objectKey) || string.IsNullOrWhiteSpace(version) ||
            string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(contentType) ||
            size == null || size < 0)
            throw Error("BLOB_CATALOG_FAILED");
        return new BlobCatalogRead(objectKey, version, fileName, contentType, size.Value);
    }

    private static bool Name(string? value) => !string.IsNullOrWhiteSpace(value) &&
        value == value.Trim() && value.EnumerateRunes().Count() <= 128 &&
        !value.Any(char.IsControl);

    private static bool Scope(string? value) => !string.IsNullOrWhiteSpace(value) &&
        value == value.Trim() && Encoding.UTF8.GetByteCount(value) <= 256 &&
        !value.Any(char.IsControl);

    private static bool EntityId(string? value) => ulong.TryParse(value, out var id) &&
        id > 0 && id.ToString() == value;

    /// <summary>Bind one target inside the caller's native graph physical transaction.</summary>
    public Task<long> BindInGraphAsync(GraphMutationSession graph, UserContext actor,
        BlobBindingChange change, IBlobBindingAuthorization authorization) =>
        graph.WithPhysicalSqlAsync(async lease =>
        {
            if (!graph.BelongsTo(actor)) throw Error("BLOB_BIND_DENIED");
            if (change?.Target == null || authorization == null ||
                !Name(change.Target.Entity) || !Name(change.Target.Field) || !EntityId(change.Target.EntityId) ||
                change.ExpectedVersion < 0 || change.NextVersion <= change.ExpectedVersion ||
                change.Action is not ("Create" or "Update" or "Delete") ||
                change.Action == "Create" && change.ExpectedVersion != 0 ||
                change.Action == "Delete" && change.Next != null)
                throw Error("BLOB_REFERENCE_INVALID");
            string scope;
            try { scope = authorization.Scope(actor); }
            catch { throw Error("BLOB_BIND_DENIED"); }
            if (!Scope(scope)) throw Error("BLOB_BIND_DENIED");
            var target = change.Target;
            Value[] identity = [new Value.TextValue(scope), new Value.TextValue(target.Entity),
                new Value.TextValue(target.EntityId), new Value.TextValue(target.Field)];
            List<Record> rows;
            try { rows = await lease.FetchAllAsync("SELECT key,version FROM teaql_blob_binding WHERE scope = @p0 AND entity = @p1 AND entity_id = @p2 AND field = @p3", identity); }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
            if (rows.Count > 1) throw Error("BLOB_CATALOG_FAILED");
            var row = rows.FirstOrDefault();
            var version = row?.GetValueOrDefault("version")?.TryI64() ?? 0;
            if (version < 0 || version != change.ExpectedVersion) throw Error("BLOB_BIND_DENIED");
            BlobRef? previous = null;
            if (row != null && row.GetValueOrDefault("key") is not (null or Value.NullValue))
            {
                try { previous = new BlobRef(row["key"].TryText()!); }
                catch { throw Error("BLOB_CATALOG_FAILED"); }
            }
            bool allowed;
            try { allowed = await authorization.AuthorizeAsync(actor, target, previous, change.Next, change.Action); }
            catch { throw Error("BLOB_BIND_DENIED"); }
            if (!allowed) throw Error("BLOB_BIND_DENIED");
            if (change.Next != null)
            {
                List<Record> available;
                try { available = await lease.FetchAllAsync("SELECT key FROM teaql_blob_catalog WHERE key = @p0 AND scope = @p1 AND status IN ('uploaded_unbound','available')",
                    [new Value.TextValue(change.Next.Key), new Value.TextValue(scope)]); }
                catch { throw Error("BLOB_CATALOG_FAILED"); }
                if (available.Count != 1) throw Error("BLOB_BIND_DENIED");
            }
            Value next = change.Next is null ? new Value.NullValue() : new Value.TextValue(change.Next.Key);
            ulong affected;
            try
            {
                affected = row != null
                    ? await lease.ExecuteAsync("UPDATE teaql_blob_binding SET key = @p0, version = @p1 WHERE scope = @p2 AND entity = @p3 AND entity_id = @p4 AND field = @p5 AND version = @p6",
                        [next, new Value.I64Value(change.NextVersion), ..identity, new Value.I64Value(version)])
                    : await lease.ExecuteAsync("INSERT INTO teaql_blob_binding (scope,entity,entity_id,field,key,version) VALUES (@p0,@p1,@p2,@p3,@p4,@p5)",
                        [..identity, next, new Value.I64Value(change.NextVersion)]);
            }
            catch { throw Error("BLOB_CATALOG_FAILED"); }
            if (affected != 1) throw Error("BLOB_BIND_DENIED");
            if (change.Next != null)
            {
                ulong promoted;
                try { promoted = await lease.ExecuteAsync(
                    "UPDATE teaql_blob_catalog SET status = 'available' WHERE key = @p0 AND scope = @p1 AND status IN ('uploaded_unbound','available')",
                    [new Value.TextValue(change.Next.Key), new Value.TextValue(scope)]); }
                catch { throw Error("BLOB_CATALOG_FAILED"); }
                if (promoted != 1) throw Error("BLOB_BIND_DENIED");
            }
            if (previous != null && !previous.Equals(change.Next))
            {
                try { await lease.ExecuteAsync(
                    "UPDATE teaql_blob_catalog SET status = 'uploaded_unbound' WHERE key = @p0 AND scope = @p1 AND status = 'available' " +
                    "AND NOT EXISTS (SELECT 1 FROM teaql_blob_binding WHERE key = @p2)",
                    [new Value.TextValue(previous.Key), new Value.TextValue(scope), new Value.TextValue(previous.Key)]); }
                catch { throw Error("BLOB_CATALOG_FAILED"); }
            }
            return change.NextVersion;
        });
}
