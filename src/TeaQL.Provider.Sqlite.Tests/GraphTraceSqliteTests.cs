using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

/// <summary>Native provider evidence; not a substitute for generated Q/E/Mutation acceptance.</summary>
public class GraphTraceSqliteTests
{
    private sealed class Sink : IDiagnosticSqlLogSink, IAppAuditEventSink
    {
        public readonly List<ExecutionMetadata> Sql = new();
        public readonly List<IReadOnlyDictionary<string, object?>> Audit = new();
        public void Write(ExecutionMetadata metadata) => Sql.Add(metadata);
        public Task RecordAsync(IReadOnlyDictionary<string, object?> item, CancellationToken token = default)
        { Audit.Add(item); return Task.CompletedTask; }
    }

    private static async Task<(SqliteConnection Db, UserContext Context, Sink Sink)> Fixture()
    {
        var db = new SqliteConnection("Data Source=:memory:"); await db.OpenAsync();
        var descriptors = new[] { "CustomerOrder", "OrderItem", "Payment", "PaymentAttempt", "Shipment" }
            .Select(name => EntityDescriptor.New(name).TableName(name.ToLowerInvariant() + "_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("version", DataType.I64).Version())
                .Property(PropertyDescriptor.New("name", DataType.Text))).ToArray();
        var module = new RuntimeModule(); foreach (var descriptor in descriptors) module.Entity(descriptor);
        var schema = new MetadataSchemaProvider(name => descriptors.FirstOrDefault(item => item.Name == name));
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(db), schema);
        var sink = new Sink();
        var context = module.IntoContext().WithDataService(provider)
            .WithDiagnosticSqlLogSink(sink).WithAppAuditEventSink(sink);
        await context.EnsureSchemaAsync();
        return (db, context, sink);
    }

    private static InsertMutationRequest Insert(string entity, ulong id, EntityRoot? ledger = null) =>
        new(new InsertCommand(entity).Value("id", new Value.U64Value(id))
            .Value("name", new Value.TextValue("ordinary display field")), "prepare graph item") {
            LedgerKey = new EntityKey(entity, new Value.U64Value(id)), LedgerRoot = ledger };

    private static string Shape(IEnumerable<TraceNode> nodes) => string.Join(" -> ",
        nodes.Select(node => $"{node.Name}#{node.EntityId}:{node.Detail}"));

    [Fact]
    public async Task RealAllocatedGraphKeepsBranchDeleteLedgerAndCommittedAuditLineage()
    {
        var fixture = await Fixture(); await using var db = fixture.Db;
        var context = fixture.Context; var sink = fixture.Sink;
        await context.RequireResource<IDataService>().MutateAsync(Insert("OrderItem", 202));
        sink.Audit.Clear(); sink.Sql.Clear();
        var expected = new Dictionary<(string, ulong), string>();
        await context.ExecuteGraphSaveAsync("submit order", async graph =>
        {
            var orderId = await graph.AllocateIdAsync("CustomerOrder");
            var root = graph.Scope("CustomerOrder", orderId, null);
            var rootShape = Shape(root.Recover());
            await Save("CustomerOrder", orderId, root);
            var itemId = await graph.AllocateIdAsync("OrderItem");
            await Save("OrderItem", itemId, graph.Scope("OrderItem", itemId, null, root));
            var paymentId = await graph.AllocateIdAsync("Payment");
            Assert.Equal(orderId, paymentId); // Distinct types legitimately share an ID.
            var payment = graph.Scope("Payment", paymentId, "authorize payment", root);
            var ledger = new EntityRoot();
            ledger.SetTraceChain(new EntityKey("Payment", new Value.U64Value(paymentId)),
                new MutationTraceScope("Payment", paymentId, "special approval", root).Recover());
            await Save("Payment", paymentId, payment, ledger);
            var attemptId = await graph.AllocateIdAsync("PaymentAttempt");
            await Save("PaymentAttempt", attemptId, graph.Scope("PaymentAttempt", attemptId, null, payment));
            var shipmentId = await graph.AllocateIdAsync("Shipment");
            await Save("Shipment", shipmentId, graph.Scope("Shipment", shipmentId, "dispatch shipment", root));
            var deleted = graph.Scope("OrderItem", 202, "remove unavailable item", root);
            var delete = new DeleteMutationRequest(new DeleteCommand("OrderItem", new Value.U64Value(202)) {
                Version = new Value.I64Value(1) }, "delete local item") {
                LedgerKey = new EntityKey("OrderItem", 202) };
            var result = await graph.MutateAsync(delete, deleted);
            expected[("OrderItem", 202)] = Shape(deleted.Recover());
            Assert.Equal(expected[("OrderItem", 202)], Shape(result.Metadata.MutationLineage));
            Assert.Empty(sink.Audit);
            Assert.Equal(rootShape, Shape(root.Recover()));
            return true;

            async Task Save(string entity, ulong id, MutationTraceScope scope, EntityRoot? ledger = null)
            {
                var request = Insert(entity, id, ledger);
                var result = await graph.MutateAsync(request, scope);
                var wanted = ledger?.TraceChain(request.LedgerKey!) is { Count: > 0 } complete
                    ? complete : scope.Recover();
                expected[(entity, id)] = Shape(wanted);
                Assert.Equal(expected[(entity, id)], Shape(result.Metadata.MutationLineage));
                Assert.Equal("CustomerOrder", result.Metadata.TraceChain[0].Name);
                Assert.All(result.Metadata.MutationLineage, node => Assert.NotNull(node.EntityId));
                Assert.Empty(sink.Audit);
            }
        });
        Assert.Equal(6, sink.Audit.Count);
        Assert.Equal(6, sink.Sql.Count(item => item.Operation != DataServiceOperation.Query));
        foreach (var audit in sink.Audit)
        {
            var key = (audit["entityType"]!.ToString()!, Convert.ToUInt64(audit["entityId"]));
            var chain = Assert.IsAssignableFrom<IEnumerable<TraceNode>>(audit["traceChain"]);
            Assert.Equal(expected[key], Shape(chain));
            Assert.Equal("submit order", audit["reason"]);
        }
    }

