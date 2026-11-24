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
            ["Tenant"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["purchaseOrderList"] = "PurchaseOrder",
                        ["orderItemList"] = "OrderItem",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["PurchaseOrder"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["tenant"] = "integer",
                       ["contact_phone"] = "string",
                       ["contactPhone"] = "string",
                       ["internal_note"] = "string",
                       ["internalNote"] = "string",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["tenant"] = "Tenant",
                        ["orderItemList"] = "OrderItem",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["tenant"] = "tenant",
                   ["contact_phone"] = "contact_phone",
                   ["contactPhone"] = "contact_phone",
                   ["internal_note"] = "internal_note",
                   ["internalNote"] = "internal_note",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
            ["OrderItem"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["purchase_order"] = "integer",
                       ["purchaseOrder"] = "integer",
                       ["tenant"] = "integer",
                       ["product_name"] = "string",
                       ["productName"] = "string",
                       ["unit_price"] = "decimal",
                       ["unitPrice"] = "decimal",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["purchase_order"] = "PurchaseOrder",
                       ["purchaseOrder"] = "PurchaseOrder",
                       ["tenant"] = "Tenant",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["purchase_order"] = "purchase_order",
                   ["purchaseOrder"] = "purchase_order",
                   ["tenant"] = "tenant",
                   ["product_name"] = "product_name",
                   ["productName"] = "product_name",
                   ["unit_price"] = "unit_price",
                   ["unitPrice"] = "unit_price",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
        });
}