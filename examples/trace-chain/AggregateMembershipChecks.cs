using System.Text.Json;
using Generated;
using Generated.Models;
using TeaQL.Core;
using TeaQL.Runtime;
using TeaQL.Sql;

// Field Assist supplies selection/count APIs; generated source stays read-only.
static class AggregateMembershipChecks
{
    public static async Task RunAsync(UserContext context, CapturingExecutor capture,
        EvidenceSink sink, FaultTransport transport)
    {
        const string secret = "DOTNET-PRIVATE-AGGREGATE-MEMBERSHIP";
        const string purpose = "verify aggregate membership and physical ancestry";
        var order = Q.CustomerOrders().Comment("prepare membership root").Purpose(purpose)
            .NewEntity(context).UpdatePlatformId(1).UpdateOrderNumber("MEM-" + Guid.NewGuid().ToString("N"))
            .UpdateDescription("complete membership root");
        foreach (var name in new[] { secret, "public membership item" })
            order.OrderItemList.Add(Q.OrderItems().Comment("prepare membership item").Purpose(purpose)
                .NewEntity(context).UpdateName(name));
        var payment = Q.Payments().Comment("prepare membership outer root").Purpose(purpose)
            .NewEntity(context).UpdateReferenceCode("MEM-PAY-" + Guid.NewGuid().ToString("N"));
        order.PaymentList.Add(payment);
        await order.AuditAs("seed membership graph").SaveAsync(context);
        var id = E.CustomerOrder(order).Id().Eval()!.Value;
        var paymentId = E.Payment(payment).Id().Eval()!.Value;
        var itemIds = order.OrderItemList.Select(item => E.OrderItem(item).Id().Eval()!.Value).Order().ToArray();

        void Clear() { capture.Clear(); sink.Clear(); transport.PhysicalStatements.Clear(); transport.QueryReads.Clear(); }
        foreach (var nested in new[] { false, true })
        foreach (var logging in new[] { false, true })
        foreach (var filtered in new[] { false, true })
        {
            context.EnableQuerySqlLog(logging).EnableMutationSqlLog(logging);
            var forward = Q.CustomerOrders().WithIdIs(filtered ? -1 : id).Limit(1);
            var selected = Q.CustomerOrders().WithIdIs(id).Limit(1)
                .CountOrderItemsWith("member_count", Q.OrderItems().WithNameIs(secret).Limit(10))
                .SelectOrderItemListWith(Q.OrderItems().OrderByIdAscending().Limit(10).SelectCustomerOrderWith(forward));
            Clear();
            CustomerOrder loaded;
            if (nested)
            {
                var outer = (await Q.Payments().WithIdIs(paymentId).Limit(1).SelectCustomerOrderWith(selected)
                    .Comment("inspect " + secret).Purpose(purpose).ExecuteForListAsync(context)).Single();
                loaded = E.Payment(outer).CustomerOrder().Eval() ?? throw new Exception("outer parent is missing");
            }
            else loaded = (await selected.Comment("inspect " + secret).Purpose(purpose)
                .ExecuteForListAsync(context)).Single();
            Verify.That(loaded.HasQueryProjection("member_count"), "count alias is present");
            Verify.Equal(1L, loaded.QueryProjection("member_count").TryI64(), "count is independent of selected forward detail");
            await Verify.Throws<KeyNotFoundException>(() => { loaded.QueryProjection("missing"); return Task.CompletedTask; }, "missing alias is not zero");
            Verify.Equal(2, E.CustomerOrder(loaded).OrderItemList().Size().Eval(), "loaded child list size");
            var items = loaded.OrderItemList.OrderBy(item => E.OrderItem(item).Id().Eval()).ToArray();
            Verify.That(items.Select(item => E.OrderItem(item).Id().Eval()!.Value).SequenceEqual(itemIds), "loaded membership retains both actual children");
            var refs = new List<CustomerOrder>();
            foreach (var item in items)
            {
                Verify.Equal(id, E.OrderItem(item).CustomerOrderId().Eval(), "actual FK survives forward filtering");
                var parent = E.OrderItem(item).CustomerOrder().Eval() ?? throw new Exception("non-null FK became loaded null");
                refs.Add(parent);
                Verify.Equal(id, E.CustomerOrder(parent).Id().Eval(), "identity-only forward target retains ID");
                if (filtered)
                    await Verify.Throws<TeaQLNotLoadedException>(() => { E.CustomerOrder(parent).Description().Eval(); return Task.CompletedTask; }, "filtered detail is NotLoaded");
                else Verify.Equal("complete membership root", E.CustomerOrder(parent).Description().Eval(), "selected forward detail is loaded");
            }
            var physical = transport.PhysicalStatements.ToArray();
            Verify.Equal(nested ? 5 : 4, physical.Length, "root, count, child and forward physical reads");
            Verify.That(physical.All(row => row.IsRead), "read-only query");
            Verify.Equal(0, capture.Commands.Count, "query emits no mutation command");
            Verify.Equal(0, sink.Audit.Count, "query emits no committed audit");
            Verify.Equal(logging ? physical.Length : 0, sink.Sql.Count, "physical diagnostics honor logging");
            var routes = nested ? new[] { "", "CustomerOrder", "CustomerOrder/OrderItemList", "CustomerOrder/OrderItemList", "CustomerOrder/OrderItemList/CustomerOrder" }
                : new[] { "", "OrderItemList", "OrderItemList", "OrderItemList/CustomerOrder" };
            for (var index = 0; index < sink.Sql.Count; index++)
            {
                var sql = sink.Sql[index];
                Verify.Equal(physical[index].Sql, sql.ParameterizedQuery, "metadata matches actual SQL statement");
                var root = nested ? "Payment" : "CustomerOrder";
                var expected = new List<(string, string, string)> { ("operation", root, "query"), ("request", root, "") };
                var owner = root;
                foreach (var relation in routes[index].Split('/', StringSplitOptions.RemoveEmptyEntries))
                {
                    expected.Add(("relation", relation, owner + "." + relation));
                    owner = relation == "OrderItemList" ? "OrderItem" : "CustomerOrder";
                }
                expected.Add(("provider", "sqlite", "")); expected.Add(("sql", "select", ""));
                Verify.That(sql.TraceChain.Select(node => (node.Kind, node.Name, node.Detail)).SequenceEqual(expected), "complete physical relation ancestry");
                Verify.Equal("inspect [REDACTED]", sql.Comment, "future private binding redacts parent intent");
                Verify.Equal(purpose, sql.Purpose, "original root purpose");
                Verify.Equal("success", sql.ExecutionOutcome, "actual SQL succeeded");
                Verify.That(!JsonSerializer.Serialize(sql).Contains(secret), "safe diagnostics never expose private binding");
            }
            var actualCount = physical.Single(row => row.Sql.Contains("COUNT(", StringComparison.OrdinalIgnoreCase));
            Verify.That(actualCount.Parameters.Any(value => value.TryText() == secret), "masking does not change actual count operand");
            var safe = sink.Sql;
            var commands = capture.Commands.Count;
            var audits = sink.Audit.Count;
            string Detail(CustomerOrder parent)
            {
                try { return E.CustomerOrder(parent).Description().Eval(); }
                catch (TeaQLNotLoadedException) { return "NotLoaded"; } // Test observation, never a business fallback.
            }
            var detailStates = refs.Select(Detail).ToArray();
            // A separate full read must not retroactively widen either filtered relation view.
            var complete = await Q.CustomerOrders().WithIdIs(id).Limit(1).Comment("independent full membership root")
                .Purpose(purpose).ExecuteForOneAsync(context) ?? throw new Exception("independent root is missing");
            Verify.Equal("complete membership root", E.CustomerOrder(complete).Description().Eval(), "independent full read");
            if (filtered)
                foreach (var parent in refs)
                    await Verify.Throws<TeaQLNotLoadedException>(() => { E.CustomerOrder(parent).Description().Eval(); return Task.CompletedTask; }, "independent read does not widen old projection");
            Console.WriteLine("DOTNET_AGGREGATE_MEMBERSHIP " + JsonSerializer.Serialize(new {
                nested, logging, filtered, id, itemIds, count = loaded.QueryProjection("member_count").TryI64(),
                foreignIDs = items.Select(item => E.OrderItem(item).CustomerOrderId().Eval()), physical, safe,
                detailStates, independentDetail = E.CustomerOrder(complete).Description().Eval(),
                oldDetailStates = refs.Select(Detail), commands, audits
            }));
        }
        context.EnableQuerySqlLog().EnableMutationSqlLog();
        Console.WriteLine("PASS .NET generated aggregate membership: 8 list scenarios; actual SQL, complete safe ancestry, FK and independent filtered views");
    }
}
