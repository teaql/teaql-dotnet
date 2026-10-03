using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;
using Xunit.Abstractions;
using Record = TeaQL.Core.Record;

namespace TeaQL.Provider.Sqlite.Tests;

public class RelationAggregateTraceTests
{
    private readonly ITestOutputHelper output;
    public RelationAggregateTraceTests(ITestOutputHelper output) => this.output = output;

    private sealed class Sink : IDiagnosticSqlLogSink
    {
        public readonly List<ExecutionMetadata> Entries = new();
        public void Write(ExecutionMetadata metadata) => Entries.Add(metadata);
    }

    private sealed class ObservedTransport(ISqlTransport inner) : ISqlTransport
    {
        public readonly List<CompiledQuery> Reads = new();
        public bool FailAggregate;
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
        {
            Reads.Add(query);
            if (FailAggregate && query.Sql.Contains("COUNT(", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("synthetic aggregate failure");
            return inner.FetchAllSqlAsync(query);
        }
    }

    private sealed class ObservedService(IDataService inner) : IDataService
    {
        public readonly List<ExecutionMetadata> Statements = new();
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Task<MutationResult> MutateAsync(MutationRequest request) => inner.MutateAsync(request);
        public Task<QueryResult> QueryAsync(QueryRequest request)
        {
            var previous = request.DiagnosticObserver;
            request.DiagnosticObserver = metadata => {
                Statements.Add(metadata);
                previous?.Invoke(metadata);
            };
            return inner.QueryAsync(request);
        }
    }

    private static EntityDescriptor Entity(string name) => EntityDescriptor.New(name)
        .TableName(name.ToLowerInvariant() + "_data")
        .Property(PropertyDescriptor.New("id", DataType.I64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version());

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task NumericGroupedPartitionKeepsCountsWithoutInventingRelationEdges(bool nested, bool logging)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        const string secret = "PRIVATE-NUMERIC-PARTITION";
        var parent = Entity("Parent").Relation(RelationDescriptor.New("children", "Child")
            .LocalKey("id").ForeignKey("parent_id").Many());
        var child = Entity("Child")
            .Property(PropertyDescriptor.New("parent_id", DataType.I64))
            .Property(PropertyDescriptor.New("bucket", DataType.I64))
            .Property(PropertyDescriptor.New("name", DataType.Text)).AuditMaskFields(new() { "name" });
        Assert.Null(child.RelationByName("bucket"));
        var descriptors = new[] { parent, child };
        var schemas = new MetadataSchemaProvider(name => descriptors.FirstOrDefault(entity => entity.Name == name));
        var transport = new ObservedTransport(new SqliteTransport(connection));
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), transport, schemas);
        var observed = new ObservedService(provider);
        var sink = new Sink();
        var context = new RuntimeModule().Entity(parent).Entity(child).IntoContext()
            .WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        context.WithDataService(observed);
        var service = context.RequireResource<IDataService>();
        await service.MutateAsync(new InsertMutationRequest(new InsertCommand("Parent")
            .Value("id", Value.FromObject(1L)), "seed numeric partition parent"));
        for (var index = 0; index < 4; index++)
            await service.MutateAsync(new InsertMutationRequest(new InsertCommand("Child")
                .Value("id", Value.FromObject(index + 1L)).Value("parent_id", Value.FromObject(1L))
                .Value("bucket", Value.FromObject(index < 2 ? 10L : 20L))
                .Value("name", Value.FromObject(index == 3 ? "excluded" : secret)), "seed numeric partition child"));
        sink.Entries.Clear(); transport.Reads.Clear(); observed.Statements.Clear();
        context.EnableQuerySqlLog(logging);
        // Native GroupBy/Count partitions by a scalar, not a relation. Root
        // execution also uses a numeric window; SQLite loads relations by probe.
        var grouped = new SelectQuery("Child").Filter(Expr.Eq("name", secret))
            .GroupBy("parent_id").GroupBy("bucket").Count("n").Limit(10);
        if (!nested) grouped.PartitionByField("bucket");
        var query = (nested ? new SelectQuery("Parent").Project("id").Limit(1)
            .RelationQuery("children", grouped) : grouped)
            .Comment("count numeric groups " + secret).Purpose("render numeric partitions " + secret);
        var request = new QueryRequest(query);
        Assert.Empty(request.TraceChain);
        var result = await service.QueryAsync(request);
        var rows = nested
            ? Assert.IsType<Value.ListValue>(Assert.Single(result.Rows)["children"]).Values
                .Select(value => Assert.IsType<Value.ObjectValue>(value).Value).ToList()
            : result.Rows;
        Assert.Equal(new[] { (10L, 2L), (20L, 1L) }, rows
            .Select(row => (row["bucket"].TryI64()!.Value, row["n"].TryI64()!.Value))
            .OrderBy(pair => pair.Item1));
        Assert.Equal(nested ? 2 : 1, transport.Reads.Count);
        var aggregate = Assert.Single(transport.Reads.Where(read => read.Sql.Contains("COUNT(")));
        Assert.Contains("GROUP BY parent_id, bucket", aggregate.Sql);
        if (!nested) Assert.Contains("PARTITION BY bucket", aggregate.Sql);
        Assert.Contains(aggregate.Params, value => value.TryText() == secret);
        Assert.Equal(transport.Reads.Count, observed.Statements.Count);
        Assert.Equal(logging ? transport.Reads.Count : 0, sink.Entries.Count);
        for (var index = 0; index < observed.Statements.Count; index++)
        {
            var statement = observed.Statements[index];
            var edges = nested && index > 0 ? new[] { "children" } : Array.Empty<string>();
            var root = nested ? "Parent" : "Child";
            Assert.Equal(new[] { root, root }.Concat(edges).Concat(new[] { "sqlite", "select" }),
                statement.TraceChain.Select(node => node.Name));
            Assert.Equal(new[] { "operation", "request" }.Concat(edges.Select(_ => "relation"))
                .Concat(new[] { "provider", "sql" }), statement.TraceChain.Select(node => node.Kind));
            Assert.Equal("count numeric groups " + secret, statement.Comment);
            Assert.Equal("render numeric partitions " + secret, statement.Purpose);
            Assert.Equal("success", statement.ExecutionOutcome);
        }
        Assert.All(sink.Entries, entry => {
            Assert.DoesNotContain(secret, entry.Comment);
            Assert.DoesNotContain(secret, entry.Purpose);
            Assert.DoesNotContain(secret, entry.DebugQuery);
            Assert.DoesNotContain(entry.TraceChain, node => node.Kind == "relation" && node.Name == "bucket");
            output.WriteLine($"NUMERIC_SQL nested={nested} {entry.DebugQuery}\n" +
                $"comment={entry.Comment} purpose={entry.Purpose} path=" +
                string.Join("/", entry.TraceChain.Select(node => $"{node.Kind}:{node.Name}")));
        });
        output.WriteLine($"NUMERIC_RESULT nested={nested} logging={logging} " +
            string.Join(", ", rows.OrderBy(row => row["bucket"].TryI64())
                .Select(row => $"bucket={row["bucket"].TryI64()} count={row["n"].TryI64()}")));
        Assert.Equal("count numeric groups " + secret, request.Comment);
        sink.Entries.Clear(); observed.Statements.Clear(); transport.Reads.Clear();
        await service.QueryAsync(new QueryRequest(new SelectQuery("Parent").Project("id").Limit(1)
            .Comment("independent " + secret).Purpose("verify numeric scope isolation")));
        Assert.Single(transport.Reads);
        if (logging) Assert.Equal("independent " + secret, Assert.Single(sink.Entries).Comment);
        else Assert.Empty(sink.Entries);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    [InlineData(false, false, false, true, false)]
    [InlineData(false, true, false, true, false)]
    [InlineData(true, false, false, true, false)]
    [InlineData(true, true, false, true, false)]
    [InlineData(false, false, false, true, true)]
    [InlineData(false, true, false, true, true)]
    [InlineData(true, false, false, true, true)]
    [InlineData(true, true, false, true, true)]
    public async Task HydratedMembershipPreservesAggregatesAndNestedRoute(
        bool nested, bool logging, bool fail, bool filtered = false, bool sibling = false)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        const string secret = "PRIVATE-PARENT-AGGREGATE";
        // The target identity is code, deliberately not id. Hydration replaces
        // parent_ref with an object; relationship matching still needs code.
        var parent = Entity("Parent").Property(PropertyDescriptor.New("code", DataType.Text))
            .Property(PropertyDescriptor.New("name", DataType.Text)).AuditMaskFields(new() { "name" })
            .Relation(RelationDescriptor.New("children", "Child").LocalKey("code").ForeignKey("parent_ref").Many());
        var child = Entity("Child").Property(PropertyDescriptor.New("parent_ref", DataType.Text))
            .Relation(RelationDescriptor.New("parent_ref", "Parent").LocalKey("parent_ref").ForeignKey("code"))
            .Relation(RelationDescriptor.New("parent_again", "Parent").LocalKey("parent_ref").ForeignKey("code"));
        var descriptors = new[] { parent, child };
        var schemas = new MetadataSchemaProvider(name => descriptors.FirstOrDefault(entity => entity.Name == name));
        var transport = new ObservedTransport(new SqliteTransport(connection));
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), transport, schemas);
        var sink = new Sink();
        var context = new RuntimeModule().Entity(parent).Entity(child).IntoContext()
            .WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        await service.MutateAsync(new InsertMutationRequest(new InsertCommand("Parent")
            .Value("id", Value.FromObject(1L)).Value("code", Value.FromObject("P1"))
            .Value("name", Value.FromObject(secret)), "seed parent"));
        await service.MutateAsync(new InsertMutationRequest(new InsertCommand("Child")
            .Value("id", Value.FromObject(1L)).Value("parent_ref", Value.FromObject("P1")), "seed child"));
        sink.Entries.Clear(); transport.Reads.Clear();
        if (!logging) context.DisableQuerySqlLog();
        var forward = new SelectQuery("Parent").Projects(new[] { "id", "code", "name" }).Limit(1);
        if (filtered) forward.Filter(Expr.Eq("code", "not-visible"));
        var childQuery = new SelectQuery("Child").Projects(new[] { "id", "parent_ref" }).Limit(1)
            .RelationQuery("parent_ref", forward);
        if (sibling) childQuery.RelationQuery("parent_again",
            new SelectQuery("Parent").Projects(new[] { "id", "code", "name" }).Limit(1));
        childQuery.RelationAggregates.Add(new RelationAggregate("parent_ref", "parent_count",
            new SelectQuery("Parent").Filter(Expr.Eq("name", secret)).CountField("id", "n"), true));
        var query = (nested ? new SelectQuery("Parent").Projects(new[] { "id", "code" }).Limit(1)
            .RelationQuery("children", childQuery) : childQuery)
            .Comment("inspect " + secret).Purpose("verify aggregate membership and ancestry");
        transport.FailAggregate = fail;
        if (fail)
        {
            var error = await Assert.ThrowsAsync<SqlExecutorException>(() => service.QueryAsync(new QueryRequest(query)));
            Assert.Contains("synthetic aggregate failure", error.Message);
            Assert.Equal(nested ? 3 : 2, transport.Reads.Count);
            Assert.Equal("failure", sink.Entries.Last().ExecutionOutcome);
        }
        else
        {
            var result = await service.QueryAsync(new QueryRequest(query));
            var row = Assert.Single(result.Rows);
            if (nested)
                row = Assert.IsType<Value.ObjectValue>(Assert.Single(Assert.IsType<Value.ListValue>(row["children"]).Values)).Value;
            Assert.Equal(1L, row["parent_count"].TryI64());
            if (filtered) Assert.IsType<Value.NullValue>(row["parent_ref"]);
            else
            {
                var loaded = Assert.IsType<Value.ObjectValue>(row["parent_ref"]).Value;
                Assert.Equal("P1", loaded["code"].TryText());
                Assert.Equal(secret, loaded["name"].TryText());
            }
            if (sibling)
                Assert.Equal("P1", Assert.IsType<Value.ObjectValue>(row["parent_again"]).Value["code"].TryText());
            Assert.Equal((nested ? 4 : 3) + (sibling ? 1 : 0), transport.Reads.Count);
        }
        if (logging)
        {
            Assert.Equal(transport.Reads.Count, sink.Entries.Count);
            Assert.All(sink.Entries, entry => {
                Assert.Equal(nested ? "Parent" : "Child", entry.TraceChain[0].Name);
                Assert.DoesNotContain(secret, entry.Comment);
                Assert.DoesNotContain(secret, entry.DebugQuery);
                Assert.Equal("verify aggregate membership and ancestry", entry.Purpose);
            });
            var count = Assert.Single(sink.Entries.Where(entry => entry.ParameterizedQuery?.Contains("COUNT(") == true));
            Assert.Equal(nested ? new[] { "children", "parent_ref" } : new[] { "parent_ref" },
                count.TraceChain.Where(node => node.Kind == "relation").Select(node => node.Name));
        }
        else Assert.Empty(sink.Entries);
        transport.FailAggregate = false; sink.Entries.Clear(); transport.Reads.Clear();
        await service.QueryAsync(new QueryRequest(new SelectQuery("Parent").Project("id").Limit(1)
            .Comment("independent next query").Purpose("verify restored scope")));
        Assert.Single(transport.Reads);
        if (logging) Assert.Equal("independent next query", Assert.Single(sink.Entries).Comment);
    }
}
