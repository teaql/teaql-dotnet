using System;
using System.Collections.Generic;
using Xunit;
using TeaQL.Sql;
using TeaQL.Core;

namespace TeaQL.Sql.Tests
{
    public class CompiledQueryTests
    {
        [Fact]
        public void SqlWithComment_AppendsComment_Correctly()
        {
            var q = new CompiledQuery("SELECT 1", new List<Value>(), "my */ comment");
            Assert.Equal("/* my * / comment */ SELECT 1", q.SqlWithComment());
            
            var q2 = new CompiledQuery("SELECT 1", new List<Value>());
            Assert.Equal("SELECT 1", q2.SqlWithComment());
        }

        [Fact]
        public void DebugSql_PostgreSql_ReplacesCorrectly()
        {
            var q = new CompiledQuery("SELECT * FROM t WHERE a = $1 AND b = $2 AND c = 'str''$1'", new List<Value> {
                new Value.I64Value(10),
                new Value.TextValue("hello")
            });

            var debug = q.DebugSql(DatabaseKind.PostgreSql);
            Assert.Equal("SELECT * FROM t WHERE a = 10 AND b = 'hello' AND c = 'str''$1'", debug);
        }
        
        [Fact]
        public void DebugSql_Sqlite_ReplacesCorrectly()
        {
            var q = new CompiledQuery("SELECT * FROM t WHERE a = ? AND b = ? AND c = 'str''?'", new List<Value> {
                new Value.I64Value(10),
                new Value.TextValue("hello")
            });

            var debug = q.DebugSql(DatabaseKind.Sqlite);
            Assert.Equal("SELECT * FROM t WHERE a = 10 AND b = 'hello' AND c = 'str''?'", debug);
        }

        [Fact]
        public void DebugSql_MySql_ReplacesCorrectly()
        {
            var q = new CompiledQuery("SELECT * FROM t WHERE a = ? AND b = ? AND c = 'str''?'", new List<Value> {
                new Value.I64Value(10),
                new Value.TextValue("hello")
            });

            var debug = q.DebugSql(DatabaseKind.MySql);
            Assert.Equal("SELECT * FROM t WHERE a = 10 AND b = 'hello' AND c = 'str''?'", debug);
        }

        [Fact]
        public void SqlCompileException_Constructors_Work()
        {
            Assert.NotNull(SqlCompileException.UnknownEntity("e"));
            Assert.NotNull(SqlCompileException.UnknownField("f"));
            Assert.NotNull(SqlCompileException.EmptyInList());
            Assert.NotNull(SqlCompileException.MissingIdProperty("e"));
            Assert.NotNull(SqlCompileException.MissingVersionProperty("e"));
            Assert.NotNull(SqlCompileException.EmptyMutation("k"));
            Assert.NotNull(SqlCompileException.InvalidRecoverVersion(1));
            Assert.NotNull(SqlCompileException.UnsupportedSchemaType(DataType.I64));
            Assert.NotNull(SqlCompileException.InvalidFunctionArguments("msg"));
            Assert.NotNull(SqlCompileException.InvalidSubQueryOperator("op"));
        }
        
        [Fact]
        public void DebugSql_InvalidKind_Throws()
        {
            var q = new CompiledQuery("SELECT 1", new List<Value>());
            Assert.Throws<ArgumentOutOfRangeException>(() => q.DebugSql((DatabaseKind)999));
        }

        [Fact]
        public void DebugSql_FormatsVariousTypes()
        {
            var values = new List<Value>
            {
                new Value.NullValue(),
                new Value.BoolValue(true),
                new Value.U64Value(42),
                new Value.F64Value(3.14),
                new Value.DecimalValue(10.5m),
                new Value.JsonValue(System.Text.Json.Nodes.JsonNode.Parse("{}")),
                new Value.DateValue(new DateTime(2023, 1, 1)),
                new Value.TimestampValue(1000),
                new Value.ObjectValue(new TeaQL.Core.Record { { "a", new Value.I64Value(1) } }),
                new Value.ListValue(new List<Value> { new Value.I64Value(1) }),
                new Value.TypedNullValue(DataType.I64)
            };

            var sql = string.Join(" ", new string('?', values.Count).ToCharArray());
            var q = new CompiledQuery(sql, values);
            var debug = q.DebugSql(DatabaseKind.Sqlite);
            
            Assert.Contains("NULL", debug);
            Assert.Contains("TRUE", debug);
            Assert.Contains("42", debug);
            Assert.Contains("3.14", debug);
            Assert.Contains("10.5", debug);
            Assert.Contains("'{}'", debug);
            Assert.Contains("'2023-01-01'", debug);
            Assert.Contains("1000", debug);
        }
    }
}
