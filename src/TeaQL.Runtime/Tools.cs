using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace TeaQL.Runtime;

public enum ToolRisk { MemoryOnly, ExternalResource, Privileged }

public sealed record ToolToken<T>(string Id, ToolRisk Risk);

public interface IToolProvider
{
    string Id { get; }
    object Create(UserContext context);
}

public sealed class ToolPolicy
{
    private readonly HashSet<string> _allowed;
    private readonly bool _allowMemoryOnly;

    private ToolPolicy(IEnumerable<string> allowed, bool allowMemoryOnly)
    {
        _allowed = new HashSet<string>(allowed, StringComparer.Ordinal);
        _allowMemoryOnly = allowMemoryOnly;
    }

    public static ToolPolicy Standard { get; } = new([], true);
    public static ToolPolicy DenyAll { get; } = new([], false);
    public static ToolPolicy Allowing<T>(params ToolToken<T>[] tokens) =>
        new(tokens.Select(token => token.Id), true);
    internal bool Allows<T>(ToolToken<T> token) =>
        (token.Risk == ToolRisk.MemoryOnly && _allowMemoryOnly) || _allowed.Contains(token.Id);
}

public sealed class Tools
{
    private readonly UserContext _context;
    private readonly ToolPolicy _policy;
    private readonly IReadOnlyDictionary<string, IToolProvider> _providers;

    internal Tools(UserContext context, ToolPolicy policy, IEnumerable<IToolProvider> providers)
    {
        _context = context;
        _policy = policy;
        _providers = new ReadOnlyDictionary<string, IToolProvider>(
            providers.ToDictionary(provider => provider.Id, StringComparer.Ordinal));
    }

    public bool Has<T>(ToolToken<T> token) => _providers.ContainsKey(token.Id);
    public T Get<T>(ToolToken<T> token)
    {
        if (!_providers.TryGetValue(token.Id, out var provider))
            throw new ToolUnavailableException(token.Id);
        if (!_policy.Allows(token)) throw new ToolDeniedException(token.Id);
        return (T)provider.Create(_context);
    }
    public IReadOnlyCollection<string> Descriptors() => _providers.Keys.ToArray();
}

public static class ContextTools
{
    public static ContextToolsBuilder Builder(UserContext context) => new(context);
    public static Tools Of(UserContext context) => Builder(context).Build();
}

public sealed class ContextToolsBuilder
{
    private readonly UserContext _context;
    private ToolPolicy _policy = ToolPolicy.Standard;
    private readonly List<IToolProvider> _providers = [];
    internal ContextToolsBuilder(UserContext context) => _context = context;
    public ContextToolsBuilder Policy(ToolPolicy policy) { _policy = policy; return this; }
    public ContextToolsBuilder Provider(IToolProvider provider) { _providers.Add(provider); return this; }
    public Tools Build() => new(_context, _policy, _providers);
}

public sealed class ToolUnavailableException(string id) : InvalidOperationException($"Tool not available: {id}");
public sealed class ToolDeniedException(string id) : UnauthorizedAccessException($"Tool denied by policy: {id}");

public sealed record HttpToolResponse(int Status, byte[] Body);

public interface IHttpToolTransport
{
    Task<HttpToolResponse> SendAsync(string method, string url, byte[]? body,
        CancellationToken cancellationToken = default);
}

public interface IHttpTool
{
    HttpIntentPhase Get(string url);
    HttpIntentPhase Post(string url, object? body);
}

public static class HttpTools
{
    public static ToolToken<IHttpTool> Token { get; } = new("http", ToolRisk.ExternalResource);
}

public sealed class HttpToolProvider(IHttpToolTransport transport) : IToolProvider
{
    public string Id => HttpTools.Token.Id;
    public object Create(UserContext context) { _ = context; return new HttpTool(transport); }
}

internal sealed class HttpTool(IHttpToolTransport transport) : IHttpTool
{
    public HttpIntentPhase Get(string url) => new(transport, "GET", url, null);
    public HttpIntentPhase Post(string url, object? body) =>
        new(transport, "POST", url, JsonSerializer.SerializeToUtf8Bytes(body));
}

public sealed class HttpIntentPhase(
    IHttpToolTransport transport, string method, string url, byte[]? body)
{
    public ExecutableHttpTool Purpose(string intent) => new(transport, method, url, body, intent);
    public ExecutableHttpTool AuditAs(string intent) => new(transport, method, url, body, intent);
}

public sealed class ExecutableHttpTool(
    IHttpToolTransport transport, string method, string url, byte[]? body, string intent)
{
    public async Task<string> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(intent))
            throw new InvalidOperationException("HTTP tool execution requires non-empty intent");
        var response = await transport.SendAsync(method, url, body, cancellationToken);
        if (response.Status is < 200 or >= 300)
            throw new InvalidOperationException($"HTTP tool failed: {response.Status}");
        return Encoding.UTF8.GetString(response.Body);
    }
}
