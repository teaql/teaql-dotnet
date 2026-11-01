using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public sealed class ContextRootAdmissionTests
{
    internal static RuntimeModule Module(IEntityChecker? checker = null,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>? roots = null) => new(
        ["OwnedRecord"], checker == null ? new Dictionary<string, IEntityChecker>()
            : new Dictionary<string, IEntityChecker> { ["OwnedRecord"] = checker },
        new Dictionary<string, Record> { ["OwnedRecord"] = new() {
            ["id"] = new Value.I64Value(0), ["version"] = new Value.I64Value(0),
            ["managed_root"] = new Value.I64Value(0), ["name"] = new Value.TextValue("") } },
        new Dictionary<string, IReadOnlyDictionary<string, bool>>(),
        relations: new Dictionary<string, IReadOnlyList<RelationDescriptor>> { ["OwnedRecord"] = [
            RelationDescriptor.New("managedRoot", "DomainRoot").LocalKey("managed_root")] },
        contextRootFields: roots ?? new Dictionary<string, IReadOnlyDictionary<string, string>> {
            ["OwnedRecord"] = new Dictionary<string, string> { ["managed_root"] = "DomainRoot" } });

    internal static MutationRequest Update(Value? original = null) =>
        new UpdateMutationRequest(new UpdateCommand("OwnedRecord", new Value.I64Value(5))
            .ExpectedVersion(1).Value("name", "pending intent"), "update a loaded record")
        .WithLoadedSnapshot(new LoadedScalarSnapshot(new EntityKey("OwnedRecord", 5),
            new Record { ["managed_root"] = original ?? new Value.I64Value(17) }));

    [Fact]
    public void UnchangedOwnershipComesFromSnapshotNotJustTheMutationDelta()
    {
        var checker = new Checker(); var context = Module(checker).IntoContext().WithActiveRoot("DomainRoot", 17);
        var snapshot = new LoadedScalarSnapshot(new Record { ["managed_root"] = new Value.I64Value(17) });
        var request = Update().WithLoadedSnapshot(snapshot);
        context.CheckAndFix(request);
        Assert.Equal(1, checker.Calls);
        Assert.False(((UpdateMutationRequest)request).Command.Values.ContainsKey("managed_root"));
        Assert.Equal(new Value.I64Value(17), snapshot.CopyValues()["managed_root"]);
        context.WithActiveRoot("DomainRoot", 18);
        var error = Assert.Throws<CheckException>(() => context.CheckAndFix(request));
        Assert.Equal("context_root_mismatch", Assert.Single(error.Violations).RuleId);
        Assert.Equal(1, checker.Calls);
        Assert.Equal("pending intent", ((UpdateMutationRequest)request).Command.Values["name"].TryText());
        context.WithActiveRoot("DomainRoot", 17).CheckAndFix(request);
        Assert.Equal(2, checker.Calls);
    }

    [Theory]
    [InlineData("missing", "context_root_missing")]
    [InlineData("wrong-type", "context_root_missing")]
    [InlineData("invalid-id", "context_root_missing")]
    [InlineData("wrong-id", "context_root_mismatch")]
    public async Task InvalidContextFailsBeforeClockCheckerPolicyProviderAndAudit(string scenario, string rule)
    {
        var checker = new Checker(); var provider = new Provider(); var policy = new Policy(); var audit = new Audit();
        var context = Module(checker).IntoContext().WithDataService(provider).WithBusinessClock(new ThrowingClock())
            .WithMutationPolicyRegistry(policy).WithAppAuditEventSink(audit);
        if (scenario == "wrong-type") context.WithActiveRoot("OtherRoot", 17);
        if (scenario == "wrong-id") context.WithActiveRoot("DomainRoot", 18);
        if (scenario == "invalid-id") context.InsertNamedResource("teaql.activeRoot", new ContextEntityRef("DomainRoot", 0));
        var request = Update();
        var error = await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>().MutateAsync(request));
        Assert.Equal(rule, Assert.Single(error.Violations).RuleId);
        Assert.Equal(0, checker.Calls); Assert.Equal(0, policy.Calls); Assert.Equal(0, audit.Calls);
        Assert.Equal(0, provider.Calls);
        Assert.Equal("pending intent", ((UpdateMutationRequest)request).Command.Values["name"].TryText());
    }

    public static IEnumerable<object[]> InvalidReferences() => new Value[] {
        new Value.I64Value(0), new Value.I64Value(-17), new Value.I64Value(18),
        new Value.U64Value(ulong.MaxValue), new Value.TextValue("17"), new Value.F64Value(17),
        new Value.DecimalValue(17), new Value.BoolValue(true), new Value.NullValue(),
        new Value.TypedNullValue(DataType.I64)
    }.Select(value => new object[] { value });

    [Theory]
    [MemberData(nameof(InvalidReferences))]
    public void OriginalReferencesAreNotCoercedOrRepairedByCustomChecker(Value value)
    {
        var checker = new Checker { Change = (_, request) => ((UpdateMutationRequest)request).Command.Value("managed_root", 17L) };
        var context = Module(checker).IntoContext().WithActiveRoot("DomainRoot", 17);
        var error = Assert.Throws<CheckException>(() => context.CheckAndFix(Update(value)));
        Assert.Equal("context_root_mismatch", Assert.Single(error.Violations).RuleId);
        Assert.Equal(0, checker.Calls);
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("snapshot")]
    [InlineData("snapshot-omitted")]
    [InlineData("context")]
    public void FinalFixCannotRebindAnAlreadyAdmittedObject(string target)
    {
        var checker = new Checker { Change = (context, request) => {
            if (target == "pending") ((UpdateMutationRequest)request).Command.Value("managed_root", 18L);
            if (target == "snapshot") request.WithLoadedSnapshot(new LoadedScalarSnapshot(new Record { ["managed_root"] = new Value.I64Value(18) }));
            if (target == "snapshot-omitted") request.WithLoadedSnapshot(new LoadedScalarSnapshot(new Record()));
            if (target == "context") context.WithActiveRoot("DomainRoot", 18);
        } };
        var context = Module(checker).IntoContext().WithActiveRoot("DomainRoot", 17);
        Assert.Throws<CheckException>(() => context.CheckAndFix(Update()));
        Assert.Equal(1, checker.Calls);
    }

    [Fact]
    public void ExplicitPendingMismatchCannotBeHiddenByTheLoadedOriginal()
    {
        var request = Update(); ((UpdateMutationRequest)request).Command.Value("managed_root", 18L);
        var checker = new Checker(); var context = Module(checker).IntoContext().WithActiveRoot("DomainRoot", 17);
        Assert.Throws<CheckException>(() => context.CheckAndFix(request)); Assert.Equal(0, checker.Calls);
    }

    [Fact]
    public void MissingPersistedOwnershipFailsClosedButOldValuesCanSupplyIt()
    {
        var context = Module().IntoContext().WithActiveRoot("DomainRoot", 17);
        var command = new UpdateCommand("OwnedRecord", new Value.I64Value(5)).ExpectedVersion(1).Value("name", "pending");
        var request = new UpdateMutationRequest(command, "update original ownership");
        var error = Assert.Throws<CheckException>(() => context.CheckAndFix(request));
        Assert.Equal("context_root_missing", Assert.Single(error.Violations).RuleId);
        var withOriginal = command with { OldValues = new Record { ["managed_root"] = new Value.I64Value(17) } };
        context.CheckAndFix(new UpdateMutationRequest(withOriginal, "update using explicit original values"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CreateMayLeaveTheContextManagedDefaultForFix(bool explicitNull)
    {
        var checker = new Checker { Change = (_, request) => ((InsertMutationRequest)request).Command.Value("managed_root", 17L) };
        var context = Module(checker).IntoContext().WithActiveRoot("DomainRoot", 17);
        var command = new InsertCommand("OwnedRecord").Value("name", "new");
        if (explicitNull) command.Values["managed_root"] = new Value.NullValue();
        context.CheckAndFix(new InsertMutationRequest(command, "create with contextual ownership"));
        Assert.Equal(1, checker.Calls); Assert.Equal(new Value.I64Value(17), command.Values["managed_root"]);
    }

    [Fact]
    public void MetadataSuppliesTheContextDefaultWithoutRequiringAGeneratedChecker()
    {
        var context = Module().IntoContext().WithActiveRoot("DomainRoot", 17);
        var command = new InsertCommand("OwnedRecord").Value("name", "new");
        context.CheckAndFix(new InsertMutationRequest(command, "create with the modeled context default"));
        Assert.Equal(new Value.I64Value(17), command.Values["managed_root"]);
    }

    [Theory]
    [InlineData("missing")]
    [InlineData("null")]
    [InlineData("mismatch")]
    public void CustomFixCannotRemoveOrChangeTheManagedCreateDefault(string value)
    {
        var checker = new Checker { Change = (_, request) => {
            var fields = ((InsertMutationRequest)request).Command.Values;
            Assert.Equal(new Value.I64Value(17), fields["managed_root"]);
            if (value == "missing") fields.Remove("managed_root");
            else fields["managed_root"] = value == "null" ? new Value.NullValue() : new Value.I64Value(18);
        } };
        var context = Module(checker).IntoContext().WithActiveRoot("DomainRoot", 17);
        Assert.Throws<CheckException>(() => context.CheckAndFix(new InsertMutationRequest(new InsertCommand("OwnedRecord"), "reject changed default")));
        Assert.Equal(1, checker.Calls);
    }

    [Fact]
    public void ExplicitWrongCreateReferenceIsRejectedBeforeFix()
    {
        var checker = new Checker(); var context = Module(checker).IntoContext().WithActiveRoot("DomainRoot", 17);
        Assert.Throws<CheckException>(() => context.CheckAndFix(new InsertMutationRequest(
            new InsertCommand("OwnedRecord").Value("managed_root", 18L), "reject conflicting create ownership")));
        Assert.Equal(0, checker.Calls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DeleteAndRecoverAlsoValidateLoadedOwnership(bool recover)
    {
        MutationRequest request = recover
            ? new RecoverMutationRequest(new RecoverCommand { Entity = "OwnedRecord", Id = new Value.I64Value(5), ExpectedVersionValue = -1 }, "recover loaded object")
            : new DeleteMutationRequest(new DeleteCommand("OwnedRecord", new Value.I64Value(5)).ExpectedVersion(1), "delete loaded object");
        request.WithLoadedSnapshot(new LoadedScalarSnapshot(new Record { ["managed_root"] = new Value.I64Value(17) }));
        var context = Module().IntoContext().WithActiveRoot("DomainRoot", 18);
        Assert.Throws<CheckException>(() => context.CheckAndFix(request));
        context.WithActiveRoot("DomainRoot", 17).CheckAndFix(request);
    }

    private sealed class Checker : IEntityChecker
    {
        internal int Calls; internal Action<UserContext, MutationRequest>? Change;
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        { Calls++; Change?.Invoke(context, request); return []; }
    }
    private sealed class ThrowingClock : IBusinessClock
    { public DateTimeOffset Now => throw new InvalidOperationException("context rejection must precede clock access"); }
    private sealed class Provider : IDataService
    {
        internal int Calls;
        public DataServiceCapabilities Capabilities => new() { Query = true, Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) { Calls++; throw new InvalidOperationException("unexpected query"); }
        public Task<MutationResult> MutateAsync(MutationRequest request) { Calls++; throw new InvalidOperationException("unexpected mutation"); }
    }
    private sealed class Policy : IMutationPolicyRegistry
    { internal int Calls; public IMutationPolicy? Resolve(string key) { Calls++; throw new InvalidOperationException("unexpected policy"); } }
    private sealed class Audit : IAppAuditEventSink
    { internal int Calls; public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default)
        { Calls++; throw new InvalidOperationException("unexpected audit"); } }
}
