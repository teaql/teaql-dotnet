using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;
using Xunit.Abstractions;
using Record = TeaQL.Core.Record;

namespace TeaQL.Provider.Sqlite.Tests;

public class FacetTraceSqliteTests
{
    private readonly ITestOutputHelper output;
    public FacetTraceSqliteTests(ITestOutputHelper output) => this.output = output;

    private sealed class Sink : IDiagnosticSqlLogSink
    {
        public readonly List<ExecutionMetadata> Entries = new();
        public void Write(ExecutionMetadata entry) => Entries.Add(entry);
    }

    private sealed class Transport(ISqlTransport inner) : ISqlTransport, ISqlTransactionTransport
    {
        public readonly List<CompiledQuery> Reads = new();
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
        { Reads.Add(query); return inner.FetchAllSqlAsync(query); }
        public async Task<ISqlTransaction> BeginSqlAsync() =>
            new Transaction(this, await ((ISqlTransactionTransport)inner).BeginSqlAsync());

        private sealed class Transaction(Transport owner, ISqlTransaction inner) : ISqlTransaction
        {
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
            public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
            { owner.Reads.Add(query); return inner.FetchAllSqlAsync(query); }
            public Task CommitSqlAsync() => inner.CommitSqlAsync();
            public Task RollbackSqlAsync() => inner.RollbackSqlAsync();
            public void Dispose() => inner.Dispose();
        }
    }

