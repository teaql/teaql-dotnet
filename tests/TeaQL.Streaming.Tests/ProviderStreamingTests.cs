using Microsoft.Data.Sqlite;
using System.Data;
using Npgsql;
using TeaQL.Core;
using TeaQL.Provider.MySql;
using TeaQL.Provider.PostgreSql;
using TeaQL.Provider.Sqlite;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Streaming.Tests;

public class ProviderStreamingTests
{
    private static readonly List<Value> NoParameters = new();

    [Fact]
    public async Task SqliteStreamsAndSupportsEarlyCancellation()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var transport = new SqliteTransport(connection);
        var ids = await ReadIds(
            transport,
            "SELECT id FROM (SELECT 1 id UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5) ORDER BY id");
        Assert.Equal(new long[] { 1, 2, 3, 4, 5 }, ids);
        using var cancellation = new CancellationTokenSource();
        await foreach (var _ in transport.StreamSqlAsync(
                           new CompiledQuery("SELECT 1 id UNION ALL SELECT 2", NoParameters),
                           cancellation.Token))
        {
            cancellation.Cancel();
            break;
        }
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public async Task PostgreSqlStreamsFromRealDatabaseWhenConfigured()
    {
        var url = Environment.GetEnvironmentVariable("TEAQL_TEST_POSTGRES_URL");
        if (string.IsNullOrWhiteSpace(url)) return;
        await using var dataSource = NpgsqlDataSource.Create(url);
        var ids = await ReadIds(
            new PostgreSqlTransport(dataSource),
            "SELECT id FROM (VALUES (1), (2), (3), (4), (5)) AS fixture(id) ORDER BY id");
        Assert.Equal(new long[] { 1, 2, 3, 4, 5 }, ids);
    }

    [Fact]
    public async Task MySqlStreamsFromRealDatabaseWhenConfigured()
    {
        var url = Environment.GetEnvironmentVariable("TEAQL_TEST_MYSQL_URL");
        if (string.IsNullOrWhiteSpace(url)) return;
        using var transport = new MySqlTransport(url);
        var ids = await ReadIds(
            transport,
            "SELECT id FROM (SELECT 1 id UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5) fixture ORDER BY id");
        Assert.Equal(new long[] { 1, 2, 3, 4, 5 }, ids);
    }

    private static async Task<List<long>> ReadIds(IStreamingSqlTransport transport, string sql)
    {
        var ids = new List<long>();
        await foreach (var row in transport.StreamSqlAsync(new CompiledQuery(sql, NoParameters)))
            ids.Add(((Value.I64Value)row["id"]).Value);
        return ids;
    }
}
