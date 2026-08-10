using System.Text.Json.Serialization;
using System.Collections.Generic;

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

        [JsonPropertyName("commentText")]
        public string? CommentText { get; set; }
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

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}
