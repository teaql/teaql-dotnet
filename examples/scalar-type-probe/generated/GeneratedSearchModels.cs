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
            ["TypePlatform"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["scalarTypeProbeList"] = "ScalarTypeProbe",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["ScalarTypeProbe"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["type_platform"] = "integer",
                       ["typePlatform"] = "integer",
                       ["required_string"] = "string",
                       ["requiredString"] = "string",
                       ["optional_string"] = "string",
                       ["optionalString"] = "string",
                       ["large_text"] = "string",
                       ["largeText"] = "string",
                       ["required_boolean"] = "boolean",
                       ["requiredBoolean"] = "boolean",
                       ["optional_boolean"] = "boolean",
                       ["optionalBoolean"] = "boolean",
                       ["required_integer"] = "integer",
                       ["requiredInteger"] = "integer",
                       ["optional_integer"] = "integer",
                       ["optionalInteger"] = "integer",
                       ["required_long"] = "integer",
                       ["requiredLong"] = "integer",
                       ["optional_long"] = "integer",
                       ["optionalLong"] = "integer",
                       ["required_decimal"] = "decimal",
                       ["requiredDecimal"] = "decimal",
                       ["optional_decimal"] = "decimal",
                       ["optionalDecimal"] = "decimal",
                       ["required_float"] = "number",
                       ["requiredFloat"] = "number",
                       ["optional_float"] = "number",
                       ["optionalFloat"] = "number",
                       ["required_double"] = "number",
                       ["requiredDouble"] = "number",
                       ["optional_double"] = "number",
                       ["optionalDouble"] = "number",
                       ["required_date"] = "date",
                       ["requiredDate"] = "date",
                       ["optional_date"] = "date",
                       ["optionalDate"] = "date",
                       ["required_time"] = "unsupported",
                       ["requiredTime"] = "unsupported",
                       ["optional_time"] = "unsupported",
                       ["optionalTime"] = "unsupported",
                       ["required_datetime"] = "timestamp",
                       ["requiredDatetime"] = "timestamp",
                       ["optional_datetime"] = "timestamp",
                       ["optionalDatetime"] = "timestamp",
                       ["create_time"] = "timestamp",
                       ["createTime"] = "timestamp",
                       ["update_time"] = "timestamp",
                       ["updateTime"] = "timestamp",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["type_platform"] = "TypePlatform",
                       ["typePlatform"] = "TypePlatform",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["type_platform"] = "type_platform",
                   ["typePlatform"] = "type_platform",
                   ["required_string"] = "required_string",
                   ["requiredString"] = "required_string",
                   ["optional_string"] = "optional_string",
                   ["optionalString"] = "optional_string",
                   ["large_text"] = "large_text",
                   ["largeText"] = "large_text",
                   ["required_boolean"] = "required_boolean",
                   ["requiredBoolean"] = "required_boolean",
                   ["optional_boolean"] = "optional_boolean",
                   ["optionalBoolean"] = "optional_boolean",
                   ["required_integer"] = "required_integer",
                   ["requiredInteger"] = "required_integer",
                   ["optional_integer"] = "optional_integer",
                   ["optionalInteger"] = "optional_integer",
                   ["required_long"] = "required_long",
                   ["requiredLong"] = "required_long",
                   ["optional_long"] = "optional_long",
                   ["optionalLong"] = "optional_long",
                   ["required_decimal"] = "required_decimal",
                   ["requiredDecimal"] = "required_decimal",
                   ["optional_decimal"] = "optional_decimal",
                   ["optionalDecimal"] = "optional_decimal",
                   ["required_float"] = "required_float",
                   ["requiredFloat"] = "required_float",
                   ["optional_float"] = "optional_float",
                   ["optionalFloat"] = "optional_float",
                   ["required_double"] = "required_double",
                   ["requiredDouble"] = "required_double",
                   ["optional_double"] = "optional_double",
                   ["optionalDouble"] = "optional_double",
                   ["required_date"] = "required_date",
                   ["requiredDate"] = "required_date",
                   ["optional_date"] = "optional_date",
                   ["optionalDate"] = "optional_date",
                   ["required_time"] = "required_time",
                   ["requiredTime"] = "required_time",
                   ["optional_time"] = "optional_time",
                   ["optionalTime"] = "optional_time",
                   ["required_datetime"] = "required_datetime",
                   ["requiredDatetime"] = "required_datetime",
                   ["optional_datetime"] = "optional_datetime",
                   ["optionalDatetime"] = "optional_datetime",
                   ["create_time"] = "create_time",
                   ["createTime"] = "create_time",
                   ["update_time"] = "update_time",
                   ["updateTime"] = "update_time",
                   ["version"] = "version",
                })),
        });
}