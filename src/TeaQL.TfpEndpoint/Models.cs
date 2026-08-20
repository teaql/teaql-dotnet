using System.Text.Json.Serialization;
using System.Collections.Generic;
using TeaQL.Core;

namespace TeaQL.TfpEndpoint
{
    public class TfpOrderBy
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = string.Empty;

        [JsonPropertyName("direction")]
        public string Direction { get; set; } = "Asc";
    }

    public class TfpSelectQuery
    {
        [JsonPropertyName("entity")]
        public string Entity { get; set; } = string.Empty;

        [JsonPropertyName("filterCondition")]
        public Dictionary<string, object>? FilterCondition { get; set; }

        [JsonPropertyName("limitValue")]
        public ulong? LimitValue { get; set; }

        [JsonPropertyName("offsetValue")]
        public ulong? OffsetValue { get; set; }

        [JsonPropertyName("orderItems")]
        public List<TfpOrderBy>? OrderItems { get; set; }

        [JsonPropertyName("selectItems")]
        public List<string>? SelectItems { get; set; }

        [JsonPropertyName("groupByItems")]
        public List<string>? GroupByItems { get; set; }

        [JsonPropertyName("aggregateItems")]
        public List<object>? AggregateItems { get; set; }

        [JsonPropertyName("commentText")]
        public string? CommentText { get; set; }

        [JsonPropertyName("purposeText")]
        public string? PurposeText { get; set; }
    }

    public class TfpMutationQuery
    {
        [JsonPropertyName("entity")]
        public string Entity { get; set; } = string.Empty;

        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("payload")]
        public Dictionary<string, object> Payload { get; set; } = new();

        [JsonPropertyName("id")]
        public object? Id { get; set; }

        [JsonPropertyName("expectedVersion")]
        public long? ExpectedVersion { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }

    public sealed class TrustedFederalContext
    {
        public required string TenantField { get; init; }
        public required Value TenantId { get; init; }
        public required string AuthenticatedUser { get; init; }
        public required string ApprovedPurpose { get; init; }
        public required ISet<string> AllowedEntities { get; init; }
        public required IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> ReadableFields { get; init; }
        public required IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> WritableFields { get; init; }
        public required IReadOnlyDictionary<string, ISet<string>> AllowedActions { get; init; }
        public int MaxPageSize { get; init; } = 100;
    }

    public sealed class TfpEndpointException(string code, string message, Exception? inner = null)
        : Exception(message, inner)
    {
        public string Code { get; } = code;
    }
}
