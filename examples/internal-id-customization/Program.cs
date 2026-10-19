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
var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
    new MetadataSchemaProvider(name => name == "School" ? school : null));
var allocator = new ExampleIdAllocator();
var context = module.IntoContext().WithDataService(provider).WithInternalIdAllocator(allocator)
    .DisableMutationSqlLog().DisableQuerySqlLog();
await context.EnsureSchemaAsync();

static InsertMutationRequest Insert(string name, ulong? id = null)
{
    var command = new InsertCommand("School").Value("name", new Value.TextValue(name));
    if (id is { } explicitId) command.Value("id", new Value.U64Value(explicitId));
    return new InsertMutationRequest(command, "save school with context-owned ID allocation");
}

var data = context.RequireResource<IDataService>();
await data.MutateAsync(Insert("direct"));
await data.MutateAsync(new BatchMutationRequest(new List<MutationRequest> {
    Insert("batch one"), Insert("batch two")
}, "save two schools together"));
await data.MutateAsync(Insert("explicit", 900));
await context.ExecuteGraphSaveAsync("save school graph", async graph => {
    var id = await graph.AllocateIdAsync("School");
    await graph.MutateAsync(Insert("graph", id));
    return true;
});

await using var read = connection.CreateCommand();
read.CommandText = "SELECT id FROM school_data ORDER BY id";
await using var reader = await read.ExecuteReaderAsync();
var ids = new List<long>();
while (await reader.ReadAsync()) ids.Add(reader.GetInt64(0));
if (!ids.SequenceEqual(new long[] { 500, 501, 502, 900, 901 }))
    throw new InvalidOperationException($"unexpected IDs: {string.Join(",", ids)}");
if (!allocator.ReservedFloors.Contains(900) || !allocator.ReservedFloors.Contains(901))
    throw new InvalidOperationException("explicit ID floor was not reserved through the installed allocator");
Console.WriteLine("PASS .NET context-owned internal ID allocation: direct, batch, explicit floor, graph");

sealed class ExampleIdAllocator : IInternalIdAllocator
{
    private ulong _level = 499;
    public List<ulong> ReservedFloors { get; } = new();
    public Task<ulong> NextIdAsync(string entity)
    {
        if (entity != "School") throw new ArgumentException("unknown entity", nameof(entity));
        return Task.FromResult(++_level);
    }
    public Task EnsureIdFloorAsync(string entity, ulong floor)
    {
        if (entity != "School") throw new ArgumentException("unknown entity", nameof(entity));
        ReservedFloors.Add(floor);
        _level = Math.Max(_level, floor);
        return Task.CompletedTask;
    }
}
