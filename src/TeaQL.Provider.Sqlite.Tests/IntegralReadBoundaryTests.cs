using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Sql;

namespace TeaQL.Provider.Sqlite.Tests;

public class IntegralReadBoundaryTests
{
    public static IEnumerable<object[]> CorruptValues()
    {
        foreach (var streaming in new[] { false, true })
            foreach (var (field, literal) in new[] {
                ("required_integer", "2147483648"), ("required_integer", "-2147483649"),
                ("required_integer", "12.75"), ("required_integer", "'PRIVATE_CORRUPT_INTEGER'"),
                ("required_long", "9223372036854775808"), ("required_long", "'PRIVATE_CORRUPT_LONG'") })
                yield return [field, literal, streaming];
    }

    [Theory]
    [MemberData(nameof(CorruptValues))]
    public async Task IntegralReadRejectsCorruptionBeforeDeliveringListOrStream(string field, string literal, bool streaming)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var transport = new SqliteTransport(connection);
        await transport.ExecuteSqlAsync(new CompiledQuery("CREATE TABLE integral_probe(id INTEGER, required_integer INTEGER, required_long INTEGER)", []));
        await transport.ExecuteSqlAsync(new CompiledQuery($"INSERT INTO integral_probe(id, {field}) VALUES(1, {literal})", []));
        var executor = Executor(transport);
        var query = new SelectQuery("IntegralProbe").Project(field).Limit(2);
        var request = Request(query);
        var delivered = 0;
        var error = await Assert.ThrowsAsync<SqlExecutorException>(async () => {
            if (streaming)
                await foreach (var chunk in executor.QueryStreamAsync(request, 1)) delivered += chunk.Rows.Count;
            else delivered = (await executor.QueryAsync(request)).Rows.Count;
        });
        Assert.Contains("IntegralProbe." + field, error.Message);
        Assert.DoesNotContain("PRIVATE_CORRUPT", error.Message);
        Assert.Equal(0, delivered);
    }

    [Fact]
    public async Task IntegralReadKeepsEndpointsSparseNullAndAggregateResultDomains()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var transport = new SqliteTransport(connection);
        await transport.ExecuteSqlAsync(new CompiledQuery("CREATE TABLE integral_probe(id INTEGER, required_integer INTEGER, required_long INTEGER)", []));
        await transport.ExecuteSqlAsync(new CompiledQuery("INSERT INTO integral_probe VALUES(1,2147483647,9223372036854775807),(2,2147483647,-9223372036854775808),(3,NULL,NULL)", []));
        var executor = Executor(transport);
        var row = Assert.Single((await executor.QueryAsync(Request(new SelectQuery("IntegralProbe")
            .Project("required_long").AndFilter(Expr.Eq("id", new Value.I64Value(1))).Limit(1)))).Rows);
        Assert.Equal(long.MaxValue, row["required_long"].TryI64()); Assert.False(row.ContainsKey("required_integer"));
        var all = (await executor.QueryAsync(Request(new SelectQuery("IntegralProbe").OrderAsc("id").Limit(3)))).Rows;
        Assert.Equal(long.MinValue, all[1]["required_long"].TryI64()); Assert.IsType<Value.NullValue>(all[2]["required_integer"]);
        var total = Assert.Single((await executor.QueryAsync(Request(new SelectQuery("IntegralProbe")
            .Sum("required_integer", "required_integer").Limit(1)))).Rows);
        Assert.Equal(4294967294L, total["required_integer"].TryI64());
        var average = Assert.Single((await executor.QueryAsync(Request(new SelectQuery("IntegralProbe")
            .Avg("required_integer", "required_long").Limit(1)))).Rows);
        Assert.Equal(2147483647d, average["required_long"].TryF64());
        var empty = Assert.Single((await executor.QueryAsync(Request(new SelectQuery("IntegralProbe")
            .Sum("required_integer", "required_integer").AndFilter(Expr.Eq("id", new Value.I64Value(99))).Limit(1)))).Rows);
        Assert.IsType<Value.NullValue>(empty["required_integer"]);
    }

    [Fact]
    public async Task IntegralBindingScaledDecimalDoesNotRoundSignedEndpointInSqlite()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(); var transport = new SqliteTransport(connection); var executor = Executor(transport);
        await transport.ExecuteSqlAsync(new CompiledQuery("CREATE TABLE integral_probe(id INTEGER, required_integer INTEGER, required_long INTEGER)", []));
        var original = new Value.DecimalValue(9223372036854775807.0m);
        var command = new InsertCommand("IntegralProbe").Value("id", new Value.I64Value(1)).Value("required_long", original);
        await executor.MutateAsync(new InsertMutationRequest(command, "preserve exact customer integral repair"));
        Assert.Same(original, command.Values["required_long"]);
        var row = Assert.Single(await transport.FetchAllSqlAsync(new CompiledQuery(
            "SELECT required_long,typeof(required_long) AS storage FROM integral_probe", [])));
        Assert.IsType<Value.I64Value>(row["required_long"]); Assert.Equal(long.MaxValue, row["required_long"].TryI64());
        Assert.Equal("integer", row["storage"].Raw);
    }

    private static QueryRequest Request(SelectQuery query) => new(query,
        new QueryIntent("inspect integral values", "verify declared model ranges before delivery"));
    private static SqlDataServiceExecutor Executor(SqliteTransport transport) => new(new SqliteDialect(), transport, new Schema());
    private sealed class Schema : ISchemaProvider
    {
        public EntityDescriptor? GetEntity(string name) => name == "IntegralProbe" ? EntityDescriptor.New(name).TableName("integral_probe")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("required_integer", DataType.I64).Int32())
            .Property(PropertyDescriptor.New("required_long", DataType.I64)) : null;
    }
}
