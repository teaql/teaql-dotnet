using TeaQL.Core;

namespace Generated;

internal sealed class CatalogChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Catalog", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Catalog", Location = ObjectLocation.Property("name"), SystemValue = 100 });


        return results;
    }
}

internal sealed class EntryChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            ContextFix.Apply(context, request, now, "create_time", "now", [], "create_time");
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            ContextFix.Apply(context, request, now, "update_time", "now", [], "update_time");
        }
        if (updating) {
            ContextFix.Apply(context, request, now, "update_time", "now", [], "update_time");
        }


        if ((creating && !values.ContainsKey("catalog")) || (values.TryGetValue("catalog", out var checkCatalog) && checkCatalog is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("catalog") });

        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Entry", Location = ObjectLocation.Property("name"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("create_time") });

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("update_time") });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Catalog",
       "Entry"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Catalog"] = new CatalogChecker(),
       ["Entry"] = new EntryChecker()
    }, new Dictionary<string, Record>
    {
       ["Catalog"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["Entry"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["catalog"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["create_time"] = new Value.TimestampValue(0),
            ["update_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Catalog"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["Entry"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["catalog"] = true,
           ["name"] = true,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Catalog"] = new List<RelationDescriptor> {
            RelationDescriptor.New("EntryList", "Entry").LocalKey("id").ForeignKey("catalog").Many()
        },
       ["Entry"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Catalog", "Catalog").LocalKey("catalog").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Catalog"] = "catalog_data",
       ["Entry"] = "entry_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Catalog"] = new List<string> {
        },
       ["Entry"] = new List<string> {
        }
    }).ContextField("Entry", "create_time", create: "now").ContextField("Entry", "update_time", create: "now", update: "now").WireEntity(WireFields.CreateMetadata("Catalog", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Entry", ["id", "catalog", "name", "create_time", "update_time", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["catalog"] = ["catalog"], ["name"] = ["name"], ["create_time"] = ["create_time"], ["update_time"] = ["update_time"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:9ea98ce6f86d4528fa8abed527685e55e825dcb5a695c61871b7aeab0eba3967\",\"entities\":{\"Catalog\":{\"labels\":{\"en\":\"catalog\",\"zh-CN\":\"目录\"},\"fields\":[{\"name\":\"entry_list\",\"targetType\":\"Entry\",\"labels\":{\"en\":\"entry\",\"zh-CN\":\"明细\"},\"aliases\":[\"EntryList\",\"entryList\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\",\"zh-CN\":\"名称\"},\"aliases\":[\"Name\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"Entry\":{\"labels\":{\"en\":\"entry\",\"zh-CN\":\"明细\"},\"fields\":[{\"name\":\"catalog\",\"targetType\":\"Catalog\",\"labels\":{\"en\":\"catalog\",\"zh-CN\":\"目录\"},\"aliases\":[\"Catalog\"]},{\"name\":\"create_time\",\"labels\":{\"en\":\"create time\"},\"aliases\":[\"CreateTime\",\"createTime\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\",\"zh-CN\":\"名称\"},\"aliases\":[\"Name\"]},{\"name\":\"update_time\",\"labels\":{\"en\":\"update time\"},\"aliases\":[\"UpdateTime\",\"updateTime\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            using var bootstrapScope = context.EnterGeneratedBootstrap("Catalog", 1);
        var domainRoot = await Q.Catalogs().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Generated.Models.Catalog.TeaqlNewWithFixedId(1);
            created.UpdateName("Catalog");
            try { domainRoot = await created.AuditAs("create generated Domain Root Catalog").SaveAsync(context); }
            catch { domainRoot = await Q.Catalogs().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Catalog", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Catalog")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("Entry")!.AuditMaskFields(new List<string> { "name" }));
    }
}