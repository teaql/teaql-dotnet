using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;

var warnings = new RecordingWarningSink();
var defaultContext = new UserContext().WithMutationGovernanceSink(warnings);
var defaultPlan = new MutationPlan(
    "example-default-1",
    "Order.saveGraph",
    "Order",
    "demonstrate generated default",
    new[]
    {
        new MutationOperation(
            MutationOperationKind.Update,
            "Order",
            new Value.I64Value(41),
            1,
            new Dictionary<string, Value> { ["state"] = new Value.TextValue("SUBMITTED") })
    });
defaultContext.ReviewMutationPlan(defaultPlan);
defaultContext.ReviewMutationPlan(defaultPlan with { ExecutionId = "example-default-2" });
Require(warnings.Events.Count == 2, "default policy warning was not delivered");
Require(warnings.Events[0].FirstOccurrence && !warnings.Events[1].FirstOccurrence,
    "default policy warning was not deduplicated");

var identity = new MutationPolicyIdentity(
    "order-submit",
    "1",
    "sha256:mutation-policy-example");
var policy = new OrderPolicy(identity);
var allowedProvider = new RecordingProvider();
var audit = new RecordingAuditSink();
var allowed = new UserContext
{
    UserIdentifier = "mutation-policy-example"
}
    .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(
        requestKey => requestKey == "Order.saveGraph" ? policy : null))
    .WithMutationPolicyApprovalProvider(new DelegatingMutationPolicyApprovalProvider(
        candidate => new MutationPolicyApproval(
            candidate,
            "security-owner",
            DateTimeOffset.UnixEpoch.AddSeconds(1))))
    .WithAppAuditEventSink(audit)
    .WithDataService(allowedProvider);

await allowed.ExecuteGraphSaveAsync(async () =>
{
    var order = Insert("Order", 42, "create approved order");
    var line = Insert("OrderLine", 99, "create approved order line");
    allowed.PreflightMutation(order);
    allowed.PreflightMutation(line);
    var service = allowed.RequireResource<IDataService>();
    await service.MutateAsync(order);
    return await service.MutateAsync(line);
});

Require(policy.LastOperationCount == 2, "policy did not receive the complete graph");
Require(allowedProvider.MutationCount == 2, "allowed graph did not persist both operations");
Require(allowedProvider.CommitCount == 1, "allowed graph did not commit atomically");
Require(audit.Events.Count == 2, "governance evidence was not attached to both audit events");
var governance = audit.Events[0]["mutationGovernance"] as IReadOnlyDictionary<string, object?>
    ?? throw new InvalidOperationException("mutation governance audit snapshot is missing");
Require(Equals(governance["policyId"], identity.Id), "policy identity is missing from audit evidence");
Require(Equals(governance["approvalStatus"], MutationPolicyApprovalStatus.Approved.ToString()),
    "exact approval status is missing from audit evidence");

var deniedProvider = new RecordingProvider();
var denied = new UserContext()
    .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(
        _ => new OrderPolicy(identity, deny: true)))
    .WithDataService(deniedProvider);
try
{
    await denied.ExecuteGraphSaveAsync(async () =>
    {
        var order = Insert("Order", 43, "create denied order");
        var line = Insert("OrderLine", 100, "create denied order line");
        denied.PreflightMutation(order);
        denied.PreflightMutation(line);
        return await denied.RequireResource<IDataService>().MutateAsync(order);
    });
    throw new InvalidOperationException("denied mutation unexpectedly completed");
}
catch (MutationPolicyException error) when (error.Message.Contains("ORDER_DENIED", StringComparison.Ordinal))
{
    // Expected: the transaction is rolled back before the first provider mutation.
}

Require(deniedProvider.MutationCount == 0, "denied graph reached the provider mutation boundary");
Require(deniedProvider.RollbackCount == 1, "denied graph did not roll back its transaction");

var missingPreflightProvider = new RecordingProvider();
var missingPreflight = new UserContext()
    .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(
        _ => new OrderPolicy(identity)))
    .WithDataService(missingPreflightProvider);
try
{
    await missingPreflight.ExecuteGraphSaveAsync(() =>
        missingPreflight.RequireResource<IDataService>().MutateAsync(
            Insert("Order", 44, "missing graph preflight")));
    throw new InvalidOperationException("customer policy graph without preflight unexpectedly completed");
}
catch (MutationPolicyException error) when (error.Message.Contains("complete graph preflight", StringComparison.Ordinal))
{
    // Expected: customer policy cannot review a partial graph.
}
Require(missingPreflightProvider.MutationCount == 0,
    "missing-preflight graph reached the provider mutation boundary");

