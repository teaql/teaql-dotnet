using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.Sqlite.Tests;

public class IntegralBindingBoundaryTests
{
    [Fact]
    public async Task IntegralBindingCannotWrapUnsignedOverflowEvenOnAClosedConnection()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        var transport = new SqliteTransport(connection);
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => transport.FetchAllSqlAsync(
            new CompiledQuery("SELECT @p0", [new Value.U64Value(ulong.MaxValue)])));
        Assert.Equal(System.Data.ConnectionState.Closed, connection.State);
    }
}
