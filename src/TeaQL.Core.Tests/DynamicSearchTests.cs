using System.Text.Json;

namespace TeaQL.Core.Tests;

public class DynamicSearchTests
{
    private static readonly IReadOnlyDictionary<string, SearchModel> Models = new Dictionary<string, SearchModel>
    {
        ["School"] = new(new Dictionary<string, string> { ["name"] = "string", ["id"] = "integer",
            ["active"] = "boolean", ["capacity"] = "number", ["established_date"] = "date",
            ["create_time"] = "timestamp", ["amount"] = "decimal" },
            new Dictionary<string, string> { ["platform"] = "Platform" }),
        ["Platform"] = new(new Dictionary<string, string> { ["name"] = "string", ["id"] = "integer" },
            new Dictionary<string, string>())
    };

    [Fact]
    public void UnknownCompleteClausesWarnWithoutValuesAndPreserveValidSiblings()
    {
        var warnings = new List<DynamicSearchWarning>();
        var result = DynamicSearch.Normalize("""
            {"filter":{"name":"School","removed":"secret","platform.removed":"secret",
             "missing.name":"secret","platform.name":"Tenant"},
             "orderBy":[{"field":"gone","direction":"asc"},{"field":"id","direction":"desc"}]}
            """, "School", Models, warnings.Add);
        Assert.Equal(new[] { "name", "platform.name" }, result.Filters.Select(x => x.FieldPath));
        Assert.Equal("id", Assert.Single(result.Orders).FieldPath);
        Assert.Equal(4, warnings.Count);
        Assert.Equal(new[] { "removed", "platform.removed", "missing.name", "gone" }, warnings.Select(x => x.FieldPath));
        Assert.All(warnings, w => Assert.Equal("DYNAMIC_SEARCH_UNKNOWN_FIELD", w.Code));
        var json = JsonSerializer.Serialize(warnings);
        Assert.DoesNotContain("secret", json);
        Assert.Contains("\"fieldPath\"", json);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("{} {}")]
    [InlineData("{\"tenant\":2}")]
    [InlineData("{\"filter\":{\"id\":true}}")]
    [InlineData("{\"filter\":{\"id\":1.2}}")]
    [InlineData("{\"filter\":{\"capacity\":1e999}}")]
    [InlineData("{\"filter\":{\"established_date\":\"2026-02-30\"}}")]
    [InlineData("{\"filter\":{\"name\":{\"$wat\":1}}}")]
    [InlineData("{\"filter\":{\"gone\":{\"$wat\":1}}}")]
    [InlineData("{\"filter\":{\"name\":{\"$in\":1}}}")]
    [InlineData("{\"filter\":{\"constructor\":1}}")]
    [InlineData("{\"filter\":{\"platform..name\":1}}")]
    [InlineData("{\"orderBy\":[{\"field\":\"id\",\"direction\":\"bad\"}]}")]
    [InlineData("{\"filter\":{},\"filter\":{}}")]
    public void InvalidInputIsFatalWithoutWarningEmission(string input)
    {
        var warnings = new List<DynamicSearchWarning>();
        Assert.Throws<ArgumentException>(() => DynamicSearch.Normalize(input, "School", Models, warnings.Add));
        Assert.Empty(warnings);
    }

    [Fact]
    public void ValuesRemainTypedAndExactDecimalStringsAreRetained()
    {
        var result = DynamicSearch.Normalize("""
            {"filter":{"id":1.0,"active":true,"established_date":"2024-02-29",
            "create_time":1700000000000,"amount":"12345678901234567890.123456789","name":null}}
            """, "School", Models);
        Assert.Equal(6, result.Filters.Count);
        Assert.Equal("12345678901234567890.123456789", result.Filters.Single(x => x.FieldPath == "amount").Value.GetString());
        Assert.Equal(JsonValueKind.Null, result.Filters.Single(x => x.FieldPath == "name").Value.ValueKind);
    }

    [Fact]
    public void LimitsAndInvalidTrustedMetadataRemainFatal()
    {
        Assert.Throws<ArgumentException>(() => DynamicSearch.Normalize("{\"filter\":{\"name\":1,\"gone\":1}}", "School", Models, maxClauses: 1));
        string oversized = JsonSerializer.Serialize(new { filter = new { id = new Dictionary<string, object> { ["$in"] = new int[1001] } } });
        Assert.Throws<ArgumentException>(() => DynamicSearch.Normalize(oversized, "School", Models));
        var broken = new Dictionary<string, SearchModel> { ["School"] = Models["School"] };
        Assert.Throws<ArgumentException>(() => DynamicSearch.Normalize("{\"filter\":{\"platform.name\":\"x\"}}", "School", broken));
    }

    [Fact]
    public void MergePreservesScopeOrderLimitAndDoesNotMutateBase()
    {
        var basis = new SelectQuery("School").Filter(Expr.Eq("tenant_id", 7)).Limit(2)
            .Comment("what: search tenant schools").Purpose("why: tenant page");
        basis.HardLimitValue = 3;
        basis.OrderByItems.Add(OrderBy.Desc("id"));
        basis.AndFilter(new FilterExpression { Operator = "eq", Field = "active", Expected = true });
        var warnings = new List<DynamicSearchWarning>();
        var result = DynamicSearch.Merge(basis, """
            {"filter":{"name":"School","gone":"secret"},"orderBy":[{"field":"name","direction":"asc"}]}
            """, Models, f => Expr.Eq(f.FieldPath, f.Value.GetString()), o => OrderBy.Asc(o.FieldPath), warnings.Add);
        Assert.NotSame(basis, result.Query);
        Assert.Equal(3, Assert.IsType<Expr.AndExpr>(result.Query.FilterCondition).Parts.Count);
        Assert.Single(basis.Filters);
        Assert.Empty(result.Query.Filters);
        Assert.IsType<Expr.BinaryExpr>(basis.FilterCondition);
        Assert.Single(basis.OrderByItems);
        Assert.Equal(new[] { "id", "name" }, result.Query.OrderByItems.Select(x => x.Field));
        Assert.Equal(basis.Slice, result.Query.Slice);
        Assert.Equal(3UL, result.Query.HardLimitValue);
        Assert.Equal(basis.PurposeText, result.Query.PurposeText);
        Assert.Single(warnings);
    }

    [Fact]
    public void LateInvalidInputOrBindingFailureDoesNotEmitEarlierWarnings()
    {
        var warnings = new List<DynamicSearchWarning>();
        Assert.Throws<ArgumentException>(() => DynamicSearch.Normalize(
            "{\"filter\":{\"gone\":1,\"id\":\"bad\"}}", "School", Models, warnings.Add));
        Assert.Throws<InvalidOperationException>(() => DynamicSearch.Merge(new SelectQuery("School"),
            "{\"filter\":{\"gone\":1,\"name\":\"x\"}}", Models,
            _ => throw new InvalidOperationException("binding failure"), _ => OrderBy.Asc("id"), warnings.Add));
        Assert.Empty(warnings);
    }
}