var snapshotProvider = new RecordingProvider();
var snapshotPolicy = new StateCapturingPolicy(identity);
var snapshotContext = new UserContext()
    .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => snapshotPolicy))
    .WithDataService(snapshotProvider);
await snapshotContext.ExecuteGraphSaveAsync(async () =>
{
    var request = Insert("Order", 45, "prove immutable plan snapshot");
    request.Command.Value("state", new Value.TextValue("DRAFT"));
    snapshotContext.PreflightMutation(request);
    request.Command.Value("state", new Value.TextValue("APPROVED"));
    return await snapshotContext.RequireResource<IDataService>().MutateAsync(request);
});
Require(snapshotPolicy.ObservedState == "DRAFT", "policy plan changed after preflight");

Console.WriteLine(
    "DOTNET_MUTATION_POLICY_PASS allowed_operations={0} denied_provider_mutations={1} audit_events={2} warnings={3}",
    policy.LastOperationCount,
    deniedProvider.MutationCount,
    audit.Events.Count,
    warnings.Events.Count);

static InsertMutationRequest Insert(string entity, long id, string reason)
{
    var command = new InsertCommand(entity).Value("id", new Value.I64Value(id));
    command.TraceChain.Add(new TraceNode(entity, null, reason));
    return new InsertMutationRequest(command);
}

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class OrderPolicy(MutationPolicyIdentity identity, bool deny = false) : IMutationPolicy
{
    public MutationPolicyIdentity Identity { get; } = identity;
    public int LastOperationCount { get; private set; }

    public MutationDecision Review(UserContext context, MutationPlan plan)
    {
        LastOperationCount = plan.Operations.Count;
        return deny
            ? MutationDecision.Denied("ORDER_DENIED", "example denial")
            : MutationDecision.Allowed();
    }
}

sealed class StateCapturingPolicy(MutationPolicyIdentity identity) : IMutationPolicy
{
    public MutationPolicyIdentity Identity { get; } = identity;
    public string? ObservedState { get; private set; }

    public MutationDecision Review(UserContext context, MutationPlan plan)
    {
        ObservedState = plan.Operations[0].ChangedValues["state"] is Value.TextValue(var state)
            ? state
            : null;
        return MutationDecision.Allowed();
    }
}

sealed class RecordingWarningSink : IMutationGovernanceSink
{
    public List<MutationGovernanceEvent> Events { get; } = new();
    public void OnWarning(UserContext context, MutationGovernanceEvent warning) => Events.Add(warning);
}

sealed class RecordingAuditSink : IAppAuditEventSink
{
    public List<IReadOnlyDictionary<string, object?>> Events { get; } = new();

    public Task RecordAsync(
        IReadOnlyDictionary<string, object?> safeEvent,
        CancellationToken cancellationToken = default)
    {
        Events.Add(safeEvent);
        return Task.CompletedTask;
    }
}

sealed class RecordingProvider : ITransactionExecutor
{
    public int MutationCount { get; private set; }
    public int CommitCount { get; private set; }
    public int RollbackCount { get; private set; }
    public DataServiceCapabilities Capabilities { get; } = new()
    {
        Transaction = true,
        Mutation = true
    };

    public Task<QueryResult> QueryAsync(QueryRequest request) => Task.FromResult(new QueryResult());

    public Task<MutationResult> MutateAsync(MutationRequest request)
    {
        MutationCount++;
        return Task.FromResult(new MutationResult { AffectedRows = 1 });
    }

    public Task<ITransaction> BeginTransactionAsync() =>
        Task.FromResult<ITransaction>(new Transaction(this));

    private sealed class Transaction(RecordingProvider owner) : ITransaction
    {
        public DataServiceCapabilities Capabilities => owner.Capabilities;
        public Task<QueryResult> QueryAsync(QueryRequest request) => owner.QueryAsync(request);
        public Task<MutationResult> MutateAsync(MutationRequest request) => owner.MutateAsync(request);
        public Task CommitAsync() { owner.CommitCount++; return Task.CompletedTask; }
        public Task RollbackAsync() { owner.RollbackCount++; return Task.CompletedTask; }
        public void Dispose() { }
    }
}
