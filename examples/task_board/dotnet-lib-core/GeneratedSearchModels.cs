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
                       ["founded"] = "timestamp",
                       ["user_email"] = "string",
                       ["userEmail"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["taskStatusList"] = "TaskStatus",
                        ["taskList"] = "Task",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["founded"] = "founded",
                   ["user_email"] = "user_email",
                   ["userEmail"] = "user_email",
                   ["version"] = "version",
                })),
            ["TaskStatus"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["code"] = "string",
                       ["color"] = "string",
                       ["display_order"] = "decimal",
                       ["displayOrder"] = "decimal",
                       ["progress"] = "decimal",
                       ["platform"] = "integer",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["platform"] = "Platform",
                        ["taskList"] = "Task",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["code"] = "code",
                   ["color"] = "color",
                   ["display_order"] = "display_order",
                   ["displayOrder"] = "display_order",
                   ["progress"] = "progress",
                   ["platform"] = "platform",
                   ["version"] = "version",
                })),
            ["Task"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["status"] = "integer",
                       ["platform"] = "integer",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["status"] = "TaskStatus",
                       ["platform"] = "Platform",
                        ["taskExecutionLogList"] = "TaskExecutionLog",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["status"] = "status",
                   ["platform"] = "platform",
                   ["version"] = "version",
                })),
            ["TaskExecutionLog"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["task"] = "integer",
                       ["action"] = "string",
                       ["detail"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["task"] = "Task",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["task"] = "task",
                   ["action"] = "action",
                   ["detail"] = "detail",
                   ["version"] = "version",
                })),
        });
}