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

internal sealed class PurchaseOrderChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("warehouse")) || (values.TryGetValue("warehouse", out var checkWarehouse) && checkWarehouse is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("warehouse") });

        if ((creating && !values.ContainsKey("reference")) || (values.TryGetValue("reference", out var checkReference) && checkReference is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("reference") });
        if (values.TryGetValue("reference", out var maxLenReference) && maxLenReference.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("reference"), SystemValue = 100 });


        return results;
    }
}

internal sealed class OrderItemChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("purchase_order")) || (values.TryGetValue("purchase_order", out var checkPurchaseOrder) && checkPurchaseOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "OrderItem", Location = ObjectLocation.Property("purchase_order") });

        if ((creating && !values.ContainsKey("sku")) || (values.TryGetValue("sku", out var checkSku) && checkSku is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "OrderItem", Location = ObjectLocation.Property("sku") });
        if (values.TryGetValue("sku", out var maxLenSku) && maxLenSku.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "OrderItem", Location = ObjectLocation.Property("sku"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("quantity")) || (values.TryGetValue("quantity", out var checkQuantity) && checkQuantity is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "OrderItem", Location = ObjectLocation.Property("quantity") });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Warehouse",
       "PurchaseOrder",
       "OrderItem"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Warehouse"] = new WarehouseChecker(),
       ["PurchaseOrder"] = new PurchaseOrderChecker(),
       ["OrderItem"] = new OrderItemChecker()
    }, new Dictionary<string, Record>
    {
       ["Warehouse"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["PurchaseOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["warehouse"] = new Value.I64Value(0),
            ["reference"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["OrderItem"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["purchase_order"] = new Value.I64Value(0),
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
       ["PurchaseOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["warehouse"] = true,
           ["reference"] = true,
           ["version"] = true
        },
       ["OrderItem"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["purchase_order"] = true,
           ["sku"] = true,
           ["quantity"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Warehouse"] = new List<RelationDescriptor> {
            RelationDescriptor.New("PurchaseOrderList", "PurchaseOrder").LocalKey("id").ForeignKey("warehouse").Many()
        },
       ["PurchaseOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Warehouse", "Warehouse").LocalKey("warehouse").ForeignKey("id"),
            RelationDescriptor.New("OrderItemList", "OrderItem").LocalKey("id").ForeignKey("purchase_order").Many()
        },
       ["OrderItem"] = new List<RelationDescriptor> {
            RelationDescriptor.New("PurchaseOrder", "PurchaseOrder").LocalKey("purchase_order").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Warehouse"] = "warehouse_data",
       ["PurchaseOrder"] = "purchase_order_data",
       ["OrderItem"] = "order_item_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Warehouse"] = new List<string> {
        },
       ["PurchaseOrder"] = new List<string> {
        },
       ["OrderItem"] = new List<string> {
            "quantity",

        }
    }).WireEntity(WireFields.CreateMetadata("Warehouse", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("PurchaseOrder", ["id", "warehouse", "reference", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["warehouse"] = ["warehouse"], ["reference"] = ["reference"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("OrderItem", ["id", "purchase_order", "sku", "quantity", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["purchase_order"] = ["purchase_order"], ["sku"] = ["sku"], ["quantity"] = ["quantity"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:46dda51da34ae53bdd51c3f7b44a0ce6ab6e8f26ca7c04a16160397cee58c3d4\",\"entities\":{\"OrderItem\":{\"labels\":{\"en\":\"Order Item\"},\"fields\":[{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"purchase_order\",\"targetType\":\"PurchaseOrder\",\"labels\":{\"en\":\"Purchase Order\"},\"aliases\":[\"PurchaseOrder\",\"purchaseOrder\"]},{\"name\":\"quantity\",\"labels\":{\"en\":\"quantity\"},\"aliases\":[\"Quantity\"]},{\"name\":\"sku\",\"labels\":{\"en\":\"sku\"},\"aliases\":[\"Sku\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"PurchaseOrder\":{\"labels\":{\"en\":\"Purchase Order\"},\"fields\":[{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"order_item_list\",\"targetType\":\"OrderItem\",\"labels\":{\"en\":\"Order Item\"},\"aliases\":[\"OrderItemList\",\"orderItemList\"]},{\"name\":\"reference\",\"labels\":{\"en\":\"reference\"},\"aliases\":[\"Reference\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]},{\"name\":\"warehouse\",\"targetType\":\"Warehouse\",\"labels\":{\"en\":\"Warehouse\"},\"aliases\":[\"Warehouse\"]}]},\"Warehouse\":{\"labels\":{\"en\":\"Warehouse\"},\"fields\":[{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"purchase_order_list\",\"targetType\":\"PurchaseOrder\",\"labels\":{\"en\":\"Purchase Order\"},\"aliases\":[\"PurchaseOrderList\",\"purchaseOrderList\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            created.UpdateName("Riverside Distribution Center");
            try { domainRoot = await created.AuditAs("create generated Domain Root Warehouse").SaveAsync(context); }
            catch { domainRoot = await Q.Warehouses().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Warehouse", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Warehouse")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("PurchaseOrder")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("OrderItem")!.AuditMaskFields(new List<string> {  }));
    }
}