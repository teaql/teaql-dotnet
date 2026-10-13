using System.Text.Json;
using TeaQL.Core;
using Xunit;

namespace TeaQL.DataService.Tests;

public class QueryRequestTreeTests
{
    private static string[] Comments(QueryRequest request) => request.Scope.Recover().Select(node => node.Comment).ToArray();

    [Fact]
    public void NativeOperationAndLocalCommentsBelongToIndependentAttachments()
    {
        var template = new SelectQuery("SchoolType");
        var query = new SelectQuery("School").Comment("load schools").Purpose("render school list");
        var operation = new QueryOperation("relation", "SelectSchoolTypeWith", Relation: "schoolType");
        query.RelationLoads.Add(new("schoolType", template) { Operation = operation });
        query.RelationLoads.Add(new("otherType", template) { Operation = operation with {Method="SelectOtherTypeWith",Relation="otherType"} });
        var root = new QueryRequest(query);
        var first = root.Derive(root.Query.RelationLoads[0].Query!, "schoolType",operation);
        var second = root.Derive(root.Query.RelationLoads[1].Query!, "otherType",root.Query.RelationLoads[1].Operation);
        Assert.Equal(new[]{"render school list","load schools","SelectSchoolTypeWith(...)","load SchoolType"},Comments(first));
        Assert.Equal("SelectOtherTypeWith(...)",Comments(second)[2]);
        Assert.NotSame(first.Scope.Parent,second.Scope.Parent);
        Assert.Same(root.Scope,first.Scope.Parent!.Parent);
        Assert.Null(template.CommentText); Assert.Null(template.PurposeText);
        var explicitChild = root.Derive(template.Copy().Comment("load visible school types"),"schoolType",operation);
        Assert.Equal("load visible school types",explicitChild.Comment);
        Assert.Equal("render school list",explicitChild.Purpose);
        Assert.Throws<RequestIntentException>(()=>root.Derive(template.Copy().Comment("\u2003"),"schoolType",operation));
        Assert.Throws<RequestIntentException>(()=>new QueryRequest(template));
        var encoded = JsonSerializer.Serialize(first.Query);
        Assert.DoesNotContain("RequestScope",encoded); Assert.DoesNotContain("TraceChain",encoded); Assert.DoesNotContain("SelectSchoolTypeWith",encoded);
    }

    [Fact]
    public void FacetAggregateAndValuesShareOperationParent()
    {
        var query = new SelectQuery("CustomerOrder").Comment("load order list").Purpose("render order list page");
        var values = new SelectQuery("OrderStatus").Comment("load status facets");
        var sum = new SelectQuery("CustomerOrder").Sum("total_amount","orderTotalAmount");
        values.RelationAggregates.Add(new("customerOrders","orderTotalAmount",sum,true) {
            Operation = new("aggregate","SumTotalAmountOfCustomerOrdersAs","orderTotalAmount","customerOrders") });
        query.Facets.Add(new("statusCounts","status",values,true) { Operation = new("facet","FacetByStatusAs","statusCounts","status") });
        var root = new QueryRequest(query);
        var selected = root.Derive(root.Query.Facets[0].Query,"status",root.Query.Facets[0].Operation);
        var aggregate = selected.Query.RelationAggregates[0];
        var derived = selected.DeriveAggregate(aggregate.Query,aggregate.RelationName,aggregate.Operation);
        Assert.Equal(new[]{"render order list page","load order list","FacetByStatusAs(\"statusCounts\")","aggregate status"},Comments(derived));
        Assert.Same(selected.Scope.Parent,derived.Scope.Parent);
        Assert.Equal("load status facets",selected.Comment);
        var count = root.DeriveAggregate(new SelectQuery("OrderItem").Count("itemCount"),"items",new("count","CountOrderItemsAs","itemCount","items"));
        Assert.Equal(new[]{"render order list page","load order list","CountOrderItemsAs(\"itemCount\")"},Comments(count));
    }

    [Fact]
    public void UnrelatedInvocationCannotReplaceOwnedQueryEvenWithIdenticalIntent()
    {
        var first = new QueryRequest(new SelectQuery("School").Comment("same comment").Purpose("same purpose"));
        var second = new QueryRequest(new SelectQuery("School").Comment("same comment").Purpose("same purpose"));
        Assert.Throws<InvalidOperationException>(() => first.WithQuery(second.Query.Copy()));
        var ownCopy = first.WithQuery(first.Query.Copy());
        Assert.Same(first.Scope, ownCopy.WithQuery(first.Query.Copy()).Scope);
        Assert.Throws<InvalidOperationException>(() => ownCopy.WithQuery(second.Query.Copy()));
    }

    [Fact]
    public void NativeAliasEscapingAndHeaderFreeze()
    {
        var root = new QueryRequest(new SelectQuery("School").Comment("what: load schools").Purpose("why: render schools"));
        var operation = new QueryOperation("count","CountAs","rows\"\n");
        var count = root.DeriveAggregate(new SelectQuery("School"),"",operation);
        var copy = count.WithQuery(count.Query.Copy().Comment("tampered").Purpose("tampered"));
        Assert.Same(count.Scope,copy.Scope); Assert.Same(count.Intent,copy.Intent);
        Assert.Equal("CountAs(\"rows\\\"\\n\")",copy.Comment);
        Assert.Equal("why: render schools",copy.Purpose);
        Assert.Equal(new[]{"purpose","comment","comment"},copy.Scope.Recover().Select(node=>node.Kind));
        Assert.Equal("QueryRequestScope(<redacted>)",copy.Scope.ToString());
    }

    [Theory]
    [InlineData("sum visible capacity")]
    [InlineData("")]
    [InlineData(" \t")]
    [InlineData("\u0085")]
    public void FacetAggregateExplicitCommentIsPreservedAndBlankIsRejected(string comment)
    {
        var aggregate = new SelectQuery("School").Sum("capacity", "sumCapacity").Comment(comment);
        var types = new SelectQuery("SchoolType").Comment("load types");
        types.RelationAggregates.Add(new("schools", "sumCapacity", aggregate, true) {
            Operation = new("aggregate", "SumCapacityOfSchoolsAs", "sumCapacity", "schools") });
        var query = new SelectQuery("School").Comment("load schools").Purpose("render schools");
        query.Facets.Add(new("types", "schoolType", types, true) {
            Operation = new("facet", "FacetBySchoolTypeAs", "types", "schoolType") });
        if (comment == "sum visible capacity")
        {
            var root = new QueryRequest(query);
            var values = root.Derive(root.Query.Facets[0].Query, "schoolType", root.Query.Facets[0].Operation);
            var relation = values.Query.RelationAggregates[0];
            var request = values.DeriveAggregate(relation.Query, relation.RelationName, relation.Operation);
            Assert.Equal(new[]{"render schools", "load schools", "FacetBySchoolTypeAs(\"types\")",
                "aggregate schoolType", "sum visible capacity"}, Comments(request));
        }
        else Assert.Equal("comment", Assert.Throws<RequestIntentException>(() => new QueryRequest(query)).Field);
    }
}
