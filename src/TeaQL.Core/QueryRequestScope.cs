using System.Text.Encodings.Web;
using System.Text.Json;

namespace TeaQL.Core;

/// <summary>Immutable generated native API metadata; never query wire input.</summary>
public sealed record QueryOperation(string Kind, string Method, string? Alias = null, string? Relation = null)
{
    public string Render()
    {
        if (string.IsNullOrWhiteSpace(Kind) || string.IsNullOrWhiteSpace(Method))
            throw new ArgumentException("Query operation requires kind and native method");
        var args = Alias == null ? "" : JsonSerializer.Serialize(Alias,
            new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        if (Kind == "relation") args += (args.Length == 0 ? "" : ", ") + "...";
        return $"{Method}({args})";
    }
}

/// <summary>Small immutable headers, not an AST, Context, entity or mutation ledger.</summary>
public sealed class QueryRequestScope
{
    public QueryRequestScope? Parent { get; }
    public QueryRequestScope Root { get; }
    public string Entity { get; }
    public string Comment { get; }
    public string Purpose => Root.intent!.Purpose;
    public QueryOperation? Operation { get; }
    internal string? ExplicitComment { get; }
    private readonly QueryIntent? intent;
    private readonly string? facetRelation;

    public QueryRequestScope(string entity, QueryIntent intent)
    {
        ArgumentNullException.ThrowIfNull(intent);
        Entity = entity; Root = this; this.intent = intent;
        Comment = intent.Comment; ExplicitComment = intent.Comment;
    }

    private QueryRequestScope(QueryRequestScope parent, string entity, string comment,
        QueryOperation? operation = null, string? explicitComment = null, string? facetRelation = null)
    {
        Parent = parent; Root = parent.Root; Entity = entity;
        Comment = RequestIntentValidation.Required(comment, "comment", "query");
        Operation = operation; ExplicitComment = explicitComment; this.facetRelation = facetRelation;
    }

    public QueryRequestScope DeriveOperation(QueryOperation operation) =>
        new(this, Entity, operation.Render(), operation);
    public QueryRequestScope Child(string entity, string? comment) =>
        new(this, entity, comment ?? $"load {entity}", explicitComment: comment);
    public QueryRequestScope Helper(string entity, string comment) => new(this, entity, comment);
    public QueryRequestScope Facet(string relation, string alias, QueryOperation? operation) =>
        operation == null ? new(this, Entity, $"facet {relation} as {alias}", facetRelation: relation) : DeriveOperation(operation);
    public bool IsFacetOperation => Operation?.Kind == "facet" || facetRelation != null;
    public string? Relation => Operation?.Relation ?? facetRelation;
    public override string ToString() => "QueryRequestScope(<redacted>)";

    public IReadOnlyList<TraceNode> Recover()
    {
        var nodes = new Stack<QueryRequestScope>();
        for (var scope = this; scope != null; scope = scope.Parent) nodes.Push(scope);
        var result = new List<TraceNode> {
            new(Root.Entity, null, Purpose) { Kind = "purpose", Name = Root.Entity, Detail = Purpose }
        };
        while (nodes.TryPop(out var node))
            result.Add(new TraceNode(node.Entity, null, node.Comment) {
                Kind = "comment", Name = node.Operation?.Method ?? node.Entity, Detail = node.Comment
            });
        return result.AsReadOnly();
    }
}
