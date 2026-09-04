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
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });


        return results;
    }
}

internal sealed class WorkItemChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("title")) || (values.TryGetValue("title", out var checkTitle) && checkTitle is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("title") });
        if (values.TryGetValue("title", out var minLenTitle) && minLenTitle.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult { RuleId = "min_length", Location = ObjectLocation.Property("title") });
        if (values.TryGetValue("title", out var maxLenTitle) && maxLenTitle.Raw?.ToString()?.Length > 80) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("title") });

        if (values.TryGetValue("description", out var maxLenDescription) && maxLenDescription.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("description") });

        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("platform") });


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
       ["Platform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["WorkItem"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["title"] = new Value.TextValue(""),
            ["description"] = new Value.TextValue(""),
            ["platform"] = new Value.I64Value(0),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Platform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["WorkItem"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["title"] = true,
           ["description"] = false,
           ["platform"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Platform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("WorkItemList", "WorkItem").LocalKey("id").ForeignKey("platform").Many()
        },
       ["WorkItem"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Platform", "Platform").LocalKey("platform").ForeignKey("id")
        }
    }).WireEntity(WireFields.CreateMetadata("Platform", ["id", "name", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("WorkItem", ["id", "title", "description", "platform", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["title"] = ["title"], ["description"] = ["description"], ["platform"] = ["platform"], ["version"] = ["version"] })).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            created.UpdateName("Runtime Example");
            try { domainRoot = await created.AuditAs("create generated Domain Root Platform").SaveAsync(context); }
            catch { domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
    }

}