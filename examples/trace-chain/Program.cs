using System.Collections.Concurrent;
using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

var database = args.Length == 2 && args[0] == "--database" ? Path.GetFullPath(args[1])
    : throw new ArgumentException("Use --database <persistent SQLite file>; the verifier never deletes it.");
Directory.CreateDirectory(Path.GetDirectoryName(database)!);
await using var connection = new SqliteConnection($"Data Source={database}");
await connection.OpenAsync();
var sink = new EvidenceSink();
var faults = new FaultTransport(new SqliteTransport(connection));
var module = GeneratedRuntimeModule.Module;
var provider = new SqlDataServiceExecutor(new SqliteDialect(), faults,
    new MetadataSchemaProvider(module.Metadata.GetEntity));
var context = module.IntoContext().WithDataService(provider)
    .WithDiagnosticSqlLogSink(sink).WithAppAuditEventSink(sink);
await context.EnsureSchemaAsync();
// This is fault-injection DDL, not application DML or manual seeding.
using (var unique = connection.CreateCommand())
{
    unique.CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS trace_payment_reference ON payment_data(reference_code)";
    await unique.ExecuteNonQueryAsync();
}
var capture = new CapturingExecutor(provider, sink);
context.InsertResource<ITransactionExecutor>(capture);
sink.ActiveTransactions = () => capture.ActiveTransactions;
var nonce = Guid.NewGuid().ToString("N");
var platform = await Q.Platforms().WithIdIs(1).Limit(1)
    .Comment("load the provided root").Purpose("reuse generated bootstrap").ExecuteForOneAsync(context)
    ?? throw new Exception("Generated bootstrap did not provision Platform#1");
Verify.Equal(1L, E.Platform(platform).Id().Eval(), "bootstrap identity");

var ownershipScenario = Environment.GetEnvironmentVariable("TEAQL_TRACE_CHAIN_SCENARIO");
if (ownershipScenario == "page")
{
    await PageChecks.RunAsync(context, capture, sink);
    return;
}
if (!string.IsNullOrEmpty(ownershipScenario))
{
    await SharedReferenceChecks.RunAsync(context, capture, sink, faults, ownershipScenario);
    return;
}
await IntentGate();
var graph = await NormativeGraph();
await ThreeLevelQuery(graph);
await ProviderRollback();
await ReadbackFailure();
await ConcurrentGraphs();
Console.WriteLine("PASS: .NET generated trace-chain 6 scenarios; same database retained; no generated edits");
await SharedReferenceChecks.RunAsync(context, capture, sink, faults);
await PageChecks.RunAsync(context, capture, sink);

CustomerOrder NewOrder(string label) => Q.CustomerOrders().Comment("prepare a test order")
    .Purpose("compose a generated graph").NewEntity(context)
    .UpdatePlatformId(1).UpdateOrderNumber(label).UpdateDescription("Draft detail");
OrderItem NewItem(string name) => Q.OrderItems().Comment("prepare an item")
    .Purpose("compose a generated child").NewEntity(context).UpdateName(name);
Payment NewPayment(string code) => Q.Payments().Comment("prepare a payment")
    .Purpose("compose a generated child").NewEntity(context).UpdateReferenceCode(code);
PaymentAttempt NewAttempt(string code) => Q.PaymentAttempts().Comment("prepare an attempt")
    .Purpose("compose a generated grandchild").NewEntity(context).UpdateReferenceCode(code);
Shipment NewShipment(string code) => Q.Shipments().Comment("prepare a shipment")
    .Purpose("compose a generated child").NewEntity(context).UpdateReferenceCode(code);

void ClearEvidence() { capture.Clear(); sink.Clear(); }
async Task IntentGate()
{
    ClearEvidence(); context.DisableQuerySqlLog().DisableMutationSqlLog();
    var order = NewOrder(nonce + "-intent");
    await Verify.Throws<RequestIntentException>(() => order.SaveAsync(context), "missing mutation reason");
    await Verify.Throws<RequestIntentException>(() => Task.FromResult(order.AuditAs(" \t")), "blank mutation reason");
    await Verify.Throws<RequestIntentException>(() => Q.CustomerOrders().Limit(1).Purpose("inspect")
        .ExecuteForListAsync(context), "missing query comment");
    await Verify.Throws<RequestIntentException>(() => Task.FromResult(Q.CustomerOrders().Limit(1)
        .Comment("inspect").Purpose(" ")), "blank query purpose");
    Verify.Equal(0, capture.BeginCount, "invalid intent must not begin a transaction");
    Verify.Equal(0, capture.Commands.Count, "invalid intent must not reach the provider");
    Verify.Equal(0, sink.Audit.Count, "invalid intent must not emit audit");
    context.EnableQuerySqlLog().EnableMutationSqlLog();
    Console.WriteLine("PASS TC-REQ: invalid intent is rejected with SQL logging disabled");
}

