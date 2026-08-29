using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public class IdSetPaginationTests
{
    [Fact]
    public async Task RetainsOrderedIdsAcrossPagesAndAvoidsCountQuery()
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
    public async Task EmptyRetainedSetIsExactAndDoesNotExecutePageQuery()
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

    private static SelectQuery Query(ulong offset) => new SelectQuery("School")
        .OrderDesc("id")
        .Offset(offset)
        .Limit(2)
        .OptimizePaginationWithIdSet("tests", 60, 100);

    private static ulong[] Ids(QueryResult result) => result.Rows
        .Select(row => row["id"].TryU64()!.Value).ToArray();

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
