using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class IntegralAdmissionTests
{
    public static IEnumerable<object[]> InvalidValues()
    {
        (string Name, string Field, Value Value)[] cases = [
            ("fractional-long", "required_long", new Value.DecimalValue(12.75m)),
            ("long-overflow", "required_long", new Value.U64Value(ulong.MaxValue)),
            ("long-float", "required_long", new Value.F64Value(12)),
            ("long-text", "required_long", new Value.TextValue("PRIVATE_INVALID_INTEGER")),
            ("long-bool", "required_long", new Value.BoolValue(true)),
            ("integer-positive-overflow", "required_integer", new Value.I64Value((long)int.MaxValue + 1)),
            ("integer-negative-overflow", "required_integer", new Value.I64Value((long)int.MinValue - 1)),
            ("integer-scaled-overflow", "required_integer", new Value.DecimalValue((decimal)int.MaxValue + 1)),
            ("fractional-reference", "type_platform", new Value.DecimalValue(1.5m)),
            ("negative-reference", "type_platform", new Value.I64Value(-1)),
            ("reference-overflow", "type_platform", new Value.DecimalValue(18446744073709551616m)),
            ("reference-float", "type_platform", new Value.F64Value(1)),
            ("reference-text", "type_platform", new Value.TextValue("PRIVATE_INVALID_REFERENCE")),
            ("reference-bool", "type_platform", new Value.BoolValue(true))
        ];
        foreach (var update in new[] { false, true })
            foreach (var item in cases) yield return [item.Name, item.Field, item.Value, update];
    }

    [Theory]
    [MemberData(nameof(InvalidValues))]
    public async Task IntegralAdmissionChecksFinalFixOutputBeforeProvider(string name, string field, Value bad, bool update)
    {
        var provider = new ProbeProvider();
        var checker = new ReplacingChecker(field, bad);
        var context = Context(provider, checker);
        var error = await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>()
            .MutateAsync(Request(field, new Value.I64Value(1), update)));
        var violation = Assert.Single(error.Violations);
        Assert.Equal("invalid_type", violation.RuleId); Assert.Equal("IntegralProbe", violation.EntityType);
        Assert.Equal(field, violation.ModelPath); Assert.Equal(field switch {
            "required_long" => "/requiredLong", "required_integer" => "/requiredInteger", _ => "/typePlatform" }, violation.InstancePath);
        Assert.Null(violation.InputValue); Assert.DoesNotContain("PRIVATE_INVALID", error.Message);
        Assert.Equal(1, checker.Calls); Assert.True(provider.Mutations == 0, name + " reached the provider"); Assert.Equal(0, provider.Begins);
    }

    [Fact]
    public async Task IntegralAdmissionDoesNotDependOnInstallingAChecker()
    {
        var provider = new ProbeProvider(); var context = Context(provider);
        await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>()
            .MutateAsync(Request("required_long", new Value.DecimalValue(12.75m))));
        Assert.Equal(0, provider.Mutations);
    }

    [Fact]
    public async Task IntegralAdmissionPermitsAnExactCustomerRepair()
    {
        var provider = new ProbeProvider(); var fix = new ReplacingChecker("required_long", new Value.DecimalValue(long.MaxValue));
        var context = Context(provider, fix);
        await context.RequireResource<IDataService>().MutateAsync(Request("required_long", new Value.DecimalValue(12.75m)));
        Assert.Equal(1, provider.Mutations); Assert.Equal(long.MaxValue, provider.Values!["required_long"].TryI64());
    }

    [Fact]
    public async Task IntegralAdmissionRejectsGraphPreparationBeforeTransactionAndLeavesRetryPossible()
    {
        var provider = new ProbeProvider(); var context = Context(provider);
        await Assert.ThrowsAsync<CheckException>(() => context.ExecuteGraphSaveAsync<int>("reject invalid integer before transaction",
            graph => { graph.Preflight(Request("required_long", new Value.DecimalValue(12.75m))); return Task.CompletedTask; },
            _ => throw new Exception("invalid graph entered work")));
        Assert.Equal(0, provider.Begins); Assert.Equal(0, provider.Mutations);
        Assert.Equal(7, await context.ExecuteGraphSaveAsync("retry a valid invocation", _ => Task.FromResult(7)).WaitAsync(TimeSpan.FromSeconds(2)));
        Assert.Equal(1, provider.Begins);
    }

    private static UserContext Context(ProbeProvider provider, IEntityChecker? checker = null)
    {
        var schema = EntityDescriptor.New("IntegralProbe")
            .Property(PropertyDescriptor.New("required_long", DataType.I64))
            .Property(PropertyDescriptor.New("required_integer", DataType.I64).Int32())
            .Property(PropertyDescriptor.New("type_platform", DataType.U64))
            .Relation(RelationDescriptor.New("type_platform", "Platform").LocalKey("type_platform").ForeignKey("id"));
        var module = new RuntimeModule().Entity(schema);
        if (checker != null) module.Checker("IntegralProbe", checker);
        return module.IntoContext().WithDataService(provider);
    }
    private static MutationRequest Request(string field, Value value, bool update = false) => update
        ? new UpdateMutationRequest(new UpdateCommand("IntegralProbe", new Value.I64Value(1)).Value(field, value), "update exact integral")
        : new InsertMutationRequest(new InsertCommand("IntegralProbe").Value(field, value), "create exact integral");
    private sealed class ReplacingChecker(string field, Value value) : IEntityChecker
    {
        public int Calls { get; private set; }
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        {
            Calls++; var values = request is InsertMutationRequest insert ? insert.Command.Values : ((UpdateMutationRequest)request).Command.Values;
            values[field] = value; return [];
        }
    }
    private sealed class ProbeProvider : IDataService, ITransactionExecutor
    {
        public int Mutations { get; private set; }
        public int Begins { get; private set; }
        public Record? Values { get; private set; }
        public DataServiceCapabilities Capabilities => new() { Mutation = true, Transaction = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new Exception("unexpected query");
        public Task<MutationResult> MutateAsync(MutationRequest request)
        {
            Mutations++; Values = new Record(request is InsertMutationRequest insert ? insert.Command.Values : ((UpdateMutationRequest)request).Command.Values);
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
