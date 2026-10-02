namespace TeaQL.Core;

/// <summary>Persistent immutable parent token owned by one graph invocation.</summary>
public sealed class MutationTraceScope
{
    public MutationTraceScope? Parent { get; }
    public TraceNode Node { get; }

    public MutationTraceScope(string entityType, ulong? id, string reason, MutationTraceScope? parent = null)
    {
        if (string.IsNullOrWhiteSpace(entityType)) throw new ArgumentException("Entity type is required", nameof(entityType));
        Parent = parent;
        Node = new TraceNode(entityType, id, "") { Kind = "auditReason", Detail = new MutationIntent(reason).Comment };
    }

    public IReadOnlyList<TraceNode> Recover()
    {
        var nodes = new List<TraceNode>();
        for (var cursor = this; cursor != null; cursor = cursor.Parent) nodes.Add(cursor.Node);
        nodes.Reverse();
        return nodes.AsReadOnly();
    }
}
