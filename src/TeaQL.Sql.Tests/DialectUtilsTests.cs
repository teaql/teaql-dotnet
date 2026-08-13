using System.Collections.Generic;
using Xunit;
using TeaQL.Sql;
using TeaQL.Core;
using Record = TeaQL.Core.Record;

namespace TeaQL.Sql.Tests
{
    public class DialectUtilsTests
    {
        [Fact]
        public void QuoteIdentifierIfNeeded_DoesNotQuote_WhenValid()
        {
            var result = DialectUtils.QuoteIdentifierIfNeeded("valid_identifier", '"');
            Assert.Equal("valid_identifier", result);
        }

        [Fact]
        public void QuoteIdentifierIfNeeded_Quotes_WhenKeyword()
        {
            var result = DialectUtils.QuoteIdentifierIfNeeded("select", '"');
            Assert.Equal("\"select\"", result);
        }
        
        [Fact]
        public void QuoteIdentifierIfNeeded_AlreadyQuoted()
        {
            Assert.Equal("\"select\"", DialectUtils.QuoteIdentifierIfNeeded("\"select\"", '"'));
            Assert.Equal("`select`", DialectUtils.QuoteIdentifierIfNeeded("`select`", '"'));
            Assert.Equal("[select]", DialectUtils.QuoteIdentifierIfNeeded("[select]", '"'));
        }
    }

    public class TestSqlDialect : SqlDialect
    {
        public override DatabaseKind Kind => (DatabaseKind)999;
        public override string QuoteIdent(string ident) => $"\"{ident}\"";
        public override string Placeholder(int index) => $"${index}";
    }

    public class SqlDialectTests
    {
        private readonly TestSqlDialect _dialect = new();

        [Fact]
        public void SchemaTypeSql_ReturnsExpected()
        {
            Assert.Equal("BOOLEAN", _dialect.SchemaTypeSql(DataType.Bool, new PropertyDescriptor("b", DataType.Bool)));
            Assert.Equal("INTEGER", _dialect.SchemaTypeSql(DataType.I64, new PropertyDescriptor("i", DataType.I64)));
            Assert.Equal("VARCHAR(255)", _dialect.SchemaTypeSql(DataType.Text, new PropertyDescriptor("t", DataType.Text)));
        }

        [Fact]
        public void ColumnDefinitionSql_ReturnsExpected()
        {
            var prop = new PropertyDescriptor("id", DataType.I64, IsId: true, Nullable: false);
            var sql = _dialect.ColumnDefinitionSql(prop);
            Assert.Equal("\"id\" INTEGER PRIMARY KEY NOT NULL", sql);
        }

        [Fact]
        public void CompileCreateTable_ReturnsExpected()
        {
            var entity = new EntityDescriptor
            {
                TableNameValue = "users",
                Properties = new List<PropertyDescriptor>
                {
                    new PropertyDescriptor("id", DataType.I64, IsId: true),
                    new PropertyDescriptor("name", DataType.Text, Nullable: false)
                }
            };
            var sql = _dialect.CompileCreateTable(entity);
            Assert.Equal("CREATE TABLE IF NOT EXISTS \"users\" (\"id\" INTEGER PRIMARY KEY NOT NULL, \"name\" VARCHAR(255) NOT NULL)", sql);
        }

        [Fact]
        public void FallbackDefaultValueSql_ReturnsExpected()
        {
            Assert.Equal("FALSE", _dialect.FallbackDefaultValueSql(DataType.Bool));
            Assert.Equal("0", _dialect.FallbackDefaultValueSql(DataType.I64));
            Assert.Equal("''", _dialect.FallbackDefaultValueSql(DataType.Text));
        }

        [Fact]
        public void CompileAddColumn_ReturnsExpected()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            var prop = new PropertyDescriptor("age", DataType.I64, Nullable: false);
            var sql = _dialect.CompileAddColumn(entity, prop);
            Assert.Equal("ALTER TABLE \"users\" ADD COLUMN \"age\" INTEGER NOT NULL DEFAULT 0", sql);
        }

        [Fact]
        public void CompileSelect_ReturnsExpected()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            var query = new SelectQuery { Projection = new List<string> { "id" } };
            entity.Properties.Add(new PropertyDescriptor("id", DataType.I64));
            
