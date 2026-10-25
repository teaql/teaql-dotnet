using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class FiniteScalarAdmissionTests
{
    public static IEnumerable<object[]> InvalidNumbers() =>
        new[] { "required_float", "optional_float", "required_double", "optional_double" }
            .SelectMany(field => new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity }
                .Select(value => new object[] { field, value }));

    [Theory]
    [MemberData(nameof(InvalidNumbers))]
    public async Task NoCheckerCannotBypassFinalFiniteGuard(string field, double value)
    {
        var provider = new ProbeProvider();
        var context = new UserContext().WithDataService(provider);
        foreach (var update in new[] { false, true })
        {
            var error = await Assert.ThrowsAsync<CheckException>(() =>
                context.RequireResource<IDataService>().MutateAsync(Request(field, value, update)));
            AssertViolation(error, field);
        }
        Assert.Equal(0, provider.Mutations);
    }

    [Theory]
    [MemberData(nameof(InvalidNumbers))]
    public async Task CompleteGraphAdmissionPrecedesTransactionAndIds(string field, double value)
    {
        var provider = new ProbeProvider();
        var context = new UserContext().WithDataService(provider);
        GraphMutationSession? expired = null;
        var error = await Assert.ThrowsAsync<CheckException>(() => context.ExecuteGraphSaveAsync<int>(
            "prepare one rejected scalar graph", graph =>
            {
                expired = graph;
                graph.Preflight(Request(field, value));
                return Task.CompletedTask;
            }, _ => throw new Exception("rejected preparation reached execution")));
        AssertViolation(error, field);
        Assert.Equal(0, provider.Begins);
        Assert.Equal(0, provider.Mutations);
        Assert.Equal("GRAPH_CAPABILITY_EXPIRED", Assert.Throws<InvalidOperationException>(() =>
            expired!.Scope("Probe", null, null)).Message);
        // Rejection must release the gate and discard its invocation-owned state.
        Assert.Equal(7, await context.ExecuteGraphSaveAsync("retry a valid graph",
            _ => Task.CompletedTask, _ => Task.FromResult(7)).WaitAsync(TimeSpan.FromSeconds(2)));
        Assert.Equal(1, provider.Begins);
    }

    [Fact]
    public async Task CustomFixMayRepairButCannotInjectNonFiniteValues()
    {
        var provider = new ProbeProvider();
        var checker = new ReplacingChecker(7.25);
        var context = new UserContext().Install(new RuntimeModule().Checker("Probe", checker))
            .WithDataService(provider);
        var request = Request("amount", double.NaN);
        await context.RequireResource<IDataService>().MutateAsync(request);
        Assert.Equal(7.25, Assert.IsType<Value.F64Value>(provider.Values!["amount"]).Value);
        Assert.Equal(1, checker.Calls);
        checker.Replacement = double.PositiveInfinity;
        AssertViolation(await Assert.ThrowsAsync<CheckException>(() =>
            context.RequireResource<IDataService>().MutateAsync(Request("amount", 1))), "amount");
        Assert.Equal(1, provider.Mutations);
    }

    [Fact]
    public async Task PreparationCannotWriteOrAllocateAndSharesOneCapturedClock()
    {
        var provider = new ProbeProvider(); var clock = new ProbeClock();
        var checker = new ReplacingChecker(7.25);
        var context = new UserContext().Install(new RuntimeModule().Checker("Probe", checker))
            .WithBusinessClock(clock).WithDataService(provider);
        var request = Request("amount", 1);
        await context.ExecuteGraphSaveAsync("save through two phases", async graph =>
        {
            graph.Preflight(request);
            Assert.Equal(0, provider.Begins);
            Assert.Equal("GRAPH_PREPARATION_CANNOT_ALLOCATE_ID", (await Assert.ThrowsAsync<InvalidOperationException>(() =>
                graph.AllocateIdAsync("Probe"))).Message);
            Assert.Equal("GRAPH_PREPARATION_CANNOT_MUTATE", (await Assert.ThrowsAsync<InvalidOperationException>(() =>
                graph.MutateAsync(request))).Message);
        }, async graph => { await graph.MutateAsync(request); return 1; });
        Assert.Equal(1, clock.Reads);
        Assert.Equal(2, checker.Times.Count);
        Assert.Equal(checker.Times[0], checker.Times[1]);
        Assert.Equal(1, provider.Begins); Assert.Equal(1, provider.Mutations);
    }

    private static MutationRequest Request(string field, double value, bool update = false) => update
        ? new UpdateMutationRequest(new UpdateCommand("Probe", new Value.I64Value(1))
            .Value(field, new Value.F64Value(value)), "update a native scalar")
        : new InsertMutationRequest(new InsertCommand("Probe")
            .Value(field, new Value.F64Value(value)), "create a native scalar");
    private static void AssertViolation(CheckException error, string field)
    {
        var violation = Assert.Single(error.Violations);
        Assert.Equal("invalid_type", violation.RuleId);
        Assert.Equal(field, violation.ModelPath);
        Assert.Null(violation.InputValue); // Diagnostics must not carry NaN/Infinity into JSON.
    }
    private sealed class ReplacingChecker(double replacement) : IEntityChecker
    {
        public double Replacement { get; set; } = replacement;
        public int Calls { get; private set; }
        public List<DateTimeOffset> Times { get; } = [];
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest mutation, DateTimeOffset now)
        {
            Calls++; Times.Add(now);
            ((InsertMutationRequest)mutation).Command.Values["amount"] = new Value.F64Value(Replacement);
            return [];
        }
    }
    private sealed class ProbeClock : IBusinessClock
    {
        public int Reads { get; private set; }
        public DateTimeOffset Now => DateTimeOffset.UnixEpoch.AddMinutes(++Reads);
    }
    private sealed class ProbeProvider : IDataService, ITransactionExecutor
    {
        public int Begins { get; private set; }
        public int Mutations { get; private set; }
        public Record? Values { get; private set; }
        public DataServiceCapabilities Capabilities => new() { Mutation = true, Transaction = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new Exception("unexpected query");
        public Task<MutationResult> MutateAsync(MutationRequest request)
        {
            Mutations++; Values = new Record(((InsertMutationRequest)request).Command.Values);
            return Task.FromResult(new MutationResult { AffectedRows = 1 });
        }
        public Task<ITransaction> BeginTransactionAsync() { Begins++; return Task.FromResult<ITransaction>(new Transaction(this)); }
        private sealed class Transaction(ProbeProvider owner) : ITransaction
        {
            public DataServiceCapabilities Capabilities => owner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) => owner.QueryAsync(request);
            public Task<MutationResult> MutateAsync(MutationRequest request) => owner.MutateAsync(request);
            public Task CommitAsync() => Task.CompletedTask;
            public Task RollbackAsync() => Task.CompletedTask;
            public void Dispose() { }
        }
    }
}
