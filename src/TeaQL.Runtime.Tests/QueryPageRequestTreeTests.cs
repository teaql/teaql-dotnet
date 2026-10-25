using TeaQL.Core;
using TeaQL.DataService;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Xunit;

namespace TeaQL.Runtime.Tests;

[Collection("Runtime telemetry SDK")]
public class QueryPageRequestTreeTests
{
    [Fact]
    public async Task NamedCountOwnsItsOperationAndDoesNotAggregatePageRows()
    {
        var query = new SelectQuery("CustomerOrder").Comment("load orders").Purpose("render orders");
        query.CountField("id", "totalOrders");
        query.PageCountOperation = new("count", "CountAs", "totalOrders");
        query.Limit(1).Offset(1);
        var request = new QueryRequest(query);
        var service = new PageService();
        var page = await RuntimeDataService.FetchPageAsync(service, new UserContext(), request);
        Assert.Equal(3, page.TotalCount);
        Assert.Equal(3, page.Aggregations["totalOrders"].TryI64());
        Assert.Single(page);
        Assert.Equal(2, service.Requests.Count);
        var count = service.Requests[0]; var rows = service.Requests[1];
        Assert.Equal("CountAs(\"totalOrders\")", count.Comment);
        Assert.Equal("render orders", count.Purpose);
        Assert.Same(request.Scope.Root, count.Scope.Root);
        Assert.Same(request.Scope, count.Scope.Parent);
        Assert.Null(count.Query.Slice);
        Assert.Equal("totalOrders", Assert.Single(count.Query.AggregateItems).Alias);
        Assert.Empty(rows.Query.AggregateItems);
        Assert.Equal(new Slice(1, 1), rows.Query.Slice);
        Assert.Same(request.Scope, rows.Scope);
        Assert.Single(query.AggregateItems); // Caller template was not rewritten.
    }

