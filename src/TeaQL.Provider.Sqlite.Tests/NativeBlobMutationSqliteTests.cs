using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;
using TeaQL.BlobReference;
using TeaQL.Core;
using TeaQL.Core.Storage;
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
        public BlobRetentionPolicy Policy = new("default", "server_stream", 86400, 604800, 3600);
        public string CurrentScope = "tenant-a";
        public bool Allowed = true;
        public string? DenyEntityId;
        public bool Denied;
        public readonly List<(string EntityId, string Field)> Visited = [];
        public string Scope(UserContext actor) => CurrentScope;
        public Task<bool> AuthorizeAsync(UserContext actor, BlobBindingTarget target,
            BlobRef? previous, BlobRef? next, string action)
        {
            Visited.Add((target.EntityId, target.Field));
            if (target.EntityId == DenyEntityId && target.Field == "legacy_document")
            {
                Denied = true;
                return Task.FromResult(false);
            }
            return Task.FromResult(Allowed);
        }
        public Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor,
            IPhysicalSqlLease lease, BlobBindingTarget target) =>
            Task.FromResult(Policy);
    }

    private sealed class ReadPermission : IBlobReadAuthorization
    {
        public Task<bool> AuthorizeAsync(UserContext actor, AuthorizedBlobSlot slot) => Task.FromResult(true);
    }

    private sealed class UploadPermission : IBlobUploadAuthorization, IBlobUploadPolicyAuthorization
    {
        public BlobRetentionPolicy Policy = new("default", "server_stream", 86400, 604800, 3600);
        public string Scope(UserContext actor) => "tenant-a";
        public BlobFieldSpec Field(UserContext actor, BlobUploadTarget target) =>
            new("Asset", "document", ["pdf"], true, "default");
        public Task<bool> AuthorizeAsync(UserContext actor, BlobUploadTarget target, string action,
            BlobUploadInput input) => Task.FromResult(true);
        public Task<BlobUploadAuthorizationState> CurrentOnLeaseAsync(UserContext actor,
            BlobUploadTarget target, string action, BlobUploadInput input, ISqlTransport lease) =>
            Task.FromResult(new BlobUploadAuthorizationState(Scope(actor), Field(actor, target), true));
        public Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor,
            ISqlTransport lease, BlobUploadTarget target) => Task.FromResult(Policy);
    }

    // Fixture-only full-byte check; production PDF parsing is covered elsewhere.
    private sealed class FixturePdfVerifier : IBlobContentVerifier
    {
        public async Task<BlobVerifiedContent> VerifyAsync(Stream content, string extension,
            long maximumBytes)
        {
            using var output = new MemoryStream();
            await content.CopyToAsync(output);
            var bytes = output.ToArray();
            if (extension != "pdf" || bytes.LongLength > maximumBytes ||
                !Encoding.UTF8.GetString(bytes).StartsWith("%PDF-") ||
                !Encoding.UTF8.GetString(bytes).Contains("%%EOF"))
                throw new InvalidOperationException("invalid test PDF");
            return new BlobVerifiedContent(bytes.LongLength,
                Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(), "application/pdf");
        }
    }

    private sealed class MaintenancePermission : IBlobMaintenanceAuthorization
    {
        public string Scope(UserContext actor) => "tenant-a";
        public Task<bool> CanMaintainAsync(UserContext actor, string scope) => Task.FromResult(true);
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
            var ledgerRef = new BlobRef("blob_native_ledger");
            foreach (var reference in new[] { first, second, ledgerRef })
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

            ulong abortedId = 0;
            async Task<ulong> SaveAllocated(bool abort)
            {
                var ledger = new EntityRoot();
                var provisional = new EntityKey("Asset", -1);
                ledger.MarkAsNew(provisional);
                ledger.Set(provisional, "document", Value.FromObject(ledgerRef));
                return await actor.ExecuteGraphSaveAsync("save allocated ledger Blob", async graph =>
                {
                    var id = await graph.AllocateIdAsync("Asset");
                    var actual = new EntityKey("Asset", new Value.U64Value(id));
                    ledger.Rekey(provisional, actual);
                    Assert.True(ledger.IsNew(actual));
                    Assert.Equal(Value.FromObject(ledgerRef), ledger.Change(actual)["document"]);
                    var request = new InsertMutationRequest(new InsertCommand("Asset")
                        .Value("id", id).Value("document", ledger.Change(actual)["document"]),
                        "save allocated ledger Blob") { LedgerRoot = ledger, LedgerKey = actual };
                    await graph.MutateAsync(request, graph.Scope("Asset", id, null));
                    if (abort)
                    {
                        abortedId = id;
                        throw new InvalidOperationException("outer owner rejected");
                    }
                    return id;
                });
            }
            var outerFailure = await Assert.ThrowsAsync<InvalidOperationException>(
                () => SaveAllocated(true));
            Assert.Equal("outer owner rejected", outerFailure.Message);
            Assert.Equal(0L, await Scalar(db, $"SELECT COUNT(*) FROM native_asset WHERE id={abortedId}"));
            Assert.Equal(0L, await Scalar(db,
                $"SELECT COUNT(*) FROM teaql_blob_binding WHERE entity_id='{abortedId}'"));
            Assert.Equal("uploaded_unbound", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key='blob_native_ledger'"));
            var committedId = await SaveAllocated(false);
            Assert.Equal(ledgerRef.Key, await Scalar(db,
                $"SELECT document FROM native_asset WHERE id={committedId}"));
            Assert.Equal((ledgerRef.Key, 1L), await Binding(db, committedId.ToString()));
            Assert.Equal("available", await Scalar(db,
                "SELECT status FROM teaql_blob_catalog WHERE key='blob_native_ledger'"));

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

    [Fact]
    public async Task MultiEntityBlobGraphRollsBackLateDenialAndOuterRejection()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var asset = EntityDescriptor.New("Asset").TableName("multi_asset")
            .Property(PropertyDescriptor.New("id", DataType.U64).Id().NotNull())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("document", DataType.Text).Blob(
                new BlobFieldSpec("Asset", "document", ["pdf"], true, "default")))
            .Property(PropertyDescriptor.New("legacy_document", DataType.Text).Blob(
                new BlobFieldSpec("Asset", "legacy_document", ["pdf"], true, "default")));
        var transport = new SqliteTransport(db);
        var actor = new RuntimeModule().Entity(asset).IntoContext()
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport,
                new MetadataSchemaProvider(name => name == "Asset" ? asset : null)));
        await actor.EnsureSchemaAsync();
        var catalog = new SqliteBlobCatalog(transport);
        await catalog.EnsureSchemaAsync();
        var permission = new BindingPermission();
        var upload = new SqliteBlobUploadService(catalog, new UploadPermission(),
            new UnusedVerifier(), 1024);
        var module = new BlobReferenceModule(catalog, upload, new ReadPermission(), permission);
        var root = Path.Combine(Path.GetTempPath(), "teaql-dotnet-multi-blob-" + Guid.NewGuid());
        Directory.CreateDirectory(root);
        try
        {
            await using var store = new FileObjectStorage(Path.Combine(root, "private"),
                Enumerable.Repeat((byte)0x42, 32).ToArray());
            actor.WithObjectStorage(store);
            module.Install(actor);
            await module.ReadyAsync(actor);
            var references = new[] { new BlobRef("blob_multi_first"),
                new BlobRef("blob_multi_second"), new BlobRef("blob_multi_third") };
            foreach (var reference in references)
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

            var ids = new ulong[2];
            async Task Save(bool catchDenial = false, bool outerReject = false)
            {
                permission.Visited.Clear();
                permission.Denied = false;
                permission.DenyEntityId = null;
                var ledger = new EntityRoot();
                var provisional = new[] { new EntityKey("Asset", -1), new EntityKey("Asset", -2) };
                foreach (var key in provisional) ledger.MarkAsNew(key);
                ledger.Set(provisional[0], "document", Value.FromObject(references[0]));
                ledger.Set(provisional[1], "document", Value.FromObject(references[1]));
                ledger.Set(provisional[1], "legacy_document", Value.FromObject(references[2]));
                await actor.ExecuteGraphSaveAsync("save multi-entity Blob graph", async graph =>
                {
                    for (var index = 0; index < ids.Length; index++)
                        ids[index] = await graph.AllocateIdAsync("Asset");
                    if (catchDenial) permission.DenyEntityId = ids[1].ToString();
                    for (var index = 0; index < ids.Length; index++)
                    {
                        var actual = new EntityKey("Asset", new Value.U64Value(ids[index]));
                        ledger.Rekey(provisional[index], actual);
                        Assert.True(ledger.IsNew(actual));
                        Assert.Equal(Value.FromObject(references[index]), ledger.Change(actual)["document"]);
                        var command = new InsertCommand("Asset")
                            .Value("id", ids[index])
                            .Value("document", ledger.Change(actual)["document"]);
                        if (index == 1)
                            command.Value("legacy_document", ledger.Change(actual)["legacy_document"]);
                        var request = new InsertMutationRequest(command, "save multi-entity Blob graph")
                        { LedgerRoot = ledger, LedgerKey = actual };
                        try
                        {
                            await graph.MutateAsync(request, graph.Scope("Asset", ids[index], null));
                        }
                        catch (InvalidOperationException) when (catchDenial) { return true; }
                    }
                    if (outerReject) throw new InvalidOperationException("outer owner rejected");
                    return true;
                });
            }

            async Task AssertRolledBack()
            {
                Assert.Equal(0L, await Scalar(db, "SELECT COUNT(*) FROM multi_asset"));
                Assert.Equal(0L, await Scalar(db, "SELECT COUNT(*) FROM teaql_blob_binding"));
                Assert.Equal(3L, await Scalar(db,
                    "SELECT COUNT(*) FROM teaql_blob_catalog WHERE status='uploaded_unbound'"));
            }

            var denied = await Assert.ThrowsAsync<InvalidOperationException>(() => Save(catchDenial: true));
            Assert.Equal("GRAPH_TRANSACTION_ROLLBACK_ONLY", denied.Message);
            Assert.True(permission.Denied);
            Assert.Equal(new[] { (ids[0].ToString(), "document"),
                (ids[0].ToString(), "legacy_document"),
                (ids[1].ToString(), "document"),
                (ids[1].ToString(), "legacy_document") }, permission.Visited);
            await AssertRolledBack();

            var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() => Save(outerReject: true));
            Assert.Equal("outer owner rejected", rejected.Message);
            await AssertRolledBack();

            await Save();
            Assert.Equal(references[0].Key, await Scalar(db,
                $"SELECT document FROM multi_asset WHERE id={ids[0]}"));
            Assert.Null(await Scalar(db,
                $"SELECT legacy_document FROM multi_asset WHERE id={ids[0]}"));
            Assert.Equal(references[1].Key, await Scalar(db,
                $"SELECT document FROM multi_asset WHERE id={ids[1]}"));
            Assert.Equal(references[2].Key, await Scalar(db,
                $"SELECT legacy_document FROM multi_asset WHERE id={ids[1]}"));
            Assert.Equal(4L, await Scalar(db, "SELECT COUNT(*) FROM teaql_blob_binding"));
            Assert.Equal(3L, await Scalar(db,
                "SELECT COUNT(*) FROM teaql_blob_binding WHERE key IS NOT NULL"));
            Assert.Equal(1L, await Scalar(db,
                $"SELECT COUNT(*) FROM teaql_blob_binding WHERE entity_id='{ids[0]}' " +
                "AND field='legacy_document' AND key IS NULL"));
            Assert.Equal((references[0].Key, 1L), await Binding(db, ids[0].ToString()));
            Assert.Equal((references[1].Key, 1L), await Binding(db, ids[1].ToString()));
            Assert.Equal(3L, await Scalar(db,
                "SELECT COUNT(*) FROM teaql_blob_catalog WHERE status='available'"));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Fact]
    public async Task NativeSharedBlobFinalUnbindRetainsRealFileUntilDeadline()
    {
        var root = Path.Combine(Path.GetTempPath(), "teaql-dotnet-shared-blob-" + Guid.NewGuid());
        Directory.CreateDirectory(root);
        try
        {
            await using var db = new SqliteConnection("Data Source=:memory:");
            await db.OpenAsync();
            var spec = new BlobFieldSpec("Asset", "document", ["pdf"], true, "default");
            var asset = EntityDescriptor.New("Asset").TableName("shared_blob_asset")
                .Property(PropertyDescriptor.New("id", DataType.U64).Id().NotNull())
                .Property(PropertyDescriptor.New("document", DataType.Text).Blob(spec));
            var transport = new SqliteTransport(db);
            var actor = new RuntimeModule().Entity(asset).IntoContext()
                .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport,
                    new MetadataSchemaProvider(name => name == "Asset" ? asset : null)));
            await actor.EnsureSchemaAsync();
            var now = DateTimeOffset.FromUnixTimeMilliseconds(1_700_000_000_000);
            var policy = new BlobRetentionPolicy("default", "server_stream", 10, 20, 0);
            var catalog = new SqliteBlobCatalog(transport, () => now);
            await catalog.EnsureSchemaAsync();
            var permission = new BindingPermission { Policy = policy };
            var upload = new SqliteBlobUploadService(catalog,
                new UploadPermission { Policy = policy }, new FixturePdfVerifier(), 1024,
                now: () => now);
            var maintenance = new SqliteBlobMaintenanceService(catalog,
                new MaintenancePermission(), now: () => now);
            var module = new BlobReferenceModule(catalog, upload, new ReadPermission(),
                permission, maintenance: maintenance);
            await using var file = new FileObjectStorage(Path.Combine(root, "private"),
                RandomNumberGenerator.GetBytes(32));
            actor.WithObjectStorage(file);
            module.Install(actor);
            await module.ReadyAsync(actor);

            var bytes = Encoding.UTF8.GetBytes("%PDF-1.7\nshared native graph\n%%EOF");
            var session = await upload.BeginAsync(actor,
                new BlobUploadTarget("Asset", "", "document"),
                new BlobUploadInput("shared.pdf", bytes.Length, "application/pdf"),
                "shared-native");
            await upload.WriteAsync(actor, session.UploadId, new MemoryStream(bytes));
            Assert.Equal(session.Reference, await upload.CompleteAsync(actor, session.UploadId));
            var reference = session.Reference;
            async Task<object?> CatalogValue(string column)
            {
                await using var command = db.CreateCommand();
                command.CommandText = $"SELECT {column} FROM teaql_blob_catalog WHERE key=@key";
                command.Parameters.AddWithValue("@key", reference.Key);
                var value = await command.ExecuteScalarAsync();
                return value is DBNull ? null : value;
            }
            var objectKey = new ObjectKey(Assert.IsType<string>(await CatalogValue("object_key")));
            Assert.NotNull(await actor.ObjectStorage("shared native graph").HeadAsync(objectKey));
            Assert.Equal("uploaded_unbound", await CatalogValue("status"));

            var ledger = new EntityRoot();
            var provisional = new[] { new EntityKey("Asset", -1), new EntityKey("Asset", -2) };
            foreach (var key in provisional)
            {
                ledger.MarkAsNew(key);
                ledger.Set(key, "document", Value.FromObject(reference));
            }
            var ids = new ulong[2];
            await actor.ExecuteGraphSaveAsync("bind shared native Blob", async graph =>
            {
                for (var index = 0; index < 2; index++)
                {
                    ids[index] = await graph.AllocateIdAsync("Asset");
                    var actual = new EntityKey("Asset", new Value.U64Value(ids[index]));
                    ledger.Rekey(provisional[index], actual);
                    var request = new InsertMutationRequest(new InsertCommand("Asset")
                        .Value("id", ids[index]).Value("document", ledger.Change(actual)["document"]),
                        "bind shared native Blob") { LedgerRoot = ledger, LedgerKey = actual };
                    await graph.MutateAsync(request, graph.Scope("Asset", ids[index], null));
                }
                return true;
            });
            Assert.NotEqual(ids[0], ids[1]);
            Assert.Equal(2L, await Scalar(db,
                "SELECT COUNT(*) FROM teaql_blob_binding WHERE key IS NOT NULL"));
            Assert.Equal("available", await CatalogValue("status"));
            Assert.Null(await CatalogValue("last_unbound_at_ms"));

            async Task Unbind(ulong id, bool rejectOuter = false)
            {
                await actor.ExecuteGraphSaveAsync("unbind shared native Blob", async graph =>
                {
                    await graph.MutateAsync(new UpdateMutationRequest(new UpdateCommand("Asset",
                        new Value.U64Value(id)).Value("document", new Value.NullValue()),
                        "unbind shared native Blob"));
                    if (rejectOuter) throw new InvalidOperationException("outer owner rejected final unbind");
                    return true;
                });
            }
            await Unbind(ids[0]);
            Assert.Equal((null, 2L), await Binding(db, ids[0].ToString()));
            Assert.Equal((reference.Key, 1L), await Binding(db, ids[1].ToString()));
            Assert.Null(await CatalogValue("last_unbound_at_ms"));
            now = now.AddSeconds(40);
            Assert.Empty((await maintenance.RunBatchAsync(actor)).Outcomes);
            Assert.NotNull(await actor.ObjectStorage("shared native graph").HeadAsync(objectKey));

            var rejected = await Assert.ThrowsAsync<InvalidOperationException>(() => Unbind(ids[1], true));
            Assert.Equal("outer owner rejected final unbind", rejected.Message);
            Assert.Equal((reference.Key, 1L), await Binding(db, ids[1].ToString()));
            Assert.Equal(reference.Key, await Scalar(db,
                $"SELECT document FROM shared_blob_asset WHERE id={ids[1]}"));
            Assert.Null(await CatalogValue("last_unbound_at_ms"));
            Assert.Empty((await maintenance.RunBatchAsync(actor)).Outcomes);

            await Unbind(ids[1]);
            Assert.Equal((null, 2L), await Binding(db, ids[1].ToString()));
            Assert.Equal("available", await CatalogValue("status"));
            Assert.Equal(now.ToUnixTimeMilliseconds(), await CatalogValue("last_unbound_at_ms"));
            now = now.AddMilliseconds(19_999);
            Assert.Empty((await maintenance.RunBatchAsync(actor)).Outcomes);
            Assert.NotNull(await actor.ObjectStorage("shared native graph").HeadAsync(objectKey));
            now = now.AddMilliseconds(1);
            Assert.Equal("deleted", Assert.Single((await maintenance.RunBatchAsync(actor)).Outcomes).Status);
            Assert.Equal("deleted", await CatalogValue("status"));
            Assert.Null(await actor.ObjectStorage("shared native graph").HeadAsync(objectKey));
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
