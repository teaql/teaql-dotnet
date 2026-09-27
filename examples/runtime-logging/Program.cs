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
    .Relation(RelationDescriptor.New("students", "Student").ForeignKey("schoolId").Many());
var student = EntityDescriptor.New("Student").TableName("student_data")
    .Property(PropertyDescriptor.New("id", DataType.I64).Id())
    .Property(PropertyDescriptor.New("schoolId", DataType.I64).ColumnName("school_id"))
    .Property(PropertyDescriptor.New("name", DataType.Text))
    .Property(PropertyDescriptor.New("version", DataType.I64).Version());
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
await service.MutateAsync(new InsertMutationRequest(schoolInsert));

var studentInsert = new InsertCommand("Student")
    .Value("id", new Value.I64Value(10))
    .Value("schoolId", new Value.I64Value(1))
    .Value("name", new Value.TextValue("Ada"))
    .Value("version", new Value.I64Value(1));
studentInsert.TraceChain.Add(new TraceNode("Student", null, "why: seed related fixture")
    { Kind = "auditReason", Name = "Student" });
await service.MutateAsync(new InsertMutationRequest(studentInsert));

var query = new SelectQuery("School").Relation("students");
var result = await service.QueryAsync(new QueryRequest
{
    Query = query,
    Comment = "what: load schools and students",
    Purpose = "why: prove default multi-level relation logging",
    TraceChain = new List<TraceNode>
    {
        new("School", null, "students") { Kind = "relation", Name = "School.students" }
    }
});
if (result.Rows.Count != 1 || result.Rows[0]["students"] is not Value.ListValue children
    || children.Values.Count != 1)
    throw new InvalidOperationException("relation hydration did not return the retained fixture");

Console.WriteLine("PASS .NET runtime-source logging example");

var markers = new[] { "PRIVATE-CREATE-CANARY", "PRIVATE-UPDATE-CANARY", "PRIVATE-FAILURE-CANARY" };
InsertMutationRequest PrivacyInsert(string name)
{
    var command = new InsertCommand("School").Value("id", 99L).Value("name", name).Value("version", 1L);
    command.TraceChain.Add(new TraceNode("School", null, "why: verify privacy persistence")
        { Kind = "auditReason", Name = "School" });
    return new InsertMutationRequest(command);
}
async Task VerifyPrivacyRow(string? name)
{
    var rows = (await service.QueryAsync(new QueryRequest
    {
        Query = new SelectQuery("School").Filter(Expr.Eq("id", new Value.I64Value(99))).Limit(1),
        Comment = "what: read privacy fixture", Purpose = "why: verify original values"
    })).Rows;
    if (name is null ? rows.Count != 0 : rows.Count != 1 || rows[0]["name"] != new Value.TextValue(name))
        throw new InvalidOperationException("privacy projection changed persistence");
}
await service.MutateAsync(PrivacyInsert(markers[0]));
await VerifyPrivacyRow(markers[0]);
var privacyUpdate = new UpdateCommand("School", new Value.I64Value(99)).ExpectedVersion(1).Value("name", markers[1]);
privacyUpdate.TraceChain.Add(new TraceNode("School", null, "why: update privacy fixture") { Kind = "auditReason", Name = "School" });
await service.MutateAsync(new UpdateMutationRequest(privacyUpdate));
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
await service.MutateAsync(new DeleteMutationRequest(privacyDelete));
await VerifyPrivacyRow(null);
var privacyLog = File.ReadAllText(logPath) + captured.Text;
if (captured.Count < 7 || privacyLog.Length == 0 || markers.Any(privacyLog.Contains))
    throw new InvalidOperationException("missing or unsafe privacy log evidence");
Console.WriteLine("PASS .NET database-backed CRUD/failure log privacy");

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

sealed class ModuleSchemaProvider(RuntimeModule module) : ISchemaProvider
{
    public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name);
}
