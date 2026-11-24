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
                        ["freightQuoteList"] = "FreightQuote",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["FreightQuote"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["platform"] = "integer",
                       ["reference"] = "string",
                       ["amount"] = "decimal",
                       ["shipment_number"] = "integer",
                       ["shipmentNumber"] = "integer",
                       ["package_count"] = "integer",
                       ["packageCount"] = "integer",
                       ["delivery_date"] = "date",
                       ["deliveryDate"] = "date",
                       ["pickup_time"] = "unsupported",
                       ["pickupTime"] = "unsupported",
                       ["quoted_at"] = "timestamp",
                       ["quotedAt"] = "timestamp",
                       ["instructions"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["platform"] = "Platform",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["platform"] = "platform",
                   ["reference"] = "reference",
                   ["amount"] = "amount",
                   ["shipment_number"] = "shipment_number",
                   ["shipmentNumber"] = "shipment_number",
                   ["package_count"] = "package_count",
                   ["packageCount"] = "package_count",
                   ["delivery_date"] = "delivery_date",
                   ["deliveryDate"] = "delivery_date",
                   ["pickup_time"] = "pickup_time",
                   ["pickupTime"] = "pickup_time",
                   ["quoted_at"] = "quoted_at",
                   ["quotedAt"] = "quoted_at",
                   ["instructions"] = "instructions",
                   ["version"] = "version",
                })),
        });
}