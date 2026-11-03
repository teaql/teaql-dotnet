using System.Text.Json.Nodes;
using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class PreparedGraphMutationTests
{
    [Fact]
    public async Task AChildOnlySaveKeepsTheDeclaredRootPolicyEntryPoint()
    {
        var provider = new Provider(); var policy = new Policy();
        var context = new UserContext().WithDataService(provider).WithMutationPolicyRegistry(policy);
        var owner = new object();
        var request = MutationRequest.Create(new UpdateCommand("Child", new Value.I64Value(4))
            .ExpectedVersion(2).Value("amount", 7.25), "update child under parent", new EntityKey("Child", 4), new EntityRoot());
        await context.ExecuteGraphSaveAsync("save parent with only changed child", graph => {
            graph.DeclareRootEntity("Parent"); graph.PrepareInput(owner, request); return Task.CompletedTask;
        }, async graph => {graph.PreflightPrepared(owner); await graph.MutatePreparedAsync(owner); return 1;});
        var plan = Assert.Single(policy.Plans);
        Assert.Equal("Parent", plan.RootEntityType); Assert.Equal("Parent.saveGraph", plan.RequestKey);
        Assert.Equal("Child", Assert.Single(plan.Operations).Entity);
    }

    [Fact]
    public async Task TypeMismatchedLedgerKeysFailBeforeTransactionAllocation()
    {
        var provider = new Provider(); var context = new UserContext().WithDataService(provider);
        var request = MutationRequest.Create(new InsertCommand("Child").Value("amount", 1.0),
            "reject wrong typed ledger key", new EntityKey("Parent", -1), new EntityRoot());
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => context.ExecuteGraphSaveAsync(
            "reject wrong key before IO", graph => {graph.PrepareInput(new object(), request); return Task.CompletedTask;},
            _ => Task.FromResult(0)));
        Assert.Equal("GRAPH_LEDGER_ENTITY_TYPE_MISMATCH", error.Message); Assert.Equal(0, provider.Begins);
    }

    [Fact]
    public async Task FixDoesNotEraseTheOriginalPrivateValueRedactionProvenance()
    {
        var provider = new Provider(); var sink = new Audit();
        var context = new RuntimeModule().Entity(EntityDescriptor.New("Probe").AuditMaskFields(new List<string> {"name"}))
            .Checker("Probe", new PrivateFix()).IntoContext().WithDataService(provider).WithAppAuditEventSink(sink);
        var owner = new object(); var request = Insert("Probe", -1);
        request.Command.Values["name"] = new Value.TextValue("PRIVATE-BEFORE-FIX");
        await context.ExecuteGraphSaveAsync("save PRIVATE-BEFORE-FIX and PRIVATE-AFTER-FIX", graph => {
            graph.PrepareInput(owner, request); return Task.CompletedTask;
        }, async graph => {await graph.ReservePreparedIdAsync(owner); graph.PreflightPrepared(owner);
            await graph.MutatePreparedAsync(owner); return 1;});
        var reason = Assert.Single(sink.Events)["reason"]?.ToString() ?? "";
        Assert.DoesNotContain("PRIVATE-BEFORE-FIX", reason);
        Assert.DoesNotContain("PRIVATE-AFTER-FIX", reason); Assert.Contains("[REDACTED]", reason);
    }

    [Fact]
    public async Task PersistsTheFirstCheckedSnapshotWithoutAnotherFixOrMutableAliases()
    {
        var provider = new Provider(); var fix = new ChangingFix(); var policy = new Policy();
        var context = new UserContext().Install(new RuntimeModule().Checker("Probe", fix))
            .WithDataService(provider).WithMutationPolicyRegistry(policy);
        var owner = new object(); var request = Insert("Probe", -1);
        await context.ExecuteGraphSaveAsync("save checked snapshot", graph => {
            graph.PrepareInput(owner, request);
            Assert.Equal(0, provider.Begins);
            request.Command.Values["amount"] = new Value.F64Value(999);
            return Task.CompletedTask;
        }, async graph => {
            await graph.ReservePreparedIdAsync(owner);
            graph.PreflightPrepared(owner);
            fix.Retained!["amount"] = new Value.F64Value(999);
            await graph.MutatePreparedAsync(owner);
            Assert.Equal("GRAPH_ENTITY_ALREADY_EXECUTED", (await Assert.ThrowsAsync<InvalidOperationException>(() =>
                graph.MutatePreparedAsync(owner))).Message);
            return 1;
        });
        Assert.Equal(1, fix.Calls);
        Assert.Equal(7.25, Assert.IsType<InsertMutationRequest>(Assert.Single(provider.Requests)).Command.Values["amount"].TryF64());
        Assert.Equal(7.25, Assert.Single(Assert.Single(policy.Plans).Operations).ChangedValues["amount"].TryF64());
    }

    [Theory]
    [InlineData("revisit", "field")]
    [InlineData("seal", "field")]
    [InlineData("revisit", "delete")]
    [InlineData("seal", "delete")]
    public async Task ChangedPreparedLedgerIsRejectedBeforeOpeningATransaction(string checkpoint, string edit)
    {
        var provider = new Provider(); var context = new UserContext().WithDataService(provider);
        var owner = new object(); var request = Insert("Probe", -1);
        var error = await Assert.ThrowsAsync<CheckException>(() => context.ExecuteGraphSaveAsync(
            "reject a rebound or changed checked entity", graph => {
                Assert.True(graph.VisitEntity(owner, GraphMutationTraversal.Prepare));
                graph.PrepareInput(owner, request);
                if (edit == "delete") request.LedgerRoot!.MarkAsDeleted(request.LedgerKey!);
                else request.LedgerRoot!.Set(request.LedgerKey!, "parent_id", new Value.I64Value(-2));
                if (checkpoint == "revisit") graph.VisitEntity(owner, GraphMutationTraversal.Prepare);
                else graph.SealPreparedGraph();
                return Task.CompletedTask;
            }, _ => Task.FromResult(0)));
        var violation = Assert.Single(error.Violations);
        Assert.Equal("mutation_changed_after_preparation", violation.RuleId);
        Assert.Equal("Probe", violation.EntityType);
        Assert.Equal(edit == "field" ? "parent_id" : "", violation.Location.ModelPath);
        Assert.Equal(0, provider.Begins);
        Assert.Empty(provider.Requests);
    }

    [Theory]
    [InlineData("review", false)]
    [InlineData("review", true)]
    [InlineData("provider", false)]
    [InlineData("provider", true)]
    public async Task LatePolicyOrProviderEditsCannotDisappearIntoACheckedCommit(string stage, bool delete)
    {
        var provider = new Provider(); var policy = new Policy();
        var context = new UserContext().WithDataService(provider).WithMutationPolicyRegistry(policy);
        var owner = new object(); var request = Insert("Probe", -1);
        request.LedgerRoot!.Set(request.LedgerKey!, "amount", new Value.F64Value(1.0));
        request.LedgerRoot.MarkAsNew(request.LedgerKey!);
        void Edit()
        {
            var assignedKey = new EntityKey("Probe", 1);
            if (delete) request.LedgerRoot.MarkAsDeleted(assignedKey);
            else request.LedgerRoot.Set(assignedKey, "amount", new Value.F64Value(999));
        }
        if (stage == "review") policy.OnReview = Edit;
        else provider.OnMutation = Edit;
        var error = await Assert.ThrowsAsync<CheckException>(() => context.ExecuteGraphSaveAsync(
            "reject late edits to a checked graph", graph => {
                graph.PrepareInput(owner, request); graph.SealPreparedGraph(); return Task.CompletedTask;
            }, async graph => {
                var id = await graph.ReservePreparedIdAsync(owner);
                request.LedgerRoot.Rekey(request.LedgerKey!, new EntityKey("Probe", checked((long)id)));
                graph.PreflightPrepared(owner); await graph.MutatePreparedAsync(owner); return 1;
            }));
        Assert.Equal("mutation_changed_after_preparation", Assert.Single(error.Violations).RuleId);
        Assert.Equal(1, provider.Begins); Assert.Equal(1, provider.Rollbacks); Assert.Equal(0, provider.Commits);
        Assert.Equal(stage == "review" ? 0 : 1, provider.Requests.Count);
        var pending = new EntityKey("Probe", 1);
        if (delete) Assert.True(request.LedgerRoot.IsDeleted(pending));
        else Assert.Equal(999, request.LedgerRoot.Change(pending)["amount"].TryF64());
    }

    [Fact]
    public async Task ResolvesTypedTemporaryIdentitiesAndOnlyModeledReferenceFields()
    {
        var provider = new Provider(); var policy = new Policy();
        var module = new RuntimeModule().Entity(EntityDescriptor.New("Parent"))
            .Entity(EntityDescriptor.New("Child").Relation(RelationDescriptor.New("Parent", "Parent")
                .LocalKey("parent_id").ForeignKey("id")));
        var context = module.IntoContext().WithDataService(provider).WithMutationPolicyRegistry(policy);
        var parent = new object(); var child = new object();
        var rootRequest = Insert("Parent", -1);
        rootRequest.Command.Values["quantity"] = new Value.I64Value(-1);
        var childRequest = Insert("Child", -1); childRequest.Command.Values["parent_id"] = new Value.I64Value(-1);
        rootRequest.LedgerRoot!.Set(rootRequest.LedgerKey!, "quantity", new Value.I64Value(-1));
        childRequest.LedgerRoot!.Set(childRequest.LedgerKey!, "parent_id", new Value.I64Value(-1));
        await context.ExecuteGraphSaveAsync("save typed equal numeric identities", graph => {
            graph.PrepareInput(parent, rootRequest); graph.PrepareInput(child, childRequest);
            return Task.CompletedTask;
        }, async graph => {
            Assert.Equal(1UL, await graph.ReservePreparedIdAsync(parent));
            Assert.Equal(1UL, await graph.ReservePreparedIdAsync(child));
            rootRequest.LedgerRoot.Rekey(rootRequest.LedgerKey!, new EntityKey("Parent", 1));
            childRequest.LedgerRoot.Rekey(childRequest.LedgerKey!, new EntityKey("Child", 1));
            childRequest.LedgerRoot.Set(new EntityKey("Child", 1), "parent_id", new Value.I64Value(1));
            graph.PreflightPrepared(parent);
            Assert.Equal("GRAPH_PREFLIGHT_INCOMPLETE", (await Assert.ThrowsAsync<InvalidOperationException>(() =>
                graph.MutatePreparedAsync(parent))).Message);
            graph.PreflightPrepared(child);
            await graph.MutatePreparedAsync(parent); await graph.MutatePreparedAsync(child);
            return 1;
        });
        var root = Assert.IsType<InsertMutationRequest>(provider.Requests[0]);
        var nested = Assert.IsType<InsertMutationRequest>(provider.Requests[1]);
        Assert.Equal(1, root.Command.Values["id"].TryI64());
        Assert.Equal(-1, root.Command.Values["quantity"].TryI64());
        Assert.Equal(1, nested.Command.Values["id"].TryI64());
        Assert.Equal(1, nested.Command.Values["parent_id"].TryI64());
        Assert.All(Assert.Single(policy.Plans).Operations, op => Assert.Equal(1, op.Id!.TryI64()));
    }

    [Fact]
    public async Task ClosedOrForeignOwnersCannotReusePreparedCommandsAndRetryIsFresh()
    {
        var provider = new Provider(); var context = new UserContext().WithDataService(provider);
        var owner = new object(); GraphMutationSession? expired = null;
        for (var round = 0; round < 2; round++)
            await context.ExecuteGraphSaveAsync("save an independent invocation", graph => {
                expired = graph; graph.PrepareInput(owner, Insert("Probe", -1));
                return Task.CompletedTask;
            }, async graph => {
                Assert.Equal("GRAPH_ENTITY_NOT_PREPARED", (await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    graph.ReservePreparedIdAsync(new object()))).Message);
                await graph.ReservePreparedIdAsync(owner); graph.PreflightPrepared(owner);
                await graph.MutatePreparedAsync(owner); return 1;
            });
        Assert.Equal(2, provider.Begins); Assert.Equal(2, provider.Requests.Count);
        Assert.Equal("GRAPH_CAPABILITY_EXPIRED", (await Assert.ThrowsAsync<InvalidOperationException>(() =>
            expired!.MutatePreparedAsync(owner))).Message);
    }

    [Fact]
    public async Task DeepSnapshotsPreserveUpdateGuardsOldValuesAndOriginalVersion()
    {
        var provider = new Provider(); var context = new UserContext().WithDataService(provider);
        var owner = new object(); var json = JsonNode.Parse("{\"code\":\"original\"}")!;
        var command = new UpdateCommand("Probe", new Value.I64Value(4)) { ExpectedVersionValue = 7,
            OldValues = new Record { ["payload"] = new Value.JsonValue(json) } };
        command.Value("payload", new Value.JsonValue(json)).Guard("payload", new Value.JsonValue(json));
        var request = MutationRequest.Create(command, "update a frozen command", new EntityKey("Probe", 4), new EntityRoot());
        await context.ExecuteGraphSaveAsync("save deep update snapshot", graph => {
            graph.PrepareInput(owner, request); json["code"] = "changed"; command.ExpectedVersionValue = 999;
            return Task.CompletedTask;
        }, async graph => { graph.PreflightPrepared(owner); await graph.MutatePreparedAsync(owner); return 1; });
        var actual = Assert.IsType<UpdateMutationRequest>(Assert.Single(provider.Requests)).Command;
        Assert.Equal(7, actual.ExpectedVersionValue);
        foreach (var value in new[] {actual.Values["payload"], actual.Guards["payload"], actual.OldValues!["payload"]})
            Assert.Equal("original", Assert.IsType<Value.JsonValue>(value).Value!["code"]!.GetValue<string>());
    }

    private static InsertMutationRequest Insert(string type, long temporaryId) =>
        (InsertMutationRequest)MutationRequest.Create(new InsertCommand(type).Value("amount", 1.0),
            "prepare one checked object", new EntityKey(type, temporaryId), new EntityRoot());
    private sealed class ChangingFix : IEntityChecker
    {
        public int Calls {get; private set;}
        public Record? Retained {get; private set;}
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        { Retained = ((InsertMutationRequest)request).Command.Values; Retained["amount"] = new Value.F64Value(7 + ++Calls * 0.25); return []; }
    }
    private sealed class PrivateFix : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        {((InsertMutationRequest)request).Command.Values["name"] = new Value.TextValue("PRIVATE-AFTER-FIX"); return [];}
    }
    private sealed class Audit : IAppAuditEventSink
    {
        public List<IReadOnlyDictionary<string, object?>> Events {get;} = [];
        public Task RecordAsync(IReadOnlyDictionary<string, object?> data, CancellationToken token = default)
        {Events.Add(data); return Task.CompletedTask;}
    }
    private sealed class Policy : IMutationPolicy, IMutationPolicyRegistry
    {
        public MutationPolicyIdentity Identity {get;} = new("fixture", "1", "frozen-v1");
        public List<MutationPlan> Plans {get;} = [];
        public Action? OnReview;
        public IMutationPolicy? Resolve(string requestKey) => this;
        public MutationDecision Review(UserContext context, MutationPlan plan) {Plans.Add(plan); OnReview?.Invoke(); return MutationDecision.Allowed();}
    }
    private sealed class Provider : IDataService, ITransactionExecutor
    {
        public int Begins {get; private set;}
        public int Commits {get; private set;}
        public int Rollbacks {get; private set;}
        public Action? OnMutation;
        public List<MutationRequest> Requests {get;} = [];
        private readonly Dictionary<string, ulong> _ids = new();
        public DataServiceCapabilities Capabilities => new() {Mutation = true, Transaction = true};
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new Exception("unexpected query");
        public Task<MutationResult> MutateAsync(MutationRequest request) {Requests.Add(request); OnMutation?.Invoke(); return Task.FromResult(new MutationResult {AffectedRows = 1});}
        public Task<ITransaction> BeginTransactionAsync() {Begins++; return Task.FromResult<ITransaction>(new Transaction(this));}
        private sealed class Transaction(Provider owner) : ITransaction, IInternalIdAllocator
        {
            public DataServiceCapabilities Capabilities => owner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) => owner.QueryAsync(request);
            public Task<MutationResult> MutateAsync(MutationRequest request) => owner.MutateAsync(request);
            public Task<ulong> NextIdAsync(string entity) {owner._ids.TryGetValue(entity, out var id); owner._ids[entity] = ++id; return Task.FromResult(id);}
            public Task EnsureIdFloorAsync(string entity, ulong floor) => Task.CompletedTask;
            public Task CommitAsync() {owner.Commits++; return Task.CompletedTask;}
            public Task RollbackAsync() {owner.Rollbacks++; return Task.CompletedTask;}
            public void Dispose() { }
        }
    }
}
