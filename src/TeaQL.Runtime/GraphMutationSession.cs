using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

/// <summary>Internal adapter capability explicitly passed to generated graph saves.</summary>
public sealed class GraphMutationSession
{
    private readonly UserContext _context;
    private readonly ITransaction _transaction;
    private readonly RuntimeDataService _service;
    private readonly HashSet<MutationTraceScope> _scopes = new();
    private readonly List<Func<Task>> _commit = new();
    private readonly List<Action> _rollback = new();
    private readonly List<Value> _intentValues = new();
    private bool _active = true;
    public MutationIntent Intent { get; }

    internal GraphMutationSession(UserContext context, ITransaction transaction, MutationIntent intent)
    {
        _context = context; _transaction = transaction; Intent = intent;
        _service = new RuntimeDataService(transaction, context, this);
    }

    internal void Validate(MutationRequest request)
    {
        RequireActive();
        if (!ReferenceEquals(request.GraphOwner, this))
            throw new InvalidOperationException("GRAPH_CAPABILITY_REQUIRED");
    }

    private void RequireActive()
    {
        if (!_active) throw new InvalidOperationException("GRAPH_CAPABILITY_EXPIRED");
    }

    public MutationTraceScope Scope(string entity, ulong? id, string? localReason,
        MutationTraceScope? parent = null)
    {
        RequireActive();
        if (parent != null && !_scopes.Contains(parent))
            throw new InvalidOperationException("GRAPH_SCOPE_OWNER_MISMATCH");
        MutationTraceScope scope;
        if (parent == null) scope = new(entity, id, Intent.Comment);
        else
        {
            try { scope = new(entity, id, new MutationIntent(localReason).Comment, parent); }
            catch (RequestIntentException) { return parent; }
        }
        _scopes.Add(scope);
        return scope;
    }

    public MutationRequest Request(MutationRequest request, MutationTraceScope? scope = null)
    {
        RequireActive();
        if (scope != null && !_scopes.Contains(scope))
            throw new InvalidOperationException("GRAPH_SCOPE_OWNER_MISMATCH");
        if (request.GraphOwner != null && !ReferenceEquals(request.GraphOwner, this))
            throw new InvalidOperationException("GRAPH_CAPABILITY_OWNER_MISMATCH");
        var captured = request.WithRootIntent(Intent);
        captured.GraphOwner = this; captured.GraphScope = scope;
        captured.InheritedIntentValues = _intentValues.ToArray();
        return captured;
    }

    public void Preflight(MutationRequest request)
    {
        var captured = Request(request);
        _context.ValidateGraphMutation(captured, this);
        _context.PreflightMutation(captured);
        _intentValues.AddRange(RuntimeDataService.PrivateMutationValues(captured, _context));
        _intentValues.AddRange(RuntimeDataService.LoadedPrivateValues(captured, _context));
    }

    public Task<MutationResult> MutateAsync(MutationRequest request, MutationTraceScope? scope = null) =>
        _service.MutateAsync(Request(request, scope));

    public Task<ulong> AllocateIdAsync(string entity)
    {
        RequireActive();
        return (_transaction as IIdGeneratorExecutor
            ?? throw new NotSupportedException("Graph provider requires ID allocation support")).NextIdAsync(entity);
    }

    public void AfterCommit(Action action) => AfterCommit(() => { action(); return Task.CompletedTask; });
    internal void AfterCommit(Func<Task> action) { RequireActive(); _commit.Add(action); }
    public void AfterRollback(Action action) { RequireActive(); _rollback.Add(action); }

    internal async Task CompleteAsync()
    {
        // Database commit is already final. Try every consumer/cleanup, never roll it back.
        _active = false;
        var errors = new List<Exception>();
        foreach (var action in _commit)
            try { await action().ConfigureAwait(false); } catch (Exception error) { errors.Add(error); }
        if (errors.Count > 0) throw new GraphCommittedException(errors);
    }

    internal void Restore()
    {
        _active = false;
        var errors = new List<Exception>();
        for (var index = _rollback.Count - 1; index >= 0; index--)
            try { _rollback[index](); } catch (Exception error) { errors.Add(error); }
        if (errors.Count > 0) throw new AggregateException("Graph rollback callbacks failed", errors);
    }

    internal void Close() => _active = false;
}

public sealed class GraphCommittedException(IReadOnlyList<Exception> errors)
    : AggregateException("Graph committed; a post-commit consumer failed", errors)
{
    public bool Committed => true;
}
