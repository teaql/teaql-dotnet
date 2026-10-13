using Generated;
using Microsoft.Data.Sqlite;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

// Application-owned consumer of the current generated Order fixture. Optional
// telemetry is assembled once by the package/environment, not by business code.
var directory = Path.Combine(Path.GetTempPath(), "teaql-dotnet-standard-order-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
var database = Environment.GetEnvironmentVariable("TEAQL_STANDARD_ORDER_DB") ?? Path.Combine(directory, "orders.sqlite");
await using var connection = new SqliteConnection("Data Source=" + database);
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
// The shared structural fixture declares ordinary fields public. This demo
// explicitly makes its customer-facing order number sensitive using the normal
// runtime metadata customization seam; do not invent a telemetry-only masker.
var orderDescriptor = module.Metadata.GetEntity("CustomerOrder")!;
module.Entity(orderDescriptor.AuditMaskFields(new() {
    orderDescriptor.Properties.Single(property => property.ColumnNameString == "order_number").Name
}));
var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection), new MetadataSchemaProvider(module.Metadata.GetEntity));
var audits = new Audits(); var ordinary = new SqlLogs();
var context = module.IntoContext().WithDataService(provider).WithAppAuditEventSink(audits)
    .WithDiagnosticSqlLogSink(ordinary).DisableQuerySqlLog().DisableMutationSqlLog();
var privateNeedle = "ORDER-PRIVATE-" + Guid.NewGuid().ToString("N");
try
{
    await context.EnsureSchemaAsync();
    var firstSeedAudits = audits.Count;
    Require(firstSeedAudits is 0 or 4, "one platform and three constant audited seeds, or existing database");
    await context.EnsureSchemaAsync();
    Require(audits.Count == firstSeedAudits, "repeated schema must not seed again");
    var customer = Q.Customers().Comment("create fixture customer").Purpose("prepare operational order queue").NewEntity(context)
        .UpdatePlatformId(1).UpdateName("North Star Retail").UpdateVersion(1);
    customer = await customer.AuditAs("seed fixture customer").SaveAsync(context);
    for (var index = 1; index <= 4; index++)
    {
        var order = Q.CustomerOrders().Comment("create fixture order").Purpose("prepare operational order queue").NewEntity(context)
            .UpdateCustomerId(customer.Id).UpdateStatusId(index == 1 ? 1001 : 1002)
            .UpdateOrderNumber(index == 4 ? "OTHER-004" : $"{privateNeedle}-{index:000}")
            .UpdateOrderDate(new DateTime(2026, 10, index, 0, 0, 0, DateTimeKind.Utc))
            .UpdateTotalAmount(index == 4 ? 900m : index * 100m).UpdateVersion(1);
        order = await order.AuditAs("seed operational queue order").SaveAsync(context);
        for (var item = 1; item <= 5; item++)
        {
            var line = Q.OrderItems().Comment("create fixture item").Purpose("prepare bounded item previews").NewEntity(context)
                .UpdateCustomerOrderId(order.Id).UpdateProductName($"Box {item}").UpdateQuantity(item)
                .UpdateUnitPrice(20m).UpdateVersion(1);
            await line.AuditAs("seed operational queue item").SaveAsync(context);
        }
    }
    Require(audits.Count == firstSeedAudits + 25, "all seeds and fixture writes have durable audit records");
    for (var repetition = 1; repetition <= 2; repetition++)
    {
        var page = await Q.CustomerOrdersWithMinimalFields().WithOrderNumberContaining(privateNeedle).CountAs("totalOrders")
            .FacetByStatusAs("statusCounts", Q.OrderStatusesWithMinimalFields().SelectName().SelectColor().CountAs("orderCount")
                .SumTotalAmountOfCustomerOrdersAs("orderTotalAmount", Q.CustomerOrdersWithMinimalFields())
                .OrderByDisplayOrderAscending().Comment("load status facets"))
            .SelectOrderNumber().SelectOrderDate()
            .SelectStatusWith(Q.OrderStatusesWithMinimalFields().SelectName().SelectColor().Comment("load status"))
            .SelectOrderItemListWith(Q.OrderItemsWithMinimalFields().SelectProductName().SelectQuantity()
                .OrderByIdAscending().Limit(3).Comment("load item previews"))
            .CountOrderItemsAs("itemCount").OrderByOrderDateDescending().OrderByIdDescending().Offset(1).Limit(1)
            .Comment("load order list").Purpose("render order list page").ExecuteForListAsync(context);
        Require(page.TotalCount == 3 && page.Aggregations["totalOrders"].TryI64() == 3 && page.Count == 1, "exact named total and bounded list");
        var row = page.Single();
        Require(E.CustomerOrder(row).OrderNumber().Eval() == privateNeedle + "-002", "typed order selection");
        Require(E.CustomerOrder(row).Status().Name().Eval() == "Paid", "loaded forward status");
        Require(E.CustomerOrder(row).OrderItemList().Size().Eval() == 3, "bounded reverse relation");
        Require(E.CustomerOrder(row).OrderItemList().First().ProductName().Eval() == "Box 1", "typed sorted preview");
        Require(row.DynamicProperties.Get("itemCount")?.TryI64() == 5, "unbounded item count independent of preview");
        var facets = page.Facets["statusCounts"];
        Require(facets.Count == 3, "three status facets");
        for (var index = 0; index < 3; index++)
        {
            Require(facets[index]["id"].TryI64() == 1001 + index, "facet ordering");
            Require(facets[index]["orderCount"].TryI64() == new long[] { 1, 2, 0 }[index], "filtered facet count");
            Require(index == 2 ? facets[index]["orderTotalAmount"].Raw == null
                : Convert.ToDecimal(facets[index]["orderTotalAmount"].Raw) == new decimal[] { 100m, 500m }[index], "facet sum including native null");
        }
        var rejected = false;
        try { _ = E.CustomerOrder(row).TotalAmount().Eval(); }
        catch (Exception error) when (error.Message.Contains("NotLoaded", StringComparison.OrdinalIgnoreCase)
            || error.Message.Contains("not loaded", StringComparison.OrdinalIgnoreCase)) { rejected = true; }
        Require(rejected, "unselected E field must fail as NotLoaded");
    }
    Require(ordinary.Count == 0 && audits.Count == firstSeedAudits + 25, "telemetry independent of ordinary SQL logging and durable audit");
    await context.RuntimeTelemetry.FlushAsync();
    Console.WriteLine($"DOTNET_STANDARD_ORDER PASSED queries=2 total=3 preview=3 itemCount=5 facets=3 first_seed_audits={firstSeedAudits} repeat_seed_audits=0 audits={audits.Count} ordinary_sql_logs=0 database={database}");
}
finally { await context.RuntimeTelemetry.ShutdownAsync(); }

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
