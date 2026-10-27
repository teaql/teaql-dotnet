using TeaQL.Core;
using TeaQL.Runtime;

namespace TeaQL.Runtime.Tests;

public class BlobViewPreparationTests
{
    [Fact]
    public async Task IndependentUrlLifetimesUseOriginalCeilingAndReturnMinimum()
    {
        var first = new BlobRef("blob_asset_1");
        var second = new BlobRef("blob_asset_2");
        var ceiling = DateTimeOffset.UtcNow.AddSeconds(90);
        var firstExpiry = DateTimeOffset.UtcNow.AddSeconds(20);
        var secondExpiry = DateTimeOffset.UtcNow.AddSeconds(40);
        var context = new UserContext();
        context.InsertResource<IResponseGraphAuthorizer>(new Authorizer((_, _, _) =>
            new AuthorizedResponseGraph(new Dictionary<string, object?>
            {
                ["first"] = first, ["second"] = second
            }, new[]
            {
                new AuthorizedBlobSlot(new object[] { "first" }, "Asset", "1", "first", "tenant-a", 4, first),
                new AuthorizedBlobSlot(new object[] { "second" }, "Asset", "1", "second", "tenant-a", 5, second)
            })));
        context.InsertResource<IBlobResponseContributor>(new Blob((_, _, supplied) =>
        {
            Assert.Equal(ceiling, supplied);
            return new[]
            {
                new ProjectedBlobUrl("https://files.example.invalid/first", firstExpiry),
                new ProjectedBlobUrl("https://files.example.invalid/second", secondExpiry)
            };
        }));

        var view = await new ViewPreparationService().PrepareAsync(context, null,
            new ViewOptions("ordinary", "read_only", ceiling));
        Assert.Equal(firstExpiry, view.ExpiresAt);
        var presentation = Assert.IsType<Dictionary<string, object?>>(view.Presentation);
        Assert.Equal("https://files.example.invalid/second", presentation["second"]);
    }

    [Fact]
    public async Task CurrentAuthorizationIsDetachedBeforeDocumentAndBlobStages()
    {
        var reference = new BlobRef("blob_asset_1");
        var source = new Dictionary<string, object?>
        {
            ["document"] = reference,
            ["nested"] = new List<object?> { new Dictionary<string, object?> { ["name"] = "untouched" } }
        };
        var stages = new List<string>();
        var ceiling = DateTimeOffset.UtcNow.AddMinutes(5);
        var context = new UserContext();
        context.InsertResource<IResponseGraphAuthorizer>(new Authorizer((_, _, _) =>
        {
            stages.Add("authorize");
            return new AuthorizedResponseGraph(source, new[]
            {
                new AuthorizedBlobSlot(new object[] { "document" }, "Asset", "1", "document",
                    "tenant-a", 4, reference)
            });
        }));
        context.InsertResource<ISecureDocumentContributor>(new Secure((_, graph, _) =>
        {
            stages.Add("secure");
            ((Dictionary<string, object?>)graph.Presentation!)["governed"] = true;
            return new PreparedSecureDocument(graph, "sealed-document", ceiling);
        }));
        context.InsertResource<IBlobResponseContributor>(new Blob((_, graph, expiry) =>
        {
            stages.Add("blob");
            Assert.Equal(ceiling, expiry);
            Assert.Equal(reference, Assert.Single(graph.BlobSlots).Reference);
            return new[] { new ProjectedBlobUrl("https://files.example.invalid/read/asset",
                ceiling.AddMinutes(-1)) };
        }));

        var view = await new ViewPreparationService().PrepareAsync(context, source,
            new ViewOptions("governed", "committed"));
        Assert.Equal(new[] { "authorize", "secure", "blob" }, stages);
        Assert.False(source.ContainsKey("governed"));
        Assert.Same(reference, source["document"]);
        var presentation = Assert.IsType<Dictionary<string, object?>>(view.Presentation);
        Assert.Equal("https://files.example.invalid/read/asset", presentation["document"]);
        Assert.Equal(true, presentation["governed"]);
        Assert.Equal("sealed-document", view.Document);
        Assert.Equal(ceiling.AddMinutes(-1), view.ExpiresAt);
        Assert.Equal("private, no-store", view.CacheControl);
    }

    [Fact]
    public async Task UnlistedBlobAndProviderSecretFailClosedAfterCommit()
    {
        var reference = new BlobRef("blob_private_canary");
        var listed = false;
        var context = new UserContext();
        context.InsertResource<IResponseGraphAuthorizer>(new Authorizer((_, _, _) =>
            new AuthorizedResponseGraph(new Dictionary<string, object?> { ["document"] = reference },
                listed ? new[] { new AuthorizedBlobSlot(new object[] { "document" }, "Asset", "1",
                    "document", "tenant-a", 4, reference) } : Array.Empty<AuthorizedBlobSlot>())));
        var service = new ViewPreparationService();
        var options = new ViewOptions("ordinary", "committed");
        var first = await Assert.ThrowsAsync<ViewPreparationException>(() =>
            service.PrepareAsync(context, null, options));
        Assert.Equal("BLOB_CAPABILITY_MISSING", first.Code);
        Assert.Equal("committed", first.BusinessState);
        Assert.Equal("read", first.Retry);
        Assert.DoesNotContain(reference.Key, first.Message);

        listed = true;
        context.InsertResource<IBlobResponseContributor>(new Blob((_, _, _) =>
            throw new InvalidOperationException("private URL https://secret.invalid/read?sig=canary")));
        var second = await Assert.ThrowsAsync<ViewPreparationException>(() =>
            service.PrepareAsync(context, null, options));
        Assert.Equal("VIEW_PREPARATION_FAILED", second.Code);
        Assert.DoesNotContain("secret.invalid", second.Message);
    }

    private sealed class Authorizer(
        Func<UserContext, object?, ViewOptions, AuthorizedResponseGraph> work) : IResponseGraphAuthorizer
    {
        public Task<AuthorizedResponseGraph> AuthorizeAsync(UserContext context, object? result,
            ViewOptions options) => Task.FromResult(work(context, result, options));
    }

    private sealed class Secure(
        Func<UserContext, AuthorizedResponseGraph, ViewOptions, PreparedSecureDocument> work)
        : ISecureDocumentContributor
    {
        public Task<PreparedSecureDocument> PrepareAsync(UserContext context,
            AuthorizedResponseGraph graph, ViewOptions options)
            => Task.FromResult(work(context, graph, options));
    }

    private sealed class Blob(
        Func<UserContext, AuthorizedResponseGraph, DateTimeOffset?, IReadOnlyList<ProjectedBlobUrl>> work)
        : IBlobResponseContributor
    {
        public Task<IReadOnlyList<ProjectedBlobUrl>> ProjectBatchAsync(UserContext context,
            AuthorizedResponseGraph graph, DateTimeOffset? expiresAt)
            => Task.FromResult(work(context, graph, expiresAt));
    }
}
