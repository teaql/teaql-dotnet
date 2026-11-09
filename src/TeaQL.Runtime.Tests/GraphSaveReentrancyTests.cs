using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public class GraphSaveReentrancyTests
{
    private static readonly TimeSpan Bound = TimeSpan.FromSeconds(2);

    [Fact]
    public async Task RootSaveReentryFromWorkRejectsInsteadOfWaitingOnItsOwnGate()
    {
        var provider = new Provider();
        var context = new UserContext().WithDataService(provider);
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            context.ExecuteGraphSaveAsync("outer work scope", async graph =>
            {
                await Task.Yield();
                return await context.ExecuteGraphSaveAsync("invalid nested root", _ => Task.FromResult(2));
            }).WaitAsync(Bound));
        Assert.Equal("GRAPH_SAVE_REENTRANT", error.Message);
        Assert.Equal(1, provider.Begins);
        Assert.Equal(0, provider.Commits);
        Assert.Equal(1, provider.Rollbacks);
    }

    [Fact]
    public async Task RootSaveReentryFromPreparationRejectsBeforeProviderAdmission()
    {
        var provider = new Provider();
        var context = new UserContext().WithDataService(provider);
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            context.ExecuteGraphSaveAsync("outer preparation scope", async graph =>
            {
                await Task.Yield();
                await context.ExecuteGraphSaveAsync("invalid preparation root", _ => Task.FromResult(2));
            }, _ => Task.FromResult(1)).WaitAsync(Bound));
        Assert.Equal("GRAPH_SAVE_REENTRANT", error.Message);
        Assert.Equal(0, provider.Begins);
        Assert.Equal(0, provider.Commits);
        Assert.Equal(0, provider.Rollbacks);
    }

    [Fact]
    public async Task ActiveInheritedChildFlowRejectsBeforeOpeningAnotherTransaction()
    {
        var provider = new Provider();
        var context = new UserContext().WithDataService(provider);
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            context.ExecuteGraphSaveAsync("outer owner with active child", async graph =>
                await Task.Run(() => context.ExecuteGraphSaveAsync("nested child root", _ => Task.FromResult(2))))
                .WaitAsync(Bound));
        Assert.Equal("GRAPH_SAVE_REENTRANT", error.Message);
        Assert.Equal(1, provider.Begins);
        Assert.Equal(0, provider.Commits);
        Assert.Equal(1, provider.Rollbacks);
    }

    [Fact]
    public async Task CaughtReentryDoesNotPoisonTheOwningGraphOrItsNextSave()
    {
        var provider = new Provider();
        var context = new UserContext().WithDataService(provider);
        Assert.Equal(1, await context.ExecuteGraphSaveAsync("catch nested rejection", async graph =>
        {
            var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                context.ExecuteGraphSaveAsync("invalid nested root", _ => Task.FromResult(2)));
            Assert.Equal("GRAPH_SAVE_REENTRANT", error.Message);
            return 1;
        }).WaitAsync(Bound));
        Assert.Equal(3, await context.ExecuteGraphSaveAsync("next independent root", _ => Task.FromResult(3)).WaitAsync(Bound));
        Assert.Equal(2, provider.Begins);
        Assert.Equal(2, provider.Commits);
        Assert.Equal(0, provider.Rollbacks);
    }

    [Fact]
    public async Task PostCommitReentryRejectsWithoutRollingBackCommittedWork()
    {
        var provider = new Provider();
        var context = new UserContext().WithDataService(provider);
        var error = await Assert.ThrowsAsync<GraphCommittedException>(() =>
            context.ExecuteGraphSaveAsync("committed owner", graph =>
            {
                graph.AfterCommit(() => context.ExecuteGraphSaveAsync("invalid postcommit root", _ => Task.FromResult(2))
                    .GetAwaiter().GetResult());
                return Task.FromResult(1);
            }).WaitAsync(Bound));
        Assert.Equal("GRAPH_SAVE_REENTRANT", Assert.Single(error.InnerExceptions).Message);
        Assert.Equal(1, provider.Begins);
        Assert.Equal(1, provider.Commits);
        Assert.Equal(0, provider.Rollbacks);
        Assert.Equal(3, await context.ExecuteGraphSaveAsync("next root after postcommit rejection", _ => Task.FromResult(3)).WaitAsync(Bound));
        Assert.Equal(2, provider.Begins);
        Assert.Equal(2, provider.Commits);
        Assert.Equal(0, provider.Rollbacks);
    }

    [Fact]
    public async Task IndependentConcurrentFlowStillQueuesAndCommitsSeparately()
    {
        var provider = new Provider();
        var context = new UserContext().WithDataService(provider);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var first = context.ExecuteGraphSaveAsync("first independent root", async graph =>
        { entered.SetResult(); await release.Task; return 1; });
        await entered.Task.WaitAsync(Bound);
        var secondEntered = false;
        var second = context.ExecuteGraphSaveAsync("second independent root", graph =>
        { secondEntered = true; return Task.FromResult(2); });
        try { await Task.Delay(25); Assert.False(secondEntered); }
        finally { release.TrySetResult(); }
        Assert.Equal(1, await first.WaitAsync(Bound));
        Assert.Equal(2, await second.WaitAsync(Bound));
        Assert.Equal(2, provider.Begins);
        Assert.Equal(2, provider.Commits);
        Assert.Equal(0, provider.Rollbacks);
    }

    [Fact]
    public async Task InheritedChildFlowMaySaveAfterItsOriginalScopeHasCompleted()
    {
        var provider = new Provider();
        var context = new UserContext().WithDataService(provider);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<int>? child = null;
        Assert.Equal(1, await context.ExecuteGraphSaveAsync("create a deferred child flow", graph =>
        {
            child = Task.Run(async () =>
            {
                await release.Task;
                return await context.ExecuteGraphSaveAsync("new root after original completion", _ => Task.FromResult(2));
            });
            return Task.FromResult(1);
        }).WaitAsync(Bound));
        release.SetResult();
        Assert.Equal(2, await child!.WaitAsync(Bound));
        Assert.Equal(2, provider.Begins);
        Assert.Equal(2, provider.Commits);
    }

    [Fact]
    public async Task DifferentContextIsNotRejectedByAnotherContextsActiveFlow()
    {
        var first = new Provider(); var second = new Provider();
        var a = new UserContext().WithDataService(first);
        var b = new UserContext().WithDataService(second);
        Assert.Equal(2, await a.ExecuteGraphSaveAsync("context A owner", async graph =>
            await b.ExecuteGraphSaveAsync("context B independent owner", _ => Task.FromResult(2))).WaitAsync(Bound));
        Assert.Equal(1, first.Begins); Assert.Equal(1, first.Commits);
        Assert.Equal(1, second.Begins); Assert.Equal(1, second.Commits);
    }

    private sealed class Provider : ITransactionExecutor
    {
        public int Begins, Commits, Rollbacks;
        public DataServiceCapabilities Capabilities { get; } = new() { Transaction = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new NotSupportedException();
        public Task<MutationResult> MutateAsync(MutationRequest request) => throw new NotSupportedException();
        public Task<ITransaction> BeginTransactionAsync()
        { Begins++; return Task.FromResult<ITransaction>(new Transaction(this)); }
        private sealed class Transaction(Provider owner) : ITransaction
        {
            public DataServiceCapabilities Capabilities => owner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) => throw new NotSupportedException();
            public Task<MutationResult> MutateAsync(MutationRequest request) => throw new NotSupportedException();
            public Task CommitAsync() { owner.Commits++; return Task.CompletedTask; }
            public Task RollbackAsync() { owner.Rollbacks++; return Task.CompletedTask; }
            public void Dispose() { }
        }
    }
}
