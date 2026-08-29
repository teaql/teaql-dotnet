using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public class IdSetPaginationTests
{
    [Fact]
    public async Task IDSET_001_IsOptInAndDisabledByDefault()
    {
        var provider = new DynamicDataService(5, 4, 3, 2, 1);
        var context = Context(provider, new InMemoryIdSetStore(), "disabled");
        var query = new SelectQuery("School").OrderDesc("id").Limit(2);
        var result = await context.RequireResource<IDataService>().QueryAsync(new QueryRequest { Query = query });
        Assert.Equal("ID_SET_DISABLED", context.IdSetPlan);
        Assert.Equal(1, provider.Calls);
        Assert.Equal(new ulong[] { 5, 4 }, Ids(result));
    }

    [Fact]
    public async Task IDSET_002_003_005_006_BuildsWithoutCountAndPreservesArbitraryPageOrder()
    {
        var provider = new ScriptedDataService();
        var context = new UserContext { UserIdentifier = "id-set-test" }
            .WithIdSetStore(new InMemoryIdSetStore())
            .WithDataService(provider);
        var service = context.RequireResource<IDataService>();

        var first = await service.QueryAsync(new QueryRequest
        {
            Query = Query(0)
        });
        Assert.Equal(new ulong[] { 5, 4 }, Ids(first));
        Assert.Equal("ID_SET_BUILD", context.IdSetPlan);
        Assert.Equal("EXACT", context.IdSetCountAccuracy);
        Assert.Equal(5UL, context.IdSetCount);

        var second = await service.QueryAsync(new QueryRequest
        {
            Query = Query(2)
        });
        Assert.Equal(new ulong[] { 3, 2 }, Ids(second));
        Assert.Equal("ID_SET_HIT", context.IdSetPlan);
        Assert.Equal(3, provider.Calls);
        Assert.DoesNotContain(provider.Queries, query => query.AggregateItems.Count != 0);
    }

    [Fact]
    public async Task IDSET_004_EmptySetIsRetainedWithExactZeroCount()
    {
        var provider = new ScriptedDataService(Array.Empty<ulong>());
        var context = new UserContext { UserIdentifier = "empty-id-set-test" }
            .WithIdSetStore(new InMemoryIdSetStore())
            .WithDataService(provider);

        var result = await context.RequireResource<IDataService>().QueryAsync(new QueryRequest
        {
            Query = Query(0)
        });

        Assert.Empty(result.Rows);
        Assert.Equal(1, provider.Calls);
        Assert.Equal("ID_SET_BUILD", context.IdSetPlan);
        Assert.Equal("EXACT", context.IdSetCountAccuracy);
        Assert.Equal(0UL, context.IdSetCount);
    }

    [Fact]
    public async Task IDSET_007_AddsDeterministicIdTieBreaker()
    {
        var provider = new DynamicDataService(3, 2, 1);
        var context = Context(provider, new InMemoryIdSetStore(), "tie-breaker");
        var query = new SelectQuery("School").OrderDesc("name").Limit(2)
            .OptimizePaginationWithIdSet("tie-breaker", 60, 100);
        await context.RequireResource<IDataService>().QueryAsync(new QueryRequest { Query = query });
        var build = Assert.Single(provider.Queries.Where(item => item.Projection.SequenceEqual(new[] { "id" })));
        Assert.Contains(build.OrderByItems, order => order.Field == "id");
    }

    [Fact]
    public async Task IDSET_008_OverflowFallsBackWithoutFalseExactCount()
    {
        var provider = new DynamicDataService(5, 4, 3, 2, 1);
        var context = Context(provider, new InMemoryIdSetStore(), "overflow");
        var query = Query(0).OptimizePaginationWithIdSet("overflow", 60, 2);
        var result = await context.RequireResource<IDataService>().QueryAsync(new QueryRequest { Query = query });
        Assert.Equal("ID_SET_FALLBACK_LIMIT_EXCEEDED", context.IdSetPlan);
        Assert.Equal("LOWER_BOUND", context.IdSetCountAccuracy);
        Assert.Equal(3UL, context.IdSetCount);
        Assert.Equal(new ulong[] { 5, 4 }, Ids(result));
    }

    [Fact]
    public async Task IDSET_009_TtlExpiryRebuildsSet()
    {
        var provider = new DynamicDataService(5, 4, 3, 2, 1);
        var store = new InMemoryIdSetStore();
        var context = Context(provider, store, "ttl");
        var service = context.RequireResource<IDataService>();
        var query = Query(0).OptimizePaginationWithIdSet("ttl", 1, 100);
        await service.QueryAsync(new QueryRequest { Query = query });
        await Task.Delay(1100);
        await service.QueryAsync(new QueryRequest { Query = query });
        Assert.Equal("ID_SET_BUILD", context.IdSetPlan);
        Assert.Equal(2, provider.IdBuildCalls);
    }

    [Fact]
    public async Task IDSET_010_SecurityParametersAndDataSourceIsolateIdentity()
    {
        var store = new InMemoryIdSetStore();
        var provider = new DynamicDataService(5, 4, 3, 2, 1);
        async Task Run(UserContext context, string parameter) =>
            await context.RequireResource<IDataService>().QueryAsync(new QueryRequest
            {
                Query = Query(0).AndFilter(Expr.Eq("name", new Value.TextValue(parameter)))
            });

        await Run(Context(provider, store, "user-a").WithTrustedTenant("tenant-a").WithActiveRoot("Platform", 1), "A");
        await Run(Context(provider, store, "user-b").WithTrustedTenant("tenant-a").WithActiveRoot("Platform", 1), "A");
        await Run(Context(provider, store, "user-a").WithTrustedTenant("tenant-b").WithActiveRoot("Platform", 1), "A");
        await Run(Context(provider, store, "user-a").WithTrustedTenant("tenant-a").WithActiveRoot("Platform", 2), "A");
        await Run(Context(provider, store, "user-a").WithTrustedTenant("tenant-a").WithActiveRoot("Platform", 1), "B");
        var policyContext = Context(provider, store, "user-a").WithTrustedTenant("tenant-a").WithActiveRoot("Platform", 1)
            .WithRequestPolicy(new PassPolicy());
        await Run(policyContext, "A");
        var otherProvider = new DynamicDataService(5, 4, 3, 2, 1);
        await Run(Context(otherProvider, store, "user-a").WithTrustedTenant("tenant-a").WithActiveRoot("Platform", 1), "A");
        Assert.Equal(6, provider.IdBuildCalls);
        Assert.Equal(1, otherProvider.IdBuildCalls);
    }

    [Fact]
    public async Task IDSET_011_ConcurrentCrossContextMissUsesSingleBuild()
    {
        var store = new InMemoryIdSetStore();
        var provider = new DynamicDataService(5, 4, 3, 2, 1) { BuildDelayMs = 100 };
        var first = Context(provider, store, "same-user");
        var second = Context(provider, store, "same-user");
        await Task.WhenAll(
            first.RequireResource<IDataService>().QueryAsync(new QueryRequest { Query = Query(0) }),
            second.RequireResource<IDataService>().QueryAsync(new QueryRequest { Query = Query(2) }));
        Assert.Equal(1, provider.IdBuildCalls);
        Assert.Contains(new[] { first.IdSetPlan, second.IdSetPlan }, plan => plan == "ID_SET_BUILD");
        Assert.Contains(new[] { first.IdSetPlan, second.IdSetPlan }, plan => plan == "ID_SET_HIT");
    }

    [Fact]
    public async Task IDSET_012_StoreFailureFallsBackWithoutChangingRows()
    {
        var provider = new DynamicDataService(5, 4, 3, 2, 1);
        var context = Context(provider, new FailingStore(), "store-failure");
        var result = await context.RequireResource<IDataService>().QueryAsync(new QueryRequest { Query = Query(0) });
        Assert.Equal("ID_SET_FALLBACK_STORE_UNAVAILABLE", context.IdSetPlan);
        Assert.Equal(new ulong[] { 5, 4 }, Ids(result));
    }

    [Fact]
    public async Task IDSET_013_UnsupportedAggregateShapeFallsBackVisibly()
    {
        var provider = new DynamicDataService(5, 4, 3, 2, 1);
        var context = Context(provider, new InMemoryIdSetStore(), "unsupported");
        var query = Query(0).Count("count");
        await context.RequireResource<IDataService>().QueryAsync(new QueryRequest { Query = query });
        Assert.Equal("ID_SET_FALLBACK_UNSUPPORTED_SHAPE", context.IdSetPlan);
        Assert.Equal(1, provider.Calls);
    }

    [Fact]
    public async Task IDSET_014_DeletionDoesNotShiftAnotherIdIntoRetainedPage()
    {
        var provider = new DynamicDataService(5, 4, 3, 2, 1);
        var context = Context(provider, new InMemoryIdSetStore(), "delete-stability");
        var service = context.RequireResource<IDataService>();
        await service.QueryAsync(new QueryRequest { Query = Query(0) });
        provider.Delete(4);
        var page = await service.QueryAsync(new QueryRequest { Query = Query(2) });
        Assert.Equal(new ulong[] { 3, 2 }, Ids(page));
        Assert.Equal("ID_SET_HIT", context.IdSetPlan);
        Assert.Equal(1, provider.IdBuildCalls);
    }

    private static SelectQuery Query(ulong offset) => new SelectQuery("School")
        .OrderDesc("id")
        .Offset(offset)
        .Limit(2)
        .OptimizePaginationWithIdSet("tests", 60, 100);

    private static ulong[] Ids(QueryResult result) => result.Rows
        .Select(row => row["id"].TryU64()!.Value).ToArray();

    private static UserContext Context(IDataService provider, IIdSetStore store, string user) =>
        new UserContext { UserIdentifier = user }.WithIdSetStore(store).WithDataService(provider);

    private sealed class PassPolicy : IRequestPolicy
    {
        public SelectQuery Apply(SelectQuery query) => query;
    }

    private sealed class FailingStore : IIdSetStore
    {
        public ValueTask<RetainedIdSet?> GetAsync(string queryKey) => throw new IOException("store unavailable");
        public ValueTask PutAsync(RetainedIdSet retained) => throw new IOException("store unavailable");
        public ValueTask InvalidateAsync(string queryKey) => throw new IOException("store unavailable");
    }

    private sealed class DynamicDataService(params ulong[] initialIds) : IDataService
    {
        private readonly object _gate = new();
        private readonly List<ulong> _ids = initialIds.ToList();
        private int _calls;
        private int _idBuildCalls;
        public int Calls => Volatile.Read(ref _calls);
        public int IdBuildCalls => Volatile.Read(ref _idBuildCalls);
        public int BuildDelayMs { get; init; }
        public List<SelectQuery> Queries { get; } = new();
        public DataServiceCapabilities Capabilities { get; } = new() { Query = true };

        public void Delete(ulong id) { lock (_gate) _ids.Remove(id); }

        public async Task<QueryResult> QueryAsync(QueryRequest request)
        {
            Interlocked.Increment(ref _calls);
            lock (_gate) Queries.Add(request.Query.CloneForExecution());
            var isBuild = request.Query.Projection.SequenceEqual(new[] { "id" }) && request.Query.IdSetPagination == null;
            if (isBuild)
            {
                Interlocked.Increment(ref _idBuildCalls);
                if (BuildDelayMs > 0) await Task.Delay(BuildDelayMs);
            }
            List<ulong> selected;
            lock (_gate)
            {
                selected = RequestedIds(request.Query.FilterCondition) ?? _ids.ToList();
            }
            if (request.Query.OrderByItems.FirstOrDefault()?.Direction == SortDirection.Desc)
                selected = selected.OrderByDescending(id => id).ToList();
            else selected = selected.OrderBy(id => id).ToList();
            if (request.Query.Slice is { } slice)
                selected = selected.Skip((int)slice.Offset).Take((int)(slice.Limit ?? ulong.MaxValue)).ToList();
            return new QueryResult
            {
                Rows = selected.Select(id => new TeaQL.Core.Record
                {
                    ["id"] = new Value.U64Value(id), ["name"] = new Value.TextValue("school-" + id)
                }).ToList()
            };
        }

        public Task<MutationResult> MutateAsync(MutationRequest request) => throw new NotSupportedException();

        private static List<ulong>? RequestedIds(Expr? expression)
        {
            if (expression is Expr.BinaryExpr
                { Left: Expr.ColumnExpr { Name: "id" }, Op: BinaryOp.In,
                  Right: Expr.ValueExpr { NodeValue: Value.ListValue list } })
                return list.Values.Select(value => value.TryU64()!.Value).ToList();
            if (expression is Expr.AndExpr and)
                foreach (var part in and.Parts)
                    if (RequestedIds(part) is { } ids) return ids;
            return null;
        }
    }

    private sealed class ScriptedDataService : IDataService
    {
        private readonly ulong[] _ids;
        public int Calls { get; private set; }
        public List<SelectQuery> Queries { get; } = new();
        public DataServiceCapabilities Capabilities { get; } = new() { Query = true };

        public ScriptedDataService(params ulong[] ids)
        {
            _ids = ids.Length == 0 ? Array.Empty<ulong>() : ids;
        }

        public ScriptedDataService() : this(5, 4, 3, 2, 1) { }

        public Task<QueryResult> QueryAsync(QueryRequest request)
        {
            Calls++;
            Queries.Add(request.Query.CloneForExecution());
            IEnumerable<ulong> selected;
            if (request.Query.Projection.SequenceEqual(new[] { "id" }))
            {
                selected = _ids;
            }
            else
            {
                // Return deliberately reversed provider order. Runtime must restore
                // the retained order selected by the requested page.
                selected = (Calls == 2 ? new ulong[] { 4, 5 } : new ulong[] { 2, 3 });
            }
            return Task.FromResult(new QueryResult
            {
                Rows = selected.Select(id => new TeaQL.Core.Record
                {
                    ["id"] = new Value.U64Value(id),
                    ["name"] = new Value.TextValue("school-" + id)
                }).ToList()
            });
        }

        public Task<MutationResult> MutateAsync(MutationRequest request) =>
            throw new NotSupportedException();
    }
}