async Task<(CustomerOrder Order, Payment Payment, PaymentAttempt Attempt)> NormativeGraph()
{
    // Align independent DB allocator floors, never override generated object IDs.
    // This deliberately makes two types share an assigned number on every replay.
    var floor = Math.Max(await provider.NextIdAsync("CustomerOrder"), await provider.NextIdAsync("Payment"));
    await provider.EnsureIdFloorAsync("CustomerOrder", floor);
    await provider.EnsureIdFloorAsync("Payment", floor);
    var order = NewOrder(nonce + "-normative");
    order.OrderItemList.Add(NewItem("Available stock"));
    order.OrderItemList.Add(NewItem("Unavailable stock"));
    order.PaymentList.Add(NewPayment(nonce + "-normative-payment"));
    await order.AuditAs("prepare order").SaveAsync(context);
    var rootOnly = await Q.CustomerOrders().WithIdIs(order.Id!.Value).Limit(1)
        .Comment("load only the root for an independent revision").Purpose("distinguish same-ID optimistic versions")
        .ExecuteForOneAsync(context) ?? throw new Exception("Prepared root is missing");
    await rootOnly.UpdateDescription("Reviewed detail").AuditAs("review order root").SaveAsync(context);
    var current = await Q.CustomerOrders().WithIdIs(E.CustomerOrder(order).Id().Eval()!.Value)
        .SelectOrderItemListWith(Q.OrderItems().OrderByIdAscending().Limit(10))
        .SelectPaymentListWith(Q.Payments().OrderByIdAscending().Limit(10))
        .Limit(1).Comment("load full graph before mutation").Purpose("preserve original versions")
        .ExecuteForOneAsync(context) ?? throw new Exception("Prepared order is missing");
    Verify.Equal(2, E.CustomerOrder(current).OrderItemList().Size().Eval(), "loaded items");
    Verify.Equal(1, E.CustomerOrder(current).PaymentList().Size().Eval(), "loaded payments");
    var item = current.OrderItemList.Single(value => E.OrderItem(value).Name().Eval() == "Available stock");
    var removed = current.OrderItemList.Single(value => E.OrderItem(value).Name().Eval() == "Unavailable stock");
    var payment = current.PaymentList.Single();
    var orderId = E.CustomerOrder(current).Id().Eval()!.Value;
    var paymentId = E.Payment(payment).Id().Eval()!.Value;
    Verify.Equal(orderId, paymentId, "same numeric ID, distinct types");
    Verify.Equal(2L, E.CustomerOrder(current).Version().Eval(), "root original version");
    Verify.Equal(1L, E.Payment(payment).Version().Eval(), "payment has its own original version despite equal ID");
    current.UpdateDescription("Submitted detail"); item.UpdateName("Fulfilled stock");
    removed.MarkForDeletion().AuditAs("remove unavailable item");
    payment.UpdateReferenceCode(nonce + "-authorized").AuditAs("authorize payment");
    var attempt = NewAttempt(nonce + "-attempt"); payment.PaymentAttemptList.Add(attempt);
    var shipment = NewShipment(nonce + "-shipment").AuditAs("dispatch shipment");
    current.ShipmentList.Add(shipment);
    ClearEvidence();
    await current.AuditAs("submit order").SaveAsync(context);
    Verify.Equal(3L, E.CustomerOrder(current).Version().Eval(), "root version advances independently");
    Verify.Equal(2L, E.Payment(payment).Version().Eval(), "payment version advances independently");
    var root = $"CustomerOrder#{orderId}:submit order";
    var expected = new Dictionary<(string, long), string> {
        [("CustomerOrder", orderId)] = root,
        [("OrderItem", E.OrderItem(item).Id().Eval()!.Value)] = root,
        [("OrderItem", E.OrderItem(removed).Id().Eval()!.Value)] = root + $" -> OrderItem#{removed.Id}:remove unavailable item",
        [("Payment", paymentId)] = root + $" -> Payment#{paymentId}:authorize payment",
        [("PaymentAttempt", E.PaymentAttempt(attempt).Id().Eval()!.Value)] = root + $" -> Payment#{paymentId}:authorize payment",
        [("Shipment", E.Shipment(shipment).Id().Eval()!.Value)] = root + $" -> Shipment#{shipment.Id}:dispatch shipment"
    };
    Verify.Equal(6, capture.Commands.Count, "normative emitted commands");
    Verify.Equal(6, sink.Audit.Count, "normative committed audit events");
    foreach (var command in capture.Commands)
    {
        Verify.Equal(expected[(command.Entity, command.Id)], Verify.Shape(command.Lineage), "actual command lineage");
        Verify.Equal("submit order", command.Comment, "root request comment");
        Verify.That(command.Lineage.All(node => node.EntityId.HasValue), "assigned IDs in emitted command lineage");
        Verify.That(command.Lineage.All(node => node.Kind == "auditReason" && node.Comment == ""), "typed audit nodes keep reasons in Detail");
    }
    foreach (var record in sink.Sql.Where(value => value.Operation != DataServiceOperation.Query))
    {
        var entity = record.TraceChain.Single(value => value.Kind == "entity").Name;
        var command = capture.Commands.Single(value => value.Entity == entity &&
            Verify.Shape(value.Lineage) == Verify.Shape(record.MutationLineage));
        Verify.Equal(expected[(command.Entity, command.Id)], Verify.Shape(record.MutationLineage), "safe SQL lineage");
        Verify.Equal("CustomerOrder", record.TraceChain[0].Name, "physical mutation root");
        Verify.Equal(0, record.TraceChain.Count(value => value.Kind == "auditReason"), "no audit nodes in physical path");
        Verify.Equal(1, record.TraceChain.Count(value => value.Kind == "provider"), "one physical provider frame");
        Verify.Equal(1, record.TraceChain.Count(value => value.Kind == "sql"), "one physical SQL frame");
    }
    Verify.Equal(6, sink.Sql.Count(value => value.Operation != DataServiceOperation.Query), "physical mutation SQL count");
    foreach (var audit in sink.Audit)
    {
        var key = (audit["entityType"]!.ToString()!, Convert.ToInt64(audit["entityId"]));
        Verify.Equal(expected[key], Verify.Shape((IEnumerable<TraceNode>)audit["traceChain"]!), "committed audit lineage");
    }
    Verify.That(capture.Commands.Single(value => value.Id == removed.Id && value.Entity == "OrderItem").Operation == "delete",
        "deleted child went through deletion command");
    var hidden = await Q.OrderItems().WithIdIs(removed.Id!.Value).Limit(1)
        .Comment("check deleted row visibility").Purpose("verify deletion semantics").ExecuteForOneAsync(context);
    Verify.That(hidden is null, "normal queries hide the deleted child");
    Console.WriteLine("PASS TC-MUT-15: six actual commands, SQL metadata and committed audit lineages; assigned typed IDs");
    return (current, payment, attempt);
}

