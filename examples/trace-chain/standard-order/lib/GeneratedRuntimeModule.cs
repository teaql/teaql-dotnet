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

internal sealed class OrderStatusChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("platform") });


        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });

        if ((creating && !values.ContainsKey("code")) || (values.TryGetValue("code", out var checkCode) && checkCode is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("code") });
        if (values.TryGetValue("code", out var maxLenCode) && maxLenCode.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("code") });

        if ((creating && !values.ContainsKey("color")) || (values.TryGetValue("color", out var checkColor) && checkColor is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("color") });
        if (values.TryGetValue("color", out var maxLenColor) && maxLenColor.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("color") });

        if ((creating && !values.ContainsKey("display_order")) || (values.TryGetValue("display_order", out var checkDisplayOrder) && checkDisplayOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("display_order") });


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
        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("platform") });

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
        if ((creating && !values.ContainsKey("customer")) || (values.TryGetValue("customer", out var checkCustomer) && checkCustomer is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("customer") });

        if ((creating && !values.ContainsKey("status")) || (values.TryGetValue("status", out var checkStatus) && checkStatus is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("status") });

        if ((creating && !values.ContainsKey("order_number")) || (values.TryGetValue("order_number", out var checkOrderNumber) && checkOrderNumber is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("order_number") });
        if (values.TryGetValue("order_number", out var maxLenOrderNumber) && maxLenOrderNumber.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("order_number") });

        if ((creating && !values.ContainsKey("order_date")) || (values.TryGetValue("order_date", out var checkOrderDate) && checkOrderDate is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("order_date") });

        if ((creating && !values.ContainsKey("total_amount")) || (values.TryGetValue("total_amount", out var checkTotalAmount) && checkTotalAmount is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("total_amount") });


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

        if ((creating && !values.ContainsKey("product_name")) || (values.TryGetValue("product_name", out var checkProductName) && checkProductName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("product_name") });
        if (values.TryGetValue("product_name", out var maxLenProductName) && maxLenProductName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("product_name") });

        if ((creating && !values.ContainsKey("quantity")) || (values.TryGetValue("quantity", out var checkQuantity) && checkQuantity is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("quantity") });

        if ((creating && !values.ContainsKey("unit_price")) || (values.TryGetValue("unit_price", out var checkUnitPrice) && checkUnitPrice is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("unit_price") });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Platform",
       "OrderStatus",
       "Customer",
       "CustomerOrder",
       "OrderItem"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Platform"] = new PlatformChecker(),
       ["OrderStatus"] = new OrderStatusChecker(),
       ["Customer"] = new CustomerChecker(),
       ["CustomerOrder"] = new CustomerOrderChecker(),
       ["OrderItem"] = new OrderItemChecker()
    }, new Dictionary<string, Record>
    {
       ["Platform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["OrderStatus"] = new Record {
            ["platform"] = new Value.I64Value(0),
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["code"] = new Value.TextValue(""),
            ["color"] = new Value.TextValue(""),
            ["display_order"] = new Value.I64Value(0),
            ["version"] = new Value.I64Value(0)
        },
       ["Customer"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["platform"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["CustomerOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["customer"] = new Value.I64Value(0),
            ["status"] = new Value.I64Value(0),
            ["order_number"] = new Value.TextValue(""),
            ["order_date"] = new Value.DateValue(DateTime.UnixEpoch),
            ["total_amount"] = new Value.DecimalValue(0),
            ["version"] = new Value.I64Value(0)
        },
       ["OrderItem"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["customer_order"] = new Value.I64Value(0),
            ["product_name"] = new Value.TextValue(""),
            ["quantity"] = new Value.I64Value(0),
            ["unit_price"] = new Value.DecimalValue(0),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Platform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["OrderStatus"] = new Dictionary<string, bool> {
           ["platform"] = true,
           ["id"] = true,
           ["name"] = true,
           ["code"] = true,
           ["color"] = true,
           ["display_order"] = true,
           ["version"] = true
        },
       ["Customer"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["platform"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["CustomerOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["customer"] = true,
           ["status"] = true,
           ["order_number"] = true,
           ["order_date"] = true,
           ["total_amount"] = true,
           ["version"] = true
        },
       ["OrderItem"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["customer_order"] = true,
           ["product_name"] = true,
           ["quantity"] = true,
           ["unit_price"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Platform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("OrderStatusList", "OrderStatus").LocalKey("id").ForeignKey("platform").Many(),
            RelationDescriptor.New("CustomerList", "Customer").LocalKey("id").ForeignKey("platform").Many()
        },
       ["OrderStatus"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Platform", "Platform").LocalKey("platform").ForeignKey("id"),
            RelationDescriptor.New("CustomerOrderList", "CustomerOrder").LocalKey("id").ForeignKey("status").Many()
        },
       ["Customer"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Platform", "Platform").LocalKey("platform").ForeignKey("id"),
            RelationDescriptor.New("CustomerOrderList", "CustomerOrder").LocalKey("id").ForeignKey("customer").Many()
        },
       ["CustomerOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Customer", "Customer").LocalKey("customer").ForeignKey("id"),
            RelationDescriptor.New("Status", "OrderStatus").LocalKey("status").ForeignKey("id"),
            RelationDescriptor.New("OrderItemList", "OrderItem").LocalKey("id").ForeignKey("customer_order").Many()
        },
       ["OrderItem"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerOrder", "CustomerOrder").LocalKey("customer_order").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Platform"] = "platform_data",
       ["OrderStatus"] = "order_status_data",
       ["Customer"] = "customer_data",
       ["CustomerOrder"] = "customer_order_data",
       ["OrderItem"] = "order_item_data"
    }).WireEntity(WireFields.CreateMetadata("Platform", ["id", "name", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("OrderStatus", ["platform", "id", "name", "code", "color", "display_order", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["platform"] = ["platform"], ["id"] = ["id"], ["name"] = ["name"], ["code"] = ["code"], ["color"] = ["color"], ["display_order"] = ["display_order"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Customer", ["id", "platform", "name", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["platform"] = ["platform"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("CustomerOrder", ["id", "customer", "status", "order_number", "order_date", "total_amount", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["customer"] = ["customer"], ["status"] = ["status"], ["order_number"] = ["order_number"], ["order_date"] = ["order_date"], ["total_amount"] = ["total_amount"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("OrderItem", ["id", "customer_order", "product_name", "quantity", "unit_price", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["customer_order"] = ["customer_order"], ["product_name"] = ["product_name"], ["quantity"] = ["quantity"], ["unit_price"] = ["unit_price"], ["version"] = ["version"] })).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            created.UpdateName("Commerce Test Platform");
            try { domainRoot = await created.AuditAs("create generated Domain Root Platform").SaveAsync(context); }
            catch { domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        var constantOrderStatus1001 = await Q.OrderStatuses().WithIdIs(1001).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantOrderStatus1001 == null)
        {
            var created = Q.OrderStatuses().Comment("what: create generated constant").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1001);
            created.UpdatePlatform(domainRoot.Id);
            created.UpdateName("New");
            created.UpdateCode("NEW");
            created.UpdateColor("#94A3B8");
            created.UpdateDisplayOrder(1L);
            try { constantOrderStatus1001 = await created.AuditAs("create model constant OrderStatus(1001)").SaveAsync(context); }
            catch { constantOrderStatus1001 = await Q.OrderStatuses().WithIdIs(1001).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantOrderStatus1001 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantOrderStatus1001.Platform, domainRoot.Id))
            {
                constantOrderStatus1001.UpdatePlatform(domainRoot.Id);
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1001.Name, "New"))
            {
                constantOrderStatus1001.UpdateName("New");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1001.Code, "NEW"))
            {
                constantOrderStatus1001.UpdateCode("NEW");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1001.Color, "#94A3B8"))
            {
                constantOrderStatus1001.UpdateColor("#94A3B8");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1001.DisplayOrder, 1L))
            {
                constantOrderStatus1001.UpdateDisplayOrder(1L);
                changed = true;
            }
            if (changed) constantOrderStatus1001 = await constantOrderStatus1001.AuditAs("reconcile model constant OrderStatus(1001)").SaveAsync(context);
        }
        var constantOrderStatus1002 = await Q.OrderStatuses().WithIdIs(1002).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantOrderStatus1002 == null)
        {
            var created = Q.OrderStatuses().Comment("what: create generated constant").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1002);
            created.UpdatePlatform(domainRoot.Id);
            created.UpdateName("Paid");
            created.UpdateCode("PAID");
            created.UpdateColor("#22C55E");
            created.UpdateDisplayOrder(2L);
            try { constantOrderStatus1002 = await created.AuditAs("create model constant OrderStatus(1002)").SaveAsync(context); }
            catch { constantOrderStatus1002 = await Q.OrderStatuses().WithIdIs(1002).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantOrderStatus1002 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantOrderStatus1002.Platform, domainRoot.Id))
            {
                constantOrderStatus1002.UpdatePlatform(domainRoot.Id);
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1002.Name, "Paid"))
            {
                constantOrderStatus1002.UpdateName("Paid");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1002.Code, "PAID"))
            {
                constantOrderStatus1002.UpdateCode("PAID");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1002.Color, "#22C55E"))
            {
                constantOrderStatus1002.UpdateColor("#22C55E");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1002.DisplayOrder, 2L))
            {
                constantOrderStatus1002.UpdateDisplayOrder(2L);
                changed = true;
            }
            if (changed) constantOrderStatus1002 = await constantOrderStatus1002.AuditAs("reconcile model constant OrderStatus(1002)").SaveAsync(context);
        }
        var constantOrderStatus1003 = await Q.OrderStatuses().WithIdIs(1003).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantOrderStatus1003 == null)
        {
            var created = Q.OrderStatuses().Comment("what: create generated constant").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1003);
            created.UpdatePlatform(domainRoot.Id);
            created.UpdateName("Cancelled");
            created.UpdateCode("CANCELLED");
            created.UpdateColor("#EF4444");
            created.UpdateDisplayOrder(3L);
            try { constantOrderStatus1003 = await created.AuditAs("create model constant OrderStatus(1003)").SaveAsync(context); }
            catch { constantOrderStatus1003 = await Q.OrderStatuses().WithIdIs(1003).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantOrderStatus1003 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantOrderStatus1003.Platform, domainRoot.Id))
            {
                constantOrderStatus1003.UpdatePlatform(domainRoot.Id);
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1003.Name, "Cancelled"))
            {
                constantOrderStatus1003.UpdateName("Cancelled");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1003.Code, "CANCELLED"))
            {
                constantOrderStatus1003.UpdateCode("CANCELLED");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1003.Color, "#EF4444"))
            {
                constantOrderStatus1003.UpdateColor("#EF4444");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1003.DisplayOrder, 3L))
            {
                constantOrderStatus1003.UpdateDisplayOrder(3L);
                changed = true;
            }
            if (changed) constantOrderStatus1003 = await constantOrderStatus1003.AuditAs("reconcile model constant OrderStatus(1003)").SaveAsync(context);
        }
        }
        context.WithActiveRoot("Platform", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Platform")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("OrderStatus")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("Customer")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("CustomerOrder")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("OrderItem")!.AuditMaskFields(new List<string> {  }));
    }
}