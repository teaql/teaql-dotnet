using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public class VersionLifecycleAdmissionTests
{
    public static IEnumerable<object[]> InvalidVersions()
    {
        foreach (var version in new[] { long.MinValue, -902, -1, 0, long.MaxValue })
            yield return [MutationKind.Update, version, version == long.MaxValue ? "version_exhausted" : "invalid_version_state"];
        foreach (var version in new[] { long.MinValue, -902, -1, 0 })
            yield return [MutationKind.Delete, version, "invalid_deletion_state"];
        foreach (var version in new[] { long.MinValue, long.MinValue + 1, 0, 1 })
            yield return [MutationKind.Recover, version, version < 0 ? "version_exhausted" : "invalid_version_state"];
    }

    [Theory]
    [MemberData(nameof(InvalidVersions))]
    public async Task Invalid_versions_fail_before_checker_policy_transaction_provider_and_audit(
        MutationKind operation, long version, string rule)
    {
        var harness = new Harness();
        var request = Request(operation, version);
        var error = await Assert.ThrowsAsync<CheckException>(() => harness.Service.MutateAsync(request));
        var violation = Assert.Single(error.Violations);
        Assert.Equal(rule, violation.RuleId); Assert.Equal("LifecycleProbe", violation.EntityType);
        Assert.Equal("version", violation.ModelPath); Assert.Equal("/version", violation.InstancePath);
        Assert.Equal(version, violation.InputValue); Assert.DoesNotContain("checker.", violation.Message!);
        Assert.Equal(0, harness.Fix.Calls); Assert.Equal(0, harness.Policy.Resolutions);
        Assert.Equal(0, harness.Policy.Reviews); harness.AssertNoIoOrAudit();
    }

    [Fact]
    public async Task Entire_nested_batch_is_admitted_before_the_first_leaf_fix()
    {
        var harness = new Harness();
        var request = new BatchMutationRequest(new List<MutationRequest> {
            Request(MutationKind.Insert, 0),
            new BatchMutationRequest(new List<MutationRequest> { Request(MutationKind.Update, long.MaxValue) }, "invalid nested version")
        }, "admit every sibling first");
        await Assert.ThrowsAsync<CheckException>(() => harness.Service.MutateAsync(request));
        Assert.Equal(0, harness.Fix.Calls); Assert.Equal(0, harness.Policy.Resolutions); harness.AssertNoIoOrAudit();
    }

    [Fact]
    public async Task Final_fix_output_is_readmitted_before_policy()
    {
        var harness = new Harness(); var request = Request(MutationKind.Update, 7);
        harness.Fix.Change = current => ((UpdateMutationRequest)current).Command.ExpectedVersionValue = long.MaxValue;
        var error = await Assert.ThrowsAsync<CheckException>(() => harness.Service.MutateAsync(request));
        Assert.Equal("version_exhausted", Assert.Single(error.Violations).RuleId);
        Assert.Equal(1, harness.Fix.Calls); Assert.Equal(0, harness.Policy.Resolutions); harness.AssertNoIoOrAudit();
        Assert.Equal("pending intent", ((UpdateMutationRequest)request).Command.Values["note"].TryText());
    }

    [Fact]
    public async Task Policy_callback_cannot_send_a_wrapped_successor_to_the_provider()
    {
        var harness = new Harness(); var request = Request(MutationKind.Update, 7);
        harness.Policy.Change = () => ((UpdateMutationRequest)request).Command.ExpectedVersionValue = long.MaxValue;
        var error = await Assert.ThrowsAsync<CheckException>(() => harness.Service.MutateAsync(request));
        Assert.Equal("version_exhausted", Assert.Single(error.Violations).RuleId);
        Assert.Equal(1, harness.Policy.Reviews); harness.AssertNoIoOrAudit();
    }

    [Fact]
    public async Task Prepared_graph_rejects_before_transaction_and_allows_a_fresh_retry()
    {
        var harness = new Harness(); var owner = new object();
        var command = new UpdateCommand("LifecycleProbe", new Value.I64Value(1)).ExpectedVersion(long.MaxValue).Value("note", "pending intent");
        var request = MutationRequest.Create(command, "update checked version", new EntityKey("LifecycleProbe", 1), new EntityRoot());
        await Assert.ThrowsAsync<CheckException>(() => harness.Context.ExecuteGraphSaveAsync("reject exhausted prepared update",
            graph => { graph.PrepareInput(owner, request); return Task.CompletedTask; }, _ => Task.FromResult(0)));
        harness.AssertNoIoOrAudit(); Assert.Equal(0, harness.Fix.Calls); Assert.Equal(0, harness.Policy.Resolutions);
        command.ExpectedVersionValue = 7;
        await harness.Context.ExecuteGraphSaveAsync("retry valid prepared update", graph => {
            graph.PrepareInput(owner, request); return Task.CompletedTask;
        }, async graph => { graph.PreflightPrepared(owner); await graph.MutatePreparedAsync(owner); return 1; });
        Assert.Equal(1, harness.Provider.Begins); Assert.Equal(1, harness.Provider.Mutations);
        Assert.Equal(1, harness.Policy.Reviews); Assert.Equal(1, harness.Audit.Calls);
    }

    [Theory]
    [InlineData(MutationKind.Update, long.MaxValue - 1)]
    [InlineData(MutationKind.Delete, long.MaxValue)]
    [InlineData(MutationKind.Recover, long.MinValue + 2)]
    [InlineData(MutationKind.Insert, 0L)]
    public async Task Legal_endpoints_and_create_version_zero_keep_their_roles(MutationKind operation, long version)
    {
        var harness = new Harness(); await harness.Service.MutateAsync(Request(operation, version));
        Assert.Equal(1, harness.Provider.Mutations); Assert.Equal(1, harness.Policy.Reviews); Assert.Equal(1, harness.Audit.Calls);
    }

    [Fact]
    public async Task Hard_delete_is_not_misclassified_as_soft_delete_or_recovery()
    {
        var harness = new Harness();
        var request = new DeleteMutationRequest(new DeleteCommand("LifecycleProbe", new Value.I64Value(1))
            .ExpectedVersion(long.MinValue).HardDelete(), "physically remove tombstone");
        await harness.Service.MutateAsync(request);
        Assert.Equal(1, harness.Provider.Mutations); Assert.Equal(1, harness.Policy.Reviews);
    }

    private static MutationRequest Request(MutationKind operation, long version) => operation switch
    {
        MutationKind.Insert => new InsertMutationRequest(new InsertCommand("LifecycleProbe").Value("version", version).Value("note", "pending intent"), "create probe"),
        MutationKind.Update => new UpdateMutationRequest(new UpdateCommand("LifecycleProbe", new Value.I64Value(1)).ExpectedVersion(version).Value("note", "pending intent"), "update probe"),
        MutationKind.Delete => new DeleteMutationRequest(new DeleteCommand("LifecycleProbe", new Value.I64Value(1)).ExpectedVersion(version), "soft delete probe"),
        MutationKind.Recover => new RecoverMutationRequest(new RecoverCommand { Entity = "LifecycleProbe", Id = new Value.I64Value(1), ExpectedVersionValue = version }, "recover probe"),
        _ => throw new ArgumentOutOfRangeException(nameof(operation))
    };

    private sealed class Harness
    {
        internal Provider Provider { get; } = new();
        internal Checker Fix { get; } = new();
        internal Policy Policy { get; } = new();
        internal Audit Audit { get; } = new();
        internal UserContext Context { get; }
        internal IDataService Service => Context.RequireResource<IDataService>();
        internal Harness() => Context = new RuntimeModule().Checker("LifecycleProbe", Fix).IntoContext()
            .WithDataService(Provider).WithMutationPolicyRegistry(Policy).WithAppAuditEventSink(Audit);
        internal void AssertNoIoOrAudit()
        {
            Assert.Equal(0, Provider.Queries); Assert.Equal(0, Provider.Mutations); Assert.Equal(0, Provider.Begins);
            Assert.Equal(0, Audit.Calls);
        }
    }
    private sealed class Checker : IEntityChecker
    {
        internal int Calls { get; private set; }
        internal Action<MutationRequest>? Change { get; set; }
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        { Calls++; Change?.Invoke(request); return []; }
    }
    private sealed class Policy : IMutationPolicy, IMutationPolicyRegistry
    {
        internal int Resolutions { get; private set; }
        internal int Reviews { get; private set; }
        internal Action? Change { get; set; }
        public MutationPolicyIdentity Identity { get; } = new("version-lifecycle", "1", "checked-domains");
        public IMutationPolicy? Resolve(string key) { Resolutions++; return this; }
        public MutationDecision Review(UserContext context, MutationPlan plan)
        { Reviews++; Change?.Invoke(); return MutationDecision.Allowed(); }
    }
    private sealed class Audit : IAppAuditEventSink
    {
        internal int Calls { get; private set; }
        public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default)
        { Calls++; return Task.CompletedTask; }
    }
    private sealed class Provider : ITransactionExecutor
    {
        internal int Queries { get; private set; }
        internal int Mutations { get; private set; }
        internal int Begins { get; private set; }
        public DataServiceCapabilities Capabilities => new() { Transaction = true, Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) { Queries++; return Task.FromResult(new QueryResult()); }
        public Task<MutationResult> MutateAsync(MutationRequest request)
        { Mutations++; return Task.FromResult(new MutationResult { AffectedRows = 1 }); }
        public Task<ITransaction> BeginTransactionAsync() { Begins++; return Task.FromResult<ITransaction>(new Transaction(this)); }
        private sealed class Transaction(Provider owner) : ITransaction
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
