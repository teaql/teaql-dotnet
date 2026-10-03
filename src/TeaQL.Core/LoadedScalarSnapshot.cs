namespace TeaQL.Core;

/// <summary>Loaded scalar provenance, independent of mutation ownership and relations.</summary>
public sealed class LoadedScalarSnapshot
{
    private readonly Record _values;
    public LoadedScalarSnapshot(Record values) => _values = Copy(values);
    public Record CopyValues() => Copy(_values);
    private static Record Copy(Record values) => new(values.ToDictionary(pair => pair.Key, pair => CopyValue(pair.Value)));
    private static Value CopyValue(Value value) => value switch
    {
        Value.JsonValue item => new Value.JsonValue(item.Value?.DeepClone()),
        Value.ObjectValue item => new Value.ObjectValue(Copy(item.Value)),
        Value.ListValue item => new Value.ListValue(item.Values.Select(CopyValue).ToList()),
        _ => value
    };
}
