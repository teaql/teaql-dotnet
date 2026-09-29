using TeaQL.Core;
using Xunit;

namespace TeaQL.Sql.Tests;

public class SqlLogRendererTests
{
    [Theory]
    [InlineData("sqlite", "SELECT \"?\", [@p0], `?`, ?, ? /* outer ? /* inner ? */ ? */", "SELECT \"?\", [@p0], `?`, 'Ada', 5 /* outer ? /* inner ? */ ? */")]
    [InlineData("postgresql", "SELECT $$ $2 $$, $tag$ $1 $tag$, $2, $1, $2", "SELECT $$ $2 $$, $tag$ $1 $tag$, 5, 'Ada', 5")]
    [InlineData("sqlserver", "SELECT [@p1], @p2, @p1, @p2", "SELECT [@p1], 5, 'Ada', 5")]
    [InlineData("sqlite", "SELECT @p1, @p0", "SELECT 5, 'Ada'")]
    public void UsesDialectBindingIndexesAndSkipsQuotedTokens(string backend, string sql, string expected)
    {
        var values = new[] { Value.FromObject("Ada"), Value.FromObject(5L) };
        Assert.Equal(expected, SqlLogRenderer.Render(sql, 2, index => SqlLogRenderer.Literal(values[index], backend), backend));
    }

    [Theory]
    [InlineData("postgresql", "SELECT $0")]
    [InlineData("postgresql", "SELECT $2")]
    [InlineData("postgresql", "SELECT $1suffix")]
    [InlineData("postgresql", "SELECT $tag$ unclosed")]
    [InlineData("postgresql", "SELECT E'escaped\\\'?' , $1")]
    [InlineData("sqlite", "SELECT ?1")]
    [InlineData("sqlite", "SELECT :named")]
    [InlineData("sqlite", "SELECT ? /* unclosed")]
    [InlineData("sqlite", "SELECT 'unclosed ?")]
    [InlineData("mysql", "SELECT ? /*! unsafe */")]
    [InlineData("mysql", "SELECT ? # comment")]
    [InlineData("mysql", "SELECT 'back\\slash', ?")]
    public void UnsupportedOrMalformedSqlNeverLeavesUnexpandedBindings(string backend, string sql)
    {
        Assert.ThrowsAny<ArgumentException>(() => SqlLogRenderer.Render(sql, 1, _ => "'projected'", backend));
    }

    [Fact]
    public void MissingExtraAndNonFiniteValuesFailClosed()
    {
        Assert.Throws<ArgumentException>(() => SqlLogRenderer.Render("SELECT ?, ?", 1, _ => "1", "sqlite"));
        Assert.Throws<ArgumentException>(() => SqlLogRenderer.Render("SELECT ?", 2, _ => "1", "sqlite"));
        Assert.Throws<ArgumentException>(() => SqlLogRenderer.Literal(new Value.F64Value(double.NaN), "sqlite"));
        Assert.Throws<ArgumentException>(() => SqlLogRenderer.Literal(new Value.F64Value(double.PositiveInfinity), "sqlite"));
    }
}