async Task ThreeLevelQuery((CustomerOrder Order, Payment Payment, PaymentAttempt Attempt) graph)
{
    ClearEvidence();
    var rows = await Q.PaymentAttemptsWithMinimalFields().WithIdIs(graph.Attempt.Id!.Value)
        .SelectReferenceCode().SelectPaymentWith(Q.PaymentsWithMinimalFields().SelectReferenceCode()
            .SelectCustomerOrderWith(Q.CustomerOrdersWithMinimalFields().SelectOrderNumber().SelectDescription()
                .SelectPlatformWith(Q.PlatformsWithMinimalFields().SelectName().Limit(1).Comment("nested platform prose"))
                .Limit(1).Comment("nested order prose"))
            .Limit(1).Comment("nested payment prose"))
        .Limit(1).Comment("load attempt detail").Purpose("render a governed three-level graph")
        .ExecuteForListAsync(context);
    Verify.Equal(1, rows.Count, "bounded root row");
    var payment = E.PaymentAttempt(rows[0]).Payment().Eval();
    var order = E.Payment(payment).CustomerOrder().Eval();
    var root = E.CustomerOrder(order).Platform().Eval();
    Verify.Equal("Trace Chain Verification", E.Platform(root).Name().Eval(), "loaded E traversal");
    Verify.Equal(graph.Order.Id, E.CustomerOrder(order).Id().Eval(), "related order identity");
    Verify.Equal(4, sink.Sql.Count, "root plus three relation SQL statements");
    var deepest = sink.Sql.Single(value => value.TraceChain.Count(node => node.Kind == "relation") == 3);
    Verify.Equal("PaymentAttempt", deepest.TraceChain[0].Name, "query origin retained through all levels");
    Verify.Equal("PaymentAttempt.Payment -> Payment.CustomerOrder -> CustomerOrder.Platform",
        string.Join(" -> ", deepest.TraceChain.Where(value => value.Kind == "relation").Select(value => value.Detail)), "qualified relation frames");
    Verify.Equal("load attempt detail", deepest.Comment, "root query comment");
    Verify.Equal("render a governed three-level graph", deepest.Purpose, "root query purpose");
    var projected = (await Q.PaymentAttemptsWithMinimalFields().WithIdIs(graph.Attempt.Id!.Value).Limit(1)
        .Comment("check incomplete projection").Purpose("ensure E fails closed").ExecuteForListAsync(context))[0];
    await Verify.Throws<TeaQLNotLoadedException>(() => Task.FromResult(E.PaymentAttempt(projected).Payment().Eval()), "NotLoaded E access");
    Console.WriteLine("PASS TC-SQL-07: generated Q/E creates three qualified relation frames without test-injected frames");
}

