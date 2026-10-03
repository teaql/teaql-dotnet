namespace TeaQL.Core;

/// <summary>Owned query-only aliases, never entity fields or pending mutations.</summary>
public sealed class QueryProjectionSnapshot
{
    public static QueryProjectionSnapshot Empty { get; } = new(new Record(), Array.Empty<string>());
    private readonly Record _values;

    public QueryProjectionSnapshot(Record row, IEnumerable<string> modeledNames)
    {
        var excluded = new HashSet<string>(modeledNames, StringComparer.Ordinal);
        _values = new Record(row.Where(pair => !excluded.Contains(pair.Key))
            .ToDictionary(pair => pair.Key, pair => LoadedScalarSnapshot.CopyValue(pair.Value)));
    }

    public bool Contains(string alias) => _values.ContainsKey(alias);
    // An absent alias throws KeyNotFoundException; null and zero remain present.
    public Value Get(string alias) => LoadedScalarSnapshot.CopyValue(_values[alias]);
}
