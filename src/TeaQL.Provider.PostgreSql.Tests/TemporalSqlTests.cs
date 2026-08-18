using Npgsql;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.PostgreSql.Tests;

public class TemporalSqlTests
{
    [Fact]
    public async Task PreparedAndDiagnosticSqlAreEquivalent()
    {
        var url = Environment.GetEnvironmentVariable("TEAQL_TEST_POSTGRES_URL");
        if (string.IsNullOrWhiteSpace(url)) return;
        await using var dataSource = NpgsqlDataSource.Create(url);
        var transport = new PostgreSqlTransport(dataSource);
        await transport.ExecuteSqlAsync(new CompiledQuery("DROP TABLE IF EXISTS teaql_temporal_dotnet_fixture", []));
        await transport.ExecuteSqlAsync(new CompiledQuery("CREATE TABLE teaql_temporal_dotnet_fixture(id BIGINT, d DATE, t TIMESTAMPTZ(3))", []));
        var prepared = new CompiledQuery(
            "INSERT INTO teaql_temporal_dotnet_fixture VALUES ($1, $2, $3)",
            [new Value.I64Value(1), new Value.DateValue(new DateTime(2024, 2, 29)), new Value.TimestampValue(-315521754322)],
            "teaql source=temporal.verify $1");
        await transport.ExecuteSqlAsync(prepared);
        await transport.ExecuteSqlAsync(new CompiledQuery(prepared.DebugSql(DatabaseKind.PostgreSql).Replace("VALUES (1,", "VALUES (2,"), []));
        var rows = await transport.FetchAllSqlAsync(new CompiledQuery("SELECT d, t FROM teaql_temporal_dotnet_fixture ORDER BY id", []));
        Assert.Equal(rows[0]["d"], rows[1]["d"]); Assert.Equal(rows[0]["t"], rows[1]["t"]);
        await transport.ExecuteSqlAsync(new CompiledQuery("DROP TABLE teaql_temporal_dotnet_fixture", []));
    }
}