async Task ProviderRollback()
{
    var label = nonce + "-rollback";
    var order = NewOrder(label);
    var payment = NewPayment(label).AuditAs("authorize risky payment");
    order.PaymentList.Add(payment); order.PaymentList.Add(NewPayment(label).AuditAs("duplicate payment check"));
    ClearEvidence();
    await Verify.Throws<SqlExecutorException>(() => order.AuditAs("rollback order").SaveAsync(context), "actual SQLite UNIQUE failure");
    Verify.Equal(0, sink.Audit.Count, "rolled back graph has no committed audit");
    var failed = sink.Sql.Single(value => value.ExecutionOutcome == "failure" && value.Operation == DataServiceOperation.Insert);
    Verify.That(Verify.Shape(failed.MutationLineage).EndsWith(":duplicate payment check", StringComparison.Ordinal), "failed SQL preserves branch reason");
    Verify.Equal("CustomerOrder", failed.TraceChain[0].Name, "failed mutation origin");
    var rows = await Q.CustomerOrders().WithOrderNumberIs(label).Limit(1)
        .Comment("verify failed graph is absent").Purpose("check atomic rollback").ExecuteForListAsync(context);
    Verify.Equal(0, rows.Count, "failed graph was rolled back");
    Console.WriteLine("PASS TC-MUT-13: real UNIQUE failure retains attempted SQL lineage and emits no committed audit");
}

async Task ReadbackFailure()
{
    var order = NewOrder(nonce + "-readback");
    var payment = NewPayment(nonce + "-readback-payment").AuditAs("authorize readback payment");
    payment.PaymentAttemptList.Add(NewAttempt(nonce + "-readback-attempt")); order.PaymentList.Add(payment);
    ClearEvidence(); faults.FailAttemptReadback = true;
    try { await Verify.Throws<SqlExecutorException>(() => order.AuditAs("readback order").SaveAsync(context), "post-write fetch failure"); }
    finally { faults.FailAttemptReadback = false; }
    Verify.Equal(0, sink.Audit.Count, "failed refresh aborts graph commit");
    var write = sink.Sql.Single(value => value.Operation == DataServiceOperation.Insert &&
        value.TraceChain.Any(node => node.Kind == "entity" && node.Name == "PaymentAttempt"));
    var refresh = sink.Sql.Single(value => value.Operation == DataServiceOperation.Query && value.ExecutionOutcome == "failure");
    Verify.Equal("success", write.ExecutionOutcome, "successful physical write is not overwritten by refresh failure");
    Verify.Equal(Verify.Shape(write.MutationLineage), Verify.Shape(refresh.MutationLineage), "readback carries branch lineage");
    Verify.That(write.MutationLineage.Count == 2, "grandchild inherits payment reason");
    Verify.Equal("PaymentAttempt", refresh.TraceChain[0].Name, "independent authoritative refresh origin");
    Console.WriteLine("PASS TC-MUT-14: successful statement and failed readback remain separate; transaction rolls back");
}

