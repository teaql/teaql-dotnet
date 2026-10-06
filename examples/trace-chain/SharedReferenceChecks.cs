using System.Reflection;
using System.Text.Json;
using Generated;
using Generated.Models;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;

// App-owned acceptance: discover Q/E/mutation APIs through retained Assist, never library source.
// Reflection observes ownership only; it supplies neither mutations nor trace frames.
static class SharedReferenceChecks
{
    public static async Task RunAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink,
        FaultTransport transport, string? scenario = null)
    {
        var checks = new (string Name, Func<Task> Work)[] {
            ("shared", () => SharedAsync(context, capture, sink, transport)),
            ("scoped", () => ScopedAsync(context, capture, sink)),
            ("descendant", () => DescendantAsync(context, capture, sink)),
            ("conflict", () => ConflictAsync(context, capture, sink))
        };
        Verify.That(scenario == null || checks.Any(value => value.Name == scenario), "known ownership scenario");
        foreach (var check in checks.Where(value => scenario == null || value.Name == scenario))
        {
            await check.Work();
            Console.WriteLine("PASS TC-OWN-" + check.Name + ": generated Q/E/save, physical SQL and committed audit");
        }
        if (scenario == null) Console.WriteLine("PASS: .NET generated ownership 4 scenarios; no generated edits");
    }

    private static EntityRoot Ledger(object entity) => (EntityRoot)(entity.GetType()
        .GetField("_entityRoot", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(entity)
        ?? throw new Exception("Generated entity omitted its runtime-owned mutation ledger"));
    private static bool Pending(EntityRoot ledger, EntityKey key) =>
        ledger.Change(key).Count > 0 || ledger.IsNew(key) || ledger.IsDeleted(key);

    private static CustomerOrder NewOrder(UserContext context, string label) => Q.CustomerOrders()
        .Comment("prepare ownership order").Purpose("test graph ownership").NewEntity(context)
        .UpdatePlatformId(1).UpdateOrderNumber(label).UpdateDescription("Original " + label);
    private static OrderItem NewItem(UserContext context, string label) => Q.OrderItems()
        .Comment("prepare ownership item").Purpose("test graph ownership").NewEntity(context).UpdateName(label);

    private static async Task<CustomerOrder> LoadAsync(UserContext context, long id, bool platform = false)
    {
        var request = Q.CustomerOrders().WithIdIs(id)
            .SelectOrderItemListWith(Q.OrderItems().OrderByIdAscending().Limit(10));
        if (platform) request.SelectPlatformWith(Q.Platforms().Limit(1));
        return await request.Limit(1).Comment("load ownership graph").Purpose("verify independent loaded graphs")
            .ExecuteForOneAsync(context) ?? throw new Exception("Prepared graph missing");
    }

    private static async Task<CustomerOrder> SeedAsync(UserContext context, string label, int children)
    {
        var order = NewOrder(context, label);
        for (var i = 0; i < children; i++) order.OrderItemList.Add(NewItem(context, label + "-item-" + i));
        await order.AuditAs("prepare ownership graph").SaveAsync(context);
        return order;
    }

    private static void Clear(CapturingExecutor capture, EvidenceSink sink) { capture.Clear(); sink.Clear(); }

    private static void Observe(string scenario, CapturingExecutor capture, EvidenceSink sink)
    {
        Console.WriteLine("OWNERSHIP EVIDENCE " + JsonSerializer.Serialize(new {
            scenario,
            commands = capture.Commands.Select(value => new { value.Entity, value.Id, value.Operation, value.Comment,
                value.ExpectedVersion, lineage = value.Lineage }),
            sql = sink.Sql.Where(value => value.Operation != DataServiceOperation.Query).Select(value => new {
                operation = value.Operation.ToString(), value.Comment, value.DebugQuery,
                value.TraceChain, value.MutationLineage, value.ExecutionOutcome }),
            audit = sink.Audit.Select(value => new { entity = value["entityType"], id = value["entityId"],
                reason = value["reason"], lineage = value["traceChain"] })
        }));
    }

    private static void AssertWrites(CapturingExecutor capture, EvidenceSink sink, string? rootReason,
        params (string Entity, long Id, long Version, string Lineage)[] expected)
    {
        Verify.Equal(expected.Length, capture.Commands.Count, "only reached changed entities emit commands");
        Verify.Equal(expected.Length, sink.Audit.Count, "one committed audit per actual mutation");
        var physical = sink.Sql.Where(value => value.Operation != DataServiceOperation.Query).ToArray();
        Verify.Equal(expected.Length, physical.Length, "unchanged nodes emit no physical SQL");
        foreach (var item in expected)
        {
            var command = capture.Commands.Single(value => value.Entity == item.Entity && value.Id == item.Id);
            Verify.Equal("update", command.Operation, "loaded mutation operation");
            Verify.Equal(item.Version, command.ExpectedVersion, "authoritative type-qualified original version");
            var expectedReason = rootReason ?? item.Lineage[(item.Lineage.IndexOf(':') + 1)..].Split(" -> ")[0];
            Verify.Equal(expectedReason, command.Comment, "explicit root request intent");
            Verify.Equal(item.Lineage, Verify.Shape(command.Lineage), "actual command business lineage");
            // The frozen Rust algorithm rebuilds physical entity frames without IDs;
            // identify the statement using its separate, fully typed mutation lineage.
            var sql = physical.Single(value => value.TraceChain.Any(node => node.Kind == "entity" &&
                node.Name == item.Entity) && Verify.Shape(value.MutationLineage) == item.Lineage);
            Verify.Equal(item.Lineage, Verify.Shape(sql.MutationLineage), "physical SQL carries actual lineage");
            Verify.Equal("success", sql.ExecutionOutcome, "physical SQL succeeded");
            Verify.Equal("CustomerOrder", sql.TraceChain[0].Name, "clean parent remains logical origin");
            Verify.That(sql.TraceChain.All(value => value.Kind != "auditReason"), "audit and physical routes remain separate");
            var audit = sink.Audit.Single(value => value["entityType"]!.ToString() == item.Entity &&
                Convert.ToInt64(value["entityId"]) == item.Id);
            Verify.Equal(item.Lineage, Verify.Shape((IEnumerable<TraceNode>)audit["traceChain"]!), "committed audit matches request");
        }
    }

    private static async Task SharedAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink, FaultTransport transport)
    {
        var nonce = Guid.NewGuid().ToString("N");
        var a = await SeedAsync(context, nonce + "-shared-a", 1);
        var b = await SeedAsync(context, nonce + "-shared-b", 1);
        var revise = await LoadAsync(context, a.Id!.Value);
        await revise.UpdateDescription("prior revision").AuditAs("revise one root").SaveAsync(context);
        transport.ReuseReadOnlyPlatformSnapshot = true;
        CustomerOrder first; CustomerOrder second;
        try { first = await LoadAsync(context, a.Id.Value, true); second = await LoadAsync(context, b.Id!.Value, true); }
        finally { transport.ReuseReadOnlyPlatformSnapshot = false; }
        Verify.Equal(2, transport.SharedPlatformUses, "both queries use the exact provider-loaded Record instance");
        var snapshot = transport.SharedPlatformSnapshot!;
        var before = snapshot.ToJsonValue().ToJsonString();
        var firstPlatform = E.CustomerOrder(first).Platform().Eval() ?? throw new Exception("Selected first Platform missing");
        var secondPlatform = E.CustomerOrder(second).Platform().Eval() ?? throw new Exception("Selected second Platform missing");
        Verify.That(!ReferenceEquals(firstPlatform, secondPlatform), "shared read-only record has separate generated wrappers");
        Verify.That(!ReferenceEquals(Ledger(firstPlatform), Ledger(secondPlatform)), "shared reference has independent mutable ownership");
        Verify.That(!ReferenceEquals(Ledger(first), Ledger(second)), "independent root query ledgers");
        var firstVersion = E.CustomerOrder(first).Version().Eval()!.Value;
        var secondVersion = E.CustomerOrder(second).Version().Eval()!.Value;
        Verify.That(firstVersion != secondVersion, "loaded roots have different authoritative versions");
        var firstItemVersion = E.OrderItem(first.OrderItemList.Single()).Version().Eval();
        var secondItemVersion = E.OrderItem(second.OrderItemList.Single()).Version().Eval();
        var platformVersion = E.Platform(firstPlatform).Version().Eval();
        first.UpdateDescription("saved shared a"); second.UpdateDescription("saved shared b");
        Clear(capture, sink);
        var pause = capture.PauseNextBegin();
        var firstTask = Task.Run(() => first.AuditAs("shared first order").SaveAsync(context));
        await pause.Entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
        // Calling the second public save reaches the same Context's asynchronous gate before first is released.
        var secondTask = second.AuditAs("shared second order").SaveAsync(context);
        Verify.That(!secondTask.IsCompleted, "second save overlaps at the public boundary");
        Verify.Equal(1, capture.BeginCount, "same Context serializes database transactions");
        pause.Release.SetResult(); await Task.WhenAll(firstTask, secondTask).WaitAsync(TimeSpan.FromSeconds(15));
        Observe("shared", capture, sink);
        Verify.Equal(2, capture.Commands.Count, "root-only updates must not rewrite loaded reverse children");
        AssertWrites(capture, sink, null,
            ("CustomerOrder", first.Id!.Value, firstVersion, $"CustomerOrder#{first.Id}:shared first order"),
            ("CustomerOrder", second.Id!.Value, secondVersion, $"CustomerOrder#{second.Id}:shared second order"));
        Verify.Equal(before, snapshot.ToJsonValue().ToJsonString(), "shared provider snapshot remains read-only throughout saves");
        Verify.That(Ledger(first).IsEmpty && Ledger(second).IsEmpty, "committed root graphs clear without phantom new keys");
        Verify.That(Ledger(firstPlatform).IsEmpty && Ledger(secondPlatform).IsEmpty, "read-only references have no mutations");
        var persistedFirst = await LoadAsync(context, first.Id.Value, true);
        var persistedSecond = await LoadAsync(context, second.Id.Value, true);
        Verify.Equal("saved shared a", E.CustomerOrder(persistedFirst).Description().Eval(), "first root Q/E readback");
        Verify.Equal("saved shared b", E.CustomerOrder(persistedSecond).Description().Eval(), "second root Q/E readback");
        Verify.Equal(firstItemVersion, E.OrderItem(persistedFirst.OrderItemList.Single()).Version().Eval(), "first child stays unchanged");
        Verify.Equal(secondItemVersion, E.OrderItem(persistedSecond.OrderItemList.Single()).Version().Eval(), "second child stays unchanged");
        Verify.Equal(platformVersion, E.Platform(E.CustomerOrder(persistedFirst).Platform().Eval()).Version().Eval(), "shared Platform stays unchanged");
    }

    private static async Task ScopedAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink)
    {
        var nonce = Guid.NewGuid().ToString("N");
        var foreignSeed = await SeedAsync(context, nonce + "-foreign", 2);
        var targetSeed = await SeedAsync(context, nonce + "-target", 0);
        var foreign = await LoadAsync(context, foreignSeed.Id!.Value);
        var target = await LoadAsync(context, targetSeed.Id!.Value);
        var reached = foreign.OrderItemList[0]; var sibling = foreign.OrderItemList[1];
        var source = Ledger(foreign);
        Verify.That(ReferenceEquals(source, Ledger(reached)) && ReferenceEquals(source, Ledger(sibling)), "one loaded source graph ledger");
        foreign.UpdateDescription("foreign pending root"); sibling.UpdateName("foreign pending sibling");
        reached.UpdateName("reached imported child").AuditAs("adopt reached item");
        target.UpdateDescription("target changed"); target.OrderItemList.Add(reached);
        var childVersion = E.OrderItem(reached).Version().Eval()!.Value;
        var targetVersion = E.CustomerOrder(target).Version().Eval()!.Value;
        Clear(capture, sink); await target.AuditAs("save target graph").SaveAsync(context);
        Observe("scoped", capture, sink);
        Verify.That(!Pending(Ledger(target), new EntityKey("CustomerOrder", foreign.Id!.Value)), "foreign root was not imported");
        Verify.That(!Pending(Ledger(target), new EntityKey("OrderItem", sibling.Id!.Value)), "unselected sibling was not imported");
        Verify.That(Pending(source, new EntityKey("CustomerOrder", foreign.Id.Value)), "source root stays pending");
        Verify.That(Pending(source, new EntityKey("OrderItem", sibling.Id.Value)), "source sibling stays pending");
        Verify.That(Pending(source, new EntityKey("OrderItem", reached.Id!.Value)), "source reached pending record was copied, not drained");
        AssertWrites(capture, sink, "save target graph",
            ("CustomerOrder", target.Id!.Value, targetVersion, $"CustomerOrder#{target.Id}:save target graph"),
            ("OrderItem", reached.Id.Value, childVersion, $"CustomerOrder#{target.Id}:save target graph -> OrderItem#{reached.Id}:adopt reached item"));
        var persistedForeign = await LoadAsync(context, foreign.Id.Value);
        Verify.That(E.CustomerOrder(persistedForeign).Description().Eval() != "foreign pending root", "foreign root was not saved");
        Verify.Equal(1, persistedForeign.OrderItemList.Count, "reached child moved to target graph");
        Verify.That(E.OrderItem(persistedForeign.OrderItemList.Single()).Name().Eval() != "foreign pending sibling", "unselected sibling was not saved");
        var persistedTarget = await LoadAsync(context, target.Id.Value);
        Verify.Equal("reached imported child", E.OrderItem(persistedTarget.OrderItemList.Single()).Name().Eval(), "generated Q/E verifies imported mutation");
        Verify.Equal(target.Id, E.OrderItem(persistedTarget.OrderItemList.Single()).CustomerOrderId().Eval(), "reached FK was changed deliberately");
    }

    private static async Task DescendantAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink)
    {
        var seed = await SeedAsync(context, Guid.NewGuid().ToString("N") + "-descendant", 1);
        var order = await LoadAsync(context, seed.Id!.Value); var child = order.OrderItemList.Single();
        var parentVersion = E.CustomerOrder(order).Version().Eval();
        var childVersion = E.OrderItem(child).Version().Eval()!.Value;
        child.UpdateName("changed child only").AuditAs("repair item");
        Clear(capture, sink); await order.AuditAs("save descendant graph").SaveAsync(context);
        Observe("descendant", capture, sink);
        AssertWrites(capture, sink, "save descendant graph", ("OrderItem", child.Id!.Value, childVersion,
            $"CustomerOrder#{order.Id}:save descendant graph -> OrderItem#{child.Id}:repair item"));
        Verify.Equal(parentVersion, E.CustomerOrder(order).Version().Eval(), "clean parent's in-memory version is unchanged");
        var persisted = await LoadAsync(context, order.Id!.Value);
        Verify.Equal(parentVersion, E.CustomerOrder(persisted).Version().Eval(), "clean parent's persisted version is unchanged");
        Verify.Equal("changed child only", E.OrderItem(persisted.OrderItemList.Single()).Name().Eval(), "child change persists through generated Q/E");
    }

    private static async Task ConflictAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink)
    {
        var seed = await SeedAsync(context, Guid.NewGuid().ToString("N") + "-conflict", 1);
        var old = await LoadAsync(context, seed.Id!.Value); var oldChild = old.OrderItemList.Single();
        var advance = await Q.OrderItems().WithIdIs(oldChild.Id!.Value).Limit(1)
            .Comment("load child for independent revision").Purpose("create conflicting loaded versions")
            .ExecuteForOneAsync(context) ?? throw new Exception("Prepared child missing");
        await advance.UpdateName("committed revision").AuditAs("revise only child").SaveAsync(context);
        var current = await LoadAsync(context, seed.Id.Value); var currentChild = current.OrderItemList.Single();
        Verify.That(E.OrderItem(oldChild).Version().Eval() != E.OrderItem(currentChild).Version().Eval(), "actual database-loaded versions differ");
        var oldLedger = Ledger(oldChild); var currentLedger = Ledger(currentChild);
        oldChild.UpdateName("old pending"); currentChild.UpdateName("current pending");
        old.OrderItemList.Add(currentChild);
        Clear(capture, sink);
        try { await old.AuditAs("reject conflicting graph").SaveAsync(context); throw new Exception("ASSERT: conflicting loaded versions were accepted"); }
        catch (InvalidOperationException error) when (error.Message.Contains("ENTITY_VERSION_CONFLICT")) { }
        catch { Observe("conflict", capture, sink); throw; }
        Verify.Equal(0, capture.Commands.Count, "version conflict rejects before business SQL");
        Verify.Equal(0, sink.Audit.Count, "version conflict emits no committed audit");
        Verify.That(Pending(oldLedger, new EntityKey("OrderItem", oldChild.Id.Value)), "old graph keeps pending change");
        Verify.That(Pending(currentLedger, new EntityKey("OrderItem", currentChild.Id!.Value)), "current graph keeps pending change");
        Verify.Equal(new Value.TextValue("old pending"), oldLedger.Change(new EntityKey("OrderItem", oldChild.Id.Value))["name"], "old pending value is not overwritten");
        Verify.Equal(new Value.TextValue("current pending"), currentLedger.Change(new EntityKey("OrderItem", currentChild.Id.Value))["name"], "current pending value is not consumed");
        Observe("conflict", capture, sink);
    }
}
