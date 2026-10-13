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
            ["Platform"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["orderStatusList"] = "OrderStatus",
                        ["customerList"] = "Customer",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["OrderStatus"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["platform"] = "integer",
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["code"] = "string",
                       ["color"] = "string",
                       ["display_order"] = "integer",
                       ["displayOrder"] = "integer",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["platform"] = "Platform",
                        ["customerOrderList"] = "CustomerOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["platform"] = "platform",
                   ["id"] = "id",
                   ["name"] = "name",
                   ["code"] = "code",
                   ["color"] = "color",
                   ["display_order"] = "display_order",
                   ["displayOrder"] = "display_order",
                   ["version"] = "version",
                })),
            ["Customer"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["platform"] = "integer",
                       ["name"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["platform"] = "Platform",
                        ["customerOrderList"] = "CustomerOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["platform"] = "platform",
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["CustomerOrder"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["customer"] = "integer",
                       ["status"] = "integer",
                       ["order_number"] = "string",
                       ["orderNumber"] = "string",
                       ["order_date"] = "date",
                       ["orderDate"] = "date",
                       ["total_amount"] = "decimal",
                       ["totalAmount"] = "decimal",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["customer"] = "Customer",
                       ["status"] = "OrderStatus",
                        ["orderItemList"] = "OrderItem",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["customer"] = "customer",
                   ["status"] = "status",
                   ["order_number"] = "order_number",
                   ["orderNumber"] = "order_number",
                   ["order_date"] = "order_date",
                   ["orderDate"] = "order_date",
                   ["total_amount"] = "total_amount",
                   ["totalAmount"] = "total_amount",
                   ["version"] = "version",
                })),
            ["OrderItem"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["customer_order"] = "integer",
                       ["customerOrder"] = "integer",
                       ["product_name"] = "string",
                       ["productName"] = "string",
                       ["quantity"] = "integer",
                       ["unit_price"] = "decimal",
                       ["unitPrice"] = "decimal",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["customer_order"] = "CustomerOrder",
                       ["customerOrder"] = "CustomerOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["customer_order"] = "customer_order",
                   ["customerOrder"] = "customer_order",
                   ["product_name"] = "product_name",
                   ["productName"] = "product_name",
                   ["quantity"] = "quantity",
                   ["unit_price"] = "unit_price",
                   ["unitPrice"] = "unit_price",
                   ["version"] = "version",
                })),
        });
}