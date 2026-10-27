using System.Text.Json;
using Microsoft.Data.Sqlite;
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
    private sealed class Host : ISqliteContextBoundDocumentHost
    {
        public TrustedReferencePrincipal Actor = new("oidc", "alice", "Merchant", 7);
        public bool FailPresent;
        public bool SwitchActorOnApply;
        public int Mutations;

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
            var rows = await sql.FetchAllAsync("""
                SELECT id,version,quantity FROM document_order WHERE business_id=@p0
                """, [new Value.TextValue(businessId)]);
            if (rows.Count != 1) throw new InvalidOperationException("order missing");
            var row = rows[0];
            var id = (ulong)row["id"].TryI64()!.Value;
            var version = row["version"].TryI64()!.Value;
            var quantity = row["quantity"].TryI64()!.Value;
            var scope = new ReferenceDocumentScope("edit-10", "edit-order", "Order", id, version);
            var identity = new ReferenceIdentity("Order", id, version);
            var snapshot = new DocumentSnapshot("model-v1", businessId, scope, identity,
                [new DocumentEntitySnapshot("order:10", identity,
                    ["quantity"], ["quantity"], ["UPDATE"])], []);
            return new CurrentBoundDocument(new BoundDocumentSnapshot(snapshot, []),
                JsonSerializer.SerializeToElement(new { quantity }), TimeSpan.FromMinutes(5));
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
        var transport = new SqliteTransport(db);
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
    }
}
