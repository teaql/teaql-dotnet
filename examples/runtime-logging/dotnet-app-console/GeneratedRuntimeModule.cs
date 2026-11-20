using TeaQL.Core;

namespace Generated;

internal sealed class WarehouseChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Warehouse", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Warehouse", Location = ObjectLocation.Property("name"), SystemValue = 100 });


        return results;
    }
}

internal sealed class ShippingOrderChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("warehouse")) || (values.TryGetValue("warehouse", out var checkWarehouse) && checkWarehouse is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "ShippingOrder", Location = ObjectLocation.Property("warehouse") });

        if ((creating && !values.ContainsKey("reference")) || (values.TryGetValue("reference", out var checkReference) && checkReference is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "ShippingOrder", Location = ObjectLocation.Property("reference") });
        if (values.TryGetValue("reference", out var maxLenReference) && maxLenReference.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "ShippingOrder", Location = ObjectLocation.Property("reference"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("customer_reference")) || (values.TryGetValue("customer_reference", out var checkCustomerReference) && checkCustomerReference is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "ShippingOrder", Location = ObjectLocation.Property("customer_reference") });
        if (values.TryGetValue("customer_reference", out var maxLenCustomerReference) && maxLenCustomerReference.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "ShippingOrder", Location = ObjectLocation.Property("customer_reference"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("instructions")) || (values.TryGetValue("instructions", out var checkInstructions) && checkInstructions is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "ShippingOrder", Location = ObjectLocation.Property("instructions") });
        if (values.TryGetValue("instructions", out var maxLenInstructions) && maxLenInstructions.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "ShippingOrder", Location = ObjectLocation.Property("instructions"), SystemValue = 100 });


        return results;
    }
}

