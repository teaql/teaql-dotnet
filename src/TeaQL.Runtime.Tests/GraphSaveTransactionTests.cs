using TeaQL.DataService;
using TeaQL.Core;

namespace TeaQL.Runtime.Tests;

public class GraphSaveTransactionTests
{
    [Fact]
    public async Task NestedGraphSaveUsesOneTransactionAndDefersCommitCallbacks()
    {
        var provider = new RecordingTransactionExecutor();
        var context = new UserContext().WithDataService(provider);
        var events = new List<string>();

        var result = await context.ExecuteGraphSaveAsync(async () =>
        {
            context.AfterGraphCommit(() => events.Add("commit-action"));
            events.Add("work");
            return await context.ExecuteGraphSaveAsync(() => Task.FromResult(42));
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

        await Assert.ThrowsAsync<InvalidOperationException>(() => context.ExecuteGraphSaveAsync<int>(() =>
        {
            context.AfterGraphRollback(() => callbacks.Add("parent"));
            context.AfterGraphRollback(() => callbacks.Add("child"));
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

        var first = context.ExecuteGraphSaveAsync(async () =>
        {
            firstEntered.SetResult();
            await releaseFirst.Task;
            return 1;
        });
        await firstEntered.Task;

        var second = context.ExecuteGraphSaveAsync(() =>
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
        var context = new UserContext()
            .Install(new RuntimeModule().Checker("Task", checker))
            .WithDataService(provider);

        await context.ExecuteGraphSaveAsync(async () =>
        {
            var service = context.RequireResource<IDataService>();
            await service.MutateAsync(new InsertMutationRequest(new InsertCommand("Task")));
            await Task.Delay(5);
            await service.MutateAsync(new InsertMutationRequest(new InsertCommand("Task")));
            return true;
        });

        Assert.Equal(2, checker.Times.Count);
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
        public Task<MutationResult> MutateAsync(MutationRequest request) => Task.FromResult(new MutationResult());
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
