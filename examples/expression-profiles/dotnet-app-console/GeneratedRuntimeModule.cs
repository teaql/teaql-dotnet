using TeaQL.Core;

namespace Generated;

internal sealed class DistributionCenterChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("dock_name")) || (values.TryGetValue("dock_name", out var checkDockName) && checkDockName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "DistributionCenter", Location = ObjectLocation.Property("dock_name") });
        if (values.TryGetValue("dock_name", out var maxLenDockName) && maxLenDockName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "DistributionCenter", Location = ObjectLocation.Property("dock_name"), SystemValue = 100 });


        return results;
    }
}

internal sealed class DispatchOrderChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("distribution_center")) || (values.TryGetValue("distribution_center", out var checkDistributionCenter) && checkDistributionCenter is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "DispatchOrder", Location = ObjectLocation.Property("distribution_center") });

        if ((creating && !values.ContainsKey("order_reference")) || (values.TryGetValue("order_reference", out var checkOrderReference) && checkOrderReference is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "DispatchOrder", Location = ObjectLocation.Property("order_reference") });
        if (values.TryGetValue("order_reference", out var maxLenOrderReference) && maxLenOrderReference.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "DispatchOrder", Location = ObjectLocation.Property("order_reference"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("handling_note")) || (values.TryGetValue("handling_note", out var checkHandlingNote) && checkHandlingNote is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "DispatchOrder", Location = ObjectLocation.Property("handling_note") });
        if (values.TryGetValue("handling_note", out var maxLenHandlingNote) && maxLenHandlingNote.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "DispatchOrder", Location = ObjectLocation.Property("handling_note"), SystemValue = 100 });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "DistributionCenter",
       "DispatchOrder"
    }, new Dictionary<string, IEntityChecker>
    {
       ["DistributionCenter"] = new DistributionCenterChecker(),
       ["DispatchOrder"] = new DispatchOrderChecker()
    }, new Dictionary<string, Record>
    {
       ["DistributionCenter"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["dock_name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["DispatchOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["distribution_center"] = new Value.I64Value(0),
            ["order_reference"] = new Value.TextValue(""),
            ["handling_note"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["DistributionCenter"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["dock_name"] = true,
           ["version"] = true
        },
       ["DispatchOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["distribution_center"] = true,
           ["order_reference"] = true,
           ["handling_note"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["DistributionCenter"] = new List<RelationDescriptor> {
            RelationDescriptor.New("DispatchOrderList", "DispatchOrder").LocalKey("id").ForeignKey("distribution_center").Many()
        },
       ["DispatchOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("DistributionCenter", "DistributionCenter").LocalKey("distribution_center").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["DistributionCenter"] = "distribution_center_data",
       ["DispatchOrder"] = "dispatch_order_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["DistributionCenter"] = new List<string> {
        },
       ["DispatchOrder"] = new List<string> {
        }
    }).WireEntity(WireFields.CreateMetadata("DistributionCenter", ["id", "dock_name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["dock_name"] = ["dock_name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("DispatchOrder", ["id", "distribution_center", "order_reference", "handling_note", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["distribution_center"] = ["distribution_center"], ["order_reference"] = ["order_reference"], ["handling_note"] = ["handling_note"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:77ad1b3df638005c05c3bca45d54d613c2443bb125585299b8cdcd3593870eae\",\"entities\":{\"DispatchOrder\":{\"labels\":{\"en\":\"dispatch order\"},\"fields\":[{\"name\":\"distribution_center\",\"targetType\":\"DistributionCenter\",\"labels\":{\"en\":\"distribution center\"},\"aliases\":[\"DistributionCenter\",\"distributionCenter\"]},{\"name\":\"handling_note\",\"labels\":{\"en\":\"handling note\"},\"aliases\":[\"HandlingNote\",\"handlingNote\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"order_reference\",\"labels\":{\"en\":\"order reference\"},\"aliases\":[\"OrderReference\",\"orderReference\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"DistributionCenter\":{\"labels\":{\"en\":\"distribution center\"},\"fields\":[{\"name\":\"dispatch_order_list\",\"targetType\":\"DispatchOrder\",\"labels\":{\"en\":\"dispatch order\"},\"aliases\":[\"DispatchOrderList\",\"dispatchOrderList\"]},{\"name\":\"dock_name\",\"labels\":{\"en\":\"dock name\"},\"aliases\":[\"DockName\",\"dockName\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            using var bootstrapScope = context.EnterGeneratedBootstrap("DistributionCenter", 1);
        var domainRoot = await Q.DistributionCenters().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Generated.Models.DistributionCenter.TeaqlNewWithFixedId(1);
            created.UpdateDockName("North Receiving Terminal");
            try { domainRoot = await created.AuditAs("create generated Domain Root DistributionCenter").SaveAsync(context); }
            catch { domainRoot = await Q.DistributionCenters().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("DistributionCenter", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("DistributionCenter")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("DispatchOrder")!.AuditMaskFields(new List<string> {  }));
    }
}