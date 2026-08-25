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
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult("required", "name"));
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", "name"));


        return results;
    }
}

internal sealed class WorkItemChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTime now)
    {
        var values = request.Command is InsertCommand insert ? insert.Values
            : request.Command is UpdateCommand update ? update.Values : new Record();
        var creating = request.Command is InsertCommand;
        var updating = request.Command is UpdateCommand;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("title")) || (values.TryGetValue("title", out var checkTitle) && checkTitle is Value.NullValue)) results.Add(new CheckResult("required", "title"));
        if (values.TryGetValue("title", out var minLenTitle) && minLenTitle.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult("min_length", "title"));
        if (values.TryGetValue("title", out var maxLenTitle) && maxLenTitle.Raw?.ToString()?.Length > 80) results.Add(new CheckResult("max_length", "title"));

        if (values.TryGetValue("description", out var maxLenDescription) && maxLenDescription.Raw?.ToString()?.Length > 100) results.Add(new CheckResult("max_length", "description"));

        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult("required", "platform"));


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Platform",
       "WorkItem"
    }, new Dictionary<string, IEntityChecker>
    {
        ["Platform"] = new PlatformChecker(),
        ["WorkItem"] = new WorkItemChecker()
    }, new Dictionary<string, Record>
    {
        ["Platform"] = new Record
        {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
        ["WorkItem"] = new Record
        {
            ["id"] = new Value.I64Value(0),
            ["title"] = new Value.TextValue(""),
            ["description"] = new Value.TextValue(""),
            ["platform"] = new Value.I64Value(0),
            ["version"] = new Value.I64Value(0)
        }
    });
}