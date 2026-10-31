using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

[Collection("Log privacy environment")]
public sealed class CheckerDeclaringTypePrivacyTests : IDisposable
{
    private const string Secret = "OWNER-MASK-CANARY";
    private const string Flag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
    private const string Ack = "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK";
    private readonly string? _previous = Environment.GetEnvironmentVariable(Flag);
    public CheckerDeclaringTypePrivacyTests() => Environment.SetEnvironmentVariable(Flag, null);
    public void Dispose() => Environment.SetEnvironmentVariable(Flag, _previous);

    [Fact]
    public async Task UnregisteredExplicitTypeUsesTheActualRequestOwnerMask()
    {
        var original = Violation("ReviewedCustomer", "contact_phone");
        var provider = new Provider();
        var context = Context(new RuntimeModule().Entity(Customer()).Checker("Customer", new Checker(original)), provider);
        var result = await Reject(context, "Customer");
        Assert.Equal("ReviewedCustomer", result.EntityType); Assert.Equal("contact_phone", result.ModelPath);
        Safe(result); Assert.Equal(Secret, original.InputValue); Assert.Equal(0, provider.Mutations);
    }

    [Fact]
    public async Task RegisteredPlainTypeCannotOverrideTheActualOwnerMask()
    {
        var original = Violation("PublicProjection", "contact_phone");
        var context = Context(new RuntimeModule().Entity(Customer()).Entity(Customer("PublicProjection", false))
            .Checker("Customer", new Checker(original)));
        var result = await Reject(context, "Customer");
        Assert.Equal("PublicProjection", result.EntityType); Safe(result);
    }

    [Fact]
    public async Task UnknownChildTypeResolvesMaskAlongTheOwnerRelationPath()
    {
        var location = ObjectLocation.Property("order_item_list").Index(1).PropertyAt("product_name");
        var original = new CheckResult { RuleId = "max_length", EntityType = "ReviewedOrderItem", Location = location,
            SourceInstancePath = "/items/1/product~1name~0alias", InputValue = Secret, SystemValue = 4 };
        var module = new RuntimeModule().Entity(EntityDescriptor.New("PurchaseOrder")
            .Property(PropertyDescriptor.New("product_name", DataType.Text))
            .Relation(RelationDescriptor.New("order_item_list", "OrderItem").Many()))
            .Entity(EntityDescriptor.New("OrderItem").Property(PropertyDescriptor.New("product_name", DataType.Text))
                .AuditMaskFields(["product_name"]))
            .Checker("PurchaseOrder", new Checker(original));
        var result = await Reject(Context(module), "PurchaseOrder");
        Assert.Equal("ReviewedOrderItem", result.EntityType); Assert.Same(location, result.Location);
        Assert.Equal("/items/1/product~1name~0alias", result.SourceInstancePath); Safe(result);
    }

    [Fact]
    public async Task AnUnknownDeclaringTypeDoesNotBlanketMaskOrdinaryFields()
    {
        var context = Context(new RuntimeModule().Entity(Customer())
            .Checker("Customer", new Checker(Violation("ReviewedCustomer", "name"))));
        var result = await Reject(context, "Customer");
        Assert.Equal(Secret, result.InputValue); Assert.Equal("ReviewedCustomer", result.EntityType);
        Assert.Equal(4, result.SystemValue);
    }

    [Fact]
    public async Task DeclaredMetadataCanAddProtectionToAnOrdinaryOwnerField()
    {
        var module = new RuntimeModule().Entity(Customer())
            .Entity(EntityDescriptor.New("PrivateProjection").Property(PropertyDescriptor.New("name", DataType.Text)).AuditMaskFields(["name"]))
            .Checker("Customer", new Checker(Violation("PrivateProjection", "name")));
        var result = await Reject(Context(module), "Customer");
        Assert.Equal("PrivateProjection", result.EntityType); Safe(result);
    }

