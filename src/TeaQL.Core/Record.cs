using System.Collections.Generic;

namespace TeaQL.Core;

public class Record : Dictionary<string, Value>
{
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
