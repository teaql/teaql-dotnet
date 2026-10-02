using System.Text.Json;
using TeaQL.Core;
using Xunit;

namespace TeaQL.DataService.Tests;

public sealed class RequestIntentVectorTests
{
    public static IEnumerable<object[]> SharedCases()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(
            Path.Combine(AppContext.BaseDirectory, "request-intent-v1.json")));
        foreach (var item in document.RootElement.GetProperty("cases").EnumerateArray())
            yield return new object[] { item.GetProperty("id").GetString()!, item.GetRawText() };
    }

    [Theory]
    [MemberData(nameof(SharedCases))]
    public void SharedRequestIntentIsRequiredAndNeverComesFromTraceOrChildren(string id, string json)
    {
        Assert.False(string.IsNullOrWhiteSpace(id));
        using var document = JsonDocument.Parse(json);
        var item = document.RootElement;
        var input = item.GetProperty("input");
        var kind = item.GetProperty("kind").GetString()!;
        var comment = Text(input, "comment");
        var purpose = Text(input, "purpose");
        // Trace, children and log controls are deliberately not intent sources.
        string ActualComment()
        {
            if (kind == "query") return new QueryRequest(new SelectQuery("Probe"),
                new QueryIntent(comment, purpose)).Comment;
            if (input.TryGetProperty("children", out _))
                return new BatchMutationRequest(new() {
                    new InsertMutationRequest(new InsertCommand("Probe"), "create item")
                }, comment).Comment;
            return new InsertMutationRequest(new InsertCommand("Probe") {
                TraceChain = new() { new TraceNode("Probe", 1, "trace-only reason") }
            }, comment).Comment;
        }
        if (item.TryGetProperty("error", out var expectedError))
        {
            var error = Assert.Throws<RequestIntentException>(() => ActualComment());
            Assert.Equal(expectedError.GetProperty("code").GetString(), error.Code);
            Assert.Equal(expectedError.GetProperty("field").GetString(), error.Field);
            Assert.Equal(kind, error.RequestKind);
            Assert.DoesNotContain("trace-only reason", error.ToString());
        }
        else
        {
            var expected = item.GetProperty("expected");
            Assert.Equal(expected.GetProperty("comment").GetString(), ActualComment());
            if (kind == "query")
                Assert.Equal(expected.GetProperty("purpose").GetString(), new QueryIntent(comment, purpose).Purpose);
        }
    }

    [Fact]
    public void CommentIsNotTheLastDiagnosticRouteAndCannotBeOverwritten()
    {
        var command = new InsertCommand("Order");
        var request = new InsertMutationRequest(command, "submit order");
        command.TraceChain.Add(new TraceNode("OrderItem", 1, ""));
        Assert.Equal("submit order", request.Comment);
        command.TraceChain.Add(new TraceNode("Payment", 1, "authorize payment"));
        Assert.Equal("submit order", request.Comment);
        Assert.Null(typeof(MutationRequest).GetProperty("Comment")!.SetMethod);
        Assert.Null(typeof(QueryIntent).GetProperty("Comment")!.SetMethod);
    }

    [Fact]
    public void BatchCannotBorrowAChildReasonAndReadbackHasExplicitRuntimePurpose()
    {
        var child = new InsertMutationRequest(new InsertCommand("OrderItem"), "add item");
        Assert.Throws<RequestIntentException>(() => new BatchMutationRequest(new() { child }, null));
        var request = new BatchMutationRequest(new() { child }, "submit order");
        Assert.Equal("submit order", request.Comment);
        Assert.Equal("submit order", request.Intent.ReadbackIntent().Comment);
        Assert.Equal("verify the persisted mutation result", request.Intent.ReadbackIntent().Purpose);
    }

    [Fact]
    public void DerivedMutationPreservesLedgerIdentityButDoesNotChangeTheChildIntent()
    {
        var ledger = new EntityRoot();
        var key = new EntityKey("OrderItem", 1);
        var child = MutationRequest.Create(new InsertCommand("OrderItem"), "add item", key, ledger);
        var derived = child.WithRootIntent(new MutationIntent("submit order"));
        Assert.Same(ledger, derived.LedgerRoot);
        Assert.Equal(key, derived.LedgerKey);
        Assert.Equal("submit order", derived.Comment);
        Assert.Equal("add item", child.Comment);
    }

    [Fact]
    public void FailureDoesNotEchoAnotherIntentOrMutationPayload()
    {
        var queryError = Assert.Throws<RequestIntentException>(() =>
            new QueryIntent("PRIVATE-QUERY-CANARY", ""));
        Assert.DoesNotContain("PRIVATE-QUERY-CANARY", queryError.ToString());
        var mutationError = Assert.Throws<RequestIntentException>(() =>
            new InsertMutationRequest(new InsertCommand("Probe") {
                Values = new TeaQL.Core.Record { ["secret"] = new Value.TextValue("PRIVATE-MUTATION-CANARY") }
            }, null));
        Assert.DoesNotContain("PRIVATE-MUTATION-CANARY", mutationError.ToString());
    }

    [Fact]
    public void UnicodeWhitespaceMatchesRustAndPreservesNonWhitespaceControls()
    {
        const string blank = "\u0009\u000a\u000b\u000c\u000d\u0020\u0085\u00a0\u1680\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u2028\u2029\u202f\u205f\u3000";
        foreach (var value in blank)
            Assert.Throws<RequestIntentException>(() => new MutationIntent(value.ToString()));
        foreach (var value in new[] { '\u001c', '\u001d', '\u001e', '\u001f', '\u200b', '\ufeff' })
            Assert.Equal(value.ToString(), new MutationIntent(value.ToString()).Comment);
    }

    private static string? Text(JsonElement item, string field) =>
        item.TryGetProperty(field, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
}
