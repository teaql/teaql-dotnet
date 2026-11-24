using TeaQL.Core;

namespace Generated;

internal sealed class PlatformChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Platform", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Platform", Location = ObjectLocation.Property("name"), SystemValue = 100 });


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
        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("platform") });

        if ((creating && !values.ContainsKey("supplier")) || (values.TryGetValue("supplier", out var checkSupplier) && checkSupplier is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("supplier") });
        if (values.TryGetValue("supplier", out var minLenSupplier) && minLenSupplier.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult { RuleId = "min_length", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("supplier"), SystemValue = 1 });
        if (values.TryGetValue("supplier", out var maxLenSupplier) && maxLenSupplier.Raw?.ToString()?.Length > 80) results.Add(new CheckResult { RuleId = "max_length", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("supplier"), SystemValue = 80 });

        if (updating && values.ContainsKey("order_number")) results.Add(new CheckResult { RuleId = "BUSINESS_ID_IMMUTABLE", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("order_number"), Message = "Business ID is immutable after create" });
        if ((creating && !values.ContainsKey("order_number")) || (values.TryGetValue("order_number", out var checkOrderNumber) && checkOrderNumber is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("order_number") });
        if (values.TryGetValue("order_number", out var maxLenOrderNumber) && maxLenOrderNumber.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "PurchaseOrder", Location = ObjectLocation.Property("order_number"), SystemValue = 100 });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Platform",
       "PurchaseOrder"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Platform"] = new PlatformChecker(),
       ["PurchaseOrder"] = new PurchaseOrderChecker()
    }, new Dictionary<string, Record>
    {
       ["Platform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["PurchaseOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["platform"] = new Value.I64Value(0),
            ["supplier"] = new Value.TextValue(""),
            ["order_number"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Platform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["PurchaseOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["platform"] = true,
           ["supplier"] = true,
           ["order_number"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Platform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("PurchaseOrderList", "PurchaseOrder").LocalKey("id").ForeignKey("platform").Many()
        },
       ["PurchaseOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Platform", "Platform").LocalKey("platform").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Platform"] = "platform_data",
       ["PurchaseOrder"] = "purchase_order_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Platform"] = new List<string> {
        },
       ["PurchaseOrder"] = new List<string> {
        }
    }, immutableUpdateFields: new Dictionary<string, IReadOnlyDictionary<string, string>>
    {
       ["Platform"] = new Dictionary<string, string> {
        },
       ["PurchaseOrder"] = new Dictionary<string, string> {
           ["order_number"] = "BUSINESS_ID_IMMUTABLE"
        }
    }).WireEntity(WireFields.CreateMetadata("Platform", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("PurchaseOrder", ["id", "platform", "supplier", "order_number", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["platform"] = ["platform"], ["supplier"] = ["supplier"], ["order_number"] = ["order_number"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:304c22e5b2a924d8fc32148174ef6d249857e8080ea8459c2cb4d8a6fd75ae60\",\"entities\":{\"Platform\":{\"labels\":{\"en\":\"platform\"},\"fields\":[{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"purchase_order_list\",\"targetType\":\"PurchaseOrder\",\"labels\":{\"en\":\"purchase order\"},\"aliases\":[\"PurchaseOrderList\",\"purchaseOrderList\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"PurchaseOrder\":{\"labels\":{\"en\":\"purchase order\"},\"fields\":[{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"order_number\",\"labels\":{\"en\":\"order number\"},\"aliases\":[\"OrderNumber\",\"orderNumber\"]},{\"name\":\"platform\",\"targetType\":\"Platform\",\"labels\":{\"en\":\"platform\"},\"aliases\":[\"Platform\"]},{\"name\":\"supplier\",\"labels\":{\"en\":\"supplier\"},\"aliases\":[\"Supplier\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            using var bootstrapScope = context.EnterGeneratedBootstrap("Platform", 1);
        var domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Generated.Models.Platform.TeaqlNewWithFixedId(1);
            created.UpdateName("Procurement");
            try { domainRoot = await created.AuditAs("create generated Domain Root Platform").SaveAsync(context); }
            catch { domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Platform", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Platform")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("PurchaseOrder")!.AuditMaskFields(new List<string> {  }));
    }
}