using System.Linq;
using Xunit;

namespace TeaQL.Core.Tests;

public class SelectQueryTests
{
    [Fact]
    public void MaterializedListHardLimitIsLocalAndEnforced()
    {
        Assert.Equal((ulong)10_000, new SelectQuery("Order").PrepareForList().Slice!.Limit);
        Assert.Throws<System.InvalidOperationException>(() => new SelectQuery("Order").Limit(10_001).PrepareForList());
        new SelectQuery("Order").Limit(10_001).HardLimit(20_000).PrepareForList();
        Assert.DoesNotContain("HardLimit", System.Text.Json.JsonSerializer.Serialize(new SelectQuery("Order").HardLimit(20_000)));
    }

    [Fact]
    public void TestSelectQueryBuilder()
    {
        var query = new SelectQuery("users")
            .Project("id")
            .Project("name")
            .WithSearchWithText("search term")
            .WithFilter(new Expr.BinaryExpr(new Expr.ColumnExpr("id"), BinaryOp.Eq, new Expr.ValueExpr(new Value.I64Value(1))))
            .AndFilter(new Expr.BinaryExpr(new Expr.ColumnExpr("name"), BinaryOp.Eq, new Expr.ValueExpr(new Value.TextValue("Alice"))))
            .OrFilter(new Expr.BinaryExpr(new Expr.ColumnExpr("status"), BinaryOp.Eq, new Expr.ValueExpr(new Value.TextValue("active"))))
            .WithHaving(new Expr.BinaryExpr(new Expr.ColumnExpr("count"), BinaryOp.Gt, new Expr.ValueExpr(new Value.I64Value(5))))
            .AndHaving(new Expr.BinaryExpr(new Expr.ColumnExpr("count"), BinaryOp.Lt, new Expr.ValueExpr(new Value.I64Value(10))))
            .OrHaving(new Expr.BinaryExpr(new Expr.ColumnExpr("count"), BinaryOp.Eq, new Expr.ValueExpr(new Value.I64Value(0))))
            .OrderAsc("name")
            .OrderDesc("created_at")
            .WithGroupBy("role")
            .Count("total")
            .EnableAggregationCacheFor(1000)
            .PropagateAggregationCache(2000)
            .WithComment("Testing builder")
            .WithRawSql("SELECT * FROM users")
            .WithRawSqlSearchCriteria("name LIKE '%a%'")
            .Relation("posts")
            .RelationQuery("comments", new SelectQuery("comments").Project("id"))
            .Limit(10)
            .Offset(20)
            .Page(30, 40)
            .Stream(500);

        Assert.Equal("users", query.Entity);
        Assert.Equal(new[] { "id", "name" }, query.Projection);
        Assert.Equal("search term", query.SearchWithText);
        Assert.NotNull(query.Filter);
        Assert.NotNull(query.Having);
        Assert.Equal(2, query.OrderBy.Count);
        Assert.Equal(new[] { "role" }, query.GroupBy);
        Assert.Single(query.Aggregates);
        Assert.NotNull(query.AggregationCache);
        Assert.True(query.AggregationCache.Enabled);
        Assert.True(query.AggregationCache.Propagate);
        Assert.Equal("Testing builder", query.Comment);
        Assert.Single(query.TraceChain);
        Assert.Equal("SELECT * FROM users", query.RawSql);
        Assert.Single(query.RawSqlSearchCriteria);
        Assert.Equal(2, query.Relations.Count);
        Assert.Equal((ulong)40, query.Slice!.Limit.Value);
        Assert.Equal((ulong)30, query.Slice.Offset);
        Assert.Equal(500, query.StreamConfig!.ChunkSize);
    }
}
