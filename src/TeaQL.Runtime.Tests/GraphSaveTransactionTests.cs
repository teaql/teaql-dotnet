using TeaQL.DataService;
using TeaQL.Core;

namespace TeaQL.Runtime.Tests;

public class GraphSaveTransactionTests
{
    [Fact]
    public async Task AuditIsNotPublishedBeforeCommitOrAfterRollback()
    {
        var provider = new RecordingTransactionExecutor();
        var audit = new AuditSink();
        var context = new UserContext().WithDataService(provider).WithAppAuditEventSink(audit);
        await Assert.ThrowsAsync<InvalidOperationException>(() => context.ExecuteGraphSaveAsync<int>(
            "save graph that rolls back", async graph =>
            {
                await graph.MutateAsync(new InsertMutationRequest(
                    new InsertCommand("Task").Value("id", Value.FromObject(1L)), "create task"));
                Assert.Empty(audit.Events);
                throw new InvalidOperationException("injected graph failure");
            }));
        Assert.Empty(audit.Events);
        Assert.Equal(new[] { "provider-rollback" }, provider.Events);
    }

    [Fact]
    public async Task AfterCommitFailureStillRunsRemainingCleanupAndNeverRollsBack()
    {
        var provider = new RecordingTransactionExecutor();
        var context = new UserContext().WithDataService(provider);
        var cleanup = false;
        var rollback = false;
        var error = await Assert.ThrowsAnyAsync<Exception>(() => context.ExecuteGraphSaveAsync(
            "save committed graph", graph =>
            {
                graph.AfterRollback(() => rollback = true);
                graph.AfterCommit(() => throw new InvalidOperationException("audit unavailable"));
                graph.AfterCommit(() => cleanup = true);
                return Task.FromResult(1);
            }));
        Assert.True(cleanup);
        Assert.False(rollback);
        Assert.Equal(new[] { "provider-commit" }, provider.Events);
        Assert.True((bool?)error.GetType().GetProperty("Committed")?.GetValue(error));
    }

    private sealed class AuditSink : IAppAuditEventSink
    {
        public List<IReadOnlyDictionary<string, object?>> Events { get; } = new();
        public Task RecordAsync(IReadOnlyDictionary<string, object?> item, CancellationToken token = default)
        { Events.Add(item); return Task.CompletedTask; }
    }

