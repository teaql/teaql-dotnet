using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Generated;
using Generated.Models;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;

// Uses the installed generated checkers unchanged. The Context gate serializes
// transactions/checking while two real public saves remain outstanding.
static class CheckerOverlapChecks
{
    public static async Task RunAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink,
        FaultTransport transport)
    {
        foreach (var logging in new[] { false, true })
            foreach (var rejectedFirst in new[] { false, true })
                await ScenarioAsync(context, capture, sink, transport, logging, rejectedFirst);
        context.EnableQuerySqlLog().EnableMutationSqlLog();
        Console.WriteLine("PASS: .NET generated Checker accepted/rejected overlap 4 cases, shared reference and retry");
    }

    private static EntityRoot Ledger(object entity) => (EntityRoot)(entity.GetType()
        .GetField("_entityRoot", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(entity)
        ?? throw new Exception("Generated entity omitted its runtime-owned ledger"));

    private static CustomerOrder NewOrder(UserContext context, string label) => Q.CustomerOrders()
        .Comment("initialize checker overlap root").Purpose("exercise installed generated checker")
        .NewEntity(context).UpdatePlatformId(1).UpdateOrderNumber(label).UpdateDescription("original " + label);

    private static OrderItem NewItem(UserContext context) => Q.OrderItems()
        .Comment("initialize checker overlap child").Purpose("exercise genuine required-field validation")
        .NewEntity(context);

    private static async Task<CustomerOrder> LoadAsync(UserContext context, long id) =>
        await Q.CustomerOrders().WithIdIs(id).SelectPlatformWith(Q.Platforms().Limit(1))
            .SelectOrderItemListWith(Q.OrderItems().OrderByIdAscending().Limit(10)).Limit(1)
            .Comment("load complete checker overlap graph").Purpose("preserve authoritative values and versions")
            .ExecuteForOneAsync(context) ?? throw new Exception("Checker overlap root missing");

    private static void Clear(CapturingExecutor capture, EvidenceSink sink, FaultTransport transport)
    { capture.Clear(); sink.Clear(); transport.PhysicalStatements.Clear(); }

    private static bool BusinessSql(PhysicalSQLObservation statement) =>
        statement.Sql.Contains("customer_order_data", StringComparison.Ordinal)
        || statement.Sql.Contains("order_item_data", StringComparison.Ordinal)
        || statement.Sql.Contains("platform_data", StringComparison.Ordinal);

    private static void AssertAccepted(CapturingExecutor capture, EvidenceSink sink, FaultTransport transport,
        bool logging, CustomerOrder root, OrderItem child, string reason, string childReason, long originalVersion)
    {
        var expected = new Dictionary<(string, long), string> {
            [("CustomerOrder", root.Id!.Value)] = $"CustomerOrder#{root.Id}:{reason}",
            [("OrderItem", child.Id!.Value)] = $"CustomerOrder#{root.Id}:{reason} -> OrderItem#{child.Id}:{childReason}"
        };
        Verify.Equal(2, capture.Commands.Count, "only accepted root and child reach mutation provider");
        foreach (var command in capture.Commands)
        {
            Verify.That(expected.ContainsKey((command.Entity, command.Id)), "no rejected or readonly-reference command");
            Verify.Equal(reason, command.Comment, "accepted root request intent");
            Verify.Equal(expected[(command.Entity, command.Id)], Verify.Shape(command.Lineage), "accepted command lineage");
        }
        Verify.Equal(originalVersion, capture.Commands.Single(value => value.Entity == "CustomerOrder").ExpectedVersion,
            "accepted root retains authoritative optimistic version");
        var physical = transport.PhysicalStatements.Where(BusinessSql).ToArray();
        Verify.Equal(4, physical.Length, "accepted two writes and two authoritative reads, even with logging off");
        Verify.Equal(2, physical.Count(value => !value.IsRead), "no rejected graph physical write");
        Verify.That(physical.All(value => !value.Sql.Contains("platform_data", StringComparison.Ordinal)), "readonly reference emits no SQL");
        var rootWrite = physical.Single(value => !value.IsRead && value.Sql.StartsWith("UPDATE customer_order_data", StringComparison.Ordinal));
        Verify.Equal(E.CustomerOrder(root).Description().Eval(), rootWrite.Parameters[0].Raw, "raw root write keeps accepted pending value");
        Verify.Equal(root.Id, rootWrite.Parameters[2].Raw, "raw root write targets only accepted root");
        Verify.Equal(originalVersion, rootWrite.Parameters[3].Raw, "raw root write retains accepted optimistic version");
        var childWrite = physical.Single(value => !value.IsRead && value.Sql.StartsWith("INSERT INTO order_item_data", StringComparison.Ordinal));
        var childName = E.OrderItem(child).Name().Eval()!;
        Verify.Equal(child.Id, childWrite.Parameters[0].Raw, "raw child write targets accepted child");
        Verify.Equal(root.Id, childWrite.Parameters[1].Raw, "raw child write belongs to accepted root");
        Verify.Equal(childName, childWrite.Parameters[2].Raw, "trusted physical binding retains private child value");
        foreach (var read in physical.Where(value => value.IsRead))
            Verify.Equal(read.Sql.Contains("customer_order_data", StringComparison.Ordinal) ? root.Id : child.Id,
                read.Parameters.Single().Raw, "authoritative read targets only accepted entity");
        Verify.Equal(logging ? 4 : 0, sink.Sql.Count, "query and mutation log switches do not alter execution");
        foreach (var sql in sink.Sql)
        {
            Verify.Equal("CustomerOrder", sql.TraceChain[0].Name, "physical SQL keeps accepted request root");
            Verify.Equal(reason, sql.Comment, "physical SQL keeps accepted intent");
            Verify.That(expected.Values.Contains(Verify.Shape(sql.MutationLineage)), "physical SQL carries only accepted graph lineage");
            Verify.That(sql.TraceChain.All(value => value.Kind != "auditReason"), "physical and mutation paths stay separate");
            Verify.Equal("success", sql.ExecutionOutcome, "Checker rejection invents no failed SQL statement");
            Verify.Equal("SAFE", sql.LogMode, "emitted SQL uses default safe boundary");
        }
        Verify.Equal(2, sink.Audit.Count, "only actual committed root and child emit audit");
        foreach (var audit in sink.Audit)
        {
            var key = (audit["entityType"]!.ToString()!, Convert.ToInt64(audit["entityId"]));
            Verify.That(expected.ContainsKey(key), "no rejected or shared-reference committed audit");
            Verify.Equal(expected[key], Verify.Shape((IEnumerable<TraceNode>)audit["traceChain"]!), "committed accepted lineage");
        }
        Verify.That(!JsonSerializer.Serialize(new { sql = sink.Sql, audit = sink.Audit }).Contains(childName, StringComparison.Ordinal),
            "safe SQL and committed audit never expose the private accepted child value");
    }

    private static async Task ScenarioAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink,
        FaultTransport transport, bool logging, bool rejectedFirst)
    {
        context.EnableQuerySqlLog().EnableMutationSqlLog();
        var nonce = Guid.NewGuid().ToString("N");
        var goodSeed = NewOrder(context, nonce + "-good");
        var badSeed = NewOrder(context, nonce + "-bad");
        await goodSeed.AuditAs("prepare accepted checker graph").SaveAsync(context);
        await badSeed.AuditAs("prepare rejected checker graph").SaveAsync(context);
        var sharedUses = transport.SharedPlatformUses;
        transport.ReuseReadOnlyPlatformSnapshot = true;
        CustomerOrder good; CustomerOrder bad;
        try { good = await LoadAsync(context, goodSeed.Id!.Value); bad = await LoadAsync(context, badSeed.Id!.Value); }
        finally { transport.ReuseReadOnlyPlatformSnapshot = false; }
        Verify.Equal(sharedUses + 2, transport.SharedPlatformUses, "both loads reuse the exact provider Platform Record");
        var shared = transport.SharedPlatformSnapshot ?? throw new Exception("Shared Platform snapshot missing");
        var sharedBefore = shared.ToJsonValue().ToJsonString();
        var goodPlatform = E.CustomerOrder(good).Platform().Eval() ?? throw new Exception("Accepted Platform missing");
        var badPlatform = E.CustomerOrder(bad).Platform().Eval() ?? throw new Exception("Rejected Platform missing");
        var platformVersion = E.Platform(goodPlatform).Version().Eval();
        Verify.That(!ReferenceEquals(goodPlatform, badPlatform), "readonly shared record has separate generated wrappers");
        Verify.That(!ReferenceEquals(Ledger(good), Ledger(bad)), "independent graph-owned root ledgers");
        Verify.That(!ReferenceEquals(Ledger(goodPlatform), Ledger(badPlatform)), "shared reference does not share mutation ownership");
        var goodVersion = E.CustomerOrder(good).Version().Eval()!.Value;
        var badVersion = E.CustomerOrder(bad).Version().Eval()!.Value;
        var badOriginal = E.CustomerOrder(bad).Description().Eval();
        good.UpdateDescription("accepted overlap " + nonce);
        bad.UpdateDescription("rejected pending " + nonce);
        var goodChild = NewItem(context).UpdateName("valid checked child " + nonce).AuditAs("accept checked child");
        var badChild = NewItem(context).AuditAs("reject incomplete child"); // No UpdateName: genuine required-field violation.
        good.OrderItemList.Add(goodChild); bad.OrderItemList.Add(badChild);
        if (!logging) context.DisableQuerySqlLog().DisableMutationSqlLog();
        Clear(capture, sink, transport);
        var pause = capture.PauseNextBegin();
        var callers = new ConcurrentQueue<UserContext>();
        async Task<CheckException?> Save(CustomerOrder entity, string reason)
        {
            callers.Enqueue(context);
            try { await entity.AuditAs(reason).SaveAsync(context); return null; }
            catch (CheckException failure) { return failure; }
        }
        Task<CheckException?> first; Task<CheckException?> second;
        first = Save(rejectedFirst ? bad : good, rejectedFirst ? "reject incomplete graph" : "accept checked graph");
        try
        {
            await pause.Entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
            second = Save(rejectedFirst ? good : bad, rejectedFirst ? "accept checked graph" : "reject incomplete graph");
            Verify.That(!first.IsCompleted && !second.IsCompleted, "two public saves overlap on the same Context");
            Verify.Equal(2, callers.Count, "both public SaveAsync calls started");
            Verify.That(callers.All(value => ReferenceEquals(value, context)), "both public calls use original Context identity");
            Verify.Equal(1, capture.BeginCount, "second save waits on the Context transaction/checker gate");
        }
        finally { pause.Release.TrySetResult(); }
        var results = await Task.WhenAll(first, second).WaitAsync(TimeSpan.FromSeconds(15));
        var accepted = results[rejectedFirst ? 1 : 0];
        var rejected = results[rejectedFirst ? 0 : 1];
        Verify.That(accepted is null && rejected is not null, "actual generated Checker accepts one graph and rejects the other");
        Console.WriteLine("CHECKER VIOLATIONS " + JsonSerializer.Serialize(rejected!.Violations.Select(value => new {
            value.RuleId, value.EntityType, value.ModelPath, value.NativePath, value.InstancePath, value.InputValue
        })));
        Verify.That(rejected.Violations.Any(value => value.RuleId.Equals("required", StringComparison.OrdinalIgnoreCase)
            && value.Location.Segments.LastOrDefault() is ObjectLocationSegment.Property { Name: "name" }),
            "real generated required-name rule rejects missing child input");
        Verify.Equal(2, capture.BeginCount, "both serialized graph transactions were reached");
        Verify.Equal(1, capture.CommitCount, "only accepted transaction commits");
        Verify.Equal(1, capture.RollbackCount, "rejected Checker transaction rolls back");
        Verify.Equal(0, capture.ActiveTransactions, "both transactions finish before assertions");
        AssertAccepted(capture, sink, transport, logging, good, goodChild, "accept checked graph", "accept checked child", goodVersion);
        Verify.That(Ledger(good).IsEmpty && !Ledger(bad).IsEmpty, "accepted ledger clears while rejected changes remain pending");
        Verify.That(Ledger(goodPlatform).IsEmpty && Ledger(badPlatform).IsEmpty, "shared reference has no pending writes");
        Verify.Equal(badVersion, E.CustomerOrder(bad).Version().Eval(), "rejected wrapper version is unchanged");
        Verify.Equal(sharedBefore, shared.ToJsonValue().ToJsonString(), "provider-shared snapshot stays unmodified");
        Console.WriteLine("CHECKER OVERLAP EVIDENCE " + JsonSerializer.Serialize(new {
            logging, rejectedFirst, sameContext = true, boundary = "public saves overlap; transactions and Checker are serialized",
            begins = capture.BeginCount, commits = capture.CommitCount, rollbacks = capture.RollbackCount,
            commands = capture.Commands, physical = transport.PhysicalStatements.Where(BusinessSql), sql = sink.Sql, audit = sink.Audit
        }));
        var goodReloaded = await LoadAsync(context, good.Id!.Value);
        var badReloaded = await LoadAsync(context, bad.Id!.Value);
        Verify.Equal(goodVersion + 1, E.CustomerOrder(goodReloaded).Version().Eval(), "accepted root version advanced once");
        Verify.Equal(1, goodReloaded.OrderItemList.Count, "accepted child persisted");
        Verify.Equal("valid checked child " + nonce, E.OrderItem(goodReloaded.OrderItemList.Single()).Name().Eval(), "accepted child value persisted");
        Verify.Equal(badVersion, E.CustomerOrder(badReloaded).Version().Eval(), "rejected database root version unchanged");
        Verify.Equal(badOriginal, E.CustomerOrder(badReloaded).Description().Eval(), "rejected database root value unchanged");
        Verify.Equal(0, badReloaded.OrderItemList.Count, "no rejected child persisted");
        Verify.Equal(platformVersion, E.Platform(E.CustomerOrder(badReloaded).Platform().Eval()).Version().Eval(), "shared reference version unchanged");

        // Follow-up uses the same rejected wrappers/ledger and original Context.
        badChild.UpdateName("repaired checked child " + nonce).AuditAs("repair checked child");
        Clear(capture, sink, transport);
        await bad.AuditAs("repair rejected graph").SaveAsync(context);
        AssertAccepted(capture, sink, transport, logging, bad, badChild, "repair rejected graph", "repair checked child", badVersion);
        Verify.Equal(1, capture.CommitCount, "next valid save commits after rejection");
        Verify.Equal(0, capture.RollbackCount, "next valid save has no stale Checker failure");
        Verify.That(Ledger(bad).IsEmpty, "repaired graph clears only after its own commit");
        var repaired = await LoadAsync(context, bad.Id.Value);
        Verify.Equal(badVersion + 1, E.CustomerOrder(repaired).Version().Eval(), "retry advances rejected root exactly once");
        Verify.Equal("repaired checked child " + nonce, E.OrderItem(repaired.OrderItemList.Single()).Name().Eval(), "retry child persisted");
        Verify.Equal(sharedBefore, shared.ToJsonValue().ToJsonString(), "retry also leaves shared snapshot unchanged");
        Console.WriteLine($"PASS TC-MUT-12 Checker overlap logging={logging} rejectedFirst={rejectedFirst}: actual required rejection, accepted-only SQL/audit, unchanged DB and same-wrapper retry");
    }
}