async Task ConcurrentGraphs()
{
    ClearEvidence(); var first = NewOrder(nonce + "-first"); var second = NewOrder(nonce + "-second");
    first.PaymentList.Add(NewPayment(nonce + "-first-payment").AuditAs("first payment branch"));
    second.PaymentList.Add(NewPayment(nonce + "-second-payment").AuditAs("second payment branch"));
    var pause = capture.PauseNextBegin();
    var firstTask = Task.Run(() => first.AuditAs("first order branch").SaveAsync(context));
    await pause.Entered.Task.WaitAsync(TimeSpan.FromSeconds(10));
    var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var secondTask = Task.Run(async () => { started.SetResult(); return await second.AuditAs("second order branch").SaveAsync(context); });
    await started.Task.WaitAsync(TimeSpan.FromSeconds(10));
    Verify.Equal(1, capture.BeginCount, "second save waits on same Context transaction gate");
    pause.Release.SetResult(); await Task.WhenAll(firstTask, secondTask).WaitAsync(TimeSpan.FromSeconds(15));
    Verify.Equal(4, capture.Commands.Count, "two graphs each emit root and payment");
    Verify.Equal(4, sink.Audit.Count, "both graphs commit their own audit");
    foreach (var command in capture.Commands)
    {
        var firstGraph = command.Comment == "first order branch";
        Verify.Equal(firstGraph ? "first order branch" : "second order branch", command.Lineage[0].Detail, "independent root intent");
        if (command.Entity == "Payment")
            Verify.Equal(firstGraph ? "first payment branch" : "second payment branch", command.Lineage[1].Detail, "independent sibling scope");
    }
    foreach (var audit in sink.Audit)
    {
        var lineage = (IReadOnlyList<TraceNode>)audit["traceChain"]!;
        Verify.Equal(audit["reason"]!.ToString(), lineage[0].Detail, "committed concurrent origin");
    }
    Console.WriteLine("PASS TC-MUT-12: overlapping Task-based generated saves in one Context have isolated lineage");
}

static class Verify
{
    public static void That(bool condition, string message) { if (!condition) throw new Exception("ASSERT: " + message); }
    public static void Equal<T>(T expected, T actual, string message) => That(EqualityComparer<T>.Default.Equals(expected, actual),
        $"{message}; expected={expected}, actual={actual}");
    public static string Shape(IEnumerable<TraceNode> nodes) => string.Join(" -> ", nodes.Select(value => $"{value.Name}#{value.EntityId}:{value.Detail}"));
    public static async Task Throws<T>(Func<Task> work, string message) where T : Exception
    {
        try { await work(); } catch (T) { return; }
        throw new Exception("ASSERT: expected " + typeof(T).Name + " for " + message);
    }
}

sealed class EvidenceSink : IDiagnosticSqlLogSink, IAppAuditEventSink
{
    private readonly ConcurrentQueue<ExecutionMetadata> _sql = new();
    private readonly ConcurrentQueue<IReadOnlyDictionary<string, object?>> _audit = new();
    public IReadOnlyList<ExecutionMetadata> Sql => _sql.ToArray();
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Audit => _audit.ToArray();
    public Func<int>? ActiveTransactions { get; set; }
    public void Write(ExecutionMetadata metadata) => _sql.Enqueue(metadata);
    public Task RecordAsync(IReadOnlyDictionary<string, object?> record, CancellationToken token = default)
    {
        Verify.Equal(0, ActiveTransactions?.Invoke() ?? 0, "audit must follow successful database commit");
        _audit.Enqueue(record); return Task.CompletedTask;
    }
    public void Clear() { _sql.Clear(); _audit.Clear(); }
}

sealed record CommandObservation(string Entity, long Id, string Operation, string Comment, IReadOnlyList<TraceNode> Lineage,
    long? ExpectedVersion);
