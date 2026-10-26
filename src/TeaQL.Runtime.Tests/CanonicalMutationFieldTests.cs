using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class CanonicalMutationFieldTests
{
    private static Record[] BadFields() => [
        new() { ["type_platform_id"] = new Value.I64Value(1) },
        new() { ["typePlatform"] = new Value.I64Value(1) },
        new() { ["requiredString"] = new Value.TextValue("PRIVATE_BAD_ALIAS") },
        new() { ["unknown_field"] = new Value.TextValue("PRIVATE_BAD_UNKNOWN") },
        new() { ["type_platform"] = new Value.I64Value(1), ["type_platform_id"] = new Value.I64Value(1) },
        new() { ["type_platform"] = new Value.I64Value(1), ["type_platform_id"] = new Value.I64Value(2) },
    ];
    public static IEnumerable<object[]> Cases()
    {
        for (var index = 0; index < BadFields().Length; index++)
            foreach (var update in new[] { false, true }) yield return [index, update];
    }
    public static IEnumerable<object[]> PolicyCases() => Enumerable.Range(0, BadFields().Length).Select(i => new object[] { i });

    [Theory, MemberData(nameof(Cases))]
    public async Task CanonicalFieldsRejectBeforeCheckerAndProvider(int index, bool update)
    {
        var provider = new Provider(); var checker = new Checker(); var context = Context(provider, checker);
        var request = Request(BadFields()[index], update);
        var error = await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>().MutateAsync(request));
        Assert.Equal(0, checker.Calls); Assert.Equal(0, provider.Mutations); Assert.Equal(0, provider.Begins);
        Assert.All(error.Violations, item => {
            Assert.Equal("unknown_field", item.RuleId); Assert.Equal("NameProbe", item.EntityType); Assert.Null(item.InputValue);
        });
        Assert.DoesNotContain("PRIVATE_BAD", error.Message);
        Assert.Equal(BadFields()[index].Keys.Order(), Values(request).Keys.Order());
    }

    [Theory, MemberData(nameof(Cases))]
    public async Task CanonicalFieldsInspectRawLedgerEvenWhenCreatePayloadAlreadyDroppedTheKey(int index, bool update)
    {
        var provider = new Provider(); var checker = new Checker(); var context = Context(provider, checker);
        var ledger = new EntityRoot(); var key = new EntityKey("NameProbe", 1);
        foreach (var pair in BadFields()[index]) ledger.Set(key, pair.Key, pair.Value);
        var values = new Record { ["required_string"] = new Value.TextValue("valid") };
        MutationRequest request = update
            ? MutationRequest.Create(new UpdateCommand("NameProbe", new Value.I64Value(1)) { Values = values }, "inspect raw ledger", key, ledger)
            : MutationRequest.Create(new InsertCommand("NameProbe") { Values = values }, "inspect raw ledger", key, ledger);
        await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>().MutateAsync(request));
        Assert.Equal(0, checker.Calls); Assert.Equal(0, provider.Mutations); Assert.Equal(0, provider.Begins);
    }

    [Theory]
    [InlineData("type_platform_id", false), InlineData("type_platform_id", true)]
    [InlineData("requiredString", false), InlineData("requiredString", true)]
    public async Task CanonicalFieldsRejectBadFixOutput(string field, bool update)
    {
        var provider = new Provider(); var checker = new Checker(field); var context = Context(provider, checker);
        var error = await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>()
            .MutateAsync(Request(new Record { ["required_string"] = new Value.TextValue("valid") }, update)));
        Assert.Equal(1, checker.Calls); Assert.Equal(0, provider.Mutations); Assert.Equal(0, provider.Begins);
        Assert.Contains(error.Violations, item => item.RuleId == "unknown_field" && item.ModelPath == field);
        Assert.DoesNotContain("PRIVATE_BAD", error.Message);
    }

    [Theory, MemberData(nameof(PolicyCases))]
    public void CanonicalFieldsRejectBeforeCustomerPolicy(int index)
    {
        var policy = new Policy(); var context = Context(new Provider()).WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => policy));
        Assert.Throws<CheckException>(() => context.ReviewMutationPlan(Plan(BadFields()[index])));
        Assert.Equal(0, policy.Calls);
    }

    [Fact]
    public async Task CanonicalFieldsRejectedPreparationReleasesOwnershipForRetry()
    {
        var provider = new Provider(); var context = Context(provider);
        await Assert.ThrowsAsync<CheckException>(() => context.ExecuteGraphSaveAsync<int>("reject malformed namespace",
            graph => { graph.Preflight(Request(BadFields()[0])); return Task.CompletedTask; },
            _ => throw new Exception("malformed input entered graph work")));
        Assert.Equal(0, provider.Begins);
        Assert.Equal(7, await context.ExecuteGraphSaveAsync("retry valid graph", _ => Task.FromResult(7)).WaitAsync(TimeSpan.FromSeconds(2)));
        Assert.Equal(1, provider.Begins);
    }

    [Fact]
    public async Task CanonicalFieldsAllowCanonicalSparseValuesUnchanged()
    {
        var provider = new Provider(); var checker = new Checker(); var context = Context(provider, checker);
        var values = new Record { ["required_string"] = new Value.TextValue("valid"), ["type_platform"] = new Value.I64Value(1) };
        await context.RequireResource<IDataService>().MutateAsync(Request(values, true));
        Assert.Equal(1, checker.Calls); Assert.Equal(1, provider.Mutations);
        Assert.Equal(values.Keys.Order(), provider.Values!.Keys.Order());
        await context.RequireResource<IDataService>().MutateAsync(Request(new Record(), true));
    }

    [Fact]
    public async Task CanonicalFieldsPreserveMetadataLessLegacyEntities()
    {
        var provider = new Provider(); var context = new UserContext().WithDataService(provider);
        await context.RequireResource<IDataService>().MutateAsync(Request(BadFields()[0]));
        context.ReviewMutationPlan(Plan(BadFields()[0])); Assert.Equal(1, provider.Mutations);
        var empty = new RuntimeModule().Entity(EntityDescriptor.New("NameProbe")).IntoContext().WithDataService(provider);
        await empty.RequireResource<IDataService>().MutateAsync(Request(BadFields()[0]));
    }

    [Fact]
    public async Task CanonicalFieldsDoNotBanAnEntitySpecificIdSuffix()
    {
        var provider = new Provider(); var context = new RuntimeModule().Entity(EntityDescriptor.New("NameProbe")
            .Property(PropertyDescriptor.New("type_platform_id", DataType.I64))).IntoContext().WithDataService(provider);
        await context.RequireResource<IDataService>().MutateAsync(Request(BadFields()[0])); Assert.Equal(1, provider.Mutations);
    }

    private static UserContext Context(Provider provider, Checker? checker = null)
    {
        var module = new RuntimeModule().Entity(EntityDescriptor.New("NameProbe")
            .Property(PropertyDescriptor.New("required_string", DataType.Text))
            .Property(PropertyDescriptor.New("type_platform", DataType.I64))
            .Relation(RelationDescriptor.New("type_platform", "Platform").LocalKey("type_platform").ForeignKey("id")));
        if (checker != null) module.Checker("NameProbe", checker);
        return module.IntoContext().WithDataService(provider);
    }
    private static MutationRequest Request(Record values, bool update = false) => update
        ? new UpdateMutationRequest(new UpdateCommand("NameProbe", new Value.I64Value(1)) { Values = values }, "inspect canonical mutation fields")
        : new InsertMutationRequest(new InsertCommand("NameProbe") { Values = values }, "inspect canonical mutation fields");
    private static MutationPlan Plan(Record values) => new("canonical-fields", "NameProbe.saveGraph", "NameProbe", "inspect canonical plan",
        [new(MutationOperationKind.Update, "NameProbe", new Value.I64Value(1), 1, values)]);
    private static Record Values(MutationRequest request) => request is InsertMutationRequest insert ? insert.Command.Values : ((UpdateMutationRequest)request).Command.Values;
    private sealed class Checker(string? field = null) : IEntityChecker
    {
        public int Calls { get; private set; }
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        {
            Calls++; if (field != null) Values(request)[field] = new Value.TextValue("PRIVATE_BAD_FIX");
            return [];
        }
    }
    private sealed class Policy : IMutationPolicy
    {
        public int Calls { get; private set; }
        public MutationPolicyIdentity Identity { get; } = new("canonical-fields", "1", "sha256:canonical-fields");
        public MutationDecision Review(UserContext context, MutationPlan plan) { Calls++; return MutationDecision.Allowed(); }
    }
    private sealed class Provider : IDataService, ITransactionExecutor
    {
        public int Mutations { get; private set; }
        public int Begins { get; private set; }
        public Record? Values { get; private set; }
        public DataServiceCapabilities Capabilities => new() { Mutation = true, Transaction = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new Exception("unexpected query");
        public Task<MutationResult> MutateAsync(MutationRequest request)
        {
            Mutations++; Values = new Record(CanonicalMutationFieldTests.Values(request));
            return Task.FromResult(new MutationResult { AffectedRows = 1 });
        }
        public Task<ITransaction> BeginTransactionAsync() { Begins++; return Task.FromResult<ITransaction>(new Transaction(this)); }
        private sealed class Transaction(Provider provider) : ITransaction
        {
            public DataServiceCapabilities Capabilities => provider.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) => provider.QueryAsync(request);
            public Task<MutationResult> MutateAsync(MutationRequest request) => provider.MutateAsync(request);
            public Task CommitAsync() => Task.CompletedTask; public Task RollbackAsync() => Task.CompletedTask; public void Dispose() { }
        }
    }
}
