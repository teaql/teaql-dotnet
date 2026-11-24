using Generated;
using Microsoft.Data.Sqlite;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

// The optional package is discovered at context startup. No SDK setup or spans
// belong in this application. Every run keeps a fresh database for evidence.
var directory = Path.Combine(Path.GetTempPath(), "teaql-dotnet-standard-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
var database = Path.Combine(directory, "example.sqlite");
await using var connection = new SqliteConnection($"Data Source={database}");
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection), new MetadataSchemaProvider(module.Metadata.GetEntity));
var audit = new Audits(); var ordinary = new SqlLogs();
var context = module.IntoContext().WithDataService(provider).WithAppAuditEventSink(audit).WithDiagnosticSqlLogSink(ordinary)
    .DisableQuerySqlLog().DisableMutationSqlLog();
try {
    await context.EnsureSchemaAsync();
    var first = audit.Count;
    await context.EnsureSchemaAsync();
    if (first != 1 || audit.Count != first) throw new Exception("expected one audited seed and no repeated seed");
    for (var round = 0; round < 2; round++) {
        var rows = await Q.Platforms().WithIdIs(1).Limit(1).Comment("load seeded platform")
            .Purpose("verify standard telemetry through generated Q and E").ExecuteForListAsync(context);
        if (rows.Count != 1 || E.Platform(rows[0]).Id().Eval() != 1 || string.IsNullOrEmpty(E.Platform(rows[0]).Name().Eval()))
            throw new Exception("generated Q/E did not verify the seeded root");
    }
    if (ordinary.Count != 0) throw new Exception("ordinary SQL logging switch ignored");
    var enabled = string.Equals(Environment.GetEnvironmentVariable(TelemetryStartupConfiguration.EnabledVariable), "true", StringComparison.OrdinalIgnoreCase);
    if (!enabled && AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetName().Name?.StartsWith("OpenTelemetry", StringComparison.Ordinal) == true))
        throw new Exception("disabled startup loaded an OpenTelemetry SDK assembly");
    await context.RuntimeTelemetry.FlushAsync();
    Console.WriteLine($"DOTNET_STANDARD_TELEMETRY_EXAMPLE PASSED first_seed_audits=1 repeat_seed_audits=0 queries=2 ordinary_sql_logs=0 database={database}");
}
finally { await context.RuntimeTelemetry.ShutdownAsync(); }

sealed class Audits : IAppAuditEventSink {
    public int Count;
    public Task RecordAsync(IReadOnlyDictionary<string, object?> audit, CancellationToken cancellationToken = default) { Count++; return Task.CompletedTask; }
}
sealed class SqlLogs : IDiagnosticSqlLogSink {
    public int Count;
    public void Write(ExecutionMetadata metadata) { Count++; }
}
