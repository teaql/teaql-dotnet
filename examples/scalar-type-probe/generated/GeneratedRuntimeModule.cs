using TeaQL.Core;

namespace Generated;

internal sealed class TypePlatformChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });


        return results;
    }
}

internal sealed class ScalarTypeProbeChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("ScalarTypeProbe", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("ScalarTypeProbe", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("ScalarTypeProbe", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("type_platform")) || (values.TryGetValue("type_platform", out var checkTypePlatform) && checkTypePlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("type_platform") });

        if ((creating && !values.ContainsKey("required_string")) || (values.TryGetValue("required_string", out var checkRequiredString) && checkRequiredString is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_string") });
        if (values.TryGetValue("required_string", out var maxLenRequiredString) && maxLenRequiredString.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("required_string") });

        if (values.TryGetValue("optional_string", out var maxLenOptionalString) && maxLenOptionalString.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("optional_string") });


        if ((creating && !values.ContainsKey("required_boolean")) || (values.TryGetValue("required_boolean", out var checkRequiredBoolean) && checkRequiredBoolean is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_boolean") });


        if ((creating && !values.ContainsKey("required_integer")) || (values.TryGetValue("required_integer", out var checkRequiredInteger) && checkRequiredInteger is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_integer") });


        if ((creating && !values.ContainsKey("required_long")) || (values.TryGetValue("required_long", out var checkRequiredLong) && checkRequiredLong is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_long") });


        if ((creating && !values.ContainsKey("required_decimal")) || (values.TryGetValue("required_decimal", out var checkRequiredDecimal) && checkRequiredDecimal is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_decimal") });


        if ((creating && !values.ContainsKey("required_float")) || (values.TryGetValue("required_float", out var checkRequiredFloat) && checkRequiredFloat is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_float") });


        if ((creating && !values.ContainsKey("required_double")) || (values.TryGetValue("required_double", out var checkRequiredDouble) && checkRequiredDouble is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_double") });


        if ((creating && !values.ContainsKey("required_date")) || (values.TryGetValue("required_date", out var checkRequiredDate) && checkRequiredDate is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_date") });


        if ((creating && !values.ContainsKey("required_time")) || (values.TryGetValue("required_time", out var checkRequiredTime) && checkRequiredTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_time") });


        if ((creating && !values.ContainsKey("required_datetime")) || (values.TryGetValue("required_datetime", out var checkRequiredDatetime) && checkRequiredDatetime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("required_datetime") });


        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("create_time") });

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("update_time") });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "TypePlatform",
       "ScalarTypeProbe"
    }, new Dictionary<string, IEntityChecker>
    {
       ["TypePlatform"] = new TypePlatformChecker(),
       ["ScalarTypeProbe"] = new ScalarTypeProbeChecker()
    }, new Dictionary<string, Record>
    {
       ["TypePlatform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["ScalarTypeProbe"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["type_platform"] = new Value.I64Value(0),
            ["required_string"] = new Value.TextValue(""),
            ["optional_string"] = new Value.TextValue(""),
            ["large_text"] = new Value.TextValue(""),
            ["required_boolean"] = new Value.BoolValue(false),
            ["optional_boolean"] = new Value.BoolValue(false),
            ["required_integer"] = new Value.I64Value(0),
            ["optional_integer"] = new Value.I64Value(0),
            ["required_long"] = new Value.I64Value(0),
            ["optional_long"] = new Value.I64Value(0),
            ["required_decimal"] = new Value.DecimalValue(0),
            ["optional_decimal"] = new Value.DecimalValue(0),
            ["required_float"] = new Value.F64Value(0),
            ["optional_float"] = new Value.F64Value(0),
            ["required_double"] = new Value.F64Value(0),
            ["optional_double"] = new Value.F64Value(0),
            ["required_date"] = new Value.DateValue(DateTime.UnixEpoch),
            ["optional_date"] = new Value.DateValue(DateTime.UnixEpoch),
            ["required_time"] = new Value.TimeValue(TimeSpan.Zero),
            ["optional_time"] = new Value.TimeValue(TimeSpan.Zero),
            ["required_datetime"] = new Value.TimestampValue(0),
            ["optional_datetime"] = new Value.TimestampValue(0),
            ["create_time"] = new Value.TimestampValue(0),
            ["update_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["TypePlatform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["ScalarTypeProbe"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["type_platform"] = true,
           ["required_string"] = true,
           ["optional_string"] = false,
           ["large_text"] = false,
           ["required_boolean"] = true,
           ["optional_boolean"] = false,
           ["required_integer"] = true,
           ["optional_integer"] = false,
           ["required_long"] = true,
           ["optional_long"] = false,
           ["required_decimal"] = true,
           ["optional_decimal"] = false,
           ["required_float"] = true,
           ["optional_float"] = false,
           ["required_double"] = true,
           ["optional_double"] = false,
           ["required_date"] = true,
           ["optional_date"] = false,
           ["required_time"] = true,
           ["optional_time"] = false,
           ["required_datetime"] = true,
           ["optional_datetime"] = false,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["TypePlatform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("ScalarTypeProbeList", "ScalarTypeProbe").LocalKey("id").ForeignKey("type_platform").Many()
        },
       ["ScalarTypeProbe"] = new List<RelationDescriptor> {
            RelationDescriptor.New("TypePlatform", "TypePlatform").LocalKey("type_platform").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["TypePlatform"] = "type_platform_data",
       ["ScalarTypeProbe"] = "scalar_type_probe_data"
    }).WireEntity(WireFields.CreateMetadata("TypePlatform", ["id", "name", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("ScalarTypeProbe", ["id", "type_platform", "required_string", "optional_string", "large_text", "required_boolean", "optional_boolean", "required_integer", "optional_integer", "required_long", "optional_long", "required_decimal", "optional_decimal", "required_float", "optional_float", "required_double", "optional_double", "required_date", "optional_date", "required_time", "optional_time", "required_datetime", "optional_datetime", "create_time", "update_time", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["type_platform"] = ["type_platform"], ["required_string"] = ["required_string"], ["optional_string"] = ["optional_string"], ["large_text"] = ["large_text"], ["required_boolean"] = ["required_boolean"], ["optional_boolean"] = ["optional_boolean"], ["required_integer"] = ["required_integer"], ["optional_integer"] = ["optional_integer"], ["required_long"] = ["required_long"], ["optional_long"] = ["optional_long"], ["required_decimal"] = ["required_decimal"], ["optional_decimal"] = ["optional_decimal"], ["required_float"] = ["required_float"], ["optional_float"] = ["optional_float"], ["required_double"] = ["required_double"], ["optional_double"] = ["optional_double"], ["required_date"] = ["required_date"], ["optional_date"] = ["optional_date"], ["required_time"] = ["required_time"], ["optional_time"] = ["optional_time"], ["required_datetime"] = ["required_datetime"], ["optional_datetime"] = ["optional_datetime"], ["create_time"] = ["create_time"], ["update_time"] = ["update_time"], ["version"] = ["version"] })).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            using var bootstrapScope = context.EnterGeneratedBootstrap("TypePlatform", 1);
        var domainRoot = await Q.TypePlatforms().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Q.TypePlatforms().Comment("what: create generated Domain Root").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1);
            created.UpdateName("Scalar Type Conformance");
            try { domainRoot = await created.AuditAs("create generated Domain Root TypePlatform").SaveAsync(context); }
            catch { domainRoot = await Q.TypePlatforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("TypePlatform", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("TypePlatform")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("ScalarTypeProbe")!.AuditMaskFields(new List<string> {  }));
    }
}