using System.Collections.Generic;

namespace TeaQL.Core;

public class Record : Dictionary<string, Value>
{
    /// <summary>Query-only facet results for an already-loaded relation, never scalar write fields.</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public Dictionary<string, SmartList<Record>> QueryFacets { get; } = new();

    public Record() : base() { }
    public Record(IReadOnlyDictionary<string, Value> dictionary)
        : base(dictionary.ToDictionary(item => item.Key, item => item.Value)) { }

    public System.Text.Json.Nodes.JsonNode ToJsonValue()
    {
        var obj = new System.Text.Json.Nodes.JsonObject();
        foreach (var kvp in this)
        {
            obj.Add(kvp.Key, kvp.Value.ToJsonValue());
        }
        return obj;
    }
}
