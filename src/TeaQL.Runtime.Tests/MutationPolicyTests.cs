using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public class MutationPolicyTests
{
    [Fact]
    public void GeneratedDefaultAllowsAndDeduplicatesWarningDelivery()
    {
        var warnings = new RecordingWarningSink();
        var context = new UserContext().WithMutationGovernanceSink(warnings);

        var first = context.ReviewMutationPlan(Plan());
        var second = context.ReviewMutationPlan(Plan());

        Assert.Equal(MutationPolicySource.GeneratedDefault, first.Source);
        Assert.Equal(MutationPolicyApprovalStatus.NotApplicable, first.ApprovalStatus);
        Assert.Equal(new[] { MutationPolicyWarnings.MissingPolicy }, second.WarningCodes);
        Assert.Equal(2, warnings.Events.Count);
        Assert.True(warnings.Events[0].FirstOccurrence);
        Assert.False(warnings.Events[1].FirstOccurrence);
    }

    [Fact]
    public void CustomerPolicyRequiresExactIdentityApprovalAndReceivesWholeGraph()
    {
        var identity = new MutationPolicyIdentity("order-submit", "3", "sha256:approved");
        var policy = new TestPolicy(identity, plan =>
        {
            Assert.Equal(2, plan.Operations.Count);
            return MutationDecision.Allowed();
        });
        var context = new UserContext().WithMutationPolicyRegistry(
            new DelegatingMutationPolicyRegistry(key => key == "Order.saveGraph" ? policy : null));

        var missing = context.ReviewMutationPlan(Plan());
        Assert.Equal(MutationPolicyApprovalStatus.Missing, missing.ApprovalStatus);
        Assert.Equal(new[] { MutationPolicyWarnings.MissingApproval }, missing.WarningCodes);

        context.WithMutationPolicyApprovalProvider(new DelegatingMutationPolicyApprovalProvider(candidate =>
            new MutationPolicyApproval(candidate, "security-owner", DateTimeOffset.UnixEpoch.AddSeconds(1))));
        var approved = context.ReviewMutationPlan(Plan());
        Assert.Equal(MutationPolicyApprovalStatus.Approved, approved.ApprovalStatus);
        Assert.Empty(approved.WarningCodes);
    }

    [Fact]
    public async Task DenialUsesCompletePreflightAndPrecedesProviderMutation()
    {
        var provider = new RecordingProvider();
        var observedOperations = 0;
        var identity = new MutationPolicyIdentity("deny-orders", "1", "sha256:deny");
        var policy = new TestPolicy(identity, plan =>
        {
            observedOperations = plan.Operations.Count;
            return MutationDecision.Denied("ORDER_DENIED", "order writes disabled", "Order.state");
        });
        var context = new UserContext()
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => policy))
            .WithDataService(provider);

        var error = await Assert.ThrowsAsync<MutationPolicyException>(() =>
            context.ExecuteGraphSaveAsync(async () =>
            {
                context.PreflightMutation(OrderInsert());
                context.PreflightMutation(LineInsert());
                return await context.RequireResource<IDataService>().MutateAsync(OrderInsert());
            }));

        Assert.Contains("ORDER_DENIED", error.Message);
        Assert.Equal(2, observedOperations);
        Assert.Equal(1, provider.BeginCount);
        Assert.Equal(0, provider.MutationCount);
        Assert.Equal(1, provider.RollbackCount);
    }

    [Fact]
    public async Task AllowedGraphAttachesGovernanceAndWarningSinkFailureIsFailOpen()
    {
        var provider = new RecordingProvider();
        var audit = new RecordingAuditSink();
        var warning = new RecordingWarningSink { Failure = new InvalidOperationException("sink unavailable") };
        var context = new UserContext()
            .WithMutationGovernanceSink(warning)
            .WithAppAuditEventSink(audit)
            .WithDataService(provider);

        await context.ExecuteGraphSaveAsync(async () =>
        {
            var request = OrderInsert();
            context.PreflightMutation(request);
            return await context.RequireResource<IDataService>().MutateAsync(request);
        });

        Assert.Equal(1, provider.MutationCount);
        Assert.Single(audit.Events);
        var governance = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(
            audit.Events[0]["mutationGovernance"]);
        Assert.Equal("Order.saveGraph", governance["requestKey"]);
        Assert.Equal(1, governance["operationCount"]);
        Assert.Null(context.CurrentMutationGovernance);
    }

    [Fact]
    public async Task CustomerPolicyFailsClosedWhenGraphSkipsPreflight()
    {
        var provider = new RecordingProvider();
        var policy = new TestPolicy(
            new MutationPolicyIdentity("orders", "1", "sha256:orders"),
            _ => MutationDecision.Allowed());
        var context = new UserContext()
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => policy))
            .WithDataService(provider);

        var error = await Assert.ThrowsAsync<MutationPolicyException>(() =>
            context.ExecuteGraphSaveAsync(() =>
                context.RequireResource<IDataService>().MutateAsync(OrderInsert())));

        Assert.Contains("complete graph preflight", error.Message);
        Assert.Equal(0, provider.MutationCount);
    }

    [Fact]
    public async Task PreflightPlanIsAnImmutableSnapshot()
    {
        var provider = new RecordingProvider();
        string? observedState = null;
        var policy = new TestPolicy(
            new MutationPolicyIdentity("orders", "1", "sha256:orders"),
            plan =>
            {
                observedState = plan.Operations[0].ChangedValues["state"] is Value.TextValue(var state)
                    ? state
                    : null;
                return MutationDecision.Allowed();
            });
        var context = new UserContext()
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => policy))
            .WithDataService(provider);

        await context.ExecuteGraphSaveAsync(async () =>
        {
            var request = OrderInsert();
            request.Command.Value("state", new Value.TextValue("DRAFT"));
            context.PreflightMutation(request);
            request.Command.Value("state", new Value.TextValue("APPROVED"));
            return await context.RequireResource<IDataService>().MutateAsync(request);
        });

        Assert.Equal("DRAFT", observedState);
    }

    [Fact]
    public async Task BatchMutationIsFlattenedForPolicyReview()
    {
        var provider = new RecordingProvider();
        var observedOperations = 0;
        var policy = new TestPolicy(
            new MutationPolicyIdentity("orders", "1", "sha256:orders"),
            plan =>
            {
                observedOperations = plan.Operations.Count;
                return MutationDecision.Allowed();
            });
        var context = new UserContext()
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => policy))
            .WithDataService(provider);

        await context.RequireResource<IDataService>().MutateAsync(new BatchMutationRequest(
            new List<MutationRequest> { OrderInsert(), LineInsert() }));

        Assert.Equal(2, observedOperations);
        Assert.Equal(1, provider.MutationCount);
    }

    private static MutationPlan Plan()
    {
        var values = new Dictionary<string, Value> { ["state"] = new Value.TextValue("SUBMITTED") };
        var lineValues = new Dictionary<string, Value> { ["quantity"] = new Value.I64Value(2) };
        return new MutationPlan(
            "test-mutation-1",
            "Order.saveGraph",
            "Order",
            "submit order",
            new[]
            {
                new MutationOperation(MutationOperationKind.Update, "Order", new Value.I64Value(42), 7, values),
                new MutationOperation(MutationOperationKind.Create, "OrderLine", new Value.I64Value(99), null, lineValues)
            });
    }

    private static InsertMutationRequest OrderInsert()
    {
        var command = new InsertCommand("Order").Value("id", new Value.I64Value(42));
        command.TraceChain.Add(new TraceNode("Order", null, "create order"));
        return new InsertMutationRequest(command);
    }

    private static InsertMutationRequest LineInsert()
    {
        var command = new InsertCommand("OrderLine").Value("id", new Value.I64Value(99));
        command.TraceChain.Add(new TraceNode("OrderLine", null, "create line"));
        return new InsertMutationRequest(command);
    }

    private sealed class TestPolicy(
        MutationPolicyIdentity identity,
        Func<MutationPlan, MutationDecision> review) : IMutationPolicy
    {
        public MutationPolicyIdentity Identity { get; } = identity;
        public MutationDecision Review(UserContext context, MutationPlan plan) => review(plan);
    }

    private sealed class RecordingWarningSink : IMutationGovernanceSink
    {
        public List<MutationGovernanceEvent> Events { get; } = new();
        public Exception? Failure { get; init; }
        public void OnWarning(UserContext context, MutationGovernanceEvent warning)
        {
            Events.Add(warning);
            if (Failure != null) throw Failure;
        }
    }

    private sealed class RecordingAuditSink : IAppAuditEventSink
    {
        public List<IReadOnlyDictionary<string, object?>> Events { get; } = new();
        public Task RecordAsync(IReadOnlyDictionary<string, object?> safeEvent,
            CancellationToken cancellationToken = default)
        {
            Events.Add(safeEvent);
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingProvider : ITransactionExecutor
    {
        public int BeginCount { get; private set; }
        public int MutationCount { get; private set; }
        public int RollbackCount { get; private set; }
        public DataServiceCapabilities Capabilities { get; } = new() { Transaction = true, Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => Task.FromResult(new QueryResult());
        public Task<MutationResult> MutateAsync(MutationRequest request)
        {
            MutationCount++;
            return Task.FromResult(new MutationResult { AffectedRows = 1 });
        }
        public Task<ITransaction> BeginTransactionAsync()
        {
            BeginCount++;
            return Task.FromResult<ITransaction>(new Transaction(this));
        }

        private sealed class Transaction(RecordingProvider owner) : ITransaction
        {
            public DataServiceCapabilities Capabilities => owner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) => owner.QueryAsync(request);
            public Task<MutationResult> MutateAsync(MutationRequest request) => owner.MutateAsync(request);
            public Task CommitAsync() => Task.CompletedTask;
            public Task RollbackAsync() { owner.RollbackCount++; return Task.CompletedTask; }
            public void Dispose() { }
        }
    }
}
