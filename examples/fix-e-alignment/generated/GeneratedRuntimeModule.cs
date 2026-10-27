using TeaQL.Core;

namespace Generated;

internal sealed class TenantChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var minLenName) && minLenName.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult { RuleId = "min_length", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 80) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });


        return results;
    }
}

internal sealed class PurchaseOrderChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if (creating) {
            var root = context.RequireActiveRoot("Tenant");
            if (!values.TryGetValue("tenant", out var suppliedTenant) || suppliedTenant is Value.NullValue) {
                values["tenant"] = new Value.I64Value(root.Id);
                context.RecordFixEvidence(new FixEvidence("PurchaseOrder", "tenant", FixEvidenceSource.Context, "activeRoot:Tenant"));
            }
            else if (Convert.ToInt64(suppliedTenant.Raw) != root.Id) results.Add(new CheckResult { RuleId = "context_root_mismatch", Location = ObjectLocation.Property("tenant") });
        } else if (updating && values.TryGetValue("tenant", out var suppliedUpdateTenant) && suppliedUpdateTenant is not Value.NullValue) {
            var root = context.RequireActiveRoot("Tenant");
            if (Convert.ToInt64(suppliedUpdateTenant.Raw) != root.Id) results.Add(new CheckResult { RuleId = "context_root_mismatch", Location = ObjectLocation.Property("tenant") });
        }



        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("PurchaseOrder", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("PurchaseOrder", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("PurchaseOrder", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("tenant")) || (values.TryGetValue("tenant", out var checkTenant) && checkTenant is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("tenant") });

        if ((creating && !values.ContainsKey("contact_phone")) || (values.TryGetValue("contact_phone", out var checkContactPhone) && checkContactPhone is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("contact_phone") });
        if (values.TryGetValue("contact_phone", out var minLenContactPhone) && minLenContactPhone.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult { RuleId = "min_length", Location = ObjectLocation.Property("contact_phone") });
        if (values.TryGetValue("contact_phone", out var maxLenContactPhone) && maxLenContactPhone.Raw?.ToString()?.Length > 32) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("contact_phone") });

        if (values.TryGetValue("internal_note", out var maxLenInternalNote) && maxLenInternalNote.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("internal_note") });

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("create_time") });

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("update_time") });


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
        if (creating) {
            var root = context.RequireActiveRoot("Tenant");
            if (!values.TryGetValue("tenant", out var suppliedTenant) || suppliedTenant is Value.NullValue) {
                values["tenant"] = new Value.I64Value(root.Id);
                context.RecordFixEvidence(new FixEvidence("OrderItem", "tenant", FixEvidenceSource.Context, "activeRoot:Tenant"));
            }
            else if (Convert.ToInt64(suppliedTenant.Raw) != root.Id) results.Add(new CheckResult { RuleId = "context_root_mismatch", Location = ObjectLocation.Property("tenant") });
        } else if (updating && values.TryGetValue("tenant", out var suppliedUpdateTenant) && suppliedUpdateTenant is not Value.NullValue) {
            var root = context.RequireActiveRoot("Tenant");
            if (Convert.ToInt64(suppliedUpdateTenant.Raw) != root.Id) results.Add(new CheckResult { RuleId = "context_root_mismatch", Location = ObjectLocation.Property("tenant") });
        }



        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("OrderItem", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("OrderItem", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("OrderItem", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("purchase_order")) || (values.TryGetValue("purchase_order", out var checkPurchaseOrder) && checkPurchaseOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("purchase_order") });

        if ((creating && !values.ContainsKey("tenant")) || (values.TryGetValue("tenant", out var checkTenant) && checkTenant is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("tenant") });

        if ((creating && !values.ContainsKey("product_name")) || (values.TryGetValue("product_name", out var checkProductName) && checkProductName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("product_name") });
        if (values.TryGetValue("product_name", out var minLenProductName) && minLenProductName.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult { RuleId = "min_length", Location = ObjectLocation.Property("product_name") });
        if (values.TryGetValue("product_name", out var maxLenProductName) && maxLenProductName.Raw?.ToString()?.Length > 80) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("product_name") });

        if ((creating && !values.ContainsKey("unit_price")) || (values.TryGetValue("unit_price", out var checkUnitPrice) && checkUnitPrice is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("unit_price") });
        if (values.TryGetValue("unit_price", out var minUnitPrice) && minUnitPrice is not Value.NullValue && Convert.ToDecimal(minUnitPrice.Raw).CompareTo(0.01m) == -1) results.Add(new CheckResult { RuleId = "min", Location = ObjectLocation.Property("unit_price") });
        if (values.TryGetValue("unit_price", out var maxUnitPrice) && maxUnitPrice is not Value.NullValue && Convert.ToDecimal(maxUnitPrice.Raw) > 1000000m) results.Add(new CheckResult { RuleId = "max", Location = ObjectLocation.Property("unit_price") });

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("create_time") });

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("update_time") });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Tenant",
       "PurchaseOrder",
       "OrderItem"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Tenant"] = new TenantChecker(),
       ["PurchaseOrder"] = new PurchaseOrderChecker(),
       ["OrderItem"] = new OrderItemChecker()
    }, new Dictionary<string, Record>
    {
       ["Tenant"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["PurchaseOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["tenant"] = new Value.I64Value(0),
            ["contact_phone"] = new Value.TextValue(""),
            ["internal_note"] = new Value.TextValue(""),
            ["create_time"] = new Value.TimestampValue(0),
            ["update_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        },
       ["OrderItem"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["purchase_order"] = new Value.I64Value(0),
            ["tenant"] = new Value.I64Value(0),
            ["product_name"] = new Value.TextValue(""),
            ["unit_price"] = new Value.DecimalValue(0),
            ["create_time"] = new Value.TimestampValue(0),
            ["update_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Tenant"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["version"] = true
        },
       ["PurchaseOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["tenant"] = true,
           ["contact_phone"] = true,
           ["internal_note"] = false,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        },
       ["OrderItem"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["purchase_order"] = true,
           ["tenant"] = true,
           ["product_name"] = true,
           ["unit_price"] = true,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Tenant"] = new List<RelationDescriptor> {
            RelationDescriptor.New("PurchaseOrderList", "PurchaseOrder").LocalKey("id").ForeignKey("tenant").Many(),
            RelationDescriptor.New("OrderItemList", "OrderItem").LocalKey("id").ForeignKey("tenant").Many()
        },
       ["PurchaseOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Tenant", "Tenant").LocalKey("tenant").ForeignKey("id"),
            RelationDescriptor.New("OrderItemList", "OrderItem").LocalKey("id").ForeignKey("purchase_order").Many()
        },
       ["OrderItem"] = new List<RelationDescriptor> {
            RelationDescriptor.New("PurchaseOrder", "PurchaseOrder").LocalKey("purchase_order").ForeignKey("id"),
            RelationDescriptor.New("Tenant", "Tenant").LocalKey("tenant").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Tenant"] = "tenant_data",
       ["PurchaseOrder"] = "purchase_order_data",
       ["OrderItem"] = "order_item_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Tenant"] = new List<string> {
        },
       ["PurchaseOrder"] = new List<string> {
        },
       ["OrderItem"] = new List<string> {
        }
    }).WireEntity(WireFields.CreateMetadata("Tenant", ["id", "name", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("PurchaseOrder", ["id", "tenant", "contact_phone", "internal_note", "create_time", "update_time", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["tenant"] = ["tenant"], ["contact_phone"] = ["contact_phone"], ["internal_note"] = ["internal_note"], ["create_time"] = ["create_time"], ["update_time"] = ["update_time"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("OrderItem", ["id", "purchase_order", "tenant", "product_name", "unit_price", "create_time", "update_time", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["purchase_order"] = ["purchase_order"], ["tenant"] = ["tenant"], ["product_name"] = ["product_name"], ["unit_price"] = ["unit_price"], ["create_time"] = ["create_time"], ["update_time"] = ["update_time"], ["version"] = ["version"] })).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
            using var bootstrapScope = context.EnterGeneratedBootstrap("Tenant", 1);
        var domainRoot = await Q.Tenants().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Q.Tenants().Comment("what: create generated Domain Root").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1);
            created.UpdateName("Tenant Name");
            try { domainRoot = await created.AuditAs("create generated Domain Root Tenant").SaveAsync(context); }
            catch { domainRoot = await Q.Tenants().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        }
        context.WithActiveRoot("Tenant", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Tenant")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("PurchaseOrder")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("OrderItem")!.AuditMaskFields(new List<string> {  }));
    }
}