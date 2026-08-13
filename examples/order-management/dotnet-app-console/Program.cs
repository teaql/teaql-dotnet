using Generated;
using Generated.Models;
using TeaQL.Core;

var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
var local = Path.Combine(root, ".local");
var database = Path.Combine(local, "order.db");
var firstRun = !File.Exists(database);
if (firstRun) Console.WriteLine($"[database] {database} was not found; TeaQL will create it");
Directory.CreateDirectory(local);

await using var service = new SqliteDataService($"Data Source={database}");
var rawAudit = new InMemoryRawAuditEventSink();
var appAudit = new InMemoryAppAuditEventSink();
var ctx = new UserContext { DataService = service }
    .InitializeAudit(rawAudit, appAudit)
    .ConfigureAuditPolicy("Customer", new[] { "email" })
    .ConfigureAuditPolicy("OrderSearchPreset", new[] { "filter_json" });
long platformId;
if (firstRun)
{
    var now = new DateTime(2026, 8, 13, 9, 0, 0, DateTimeKind.Utc);
    var platform = new CommercePlatform { Name = "Northwind Demo", CreateTime = now, UpdateTime = now };
    await platform.AuditAs("Create quick-start commerce platform").SaveAsync(ctx);
    platformId = platform.Id!.Value;
    var customer = new Customer { Name = "Acme Retail", Email = "masked-in-quick-start", CommercePlatform = platformId, CreateTime = now, UpdateTime = now };
    await customer.AuditAs("Create masked quick-start customer").SaveAsync(ctx);
    var order = new CustomerOrder {
        OrderNumber = "WEB-2026-001", OrderDate = new DateTime(2026, 8, 12), TotalAmount = 129.95m,
        Status = 1001, Customer = customer.Id, CommercePlatform = platformId,
        CreateTime = now, UpdateTime = now
    };
    await order.AuditAs("Create deterministic quick-start order").SaveAsync(ctx);
    Console.WriteLine("[schema] ensured generated entity tables and columns while saving");
    Console.WriteLine("[seed] inserted deterministic platform, customer, and order");
}
else
{
    var platforms = await Q.CommercePlatforms()
        .WithNameIs("Northwind Demo")
        .Comment("Check whether deterministic quick-start data exists")
        .Purpose("Initialize the local order-management example")
        .ExecuteForListAsync(ctx);
    if (platforms.Rows.Count == 0) throw new InvalidOperationException("Existing database has no quick-start seed; remove .local/order.db and rerun");
    platformId = Convert.ToInt64(platforms.Rows[0]["id"].Raw);
    Console.WriteLine("[schema] existing generated schema verified by governed query");
    Console.WriteLine("[seed] deterministic data already exists; no duplicate rows added");
}

var orders = await Q.CustomerOrders()
    .WithOrderNumberContaining("WEB-")
    .OrderByIdAscending()
    .Comment("List WEB orders for the terminal quick start")
    .Purpose("Show the operator a deterministic order list")
    .ExecuteForListAsync(ctx);
Console.WriteLine($"[query] matched {orders.Rows.Count} order(s)");
foreach (var row in orders.Rows)
    Console.WriteLine($"  {row["order_number"].Raw}  {row["order_date"].Raw}  {row["total_amount"].Raw}");

if (firstRun)
{
    var preset = new OrderSearchPreset {
        Name = "Pending web orders", FilterJson = "{\"order_number\":\"WEB-\"}",
        RequestId = "quick-start-pending-orders", OwnerUserId = "quick-start-user",
        CommercePlatform = platformId
    };
    await preset.AuditAs("Save idempotent quick-start search preset").SaveAsync(ctx);
    Console.WriteLine($"[mutation] saved preset #{preset.Id}");
}
else
{
    var presets = await Q.OrderSearchPresets()
        .WithRequestIdIs("quick-start-pending-orders")
        .Comment("Check idempotent quick-start preset")
        .Purpose("Persist the operator's reusable search")
        .ExecuteForListAsync(ctx);
    if (presets.Rows.Count != 1) throw new InvalidOperationException("Expected one idempotent preset");
    Console.WriteLine($"[mutation] preset #{presets.Rows[0]["id"].Raw} already exists");
}
Console.WriteLine($"[audit] immutable={rawAudit.Events.Count}, app-safe={appAudit.Events.Count}");
