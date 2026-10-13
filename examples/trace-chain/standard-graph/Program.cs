using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using TeaQL.Telemetry.OpenTelemetry;

// No SDK providers, manual spans, raw DML or synthetic telemetry records.
var database = Environment.GetEnvironmentVariable("TEAQL_STANDARD_GRAPH_DB")
    ?? Path.Combine(Path.GetTempPath(), "teaql-dotnet-standard-graph-" + Guid.NewGuid().ToString("N") + ".sqlite");
await using var connection = new SqliteConnection("Data Source=" + database);
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection), new MetadataSchemaProvider(module.Metadata.GetEntity));
var audit = new Audits(); var ordinary = new SqlLogs();
var context = module.IntoContext().WithDataService(provider).WithAppAuditEventSink(audit)
    .WithDiagnosticSqlLogSink(ordinary).DisableQuerySqlLog().DisableMutationSqlLog();
try
{
    await context.EnsureSchemaAsync(); var firstSeedAudits = audit.Count;
    Require(firstSeedAudits is 0 or 1, "first audited platform seed");
    await context.EnsureSchemaAsync(); Require(audit.Count == firstSeedAudits, "repeated seed is idempotent");
    var secret = "GRAPH-PRIVATE-" + Guid.NewGuid().ToString("N");
    var order = NewOrder(secret + "-simple");
    var payment = NewPayment(secret + "-payment").AuditAs("authorize standard payment");
    var attempt = NewAttempt(secret + "-attempt");
    payment.PaymentAttemptList.Add(attempt); order.PaymentList.Add(payment);
    await order.AuditAs("submit standard graph").SaveAsync(context);
    Require(audit.Count == firstSeedAudits + 3, "three independently committed audit records");
    var rows = await Q.PaymentAttemptsWithMinimalFields().WithIdIs(E.PaymentAttempt(attempt).Id().Eval()!.Value)
        .SelectReferenceCode().SelectPaymentWith(Q.PaymentsWithMinimalFields().SelectReferenceCode()
            .SelectCustomerOrderWith(Q.CustomerOrdersWithMinimalFields().SelectOrderNumber().SelectDescription()
                .SelectPlatformWith(Q.PlatformsWithMinimalFields().SelectName().Limit(1).Comment("load standard platform"))
                .Limit(1).Comment("load standard order"))
            .Limit(1).Comment("load standard payment"))
        .Limit(1).Comment("load standard attempt graph").Purpose("verify saved standard graph")
        .ExecuteForListAsync(context);
    Require(rows.Count == 1, "bounded root query");
    var loadedPayment = E.PaymentAttempt(rows[0]).Payment().Eval();
    var loadedOrder = E.Payment(loadedPayment).CustomerOrder().Eval();
    var loadedPlatform = E.CustomerOrder(loadedOrder).Platform().Eval();
    Require(E.CustomerOrder(loadedOrder).OrderNumber().Eval() == secret + "-simple", "typed saved order traversal");
    Require(E.Platform(loadedPlatform).Name().Eval() == "Trace Chain Verification", "four-level typed graph");

    // A separate graph covers update, deletion, new children and local reasons.
    var mixed = NewOrder(secret + "-mixed");
    mixed.OrderItemList.Add(NewItem(secret + "-available"));
    mixed.OrderItemList.Add(NewItem(secret + "-unavailable"));
    mixed.PaymentList.Add(NewPayment(secret + "-prepared"));
    await mixed.AuditAs("prepare standard mixed graph").SaveAsync(context);
    var rootOnly = await Q.CustomerOrders().WithIdIs(E.CustomerOrder(mixed).Id().Eval()!.Value).Limit(1)
        .Comment("load independent standard root revision").Purpose("distinguish typed optimistic versions")
        .ExecuteForOneAsync(context) ?? throw new InvalidOperationException("prepared root missing");
    await rootOnly.UpdateDescription("Reviewed detail").AuditAs("review standard mixed root").SaveAsync(context);
    var current = await Q.CustomerOrders().WithIdIs(E.CustomerOrder(mixed).Id().Eval()!.Value)
        .SelectOrderItemListWith(Q.OrderItems().OrderByIdAscending().Limit(10).Comment("load mutable items"))
        .SelectPaymentListWith(Q.Payments().OrderByIdAscending().Limit(10).Comment("load mutable payment"))
        .Limit(1).Comment("load standard mutable graph").Purpose("prepare a full mixed graph mutation")
        .ExecuteForOneAsync(context) ?? throw new InvalidOperationException("prepared graph missing");
    Require(E.CustomerOrder(current).OrderItemList().Size().Eval() == 2, "two loaded items");
    var ordinaryItem = current.OrderItemList.Single(item => E.OrderItem(item).Name().Eval() == secret + "-available");
    var removed = current.OrderItemList.Single(item => E.OrderItem(item).Name().Eval() == secret + "-unavailable");
    var currentPayment = current.PaymentList.Single();
    var rootId = E.CustomerOrder(current).Id().Eval();
    Require(rootId == E.Payment(currentPayment).Id().Eval(), "same numeric ID remains typed by entity kind");
    Require(E.CustomerOrder(current).Version().Eval() == 2 && E.Payment(currentPayment).Version().Eval() == 1,
        "same-ID parent and payment start with different original versions");
    current.UpdateDescription("Submitted detail"); ordinaryItem.UpdateName(secret + "-fulfilled");
    removed.MarkForDeletion().AuditAs("remove standard unavailable item");
    currentPayment.UpdateReferenceCode(secret + "-authorized").AuditAs("authorize standard mixed payment");
    currentPayment.PaymentAttemptList.Add(NewAttempt(secret + "-mixed-attempt"));
    current.ShipmentList.Add(NewShipment(secret + "-shipment").AuditAs("dispatch standard shipment"));
    var beforeMixed = audit.Count;
    await current.AuditAs("submit standard mixed graph").SaveAsync(context);
    Require(audit.Count == beforeMixed + 6, "six mixed mutations have committed audit");
    Require(E.CustomerOrder(current).Version().Eval() == 3 && E.Payment(currentPayment).Version().Eval() == 2, "typed independent optimistic versions");
    var deleted = await Q.OrderItems().WithIdIs(E.OrderItem(removed).Id().Eval()!.Value).Limit(1)
        .Comment("verify deleted standard item").Purpose("verify mixed graph effects").ExecuteForListAsync(context);
    Require(deleted.Count == 0, "deleted child absent after commit");
    Require(audit.Count == firstSeedAudits + 14 && ordinary.Count == 0, "independent durable audit with ordinary SQL log off");
    await context.RuntimeTelemetry.FlushAsync();
    if (args.Contains("--queue-pressure")) {
        Require(context.RuntimeTelemetry is StandardTelemetry, "pressure requires standard telemetry");
        var standard = (StandardTelemetry)context.RuntimeTelemetry;
        var control = Environment.GetEnvironmentVariable("TEAQL_STANDARD_PRESSURE_CONTROL")
            ?? throw new InvalidOperationException("controlled receiver barrier required");
        using var barriers = new HttpClient();
        // Test-only receiver barrier, not application instrumentation or SDK IO.
        (await barriers.GetAsync(control + "/phase/hold")).EnsureSuccessStatusCode();
        Console.WriteLine("DOTNET_PRESSURE_BASELINE " + System.Text.Json.JsonSerializer.Serialize(new {
            standard.ExportQueues, standard.ExportHealth, standard.QueueLosses
        }));
        var id = E.CustomerOrder(current).Id().Eval()!.Value;
        for (var index = 0; index < 1500; index++) {
            var pressureRows = await Q.CustomerOrdersWithMinimalFields().WithIdIs(id).SelectOrderNumber().Limit(1)
                .Comment("load bounded pressure order").Purpose("verify default SDK capacity")
                .ExecuteForListAsync(context);
            Require(pressureRows.Count == 1 && E.CustomerOrder(pressureRows[0]).OrderNumber().Eval() == secret + "-mixed", "pressure typed Q/E");
        }
        Console.WriteLine("DOTNET_QUEUE_PRESSURE " + System.Text.Json.JsonSerializer.Serialize(new {
            reads = 1500, standard.ExportQueues, standard.ExportHealth, standard.QueueLosses
        }));
        if (args.Contains("--shutdown-backlog")) {
            var clock = System.Diagnostics.Stopwatch.StartNew(); await standard.ShutdownAsync();
            Console.WriteLine("DOTNET_BACKLOG_SHUTDOWN " + System.Text.Json.JsonSerializer.Serialize(new {
                elapsedMs = clock.ElapsedMilliseconds, standard.ExportQueues, standard.ExportHealth, standard.QueueLosses, standard.ExportResources
            }));
        }
        else {
            (await barriers.GetAsync(control + "/phase/release")).EnsureSuccessStatusCode();
            await standard.FlushAsync();
            var recovered = await Q.CustomerOrdersWithMinimalFields().WithIdIs(id).SelectOrderNumber().Limit(1)
                .Comment("load recovered pressure order").Purpose("verify recovered capacity").ExecuteForListAsync(context);
            Require(recovered.Count == 1 && E.CustomerOrder(recovered[0]).OrderNumber().Eval() == secret + "-mixed", "recovered typed Q/E");
            await standard.FlushAsync();
            Console.WriteLine("DOTNET_QUEUE_RECOVERY " + System.Text.Json.JsonSerializer.Serialize(new {
                standard.ExportQueues, standard.ExportHealth, standard.QueueLosses, standard.ExportResources
            }));
            await standard.ShutdownAsync();
        }
        var lateRows = await Q.CustomerOrdersWithMinimalFields().WithIdIs(id).SelectOrderNumber().Limit(1)
            .Comment("load order after telemetry shutdown").Purpose("verify business continuity").ExecuteForListAsync(context);
        Require(lateRows.Count == 1 && E.CustomerOrder(lateRows[0]).OrderNumber().Eval() == secret + "-mixed", "late typed Q/E");
        var lateOrder = NewOrder(secret + "-after-shutdown");
        lateOrder.PaymentList.Add(NewPayment(secret + "-late-payment").AuditAs("authorize payment after telemetry shutdown"));
        await lateOrder.AuditAs("save after telemetry shutdown").SaveAsync(context);
        Require(audit.Count == firstSeedAudits + 16 && ordinary.Count == 0, "late audit independent of telemetry");
        Console.WriteLine("DOTNET_POST_SHUTDOWN_READ " + System.Text.Json.JsonSerializer.Serialize(new {
            rows = lateRows.Count, audits = audit.Count, standard.ExportQueues, standard.ExportHealth, standard.QueueLosses, standard.ExportResources
        }));
    }
    Console.WriteLine($"DOTNET_STANDARD_GRAPH PASSED savedEntities=3 mixedMutations=6 typedDepth=4 first_seed_audits={firstSeedAudits} repeat_seed_audits=0 audits={audit.Count} ordinary_sql_logs=0 database={database}");
}
finally { await context.RuntimeTelemetry.ShutdownAsync(); }
if (context.RuntimeTelemetry is StandardTelemetry finalPipeline)
    Console.WriteLine("DOTNET_STANDARD_EXPORT_HEALTH " + System.Text.Json.JsonSerializer.Serialize(new {
        finalPipeline.IsShutdown, finalPipeline.ExportHealth, finalPipeline.RejectedRecords, finalPipeline.ExportResponses,
        finalPipeline.ExportQueues, finalPipeline.QueueLosses, finalPipeline.ExportResources
    }));

