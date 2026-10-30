using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

public sealed class CheckerMetadataTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task MissingTypeUsesTheDeclaringCheckerWithoutMutatingItsResult(string? declared)
    {
        var original = Violation(declared);
        var provider = new Provider();
        var checker = new CachedChecker(original);
        var context = new UserContext().Install(new RuntimeModule()
            .Checker("PurchaseOrder", checker).Checker("OrderItem", checker)).WithDataService(provider);
        var first = await Reject(context, "PurchaseOrder");
        var second = await Reject(context, "OrderItem");

        Assert.Equal("PurchaseOrder", first.EntityType);
        Assert.Equal("OrderItem", second.EntityType);
        Assert.Equal(declared, original.EntityType);
        Assert.Equal("customer-owned diagnostic", original.Message);
        Assert.NotSame(original, first);
        Assert.NotSame(first, second);
        Assert.Equal(0, provider.Queries);
        Assert.Equal(0, provider.Mutations);
    }

    [Fact]
    public async Task ExplicitChildTypeAliasAndValuesSurviveOwnerBindingAndLocalization()
    {
        var original = Violation("ReviewedOrderItem");
        var context = new UserContext().Install(new RuntimeModule()
            .Checker("PurchaseOrder", new CachedChecker(original))).WithDataService(new Provider());
        var result = await Reject(context, "PurchaseOrder");

        Assert.Equal("ReviewedOrderItem", result.EntityType);
        Assert.Equal("order_item_list[2].product_name", result.ModelPath);
        Assert.Equal("OrderItemList[2].ProductName", result.NativePath);
        Assert.Equal("/orderItems/2/product~1name~0alias", result.SourceInstancePath);
        Assert.Equal("synthetic", result.InputValue);
        Assert.Equal(80, result.SystemValue);
        Assert.Equal("customer-owned diagnostic", original.Message);
    }

    [Fact]
    public async Task LocalizationCannotRewriteAnEarlierFailureOrACustomerCachedResult()
    {
        var original = Violation(null);
        var context = new UserContext().Install(new RuntimeModule()
            .Checker("PurchaseOrder", new CachedChecker(original))).WithDataService(new Provider());
        var english = await Reject(context.SetLocaleCode("en"), "PurchaseOrder");
        var message = english.Message;
        var chinese = await Reject(context.SetLocaleCode("zh-CN"), "PurchaseOrder");

        Assert.Equal(message, english.Message);
        Assert.NotEqual(english.Message, chinese.Message);
        Assert.Equal("customer-owned diagnostic", original.Message);
        Assert.Null(original.EntityType);
        Assert.Contains("OrderItemList[2].ProductName", english.Message);
    }

    [Theory]
    [InlineData(JsonFieldNamingProfile.CamelCase, "/orderItemList/2/productName")]
    [InlineData(JsonFieldNamingProfile.SnakeCase, "/order_item_list/2/product_name")]
    [InlineData(JsonFieldNamingProfile.PascalCase, "/OrderItemList/2/ProductName")]
    public void WireJsonContainsCanonicalTypedSegmentsRatherThanEmptyObjects(
        JsonFieldNamingProfile profile, string expectedPath)
    {
        var wire = Violation("OrderItem").ToWire(profile);
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(wire,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        var root = json.RootElement;
        Assert.Equal(expectedPath, root.GetProperty("instancePath").GetString());
        Assert.Equal("/orderItems/2/product~1name~0alias", root.GetProperty("sourceInstancePath").GetString());
        var location = root.GetProperty("location");
        Assert.Equal(3, location.GetArrayLength());
        Assert.Equal("property", location[0].GetProperty("kind").GetString());
        Assert.Equal("order_item_list", location[0].GetProperty("name").GetString());
        Assert.Equal("index", location[1].GetProperty("kind").GetString());
        Assert.Equal(2, location[1].GetProperty("index").GetInt32());
        Assert.Equal("product_name", location[2].GetProperty("name").GetString());
        Assert.Equal("OrderItem", root.GetProperty("entityType").GetString());
        var roundTrip = JsonSerializer.Deserialize<WireCheckResult>(root.GetRawText(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(roundTrip);
        Assert.Equal(wire.Location, roundTrip.Location);
        using var defaultJson = JsonDocument.Parse(JsonSerializer.Serialize(wire));
        Assert.Equal(expectedPath, defaultJson.RootElement.GetProperty("instancePath").GetString());
        Assert.Equal("property", defaultJson.RootElement.GetProperty("location")[0].GetProperty("kind").GetString());
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"kind\":\"index\",\"index\":-1}")]
    [InlineData("{\"kind\":\"index\",\"index\":1.5}")]
    [InlineData("{\"kind\":\"index\",\"index\":2147483648}")]
    [InlineData("{\"kind\":\"property\",\"name\":\"\"}")]
    [InlineData("{\"kind\":\"property\",\"name\":\"one\",\"name\":\"two\"}")]
    [InlineData("{\"kind\":\"index\",\"name\":\"one\"}")]
    [InlineData("{\"kind\":\"unknown\",\"name\":\"one\"}")]
    public void MalformedWireSegmentsFailClosed(string json) =>
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ObjectLocationSegment>(json));

    [Fact]
    public async Task UnknownCustomerRuleKeepsItsExplanationInsteadOfAnUnresolvedCatalogKey()
    {
        var original = new CheckResult { RuleId = "CUSTOM_REVIEW", Message = "Manual review is required",
            Location = ObjectLocation.Property("product_name") };
        var context = new UserContext().Install(new RuntimeModule().Checker("OrderItem", new CachedChecker(original)))
            .WithDataService(new Provider());
        var result = await Reject(context, "OrderItem");
        Assert.Equal("Manual review is required", result.Message);
        Assert.Equal("OrderItem", result.EntityType);
        Assert.Equal("Manual review is required", original.Message);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("zh-CN")]
    public void OmittedInputNeverClaimsToBeNullOrHaveFourCharacters(string locale)
    {
        var result = new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("contact_phone"), SystemValue = 32 };
        new UserContext().SetLocaleCode(locale).TranslateCheckResults([result]);
        Assert.Contains("[omitted]", result.Message);
        Assert.DoesNotContain("null", result.Message);
        Assert.DoesNotContain(" 4", result.Message);
        Assert.Null(result.InputValue);
    }

    private static CheckResult Violation(string? entityType) => new()
    {
        RuleId = "max_length", EntityType = entityType,
        Location = ObjectLocation.Property("order_item_list").Index(2).PropertyAt("product_name"),
        SourceInstancePath = "/orderItems/2/product~1name~0alias",
        InputValue = "synthetic", SystemValue = 80, Message = "customer-owned diagnostic"
    };

    private static async Task<CheckResult> Reject(UserContext context, string entity)
    {
        var error = await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>()
            .MutateAsync(new InsertMutationRequest(new InsertCommand(entity), "Check synthetic metadata before persistence")));
        return Assert.Single(error.Violations);
    }

    private sealed class CachedChecker(CheckResult result) : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now) => [result];
    }

    private sealed class Provider : IDataService
    {
        public int Queries { get; private set; }
        public int Mutations { get; private set; }
        public DataServiceCapabilities Capabilities { get; } = new() { Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) { Queries++; return Task.FromResult(new QueryResult()); }
        public Task<MutationResult> MutateAsync(MutationRequest request) { Mutations++; return Task.FromResult(new MutationResult()); }
    }
}
