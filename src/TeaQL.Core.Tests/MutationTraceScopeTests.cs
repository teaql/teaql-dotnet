using Xunit;

namespace TeaQL.Core.Tests;

public class MutationTraceScopeTests
{
    [Fact]
    public void PersistentRecoveryIsIterativeAndKeepsSiblingParentsImmutable()
    {
        var root = new MutationTraceScope("CustomerOrder", 1, "submit order");
        var payment = new MutationTraceScope("Payment", 1, "authorize payment", root);
        var shipment = new MutationTraceScope("Shipment", 1, "dispatch shipment", root);
        Assert.Equal(new[] { "CustomerOrder", "Payment" }, payment.Recover().Select(node => node.Name));
        Assert.Equal(new[] { "CustomerOrder", "Shipment" }, shipment.Recover().Select(node => node.Name));
        Assert.Single(root.Recover());
        var deep = root;
        for (var index = 0; index < 2000; index++) deep = new("Nested", (ulong)index, "nested reason", deep);
        Assert.Equal(2001, deep.Recover().Count);
        Assert.Same(root.Node, deep.Recover()[0]);
    }

    [Fact]
    public void LedgerTraceOwnsSnapshotsAndUsesTypedKeysThroughMergeRekeyAndClear()
    {
        var ledger = new EntityRoot();
        var order = new EntityKey("CustomerOrder", 1);
        var payment = new EntityKey("Payment", 1);
        var nodes = new[] { new MutationTraceScope("CustomerOrder", 1, "root reason").Node };
        ledger.SetTraceChain(order, nodes);
        nodes[0] = nodes[0] with { Detail = "caller changed" };
        ledger.SetTraceChain(payment, new MutationTraceScope("Payment", 1, "payment reason").Recover());
        Assert.Equal("root reason", ledger.TraceChain(order)[0].Detail);
        Assert.Equal("payment reason", ledger.TraceChain(payment)[0].Detail);
        var merged = new EntityRoot();
        merged.MergeFrom(ledger);
        var assigned = new EntityKey("Payment", 601);
        merged.Rekey(payment, assigned);
        Assert.Empty(merged.TraceChain(payment));
        Assert.Single(merged.TraceChain(assigned));
        merged.ClearEntity(assigned);
        Assert.Empty(merged.TraceChain(assigned));
        merged.ClearCommitted();
        Assert.Empty(merged.TraceChain(order));
        Assert.Single(ledger.TraceChain(payment));
        Assert.True(ledger.IsEmpty); // Trace-only evidence does not create a pending write.
    }
}
