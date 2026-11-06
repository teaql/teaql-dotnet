using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

// Application-owned regression. Two generated requests overlap; the single
// SQLite connection is intentionally serialized, not advertised as parallel SQL.
static class ConcurrentQueryProbe
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }
    static object[] Nodes(IEnumerable<TraceNode> nodes) => nodes.Select(n => (object)new {
        kind = n.Kind, name = n.Name, comment = n.Comment, detail = n.Detail
    }).ToArray();

    public static async Task Verify(string filename)
    {
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module; var transport = new Transport(new SqliteTransport(connection));
        var service = new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module));
        var sink = new Sink();
        var context = module.IntoContext().WithDataService(service).WithDiagnosticSqlLogSink(sink)
            .WithAppAuditEventSink(sink).WithBusinessClock(new FixedBusinessClock(new DateTimeOffset(2032, 2, 29, 10, 15, 30, TimeSpan.Zero)));
        await context.EnsureSchemaAsync();
        var previous = (await Q.PurchaseOrders().Limit(5).Comment("Determine the concurrent-query example round")
            .Purpose("Retain two rounds without resetting the SQLite database").ExecuteForListAsync(context)).Count;
        Require(previous is 0 or 2, "Unexpected concurrent-query round boundary");
        var round = previous / 2 + 1;
        sink.Clear(); transport.Reset();
        var ids = new Dictionary<string, ulong>(); var childIds = new Dictionary<string, ulong>();
        foreach (var label in new[] { "alpha", "beta" })
        {
            var parent = Q.PurchaseOrders().Comment("Prepare the " + label + " query fixture")
                .Purpose("Use generated audited graph mutation before concurrent reads").NewEntity(context);
            parent.UpdateContactPhone(label == "alpha" ? "5550701" : "5550702"); parent.UpdateInternalNote(null!);
            var child = Q.OrderItems().Comment("Prepare the " + label + " query leaf")
                .Purpose("Read the persisted child identity through generated Q and E").NewEntity(context);
            child.UpdateProductName(label + " child"); child.UpdateUnitPrice(1.00m); parent.OrderItemList.Add(child);
            await parent.AuditAs("Create the " + label + " concurrent-query graph").SaveAsync(context);
            ids[label] = parent.Id ?? throw new InvalidOperationException("Missing parent ID");
            var seeded = (await Q.PurchaseOrders().WithIdIs(ids[label])
                .SelectOrderItemListWith(Q.OrderItems().Limit(10).Comment("Read the saved " + label + " child"))
                .Limit(1).Comment("Read back the " + label + " graph").Purpose("Never infer child identity from allocation order")
                .ExecuteForListAsync(context)).Single();
            childIds[label] = E.PurchaseOrder(seeded).OrderItemList().First().Id().Eval()
                ?? throw new InvalidOperationException("Missing child ID");
        }
        Require(sink.Audits == 4 && transport.Reads > 0 && transport.Writes > 0, "Creation did not calibrate real I/O and audit observers");
        using var overlapping = new OverlappingService(service);
        context.WithDataService(overlapping);
        var contexts = new ConcurrentQueue<UserContext>();
        async Task<PurchaseOrder> Load(string label, ulong id, UserContext supplied)
        {
            contexts.Enqueue(supplied);
            return (await Q.PurchaseOrders().WithIdIs(id)
                .SelectOrderItemListWith(Q.OrderItems().Limit(10).Comment(label + " children"))
                .Limit(1).Comment(label + " root").Purpose(label + " purpose").ExecuteForListAsync(supplied)).Single();
        }
        _ = await Load("calibration", ids["alpha"], context);
        contexts.Clear(); sink.Clear(); transport.Reset(); overlapping.Clear();
        var baseline = new ContextSnapshot(context);
        var alphaTask = Load("alpha", ids["alpha"], context);
        var betaTask = Load("beta", ids["beta"], context);
        bool bothLive = false, unchangedWhileLive = false, distinctRoots = false;
        object[] checkpoints = Array.Empty<object>();
        try
        {
            await overlapping.BothEntered.Task.WaitAsync(TimeSpan.FromSeconds(10));
            bothLive = !alphaTask.IsCompleted && !betaTask.IsCompleted;
            var requests = overlapping.Requests.OrderBy(r => r.Comment, StringComparer.Ordinal).ToArray();
            distinctRoots = requests.Length == 2 && !ReferenceEquals(requests[0].Scope.Root, requests[1].Scope.Root);
            checkpoints = requests.Select(r => (object)new { comment = r.Comment, purpose = r.Purpose,
                parentAbsent = r.Scope.Parent is null, rootIsSelf = ReferenceEquals(r.Scope, r.Scope.Root), nodes = Nodes(r.Scope.Recover()) }).ToArray();
            unchangedWhileLive = baseline.Matches(context);
        }
        finally
        {
            overlapping.Release.TrySetResult();
            // Always settle both task lifetimes before the provider/connection is disposed.
            await Task.WhenAll(alphaTask, betaTask);
        }
        var alpha = await alphaTask; var beta = await betaTask; var after = await Load("after", ids["alpha"], context);
        var beforeE = transport.Reads; var beforeProviderE = overlapping.Calls; var beforeLogsE = sink.Sql.Count;
        var values = new Dictionary<string, object>();
        foreach (var (label, row) in new[] { ("alpha", alpha), ("beta", beta), ("after", after) })
            values[label] = new { id = row.Id, phone = E.PurchaseOrder(row).ContactPhone().Eval(),
                count = E.PurchaseOrder(row).OrderItemList().Size().Eval(), product = E.PurchaseOrder(row).OrderItemList().First().ProductName().Eval() };
        var eProviderReads = overlapping.Calls - beforeProviderE; var eSqlReads = transport.Reads - beforeE; var eLogEntries = sink.Sql.Count - beforeLogsE;
        var sameContext = contexts.Count == 3 && contexts.All(c => ReferenceEquals(c, context));
        var events = sink.Sql.Select(m => new { operation = m.Operation.ToString(), outcome = m.ExecutionOutcome, resultCount = m.ResultCount,
            comment = m.Comment, purpose = m.Purpose, sql = m.ParameterizedQuery, business = Nodes(m.RequestTraceChain), physical = Nodes(m.TraceChain) }).ToArray();
        var unchangedAfter = baseline.Matches(context);
        var observation = new { language = "dotnet", round, sameContext, distinctRoots, bothLive, overlapEntered = overlapping.OverlapEntered,
            checkpoints, contextStateUnchangedWhileLive = unchangedWhileLive, contextStateUnchangedAfter = unchangedAfter,
            contextFieldCount = baseline.FieldCount, providerRootCalls = overlapping.Calls, physicalQueries = transport.Reads,
            physicalWrites = transport.Writes, eProviderReads, eSqlReads, eLogEntries, auditEvents = sink.Audits, ids, childIds, values, events,
            serializedSqliteConnection = true, generatedImplementationsModified = false };
        Console.WriteLine("DOTNET_CONCURRENT_QUERY_OBSERVATION=" + JsonSerializer.Serialize(observation));
        Require(bothLive && sameContext && distinctRoots && overlapping.OverlapEntered == 2, "Generated requests did not genuinely overlap with independent scopes");
        Require(unchangedWhileLive && unchangedAfter, "A request wrote ambient Context state");
        Require(overlapping.Calls == 3 && transport.Reads == 6 && sink.Sql.Count == 6 && transport.Writes == 0 && sink.Audits == 0,
            "Wrong physical query/provider/audit cardinality");
        Require(eProviderReads == 0 && eSqlReads == 0 && eLogEntries == 0, "Loaded E traversal performed hidden I/O");
        foreach (var label in new[] { "alpha", "beta", "after" })
        {
            var pair = sink.Sql.Where(m => m.Purpose == label + " purpose").ToArray();
            Require(pair.Length == 2 && pair.All(m => m.ExecutionOutcome == "success" && m.ResultCount == 1), "Missing real root/child statement");
            var root = pair.Single(m => m.RequestTraceChain.Count == 2); var leaf = pair.Single(m => m.RequestTraceChain.Count == 4);
            Require(root.Comment == label + " root" && leaf.Comment == label + " children", "SQL envelope lost its request-local comment");
            Require(leaf.RequestTraceChain[0].Comment == label + " purpose" && leaf.RequestTraceChain[1].Comment == label + " root"
                && leaf.RequestTraceChain[2].Name == "SelectOrderItemListWith" && leaf.RequestTraceChain[2].Comment == "SelectOrderItemListWith(...)"
                && leaf.RequestTraceChain[3].Comment == label + " children", "Business ancestry crossed independent requests");
        }
        Console.WriteLine("DOTNET_CONCURRENT_QUERY=" + JsonSerializer.Serialize(new { passed = true, observation }));
    }

    sealed class ContextSnapshot
    {
        readonly Dictionary<FieldInfo, object?> fields;
        readonly Dictionary<string, (object Key, object? Value)[]> maps;
        public int FieldCount => fields.Count;
        public ContextSnapshot(UserContext context)
        {
            fields = typeof(UserContext).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToDictionary(f => f, f => f.GetValue(context));
            Require(!fields.Keys.Any(f => f.FieldType == typeof(QueryRequestScope) || f.FieldType == typeof(QueryIntent)
                || f.FieldType == typeof(QueryRequest) || f.FieldType == typeof(Stack<TraceNode>) || f.FieldType == typeof(List<TraceNode>)),
                "Context must not own query scope/intent or a mutable trace stack");
            maps = new[] { "_typedResources", "_namedResources", "_locals" }.ToDictionary(n => n, n => Entries(fields.Single(f => f.Key.Name == n).Value));
        }
        static (object, object?)[] Entries(object? map) => ((IEnumerable)(map ?? throw new InvalidOperationException("Missing Context dictionary")))
            .Cast<object>().Select(v => (v.GetType().GetProperty("Key")!.GetValue(v)!, v.GetType().GetProperty("Value")!.GetValue(v))).ToArray();
        public bool Matches(UserContext context) => fields.All(f => f.Key.FieldType.IsValueType
            ? Equals(f.Value, f.Key.GetValue(context)) : ReferenceEquals(f.Value, f.Key.GetValue(context)))
            && maps.All(m => { var current = Entries(fields.Single(f => f.Key.Name == m.Key).Key.GetValue(context));
                return current.Length == m.Value.Length && m.Value.All(v => current.Any(c => Equals(c.Item1, v.Key) && ReferenceEquals(c.Item2, v.Value))); });
    }
    sealed class Schema(RuntimeModule module) : ISchemaProvider
    { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
    sealed class Sink : IDiagnosticSqlLogSink, IAppAuditEventSink
    {
        readonly ConcurrentQueue<ExecutionMetadata> sql = new(); int audits;
        public IReadOnlyList<ExecutionMetadata> Sql => sql.ToArray(); public int Audits => audits;
        public void Write(ExecutionMetadata metadata) => sql.Enqueue(metadata);
        public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default)
        { Interlocked.Increment(ref audits); return Task.CompletedTask; }
        public void Clear() { sql.Clear(); audits = 0; }
    }
    sealed class Transport(SqliteTransport inner) : IAutomaticMutationTransactionTransport
    {
        int reads, writes; public int Reads => reads; public int Writes => writes;
        public void Reset() { reads = 0; writes = 0; }
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Interlocked.Increment(ref reads); return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Interlocked.Increment(ref writes); return inner.ExecuteSqlAsync(query); }
        public async Task<ISqlTransaction> BeginSqlAsync() => new Transaction(this, await inner.BeginSqlAsync());
        sealed class Transaction(Transport owner, ISqlTransaction inner) : ISqlTransaction
        {
            public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Interlocked.Increment(ref owner.reads); return inner.FetchAllSqlAsync(query); }
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Interlocked.Increment(ref owner.writes); return inner.ExecuteSqlAsync(query); }
            public Task CommitSqlAsync() => inner.CommitSqlAsync();
            public Task RollbackSqlAsync() => inner.RollbackSqlAsync();
            public void Dispose() => inner.Dispose();
        }
    }
    sealed class OverlappingService(IDataService inner) : IDataService, IDisposable
    {
        readonly SemaphoreSlim singleConnection = new(1, 1); readonly ConcurrentQueue<QueryRequest> requests = new(); int calls, entered;
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public TaskCompletionSource BothEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public IReadOnlyList<QueryRequest> Requests => requests.ToArray(); public int Calls => calls; public int OverlapEntered => entered;
        public void Clear() { calls = 0; requests.Clear(); }
        public async Task<QueryResult> QueryAsync(QueryRequest request)
        {
            Interlocked.Increment(ref calls);
            if (request.Comment is "alpha root" or "beta root")
            {
                requests.Enqueue(request); if (Interlocked.Increment(ref entered) == 2) BothEntered.TrySetResult();
                await Release.Task;
            }
            await singleConnection.WaitAsync();
            try { return await inner.QueryAsync(request); } finally { singleConnection.Release(); }
        }
        public Task<MutationResult> MutateAsync(MutationRequest request) => throw new InvalidOperationException("Concurrent query phase is read-only");
        public void Dispose() => singleConnection.Dispose();
    }
}
