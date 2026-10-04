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
    [InlineData(false, false, false, false)]
    [InlineData(false, false, true, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, true, true, false)]
    [InlineData(true, false, false, false)]
    [InlineData(true, false, true, false)]
    [InlineData(true, true, false, false)]
    [InlineData(true, true, true, false)]
    public async Task NullParentKeysDoNotCreateMembershipButKeepRequestedFacets(
        bool allNull, bool transaction, bool includeAll, bool logging = true)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var parent = Entity("Parent").Property(PropertyDescriptor.New("code", DataType.Text))
            .Relation(RelationDescriptor.New("children", "Child").LocalKey("code").ForeignKey("parent_ref").Many());
        var child = Entity("Child").Property(PropertyDescriptor.New("parent_ref", DataType.Text))
            .Property(PropertyDescriptor.New("category_id", DataType.I64))
            .Relation(RelationDescriptor.New("category", "Category").LocalKey("category_id").ForeignKey("id"));
        var category = Entity("Category").Property(PropertyDescriptor.New("name", DataType.Text));
        var descriptors = new[] { parent, child, category };
        var transport = new Transport(new SqliteTransport(connection));
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), transport,
            new MetadataSchemaProvider(name => descriptors.SingleOrDefault(entity => entity.Name == name)));
        var sink = new Sink();
        var context = new RuntimeModule().Entity(parent).Entity(child).Entity(category).IntoContext()
            .WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        await Seed(new InsertCommand("Parent").Value("id", Value.FromObject(1L)).Value("code", Value.FromObject("P1")));
        await Seed(new InsertCommand("Parent").Value("id", Value.FromObject(2L)).Value("code", new Value.NullValue()));
        await Seed(new InsertCommand("Parent").Value("id", Value.FromObject(3L)).Value("code", Value.FromObject("EMPTY")));
        await Seed(new InsertCommand("Category").Value("id", Value.FromObject(20L)).Value("name", Value.FromObject("related")));
        await Seed(new InsertCommand("Category").Value("id", Value.FromObject(21L)).Value("name", Value.FromObject("orphan")));
        await Seed(new InsertCommand("Child").Value("id", Value.FromObject(10L))
            .Value("parent_ref", Value.FromObject("P1")).Value("category_id", Value.FromObject(20L)));
        await Seed(new InsertCommand("Child").Value("id", Value.FromObject(11L))
            .Value("parent_ref", new Value.NullValue()).Value("category_id", Value.FromObject(21L)));
        var children = new SelectQuery("Child").Projects(new[] { "id", "parent_ref", "category_id" }).Limit(10);
        children.Facets.Add(new FacetRequest("categories", "category", new SelectQuery("Category").Limit(10).Count("members"), includeAll));
        var query = new SelectQuery("Parent").Projects(new[] { "id", "code" }).OrderAsc("id").Limit(10)
            .RelationQuery("children", children).Comment("load nullable membership").Purpose("exclude orphan children from relation facets");
        query.Facets.Add(new FacetRequest("allChildren", "children",
            new SelectQuery("Child").Projects(new[] { "id", "parent_ref" }).OrderAsc("id").Limit(10).Count("parents"), includeAll));
        if (allNull) query.Filter(Expr.Eq("id", 2L));
        sink.Entries.Clear(); transport.Reads.Clear();
        context.EnableQuerySqlLog(logging);
        using var tx = transaction ? await provider.BeginTransactionAsync() : null;
        var execution = tx is null ? service : new RuntimeDataService(tx, context);
        var result = await execution.QueryAsync(new QueryRequest(query));
        Assert.Equal(allNull ? 1 : 3, result.Rows.Count);
        var nullParent = result.Rows.Single(row => row["id"].TryI64() == 2L);
        Assert.Empty(Assert.IsType<Value.ListValue>(nullParent["children"]).Values);
        foreach (var owner in result.Rows)
        {
            var loaded = Assert.IsType<SmartList<Value>>(Assert.IsType<Value.ListValue>(owner["children"]).Values);
            Assert.True(loaded.IsLoaded);
            var hasMember = owner["id"].TryI64() == 1L;
            Assert.Equal(hasMember ? 1 : 0, loaded.Count);
            Assert.True(loaded.Facets.TryGetValue("categories", out var categories));
            Assert.Equal(includeAll ? 2 : hasMember ? 1 : 0, categories!.Count);
            foreach (var candidate in categories)
                Assert.Equal(hasMember && candidate["id"].TryI64() == 20L ? 1L : 0L, candidate["members"].TryI64());
            Assert.DoesNotContain("categories", owner.ToJsonValue().ToJsonString());
            Assert.DoesNotContain("categories", JsonSerializer.Serialize(owner));
            var snapshot = new LoadedScalarSnapshot(owner).CopyValues();
            Assert.IsNotType<SmartList<Value>>(Assert.IsType<Value.ListValue>(snapshot["children"]).Values);
        }
        if (!allNull)
        {
            var related = Assert.IsType<Value.ObjectValue>(Assert.Single(
                Assert.IsType<Value.ListValue>(result.Rows[0]["children"]).Values)).Value;
            Assert.Equal(10L, related["id"].TryI64());
            var counted = Assert.Single(related.QueryFacets["categories"].Where(row => row["id"].TryI64() == 20L));
            Assert.Equal(20L, counted["id"].TryI64());
            Assert.Equal(1L, counted["members"].TryI64());
        }
        // IncludeAll may enumerate an orphan as a zero-count candidate; it must
        // never be attached to a null-key parent or acquire a false membership.
        Assert.True(result.Facets.TryGetValue("allChildren", out var requested));
        Assert.Equal(includeAll ? 2 : allNull ? 0 : 1, requested!.Count);
        foreach (var candidate in requested)
            Assert.Equal(!allNull && candidate["id"].TryI64() == 10L ? 1L : 0L, candidate["parents"].TryI64());
        Assert.DoesNotContain(transport.Reads.SelectMany(read => read.Params), value => value is Value.NullValue or Value.TypedNullValue);
        Assert.All(sink.Entries, entry => Assert.Equal("Parent", entry.TraceChain[0].Name));
        if (!logging) Assert.Empty(sink.Entries);
        output.WriteLine("NULL FACET EVIDENCE " + JsonSerializer.Serialize(new {
            allNull, transaction, includeAll, logging, physicalReads = transport.Reads.Count,
            candidateMembership = requested.Select(row => new { id = row["id"].TryI64(), count = row["parents"].TryI64() }),
            sql = sink.Entries
        }));
        if (tx is not null) await tx.CommitAsync();

        Task<MutationResult> Seed(InsertCommand command) => service.MutateAsync(new InsertMutationRequest(command, "seed nullable facet fixture"));
    }

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
