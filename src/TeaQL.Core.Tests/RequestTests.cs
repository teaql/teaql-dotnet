using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class RequestTests
{
    [Fact]
    public void FieldOperatorExpr_Equal_CreatesEqExpr()
    {
        var values = new List<Value> { new Value.I64Value(42) };
        var expr = RequestHelpers.FieldOperatorExpr("age", FieldOperator.Equal, values);
        Assert.IsType<Expr.BinaryExpr>(expr);
        var binaryExpr = (Expr.BinaryExpr)expr;
        Assert.Equal(BinaryOp.Eq, binaryExpr.Op);
    }

    [Fact]
    public void DynamicJsonValueToTeaqlValue_ParsesCorrectly()
    {
        var node = JsonNode.Parse("{\"id\": 10}");
        var value = RequestHelpers.DynamicJsonValueToTeaqlValue(node);
        Assert.IsType<Value.I64Value>(value);
        Assert.Equal(10, ((Value.I64Value)value).Value);
    }

    [Fact]
    public void ApplyRuntimeMetadata_ModifiesQueryCorrectly()
    {
        var query = new SelectQuery { Entity = "User" };
        var options = new QueryOptions
        {
            Comment = "test comment",
            RawSql = "SELECT * FROM Users"
        };

        var result = RequestHelpers.ApplyRuntimeMetadata(query, options, new List<QuerySelection>());

        Assert.Equal("test comment", result.CommentText);
        Assert.Equal("SELECT * FROM Users", result.RawSqlText);
    }

    [Fact]
    public void DynamicJsonOperator_ReturnsExpectedOperators()
    {
        Assert.Equal(FieldOperator.IsNull, RequestHelpers.DynamicJsonOperator(JsonNode.Parse("\"__is_null__\"")));
        Assert.Equal(FieldOperator.IsNotNull, RequestHelpers.DynamicJsonOperator(JsonNode.Parse("\"__is_not_null__\"")));
        Assert.Equal(FieldOperator.Contain, RequestHelpers.DynamicJsonOperator(JsonNode.Parse("\"some text\"")));
        Assert.Equal(FieldOperator.Equal, RequestHelpers.DynamicJsonOperator(JsonNode.Parse("42")));
        Assert.Equal(FieldOperator.In, RequestHelpers.DynamicJsonOperator(JsonNode.Parse("[\"a\", \"b\"]")));
        Assert.Equal(FieldOperator.Between, RequestHelpers.DynamicJsonOperator(JsonNode.Parse("[1, 2]")));
    }
}
