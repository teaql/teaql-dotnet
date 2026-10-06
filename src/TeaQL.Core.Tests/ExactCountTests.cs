using Xunit;

namespace TeaQL.Core.Tests;

public class ExactCountTests
{
    [Fact]
    public void CountRemovesPresentationButRetainsDetachedFiltersAndIntent()
    {
        var values = new List<Value> { Value.FromObject(7L) };
        var query = new SelectQuery("Document").Filter(Expr.InList("id", values))
            .Project("name").OrderAsc("name").Offset(100).Limit(20)
            .RelationQuery("lines", new SelectQuery("Line").Limit(10))
            .OptimizePaginationWithIdSet().Comment("count documents").Purpose("render pages");
        query.ChildEnhancements.Add(new SelectQuery("Line"));
        query.ObjectGroupBys.Add(new ObjectGroupBy("group", "line", new SelectQuery("Line")));
        query.Facets.Add(new FacetRequest("facet", "lines", new SelectQuery("Line"), false));
        var count = query.ForExactCount();
        values.Clear();
        Assert.Empty(count.Projection); Assert.Empty(count.OrderByItems);
        Assert.Empty(count.RelationLoads); Assert.Empty(count.ChildEnhancements);
        Assert.Empty(count.ObjectGroupBys); Assert.Empty(count.Facets);
        Assert.Null(count.Slice); Assert.Null(count.IdSetPagination);
        Assert.Equal(Aggregate.Count("count"), Assert.Single(count.AggregateItems));
        var filter = Assert.IsType<Expr.BinaryExpr>(count.FilterCondition);
        Assert.Single(Assert.IsType<Value.ListValue>(Assert.IsType<Expr.ValueExpr>(filter.Right).NodeValue).Values);
        Assert.Equal("count documents", count.CommentText); Assert.Equal("render pages", count.PurposeText);
        Assert.Single(query.RelationLoads); Assert.Equal(100UL, query.Slice!.Offset);
    }

    [Fact]
    public void UnsupportedGroupedAndRawShapesAreNotMisreportedAsEntityCounts()
    {
        var grouped = new SelectQuery("Document"); grouped.GroupByItems.Add("state");
        Assert.Throws<NotSupportedException>(() => grouped.ForExactCount());
        var aggregate = new SelectQuery("Document"); aggregate.AggregateItems.Add(Aggregate.Sum("amount", "sum"));
        Assert.Throws<NotSupportedException>(() => aggregate.ForExactCount());
        var raw = new SelectQuery("Document") { RawSqlText = "SELECT 1" };
        Assert.Throws<NotSupportedException>(() => raw.ForExactCount());
        var having = new SelectQuery("Document") { HavingCondition = Expr.Gt("amount", 0) };
        Assert.Throws<NotSupportedException>(() => having.ForExactCount());
    }
}
