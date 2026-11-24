using TeaQL.Core;

namespace Generated;

internal sealed class MerchantChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Merchant", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Merchant", Location = ObjectLocation.Property("name"), SystemValue = 100 });


        return results;
    }
}

internal sealed class FederatedOrderChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("merchant")) || (values.TryGetValue("merchant", out var checkMerchant) && checkMerchant is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FederatedOrder", Location = ObjectLocation.Property("merchant") });

        if ((creating && !values.ContainsKey("order_number")) || (values.TryGetValue("order_number", out var checkOrderNumber) && checkOrderNumber is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FederatedOrder", Location = ObjectLocation.Property("order_number") });
        if (values.TryGetValue("order_number", out var maxLenOrderNumber) && maxLenOrderNumber.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "FederatedOrder", Location = ObjectLocation.Property("order_number"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("fulfillment_status")) || (values.TryGetValue("fulfillment_status", out var checkFulfillmentStatus) && checkFulfillmentStatus is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FederatedOrder", Location = ObjectLocation.Property("fulfillment_status") });
        if (values.TryGetValue("fulfillment_status", out var maxLenFulfillmentStatus) && maxLenFulfillmentStatus.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "FederatedOrder", Location = ObjectLocation.Property("fulfillment_status"), SystemValue = 100 });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Merchant",
       "FederatedOrder"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Merchant"] = new MerchantChecker(),
       ["FederatedOrder"] = new FederatedOrderChecker()
    }, new Dictionary<string, Record>
    {
       ["Merchant"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["FederatedOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["merchant"] = new Value.I64Value(0),
            ["order_number"] = new Value.TextValue(""),
            ["fulfillment_status"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Merchant"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["FederatedOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["merchant"] = true,
           ["order_number"] = true,
           ["fulfillment_status"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Merchant"] = new List<RelationDescriptor> {
            RelationDescriptor.New("FederatedOrderList", "FederatedOrder").LocalKey("id").ForeignKey("merchant").Many()
        },
       ["FederatedOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Merchant", "Merchant").LocalKey("merchant").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Merchant"] = "merchant_data",
       ["FederatedOrder"] = "federated_order_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Merchant"] = new List<string> {
        },
       ["FederatedOrder"] = new List<string> {
        }
    }).WireEntity(WireFields.CreateMetadata("Merchant", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("FederatedOrder", ["id", "merchant", "order_number", "fulfillment_status", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["merchant"] = ["merchant"], ["order_number"] = ["order_number"], ["fulfillment_status"] = ["fulfillment_status"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:5453e05b286fd5877abd67c085d9fdb6310e2fd0dbe9ab3ab7dd69a53ba09cb6\",\"entities\":{\"FederatedOrder\":{\"labels\":{\"en\":\"Federated Order\"},\"fields\":[{\"name\":\"fulfillment_status\",\"labels\":{\"en\":\"fulfillment status\"},\"aliases\":[\"FulfillmentStatus\",\"fulfillmentStatus\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"merchant\",\"targetType\":\"Merchant\",\"labels\":{\"en\":\"Merchant\"},\"aliases\":[\"Merchant\"]},{\"name\":\"order_number\",\"labels\":{\"en\":\"order number\"},\"aliases\":[\"OrderNumber\",\"orderNumber\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"Merchant\":{\"labels\":{\"en\":\"Merchant\"},\"fields\":[{\"name\":\"federated_order_list\",\"targetType\":\"FederatedOrder\",\"labels\":{\"en\":\"Federated Order\"},\"aliases\":[\"FederatedOrderList\",\"federatedOrderList\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            using var bootstrapScope = context.EnterGeneratedBootstrap("Merchant", 1);
        var domainRoot = await Q.Merchants().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Generated.Models.Merchant.TeaqlNewWithFixedId(1);
            created.UpdateName("Harbour Retail");
            try { domainRoot = await created.AuditAs("create generated Domain Root Merchant").SaveAsync(context); }
            catch { domainRoot = await Q.Merchants().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Merchant", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Merchant")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("FederatedOrder")!.AuditMaskFields(new List<string> {  }));
    }
}