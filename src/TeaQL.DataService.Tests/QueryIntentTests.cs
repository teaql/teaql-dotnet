using TeaQL.Core;
using Xunit;

namespace TeaQL.DataService.Tests;

public class QueryIntentTests
{
    [Fact]
    public void RequestCapturesQueryIntent()
    {
        var query = new SelectQuery("School").Comment("what: load schools")
            .Purpose("why: verify generated request intent");
        var request = new QueryRequest(query);
        Assert.Equal(query.CommentText, request.Comment);
        Assert.Equal(query.PurposeText, request.Purpose);
        Assert.NotSame(query, request.Query);
        query.Comment("changed builder").Purpose("changed purpose");
        Assert.Equal("what: load schools", request.Comment);
        Assert.Equal("why: verify generated request intent", request.Purpose);
    }

    [Fact]
    public void ExplicitRequestIntentTakesPrecedenceWithoutChangingQuery()
    {
        var query = new SelectQuery("School").Comment("query comment").Purpose("query purpose");
        var request = new QueryRequest(query, new QueryIntent("request comment", "request purpose"));
        Assert.Equal("request comment", request.Comment);
        Assert.Equal("request purpose", request.Purpose);
        Assert.Equal("query comment", query.CommentText);
        Assert.Equal("query purpose", query.PurposeText);
    }

    [Fact]
    public void MissingIntentIsNotInvented()
    {
        var error = Assert.Throws<RequestIntentException>(() => new QueryRequest(new SelectQuery("School")));
        Assert.Equal("REQUEST_COMMENT_REQUIRED", error.Code);
    }

    [Fact]
    public void ExplicitEmptyIntentIsNotReplaced()
    {
        var query = new SelectQuery("School").Comment("query comment").Purpose("query purpose");
        var error = Assert.Throws<RequestIntentException>(() =>
            new QueryRequest(query, new QueryIntent("", "")));
        Assert.Equal("REQUEST_COMMENT_REQUIRED", error.Code);
    }

    [Fact]
    public void DerivedQueryRetainsRootIntentAndDoesNotAcceptNestedOverride()
    {
        var root = new QueryRequest(new SelectQuery("School").Comment("load school graph").Purpose("show school"));
        var child = root.WithQuery(new SelectQuery("Platform").Comment("child override").Purpose("child override"));
        Assert.Same(root.Intent, child.Intent);
        Assert.Equal("load school graph", child.Comment);
        Assert.Equal("show school", child.Purpose);
    }
}
