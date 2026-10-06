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

    private sealed class AuditProbe : IEntity
    {
        public static string EntityName => "PaymentAttempt";
        public static EntityDescriptor EntityDescriptor() => TeaQL.Core.EntityDescriptor.New(EntityName);
        public TeaQL.Core.Record IntoRecord() => new();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BlankLocalReasonsUseRuntimeParentFallbackWithoutLeakingSiblingReasons(bool logging)
    {
        var fixture = await Fixture(); await using var db = fixture.Db;
        var context = fixture.Context; var sink = fixture.Sink;
        context.EnableQuerySqlLog(logging).EnableMutationSqlLog(logging);
        var blanks = new string?[] { null, "", " \t\r\n", "\u0085", "\u00a0", "\u2003" };
        const string paymentLineage = "CustomerOrder#100:submit order -> Payment#201:authorize payment";
        const string shipmentLineage = "CustomerOrder#100:submit order -> Shipment#301:dispatch shipment";
        await context.ExecuteGraphSaveAsync("submit order", async graph =>
        {
            var root = graph.Scope("CustomerOrder", 100, null);
            var payment = graph.Scope("Payment", 201, "authorize payment", root);
            var shipment = graph.Scope("Shipment", 301, "dispatch shipment", root);
            for (var index = 0; index < blanks.Length; index++)
            {
                var blank = blanks[index];
                var intentError = Assert.Throws<RequestIntentException>(() => new MutationIntent(blank));
                Assert.Equal("REQUEST_COMMENT_REQUIRED", intentError.Code);
                Assert.Equal("comment", intentError.Field);
                Assert.Equal("mutation", intentError.RequestKind);
                Assert.Equal("comment", Assert.Throws<ArgumentException>(() =>
                    new Audited<AuditProbe>(new AuditProbe(), blank!)).ParamName);

                // Feed the actual invalid local reason to the runtime adapter.
                // No test-side validation, coalescing or fallback substitution.
                var inherited = graph.Scope("PaymentAttempt", (ulong)(400 + index), blank, payment);
                Assert.Same(payment, inherited);
                var result = await graph.MutateAsync(Insert("PaymentAttempt", (ulong)(400 + index)), inherited);
                Assert.Equal(paymentLineage, Shape(result.Metadata.MutationLineage));
                Assert.Equal(2, result.Metadata.Statements.Count);
                Assert.All(result.Metadata.Statements, statement => {
                    Assert.Equal(paymentLineage, Shape(statement.MutationLineage));
                    Assert.Equal("submit order", statement.AuditReason);
                });
                Assert.Empty(sink.Audit);
            }
            var sibling = await graph.MutateAsync(Insert("Shipment", 301), shipment);
            Assert.Equal(shipmentLineage, Shape(sibling.Metadata.MutationLineage));
            Assert.Equal(paymentLineage, Shape(payment.Recover()));
            Assert.Equal("CustomerOrder#100:submit order", Shape(root.Recover()));
            return true;
        });
        Assert.Equal(blanks.Length + 1, sink.Audit.Count);
        Assert.All(sink.Audit, audit => {
            Assert.Equal("submit order", audit["reason"]);
            var nodes = Assert.IsAssignableFrom<IEnumerable<TraceNode>>(audit["traceChain"]);
            Assert.Equal(audit["entityType"]!.ToString() == "Shipment" ? shipmentLineage : paymentLineage,
                Shape(nodes));
        });
        Assert.Equal(logging ? 2 * (blanks.Length + 1) : 0, sink.Sql.Count);
        Assert.All(sink.Sql, statement => {
            Assert.Equal("submit order", statement.AuditReason);
            Assert.Contains(Shape(statement.MutationLineage), new[] { paymentLineage, shipmentLineage });
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RealAllocatedGraphKeepsBranchDeleteLedgerAndCommittedAuditLineage(bool logging)
    {
        var fixture = await Fixture(); await using var db = fixture.Db;
        var context = fixture.Context; var sink = fixture.Sink;
        context.EnableQuerySqlLog(logging).EnableMutationSqlLog(logging);
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
            Assert.Equal(2, result.Metadata.Statements.Count); // Soft delete also reads back its tombstone.
            var deleteSql = result.Metadata.Statements[0];
            Assert.Equal(DataServiceOperation.Delete, deleteSql.Operation);
            Assert.Equal(expected[("OrderItem", 202)], Shape(deleteSql.MutationLineage));
            Assert.Equal("success", deleteSql.ExecutionOutcome);
            Assert.Equal(1UL, deleteSql.AffectedRows);
            var deleteReadback = result.Metadata.Statements[1];
            Assert.Equal(DataServiceOperation.Query, deleteReadback.Operation);
            Assert.Equal(expected[("OrderItem", 202)], Shape(deleteReadback.MutationLineage));
            Assert.Equal("success", deleteReadback.ExecutionOutcome);
            Assert.Equal(1, deleteReadback.ResultCount);
            Assert.Equal(new Value.U64Value(202), Assert.Single(deleteReadback.Parameters));
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
                // Inspect the actual INSERT and its SELECT readback separately;
                // a correct result envelope alone cannot prove physical lineage.
                Assert.Equal(2, result.Metadata.Statements.Count);
                for (var index = 0; index < result.Metadata.Statements.Count; index++)
                {
                    var physical = result.Metadata.Statements[index];
                    Assert.Equal(index == 0 ? DataServiceOperation.Insert : DataServiceOperation.Query,
                        physical.Operation);
                    Assert.Equal(expected[(entity, id)], Shape(physical.MutationLineage));
                    Assert.Equal("submit order", physical.AuditReason);
                    Assert.Equal("success", physical.ExecutionOutcome);
                    Assert.Equal(index == 0 ? "insert" : "select", physical.TraceChain[^1].Name);
                    if (index == 0) Assert.Equal(1UL, physical.AffectedRows);
                    else
                    {
                        Assert.Equal(1, physical.ResultCount);
                        Assert.Equal(new Value.U64Value(id), Assert.Single(physical.Parameters));
                    }
                }
                Assert.Equal("CustomerOrder", result.Metadata.TraceChain[0].Name);
                Assert.All(result.Metadata.MutationLineage, node => Assert.NotNull(node.EntityId));
                Assert.Empty(sink.Audit);
            }
        });
        Assert.Equal(6, sink.Audit.Count);
        Assert.Equal(logging ? 6 : 0, sink.Sql.Count(item => item.Operation != DataServiceOperation.Query));
        Assert.Equal(logging ? 12 : 0, sink.Sql.Count);
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
