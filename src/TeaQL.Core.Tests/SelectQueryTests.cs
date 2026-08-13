using System.Linq;
using Xunit;

namespace TeaQL.Core.Tests;

public class SelectQueryTests
{
    [Fact]
    public void ContinuousPageFetchIsExplicitLocalAndValidated()
    {
        var query = new SelectQuery("Order").OptimizeForContinuousPageFetchWith("recent-orders", 30);
        Assert.Equal("recent-orders", query.ContinuousPageFetch!.Namespace);
        Assert.DoesNotContain("ContinuousPageFetch", System.Text.Json.JsonSerializer.Serialize(query));
        Assert.Throws<ArgumentException>(() => new SelectQuery("Order").OptimizeForContinuousPageFetchWith(" ", 30));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SelectQuery("Order").OptimizeForContinuousPageFetchWith("orders", 0));
    }

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
            .SearchWithText("search term")
            .Filter(new Expr.BinaryExpr(new Expr.ColumnExpr("id"), BinaryOp.Eq, new Expr.ValueExpr(new Value.I64Value(1))))
            .AndFilter(new Expr.BinaryExpr(new Expr.ColumnExpr("name"), BinaryOp.Eq, new Expr.ValueExpr(new Value.TextValue("Alice"))))
            .OrFilter(new Expr.BinaryExpr(new Expr.ColumnExpr("status"), BinaryOp.Eq, new Expr.ValueExpr(new Value.TextValue("active"))))
            .Having(new Expr.BinaryExpr(new Expr.ColumnExpr("count"), BinaryOp.Gt, new Expr.ValueExpr(new Value.I64Value(5))))
            .AndHaving(new Expr.BinaryExpr(new Expr.ColumnExpr("count"), BinaryOp.Lt, new Expr.ValueExpr(new Value.I64Value(10))))
            .OrHaving(new Expr.BinaryExpr(new Expr.ColumnExpr("count"), BinaryOp.Eq, new Expr.ValueExpr(new Value.I64Value(0))))
            .OrderAsc("name")
            .OrderDesc("created_at")
            .GroupBy("role")
            .Count("total")
            .EnableAggregationCacheFor(1000)
            .PropagateAggregationCache(2000)
            .Comment("Testing builder")
            .RawSql("SELECT * FROM users")
            .RawSqlSearchCriteria("name LIKE '%a%'")
            .Relation("posts")
            .RelationQuery("comments", new SelectQuery("comments").Project("id"))
            .Limit(10)
            .Offset(20)
            .Page(30, 40)
            .Stream(500);

        Assert.Equal("users", query.Entity);
        Assert.Equal(new[] { "id", "name" }, query.Projection);
        Assert.Equal("search term", query.SearchText);
        Assert.NotNull(query.FilterCondition);
        Assert.NotNull(query.HavingCondition);
        Assert.Equal(2, query.OrderByItems.Count);
        Assert.Equal(new[] { "role" }, query.GroupByItems);
        Assert.Single(query.AggregateItems);
        Assert.NotNull(query.AggregationCache);
        Assert.True(query.AggregationCache.EnabledValue);
        Assert.True(query.AggregationCache.PropagateValue);
        Assert.Equal("Testing builder", query.CommentText);
        Assert.Single(query.TraceChain);
        Assert.Equal("SELECT * FROM users", query.RawSqlText);
        Assert.Single(query.RawSqlSearchCriteriaItems);
        Assert.Equal(2, query.RelationLoads.Count);
        Assert.Equal((ulong)40, query.Slice!.Limit.Value);
        Assert.Equal((ulong)30, query.Slice.Offset);
        Assert.Equal(500, query.StreamConfig!.ChunkSize);
    }
}
