using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using TeaQL.Core;

namespace TeaQL.DataService;

public class DataServiceCapabilities
{
    public bool Query { get; set; }
    public bool Mutation { get; set; }
    public bool Transaction { get; set; }
    public bool Schema { get; set; }
    public bool IdGeneration { get; set; }
    public bool BatchMutation { get; set; }
    public bool Returning { get; set; }
}

public class QueryRequest
{
    // Execution-local acknowledgement for typed streaming. Never serialized or
    // shared by two enumerations; raw chunk consumers retain chunk semantics.
    internal StreamDelivery? StreamDelivery { get; set; }
    internal Func<string, DataServiceOperation, ISqlExecutionObservation>? StatementObserver { get; set; }
    // Execution-local callback, never part of a serialized request.
    internal Action<ExecutionMetadata>? DiagnosticObserver { get; set; }
    // Bind provenance for inherited intent on derived queries, not shared context state.
    internal ExecutionMetadata? IntentSource { get; set; }
    // One derived relation load captures scalar join keys before hydration.
    // Not inherited by descendants, serialized, or exposed in result records.
    internal Action<IReadOnlyList<Record>>? CaptureRelationKeys { get; set; }
    public SelectQuery Query { get; }
    [System.Text.Json.Serialization.JsonIgnore] public List<TraceNode> TraceChain { get; set; } = new();
    // Immutable runtime-owned source. Public diagnostic frames are not provenance.
    internal IReadOnlyList<TraceNode> TraceSource { get; private init; }
    public QueryIntent Intent { get; }
    [System.Text.Json.Serialization.JsonIgnore] public QueryRequestScope Scope { get; }
    public string Comment => Intent.Comment;
    public string Purpose => Intent.Purpose;
    /// <summary>Runtime-only observer; providers must not serialize it.</summary>
    public IRelationLoadObserver? RelationLoadObserver { get; set; }
    public QueryRequest(SelectQuery query)
        : this(query, query.RequestScope ?? new QueryRequestScope(query.Entity, new QueryIntent(query.CommentText, query.PurposeText))) { }

    public QueryRequest(SelectQuery query, QueryIntent intent)
        : this(query, new QueryRequestScope(query.Entity, intent), intent) { }

    private QueryRequest(SelectQuery query, QueryRequestScope scope, QueryIntent? capturedIntent = null)
    {
        ArgumentNullException.ThrowIfNull(query);
        Scope = scope;
        Intent = capturedIntent ?? new QueryIntent(scope.Comment, scope.Purpose);
        Query = query.CloneForExecution();
        BindTree(Query, scope);
        TraceSource = Array.AsReadOnly(new[] {
            new TraceNode(scope.Root.Entity, null, "") { Kind = "comment", Detail = scope.Root.Comment },
            new TraceNode(scope.Root.Entity, null, "") { Kind = "purpose", Detail = scope.Purpose }
        });
    }

    /// <summary>Derived work inherits validated root intent, not nested builder prose.</summary>
    public QueryRequest WithQuery(SelectQuery query)
    {
        if (query.RequestScope is { } previous && !ReferenceEquals(previous.Root, Scope.Root))
            throw new InvalidOperationException("QUERY_SCOPE_OWNER_MISMATCH: replacement belongs to another query invocation");
        return new(query, Scope, Intent)
        {
            TraceChain = new List<TraceNode>(TraceChain), IntentSource = IntentSource,
            TraceSource = TraceSource,
            RelationLoadObserver = RelationLoadObserver, DiagnosticObserver = DiagnosticObserver, StatementObserver = StatementObserver,
            StreamDelivery = StreamDelivery
        };
    }

    public QueryRequest Derive(SelectQuery query, string relation, QueryOperation? operation = null) => new(query,
        ReusableChildScope(query) ?? ChildScope(Scope, query, relation, operation))
    {
        TraceSource = Array.AsReadOnly(TraceSource.Append(
            new TraceNode(query.Entity, null, "") { Kind = "relation", Name = relation,
                Detail = $"{Query.Entity}.{relation}" }).ToArray()),
        IntentSource = IntentSource, RelationLoadObserver = RelationLoadObserver,
        DiagnosticObserver = DiagnosticObserver, StatementObserver = StatementObserver
    };

    public QueryRequest DeriveAggregate(SelectQuery query, string relation, QueryOperation? operation = null)
    {
        var scope = ReusableChildScope(query) ?? AggregateScope(Scope, query, relation, operation);
        return DerivedAt(query, scope!, relation);
    }

