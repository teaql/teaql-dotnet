using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core;
using TeaQL.Core.Storage;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class BlobRetentionSqliteTests
{
    private sealed class Authority : IBlobUploadAuthorization, IBlobUploadPolicyAuthorization
    {
        public BlobRetentionPolicy Policy = new("default", "server_stream", 86400, 604800, 3600);
        public int PolicyCalls;
        public string Scope(UserContext _) => "tenant-a";
        public BlobFieldSpec Field(UserContext _, BlobUploadTarget target) =>
            new(target.Entity, target.Field, ["pdf"], true, "default");
        public Task<bool> AuthorizeAsync(UserContext _, BlobUploadTarget target, string action,
            BlobUploadInput input) => Task.FromResult(target.Entity == "Asset" && target.Field == "file");
        public Task<BlobUploadAuthorizationState> CurrentOnLeaseAsync(UserContext actor,
            BlobUploadTarget target, string action, BlobUploadInput input, ISqlTransport lease) =>
            Task.FromResult(new BlobUploadAuthorizationState(Scope(actor), Field(actor, target), true));
        public async Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor,
            ISqlTransport lease, BlobUploadTarget target)
        {
            var rows = await lease.FetchAllSqlAsync(new CompiledQuery("SELECT 1 AS alive", []));
            Assert.Equal(1, Assert.Single(rows)["alive"].TryI64());
            PolicyCalls++;
            return Policy;
        }
    }

    private sealed class Verifier : IBlobContentVerifier
    {
        public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
            long maximumBytes)
        {
            using var output = new MemoryStream();
            await content.CopyToAsync(output);
            var bytes = output.ToArray();
            Assert.Equal("pdf", extension);
            Assert.StartsWith("%PDF-", Encoding.UTF8.GetString(bytes));
            return new(bytes.Length, Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),
                "application/pdf");
        }
    }

    [Fact]
    public void SharedPolicyVectorsKeepExactBytesAndDeadlines()
    {
        var bytes = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory,
            "Fixtures", "blob-policy-cases.json"));
        Assert.Equal("b93cff3f8dabd5690aac5f8f2096633dcd4e158d3aa0424b9a902a16f6d3c2b6",
            Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant());
        using var doc = JsonDocument.Parse(bytes);
        var cases = doc.RootElement.GetProperty("cases").EnumerateArray()
            .ToDictionary(item => item.GetProperty("id").GetString()!);
        var maximum = cases["maximum-retention-across-fields"];
        var input = maximum.GetProperty("input");
        var accepted = input.GetProperty("acceptedRetentionSeconds").EnumerateArray()
            .Select(item => item.GetInt64()).ToArray();
        var seconds = BlobRetentionPolicy.EffectiveRetention(accepted, 0);
        Assert.Equal(maximum.GetProperty("expected")
            .GetProperty("effectiveUnreferencedRetentionSeconds").GetInt64(), seconds);
        Assert.Equal(3_000_000_000, BlobRetentionPolicy.UnreferencedEligibleAt(100_000,
            seconds, 3_000_000_000));
        var expired = cases["unbound-deadline-rejects-bind-before-gc"].GetProperty("input");
        Assert.Equal("BLOB_REFERENCE_EXPIRED", Assert.Throws<InvalidOperationException>(() =>
            BlobRetentionPolicy.RequireBindable("uploaded_unbound",
                expired.GetProperty("nowSeconds").GetInt64() * 1000,
                expired.GetProperty("completedAtSeconds").GetInt64() * 1000,
                expired.GetProperty("unboundRetentionSeconds").GetInt64(), null)).Message);
        Assert.Equal(2592000, BlobRetentionPolicy.EffectiveRetention([2592000], 3600));
    }

    [Fact]
    public async Task PolicySnapshotAndCompletedClockUsePhysicalSQLiteLease()
    {
        var root = Path.Combine(Path.GetTempPath(), "teaql-dotnet-retention-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            await using var db = new SqliteConnection("Data Source=" + Path.Combine(root, "catalog.db"));
            await db.OpenAsync();
            var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
            await catalog.EnsureSchemaAsync();
            await catalog.ValidateSchemaAsync();
            await using var file = new FileObjectStorage(Path.Combine(root, "private"),
                RandomNumberGenerator.GetBytes(32));
            var actor = new UserContext().WithObjectStorage(file);
            var authority = new Authority();
            var clock = DateTimeOffset.FromUnixTimeMilliseconds(1_700_000_000_000);
            var service = new SqliteBlobUploadService(catalog, authority, new Verifier(), 10_000,
                now: () => clock);
            var bytes = Encoding.UTF8.GetBytes("%PDF-1.7\nretention\n%%EOF");
            var target = new BlobUploadTarget("Asset", "1", "file");
            var input = new BlobUploadInput("report.pdf", bytes.Length);
            var session = await service.BeginAsync(actor, target, input, "retention-1");
            Assert.Equal(1, authority.PolicyCalls);
            await using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "SELECT policy_profile,upload_mode,unbound_retention_seconds,"
                    + "effective_unref_seconds,completed_at_ms FROM teaql_blob_catalog WHERE key=@key";
                cmd.Parameters.AddWithValue("@key", session.Reference.Key);
                await using var reader = await cmd.ExecuteReaderAsync();
                Assert.True(await reader.ReadAsync());
                Assert.Equal("default", reader.GetString(0));
                Assert.Equal("server_stream", reader.GetString(1));
                Assert.Equal(86400, reader.GetInt64(2));
                Assert.Equal(604800, reader.GetInt64(3));
                Assert.True(reader.IsDBNull(4));
            }
            authority.Policy = new("shorter", "server_stream", 1, 1, 1);
            Assert.Equal(session.UploadId,
                (await service.BeginAsync(actor, target, input, "retention-1")).UploadId);
            Assert.Equal(1, authority.PolicyCalls);
            await service.WriteAsync(actor, session.UploadId, new MemoryStream(bytes));
            clock = clock.AddSeconds(30);
            Assert.Equal(session.Reference, await service.CompleteAsync(actor, session.UploadId));
            Assert.Equal(session.Reference, await service.CompleteAsync(actor, session.UploadId));
            await using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "SELECT policy_profile,completed_at_ms,status FROM teaql_blob_catalog WHERE key=@key";
                cmd.Parameters.AddWithValue("@key", session.Reference.Key);
                await using var reader = await cmd.ExecuteReaderAsync();
                Assert.True(await reader.ReadAsync());
                Assert.Equal("default", reader.GetString(0));
                Assert.Equal(clock.ToUnixTimeMilliseconds(), reader.GetInt64(1));
                Assert.Equal("uploaded_unbound", reader.GetString(2));
            }
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public async Task AdditiveMigrationLeavesLegacyFactsUnknownAndDirectModeCannotBegin()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        await using (var command = db.CreateCommand())
        {
            command.CommandText = """
                CREATE TABLE teaql_blob_catalog (key TEXT PRIMARY KEY, scope TEXT NOT NULL,
                  object_key TEXT NOT NULL, content_version TEXT NOT NULL,
                  file_name TEXT NOT NULL, content_type TEXT NOT NULL,
                  size_bytes BIGINT NOT NULL, status TEXT NOT NULL);
                INSERT INTO teaql_blob_catalog VALUES
                  ('blob_legacy','tenant-a','objects/legacy','v1','old.pdf','application/pdf',5,'available');
                """;
            await command.ExecuteNonQueryAsync();
        }
        var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
        await catalog.EnsureSchemaAsync();
        await catalog.EnsureSchemaAsync();
        await catalog.ValidateSchemaAsync();
        await using (var command = db.CreateCommand())
        {
            command.CommandText = "SELECT completed_at_ms,last_unbound_at_ms,policy_profile,upload_mode,"
                + "unbound_retention_seconds,effective_unref_seconds,retention_floor_until_ms "
                + "FROM teaql_blob_catalog WHERE key='blob_legacy'";
            await using var reader = await command.ExecuteReaderAsync();
            Assert.True(await reader.ReadAsync());
            for (var i = 0; i < 7; i++) Assert.True(reader.IsDBNull(i));
        }
        var authority = new Authority { Policy = new("default", "direct", 1, 1, 1) };
        var service = new SqliteBlobUploadService(catalog, authority, new Verifier(), 10_000);
        Assert.Equal("BLOB_CAPABILITY_MISSING", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.BeginAsync(new UserContext(), new BlobUploadTarget("Asset", "1", "file"),
                new BlobUploadInput("direct.pdf", 4), "direct-mode"))).Message);
    }
}
