using System.Text.Json;
using Generated;
using TeaQL.Runtime;

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
        for (var round = 0; round < 2; round++)
        {
            var nextValue = "PRIVATE-CHANGED-" + alphabetic + "-" + (char)('A' + round);
            loaded.UpdateDescription("revision " + round);
            loadedChild.UpdateName(nextValue);
            capture.Clear(); sink.Clear();
            await loaded.AuditAs("page 1 replace " + oldValue + " with " + nextValue).SaveAsync(context);
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
