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
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("CommercePlatform", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("CommercePlatform", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("CommercePlatform", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("create_time") });

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("update_time") });


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
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("Customer", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("Customer", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("Customer", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });

        if ((creating && !values.ContainsKey("email")) || (values.TryGetValue("email", out var checkEmail) && checkEmail is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("email") });
        if (values.TryGetValue("email", out var maxLenEmail) && maxLenEmail.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("email") });

        if ((creating && !values.ContainsKey("commerce_platform")) || (values.TryGetValue("commerce_platform", out var checkCommercePlatform) && checkCommercePlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("commerce_platform") });

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("create_time") });

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("update_time") });


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
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });

        if ((creating && !values.ContainsKey("code")) || (values.TryGetValue("code", out var checkCode) && checkCode is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("code") });
        if (values.TryGetValue("code", out var maxLenCode) && maxLenCode.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("code") });

        if (values.TryGetValue("color", out var maxLenColor) && maxLenColor.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("color") });


        if ((creating && !values.ContainsKey("commerce_platform")) || (values.TryGetValue("commerce_platform", out var checkCommercePlatform) && checkCommercePlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("commerce_platform") });


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
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("CustomerOrder", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("CustomerOrder", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("CustomerOrder", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("order_number")) || (values.TryGetValue("order_number", out var checkOrderNumber) && checkOrderNumber is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("order_number") });
        if (values.TryGetValue("order_number", out var maxLenOrderNumber) && maxLenOrderNumber.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("order_number") });

        if ((creating && !values.ContainsKey("order_date")) || (values.TryGetValue("order_date", out var checkOrderDate) && checkOrderDate is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("order_date") });

        if ((creating && !values.ContainsKey("total_amount")) || (values.TryGetValue("total_amount", out var checkTotalAmount) && checkTotalAmount is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("total_amount") });

        if ((creating && !values.ContainsKey("status")) || (values.TryGetValue("status", out var checkStatus) && checkStatus is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("status") });

        if ((creating && !values.ContainsKey("customer")) || (values.TryGetValue("customer", out var checkCustomer) && checkCustomer is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("customer") });

        if ((creating && !values.ContainsKey("commerce_platform")) || (values.TryGetValue("commerce_platform", out var checkCommercePlatform) && checkCommercePlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("commerce_platform") });

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("create_time") });

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("update_time") });


        return results;
    }
}

internal sealed class ProductChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("Product", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("Product", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("Product", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });

        if ((creating && !values.ContainsKey("sku")) || (values.TryGetValue("sku", out var checkSku) && checkSku is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("sku") });
        if (values.TryGetValue("sku", out var maxLenSku) && maxLenSku.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("sku") });

        if (values.TryGetValue("image_url", out var maxLenImageUrl) && maxLenImageUrl.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("image_url") });

        if ((creating && !values.ContainsKey("commerce_platform")) || (values.TryGetValue("commerce_platform", out var checkCommercePlatform) && checkCommercePlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("commerce_platform") });

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("create_time") });

        if ((creating && !values.ContainsKey("update_time")) || (values.TryGetValue("update_time", out var checkUpdateTime) && checkUpdateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("update_time") });


        return results;
    }
}

internal sealed class OrderLineChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("OrderLine", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("customer_order")) || (values.TryGetValue("customer_order", out var checkCustomerOrder) && checkCustomerOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("customer_order") });

        if ((creating && !values.ContainsKey("product")) || (values.TryGetValue("product", out var checkProduct) && checkProduct is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("product") });

        if ((creating && !values.ContainsKey("product_name")) || (values.TryGetValue("product_name", out var checkProductName) && checkProductName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("product_name") });
        if (values.TryGetValue("product_name", out var maxLenProductName) && maxLenProductName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("product_name") });

        if ((creating && !values.ContainsKey("sku")) || (values.TryGetValue("sku", out var checkSku) && checkSku is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("sku") });
        if (values.TryGetValue("sku", out var maxLenSku) && maxLenSku.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("sku") });

        if ((creating && !values.ContainsKey("quantity")) || (values.TryGetValue("quantity", out var checkQuantity) && checkQuantity is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("quantity") });

        if ((creating && !values.ContainsKey("commerce_platform")) || (values.TryGetValue("commerce_platform", out var checkCommercePlatform) && checkCommercePlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("commerce_platform") });

        if ((creating && !values.ContainsKey("create_time")) || (values.TryGetValue("create_time", out var checkCreateTime) && checkCreateTime is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("create_time") });


        return results;
    }
}

internal sealed class OrderSearchPresetChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if (creating && (!values.TryGetValue("create_time", out var createCreateTime) || createCreateTime is Value.NullValue)) {
            values["create_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("OrderSearchPreset", "create_time", FixEvidenceSource.Clock, "graphClock"));
        }

