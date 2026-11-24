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
        if (creating && (!values.TryGetValue("born_at", out var createBornAt) || createBornAt is Value.NullValue)) {
            ContextFix.Apply(context, request, now, "born_at", "now", [], "born_at");
        }

        if (creating && (!values.TryGetValue("touched_at", out var createTouchedAt) || createTouchedAt is Value.NullValue)) {
            ContextFix.Apply(context, request, now, "touched_at", "now", [], "touched_at");
        }
        if (updating) {
            ContextFix.Apply(context, request, now, "touched_at", "now", [], "touched_at");
        }





        if (creating && (!values.TryGetValue("guarded_token", out var createGuardedToken) || createGuardedToken is Value.NullValue)) {
            ContextFix.Apply(context, request, now, "guarded_token", "uniStr", [""], "guarded_token");
        }


        if ((creating && !values.ContainsKey("catalog")) || (values.TryGetValue("catalog", out var checkCatalog) && checkCatalog is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("catalog") });

        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Entry", Location = ObjectLocation.Property("name"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("born_at")) || (values.TryGetValue("born_at", out var checkBornAt) && checkBornAt is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("born_at") });

        if ((creating && !values.ContainsKey("touched_at")) || (values.TryGetValue("touched_at", out var checkTouchedAt) && checkTouchedAt is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("touched_at") });

        if ((creating && !values.ContainsKey("source_ip")) || (values.TryGetValue("source_ip", out var checkSourceIp) && checkSourceIp is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("source_ip") });
        if (values.TryGetValue("source_ip", out var maxLenSourceIp) && maxLenSourceIp.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Entry", Location = ObjectLocation.Property("source_ip"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("last_ip")) || (values.TryGetValue("last_ip", out var checkLastIp) && checkLastIp is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("last_ip") });
        if (values.TryGetValue("last_ip", out var maxLenLastIp) && maxLenLastIp.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Entry", Location = ObjectLocation.Property("last_ip"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("opened_by")) || (values.TryGetValue("opened_by", out var checkOpenedBy) && checkOpenedBy is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("opened_by") });
        if (values.TryGetValue("opened_by", out var maxLenOpenedBy) && maxLenOpenedBy.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Entry", Location = ObjectLocation.Property("opened_by"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("changed_by")) || (values.TryGetValue("changed_by", out var checkChangedBy) && checkChangedBy is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("changed_by") });
        if (values.TryGetValue("changed_by", out var maxLenChangedBy) && maxLenChangedBy.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Entry", Location = ObjectLocation.Property("changed_by"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("guarded_token")) || (values.TryGetValue("guarded_token", out var checkGuardedToken) && checkGuardedToken is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Entry", Location = ObjectLocation.Property("guarded_token") });
        if (values.TryGetValue("guarded_token", out var maxLenGuardedToken) && maxLenGuardedToken.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Entry", Location = ObjectLocation.Property("guarded_token"), SystemValue = 100 });


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
            ["born_at"] = new Value.TimestampValue(0),
            ["touched_at"] = new Value.TimestampValue(0),
            ["source_ip"] = new Value.TextValue(""),
            ["last_ip"] = new Value.TextValue(""),
            ["opened_by"] = new Value.TextValue(""),
            ["changed_by"] = new Value.TextValue(""),
            ["guarded_token"] = new Value.TextValue(""),
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
           ["born_at"] = true,
           ["touched_at"] = true,
           ["source_ip"] = true,
           ["last_ip"] = true,
           ["opened_by"] = true,
           ["changed_by"] = true,
           ["guarded_token"] = true,
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
    }).ContextField("Entry", "born_at", create: "now").ContextField("Entry", "touched_at", create: "now", update: "now").ContextField("Entry", "guarded_token", create: "uniStr").WireEntity(WireFields.CreateMetadata("Catalog", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Entry", ["id", "catalog", "name", "born_at", "touched_at", "source_ip", "last_ip", "opened_by", "changed_by", "guarded_token", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["catalog"] = ["catalog"], ["name"] = ["name"], ["born_at"] = ["born_at"], ["touched_at"] = ["touched_at"], ["source_ip"] = ["source_ip"], ["last_ip"] = ["last_ip"], ["opened_by"] = ["opened_by"], ["changed_by"] = ["changed_by"], ["guarded_token"] = ["guarded_token"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:ba88312203d540bc7019fb210837b5e15ba8bdff10957f7f03ff076e6896921b\",\"entities\":{\"Catalog\":{\"labels\":{\"en\":\"catalog\"},\"fields\":[{\"name\":\"entry_list\",\"targetType\":\"Entry\",\"labels\":{\"en\":\"entry\"},\"aliases\":[\"EntryList\",\"entryList\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"Entry\":{\"labels\":{\"en\":\"entry\"},\"fields\":[{\"name\":\"born_at\",\"labels\":{\"en\":\"born at\"},\"aliases\":[\"BornAt\",\"bornAt\"]},{\"name\":\"catalog\",\"targetType\":\"Catalog\",\"labels\":{\"en\":\"catalog\"},\"aliases\":[\"Catalog\"]},{\"name\":\"changed_by\",\"labels\":{\"en\":\"changed by\"},\"aliases\":[\"ChangedBy\",\"changedBy\"]},{\"name\":\"guarded_token\",\"labels\":{\"en\":\"guarded token\"},\"aliases\":[\"GuardedToken\",\"guardedToken\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"last_ip\",\"labels\":{\"en\":\"last ip\"},\"aliases\":[\"LastIp\",\"lastIp\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"opened_by\",\"labels\":{\"en\":\"opened by\"},\"aliases\":[\"OpenedBy\",\"openedBy\"]},{\"name\":\"source_ip\",\"labels\":{\"en\":\"source ip\"},\"aliases\":[\"SourceIp\",\"sourceIp\"]},{\"name\":\"touched_at\",\"labels\":{\"en\":\"touched at\"},\"aliases\":[\"TouchedAt\",\"touchedAt\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
        Module.Entity(Module.Metadata.GetEntity("Entry")!.AuditMaskFields(new List<string> { "guarded_token" }));
    }
}