namespace TeaQL.Core;

public sealed record SqlTraceProjection(IReadOnlyList<TraceNode> TracePath,
    string? Comment, string? Purpose, string? AuditReason);

/// <summary>Pure Rust-baseline SQL path algorithm, not request validation.</summary>
public static class SqlTraceChain
{
    public static SqlTraceProjection Canonical(IEnumerable<TraceNode> source, string backend, string operation)
    {
        if (operation is not ("select" or "insert" or "update" or "delete" or "recover"))
            throw new ArgumentException("Unsupported SQL trace operation", nameof(operation));
        var nodes = source.ToArray();
        bool Is(TraceNode node, string kind) => string.Equals(node.Kind, kind, StringComparison.OrdinalIgnoreCase);
        bool IsIntent(TraceNode node) => Is(node, "comment") || Is(node, "purpose") || Is(node, "auditReason");
        string? Last(string kind) => nodes.LastOrDefault(node => Is(node, kind))?.Detail;
        List<TraceNode> path;
        if (nodes.Any(node => Is(node, "operation")) && nodes.Any(node => Is(node, "provider"))
            && nodes.Any(node => Is(node, "sql")))
            path = nodes.Where(node => !IsIntent(node)).Select(node => node with { }).ToList();
        else
        {
            var root = nodes.FirstOrDefault(node => !string.IsNullOrWhiteSpace(node.Name))?.Name ?? "unknown";
            var entity = operation == "select" ? root
                : nodes.LastOrDefault(node => Is(node, "entity") && !string.IsNullOrWhiteSpace(node.Name))?.Name ?? root;
            TraceNode Node(string kind, string name, string detail = "") =>
                new(name, null, "") { Kind = kind, Name = name, Detail = detail };
            path = new() { Node("operation", root, operation == "select" ? "query" : "mutation"),
                Node(operation == "select" ? "request" : "entity", entity) };
            path.AddRange(nodes.Where(node => Is(node, "relation")).Select(node => node with { }));
            path.Add(Node("provider", string.IsNullOrWhiteSpace(backend) ? "unknown" : backend));
            path.Add(Node("sql", operation));
        }
        return new(path.AsReadOnly(), Last("comment"), Last("purpose"), Last("auditReason"));
    }
}
