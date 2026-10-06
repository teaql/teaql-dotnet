using System.Text.Json;
using Generated;
using TeaQL.Runtime;
using TeaQL.Core;

static class LoadedPrivacyChecks
{
    public static async Task RunAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink)
    {
        var nonce = Guid.NewGuid().ToString("N");
        var alphabetic = string.Concat(nonce.Select(c => (char)('A' + Convert.ToInt32(c.ToString(), 16))));
        var oldValue = "PRIVATE-LOADED-" + alphabetic;
        var root = Q.CustomerOrders().Comment("prepare loaded privacy graph").Purpose("verify old scalar provenance")
            .NewEntity(context).UpdatePlatformId(1).UpdateOrderNumber("loaded-" + nonce).UpdateDescription("initial");
        var child = Q.OrderItems().Comment("prepare private child").Purpose("verify old scalar provenance")
            .NewEntity(context).UpdateName(oldValue);
        root.OrderItemList.Add(child);
        await root.AuditAs("seed loaded privacy graph").SaveAsync(context);
        var rootId = E.CustomerOrder(root).Id().Eval()!.Value;
        var childId = E.OrderItem(child).Id().Eval()!.Value;
        var loaded = await Q.CustomerOrders().WithIdIs(rootId)
            .SelectOrderItemListWith(Q.OrderItems().Limit(2)).Limit(1)
            .Comment("load graph with private prior value").Purpose("modify fully loaded graph").ExecuteForOneAsync(context)
            ?? throw new Exception("missing loaded parent");
        var loadedChild = loaded.OrderItemList.Single();
        Verify.Equal(oldValue, E.OrderItem(loadedChild).Name().Eval(), "query hydrates private prior value");
        void AssertPrivateChain(string reason, params string[] privateValues)
        {
            var safeReason = reason;
            foreach (var value in privateValues) safeReason = safeReason.Replace(value, "[REDACTED]", StringComparison.Ordinal);
            void Chain(IEnumerable<TraceNode> actual, string expectedReason, string boundary)
            {
                var nodes = actual.ToArray();
                Verify.Equal(1, nodes.Length, boundary + ": complete root chain required");
                var node = nodes[0];
                Verify.That(node.Kind == "auditReason" && node.Name == "CustomerOrder" &&
                    node.EntityId == checked((ulong)rootId) && node.Detail == expectedReason && node.Comment == "",
                    boundary + ": typed root identity and intent preserved");
            }
            Verify.Equal(2, capture.Commands.Count, "privacy commands contain parent and child");
            Verify.ExactIdentities(new[] { ("CustomerOrder", rootId), ("OrderItem", childId) },
                capture.Commands.Select(command => (command.Entity, command.Id)), "privacy commands retain target identities");
            foreach (var command in capture.Commands)
            {
                Verify.Equal(reason, command.Comment, "privacy command retains original request intent");
                Chain(command.Lineage, reason, "raw privacy command");
            }
            if (privateValues.Length == 2)
                Verify.That(capture.MutationResults.Any(result => result.Parameters.Contains(Value.FromObject(privateValues[1]))),
                    "privacy projection must not rewrite provider bindings");
            Verify.Equal(4, sink.Sql.Count, "privacy writes retain both authoritative readbacks");
            foreach (var fact in sink.Sql)
            {
                Verify.Equal(safeReason, fact.AuditReason, "safe SQL keeps root reason");
                Chain(fact.MutationLineage, safeReason, "safe privacy SQL");
                Verify.Equal("success", fact.ExecutionOutcome, "privacy physical statement succeeded");
                Verify.Equal(4, fact.TraceChain.Count, "privacy SQL route stays complete");
                Verify.That(fact.TraceChain[0].Kind == "operation" && fact.TraceChain[0].Name == "CustomerOrder" &&
                    fact.TraceChain[2].Kind == "provider" && fact.TraceChain[2].Name == "sqlite" && fact.TraceChain[3].Kind == "sql",
                    "privacy SQL keeps operation root and physical tail");
            }
            Verify.Equal(2, sink.Audit.Count, "privacy retains two committed audit events");
            foreach (var fact in sink.Audit)
            {
                Verify.Equal(safeReason, fact["reason"], "safe audit keeps root reason");
                Chain((IEnumerable<TraceNode>)fact["traceChain"]!, safeReason, "safe privacy audit");
            }
            Console.WriteLine("PRIVATE_LINEAGE_OBSERVED " + JsonSerializer.Serialize(new {
                rootId, rawReason = reason, safeReason, commands = capture.Commands, sql = sink.Sql, audit = sink.Audit }));
            Console.WriteLine("PASS .NET complete private lineage: raw commands, safe SQL/readback and committed audit");
        }
        for (var round = 0; round < 2; round++)
        {
            var nextValue = "PRIVATE-CHANGED-" + alphabetic + "-" + (char)('A' + round);
            loaded.UpdateDescription("revision " + round);
            loadedChild.UpdateName(nextValue);
            capture.Clear(); sink.Clear();
            var reason = "page 1 replace " + oldValue + " with " + nextValue;
            await loaded.AuditAs(reason).SaveAsync(context);
            AssertPrivateChain(reason, oldValue, nextValue);
            Verify.Equal(2, capture.Commands.Count, "parent and child saved");
            Verify.Equal(4, sink.Sql.Count, "parent and child write/readback evidence");
            Verify.Equal(2, sink.Audit.Count, "committed parent and child audits");
            foreach (var value in new[] { oldValue, nextValue })
            {
                Verify.That(!JsonSerializer.Serialize(sink.Sql).Contains(value), "loaded old/new values masked in SQL evidence");
                Verify.That(!JsonSerializer.Serialize(sink.Audit).Contains(value), "loaded old/new values masked in audit evidence");
            }
            Verify.That(sink.Sql.All(fact => fact.AuditReason!.Contains("page 1")), "public number survives SQL scrubbing");
            Verify.That(sink.Audit.All(fact => fact["reason"]!.ToString()!.Contains("page 1")), "public number survives audit scrubbing");
            var stored = await Q.OrderItems().WithIdIs(childId).Limit(1).Comment("verify updated private value")
                .Purpose("prove privacy projection never changes storage").ExecuteForOneAsync(context)
                ?? throw new Exception("missing updated child");
            Verify.Equal(nextValue, E.OrderItem(stored).Name().Eval(), "persisted business value unchanged by masking");
            oldValue = nextValue; // The same wrapper must now use the last committed snapshot.
        }
        loaded.UpdateDescription("remove child");
        loadedChild.MarkForDeletion();
        capture.Clear(); sink.Clear();
        await loaded.AuditAs("remove " + oldValue).SaveAsync(context);
        AssertPrivateChain("remove " + oldValue, oldValue);
        Verify.Equal(2, capture.Commands.Count, "delete remains part of graph save");
        Verify.Equal(2, sink.Audit.Count, "delete and parent update audited");
        Verify.That(!JsonSerializer.Serialize(sink.Sql).Contains(oldValue), "delete has old provenance without field payload");
        Verify.That(!JsonSerializer.Serialize(sink.Audit).Contains(oldValue), "delete audit masks loaded old value");
        var deleted = await Q.OrderItems().WithIdIs(childId).Limit(1).Comment("verify deletion")
            .Purpose("normal query must hide deleted child").ExecuteForOneAsync(context);
        Verify.That(deleted is null, "normal query excludes deleted child");
        sink.Clear();
        _ = await Q.CustomerOrders().WithIdIs(rootId).Limit(1).Comment(oldValue)
            .Purpose("independent query must not inherit snapshot redaction").ExecuteForOneAsync(context);
        Verify.Equal(oldValue, sink.Sql.Single().Comment, "privacy does not escape the graph");
        Console.WriteLine("PASS: .NET loaded scalar privacy, snapshot refresh, deletion and independent request");
    }
}
