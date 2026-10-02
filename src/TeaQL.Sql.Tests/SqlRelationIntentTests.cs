using System.Text.Json;
using Moq;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Sql.Tests;

[Collection("Readback log environment")]
public class SqlRelationIntentTests
{
    [Fact]
    public async Task LogCaptureDoesNotMutateUnexecutedAggregateChildBuilders()
    {
        var customer = EntityDescriptor.New("Customer").TableName("customer_data")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Relation(RelationDescriptor.New("orders", "Order").ForeignKey("customerId").Many());
        var order = EntityDescriptor.New("Order").TableName("order_data")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("customerId", DataType.I64))
            .Relation(RelationDescriptor.New("lines", "Line").ForeignKey("orderId").Many());
        var line = EntityDescriptor.New("Line").TableName("line_data")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("orderId", DataType.I64));
        var schema = new Mock<ISchemaProvider>();
        foreach (var entity in new[] { customer, order, line })
            schema.Setup(value => value.GetEntity(entity.Name)).Returns(entity);
        var transport = new Mock<ISqlTransport>();
        transport.Setup(value => value.FetchAllSqlAsync(It.IsAny<CompiledQuery>())).ReturnsAsync(new List<Record>());
        var sink = new Sink();
        var context = new UserContext().WithDataService(new SqlDataServiceExecutor(new Dialect(), transport.Object, schema.Object))
            .WithDiagnosticSqlLogSink(sink);
        var grandchild = new SelectQuery("Line");
        var child = new SelectQuery("Order").RelationQuery("lines", grandchild);
        var query = new SelectQuery("Customer").Limit(1)
            .RelationAggregate("orders", "Order", "customerId", "count", child, true)
            .Comment("observe an empty parent result").Purpose("keep unused child builders unchanged");
        await context.RequireResource<IDataService>().QueryAsync(new QueryRequest(query));
        Assert.Null(child.Slice); Assert.Null(grandchild.Slice);
        transport.Verify(value => value.FetchAllSqlAsync(It.IsAny<CompiledQuery>()), Times.Once);
        Assert.Single(sink.Entries);
    }

    private sealed class Dialect : TestSqlDialect
    {
        public override DatabaseKind Kind => DatabaseKind.PostgreSql;
    }

    private sealed class Transport(bool nested) : ISqlTransaction
    {
        public readonly List<CompiledQuery> Queries = new();
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
        {
            Queries.Add(query);
            if (query.Sql.Contains("customer_data"))
                return Task.FromResult(new List<Record> { new() {
                    ["id"] = Value.FromObject(1L), ["name"] = Value.FromObject("Riverside") } });
            if (nested && query.Sql.Contains("order_data"))
                return Task.FromResult(new List<Record> { new() {
                    ["id"] = Value.FromObject(2L), ["customerId"] = Value.FromObject(1L) } });
            return Task.FromException<List<Record>>(new InvalidOperationException("DRIVER-CANARY"));
        }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => throw new NotSupportedException();
        public Task CommitSqlAsync() => Task.CompletedTask;
        public Task RollbackSqlAsync() => Task.CompletedTask;
        public void Dispose() { }
    }

    private sealed class Sink : IDiagnosticSqlLogSink, ISensitiveDiagnosticSqlLogSink
    {
        public readonly List<ExecutionMetadata> Entries = new();
        public void Write(ExecutionMetadata metadata) => Entries.Add(metadata);
    }

    public static IEnumerable<object[]> Cases()
    {
        foreach (var transaction in new[] { false, true })
        foreach (var shape in new[] { "batch", "probe", "aggregate", "nested" })
        foreach (var debug in new[] { false, true })
            yield return new object[] { transaction, shape, debug };
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task DerivedFailureRetainsAncestorMaskPolicy(bool transaction, string shape, bool debug)
    {
        const string flag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
        var previous = Environment.GetEnvironmentVariable(flag);
        try
        {
            Environment.SetEnvironmentVariable(flag, debug ? "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK" : null);
            var customer = EntityDescriptor.New("Customer").TableName("customer_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .Property(PropertyDescriptor.New("password", DataType.Text))
                .AuditMaskFields(new() { "name" })
                .Relation(RelationDescriptor.New("orders", "Order").ForeignKey("customerId").Many());
            var order = EntityDescriptor.New("Order").TableName("order_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("customerId", DataType.I64))
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .AuditMaskFields(new() { "name" })
                .Relation(RelationDescriptor.New("lines", "Line").ForeignKey("orderId").Many());
            var line = EntityDescriptor.New("Line").TableName("line_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("orderId", DataType.I64));
            var schema = new Mock<ISchemaProvider>();
            foreach (var entity in new[] { customer, order, line })
                schema.Setup(s => s.GetEntity(entity.Name)).Returns(entity);
            var transport = new Transport(shape == "nested");
            IDataService provider = transaction
                ? new SqlDataServiceTransaction(new Dialect(), transport, schema.Object)
                : new SqlDataServiceExecutor(new Dialect(), transport, schema.Object);
            var normal = new Sink(); var sensitive = new Sink();
            var context = new UserContext().WithDiagnosticSqlLogSink(normal).WithSensitiveDiagnosticSqlLogSink(sensitive);
            var service = new RuntimeDataService(provider, context);
            var query = new SelectQuery("Customer").Filter(Expr.And(new List<Expr> {
                Expr.Eq("name", "Riverside"), Expr.Eq("password", "PASSWORD-CANARY") }))
                .Limit(1).Comment("what: load Riverside PASSWORD-CANARY" + (shape == "nested" ? " Lakeside" : ""))
                .Purpose("why: verify relation failure intent");
            var child = new SelectQuery("Order");
            if (shape == "probe") { child.Limit(1); child.TopNProbeThreshold = 32; }
            if (shape == "nested") child.Filter(Expr.Eq("name", "Lakeside")).Relation("lines");
            if (shape == "aggregate") query.RelationAggregate("orders", "Order", "customerId", "count", child, true);
            else query.RelationQuery("orders", child);

            await Assert.ThrowsAsync<SqlExecutorException>(() => service.QueryAsync(new QueryRequest(query)));
            Assert.Equal(shape == "nested" ? 3 : 2, normal.Entries.Count);
            var safe = Assert.Single(normal.Entries.Where(entry => entry.ExecutionOutcome == "failure"));
            var safeJson = JsonSerializer.Serialize(safe);
            Assert.DoesNotContain("Riverside", safeJson);
            Assert.DoesNotContain("PASSWORD-CANARY", safeJson);
            Assert.DoesNotContain("DRIVER-CANARY", safeJson);
            if (shape == "nested") Assert.DoesNotContain("Lakeside", safeJson);
            Assert.Contains("what: load", safe.Comment);
            Assert.Equal("why: verify relation failure intent", safe.Purpose);
            Assert.Equal("failure", safe.ExecutionOutcome);
            Assert.Contains(shape == "nested" ? "line_data" : "order_data", safe.DebugQuery);
            Assert.DoesNotContain("IntentSource", safeJson);
            var debugEntry = Assert.Single(sensitive.Entries.Where(entry => entry.ExecutionOutcome == "failure"));
            Assert.Equal(debug, debugEntry.Comment!.Contains("Riverside"));
            Assert.DoesNotContain("PASSWORD-CANARY", JsonSerializer.Serialize(debugEntry));
            foreach (var entry in normal.Entries)
            {
                var json = JsonSerializer.Serialize(entry);
                Assert.DoesNotContain("Riverside", json);
                Assert.DoesNotContain("PASSWORD-CANARY", json);
                if (shape == "nested") Assert.DoesNotContain("Lakeside", json);
            }
            foreach (var entry in sensitive.Entries)
                Assert.DoesNotContain("PASSWORD-CANARY", JsonSerializer.Serialize(entry));
            Assert.Contains(transport.Queries[0].Params, value => value.TryText() == "Riverside");
            Assert.Contains("Riverside", query.CommentText);

            // Provenance belongs to this execution, not the context or provider.
            await service.QueryAsync(new QueryRequest(new SelectQuery("Customer").Limit(1)
                .Comment("what: independent Riverside").Purpose("why: verify isolation")));
            Assert.Equal("what: independent Riverside", normal.Entries[^1].Comment);
        }
        finally { Environment.SetEnvironmentVariable(flag, previous); }
    }
}
