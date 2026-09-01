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
var context = module.IntoContext().WithDataService(executor);
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

sealed class ModuleSchemaProvider(RuntimeModule module) : ISchemaProvider
{
    public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name);
}
