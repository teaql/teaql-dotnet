using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class InternalIdAllocatorSqliteTests
{
    // A transaction adapter intentionally without the legacy ID capability.
    // The custom context allocator must still make graph creation possible.
    private sealed class IdBlindTransactions(ITransactionExecutor inner) : ITransactionExecutor
    {
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Task<QueryResult> QueryAsync(QueryRequest request) => inner.QueryAsync(request);
        public Task<MutationResult> MutateAsync(MutationRequest request) => inner.MutateAsync(request);
        public async Task<ITransaction> BeginTransactionAsync() =>
            new IdBlindTransaction(await inner.BeginTransactionAsync());

        private sealed class IdBlindTransaction(ITransaction inner) : ITransaction
        {
            public DataServiceCapabilities Capabilities => inner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) => inner.QueryAsync(request);
            public Task<MutationResult> MutateAsync(MutationRequest request) => inner.MutateAsync(request);
            public Task CommitAsync() => inner.CommitAsync();
            public Task RollbackAsync() => inner.RollbackAsync();
            public void Dispose() => inner.Dispose();
        }
    }

    private sealed class Allocator : IInternalIdAllocator
    {
        private readonly object _gate = new();
        private readonly Dictionary<string, ulong> _floor = new();
        public readonly List<(string Entity, ulong Floor)> Reservations = new();
        public bool FailNext { get; set; }
        public bool FailFloor { get; set; }

        public Task<ulong> NextIdAsync(string entity)
        {
            lock (_gate)
            {
                if (FailNext) throw new InvalidOperationException("custom allocation rejected");
                var next = (_floor.TryGetValue(entity, out var floor) ? floor : 499UL) + 1;
                _floor[entity] = next;
                return Task.FromResult(next);
            }
        }

        public Task EnsureIdFloorAsync(string entity, ulong floor)
        {
            lock (_gate)
            {
                if (FailFloor) throw new InvalidOperationException("custom floor rejected");
                Reservations.Add((entity, floor));
                _floor[entity] = Math.Max(_floor.TryGetValue(entity, out var current) ? current : 499UL, floor);
                return Task.CompletedTask;
            }
        }
    }

    private static async Task<(SqliteConnection Db, UserContext Context, Allocator Allocator)> Open()
    {
        var db = new SqliteConnection("Data Source=:memory:");
        await db.OpenAsync();
        var school = EntityDescriptor.New("School").TableName("school_data")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("name", DataType.Text));
        var module = new RuntimeModule().Entity(school);
        var sql = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(db),
            new MetadataSchemaProvider(name => name == "School" ? school : null));
        var allocator = new Allocator();
        var context = module.IntoContext().WithInternalIdAllocator(allocator).WithDataService(sql)
            .DisableMutationSqlLog().DisableQuerySqlLog();
        await context.EnsureSchemaAsync();
        return (db, context, allocator);
    }

    private static InsertMutationRequest Insert(string name, ulong? id = null)
    {
        var command = new InsertCommand("School").Value("name", new Value.TextValue(name));
        if (id is { } explicitId) command.Value("id", new Value.U64Value(explicitId));
        return new InsertMutationRequest(command, "create school for internal ID test");
    }

    private static async Task<long[]> Ids(SqliteConnection db)
    {
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT id FROM school_data ORDER BY id";
        await using var reader = await command.ExecuteReaderAsync();
        var ids = new List<long>();
        while (await reader.ReadAsync()) ids.Add(reader.GetInt64(0));
        return ids.ToArray();
    }

    private static async Task AssertDefaultIdSpaceUntouched(SqliteConnection db)
    {
        await using var command = db.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='teaql_id_space'";
        Assert.Equal(0L, Convert.ToInt64(await command.ExecuteScalarAsync()));
    }

    [Fact]
    public async Task ContextAllocatorCoversDirectBatchExplicitFloorAndGraphWithoutDefaultIdSpace()
    {
        var fixture = await Open(); await using var db = fixture.Db;
        var context = fixture.Context; var allocator = fixture.Allocator;
        var service = context.RequireResource<IDataService>();

        await service.MutateAsync(Insert("direct"));
        await service.MutateAsync(new BatchMutationRequest(new List<MutationRequest> {
            Insert("batch one"),
            new BatchMutationRequest(new List<MutationRequest> { Insert("batch nested") }, "nested batch"),
        }, "batch of schools"));
        await service.MutateAsync(Insert("explicit", 900));

        await context.ExecuteGraphSaveAsync("save school graph", async graph =>
        {
            Assert.Equal(901UL, await graph.AllocateIdAsync("School"));
            await graph.MutateAsync(Insert("graph", 901));
            var concurrent = await Task.WhenAll(Enumerable.Range(0, 24)
                .Select(_ => graph.AllocateIdAsync("School")));
            Assert.Equal(24, concurrent.Distinct().Count());
            Assert.Equal(902UL, concurrent.Min());
            return true;
        });

        Assert.Equal(new long[] { 500, 501, 502, 900, 901 }, await Ids(db));
        Assert.Contains(("School", 900UL), allocator.Reservations);
        Assert.Contains(("School", 901UL), allocator.Reservations);
        await AssertDefaultIdSpaceUntouched(db);
    }

    [Fact]
    public async Task AllocatorFailureDoesNotFallBackToProviderOrWriteRow()
    {
        var fixture = await Open(); await using var db = fixture.Db;
        fixture.Allocator.FailNext = true;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Context.RequireResource<IDataService>().MutateAsync(Insert("rejected")));
        Assert.Equal("custom allocation rejected", error.Message);
        Assert.Empty(await Ids(db));
        await AssertDefaultIdSpaceUntouched(db);
    }

    [Fact]
    public async Task ExplicitFloorFailureDoesNotFallBackOrWriteRow()
    {
        var fixture = await Open(); await using var db = fixture.Db;
        fixture.Allocator.FailFloor = true;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Context.RequireResource<IDataService>().MutateAsync(Insert("rejected explicit", 900)));
        Assert.Equal("custom floor rejected", error.Message);
        Assert.Empty(await Ids(db));
        await AssertDefaultIdSpaceUntouched(db);
    }

    [Fact]
    public async Task GraphUsesCustomAllocatorWhenTransactionHasNoIdCapability()
    {
        var fixture = await Open(); await using var db = fixture.Db;
        var school = fixture.Context.Metadata!.GetEntity("School")!;
        var sql = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(db),
            new MetadataSchemaProvider(name => name == "School" ? school : null));
        fixture.Context.InsertResource<ITransactionExecutor>(new IdBlindTransactions(sql));

        await fixture.Context.ExecuteGraphSaveAsync("save with independent ID capability", async graph =>
        {
            var id = await graph.AllocateIdAsync("School");
            Assert.Equal(500UL, id);
            await graph.MutateAsync(Insert("graph with ID-blind transaction", id));
            return true;
        });

        Assert.Equal(new long[] { 500 }, await Ids(db));
        await AssertDefaultIdSpaceUntouched(db);
    }

    [Fact]
    public async Task NoOverrideRetainsProviderAllocation()
    {
        var fixture = await Open(); await using var db = fixture.Db;
        var school = fixture.Context.Metadata!.GetEntity("School")!;
        var sql = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(db),
            new MetadataSchemaProvider(name => name == "School" ? school : null));
        var plain = new RuntimeModule().Entity(school).IntoContext().WithDataService(sql)
            .DisableMutationSqlLog();
        await plain.RequireResource<IDataService>().MutateAsync(Insert("default"));
        Assert.Equal(new long[] { 1 }, await Ids(db));
    }
}
