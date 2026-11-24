using System.Runtime.ExceptionServices;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public partial class UserContext
{
    private ContextTransaction? _contextTransaction;
    internal bool HasExplicitTransaction => _contextTransaction != null;

    /// <summary>Use one database transaction for the saves made with the callback's context.</summary>
    public async Task<T> TransactionAsync<T>(Func<UserContext, Task<T>> work)
    {
        ArgumentNullException.ThrowIfNull(work);
        EnsureGraphSaveCanEnter();
        if (_contextTransaction is { } existing)
        {
            existing.RequireWritable();
            try { return await work(this).ConfigureAwait(false); }
            catch (Exception error) { existing.Fail(error); throw; }
        }

        var view = CreateTransactionView();
        var provider = await RequireResource<ITransactionExecutor>().BeginTransactionAsync().ConfigureAwait(false);
        var scope = new ContextTransaction(provider);
        view._contextTransaction = scope;
        view._typedResources[typeof(IDataService)] = new RuntimeDataService(provider, view);
        var disposed = false;
        try
        {
            T result = default!;
            ExceptionDispatchInfo? callbackFailure = null;
            try { result = await work(view).ConfigureAwait(false); }
            catch (Exception error) { callbackFailure = ExceptionDispatchInfo.Capture(error); }
            finally { scope.EndCallback(); }

            await view._graphSaveGate.WaitAsync().ConfigureAwait(false);
            view._graphSaveGate.Release();
            await scope.DrainCommandsAsync().ConfigureAwait(false);

            if ((scope.Failure ?? callbackFailure) is { } failure)
            {
                try { await provider.RollbackAsync().ConfigureAwait(false); }
                catch (Exception)
                {
                    scope.FenceGraphs();
                    throw new TransactionOutcomeUnknownException();
                }
                scope.RestoreGraphs();
                failure.Throw();
            }

            try { await provider.CommitAsync().ConfigureAwait(false); }
            catch (Exception)
            {
                scope.FenceGraphs();
                throw new TransactionOutcomeUnknownException();
            }

            var errors = new List<Exception>();
            disposed = true;
            try { provider.Dispose(); }
            catch (Exception error) { errors.Add(error); }
            await scope.DeliverCommitAsync(errors).ConfigureAwait(false);
            if (errors.Count != 0) throw new GraphCommittedException(errors);
            return result;
        }
        finally
        {
            scope.CloseGraphs();
            if (!disposed)
                try { provider.Dispose(); }
                catch (Exception) { /* Cleanup cannot decide the transaction outcome. */ }
        }
    }

    private UserContext CreateTransactionView()
    {
        var view = (UserContext)MemberwiseClone();
        view._typedResources = new(_typedResources);
        view._namedResources = new(_namedResources);
        view._locals = new(_locals);
        view._managedEntities = new(_managedEntities);
        view._checkers = new(_checkers);
        view._entityInitializers = new(_entityInitializers.ToDictionary(pair => pair.Key, pair =>
        {
            lock (pair.Value) return new List<Action<UserContext, object>>(pair.Value);
        }));
        view._mutationPolicy = _mutationPolicy.ForInvocation();
        view._graphSaveGate = new(1, 1);
        view._graphSaveInvocation = new();
        view._activeGraphSave = null;
        view._graphFixTime = null;
        view._currentFixEvidence = new();
        view._lastFixEvidence = Array.Empty<FixEvidence>();
        return view;
    }

    internal async Task<IDisposable?> EnterTransactionCommandAsync(CancellationToken cancellationToken = default)
        => _contextTransaction is { } scope ? await scope.EnterCommandAsync(cancellationToken).ConfigureAwait(false) : null;

    internal void RequireTransactionAccess() => _contextTransaction?.RequireAccess();

    internal bool DeferTransactionAudits(IEnumerable<Func<Task>> deliveries)
    {
        if (_contextTransaction is not { } scope) return false;
        foreach (var deliver in deliveries) scope.AddCommit(deliver);
        return true;
    }
}