sealed class BeginPause
{
    public TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
}
sealed class CapturingExecutor(ITransactionExecutor inner, EvidenceSink sink) : ITransactionExecutor
{
    private readonly EvidenceSink _sink = sink;
    private readonly ConcurrentQueue<CommandObservation> _commands = new();
    private BeginPause? _pause;
    private int _begin, _active;
    public int BeginCount => _begin;
    public int ActiveTransactions => _active;
    public IReadOnlyList<CommandObservation> Commands => _commands.ToArray();
    public DataServiceCapabilities Capabilities => inner.Capabilities;
    public Task<QueryResult> QueryAsync(QueryRequest request) => inner.QueryAsync(request);
    public Task<MutationResult> MutateAsync(MutationRequest request) => inner.MutateAsync(request);
    public BeginPause PauseNextBegin() => _pause = new BeginPause();
    public void Clear() { _commands.Clear(); _begin = 0; }
    public async Task<ITransaction> BeginTransactionAsync()
    {
        Interlocked.Increment(ref _begin);
        var pause = Interlocked.Exchange(ref _pause, null);
        if (pause != null) { pause.Entered.SetResult(); await pause.Release.Task; }
        var transaction = await inner.BeginTransactionAsync(); Interlocked.Increment(ref _active);
        return new CaptureTransaction(this, transaction, _sink.Audit.Count);
    }
    private sealed class CaptureTransaction(CapturingExecutor owner, ITransaction inner, int auditCount)
        : ITransaction, IIdGeneratorExecutor
    {
        private bool _finished;
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Task<QueryResult> QueryAsync(QueryRequest request) => inner.QueryAsync(request);
        public Task<ulong> NextIdAsync(string entity) => ((IIdGeneratorExecutor)inner).NextIdAsync(entity);
        public Task EnsureIdFloorAsync(string entity, ulong floor) => ((IIdGeneratorExecutor)inner).EnsureIdFloorAsync(entity, floor);
        public async Task<MutationResult> MutateAsync(MutationRequest request)
        {
            var key = request.LedgerKey ?? throw new Exception("Generated mutation omitted its typed ledger key");
            var operation = request switch { InsertMutationRequest => "insert", UpdateMutationRequest => "update", DeleteMutationRequest => "delete", _ => "other" };
            var version = request switch { UpdateMutationRequest update => update.Command.ExpectedVersionValue,
                DeleteMutationRequest delete => delete.Command.Version.TryI64(), _ => (long?)null };
            owner._commands.Enqueue(new(key.EntityType, checked((long)key.Id.TryU64()!.Value), operation, request.Comment, request.MutationLineage, version));
            Verify.Equal(auditCount, owner._sink.Audit.Count, "no audit before graph commit");
            var result = await inner.MutateAsync(request);
            Verify.Equal(Verify.Shape(request.MutationLineage), Verify.Shape(result.Metadata.MutationLineage), "provider metadata matches actual request");
            Verify.Equal(auditCount, owner._sink.Audit.Count, "no audit immediately after a statement");
            return result;
        }
        public async Task CommitAsync() { await inner.CommitAsync(); Finish(); }
        public async Task RollbackAsync() { await inner.RollbackAsync(); Finish(); }
        private void Finish() { if (!_finished) { _finished = true; Interlocked.Decrement(ref owner._active); } }
        public void Dispose() { inner.Dispose(); Finish(); }
    }
}

sealed class FaultTransport(SqliteTransport inner) : IAutomaticMutationTransactionTransport, ISchemaConnectionInitializer
{
    public bool FailAttemptReadback { get; set; }
    public bool ReuseReadOnlyPlatformSnapshot { get; set; }
    public Record? SharedPlatformSnapshot { get; private set; }
    public int SharedPlatformUses { get; private set; }
    public Task EnsureSchemaFunctionsAsync() => inner.EnsureSchemaFunctionsAsync();
    public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
    {
        var rows = await inner.FetchAllSqlAsync(query);
        if (ReuseReadOnlyPlatformSnapshot && query.Sql.Contains("platform_data", StringComparison.Ordinal) && rows.Count == 1)
        {
            if (SharedPlatformSnapshot == null) SharedPlatformSnapshot = rows[0];
            Verify.Equal(SharedPlatformSnapshot.ToJsonValue().ToJsonString(), rows[0].ToJsonValue().ToJsonString(),
                "shared provider-loaded Platform snapshot has identical fields");
            rows[0] = SharedPlatformSnapshot;
            SharedPlatformUses++;
        }
        return rows;
    }
    public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
    public async Task<ISqlTransaction> BeginSqlAsync() => new FaultTransaction(this, await inner.BeginSqlAsync());
    private sealed class FaultTransaction(FaultTransport owner, ISqlTransaction inner) : ISqlTransaction
    {
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) => owner.FailAttemptReadback &&
            query.Sql.Contains("payment_attempt_data", StringComparison.Ordinal)
            ? Task.FromException<List<Record>>(new SqlExecutorException("Injected post-write authoritative fetch failure"))
            : inner.FetchAllSqlAsync(query);
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public Task CommitSqlAsync() => inner.CommitSqlAsync();
        public Task RollbackSqlAsync() => inner.RollbackSqlAsync();
        public void Dispose() => inner.Dispose();
    }
}
