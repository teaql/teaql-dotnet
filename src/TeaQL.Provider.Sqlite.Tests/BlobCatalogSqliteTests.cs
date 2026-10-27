using Microsoft.Data.Sqlite;
using System.Net;
using System.Security.Cryptography;
using TeaQL.BlobReference;
using TeaQL.Core;
using TeaQL.Core.Storage;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Runtime.Storage;
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

    private sealed class ReadAuthorization(bool allow) : IBlobReadAuthorization
    {
        public Task<bool> AuthorizeAsync(UserContext context, AuthorizedBlobSlot slot) =>
            Task.FromResult(allow);
    }

    private sealed class GraphAuthorizer(AuthorizedResponseGraph graph) : IResponseGraphAuthorizer
    {
        public Task<AuthorizedResponseGraph> AuthorizeAsync(UserContext context, object? result,
            ViewOptions options) => Task.FromResult(graph);
    }

    private sealed class SignedStore : IObjectStorageProvider
    {
        public int Signed;
        public string? Disposition;
        public string? Key;
        public Task<ObjectReadUrl> GetReadUrlAsync(StorageInvocation invocation, ObjectKey key,
            ObjectReadUrlOptions options)
        {
            Signed++;
            Key = key.Value;
            Disposition = options.ContentDisposition;
            return Task.FromResult(new ObjectReadUrl(new Uri("https://example.invalid/opaque-ticket"),
                DateTimeOffset.UtcNow.AddSeconds(30)));
        }
        public Task<ObjectMetadata> PutAsync(StorageInvocation invocation, ObjectKey key,
            Stream content, ObjectPutOptions options) => throw new NotSupportedException();
        public Task<ObjectContent> GetAsync(StorageInvocation invocation, ObjectKey key) =>
            throw new NotSupportedException();
        public Task<ObjectMetadata?> HeadAsync(StorageInvocation invocation, ObjectKey key) =>
            throw new NotSupportedException();
        public Task DeleteAsync(StorageInvocation invocation, ObjectKey key) =>
            throw new NotSupportedException();
    }

    private static readonly BlobBindingTarget Target = new("Asset", "1", "file");
    private static readonly BlobRef Reference = new("blob_abc");

    private sealed class ModuleUploadAuthorization : IBlobUploadAuthorization
    {
        public string Scope(UserContext _) => "tenant-a";
        public BlobFieldSpec Field(UserContext _, BlobUploadTarget target) =>
            new(target.Entity, target.Field, ["pdf"], true, "default");
        public Task<bool> AuthorizeAsync(UserContext _, BlobUploadTarget __, string ___,
            BlobUploadInput ____) => Task.FromResult(true);
    }
    private sealed class ModuleVerifier : IBlobContentVerifier
    {
        public Task<BlobVerifiedContent> VerifyAsync(Stream _, string __, long ___) =>
            throw new NotSupportedException("No upload occurs in the readiness test");
    }

    [Fact]
    public async Task BlobModuleInstallsPassivelyAndRequiresProviderAtExplicitReadiness()
    {
        var (db, actor, catalog) = await Fixture();
        await using (db)
        {
            var upload = new SqliteBlobUploadService(catalog, new ModuleUploadAuthorization(),
                new ModuleVerifier(), 10_000);
            var module = new BlobReferenceModule(catalog, upload, new ReadAuthorization(true),
                new Authorization("tenant-a"));
            module.Install(actor);
            Assert.Same(upload, actor.GetResource<SqliteBlobUploadService>());
            Assert.Same(module.Read, actor.GetResource<IBlobResponseContributor>());
            Assert.Equal("BLOB_CAPABILITY_MISSING", (await Assert.ThrowsAsync<InvalidOperationException>(
                () => module.ReadyAsync(actor))).Message);
            actor.WithObjectStorage(new SignedStore());
            await module.ReadyAsync(actor);
            Assert.Equal("BLOB_CAPABILITY_MISSING", Assert.Throws<InvalidOperationException>(
                () => module.Install(actor)).Message);
        }
    }

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
            VALUES ('blob_abc', 'tenant-a', 'objects/abc', 'v1', 'report.pdf', 'application/pdf', 7, 'uploaded_unbound')
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

    private static async Task<string> CatalogStatus(SqliteConnection db)
    {
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT status FROM teaql_blob_catalog WHERE key = 'blob_abc'";
        return (string)(await command.ExecuteScalarAsync())!;
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
        Assert.Equal("available", await CatalogStatus(db));

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
        Assert.Equal("available", await CatalogStatus(db));
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
        Assert.Equal("uploaded_unbound", await CatalogStatus(db));
        await actor.ExecuteGraphSaveAsync("rebind", async graph =>
        {
            Assert.Equal(3, await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(Target, Reference, 2, 3, "Update"), allowed));
            return true;
        });
        Assert.Equal("available", await CatalogStatus(db));
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT key, version FROM teaql_blob_binding WHERE scope = 'tenant-a'";
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal(Reference.Key, reader.GetString(0));
        Assert.Equal(3, reader.GetInt64(1));
        Assert.False(await reader.ReadAsync());
    }

    [Fact]
    public async Task FinalUnbindAloneReturnsSharedBlobToUnbound()
    {
        var (db, actor, catalog) = await Fixture(); await using var lifetime = db;
        var allowed = new Authorization("tenant-a");
        var second = new BlobBindingTarget("Asset", "2", "file");
        await actor.ExecuteGraphSaveAsync("bind two targets", async graph =>
        {
            await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(Target, Reference, 0, 1, "Create"), allowed);
            await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(second, Reference, 0, 1, "Create"), allowed);
            return true;
        });
        Assert.Equal("available", await CatalogStatus(db));
        await actor.ExecuteGraphSaveAsync("clear first", async graph =>
        {
            await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(Target, null, 1, 2, "Delete"), allowed);
            return true;
        });
        Assert.Equal("available", await CatalogStatus(db));
        await actor.ExecuteGraphSaveAsync("clear final", async graph =>
        {
            await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(second, null, 1, 2, "Delete"), allowed);
            return true;
        });
        Assert.Equal("uploaded_unbound", await CatalogStatus(db));
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

    [Fact]
    public async Task AuthorizedViewRereadsCurrentBindingBeforeSigning()
    {
        var (db, actor, catalog) = await Fixture(); await using var lifetime = db;
        await actor.ExecuteGraphSaveAsync("bind", async graph =>
        {
            await catalog.BindInGraphAsync(graph, actor,
                new BlobBindingChange(Target, Reference, 0, 1, "Create"),
                new Authorization("tenant-a"));
            return true;
        });
        var slot = new AuthorizedBlobSlot(["document"], "Asset", "1", "file",
            "tenant-a", 1, Reference);
        var record = await catalog.CurrentReadAsync(slot);
        Assert.Equal("objects/abc", record.ObjectKey);
        Assert.Equal("v1", record.ContentVersion);
        Assert.Equal(7, record.SizeBytes);
        foreach (var invalid in new[] {
            slot with { BindingVersion = 2 },
            slot with { Scope = "tenant-b" },
            slot with { Reference = new BlobRef("blob_other") }
        })
        {
            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => catalog.CurrentReadAsync(invalid));
            Assert.Equal("BLOB_READ_DENIED", error.Message);
        }
        var store = new SignedStore();
        actor.WithObjectStorage(store);
        actor.InsertResource<IBlobResponseContributor>(new SqlBlobResponseContributor(catalog,
            new ReadAuthorization(true)));
        actor.InsertResource<IResponseGraphAuthorizer>(new GraphAuthorizer(
            new AuthorizedResponseGraph(new Dictionary<string, object?> { ["document"] = Reference }, [slot])));
        var view = await new ViewPreparationService().PrepareAsync(actor, null,
            new ViewOptions("ordinary", "read_only", DateTimeOffset.UtcNow.AddSeconds(60)));
        var presentation = Assert.IsType<Dictionary<string, object?>>(view.Presentation);
        Assert.Equal("https://example.invalid/opaque-ticket", presentation["document"]);
        Assert.Equal("objects/abc", store.Key);
        Assert.Equal("inline; filename=\"report.pdf\"", store.Disposition);
        Assert.Equal(1, store.Signed);

        actor.InsertResource<IBlobResponseContributor>(new SqlBlobResponseContributor(catalog,
            new ReadAuthorization(false)));
        var denied = await Assert.ThrowsAsync<ViewPreparationException>(() =>
            new ViewPreparationService().PrepareAsync(actor, null,
                new ViewOptions("ordinary", "read_only", DateTimeOffset.UtcNow.AddSeconds(60))));
        Assert.Equal("BLOB_READ_DENIED", denied.Code);
        Assert.Equal(1, store.Signed);

        await using var command = db.CreateCommand();
        command.CommandText = "UPDATE teaql_blob_catalog SET status = 'deleted' WHERE key = 'blob_abc'";
        await command.ExecuteNonQueryAsync();
        var stale = await Assert.ThrowsAsync<InvalidOperationException>(() => catalog.CurrentReadAsync(slot));
        Assert.Equal("BLOB_READ_DENIED", stale.Message);
    }

    [Fact]
    public async Task AuthorizedViewProjectsARealFileUrlAndDeniesRefreshAfterRevocation()
    {
        var (db, actor, catalog) = await Fixture(); await using var lifetime = db;
        var parent = Path.Combine(Path.GetTempPath(), "teaql-blob-view-file-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(parent);
        try
        {
            await using var store = new FileObjectStorage(Path.Combine(parent, "private"), RandomNumberGenerator.GetBytes(32));
            store.Start();
            var bytes = "%PDF-1x"u8.ToArray();
            actor.WithObjectStorage(store);
            await actor.ObjectStorage("put current Blob").PutBytesAsync(new ObjectKey("objects/abc"), bytes,
                new(bytes.Length, ObjectWriteMode.CreateOnly, "application/pdf"));
            await actor.ExecuteGraphSaveAsync("bind", async graph =>
            {
                await catalog.BindInGraphAsync(graph, actor,
                    new BlobBindingChange(Target, Reference, 0, 1, "Create"), new Authorization("tenant-a"));
                return true;
            });
            var slot = new AuthorizedBlobSlot(["document"], "Asset", "1", "file", "tenant-a", 1, Reference);
            actor.InsertResource<IBlobResponseContributor>(new SqlBlobResponseContributor(catalog, new ReadAuthorization(true)));
            actor.InsertResource<IResponseGraphAuthorizer>(new GraphAuthorizer(
                new AuthorizedResponseGraph(new Dictionary<string, object?> { ["document"] = Reference }, [slot])));
            var options = new ViewOptions("ordinary", "read_only", DateTimeOffset.UtcNow.AddSeconds(60));
            var view = await new ViewPreparationService().PrepareAsync(actor, null, options);
            var link = Assert.IsType<string>(Assert.IsType<Dictionary<string, object?>>(view.Presentation)["document"]);
            Assert.DoesNotContain("objects/abc", link);
            using var http = new HttpClient(new HttpClientHandler { UseProxy = false });
            using (var response = await http.GetAsync(link))
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(bytes, await response.Content.ReadAsByteArrayAsync());
                Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
                Assert.Equal("inline; filename=\"report.pdf\"", response.Content.Headers.ContentDisposition?.ToString());
            }
            actor.InsertResource<IBlobResponseContributor>(new SqlBlobResponseContributor(catalog, new ReadAuthorization(false)));
            var denied = await Assert.ThrowsAsync<ViewPreparationException>(() =>
                new ViewPreparationService().PrepareAsync(actor, null, options));
            Assert.Equal("BLOB_READ_DENIED", denied.Code);
        }
        finally { Directory.Delete(parent, true); }
    }
}