    public QueryRequest DeriveAt(SelectQuery query, QueryRequestScope scope, string? relation = null) => relation != null
        ? DerivedAt(query, scope, relation) : new(query, scope) {
        TraceSource = TraceSource, IntentSource = IntentSource,
        RelationLoadObserver = RelationLoadObserver, DiagnosticObserver = DiagnosticObserver, StatementObserver = StatementObserver
    };

    private QueryRequestScope? ReusableChildScope(SelectQuery query)
    {
        var child = query.RequestScope;
        if (child == null || child == Scope || child.Root != Scope.Root) return null;
        for (var parent = child.Parent; parent != null; parent = parent.Parent)
            if (parent == Scope) return child;
        // Reverse facet aggregation is a sibling of value materialization.
        return Scope.Parent?.IsFacetOperation == true && child.Parent == Scope.Parent ? child : null;
    }

    private QueryRequest DerivedAt(SelectQuery query, QueryRequestScope scope, string relation) => new(query, scope) {
        TraceSource = Array.AsReadOnly(TraceSource.Append(new TraceNode(query.Entity, null, "") {
            Kind = "relation", Name = relation, Detail = $"{Query.Entity}.{relation}" }).ToArray()),
        IntentSource = IntentSource, RelationLoadObserver = RelationLoadObserver, DiagnosticObserver = DiagnosticObserver, StatementObserver = StatementObserver
    };

    private static string? LocalComment(SelectQuery query) => query.RequestScope?.ExplicitComment ??
        (query.RequestScope == null ? query.CommentText : null);
    private static QueryRequestScope ChildScope(QueryRequestScope parent, SelectQuery query, string relation, QueryOperation? operation) =>
        (operation == null ? parent.Helper(parent.Entity, $"select relation {relation}") : parent.DeriveOperation(operation))
            .Child(query.Entity, LocalComment(query));
    private static QueryRequestScope AggregateScope(QueryRequestScope parent, SelectQuery query, string relation, QueryOperation? operation)
    {
        var scope = parent.Parent?.IsFacetOperation == true
            ? parent.Parent.Helper(query.Entity, $"aggregate {parent.Parent.Relation}")
            : operation == null ? parent.Helper(query.Entity, $"aggregate {relation}") : parent.DeriveOperation(operation);
        return LocalComment(query) is { } comment ? scope.Child(query.Entity, comment) : scope;
    }

    // Fork every attachment. Sharing a builder in two branches must not make
    // the second attachment overwrite the first branch's execution headers.
    private static void BindTree(SelectQuery query, QueryRequestScope scope)
    {
        query.RequestScope = scope; query.CommentText = scope.Comment; query.Purpose(scope.Purpose);
        query.RelationLoads = query.RelationLoads.Select(load => {
            if (load.Query == null) return load;
            var child = load.Query.CloneForExecution(); BindTree(child, ChildScope(scope, child, load.Name, load.Operation));
            return load with { Query = child };
        }).ToList();
        query.Facets = query.Facets.Select(facet => {
            var child = facet.Query.CloneForExecution();
            var parent = scope.Facet(facet.RelationName, facet.Name, facet.Operation);
            BindTree(child, parent.Child(child.Entity, LocalComment(child)));
            return facet with { Query = child };
        }).ToList();
        query.RelationAggregates = query.RelationAggregates.Select(aggregate => {
            var child = aggregate.Query.CloneForExecution(); BindTree(child, AggregateScope(scope, child, aggregate.RelationName, aggregate.Operation));
            return aggregate with { Query = child };
        }).ToList();
    }

    internal static QueryRequest Readback(SelectQuery query, MutationRequest mutation)
    {
        var intent = mutation.Intent.ReadbackIntent();
        var root = mutation.AuditLineage(query.Entity).FirstOrDefault()?.Name ?? query.Entity;
        return new QueryRequest(query, intent) {
            StatementObserver = mutation.StatementObserver,
            TraceSource = Array.AsReadOnly(new[] {
                new TraceNode(root, null, "") { Kind = "comment", Detail = intent.Comment },
                new TraceNode(root, null, "") { Kind = "purpose", Detail = intent.Purpose }
            })
        };
    }
}

public interface IRelationLoadObserver
{
    Task ObserveAsync(string entity, string relation,
        IReadOnlyDictionary<string, object> attributes, Func<Task> body);
}

