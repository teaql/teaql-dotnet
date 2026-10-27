using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class BlobCatalogSqliteTests
{
    private sealed class Authorization(string scope, bool allow = true) : IBlobBindingAuthorization
    {
        public string Scope(UserContext context) => scope;
        public Task<bool> AuthorizeAsync(UserContext context, BlobBindingTarget target,
            BlobRef? previous, BlobRef? next, string action) => Task.FromResult(allow);
    }

    private static readonly BlobBindingTarget Target = new("Asset", "1", "file");
    private static readonly BlobRef Reference = new("blob_abc");

    private static async Task<(SqliteConnection Db, UserContext Actor, SqliteBlobCatalog Catalog)> Fixture()
    {
        var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var asset = EntityDescriptor.New("Asset").TableName("asset")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("name", DataType.Text));
        var actor = new RuntimeModule().Entity(asset).IntoContext().WithDataService(
            new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(db),
                new MetadataSchemaProvider(name => name == "Asset" ? asset : null)));
        await actor.EnsureSchemaAsync();
        var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
        await catalog.EnsureSchemaAsync();
        await catalog.ValidateSchemaAsync();
        await using var command = db.CreateCommand();
        command.CommandText = """
            INSERT INTO teaql_blob_catalog
            (key, scope, object_key, content_version, file_name, content_type, size_bytes, status)
            VALUES ('blob_abc', 'tenant-a', 'objects/abc', 'v1', 'report.pdf', 'application/pdf', 7, 'available')
            """;
        await command.ExecuteNonQueryAsync();
        return (db, actor, catalog);
    }

    private static async Task<long> Count(SqliteConnection db, string table)
    {
        await using var command = db.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {table}";
        return (long)(await command.ExecuteScalarAsync())!;
    }

    private static async Task Insert(GraphMutationSession graph, ulong id)
    {
        await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Asset")
            .Value("id", new Value.U64Value(id))
            .Value("name", new Value.TextValue("document")), "save asset"),
            graph.Scope("Asset", id, null));
    }

    [Fact]
    public async Task BindingSharesPhysicalBusinessTransactionAndRejectsRevokedActor()
    {
        var (db, actor, catalog) = await Fixture(); await using var lifetime = db;
        var allowed = new Authorization("tenant-a");
        await actor.ExecuteGraphSaveAsync("save asset", async graph =>
        {
            await Insert(graph, 1);
            Assert.Equal(1, await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(Target, Reference, 0, 1, "Create"), allowed));
            return true;
        });
        Assert.Equal(1, await Count(db, "asset"));
        Assert.Equal(1, await Count(db, "teaql_blob_binding"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => actor.ExecuteGraphSaveAsync(
            "rollback", async graph =>
            {
                await Insert(graph, 2);
                await catalog.BindInGraphAsync(graph, actor,
                    new BlobBindingChange(new BlobBindingTarget("Asset", "2", "file"), Reference,
                        0, 1, "Create"), allowed);
                throw new InvalidOperationException("abort graph");
#pragma warning disable CS0162
                return true;
#pragma warning restore CS0162
            }));
        Assert.Equal(1, await Count(db, "asset"));
        Assert.Equal(1, await Count(db, "teaql_blob_binding"));

        var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() => actor.ExecuteGraphSaveAsync(
            "revoked", async graph =>
            {
                await Insert(graph, 3);
                try { await catalog.BindInGraphAsync(graph, actor,
                    new BlobBindingChange(new BlobBindingTarget("Asset", "3", "file"), Reference,
                        0, 1, "Create"), new Authorization("tenant-a", false)); }
                catch (InvalidOperationException error) { Assert.Equal("BLOB_BIND_DENIED", error.Message); }
                return true;
            }));
        Assert.Equal("GRAPH_TRANSACTION_ROLLBACK_ONLY", rejected.Message);
        Assert.Equal(1, await Count(db, "asset"));
        Assert.Equal(1, await Count(db, "teaql_blob_binding"));
    }

    [Fact]
    public async Task ScopeVersionActorAndTombstoneAreEnforced()
    {
        var (db, actor, catalog) = await Fixture(); await using var lifetime = db;
        var allowed = new Authorization("tenant-a");
        var other = new RuntimeModule().IntoContext();
        await actor.ExecuteGraphSaveAsync("bind", async graph =>
        {
            await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(Target, Reference, 0, 1, "Create"), allowed);
            return true;
        });
        foreach (var (change, auth, who) in new[] {
            (new BlobBindingChange(Target, Reference, 0, 2, "Update"), allowed, actor),
            (new BlobBindingChange(Target, Reference, 1, 2, "Update"),
                new Authorization("tenant-b"), actor),
            (new BlobBindingChange(Target, Reference, 1, 2, "Update"), allowed, other),
            (new BlobBindingChange(new BlobBindingTarget("Asset", "01", "file"), Reference,
                1, 2, "Update"), allowed, actor)
        })
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => actor.ExecuteGraphSaveAsync(
                "reject", async graph =>
                { await catalog.BindInGraphAsync(graph, who, change, auth); return true; }));
        }
        await actor.ExecuteGraphSaveAsync("clear", async graph =>
        {
            Assert.Equal(2, await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(Target, null, 1, 2, "Delete"), allowed));
            return true;
        });
        await actor.ExecuteGraphSaveAsync("rebind", async graph =>
        {
            Assert.Equal(3, await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(Target, Reference, 2, 3, "Update"), allowed));
            return true;
        });
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT key, version FROM teaql_blob_binding WHERE scope = 'tenant-a'";
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal(Reference.Key, reader.GetString(0));
        Assert.Equal(3, reader.GetInt64(1));
        Assert.False(await reader.ReadAsync());
    }

    [Fact]
    public async Task OldNonNullableKeySchemaIsRejected()
    {
        var db = new SqliteConnection("Data Source=:memory:"); await db.OpenAsync();
        await using var lifetime = db;
        var catalog = new SqliteBlobCatalog(new SqliteTransport(db));
        await catalog.EnsureSchemaAsync();
        await using (var command = db.CreateCommand())
        {
            command.CommandText = "DROP TABLE teaql_blob_binding";
            await command.ExecuteNonQueryAsync();
            command.CommandText = "CREATE TABLE teaql_blob_binding (scope TEXT, entity TEXT, entity_id TEXT, field TEXT, key TEXT NOT NULL, version BIGINT NOT NULL)";
            await command.ExecuteNonQueryAsync();
        }
        var error = await Assert.ThrowsAsync<InvalidOperationException>(catalog.ValidateSchemaAsync);
        Assert.Equal("BLOB_SCHEMA_MISSING", error.Message);
    }
}
