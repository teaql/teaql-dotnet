using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

var path = Path.Combine(Path.GetTempPath(), $"teaql-business-id-{Guid.NewGuid():N}.db");
try
{
    await using var connection = new SqliteConnection($"Data Source={path};Pooling=False");
    await connection.OpenAsync();
    var allocator = new OptimisticBusinessIdAllocator(
        new SqliteTransport(connection), new SqliteDialect());
    await allocator.EnsureSchemaAsync();
    var context = new UserContext()
        .WithBusinessClock(new FixedBusinessClock(
            new DateTimeOffset(2026, 10, 1, 8, 30, 0, TimeSpan.Zero)))
        .WithBusinessIdKeyProvider(new StaticBusinessIdKeyProvider(
            new BusinessIdEncodingKey(1,
                Enumerable.Range(0, 32).Select(value => (byte)value).ToArray())))
        .WithBusinessIdProfileFactory(new DefaultBusinessIdProfileFactory())
        .WithBusinessIdService(new DefaultBusinessIdService(allocator));
    var slot = new OrderNumberSlot();
    var definition = BusinessIdDefinition.DailyPermuted(
        "order_number", "ORD", "order_number");
    var first = await context.EnsureBusinessIdAsync(
        definition, "commerce", "commerce_order", slot);
    var retry = await context.EnsureBusinessIdAsync(
        definition, "commerce", "commerce_order", slot);
    if (first != retry || !slot.CurrentValue!.StartsWith("ORD-20261001-"))
        throw new InvalidOperationException("Business ID retry was not idempotent");
    Console.WriteLine("PASS .NET governed Business ID lifecycle example {0}", first.Value);
}
finally { if (File.Exists(path)) File.Delete(path); }

sealed class OrderNumberSlot : IBusinessIdSlot
{
    public string? CurrentValue { get; private set; }
    public bool IsNewAggregate => true;
    public void AssignCanonicalValue(string value) => CurrentValue = value;
}
