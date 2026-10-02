using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var school = EntityDescriptor.New("School").TableName("school_data")
    .Property(PropertyDescriptor.New("id", DataType.I64).Id())
    .Property(PropertyDescriptor.New("name", DataType.Text))
    .Property(PropertyDescriptor.New("version", DataType.I64).Version())
    .AuditMaskFields(new() { "name" })
    .Relation(RelationDescriptor.New("students", "Student").ForeignKey("schoolId").Many());
var student = EntityDescriptor.New("Student").TableName("student_data")
    .Property(PropertyDescriptor.New("id", DataType.I64).Id())
    .Property(PropertyDescriptor.New("schoolId", DataType.I64).ColumnName("school_id"))
    .Property(PropertyDescriptor.New("name", DataType.Text))
    .Property(PropertyDescriptor.New("password", DataType.Text))
    .Property(PropertyDescriptor.New("version", DataType.I64).Version())
    // Explicitly declare the generated-era policy. Absent metadata must stay
    // fail-closed; this example intentionally leaves ordinary fields visible.
    .AuditMaskFields(new());
var module = new RuntimeModule().Entity(school).Entity(student);
var executor = new SqlDataServiceExecutor(
    new SqliteDialect(), new SqliteTransport(connection), new ModuleSchemaProvider(module));
// Retain a real file endpoint and a custom sink to prove projection happens
// before either destination, without changing database execution values.
var logPath = Path.Combine(Path.GetTempPath(), $"teaql-privacy-{Guid.NewGuid()}.log");
using var logWriter = new StreamWriter(logPath) { AutoFlush = true };
var captured = new PrivacyEvidenceSink();
var context = module.IntoContext().WithDataService(executor)
    .WithDiagnosticSqlLogSink(new TextDiagnosticSqlLogSink(logWriter))
    .WithSensitiveDiagnosticSqlLogSink(captured);
await context.EnsureSchemaAsync();
var service = context.RequireResource<IDataService>();

var schoolInsert = new InsertCommand("School")
    .Value("id", new Value.I64Value(1))
    .Value("name", new Value.TextValue("Runtime School"))
    .Value("version", new Value.I64Value(1));
schoolInsert.TraceChain.Add(new TraceNode("School", null, "why: seed runtime log fixture")
    { Kind = "auditReason", Name = "School" });
await service.MutateAsync(new InsertMutationRequest(schoolInsert, "why: seed runtime log fixture"));

var studentInsert = new InsertCommand("Student")
    .Value("id", new Value.I64Value(10))
    .Value("schoolId", new Value.I64Value(1))
    .Value("name", new Value.TextValue("Ada"))
    .Value("version", new Value.I64Value(1));
studentInsert.TraceChain.Add(new TraceNode("Student", null, "why: seed related fixture")
    { Kind = "auditReason", Name = "Student" });
await service.MutateAsync(new InsertMutationRequest(studentInsert, "why: seed related fixture"));

var query = new SelectQuery("School").Relation("students").Limit(10)
    .Comment("what: load schools and students")
    .Purpose("why: prove default multi-level relation logging");
var result = await service.QueryAsync(new QueryRequest(query)
{
    TraceChain = new List<TraceNode>
    {
        new("School", null, "students") { Kind = "relation", Name = "School.students" }
    }
});
if (result.Rows.Count != 1 || result.Rows[0]["students"] is not Value.ListValue children
    || children.Values.Count != 1)
    throw new InvalidOperationException("relation hydration did not return the retained fixture");

var queryLog = File.ReadAllText(logPath);
if (!queryLog.Contains("comment=what: load schools and students")
    || !queryLog.Contains("purpose=why: prove default multi-level relation logging"))
    throw new InvalidOperationException("query-owned intent was lost before file logging");
Console.WriteLine("PASS .NET runtime-source logging example with query-owned intent");

