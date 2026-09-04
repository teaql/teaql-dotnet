using Generated;
using Generated.Models;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Microsoft.Data.Sqlite;

var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
var local = Path.Combine(root, ".local");
var database = Path.Combine(local, "order.db");
var firstRun = !File.Exists(database);
if (firstRun) Console.WriteLine($"[database] {database} was not found; TeaQL will create it");
Directory.CreateDirectory(local);

await using var connection = new SqliteConnection($"Data Source={database}");
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var service = new SqlDataServiceExecutor(
    new SqliteDialect(), new SqliteTransport(connection), new ModuleSchemaProvider(module));
var context = module.IntoContext().WithDataService(service);
await context.EnsureSchemaAsync();
long platformId;
if (firstRun)
{
    var now = new DateTime(2026, 8, 13, 9, 0, 0, DateTimeKind.Utc);
    var platform = await Q.CommercePlatforms().WithIdIs(1)
        .Comment("Load generated commerce root")
        .Purpose("Seed quick-start data")
        .ExecuteForOneAsync(context)
        ?? throw new InvalidOperationException("Generated root was not seeded");
    platformId = platform.Id!.Value;
    var customer = new Customer { Name = "Acme Retail", Email = "masked-in-quick-start", CommercePlatform = platformId, CreateTime = now, UpdateTime = now };
    await customer.AuditAs("Create masked quick-start customer").SaveAsync(context);
    var order = new CustomerOrder {
        OrderNumber = "WEB-2026-001", OrderDate = new DateTime(2026, 8, 12), TotalAmount = 129.95m,
        Status = 1001, Customer = customer.Id, CommercePlatform = platformId,
        CreateTime = now, UpdateTime = now
    };
    await order.AuditAs("Create deterministic quick-start order").SaveAsync(context);
    Console.WriteLine("[schema] ensured generated entity tables and columns while saving");
    Console.WriteLine("[seed] inserted deterministic platform, customer, and order");
}
else
{
    var platforms = await Q.CommercePlatforms()
        .WithNameIs("Northwind Demo")
        .Comment("Check whether deterministic quick-start data exists")
        .Purpose("Initialize the local order-management example")
        .ExecuteForListAsync(context);
    if (platforms.Count == 0) throw new InvalidOperationException("Existing database has no quick-start seed; remove .local/order.db and rerun");
    platformId = platforms[0].Id!.Value;
    Console.WriteLine("[schema] existing generated schema verified by governed query");
    Console.WriteLine("[seed] deterministic data already exists; no duplicate rows added");
}

var orders = await Q.CustomerOrders()
    .WithOrderNumberContaining("WEB-")
    .OrderByIdAscending()
    .Comment("List WEB orders for the terminal quick start")
    .Purpose("Show the operator a deterministic order list")
    .ExecuteForListAsync(context);
Console.WriteLine($"[query] matched {orders.Count} order(s)");
foreach (var row in orders)
    Console.WriteLine($"  {row.OrderNumber}  {row.OrderDate:yyyy-MM-dd}  {row.TotalAmount}");

if (firstRun)
{
    var preset = new OrderSearchPreset {
        Name = "Pending web orders", FilterJson = "{\"order_number\":\"WEB-\"}",
        RequestId = "quick-start-pending-orders", OwnerUserId = "quick-start-user",
        CommercePlatform = platformId
    };
    await preset.AuditAs("Save idempotent quick-start search preset").SaveAsync(context);
    Console.WriteLine($"[mutation] saved preset #{preset.Id}");
}
else
{
    var presets = await Q.OrderSearchPresets()
        .WithRequestIdIs("quick-start-pending-orders")
        .Comment("Check idempotent quick-start preset")
        .Purpose("Persist the operator's reusable search")
        .ExecuteForListAsync(context);
    if (presets.Count != 1) throw new InvalidOperationException("Expected one idempotent preset");
    Console.WriteLine($"[mutation] preset #{presets[0].Id} already exists");
}
Console.WriteLine("[audit] governed mutation path verified");

sealed class ModuleSchemaProvider(RuntimeModule module) : ISchemaProvider
{
    public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name);
}
