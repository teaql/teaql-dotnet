using Xunit;
using TeaQL.Provider.MySql;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.MySql.Tests
{
    public class MySqlDialectTests
    {
        [Fact]
        public void Kind_ReturnsMySql()
        {
            var dialect = new MySqlDialect();
            Assert.Equal(DatabaseKind.MySql, dialect.Kind);
        }

        [Fact]
        public void QuoteIdent_ReturnsQuoted()
        {
            var dialect = new MySqlDialect();
            Assert.Equal("`select`", dialect.QuoteIdent("select"));
            Assert.Equal("`id`", dialect.QuoteIdent("id"));
            Assert.Equal("`i``d`", dialect.QuoteIdent("i`d"));
        }

        [Fact]
        public void Placeholder_ReturnsQuestionMark()
        {
            var dialect = new MySqlDialect();
            Assert.Equal("?", dialect.Placeholder(1));
            Assert.Equal("?", dialect.Placeholder(2));
        }

        [Theory]
        [InlineData(DataType.Bool, "BOOLEAN")]
        [InlineData(DataType.I64, "BIGINT")]
        [InlineData(DataType.U64, "BIGINT UNSIGNED")]
        [InlineData(DataType.F64, "DOUBLE")]
        [InlineData(DataType.Decimal, "DECIMAL(38, 18)")]
        [InlineData(DataType.Text, "VARCHAR(255)")]
        [InlineData(DataType.LargeText, "LONGTEXT")]
        [InlineData(DataType.Json, "JSON")]
        [InlineData(DataType.Date, "DATE")]
        [InlineData(DataType.Timestamp, "DATETIME(3)")]
        public void SchemaTypeSql_ReturnsExpectedType(DataType dataType, string expectedSql)
        {
            var dialect = new MySqlDialect();
            var property = new PropertyDescriptor("test", dataType, IsId: false);
            Assert.Equal(expectedSql, dialect.SchemaTypeSql(dataType, property));
        }
    }
}
