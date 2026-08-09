using System.Collections.Generic;

namespace TeaQL.Core;

public class Record : Dictionary<string, Value>
{
    public Record() : base() { }
    public Record(IDictionary<string, Value> dictionary) : base(dictionary) { }
}
