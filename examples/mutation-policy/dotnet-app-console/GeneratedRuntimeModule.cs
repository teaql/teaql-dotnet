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

internal sealed class CustomerChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Customer", Location = ObjectLocation.Property("platform") });

        if ((creating && !values.ContainsKey("email")) || (values.TryGetValue("email", out var checkEmail) && checkEmail is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Customer", Location = ObjectLocation.Property("email") });
        if (values.TryGetValue("email", out var minLenEmail) && minLenEmail.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult { RuleId = "min_length", EntityType = "Customer", Location = ObjectLocation.Property("email"), SystemValue = 1 });
        if (values.TryGetValue("email", out var maxLenEmail) && maxLenEmail.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Customer", Location = ObjectLocation.Property("email"), SystemValue = 100 });


        return results;
    }
}

internal sealed class CustomerContactChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("customer")) || (values.TryGetValue("customer", out var checkCustomer) && checkCustomer is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "CustomerContact", Location = ObjectLocation.Property("customer") });

        if ((creating && !values.ContainsKey("channel")) || (values.TryGetValue("channel", out var checkChannel) && checkChannel is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "CustomerContact", Location = ObjectLocation.Property("channel") });
        if (values.TryGetValue("channel", out var minLenChannel) && minLenChannel.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult { RuleId = "min_length", EntityType = "CustomerContact", Location = ObjectLocation.Property("channel"), SystemValue = 1 });
        if (values.TryGetValue("channel", out var maxLenChannel) && maxLenChannel.Raw?.ToString()?.Length > 40) results.Add(new CheckResult { RuleId = "max_length", EntityType = "CustomerContact", Location = ObjectLocation.Property("channel"), SystemValue = 40 });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Platform",
       "Customer",
       "CustomerContact"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Platform"] = new PlatformChecker(),
       ["Customer"] = new CustomerChecker(),
       ["CustomerContact"] = new CustomerContactChecker()
    }, new Dictionary<string, Record>
    {
       ["Platform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["Customer"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["platform"] = new Value.I64Value(0),
            ["email"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["CustomerContact"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["customer"] = new Value.I64Value(0),
            ["channel"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Platform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["Customer"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["platform"] = true,
           ["email"] = true,
           ["version"] = true
        },
       ["CustomerContact"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["customer"] = true,
           ["channel"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Platform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerList", "Customer").LocalKey("id").ForeignKey("platform").Many()
        },
       ["Customer"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Platform", "Platform").LocalKey("platform").ForeignKey("id"),
            RelationDescriptor.New("CustomerContactList", "CustomerContact").LocalKey("id").ForeignKey("customer").Many()
        },
       ["CustomerContact"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Customer", "Customer").LocalKey("customer").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Platform"] = "platform_data",
       ["Customer"] = "customer_data",
       ["CustomerContact"] = "customer_contact_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Platform"] = new List<string> {
        },
       ["Customer"] = new List<string> {
        },
       ["CustomerContact"] = new List<string> {
        }
    }).WireEntity(WireFields.CreateMetadata("Platform", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Customer", ["id", "platform", "email", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["platform"] = ["platform"], ["email"] = ["email"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("CustomerContact", ["id", "customer", "channel", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["customer"] = ["customer"], ["channel"] = ["channel"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:c4dce4f2f6d12ed07cc3f68d59eebf08de6689664fb11cc4e7621965517d9763\",\"entities\":{\"Customer\":{\"labels\":{\"en\":\"customer\"},\"fields\":[{\"name\":\"customer_contact_list\",\"targetType\":\"CustomerContact\",\"labels\":{\"en\":\"customer contact\"},\"aliases\":[\"CustomerContactList\",\"customerContactList\"]},{\"name\":\"email\",\"labels\":{\"en\":\"email\"},\"aliases\":[\"Email\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"platform\",\"targetType\":\"Platform\",\"labels\":{\"en\":\"platform\"},\"aliases\":[\"Platform\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"CustomerContact\":{\"labels\":{\"en\":\"customer contact\"},\"fields\":[{\"name\":\"channel\",\"labels\":{\"en\":\"channel\"},\"aliases\":[\"Channel\"]},{\"name\":\"customer\",\"targetType\":\"Customer\",\"labels\":{\"en\":\"customer\"},\"aliases\":[\"Customer\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"Platform\":{\"labels\":{\"en\":\"platform\"},\"fields\":[{\"name\":\"customer_list\",\"targetType\":\"Customer\",\"labels\":{\"en\":\"customer\"},\"aliases\":[\"CustomerList\",\"customerList\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            created.UpdateName("Customer Services");
            try { domainRoot = await created.AuditAs("create generated Domain Root Platform").SaveAsync(context); }
            catch { domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Platform", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Platform")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("Customer")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("CustomerContact")!.AuditMaskFields(new List<string> {  }));
    }
}