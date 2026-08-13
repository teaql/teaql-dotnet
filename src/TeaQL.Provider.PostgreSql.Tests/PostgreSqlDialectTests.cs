using Xunit;
using TeaQL.Provider.PostgreSql;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.PostgreSql.Tests
{
    public class PostgreSqlDialectTests
    {
        [Fact]
        public void Kind_ReturnsPostgreSql()
        {
            var dialect = new PostgreSqlDialect();
            Assert.Equal(DatabaseKind.PostgreSql, dialect.Kind);
        }

        [Fact]
        public void QuoteIdent_ReturnsQuoted()
        {
            var dialect = new PostgreSqlDialect();
            Assert.Equal("\"select\"", dialect.QuoteIdent("select"));
            Assert.Equal("id", dialect.QuoteIdent("id"));
        }

        [Fact]
        public void Placeholder_ReturnsIndexed()
        {
            var dialect = new PostgreSqlDialect();
            Assert.Equal("$1", dialect.Placeholder(1));
            Assert.Equal("$2", dialect.Placeholder(2));
        }
    }
}
