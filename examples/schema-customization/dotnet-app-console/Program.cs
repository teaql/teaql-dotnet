using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

var database = Environment.GetEnvironmentVariable("TEAQL_SHOWCASE_DATABASE")
    ?? throw new InvalidOperationException("Supply an example SQLite path");
var label = Environment.GetEnvironmentVariable("TEAQL_SHOWCASE_ROUND")
    ?? throw new InvalidOperationException("Supply an example round label");
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = database, Pooling = false,
}.ToString());
await connection.OpenAsync();

var module = GeneratedRuntimeModule.Module;
var dataService = new SqlDataServiceExecutor(new SqliteDialect(),
    new SqliteTransport(connection), new MetadataSchemaProvider(module.Metadata.GetEntity));
var schema = new SchoolSchemaExecutor(dataService);
// The query/mutation provider can be installed after the application schema decorator.
var context = module.IntoContext().WithSchemaExecutor(schema).WithDataService(dataService);
await context.EnsureSchemaAsync();
await context.EnsureSchemaAsync();
Require(schema.SuccessfulCalls.Count == 2 && schema.SuccessfulCalls["Platform"] == 2
    && schema.SuccessfulCalls["School"] == 2, "the decorator must see both modeled entities on every ensure");

var name = $"Riverside-{label}";
var school = await Q.Schools().Comment("Initialize a school profile")
    .Purpose("Register the school with its district").NewEntityAsync(context);
school.UpdatePlatform(1).UpdateName(name);
await school.AuditAs("Register Riverside School in the district catalog").SaveAsync(context);
var found = await FindSchool(context, name);
Require(found is not null && found.Id == school.Id && found.Name == name,
    "the generated query did not return the registered school");

schema.RejectSchools = true;
try
{
    await context.EnsureSchemaAsync();
    throw new InvalidOperationException("the rejected school schema reached a successful ensure");
}
catch (SchoolSchemaRejected)
{
    Require(schema.RejectedSchoolCalls == 1, "the expected schema failure did not pass through the decorator");
}
var retained = await FindSchool(context, name);
Require(retained is not null && retained.Id == school.Id && retained.Name == name,
    "the schema failure disturbed the existing school");
Console.WriteLine("SCHOOL_SCHEMA=" + JsonSerializer.Serialize(new
{
    round = label, id = school.Id, name, ensuredSchool = schema.SuccessfulCalls["School"],
    rejectedSchool = schema.RejectedSchoolCalls, readAfterFailure = true,
}));

static Task<School?> FindSchool(UserContext context, string name) =>
    Q.Schools().WithNameIs(name).Limit(2).Comment("Read the registered school by name")
        .Purpose("Check query execution remains independent of schema setup").ExecuteForOneAsync(context);

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class SchoolSchemaRejected : Exception { }

sealed class SchoolSchemaExecutor(ISchemaExecutor inner) : ISchemaExecutor
{
    public Dictionary<string, int> SuccessfulCalls { get; } = new(StringComparer.Ordinal);
    public bool RejectSchools { get; set; }
    public int RejectedSchoolCalls { get; private set; }

    public async Task<SchemaResult> EnsureSchemaAsync(SchemaRequest request)
    {
        var name = request.EntityName ?? throw new InvalidOperationException("Missing schema entity name");
        if (RejectSchools && name == "School")
        {
            RejectedSchoolCalls++;
            throw new SchoolSchemaRejected();
        }
        var result = await inner.EnsureSchemaAsync(request);
        SuccessfulCalls[name] = SuccessfulCalls.GetValueOrDefault(name) + 1;
        return result;
    }
}