        if (creating && (!values.TryGetValue("update_time", out var createUpdateTime) || createUpdateTime is Value.NullValue)) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("OrderSearchPreset", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }
        if (updating) {
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("OrderSearchPreset", "update_time", FixEvidenceSource.Clock, "graphClock"));
        }


        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("name") });

        if ((creating && !values.ContainsKey("filter_json")) || (values.TryGetValue("filter_json", out var checkFilterJson) && checkFilterJson is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("filter_json") });
        if (values.TryGetValue("filter_json", out var maxLenFilterJson) && maxLenFilterJson.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("filter_json") });

        if ((creating && !values.ContainsKey("request_id")) || (values.TryGetValue("request_id", out var checkRequestId) && checkRequestId is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("request_id") });
        if (values.TryGetValue("request_id", out var maxLenRequestId) && maxLenRequestId.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("request_id") });

        if ((creating && !values.ContainsKey("owner_user_id")) || (values.TryGetValue("owner_user_id", out var checkOwnerUserId) && checkOwnerUserId is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("owner_user_id") });
        if (values.TryGetValue("owner_user_id", out var maxLenOwnerUserId) && maxLenOwnerUserId.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", Location = ObjectLocation.Property("owner_user_id") });

        if ((creating && !values.ContainsKey("commerce_platform")) || (values.TryGetValue("commerce_platform", out var checkCommercePlatform) && checkCommercePlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", Location = ObjectLocation.Property("commerce_platform") });

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
       "CommercePlatform",
       "Customer",
       "OrderStatus",
       "CustomerOrder",
       "Product",
       "OrderLine",
       "OrderSearchPreset"
    }, new Dictionary<string, IEntityChecker>
    {
       ["CommercePlatform"] = new CommercePlatformChecker(),
       ["Customer"] = new CustomerChecker(),
       ["OrderStatus"] = new OrderStatusChecker(),
       ["CustomerOrder"] = new CustomerOrderChecker(),
       ["Product"] = new ProductChecker(),
       ["OrderLine"] = new OrderLineChecker(),
       ["OrderSearchPreset"] = new OrderSearchPresetChecker()
    }, new Dictionary<string, Record>
    {
       ["CommercePlatform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["create_time"] = new Value.TimestampValue(0),
            ["update_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        },
       ["Customer"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["email"] = new Value.TextValue(""),
            ["commerce_platform"] = new Value.I64Value(0),
            ["create_time"] = new Value.TimestampValue(0),
            ["update_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        },
       ["OrderStatus"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["code"] = new Value.TextValue(""),
            ["color"] = new Value.TextValue(""),
            ["display_order"] = new Value.DecimalValue(0),
            ["commerce_platform"] = new Value.I64Value(0),
            ["version"] = new Value.I64Value(0)
        },
       ["CustomerOrder"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["order_number"] = new Value.TextValue(""),
            ["order_date"] = new Value.DateValue(DateTime.UnixEpoch),
            ["total_amount"] = new Value.DecimalValue(0),
            ["status"] = new Value.I64Value(0),
            ["customer"] = new Value.I64Value(0),
            ["commerce_platform"] = new Value.I64Value(0),
            ["create_time"] = new Value.TimestampValue(0),
            ["update_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        },
       ["Product"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["sku"] = new Value.TextValue(""),
            ["image_url"] = new Value.TextValue(""),
            ["commerce_platform"] = new Value.I64Value(0),
            ["create_time"] = new Value.TimestampValue(0),
            ["update_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        },
       ["OrderLine"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["customer_order"] = new Value.I64Value(0),
            ["product"] = new Value.I64Value(0),
            ["product_name"] = new Value.TextValue(""),
            ["sku"] = new Value.TextValue(""),
            ["quantity"] = new Value.I64Value(0),
            ["commerce_platform"] = new Value.I64Value(0),
            ["create_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        },
       ["OrderSearchPreset"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["filter_json"] = new Value.TextValue(""),
            ["request_id"] = new Value.TextValue(""),
            ["owner_user_id"] = new Value.TextValue(""),
            ["commerce_platform"] = new Value.I64Value(0),
            ["create_time"] = new Value.TimestampValue(0),
            ["update_time"] = new Value.TimestampValue(0),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["CommercePlatform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        },
       ["Customer"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["email"] = true,
           ["commerce_platform"] = true,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        },
       ["OrderStatus"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["code"] = true,
           ["color"] = false,
           ["display_order"] = false,
           ["commerce_platform"] = true,
           ["version"] = true
        },
       ["CustomerOrder"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["order_number"] = true,
           ["order_date"] = true,
           ["total_amount"] = true,
           ["status"] = true,
           ["customer"] = true,
           ["commerce_platform"] = true,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        },
       ["Product"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["sku"] = true,
           ["image_url"] = false,
           ["commerce_platform"] = true,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        },
       ["OrderLine"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["customer_order"] = true,
           ["product"] = true,
           ["product_name"] = true,
           ["sku"] = true,
           ["quantity"] = true,
           ["commerce_platform"] = true,
           ["create_time"] = true,
           ["version"] = true
        },
       ["OrderSearchPreset"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["filter_json"] = true,
           ["request_id"] = true,
           ["owner_user_id"] = true,
           ["commerce_platform"] = true,
           ["create_time"] = true,
           ["update_time"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["CommercePlatform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerList", "Customer").LocalKey("id").ForeignKey("commerce_platform").Many(),
            RelationDescriptor.New("OrderStatusList", "OrderStatus").LocalKey("id").ForeignKey("commerce_platform").Many(),
            RelationDescriptor.New("CustomerOrderList", "CustomerOrder").LocalKey("id").ForeignKey("commerce_platform").Many(),
            RelationDescriptor.New("ProductList", "Product").LocalKey("id").ForeignKey("commerce_platform").Many(),
            RelationDescriptor.New("OrderLineList", "OrderLine").LocalKey("id").ForeignKey("commerce_platform").Many(),
            RelationDescriptor.New("OrderSearchPresetList", "OrderSearchPreset").LocalKey("id").ForeignKey("commerce_platform").Many()
        },
       ["Customer"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CommercePlatform", "CommercePlatform").LocalKey("commerce_platform").ForeignKey("id"),
            RelationDescriptor.New("CustomerOrderList", "CustomerOrder").LocalKey("id").ForeignKey("customer").Many()
        },
       ["OrderStatus"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CommercePlatform", "CommercePlatform").LocalKey("commerce_platform").ForeignKey("id"),
            RelationDescriptor.New("CustomerOrderList", "CustomerOrder").LocalKey("id").ForeignKey("status").Many()
        },
       ["CustomerOrder"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Status", "OrderStatus").LocalKey("status").ForeignKey("id"),
            RelationDescriptor.New("Customer", "Customer").LocalKey("customer").ForeignKey("id"),
            RelationDescriptor.New("CommercePlatform", "CommercePlatform").LocalKey("commerce_platform").ForeignKey("id"),
            RelationDescriptor.New("OrderLineList", "OrderLine").LocalKey("id").ForeignKey("customer_order").Many()
        },
       ["Product"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CommercePlatform", "CommercePlatform").LocalKey("commerce_platform").ForeignKey("id"),
            RelationDescriptor.New("OrderLineList", "OrderLine").LocalKey("id").ForeignKey("product").Many()
        },
       ["OrderLine"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CustomerOrder", "CustomerOrder").LocalKey("customer_order").ForeignKey("id"),
            RelationDescriptor.New("Product", "Product").LocalKey("product").ForeignKey("id"),
            RelationDescriptor.New("CommercePlatform", "CommercePlatform").LocalKey("commerce_platform").ForeignKey("id")
        },
       ["OrderSearchPreset"] = new List<RelationDescriptor> {
            RelationDescriptor.New("CommercePlatform", "CommercePlatform").LocalKey("commerce_platform").ForeignKey("id")
        }
    }).WireEntity(WireFields.CreateMetadata("CommercePlatform", ["id", "name", "create_time", "update_time", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["create_time"] = ["create_time"], ["update_time"] = ["update_time"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Customer", ["id", "name", "email", "commerce_platform", "create_time", "update_time", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["email"] = ["email"], ["commerce_platform"] = ["commerce_platform"], ["create_time"] = ["create_time"], ["update_time"] = ["update_time"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("OrderStatus", ["id", "name", "code", "color", "display_order", "commerce_platform", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["code"] = ["code"], ["color"] = ["color"], ["display_order"] = ["display_order"], ["commerce_platform"] = ["commerce_platform"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("CustomerOrder", ["id", "order_number", "order_date", "total_amount", "status", "customer", "commerce_platform", "create_time", "update_time", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["order_number"] = ["order_number"], ["order_date"] = ["order_date"], ["total_amount"] = ["total_amount"], ["status"] = ["status"], ["customer"] = ["customer"], ["commerce_platform"] = ["commerce_platform"], ["create_time"] = ["create_time"], ["update_time"] = ["update_time"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Product", ["id", "name", "sku", "image_url", "commerce_platform", "create_time", "update_time", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["sku"] = ["sku"], ["image_url"] = ["image_url"], ["commerce_platform"] = ["commerce_platform"], ["create_time"] = ["create_time"], ["update_time"] = ["update_time"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("OrderLine", ["id", "customer_order", "product", "product_name", "sku", "quantity", "commerce_platform", "create_time", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["customer_order"] = ["customer_order"], ["product"] = ["product"], ["product_name"] = ["product_name"], ["sku"] = ["sku"], ["quantity"] = ["quantity"], ["commerce_platform"] = ["commerce_platform"], ["create_time"] = ["create_time"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("OrderSearchPreset", ["id", "name", "filter_json", "request_id", "owner_user_id", "commerce_platform", "create_time", "update_time", "version"], aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["filter_json"] = ["filter_json"], ["request_id"] = ["request_id"], ["owner_user_id"] = ["owner_user_id"], ["commerce_platform"] = ["commerce_platform"], ["create_time"] = ["create_time"], ["update_time"] = ["update_time"], ["version"] = ["version"] })).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

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
        using var bootstrapScope = context.EnterGeneratedBootstrap("CommercePlatform", 1);
        var domainRoot = await Q.CommercePlatforms().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Q.CommercePlatforms().Comment("what: create generated Domain Root").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1);
            created.UpdateName("Northwind Demo");
            try { domainRoot = await created.AuditAs("create generated Domain Root CommercePlatform").SaveAsync(context); }
            catch { domainRoot = await Q.CommercePlatforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        var constantOrderStatus1001 = await Q.OrderStatuses().WithIdIs(1001).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantOrderStatus1001 == null)
        {
            var created = Q.OrderStatuses().Comment("what: create generated constant").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1001);
            created.UpdateName("Pending");
            created.UpdateCode("PENDING");
            created.UpdateColor("#F59E0B");
            created.UpdateDisplayOrder(decimal.Parse("1", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdateCommercePlatform(domainRoot.Id);
            try { constantOrderStatus1001 = await created.AuditAs("create model constant OrderStatus(1001)").SaveAsync(context); }
            catch { constantOrderStatus1001 = await Q.OrderStatuses().WithIdIs(1001).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantOrderStatus1001 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantOrderStatus1001.Name, "Pending"))
            {
                constantOrderStatus1001.UpdateName("Pending");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1001.Code, "PENDING"))
            {
                constantOrderStatus1001.UpdateCode("PENDING");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1001.Color, "#F59E0B"))
            {
                constantOrderStatus1001.UpdateColor("#F59E0B");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1001.DisplayOrder, decimal.Parse("1", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantOrderStatus1001.UpdateDisplayOrder(decimal.Parse("1", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1001.CommercePlatform, domainRoot.Id))
            {
                constantOrderStatus1001.UpdateCommercePlatform(domainRoot.Id);
                changed = true;
            }
            if (changed) constantOrderStatus1001 = await constantOrderStatus1001.AuditAs("reconcile model constant OrderStatus(1001)").SaveAsync(context);
        }
        var constantOrderStatus1002 = await Q.OrderStatuses().WithIdIs(1002).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantOrderStatus1002 == null)
        {
            var created = Q.OrderStatuses().Comment("what: create generated constant").Purpose("why: initialize runtime bootstrap").NewEntity(context);
            created.TeaqlInitializeGeneratedBootstrapId(1002);
            created.UpdateName("Confirmed");
            created.UpdateCode("CONFIRMED");
            created.UpdateColor("#10B981");
            created.UpdateDisplayOrder(decimal.Parse("2", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdateCommercePlatform(domainRoot.Id);
            try { constantOrderStatus1002 = await created.AuditAs("create model constant OrderStatus(1002)").SaveAsync(context); }
            catch { constantOrderStatus1002 = await Q.OrderStatuses().WithIdIs(1002).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantOrderStatus1002 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantOrderStatus1002.Name, "Confirmed"))
            {
                constantOrderStatus1002.UpdateName("Confirmed");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1002.Code, "CONFIRMED"))
            {
                constantOrderStatus1002.UpdateCode("CONFIRMED");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1002.Color, "#10B981"))
            {
                constantOrderStatus1002.UpdateColor("#10B981");
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1002.DisplayOrder, decimal.Parse("2", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantOrderStatus1002.UpdateDisplayOrder(decimal.Parse("2", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantOrderStatus1002.CommercePlatform, domainRoot.Id))
            {
                constantOrderStatus1002.UpdateCommercePlatform(domainRoot.Id);
                changed = true;
            }
            if (changed) constantOrderStatus1002 = await constantOrderStatus1002.AuditAs("reconcile model constant OrderStatus(1002)").SaveAsync(context);
        }
    }

}