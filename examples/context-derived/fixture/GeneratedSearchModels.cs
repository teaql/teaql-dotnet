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
            ["Catalog"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["entryList"] = "Entry",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["Entry"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["catalog"] = "integer",
                       ["name"] = "string",
                       ["born_at"] = "timestamp",
                       ["bornAt"] = "timestamp",
                       ["touched_at"] = "timestamp",
                       ["touchedAt"] = "timestamp",
                       ["source_ip"] = "string",
                       ["sourceIp"] = "string",
                       ["last_ip"] = "string",
                       ["lastIp"] = "string",
                       ["opened_by"] = "string",
                       ["openedBy"] = "string",
                       ["changed_by"] = "string",
                       ["changedBy"] = "string",
                       ["guarded_token"] = "string",
                       ["guardedToken"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["catalog"] = "Catalog",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["catalog"] = "catalog",
                   ["name"] = "name",
                   ["born_at"] = "born_at",
                   ["bornAt"] = "born_at",
                   ["touched_at"] = "touched_at",
                   ["touchedAt"] = "touched_at",
                   ["source_ip"] = "source_ip",
                   ["sourceIp"] = "source_ip",
                   ["last_ip"] = "last_ip",
                   ["lastIp"] = "last_ip",
                   ["opened_by"] = "opened_by",
                   ["openedBy"] = "opened_by",
                   ["changed_by"] = "changed_by",
                   ["changedBy"] = "changed_by",
                   ["guarded_token"] = "guarded_token",
                   ["guardedToken"] = "guarded_token",
                   ["version"] = "version",
                })),
        });
}