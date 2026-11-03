using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

// Application-owned generated Q/E/audited-save proof. No generated source is
// read or edited. The registry reflection seam only decorates the real Checkers.
static class GraphFixLifecycleProbe
{
    static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static async Task Verify(string filename)
    {
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
        await connection.OpenAsync();
        var transport = new Transport(new SqliteTransport(connection)); var visits = new List<Visit>();
        var module = GeneratedRuntimeModule.Module;
        var property = typeof(RuntimeModule).GetProperty("Checkers", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("CHECKER_OBSERVATION_SEAM_CHANGED");
        var original = (Dictionary<string, IEntityChecker>)property.GetValue(module)!;
        var overrides = new RuntimeModule();
        foreach (var (entity, checker) in original) overrides.Checker(entity, new Checker(entity, checker, transport, visits));
        module = module.And(overrides);
        var audit = new Audit(); var policy = new Policy();
        var context = module.IntoContext().WithActiveRoot("Tenant", 1)
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module)))
            .WithAppAuditEventSink(audit).WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => policy));
        await context.EnsureSchemaAsync();
        var previous = (await Q.PurchaseOrders().Limit(10).Comment("Determine the graph Fix regression round")
            .Purpose("Run twice without resetting the graph lifecycle store").ExecuteForListAsync(context)).Count;
        Require(previous is 0 or 4, "Unexpected graph lifecycle round boundary");
        var round = previous / 4 + 1; var ordinal = 0; var emitted = 0; Clock clock = null!;
        void Begin()
        {
            clock = new Clock(new DateTimeOffset(2037, 1, round, 0, ++ordinal, 0, TimeSpan.Zero));
            context.WithBusinessClock(clock); transport.Reset(); visits.Clear(); audit.Calls = 0; policy.Calls = 0;
        }
        void Emit(string name, bool passed, Effects effects, int expectedVisits, Exception? failure = null)
        {
            var once = visits.Count == expectedVisits && visits.All(v => v.ProviderDelta.IsZero)
                && visits.All(v => v.InstantMillis == clock.Base.ToUnixTimeMilliseconds());
            var row = new { @case = name, round, passed = passed && once && clock.Calls == 1,
                clockCalls = clock.Calls, instantMillis = clock.Base.ToUnixTimeMilliseconds(), visits = visits.ToArray(),
                effects, auditCalls = audit.Calls, policyCalls = policy.Calls,
                error = failure?.GetType().Name, rules = failure is CheckException error
                    ? error.Violations.Select(v => new { rule = v.RuleId, path = v.ModelPath }).ToArray() : null };
            Console.WriteLine("DOTNET_GRAPH_LIFECYCLE_CASE=" + JsonSerializer.Serialize(row, Json));
            Require(row.passed, "Graph lifecycle case failed: " + name); emitted++;
        }
        (PurchaseOrder Parent, OrderItem Child) NewGraph(string label, bool valid = true)
        {
            var parent = Q.PurchaseOrders().Comment("Prepare a graph lifecycle parent")
                .Purpose("Verify generated graph save boundaries").NewEntity(context);
            var child = Q.OrderItems().Comment("Prepare a graph lifecycle child")
                .Purpose("Verify duplicate references and customer Fix").NewEntity(context);
            parent.UpdateContactPhone("5550811").UpdateInternalNote(null!);
            if (valid) child.UpdateProductName(label + "-" + round);
            child.UpdateUnitPrice(10.00m); parent.OrderItemList.Add(child); parent.OrderItemList.Add(child);
            return (parent, child);
        }
        async Task<PurchaseOrder> Reload(long id) => (await Q.PurchaseOrders().WithIdIs(id)
            .SelectOrderItemListWith(Q.OrderItems().Limit(10)).Limit(1)
            .Comment("Reload the complete checked graph").Purpose("Verify loaded E values and optimistic state")
            .ExecuteForListAsync(context)).Single();
        async Task Check(PurchaseOrder parent, decimal price, long parentVersion, long childVersion)
        {
            var persisted = await Reload(parent.Id!.Value);
            Require(persisted.Version == parentVersion && persisted.OrderItemList.Count == 1, "persisted graph cardinality/version");
            var child = persisted.OrderItemList.Single();
            Require(child.Version == childVersion && E.OrderItem(child).UnitPrice().Eval() == price
                && E.OrderItem(child).PurchaseOrderId().Eval() == parent.Id, "persisted customer Fix and typed FK");
        }
        async Task<Exception> Failed(PurchaseOrder parent)
        {
            try { await parent.AuditAs("Attempt a controlled graph failure").SaveAsync(context); }
            catch (Exception error) { return error; }
            throw new InvalidOperationException("The controlled failure unexpectedly committed");
        }

        var primary = NewGraph("customer-fix"); Begin();
        await primary.Parent.AuditAs("Create a graph with a repeated child reference").SaveAsync(context);
        var effects = transport.Counts();
        Emit("customer-fix-create-duplicate", effects is { ParentWrites: 1, ChildWrites: 1, Begins: 1, Commits: 1, Rollbacks: 0 }
            && audit.Calls == 2 && policy.Calls == 1, effects, 2);
        await Check(primary.Parent, 10.01m, 1, 1);

        var loaded = await Reload(primary.Parent.Id!.Value); var item = loaded.OrderItemList.Single(); loaded.OrderItemList.Add(item);
        loaded.UpdateContactPhone("5550812"); item.UpdateUnitPrice(20.00m); Begin();
        await loaded.AuditAs("Update a graph with a repeated child reference").SaveAsync(context); effects = transport.Counts();
        Emit("customer-fix-update-duplicate", effects is { ParentWrites: 1, ChildWrites: 1, Begins: 1, Commits: 1, Rollbacks: 0 }
            && audit.Calls == 2 && policy.Calls == 1, effects, 2);
        await Check(loaded, 20.01m, 2, 2);

        loaded = await Reload(primary.Parent.Id!.Value); item = loaded.OrderItemList.Single(); loaded.OrderItemList.Add(item);
        item.UpdateUnitPrice(30.00m); Begin();
        await loaded.AuditAs("Save only the modified child under an unchanged parent").SaveAsync(context); effects = transport.Counts();
        Emit("customer-fix-child-only", effects is { ParentWrites: 0, ChildWrites: 1, Begins: 1, Commits: 1, Rollbacks: 0 }
            && audit.Calls == 1 && policy.Calls == 1, effects, 1);
        await Check(loaded, 30.01m, 2, 3);

        foreach (var cancelled in new[] { false, true })
        {
            var graph = NewGraph(cancelled ? "cancel-retry" : "provider-retry"); Begin();
            transport.AfterChildInsert = cancelled ? () => throw new OperationCanceledException(new CancellationToken(true))
                : () => throw new InjectedWriteFailure();
            var error = await Failed(graph.Parent); effects = transport.Counts(); transport.AfterChildInsert = null;
            Emit(cancelled ? "provider-cancellation" : "provider-write-failure", effects is { ParentWrites: 1, ChildWrites: 1, Begins: 1, Commits: 0, Rollbacks: 1 }
                && graph.Parent.Id is null && graph.Child.Id is null && audit.Calls == 0
                && (cancelled ? error is OperationCanceledException : error is InjectedWriteFailure), effects, 2, error);
            var absent = await Q.OrderItems().WithProductNameIs((cancelled ? "cancel-retry" : "provider-retry") + "-" + round)
                .Limit(1).Comment("Confirm a failed graph left no persisted child")
                .Purpose("Independently verify real SQLite rollback").ExecuteForListAsync(context);
            Require(absent.Count == 0, "Failed child insert was not rolled back");
            Begin(); await graph.Parent.AuditAs("Retry the same failed object graph").SaveAsync(context); effects = transport.Counts();
            Emit(cancelled ? "same-graph-cancellation-retry" : "same-graph-provider-retry", effects is { ParentWrites: 1, ChildWrites: 1, Begins: 1, Commits: 1, Rollbacks: 0 }
                && audit.Calls == 2 && policy.Calls == 1, effects, 2);
            await Check(graph.Parent, 10.01m, 1, 1);
        }

        var invalid = NewGraph("missing-input", false); Begin(); var missing = await Failed(invalid.Parent); effects = transport.Counts();
        Emit("missing-child-input-before-io", effects.IsZero && missing is CheckException
            && invalid.Parent.Id is null && invalid.Child.Id is null && audit.Calls == 0 && policy.Calls == 0, effects, 2, missing);
        invalid.Child.UpdateProductName("repaired-input-" + round); Begin();
        await invalid.Parent.AuditAs("Repair and retry the same previously invalid graph").SaveAsync(context); effects = transport.Counts();
        Emit("same-graph-input-repair", effects is { ParentWrites: 1, ChildWrites: 1, Begins: 1, Commits: 1, Rollbacks: 0 }
            && audit.Calls == 2 && policy.Calls == 1, effects, 2); await Check(invalid.Parent, 10.01m, 1, 1);

        var root = (await Q.Tenants().WithIdIs(1).Limit(1).Comment("Load the root for a conflicting-parent graph")
            .Purpose("Reject mutation changes after one child has been checked").ExecuteForListAsync(context)).Single();
        var a = NewGraph("conflicting-parent"); var b = NewGraph("unused-child"); b.Parent.OrderItemList.Clear();
        b.Parent.OrderItemList.Add(a.Child); root.PurchaseOrderList.Add(a.Parent); root.PurchaseOrderList.Add(b.Parent);
        Begin(); Exception? conflict = null;
        try { await root.AuditAs("Reject a child rebound after its first preparation").SaveAsync(context); }
        catch (Exception error) { conflict = error; }
        effects = transport.Counts();
        Emit("conflicting-parent-before-io", effects.IsZero && conflict is CheckException ce
            && ce.Violations.Any(v => v.RuleId == "mutation_changed_after_preparation") && audit.Calls == 0 && policy.Calls == 0,
            effects, 3, conflict);

        loaded = await Reload(primary.Parent.Id!.Value); item = loaded.OrderItemList.Single(); loaded.OrderItemList.Add(item);
        loaded.UpdateContactPhone("5550818"); item.UpdateUnitPrice(40.00m); policy.OnReview = () => item.UpdateUnitPrice(999.00m);
        Begin(); Exception? late = null;
        try { await loaded.AuditAs("Reject a late customer-policy edit of the checked graph").SaveAsync(context); }
        catch (Exception error) { late = error; }
        effects = transport.Counts(); policy.OnReview = null;
        Emit("late-policy-edit-rejected", late != null && effects is { ParentWrites: 0, ChildWrites: 0, Begins: 1, Commits: 0, Rollbacks: 1 }
            && audit.Calls == 0 && policy.Calls == 1, effects, 2, late);
        await Check(loaded, 30.01m, 2, 3);
        item.UpdateUnitPrice(40.00m); Begin();
        await loaded.AuditAs("Retry the checked graph after removing the late-edit policy").SaveAsync(context); effects = transport.Counts();
        Emit("same-graph-policy-retry", effects is { ParentWrites: 1, ChildWrites: 1, Begins: 1, Commits: 1, Rollbacks: 0 }
            && audit.Calls == 2 && policy.Calls == 1, effects, 2); await Check(loaded, 40.01m, 3, 4);
        Require(emitted == 12, "Every graph lifecycle scenario must execute");
        Console.WriteLine("DOTNET_GRAPH_LIFECYCLE_RESULT=" + JsonSerializer.Serialize(new { round, passed = true, cases = emitted }, Json));
    }

    sealed class Schema(RuntimeModule module) : ISchemaProvider
    { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
    sealed class Clock(DateTimeOffset value) : IBusinessClock
    { public DateTimeOffset Base { get; } = value; public int Calls; public DateTimeOffset Now => Base.AddMilliseconds(Calls++); }
    sealed class Audit : IAppAuditEventSink
    { public int Calls; public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default) { Calls++; return Task.CompletedTask; } }
    sealed class Policy : IMutationPolicy
    {
        public int Calls; public Action? OnReview;
        public MutationPolicyIdentity Identity { get; } = new("graph-example", "1", "graph-fix-lifecycle-v1");
        public MutationDecision Review(UserContext context, MutationPlan plan) { Calls++; OnReview?.Invoke(); return MutationDecision.Allowed(); }
    }
    sealed record Effects(int Statements, int Begins, int Commits, int Rollbacks, int ParentWrites, int ChildWrites)
    { public bool IsZero => this == new Effects(0, 0, 0, 0, 0, 0); }
    sealed record Visit(string Entity, long InstantMillis, Effects ProviderDelta);
    sealed class Checker(string entity, IEntityChecker original, Transport transport, List<Visit> visits) : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        {
            var before = transport.Counts();
            try {
                Record? values = request switch { InsertMutationRequest insert => insert.Command.Values, UpdateMutationRequest update => update.Command.Values, _ => null };
                if (entity == "OrderItem" && values != null && values.TryGetValue("unit_price", out var value) && value is Value.DecimalValue number)
                    values["unit_price"] = new Value.DecimalValue(number.Value + 0.01m);
                return original.CheckAndFix(context, request, now);
            } finally { var after = transport.Counts(); visits.Add(new Visit(entity, now.ToUnixTimeMilliseconds(),
                new Effects(after.Statements - before.Statements, after.Begins - before.Begins, after.Commits - before.Commits,
                    after.Rollbacks - before.Rollbacks, after.ParentWrites - before.ParentWrites, after.ChildWrites - before.ChildWrites))); }
        }
    }
    sealed class InjectedWriteFailure : Exception { }
    sealed class Transport(SqliteTransport inner) : IAutomaticMutationTransactionTransport
    {
        int statements, begins, commits, rollbacks, parents, children; public Action? AfterChildInsert;
        public void Reset() { statements = begins = commits = rollbacks = parents = children = 0; }
        public Effects Counts() => new(statements, begins, commits, rollbacks, parents, children);
        bool Observe(CompiledQuery query)
        {
            statements++; var sql = query.Sql.Replace("\"", "");
            if (Regex.IsMatch(sql, @"^\s*(INSERT INTO|UPDATE)\s+purchase_order_data\b", RegexOptions.IgnoreCase)) parents++;
            if (Regex.IsMatch(sql, @"^\s*(INSERT INTO|UPDATE)\s+order_item_data\b", RegexOptions.IgnoreCase)) children++;
            return Regex.IsMatch(sql, @"^\s*INSERT INTO\s+order_item_data\b", RegexOptions.IgnoreCase);
        }
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Observe(query); return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Observe(query); return inner.ExecuteSqlAsync(query); }
        public async Task<ISqlTransaction> BeginSqlAsync() { begins++; return new Transaction(await inner.BeginSqlAsync(), this); }
        sealed class Transaction(ISqlTransaction actual, Transport owner) : ISqlTransaction
        {
            public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { var child = owner.Observe(query); var result = await actual.FetchAllSqlAsync(query); if (child) owner.AfterChildInsert?.Invoke(); return result; }
            public async Task<ulong> ExecuteSqlAsync(CompiledQuery query) { var child = owner.Observe(query); var result = await actual.ExecuteSqlAsync(query); if (child) owner.AfterChildInsert?.Invoke(); return result; }
            public Task CommitSqlAsync() { owner.commits++; return actual.CommitSqlAsync(); }
            public Task RollbackSqlAsync() { owner.rollbacks++; return actual.RollbackSqlAsync(); }
            public void Dispose() => actual.Dispose();
        }
    }
}