internal sealed class ShippingItemChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("shipping_order")) || (values.TryGetValue("shipping_order", out var checkShippingOrder) && checkShippingOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "ShippingItem", Location = ObjectLocation.Property("shipping_order") });

        if ((creating && !values.ContainsKey("sku")) || (values.TryGetValue("sku", out var checkSku) && checkSku is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "ShippingItem", Location = ObjectLocation.Property("sku") });
        if (values.TryGetValue("sku", out var maxLenSku) && maxLenSku.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "ShippingItem", Location = ObjectLocation.Property("sku"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("quantity")) || (values.TryGetValue("quantity", out var checkQuantity) && checkQuantity is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "ShippingItem", Location = ObjectLocation.Property("quantity") });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Warehouse",
       "ShippingOrder",
       "ShippingItem"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Warehouse"] = new WarehouseChecker(),
       ["ShippingOrder"] = new ShippingOrderChecker(),
       ["ShippingItem"] = new ShippingItemChecker()
    }, new Dictionary<string, Record>
    {
       ["Warehouse"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["ShippingOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["warehouse"] = new Value.I64Value(0),
            ["reference"] = new Value.TextValue(""),
            ["customer_reference"] = new Value.TextValue(""),
            ["instructions"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["ShippingItem"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["shipping_order"] = new Value.I64Value(0),
            ["sku"] = new Value.TextValue(""),
            ["quantity"] = new Value.I64Value(0),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Warehouse"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["ShippingOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["warehouse"] = true,
           ["reference"] = true,
           ["customer_reference"] = true,
           ["instructions"] = true,
           ["version"] = true
        },
       ["ShippingItem"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["shipping_order"] = true,
           ["sku"] = true,
           ["quantity"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Warehouse"] = new List<RelationDescriptor> {
            RelationDescriptor.New("ShippingOrderList", "ShippingOrder").LocalKey("id").ForeignKey("warehouse").Many()
        },
       ["ShippingOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Warehouse", "Warehouse").LocalKey("warehouse").ForeignKey("id"),
            RelationDescriptor.New("ShippingItemList", "ShippingItem").LocalKey("id").ForeignKey("shipping_order").Many()
        },
       ["ShippingItem"] = new List<RelationDescriptor> {
            RelationDescriptor.New("ShippingOrder", "ShippingOrder").LocalKey("shipping_order").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Warehouse"] = "warehouse_data",
       ["ShippingOrder"] = "shipping_order_data",
       ["ShippingItem"] = "shipping_item_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Warehouse"] = new List<string> {
        },
       ["ShippingOrder"] = new List<string> {
        },
       ["ShippingItem"] = new List<string> {
            "quantity",

        }
    }).WireEntity(WireFields.CreateMetadata("Warehouse", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("ShippingOrder", ["id", "warehouse", "reference", "customer_reference", "instructions", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["warehouse"] = ["warehouse"], ["reference"] = ["reference"], ["customer_reference"] = ["customer_reference"], ["instructions"] = ["instructions"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("ShippingItem", ["id", "shipping_order", "sku", "quantity", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["shipping_order"] = ["shipping_order"], ["sku"] = ["sku"], ["quantity"] = ["quantity"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:0c64ae5629d6c86452affbf87e7b2fdf0e3ac6351a537f083e6e0d86eb41c1cf\",\"entities\":{\"ShippingItem\":{\"labels\":{\"en\":\"Shipping Item\"},\"fields\":[{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"quantity\",\"labels\":{\"en\":\"quantity\"},\"aliases\":[\"Quantity\"]},{\"name\":\"shipping_order\",\"targetType\":\"ShippingOrder\",\"labels\":{\"en\":\"Shipping Order\"},\"aliases\":[\"ShippingOrder\",\"shippingOrder\"]},{\"name\":\"sku\",\"labels\":{\"en\":\"sku\"},\"aliases\":[\"Sku\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"ShippingOrder\":{\"labels\":{\"en\":\"Shipping Order\"},\"fields\":[{\"name\":\"customer_reference\",\"labels\":{\"en\":\"customer reference\"},\"aliases\":[\"CustomerReference\",\"customerReference\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"instructions\",\"labels\":{\"en\":\"instructions\"},\"aliases\":[\"Instructions\"]},{\"name\":\"reference\",\"labels\":{\"en\":\"reference\"},\"aliases\":[\"Reference\"]},{\"name\":\"shipping_item_list\",\"targetType\":\"ShippingItem\",\"labels\":{\"en\":\"Shipping Item\"},\"aliases\":[\"ShippingItemList\",\"shippingItemList\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]},{\"name\":\"warehouse\",\"targetType\":\"Warehouse\",\"labels\":{\"en\":\"Dispatch Warehouse\"},\"aliases\":[\"Warehouse\"]}]},\"Warehouse\":{\"labels\":{\"en\":\"Dispatch Warehouse\"},\"fields\":[{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"shipping_order_list\",\"targetType\":\"ShippingOrder\",\"labels\":{\"en\":\"Shipping Order\"},\"aliases\":[\"ShippingOrderList\",\"shippingOrderList\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

    private static async Task EnsureGeneratedBootstrapAsync(UserContext context)
    {
        Exception? lastError = null;
        for (var attempt = 0; attempt < 5; attempt++)
        {
            try { await EnsureGeneratedBootstrapOnceAsync(context); return; }
            catch (Exception error) { lastError = error; if (attempt < 4) await Task.Delay((attempt + 1) * 10); }
        }
        throw lastError!;
    }

    private static async Task EnsureGeneratedBootstrapOnceAsync(UserContext context)
    {
        {
            using var bootstrapScope = context.EnterGeneratedBootstrap("Warehouse", 1);
        var domainRoot = await Q.Warehouses().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Generated.Models.Warehouse.TeaqlNewWithFixedId(1);
            created.UpdateName("Riverside Dispatch");
            try { domainRoot = await created.AuditAs("create generated Domain Root Warehouse").SaveAsync(context); }
            catch { domainRoot = await Q.Warehouses().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Warehouse", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Warehouse")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("ShippingOrder")!.AuditMaskFields(new List<string> { "customer_reference" }));
        Module.Entity(Module.Metadata.GetEntity("ShippingItem")!.AuditMaskFields(new List<string> {  }));
    }
}