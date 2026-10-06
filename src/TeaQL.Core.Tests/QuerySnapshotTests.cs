using System.Text.Json.Nodes;
using Xunit;

namespace TeaQL.Core.Tests;

public class QuerySnapshotTests
{
    [Fact]
    public void AllQueryBearingSlotsCaptureIndependentChildren()
    {
        var child = new SelectQuery("Child").Filter(Expr.Eq("name", "original"));
        var query = new SelectQuery("Parent").Comment("root").Purpose("snapshot");
        query.RelationQuery("children", child);
        query.RelationAggregates.Add(new("children", "summary", child, false));
        query.ObjectGroupBys.Add(new("group", "child_id", child));
        query.ChildEnhancements.Add(child);
        query.Facets.Add(new("names", "children", child, true));
        var copy = query.CloneForExecution();
        child.Filter(Expr.Eq("name", "changed"));
        var children = new[] {copy.RelationLoads[0].Query!, copy.RelationAggregates[0].Query,
            copy.ObjectGroupBys[0].Query, copy.ChildEnhancements[0], copy.Facets[0].Query};
        Assert.All(children, captured => {
            Assert.NotSame(child, captured);
            Assert.Equal(Expr.Eq("name", "original"), captured.FilterCondition);
        });
        Assert.All(children, captured => Assert.Same(children[0], captured));
        Assert.Equal("root", copy.CommentText); Assert.Equal("snapshot", copy.PurposeText);
    }

    [Fact]
    public void NestedExpressionListsValuesAndGeneratedFiltersAreOwned()
    {
        var values = new List<Value> {new Value.I64Value(1)};
        var json = JsonNode.Parse("{\"name\":\"original\"}")!;
        var record = new Record { ["name"] = new Value.TextValue("original") };
        var expression = new Expr.AndExpr(new List<Expr> {
            Expr.Binary(Expr.Column("id"), BinaryOp.In, Expr.Value(new Value.ListValue(values))),
            Expr.Eq("payload", new Value.JsonValue(json)), Expr.Eq("object", Value.Object(record)) });
        var query = new SelectQuery("Entry").Filter(expression).Having(expression)
            .ProjectExpr("projection", expression).OrderExprAsc(expression);
        var generated = new long[] {1,2};
        query.Filters.Add(new FilterExpression {Operator="in", Field="id", Expected=generated});
        var copy = query.CloneForExecution();
        values.Clear(); json["name"]="changed"; record["name"]=new Value.TextValue("changed");
        expression.Parts.Clear(); generated[0]=999;
        foreach (var captured in new[] {copy.FilterCondition,copy.HavingCondition,copy.ExprProjection[0].Expr,copy.OrderByItems[0].ExprValue}) {
            var parts=Assert.IsType<Expr.AndExpr>(captured).Parts; Assert.Equal(3,parts.Count);
            var value=Assert.IsType<Expr.ValueExpr>(Assert.IsType<Expr.BinaryExpr>(parts[0]).Right).NodeValue;
            Assert.Equal(new Value.I64Value(1),Assert.Single(Assert.IsType<Value.ListValue>(value).Values));
            var payload=Assert.IsType<Expr.ValueExpr>(Assert.IsType<Expr.BinaryExpr>(parts[1]).Right).NodeValue;
            Assert.Equal("original",Assert.IsType<Value.JsonValue>(payload).Value!["name"]!.GetValue<string>());
            var obj=Assert.IsType<Expr.ValueExpr>(Assert.IsType<Expr.BinaryExpr>(parts[2]).Right).NodeValue;
            Assert.Equal(new Value.TextValue("original"),Assert.IsType<Value.ObjectValue>(obj).Value["name"]);
        }
        var generatedExpr=Assert.IsType<Expr.BinaryExpr>(copy.Filters[0].ToExpr());
        Assert.Equal(new Value.I64Value(1),Assert.IsType<Value.ListValue>(Assert.IsType<Expr.ValueExpr>(generatedExpr.Right).NodeValue).Values[0]);
    }

    [Fact]
    public void SelfReferencingQueryAndExpressionRejectWithoutStackOverflow()
    {
        var query=new SelectQuery("Entry");query.RelationQuery("self",query);
        Assert.Throws<ArgumentException>(()=>query.CloneForExecution());
        var expression=new Expr.AndExpr(new List<Expr>());expression.Parts.Add(expression);
        Assert.Throws<ArgumentException>(()=>new SelectQuery("Entry").Filter(expression).CloneForExecution());
    }
}
