using System.Text;
using TeaQL.Runtime;

namespace TeaQL.Runtime.Tests;

public class ToolsTests
{
    private sealed class StubTransport(int status = 200) : IHttpToolTransport
    {
        public Task<HttpToolResponse> SendAsync(string method, string url, byte[]? body,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new HttpToolResponse(status, Encoding.UTF8.GetBytes($"{method}:{url}")));
    }

    [Fact]
    public async Task PolicyGatedHttpPreservesNativeString()
    {
        var tools = ContextTools.Builder(new UserContext())
            .Policy(ToolPolicy.Allowing(HttpTools.Token))
            .Provider(new HttpToolProvider(new StubTransport())).Build();
        string value = await tools.Get(HttpTools.Token).Get("https://example.com")
            .Purpose("status").ExecuteAsync();
        Assert.Equal("GET:https://example.com", value);
    }

    [Fact]
    public async Task DeniedUnknownBlankIntentAndStatusAreExplicit()
    {
        var denied = ContextTools.Builder(new UserContext())
            .Provider(new HttpToolProvider(new StubTransport())).Build();
        Assert.Throws<ToolDeniedException>(() => denied.Get(HttpTools.Token));
        Assert.Throws<ToolUnavailableException>(() => denied.Get(new ToolToken<object>("unknown", ToolRisk.MemoryOnly)));

        var allowed = ContextTools.Builder(new UserContext())
            .Policy(ToolPolicy.Allowing(HttpTools.Token))
            .Provider(new HttpToolProvider(new StubTransport())).Build();
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            allowed.Get(HttpTools.Token).Get("https://example.com").Purpose(" ").ExecuteAsync());
        var failed = ContextTools.Builder(new UserContext())
            .Policy(ToolPolicy.Allowing(HttpTools.Token))
            .Provider(new HttpToolProvider(new StubTransport(503))).Build();
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            failed.Get(HttpTools.Token).Get("https://example.com").AuditAs("health").ExecuteAsync());
    }
}