    [Fact]
    public async Task FailedCountDoesNotProduceRowsOrInventSuccess()
    {
        var query = new SelectQuery("CustomerOrder").Comment("load orders").Purpose("render orders");
        query.PageCountOperation = new("count", "CountAs", "totalOrders");
        var service = new PageService { Fail = true };
        var failure = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            RuntimeDataService.FetchPageAsync(service, new UserContext(), new QueryRequest(query)));
        Assert.Equal("count failed", failure.Message);
        Assert.Single(service.Requests);
    }

    [Fact]
    public async Task CountAndListShareOneOperationRootWithDeliveredCardinality()
    {
        var spans = new List<Activity>();
        using var tracer = Sdk.CreateTracerProviderBuilder().AddSource("io.teaql.runtime").AddInMemoryExporter(spans).Build();
        using var telemetry = new OpenTelemetryRuntimeTelemetry();
        var provider = new PageService();
        var context = new UserContext().WithDataService(provider).WithRuntimeTelemetry(telemetry);
        var request = CountedRequest();
        var page = await RuntimeDataService.FetchPageAsync(context.RequireResource<IDataService>(), context, request);
        tracer.ForceFlush();
        Assert.Equal(3, page.TotalCount);
        var root = Assert.Single(spans.Where(span => span.OperationName == "teaql.query"));
        Assert.Equal("CustomerOrder.list", root.GetTagItem("teaql.operation.name"));
        Assert.Equal(1, Convert.ToInt32(root.GetTagItem("teaql.result.cardinality")));
        var branches = spans.Where(span => span.OperationName == "teaql.provider").ToArray();
        Assert.Equal(2, branches.Length);
        Assert.All(branches, span => { Assert.Equal(root.TraceId, span.TraceId); Assert.Equal(root.SpanId, span.ParentSpanId); });
        Assert.Same(request.Scope.Root, provider.Requests[0].Scope.Root);
        Assert.Same(request.Scope.Root, provider.Requests[1].Scope.Root);
    }

    [Fact]
    public async Task FailedCountClosesTheSingleRootAndPreservesBusinessError()
    {
        var spans = new List<Activity>();
        using var tracer = Sdk.CreateTracerProviderBuilder().AddSource("io.teaql.runtime").AddInMemoryExporter(spans).Build();
        using var telemetry = new OpenTelemetryRuntimeTelemetry();
        var provider = new PageService { Fail = true };
        var context = new UserContext().WithDataService(provider).WithRuntimeTelemetry(telemetry);
        var failure = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            RuntimeDataService.FetchPageAsync(context.RequireResource<IDataService>(), context, CountedRequest()));
        tracer.ForceFlush();
        Assert.Equal("count failed", failure.Message);
        Assert.Single(provider.Requests);
        var root = Assert.Single(spans.Where(span => span.OperationName == "teaql.query"));
        Assert.Equal(ActivityStatusCode.Error, root.Status);
        Assert.DoesNotContain(spans.SelectMany(span => span.TagObjects), tag => Equals(tag.Value, "count failed"));
    }

    [Fact]
    public async Task IndependentNestedQueryIsNotSuppressedByCountedListOwnership()
    {
        var spans = new List<Activity>();
        using var tracer = Sdk.CreateTracerProviderBuilder().AddSource("io.teaql.runtime").AddInMemoryExporter(spans).Build();
        using var telemetry = new OpenTelemetryRuntimeTelemetry();
        var provider = new PageService();
        var context = new UserContext().WithDataService(provider).WithRuntimeTelemetry(telemetry);
        provider.BeforeQuery = async request => {
            if (request.Query.AggregateItems.Count > 0)
                await context.RequireResource<IDataService>().QueryAsync(new QueryRequest(
                    new SelectQuery("OrderStatus").Comment("load independent status").Purpose("verify independent ownership").Limit(1)));
        };
        await RuntimeDataService.FetchPageAsync(context.RequireResource<IDataService>(), context, CountedRequest());
        tracer.ForceFlush();
        Assert.Single(spans.Where(span => Equals(span.GetTagItem("teaql.operation.name"), "CustomerOrder.list")));
        Assert.Single(spans.Where(span => Equals(span.GetTagItem("teaql.operation.name"), "OrderStatus.list")));
        Assert.Equal(3, provider.Requests.Count);
    }

    [Fact]
    public async Task ConcurrentCountedListsHaveDistinctRootsWithoutContextLedgerChanges()
    {
        var spans = new List<Activity>();
        using var tracer = Sdk.CreateTracerProviderBuilder().AddSource("io.teaql.runtime").AddInMemoryExporter(spans).Build();
        using var telemetry = new OpenTelemetryRuntimeTelemetry();
        var provider = new PageService();
        var context = new UserContext().WithDataService(provider).WithRuntimeTelemetry(telemetry);
        var barrier = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); var arrivals = 0;
        provider.BeforeQuery = async request => {
            if (request.Query.AggregateItems.Count == 0) return;
            if (Interlocked.Increment(ref arrivals) == 2) barrier.SetResult();
            await barrier.Task.WaitAsync(TimeSpan.FromSeconds(5));
        };
        var service = context.RequireResource<IDataService>();
        await Task.WhenAll(RuntimeDataService.FetchPageAsync(service, context, CountedRequest()),
            RuntimeDataService.FetchPageAsync(service, context, CountedRequest()));
        tracer.ForceFlush();
        var roots = spans.Where(span => span.OperationName == "teaql.query").ToArray();
        Assert.Equal(2, roots.Length); Assert.NotEqual(roots[0].TraceId, roots[1].TraceId);
        Assert.All(roots, root => Assert.Equal(2, spans.Count(span => span.OperationName == "teaql.provider" && span.ParentSpanId == root.SpanId)));
    }

    [Fact]
    public async Task CountAlongsideScalarAggregateCountsMatchingEntitiesSeparately()
    {
        var request = CountedRequest();
        request.Query.Sum("amount", "sum");
        var service = new PageService();
        var result = await RuntimeDataService.FetchPageAsync(service, new UserContext(), request);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(AggregateFunction.Count, Assert.Single(service.Requests[0].Query.AggregateItems).Function);
        Assert.Equal(AggregateFunction.Sum, Assert.Single(service.Requests[1].Query.AggregateItems).Function);
        Assert.Equal(2, request.Query.AggregateItems.Count);
    }

    [Fact]
    public async Task ExactCountRejectsFractionNegativeOverflowAndMissingValue()
    {
        foreach (var value in new Value[] {
            new Value.DecimalValue(1.5m), new Value.I64Value(-1),
            new Value.DecimalValue(decimal.MaxValue), new Value.U64Value(ulong.MaxValue), new Value.NullValue()
        }) {
            var service = new PageService { CountValue = value };
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                RuntimeDataService.FetchPageAsync(service, new UserContext(), CountedRequest()));
            Assert.Single(service.Requests);
        }
        foreach (var value in new Value[] { new Value.DecimalValue(0m), new Value.DecimalValue(3m) }) {
            var page = await RuntimeDataService.FetchPageAsync(new PageService { CountValue = value }, new UserContext(), CountedRequest());
            Assert.Equal((long)value.TryDecimal()!, page.TotalCount);
        }
    }

    private static QueryRequest CountedRequest()
    {
        var query = new SelectQuery("CustomerOrder").Comment("load orders").Purpose("render orders").Limit(1).Offset(1);
        query.CountField("id", "totalOrders"); query.PageCountOperation = new("count", "CountAs", "totalOrders");
        return new QueryRequest(query);
    }

    private sealed class PageService : IDataService
    {
        public DataServiceCapabilities Capabilities { get; } = new() { Query = true };
        public List<QueryRequest> Requests { get; } = new();
        public bool Fail { get; init; }
        public Value CountValue { get; init; } = new Value.I64Value(3);
        public Func<QueryRequest, Task>? BeforeQuery { get; set; }
        public async Task<QueryResult> QueryAsync(QueryRequest request)
        {
            lock (Requests) Requests.Add(request);
            if (BeforeQuery != null) await BeforeQuery(request);
            if (Fail) throw new InvalidOperationException("count failed");
            var row = new TeaQL.Core.Record();
            if (request.Query.AggregateItems.Count > 0) row["totalOrders"] = CountValue;
            else row["id"] = new Value.I64Value(2);
            return new QueryResult { Rows = new() { row } };
        }
        public Task<MutationResult> MutateAsync(MutationRequest request) => throw new NotSupportedException();
    }
}
