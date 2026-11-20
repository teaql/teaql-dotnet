using TeaQL.Core;
using Generated;
using Generated.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using TeaQL.Runtime;
using TeaQL.DataService;
using TeaQL.Sql;
using Microsoft.Data.Sqlite;
using TeaQL.Provider.Sqlite;
var module = GeneratedRuntimeModule.Module;
var context = module.IntoContext().WithBusinessClock(new FixedBusinessClock(
    new DateTimeOffset(2026, 10, 1, 8, 30, 0, TimeSpan.Zero)));
var database = Environment.GetEnvironmentVariable("TEAQL_PURCHASE_NUMBERING_DATABASE")
    ?? throw new InvalidOperationException("Supply an example database");
var round = Environment.GetEnvironmentVariable("TEAQL_PURCHASE_NUMBERING_ROUND")
    ?? throw new InvalidOperationException("Supply a round label");
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = database, Pooling = false,
}.ToString());
await connection.OpenAsync();
var dialect = new SqliteDialect();
var transport = new SqliteTransport(connection);
context.WithDataService(new SqlDataServiceExecutor(dialect, transport, new MetadataSchemaProvider(module.Metadata.GetEntity)));
var businessIdAllocator = new OptimisticBusinessIdAllocator(transport, dialect);
context
    .WithBusinessIdProfileFactory(new DefaultBusinessIdProfileFactory())
    .WithBusinessIdKeyProvider(BusinessIdKeyProviderFromEnvironment())
    .WithBusinessIdService(new DefaultBusinessIdService(businessIdAllocator));
await context.EnsureSchemaAsync();
await businessIdAllocator.EnsureSchemaAsync();
var first = await CreateOrderAsync(context, $"Northwind-{round}");
var second = await CreateOrderAsync(context, $"Contoso-{round}");
var firstId = first.Id;
var firstNumber = first.OrderNumber ?? throw new InvalidOperationException("First order has no number");
var secondNumber = second.OrderNumber ?? throw new InvalidOperationException("Second order has no number");
const string format = @"^ORD-20261001-[0-9A-Z]{6}$";
if (firstId == second.Id || firstNumber == secondNumber
    || !Regex.IsMatch(firstNumber.Value, format) || !Regex.IsMatch(secondNumber.Value, format))
    throw new InvalidOperationException("Expected distinct order numbers from the Context business day");

await first.AuditAs("Retry recording Northwind's purchase order").SaveAsync(context);
if (first.Id != firstId || first.OrderNumber != firstNumber)
    throw new InvalidOperationException("Retry changed the purchase order identity");

foreach (var order in new[] { first, second })
{
    var persisted = await Q.PurchaseOrderByOrderNumber(order.OrderNumber!.Value)
        .Comment("Read a purchase order by its assigned number")
        .Purpose("Verify the procurement record and allocation").ExecuteForOneAsync(context);
    if (persisted is null || persisted.Id != order.Id || persisted.OrderNumber != order.OrderNumber
        || persisted.Supplier != order.Supplier)
        throw new InvalidOperationException("Typed order-number query returned a different purchase order");
}

const string rejectedSupplier = "Unaudited Supplier";
var rejected = await PrepareOrderAsync(context, rejectedSupplier);
string rejection;
try
{
    await rejected.AuditAs(" ").SaveAsync(context);
    throw new InvalidOperationException("Unaudited purchase order was accepted");
}
catch (RequestIntentException error) when (error.Code == "REQUEST_COMMENT_REQUIRED" && error.RequestKind == "mutation")
{
    rejection = error.Code;
}
var found = await Q.PurchaseOrders().WithSupplierIs(rejectedSupplier).Limit(2)
    .Comment("Look for the unaudited purchase order")
    .Purpose("Prove the rejected operation stored no order").ExecuteForOneAsync(context);
if (found is not null)
    throw new InvalidOperationException("Unaudited purchase order persisted");

Console.WriteLine("PURCHASE_ORDER_NUMBERING=" + JsonSerializer.Serialize(new
{
    round, firstId, firstNumber = firstNumber.Value, secondId = second.Id, secondNumber = secondNumber.Value,
    retryNumber = first.OrderNumber!.Value.Value, rejection, typedReadback = true,
}));

static async Task<PurchaseOrder> PrepareOrderAsync(UserContext context, string supplier)
{
    var order = await Q.PurchaseOrders().Comment("Initialize the purchase order")
        .Purpose("Record a procurement commitment").NewEntityAsync(context);
    return order.UpdatePlatform(1).UpdateSupplier(supplier);
}

static async Task<PurchaseOrder> CreateOrderAsync(UserContext context, string supplier)
{
    var order = await PrepareOrderAsync(context, supplier);
    await order.AuditAs("Record the approved purchase order").SaveAsync(context);
    return order;
}

static StaticBusinessIdKeyProvider BusinessIdKeyProviderFromEnvironment()
{
    const string keyVariable = "TEAQL_BUSINESS_ID_KEY_V1_HEX";
    var keyText = Environment.GetEnvironmentVariable(keyVariable);
    if (keyText is null || keyText.Length != 64)
        throw new InvalidOperationException($"{keyVariable} must contain exactly 64 hexadecimal characters");
    byte[] key;
    try { key = Convert.FromHexString(keyText); }
    catch (FormatException error)
    {
        throw new InvalidOperationException($"{keyVariable} must contain exactly 64 hexadecimal characters", error);
    }
    var versionText = Environment.GetEnvironmentVariable("TEAQL_BUSINESS_ID_KEY_VERSION");
    var version = versionText is null ? 1U : uint.TryParse(versionText, out var parsed) && parsed > 0
        ? parsed
        : throw new InvalidOperationException("TEAQL_BUSINESS_ID_KEY_VERSION must be a positive unsigned integer");
    return new StaticBusinessIdKeyProvider(new BusinessIdEncodingKey(version, key));
}