var markers = new[] { "PRIVATE-CREATE-CANARY", "PRIVATE-UPDATE-CANARY", "PRIVATE-FAILURE-CANARY" };
InsertMutationRequest PrivacyInsert(string name)
{
    var command = new InsertCommand("School").Value("id", 99L).Value("name", name).Value("version", 1L);
    command.TraceChain.Add(new TraceNode("School", null, "why: verify privacy persistence")
        { Kind = "auditReason", Name = "School" });
    return new InsertMutationRequest(command, "why: verify privacy persistence");
}
async Task VerifyPrivacyRow(string? name)
{
    var rows = (await service.QueryAsync(new QueryRequest(
        new SelectQuery("School").Filter(Expr.Eq("id", new Value.I64Value(99))).Limit(1),
        new QueryIntent("what: read privacy fixture", "why: verify original values")))).Rows;
    if (name is null ? rows.Count != 0 : rows.Count != 1 || rows[0]["name"] != new Value.TextValue(name))
        throw new InvalidOperationException("privacy projection changed persistence");
}
await service.MutateAsync(PrivacyInsert(markers[0]));
await VerifyPrivacyRow(markers[0]);
var privacyUpdate = new UpdateCommand("School", new Value.I64Value(99)).ExpectedVersion(1).Value("name", markers[1]);
privacyUpdate.TraceChain.Add(new TraceNode("School", null, "why: update privacy fixture") { Kind = "auditReason", Name = "School" });
await service.MutateAsync(new UpdateMutationRequest(privacyUpdate, "why: update privacy fixture"));
await VerifyPrivacyRow(markers[1]);
try
{
    await service.MutateAsync(PrivacyInsert(markers[2]));
    throw new InvalidOperationException("duplicate key unexpectedly succeeded");
}
catch (SqlExecutorException error) when (error.InnerException is SqliteException { SqliteErrorCode: 19 }) { }
await VerifyPrivacyRow(markers[1]);
var privacyDelete = new DeleteCommand("School", new Value.I64Value(99)).ExpectedVersion(2).HardDelete();
privacyDelete.TraceChain.Add(new TraceNode("School", null, "why: delete privacy fixture") { Kind = "auditReason", Name = "School" });
await service.MutateAsync(new DeleteMutationRequest(privacyDelete, "why: delete privacy fixture"));
await VerifyPrivacyRow(null);
var privacyLog = File.ReadAllText(logPath) + captured.Text;
if (captured.Count < 7 || privacyLog.Length == 0 || markers.Any(privacyLog.Contains))
    throw new InvalidOperationException("missing or unsafe privacy log evidence");
if (!privacyLog.Contains("Ru**********ol") || privacyLog.Contains("Parameterized SQL:")
    || privacyLog.Contains("SQL OMITTED") || !privacyLog.Contains("/* masked */"))
    throw new InvalidOperationException("SQL must be expanded with field-aware masks, not omitted or parameterized");
Console.WriteLine("PASS .NET database-backed CRUD/failure log privacy");

// A logical batch must keep independent statement bindings and credential policy.
var batchRequests = new List<MutationRequest>();
foreach (var id in new[] { 20L, 21L })
{
    var command = new InsertCommand("Student").Value("id", id).Value("schoolId", 1L)
        .Value("name", $"Public Student {id}").Value("password", "BATCH-PASSWORD-CANARY").Value("version", 1L);
    command.TraceChain.Add(new TraceNode("Student", null, "why: verify independent batch log")
        { Kind = "auditReason", Name = "Student" });
    batchRequests.Add(new InsertMutationRequest(command, "why: verify independent batch log"));
}
await service.MutateAsync(new BatchMutationRequest(batchRequests, "why: verify independent batch log"));
var batchRows = (await service.QueryAsync(new QueryRequest(
    new SelectQuery("Student").Filter(Expr.Gte("id", 20L)).Limit(2),
    new QueryIntent("what: read batch rows", "why: ensure log projection preserves credentials in storage")))).Rows;
