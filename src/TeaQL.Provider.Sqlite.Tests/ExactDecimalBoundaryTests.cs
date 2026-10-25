using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public sealed class ExactDecimalBoundaryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SqliteTransport _transport;

    public ExactDecimalBoundaryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        _transport = new SqliteTransport(_connection);
    }

    public void Dispose() => _connection.Dispose();

    private sealed class ModuleSchemaProvider(RuntimeModule module) : ISchemaProvider
    {
        public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name);
    }

    private static EntityDescriptor AmountEntity() => EntityDescriptor.New("Amount").TableName("amount_data")
        .Property(PropertyDescriptor.New("id", DataType.I64).Id())
        .Property(PropertyDescriptor.New("amount", DataType.Decimal))
        .Property(PropertyDescriptor.New("label", DataType.Text));

    private SqlDataServiceExecutor Executor(EntityDescriptor entity, out UserContext context)
    {
        var module = new RuntimeModule().Entity(entity);
        var executor = new SqlDataServiceExecutor(new SqliteDialect(), _transport, new ModuleSchemaProvider(module));
        context = module.IntoContext().WithDataService(executor);
        return executor;
    }

    private Task<ulong> Insert(long id, decimal amount, string label = "fixture") =>
        _transport.ExecuteSqlAsync(new CompiledQuery(
            "INSERT INTO amount_data(id,amount,label) VALUES (@p0,@p1,@p2)",
            [new Value.I64Value(id), new Value.DecimalValue(amount), new Value.TextValue(label)]));

    private static QueryRequest Request(SelectQuery query) => new(query.Limit(100),
        new QueryIntent("read exact Decimal fixture", "verify issue 55 boundaries"));

    [Fact]
    public async Task SentinelLookingTextRemainsTextAcrossListStreamAndTransaction()
    {
        const string text = "__teaql_decimal__:3.50";
        var query = new CompiledQuery("SELECT @p0 AS note", [new Value.TextValue(text)]);
        var rows = await _transport.FetchAllSqlAsync(query);
        Assert.Equal(text, Assert.IsType<Value.TextValue>(Assert.Single(rows)["note"]).Value);
        await foreach (var row in _transport.StreamSqlAsync(query))
            Assert.Equal(text, Assert.IsType<Value.TextValue>(row["note"]).Value);
        using var transaction = await _transport.BeginSqlAsync();
        try
        {
            rows = await transaction.FetchAllSqlAsync(query);
            Assert.Equal(text, Assert.IsType<Value.TextValue>(Assert.Single(rows)["note"]).Value);
        }
        finally
        {
            await transaction.RollbackSqlAsync();
        }
    }

    [Theory]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    [InlineData("not-a-decimal")]
    [InlineData("0.00000000000000000000000000001")]
    [InlineData("1.00000000000000000000000000001")]
    [InlineData("79228162514264337593543950335.1")]
    public async Task ModeledDecimalRejectsMalformedOrUnrepresentableStorage(string text)
    {
        Executor(AmountEntity(), out var context);
        await context.EnsureSchemaAsync();
        var error = await Assert.ThrowsAsync<SqliteException>(() => _transport.ExecuteSqlAsync(
            new CompiledQuery("INSERT INTO amount_data(id,amount) VALUES (1,@p0)", [new Value.TextValue(text)])));
        Assert.Contains("CHECK constraint failed", error.Message);
        Assert.DoesNotContain(text, error.Message);
    }

    [Theory]
    [InlineData("0.00000000000000000000000000001")]
    [InlineData("1.00000000000000000000000000001")]
    public async Task CorruptExternalDecimalIsRejectedWithoutRounding(string text)
    {
        await _transport.ExecuteSqlAsync(new CompiledQuery("CREATE TABLE unchecked_data(amount TEAQL_DECIMAL_TEXT)", []));
        await _transport.ExecuteSqlAsync(new CompiledQuery("INSERT INTO unchecked_data VALUES (@p0)", [new Value.TextValue(text)]));
        var error = await Assert.ThrowsAsync<FormatException>(() => _transport.FetchAllSqlAsync(
            new CompiledQuery("SELECT amount FROM unchecked_data", [])));
        Assert.DoesNotContain(text, error.Message);
    }

    [Fact]
    public async Task SumCannotSilentlyRoundAnUnrepresentableFraction()
    {
        var executor = Executor(AmountEntity(), out var context);
        await context.EnsureSchemaAsync();
        await Insert(1, decimal.MaxValue - 1m);
        await Insert(2, 0.1m);
        var error = await Assert.ThrowsAnyAsync<Exception>(() => executor.QueryAsync(
            Request(new SelectQuery("Amount").Sum("amount", "total"))));
        Assert.Contains("precision", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task OverflowingPartialSumCanCancelToAnExactRepresentableFinalSum()
    {
        var executor = Executor(AmountEntity(), out var context);
        await context.EnsureSchemaAsync();
        await Insert(1, decimal.MaxValue);
        await Insert(2, 1m);
        await Insert(3, -1m);
        var rows = (await executor.QueryAsync(Request(new SelectQuery("Amount").Sum("amount", "total")))).Rows;
        Assert.Equal(decimal.MaxValue, Assert.IsType<Value.DecimalValue>(Assert.Single(rows)["total"]).Value);
    }

    [Fact]
    public async Task HavingComparesAggregateValuesNumericallyAndAliasesStayTyped()
    {
        var executor = Executor(AmountEntity(), out var context);
        await context.EnsureSchemaAsync();
        await Insert(1, 2.0m, "small");
        await Insert(2, 10.00m, "large");
        var sum = Expr.SumExpr(Expr.Column("amount"));
        var query = new SelectQuery("Amount").GroupBy("label").Sum("amount", "total")
            .Having(Expr.Binary(sum, BinaryOp.Gt, Expr.Value(new Value.DecimalValue(3m))));
        var row = Assert.Single((await executor.QueryAsync(Request(query))).Rows);
        Assert.Equal("large", Assert.IsType<Value.TextValue>(row["label"]).Value);
        Assert.Equal(10.00m, Assert.IsType<Value.DecimalValue>(row["total"]).Value);
        var projected = Assert.Single((await executor.QueryAsync(Request(
            new SelectQuery("Amount").ProjectExpr("note", sum)))).Rows);
        Assert.Equal(12.00m, Assert.IsType<Value.DecimalValue>(projected["note"]).Value);
    }

    [Theory]
    [InlineData("TEAQL_DECIMAL_TEXT")]
    [InlineData("TEAQL_DECIMAL_TEXT COLLATE TEAQL_DECIMAL")]
    public async Task ExactTypeNameWithoutCollationOrValueCheckRequiresMigration(string declaration)
    {
        await _transport.ExecuteSqlAsync(new CompiledQuery(
            $"CREATE TABLE amount_data(id INTEGER PRIMARY KEY, amount {declaration}, label TEXT)", []));
        Executor(AmountEntity(), out var context);
        var failure = await Assert.ThrowsAsync<SqlExecutorException>(() => context.EnsureSchemaAsync());
        Assert.Contains("migrate", failure.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DecimalSumKeepsScaleAndDoesNotRetypeACountAlias()
    {
        var executor = Executor(AmountEntity(), out var context);
        await context.EnsureSchemaAsync();
        await Insert(1, 3.50m);
        await Insert(2, 0.10m);
        var row = Assert.Single((await executor.QueryAsync(Request(
            new SelectQuery("Amount").Sum("amount", "total")))).Rows);
        Assert.Equal("3.60", Assert.IsType<Value.DecimalValue>(row["total"]).Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        var count = Assert.Single((await executor.QueryAsync(Request(
            new SelectQuery("Amount").Count("amount")))).Rows);
        Assert.Equal(2L, Assert.IsType<Value.I64Value>(count["amount"]).Value);
    }
}
