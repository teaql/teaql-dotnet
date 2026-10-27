using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class GraphPhysicalSqlOwnerTests
{
    private static async Task<(SqliteConnection Db, UserContext Context)> Fixture()
    {
        var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var asset = EntityDescriptor.New("Asset").TableName("asset")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("name", DataType.Text));
        var context = new RuntimeModule().Entity(asset).IntoContext().WithDataService(
            new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(db),
                new MetadataSchemaProvider(name => name == "Asset" ? asset : null)));
        await context.EnsureSchemaAsync();
        await using var command = db.CreateCommand();
        command.CommandText = "CREATE TABLE module_binding (id INTEGER PRIMARY KEY, asset_id INTEGER NOT NULL)";
        await command.ExecuteNonQueryAsync();
        return (db, context);
    }

    private static async Task<long> Count(SqliteConnection db, string table)
    {
        await using var command = db.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {table}";
        return (long)(await command.ExecuteScalarAsync())!;
    }

    private static async Task Save(GraphMutationSession graph, ulong id, bool abort, List<IPhysicalSqlLease> retained)
    {
        var insert = new InsertMutationRequest(new InsertCommand("Asset")
            .Value("id", new Value.U64Value(id))
            .Value("name", new Value.TextValue("document")), "save asset");
        await graph.MutateAsync(insert, graph.Scope("Asset", id, null));
        await graph.WithPhysicalSqlAsync(async sql =>
        {
            retained.Add(sql);
            Assert.False(sql is ISqlTransaction);
            await sql.ExecuteAsync("INSERT INTO module_binding(id, asset_id) VALUES (@p0, @p1)",
                new Value[] { new Value.U64Value(id), new Value.U64Value(id) });
            var rows = await sql.FetchAllAsync("SELECT id FROM asset WHERE id = @p0",
                new Value[] { new Value.U64Value(id) });
            Assert.Single(rows);
            return true;
        });
        if (abort) throw new InvalidOperationException("abort graph");
    }

    [Fact]
    public async Task NativeBusinessAndModuleRowsShareCommitAndRollback()
    {
        var (db, context) = await Fixture(); await using var lifetime = db;
        var retained = new List<IPhysicalSqlLease>();
        await context.ExecuteGraphSaveAsync("save asset", async graph =>
        { await Save(graph, 1, false, retained); return true; });
        Assert.Equal(1, await Count(db, "asset"));
        Assert.Equal(1, await Count(db, "module_binding"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => context.ExecuteGraphSaveAsync(
            "save asset", async graph => { await Save(graph, 2, true, retained); return true; }));
        Assert.Equal(1, await Count(db, "asset"));
        Assert.Equal(1, await Count(db, "module_binding"));
        var expired = await Assert.ThrowsAsync<InvalidOperationException>(() => retained[0].ExecuteAsync(
            "DELETE FROM module_binding", Array.Empty<Value>()));
        Assert.Equal("BLOB_TRANSACTION_REQUIRED", expired.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CaughtCallbackOrSqlFailureStillRollsBack(bool sqlFailure)
    {
        var (db, context) = await Fixture(); await using var lifetime = db;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => context.ExecuteGraphSaveAsync(
            "save asset", async graph =>
            {
                var insert = new InsertMutationRequest(new InsertCommand("Asset")
                    .Value("id", new Value.U64Value(1))
                    .Value("name", new Value.TextValue("document")), "save asset");
                await graph.MutateAsync(insert, graph.Scope("Asset", 1, null));
                try
                {
                    await graph.WithPhysicalSqlAsync(async sql =>
                    {
                        await sql.ExecuteAsync("INSERT INTO module_binding(id, asset_id) VALUES (1, 1)", Array.Empty<Value>());
                        if (sqlFailure)
                        {
                            try { await sql.ExecuteAsync("INSERT INTO module_binding(id, asset_id) VALUES (1, 1)", Array.Empty<Value>()); }
                            catch (SqlExecutorException) { }
                        }
                        else throw new InvalidOperationException("module rejected binding");
                        return true;
                    });
                }
                catch (Exception) { }
                return true;
            }));
        Assert.Equal("GRAPH_TRANSACTION_ROLLBACK_ONLY", error.Message);
        Assert.Equal(0, await Count(db, "asset"));
        Assert.Equal(0, await Count(db, "module_binding"));
    }

    [Fact]
    public async Task CompletedGraphCannotLendPhysicalSqlAgain()
    {
        var (db, context) = await Fixture(); await using var lifetime = db;
        GraphMutationSession? retained = null;
        await context.ExecuteGraphSaveAsync("inspect graph", async graph =>
        {
            retained = graph;
            await graph.WithPhysicalSqlAsync(sql => sql.FetchAllAsync("SELECT 1", Array.Empty<Value>()));
            return true;
        });
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            retained!.WithPhysicalSqlAsync(sql => sql.FetchAllAsync("SELECT 1", Array.Empty<Value>())));
        Assert.Equal("GRAPH_CAPABILITY_EXPIRED", error.Message);
    }
}
