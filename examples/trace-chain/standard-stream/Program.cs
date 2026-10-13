using Generated;
using Microsoft.Data.Sqlite;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

var database = Environment.GetEnvironmentVariable("TEAQL_STANDARD_STREAM_DB")
    ?? Path.Combine(Path.GetTempPath(), "teaql-dotnet-standard-stream-" + Guid.NewGuid().ToString("N") + ".sqlite");
await using var connection = new SqliteConnection("Data Source=" + database); await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection), new MetadataSchemaProvider(module.Metadata.GetEntity));
var audits = new Audits(); var ordinary = new SqlLogs();
var context = module.IntoContext().WithDataService(provider).WithAppAuditEventSink(audits)
    .WithDiagnosticSqlLogSink(ordinary).DisableQuerySqlLog().DisableMutationSqlLog();
try
{
    await context.EnsureSchemaAsync(); var first = audits.Count;
    await context.EnsureSchemaAsync(); Require(first is 0 or 1 && audits.Count == first, "audited idempotent bootstrap");
    var secret = "STREAM-PRIVATE-" + Guid.NewGuid().ToString("N");
    var parent = Q.CustomerOrders().Comment("prepare standard stream parent").Purpose("own bounded stream rows")
        .NewEntity(context).UpdatePlatformId(1).UpdateOrderNumber(secret).UpdateDescription("stream fixture");
    for (var index = 0; index < 3; index++) parent.OrderItemList.Add(Q.OrderItems().Comment("prepare standard streamed entity")
        .Purpose("seed bounded stream rows").NewEntity(context).UpdateName(secret));
    await parent.AuditAs("seed standard stream graph").SaveAsync(context);
    _ = Q.OrderItems().WithNameIs(secret).Limit(3).Comment("unpolled standard stream")
        .Purpose("verify lazy standard stream").ExecuteForStreamAsync(context, chunkSize: 2);
    var completeRequest = Q.OrderItems().WithNameIs(secret).OrderByIdAscending().Limit(3).Comment("complete standard stream");
    var complete = completeRequest.Purpose("verify standard complete stream").ExecuteForStreamAsync(context, chunkSize: 2);
    completeRequest.WithNameIs("changed after stream capture").Comment("changed builder comment");
    var early = Q.OrderItems().WithNameIs(secret).OrderByIdAscending().Limit(3).Comment("close standard stream early")
        .Purpose("verify standard early-close stream").ExecuteForStreamAsync(context, chunkSize: 2);
    var delivered = 0;
    await using (var left = complete.GetAsyncEnumerator())
    {
        Require(await left.MoveNextAsync(), "complete first row");
        Require(E.OrderItem(left.Current).Name().Eval() == secret, "first typed streamed row"); delivered++;
        var platform = await Q.PlatformsWithMinimalFields().WithIdIs(1).SelectName().Limit(1)
            .Comment("intervening standard stream query").Purpose("verify stream activation isolation").ExecuteForListAsync(context);
        Require(platform.Count == 1 && E.Platform(platform[0]).Name().Eval() == "Trace Chain Verification", "independent typed query");
        await using (var right = early.GetAsyncEnumerator())
        {
            Require(await right.MoveNextAsync(), "early-close first row");
            Require(E.OrderItem(right.Current).Name().Eval() == secret, "early typed row");
            while (await left.MoveNextAsync()) { Require(E.OrderItem(left.Current).Name().Eval() == secret, "remaining typed rows"); delivered++; }
        }
    }
    using (var cancellation = new CancellationTokenSource())
    {
        var cancellable = Q.OrderItems().WithNameIs(secret).OrderByIdAscending().Limit(3).Comment("cancel standard stream by token")
            .Purpose("verify standard token-cancel stream").ExecuteForStreamAsync(context, chunkSize: 2);
        await using var cursor = cancellable.GetAsyncEnumerator(cancellation.Token);
        Require(await cursor.MoveNextAsync(), "token-cancel first row"); cancellation.Cancel();
        var rejected = false;
        try { await cursor.MoveNextAsync(); } catch (OperationCanceledException) { rejected = true; }
        Require(rejected, "cancel token must reject prefetched next typed row");
    }
    Require(delivered == 3 && audits.Count == first + 4 && ordinary.Count == 0, "stream cardinality and independent audit/log controls");
    await context.RuntimeTelemetry.FlushAsync();
    Console.WriteLine($"DOTNET_STANDARD_STREAM PASSED complete=3 early=1 chunkSize=2 token=1 capturedBuilder=true interveningQuery=1 unpolled=0 first_seed_audits={first} repeat_seed_audits=0 audits={audits.Count} ordinary_sql_logs=0 database={database}");
}
finally { await context.RuntimeTelemetry.ShutdownAsync(); }

static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
sealed class Audits : IAppAuditEventSink
{
    public int Count;
    public Task RecordAsync(IReadOnlyDictionary<string, object?> audit, CancellationToken cancellationToken = default)
    { Count++; return Task.CompletedTask; }
}
sealed class SqlLogs : IDiagnosticSqlLogSink
{
    public int Count;
    public void Write(ExecutionMetadata metadata) { Count++; }
}
