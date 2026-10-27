using System.Collections;
using TeaQL.Core;

namespace TeaQL.Runtime;

/// <summary>Chosen by the trusted host after business work and its transaction finish.</summary>
public sealed record ViewOptions(string Mode, string BusinessOutcome, DateTimeOffset? ExpiresAt = null);

/// <summary>A currently authorized field position, never reconstructed from client JSON.</summary>
public sealed record AuthorizedBlobSlot(
    IReadOnlyList<object> Path, string Entity, string EntityId, string Field,
    string Scope, long BindingVersion, BlobRef Reference);

public sealed record AuthorizedResponseGraph(
    object? Presentation, IReadOnlyList<AuthorizedBlobSlot> BlobSlots);

public interface IResponseGraphAuthorizer
{
    Task<AuthorizedResponseGraph> AuthorizeAsync(UserContext context, object? result,
        ViewOptions options);
}

public sealed record PreparedSecureDocument(
    AuthorizedResponseGraph Graph, object? Envelope, DateTimeOffset ExpiresAt);

public interface ISecureDocumentContributor
{
    Task<PreparedSecureDocument> PrepareAsync(UserContext context,
        AuthorizedResponseGraph graph, ViewOptions options);
}

public sealed record ProjectedBlobUrl(string Url, DateTimeOffset ExpiresAt);

public interface IBlobResponseContributor
{
    Task<IReadOnlyList<ProjectedBlobUrl>> ProjectBatchAsync(UserContext context,
        AuthorizedResponseGraph graph, DateTimeOffset? expiresAt);
}

public sealed record PreparedView(
    object? Presentation, object? Document, DateTimeOffset? ExpiresAt,
    string CacheControl = "private, no-store");

public sealed class ViewPreparationException : Exception
{
    public string Code { get; }
    public string BusinessState { get; }
    public string Retry => BusinessState == "committed" ? "read" : "prepare";

    public ViewPreparationException(string code, string businessState = "read_only")
        : base(code)
    {
        Code = code;
        BusinessState = businessState;
    }
}

/// <summary>Authorize once, detach, apply optional governance, then project Blob URLs.</summary>
public sealed class ViewPreparationService
{
    private static readonly HashSet<string> SafeCodes = new(StringComparer.Ordinal)
    {
        "VIEW_OPTIONS_INVALID", "VIEW_AUTHORIZER_MISSING", "VIEW_GRAPH_INVALID",
        "DOCUMENT_CONFIGURATION", "BLOB_CAPABILITY_MISSING", "BLOB_SIGN_FAILED",
        "BLOB_READ_DENIED", "BLOB_REFERENCE_EXPIRED", "BLOB_SCHEMA_MISSING",
        "BLOB_CATALOG_FAILED"
    };

    public async Task<PreparedView> PrepareAsync(UserContext context, object? result,
        ViewOptions options)
    {
        var state = options?.BusinessOutcome == "committed" ? "committed" : "read_only";
        try
        {
            if (context == null || options == null
                || options.Mode is not ("ordinary" or "governed")
                || options.BusinessOutcome is not ("read_only" or "committed")
                || options.ExpiresAt is { } supplied && supplied <= DateTimeOffset.UtcNow)
                throw Failure("VIEW_OPTIONS_INVALID");

            var authorizer = context.GetResource<IResponseGraphAuthorizer>()
                ?? throw Failure("VIEW_AUTHORIZER_MISSING");
            var graph = await authorizer.AuthorizeAsync(context, result, options)
                .ConfigureAwait(false);
            graph = DetachedGraph(graph);
            object? document = null;
            var expiry = options.ExpiresAt;
            if (options.Mode == "governed")
            {
                var secure = context.GetResource<ISecureDocumentContributor>()
                    ?? throw Failure("DOCUMENT_CONFIGURATION");
                var prepared = await secure.PrepareAsync(context, graph, options)
                    .ConfigureAwait(false);
                if (prepared.ExpiresAt <= DateTimeOffset.UtcNow)
                    throw Failure("DOCUMENT_CONFIGURATION");
                graph = DetachedGraph(prepared.Graph);
                document = prepared.Envelope;
                expiry = Earlier(expiry, prepared.ExpiresAt);
            }
            var presentation = Clone(graph.Presentation, 0, new Budget());
            var paths = new HashSet<string>(StringComparer.Ordinal);
            foreach (var slot in graph.BlobSlots)
            {
                var path = PathKey(slot.Path);
                if (!paths.Add(path) || AtPath(presentation, slot.Path) is not BlobRef reference
                    || !reference.Equals(slot.Reference))
                    throw Failure("VIEW_GRAPH_INVALID");
            }
            if (graph.BlobSlots.Count > 0)
            {
                var blob = context.GetResource<IBlobResponseContributor>()
                    ?? throw Failure("BLOB_CAPABILITY_MISSING");
                var ceiling = expiry;
                var urls = await blob.ProjectBatchAsync(context, graph, ceiling)
                    .ConfigureAwait(false);
                if (urls.Count != graph.BlobSlots.Count) throw Failure("BLOB_SIGN_FAILED");
                for (var index = 0; index < urls.Count; index++)
                {
                    var signed = urls[index];
                    if (!Uri.TryCreate(signed.Url, UriKind.Absolute, out var uri)
                        || uri.Scheme is not ("http" or "https") || string.IsNullOrEmpty(uri.Host)
                        || signed.ExpiresAt <= DateTimeOffset.UtcNow
                        || ceiling is { } limit && signed.ExpiresAt > limit)
                        throw Failure("BLOB_SIGN_FAILED");
                    Replace(presentation, graph.BlobSlots[index].Path, signed.Url);
                    expiry = Earlier(expiry, signed.ExpiresAt);
                }
            }
            if (ContainsBlob(presentation, 0, new Budget()))
                throw Failure("BLOB_CAPABILITY_MISSING");
            return new PreparedView(presentation, document, expiry);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            var code = exception is ViewPreparationException known && SafeCodes.Contains(known.Code)
                ? known.Code : "VIEW_PREPARATION_FAILED";
            throw new ViewPreparationException(code, state);
        }
    }