    private static EntityDescriptor Entity(string name) => EntityDescriptor.New(name)
        .TableName(name.ToLowerInvariant() + "_data")
        .Property(PropertyDescriptor.New("id", DataType.I64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version());

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, true, true)]
    [InlineData(false, false, true, true)]
    [InlineData(false, true, false, true)]
    [InlineData(true, false, true, true)]
    [InlineData(true, true, true, true)]
    [InlineData(true, false, true, false, true)]
    [InlineData(true, true, true, false, true)]
    public async Task NestedAndLoadedRelationFacetsKeepCountsMaterializationAndOrigin(
        bool loadedRelation, bool transaction, bool logging, bool failure = false, bool multipleParents = false)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var attempt = Entity("Attempt")
            .Property(PropertyDescriptor.New("payment_id", DataType.I64))
            .Property(PropertyDescriptor.New("status", DataType.Text))
            .Relation(RelationDescriptor.New("payment", "Payment").LocalKey("payment_id").ForeignKey("id"));
        var payment = Entity("Payment")
            .Property(PropertyDescriptor.New("order_id", DataType.I64))
            .Relation(RelationDescriptor.New("order", "CustomerOrder").LocalKey("order_id").ForeignKey("id"));
        var order = Entity("CustomerOrder")
            .Property(PropertyDescriptor.New("name", DataType.Text)).AuditMaskFields(new() { "name" });
        var descriptors = new[] { attempt, payment, order };
        var transport = new Transport(new SqliteTransport(connection));
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), transport,
            new MetadataSchemaProvider(name => descriptors.SingleOrDefault(entity => entity.Name == name)));
        var sink = new Sink();
        var context = new RuntimeModule().Entity(attempt).Entity(payment).Entity(order).IntoContext()
            .WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        const string secret = "PRIVATE-FUTURE-FACET";
        await Seed("CustomerOrder", 20, "name", secret);
        await Seed("CustomerOrder", 21, "name", "unselected order");
        await Seed("Payment", 10, "order_id", 20L);
        await Seed("Payment", 11, "order_id", 21L);
        for (long id = 1; id <= 3; id++)
            await service.MutateAsync(new InsertMutationRequest(new InsertCommand("Attempt")
                .Value("id", Value.FromObject(id)).Value("payment_id", Value.FromObject(id < 3 ? 10L : 11L))
                .Value("status", Value.FromObject(id < 3 ? "selected" : "excluded")), "seed facet membership"));

        var orders = new SelectQuery("CustomerOrder").Filter(Expr.Eq("name", secret)).Limit(10).Count("matching_payments");
        var payments = new SelectQuery("Payment").Limit(10).Count("matching_attempts");
        payments.Facets.Add(new FacetRequest("orders", "order", orders, false));
        var query = new SelectQuery("Attempt").Filter(Expr.Eq("status", "selected")).Limit(1)
            .Comment("inspect " + secret).Purpose("verify nested facet provenance");
        if (multipleParents) { query.FilterCondition = null; query.Limit(10).OrderAsc("id"); }
        if (loadedRelation)
        {
            payments.AggregateItems.Clear();
            query.RelationQuery("payment", payments);
        }
        else query.Facets.Add(new FacetRequest("payments", "payment", payments, false));
        var request = new QueryRequest(query);
        orders.Filter(Expr.Eq("name", "caller changed"));
        query.Comment("caller changed");
        sink.Entries.Clear(); transport.Reads.Clear();
        context.EnableQuerySqlLog(logging);
        if (failure)
        {
            await using var ddl = connection.CreateCommand();
            ddl.CommandText = "DROP TABLE customerorder_data";
            await ddl.ExecuteNonQueryAsync();
        }
        using var tx = transaction ? await provider.BeginTransactionAsync() : null;
        var execution = tx is null ? service : new RuntimeDataService(tx, context);
        if (failure)
        {
            var error = await Assert.ThrowsAsync<SqlExecutorException>(() => execution.QueryAsync(request));
            Assert.Contains("no such table", error.Message);
        }
        else
        {
            var result = await execution.QueryAsync(request);
            Assert.Equal(multipleParents ? 3 : 1, result.Rows.Count);
            var root = result.Rows[0];
            SmartList<Record> orderFacet;
            if (loadedRelation)
            {
                var loaded = Assert.IsType<Value.ObjectValue>(root["payment"]).Value;
                Assert.Equal(10L, loaded["id"].TryI64());
                orderFacet = loaded.QueryFacets["orders"];
                Assert.DoesNotContain("QueryFacets", JsonSerializer.Serialize(loaded));
                Assert.False(loaded.ContainsKey("orders")); // Facets are not scalar write fields.
                if (multipleParents)
                {
                    var unrelated = Assert.IsType<Value.ObjectValue>(result.Rows[2]["payment"]).Value;
                    Assert.Equal(11L, unrelated["id"].TryI64());
                    Assert.Empty(unrelated.QueryFacets["orders"]);
                    Assert.NotSame(orderFacet, unrelated.QueryFacets["orders"]);
                }
            }
            else
            {
                var paymentFacet = result.Facets["payments"];
                var selected = Assert.Single(paymentFacet);
                Assert.Equal(10L, selected["id"].TryI64());
                Assert.Equal(2L, selected["matching_attempts"].TryI64()); // Not the root's LIMIT 1.
                orderFacet = paymentFacet.Facets["orders"];
            }
            var selectedOrder = Assert.Single(orderFacet);
            Assert.Equal(20L, selectedOrder["id"].TryI64());
            Assert.Equal(secret, selectedOrder["name"].TryText());
            Assert.Equal(1L, selectedOrder["matching_payments"].TryI64());
        }
        Assert.Equal(multipleParents ? 7 : loadedRelation ? 4 : 5, transport.Reads.Count);
        Assert.Equal(multipleParents ? 2 : loadedRelation ? 1 : 2,
            transport.Reads.Count(read => read.Sql.Contains("COUNT(", StringComparison.OrdinalIgnoreCase)));
        if (logging)
        {
            Assert.Equal(transport.Reads.Count, sink.Entries.Count);
            var expectedRoutes = multipleParents
                ? new[] { "", "payment", "payment/order", "payment/order", "payment", "payment/order", "payment/order" }
                : loadedRelation
                ? new[] { "", "payment", "payment/order", "payment/order" }
                : new[] { "", "payment", "payment", "payment/order", "payment/order" };
            Assert.Equal(expectedRoutes, sink.Entries.Select(entry => string.Join("/", entry.TraceChain
                .Where(node => node.Kind == "relation").Select(node => node.Name))));
            Assert.All(sink.Entries, entry => {
                Assert.Equal("Attempt", entry.TraceChain[0].Name);
                Assert.Equal("Attempt", entry.TraceChain[1].Name);
                Assert.Equal("operation", entry.TraceChain[0].Kind);
                Assert.Equal("request", entry.TraceChain[1].Kind);
                Assert.Equal("sqlite", entry.TraceChain[^2].Name);
                Assert.Equal("select", entry.TraceChain[^1].Name);
                Assert.Equal(entry.TraceChain.Where(node => node.Kind == "relation").Select(node => node.Name == "payment"
                        ? "Attempt.payment" : "Payment.order"),
                    entry.TraceChain.Where(node => node.Kind == "relation").Select(node => node.Detail));
                Assert.Equal("verify nested facet provenance", entry.Purpose);
                Assert.DoesNotContain(secret, JsonSerializer.Serialize(entry));
                Assert.DoesNotContain("caller changed", entry.Comment);
            });
            Assert.Equal(failure ? "failure" : "success", sink.Entries[^1].ExecutionOutcome);
        }
        else Assert.Empty(sink.Entries);
        Assert.Equal("inspect " + secret, request.Comment);
        output.WriteLine("FACET EVIDENCE " + JsonSerializer.Serialize(new {
            loadedRelation, transaction, logging, failure, multipleParents,
            physicalReads = transport.Reads.Count, sql = sink.Entries
        }));
        if (tx is not null)
        {
            if (failure) await tx.RollbackAsync();
            else await tx.CommitAsync();
        }
        sink.Entries.Clear(); transport.Reads.Clear();
        await service.QueryAsync(new QueryRequest(new SelectQuery("Attempt").Limit(1)
            .Comment("independent " + secret).Purpose("verify request-local provenance")));
        if (logging) Assert.Equal("independent " + secret, Assert.Single(sink.Entries).Comment);

        Task<MutationResult> Seed(string entity, long id, string field, object value) =>
            service.MutateAsync(new InsertMutationRequest(new InsertCommand(entity)
                .Value("id", Value.FromObject(id)).Value(field, Value.FromObject(value)), "seed facet fixture"));
    }
}