if (batchRows.Count != 2 || batchRows.Any(row => row["password"].TryText() != "BATCH-PASSWORD-CANARY"))
    throw new InvalidOperationException("batch logging changed driver data");
var batchLog = File.ReadAllText(logPath) + captured.Text;
if (batchLog.Contains("BATCH-PASSWORD-CANARY") || !batchLog.Contains("Public Student 20")
    || !batchLog.Contains("Public Student 21") || batchLog.Contains("SQL OMITTED"))
    throw new InvalidOperationException("batch logs lost per-field policy or independent bindings");
Console.WriteLine("PASS .NET real SQLite batch logs preserve ordinary fields and hide credentials");

// The second statement fails. The first executed successfully, but SQLite's
// automatic transaction rolls it back. SQL success must not imply commit.
var partialStart = File.ReadAllText(logPath).Length;
var partialRequests = new List<MutationRequest>();
foreach (var id in new[] { 30L, 20L, 31L })
{
    var command = new InsertCommand("Student").Value("id", id).Value("schoolId", 1L)
        .Value("name", $"Partial Student {id}").Value("password", "PARTIAL-PASSWORD-CANARY").Value("version", 1L);
    command.TraceChain.Add(new TraceNode("Student", null, "why: verify partial batch rollback")
        { Kind = "auditReason", Name = "Student" });
    partialRequests.Add(new InsertMutationRequest(command, "why: verify partial batch rollback"));
}
try
{
    await service.MutateAsync(new BatchMutationRequest(partialRequests, "why: verify partial batch rollback"));
    throw new InvalidOperationException("partial batch unexpectedly succeeded");
}
catch (SqlExecutorException error) when (error.InnerException is SqliteException { SqliteErrorCode: 19 }) { }
var partialLog = File.ReadAllText(logPath)[partialStart..];
if (partialLog.Split("[TeaQL SQL]").Length - 1 != 2
    || !partialLog.Contains("outcome=success") || !partialLog.Contains("outcome=failure")
    || partialLog.IndexOf("outcome=success", StringComparison.Ordinal) > partialLog.IndexOf("outcome=failure", StringComparison.Ordinal)
    || !partialLog.Contains("Partial Student 30") || !partialLog.Contains("Partial Student 20")
    || partialLog.Contains("Partial Student 31") || partialLog.Contains("PARTIAL-PASSWORD-CANARY"))
    throw new InvalidOperationException("partial batch diagnostics lost statement order, scope, or masking");
var rolledBackRows = await service.QueryAsync(new QueryRequest(
    new SelectQuery("Student").Filter(Expr.Gte("id", 30L)).Limit(2),
    new QueryIntent("what: inspect partial batch rows", "why: distinguish SQL success from transaction commit")));
if (rolledBackRows.Rows.Count != 0)
    throw new InvalidOperationException("partial batch did not roll back");
Console.WriteLine("PASS .NET partial SQLite batch reports executed SQL without claiming commit");

