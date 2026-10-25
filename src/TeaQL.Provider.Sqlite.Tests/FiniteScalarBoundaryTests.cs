using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.Sqlite.Tests;

public class FiniteScalarBoundaryTests
{
    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public async Task NonFiniteBindingIsRejectedBeforeExecutingOnEvenAClosedConnection(double value)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        var transport = new SqliteTransport(connection);
        var error = await Assert.ThrowsAsync<ArgumentException>(() => transport.FetchAllSqlAsync(
            new CompiledQuery("SELECT @p0 AS amount", new List<Value> { new Value.F64Value(value) })));
        Assert.Contains("must be finite", error.Message);
        Assert.Equal(System.Data.ConnectionState.Closed, connection.State);
    }

    [Theory]
    [InlineData("1e999")]
    [InlineData("-1e999")]
    public async Task LegacyNonFiniteResultsFailRatherThanBecomingNull(string literal)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var transport = new SqliteTransport(connection);
        var error = await Assert.ThrowsAsync<FormatException>(() => transport.FetchAllSqlAsync(
            new CompiledQuery($"SELECT {literal} AS amount", new List<Value>())));
        Assert.Contains("Non-finite SQLite Float/Double", error.Message);
    }

    [Fact]
    public async Task FiniteAndNullResultsKeepTheirNativeTypes()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var row = Assert.Single(await new SqliteTransport(connection).FetchAllSqlAsync(
            new CompiledQuery("SELECT @p0 AS amount, NULL AS optional_amount",
                new List<Value> { new Value.F64Value(7.25) })));
        Assert.Equal(7.25, Assert.IsType<Value.F64Value>(row["amount"]).Value);
        Assert.IsType<Value.NullValue>(row["optional_amount"]);
    }
}
