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
                       ["base_url"] = "string",
                       ["baseUrl"] = "string",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["schoolTypeList"] = "SchoolType",
                        ["schoolList"] = "School",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["base_url"] = "base_url",
                   ["baseUrl"] = "base_url",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
            ["SchoolType"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["platform"] = "integer",
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["code"] = "string",
                       ["display_order"] = "decimal",
                       ["displayOrder"] = "decimal",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["platform"] = "Platform",
                        ["schoolList"] = "School",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["platform"] = "platform",
                   ["id"] = "id",
                   ["name"] = "name",
                   ["code"] = "code",
                   ["display_order"] = "display_order",
                   ["displayOrder"] = "display_order",
                   ["version"] = "version",
                })),
            ["School"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["platform"] = "integer",
                       ["school_type"] = "integer",
                       ["schoolType"] = "integer",
                       ["name"] = "string",
                       ["address"] = "string",
                       ["established_date"] = "date",
                       ["establishedDate"] = "date",
                       ["student_capacity"] = "integer",
                       ["studentCapacity"] = "integer",
                       ["active"] = "boolean",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["platform"] = "Platform",
                       ["school_type"] = "SchoolType",
                       ["schoolType"] = "SchoolType",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["platform"] = "platform",
                   ["school_type"] = "school_type",
                   ["schoolType"] = "school_type",
                   ["name"] = "name",
                   ["address"] = "address",
                   ["established_date"] = "established_date",
                   ["establishedDate"] = "established_date",
                   ["student_capacity"] = "student_capacity",
                   ["studentCapacity"] = "student_capacity",
                   ["active"] = "active",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
        });
}