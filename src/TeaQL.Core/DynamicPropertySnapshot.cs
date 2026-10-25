using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace TeaQL.Core;

/// <summary>Query-derived properties belong to the result, not the mutation
/// payload. Missing properties return null; fixed model fields remain typed.</summary>
public sealed class DynamicPropertySnapshot
{
    private readonly IReadOnlyDictionary<string,Value> values;
    private DynamicPropertySnapshot(IReadOnlyDictionary<string,Value> values) => this.values = values;
    public static DynamicPropertySnapshot Empty { get; } = new(new ReadOnlyDictionary<string,Value>(new Dictionary<string,Value>()));
    public Value? Get(string name) => values.TryGetValue(name,out var value) ? new QuerySnapshot().Data(value) : null;
    public bool Contains(string name) => values.ContainsKey(name);
    /// <summary>A detached JSON view of query aliases, including explicit nulls.</summary>
    public JsonObject ToJson() => new(values.Select(item =>
        new KeyValuePair<string,JsonNode?>(item.Key,item.Value.ToJsonValue())));
    public static DynamicPropertySnapshot Capture(Record record,IEnumerable<string> modelFields)
    {
        var fixedFields = new HashSet<string>(modelFields,StringComparer.Ordinal);
        var snapshot = new QuerySnapshot();
        return new(new ReadOnlyDictionary<string,Value>(record.Where(item=>!fixedFields.Contains(item.Key))
            .ToDictionary(item=>item.Key,item=>snapshot.Data(item.Value))));
    }
}
