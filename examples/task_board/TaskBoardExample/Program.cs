using Generated;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

var database = Environment.GetEnvironmentVariable("TEAQL_TASK_BOARD_DB") ?? ":memory:";
await using var connection = new SqliteConnection($"Data Source={database}");
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var context = module.IntoContext().WithDataService(new SqlDataServiceExecutor(
    new SqliteDialect(), new SqliteTransport(connection),
    new MetadataSchemaProvider(module.Metadata.GetEntity)));

await context.EnsureSchemaAsync();
await context.EnsureSchemaAsync();

var pending = (await Q.Tasks().Comment("prepare an authorized robot task")
    .Purpose("demonstrate generated task mutations").NewEntityAsync(context))
    .UpdateName("Inspect robot charging station").UpdatePlatform(1).UpdateStatusToPlanned();
try
{
    await pending.SaveAsync(context);
    throw new InvalidOperationException("A write without an audit comment succeeded");
}
catch (RequestIntentException error) when (error.Code == "REQUEST_COMMENT_REQUIRED"
    && error.RequestKind == "mutation") { }

var saved = await pending.AuditAs("Create a planned robot task").SaveAsync(context);
var taskId = saved.Id ?? throw new InvalidOperationException("Created task has no ID");
Require(saved.Version == 1, "Create did not return version 1");

var tasks = await Q.Tasks().WithIdIs(taskId).Limit(1)
    .Comment("load the complete task before renaming it")
    .Purpose("verify generated Q and E on the task board").ExecuteForListAsync(context);
Require(tasks.Count == 1, "Task query did not return exactly one row");
var loaded = tasks[0];
Require(E.Task(loaded).Name().Eval() == "Inspect robot charging station", "Loaded name mismatch");
Require(E.Task(loaded).StatusId().Eval() == 1001, "Planned constant relation mismatch");

var updated = await loaded.UpdateName("Verified robot charging station")
    .AuditAs("Rename the verified robot task").SaveAsync(context);
Require(updated.Id == taskId && updated.Version == 2, "Update lost identity or optimistic version");

var log = (await Q.TaskExecutionLogs().Comment("prepare a robot task execution record")
    .Purpose("retain the explicit task history").NewEntityAsync(context))
    .UpdateTask(taskId).UpdateAction("Rename task")
    .UpdateDetail("Changed the task name after verification");
var savedLog = await log.AuditAs("Record the verified task rename").SaveAsync(context);
var logId = savedLog.Id ?? throw new InvalidOperationException("Created history record has no ID");

var history = await Q.TaskExecutionLogs().WithIdIs(logId).Limit(1)
    .Comment("load the new execution history record")
    .Purpose("verify the task history survives audited save").ExecuteForListAsync(context);
Require(history.Count == 1 && history[0].Task == taskId && history[0].Action == "Rename task",
    "Saved history is not associated with its task");

var reloaded = await Q.Tasks().WithIdIs(taskId).Limit(1)
    .Comment("reload the renamed robot task")
    .Purpose("verify persisted values with the generated E API").ExecuteForListAsync(context);
Require(reloaded.Count == 1 && E.Task(reloaded[0]).Name().Eval() == "Verified robot charging station"
    && E.Task(reloaded[0]).Version().Eval() == 2, "Persisted rename mismatch");
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new {
    marker = "DOTNET_TASK_BOARD", passed = true, taskId, logId,
    version = updated.Version, generatedQueries = true, generatedExpressions = true,
    auditedSaves = true, repeatedBootstrap = true, missingAuditRejected = true
}));
Console.WriteLine("PASS .NET generated Task Board");