    [Fact]
    public async Task UntaggedMutationsAndExpiredOrForeignCapabilitiesFailClosed()
    {
        var context = new UserContext().WithDataService(new RecordingTransactionExecutor());
        GraphMutationSession? previous = null;
        MutationTraceScope? foreign = null;
        await context.ExecuteGraphSaveAsync("first graph", async graph =>
        {
            previous = graph; foreign = graph.Scope("Task", 1, null);
            var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                context.RequireResource<IDataService>().MutateAsync(
                    new InsertMutationRequest(new InsertCommand("Task"), "untagged")));
            Assert.Equal("GRAPH_CAPABILITY_REQUIRED", error.Message);
            return 1;
        });
        Assert.Equal("GRAPH_CAPABILITY_EXPIRED", Assert.Throws<InvalidOperationException>(() =>
            previous!.Scope("Task", 1, null)).Message);
        await context.ExecuteGraphSaveAsync("second graph", graph =>
        {
            Assert.Equal("GRAPH_SCOPE_OWNER_MISMATCH", Assert.Throws<InvalidOperationException>(() =>
                graph.Scope("Task", 2, "second local reason", foreign)).Message);
            return Task.FromResult(2);
        });
    }
    [Fact]
    public async Task ExplicitGraphCapabilityUsesOneTransactionAndDefersCommitCallbacks()
    {
        var provider = new RecordingTransactionExecutor();
        var context = new UserContext().WithDataService(provider);
        var events = new List<string>();

        var result = await context.ExecuteGraphSaveAsync("save parent graph", async graph =>
        {
            graph.AfterCommit(() => events.Add("commit-action"));
            events.Add("work");
            return await Task.FromResult(42);
        });

        Assert.Equal(42, result);
        Assert.Equal(1, provider.BeginCount);
        Assert.Equal(new[] { "provider-commit" }, provider.Events);
        Assert.Equal(new[] { "work", "commit-action" }, events);
    }

    [Fact]
    public async Task FailureRollsBackAndRunsEntityCallbacksInReverseOrder()
    {
        var provider = new RecordingTransactionExecutor();
        var context = new UserContext().WithDataService(provider);
        var callbacks = new List<string>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => context.ExecuteGraphSaveAsync<int>("save rollback graph", graph =>
        {
            graph.AfterRollback(() => callbacks.Add("parent"));
            graph.AfterRollback(() => callbacks.Add("child"));
            throw new InvalidOperationException("injected");
        }));

        Assert.Equal(new[] { "provider-rollback" }, provider.Events);
        Assert.Equal(new[] { "child", "parent" }, callbacks);
    }

    [Fact]
    public async Task IndependentConcurrentSavesDoNotJoinTheActiveTransaction()
    {
        var provider = new RecordingTransactionExecutor();
        var context = new UserContext().WithDataService(provider);
        var firstEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFirst = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondEntered = false;

        var first = context.ExecuteGraphSaveAsync("save first independent graph", async graph =>
        {
            firstEntered.SetResult();
            await releaseFirst.Task;
            return 1;
        });
        await firstEntered.Task;

        var second = context.ExecuteGraphSaveAsync("save second independent graph", graph =>
        {
            secondEntered = true;
            return Task.FromResult(2);
        });
        await Task.Delay(25);
        Assert.False(secondEntered);

        releaseFirst.SetResult();
        Assert.Equal(1, await first);
        Assert.Equal(2, await second);
        Assert.Equal(2, provider.BeginCount);
    }

    [Fact]
    public async Task OneGraphUsesOneCapturedFixClock()
    {
        var provider = new RecordingTransactionExecutor();
        var checker = new ClockChecker();
        var expected = new DateTimeOffset(2026, 10, 1, 9, 30, 15, TimeSpan.FromHours(8));
        var context = new UserContext()
            .WithBusinessClock(new FixedBusinessClock(expected))
            .Install(new RuntimeModule().Checker("Task", checker))
            .WithDataService(provider);

        await context.ExecuteGraphSaveAsync("create tasks with one business clock", async graph =>
        {
            await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Task"), "create first task"));
            await Task.Delay(5);
            await graph.MutateAsync(new InsertMutationRequest(new InsertCommand("Task"), "create second task"));
            return true;
        });

        Assert.Equal(2, checker.Times.Count);
        Assert.Equal(expected, checker.Times[0]);
        Assert.Equal(checker.Times[0], checker.Times[1]);
    }

    private sealed class ClockChecker : IEntityChecker
    {
        public List<DateTimeOffset> Times { get; } = new();
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest mutation, DateTimeOffset now)
        { Times.Add(now); return []; }
    }

    private sealed class RecordingTransactionExecutor : ITransactionExecutor
    {
        public int BeginCount { get; private set; }
        public List<string> Events { get; } = new();
        public DataServiceCapabilities Capabilities { get; } = new() { Transaction = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => Task.FromResult(new QueryResult());
        public Task<MutationResult> MutateAsync(MutationRequest request) => Task.FromResult(new MutationResult { AffectedRows = 1 });
        public Task<ITransaction> BeginTransactionAsync()
        {
            BeginCount++;
            return Task.FromResult<ITransaction>(new RecordingTransaction(this));
        }

        private sealed class RecordingTransaction(RecordingTransactionExecutor owner) : ITransaction
        {
            public DataServiceCapabilities Capabilities => owner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) => owner.QueryAsync(request);
            public Task<MutationResult> MutateAsync(MutationRequest request) => owner.MutateAsync(request);
            public Task CommitAsync() { owner.Events.Add("provider-commit"); return Task.CompletedTask; }
            public Task RollbackAsync() { owner.Events.Add("provider-rollback"); return Task.CompletedTask; }
            public void Dispose() { }
        }
    }
}
