using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Provider.Sqlite.Tests;

public class RelationAggregateTraceTests
{
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

    private static EntityDescriptor Entity(string name) => EntityDescriptor.New(name)
        .TableName(name.ToLowerInvariant() + "_data")
        .Property(PropertyDescriptor.New("id", DataType.I64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version());

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
