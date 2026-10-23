using Generated;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static NotLoadedDiagnostic Missing(Func<object?> read, string location, string attempted, string expression)
{
    try { read(); }
    catch (TeaQLNotLoadedException error)
    {
        var wire = error.ToWire();
        Require(wire.Code == "NOT_LOADED", "Expected a stable NotLoaded error code");
        Require(wire.InstancePath == location, "Wrong first missing location");
        Require(wire.AttemptedInstancePath == attempted, "The full attempted traversal was not preserved");
        Require(wire.ExpressionWithBrokenPoint == expression, "Wrong native expression or positional breakpoint");
        Require(!error.Message.Contains("PRIVATE-FALLBACK"), "Diagnostics must not export argument values");
        Require(error.Message.Contains("<broken>"), "Native error text must show the breakpoint");
        return wire;
    }
    throw new InvalidOperationException("NotLoaded must not become a null or fallback result");
}

var first = new DateTimeOffset(2032, 2, 29, 10, 15, 30, TimeSpan.Zero);
var second = first.AddHours(25);
await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var service = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection), new ModuleSchemaProvider(module));
var context = module.IntoContext().WithDataService(service).WithBusinessClock(new FixedBusinessClock(first));
await context.EnsureSchemaAsync();
var order = Q.PurchaseOrders().Comment("Prepare the synthetic clock fixture")
    .Purpose("Verify generated audited saves and expression diagnostics").NewEntity(context);
order.UpdateContactPhone("5550100");
await order.AuditAs("Create the synthetic Fix order").SaveAsync(context);
Require(E.PurchaseOrder(order).CreateTime().Eval() == first.UtcDateTime, "Create time did not consume injected T0");
Require(E.PurchaseOrder(order).UpdateTime().Eval() == first.UtcDateTime, "Update time did not consume injected T0");
Require(E.PurchaseOrder(order).TenantId().Eval() == 1, "Declared context root was not injected");
var orderId = order.Id ?? throw new InvalidOperationException("Save did not allocate an ID");
Require(E.PurchaseOrder(order).InternalNote().OrIfNull("fallback") == "fallback", "Loaded null must retain fallback semantics");

var rows = await Q.PurchaseOrdersWithMinimalFields().WithIdIs(orderId).Limit(1)
    .Comment("Load the identity projection without eager data")
    .Purpose("Verify complete E diagnostics without implicit loading").ExecuteForListAsync(context);
Require(rows.Count == 1, "Missing saved order");
var partial = rows[0];
var diagnostics = new List<NotLoadedDiagnostic>
{
    Missing(() => E.PurchaseOrder(partial).Tenant().Name().Eval(), "/tenant", "/tenant/name",
        "E.PurchaseOrder(<root>).Tenant()<broken>.Name().Eval()"),
    Missing(() => E.PurchaseOrder(partial).Tenant().PurchaseOrderList().First().Tenant().Name().Eval(),
        "/tenant", "/tenant/purchaseOrderList/0/tenant/name",
        "E.PurchaseOrder(<root>).Tenant()<broken>.PurchaseOrderList().First().Tenant().Name().Eval()"),
    Missing(() => E.PurchaseOrder(partial).OrderItemList().Get(2).ProductName().Eval(),
        "/orderItemList", "/orderItemList/2/productName",
        "E.PurchaseOrder(<root>).OrderItemList()<broken>.Get(2).ProductName().Eval()"),
    Missing(() => E.PurchaseOrder(partial).ContactPhone().OrIfNull("PRIVATE-FALLBACK"),
        "/contactPhone", "/contactPhone",
        "E.PurchaseOrder(<root>).ContactPhone()<broken>.OrIfNull(<fallback>)")
};
context.WithBusinessClock(new FixedBusinessClock(second));
rows = await Q.PurchaseOrders().WithIdIs(orderId).Limit(1)
    .Comment("Reload the complete order before changing it")
    .Purpose("Verify update Fix preserves create time").ExecuteForListAsync(context);
Require(rows.Count == 1, "Missing saved order before update");
var loaded = rows[0];
loaded.UpdateContactPhone("5550101");
await loaded.AuditAs("Update the synthetic Fix order").SaveAsync(context);
rows = await Q.PurchaseOrders().WithIdIs(orderId).Limit(1)
    .Comment("Reload the persisted update").Purpose("Verify Business Clock values using E").ExecuteForListAsync(context);
Require(rows.Count == 1, "Missing updated order");
var persisted = rows[0];
Require(E.PurchaseOrder(persisted).CreateTime().Eval() == first.UtcDateTime, "Create time was not preserved");
Require(E.PurchaseOrder(persisted).UpdateTime().Eval() == second.UtcDateTime, "Update time did not advance to T1");
Require(E.PurchaseOrder(persisted).ContactPhone().Eval() == "5550101", "Business input was not updated");
Console.WriteLine("FIX_E_EXAMPLE=" + JsonSerializer.Serialize(new {
    language = "dotnet", passed = true, normalGeneratedSave = true, createTimePreserved = true,
    updateTimeAdvanced = true, declaredContextRootInjected = true, diagnostics
}));

sealed class ModuleSchemaProvider(RuntimeModule module) : ISchemaProvider
{
    public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name);
}