public class QueryResult
{
    public List<Record> Rows { get; set; } = new();
    public ExecutionMetadata Metadata { get; set; } = new();
    public Dictionary<string, SmartList<Record>> Facets { get; set; } = new();
}

public abstract class MutationRequest
{
    internal Func<string, DataServiceOperation, ISqlExecutionObservation>? StatementObserver { get; set; }
    // Execution-local ID capability installed by UserContext. Never a wire field.
    internal IInternalIdAllocator? InternalIdAllocator { get; set; }
    internal LoadedScalarSnapshot? LoadedSnapshot { get; private set; }
    /// <summary>Generated hydration provenance; never a JSON field or mutation payload.</summary>
    public MutationRequest WithLoadedSnapshot(LoadedScalarSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        LoadedSnapshot = snapshot;
        return this;
    }
    internal object? GraphOwner { get; set; }
    internal MutationTraceScope? GraphScope { get; set; }
    internal Action<ExecutionMetadata>? DiagnosticObserver { get; set; }
    // Execution-local prose redaction provenance for inherited graph intent.
    // Never serialized, never used as SQL bindings or application payload.
    internal IReadOnlyList<Value> InheritedIntentValues { get; set; } = Array.Empty<Value>();
    public EntityKey? LedgerKey { get; init; }
    public EntityRoot? LedgerRoot { get; init; }
    public abstract IReadOnlyList<TraceNode> TraceChain { get; }
    public MutationIntent Intent { get; }
    public string Comment => Intent.Comment;

    /// <summary>
    /// Provider SPI observation of the runtime-derived business lineage. This
    /// immutable snapshot is separate from physical SQL and is not a wire field.
    /// It cannot install a scope or change the request's graph capability.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public IReadOnlyList<TraceNode> MutationLineage => Array.AsReadOnly(AuditLineage(this switch
    {
        InsertMutationRequest insert => insert.Command.Entity,
        UpdateMutationRequest update => update.Command.Entity,
        DeleteMutationRequest delete => delete.Command.Entity,
        RecoverMutationRequest recover => recover.Command.Entity,
        BatchMutationRequest => "batch",
        _ => "unknown"
    }).ToArray());

    protected MutationRequest(MutationIntent intent)
    {
        ArgumentNullException.ThrowIfNull(intent);
        Intent = intent;
    }

    /// <summary>Snapshot of request and item reasons, separate from physical SQL.</summary>
    internal IReadOnlyList<TraceNode> AuditLineage(string entity)
    {
        if (LedgerKey != null && LedgerRoot?.TraceChain(LedgerKey) is { Count: > 0 } complete)
            return complete;
        if (GraphScope != null) return GraphScope.Recover();
        var nodes = new List<TraceNode> { new(entity, LedgerKey?.Id.TryU64(), "") {
            Kind = "auditReason", Detail = Comment } };
        foreach (var node in TraceChain)
        {
            var reason = node.Kind.Equals("auditReason", StringComparison.OrdinalIgnoreCase)
                && node.Detail.Length > 0 ? node.Detail : node.Comment;
            if (!string.IsNullOrWhiteSpace(reason) && reason != Comment)
                nodes.Add(new TraceNode(node.EntityType, node.EntityId, "") {
                    Kind = "auditReason", Name = node.EntityType, Detail = reason });
        }
        return nodes.AsReadOnly();
    }

    public MutationRequest WithRootIntent(MutationIntent intent)
    {
        MutationRequest request = this switch
        {
            InsertMutationRequest insert => new InsertMutationRequest(insert.Command, intent.Comment) { LedgerKey = LedgerKey, LedgerRoot = LedgerRoot },
            UpdateMutationRequest update => new UpdateMutationRequest(update.Command, intent.Comment) { LedgerKey = LedgerKey, LedgerRoot = LedgerRoot },
            DeleteMutationRequest delete => new DeleteMutationRequest(delete.Command, intent.Comment) { LedgerKey = LedgerKey, LedgerRoot = LedgerRoot },
            RecoverMutationRequest recover => new RecoverMutationRequest(recover.Command, intent.Comment) { LedgerKey = LedgerKey, LedgerRoot = LedgerRoot },
            BatchMutationRequest batch => new BatchMutationRequest(batch.Requests, intent.Comment) { LedgerKey = LedgerKey, LedgerRoot = LedgerRoot },
            _ => throw new NotSupportedException("Unknown mutation request kind")
        };
        request.DiagnosticObserver = DiagnosticObserver;
        request.StatementObserver = StatementObserver;
        request.InternalIdAllocator = InternalIdAllocator;
        request.InheritedIntentValues = InheritedIntentValues;
        request.LoadedSnapshot = LoadedSnapshot;
        request.GraphOwner = GraphOwner;
        request.GraphScope = GraphScope;
        return request;
    }

    public static MutationRequest Create(InsertCommand command, string comment, EntityKey ledgerKey, EntityRoot ledgerRoot)
    {
        AddAuditTrace(command.TraceChain, command.Entity, comment);
        return new InsertMutationRequest(command, comment) { LedgerKey = ledgerKey, LedgerRoot = ledgerRoot };
    }

    public static MutationRequest Create(UpdateCommand command, string comment, EntityKey ledgerKey, EntityRoot ledgerRoot)
    {
        AddAuditTrace(command.TraceChain, command.Entity, comment);
        return new UpdateMutationRequest(command, comment) { LedgerKey = ledgerKey, LedgerRoot = ledgerRoot };
    }

    public static MutationRequest Create(DeleteCommand command, string comment, EntityKey ledgerKey, EntityRoot ledgerRoot)
    {
        AddAuditTrace(command.TraceChain, command.Entity, comment);
        return new DeleteMutationRequest(command, comment) { LedgerKey = ledgerKey, LedgerRoot = ledgerRoot };
    }

    private static void AddAuditTrace(List<TraceNode> trace, string entity, string comment)
    {
        _ = new MutationIntent(comment); // Validate before changing diagnostic state.
        trace.Add(new TraceNode(entity, null, comment));
    }
}

