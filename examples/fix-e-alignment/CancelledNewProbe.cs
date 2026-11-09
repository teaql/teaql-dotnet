using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Text.RegularExpressions;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

// ML-12 through generated Q/E/audited saves. No handwritten business SQL.
static class CancelledNewProbe
{
    public static readonly string[] Scenarios = ["root", "complete-child", "incomplete-child",
        "already-composed-child", "invalid-live-sibling", "rollback-retry"];
    static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    static void Require(bool value, string message) { if (!value) throw new AssertionFailure(message); }
    public static async Task<bool> Verify(string filename, int round)
    {
        var passed = 0;
        foreach (var name in Scenarios)
        {
            var row = new Dictionary<string, object?> { ["scenario"] = name, ["round"] = round, ["passed"] = false,
                ["runtimeAssembly"] = typeof(UserContext).Assembly.Location };
            await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
            await connection.OpenAsync();
            var transport = new Transport(new SqliteTransport(connection)); var clock = new Clock(); var audit = new Audit();
            var module = GeneratedRuntimeModule.Module;
            var context = module.IntoContext().WithActiveRoot("Tenant", 1)
                .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module)))
                .WithBusinessClock(clock).WithAppAuditEventSink(audit)
                .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => new Policy()));
            try
            {
                await context.EnsureSchemaAsync(); await context.EnsureSchemaAsync();
                var parent = await Q.PurchaseOrders().Comment("Prepare the cancellation owner")
                    .Purpose("Verify abandoned creation never becomes a stored Delete").NewEntityAsync(context);
                parent.UpdateContactPhone("5550188");
                async Task<OrderItem> Child(bool complete)
                {
                    var value = await Q.OrderItems().Comment("Prepare a cancellation child")
                        .Purpose("Separate abandoned and live creation intent").NewEntityAsync(context);
                    if (complete) value.UpdateProductName("live child").UpdateUnitPrice(12.34m);
                    return value;
                }
                OrderItem? cancelled = null;
                if (name == "root") parent.MarkForDeletion();
                else
                {
                    cancelled = await Child(name != "incomplete-child");
                    if (name == "already-composed-child") parent.OrderItemList.Add(cancelled);
                    cancelled.MarkForDeletion();
                    if (name != "already-composed-child") parent.OrderItemList.Add(cancelled);
                    parent.OrderItemList.Add(await Child(name != "invalid-live-sibling"));
                }
                var parentId = parent.Id; var childId = cancelled?.Id; var cancelledVersion = cancelled?.Version;
                row["parentId"] = parentId; row["cancelledId"] = childId;
                transport.Reset(); audit.Calls = 0; clock.Calls = 0;
                Task<PurchaseOrder> Save() => parent.AuditAs("Save only the intended cancellation graph").SaveAsync(context);
                async Task<bool> ParentAbsent() => (await Q.PurchaseOrders().WithIdIs(parentId!.Value).Limit(1)
                    .Comment("Check the rejected or rolled-back graph")
                    .Purpose("Verify the graph left no committed parent").ExecuteForListAsync(context)).Count == 0;
                if (name is "root" or "invalid-live-sibling")
                {
                    Exception? failure = null;
                    try { await Save(); } catch (Exception error) { failure = error; }
                    row["rejectionType"] = failure?.GetType().Name;
                    row["clockCalls"] = clock.Calls;
                    Require(failure != null, "invalid graph was accepted");
                    if (name == "invalid-live-sibling")
                    {
                        Require(failure is CheckException, "live sibling did not reach Checker");
                        var rules = ((CheckException)failure!).Violations.Select(v => new { v.RuleId, v.ModelPath }).ToArray();
                        row["rules"] = rules;
                        Require(rules.Any(v => v.RuleId == "required" && v.ModelPath.Contains("product_name")), "missing live child required violation");
                    }
                    Require(transport.Counts().IsZero && audit.Calls == 0, "rejection had provider or committed audit effects");
                    if (name == "root") Require(clock.Calls == 0, "cancelled root consumed Business Clock");
                    Require(await ParentAbsent(), "rejected parent persisted");
                    row["rejected"] = true; row["zeroProviderEffects"] = true; row["pureRejection"] = name == "root";
                }
                else
                {
                    if (name == "rollback-retry")
                    {
                        transport.AfterChildInsert = () => throw new InjectedWriteFailure();
                        Exception? failure = null;
                        try { await Save(); } catch (Exception error) { failure = error; }
                        row["injectedFailureType"] = failure?.GetBaseException().GetType().Name;
                        Require(failure?.GetBaseException() is InjectedWriteFailure, "injected real-write failure did not occur");
                        Require(transport.Counts() is { ParentWrites: 1, ChildWrites: 1, Rollbacks: 1, Commits: 0 }, "partial writes were not rolled back");
                        Require(audit.Calls == 0 && parent.Id == parentId && cancelled!.Id == childId && parent.Version is null,
                            "rollback lost original graph identity or emitted committed audit");
                        Require(await ParentAbsent(), "rolled-back parent persisted");
                        transport.AfterChildInsert = null; transport.Reset(); audit.Calls = 0; clock.Calls = 0;
                    }
                    await Save(); var effects = transport.Counts();
                    Require(effects is { ParentWrites: 1, ChildWrites: 1, Begins: 1, Commits: 1, Rollbacks: 0 }, "wrong committed writes");
                    Require(parent.Version == 1 && cancelled!.Version == cancelledVersion && audit.Calls == 2, "wrong committed versions or audit cardinality");
                    var loaded = (await Q.PurchaseOrders().WithIdIs(parentId!.Value)
                        .SelectOrderItemListWith(Q.OrderItems().Limit(10)).Limit(1)
                        .Comment("Reload only the intended cancellation graph")
                        .Purpose("Verify live children with generated E and abandoned row absence").ExecuteForListAsync(context)).Single();
                    Require(E.PurchaseOrder(loaded).OrderItemList().Size().Eval() == 1, "wrong generated relation cardinality");
                    var absent = (await Q.OrderItems().WithIdIs(childId!.Value).Limit(1)
                        .Comment("Check the abandoned child is absent")
                        .Purpose("An abandoned new child must not produce a row or tombstone").ExecuteForListAsync(context)).Count == 0;
                    Require(absent, "cancelled child was stored");
                    row["rejected"] = false; row["parentVersion"] = parent.Version; row["liveChildren"] = 1;
                    row["cancelledAbsent"] = true; row["mutations"] = effects.ParentWrites + effects.ChildWrites;
                    row["audits"] = audit.Calls; row["retried"] = name == "rollback-retry";
                }
                row["passed"] = true; passed++;
            }
            catch (Exception error)
            {
                row["errorType"] = error.GetType().Name; row["cause"] = error.GetBaseException().GetType().Name;
                row["errorCode"] = error is EntityCreationException creation ? creation.Code : null;
                row["failedAssertion"] = error is AssertionFailure ? error.Message : null;
                row["clockCalls"] = clock.Calls; row["effects"] = transport.Counts();
            }
            Console.WriteLine("DOTNET_CANCELLED_NEW=" + JsonSerializer.Serialize(row, Json));
        }
        Console.WriteLine("DOTNET_CANCELLED_NEW_RESULT=" + JsonSerializer.Serialize(new { round, passed = passed == Scenarios.Length,
            cases = Scenarios.Length, passedCases = passed }, Json));
        return passed == Scenarios.Length;
    }
    sealed class AssertionFailure(string message) : Exception(message);
    sealed class InjectedWriteFailure : Exception;
    sealed class Schema(RuntimeModule module) : ISchemaProvider { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
    sealed class Clock : IBusinessClock { public int Calls; public DateTimeOffset Now => new DateTimeOffset(2037, 6, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(Calls++); }
    sealed class Audit : IAppAuditEventSink { public int Calls; public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default) { Calls++; return Task.CompletedTask; } }
    sealed class Policy : IMutationPolicy
    { public MutationPolicyIdentity Identity { get; } = new("cancelled-new-example", "1", "cancelled-new-v1"); public MutationDecision Review(UserContext context, MutationPlan plan) => MutationDecision.Allowed(); }
    sealed record Effects(int Statements, int Begins, int Commits, int Rollbacks, int ParentWrites, int ChildWrites)
    { public bool IsZero => this == new Effects(0, 0, 0, 0, 0, 0); }
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
            public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { var child = owner.Observe(query); var value = await actual.FetchAllSqlAsync(query); if (child) owner.AfterChildInsert?.Invoke(); return value; }
            public async Task<ulong> ExecuteSqlAsync(CompiledQuery query) { var child = owner.Observe(query); var value = await actual.ExecuteSqlAsync(query); if (child) owner.AfterChildInsert?.Invoke(); return value; }
            public Task CommitSqlAsync() { owner.commits++; return actual.CommitSqlAsync(); }
            public Task RollbackSqlAsync() { owner.rollbacks++; return actual.RollbackSqlAsync(); }
            public void Dispose() => actual.Dispose();
        }
    }
}
