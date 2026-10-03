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

internal sealed class CustomerOrderChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("platform") });

        if ((creating && !values.ContainsKey("order_number")) || (values.TryGetValue("order_number", out var checkOrderNumber) && checkOrderNumber is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("order_number") });
        if (values.TryGetValue("order_number", out var maxLenOrderNumber) && maxLenOrderNumber.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("order_number") });

        if ((creating && !values.ContainsKey("description")) || (values.TryGetValue("description", out var checkDescription) && checkDescription is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("description") });
        if (values.TryGetValue("description", out var maxLenDescription) && maxLenDescription.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("description") });


        return results;
    }
}

internal sealed class OrderItemChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("customer_order")) || (values.TryGetValue("customer_order", out var checkCustomerOrder) && checkCustomerOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("customer_order") });

        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });


        return results;
    }
}

internal sealed class PaymentChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("customer_order")) || (values.TryGetValue("customer_order", out var checkCustomerOrder) && checkCustomerOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("customer_order") });

        if ((creating && !values.ContainsKey("reference_code")) || (values.TryGetValue("reference_code", out var checkReferenceCode) && checkReferenceCode is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("reference_code") });
        if (values.TryGetValue("reference_code", out var maxLenReferenceCode) && maxLenReferenceCode.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("reference_code") });


        return results;
    }
}

internal sealed class PaymentAttemptChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("payment")) || (values.TryGetValue("payment", out var checkPayment) && checkPayment is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("payment") });

        if ((creating && !values.ContainsKey("reference_code")) || (values.TryGetValue("reference_code", out var checkReferenceCode) && checkReferenceCode is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("reference_code") });
        if (values.TryGetValue("reference_code", out var maxLenReferenceCode) && maxLenReferenceCode.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("reference_code") });


        return results;
    }
}

internal sealed class ShipmentChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("customer_order")) || (values.TryGetValue("customer_order", out var checkCustomerOrder) && checkCustomerOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("customer_order") });

        if ((creating && !values.ContainsKey("reference_code")) || (values.TryGetValue("reference_code", out var checkReferenceCode) && checkReferenceCode is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("reference_code") });
        if (values.TryGetValue("reference_code", out var maxLenReferenceCode) && maxLenReferenceCode.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("reference_code") });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Platform",
       "CustomerOrder",
       "OrderItem",
       "Payment",
       "PaymentAttempt",
       "Shipment"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Platform"] = new PlatformChecker(),
       ["CustomerOrder"] = new CustomerOrderChecker(),
       ["OrderItem"] = new OrderItemChecker(),
       ["Payment"] = new PaymentChecker(),
       ["PaymentAttempt"] = new PaymentAttemptChecker(),
       ["Shipment"] = new ShipmentChecker()
    }, new Dictionary<string, Record>
    {
       ["Platform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["CustomerOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["platform"] = new Value.I64Value(0),
            ["order_number"] = new Value.TextValue(""),
            ["description"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["OrderItem"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["customer_order"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["Payment"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["customer_order"] = new Value.I64Value(0),
            ["reference_code"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["PaymentAttempt"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["payment"] = new Value.I64Value(0),
            ["reference_code"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["Shipment"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["customer_order"] = new Value.I64Value(0),
            ["reference_code"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Platform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["CustomerOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["platform"] = true,
           ["order_number"] = true,
           ["description"] = true,
           ["version"] = true
        },
       ["OrderItem"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["customer_order"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["Payment"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["customer_order"] = true,
           ["reference_code"] = true,
           ["version"] = true
        },
       ["PaymentAttempt"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["payment"] = true,
           ["reference_code"] = true,
           ["version"] = true
        },
       ["Shipment"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["customer_order"] = true,
           ["reference_code"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Platform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerOrderList", "CustomerOrder").LocalKey("id").ForeignKey("platform").Many()
        },
       ["CustomerOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Platform", "Platform").LocalKey("platform").ForeignKey("id"),
            RelationDescriptor.New("OrderItemList", "OrderItem").LocalKey("id").ForeignKey("customer_order").Many(),
            RelationDescriptor.New("PaymentList", "Payment").LocalKey("id").ForeignKey("customer_order").Many(),
            RelationDescriptor.New("ShipmentList", "Shipment").LocalKey("id").ForeignKey("customer_order").Many()
        },
       ["OrderItem"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerOrder", "CustomerOrder").LocalKey("customer_order").ForeignKey("id")
        },
       ["Payment"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerOrder", "CustomerOrder").LocalKey("customer_order").ForeignKey("id"),
            RelationDescriptor.New("PaymentAttemptList", "PaymentAttempt").LocalKey("id").ForeignKey("payment").Many()
        },
       ["PaymentAttempt"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Payment", "Payment").LocalKey("payment").ForeignKey("id")
        },
       ["Shipment"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerOrder", "CustomerOrder").LocalKey("customer_order").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Platform"] = "platform_data",
       ["CustomerOrder"] = "customer_order_data",
       ["OrderItem"] = "order_item_data",
       ["Payment"] = "payment_data",
       ["PaymentAttempt"] = "payment_attempt_data",
       ["Shipment"] = "shipment_data"
    }).WireEntity(WireFields.CreateMetadata("Platform", ["id", "name", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("CustomerOrder", ["id", "platform", "order_number", "description", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["platform"] = ["platform"], ["order_number"] = ["order_number"], ["description"] = ["description"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("OrderItem", ["id", "customer_order", "name", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["customer_order"] = ["customer_order"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Payment", ["id", "customer_order", "reference_code", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["customer_order"] = ["customer_order"], ["reference_code"] = ["reference_code"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("PaymentAttempt", ["id", "payment", "reference_code", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["payment"] = ["payment"], ["reference_code"] = ["reference_code"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Shipment", ["id", "customer_order", "reference_code", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["customer_order"] = ["customer_order"], ["reference_code"] = ["reference_code"], ["version"] = ["version"] })).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            var created = Q.Platforms().Comment("what: create generated Domain Root").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1);
            created.UpdateName("Trace Chain Verification");
            try { domainRoot = await created.AuditAs("create generated Domain Root Platform").SaveAsync(context); }
            catch { domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Platform", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Platform")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("CustomerOrder")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("OrderItem")!.AuditMaskFields(new List<string> { "name" }));
        Module.Entity(Module.Metadata.GetEntity("Payment")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("PaymentAttempt")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("Shipment")!.AuditMaskFields(new List<string> {  }));
    }
}