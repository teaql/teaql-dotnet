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
                       ["platform"] = "integer",
                       ["order_number"] = "string",
                       ["orderNumber"] = "string",
                       ["description"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["platform"] = "Platform",
                        ["orderItemList"] = "OrderItem",
                        ["paymentList"] = "Payment",
                        ["shipmentList"] = "Shipment",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["platform"] = "platform",
                   ["order_number"] = "order_number",
                   ["orderNumber"] = "order_number",
                   ["description"] = "description",
                   ["version"] = "version",
                })),
            ["OrderItem"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["customer_order"] = "integer",
                       ["customerOrder"] = "integer",
                       ["name"] = "string",
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
                   ["name"] = "name",
                   ["version"] = "version",
                })),
            ["Payment"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["customer_order"] = "integer",
                       ["customerOrder"] = "integer",
                       ["reference_code"] = "string",
                       ["referenceCode"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["customer_order"] = "CustomerOrder",
                       ["customerOrder"] = "CustomerOrder",
                        ["paymentAttemptList"] = "PaymentAttempt",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["customer_order"] = "customer_order",
                   ["customerOrder"] = "customer_order",
                   ["reference_code"] = "reference_code",
                   ["referenceCode"] = "reference_code",
                   ["version"] = "version",
                })),
            ["PaymentAttempt"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["payment"] = "integer",
                       ["reference_code"] = "string",
                       ["referenceCode"] = "string",
                       ["version"] = "integer",
                    }),
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["payment"] = "Payment",
                    })),
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                   ["id"] = "id",
                   ["payment"] = "payment",
                   ["reference_code"] = "reference_code",
                   ["referenceCode"] = "reference_code",
                   ["version"] = "version",
                })),
            ["Shipment"] = new GeneratedSearchModel(
                new SearchModel(
                    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                       ["id"] = "integer",
                       ["customer_order"] = "integer",
                       ["customerOrder"] = "integer",
                       ["reference_code"] = "string",
                       ["referenceCode"] = "string",
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
                   ["reference_code"] = "reference_code",
                   ["referenceCode"] = "reference_code",
                   ["version"] = "version",
                })),
        });
}