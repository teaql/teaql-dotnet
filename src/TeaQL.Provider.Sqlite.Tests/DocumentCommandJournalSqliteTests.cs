using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.SecureDocumentReference;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class DocumentCommandJournalSqliteTests
{
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

    private static async Task<long> ReceiptCount(SqliteConnection db)
    {
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM teaql_document_command_receipt";
        return (long)(await command.ExecuteScalarAsync())!;
    }

    [Fact]
    public async Task ReceiptSharesGraphTransactionAndReplaysWithCurrentPermission()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        await Execute(db, "CREATE TABLE document_order(id INTEGER PRIMARY KEY,version INTEGER NOT NULL,quantity INTEGER NOT NULL)");
        await Execute(db, "CREATE TABLE document_permission(subject TEXT PRIMARY KEY,can_edit INTEGER NOT NULL)");
        await Execute(db, "INSERT INTO document_order VALUES(10,1,1)");
        await Execute(db, "INSERT INTO document_permission VALUES('alice',1)");
        var transport = new SqliteTransport(db);
        var actor = new RuntimeModule().IntoContext().WithDataService(
            new SqlDataServiceExecutor(new SqliteDialect(), transport,
                new MetadataSchemaProvider(_ => null)));
        var journal = new SqliteDocumentCommandJournal(transport);
        await journal.EnsureSchemaAsync();
        await journal.ValidateSchemaAsync();
        var principal = new TrustedReferencePrincipal("oidc", "alice", "Merchant", 7);
        var scope = new ReferenceDocumentScope("edit-10", "edit-order", "Order", 10, 1);
        var command = new SubmittedDocumentCommand("ORD-10", "tqd2.fixture", "command-1",
            JsonSerializer.SerializeToElement(new { quantity = 3, note = "ok" }));
        var mutations = 0;

        async Task<(DocumentCommandReceipt Receipt, bool Replayed)> Apply(SubmittedDocumentCommand input) =>
            await actor.ExecuteGraphSaveAsync("accept protected document command", graph =>
                journal.ApplyInGraphAsync(graph, actor, scope, input,
                    async (_, _, sql) =>
                    {
                        var rows = await sql.FetchAllAsync(
                            "SELECT can_edit FROM document_permission WHERE subject=@p0",
                            [new Value.TextValue(principal.Subject)]);
                        if (rows.Count != 1 || rows[0]["can_edit"].TryI64() != 1)
                            throw new InvalidOperationException("revoked");
                        return principal;
                    },
                    async (sql, frozen) =>
                    {
                        mutations++;
                        Assert.Equal(3, frozen.Body.GetProperty("quantity").GetInt32());
                        var changed = await sql.ExecuteAsync("""
                            UPDATE document_order SET quantity=@p0,version=version+1
                            WHERE id=@p1 AND version=@p2
                            """, [new Value.I64Value(3), new Value.I64Value(10),
                                new Value.I64Value(scope.AggregateRevision)]);
                        if (changed != 1) throw new InvalidOperationException("stale order");
                        return new DocumentCommandReceipt(scope.AggregateRevision,
                            scope.AggregateRevision + 1, "tqd2.accepted");
                    }));

        var first = await Apply(command);
        Assert.False(first.Replayed);
        Assert.Equal(2, first.Receipt.NewRevision);
        Assert.Equal((2L, 3L), await Order(db));
        Assert.Equal(1, await ReceiptCount(db));
        Assert.DoesNotContain(first.Receipt.Response, first.Receipt.ToString());

        var replay = await Apply(command);
        Assert.True(replay.Replayed);
        Assert.Equal(first.Receipt, replay.Receipt);
        Assert.Equal(1, mutations);
        var legacyParts = new[] { "ORD-10", "tqd2.fixture", "command-1",
            "{\"note\":\"ok\",\"quantity\":3}" };
        var legacyHash = Convert.ToHexString(SHA256.HashData(
            JsonSerializer.SerializeToUtf8Bytes(legacyParts))).ToLowerInvariant();
        await using (var stored = db.CreateCommand())
        {
            stored.CommandText = "SELECT request_hash FROM teaql_document_command_receipt";
            Assert.NotEqual(legacyHash, (string)(await stored.ExecuteScalarAsync())!);
            stored.CommandText = "UPDATE teaql_document_command_receipt SET request_hash=@hash";
            stored.Parameters.AddWithValue("@hash", legacyHash);
            Assert.Equal(1, await stored.ExecuteNonQueryAsync());
        }
        var oldReplay = await Apply(command);
        Assert.True(oldReplay.Replayed);
        Assert.Equal(first.Receipt, oldReplay.Receipt);
        Assert.Equal(1, mutations);
        var changedCommand = command with
        { Body = JsonSerializer.SerializeToElement(new { quantity = 4, note = "ok" }) };
        Assert.Equal("DOCUMENT_REPLAY_CONFLICT", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Apply(changedCommand))).Message);
        await Execute(db, "UPDATE document_permission SET can_edit=0 WHERE subject='alice'");
        Assert.Equal("DOCUMENT_AUTHORIZATION_REQUIRED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Apply(command))).Message);
        await Execute(db, "UPDATE document_permission SET can_edit=1 WHERE subject='alice'");

        scope = scope with { AggregateRevision = 2 };
        await Execute(db, """
            CREATE TRIGGER reject_document_receipt BEFORE INSERT ON teaql_document_command_receipt
            WHEN NEW.command_id='command-trigger' BEGIN SELECT RAISE(ABORT, 'receipt rejected'); END
            """);
        var triggered = command with { CommandId = "command-trigger" };
        Assert.Equal("DOCUMENT_CATALOG_FAILED", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => Apply(triggered))).Message);
        Assert.Equal((2L, 3L), await Order(db));
        Assert.Equal(1, await ReceiptCount(db));
    }

    [Fact]
    public async Task MissingReplayPrimaryKeyFailsReadiness()
    {
        await using var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var transport = new SqliteTransport(db);
        await Execute(db, """
            CREATE TABLE teaql_document_command_receipt (
                scope_hash TEXT NOT NULL, actor_hash TEXT NOT NULL, command_id TEXT NOT NULL,
                request_hash TEXT NOT NULL, previous_revision BIGINT NOT NULL,
                new_revision BIGINT NOT NULL, response TEXT NOT NULL)
            """);
        var journal = new SqliteDocumentCommandJournal(transport);
        await journal.EnsureSchemaAsync();
        Assert.Equal("DOCUMENT_SCHEMA_MISSING", (await Assert.ThrowsAsync<InvalidOperationException>(
            () => journal.ValidateSchemaAsync())).Message);
    }
}