    [Fact]
    public async Task DebugSnapshotRemembersTheTrustedOwnerMaskAfterRevocation()
    {
        Environment.SetEnvironmentVariable(Flag, Ack);
        var context = Context(new RuntimeModule().Entity(Customer())
            .Checker("Customer", new Checker(Violation("ReviewedCustomer", "contact_phone"))));
        var result = await Reject(context, "Customer"); Assert.Equal(Secret, result.InputValue);
        Environment.SetEnvironmentVariable(Flag, null);
        new UserContext().TranslateCheckResults([result]); Safe(result);
    }

    [Fact]
    public async Task ConcurrentRequestsKeepTheirOwnModelPolicyOnOneContext()
    {
        var original = Violation("ReviewProjection", "contact_phone"); var checker = new Checker(original);
        var provider = new Provider();
        var context = Context(new RuntimeModule().Entity(Customer()).Entity(Customer("PublicCustomer", false))
            .Checker("Customer", checker).Checker("PublicCustomer", checker), provider);
        var results = await Task.WhenAll(Enumerable.Range(0, 12).Select(async i =>
            (Private: i % 2 == 0, Value: await Reject(context, i % 2 == 0 ? "Customer" : "PublicCustomer"))));
        foreach (var result in results)
            if (result.Private) Safe(result.Value); else Assert.Equal(Secret, result.Value.InputValue);
        Assert.Equal(Secret, original.InputValue); Assert.Equal("customer explanation", original.Message);
        Assert.Equal(0, provider.Mutations);
    }

    [Fact]
    public async Task DynamicFieldMaskUsesTheActualOwnerWithoutChangingItsLocation()
    {
        var context = Context(new RuntimeModule().Entity(Customer())
            .Checker("Customer", new Checker(Violation("ReviewedCustomer", "#private_note"))));
        var result = await Reject(context, "Customer"); Safe(result);
        Assert.Equal("#private_note", result.ModelPath); Assert.Equal("ReviewedCustomer", result.EntityType);
    }

    [Fact]
    public async Task NestedOrdinaryFieldsDoNotBorrowAShadowingParentScalarMask()
    {
        var location = ObjectLocation.Property("order_item_list").Index(1).PropertyAt("product_name");
        var original = new CheckResult { RuleId = "max_length", EntityType = "PurchaseOrder", Location = location, InputValue = Secret, SystemValue = 4 };
        var module = NestedModule().Checker("PurchaseOrder", new Checker(original));
        var result = await Reject(Context(module), "PurchaseOrder");
        Assert.Equal(Secret, result.InputValue); Assert.Equal(location, result.Location); Assert.Equal("PurchaseOrder", result.EntityType);
    }

    [Fact]
    public void DirectTranslationUsesARealRelationBeforeTheLeafShortcut()
    {
        var value = new CheckResult { RuleId = "max_length", EntityType = "PurchaseOrder",
            Location = ObjectLocation.Property("order_item_list").Index(1).PropertyAt("product_name"), InputValue = Secret, SystemValue = 4 };
        new UserContext().Install(NestedModule()).TranslateCheckResults([value]);
        Assert.Equal(Secret, value.InputValue); Assert.Equal("/orderItemList/1/productName", value.InstancePath);
    }

    [Fact]
    public async Task RootStructuredInputProtectsModeledFieldsInEveryJsonSpelling()
    {
        foreach (var field in new[] { "contact_phone", "contactPhone", "ContactPhone" })
        {
            var input = new Dictionary<string, object?> { [field] = Secret };
            var original = new CheckResult { RuleId = "CUSTOM_REVIEW", EntityType = "ReviewedCustomer",
                Location = ObjectLocation.Root(), InputValue = input, Message = "Review the record" };
            var result = await Reject(Context(new RuntimeModule().Entity(Customer()).Checker("Customer", new Checker(original))), "Customer");
            Assert.Equal("[REDACTED]", result.InputValue); Assert.DoesNotContain(Secret, JsonSerializer.Serialize(result.ToWire()));
            Assert.Equal(Secret, input[field]); Assert.Equal("ReviewedCustomer", result.EntityType);
        }
    }

