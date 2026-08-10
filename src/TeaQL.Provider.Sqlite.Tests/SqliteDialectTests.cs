using System;
using Xunit;
using TeaQL.Provider.Sqlite;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.Sqlite.Tests
{
    public class SqliteDialectTests
    {
        [Fact]
        public void Kind_ReturnsSqlite()
        {
            var dialect = new SqliteDialect();
            Assert.Equal(DatabaseKind.Sqlite, dialect.Kind);
        }

        [Fact]
        public void Placeholder_ReturnsIndexedParameter()
        {
            var dialect = new SqliteDialect();
            Assert.Equal("@p0", dialect.Placeholder(1));
            Assert.Equal("@p1", dialect.Placeholder(2));
        }

        [Theory]
        [InlineData(DataType.Bool, "INTEGER")]
        [InlineData(DataType.I64, "INTEGER")]
        [InlineData(DataType.U64, "INTEGER")]
        [InlineData(DataType.F64, "REAL")]
        [InlineData(DataType.Decimal, "NUMERIC")]
        [InlineData(DataType.Text, "VARCHAR(255)")]
        [InlineData(DataType.LargeText, "TEXT")]
        [InlineData(DataType.Json, "JSON")]
        [InlineData(DataType.Date, "DATE")]
        [InlineData(DataType.Timestamp, "TIMESTAMP")]
        public void SchemaTypeSql_ReturnsExpectedType(DataType dataType, string expectedSql)
        {
            var dialect = new SqliteDialect();
            var property = new PropertyDescriptor("test", dataType, IsId: false);
            Assert.Equal(expectedSql, dialect.SchemaTypeSql(dataType, property));
        }

        [Fact]
        public void SchemaTypeSql_IsId_ReturnsInteger()
        {
            var dialect = new SqliteDialect();
            var property = new PropertyDescriptor("id", DataType.I64, IsId: true);
            Assert.Equal("INTEGER", dialect.SchemaTypeSql(DataType.I64, property));
        }

        [Fact]
        public void SchemaTypeSql_Throws_ForUnknown()
        {
            var dialect = new SqliteDialect();
            var property = new PropertyDescriptor("unknown", (DataType)999);
            Assert.Throws<SqlCompileException>(() => dialect.SchemaTypeSql((DataType)999, property));
        }

        [Fact]
        public void CompileAddColumn_StripsNotNull()
        {
            var dialect = new SqliteDialect();
            var entity = new EntityDescriptor { TableNameValue = "TestTable" };
            var property = new PropertyDescriptor("test_col", DataType.Text, Nullable: false);
            
            var sql = dialect.CompileAddColumn(entity, property);
            Assert.Contains("ALTER TABLE TestTable ADD COLUMN test_col VARCHAR(255)", sql);
            Assert.DoesNotContain("NOT NULL", sql);
        }

        [Fact]
        public void QuoteIdent_QuotesKeywords()
        {
            var dialect = new SqliteDialect();
            Assert.Equal("\"select\"", dialect.QuoteIdent("select"));
            Assert.Equal("valid_col", dialect.QuoteIdent("valid_col"));
        }
    }
}
