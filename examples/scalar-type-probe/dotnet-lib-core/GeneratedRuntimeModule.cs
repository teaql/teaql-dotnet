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

internal sealed class FreightQuoteChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FreightQuote", Location = ObjectLocation.Property("platform") });

        if ((creating && !values.ContainsKey("reference")) || (values.TryGetValue("reference", out var checkReference) && checkReference is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FreightQuote", Location = ObjectLocation.Property("reference") });
        if (values.TryGetValue("reference", out var maxLenReference) && maxLenReference.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "FreightQuote", Location = ObjectLocation.Property("reference"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("amount")) || (values.TryGetValue("amount", out var checkAmount) && checkAmount is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FreightQuote", Location = ObjectLocation.Property("amount") });

        if ((creating && !values.ContainsKey("shipment_number")) || (values.TryGetValue("shipment_number", out var checkShipmentNumber) && checkShipmentNumber is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FreightQuote", Location = ObjectLocation.Property("shipment_number") });

        if ((creating && !values.ContainsKey("package_count")) || (values.TryGetValue("package_count", out var checkPackageCount) && checkPackageCount is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FreightQuote", Location = ObjectLocation.Property("package_count") });

        if ((creating && !values.ContainsKey("delivery_date")) || (values.TryGetValue("delivery_date", out var checkDeliveryDate) && checkDeliveryDate is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FreightQuote", Location = ObjectLocation.Property("delivery_date") });

        if ((creating && !values.ContainsKey("pickup_time")) || (values.TryGetValue("pickup_time", out var checkPickupTime) && checkPickupTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FreightQuote", Location = ObjectLocation.Property("pickup_time") });

        if ((creating && !values.ContainsKey("quoted_at")) || (values.TryGetValue("quoted_at", out var checkQuotedAt) && checkQuotedAt is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "FreightQuote", Location = ObjectLocation.Property("quoted_at") });

        if (values.TryGetValue("instructions", out var maxLenInstructions) && maxLenInstructions.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "FreightQuote", Location = ObjectLocation.Property("instructions"), SystemValue = 100 });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Platform",
       "FreightQuote"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Platform"] = new PlatformChecker(),
       ["FreightQuote"] = new FreightQuoteChecker()
    }, new Dictionary<string, Record>
    {
       ["Platform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["FreightQuote"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["platform"] = new Value.I64Value(0),
            ["reference"] = new Value.TextValue(""),
            ["amount"] = new Value.DecimalValue(0),
            ["shipment_number"] = new Value.I64Value(0),
            ["package_count"] = new Value.I64Value(0),
            ["delivery_date"] = new Value.DateValue(DateTime.UnixEpoch),
            ["pickup_time"] = new Value.TimeValue(TimeSpan.Zero),
            ["quoted_at"] = new Value.TimestampValue(0),
            ["instructions"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Platform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["FreightQuote"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["platform"] = true,
           ["reference"] = true,
           ["amount"] = true,
           ["shipment_number"] = true,
           ["package_count"] = true,
           ["delivery_date"] = true,
           ["pickup_time"] = true,
           ["quoted_at"] = true,
           ["instructions"] = false,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Platform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("FreightQuoteList", "FreightQuote").LocalKey("id").ForeignKey("platform").Many()
        },
       ["FreightQuote"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Platform", "Platform").LocalKey("platform").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Platform"] = "platform_data",
       ["FreightQuote"] = "freight_quote_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Platform"] = new List<string> {
        },
       ["FreightQuote"] = new List<string> {
            "package_count",

        }
    }).WireEntity(WireFields.CreateMetadata("Platform", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("FreightQuote", ["id", "platform", "reference", "amount", "shipment_number", "package_count", "delivery_date", "pickup_time", "quoted_at", "instructions", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["platform"] = ["platform"], ["reference"] = ["reference"], ["amount"] = ["amount"], ["shipment_number"] = ["shipment_number"], ["package_count"] = ["package_count"], ["delivery_date"] = ["delivery_date"], ["pickup_time"] = ["pickup_time"], ["quoted_at"] = ["quoted_at"], ["instructions"] = ["instructions"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:3b8da34e762562f0877bfd04865e33c225b4d854c8a5914ea30b4a922d3df3b7\",\"entities\":{\"FreightQuote\":{\"labels\":{\"en\":\"Freight Quote\"},\"fields\":[{\"name\":\"amount\",\"labels\":{\"en\":\"amount\"},\"aliases\":[\"Amount\"]},{\"name\":\"delivery_date\",\"labels\":{\"en\":\"delivery date\"},\"aliases\":[\"DeliveryDate\",\"deliveryDate\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"instructions\",\"labels\":{\"en\":\"instructions\"},\"aliases\":[\"Instructions\"]},{\"name\":\"package_count\",\"labels\":{\"en\":\"package count\"},\"aliases\":[\"PackageCount\",\"packageCount\"]},{\"name\":\"pickup_time\",\"labels\":{\"en\":\"pickup time\"},\"aliases\":[\"PickupTime\",\"pickupTime\"]},{\"name\":\"platform\",\"targetType\":\"Platform\",\"labels\":{\"en\":\"Dispatch Platform\"},\"aliases\":[\"Platform\"]},{\"name\":\"quoted_at\",\"labels\":{\"en\":\"quoted at\"},\"aliases\":[\"QuotedAt\",\"quotedAt\"]},{\"name\":\"reference\",\"labels\":{\"en\":\"reference\"},\"aliases\":[\"Reference\"]},{\"name\":\"shipment_number\",\"labels\":{\"en\":\"shipment number\"},\"aliases\":[\"ShipmentNumber\",\"shipmentNumber\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"Platform\":{\"labels\":{\"en\":\"Dispatch Platform\"},\"fields\":[{\"name\":\"freight_quote_list\",\"targetType\":\"FreightQuote\",\"labels\":{\"en\":\"Freight Quote\"},\"aliases\":[\"FreightQuoteList\",\"freightQuoteList\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            created.UpdateName("Harbor Freight Desk");
            try { domainRoot = await created.AuditAs("create generated Domain Root Platform").SaveAsync(context); }
            catch { domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Platform", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Platform")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("FreightQuote")!.AuditMaskFields(new List<string> {  }));
    }
}