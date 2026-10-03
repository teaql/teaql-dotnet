using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

[Collection("Readback log environment")]
public class QueryCountIntentTests
{
    private sealed class Sink : IDiagnosticSqlLogSink, ISensitiveDiagnosticSqlLogSink
    {
        public readonly List<ExecutionMetadata> Entries = new();
        public void Write(ExecutionMetadata metadata) => Entries.Add(metadata);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(false, false, false, "aggregate")]
    [InlineData(false, false, false, "facet")]
    [InlineData(false, false, false, "enhancement")]
    [InlineData(false, false, false, "group")]
    public async Task RemovedNestedBindingsRemainPrivateWithoutExecutingChildren(bool transaction, bool failure, bool debug, string shape = "relation")
    {
        const string flag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
        var previous = Environment.GetEnvironmentVariable(flag);
        try
        {
            Environment.SetEnvironmentVariable(flag, debug ? "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK" : null);
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var root = EntityDescriptor.New("Document").TableName("document_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("version", DataType.I64).Version())
                .Relation(RelationDescriptor.New("lines", "Line").LocalKey("id").ForeignKey("documentId").Many());
            var line = EntityDescriptor.New("Line").TableName("line_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("documentId", DataType.I64))
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .Property(PropertyDescriptor.New("password", DataType.Text))
                .AuditMaskFields(new() { "name" });
            var module = new RuntimeModule().Entity(root).Entity(line);
            var schema = new MetadataSchemaProvider(name => name == "Document" ? root : name == "Line" ? line : null);
            var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection), schema);
            var normal = new Sink(); var sensitive = new Sink();
            var context = module.IntoContext().WithDataService(provider)
                .WithDiagnosticSqlLogSink(normal).WithSensitiveDiagnosticSqlLogSink(sensitive);
            await context.EnsureSchemaAsync();
            await context.RequireResource<IDataService>().MutateAsync(new InsertMutationRequest(
                new InsertCommand("Document").Value("id", Value.FromObject(1L)).Value("version", Value.FromObject(1L)),
                "seed count fixture"));
            normal.Entries.Clear(); sensitive.Entries.Clear();
            var child = new SelectQuery("Line").Filter(Expr.And(new List<Expr> {
                Expr.Eq("name", "COUNT-PRIVATE-CANARY"), Expr.Eq("password", "COUNT-CREDENTIAL-CANARY") }));
            var query = new SelectQuery("Document").Filter(Expr.Eq("id", 1L)).Offset(10).Limit(2)
                .Comment("count COUNT-PRIVATE-CANARY COUNT-CREDENTIAL-CANARY")
                .Purpose("page COUNT-PRIVATE-CANARY COUNT-CREDENTIAL-CANARY");
            switch (shape)
            {
                case "relation": query.RelationQuery("lines", child); break;
                case "aggregate": query.RelationAggregates.Add(new RelationAggregate("lines", "lineCount", child, true)); break;
                case "facet": query.Facets.Add(new FacetRequest("lineFacet", "lines", child, false)); break;
                case "enhancement": query.ChildEnhancements.Add(child); break;
                case "group": query.ObjectGroupBys.Add(new ObjectGroupBy("lineGroup", "lines", child)); break;
                default: throw new InvalidOperationException("unknown test shape");
            }
            var count = query.ForExactCount();
            // Both source and prepared query can evolve without changing removed binding provenance.
            child.Filter(Expr.Eq("name", "CALLER-CHANGED")); query.Filter(Expr.Eq("id", 999L));
            Assert.Empty(count.RelationLoads); Assert.Null(count.Slice);
            var wire = JsonSerializer.Serialize(count);
            Assert.DoesNotContain("DiagnosticOrigin", wire);
            Assert.DoesNotContain("diagnosticOrigin", wire);
            var withoutProse = (count with {CommentText=null, TraceChain=new()}).Purpose("no prose canary");
            Assert.DoesNotContain("COUNT-PRIVATE-CANARY", JsonSerializer.Serialize(withoutProse));
            Assert.DoesNotContain("COUNT-PRIVATE-CANARY", withoutProse.ToString());

            // Only the parent is queried. A nonexistent child table must not affect COUNT.
            await using (var ddl = connection.CreateCommand())
            {
                ddl.CommandText = failure ? "DROP TABLE document_data" : "DROP TABLE line_data";
                await ddl.ExecuteNonQueryAsync();
            }
            var tx = transaction ? await provider.BeginTransactionAsync() : null;
            var service = tx == null ? context.RequireResource<IDataService>() : new RuntimeDataService(tx, context);
            try
            {
                if (failure) await Assert.ThrowsAsync<SqlExecutorException>(() => service.QueryAsync(new QueryRequest(count)));
                else
                {
                    var result = await service.QueryAsync(new QueryRequest(count));
                    Assert.Equal(1L, Assert.Single(result.Rows)["count"].TryI64());
                }
                var safe = Assert.Single(normal.Entries);
                Assert.Equal(failure ? "failure" : "success", safe.ExecutionOutcome);
                Assert.DoesNotContain("COUNT-PRIVATE-CANARY", JsonSerializer.Serialize(safe));
                Assert.DoesNotContain("COUNT-CREDENTIAL-CANARY", JsonSerializer.Serialize(safe));
                Assert.Contains("count", safe.Comment);
                Assert.Contains("page", safe.Purpose);
                Assert.Equal(new[] {"Document", "Document", "sqlite", "select"}, safe.TraceChain.Select(node=>node.Name));
                Assert.DoesNotContain(safe.TraceChain, node=>node.Kind == "relation");
                Assert.Contains("COUNT", safe.DebugQuery!, StringComparison.OrdinalIgnoreCase);
                var debugEntry = Assert.Single(sensitive.Entries);
                Assert.Equal(debug, debugEntry.Comment!.Contains("COUNT-PRIVATE-CANARY"));
                Assert.DoesNotContain("COUNT-CREDENTIAL-CANARY", JsonSerializer.Serialize(debugEntry));
            }
            finally { if (tx != null) { await tx.RollbackAsync(); tx.Dispose(); } }
            if (!failure)
            {
                await context.RequireResource<IDataService>().QueryAsync(new QueryRequest(new SelectQuery("Document").Limit(1)
                    .Comment("independent COUNT-PRIVATE-CANARY").Purpose("not captured by another request")));
                Assert.Equal("independent COUNT-PRIVATE-CANARY", normal.Entries[^1].Comment);
            }
        }
        finally { Environment.SetEnvironmentVariable(flag, previous); }
    }
}