    [Fact]
    public async Task RealSQLiteRollbackDoesNotEmitAuditAndConcurrentGraphsRemainIndependent()
    {
        var fixture = await Fixture(); await using var db = fixture.Db;
        var context = fixture.Context; var sink = fixture.Sink;
        await Assert.ThrowsAsync<SqlExecutorException>(() => context.ExecuteGraphSaveAsync("rollback graph", async graph =>
        {
            var id = await graph.AllocateIdAsync("CustomerOrder");
            var scope = graph.Scope("CustomerOrder", id, null);
            await graph.MutateAsync(Insert("CustomerOrder", id), scope);
            await graph.MutateAsync(Insert("CustomerOrder", id), scope); // Actual UNIQUE failure.
            return true;
        }));
        Assert.Empty(sink.Audit);
        Assert.Contains(sink.Sql, item => item.ExecutionOutcome == "failure" && item.MutationLineage.Count == 1);
        var rows = await context.RequireResource<IDataService>().QueryAsync(new QueryRequest(
            new SelectQuery("CustomerOrder").Limit(10).Comment("verify rollback").Purpose("check atomic state")));
        Assert.Empty(rows.Rows);

        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondEntered = false;
        var first = Task.Run(() => context.ExecuteGraphSaveAsync("first concurrent graph", async graph =>
        {
            entered.SetResult(); await release.Task;
            return await Save(graph);
        }));
        await entered.Task;
        var second = Task.Run(async () =>
        {
            secondStarted.SetResult();
            return await context.ExecuteGraphSaveAsync("second concurrent graph", async graph =>
            { secondEntered = true; return await Save(graph); });
        });
        await secondStarted.Task; Assert.False(secondEntered); release.SetResult();
        await Task.WhenAll(first, second);
        Assert.Equal(2, sink.Audit.Count);
        var reasons = sink.Audit.Select(audit => audit["reason"]).ToArray();
        Assert.Equal(new[] { "first concurrent graph", "second concurrent graph" }, reasons);
        foreach (var audit in sink.Audit)
            Assert.Equal(audit["reason"], Assert.Single((IEnumerable<TraceNode>)audit["traceChain"]!).Detail);

        async Task<MutationResult> Save(GraphMutationSession graph)
        {
            var id = await graph.AllocateIdAsync("CustomerOrder");
            return await graph.MutateAsync(Insert("CustomerOrder", id), graph.Scope("CustomerOrder", id, null));
        }
    }
}
