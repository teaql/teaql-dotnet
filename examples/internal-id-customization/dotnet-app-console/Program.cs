using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

if (args.Length != 2 || args[1] is not ("first" or "restart"))
    throw new ArgumentException("Usage: warehouse-order-allocation DATABASE {first|restart}");
var phase = args[1];
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = args[0], Pooling = false,
}.ToString());
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var context = module.IntoContext()
    .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
        new MetadataSchemaProvider(module.Metadata.GetEntity)))
    .DisableMutationSqlLog().DisableQuerySqlLog();
await context.EnsureSchemaAsync();
var warehouse = await Q.Warehouses().Limit(1).Comment("Read the seeded distribution center")
    .Purpose("Associate orders with the warehouse").ExecuteForOneAsync(context)
    ?? throw new InvalidOperationException("Generated warehouse is missing");
var warehouseId = warehouse.Id ?? throw new InvalidOperationException("The warehouse has no identity");
Require(warehouseId == 1, "Warehouse identity changed");
var allocator = new WarehouseAllocator();
context.WithInternalIdAllocator(allocator);

if (phase == "first")
    await ReceiveAsync(context, warehouseId, allocator);
var orders = await LoadOrdersAsync(context);
Require(orders.Select(order => order.Id).SequenceEqual(new long?[] { 100, 101, 102, 500, 501 }),
    "Persisted purchase order IDs changed");
Require(orders.Select(order => order.Reference).SequenceEqual(new[] { "DIRECT", "BATCH", "NESTED", "EXPLICIT", "GRAPH" }),
    "Persisted purchase order references changed");
var items = orders[^1].OrderItemList.OrderBy(item => item.Id).ToArray();
Require(items.Select(item => item.Id).SequenceEqual(new long?[] { 200, 201 })
    && items.Select(item => item.Sku).SequenceEqual(new[] { "FILTER", "GASKET" })
    && items[0].Quantity == 2 && items[1].Quantity == 3, "Persisted graph items changed");
Console.WriteLine("WAREHOUSE_ORDER_IDS=" + JsonSerializer.Serialize(new
{
    phase, orders = orders.Select(order => order.Id), items = items.Select(item => item.Id),
    failureRejected = phase == "first", rootId = warehouseId,
}));

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static InsertMutationRequest OrderRequest(long warehouse, string reference, ulong? explicitId = null)
{
    var command = new InsertCommand("PurchaseOrder")
        .Value("warehouse", new Value.I64Value(warehouse)).Value("reference", new Value.TextValue(reference));
    if (explicitId is { } identity)
        command.Value("id", new Value.U64Value(identity));
    return new InsertMutationRequest(command, "Receive the numbered purchase order");
}

static async Task<PurchaseOrder> NewOrderAsync(UserContext context, long warehouse, string reference)
{
    var order = await Q.PurchaseOrders().Comment("Prepare an incoming purchase order")
        .Purpose("Reserve its warehouse identity").NewEntityAsync(context);
    return order.UpdateWarehouse(warehouse).UpdateReference(reference);
}

static async Task<OrderItem> NewItemAsync(UserContext context, long order, string sku, long units)
{
    var item = await Q.OrderItems().Comment("Prepare an order item")
        .Purpose("Reserve an item identity before graph save").NewEntityAsync(context);
    return item.UpdatePurchaseOrder(order).UpdateSku(sku).UpdateQuantity(units);
}

static async Task<PurchaseOrder[]> LoadOrdersAsync(UserContext context)
{
    var rows = await Q.PurchaseOrders().OrderByIdAscending().Limit(10).SelectOrderItemList()
        .Comment("Read assigned order IDs and items").Purpose("Reconcile the warehouse ledger").ExecuteForListAsync(context);
    return rows.ToArray();
}

static async Task ReceiveAsync(UserContext context, long warehouse, WarehouseAllocator allocator)
{
    var direct = await NewOrderAsync(context, warehouse, "DIRECT");
    Require(direct.Id == 100, "Direct allocation bypassed the custom allocator");
    await direct.AuditAs("Receive one purchase order").SaveAsync(context);
    var service = context.RequireResource<IDataService>();
    await service.MutateAsync(new BatchMutationRequest(new List<MutationRequest>
    {
        OrderRequest(warehouse, "BATCH"),
        new BatchMutationRequest(new List<MutationRequest> { OrderRequest(warehouse, "NESTED") },
            "receive a nested dispatch"),
    }, "receive a batch of purchase orders"));
    await service.MutateAsync(OrderRequest(warehouse, "EXPLICIT", 500));
    Require(allocator.Floors.Contains(("PurchaseOrder", 500UL)), "Explicit order did not advance the Context floor");

    var graph = await NewOrderAsync(context, warehouse, "GRAPH");
    var parentId = graph.Id ?? throw new InvalidOperationException("The graph order has no identity");
    Require(parentId == 501, "Graph order did not respect the explicit floor");
    var first = await NewItemAsync(context, parentId, "FILTER", 2);
    Require(first.Id == 200, "Graph item did not use the independent item sequence");
    allocator.FailNextItem = true;
    try
    {
        await NewItemAsync(context, parentId, "GASKET", 3);
        throw new InvalidOperationException("Item allocator failure was bypassed");
    }
    catch (AllocationUnavailableException error)
    {
        Require(error.Message.EndsWith("for OrderItem"), "Unexpected allocation failure");
    }
    var beforeRetry = await LoadOrdersAsync(context);
    Require(beforeRetry.Select(order => order.Id).SequenceEqual(new long?[] { 100, 101, 102, 500 }),
        "Allocation failure wrote the pending order");
    var itemsBeforeRetry = await Q.OrderItems().Limit(5).Comment("Check for partial receiving")
        .Purpose("Allocator refusal must precede any graph write").ExecuteForListAsync(context);
    Require(itemsBeforeRetry.Count == 0, "Allocation failure wrote a partial item");
    var second = await NewItemAsync(context, parentId, "GASKET", 3);
    Require(graph.Id == parentId && second.Id == 201, "Retry changed the reserved order or next item identity");

    graph.OrderItemList.Add(first);
    graph.OrderItemList.Add(second);
    await graph.AuditAs("Receive purchase order and two stocked items").SaveAsync(context);
    await graph.AuditAs("Retry the same accepted purchase order").SaveAsync(context);
    Require(graph.Id == 501, "Repeated save replaced the order identity");
}

sealed class AllocationUnavailableException : Exception
{
    public AllocationUnavailableException() : base("warehouse ID service unavailable for OrderItem") {}
}

sealed class WarehouseAllocator : IInternalIdAllocator
{
    private readonly Dictionary<string, ulong> _levels = new() { ["PurchaseOrder"] = 99, ["OrderItem"] = 199 };
    public List<(string Entity, ulong Floor)> Floors { get; } = [];
    public bool FailNextItem { get; set; }

    public Task<ulong> NextIdAsync(string entity)
    {
        lock (_levels)
        {
            if (entity == "OrderItem" && FailNextItem)
            {
                FailNextItem = false;
                throw new AllocationUnavailableException();
            }
            return Task.FromResult(++_levels[entity]);
        }
    }

    public Task EnsureIdFloorAsync(string entity, ulong floor)
    {
        lock (_levels)
        {
            Floors.Add((entity, floor));
            _levels[entity] = Math.Max(_levels[entity], floor);
        }
        return Task.CompletedTask;
    }
}
