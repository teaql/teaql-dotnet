using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;
using TeaQL.BlobReference;
using TeaQL.Core.Storage;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class BlobMaintenanceSqliteTests
{
    [Fact]
    public async Task ExpiredDirectUploadsRemainUntouchedWithoutQuiescence()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
        await catalog.EnsureSchemaAsync();
        var store = new ConditionalStore();
        var actor = new UserContext().WithObjectStorage(store);
        var service = new SqliteBlobMaintenanceService(catalog, new Authority(), 10,
            () => DateTimeOffset.FromUnixTimeMilliseconds(10000));
        foreach (var (key, state, status) in new[] {
            ("blob_direct_initiated", "initiated", "initiated"),
            ("blob_direct_written", "written", "initiated"),
            ("blob_direct_deleting", "initiated", "deleting") })
        {
            await Seed(db, key, state: state, writtenETag: state == "written" ? "etag-direct" : null);
            await Execute(db, "UPDATE teaql_blob_catalog SET upload_mode='direct',status=@status WHERE key=@key",
                ("@status", status), ("@key", key));
            store.Objects["objects/" + key] = "etag-direct";
        }
        var batch = await service.RunBatchAsync(actor);
        Assert.Empty(batch.Outcomes);
        Assert.Equal(0, store.Deletes);
        Assert.Equal(3, store.Objects.Count);
        foreach (var key in new[] { "blob_direct_initiated", "blob_direct_written", "blob_direct_deleting" })
            Assert.NotEqual("deleted", await Status(db, key));
    }

    [Fact]
    public async Task RealSqliteAndFileDeleteRecoverReceiptAndPreserveNewGeneration()
    {
        var parent = Path.Combine(Path.GetTempPath(), "teaql-file-maintenance-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(parent);
        try
        {
            await using var db = new SqliteConnection("Data Source=" + Path.Combine(parent, "catalog.sqlite"));
            await db.OpenAsync();
            var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
            await catalog.EnsureSchemaAsync();
            await using var file = new FileObjectStorage(Path.Combine(parent, "private"),
                RandomNumberGenerator.GetBytes(32));
            var actor = new UserContext().WithObjectStorage(file);
            var storage = actor.ObjectStorage("seed real File upload");
            var service = new SqliteBlobMaintenanceService(catalog, new Authority(), 10,
                () => DateTimeOffset.FromUnixTimeMilliseconds(10000));
            var bytes = Encoding.UTF8.GetBytes("%PDF-");
            var key = new ObjectKey("objects/blob_file_receipt");
            _ = await storage.PutBytesAsync(key, bytes, new(bytes.Length, ObjectWriteMode.CreateOnly));
            await Seed(db, "blob_file_receipt", state: "initiated");
            await Execute(db, """
                CREATE TRIGGER fail_file_receipt BEFORE UPDATE OF status ON teaql_blob_catalog
                  WHEN NEW.status='deleted' BEGIN SELECT RAISE(ABORT,'receipt failure'); END
                """);
            var failed = await service.RunBatchAsync(actor);
            Assert.Equal("failed", Assert.Single(failed.Outcomes).Status);
            Assert.Equal("deleting", await Status(db, "blob_file_receipt"));
            Assert.Null(await storage.HeadAsync(key));
            await Execute(db, "DROP TRIGGER fail_file_receipt");
            var retry = await service.RunBatchAsync(actor);
            Assert.Equal("already_missing", Assert.Single(retry.Outcomes).Status);
            Assert.Equal("deleted", await Status(db, "blob_file_receipt"));

            var staleKey = new ObjectKey("objects/blob_file_stale");
            var old = await storage.PutBytesAsync(staleKey, bytes,
                new(bytes.Length, ObjectWriteMode.CreateOnly));
            await Seed(db, "blob_file_stale", state: "initiated", version: old.ETag!);
            var newer = await storage.PutBytesAsync(staleKey, bytes,
                new(bytes.Length, ObjectWriteMode.Replace));
            var stale = await service.RunBatchAsync(actor);
            Assert.Equal("version_conflict", Assert.Single(stale.Outcomes).Status);
            Assert.Equal(newer.ETag, (await storage.HeadAsync(staleKey))?.ETag);

            var partialKey = new ObjectKey("objects/blob_file_partial");
            var partial = await storage.PutBytesAsync(partialKey, bytes,
                new(bytes.Length, ObjectWriteMode.CreateOnly));
            await Seed(db, "blob_file_partial", state: "initiated", version: partial.ETag!);
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(partialKey.Value))).ToLowerInvariant();
            File.Delete(Path.Combine(parent, "private", hash[..2], hash,
                "data-" + partial.ETag!.Split('.').Last()));
            var recoveredPartial = await service.RunBatchAsync(actor);
            Assert.Contains(recoveredPartial.Outcomes,
                item => item.Reference.Key == "blob_file_partial" && item.Status == "deleted");
            Assert.Equal("deleted", await Status(db, "blob_file_partial"));
        }
        finally { Directory.Delete(parent, true); }
    }

    [Fact]
    public async Task CompletedServerStreamRetentionUsesRealFileVersionAndRecoverableReceipt()
    {
        var parent = Path.Combine(Path.GetTempPath(), "teaql-completed-retention-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(parent);
        try
        {
            await using var db = new SqliteConnection("Data Source=" + Path.Combine(parent, "catalog.sqlite"));
            await db.OpenAsync();
            var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
            await catalog.EnsureSchemaAsync();
            await using var file = new FileObjectStorage(Path.Combine(parent, "private"),
                RandomNumberGenerator.GetBytes(32));
            var actor = new UserContext().WithObjectStorage(file);
            var storage = actor.ObjectStorage("real completed retention");
            var service = new SqliteBlobMaintenanceService(catalog, new Authority(), 100,
                () => DateTimeOffset.FromUnixTimeMilliseconds(10000));
            var bytes = Encoding.UTF8.GetBytes("%PDF-1.7\nretained\n%%EOF");

            async Task<(ObjectKey Key, string Version)> SeedCompleted(string name,
                string status = "uploaded_unbound", long? last = null, long? floor = null,
                string? profile = "default", string mode = "server_stream", bool bound = false)
            {
                var key = new ObjectKey("objects/" + name);
                var created = await storage.PutBytesAsync(key, bytes,
                    new(bytes.Length, ObjectWriteMode.CreateOnly));
                await Seed(db, name, state: "completed", version: created.ETag!,
                    writtenETag: created.ETag, bound: bound);
                await Execute(db, """
                    UPDATE teaql_blob_catalog SET status=@status,completed_at_ms=9000,
                      last_unbound_at_ms=@last,policy_profile=@profile,upload_mode=@mode,
                      unbound_retention_seconds=1,effective_unref_seconds=2,
                      retention_floor_until_ms=@floor,content_type='application/pdf',size_bytes=@size
                    WHERE key=@key
                    """, ("@status", status), ("@last", last), ("@profile", profile),
                    ("@mode", mode), ("@floor", floor), ("@size", bytes.Length), ("@key", name));
                return (key, created.ETag!);
            }

            var (expired, _) = await SeedCompleted("blob_completed_expired");
            var (former, _) = await SeedCompleted("blob_completed_former", status: "available",
                last: 7000, floor: 10000);
            var (active, _) = await SeedCompleted("blob_completed_active", status: "available", bound: true);
            var (floored, _) = await SeedCompleted("blob_completed_floor", status: "available",
                last: 7000, floor: 20000);
            var (legacy, _) = await SeedCompleted("blob_completed_legacy", profile: null);
            var (direct, _) = await SeedCompleted("blob_completed_direct", mode: "direct");
            var first = await service.RunBatchAsync(actor);
            Assert.Equal(["blob_completed_expired", "blob_completed_former"],
                first.Outcomes.Select(item => item.Reference.Key).ToArray());
            Assert.All(first.Outcomes, item => Assert.Equal("deleted", item.Status));
            Assert.Null(await storage.HeadAsync(expired));
            Assert.Null(await storage.HeadAsync(former));
            foreach (var key in new[] { active, floored, legacy, direct })
                Assert.NotNull(await storage.HeadAsync(key));

            var (staleKey, oldVersion) = await SeedCompleted("blob_completed_stale");
            var newer = await storage.PutBytesAsync(staleKey, bytes,
                new(bytes.Length, ObjectWriteMode.Replace));
            Assert.NotEqual(oldVersion, newer.ETag);
            var stale = await service.RunBatchAsync(actor);
            Assert.Equal("version_conflict", Assert.Single(stale.Outcomes).Status);
            Assert.Equal(newer.ETag, (await storage.HeadAsync(staleKey))?.ETag);
            Assert.Equal("deleting", await Status(db, "blob_completed_stale"));

            var (retryKey, _) = await SeedCompleted("blob_completed_retry");
            await Execute(db, """
                CREATE TRIGGER fail_completed_ack BEFORE UPDATE OF status ON teaql_blob_catalog
                  WHEN NEW.status='deleted' AND OLD.key='blob_completed_retry'
                  BEGIN SELECT RAISE(ABORT,'receipt failure'); END
                """);
            var failed = await service.RunBatchAsync(actor);
            Assert.Contains(failed.Outcomes, item => item.Reference.Key == "blob_completed_retry"
                && item.Status == "failed");
            Assert.Equal("deleting", await Status(db, "blob_completed_retry"));
            Assert.Null(await storage.HeadAsync(retryKey));
            await Execute(db, "DROP TRIGGER fail_completed_ack");
            var recovered = await service.RunBatchAsync(actor);
            Assert.Contains(recovered.Outcomes, item => item.Reference.Key == "blob_completed_retry"
                && item.Status == "already_missing");
            Assert.Equal("deleted", await Status(db, "blob_completed_retry"));

            var (racedKey, _) = await SeedCompleted("blob_completed_race");
            var calls = 0;
            var raceAuthority = new Authority {
                OnMaintainAsync = async () => {
                    if (++calls == 2)
                        await Execute(db, """
                            INSERT INTO teaql_blob_binding(scope,entity,entity_id,field,key,version)
                            VALUES('tenant-a','Asset','9','document','blob_completed_race',1)
                            """);
                }
            };
            var raceService = new SqliteBlobMaintenanceService(catalog, raceAuthority, 100,
                () => DateTimeOffset.FromUnixTimeMilliseconds(10000));
            var raced = await raceService.RunBatchAsync(actor);
            Assert.Contains(raced.Outcomes, item => item.Reference.Key == "blob_completed_race"
                && item.Status == "not_eligible");
            Assert.Equal("uploaded_unbound", await Status(db, "blob_completed_race"));
            Assert.NotNull(await storage.HeadAsync(racedKey));
        }
        finally { Directory.Delete(parent, true); }
    }

    private sealed class Authority : IBlobMaintenanceAuthorization
    {
        public string CurrentScope = "tenant-a";
        public bool Allowed = true;
        public Func<Task>? OnMaintainAsync;
        public string Scope(UserContext actor) => CurrentScope;
        public async Task<bool> CanMaintainAsync(UserContext actor, string scope)
        {
            if (OnMaintainAsync != null) await OnMaintainAsync();
            return Allowed;
        }
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
        Assert.Equal(2, store.Deletes); // stale pinned version is refused by conditional deletion
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
