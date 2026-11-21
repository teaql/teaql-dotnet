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

internal sealed class SupplierChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("warehouse")) || (values.TryGetValue("warehouse", out var checkWarehouse) && checkWarehouse is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Supplier", Location = ObjectLocation.Property("warehouse") });

        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Supplier", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var minLenName) && minLenName.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult { RuleId = "min_length", EntityType = "Supplier", Location = ObjectLocation.Property("name"), SystemValue = 1 });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 80) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Supplier", Location = ObjectLocation.Property("name"), SystemValue = 80 });

        if ((creating && !values.ContainsKey("minimum_order_quantity")) || (values.TryGetValue("minimum_order_quantity", out var checkMinimumOrderQuantity) && checkMinimumOrderQuantity is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Supplier", Location = ObjectLocation.Property("minimum_order_quantity") });
        if (values.TryGetValue("minimum_order_quantity", out var minMinimumOrderQuantity) && minMinimumOrderQuantity is not Value.NullValue && Convert.ToDecimal(minMinimumOrderQuantity.Raw).CompareTo(10m) == -1) results.Add(new CheckResult { RuleId = "min", EntityType = "Supplier", Location = ObjectLocation.Property("minimum_order_quantity"), SystemValue = 10m });
        if (values.TryGetValue("minimum_order_quantity", out var maxMinimumOrderQuantity) && maxMinimumOrderQuantity is not Value.NullValue && Convert.ToDecimal(maxMinimumOrderQuantity.Raw) > 10000m) results.Add(new CheckResult { RuleId = "max", EntityType = "Supplier", Location = ObjectLocation.Property("minimum_order_quantity"), SystemValue = 10000m });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Warehouse",
       "Supplier"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Warehouse"] = new WarehouseChecker(),
       ["Supplier"] = new SupplierChecker()
    }, new Dictionary<string, Record>
    {
       ["Warehouse"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["Supplier"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["warehouse"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["minimum_order_quantity"] = new Value.I64Value(0),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Warehouse"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["Supplier"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["warehouse"] = true,
           ["name"] = true,
           ["minimum_order_quantity"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Warehouse"] = new List<RelationDescriptor> {
            RelationDescriptor.New("SupplierList", "Supplier").LocalKey("id").ForeignKey("warehouse").Many()
        },
       ["Supplier"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Warehouse", "Warehouse").LocalKey("warehouse").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Warehouse"] = "warehouse_data",
       ["Supplier"] = "supplier_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Warehouse"] = new List<string> {
        },
       ["Supplier"] = new List<string> {
            "minimum_order_quantity",

        }
    }).WireEntity(WireFields.CreateMetadata("Warehouse", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Supplier", ["id", "warehouse", "name", "minimum_order_quantity", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["warehouse"] = ["warehouse"], ["name"] = ["name"], ["minimum_order_quantity"] = ["minimum_order_quantity"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:f4b63b86dd7cad758f18ebc623d0a4976afb15dbe9f61002523f909bb75098b0\",\"entities\":{\"Supplier\":{\"labels\":{\"en\":\"Supplier\",\"zh-CN\":\"供应商\"},\"fields\":[{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"minimum_order_quantity\",\"labels\":{\"en\":\"minimum order quantity\"},\"aliases\":[\"MinimumOrderQuantity\",\"minimumOrderQuantity\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]},{\"name\":\"warehouse\",\"targetType\":\"Warehouse\",\"labels\":{\"en\":\"Warehouse\",\"zh-CN\":\"仓库\"},\"aliases\":[\"Warehouse\"]}]},\"Warehouse\":{\"labels\":{\"en\":\"Warehouse\",\"zh-CN\":\"仓库\"},\"fields\":[{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"supplier_list\",\"targetType\":\"Supplier\",\"labels\":{\"en\":\"Supplier\",\"zh-CN\":\"供应商\"},\"aliases\":[\"SupplierList\",\"supplierList\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            created.UpdateName("Central Warehouse");
            try { domainRoot = await created.AuditAs("create generated Domain Root Warehouse").SaveAsync(context); }
            catch { domainRoot = await Q.Warehouses().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Warehouse", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Warehouse")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("Supplier")!.AuditMaskFields(new List<string> {  }));
    }
}