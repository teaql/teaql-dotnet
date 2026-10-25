using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Provider.Sqlite.Tests;

public sealed class EmptyAggregateTests
{
    private sealed class Schema(RuntimeModule module) : ISchemaProvider
    {
        public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name);
    }

    [Fact]
    public async Task EmptyAggregatesPreserveNullZeroRelationsFacetsAndJson()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var bucket = EntityDescriptor.New("Bucket").TableName("ea_bucket")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Relation(RelationDescriptor.New("entries", "Entry").ForeignKey("bucket").Many());
        var entry = EntityDescriptor.New("Entry").TableName("ea_entry")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("bucket", DataType.I64))
            .Property(PropertyDescriptor.New("amount", DataType.I64))
            .Relation(RelationDescriptor.New("bucket", "Bucket").LocalKey("bucket").ForeignKey("id"));
        var module = new RuntimeModule().Entity(bucket).Entity(entry);
        var transport = new SqliteTransport(connection);
        var executor = new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module));
        var context = module.IntoContext().WithDataService(executor);
        await context.EnsureSchemaAsync();
        // Provider fixture construction only; every observed result executes the Q AST.
        foreach (var sql in new[] {
            "INSERT INTO ea_bucket(id) VALUES(1),(2),(3),(4)",
            "INSERT INTO ea_entry(id,bucket,amount) VALUES(1,1,4),(2,1,-4),(3,2,NULL),(4,2,NULL),(5,3,NULL),(6,3,2),(7,3,-1),(8,3,NULL)" })
            await transport.ExecuteSqlAsync(new CompiledQuery(sql, []));
        static SelectQuery Bounded(string entity) => new SelectQuery(entity).Limit(10)
            .Comment("inspect empty aggregates").Purpose("verify EA-01 through EA-08");
        static QueryRequest Request(SelectQuery query) => new(query,
            new QueryIntent("inspect empty aggregates", "verify EA-01 through EA-08"));
        foreach (var (owner, count, values, total) in new (long?, long, long, long?)[] {
            (4, 0, 0, null), (2, 2, 0, null), (3, 4, 2, 1), (1, 2, 2, 0), (null, 8, 4, 1) })
        {
            var query = Bounded("Entry").Count("rows").Aggregate(Aggregate.CountField("amount", "values"))
                .Sum("amount", "total");
            if (owner.HasValue) query.AndFilter(Expr.Eq("bucket", new Value.I64Value(owner.Value)));
            var result = await executor.QueryAsync(Request(query));
            var row = Assert.Single(result.Rows);
            Assert.Equal(count, row["rows"].TryI64());
            Assert.Equal(values, row["values"].TryI64());
            Assert.True(row.ContainsKey("total"));
            Assert.Equal(total, row["total"].TryI64());
            if (total is null) Assert.IsType<Value.NullValue>(row["total"]);
            var json = row.ToJsonValue()!.AsObject();
            Assert.True(json.ContainsKey("total"));
            Assert.Equal(total, json["total"]?.GetValue<long>());
            var explicitDefault = row["total"].TryI64() ?? 0;
            Assert.Equal(total ?? 0, explicitDefault);
            Assert.Equal(total, row["total"].TryI64());
        }
        var grouped = Bounded("Entry").Sum("amount", "total").GroupBy("bucket");
        grouped.AndFilter(Expr.Eq("bucket", new Value.I64Value(4)));
        Assert.Empty((await executor.QueryAsync(Request(grouped))).Rows);
        var parents = Bounded("Bucket").OrderAsc("id");
        parents.RelationAggregates = [
            new("entries", "entryCount", Bounded("Entry").Count("count"), true),
            new("entries", "entryTotal", Bounded("Entry").Sum("amount", "sum"), true) ];
        static void AssertBuckets(IReadOnlyList<Record> rows)
        {
            Assert.Equal(4, rows.Count);
            Assert.Equal(new long?[] { 2, 2, 4, 0 }, rows.Select(r => r["entryCount"].TryI64()));
            Assert.Equal(new long?[] { 0, null, 1, null }, rows.Select(r => r["entryTotal"].TryI64()));
            Assert.IsType<Value.NullValue>(rows[1]["entryTotal"]);
            Assert.IsType<Value.NullValue>(rows[3]["entryTotal"]);
        }
        AssertBuckets((await executor.QueryAsync(Request(parents))).Rows);
        var outer = Bounded("Entry");
        var choices = Bounded("Bucket").OrderAsc("id").Count("entryCount");
        choices.RelationAggregates.Add(parents.RelationAggregates[1]);
        outer.Facets.Add(new FacetRequest("buckets", "bucket", choices, true));
        AssertBuckets((await executor.QueryAsync(Request(outer))).Facets["buckets"].ToList());
    }
}