CustomerOrder NewOrder(string number) => Q.CustomerOrders().Comment("prepare standard order").Purpose("compose standard graph")
    .NewEntity(context).UpdatePlatformId(1).UpdateOrderNumber(number).UpdateDescription("Draft detail");
OrderItem NewItem(string name) => Q.OrderItems().Comment("prepare standard item").Purpose("compose standard graph")
    .NewEntity(context).UpdateName(name);
Payment NewPayment(string code) => Q.Payments().Comment("prepare standard payment").Purpose("compose standard graph")
    .NewEntity(context).UpdateReferenceCode(code);
PaymentAttempt NewAttempt(string code) => Q.PaymentAttempts().Comment("prepare standard attempt").Purpose("compose standard graph")
    .NewEntity(context).UpdateReferenceCode(code);
Shipment NewShipment(string code) => Q.Shipments().Comment("prepare standard shipment").Purpose("compose standard graph")
    .NewEntity(context).UpdateReferenceCode(code);
static void Require(bool condition, string reason) { if (!condition) throw new InvalidOperationException(reason); }
sealed class Audits : IAppAuditEventSink
{
    public int Count;
    public Task RecordAsync(IReadOnlyDictionary<string, object?> audit, CancellationToken cancellationToken = default)
    { Count++; return Task.CompletedTask; }
}
sealed class SqlLogs : IDiagnosticSqlLogSink
{
    public int Count;
    public void Write(ExecutionMetadata metadata) { Count++; }
}
