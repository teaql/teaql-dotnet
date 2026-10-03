using System.Reflection;
using System.Text.Json;
using Generated;
using Generated.Models;
using TeaQL.Core;
using TeaQL.Runtime;

// Current list-page/field Assist supplies Q/E API names. No generated source discovery.
static class PageChecks
{
    private sealed class Scope(long excluded) : IRequestPolicy
    {
        public int RootCalls;
        public SelectQuery Apply(SelectQuery query)
        {
            if (query.Entity == "CustomerOrder") { RootCalls++; query.AndFilter(Expr.Ne("id", excluded)); }
            return query;
        }
    }
    private sealed class NoScope : IRequestPolicy { public SelectQuery Apply(SelectQuery query) => query; }
    private static EntityRoot Ledger(object entity) => (EntityRoot)(entity.GetType()
        .GetField("_entityRoot", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(entity)
        ?? throw new Exception("missing runtime ledger"));

    public static async Task RunAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink)
    {
        var prefix = "page-" + Guid.NewGuid().ToString("N");
        const string secret = "PAGE-CHILD-PRIVATE-CANARY";
        var originals = new List<CustomerOrder>();
        for (var i = 0; i < 4; i++)
        {
            var root = Q.CustomerOrders().Comment("prepare page graph").Purpose("verify page ownership").NewEntity(context)
                .UpdatePlatformId(1).UpdateOrderNumber(prefix + "-" + i).UpdateDescription("original");
            root.OrderItemList.Add(Q.OrderItems().Comment("prepare page child").Purpose("verify child ownership")
                .NewEntity(context).UpdateName(secret));
            await root.AuditAs("seed page graph").SaveAsync(context);
            originals.Add(root);
        }
        var policy = new Scope(originals[3].Id!.Value);
        var previous = context.GetResource<IRequestPolicy>();
        List<CustomerOrder> rows;
        sink.Clear(); capture.Clear();
        try
        {
            context.WithRequestPolicy(policy);
            var page = await Q.CustomerOrders().WithOrderNumberStartingWith(prefix)
                .SelectOrderItemListWith(Q.OrderItems().WithNameIs(secret).OrderByIdAscending().Limit(10))
                .OrderByIdAscending().Comment("load page " + secret).Purpose("inspect page " + secret)
                .ExecuteForPageAsync(context, 1, 2);
            Verify.Equal(3L, page.TotalCount, "count excludes policy-denied root and ignores page offset");
            Verify.Equal(1, policy.RootCalls, "count and rows reuse one prepared scope");
            rows = page.Rows.ToList();
            Verify.Equal(2, rows.Count, "bounded page size");
            Verify.Equal(originals[1].Id, E.CustomerOrder(rows[0]).Id().Eval(), "stable page first ID");
            Verify.Equal(originals[2].Id, E.CustomerOrder(rows[1]).Id().Eval(), "stable page second ID");
            Verify.Equal(4, sink.Sql.Count, "root, two child probes, one COUNT; no diagnostic-only execution");
            Verify.That(!JsonSerializer.Serialize(sink.Sql).Contains(secret), "all physical SQL and prose mask child value");
            var count = sink.Sql.Single(entry => entry.DebugQuery?.Contains("COUNT(", StringComparison.OrdinalIgnoreCase) == true);
            Verify.That(count.TraceChain.All(node => node.Kind != "relation"), "COUNT adds no fictional relation path");
            Verify.That(count.TraceChain.Select(node => node.Name).SequenceEqual(new[] {"CustomerOrder", "CustomerOrder", "sqlite", "select"}), "COUNT canonical path");
            Console.WriteLine("PAGE QUERY EVIDENCE " + JsonSerializer.Serialize(new {page.TotalCount, ids=rows.Select(row=>row.Id), sql=sink.Sql}));
        }
        finally { context.WithRequestPolicy(previous ?? new NoScope()); }

        Verify.That(!ReferenceEquals(Ledger(rows[0]), Ledger(rows[1])), "page roots own separate ledgers");
        foreach (var row in rows)
        {
            Verify.Equal(1, E.CustomerOrder(row).OrderItemList().Size().Eval(), "selected child loaded");
            Verify.That(ReferenceEquals(Ledger(row), Ledger(row.OrderItemList.Single())), "child belongs to its own root");
        }
        rows[0].UpdateDescription("first saved"); rows[0].OrderItemList.Single().UpdateName("first child saved");
        rows[1].UpdateDescription("second pending"); rows[1].OrderItemList.Single().UpdateName("second child pending");
        capture.Clear(); sink.Clear();
        await rows[0].AuditAs("save first page graph").SaveAsync(context);
        Verify.Equal(2, capture.Commands.Count, "only first root and its child written");
        Verify.That(!Ledger(rows[1]).IsEmpty, "saving first graph preserves second pending mutations");
        var secondBefore = await Q.CustomerOrders().WithIdIs(rows[1].Id!.Value).Limit(1)
            .Comment("check second graph before its save").Purpose("prove no sibling writes").ExecuteForOneAsync(context)
            ?? throw new Exception("second root missing");
        Verify.Equal("original", E.CustomerOrder(secondBefore).Description().Eval(), "second root persisted value unchanged");
        Verify.Equal(1L, E.CustomerOrder(secondBefore).Version().Eval(), "second root version unchanged");
        capture.Clear(); sink.Clear();
        await rows[1].AuditAs("save second page graph").SaveAsync(context);
        Verify.Equal(2, capture.Commands.Count, "second root and child save independently");
        var loaded = await Q.CustomerOrders().WithOrderNumberStartingWith(prefix)
            .SelectOrderItemListWith(Q.OrderItems().OrderByIdAscending().Limit(10))
            .OrderByIdAscending().Limit(10).Comment("reload page test graphs").Purpose("verify persisted independent saves")
            .ExecuteForListAsync(context);
        Verify.Equal(4, loaded.Count, "all retained fixture roots present");
        Verify.That(!ReferenceEquals(Ledger(loaded[1]), Ledger(loaded[2])), "list roots also own independent ledgers");
        Verify.Equal("first saved", E.CustomerOrder(loaded[1]).Description().Eval(), "first persisted");
        Verify.Equal("second pending", E.CustomerOrder(loaded[2]).Description().Eval(), "second persisted");
        Verify.Equal("first child saved", E.OrderItem(loaded[1].OrderItemList.Single()).Name().Eval(), "first child persisted");
        Verify.Equal("second child pending", E.OrderItem(loaded[2].OrderItemList.Single()).Name().Eval(), "second child persisted");
        Verify.Equal(2L, E.CustomerOrder(loaded[1]).Version().Eval(), "first version");
        Verify.Equal(2L, E.CustomerOrder(loaded[2]).Version().Eval(), "second version");
        Console.WriteLine("PASS: .NET generated page count, scope, privacy and independent graph saves");
    }
}
