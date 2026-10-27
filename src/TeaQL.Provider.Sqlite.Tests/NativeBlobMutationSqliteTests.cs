using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public sealed class NativeBlobMutationSqliteTests
{
    private sealed class BindingPermission : IBlobBindingAuthorization, IBlobBindingPolicyAuthorization
    {
        public string CurrentScope = "tenant-a";
        public bool Allowed = true;
        public string Scope(UserContext actor) => CurrentScope;
        public Task<bool> AuthorizeAsync(UserContext actor, BlobBindingTarget target,
            BlobRef? previous, BlobRef? next, string action) => Task.FromResult(Allowed);
        public Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor,
            IPhysicalSqlLease lease, BlobBindingTarget target) =>
            Task.FromResult(new BlobRetentionPolicy("default", "server_stream", 86400, 604800, 3600));
    }

    private sealed class ReadPermission : IBlobReadAuthorization
    {
        public Task<bool> AuthorizeAsync(UserContext actor, AuthorizedBlobSlot slot) => Task.FromResult(true);
    }

    private sealed class UploadPermission : IBlobUploadAuthorization
    {
        public string Scope(UserContext actor) => "tenant-a";
        public BlobFieldSpec Field(UserContext actor, BlobUploadTarget target) =>
            new("Asset", "document", ["pdf"], true, "default");
        public Task<bool> AuthorizeAsync(UserContext actor, BlobUploadTarget target, string action,
            BlobUploadInput input) => Task.FromResult(true);
        public Task<BlobUploadAuthorizationState> CurrentOnLeaseAsync(UserContext actor,
            BlobUploadTarget target, string action, BlobUploadInput input, ISqlTransport lease) =>
            Task.FromResult(new BlobUploadAuthorizationState(Scope(actor), Field(actor, target), true));
    }

    private sealed class UnusedVerifier : IBlobContentVerifier
    {
        public Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
            long maximumBytes) => throw new NotSupportedException("No upload in this transaction test");
    }

    [Fact]
    public async Task ModeledGraphMutationAutomaticallyBindsAndPoisonsCaughtDenial()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var spec = new BlobFieldSpec("Asset", "document", ["pdf"], true, "default");
        var asset = EntityDescriptor.New("Asset").TableName("native_asset")
            .Property(PropertyDescriptor.New("id", DataType.U64).Id().NotNull())
            .Property(PropertyDescriptor.New("document", DataType.Text).Blob(spec));
        var control = EntityDescriptor.New("Control").TableName("native_control")
            .Property(PropertyDescriptor.New("id", DataType.U64).Id().NotNull());
        var transport = new SqliteTransport(db);
        var actor = new RuntimeModule().Entity(asset).Entity(control).IntoContext()
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport,
                new MetadataSchemaProvider(name => name == "Asset" ? asset :
                    name == "Control" ? control : null)));
        await actor.EnsureSchemaAsync();
        var catalog = new SqliteBlobCatalog(transport);
        await catalog.EnsureSchemaAsync();
        var permission = new BindingPermission();
        var upload = new SqliteBlobUploadService(catalog, new UploadPermission(),
            new UnusedVerifier(), 1024);
        var module = new BlobReferenceModule(catalog, upload, new ReadPermission(), permission);
        var root = Path.Combine(Path.GetTempPath(), "teaql-dotnet-native-blob-" + Guid.NewGuid());
        Directory.CreateDirectory(root);
        try
        {
            await using var store = new FileObjectStorage(Path.Combine(root, "private"),
                Enumerable.Repeat((byte)0x42, 32).ToArray());
            actor.WithObjectStorage(store);
            module.Install(actor);
            await module.ReadyAsync(actor);
            var first = new BlobRef("blob_native_first");
            var second = new BlobRef("blob_native_second");
            foreach (var reference in new[] { first, second })
            {
                await using var insert = db.CreateCommand();
                insert.CommandText = """
                    INSERT INTO teaql_blob_catalog
                    (key,scope,object_key,content_version,file_name,content_type,size_bytes,status,
                     completed_at_ms,policy_profile,upload_mode,unbound_retention_seconds,effective_unref_seconds)
                    VALUES (@key,'tenant-a',@object,'v1','report.pdf','application/pdf',7,
                      'uploaded_unbound',CAST(strftime('%s','now') AS INTEGER)*1000,
                      'default','server_stream',86400,604800)
                    """;
                insert.Parameters.AddWithValue("@key", reference.Key);
                insert.Parameters.AddWithValue("@object", "objects/" + reference.Key);
                await insert.ExecuteNonQueryAsync();
            }

            await actor.ExecuteGraphSaveAsync("create Blob", async graph =>
            {
                await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Asset")
                    .Value("id", 1UL).Value("document", first), "create Blob"));
                return true;
            });
            await actor.ExecuteGraphSaveAsync("rebind Blob", async graph =>
            {
                await graph.MutateAsync(new UpdateMutationRequest(new UpdateCommand("Asset",
                    new Value.U64Value(1)).Value("document", second), "rebind Blob"));
                return true;
            });
            await actor.ExecuteGraphSaveAsync("clear Blob", async graph =>
            {
                await graph.MutateAsync(new UpdateMutationRequest(new UpdateCommand("Asset",
                    new Value.U64Value(1)).Value("document", new Value.NullValue()), "clear Blob"));
                return true;
            });
            Assert.Equal((null, 3L), await Binding(db, "1"));
            Assert.Null(await Scalar(db, "SELECT document FROM native_asset WHERE id=1"));

            permission.Allowed = false;
            var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                actor.ExecuteGraphSaveAsync("caught denial", async graph =>
                {
                    await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Control")
                        .Value("id", 9UL), "caught denial"));
                    try
                    {
                        await graph.MutateAsync(new UpdateMutationRequest(new UpdateCommand("Asset",
                            new Value.U64Value(1)).Value("document", first), "caught denial"));
                    }
                    catch (InvalidOperationException) { }
                    return true;
                }));
            Assert.Equal("GRAPH_TRANSACTION_ROLLBACK_ONLY", rejected.Message);
            Assert.Equal(0L, await Scalar(db, "SELECT COUNT(*) FROM native_control"));
            Assert.Equal((null, 3L), await Binding(db, "1"));
            permission.Allowed = true;

            await Exec(db, "INSERT INTO native_asset(id,document) VALUES (7,NULL)");
            var missing = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                actor.ExecuteGraphSaveAsync("missing old binding", async graph =>
                {
                    await graph.MutateAsync(new UpdateMutationRequest(new UpdateCommand("Asset",
                        new Value.U64Value(7)).Value("document", first), "missing old binding"));
                    return true;
                }));
            Assert.Equal("BLOB_BIND_DENIED", missing.Message);
            permission.CurrentScope = "tenant-b";
            var crossScope = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                actor.ExecuteGraphSaveAsync("cross scope", async graph =>
                {
                    await graph.MutateAsync(new UpdateMutationRequest(new UpdateCommand("Asset",
                        new Value.U64Value(1)).Value("document", first), "cross scope"));
                    return true;
                }));
            Assert.Equal("BLOB_BIND_DENIED", crossScope.Message);
            permission.CurrentScope = "tenant-a";

            await actor.ExecuteGraphSaveAsync("hard delete Blob", async graph =>
            {
                await graph.MutateAsync(new DeleteMutationRequest(new DeleteCommand("Asset",
                    new Value.U64Value(1)).HardDelete(), "hard delete Blob"));
                return true;
            });
            Assert.Equal((null, 4L), await Binding(db, "1"));
            Assert.Equal(0L, await Scalar(db, "SELECT COUNT(*) FROM native_asset WHERE id=1"));
            await Assert.ThrowsAnyAsync<Exception>(() => actor.ExecuteGraphSaveAsync(
                "reject raw Key", async graph =>
                {
                    await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Asset")
                        .Value("id", 2UL).Value("document", first.Key), "reject raw Key"));
                    return true;
                }));
            Assert.Equal(0L, await Scalar(db, "SELECT COUNT(*) FROM native_asset WHERE id=2"));

            var alien = new RuntimeModule().Entity(asset).Entity(control).IntoContext()
                .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(),
                    new SqliteTransport(db), new MetadataSchemaProvider(name =>
                        name == "Asset" ? asset : name == "Control" ? control : null)))
                .WithObjectStorage(store);
            module.Install(alien);
            var readiness = await Assert.ThrowsAsync<InvalidOperationException>(
                () => module.ReadyAsync(alien));
            Assert.Equal("BLOB_CAPABILITY_MISSING", readiness.Message);
            var wrongOwner = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                alien.ExecuteGraphSaveAsync("reject different transport owner", async graph =>
                {
                    await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Asset")
                        .Value("id", 3UL).Value("document", first),
                        "reject different transport owner"));
                    return true;
                }));
            Assert.Equal("BLOB_TRANSACTION_REQUIRED", wrongOwner.Message);
            Assert.Equal(0L, await Scalar(db, "SELECT COUNT(*) FROM native_asset WHERE id=3"));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static async Task Exec(SqliteConnection db, string sql)
    {
        await using var command = db.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<object?> Scalar(SqliteConnection db, string sql)
    {
        await using var command = db.CreateCommand();
        command.CommandText = sql;
        var value = await command.ExecuteScalarAsync();
        return value is DBNull ? null : value;
    }

    private static async Task<(string? Key, long Version)> Binding(SqliteConnection db, string id)
    {
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT key,version FROM teaql_blob_binding WHERE scope='tenant-a' AND entity='Asset' AND entity_id=@id AND field='document'";
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        return (reader.IsDBNull(0) ? null : reader.GetString(0), reader.GetInt64(1));
    }
}