            var compiled = _dialect.CompileSelect(entity, query);
            Assert.Equal("SELECT \"id\" FROM \"users\"", compiled.Sql);
        }

        [Fact]
        public void CompileSelect_AppliesNestedLimitPerPartition()
        {
            var entity = EntityDescriptor.New("OrderLine").TableName("orderline")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("order_id", DataType.I64))
                .Property(PropertyDescriptor.New("name", DataType.Text));
            var query = new SelectQuery("OrderLine")
                .Project("id").Project("order_id").Project("name")
                .AndFilter(Expr.InList("order_id", new Value[] { new Value.I64Value(11), new Value.I64Value(12) }))
                .OrderDesc("id").Offset(1).Limit(3).PartitionByField("order_id");

            var compiled = _dialect.CompileSelect(entity, query);

            Assert.Equal("SELECT * FROM (SELECT \"id\", \"order_id\", \"name\", ROW_NUMBER() OVER (PARTITION BY \"order_id\" ORDER BY \"id\" DESC) AS \"__teaql_partition_rank\" FROM \"orderline\" WHERE (\"order_id\" IN ($1, $2))) AS \"__teaql_partitioned\" WHERE \"__teaql_partition_rank\" > 1 AND \"__teaql_partition_rank\" <= 4 ORDER BY \"__teaql_partition_rank\"", compiled.Sql);
            Assert.Equal(2, compiled.Params.Count);
        }

        [Fact]
        public void CompileInsert_ReturnsExpected()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("name", DataType.Text));
            
            var cmd = new InsertCommand { Values = new Record { { "name", new Value.TextValue("John") } } };
            
            var compiled = _dialect.CompileInsert(entity, cmd);
            Assert.Equal("INSERT INTO \"users\" (\"name\") VALUES ($1)", compiled.Sql);
            Assert.Single(compiled.Params);
        }

        [Fact]
        public void CompileUpdate_ReturnsExpected()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("id", DataType.I64, IsId: true));
            entity.Properties.Add(new PropertyDescriptor("name", DataType.Text));
            
            var cmd = new UpdateCommand 
            { 
                Id = new Value.I64Value(1),
                Values = new Record { { "name", new Value.TextValue("John") } } 
            };
            
            var compiled = _dialect.CompileUpdate(entity, cmd);
            Assert.Equal("UPDATE \"users\" SET \"name\" = $1 WHERE \"id\" = $2", compiled.Sql);
            Assert.Equal(2, compiled.Params.Count);
        }

        [Fact]
        public void CompileDelete_ReturnsExpected()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("id", DataType.I64, IsId: true));
            
            var cmd = new DeleteCommand { Id = new Value.I64Value(1), SoftDelete = false };
            
            var compiled = _dialect.CompileDelete(entity, cmd);
            Assert.Equal("DELETE FROM \"users\" WHERE \"id\" = $1", compiled.Sql);
            Assert.Single(compiled.Params);
        }

        [Fact]
        public void CompileDelete_SoftDelete_ReturnsExpected()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("id", DataType.I64, IsId: true));
            entity.Properties.Add(new PropertyDescriptor("version", DataType.I64, IsVersion: true));
            
            var cmd = new DeleteCommand { Id = new Value.I64Value(1), SoftDelete = true };
            
            var compiled = _dialect.CompileDelete(entity, cmd);
            Assert.Equal("UPDATE \"users\" SET \"version\" = $1 WHERE \"id\" = $2", compiled.Sql);
            Assert.Equal(2, compiled.Params.Count);
        }

        [Fact]
        public void CompileBatchInsert_ReturnsExpected()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("name", DataType.Text));
            
            var cmd = new BatchInsertCommand { BatchValues = new List<Record> { new Record { { "name", new Value.TextValue("A") } }, new Record { { "name", new Value.TextValue("B") } } } };
            
            var compiled = _dialect.CompileBatchInsert(entity, cmd);
            Assert.Equal("INSERT INTO \"users\" (\"name\") VALUES ($1), ($2)", compiled.Sql);
            Assert.Equal(2, compiled.Params.Count);
        }

        [Fact]
        public void CompileBatchUpdate_ReturnsExpected()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("id", DataType.I64, IsId: true));
            entity.Properties.Add(new PropertyDescriptor("name", DataType.Text));
            
            var cmd = new BatchUpdateCommand 
            {
                BatchIds = new List<Value> { new Value.I64Value(1), new Value.I64Value(2) },
                UpdateFields = new List<string> { "name" },
                BatchValues = new List<Record> { new Record { { "name", new Value.TextValue("A") } }, new Record { { "name", new Value.TextValue("B") } } },
                BatchExpectedVersions = new List<long?> { null, null }
            };
            
            var compiled = _dialect.CompileBatchUpdate(entity, cmd);
            Assert.Contains("UPDATE \"users\" SET \"name\" = CASE \"id\" WHEN $1 THEN $2 WHEN $3 THEN $4 ELSE \"name\" END WHERE \"id\" IN ($5, $6)", compiled.Sql);
        }
    }
}
