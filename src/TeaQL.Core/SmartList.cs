using System.Collections.Generic;

namespace TeaQL.Core;

public class SmartList<T> : List<T>
{
    public long TotalCount { get; set; }
    public Record Aggregations { get; set; } = new();
    public Record Summary { get; set; } = new();
    public Dictionary<string, SmartList<Record>> Facets { get; set; } = new();
    public bool IsLoaded { get; set; }

    public SmartList() { }
    public SmartList(IEnumerable<T> collection) : base(collection) { }
}
