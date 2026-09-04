using TeaQL.Core;

namespace Generated;

internal sealed class PlatformChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTime now)
    {
        var values = request.Command is InsertCommand insert ? insert.Values
            : request.Command is UpdateCommand update ? update.Values : new Record();
        var creating = request.Command is InsertCommand;
        var updating = request.Command is UpdateCommand;
        var results = new List<CheckResult>();
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.DateTimeValue(now);
            context.RecordFixEvidence(new FixEvidence("Platform", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.DateTimeValue(now);
            context.RecordFixEvidence(new FixEvidence("Platform", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.DateTimeValue(now);
            context.RecordFixEvidence(new FixEvidence("Platform", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("name")));
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", ObjectLocation.Property("name")));

        if ((creating && !values.ContainsKey("base_url")) || (values.TryGetValue("base_url", out var checkBaseUrl) && checkBaseUrl is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("base_url")));
        if (values.TryGetValue("base_url", out var maxLenBaseUrl) && maxLenBaseUrl.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", ObjectLocation.Property("base_url")));

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("create_time")));

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("update_time")));


        return results;
    }
}

internal sealed class SchoolTypeChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTime now)
    {
        var values = request.Command is InsertCommand insert ? insert.Values
            : request.Command is UpdateCommand update ? update.Values : new Record();
        var creating = request.Command is InsertCommand;
        var updating = request.Command is UpdateCommand;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("platform")));


        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("name")));
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", ObjectLocation.Property("name")));

        if ((creating && !values.ContainsKey("code")) || (values.TryGetValue("code", out var checkCode) && checkCode is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("code")));
        if (values.TryGetValue("code", out var maxLenCode) && maxLenCode.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", ObjectLocation.Property("code")));

        if ((creating && !values.ContainsKey("display_order")) || (values.TryGetValue("display_order", out var checkDisplayOrder) && checkDisplayOrder is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("display_order")));


        return results;
    }
}

