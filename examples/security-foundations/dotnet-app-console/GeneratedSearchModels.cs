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
            ["Merchant"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["customerProfileList"] = "CustomerProfile",
                        ["customerOrderList"] = "CustomerOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["CustomerProfile"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["merchant"] = "integer",
                       ["email"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["merchant"] = "Merchant",
                        ["customerOrderList"] = "CustomerOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["merchant"] = "merchant",
                   ["email"] = "email",
                   ["version"] = "version",
                })),
            ["CustomerOrder"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["merchant"] = "integer",
                       ["customer_profile"] = "integer",
                       ["customerProfile"] = "integer",
                       ["order_number"] = "string",
                       ["orderNumber"] = "string",
                       ["fulfillment_status"] = "string",
                       ["fulfillmentStatus"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["merchant"] = "Merchant",
                       ["customer_profile"] = "CustomerProfile",
                       ["customerProfile"] = "CustomerProfile",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["merchant"] = "merchant",
                   ["customer_profile"] = "customer_profile",
                   ["customerProfile"] = "customer_profile",
                   ["order_number"] = "order_number",
                   ["orderNumber"] = "order_number",
                   ["fulfillment_status"] = "fulfillment_status",
                   ["fulfillmentStatus"] = "fulfillment_status",
                   ["version"] = "version",
                })),
        });
}