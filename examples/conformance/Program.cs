using Generated;
using Generated.Models;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Microsoft.Data.Sqlite;

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

var orderKey = new EntityKey("Order", 1);
var executionKey = new EntityKey("InferenceExecution", 1);
var targetLedger = new EntityRoot();
var sourceLedger = new EntityRoot();
targetLedger.SetOriginalVersion(orderKey, 3);
sourceLedger.SetOriginalVersion(executionKey, 9);
sourceLedger.Set(executionKey, "execution_status", new Value.TextValue("COMPLETED"));
targetLedger.MergeFrom(sourceLedger);
Require(targetLedger.OriginalVersion(orderKey) == 3, "Order#1 version was overwritten");
Require(targetLedger.OriginalVersion(executionKey) == 9,
    "InferenceExecution#1 version was resolved through Order#1");
Console.WriteLine("PASS Mutation ledger identity (same ID, different entity types keep versions 3/9)");

var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
var directory = Path.Combine(projectRoot, ".local");
Directory.CreateDirectory(directory);
var database = Path.Combine(directory, "conformance.sqlite");
File.Delete(database);

await using var connection = new SqliteConnection($"Data Source={database}");
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var service = new SqlDataServiceExecutor(
    new SqliteDialect(), new SqliteTransport(connection), new ModuleSchemaProvider(module));
var context = module.IntoContext().WithDataService(service);
await context.EnsureSchemaAsync();
Console.WriteLine("PASS EnsureSchemaAsync (explicit SQLite DDL from Runtime Module)");

var invalid = new WorkItem().UpdatePlatform(1);
try
{
    await invalid.AuditAs("Checker must reject a missing title").SaveAsync(context);
    throw new InvalidOperationException("Checker accepted a missing required title");
}
catch (CheckException error)
{
    Require(error.Violations.Any(item => item.RuleId == "required"
        && item.Location.ToString().Contains("title", StringComparison.OrdinalIgnoreCase)),
        "Checker did not identify title");
}
Console.WriteLine("PASS Checker (canonical title key, rejected before SQL)");

var created = await new WorkItem()
    .UpdateTitle("Verify .NET runtime")
    .UpdatePlatform(1)
    .AuditAs("Create conformance work item")
    .SaveAsync(context);
Require(created.Id.HasValue && created.Version == 1, "Create did not return ID/version");
Console.WriteLine($"PASS Create (id={created.Id}, version={created.Version})");

var queried = await Q.WorkItems().WithIdIs(created.Id.Value)
    .Comment("Load the complete work item before mutation")
    .Purpose("Verify typed Q API and update semantics")
    .ExecuteForOneAsync(context);
Require(queried is not null && queried.Title == "Verify .NET runtime", "Q API result mismatch");
Console.WriteLine("PASS Q API (typed SmartList<WorkItem>)");

var related = await Q.WorkItems().WithIdIs(created.Id.Value)
    .SelectPlatformWith(Q.PlatformsWithMinimalFields().SelectName())
    .Comment("what: load work item with platform")
    .Purpose("why: prove generated relation trace inheritance")
    .ExecuteForOneAsync(context);
Require(related?.PlatformEntity?.Name == "Runtime Example",
    "Forward Platform relation was not loaded");
Console.WriteLine("PASS relation query (typed Platform and inherited trace intent)");

Require(E.WorkItem(queried).Title().Eval() == "Verify .NET runtime", "E loaded scalar mismatch");
Require(E.WorkItem(queried).Description().OrIfNull("N/A") == "N/A", "E null fallback mismatch");
var minimal = await Q.WorkItemsWithMinimalFields().WithIdIs(created.Id.Value)
    .Comment("Load only mandatory identity fields")
    .Purpose("Verify E not-loaded semantics")
    .ExecuteForOneAsync(context);
try
{
    _ = E.WorkItem(minimal).Title().Eval();
    throw new InvalidOperationException("E treated not-loaded title as null");
}
catch (TeaQLNotLoadedException) { }
Console.WriteLine("PASS E API (loaded, null fallback, and not-loaded are distinct)");

var oldVersion = queried.Version.Value;
var updated = await queried.UpdateTitle("Verified .NET runtime")
    .AuditAs("Update conformance work item").SaveAsync(context);
Require(updated.Version == oldVersion + 1, "Update did not increment version");
Console.WriteLine($"PASS Update (version {oldVersion} -> {updated.Version})");

await updated.MarkForDeletion().AuditAs("Delete conformance work item").SaveAsync(context);
var remaining = await Q.WorkItems().WithIdIs(created.Id.Value)
    .Comment("Verify soft-deleted work item is excluded")
    .Purpose("Verify delete semantics")
    .ExecuteForListAsync(context);
Require(remaining.Count == 0, "Default Q returned a deleted row");
Console.WriteLine("PASS Delete (default Q excludes deleted rows)");
Console.WriteLine("PASS .NET minimum runtime conformance: 8/8");

sealed class ModuleSchemaProvider(RuntimeModule module) : ISchemaProvider
{
    public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name);
}
