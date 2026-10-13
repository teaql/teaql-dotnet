using System.Text.Json;

namespace TeaQL.Core.Tests;

public class GeneratedScalarSearchTests
{
    private static readonly IReadOnlyDictionary<string, GeneratedSearchModel> Models = new Dictionary<string, GeneratedSearchModel>
    {
        ["Invoice"] = new(new SearchModel(new Dictionary<string, string>
        {
            ["reference_code"] = "string", ["referenceCode"] = "string", ["amount"] = "decimal",
            ["active"] = "boolean", ["customer"] = "integer", ["date"] = "date",
            ["created"] = "timestamp", ["ratio"] = "number", ["payload"] = "unsupported"
        }, new Dictionary<string, string> { ["customer"] = "Customer" }), new Dictionary<string, string>
        {
            ["reference_code"] = "reference_code", ["referenceCode"] = "reference_code", ["amount"] = "amount",
            ["active"] = "active", ["customer"] = "customer_id", ["date"] = "date",
            ["created"] = "created", ["ratio"] = "ratio", ["payload"] = "payload"
        }),
        ["Customer"] = new(new SearchModel(new Dictionary<string, string> { ["name"] = "string" },
            new Dictionary<string, string>()), new Dictionary<string, string> { ["name"] = "name" })
    };

    [Fact]
    public void PreservesNativeScopeAndBindsAllSiblingsAndAliases()
    {
        var basis = new SelectQuery("Invoice").Filter(Expr.Eq("tenant", 7L)).Limit(10);
        basis.OrderByItems.Add(OrderBy.Asc("id"));
        basis.AndFilter(new FilterExpression { Operator = "eq", Field = "active", Expected = true });
        var before = JsonSerializer.Serialize(basis);
        var warnings = new List<DynamicSearchWarning>();
        var result = DynamicSearch.MergeGeneratedScalar(basis, """
            {"filter":{"referenceCode":{"$contains":"KEEP"},"amount":{"$gte":"100.50"},"active":true,"removed":"SECRET"},
             "orderBy":[{"field":"reference_code","direction":"desc"}]}
            """, Models, warnings.Add);
        var parts = Assert.IsType<Expr.AndExpr>(result.Query.FilterCondition).Parts;
        Assert.Contains(Expr.Eq("tenant", 7L), parts);
        Assert.Contains(Expr.Gte("amount", new Value.DecimalValue(100.50m)), parts);
        Assert.Contains(Expr.Eq("active", true), parts);
        var contains = Assert.Single(parts.OfType<Expr.BinaryExpr>().Where(expr => expr.Op == BinaryOp.Like));
        Assert.Equal(Expr.Column("reference_code"), contains.Left);
        Assert.Equal(new Value.TextValue("%KEEP%"), Assert.IsType<Expr.ValueExpr>(contains.Right).NodeValue);
        Assert.Equal(new[] { OrderBy.Asc("id"), OrderBy.Desc("reference_code") }, result.Query.OrderByItems);
        Assert.Equal(basis.Slice, result.Query.Slice);
        Assert.Empty(result.Query.Filters);
        Assert.Equal(before, JsonSerializer.Serialize(basis));
        Assert.Equal(warnings, result.Warnings);
        Assert.Equal("removed", Assert.Single(warnings).FieldPath);
        Assert.DoesNotContain("SECRET", JsonSerializer.Serialize(warnings));
    }

    [Theory]
    [InlineData("{\"filter\":{\"reference_code\":\"KEEP\",\"customer.name\":\"Ada\",\"removed\":\"secret\"}}")]
    [InlineData("{\"orderBy\":[{\"field\":\"customer.name\",\"direction\":\"asc\"}]}")]
    [InlineData("{\"filter\":{\"active\":\"true\"}}")]
    [InlineData("{\"filter\":{\"amount\":\"NaN\"}}")]
    [InlineData("{\"filter\":{\"amount\":\"100000000000000000000000000000000000\"}}")]
    [InlineData("{\"filter\":{\"payload\":null}}")]
    [InlineData("{\"orderBy\":[{\"field\":\"payload\",\"direction\":\"asc\"}]}")]
    [InlineData("{\"filter\":{\"reference_code\":{\"$contains\":null}}}")]
    [InlineData("{\"filter\":{\"created\":{\"$gt\":null}}}")]
    [InlineData("{\"tenant\":99}")]
    public void InvalidInputNeverMutatesOriginalOrEmitsWarnings(string source)
    {
        var basis = new SelectQuery("Invoice").Filter(Expr.Eq("tenant", 7L));
        var before = JsonSerializer.Serialize(basis);
        var warnings = new List<DynamicSearchWarning>();
        Assert.Throws<ArgumentException>(() => DynamicSearch.MergeGeneratedScalar(basis, source, Models, warnings.Add));
        Assert.Equal(before, JsonSerializer.Serialize(basis));
        Assert.Empty(warnings);
    }

    [Fact]
    public void UsesNativeNumericDateReferenceAndNullTypes()
    {
        var result = DynamicSearch.MergeGeneratedScalar(new SelectQuery("Invoice"), """
            {"filter":{"date":"2026-10-11","created":1791676800000,"ratio":{"$lt":1.25},"customer":{"$in":[1,2]},
             "referenceCode":null,"reference_code":{"$ne":null},"amount":{"$lte":1e2}}}
            """, Models);
        var parts = Assert.IsType<Expr.AndExpr>(result.Query.FilterCondition).Parts;
        Assert.Contains(Expr.Eq("date", new Value.DateValue(new DateTime(2026, 10, 11, 0, 0, 0, DateTimeKind.Utc))), parts);
        Assert.Contains(Expr.Eq("created", new Value.TimestampValue(1791676800000)), parts);
        Assert.Contains(Expr.Lt("ratio", new Value.F64Value(1.25)), parts);
        Assert.Contains(Expr.IsNull("reference_code"), parts);
        Assert.Contains(Expr.IsNotNull("reference_code"), parts);
        Assert.Contains(Expr.Lte("amount", new Value.DecimalValue(100m)), parts);
        var membership = Assert.Single(parts.OfType<Expr.BinaryExpr>().Where(expr => expr.Op == BinaryOp.In));
        Assert.Equal(Expr.Column("customer_id"), membership.Left);
        Assert.Equal(new Value[] { new Value.I64Value(1), new Value.I64Value(2) },
            Assert.IsType<Value.ListValue>(Assert.IsType<Expr.ValueExpr>(membership.Right).NodeValue).Values);
    }
}
