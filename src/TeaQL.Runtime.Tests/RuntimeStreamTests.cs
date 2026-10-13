using System.Runtime.CompilerServices;
using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class RuntimeStreamTests
{
    [Fact]
    public async Task UnpolledIsLazyAndFullEnumerationCountsTypedRows()
    {
        var provider = new Rows(); var telemetry = new Probe(); var context = Context(provider, telemetry);
        var stream = Stream(context);
        Assert.Equal(0, provider.Opened); Assert.Empty(telemetry.Completed);
        Assert.Equal(new long[] { 1, 2, 3 }, await Read(stream));
        Assert.Equal(1, provider.Opened); Assert.Equal(1, provider.Closed);
        Assert.Equal((3, "success"), Assert.Single(telemetry.Completed));
    }

    [Fact]
    public async Task EarlyCloseInsideChunkCountsOnlyOneTypedRow()
    {
        var provider = new Rows(); var telemetry = new Probe(); var context = Context(provider, telemetry);
        await using (var cursor = Stream(context).GetAsyncEnumerator()) { Assert.True(await cursor.MoveNextAsync()); Assert.Equal(1, cursor.Current); }
        Assert.Equal(1, provider.Closed); Assert.Equal((1, "cancelled"), Assert.Single(telemetry.Completed));
    }

    [Fact]
    public async Task CancellationWithinPrefetchedChunkRejectsTheNextRow()
    {
        var provider = new Rows(); var telemetry = new Probe(); var context = Context(provider, telemetry);
        using var cancellation = new CancellationTokenSource();
        await using (var cursor = Stream(context).GetAsyncEnumerator(cancellation.Token))
        {
            Assert.True(await cursor.MoveNextAsync()); cancellation.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await cursor.MoveNextAsync());
        }
        Assert.Equal(1, provider.Closed); Assert.Equal((1, "cancelled"), Assert.Single(telemetry.Completed));
    }

    [Fact]
    public async Task HydrationFailurePreservesErrorAndDeliveredCount()
    {
        var provider = new Rows(); var telemetry = new Probe(); var context = Context(provider, telemetry);
        var injected = new FormatException("PRIVATE-HYDRATION-CANARY");
        var stream = RuntimeDataService.StreamEntities(context, Request(), row => {
            if (row["id"].TryI64() == 2) throw injected;
            return row["id"].TryI64()!.Value;
        }, 2);
        Assert.Same(injected, await Assert.ThrowsAsync<FormatException>(() => Read(stream)));
        Assert.Same(injected, telemetry.Error); Assert.Equal((1, "failure"), Assert.Single(telemetry.Completed)); Assert.Equal(1, provider.Closed);
    }

    [Fact]
    public async Task ProviderFailurePreservesErrorAndClosesRoot()
    {
        var injected = new InvalidOperationException("PRIVATE-PROVIDER-CANARY");
        var provider = new Rows { Failure = injected }; var telemetry = new Probe(); var context = Context(provider, telemetry);
        Assert.Same(injected, await Assert.ThrowsAsync<InvalidOperationException>(() => Read(Stream(context))));
        Assert.Equal((2, "failure"), Assert.Single(telemetry.Completed)); Assert.Equal(1, provider.Closed);
    }

    [Fact]
    public async Task RepeatedEnumerationOwnsSeparateDeliveryCounters()
    {
        var provider = new Rows(); var telemetry = new Probe(); var context = Context(provider, telemetry);
        var stream = Stream(context);
        await using (var cursor = stream.GetAsyncEnumerator()) { Assert.True(await cursor.MoveNextAsync()); }
        Assert.Equal(new long[] { 1, 2, 3 }, await Read(stream));
        Assert.Equal(new[] { (1, "cancelled"), (3, "success") }, telemetry.Completed);
        Assert.Equal(2, provider.Opened); Assert.Equal(2, provider.Closed);
    }

    [Fact]
    public async Task RawChunkStreamRetainsChunkDeliverySemantics()
    {
        var provider = new Rows(); var telemetry = new Probe(); var context = Context(provider, telemetry);
        var streaming = Assert.IsAssignableFrom<IStreamQueryExecutor>(context.RequireResource<IDataService>());
        await using (var cursor = streaming.QueryStreamAsync(Request(), 2).GetAsyncEnumerator())
        { Assert.True(await cursor.MoveNextAsync()); Assert.Equal(2, cursor.Current.Rows.Count); }
        Assert.Equal((2, "cancelled"), Assert.Single(telemetry.Completed)); Assert.Equal(1, provider.Closed);
    }

    [Fact]
    public async Task ThrowingTelemetryCompletionCannotChangeStreamResult()
    {
        var provider = new Rows(); var telemetry = new Probe { ThrowCompletion = true }; var context = Context(provider, telemetry);
        Assert.Equal(new long[] { 1, 2, 3 }, await Read(Stream(context))); Assert.Equal(1, provider.Closed);
    }

    [Fact]
    public async Task DisposalCannotReplaceAnExistingHydrationFailure()
    {
        var provider = new Rows { DisposeFailure = new InvalidOperationException("PRIVATE-CLEANUP-CANARY") };
        var telemetry = new Probe(); var context = Context(provider, telemetry);
        var original = new FormatException("PRIVATE-HYDRATION-CANARY");
        var stream = RuntimeDataService.StreamEntities<long>(context, Request(), _ => throw original, 2);
        Assert.Same(original, await Assert.ThrowsAsync<FormatException>(() => Read(stream)));
        Assert.Same(original, telemetry.Error); Assert.Equal((0, "failure"), Assert.Single(telemetry.Completed)); Assert.Equal(1, provider.Closed);
    }

    [Fact]
    public async Task LegacyApplicationScopeUsesItsExistingFailureContract()
    {
        var provider = new Rows(); var legacy = new Legacy(); var context = Context(provider, legacy);
        await using (var cursor = Stream(context).GetAsyncEnumerator()) { Assert.True(await cursor.MoveNextAsync()); }
        Assert.Equal(1, legacy.Failures); Assert.IsType<OperationCanceledException>(legacy.Error); Assert.Equal(1, provider.Closed);
    }

    private static UserContext Context(Rows provider, IRuntimeTelemetry telemetry) => new UserContext().WithDataService(provider).WithRuntimeTelemetry(telemetry);
    private static QueryRequest Request() => new(new SelectQuery("Entry").Comment("stream entries").Purpose("verify lazy typed streaming").Limit(3));
    private static IAsyncEnumerable<long> Stream(UserContext context) => RuntimeDataService.StreamEntities(context, Request(), row => row["id"].TryI64()!.Value, 2);
    private static async Task<long[]> Read(IAsyncEnumerable<long> source) { var rows = new List<long>(); await foreach (var row in source) rows.Add(row); return rows.ToArray(); }

    private sealed class Rows : IStreamQueryExecutor
    {
        public int Opened, Closed; public Exception? Failure, DisposeFailure;
        public DataServiceCapabilities Capabilities { get; } = new() { Query = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => Task.FromResult(new QueryResult());
        public Task<MutationResult> MutateAsync(MutationRequest request) => throw new NotSupportedException();
        public async IAsyncEnumerable<StreamChunk> QueryStreamAsync(QueryRequest request, int chunkSize, [EnumeratorCancellation] CancellationToken token = default)
        {
            Opened++;
            try {
                yield return new StreamChunk { Rows = [Row(1), Row(2)], ChunkIndex = 0 };
                await Task.Yield(); token.ThrowIfCancellationRequested();
                if (Failure != null) throw Failure;
                yield return new StreamChunk { Rows = [Row(3)], ChunkIndex = 1, IsLast = true };
            }
            finally { Closed++; if (DisposeFailure != null) throw DisposeFailure; }
        }
        private static Record Row(long id) => new() { ["id"] = new Value.I64Value(id) };
    }
    private sealed class Probe : IRuntimeTelemetry
    {
        public List<(int Count, string Outcome)> Completed { get; } = new();
        public Exception? Error; public bool ThrowCompletion;
        public IRuntimeTelemetryScope Start(RuntimeOperation operation) => new Scope(this);
        private sealed class Scope(Probe owner) : IRuntimeTelemetryScope
        {
            public void Success(IReadOnlyDictionary<string, object>? attributes = null) => End(null, attributes);
            public void Failure(Exception error) => End(error, null);
            public void Failure(Exception error, IReadOnlyDictionary<string, object>? attributes) => End(error, attributes);
            private void End(Exception? error, IReadOnlyDictionary<string, object>? attributes)
            {
                owner.Error = error; owner.Completed.Add((Convert.ToInt32(attributes!["teaql.result.cardinality"]),
                    error == null ? "success" : error is OperationCanceledException ? "cancelled" : "failure"));
                if (owner.ThrowCompletion) throw new InvalidOperationException("PRIVATE-TELEMETRY-CANARY");
            }
        }
    }
    private sealed class Legacy : IRuntimeTelemetry, IRuntimeTelemetryScope
    {
        public int Failures; public Exception? Error;
        public IRuntimeTelemetryScope Start(RuntimeOperation operation) => this;
        public void Success(IReadOnlyDictionary<string, object>? attributes = null) { }
        public void Failure(Exception error) { Failures++; Error = error; }
    }
}
