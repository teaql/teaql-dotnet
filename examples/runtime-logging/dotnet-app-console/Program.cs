using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

const string customerReference = "PRIVATE-CARRIER-REFERENCE-5821";
const string amendment = "Keep upright for the carrier";
if (args.Length != 2)
    throw new ArgumentException("Usage: shipping-order-logging DATABASE SAFE_AUDIT_JSON");
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = args[0], Pooling = false,
}.ToString());
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var audit = new ShippingAudit();
var context = module.IntoContext()
    .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
        new MetadataSchemaProvider(module.Metadata.GetEntity)))
    .WithAppAuditEventSink(audit);
await context.EnsureSchemaAsync();
var warehouse = await Q.Warehouses().Limit(1).Comment("Read the dispatch warehouse")
    .Purpose("Associate the shipping order with its warehouse").ExecuteForOneAsync(context)
    ?? throw new InvalidOperationException("The warehouse is missing");
audit.Events.Clear();

var order = await Q.ShippingOrders().Comment("Prepare a shipping order")
    .Purpose("Record a carrier shipment").NewEntityAsync(context);
order.UpdateWarehouse(warehouse.Id).UpdateReference("SHIP-001")
    .UpdateCustomerReference(customerReference).UpdateInstructions("Await carrier");
var item = await Q.ShippingItems().Comment("Prepare one returnable case")
    .Purpose("Add the carrier load to the order").NewEntityAsync(context);
item.UpdateShippingOrder(order.Id).UpdateSku("RETURNABLE-CASE").UpdateQuantity(2);
order.ShippingItemList.Add(item);
await order.AuditAs("Accept the carrier shipment").SaveAsync(context);

async Task<ShippingOrder> LookupAsync()
{
    var items = Q.ShippingItems().SelectQuantity().OrderByIdAscending().Limit(5)
        .Comment("Load the carrier line quantity");
    return await Q.ShippingOrders().WithCustomerReferenceIs(customerReference)
        .Limit(1).SelectShippingItemListWith(items).Comment("Find the carrier shipment")
        .Purpose("Read the shipping order and its loading lines").ExecuteForOneAsync(context)
        ?? throw new InvalidOperationException("The shipping order is missing");
}

var loaded = await LookupAsync();
if (loaded.Reference != "SHIP-001" || loaded.CustomerReference != customerReference
    || loaded.ShippingItemList.Single().Quantity != 2)
    throw new InvalidOperationException("The carrier load did not round-trip through generated Q");
loaded.UpdateInstructions(amendment);
await loaded.AuditAs("Change handling instructions").SaveAsync(context);
var committed = audit.Events.Count;

// Exercise the real provider failure path for a forbidden carrier override.
await using var fault = connection.CreateCommand();
fault.CommandText = """
    CREATE TRIGGER reject_carrier_override BEFORE UPDATE OF instructions ON shipping_order_data
    WHEN NEW.instructions = 'Declined override'
    BEGIN SELECT RAISE(ABORT, 'carrier override rejected'); END
    """;
await fault.ExecuteNonQueryAsync();
try
{
    loaded.UpdateInstructions("Declined override");
    await loaded.AuditAs("Reject a carrier override").SaveAsync(context);
    throw new InvalidOperationException("The forbidden carrier override was saved");
}
catch (SqlExecutorException error) when (error.InnerException is SqliteException { SqliteErrorCode: 19 })
{
}
finally
{
    fault.CommandText = "DROP TRIGGER reject_carrier_override";
    await fault.ExecuteNonQueryAsync();
}

var saved = await LookupAsync();
if (saved.Id != order.Id || saved.CustomerReference != customerReference || saved.Instructions != amendment
    || saved.Version != 2 || committed != 3 || audit.Events.Count != committed)
    throw new InvalidOperationException("The rejected override changed the committed shipment or produced a success audit");
var safe = JsonSerializer.Serialize(audit.Events);
if (safe.Contains(customerReference, StringComparison.Ordinal)
    || !safe.Contains("Change handling instructions", StringComparison.Ordinal))
    throw new InvalidOperationException("The safe audit sink lost its privacy or business reason");
await File.WriteAllTextAsync(args[1], safe + "\n");
Console.WriteLine("SHIPPING_LOG_RESULT=" + JsonSerializer.Serialize(new
{
    orderId = saved.Id, itemId = saved.ShippingItemList.Single().Id, version = saved.Version,
    committedAudits = committed, rejected = true,
}));

sealed class ShippingAudit : IAppAuditEventSink
{
    public List<IReadOnlyDictionary<string, object?>> Events { get; } = [];

    public Task RecordAsync(IReadOnlyDictionary<string, object?> safeEvent, CancellationToken cancellationToken = default)
    {
        Events.Add(safeEvent);
        return Task.CompletedTask;
    }
}
