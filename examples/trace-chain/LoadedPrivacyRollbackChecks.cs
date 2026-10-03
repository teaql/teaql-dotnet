using System.Text.Json;
using Generated;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;

static class LoadedPrivacyRollbackChecks
{
    public static async Task RunAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink, FaultTransport faults)
    {
        var nonce = Guid.NewGuid().ToString("N");
        // Alphabetic canaries cannot disappear accidentally when numeric target
        // IDs are redacted. The nonce still distinguishes retained-DB starts.
        var alphabetic = string.Concat(nonce.Select(c => (char)('A' + Convert.ToInt32(c.ToString(), 16))));
        var oldValue = "PRIVATE-ROLLBACK-OLD-" + alphabetic;
        var nextValue = "PRIVATE-ROLLBACK-NEW-" + alphabetic;
        var root = Q.CustomerOrders().Comment("prepare rollback privacy graph").Purpose("exercise generated rollback and retry")
            .NewEntity(context).UpdatePlatformId(1).UpdateOrderNumber("rollback-" + nonce).UpdateDescription("before retry");
        var child = Q.OrderItems().Comment("prepare rollback private child").Purpose("persist an original private value")
            .NewEntity(context).UpdateName(oldValue);
        root.OrderItemList.Add(child);
        await root.AuditAs("seed rollback privacy graph").SaveAsync(context);
        var rootId = E.CustomerOrder(root).Id().Eval()!.Value;
        var childId = E.OrderItem(child).Id().Eval()!.Value;
        var loaded = await Q.CustomerOrders().WithIdIs(rootId)
            .SelectOrderItemListWith(Q.OrderItems().Limit(2)).Limit(1)
            .Comment("load rollback privacy graph").Purpose("retain private originals and optimistic versions")
            .ExecuteForOneAsync(context) ?? throw new Exception("missing loaded parent");
        var loadedChild = loaded.OrderItemList.Single();
        loaded.UpdateDescription("after retry");
        loadedChild.UpdateName(nextValue);
        var reason = "review replace " + oldValue + " with " + nextValue;
        capture.Clear(); sink.Clear();
        faults.FailItemReadback = true;
        try
        {
            await Verify.Throws<SqlExecutorException>(() => loaded.AuditAs(reason).SaveAsync(context), "private child readback failure");
        }
        finally { faults.FailItemReadback = false; }
        Verify.Equal(2, capture.Commands.Count, "both writes attempted before readback failure");
        Verify.That(capture.Commands.All(command => command.Comment == reason), "trusted comments are unchanged");
        Verify.Equal(2, sink.Sql.Count(fact => fact.Operation == DataServiceOperation.Update && fact.ExecutionOutcome == "success"),
            "both physical writes succeeded before rollback");
        var failedRead = sink.Sql.Single(fact => fact.Operation == DataServiceOperation.Query && fact.ExecutionOutcome == "failure");
        Verify.Equal("CustomerOrder", failedRead.TraceChain[0].Name, "failed child readback retains request root");
        Verify.Equal("select", failedRead.TraceChain.Last().Name, "failed readback remains a SELECT");
        Verify.That(failedRead.AuditReason!.Contains("review replace"), "failure retains public business intent");
        Verify.Equal(0, sink.Audit.Count, "no committed audit on rollback");
        foreach (var value in new[] { oldValue, nextValue })
            Verify.That(!JsonSerializer.Serialize(sink.Sql).Contains(value), "rollback SQL masks loaded old/new values");
        Verify.Equal(1L, E.CustomerOrder(loaded).Version().Eval(), "rollback restores root wrapper version");
        Verify.Equal(1L, E.OrderItem(loadedChild).Version().Eval(), "rollback restores child wrapper version");
        var storedRoot = await Q.CustomerOrders().WithIdIs(rootId).Limit(1)
            .Comment("check rolled-back root").Purpose("verify authoritative storage after failure")
            .ExecuteForOneAsync(context) ?? throw new Exception("missing persisted root");
        var storedChild = await Q.OrderItems().WithIdIs(childId).Limit(1)
            .Comment("check rolled-back child").Purpose("verify original private scalar survived")
            .ExecuteForOneAsync(context) ?? throw new Exception("missing persisted child");
        Verify.Equal("before retry", E.CustomerOrder(storedRoot).Description().Eval(), "parent value rolled back");
        Verify.Equal(oldValue, E.OrderItem(storedChild).Name().Eval(), "child value rolled back");
        Verify.Equal(1L, E.CustomerOrder(storedRoot).Version().Eval(), "stored root version rolled back");
        Verify.Equal(1L, E.OrderItem(storedChild).Version().Eval(), "stored child version rolled back");

        // .NET saves mutate the wrapper in place. Retry that same graph, keeping
        // its pending values and original private snapshot, without reloading it.
        capture.Clear(); sink.Clear();
        await loaded.AuditAs(reason).SaveAsync(context);
        Verify.Equal(2, capture.Commands.Count, "retry preserves both pending changes");
        Verify.Equal(2, sink.Audit.Count, "only committed retry emits audit");
        foreach (var value in new[] { oldValue, nextValue })
        {
            Verify.That(!JsonSerializer.Serialize(sink.Sql).Contains(value), "retry SQL masks original and pending values");
            Verify.That(!JsonSerializer.Serialize(sink.Audit).Contains(value), "retry audit masks original and pending values");
        }
        Verify.Equal(2L, E.CustomerOrder(loaded).Version().Eval(), "retry increments root once");
        Verify.Equal(2L, E.OrderItem(loadedChild).Version().Eval(), "retry increments child once");
        storedRoot = await Q.CustomerOrders().WithIdIs(rootId).Limit(1)
            .Comment("verify retried root").Purpose("verify committed pending value")
            .ExecuteForOneAsync(context) ?? throw new Exception("missing retried root");
        storedChild = await Q.OrderItems().WithIdIs(childId).Limit(1)
            .Comment("verify retried child").Purpose("verify committed private value")
            .ExecuteForOneAsync(context) ?? throw new Exception("missing retried child");
        Verify.Equal("after retry", E.CustomerOrder(storedRoot).Description().Eval(), "retry persisted parent change");
        Verify.Equal(nextValue, E.OrderItem(storedChild).Name().Eval(), "retry persisted child change");
        Verify.Equal(2L, E.CustomerOrder(storedRoot).Version().Eval(), "persisted root incremented once");
        Verify.Equal(2L, E.OrderItem(storedChild).Version().Eval(), "persisted child incremented once");
        sink.Clear();
        _ = await Q.CustomerOrders().WithIdIs(rootId).Limit(1).Comment(oldValue)
            .Purpose("independent request after rollback and retry").ExecuteForOneAsync(context);
        Verify.Equal(oldValue, sink.Sql.Single().Comment, "rollback privacy does not leak into Context");
        Console.WriteLine("PASS: .NET loaded graph privacy rollback and same-wrapper retry");
    }
}
