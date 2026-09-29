using TeaQL.Core;
using Xunit;

namespace TeaQL.DataService.Tests;

public class QueryIntentTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RequestInheritsQueryIntent(bool initializer)
    {
        var query = new SelectQuery("School").Comment("what: load schools")
            .Purpose("why: verify generated request intent");
        var request = initializer ? new QueryRequest { Query = query } : new QueryRequest(query);
        Assert.Equal(query.CommentText, request.Comment);
        Assert.Equal(query.PurposeText, request.Purpose);
        Assert.Same(query, request.Query);
    }

    [Fact]
    public void ExplicitRequestIntentTakesPrecedenceWithoutChangingQuery()
    {
        var query = new SelectQuery("School").Comment("query comment").Purpose("query purpose");
        var request = new QueryRequest(query) { Comment = "request comment", Purpose = "request purpose" };
        Assert.Equal("request comment", request.Comment);
        Assert.Equal("request purpose", request.Purpose);
        Assert.Equal("query comment", query.CommentText);
        Assert.Equal("query purpose", query.PurposeText);
    }

    [Fact]
    public void MissingIntentIsNotInvented()
    {
        var request = new QueryRequest(new SelectQuery("School"));
        Assert.Null(request.Comment);
        Assert.Null(request.Purpose);
    }

    [Fact]
    public void ExplicitEmptyIntentIsNotReplaced()
    {
        var request = new QueryRequest(new SelectQuery("School").Comment("query comment").Purpose("query purpose"))
            { Comment = "", Purpose = "" };
        Assert.Equal("", request.Comment);
        Assert.Equal("", request.Purpose);
    }
}
