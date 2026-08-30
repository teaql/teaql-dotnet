using Generated;
using Generated.Models;
using TeaQL.Core;

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

var directory = Path.Combine(Directory.GetCurrentDirectory(), ".local");
Directory.CreateDirectory(directory);
var database = Path.Combine(directory, "conformance.sqlite");
File.Delete(database);

await using var service = new SqliteDataService($"Data Source={database}");
var context = new UserContext { DataService = service }.Install(GeneratedRuntimeModule.Module);
await context.EnsureSchemaAsync();
Console.WriteLine("PASS EnsureSchemaAsync (explicit SQLite DDL from Runtime Module)");

var sqlBeforeInvalid = context.SqlTrace.Count;
var invalid = new WorkItem().UpdatePlatform(1);
try
{
    await invalid.AuditAs("Checker must reject a missing title").SaveAsync(context);
    throw new InvalidOperationException("Checker accepted a missing required title");
}
catch (CheckException error)
{
    Require(error.Violations.Any(item => item.RuleId == "required" && item.Location.Contains("title")),
        "Checker did not identify title");
}
Require(context.SqlTrace.Count == sqlBeforeInvalid, "Checker must run before mutation SQL");
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
Console.WriteLine("PASS .NET minimum runtime conformance: 7/7");