// Test-only DDL forces an invalid authoritative snapshot after a successful
// audited write. The trigger and insert are rolled back/removed independently.
using (var trigger = connection.CreateCommand())
{
    trigger.CommandText = "CREATE TRIGGER log_readback_probe AFTER INSERT ON school_data WHEN NEW.id = 777 BEGIN DELETE FROM school_data WHERE id = NEW.id; END";
    await trigger.ExecuteNonQueryAsync();
}
var readbackStart = File.ReadAllText(logPath).Length;
const string plaintextFlag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
var previousPlaintext = Environment.GetEnvironmentVariable(plaintextFlag);
var readbackDebug = new RetainedDebugSink();
try
{
    // Explicit test-only opt-in: debug records stay in memory, never in the file sink.
    Environment.SetEnvironmentVariable(plaintextFlag, "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK");
    context.WithSensitiveDiagnosticSqlLogSink(readbackDebug);
    var command = new InsertCommand("School").Value("id", 777L)
        .Value("name", "READBACK-PRIVATE-CANARY").Value("version", 1L);
    command.TraceChain.Add(new TraceNode("School", null, "why: verify READBACK-PRIVATE-CANARY authoritative snapshot")
        { Kind = "auditReason", Name = "School" });
    try
    {
        await service.MutateAsync(new InsertMutationRequest(command, "why: verify READBACK-PRIVATE-CANARY authoritative snapshot"));
        throw new InvalidOperationException("missing authoritative snapshot unexpectedly accepted");
    }
    catch (SqlExecutorException error) when (error.Message.StartsWith("Authoritative persisted row not found", StringComparison.Ordinal)) { }
    if (readbackDebug.Entries.Count != 2 || !readbackDebug.Entries[1].Comment!.Contains("READBACK-PRIVATE-CANARY"))
        throw new InvalidOperationException("missing debug readback evidence");
    Environment.SetEnvironmentVariable(plaintextFlag, null);
    var downgradedPath = Path.Combine(Path.GetTempPath(), $"teaql-readback-revoked-{Guid.NewGuid()}.log");
    using (var writer = new StreamWriter(downgradedPath))
    {
        var revokedSink = new SensitiveDiagnosticSqlLogSink(writer);
        foreach (var entry in readbackDebug.Entries) revokedSink.Write(entry);
    }
    var downgraded = File.ReadAllText(downgradedPath);
    if (downgraded.Contains("READBACK-PRIVATE-CANARY") || downgraded.Contains("DEBUG PLAINTEXT")
        || !downgraded.Contains("authoritative snapshot") || !downgraded.Contains("SELECT"))
        throw new InvalidOperationException("retained readback debug record leaked after revoking opt-in");
    Console.WriteLine("PASS .NET retained debug readback is safely re-emitted to disk after opt-in revocation");
}
finally
{
    Environment.SetEnvironmentVariable(plaintextFlag, previousPlaintext);
    context.WithSensitiveDiagnosticSqlLogSink(captured);
    using var dropTrigger = connection.CreateCommand();
    dropTrigger.CommandText = "DROP TRIGGER log_readback_probe";
    await dropTrigger.ExecuteNonQueryAsync();
}
var readbackLog = File.ReadAllText(logPath)[readbackStart..];
if (readbackLog.Split("[TeaQL SQL]").Length - 1 != 2
    || !readbackLog.Contains("1 rows affected outcome=success")
    || !readbackLog.Contains("0 rows returned outcome=success")
    || readbackLog.Contains("outcome=failure") || readbackLog.Contains("READBACK-PRIVATE-CANARY"))
    throw new InvalidOperationException("readback diagnostic confused SQL success with snapshot validity or leaked inherited intent");
var afterReadback = await service.QueryAsync(new QueryRequest(
    new SelectQuery("School").Filter(Expr.Eq("id", 777L)).Limit(1),
    new QueryIntent("what: inspect readback probe", "why: verify failed mutation is not persisted")));
if (afterReadback.Rows.Count != 0)
    throw new InvalidOperationException("failed readback mutation was committed");
Console.WriteLine("PASS .NET SQLite readback reports write success and zero-row query without exposing inherited intent");

// Query through the context-bound wrapper so stream disposal follows the same
// projection policy as ordinary queries, including file and custom sinks.
var streamLogStart = File.ReadAllText(logPath).Length;
var delivered = 0;
await foreach (var chunk in ((IStreamQueryExecutor)service).QueryStreamAsync(new QueryRequest(
    new SelectQuery("School").Filter(Expr.Eq("name", "Runtime School")).Limit(3),
    new QueryIntent("what: stream schools", "why: verify early disposal diagnostics")), 1))
{
    if (chunk.Rows[0]["name"].TryText() != "Runtime School")
        throw new InvalidOperationException("stream changed original data");
    delivered += chunk.Rows.Count;
    break;
}
var streamLog = File.ReadAllText(logPath)[streamLogStart..];
if (delivered != 1 || !streamLog.Contains("outcome=cancelled") || !streamLog.Contains("1 rows returned")
    || !streamLog.Contains("Ru**********ol") || streamLog.Contains("Runtime School"))
    throw new InvalidOperationException("missing or unsafe stream terminal diagnostic");
