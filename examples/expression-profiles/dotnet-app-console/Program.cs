using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

const string reference = "DOCK-042";
const string note = "Lift-gate pickup after 14:00";
Require(args.Length == 3 && args[2] is "first" or "restart", "Supply DATABASE PROFILE {first|restart}");
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = args[0], Pooling = false,
}.ToString());
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var context = module.IntoContext().WithDataService(new SqlDataServiceExecutor(
    new SqliteDialect(), new SqliteTransport(connection), new MetadataSchemaProvider(module.Metadata.GetEntity)))
    .DisableMutationSqlLog().DisableQuerySqlLog();
await context.EnsureSchemaAsync();
if (args[2] == "first")
{
    var order = await Q.DispatchOrders().Comment("Prepare the receiving order")
        .Purpose("Set its loading instruction").NewEntityAsync(context);
    order.UpdateDistributionCenter(1).UpdateOrderReference(reference).UpdateHandlingNote(note);
    await order.AuditAs("Register the dispatch at the receiving terminal").SaveAsync(context);
}

var detail = Q.DispatchOrders().WithOrderReferenceIs(reference).Limit(1).Comment("Select the dispatch order");
var root = await Q.DistributionCenters().WithIdIs(1).SelectDispatchOrderListWith(detail).Limit(1)
    .Comment("Read the dispatch desk and its order").Purpose("Check the complete loading instruction")
    .ExecuteForOneAsync(context) ?? throw new InvalidOperationException("The dispatch desk is missing");
var expression = E.DistributionCenter(root);
Require(expression.DockName().Eval() == "North Receiving Terminal", "Wrong dispatch desk");
Require(expression.DispatchOrderList().First().OrderReference().Eval() == reference
    && expression.DispatchOrderList().First().HandlingNote().Eval() == note, "The loaded order is wrong");
var matches = await Q.DispatchOrders().WithOrderReferenceIs(reference).Limit(2).Comment("Read back the dispatch record")
    .Purpose("Check the typed JSON round trip").ExecuteForListAsync(context);
Require(matches.Count == 1 && matches[0].Id == 1 && matches[0].Version == 1, "Wrong retained dispatch record");
var payload = matches[0].ToJson();
var decoded = E.DispatchOrder(DispatchOrder.FromJson(payload));
Require(decoded.OrderReference().Eval() == reference && decoded.HandlingNote().Eval() == note, "JSON changed the dispatch record");

var sparse = await Q.DistributionCentersWithMinimalFields().WithIdIs(1).SelectDockName().Limit(1)
    .Comment("Read the desk without its dispatch orders").Purpose("Verify the modeled preload instruction")
    .ExecuteForOneAsync(context) ?? throw new InvalidOperationException("The sparse dispatch desk is missing");
Require(E.DistributionCenter(sparse).DockName().Eval() == expression.DockName().Eval(), "The sparse dock name changed");
NotLoadedDiagnostic diagnostic;
string human;
try
{
    E.DistributionCenter(sparse).DispatchOrderList().First().OrderReference().Eval();
    throw new InvalidOperationException("A dispatch order was returned without loading the list");
}
catch (TeaQLNotLoadedException error)
{
    diagnostic = error.ToWire();
    human = error.Message;
}
using var document = JsonDocument.Parse(payload);
Console.WriteLine("DISPATCH_DOCUMENT_PROFILE=" + JsonSerializer.Serialize(new
{
    profile = args[1], phase = args[2], reference, handlingNote = note, id = matches[0].Id,
    version = matches[0].Version, dock = expression.DockName().Eval(), document = document.RootElement,
    diagnostic, human,
}));

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
