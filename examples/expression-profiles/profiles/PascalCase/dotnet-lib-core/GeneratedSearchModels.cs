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
            ["DistributionCenter"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["Id"] = "integer",
                       ["dock_name"] = "string",
                       ["dockName"] = "string",
                       ["DockName"] = "string",
                       ["version"] = "integer",
                       ["Version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["dispatchOrderList"] = "DispatchOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["Id"] = "id",
                   ["dock_name"] = "dock_name",
                   ["dockName"] = "dock_name",
                   ["DockName"] = "dock_name",
                   ["version"] = "version",
                   ["Version"] = "version",
                })),
            ["DispatchOrder"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["Id"] = "integer",
                       ["distribution_center"] = "integer",
                       ["distributionCenter"] = "integer",
                       ["DistributionCenter"] = "integer",
                       ["order_reference"] = "string",
                       ["orderReference"] = "string",
                       ["OrderReference"] = "string",
                       ["handling_note"] = "string",
                       ["handlingNote"] = "string",
                       ["HandlingNote"] = "string",
                       ["version"] = "integer",
                       ["Version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["distribution_center"] = "DistributionCenter",
                       ["distributionCenter"] = "DistributionCenter",
                       ["DistributionCenter"] = "DistributionCenter",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["Id"] = "id",
                   ["distribution_center"] = "distribution_center",
                   ["distributionCenter"] = "distribution_center",
                   ["DistributionCenter"] = "distribution_center",
                   ["order_reference"] = "order_reference",
                   ["orderReference"] = "order_reference",
                   ["OrderReference"] = "order_reference",
                   ["handling_note"] = "handling_note",
                   ["handlingNote"] = "handling_note",
                   ["HandlingNote"] = "handling_note",
                   ["version"] = "version",
                   ["Version"] = "version",
                })),
        });
}