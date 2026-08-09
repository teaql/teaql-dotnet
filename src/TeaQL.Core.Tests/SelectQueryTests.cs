using System.Linq;
using Xunit;

namespace TeaQL.Core.Tests;

public class SelectQueryTests
{
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
