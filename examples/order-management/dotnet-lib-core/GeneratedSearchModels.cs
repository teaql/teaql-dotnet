using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TeaQL.Core;

namespace Generated;

// Generated naming/type metadata. Execution and authorization remain in the runtime.
internal static class GeneratedSearchModels
{
    internal static readonly IReadOnlyDictionary<string, GeneratedSearchModel> Models =
        new ReadOnlyDictionary<string, GeneratedSearchModel>(new Dictionary<string, GeneratedSearchModel>(StringComparer.Ordinal)
        {
            ["CommercePlatform"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["customerList"] = "Customer",
                        ["orderStatusList"] = "OrderStatus",
                        ["customerOrderList"] = "CustomerOrder",
                        ["productList"] = "Product",
                        ["orderLineList"] = "OrderLine",
                        ["orderSearchPresetList"] = "OrderSearchPreset",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
            ["Customer"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["email"] = "string",
                       ["commerce_platform"] = "integer",
                       ["commercePlatform"] = "integer",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["commerce_platform"] = "CommercePlatform",
                       ["commercePlatform"] = "CommercePlatform",
                        ["customerOrderList"] = "CustomerOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["email"] = "email",
                   ["commerce_platform"] = "commerce_platform",
                   ["commercePlatform"] = "commerce_platform",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
            ["OrderStatus"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["code"] = "string",
                       ["color"] = "string",
                       ["display_order"] = "decimal",
                       ["displayOrder"] = "decimal",
                       ["commerce_platform"] = "integer",
                       ["commercePlatform"] = "integer",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["commerce_platform"] = "CommercePlatform",
                       ["commercePlatform"] = "CommercePlatform",
                        ["customerOrderList"] = "CustomerOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["code"] = "code",
                   ["color"] = "color",
                   ["display_order"] = "display_order",
                   ["displayOrder"] = "display_order",
                   ["commerce_platform"] = "commerce_platform",
                   ["commercePlatform"] = "commerce_platform",
                   ["version"] = "version",
                })),
            ["CustomerOrder"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["order_number"] = "string",
                       ["orderNumber"] = "string",
                       ["order_date"] = "date",
                       ["orderDate"] = "date",
                       ["total_amount"] = "decimal",
                       ["totalAmount"] = "decimal",
                       ["status"] = "integer",
                       ["customer"] = "integer",
                       ["commerce_platform"] = "integer",
                       ["commercePlatform"] = "integer",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["status"] = "OrderStatus",
                       ["customer"] = "Customer",
                       ["commerce_platform"] = "CommercePlatform",
                       ["commercePlatform"] = "CommercePlatform",
                        ["orderLineList"] = "OrderLine",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["order_number"] = "order_number",
                   ["orderNumber"] = "order_number",
                   ["order_date"] = "order_date",
                   ["orderDate"] = "order_date",
                   ["total_amount"] = "total_amount",
                   ["totalAmount"] = "total_amount",
                   ["status"] = "status",
                   ["customer"] = "customer",
                   ["commerce_platform"] = "commerce_platform",
                   ["commercePlatform"] = "commerce_platform",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
            ["Product"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["sku"] = "string",
                       ["image_url"] = "string",
                       ["imageUrl"] = "string",
                       ["commerce_platform"] = "integer",
                       ["commercePlatform"] = "integer",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["commerce_platform"] = "CommercePlatform",
                       ["commercePlatform"] = "CommercePlatform",
                        ["orderLineList"] = "OrderLine",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["sku"] = "sku",
                   ["image_url"] = "image_url",
                   ["imageUrl"] = "image_url",
                   ["commerce_platform"] = "commerce_platform",
                   ["commercePlatform"] = "commerce_platform",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
            ["OrderLine"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["customer_order"] = "integer",
                       ["customerOrder"] = "integer",
                       ["product"] = "integer",
                       ["product_name"] = "string",
                       ["productName"] = "string",
                       ["sku"] = "string",
                       ["quantity"] = "integer",
                       ["commerce_platform"] = "integer",
                       ["commercePlatform"] = "integer",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["customer_order"] = "CustomerOrder",
                       ["customerOrder"] = "CustomerOrder",
                       ["product"] = "Product",
                       ["commerce_platform"] = "CommercePlatform",
                       ["commercePlatform"] = "CommercePlatform",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["customer_order"] = "customer_order",
                   ["customerOrder"] = "customer_order",
                   ["product"] = "product",
                   ["product_name"] = "product_name",
                   ["productName"] = "product_name",
                   ["sku"] = "sku",
                   ["quantity"] = "quantity",
                   ["commerce_platform"] = "commerce_platform",
                   ["commercePlatform"] = "commerce_platform",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["version"] = "version",
                })),
            ["OrderSearchPreset"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["filter_json"] = "string",
                       ["filterJson"] = "string",
                       ["request_id"] = "string",
                       ["requestId"] = "string",
                       ["owner_user_id"] = "string",
                       ["ownerUserId"] = "string",
                       ["commerce_platform"] = "integer",
                       ["commercePlatform"] = "integer",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["commerce_platform"] = "CommercePlatform",
                       ["commercePlatform"] = "CommercePlatform",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["filter_json"] = "filter_json",
                   ["filterJson"] = "filter_json",
                   ["request_id"] = "request_id",
                   ["requestId"] = "request_id",
                   ["owner_user_id"] = "owner_user_id",
                   ["ownerUserId"] = "owner_user_id",
                   ["commerce_platform"] = "commerce_platform",
                   ["commercePlatform"] = "commerce_platform",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
        });
}