// Reuse the same SQLite connection after cursor disposal.
var afterStream = await service.QueryAsync(new QueryRequest(
    new SelectQuery("School").Filter(Expr.Eq("id", 1L)).Limit(1),
    new QueryIntent("what: inspect school after stream", "why: verify connection remains usable")));
if (afterStream.Rows.Count != 1 || !File.ReadAllText(logPath).Contains("outcome=failure"))
    throw new InvalidOperationException("missing SQLite failure evidence or unusable connection");
Console.WriteLine("PASS .NET real SQLite stream disposal and failure terminal diagnostics");

// Test-only fault injection: the root query succeeds, but the related table is
// temporarily unavailable. Restore it without deleting data, even on failure.
var relationLogStart = File.ReadAllText(logPath).Length;
var relationSinkStart = captured.Text.Length;
using (var rename = connection.CreateCommand())
{
    rename.CommandText = "ALTER TABLE student_data RENAME TO student_data_relation_probe";
    await rename.ExecuteNonQueryAsync();
}
try
{
    try
    {
        await service.QueryAsync(new QueryRequest(new SelectQuery("School")
            .Filter(Expr.Eq("name", "Runtime School")).Limit(1)
            .RelationQuery("students", new SelectQuery("Student").Limit(10))
            .Comment("what: load Runtime School details").Purpose("why: verify derived query privacy")));
        throw new InvalidOperationException("missing relation table unexpectedly succeeded");
    }
    catch (SqlExecutorException error) when (error.InnerException is SqliteException { SqliteErrorCode: 1 }) { }
}
finally
{
    using var restore = connection.CreateCommand();
    restore.CommandText = "ALTER TABLE student_data_relation_probe RENAME TO student_data";
    await restore.ExecuteNonQueryAsync();
}
var relationLog = File.ReadAllText(logPath)[relationLogStart..] + captured.Text[relationSinkStart..];
if (relationLog.Contains("Runtime School") || !relationLog.Contains("student_data")
    || !relationLog.Contains("what: load [REDACTED] details") || !relationLog.Contains("outcome=failure"))
    throw new InvalidOperationException("derived relation diagnostic lost SQL or leaked inherited intent");
var restoredGraph = await service.QueryAsync(new QueryRequest(new SelectQuery("School").Limit(1)
    .RelationQuery("students", new SelectQuery("Student").Project("id").Limit(10))
    .Comment("what: reload restored graph").Purpose("why: prove relation fixture is recoverable")));
if (restoredGraph.Rows.Count != 1 || restoredGraph.Rows[0]["students"] is not Value.ListValue restoredStudents
    || restoredStudents.Values.Count != 3
    || !restoredStudents.Values.Select(value => ((Value.ObjectValue)value).Value["id"].TryI64())
        .OrderBy(id => id).SequenceEqual(new long?[] { 10, 20, 21 }))
    throw new InvalidOperationException("relation fault injection changed stored data");
Console.WriteLine("PASS .NET real SQLite derived relation failure masks inherited intent before file and custom sinks");

sealed class PrivacyEvidenceSink : ISensitiveDiagnosticSqlLogSink
{
    public int Count { get; private set; }
    public string Text { get; private set; } = "";
    public void Write(ExecutionMetadata metadata)
    {
        Count++;
        Text += System.Text.Json.JsonSerializer.Serialize(metadata);
        Text += string.Join(",", metadata.Parameters);
    }
}

sealed class RetainedDebugSink : ISensitiveDiagnosticSqlLogSink
{
    public List<ExecutionMetadata> Entries { get; } = new();
    public void Write(ExecutionMetadata metadata) => Entries.Add(metadata);
}

sealed class ModuleSchemaProvider(RuntimeModule module) : ISchemaProvider
{
    public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name);
}
