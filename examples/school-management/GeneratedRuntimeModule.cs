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
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) values["create_time"] = new Value.DateTimeValue(now);

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) values["update_time"] = new Value.DateTimeValue(now);
        if (updating) values["update_time"] = new Value.DateTimeValue(now);


        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult("required", "name"));
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", "name"));

        if ((creating && !values.ContainsKey("base_url")) || (values.TryGetValue("base_url", out var checkBaseUrl) && checkBaseUrl is Value.NullValue)) results.Add(new CheckResult("required", "baseUrl"));
        if (values.TryGetValue("base_url", out var maxLenBaseUrl) && maxLenBaseUrl.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", "baseUrl"));

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult("required", "createTime"));

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult("required", "updateTime"));


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
        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult("required", "platform"));


        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult("required", "name"));
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", "name"));

        if ((creating && !values.ContainsKey("code")) || (values.TryGetValue("code", out var checkCode) && checkCode is Value.NullValue)) results.Add(new CheckResult("required", "code"));
        if (values.TryGetValue("code", out var maxLenCode) && maxLenCode.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", "code"));

        if ((creating && !values.ContainsKey("display_order")) || (values.TryGetValue("display_order", out var checkDisplayOrder) && checkDisplayOrder is Value.NullValue)) results.Add(new CheckResult("required", "displayOrder"));


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
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) values["create_time"] = new Value.DateTimeValue(now);

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) values["update_time"] = new Value.DateTimeValue(now);
        if (updating) values["update_time"] = new Value.DateTimeValue(now);


        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult("required", "platform"));

        if ((creating && !values.ContainsKey("school_type")) || (values.TryGetValue("school_type", out var checkSchoolType) && checkSchoolType is Value.NullValue)) results.Add(new CheckResult("required", "schoolType"));

        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult("required", "name"));
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", "name"));

        if ((creating && !values.ContainsKey("address")) || (values.TryGetValue("address", out var checkAddress) && checkAddress is Value.NullValue)) results.Add(new CheckResult("required", "address"));
        if (values.TryGetValue("address", out var maxLenAddress) && maxLenAddress.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", "address"));

        if ((creating && !values.ContainsKey("established_date")) || (values.TryGetValue("established_date", out var checkEstablishedDate) && checkEstablishedDate is Value.NullValue)) results.Add(new CheckResult("required", "establishedDate"));

        if ((creating && !values.ContainsKey("student_capacity")) || (values.TryGetValue("student_capacity", out var checkStudentCapacity) && checkStudentCapacity is Value.NullValue)) results.Add(new CheckResult("required", "studentCapacity"));

        if ((creating && !values.ContainsKey("active")) || (values.TryGetValue("active", out var checkActive) && checkActive is Value.NullValue)) results.Add(new CheckResult("required", "active"));

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult("required", "createTime"));

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult("required", "updateTime"));


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
            ["create_time"] = new Value.DateValue(DateTime.UnixEpoch),
            ["update_time"] = new Value.DateValue(DateTime.UnixEpoch),
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
            ["active"] = new Value.I64Value(0),
            ["create_time"] = new Value.DateValue(DateTime.UnixEpoch),
            ["update_time"] = new Value.DateValue(DateTime.UnixEpoch),
            ["version"] = new Value.I64Value(0)
        }
    }, new[] { new BootstrapEntity("Platform", 1, new Record { ["name"] = new Value.TextValue("Campus Learning Platform"), ["base_url"] = new Value.TextValue("https://campus.example.com"), ["create_time"] = new Value.DateTimeValue(DateTime.UtcNow), ["update_time"] = new Value.DateTimeValue(DateTime.UtcNow) }) }, new[] { new BootstrapEntity("SchoolType", 1001, new Record { ["platform"] = new Value.I64Value(1), ["name"] = new Value.TextValue("Primary"), ["code"] = new Value.TextValue("PRIMARY"), ["display_order"] = new Value.DecimalValue(1m) }), new BootstrapEntity("SchoolType", 1002, new Record { ["platform"] = new Value.I64Value(1), ["name"] = new Value.TextValue("Secondary"), ["code"] = new Value.TextValue("SECONDARY"), ["display_order"] = new Value.DecimalValue(2m) }) });
}