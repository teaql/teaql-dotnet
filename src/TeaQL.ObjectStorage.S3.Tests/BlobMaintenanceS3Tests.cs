using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core.Storage;
using TeaQL.ObjectStorage.S3;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using Xunit;

namespace TeaQL.Tests.ObjectStorageS3;

public class BlobMaintenanceS3Tests
{
    private sealed class Authority : IBlobMaintenanceAuthorization
    {
        public string Scope(UserContext actor) => "tenant-a";
        public Task<bool> CanMaintainAsync(UserContext actor, string scope) => Task.FromResult(scope == "tenant-a");
    }

    [Fact]
    public async Task RealS3CompletedRetentionDeletesOnlyEligibleServerStreamObjects()
    {
        if (Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BLOB_MAINTENANCE") != "1") return;
        var endpoint = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ENDPOINT");
        var bucket = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BUCKET");
        var access = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ACCESS_KEY");
        var secret = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_SECRET_KEY");
        Assert.All(new[] { endpoint, bucket, access, secret }, value => Assert.False(string.IsNullOrEmpty(value)));
        using var provider = new S3ObjectStorage(new AmazonS3Client(
            new BasicAWSCredentials(access!, secret!), new AmazonS3Config {
                ServiceURL = endpoint, AuthenticationRegion = "us-east-1", ForcePathStyle = true
            }), bucket!);
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
        await catalog.EnsureSchemaAsync();
        var actor = new UserContext().WithObjectStorage(provider);
        var storage = actor.ObjectStorage("real S3 completed retention");
        var service = new SqliteBlobMaintenanceService(catalog, new Authority(), 10,
            () => DateTimeOffset.FromUnixTimeMilliseconds(10000));
        var prefix = Guid.NewGuid().ToString("N");
        var keys = new List<ObjectKey>();

        async Task<(string Reference, ObjectKey Key)> Seed(string suffix, string status,
            long? lastUnbound = null, long? floor = null, string? profile = "default",
            string mode = "server_stream", bool bound = false)
        {
            var reference = "blob_" + prefix + "_" + suffix;
            var key = new ObjectKey("f4/dotnet/completed-retention/" + prefix + "/" + suffix);
            keys.Add(key);
            var bytes = System.Text.Encoding.UTF8.GetBytes("%PDF-1.7\n" + suffix + "\n%%EOF");
            var created = await storage.PutBytesAsync(key, bytes,
                new ObjectPutOptions(bytes.Length, ObjectWriteMode.CreateOnly));
            await using var command = db.CreateCommand();
            command.CommandText = """
                INSERT INTO teaql_blob_catalog
                  (key,scope,object_key,content_version,file_name,content_type,size_bytes,
                   completed_at_ms,last_unbound_at_ms,policy_profile,upload_mode,
                   unbound_retention_seconds,effective_unref_seconds,retention_floor_until_ms,status)
                  VALUES (@key,'tenant-a',@object,@etag,'file.pdf','application/pdf',@size,
                    9000,@last,@profile,@mode,1,2,@floor,@status);
                INSERT INTO teaql_blob_upload
                  (upload_id,scope,replay_key,request_hash,key,entity,entity_id,field,file_name,
                   extension,declared_size,expires_ms,state,written_etag)
                  VALUES (@id,'tenant-a',@replay,@hash,@key,'Asset','1','document','file.pdf',
                    'pdf',@size,9000,'completed',@etag)
                """;
            command.Parameters.AddWithValue("@key", reference);
            command.Parameters.AddWithValue("@object", key.Value);
            command.Parameters.AddWithValue("@etag", created.ETag!);
            command.Parameters.AddWithValue("@size", bytes.Length);
            command.Parameters.AddWithValue("@last", (object?)lastUnbound ?? DBNull.Value);
            command.Parameters.AddWithValue("@floor", (object?)floor ?? DBNull.Value);
            command.Parameters.AddWithValue("@profile", (object?)profile ?? DBNull.Value);
            command.Parameters.AddWithValue("@mode", mode);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", "bu_" + prefix + "_" + suffix);
            command.Parameters.AddWithValue("@replay", "replay_" + prefix + "_" + suffix);
            command.Parameters.AddWithValue("@hash", "hash_" + prefix + "_" + suffix);
            await command.ExecuteNonQueryAsync();
            if (bound)
            {
                await using var binding = db.CreateCommand();
                binding.CommandText = """
                    INSERT INTO teaql_blob_binding(scope,entity,entity_id,field,key,version)
                    VALUES ('tenant-a','Asset',@id,'document',@key,1)
                    """;
                binding.Parameters.AddWithValue("@id", prefix + "_" + suffix);
                binding.Parameters.AddWithValue("@key", reference);
                await binding.ExecuteNonQueryAsync();
            }
            return (reference, key);
        }

        async Task<string> Status(string reference)
        {
            await using var command = db.CreateCommand();
            command.CommandText = "SELECT status FROM teaql_blob_catalog WHERE key=@key";
            command.Parameters.AddWithValue("@key", reference);
            return (string)(await command.ExecuteScalarAsync())!;
        }

        try
        {
            var expired = await Seed("expired", "uploaded_unbound");
            var former = await Seed("former", "available", lastUnbound: 7000, floor: 10000);
            var active = await Seed("active", "available", bound: true);
            var floored = await Seed("floored", "available", lastUnbound: 7000, floor: 20000);
            var legacy = await Seed("legacy", "uploaded_unbound", profile: null);
            var direct = await Seed("direct", "uploaded_unbound", mode: "direct");

            var batch = await service.RunBatchAsync(actor);
            Assert.Equal(new[] { expired.Reference, former.Reference },
                batch.Outcomes.Select(outcome => outcome.Reference.Key));
            Assert.All(batch.Outcomes, outcome => Assert.Equal("deleted", outcome.Status));
            foreach (var item in new[] { expired, former })
            {
                Assert.Null(await storage.HeadAsync(item.Key));
                Assert.Equal("deleted", await Status(item.Reference));
            }
            foreach (var item in new[] { active, floored, legacy, direct })
            {
                Assert.NotNull(await storage.HeadAsync(item.Key));
                Assert.NotEqual("deleted", await Status(item.Reference));
            }
        }
        finally
        {
            foreach (var key in keys) await storage.DeleteAsync(key);
        }
    }

    [Fact]
    public async Task RealS3ExpiredUploadDeleteRecoveryAndStaleVersion()
    {
        if (Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BLOB_MAINTENANCE") != "1") return;
        var endpoint = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ENDPOINT");
        var bucket = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_BUCKET");
        var access = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_ACCESS_KEY");
        var secret = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_SECRET_KEY");
        Assert.All(new[] { endpoint, bucket, access, secret }, value => Assert.False(string.IsNullOrEmpty(value)));
        using var provider = new S3ObjectStorage(new AmazonS3Client(
            new BasicAWSCredentials(access!, secret!), new AmazonS3Config {
                ServiceURL = endpoint, AuthenticationRegion = "us-east-1", ForcePathStyle = true
            }), bucket!);
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
        await catalog.EnsureSchemaAsync();
        var actor = new UserContext().WithObjectStorage(provider);
        var storage = actor.ObjectStorage("real S3 expired Blob maintenance");
        var service = new SqliteBlobMaintenanceService(catalog, new Authority(), 10);
        var prefix = Guid.NewGuid().ToString("N");
        var keys = new List<ObjectKey>();

        async Task<(string Reference, ObjectKey Key)> Seed(string suffix, byte[] bytes)
        {
            var reference = "blob_" + prefix + "_" + suffix;
            var key = new ObjectKey("f4/dotnet/maintenance/" + prefix + "/" + suffix);
            keys.Add(key);
            var created = await storage.PutBytesAsync(key, bytes,
                new ObjectPutOptions(bytes.Length, ObjectWriteMode.CreateOnly));
            await using var command = db.CreateCommand();
            command.CommandText = """
                INSERT INTO teaql_blob_catalog
                  (key,scope,object_key,content_version,file_name,content_type,size_bytes,status)
                  VALUES (@key,'tenant-a',@object,'','file.pdf','',0,'initiated');
                INSERT INTO teaql_blob_upload
                  (upload_id,scope,replay_key,request_hash,key,entity,entity_id,field,file_name,
                   extension,declared_size,expires_ms,state,written_etag)
                  VALUES (@id,'tenant-a',@replay,@hash,@key,'Asset','1','document','file.pdf',
                    'pdf',@size,@expiry,'written',@etag)
                """;
            command.Parameters.AddWithValue("@key", reference);
            command.Parameters.AddWithValue("@object", key.Value);
            command.Parameters.AddWithValue("@id", "bu_" + suffix);
            command.Parameters.AddWithValue("@replay", "replay_" + suffix);
            command.Parameters.AddWithValue("@hash", "hash_" + suffix);
            command.Parameters.AddWithValue("@size", bytes.Length);
            command.Parameters.AddWithValue("@expiry", DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeMilliseconds());
            command.Parameters.AddWithValue("@etag", created.ETag!);
            await command.ExecuteNonQueryAsync();
            return (reference, key);
        }

        async Task<string> Status(string reference)
        {
            await using var command = db.CreateCommand();
            command.CommandText = "SELECT status FROM teaql_blob_catalog WHERE key=@key";
            command.Parameters.AddWithValue("@key", reference);
            return (string)(await command.ExecuteScalarAsync())!;
        }

        try
        {
            var (firstRef, firstKey) = await Seed("first", "first version"u8.ToArray());
            var first = await service.RunBatchAsync(actor);
            Assert.Equal(firstRef, Assert.Single(first.Outcomes).Reference.Key);
            Assert.Equal("deleted", first.Outcomes[0].Status);
            Assert.Null(await storage.HeadAsync(firstKey));
            Assert.Equal("deleted", await Status(firstRef));

            var (recoveryRef, recoveryKey) = await Seed("recovery", "recovery version"u8.ToArray());
            await using (var trigger = db.CreateCommand())
            {
                trigger.CommandText = $"""
                    CREATE TRIGGER fail_final_ack BEFORE UPDATE OF status ON teaql_blob_catalog
                      WHEN NEW.status='deleted' AND OLD.key='{recoveryRef}'
                      BEGIN SELECT RAISE(ABORT,'private trigger detail'); END
                    """;
                await trigger.ExecuteNonQueryAsync();
            }
            var failed = await service.RunBatchAsync(actor);
            Assert.Equal("failed", Assert.Single(failed.Outcomes).Status);
            Assert.Equal("BLOB_CATALOG_FAILED", failed.Outcomes[0].Error);
            Assert.Null(await storage.HeadAsync(recoveryKey));
            Assert.Equal("deleting", await Status(recoveryRef));
            await using (var drop = db.CreateCommand())
            {
                drop.CommandText = "DROP TRIGGER fail_final_ack";
                await drop.ExecuteNonQueryAsync();
            }
            var recovered = await service.RunBatchAsync(actor);
            Assert.Equal("already_missing", Assert.Single(recovered.Outcomes).Status);
            Assert.Equal("deleted", await Status(recoveryRef));

            var (staleRef, staleKey) = await Seed("stale", "old version"u8.ToArray());
            var replacementBytes = "replacement version"u8.ToArray();
            var replacement = await storage.PutBytesAsync(staleKey, replacementBytes,
                new ObjectPutOptions(replacementBytes.Length, ObjectWriteMode.Replace));
            var conflict = await service.RunBatchAsync(actor);
            Assert.Equal("version_conflict", Assert.Single(conflict.Outcomes).Status);
            Assert.Equal(replacement.ETag, (await storage.HeadAsync(staleKey))?.ETag);
            Assert.Equal("deleting", await Status(staleRef));
        }
        finally
        {
            foreach (var key in keys) await storage.DeleteAsync(key);
        }
        var marker = Environment.GetEnvironmentVariable("TEAQL_S3_TEST_GATE_MARKER");
        if (!string.IsNullOrEmpty(marker)) File.WriteAllText(marker, "expired-upload-maintenance-real-gate-passed");
    }
}
