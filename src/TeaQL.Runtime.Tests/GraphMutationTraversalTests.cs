using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public class GraphMutationTraversalTests
{
    [Theory]
    [InlineData(GraphMutationTraversal.Prepare)]
    [InlineData(GraphMutationTraversal.Reserve)]
    [InlineData(GraphMutationTraversal.Preflight)]
    [InlineData(GraphMutationTraversal.Write)]
    public async Task ReferenceIdentityAndPhaseBothBelongToTheInvocation(GraphMutationTraversal phase)
    {
        var provider = new Provider(); var context = new UserContext().WithDataService(provider);
        var first = new View(1); var second = new View(1); GraphMutationSession? expired = null;
        Assert.Equal(first, second);
        await context.ExecuteGraphSaveAsync("observe generated traversal identity", graph => {
            expired = graph;
            Assert.True(graph.VisitEntity(first, phase));
            Assert.False(graph.VisitEntity(first, phase));
            Assert.True(graph.VisitEntity(second, phase));
            Assert.True(graph.VisitEntity(first, (GraphMutationTraversal)(((int)phase + 1) % 4)));
            graph.SealPreparedGraph();
            return Task.CompletedTask;
        }, _ => Task.FromResult(1));
        Assert.Equal(0, provider.Begins);
        Assert.Equal("GRAPH_CAPABILITY_EXPIRED", Assert.Throws<InvalidOperationException>(() => expired!.VisitEntity(first, phase)).Message);
    }

    [Fact]
    public async Task FailedTraversalCannotPoisonTheSameObjectRetry()
    {
        var provider = new Provider(); var context = new UserContext().WithDataService(provider);
        var owner = new object();
        await Assert.ThrowsAsync<OperationCanceledException>(() => context.ExecuteGraphSaveAsync<int>(
            "cancel one generated preparation", graph => {
                Assert.True(graph.VisitEntity(owner, GraphMutationTraversal.Prepare));
                throw new OperationCanceledException();
            }, _ => Task.FromResult(0)));
        Assert.Equal(1, await context.ExecuteGraphSaveAsync("retry the same generated object", graph => {
            Assert.True(graph.VisitEntity(owner, GraphMutationTraversal.Prepare));
            graph.SealPreparedGraph();
            return Task.CompletedTask;
        }, _ => Task.FromResult(1)));
        Assert.Equal(0, provider.Begins);
    }

    [Fact]
    public async Task NullObjectsAndUnknownPhasesFailClosed()
    {
        var context = new UserContext().WithDataService(new Provider());
        await context.ExecuteGraphSaveAsync("validate generated traversal arguments", graph => {
            Assert.Throws<ArgumentNullException>(() => graph.VisitEntity(null!, GraphMutationTraversal.Prepare));
            Assert.Throws<ArgumentOutOfRangeException>(() => graph.VisitEntity(new object(), (GraphMutationTraversal)4));
            graph.SealPreparedGraph();
            return Task.CompletedTask;
        }, _ => Task.FromResult(1));
    }

    private sealed record View(long Id);
    private sealed class Provider : IDataService, ITransactionExecutor
    {
        public int Begins;
        public DataServiceCapabilities Capabilities => new() { Mutation = true, Transaction = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new InvalidOperationException("unexpected query");
        public Task<MutationResult> MutateAsync(MutationRequest request) => throw new InvalidOperationException("unexpected mutation");
        public Task<ITransaction> BeginTransactionAsync() { Begins++; throw new InvalidOperationException("empty graph must not begin"); }
    }
}
