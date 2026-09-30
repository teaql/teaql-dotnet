using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class BusinessIdAllocatorTests
{
    private static BusinessIdPlan Plan(string root, string @namespace,
        ulong maximum = 100) => new(
        BusinessIdDefinition.DailyPermuted(
            "order_number", "ORD", "order_number"),
        new BusinessIdScope(root, "commerce_order", @namespace, "20261001"),
        new DateOnly(2026, 10, 1), "20261001", 0, maximum);

    private static async Task<SqliteConnection> OpenAsync(string path)
    {
        var connection = new SqliteConnection(
            $"Data Source={path};Default Timeout=5;Pooling=False");
        await connection.OpenAsync();
        return connection;
    }

    [Fact]
    public async Task ConstructionDoesNotCreateSchema()
    {
        var path = Path.Combine(Path.GetTempPath(), $"teaql-business-explicit-{Guid.NewGuid():N}.db");
        try
        {
            await using var connection = await OpenAsync(path);
            var allocator = new OptimisticBusinessIdAllocator(
                new SqliteTransport(connection), new SqliteDialect());
            var error = await Assert.ThrowsAsync<SqlExecutorException>(() =>
                allocator.AllocateAsync(Plan("root", "order_number")));
            Assert.Contains("explicit EnsureSchemaAsync", error.Message);
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    [Fact]
    public async Task AllocationIsSharedConcurrentRestartSafeAndScopeIsolated()
    {
        var path = Path.Combine(Path.GetTempPath(), $"teaql-business-shared-{Guid.NewGuid():N}.db");
        try
        {
            await using var firstConnection = await OpenAsync(path);
            await using var secondConnection = await OpenAsync(path);
            var first = new OptimisticBusinessIdAllocator(
                new SqliteTransport(firstConnection), new SqliteDialect());
            var second = new OptimisticBusinessIdAllocator(
                new SqliteTransport(secondConnection), new SqliteDialect());
            await first.EnsureSchemaAsync();
            var plan = Plan("root", "order_number");

            var tasks = Enumerable.Range(0, 40)
                .Select(index => (index % 2 == 0 ? first : second).AllocateAsync(plan));
            var allocated = (await Task.WhenAll(tasks))
                .Select(value => value.Sequence).OrderBy(value => value).ToArray();
            Assert.Equal(Enumerable.Range(0, 40).Select(value => (ulong)value), allocated);

            await using var restartedConnection = await OpenAsync(path);
            var restarted = new OptimisticBusinessIdAllocator(
                new SqliteTransport(restartedConnection), new SqliteDialect());
            Assert.Equal(40ul, (await restarted.AllocateAsync(plan)).Sequence);
            Assert.Equal(0ul, (await restarted.AllocateAsync(
                Plan("other-root", "order_number"))).Sequence);
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    [Fact]
    public async Task CapacityExhaustionIsClassified()
    {
        var path = Path.Combine(Path.GetTempPath(), $"teaql-business-capacity-{Guid.NewGuid():N}.db");
        try
        {
            await using var connection = await OpenAsync(path);
            var allocator = new OptimisticBusinessIdAllocator(
                new SqliteTransport(connection), new SqliteDialect());
            await allocator.EnsureSchemaAsync();
            var plan = Plan("root", "tiny", 1);
            Assert.Equal(0ul, (await allocator.AllocateAsync(plan)).Sequence);
            Assert.Equal(1ul, (await allocator.AllocateAsync(plan)).Sequence);
            var error = await Assert.ThrowsAsync<BusinessIdException>(() =>
                allocator.AllocateAsync(plan));
            Assert.Equal(BusinessIdErrorCode.BusinessIdRangeExhausted, error.Code);
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }
}
