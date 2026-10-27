using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core.Storage;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class BlobMaintenanceSqliteTests
{
    private sealed class Authority : IBlobMaintenanceAuthorization
    {
        public string CurrentScope = "tenant-a";
        public bool Allowed = true;
        public string Scope(UserContext actor) => CurrentScope;
        public Task<bool> CanMaintainAsync(UserContext actor, string scope) => Task.FromResult(Allowed);
    }

    private sealed class ConditionalStore : IObjectStorageProvider, IObjectStorageConditionalDeleter
    {
        public readonly Dictionary<string, string> Objects = new();
        public int Deletes;
        public Task<ObjectMetadata?> HeadAsync(StorageInvocation invocation, ObjectKey key) =>
            Task.FromResult(Objects.TryGetValue(key.Value, out var version)
                ? new ObjectMetadata(key, 1, "application/pdf", version) : null);
        public Task<ObjectConditionalDeleteResult> DeleteIfVersionAsync(StorageInvocation invocation,
            ObjectKey key, string contentVersion)
        {
            Deletes++;
            if (!Objects.TryGetValue(key.Value, out var actual))
                return Task.FromResult(ObjectConditionalDeleteResult.AlreadyMissing);
            if (actual != contentVersion)
                return Task.FromResult(ObjectConditionalDeleteResult.VersionMismatch);
            Objects.Remove(key.Value);
            return Task.FromResult(ObjectConditionalDeleteResult.Deleted);
        }
        public Task DeleteAsync(StorageInvocation invocation, ObjectKey key) =>
            throw new InvalidOperationException("unconditional delete forbidden");
        public Task<ObjectMetadata> PutAsync(StorageInvocation invocation, ObjectKey key,
            Stream content, ObjectPutOptions options) => throw new NotSupportedException();
        public Task<ObjectContent> GetAsync(StorageInvocation invocation, ObjectKey key) =>
            throw new NotSupportedException();
        public Task<ObjectReadUrl> GetReadUrlAsync(StorageInvocation invocation, ObjectKey key,
            ObjectReadUrlOptions options) => throw new NotSupportedException();
    }

    private static async Task Execute(SqliteConnection db, string statement, params (string Name, object? Value)[] values)
    {
        await using var command = db.CreateCommand();
        command.CommandText = statement;
        foreach (var (name, value) in values)
            command.Parameters.AddWithValue(name, value ?? DBNull.Value);
        await command.ExecuteNonQueryAsync();
    }

    private static async Task Seed(SqliteConnection db, string key, string scope = "tenant-a",
        string state = "written", string version = "", string? writtenETag = null,
        long expires = 9000, bool bound = false)
    {
        await Execute(db, """
            INSERT INTO teaql_blob_catalog
              (key,scope,object_key,content_version,file_name,content_type,size_bytes,status)
              VALUES (@key,@scope,@object,@version,'file.pdf','',0,@status)
            """, ("@key", key), ("@scope", scope), ("@object", "objects/" + key),
            ("@version", version), ("@status", state == "completed" ? "uploaded_unbound" : "initiated"));
        await Execute(db, """
            INSERT INTO teaql_blob_upload
              (upload_id,scope,replay_key,request_hash,key,entity,entity_id,field,file_name,
               extension,declared_size,expires_ms,state,written_etag)
              VALUES (@id,@scope,@replay,@hash,@key,'Asset','1','document','file.pdf',
                'pdf',1,@expires,@state,@etag)
            """, ("@id", "bu_" + key), ("@scope", scope), ("@replay", "replay_" + key),
            ("@hash", "hash_" + key), ("@key", key), ("@expires", expires),
            ("@state", state), ("@etag", writtenETag));
        if (bound)
            await Execute(db, """
                INSERT INTO teaql_blob_binding (scope,entity,entity_id,field,key,version)
                  VALUES (@scope,'Asset',@key,'document',@key,1)
                """, ("@scope", scope), ("@key", key));
    }

    private static async Task<string> Status(SqliteConnection db, string key)
    {
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT status FROM teaql_blob_catalog WHERE key=@key";
        command.Parameters.AddWithValue("@key", key);
        return (string)(await command.ExecuteScalarAsync())!;
    }

    [Fact]
    public async Task ExpiredUploadClaimScopeVersionAndRecoveryUseRealSqlite()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
        await catalog.EnsureSchemaAsync();
        var store = new ConditionalStore();
        var actor = new UserContext().WithObjectStorage(store);
        var authority = new Authority();
        var service = new SqliteBlobMaintenanceService(catalog, authority, 2,
            () => DateTimeOffset.FromUnixTimeMilliseconds(10000));
        await Seed(db, "blob_absent", state: "initiated");
        await Seed(db, "blob_bound", state: "initiated", bound: true);
        await Seed(db, "blob_completed", state: "completed", writtenETag: "etag-completed");
        await Seed(db, "blob_future", expires: 20000, writtenETag: "etag-future");
        await Seed(db, "blob_other", scope: "tenant-b", writtenETag: "etag-other");
        await Seed(db, "blob_race", version: "etag-old", writtenETag: "etag-old");
        await Seed(db, "blob_written", writtenETag: "etag-written");
        store.Objects["objects/blob_race"] = "etag-new";
        store.Objects["objects/blob_written"] = "etag-written";
        await catalog.ValidateSchemaAsync();

        authority.Allowed = false;
        Assert.Equal("BLOB_MAINTENANCE_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RunBatchAsync(actor))).Message);
        authority.Allowed = true;
        var first = await service.RunBatchAsync(actor);
        Assert.Equal(["blob_absent", "blob_race"], first.Outcomes.Select(o => o.Reference.Key).ToArray());
        Assert.Equal(["already_missing", "version_conflict"], first.Outcomes.Select(o => o.Status).ToArray());
        Assert.NotNull(first.Next);
        var second = await service.RunBatchAsync(actor, first.Next);
        Assert.Single(second.Outcomes);
        Assert.Equal("deleted", second.Outcomes[0].Status);
        Assert.Equal(1, store.Deletes);
        Assert.Equal("etag-new", store.Objects["objects/blob_race"]);
        foreach (var (key, expected) in new Dictionary<string, string> {
            ["blob_absent"] = "deleted", ["blob_race"] = "deleting", ["blob_written"] = "deleted",
            ["blob_completed"] = "uploaded_unbound", ["blob_future"] = "initiated",
            ["blob_other"] = "initiated", ["blob_bound"] = "initiated" })
            Assert.Equal(expected, await Status(db, key));
        authority.CurrentScope = "tenant-b";
        Assert.Equal("BLOB_MAINTENANCE_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RunBatchAsync(actor, first.Next))).Message);
        authority.CurrentScope = "tenant-a";

        await Seed(db, "blob_retry", writtenETag: "etag-retry");
        store.Objects["objects/blob_retry"] = "etag-retry";
        await Execute(db, """
            CREATE TRIGGER fail_final_ack BEFORE UPDATE OF status ON teaql_blob_catalog
              WHEN NEW.status='deleted' AND OLD.key='blob_retry'
              BEGIN SELECT RAISE(ABORT,'private trigger detail'); END
            """);
        var failed = await service.RunBatchAsync(actor);
        Assert.Equal("failed", failed.Outcomes[1].Status);
        Assert.Equal("BLOB_CATALOG_FAILED", failed.Outcomes[1].Error);
        Assert.False(store.Objects.ContainsKey("objects/blob_retry"));
        Assert.Equal("deleting", await Status(db, "blob_retry"));
        await Execute(db, "DROP TRIGGER fail_final_ack");
        var recovered = await service.RunBatchAsync(actor);
        Assert.Equal("already_missing", recovered.Outcomes[1].Status);
        Assert.Equal("deleted", await Status(db, "blob_retry"));
    }

    [Fact]
    public async Task MissingConditionalCapabilityAndObjectKeyIndexAreRejected()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
        await catalog.EnsureSchemaAsync();
        var authority = new Authority();
        var service = new SqliteBlobMaintenanceService(catalog, authority, 2);
        var actor = new UserContext().WithObjectStorage(new UnfencedStore());
        Assert.Equal("BLOB_CAPABILITY_MISSING", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RunBatchAsync(actor))).Message);
        actor.WithObjectStorage(new ConditionalStore());
        await Execute(db, "DROP INDEX teaql_blob_catalog_object_key");
        Assert.Equal("BLOB_SCHEMA_MISSING", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RunBatchAsync(actor))).Message);
    }

    private sealed class UnfencedStore : IObjectStorageProvider
    {
        public Task<ObjectMetadata> PutAsync(StorageInvocation i, ObjectKey k, Stream c, ObjectPutOptions o) => throw new NotSupportedException();
        public Task<ObjectContent> GetAsync(StorageInvocation i, ObjectKey k) => throw new NotSupportedException();
        public Task<ObjectMetadata?> HeadAsync(StorageInvocation i, ObjectKey k) => throw new NotSupportedException();
        public Task DeleteAsync(StorageInvocation i, ObjectKey k) => throw new NotSupportedException();
        public Task<ObjectReadUrl> GetReadUrlAsync(StorageInvocation i, ObjectKey k, ObjectReadUrlOptions o) => throw new NotSupportedException();
    }
}
