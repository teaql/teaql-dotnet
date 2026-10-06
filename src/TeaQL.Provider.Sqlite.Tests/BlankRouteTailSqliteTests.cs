using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

// TC-REQ-13 native caller diagnostic stimuli, not generated graph lineage proof.
public class BlankRouteTailSqliteTests
{
    private sealed class Sink(string path) : IDiagnosticSqlLogSink, IAppAuditEventSink
    {
        public List<ExecutionMetadata> Sql { get; } = new();
        public List<IReadOnlyDictionary<string, object?>> Audit { get; } = new();
        public void Write(ExecutionMetadata metadata) => Sql.Add(metadata);
        public async Task RecordAsync(IReadOnlyDictionary<string, object?> item, CancellationToken token = default)
        {
            await using var independent = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path }.ToString());
            await independent.OpenAsync(token);
            using var command = independent.CreateCommand();
            command.CommandText = "SELECT id, version FROM route_tail_order WHERE id=810";
            await using var row = await command.ExecuteReaderAsync(token);
            Assert.True(await row.ReadAsync(token)); // Not visible on this connection until commit.
            Assert.Equal(810, row.GetInt64(0)); Assert.Equal(1, row.GetInt64(1));
            Assert.False(await row.ReadAsync(token));
            Audit.Add(item);
        }
    }

    private sealed class Transactions(ITransactionExecutor inner) : ITransactionExecutor
    {
        public List<MutationRequest> Requests { get; } = new();
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Task<QueryResult> QueryAsync(QueryRequest request) => inner.QueryAsync(request);
        public Task<MutationResult> MutateAsync(MutationRequest request) => inner.MutateAsync(request);
        public async Task<ITransaction> BeginTransactionAsync() => new Transaction(this, await inner.BeginTransactionAsync());
        private sealed class Transaction(Transactions owner, ITransaction inner) : ITransaction
        {
            public DataServiceCapabilities Capabilities => inner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) => inner.QueryAsync(request);
            public Task<MutationResult> MutateAsync(MutationRequest request)
            { owner.Requests.Add(request); return inner.MutateAsync(request); }
            public Task CommitAsync() => inner.CommitAsync();
            public Task RollbackAsync() => inner.RollbackAsync();
            public void Dispose() => inner.Dispose();
        }
    }

    [Theory]
    [InlineData(false, "entity", "CustomerOrder")]
    [InlineData(false, "provider", "sqlite")]
    [InlineData(false, "sql", "insert")]
    [InlineData(true, "entity", "CustomerOrder")]
    [InlineData(true, "provider", "sqlite")]
    [InlineData(true, "sql", "insert")]
    public async Task ExplicitRootCommentSurvivesBlankTypedTailAtRealSinks(bool logging, string kind, string name)
    {
        var path = Path.Combine(Path.GetTempPath(), "teaql-dotnet-route-tail-" + Guid.NewGuid() + ".sqlite");
        await using var db = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path }.ToString());
        await db.OpenAsync();
        var entity = EntityDescriptor.New("CustomerOrder").TableName("route_tail_order")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("name", DataType.Text));
        var module = new RuntimeModule().Entity(entity);
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(db),
            new MetadataSchemaProvider(n => n == entity.Name ? entity : null));
        var sink = new Sink(path);
        var context = module.IntoContext().WithDataService(provider).WithDiagnosticSqlLogSink(sink).WithAppAuditEventSink(sink);
        await context.EnsureSchemaAsync(); sink.Sql.Clear(); sink.Audit.Clear();
        context.EnableQuerySqlLog(logging).EnableMutationSqlLog(logging);
        var transactions = new Transactions(provider);
        context.InsertResource<ITransactionExecutor>(transactions);
        const string comment = "  explicit owned mutation reason  ";
        var tail = new TraceNode(name, null, "") { Kind = kind, Name = name, Detail = "" };
        var source = new List<TraceNode> {
            new("CustomerOrder", 810, "") { Kind = "auditReason", Detail = comment }, tail
        };
        var command = new InsertCommand("CustomerOrder").Value("id", 810L).Value("name", "native route fixture");
        command.TraceChain.AddRange(source);
        var request = new InsertMutationRequest(command, comment) { LedgerKey = new EntityKey("CustomerOrder", 810) };
        Assert.Equal(tail, request.TraceChain.Last()); Assert.Equal("", tail.Detail);
        Assert.Equal(comment, request.Comment); Assert.Equal(comment, request.Intent.ReadbackIntent().Comment);
        var result = await context.ExecuteGraphSaveAsync(comment, async graph => {
            var mutation = await graph.MutateAsync(request);
            Assert.Empty(sink.Audit);
            return mutation;
        });
        var emitted = Assert.Single(transactions.Requests);
        Assert.Equal(source, emitted.TraceChain); Assert.Equal(tail, emitted.TraceChain.Last());
        Assert.Equal(comment, emitted.Comment); Assert.Equal(comment, emitted.Intent.ReadbackIntent().Comment);
        var responsibility = Assert.Single(emitted.MutationLineage);
        Assert.Equal(comment, responsibility.Detail);
        Assert.Equal(2, result.Metadata.Statements.Count);
        for (var index = 0; index < result.Metadata.Statements.Count; index++)
        {
            var fact = result.Metadata.Statements[index];
            Assert.Equal(comment, fact.AuditReason);
            Assert.Equal(comment, fact.Comment);
            Assert.Equal(index == 0 ? DataServiceOperation.Insert : DataServiceOperation.Query, fact.Operation);
            Assert.Equal("success", fact.ExecutionOutcome);
            Assert.Equal(emitted.MutationLineage, fact.MutationLineage);
            Assert.Equal("CustomerOrder", fact.TraceChain[0].Name);
            Assert.Equal("sqlite", fact.TraceChain[^2].Name);
            Assert.Equal(index == 0 ? "insert" : "select", fact.TraceChain[^1].Name);
        }
        var audit = Assert.Single(sink.Audit);
        Assert.Equal(comment, audit["reason"]);
        Assert.Equal(emitted.MutationLineage, Assert.IsAssignableFrom<IEnumerable<TraceNode>>(audit["traceChain"]));
        Assert.Equal(logging ? 2 : 0, sink.Sql.Count);
        Assert.All(sink.Sql, fact => Assert.Equal(comment, fact.AuditReason));
        Assert.Equal(source, request.TraceChain);
    }
}
