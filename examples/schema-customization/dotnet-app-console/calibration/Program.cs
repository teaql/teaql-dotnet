using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var school = EntityDescriptor.New("School").TableName("school_data")
    .Property(PropertyDescriptor.New("id", DataType.I64).Id())
    .Property(PropertyDescriptor.New("name", DataType.Text));
var module = new RuntimeModule().Entity(school);
var sql = new SqlDataServiceExecutor(new SqliteDialect(),
    new SqliteTransport(connection), new ModuleSchemaProvider(module));
var schema = new CountingSchemaExecutor((ISchemaExecutor)sql);

// The custom schema capability is independent of the query/mutation provider.
// It remains installed even when WithDataService is called afterward.
var context = module.IntoContext().WithSchemaExecutor(schema).WithDataService(sql);
await context.EnsureSchemaAsync();
await context.EnsureSchemaAsync();
if (schema.Calls != 2) throw new InvalidOperationException("custom schema executor was bypassed");

var rows = await context.RequireResource<IDataService>().QueryAsync(new QueryRequest(
    new SelectQuery("School").Limit(1),
    new QueryIntent("read school after schema setup", "verify composed data service")));
if (rows.Rows.Count != 0) throw new InvalidOperationException("unexpected school rows");
Console.WriteLine("PASS .NET context-owned schema SPI and independent data-service composition");

sealed class CountingSchemaExecutor(ISchemaExecutor inner) : ISchemaExecutor
{
    public int Calls { get; private set; }

    public async Task<SchemaResult> EnsureSchemaAsync(SchemaRequest request)
    {
        Calls++;
        return await inner.EnsureSchemaAsync(request);
    }
}

sealed class ModuleSchemaProvider(RuntimeModule module) : ISchemaProvider
{
    public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name);
}