public class InsertMutationRequest : MutationRequest
{
    public InsertCommand Command { get; }
    
    public InsertMutationRequest(InsertCommand command, string? comment) : base(new MutationIntent(comment))
    {
        Command = command;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Command.TraceChain;
}

public class UpdateMutationRequest : MutationRequest
{
    public UpdateCommand Command { get; }
    
    public UpdateMutationRequest(UpdateCommand command, string? comment) : base(new MutationIntent(comment))
    {
        Command = command;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Command.TraceChain;
}

public class DeleteMutationRequest : MutationRequest
{
    public DeleteCommand Command { get; }
    
    public DeleteMutationRequest(DeleteCommand command, string? comment) : base(new MutationIntent(comment))
    {
        Command = command;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Command.TraceChain;
}

public class RecoverMutationRequest : MutationRequest
{
    public RecoverCommand Command { get; }
    
    public RecoverMutationRequest(RecoverCommand command, string? comment) : base(new MutationIntent(comment))
    {
        Command = command;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Command.TraceChain;
}

public class BatchMutationRequest : MutationRequest
{
    public List<MutationRequest> Requests { get; }
    
    public BatchMutationRequest(List<MutationRequest> requests, string? comment) : base(new MutationIntent(comment))
    {
        Requests = requests;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Array.Empty<TraceNode>();
}

public class MutationResult
{
    // SQL execution's ordered request/result correlation, not a wire payload.
    // Keep the public aggregate result unchanged; never infer item outcomes
    // from its affected-row total or from physical readback statement counts.
    internal IReadOnlyList<MutationBatchItemResult>? BatchItems { get; init; }
    public ulong AffectedRows { get; set; }
    public Record GeneratedValues { get; set; } = new();
    public Record? PersistedRecord { get; set; }
    public ExecutionMetadata Metadata { get; set; } = new();
}

internal sealed record MutationBatchItemResult(int Index, MutationRequest Request, MutationResult Result);

public enum DataServiceOperation
{
    Query,
    Insert,
    Update,
    Delete,
    Recover,
    Batch,
    Schema
}

public class ExecutionMetadata
{
    [System.Text.Json.Serialization.JsonIgnore] public SqlExecutionCorrelation? SqlCorrelation { get; set; }
    // A result envelope keeps business totals while carrying ordered physical
    // statements. Do not mutate a leaf into its own parent (or create a cycle
    // through inherited intent provenance).
    internal ExecutionMetadata WithStatements(params ExecutionMetadata[] statements)
    {
        var envelope = (ExecutionMetadata)MemberwiseClone();
        envelope.Statements = Array.AsReadOnly(statements);
        return envelope;
    }
    // Provider success can be observed before relation enhancement completes.
    // Runtime must not publish that same physical statement a second time.
    internal bool DiagnosticReported { get; set; }
    // Internal provenance for inherited intent only; never exposed to sinks or wire serialization.
    internal ExecutionMetadata? IntentSource { get; set; }
    // Mutation target identifiers redact prose, never SQL bind values or structured counts.
    internal IReadOnlyList<Value> IntentValues { get; set; } = Array.Empty<Value>();
    /// <summary>Statement/cursor termination, not transaction commit.</summary>
    public string? ExecutionOutcome { get; set; }
    public string Backend { get; set; } = string.Empty;
    public DataServiceOperation Operation { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset EndedAt { get; set; }
    public ulong? AffectedRows { get; set; }
    public int? ResultCount { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("tracePath")] public List<TraceNode> TraceChain { get; set; } = new();
    [System.Text.Json.Serialization.JsonPropertyName("traceChain")] public IReadOnlyList<TraceNode> RequestTraceChain { get; set; } = Array.Empty<TraceNode>();
    public string? Comment { get; set; }
    public IReadOnlyList<TraceNode> MutationLineage { get; set; } = Array.Empty<TraceNode>();
    public string? Purpose { get; set; }
    public string? AuditReason { get; set; }
    public string? BackendRequestId { get; set; }
    /// <summary>Provider-native request text with placeholders, never interpolated bind values.</summary>
    public string? ParameterizedQuery { get; set; }
    /// <summary>Structured bind values for trusted runtime diagnostics.</summary>
    public IReadOnlyList<Value> Parameters { get; set; } = Array.Empty<Value>();
    private int? _parameterCount;
    public int ParameterCount { get => _parameterCount ?? Parameters.Count; set => _parameterCount = value; }
    public string? DebugQuery { get; set; }
    public IReadOnlyList<SqlParameterLogPolicy> ParameterLogPolicies { get; set; } = Array.Empty<SqlParameterLogPolicy>();
    public IReadOnlyList<bool> MaskedParameters { get; set; } = Array.Empty<bool>();
    public bool GeneratedSql { get; set; }
    public string? LogMode { get; set; }
    public string? SqlOmissionReason { get; set; }
    public IReadOnlyList<ExecutionMetadata> Statements { get; set; } = Array.Empty<ExecutionMetadata>();
}

public interface IDataService
{
    DataServiceCapabilities Capabilities { get; }
    Task<QueryResult> QueryAsync(QueryRequest request);
    Task<MutationResult> MutateAsync(MutationRequest request);
}

public class StreamChunk
{
    public List<Record> Rows { get; set; } = new();
    public int ChunkIndex { get; set; }
    public bool IsLast { get; set; }
}

internal sealed class StreamDelivery
{
    private int _count;
    internal int Count => System.Threading.Volatile.Read(ref _count);
    internal Exception? Error { get; set; }
    internal void Delivered() => System.Threading.Interlocked.Increment(ref _count);
}

public interface IStreamQueryExecutor : IDataService
{
    IAsyncEnumerable<StreamChunk> QueryStreamAsync(QueryRequest request, int chunkSize, CancellationToken cancellationToken = default);
}

public interface ITransaction : IDataService, IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}

public interface ITransactionExecutor : IDataService
{
    Task<ITransaction> BeginTransactionAsync();
}

/// <summary>A context-owned request to reconcile one modeled entity's schema.</summary>
public class SchemaRequest
{
    public string EntityName { get; set; } = string.Empty;
}

/// <summary>The result of one schema reconciliation operation.</summary>
public class SchemaResult
{
    public bool Changed { get; set; }
}

/// <summary>Replaceable schema execution capability; applications invoke it through UserContext.</summary>
public interface ISchemaExecutor
{
    Task<SchemaResult> EnsureSchemaAsync(SchemaRequest request);
}

/// <summary>
/// Context-installable internal ID allocation capability. Implementations must
/// allocate unique IDs and advance their floor for explicit IDs of each entity type.
/// Gaps after failed or rolled-back mutations are permitted.
/// </summary>
public interface IInternalIdAllocator
{
    Task<ulong> NextIdAsync(string entity);
    Task EnsureIdFloorAsync(string entity, ulong floor);
}

public interface IIdGeneratorExecutor : IDataService, IInternalIdAllocator { }
