using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core.Storage;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class BlobFileGenerationSweepSqliteTests
{
    private sealed class Authority : IBlobMaintenanceAuthorization
    {
        public string CurrentScope = "tenant-a";
        public bool Allowed = true;
        public int Calls;
        public string Scope(UserContext actor) => CurrentScope;
        public Task<bool> CanMaintainAsync(UserContext actor, string scope)
        {
            if (++Calls == 2) Allowed = false;
            return Task.FromResult(Allowed);
        }
    }

    [Fact]
    public async Task CatalogSweepIncludesDeletedRowsAndPreservesCurrentGeneration()
    {
        var parent = Path.Combine(Path.GetTempPath(), "teaql-catalog-file-sweep-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(parent);
        try
        {
            await using var db = new SqliteConnection("Data Source=" + Path.Combine(parent, "catalog.sqlite"));
            await db.OpenAsync();
            var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
            await catalog.EnsureSchemaAsync();
            await using var provider = new FileObjectStorage(Path.Combine(parent, "private"),
                RandomNumberGenerator.GetBytes(32));
            var actor = new UserContext().WithObjectStorage(provider);
            var storage = actor.ObjectStorage("seed catalog File sweep");
            var authority = new Authority();
            Assert.Throws<InvalidOperationException>(() =>
                new SqliteBlobFileGenerationSweepService(catalog, authority, false, 1));
            var service = new SqliteBlobFileGenerationSweepService(catalog, authority, true, 1);
            var bytes = Encoding.UTF8.GetBytes("same bytes");

            async Task<(ObjectKey Key, string Version)> Seed(string name, string scope,
                string status, string mode)
            {
                var key = new ObjectKey("objects/" + name);
                _ = await storage.PutBytesAsync(key, bytes, new(bytes.Length, ObjectWriteMode.CreateOnly));
                var current = await storage.PutBytesAsync(key, bytes,
                    new(bytes.Length, ObjectWriteMode.Replace));
                await using var command = db.CreateCommand();
                command.CommandText = """
                    INSERT INTO teaql_blob_catalog
                    (key,scope,object_key,content_version,file_name,content_type,size_bytes,status,upload_mode)
                    VALUES (@key,@scope,@object,@version,'file.pdf','application/pdf',@size,@status,@mode)
                    """;
                command.Parameters.AddWithValue("@key", name);
                command.Parameters.AddWithValue("@scope", scope);
                command.Parameters.AddWithValue("@object", key.Value);
                command.Parameters.AddWithValue("@version", current.ETag!);
                command.Parameters.AddWithValue("@size", bytes.Length);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@mode", mode);
                await command.ExecuteNonQueryAsync();
                return (key, current.ETag!);
            }

            var deleted = await Seed("blob_a_deleted", "tenant-a", "deleted", "server_stream");
            var active = await Seed("blob_b_active", "tenant-a", "available", "server_stream");
            var direct = await Seed("blob_c_direct", "tenant-a", "available", "direct");
            var other = await Seed("blob_d_other", "tenant-b", "available", "server_stream");

            var denied = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RunBatchAsync(actor, provider));
            Assert.Equal("BLOB_MAINTENANCE_DENIED", denied.Message);
            authority.Allowed = true;
            var first = await service.RunBatchAsync(actor, provider);
            Assert.Equal("blob_a_deleted", Assert.Single(first.Outcomes).Reference.Key);
            Assert.Equal(1, first.Outcomes[0].RemovedGenerations);
            Assert.NotNull(first.Next);
            var second = await service.RunBatchAsync(actor, provider, first.Next);
            Assert.Equal("blob_b_active", Assert.Single(second.Outcomes).Reference.Key);
            Assert.Equal(1, second.Outcomes[0].RemovedGenerations);
            Assert.Equal(active.Version, (await storage.HeadAsync(active.Key))?.ETag);
            Assert.Empty((await service.RunBatchAsync(actor, provider, second.Next)).Outcomes);
            Assert.Equal(0, await provider.SweepOldGenerationsAsync(deleted.Key));
            Assert.Equal(1, await provider.SweepOldGenerationsAsync(direct.Key));
            Assert.Equal(1, await provider.SweepOldGenerationsAsync(other.Key));

            await using var wrong = new FileObjectStorage(Path.Combine(parent, "wrong"),
                RandomNumberGenerator.GetBytes(32));
            var mismatch = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RunBatchAsync(actor, wrong));
            Assert.Equal("BLOB_CAPABILITY_MISSING", mismatch.Message);
            authority.CurrentScope = "tenant-b";
            var cursorMismatch = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.RunBatchAsync(actor, provider, first.Next));
            Assert.Equal("BLOB_MAINTENANCE_DENIED", cursorMismatch.Message);
        }
        finally { Directory.Delete(parent, true); }
    }
}
