using System.Text.Json;
using Generated;
using Generated.Models;
using TeaQL.Core;
using TeaQL.Runtime;
using TeaQL.Sql;

// APIs discovered through current entity/field Assist, not generated sources.
static class AggregateChecks
{
    public static async Task RunAsync(UserContext context, CapturingExecutor capture, EvidenceSink sink, FaultTransport transport)
    {
        var secret = "PRIVATE-AGGREGATE-" + Guid.NewGuid().ToString("N");
        var order = Q.CustomerOrders().Comment("prepare aggregate root").Purpose("verify generated counts")
            .NewEntity(context).UpdatePlatformId(1).UpdateOrderNumber("AGG-" + Guid.NewGuid().ToString("N"))
            .UpdateDescription("original aggregate root");
        foreach (var name in new[] { secret, "public item" })
            order.OrderItemList.Add(Q.OrderItems().Comment("prepare aggregate item").Purpose("verify generated counts")
                .NewEntity(context).UpdateName(name));
        var payment = Q.Payments().Comment("prepare payment").Purpose("verify nested count")
            .NewEntity(context).UpdateReferenceCode("AGG-PAY-" + Guid.NewGuid().ToString("N"));
        order.PaymentList.Add(payment);
        await order.AuditAs("seed aggregate graph").SaveAsync(context);
        var orderId = E.CustomerOrder(order).Id().Eval()!.Value;
        var paymentId = E.Payment(payment).Id().Eval()!.Value;

        async Task<CustomerOrder> Load(bool nested, string filter)
        {
            var counts = Q.CustomerOrders().WithIdIs(orderId).Limit(1)
                .CountOrderItemsWith("filtered_count", Q.OrderItems().WithNameIs(filter).Limit(10))
                .CountOrderItemsAs("SaveAsync")
                .SelectOrderItemListWith(Q.OrderItems().Limit(10));
            if (!nested)
                return (await counts.Comment("inspect " + secret).Purpose("verify generated aggregate ancestry")
                    .ExecuteForListAsync(context)).Single();
            var loaded = (await Q.Payments().WithIdIs(paymentId).Limit(1).SelectCustomerOrderWith(counts)
                .Comment("inspect " + secret).Purpose("verify generated aggregate ancestry")
                .ExecuteForListAsync(context)).Single();
            return E.Payment(loaded).CustomerOrder().Eval() ?? throw new Exception("loaded order reference is null");
        }
        void Clear() { sink.Clear(); capture.Clear(); transport.QueryReads.Clear(); }

        foreach (var logging in new[] { true, false })
        {
            context.EnableQuerySqlLog(logging).EnableMutationSqlLog(logging);
            foreach (var nested in new[] { false, true })
            {
                Clear();
                var loaded = await Load(nested, secret);
                Verify.Equal(orderId, E.CustomerOrder(loaded).Id().Eval(), "aggregate identity");
                Verify.Equal(2, E.CustomerOrder(loaded).OrderItemList().Size().Eval(), "loaded items");
                Verify.That(loaded.HasQueryProjection("filtered_count"), "projection is present");
                Verify.Equal(1L, loaded.QueryProjection("filtered_count").TryI64(), "filtered count");
                Verify.Equal(2L, loaded.QueryProjection("SaveAsync").TryI64(), "total count alias");
                Verify.That(!loaded.HasQueryProjection("id") && !loaded.HasQueryProjection("OrderItemList"), "modeled values excluded");
                await Verify.Throws<KeyNotFoundException>(() => { loaded.QueryProjection("missing"); return Task.CompletedTask; }, "missing is not zero");
                Verify.Equal(nested ? 5 : 4, transport.QueryReads.Count, "actual physical aggregate reads");
                Verify.Equal(0, capture.Commands.Count, "query emits no mutation");
                Verify.Equal(0, sink.Audit.Count, "query emits no mutation audit");
                if (logging)
                {
                    Verify.Equal(transport.QueryReads.Count, sink.Sql.Count, "one diagnostic per physical SQL");
                    Verify.That(!JsonSerializer.Serialize(sink.Sql).Contains(secret), "future private binding masks all diagnostics");
                    Verify.That(sink.Sql.All(entry => entry.TraceChain[0].Name == (nested ? "Payment" : "CustomerOrder")
                        && entry.Purpose == "verify generated aggregate ancestry"), "original request root and purpose");
                    var aggregates = sink.Sql.Where(entry => entry.ParameterizedQuery?.Contains("COUNT(") == true).ToArray();
                    Verify.Equal(2, aggregates.Length, "two count statements");
                    foreach (var aggregate in aggregates)
                        Verify.That(aggregate.TraceChain.Where(node => node.Kind == "relation").Select(node => node.Name)
                            .SequenceEqual(nested ? new[] { "CustomerOrder", "OrderItemList" } : new[] { "OrderItemList" }), "complete aggregate relation path");
                }
                else Verify.Equal(0, sink.Sql.Count, "logging disabled, valid query still executes");
                Console.WriteLine("AGGREGATE QUERY " + JsonSerializer.Serialize(new { nested, logging,
                    count = loaded.QueryProjection("filtered_count").TryI64(), reads = transport.QueryReads.Count, sql = sink.Sql }));
                Clear();
                var updated = "saved aggregate " + Guid.NewGuid().ToString("N");
                await loaded.UpdateDescription(updated).AuditAs("save modeled field without aliases").SaveAsync(context);
                Verify.Equal(1, capture.Commands.Count, "only root mutation");
                Verify.Equal(1, sink.Audit.Count, "one committed audit");
                Verify.That(capture.ChangedFields.Single().SequenceEqual(new[] { "description" }), "query aliases not persisted");
                Verify.Equal(2L, loaded.QueryProjection("SaveAsync").TryI64(), "save method remains callable and snapshot intact");
                Console.WriteLine("AGGREGATE SAVE " + JsonSerializer.Serialize(new { nested, logging,
                    writes = capture.Commands.Count, audits = sink.Audit.Count, fields = capture.ChangedFields.Single() }));
                var fresh = await Load(nested, "no match " + Guid.NewGuid().ToString("N"));
                Verify.Equal(updated, E.CustomerOrder(fresh).Description().Eval(), "saved description reloads");
                Verify.That(fresh.HasQueryProjection("filtered_count"), "zero count is present");
                Verify.Equal(0L, fresh.QueryProjection("filtered_count").TryI64(), "empty aggregate is zero");
            }
            Clear(); transport.FailAggregateQuery = true;
            try { await Verify.Throws<SqlExecutorException>(async () => { await Load(false, secret); }, "count failure propagates"); }
            finally { transport.FailAggregateQuery = false; }
            Verify.Equal(2, transport.QueryReads.Count, "root then failed count");
            Verify.Equal(0, sink.Audit.Count, "failed query emits no committed audit");
            if (logging) Verify.Equal("failure", sink.Sql.Last().ExecutionOutcome, "failed count diagnostic");
            else Verify.Equal(0, sink.Sql.Count, "failed query respects disabled diagnostics");
            Verify.That(!JsonSerializer.Serialize(sink.Sql).Contains(secret), "failure is safely projected");
            Console.WriteLine("AGGREGATE FAILURE " + JsonSerializer.Serialize(new { logging, reads = transport.QueryReads.Count, sql = sink.Sql }));
            Clear();
            await Q.Platforms().WithIdIs(1).Limit(1).Comment("independent next query").Purpose("verify restored scope").ExecuteForListAsync(context);
            Verify.Equal(1, transport.QueryReads.Count, "independent query");
            if (logging) Verify.Equal("independent next query", sink.Sql.Single().Comment, "restored intent");
        }
        context.EnableQuerySqlLog().EnableMutationSqlLog();
        Console.WriteLine("PASS: .NET generated aggregates 4 cases, isolated saves and 2 failure recoveries");
    }
}