internal sealed class SchoolChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTime now)
    {
        var values = request.Command is InsertCommand insert ? insert.Values
            : request.Command is UpdateCommand update ? update.Values : new Record();
        var creating = request.Command is InsertCommand;
        var updating = request.Command is UpdateCommand;
        var results = new List<CheckResult>();
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.DateTimeValue(now);
            context.RecordFixEvidence(new FixEvidence("School", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.DateTimeValue(now);
            context.RecordFixEvidence(new FixEvidence("School", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.DateTimeValue(now);
            context.RecordFixEvidence(new FixEvidence("School", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("platform")));

        if ((creating && !values.ContainsKey("school_type")) || (values.TryGetValue("school_type", out var checkSchoolType) && checkSchoolType is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("school_type")));

        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("name")));
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", ObjectLocation.Property("name")));

        if ((creating && !values.ContainsKey("address")) || (values.TryGetValue("address", out var checkAddress) && checkAddress is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("address")));
        if (values.TryGetValue("address", out var maxLenAddress) && maxLenAddress.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", ObjectLocation.Property("address")));

        if ((creating && !values.ContainsKey("established_date")) || (values.TryGetValue("established_date", out var checkEstablishedDate) && checkEstablishedDate is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("established_date")));

        if ((creating && !values.ContainsKey("student_capacity")) || (values.TryGetValue("student_capacity", out var checkStudentCapacity) && checkStudentCapacity is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("student_capacity")));

        if ((creating && !values.ContainsKey("active")) || (values.TryGetValue("active", out var checkActive) && checkActive is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("active")));

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("create_time")));

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult("required", ObjectLocation.Property("update_time")));


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Platform",
       "SchoolType",
       "School"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Platform"] = new PlatformChecker(),
       ["SchoolType"] = new SchoolTypeChecker(),
       ["School"] = new SchoolChecker()
    }, new Dictionary<string, Record>
    {
       ["Platform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["base_url"] = new Value.TextValue(""),
            ["create_time"] = new Value.DateTimeValue(DateTime.UnixEpoch),
            ["update_time"] = new Value.DateTimeValue(DateTime.UnixEpoch),
            ["version"] = new Value.I64Value(0)
        },
       ["SchoolType"] = new Record {
            ["platform"] = new Value.I64Value(0),
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["code"] = new Value.TextValue(""),
            ["display_order"] = new Value.DecimalValue(0),
            ["version"] = new Value.I64Value(0)
        },
       ["School"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["platform"] = new Value.I64Value(0),
            ["school_type"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["address"] = new Value.TextValue(""),
            ["established_date"] = new Value.DateValue(DateTime.UnixEpoch),
            ["student_capacity"] = new Value.I64Value(0),
            ["active"] = new Value.BoolValue(false),
            ["create_time"] = new Value.DateTimeValue(DateTime.UnixEpoch),
            ["update_time"] = new Value.DateTimeValue(DateTime.UnixEpoch),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Platform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["base_url"] = true,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        },
       ["SchoolType"] = new Dictionary<string, bool> {
           ["platform"] = true,
           ["id"] = true,
           ["name"] = true,
           ["code"] = true,
           ["display_order"] = true,
           ["version"] = true
        },
       ["School"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["platform"] = true,
           ["school_type"] = true,
           ["name"] = true,
           ["address"] = true,
           ["established_date"] = true,
           ["student_capacity"] = true,
           ["active"] = true,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        }
    }).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
        using var bootstrapScope = context.EnterGeneratedBootstrap("Platform", 1);
        var domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Q.Platforms().Comment("what: create generated Domain Root").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1);
            created.UpdateName("Campus Learning Platform");
            created.UpdateBaseUrl("https://campus.example.com");
            try { domainRoot = await created.AuditAs("create generated Domain Root Platform").SaveAsync(context); }
            catch { domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        var constantSchoolType1001 = await Q.SchoolTypes().WithIdIs(1001).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantSchoolType1001 == null)
        {
            var created = Q.SchoolTypes().Comment("what: create generated constant").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1001);
            created.UpdatePlatform(domainRoot.Id);
            created.UpdateName("Primary");
            created.UpdateCode("PRIMARY");
            created.UpdateDisplayOrder(decimal.Parse("1", System.Globalization.CultureInfo.InvariantCulture));
            try { constantSchoolType1001 = await created.AuditAs("create model constant SchoolType(1001)").SaveAsync(context); }
            catch { constantSchoolType1001 = await Q.SchoolTypes().WithIdIs(1001).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantSchoolType1001 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantSchoolType1001.Platform, domainRoot.Id))
            {
                constantSchoolType1001.UpdatePlatform(domainRoot.Id);
                changed = true;
            }
            if (!object.Equals(constantSchoolType1001.Name, "Primary"))
            {
                constantSchoolType1001.UpdateName("Primary");
                changed = true;
            }
            if (!object.Equals(constantSchoolType1001.Code, "PRIMARY"))
            {
                constantSchoolType1001.UpdateCode("PRIMARY");
                changed = true;
            }
            if (!object.Equals(constantSchoolType1001.DisplayOrder, decimal.Parse("1", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantSchoolType1001.UpdateDisplayOrder(decimal.Parse("1", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (changed) constantSchoolType1001 = await constantSchoolType1001.AuditAs("reconcile model constant SchoolType(1001)").SaveAsync(context);
        }
        var constantSchoolType1002 = await Q.SchoolTypes().WithIdIs(1002).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantSchoolType1002 == null)
        {
            var created = Q.SchoolTypes().Comment("what: create generated constant").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1002);
            created.UpdatePlatform(domainRoot.Id);
            created.UpdateName("Secondary");
            created.UpdateCode("SECONDARY");
            created.UpdateDisplayOrder(decimal.Parse("2", System.Globalization.CultureInfo.InvariantCulture));
            try { constantSchoolType1002 = await created.AuditAs("create model constant SchoolType(1002)").SaveAsync(context); }
            catch { constantSchoolType1002 = await Q.SchoolTypes().WithIdIs(1002).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantSchoolType1002 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantSchoolType1002.Platform, domainRoot.Id))
            {
                constantSchoolType1002.UpdatePlatform(domainRoot.Id);
                changed = true;
            }
            if (!object.Equals(constantSchoolType1002.Name, "Secondary"))
            {
                constantSchoolType1002.UpdateName("Secondary");
                changed = true;
            }
            if (!object.Equals(constantSchoolType1002.Code, "SECONDARY"))
            {
                constantSchoolType1002.UpdateCode("SECONDARY");
                changed = true;
            }
            if (!object.Equals(constantSchoolType1002.DisplayOrder, decimal.Parse("2", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantSchoolType1002.UpdateDisplayOrder(decimal.Parse("2", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (changed) constantSchoolType1002 = await constantSchoolType1002.AuditAs("reconcile model constant SchoolType(1002)").SaveAsync(context);
        }
    }

}