    private static ViewPreparationException Failure(string code) => new(code);

    private static AuthorizedResponseGraph DetachedGraph(AuthorizedResponseGraph graph)
    {
        if (graph.BlobSlots == null || graph.BlobSlots.Count > 10_000)
            throw Failure("VIEW_GRAPH_INVALID");
        var slots = graph.BlobSlots.Select(slot => slot with { Path = slot.Path.ToArray() }).ToArray();
        return new AuthorizedResponseGraph(Clone(graph.Presentation, 0, new Budget()), slots);
    }

    private static DateTimeOffset Earlier(DateTimeOffset? left, DateTimeOffset right)
        => left is { } value && value < right ? value : right;

    private sealed class Budget { public int Remaining = 100_000; }

    private static object? Clone(object? value, int depth, Budget budget)
    {
        if (--budget.Remaining < 0 || depth > 64) throw Failure("VIEW_GRAPH_INVALID");
        if (value == null || value is string or bool or byte or short or int or long
            or sbyte or ushort or uint or ulong or float or double or decimal
            or DateTimeOffset or DateTime or BlobRef)
            return value;
        if (value is Dictionary<string, object?> map)
        {
            if (map.Count > budget.Remaining) throw Failure("VIEW_GRAPH_INVALID");
            var result = new Dictionary<string, object?>(map.Count, StringComparer.Ordinal);
            foreach (var (key, child) in map)
                result[key] = Clone(child, depth + 1, budget);
            return result;
        }
        if (value is List<object?> list)
        {
            if (list.Count > budget.Remaining) throw Failure("VIEW_GRAPH_INVALID");
            return list.Select(child => Clone(child, depth + 1, budget)).ToList();
        }
        throw Failure("VIEW_GRAPH_INVALID");
    }

    private static string PathKey(IReadOnlyList<object> path)
    {
        if (path == null || path.Count is 0 or > 64) throw Failure("VIEW_GRAPH_INVALID");
        var parts = new System.Text.StringBuilder();
        foreach (var part in path)
        {
            switch (part)
            {
                case string text when text.Length > 0:
                    parts.Append('s').Append(text.Length).Append(':').Append(text);
                    break;
                case int index when index >= 0:
                    parts.Append('i').Append(index).Append(':');
                    break;
                default: throw Failure("VIEW_GRAPH_INVALID");
            }
        }
        return parts.ToString();
    }

    private static object? AtPath(object? root, IReadOnlyList<object> path)
    {
        var current = root;
        foreach (var component in path)
        {
            if (component is string field && current is Dictionary<string, object?> map
                && map.TryGetValue(field, out var child)) current = child;
            else if (component is int index && current is List<object?> list
                     && index >= 0 && index < list.Count) current = list[index];
            else throw Failure("VIEW_GRAPH_INVALID");
        }
        return current;
    }

    private static void Replace(object? root, IReadOnlyList<object> path, string url)
    {
        _ = AtPath(root, path);
        var parent = AtPath(root, path.Take(path.Count - 1).ToArray());
        switch (path[^1])
        {
            case string field when parent is Dictionary<string, object?> map:
                map[field] = url;
                break;
            case int index when parent is List<object?> list:
                list[index] = url;
                break;
            default: throw Failure("VIEW_GRAPH_INVALID");
        }
    }

    private static bool ContainsBlob(object? value, int depth, Budget budget)
    {
        if (--budget.Remaining < 0 || depth > 64) return true;
        if (value is BlobRef) return true;
        if (value is Dictionary<string, object?> map)
            return map.Values.Any(child => ContainsBlob(child, depth + 1, budget));
        if (value is List<object?> list)
            return list.Any(child => ContainsBlob(child, depth + 1, budget));
        return false;
    }
}
