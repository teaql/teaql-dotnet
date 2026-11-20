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
            ["Warehouse"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["shippingOrderList"] = "ShippingOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["ShippingOrder"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["warehouse"] = "integer",
                       ["reference"] = "string",
                       ["customer_reference"] = "string",
                       ["customerReference"] = "string",
                       ["instructions"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["warehouse"] = "Warehouse",
                        ["shippingItemList"] = "ShippingItem",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["warehouse"] = "warehouse",
                   ["reference"] = "reference",
                   ["customer_reference"] = "customer_reference",
                   ["customerReference"] = "customer_reference",
                   ["instructions"] = "instructions",
                   ["version"] = "version",
                })),
            ["ShippingItem"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["shipping_order"] = "integer",
                       ["shippingOrder"] = "integer",
                       ["sku"] = "string",
                       ["quantity"] = "integer",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["shipping_order"] = "ShippingOrder",
                       ["shippingOrder"] = "ShippingOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["shipping_order"] = "shipping_order",
                   ["shippingOrder"] = "shipping_order",
                   ["sku"] = "sku",
                   ["quantity"] = "quantity",
                   ["version"] = "version",
                })),
        });
}