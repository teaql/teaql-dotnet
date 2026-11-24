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
            ["CommercePlatform"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["name"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["customerOrderList"] = "CustomerOrder",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["CustomerOrder"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["order_number"] = "string",
                       ["orderNumber"] = "string",
                       ["commerce_platform"] = "integer",
                       ["commercePlatform"] = "integer",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["commerce_platform"] = "CommercePlatform",
                       ["commercePlatform"] = "CommercePlatform",
                        ["orderAttachmentList"] = "OrderAttachment",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["order_number"] = "order_number",
                   ["orderNumber"] = "order_number",
                   ["commerce_platform"] = "commerce_platform",
                   ["commercePlatform"] = "commerce_platform",
                   ["version"] = "version",
                })),
            ["OrderAttachment"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["customer_order"] = "integer",
                       ["customerOrder"] = "integer",
                       ["file_name"] = "string",
                       ["fileName"] = "string",
                       ["content_type"] = "string",
                       ["contentType"] = "string",
                       ["content_length"] = "integer",
                       ["contentLength"] = "integer",
                       ["content_sha256"] = "string",
                       ["contentSha256"] = "string",
                       ["object_key"] = "string",
                       ["objectKey"] = "string",
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
                   ["file_name"] = "file_name",
                   ["fileName"] = "file_name",
                   ["content_type"] = "content_type",
                   ["contentType"] = "content_type",
                   ["content_length"] = "content_length",
                   ["contentLength"] = "content_length",
                   ["content_sha256"] = "content_sha256",
                   ["contentSha256"] = "content_sha256",
                   ["object_key"] = "object_key",
                   ["objectKey"] = "object_key",
                   ["version"] = "version",
                })),
        });
}