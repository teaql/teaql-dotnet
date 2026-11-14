using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Provider.Sqlite.Tests;

public sealed class NestedPopulationTests
{
    private sealed class Schema(RuntimeModule module) : ISchemaProvider
    { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }

    [Fact]
    public async Task NestedPopulationKeepsAllDetailsAndStatisticalGroups()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var bucket = EntityDescriptor.New("Bucket").TableName("np_bucket")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Relation(RelationDescriptor.New("entries", "Entry").ForeignKey("bucket").Many());
        var entry = EntityDescriptor.New("Entry").TableName("np_entry")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("bucket", DataType.I64));
        var module = new RuntimeModule().Entity(bucket).Entity(entry);
        var transport = new SqliteTransport(connection);
        var executor = new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module));
        var context = module.IntoContext().WithDataService(executor);
        await context.EnsureSchemaAsync();
        foreach (var sql in new[] {
            "WITH RECURSIVE n(x) AS (SELECT 1 UNION ALL SELECT x+1 FROM n WHERE x<10003) INSERT INTO np_bucket SELECT x,1 FROM n",
            "WITH RECURSIVE n(x) AS (SELECT 1 UNION ALL SELECT x+1 FROM n WHERE x<10003) INSERT INTO np_entry SELECT x,1,1 FROM n" })
            await transport.ExecuteSqlAsync(new CompiledQuery(sql, []));
        static SelectQuery Query(string entity) => new SelectQuery(entity)
            .Comment("inspect complete nested population").Purpose("avoid silent statistical truncation");
        var root = Query("Bucket").Limit(1).AndFilter(Expr.Eq("id", new Value.I64Value(1)))
            .RelationQuery("entries", Query("Entry"));
        var result = await executor.QueryAsync(new QueryRequest(root));
        var children = Assert.IsType<Value.ListValue>(Assert.Single(result.Rows)["entries"]);
        Assert.Equal(10003, children.Values.Count);
        root = Query("Bucket").Limit(1).AndFilter(Expr.Eq("id", new Value.I64Value(1)));
        root.RelationAggregates.Add(new("entries", "n", Query("Entry").Count("n"), true));
        result = await executor.QueryAsync(new QueryRequest(root));
        Assert.Equal(10003, Assert.Single(result.Rows)["n"].TryI64());
        var grouped = new QueryRequest(Query("Bucket").Limit(1)).Derive(Query("Entry").GroupBy("id").Count("n"),"entries");
        Assert.Equal(10003,(await executor.QueryAsync(grouped)).Rows.Count);
        Assert.Equal(10000, (await executor.QueryAsync(new QueryRequest(Query("Entry")))).Rows.Count);
        root = Query("Bucket").Limit(1).AndFilter(Expr.Eq("id",new Value.I64Value(1)))
            .RelationQuery("entries",Query("Entry").Limit(2));
        Assert.Equal(2,Assert.IsType<Value.ListValue>(Assert.Single((await executor.QueryAsync(new QueryRequest(root))).Rows)["entries"]).Values.Count);
    }
}
