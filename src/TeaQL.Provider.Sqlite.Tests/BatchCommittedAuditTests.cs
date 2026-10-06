using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Provider.Sqlite.Tests;

// Public native batch-container acceptance, NOT a prepared/grouped execution claim.
public class BatchCommittedAuditTests
{
    private sealed class Sink : IDiagnosticSqlLogSink, IAppAuditEventSink
    {
        public readonly List<ExecutionMetadata> Sql = new();
        public readonly List<IReadOnlyDictionary<string, object?>> Audit = new();
        public Func<int> ActiveTransactions = () => 0;
        public Func<IReadOnlyDictionary<string, object?>, Exception?>? Failure;
        public int Attempts;
        public void Write(ExecutionMetadata metadata) => Sql.Add(metadata);
        public Task RecordAsync(IReadOnlyDictionary<string, object?> item, CancellationToken token = default)
        {
            Assert.Equal(0, ActiveTransactions());
            Attempts++;
            if (Failure?.Invoke(item) is { } error) throw error;
            Audit.Add(item);
            return Task.CompletedTask;
        }
        public void Clear() { Sql.Clear(); Audit.Clear(); Attempts = 0; }
    }

    private sealed class Transactions(ITransactionExecutor inner) : ITransactionExecutor
    {
        public int Begins, Commits, Rollbacks, Active;
        public readonly List<MutationRequest> Requests = new();
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Task<QueryResult> QueryAsync(QueryRequest request) => inner.QueryAsync(request);
        public Task<MutationResult> MutateAsync(MutationRequest request) => inner.MutateAsync(request);
        public async Task<ITransaction> BeginTransactionAsync()
        {
            var transaction = await inner.BeginTransactionAsync(); Begins++; Active++;
            return new Transaction(this, transaction);
        }
        private sealed class Transaction(Transactions owner, ITransaction inner) : ITransaction
        {
            public DataServiceCapabilities Capabilities => inner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) => inner.QueryAsync(request);
            public Task<MutationResult> MutateAsync(MutationRequest request)
            { owner.Requests.Add(request); return inner.MutateAsync(request); }
            public async Task CommitAsync() { await inner.CommitAsync(); owner.Commits++; owner.Active--; }
            public async Task RollbackAsync() { await inner.RollbackAsync(); owner.Rollbacks++; owner.Active--; }
            public void Dispose() => inner.Dispose();
        }
    }

    private sealed record Fixture(SqliteConnection Db, UserContext Context, Sink Sink, Transactions Transactions, long Id);

    private static async Task<Fixture> Open()
    {
        var path = Environment.GetEnvironmentVariable("TEAQL_DOTNET_BATCH_AUDIT_DB") ?? ":memory:";
        var db = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path }.ToString());
        await db.OpenAsync();
        var descriptors = new[] { "Child", "Payment" }.Select(name => EntityDescriptor.New(name)
            .TableName("batch_audit_" + name.ToLowerInvariant())
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("name", DataType.Text)).AuditMaskFields(new() { "name" })).ToArray();
        var module = new RuntimeModule(); foreach (var descriptor in descriptors) module.Entity(descriptor);
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(db),
            new MetadataSchemaProvider(name => descriptors.Single(item => item.Name == name)));
        var sink = new Sink(); var transactions = new Transactions(provider);
        sink.ActiveTransactions = () => transactions.Active;
        var context = module.IntoContext().WithDataService(provider)
            .WithDiagnosticSqlLogSink(sink).WithAppAuditEventSink(sink);
        await context.EnsureSchemaAsync();
        context.InsertResource<ITransactionExecutor>(transactions);
        using var next = db.CreateCommand();
        next.CommandText = "SELECT COALESCE(MAX(id), 0) + 10 FROM batch_audit_child";
        return new(db, context, sink, transactions, Convert.ToInt64(await next.ExecuteScalarAsync()));
    }

    private static MutationRequest Insert(string entity, long id, string name) => new InsertMutationRequest(
        new InsertCommand(entity).Value("id", id).Value("name", name), "local construction intent") {
            LedgerKey = new EntityKey(entity, Value.FromObject(id)) };
    private static MutationRequest Update(string entity, long id, string name, long version = 1) => new UpdateMutationRequest(
        new UpdateCommand(entity, Value.FromObject(id)).ExpectedVersion(version).Value("name", name), "local update intent") {
            LedgerKey = new EntityKey(entity, Value.FromObject(id)) };
    private static IEnumerable<ExecutionMetadata> Leaves(ExecutionMetadata metadata) => metadata.Statements.Count == 0
        ? new[] { metadata } : metadata.Statements.SelectMany(Leaves);
    private static string Shape(IEnumerable<TraceNode> nodes) => string.Join(" -> ", nodes.Select(node =>
        $"{node.Name}#{node.EntityId}:{node.Detail}"));
    private static async Task<Record?> Stored(UserContext context, string entity, long id) =>
        (await context.RequireResource<IDataService>().QueryAsync(new QueryRequest(new SelectQuery(entity)
            .Filter(Expr.Eq("id", Value.FromObject(id))).Limit(1).Comment("verify batch persistence")
            .Purpose("read authoritative native fixture state")))).Rows.SingleOrDefault();

    [Theory]
    [InlineData(false, false, false)] [InlineData(false, false, true)]
    [InlineData(false, true, false)] [InlineData(false, true, true)]
    [InlineData(true, false, false)] [InlineData(true, false, true)]
    [InlineData(true, true, false)] [InlineData(true, true, true)]
    public async Task BatchCommitRetainsOrderedTypedItemAuditAndPrivateRootIntent(bool logging, bool nested, bool updating)
    {
        var fixture = await Open(); await using var db = fixture.Db;
        var (context, sink, transactions, id) = (fixture.Context, fixture.Sink, fixture.Transactions, fixture.Id);
        var items = new[] { (Entity: "Child", Id: id + 1, Name: "SECOND-PRIVATE"),
            (Entity: "Payment", Id: id + 1, Name: "PAYMENT-PRIVATE"), (Entity: "Child", Id: id, Name: "FUTURE-PRIVATE") };
        if (updating) foreach (var item in items)
            await context.RequireResource<IDataService>().MutateAsync(Insert(item.Entity, item.Id, "old private name"));
        sink.Clear(); context.EnableQuerySqlLog(logging).EnableMutationSqlLog(logging);
        const string comment = "save FUTURE-PRIVATE and SECOND-PRIVATE as ordered batch";
        MutationResult? result = null;
        var expected = new List<string>();
        await context.ExecuteGraphSaveAsync(comment, async graph => {
            var root = graph.Scope("Parent", 100, null);
            var requests = new List<MutationRequest>();
            for (var index = 0; index < items.Length; index++)
            {
                var item = items[index];
                var request = updating ? Update(item.Entity, item.Id, item.Name) : Insert(item.Entity, item.Id, item.Name);
                graph.Preflight(request);
                var scope = graph.Scope(item.Entity, (ulong)item.Id, "responsibility " + index, root);
                requests.Add(graph.Request(request, scope)); expected.Add(Shape(scope.Recover()));
            }
            // Zero affected rows must not manufacture a committed item event.
            var zero = Update("Child", id + 9, "zero affected private value"); graph.Preflight(zero);
            requests.Add(graph.Request(zero, graph.Scope("Child", (ulong)(id + 9), "no matched row", root)));
            if (nested) requests = new() { new BatchMutationRequest(requests.Take(2).ToList(), "nested prose is not root intent"),
                new BatchMutationRequest(requests.Skip(2).ToList(), "second nested prose") };
            result = await graph.MutateAsync(new BatchMutationRequest(requests, "container construction intent"));
            Assert.Empty(sink.Audit);
            // A caller can still mutate a command object before commit. Audit
            // delivery must use the already projected execution snapshot.
            void ChangeCaller(MutationRequest request)
            {
                if (request is BatchMutationRequest batch) foreach (var child in batch.Requests) ChangeCaller(child);
                else if (request is InsertMutationRequest insert) insert.Command.Values["name"] = Value.FromObject("changed after provider return");
                else if (request is UpdateMutationRequest update) update.Command.Values["name"] = Value.FromObject("changed after provider return");
            }
            foreach (var request in requests) ChangeCaller(request);
            return true;
        });
        Assert.Equal((1, 1, 0, 0), (transactions.Begins, transactions.Commits, transactions.Rollbacks, transactions.Active));
        Assert.IsType<BatchMutationRequest>(Assert.Single(transactions.Requests));
        Assert.Equal(3UL, result!.AffectedRows); Assert.Empty(result.GeneratedValues); Assert.Null(result.PersistedRecord);
        Assert.Equal(DataServiceOperation.Batch, result.Metadata.Operation);
        var physical = Leaves(result.Metadata).ToArray(); Assert.Equal(7, physical.Length);
        for (var index = 0; index < 3; index++)
        {
            Assert.Equal(expected[index], Shape(physical[index * 2].MutationLineage));
            Assert.Equal(expected[index], Shape(physical[index * 2 + 1].MutationLineage));
            Assert.Contains(Value.FromObject(items[index].Name), physical[index * 2].Parameters);
            Assert.Equal(DataServiceOperation.Query, physical[index * 2 + 1].Operation);
        }
        Assert.All(physical, statement => Assert.Equal(comment, statement.Comment));
        Assert.Equal(0UL, physical.Last().AffectedRows);
        Assert.Equal(logging ? 7 : 0, sink.Sql.Count);
        Console.WriteLine("BATCH BEFORE AUDIT ASSERT " + JsonSerializer.Serialize(new {
            logging, nested, updating, begins = transactions.Begins, commits = transactions.Commits,
            rollbacks = transactions.Rollbacks, audit = sink.Audit, sql = sink.Sql }));
        Assert.Equal(3, sink.Audit.Count);
        for (var index = 0; index < 3; index++)
        {
            var audit = sink.Audit[index]; var item = items[index];
            Assert.Equal(item.Entity, audit["entityType"]); Assert.Equal(item.Id, Convert.ToInt64(audit["entityId"]));
            Assert.Equal(updating ? "update" : "create", audit["mutationKind"]);
            Assert.Equal(updating ? 2L : 1L, Convert.ToInt64(audit["resultVersion"])); Assert.Equal(1UL, audit["affectedRows"]);
            var lineage = Assert.IsAssignableFrom<IEnumerable<TraceNode>>(audit["traceChain"]).ToArray();
            Assert.Equal(new[] { "Parent", item.Entity }, lineage.Select(node => node.Name));
            Assert.Equal(new ulong?[] { 100, (ulong)item.Id }, lineage.Select(node => node.EntityId));
            Assert.Equal("responsibility " + index, lineage[1].Detail);
            Assert.Equal(audit["reason"], lineage[0].Detail);
        }
        var safe = JsonSerializer.Serialize(new { sql = sink.Sql, audit = sink.Audit });
        Assert.DoesNotContain("FUTURE-PRIVATE", safe); Assert.DoesNotContain("SECOND-PRIVATE", safe);
        Assert.DoesNotContain("BatchItems", JsonSerializer.Serialize(result));
        Assert.DoesNotContain("changed after provider return", safe);
        foreach (var item in items)
        {
            var row = await Stored(context, item.Entity, item.Id); Assert.NotNull(row);
            Assert.Equal(item.Name, row!["name"].Raw); Assert.Equal(updating ? 2L : 1L, row["version"].TryI64());
        }
        sink.Clear(); context.EnableQuerySqlLog();
        await context.RequireResource<IDataService>().QueryAsync(new QueryRequest(new SelectQuery("Child").Limit(1)
            .Comment("independent FUTURE-PRIVATE and SECOND-PRIVATE").Purpose("verify batch privacy does not persist")));
        Assert.Equal("independent FUTURE-PRIVATE and SECOND-PRIVATE", Assert.Single(sink.Sql).Comment);
    }

    [Theory]
    [InlineData(false, false)] [InlineData(false, true)]
    [InlineData(true, false)] [InlineData(true, true)]
    public async Task RollbackDropsEveryNestedBatchAuditAndNextGraphSucceeds(bool logging, bool failAfterBatch)
    {
        var fixture = await Open(); await using var db = fixture.Db;
        var (context, sink, transactions, id) = (fixture.Context, fixture.Sink, fixture.Transactions, fixture.Id);
        context.EnableQuerySqlLog(logging).EnableMutationSqlLog(logging);
        var operation = context.ExecuteGraphSaveAsync<bool>("rollback LATER-PRIVATE", async graph => {
            var root = graph.Scope("Parent", 100, null);
            MutationRequest Capture(string entity, long identity, string value, string reason) =>
                graph.Request(Insert(entity, identity, value), graph.Scope(entity, (ulong)identity, reason, root));
            var first = Capture("Child", id, "earlier private value", "first responsibility");
            var second = Capture("Payment", id, "LATER-PRIVATE", "second responsibility");
            var tail = new List<MutationRequest> { second };
            if (!failAfterBatch) tail.Add(Capture("Child", id, "duplicate must fail", "failed responsibility"));
            var batch = new BatchMutationRequest(new() {
                new BatchMutationRequest(new() { first }, "nested first"), new BatchMutationRequest(tail, "nested later")
            }, "nested container");
            graph.Preflight(batch);
            var result = await graph.MutateAsync(batch);
            Assert.Equal(2UL, result.AffectedRows); Assert.Empty(sink.Audit);
            throw new InvalidOperationException("fail after successful batch before commit");
        });
        if (failAfterBatch) await Assert.ThrowsAsync<InvalidOperationException>(() => operation);
        else await Assert.ThrowsAsync<SqlExecutorException>(() => operation); // Real SQLite duplicate primary key.
        Assert.Equal((1, 0, 1, 0), (transactions.Begins, transactions.Commits, transactions.Rollbacks, transactions.Active));
        Assert.Empty(sink.Audit); Assert.Equal(0, sink.Attempts);
        Assert.Equal(logging ? (failAfterBatch ? 4 : 5) : 0, sink.Sql.Count);
        if (logging && !failAfterBatch) Assert.Equal("failure", sink.Sql.Last().ExecutionOutcome);
        Assert.DoesNotContain("LATER-PRIVATE", JsonSerializer.Serialize(sink.Sql));
        Console.WriteLine("BATCH ROLLBACK " + JsonSerializer.Serialize(new {
            logging, failAfterBatch, begins = transactions.Begins, commits = transactions.Commits,
            rollbacks = transactions.Rollbacks, auditCount = sink.Audit.Count, sql = sink.Sql }));
        Assert.Null(await Stored(context, "Child", id)); Assert.Null(await Stored(context, "Payment", id));
        sink.Clear();
        await context.ExecuteGraphSaveAsync("independent LATER-PRIVATE", async graph => {
            var scope = graph.Scope("Child", (ulong)id, null);
            var batch = new BatchMutationRequest(new() { graph.Request(Insert("Child", id, "next ordinary value"), scope) }, "next container");
            graph.Preflight(batch); return await graph.MutateAsync(batch);
        });
        Assert.Equal((2, 1, 1, 0), (transactions.Begins, transactions.Commits, transactions.Rollbacks, transactions.Active));
        Assert.Equal("independent LATER-PRIVATE", Assert.Single(sink.Audit)["reason"]);
        Assert.Equal("next ordinary value", (await Stored(context, "Child", id))!["name"].Raw);
    }

    [Theory]
    [InlineData(false, false)] [InlineData(false, true)]
    [InlineData(true, false)] [InlineData(true, true)]
    public async Task DirectBatchKeepsRepeatedTypedKeyResultsAndAttemptsAllCommittedSinks(bool logging, bool failingSink)
    {
        var fixture = await Open(); await using var db = fixture.Db;
        var (context, sink, id) = (fixture.Context, fixture.Sink, fixture.Id);
        context.EnableQuerySqlLog(logging).EnableMutationSqlLog(logging);
        var failure = new InvalidOperationException("first committed audit sink failed");
        if (failingSink) sink.Failure = _ => sink.Attempts == 1 ? failure : null;
        var batch = new BatchMutationRequest(new() {
            Insert("Child", id, "FIRST-PRIVATE"),
            new BatchMutationRequest(new() { Update("Child", id, "NEXT-PRIVATE"), Insert("Payment", id, "PAYMENT-PRIVATE") }, "ignored nested prose")
        }, "direct FIRST-PRIVATE to NEXT-PRIVATE with PAYMENT-PRIVATE");
        var operation = context.RequireResource<IDataService>().MutateAsync(batch);
        if (failingSink) Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => operation));
        else
        {
            var result = await operation; Assert.Equal(3UL, result.AffectedRows); Assert.Null(result.PersistedRecord);
            Assert.Empty(result.GeneratedValues); Assert.Equal(6, Leaves(result.Metadata).Count());
        }
        Assert.Equal(3, sink.Attempts); Assert.Equal(failingSink ? 2 : 3, sink.Audit.Count);
        var allVersions = new long[] { 1, 2, 1 }.Skip(failingSink ? 1 : 0).ToArray();
        Assert.Equal(allVersions, sink.Audit.Select(item => Convert.ToInt64(item["resultVersion"])));
        Assert.Equal(new[] { "Child", "Child", "Payment" }.Skip(failingSink ? 1 : 0), sink.Audit.Select(item => item["entityType"]));
        Assert.All(sink.Audit, item => Assert.Equal(id, Convert.ToInt64(item["entityId"])));
        Assert.Equal(logging ? 6 : 0, sink.Sql.Count);
        var safe = JsonSerializer.Serialize(new { sql = sink.Sql, audit = sink.Audit });
        foreach (var secret in new[] { "FIRST-PRIVATE", "NEXT-PRIVATE", "PAYMENT-PRIVATE" }) Assert.DoesNotContain(secret, safe);
        Console.WriteLine("DIRECT BATCH " + JsonSerializer.Serialize(new { logging, failingSink, attempts = sink.Attempts, audit = sink.Audit, sql = sink.Sql }));
        // A post-commit sink failure cannot undo or re-run the already persisted SQL.
        Assert.Equal("NEXT-PRIVATE", (await Stored(context, "Child", id))!["name"].Raw);
        Assert.Equal(2L, (await Stored(context, "Child", id))!["version"].TryI64());
        Assert.Equal("PAYMENT-PRIVATE", (await Stored(context, "Payment", id))!["name"].Raw);
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public async Task DirectBatchSnapshotsAllLeafAuditsBeforeFirstSinkCanMutateLaterCommands(bool nested)
    {
        var fixture = await Open(); await using var db = fixture.Db;
        var (context, sink, id) = (fixture.Context, fixture.Sink, fixture.Id);
        var first = Insert("Child", id, "first stored value");
        var second = Assert.IsType<InsertMutationRequest>(Insert("Child", id + 1, "BATCH-PRIVATE"));
        var third = Assert.IsType<InsertMutationRequest>(Insert("Payment", id, "third stored value"));
        sink.Failure = _ => {
            if (sink.Attempts == 1)
            {
                // Adversarial caller mutation AFTER SQLite completed. This is
                // not expected provenance and must never enter committed audit.
                foreach (var request in new[] { second, third })
                {
                    request.Command.Values.Remove("name");
                    request.Command.Values["late_caller_field"] = Value.FromObject("not executed");
                    request.Command.TraceChain.AddRange(new MutationTraceScope(request.Command.Entity,
                        request.LedgerKey!.Id.TryU64(), "post-execution local reason").Recover());
                }
            }
            return null;
        };
        var children = nested ? new List<MutationRequest> { first,
            new BatchMutationRequest(new() { second, third }, "nested caller intent") }
            : new List<MutationRequest> { first, second, third };
        await context.RequireResource<IDataService>().MutateAsync(new BatchMutationRequest(children, "stable BATCH-PRIVATE reason"));
        Assert.Equal(3, sink.Attempts); Assert.Equal(3, sink.Audit.Count);
        Console.WriteLine("DIRECT BATCH CALLER MUTATION " + JsonSerializer.Serialize(new { nested, audit = sink.Audit }));
        Assert.All(sink.Audit, item => {
            Assert.Equal(new[] { "id", "name", "version" }, Assert.IsType<string[]>(item["changedFields"]));
            var node = Assert.Single(Assert.IsAssignableFrom<IEnumerable<TraceNode>>(item["traceChain"]));
            Assert.Equal(item["reason"], node.Detail);
        });
        Assert.DoesNotContain("post-execution", JsonSerializer.Serialize(sink.Audit));
        Assert.DoesNotContain("BATCH-PRIVATE", JsonSerializer.Serialize(sink.Audit));
        Assert.Equal("BATCH-PRIVATE", (await Stored(context, "Child", id + 1))!["name"].Raw);
        Assert.Equal("third stored value", (await Stored(context, "Payment", id))!["name"].Raw);
    }
}
