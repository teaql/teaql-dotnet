using TeaQL.Core;

namespace Generated;

internal sealed class CommercePlatformChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "CommercePlatform", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "CommercePlatform", Location = ObjectLocation.Property("name"), SystemValue = 100 });


        return results;
    }
}

internal sealed class CustomerOrderChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("order_number")) || (values.TryGetValue("order_number", out var checkOrderNumber) && checkOrderNumber is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "CustomerOrder", Location = ObjectLocation.Property("order_number") });
        if (values.TryGetValue("order_number", out var maxLenOrderNumber) && maxLenOrderNumber.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "CustomerOrder", Location = ObjectLocation.Property("order_number"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("commerce_platform")) || (values.TryGetValue("commerce_platform", out var checkCommercePlatform) && checkCommercePlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "CustomerOrder", Location = ObjectLocation.Property("commerce_platform") });


        return results;
    }
}

internal sealed class OrderAttachmentChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("customer_order")) || (values.TryGetValue("customer_order", out var checkCustomerOrder) && checkCustomerOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "OrderAttachment", Location = ObjectLocation.Property("customer_order") });

        if ((creating && !values.ContainsKey("file_name")) || (values.TryGetValue("file_name", out var checkFileName) && checkFileName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "OrderAttachment", Location = ObjectLocation.Property("file_name") });
        if (values.TryGetValue("file_name", out var maxLenFileName) && maxLenFileName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "OrderAttachment", Location = ObjectLocation.Property("file_name"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("content_type")) || (values.TryGetValue("content_type", out var checkContentType) && checkContentType is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "OrderAttachment", Location = ObjectLocation.Property("content_type") });
        if (values.TryGetValue("content_type", out var maxLenContentType) && maxLenContentType.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "OrderAttachment", Location = ObjectLocation.Property("content_type"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("content_length")) || (values.TryGetValue("content_length", out var checkContentLength) && checkContentLength is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "OrderAttachment", Location = ObjectLocation.Property("content_length") });

        if ((creating && !values.ContainsKey("content_sha256")) || (values.TryGetValue("content_sha256", out var checkContentSha256) && checkContentSha256 is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "OrderAttachment", Location = ObjectLocation.Property("content_sha256") });
        if (values.TryGetValue("content_sha256", out var maxLenContentSha256) && maxLenContentSha256.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "OrderAttachment", Location = ObjectLocation.Property("content_sha256"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("object_key")) || (values.TryGetValue("object_key", out var checkObjectKey) && checkObjectKey is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "OrderAttachment", Location = ObjectLocation.Property("object_key") });
        if (values.TryGetValue("object_key", out var maxLenObjectKey) && maxLenObjectKey.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "OrderAttachment", Location = ObjectLocation.Property("object_key"), SystemValue = 100 });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "CommercePlatform",
       "CustomerOrder",
       "OrderAttachment"
    }, new Dictionary<string, IEntityChecker>
    {
       ["CommercePlatform"] = new CommercePlatformChecker(),
       ["CustomerOrder"] = new CustomerOrderChecker(),
       ["OrderAttachment"] = new OrderAttachmentChecker()
    }, new Dictionary<string, Record>
    {
       ["CommercePlatform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["CustomerOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["order_number"] = new Value.TextValue(""),
            ["commerce_platform"] = new Value.I64Value(0),
            ["version"] = new Value.I64Value(0)
        },
       ["OrderAttachment"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["customer_order"] = new Value.I64Value(0),
            ["file_name"] = new Value.TextValue(""),
            ["content_type"] = new Value.TextValue(""),
            ["content_length"] = new Value.I64Value(0),
            ["content_sha256"] = new Value.TextValue(""),
            ["object_key"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["CommercePlatform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["CustomerOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["order_number"] = true,
           ["commerce_platform"] = true,
           ["version"] = true
        },
       ["OrderAttachment"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["customer_order"] = true,
           ["file_name"] = true,
           ["content_type"] = true,
           ["content_length"] = true,
           ["content_sha256"] = true,
           ["object_key"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["CommercePlatform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerOrderList", "CustomerOrder").LocalKey("id").ForeignKey("commerce_platform").Many()
        },
       ["CustomerOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CommercePlatform", "CommercePlatform").LocalKey("commerce_platform").ForeignKey("id"),
            RelationDescriptor.New("OrderAttachmentList", "OrderAttachment").LocalKey("id").ForeignKey("customer_order").Many()
        },
       ["OrderAttachment"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerOrder", "CustomerOrder").LocalKey("customer_order").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["CommercePlatform"] = "commerce_platform_data",
       ["CustomerOrder"] = "customer_order_data",
       ["OrderAttachment"] = "order_attachment_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["CommercePlatform"] = new List<string> {
        },
       ["CustomerOrder"] = new List<string> {
        },
       ["OrderAttachment"] = new List<string> {
            "content_length",

        }
    }).WireEntity(WireFields.CreateMetadata("CommercePlatform", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("CustomerOrder", ["id", "order_number", "commerce_platform", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["order_number"] = ["order_number"], ["commerce_platform"] = ["commerce_platform"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("OrderAttachment", ["id", "customer_order", "file_name", "content_type", "content_length", "content_sha256", "object_key", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["customer_order"] = ["customer_order"], ["file_name"] = ["file_name"], ["content_type"] = ["content_type"], ["content_length"] = ["content_length"], ["content_sha256"] = ["content_sha256"], ["object_key"] = ["object_key"], ["version"] = ["version"] })).CheckerPresentation(CheckerPresentationModel.FromJson("{\"schema\":\"teaql.checker-model/v1\",\"revision\":\"sha256:f6fb4b9ca6d876325850fdce5a0d2ab5c57f19c74af6bbffb2b494779e9c27b9\",\"entities\":{\"CommercePlatform\":{\"labels\":{\"en\":\"commerce platform\"},\"fields\":[{\"name\":\"customer_order_list\",\"targetType\":\"CustomerOrder\",\"labels\":{\"en\":\"customer order\"},\"aliases\":[\"CustomerOrderList\",\"customerOrderList\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"name\",\"labels\":{\"en\":\"name\"},\"aliases\":[\"Name\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"CustomerOrder\":{\"labels\":{\"en\":\"customer order\"},\"fields\":[{\"name\":\"commerce_platform\",\"targetType\":\"CommercePlatform\",\"labels\":{\"en\":\"commerce platform\"},\"aliases\":[\"CommercePlatform\",\"commercePlatform\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"order_attachment_list\",\"targetType\":\"OrderAttachment\",\"labels\":{\"en\":\"order attachment\"},\"aliases\":[\"OrderAttachmentList\",\"orderAttachmentList\"]},{\"name\":\"order_number\",\"labels\":{\"en\":\"order number\"},\"aliases\":[\"OrderNumber\",\"orderNumber\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]},\"OrderAttachment\":{\"labels\":{\"en\":\"order attachment\"},\"fields\":[{\"name\":\"content_length\",\"labels\":{\"en\":\"content length\"},\"aliases\":[\"ContentLength\",\"contentLength\"]},{\"name\":\"content_sha256\",\"labels\":{\"en\":\"content sha256\"},\"aliases\":[\"ContentSha256\",\"contentSha256\"]},{\"name\":\"content_type\",\"labels\":{\"en\":\"content type\"},\"aliases\":[\"ContentType\",\"contentType\"]},{\"name\":\"customer_order\",\"targetType\":\"CustomerOrder\",\"labels\":{\"en\":\"customer order\"},\"aliases\":[\"CustomerOrder\",\"customerOrder\"]},{\"name\":\"file_name\",\"labels\":{\"en\":\"file name\"},\"aliases\":[\"FileName\",\"fileName\"]},{\"name\":\"id\",\"labels\":{\"en\":\"id\"},\"aliases\":[\"Id\"]},{\"name\":\"object_key\",\"labels\":{\"en\":\"object key\"},\"aliases\":[\"ObjectKey\",\"objectKey\"]},{\"name\":\"version\",\"labels\":{\"en\":\"version\"},\"aliases\":[\"Version\"]}]}}}")).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            using var bootstrapScope = context.EnterGeneratedBootstrap("CommercePlatform", 1);
        var domainRoot = await Q.CommercePlatforms().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Generated.Models.CommercePlatform.TeaqlNewWithFixedId(1);
            created.UpdateName("Order Attachments");
            try { domainRoot = await created.AuditAs("create generated Domain Root CommercePlatform").SaveAsync(context); }
            catch { domainRoot = await Q.CommercePlatforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("CommercePlatform", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("CommercePlatform")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("CustomerOrder")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("OrderAttachment")!.AuditMaskFields(new List<string> { "object_key" }));
    }
}