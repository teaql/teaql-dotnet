using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.BlobReference;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.SecureDocumentReference;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class SecureDocumentServiceSqliteTests
{
    private sealed class LostDocumentCommitReply(ISqlTransactionTransport inner) : ISqlTransactionTransport
    {
        public bool LoseNextCommitReply;
        public Task<List<TeaQL.Core.Record>> FetchAllSqlAsync(CompiledQuery query) => inner.FetchAllSqlAsync(query);
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public async Task<ISqlTransaction> BeginSqlAsync()
        {
            var transaction = await inner.BeginSqlAsync();
            if (!LoseNextCommitReply) return transaction;
            LoseNextCommitReply = false;
            return new CommitThenFail(transaction);
        }
        private sealed class CommitThenFail(ISqlTransaction inner) : ISqlTransaction
        {
            public Task<List<TeaQL.Core.Record>> FetchAllSqlAsync(CompiledQuery query) => inner.FetchAllSqlAsync(query);
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
            public async Task CommitSqlAsync()
            {
                await inner.CommitSqlAsync();
                throw new IOException("document commit acknowledgement unavailable");
            }
            public Task RollbackSqlAsync() => inner.RollbackSqlAsync();
            public void Dispose() => inner.Dispose();
        }
    }

    private sealed class Host : ISqliteContextBoundDocumentHost, ISqliteDocumentBlobSelectionHost,
        ISqliteDocumentNewBlobSelectionHost
    {
        public TrustedReferencePrincipal Actor = new("oidc", "alice", "Merchant", 7);
        public bool FailPresent;
        public bool SwitchActorOnApply;
        public int Mutations;
        public SqliteBlobCatalog? Catalog;
        public bool ChildRows;
        public bool WrongResolve;

        public TrustedReferencePrincipal Principal(UserContext _) => Actor;

        public async Task AuthorizeAsync(UserContext _, TrustedReferencePrincipal principal,
            ReferenceDocumentScope __, IPhysicalSqlLease sql)
        {
            var rows = await sql.FetchAllAsync(
                "SELECT can_edit FROM document_permission WHERE subject=@p0",
                [new Value.TextValue(principal.Subject)]);
            if (rows.Count != 1 || rows[0]["can_edit"].TryI64() != 1)
                throw new InvalidOperationException("revoked");
        }

        public async Task<CurrentBoundDocument> LoadAsync(UserContext _, string businessId,
            IPhysicalSqlLease sql)
        {
            var projection = Catalog == null
                ? "SELECT id,version,quantity FROM document_order WHERE business_id=@p0"
                : "SELECT id,version,quantity,blob_key FROM document_order WHERE business_id=@p0";
            var rows = await sql.FetchAllAsync(projection, [new Value.TextValue(businessId)]);
            if (rows.Count != 1) throw new InvalidOperationException("order missing");
            var row = rows[0];
            var id = (ulong)row["id"].TryI64()!.Value;
            var version = row["version"].TryI64()!.Value;
            var quantity = row["quantity"].TryI64()!.Value;
            var scope = new ReferenceDocumentScope("edit-10", "edit-order", "Order", id, version);
            var identity = new ReferenceIdentity("Order", id, version);
            var fields = Catalog == null ? new[] { "quantity" } : ["attachment", "quantity"];
            var snapshot = new DocumentSnapshot("model-v1", businessId, scope, identity,
                [new DocumentEntitySnapshot("order:10", identity, fields, fields, ["UPDATE"])], []);
            List<DocumentBlobBindingSnapshot> bindings = [];
            if (Catalog != null)
            {
                var bound = await sql.FetchAllAsync("""
                    SELECT key,version FROM teaql_blob_binding WHERE scope=@p0 AND entity=@p1
                    AND entity_id=@p2 AND field=@p3
                    """, [new Value.TextValue("merchant:7"), new Value.TextValue("Order"),
                        new Value.TextValue("10"), new Value.TextValue("attachment")]);
                Assert.True(bound.Count <= 1);
                var key = bound.Count == 0 ? null : bound[0]["key"].TryText();
                var bindingVersion = bound.Count == 0 ? 0 : bound[0]["version"].TryI64()!.Value;
                Assert.Equal(row["blob_key"].TryText(), key);
                bindings.Add(new DocumentBlobBindingSnapshot("order:10", "attachment", "Order",
                    key == null ? null : new BlobRef(key), (ulong)bindingVersion, true, true));
            }
            if (ChildRows)
            {
                var children = await sql.FetchAllAsync(
                    "SELECT id,version,blob_key FROM document_order_line WHERE order_id=@p0 ORDER BY id",
                    [new Value.I64Value(10)]);
                var entities = snapshot.Entities.ToList();
                var rowKeys = new List<string>();
                foreach (var child in children)
                {
                    var childId = child["id"].TryI64()!.Value;
                    var rowKey = $"line:{childId}";
                    rowKeys.Add(rowKey);
                    var linked = await sql.FetchAllAsync("""
                        SELECT key,version FROM teaql_blob_binding WHERE scope=@p0 AND entity=@p1
                        AND entity_id=@p2 AND field=@p3
                        """, [new Value.TextValue("merchant:7"), new Value.TextValue("OrderLine"),
                            new Value.TextValue(childId.ToString()), new Value.TextValue("attachment")]);
                    Assert.True(linked.Count <= 1);
                    var key = linked.Count == 0 ? null : linked[0]["key"].TryText();
                    var bindingVersion = linked.Count == 0 ? 0 : linked[0]["version"].TryI64()!.Value;
                    Assert.Equal(child["blob_key"].TryText(), key);
                    entities.Add(new DocumentEntitySnapshot(rowKey,
                        new ReferenceIdentity("OrderLine", (ulong)childId, child["version"].TryI64()!.Value),
                        ["attachment"], ["attachment"], ["UPDATE"]));
                    bindings.Add(new DocumentBlobBindingSnapshot(rowKey, "attachment", "OrderLine",
                        key == null ? null : new BlobRef(key), (ulong)bindingVersion, true, true));
                }
                snapshot = snapshot with { Entities = entities,
                    Relations = [new DocumentRelationSnapshot("lines", rowKeys, "COMPLETE", true,
                        ["CREATE_CHILD"])] };
                bindings.Sort((left, right) =>
                {
                    var byRow = string.CompareOrdinal(left.RowKey, right.RowKey);
                    return byRow != 0 ? byRow :
                        string.CompareOrdinal(left.CanonicalField, right.CanonicalField);
                });
            }
            return new CurrentBoundDocument(new BoundDocumentSnapshot(snapshot, bindings),
                JsonSerializer.SerializeToElement(new { quantity }), TimeSpan.FromMinutes(5));
        }

        private sealed class BindingAuthorization(string entity, string entityId, string expectedAction)
            : IBlobBindingAuthorization, IBlobBindingPolicyAuthorization
        {
            public string Scope(UserContext _) => "merchant:7";
            public Task<bool> AuthorizeAsync(UserContext _, BlobBindingTarget target,
                BlobRef? previous, BlobRef? next, string action) =>
                Task.FromResult(target == new BlobBindingTarget(entity, entityId, "attachment") &&
                    next != null && action == expectedAction);
            public Task<BlobRetentionPolicy> CurrentPolicyOnLeaseAsync(UserContext actor,
                IPhysicalSqlLease lease, BlobBindingTarget target) =>
                Task.FromResult(new BlobRetentionPolicy("default", "server_stream", 86400, 604800, 3600));
        }

        public async Task BindBlobSelectionAsync(UserContext actor, GraphMutationSession graph,
            VerifiedDocumentBlobSelection selection, IPhysicalSqlLease sql)
        {
            var child = selection.Target.Kind == "NEW_CLIENT_KEY" &&
                selection.Target.TargetEntity == "OrderLine" &&
                selection.RowKey == $"line:{selection.EntityId}";
            var parent = selection.Target.Kind == "EXISTING_ROW" &&
                selection.Target.TargetEntity == "Order" &&
                selection.Target.TargetKey == "order:10" && selection.EntityId == "10";
            if (Catalog == null || (!parent && !child) ||
                selection.Target.CanonicalField != "attachment")
                throw new InvalidOperationException("BLOB_BIND_DENIED");
            var affected = await sql.ExecuteAsync(
                child ? "UPDATE document_order_line SET blob_key=@p0 WHERE id=@p1" :
                    "UPDATE document_order SET blob_key=@p0 WHERE id=@p1",
                [new Value.TextValue(selection.Reference.Key),
                    new Value.I64Value(child ? long.Parse(selection.EntityId) : 10)]);
            if (affected != 1) throw new InvalidOperationException("BLOB_BIND_DENIED");
            var entity = child ? "OrderLine" : "Order";
            var entityId = child ? selection.EntityId : "10";
            var action = child ? "Create" : "Update";
            await Catalog.BindOnGraphLeaseAsync(graph, actor, sql,
                new BlobBindingChange(new BlobBindingTarget(entity, entityId, "attachment"),
                    selection.Reference, selection.ExpectedVersion, selection.NextVersion, action),
                new BindingAuthorization(entity, entityId, action));
        }

        public async Task<(string RowKey, string EntityId)> ResolveNewBlobSelectionAsync(
            UserContext _, BoundDocumentSnapshot __, SubmittedDocumentCommand ___,
            DocumentBlobSelectionClaim selection, IPhysicalSqlLease sql)
        {
            if (WrongResolve) return ("order:10", "10");
            var rows = await sql.FetchAllAsync("""
                SELECT id FROM document_order_line WHERE order_id=@p0 AND client_key=@p1
                """, [new Value.I64Value(10), new Value.TextValue(selection.Target.TargetKey)]);
            if (rows.Count != 1) throw new InvalidOperationException("DOCUMENT_PROJECTION_VIOLATION");
            var id = rows[0]["id"].TryI64()!.Value.ToString();
            return ($"line:{id}", id);
        }

        public async Task ApplyAsync(UserContext _, BoundDocumentSnapshot issued,
            SubmittedDocumentCommand submitted, IPhysicalSqlLease sql)
        {
            Mutations++;
            if (submitted.Body.GetProperty("quantity").GetInt64() != 3)
                throw new InvalidOperationException("invalid quantity");
            var changed = await sql.ExecuteAsync("""
                UPDATE document_order SET quantity=@p0,version=version+1
                WHERE id=@p1 AND version=@p2
                """, [new Value.I64Value(3), new Value.I64Value(10),
                    new Value.I64Value(issued.Snapshot.Scope.AggregateRevision)]);
            if (changed != 1) throw new InvalidOperationException("stale order");
            if (ChildRows)
                foreach (var selection in submitted.BlobSelections ?? [])
                {
                    if (selection.Kind != "NEW_CLIENT_KEY") continue;
                    if (selection.TargetEntity != "OrderLine" ||
                        selection.CanonicalField != "attachment" ||
                        selection.TargetKey is not ("client-line-1" or "client-line-2"))
                        throw new InvalidOperationException("DOCUMENT_PROJECTION_VIOLATION");
                    var childId = selection.TargetKey == "client-line-1" ? 21 : 22;
                    changed = await sql.ExecuteAsync("""
                        INSERT INTO document_order_line(id,order_id,client_key,version,blob_key)
                        VALUES (@p0,@p1,@p2,1,NULL)
                        """, [new Value.I64Value(childId), new Value.I64Value(10),
                            new Value.TextValue(selection.TargetKey)]);
                    if (changed != 1) throw new InvalidOperationException("DOCUMENT_PROJECTION_VIOLATION");
                }
            if (SwitchActorOnApply) Actor = Actor with { Subject = "bob" };
        }

        public Task PresentAsync(UserContext _, ContextualDocument document)
        {
            Assert.Equal("ORD-10", document.BusinessId);
            if (FailPresent) throw new InvalidOperationException("presentation failed");
            return Task.CompletedTask;
        }
    }

    private static async Task Execute(SqliteConnection db, string statement)
    {
        await using var command = db.CreateCommand();
        command.CommandText = statement;
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<(long Version, long Quantity)> Order(SqliteConnection db)
    {
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT version,quantity FROM document_order WHERE id=10";
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        return (reader.GetInt64(0), reader.GetInt64(1));
    }

    private static async Task<long> Receipts(SqliteConnection db)
    {
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM teaql_document_command_receipt";
        return (long)(await command.ExecuteScalarAsync())!;
    }

    [Fact]
    public async Task OpenAcceptReplayAndRollbackUseRealSqlite()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        await Execute(db, "CREATE TABLE document_order(id INTEGER PRIMARY KEY,business_id TEXT NOT NULL UNIQUE,version INTEGER NOT NULL,quantity INTEGER NOT NULL)");
        await Execute(db, "CREATE TABLE document_permission(subject TEXT PRIMARY KEY,can_edit INTEGER NOT NULL)");
        await Execute(db, "INSERT INTO document_order VALUES(10,'ORD-10',1,1)");
        await Execute(db, "INSERT INTO document_permission VALUES('alice',1)");
        var transport = new LostDocumentCommitReply(new SqliteTransport(db));
        var actor = new RuntimeModule().IntoContext().WithDataService(
            new SqlDataServiceExecutor(new SqliteDialect(), transport,
                new MetadataSchemaProvider(_ => null)));
        var keys = new StaticReferenceKeyProvider("k1", new ReferenceKey("k1", Enumerable.Repeat((byte)0x42, 32).ToArray()));
        var module = new SecureDocumentReferenceModule("order-service", "test", keys, (_, _, _) => true);
        var journal = new SqliteDocumentCommandJournal(transport);
        await journal.EnsureSchemaAsync(); await journal.ValidateSchemaAsync();
        var host = new Host();
        var service = new SqliteContextBoundDocumentService(actor, transport, module, journal, host);
        Task<ContextualDocument> Open() => service.OpenAsync(actor,
            new DocumentOpenRequest("Order", "ORD-10", "edit-order", TimeSpan.FromMinutes(1)));
        Task<AcceptedDocument> Accept(SubmittedDocumentCommand command) => service.AcceptAsync(actor,
            new DocumentAcceptRequest("Order", "edit-order", command));

        var opened = await Open();
        Assert.StartsWith("tqd2.", opened.DocumentToken);
        Assert.Equal(1, opened.Body.GetProperty("quantity").GetInt64());
        Assert.DoesNotContain(opened.DocumentToken, opened.ToString());
        var command = new SubmittedDocumentCommand("ORD-10", opened.DocumentToken,
            "command-1", JsonSerializer.SerializeToElement(new { quantity = 3 }));
        var first = await Accept(command);
        Assert.False(first.Replayed);
        Assert.Equal((1L, 2L), (first.PreviousRevision, first.NewRevision));
        Assert.Equal((2L, 3L), await Order(db));
        Assert.Equal(1, await Receipts(db));
        Assert.DoesNotContain(first.Document.DocumentToken, first.ToString());

        var replay = await Accept(command);
        Assert.True(replay.Replayed);
        Assert.Equal(first.Document.DocumentToken, replay.Document.DocumentToken);
        Assert.Equal(1, host.Mutations);
        Assert.Equal("DOCUMENT_REPLAY_CONFLICT", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(command with
            { Body = JsonSerializer.SerializeToElement(new { quantity = 4 }) }))).Message);
        Assert.Equal("DOCUMENT_REVISION_CONFLICT", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(command with { CommandId = "command-stale" }))).Message);

        await Execute(db, "UPDATE document_permission SET can_edit=0 WHERE subject='alice'");
        Assert.Equal("DOCUMENT_AUTHORIZATION_REQUIRED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(command))).Message);
        Assert.Equal("DOCUMENT_AUTHORIZATION_REQUIRED", (await Assert.ThrowsAsync<InvalidOperationException>(
            Open)).Message);
        await Execute(db, "UPDATE document_permission SET can_edit=1 WHERE subject='alice'");

        var latest = await Open();
        var followup = new SubmittedDocumentCommand("ORD-10", latest.DocumentToken,
            "command-2", JsonSerializer.SerializeToElement(new { quantity = 3 }));
        host.FailPresent = true;
        await Assert.ThrowsAsync<GraphCommittedException>(() => Accept(followup));
        host.FailPresent = false;
        var recovered = await Accept(followup);
        Assert.True(recovered.Replayed);
        Assert.Equal(3, recovered.NewRevision);
        Assert.Equal(2, host.Mutations);

        await Execute(db, """
            CREATE TRIGGER reject_document_receipt BEFORE INSERT ON teaql_document_command_receipt
            WHEN NEW.command_id='command-trigger' BEGIN SELECT RAISE(ABORT, 'receipt rejected'); END
            """);
        latest = await Open();
        Assert.Equal("DOCUMENT_CATALOG_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(new SubmittedDocumentCommand("ORD-10", latest.DocumentToken,
                "command-trigger", JsonSerializer.SerializeToElement(new { quantity = 3 }))))).Message);
        Assert.Equal((3L, 3L), await Order(db));
        Assert.Equal(2, await Receipts(db));

        host.SwitchActorOnApply = true;
        Assert.Equal("DOCUMENT_AUTHORIZATION_REQUIRED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(new SubmittedDocumentCommand("ORD-10", latest.DocumentToken,
                "command-actor-drift", JsonSerializer.SerializeToElement(new { quantity = 3 }))))).Message);
        host.Actor = host.Actor with { Subject = "alice" };
        Assert.Equal((3L, 3L), await Order(db));
        Assert.Equal(2, await Receipts(db));
        host.SwitchActorOnApply = false;
        var beforeUnknown = host.Mutations;
        var unknownOpen = await Open();
        var unknown = new SubmittedDocumentCommand("ORD-10", unknownOpen.DocumentToken,
            "command-commit-unknown", JsonSerializer.SerializeToElement(new { quantity = 3 }));
        transport.LoseNextCommitReply = true;
        var commitError = await Assert.ThrowsAsync<SqlExecutorException>(() => Accept(unknown));
        Assert.IsType<IOException>(commitError.InnerException);
        Assert.Contains("document commit acknowledgement unavailable", commitError.Message);
        Assert.Equal((4L, 3L), await Order(db));
        Assert.Equal(3, await Receipts(db));
        Assert.Equal(beforeUnknown + 1, host.Mutations);
        async Task<string> DurableReceipt()
        {
            await using var command = db.CreateCommand();
            command.CommandText = "SELECT response FROM teaql_document_command_receipt WHERE command_id='command-commit-unknown'";
            return (string)(await command.ExecuteScalarAsync())!;
        }
        var durable = await DurableReceipt();
        await Execute(db, "UPDATE document_permission SET can_edit=0 WHERE subject='alice'");
        Assert.Equal("DOCUMENT_AUTHORIZATION_REQUIRED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(unknown))).Message);
        await Execute(db, "UPDATE document_permission SET can_edit=1 WHERE subject='alice'");
        Assert.Equal("DOCUMENT_REPLAY_CONFLICT", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(unknown with { Body = JsonSerializer.SerializeToElement(new { quantity = 4 }) }))).Message);
        var recoveredUnknown = await Accept(unknown);
        Assert.True(recoveredUnknown.Replayed);
        Assert.Equal(4, recoveredUnknown.NewRevision);
        Assert.Equal(beforeUnknown + 1, host.Mutations);
        Assert.True(await DurableReceipt() == durable, "commit-unknown replay changed its durable receipt");
        Assert.Equal((4L, 3L), await Order(db));
    }

    [Fact]
    public async Task ExistingRowBlobSelectionSharesBusinessBindingAndReceipt()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        await Execute(db, "CREATE TABLE document_order(id INTEGER PRIMARY KEY,business_id TEXT NOT NULL UNIQUE,version INTEGER NOT NULL,quantity INTEGER NOT NULL,blob_key TEXT)");
        await Execute(db, "CREATE TABLE document_permission(subject TEXT PRIMARY KEY,can_edit INTEGER NOT NULL)");
        await Execute(db, "INSERT INTO document_order VALUES(10,'ORD-10',1,1,NULL)");
        await Execute(db, "INSERT INTO document_permission VALUES('alice',1)");
        var transport = new SqliteTransport(db);
        var catalog = new SqliteBlobCatalog(transport);
        await catalog.EnsureSchemaAsync(); await catalog.ValidateSchemaAsync();
        await Execute(db, """
            INSERT INTO teaql_blob_catalog
            (key,scope,object_key,content_version,file_name,content_type,size_bytes,status,
             completed_at_ms,policy_profile,upload_mode,unbound_retention_seconds,effective_unref_seconds)
            VALUES ('blob_A','merchant:7','objects/a','v1','a.pdf','application/pdf',3,
              'uploaded_unbound',CAST(strftime('%s','now') AS INTEGER)*1000,'default','server_stream',86400,604800),
                   ('blob_B','merchant:7','objects/b','v1','b.pdf','application/pdf',3,
              'uploaded_unbound',CAST(strftime('%s','now') AS INTEGER)*1000,'default','server_stream',86400,604800)
            """);
        var actor = new RuntimeModule().IntoContext().WithDataService(
            new SqlDataServiceExecutor(new SqliteDialect(), transport,
                new MetadataSchemaProvider(_ => null)));
        var keys = new StaticReferenceKeyProvider("k1",
            new ReferenceKey("k1", Enumerable.Repeat((byte)0x42, 32).ToArray()));
        var module = new SecureDocumentReferenceModule("order-service", "test", keys,
            (_, _, _) => true);
        var journal = new SqliteDocumentCommandJournal(transport);
        await journal.EnsureSchemaAsync(); await journal.ValidateSchemaAsync();
        var host = new Host { Catalog = catalog };
        var service = new SqliteContextBoundDocumentService(actor, transport, module, journal, host);
        Task<ContextualDocument> Open() => service.OpenAsync(actor,
            new DocumentOpenRequest("Order", "ORD-10", "edit-order", TimeSpan.FromMinutes(1)));
        Task<AcceptedDocument> Accept(SubmittedDocumentCommand command) => service.AcceptAsync(actor,
            new DocumentAcceptRequest("Order", "edit-order", command));
        DocumentBlobSelectionSubmission Select(ContextualDocument opened, string commandId, string key)
        {
            var issued = module.ConsumeBoundDocumentSnapshot(host.Actor, opened.DocumentToken,
                "Order", "edit-order").Document;
            var target = new BlobSelectionTarget(issued.Snapshot.Scope, "Order", "attachment",
                "EXISTING_ROW", "order:10", commandId);
            var token = module.IssueBlobSelection(host.Actor, opened.DocumentToken,
                new BlobSelectionClaim(target, new BlobRef(key)), TimeSpan.FromMinutes(1));
            return new DocumentBlobSelectionSubmission(token, "Order", "attachment",
                "EXISTING_ROW", "order:10");
        }
        async Task<(long Version, long Quantity, string Key)> BoundOrder()
        {
            await using var command = db.CreateCommand();
            command.CommandText = "SELECT version,quantity,blob_key FROM document_order";
            await using var reader = await command.ExecuteReaderAsync();
            Assert.True(await reader.ReadAsync());
            return (reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2));
        }
        async Task<(string Key, long Version)> Binding()
        {
            await using var command = db.CreateCommand();
            command.CommandText = "SELECT key,version FROM teaql_blob_binding";
            await using var reader = await command.ExecuteReaderAsync();
            Assert.True(await reader.ReadAsync());
            return (reader.GetString(0), reader.GetInt64(1));
        }
        async Task<string> Status(string key)
        {
            await using var command = db.CreateCommand();
            command.CommandText = "SELECT status FROM teaql_blob_catalog WHERE key=$key";
            command.Parameters.AddWithValue("$key", key);
            return (string)(await command.ExecuteScalarAsync())!;
        }

        var opened = await Open();
        var command = new SubmittedDocumentCommand("ORD-10", opened.DocumentToken,
            "command-1", JsonSerializer.SerializeToElement(new { quantity = 3 }),
            [Select(opened, "command-1", "blob_A")]);
        var accepted = await Accept(command);
        Assert.False(accepted.Replayed);
        Assert.Equal((1L, 2L), (accepted.PreviousRevision, accepted.NewRevision));
        Assert.Equal((2L, 3L, "blob_A"), await BoundOrder());
        Assert.Equal(("blob_A", 1L), await Binding());
        Assert.Equal("available", await Status("blob_A"));
        Assert.Equal(1, await Receipts(db));
        Assert.True((await Accept(command)).Replayed);
        Assert.Equal(1, host.Mutations);
        Assert.Equal("DOCUMENT_REPLAY_CONFLICT", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(command with { BlobSelections = null }))).Message);

        var fresh = await Open();
        var wrong = new SubmittedDocumentCommand("ORD-10", fresh.DocumentToken,
            "command-wrong", JsonSerializer.SerializeToElement(new { quantity = 3 }),
            [Select(fresh, "other-command", "blob_A")]);
        Assert.Equal("ROUND_TRIP_REFERENCE_SCOPE_MISMATCH", (await Assert.ThrowsAsync<ReferenceException>(
            () => Accept(wrong))).Code);
        var duplicate = Select(fresh, "command-duplicate", "blob_A");
        Assert.Equal("DOCUMENT_COMMAND_INVALID", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(new SubmittedDocumentCommand("ORD-10", fresh.DocumentToken,
                "command-duplicate", JsonSerializer.SerializeToElement(new { quantity = 3 }),
                [duplicate, duplicate])))).Message);
        Assert.Equal("BLOB_BIND_DENIED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(new SubmittedDocumentCommand("ORD-10", fresh.DocumentToken,
                "command-missing", JsonSerializer.SerializeToElement(new { quantity = 3 }),
                [Select(fresh, "command-missing", "blob_missing")])))).Message);
        Assert.Equal((2L, 3L, "blob_A"), await BoundOrder());

        await Execute(db, """
            CREATE TRIGGER reject_selected_receipt BEFORE INSERT ON teaql_document_command_receipt
            WHEN NEW.command_id='command-receipt-fail'
            BEGIN SELECT RAISE(ABORT, 'receipt rejected'); END
            """);
        Assert.Equal("DOCUMENT_CATALOG_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(new SubmittedDocumentCommand("ORD-10", fresh.DocumentToken,
                "command-receipt-fail", JsonSerializer.SerializeToElement(new { quantity = 3 }),
                [Select(fresh, "command-receipt-fail", "blob_B")])))).Message);
        Assert.Equal((2L, 3L, "blob_A"), await BoundOrder());
        Assert.Equal(("blob_A", 1L), await Binding());
        Assert.Equal("uploaded_unbound", await Status("blob_B"));
        Assert.Equal(1, await Receipts(db));
    }

    [Fact]
    public async Task NewChildBlobSelectionCommitsAndReceiptFailureRollsBack()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        await Execute(db, "CREATE TABLE document_order(id INTEGER PRIMARY KEY,business_id TEXT UNIQUE NOT NULL,version INTEGER NOT NULL,quantity INTEGER NOT NULL,blob_key TEXT)");
        await Execute(db, "CREATE TABLE document_order_line(id INTEGER PRIMARY KEY,order_id INTEGER NOT NULL,client_key TEXT UNIQUE NOT NULL,version INTEGER NOT NULL,blob_key TEXT)");
        await Execute(db, "CREATE TABLE document_permission(subject TEXT PRIMARY KEY,can_edit INTEGER NOT NULL)");
        await Execute(db, "INSERT INTO document_order VALUES(10,'ORD-10',1,1,NULL)");
        await Execute(db, "INSERT INTO document_permission VALUES('alice',1)");
        var transport = new SqliteTransport(db);
        var catalog = new SqliteBlobCatalog(transport);
        await catalog.EnsureSchemaAsync(); await catalog.ValidateSchemaAsync();
        await Execute(db, """
            INSERT INTO teaql_blob_catalog
            (key,scope,object_key,content_version,file_name,content_type,size_bytes,status,
             completed_at_ms,policy_profile,upload_mode,unbound_retention_seconds,effective_unref_seconds)
            VALUES ('blob_C','merchant:7','objects/c','v1','c.pdf','application/pdf',3,
              'uploaded_unbound',CAST(strftime('%s','now') AS INTEGER)*1000,'default','server_stream',86400,604800),
                   ('blob_D','merchant:7','objects/d','v1','d.pdf','application/pdf',3,
              'uploaded_unbound',CAST(strftime('%s','now') AS INTEGER)*1000,'default','server_stream',86400,604800)
            """);
        var actor = new RuntimeModule().IntoContext().WithDataService(
            new SqlDataServiceExecutor(new SqliteDialect(), transport,
                new MetadataSchemaProvider(_ => null)));
        var keys = new StaticReferenceKeyProvider("k1",
            new ReferenceKey("k1", Enumerable.Repeat((byte)0x42, 32).ToArray()));
        var module = new SecureDocumentReferenceModule("order-service", "test", keys,
            (_, _, _) => true);
        var journal = new SqliteDocumentCommandJournal(transport);
        await journal.EnsureSchemaAsync(); await journal.ValidateSchemaAsync();
        var host = new Host { Catalog = catalog, ChildRows = true };
        var service = new SqliteContextBoundDocumentService(actor, transport, module, journal, host);
        Task<ContextualDocument> Open() => service.OpenAsync(actor,
            new DocumentOpenRequest("Order", "ORD-10", "edit-order", TimeSpan.FromMinutes(1)));
        Task<AcceptedDocument> Accept(SubmittedDocumentCommand command) => service.AcceptAsync(actor,
            new DocumentAcceptRequest("Order", "edit-order", command));
        DocumentBlobSelectionSubmission Select(ContextualDocument opened, string commandId,
            string clientKey, string key)
        {
            var issued = module.ConsumeBoundDocumentSnapshot(host.Actor, opened.DocumentToken,
                "Order", "edit-order").Document;
            var target = new BlobSelectionTarget(issued.Snapshot.Scope, "OrderLine", "attachment",
                "NEW_CLIENT_KEY", clientKey, commandId);
            var token = module.IssueBlobSelection(host.Actor, opened.DocumentToken,
                new BlobSelectionClaim(target, new BlobRef(key)), TimeSpan.FromMinutes(1));
            return new DocumentBlobSelectionSubmission(token, "OrderLine", "attachment",
                "NEW_CLIENT_KEY", clientKey);
        }
        async Task<long> Scalar(string sql)
        {
            await using var command = db.CreateCommand();
            command.CommandText = sql;
            return (long)(await command.ExecuteScalarAsync())!;
        }
        async Task<string> TextScalar(string sql)
        {
            await using var command = db.CreateCommand();
            command.CommandText = sql;
            return (string)(await command.ExecuteScalarAsync())!;
        }

        var opened = await Open();
        var submitted = new SubmittedDocumentCommand("ORD-10", opened.DocumentToken,
            "command-child-1", JsonSerializer.SerializeToElement(new { quantity = 3 }),
            [Select(opened, "command-child-1", "client-line-1", "blob_C")]);
        host.WrongResolve = true;
        Assert.Equal("DOCUMENT_PROJECTION_VIOLATION", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(submitted))).Message);
        Assert.Equal((1L, 1L), await Order(db));
        Assert.Equal(0, await Receipts(db));
        host.WrongResolve = false;
        var first = await Accept(submitted);
        Assert.Equal((1L, 2L, false),
            (first.PreviousRevision, first.NewRevision, first.Replayed));
        Assert.True((await Accept(submitted)).Replayed);
        Assert.Equal((2L, 3L), await Order(db));
        Assert.Equal(21, await Scalar("SELECT id FROM document_order_line"));
        Assert.Equal("blob_C", await TextScalar("SELECT blob_key FROM document_order_line"));
        Assert.Equal("blob_C", await TextScalar(
            "SELECT key FROM teaql_blob_binding WHERE entity='OrderLine' AND entity_id='21'"));
        Assert.Equal("available", await TextScalar(
            "SELECT status FROM teaql_blob_catalog WHERE key='blob_C'"));
        Assert.Equal(1, await Receipts(db));

        opened = await Open();
        var failed = new SubmittedDocumentCommand("ORD-10", opened.DocumentToken,
            "command-child-2", JsonSerializer.SerializeToElement(new { quantity = 3 }),
            [Select(opened, "command-child-2", "client-line-2", "blob_D")]);
        await Execute(db, """
            CREATE TRIGGER reject_child_receipt BEFORE INSERT ON teaql_document_command_receipt
            WHEN NEW.command_id='command-child-2'
            BEGIN SELECT RAISE(ABORT, 'receipt rejected'); END
            """);
        Assert.Equal("DOCUMENT_CATALOG_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Accept(failed))).Message);
        Assert.Equal((2L, 3L), await Order(db));
        Assert.Equal(1, await Scalar("SELECT COUNT(*) FROM document_order_line"));
        Assert.Equal(0, await Scalar(
            "SELECT COUNT(*) FROM teaql_blob_binding WHERE entity_id='22'"));
        Assert.Equal("uploaded_unbound", await TextScalar(
            "SELECT status FROM teaql_blob_catalog WHERE key='blob_D'"));
        Assert.Equal(1, await Receipts(db));
    }
}
