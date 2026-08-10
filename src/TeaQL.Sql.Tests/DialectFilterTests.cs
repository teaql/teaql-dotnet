using System;
using System.Collections.Generic;
using Xunit;
using TeaQL.Sql;
using TeaQL.Core;

namespace TeaQL.Sql.Tests
{
    public class DialectFilterTests
    {
        private readonly TestSqlDialect _dialect = new();

        [Fact]
        public void FilterExpressionSql_BinaryOperators_RenderCorrectly()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("age", DataType.I64));
            entity.Properties.Add(new PropertyDescriptor("name", DataType.Text));

            var paramsList = new List<Value>();

            // Eq
            var eq = Expr.Eq("age", new Value.I64Value(10));
            Assert.Equal("(\"age\" = $1)", _dialect.CompileExpr(entity, eq, paramsList));

            // Ne
            var ne = Expr.Ne("age", new Value.I64Value(20));
            Assert.Equal("(\"age\" != $2)", _dialect.CompileExpr(entity, ne, paramsList));

            // Gt
            var gt = Expr.Gt("age", new Value.I64Value(30));
            Assert.Equal("(\"age\" > $3)", _dialect.CompileExpr(entity, gt, paramsList));

            // Gte
            var gte = Expr.Gte("age", new Value.I64Value(40));
            Assert.Equal("(\"age\" >= $4)", _dialect.CompileExpr(entity, gte, paramsList));

            // Lt
            var lt = Expr.Lt("age", new Value.I64Value(50));
            Assert.Equal("(\"age\" < $5)", _dialect.CompileExpr(entity, lt, paramsList));

            // Lte
            var lte = Expr.Lte("age", new Value.I64Value(60));
            Assert.Equal("(\"age\" <= $6)", _dialect.CompileExpr(entity, lte, paramsList));

            // Like
            var like = Expr.Like("name", "%John%");
            Assert.Equal("(\"name\" LIKE $7)", _dialect.CompileExpr(entity, like, paramsList));

            // NotLike
            var notLike = Expr.NotLike("name", "Jane%");
            Assert.Equal("(\"name\" NOT LIKE $8)", _dialect.CompileExpr(entity, notLike, paramsList));
            
            // In
            var @in = Expr.InList("age", new[] { new Value.I64Value(1), new Value.I64Value(2) });
            Assert.Equal("(\"age\" IN ($9, $10))", _dialect.CompileExpr(entity, @in, paramsList));

            // NotIn
            var notIn = Expr.NotInList("age", new[] { new Value.I64Value(3) });
            Assert.Equal("(\"age\" NOT IN ($11))", _dialect.CompileExpr(entity, notIn, paramsList));
        }

        [Fact]
        public void FilterExpressionSql_LogicalOperators_RenderCorrectly()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("age", DataType.I64));
            var paramsList = new List<Value>();

            var and = Expr.And(new[] { Expr.Eq("age", new Value.I64Value(1)), Expr.Eq("age", new Value.I64Value(2)) });
            Assert.Equal("((\"age\" = $1) AND (\"age\" = $2))", _dialect.CompileExpr(entity, and, paramsList));

            var or = Expr.Or(new[] { Expr.Gt("age", new Value.I64Value(10)), Expr.Lt("age", new Value.I64Value(5)) });
            Assert.Equal("((\"age\" > $3) OR (\"age\" < $4))", _dialect.CompileExpr(entity, or, paramsList));

            var not = Expr.Negate(Expr.Eq("age", new Value.I64Value(100)));
            Assert.Equal("(NOT (\"age\" = $5))", _dialect.CompileExpr(entity, not, paramsList));
        }

        [Fact]
        public void FilterExpressionSql_NullChecks_RenderCorrectly()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("name", DataType.Text));
            var paramsList = new List<Value>();

            Assert.Equal("(\"name\" IS NULL)", _dialect.CompileExpr(entity, Expr.IsNull("name"), paramsList));
            Assert.Equal("(\"name\" IS NOT NULL)", _dialect.CompileExpr(entity, Expr.IsNotNull("name"), paramsList));
        }

        [Fact]
        public void FilterExpressionSql_Between_RenderCorrectly()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("age", DataType.I64));
            var paramsList = new List<Value>();

            var between = Expr.Between("age", new Value.I64Value(10), new Value.I64Value(20));
            Assert.Equal("(\"age\" BETWEEN $1 AND $2)", _dialect.CompileExpr(entity, between, paramsList));
        }

        [Fact]
        public void FilterExpressionSql_Subquery_RenderCorrectly()
        {
            var userEntity = new EntityDescriptor { TableNameValue = "users" };
            userEntity.Properties.Add(new PropertyDescriptor("id", DataType.I64));

            var roleEntity = new EntityDescriptor { TableNameValue = "roles" };
            roleEntity.Properties.Add(new PropertyDescriptor("user_id", DataType.I64));
            roleEntity.Properties.Add(new PropertyDescriptor("role", DataType.Text));

            var query = new SelectQuery { Entity = "roles", FilterCondition = Expr.Eq("role", new Value.TextValue("admin")) };
            var subquery = Expr.InSubquery("id", roleEntity, query, "user_id");

            var paramsList = new List<Value>();
            Assert.Equal("(\"id\" IN (SELECT \"user_id\" FROM \"roles\" WHERE (\"role\" = $1)))", _dialect.CompileExpr(userEntity, subquery, paramsList));
        }

        [Fact]
        public void FilterExpressionSql_Functions_RenderCorrectly()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("age", DataType.I64));
            var paramsList = new List<Value>();

            var func = Expr.CompareColumns("age", BinaryOp.Eq, "age");
            Assert.Equal("(\"age\" = \"age\")", _dialect.CompileExpr(entity, func, paramsList));
        }
    }

    public class DialectOrderByTests
    {
        private readonly TestSqlDialect _dialect = new();

        [Fact]
        public void OrderBySql_RendersCorrectly()
        {
            var entity = new EntityDescriptor { TableNameValue = "users" };
            entity.Properties.Add(new PropertyDescriptor("name", DataType.Text));
            entity.Properties.Add(new PropertyDescriptor("age", DataType.I64));

            var orderBy1 = OrderBy.Asc("name");
            var sql1 = _dialect.OrderBySql(entity, orderBy1, new List<Value>());
            Assert.Equal("\"name\" ASC", sql1);

            var orderBy2 = OrderBy.Desc("age");
            var sql2 = _dialect.OrderBySql(entity, orderBy2, new List<Value>());
            Assert.Equal("\"age\" DESC", sql2);
        }
    }
}
