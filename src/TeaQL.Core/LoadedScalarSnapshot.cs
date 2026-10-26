namespace TeaQL.Core;

/// <summary>Loaded scalar provenance, independent of mutation ownership and relations.</summary>
public sealed class LoadedScalarSnapshot
{
    private readonly Record _values;
    internal EntityKey? Owner { get; }
    public LoadedScalarSnapshot(Record values) => _values = Copy(values);
    public LoadedScalarSnapshot(EntityKey owner, Record values)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _values = Copy(values);
    }
    public Record CopyValues() => Copy(_values);
    internal bool TryOriginal(string field, out Value value) => _values.TryGetValue(field, out value!);

    /// <summary>Exact mutation representation, not coercive query equality.</summary>
    public static bool SameValue(Value left, Value right) => SameValue(left, right, 0);
    private static bool SameValue(Value left, Value right, int depth)
    {
        if (depth > 32) return false;
        return (left, right) switch
        {
            (Value.NullValue or Value.TypedNullValue, Value.NullValue or Value.TypedNullValue) => true,
            (Value.F64Value a, Value.F64Value b) => double.IsFinite(a.Value) && double.IsFinite(b.Value)
                && BitConverter.DoubleToInt64Bits(a.Value) == BitConverter.DoubleToInt64Bits(b.Value),
            (Value.DecimalValue a, Value.DecimalValue b) => decimal.GetBits(a.Value).SequenceEqual(decimal.GetBits(b.Value)),
            (Value.DateValue a, Value.DateValue b) => a.Value.Ticks == b.Value.Ticks && a.Value.Kind == b.Value.Kind,
            (Value.DateTimeValue a, Value.DateTimeValue b) => a.Value.Ticks == b.Value.Ticks && a.Value.Kind == b.Value.Kind,
            (Value.JsonValue a, Value.JsonValue b) => a.Value?.ToJsonString() == b.Value?.ToJsonString(),
            (Value.ObjectValue a, Value.ObjectValue b) => a.Value.Count == b.Value.Count
                && a.Value.All(item => b.Value.TryGetValue(item.Key, out var other) && SameValue(item.Value, other, depth + 1)),
            (Value.ListValue a, Value.ListValue b) => a.Values.Count == b.Values.Count
                && a.Values.Zip(b.Values).All(item => SameValue(item.First, item.Second, depth + 1)),
            _ => left.GetType() == right.GetType() && left == right
        };
    }
    private static Record Copy(Record values) => new(values.ToDictionary(pair => pair.Key, pair => CopyValue(pair.Value)));
    internal static Value CopyValue(Value value) => value switch
    {
        Value.JsonValue item => new Value.JsonValue(item.Value?.DeepClone()),
        Value.ObjectValue item => new Value.ObjectValue(Copy(item.Value)),
        Value.ListValue item => new Value.ListValue(item.Values.Select(CopyValue).ToList()),
        _ => value
    };
}
