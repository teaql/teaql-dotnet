using System.Runtime.ExceptionServices;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public sealed class TransactionOutcomeUnknownException()
    : InvalidOperationException("TRANSACTION_OUTCOME_UNKNOWN: reload the affected records before retrying")
{
}

internal sealed class ContextTransaction(ITransaction provider)
{
    private readonly SemaphoreSlim _command = new(1, 1);
    private readonly List<GraphMutationSession> _graphs = new();
    private readonly List<Func<Task>> _commit = new();
    private ExceptionDispatchInfo? _failure;
    private volatile bool _active = true;

    internal ITransaction Provider { get; } = provider;
    internal ExceptionDispatchInfo? Failure => Volatile.Read(ref _failure);

    internal void RequireAccess()
    {
        if (!_active) throw new InvalidOperationException("Transaction UserContext is outside its callback");
    }

    internal void RequireWritable()
    {
        RequireAccess();
        if (Failure is { } failure)
            throw new InvalidOperationException("Transaction is marked for rollback", failure.SourceException);
    }

    internal void Fail(Exception error) => Interlocked.CompareExchange(
        ref _failure, ExceptionDispatchInfo.Capture(error), null);

    internal void Enlist(GraphMutationSession session)
    {
        _graphs.Add(session);
        AddCommit(session.CompleteAsync);
    }

    internal void AddCommit(Func<Task> completion)
    {
        lock (_commit) _commit.Add(completion);
    }

    internal async Task<IDisposable> EnterCommandAsync(CancellationToken cancellationToken)
    {
        RequireAccess();
        if (!await _command.WaitAsync(0, cancellationToken).ConfigureAwait(false))
            throw new InvalidOperationException("Another command is active in this transaction");
        var lease = new CommandLease(_command);
        if (_active) return lease;
        lease.Dispose();
        RequireAccess();
        throw new InvalidOperationException("Transaction UserContext is outside its callback");
    }

    private sealed class CommandLease(SemaphoreSlim command) : IDisposable
    {
        public void Dispose() => command.Release();
    }

    internal void EndCallback() => _active = false;

    internal async Task DrainCommandsAsync()
    {
        await _command.WaitAsync().ConfigureAwait(false);
        _command.Release();
    }

    internal async Task DeliverCommitAsync(List<Exception> errors)
    {
        foreach (var complete in _commit)
        {
            try { await complete().ConfigureAwait(false); }
            catch (GraphCommittedException error) { errors.AddRange(error.InnerExceptions); }
            catch (Exception error) { errors.Add(error); }
        }
    }

    internal void RestoreGraphs()
    {
        for (var index = _graphs.Count - 1; index >= 0; index--)
            try { _graphs[index].Restore(); }
            catch (AggregateException) { /* Preserve the provider or business failure. */ }
    }

    internal void FenceGraphs()
    {
        foreach (var graph in _graphs) graph.FenceTransactionOutcome();
    }

    internal void CloseGraphs()
    {
        foreach (var graph in _graphs) graph.Close();
        _graphs.Clear();
        _commit.Clear();
    }
}