    [Fact]
    public async Task RootStructuredOrdinaryFieldsStayAvailableWithoutBlanketMasking()
    {
        var original = new CheckResult { RuleId = "CUSTOM_REVIEW", EntityType = "ReviewedCustomer", Location = ObjectLocation.Root(),
            InputValue = new Dictionary<string, object?> { ["name"] = Secret }, Message = "Review the record" };
        var result = await Reject(Context(new RuntimeModule().Entity(Customer()).Checker("Customer", new Checker(original))), "Customer");
        var value = Assert.IsAssignableFrom<IDictionary<string, object?>>(result.InputValue); Assert.Equal(Secret, value["name"]);
    }

    [Fact]
    public async Task JsonScalarContentsDoNotBorrowUnrelatedOuterModelMasks()
    {
        var original = new CheckResult { RuleId = "CUSTOM_REVIEW", EntityType = "Customer", Location = ObjectLocation.Property("payload"),
            InputValue = new Dictionary<string, object?> { ["contact_phone"] = Secret }, Message = "Review the JSON value" };
        var module = new RuntimeModule().Entity(Customer().Property(PropertyDescriptor.New("payload", DataType.Json)))
            .Checker("Customer", new Checker(original));
        var result = await Reject(Context(module), "Customer");
        var value = Assert.IsAssignableFrom<IDictionary<string, object?>>(result.InputValue); Assert.Equal(Secret, value["contact_phone"]);
    }

    private static RuntimeModule NestedModule() => new RuntimeModule()
        .Entity(EntityDescriptor.New("PurchaseOrder").Property(PropertyDescriptor.New("product_name", DataType.Text))
            .AuditMaskFields(["product_name"]).Relation(RelationDescriptor.New("order_item_list", "OrderItem").Many()))
        .Entity(EntityDescriptor.New("OrderItem").Property(PropertyDescriptor.New("product_name", DataType.Text)).AuditMaskFields([]));

    private static EntityDescriptor Customer(string name = "Customer", bool masked = true) => EntityDescriptor.New(name)
        .Property(PropertyDescriptor.New("contact_phone", DataType.Text))
        .Property(PropertyDescriptor.New("name", DataType.Text))
        .AuditMaskFields(masked ? ["contact_phone", "#private_note"] : []);
    private static CheckResult Violation(string type, string field) => new() {
        RuleId = "max_length", EntityType = type, Location = ObjectLocation.Property(field), InputValue = Secret,
        SystemValue = 4, Message = "customer explanation"
    };
    private static UserContext Context(RuntimeModule module, Provider? provider = null) =>
        new UserContext().Install(module).WithDataService(provider ?? new Provider());
    private static async Task<CheckResult> Reject(UserContext context, string type)
    {
        // Deliberately no request values: protection must come from model policy,
        // not coincidental matching against a submitted sensitive string.
        var error = await Assert.ThrowsAsync<CheckException>(() => context.RequireResource<IDataService>()
            .MutateAsync(new InsertMutationRequest(new InsertCommand(type), "Verify declaring-type privacy")));
        return Assert.Single(error.Violations);
    }
    private static void Safe(CheckResult result)
    {
        Assert.Equal(LogPrivacy.MaskAuditValue(Secret), result.InputValue); Assert.Equal(4, result.SystemValue);
        Assert.DoesNotContain(Secret, JsonSerializer.Serialize(result));
        Assert.DoesNotContain(Secret, new CheckException([result]).ToString());
        foreach (var profile in Enum.GetValues<JsonFieldNamingProfile>())
        {
            var wire = result.ToWire(profile); Assert.DoesNotContain(Secret, JsonSerializer.Serialize(wire));
            Assert.Equal(result.Location.InstancePathWith(profile), wire.InstancePath);
        }
    }
    private sealed class Checker(CheckResult original) : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now) => [original];
    }
    private sealed class Provider : IDataService
    {
        public int Mutations;
        public DataServiceCapabilities Capabilities { get; } = new() { Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new InvalidOperationException("Checker must not query");
        public Task<MutationResult> MutateAsync(MutationRequest request) { Interlocked.Increment(ref Mutations); return Task.FromResult(new MutationResult()); }
    }
}
