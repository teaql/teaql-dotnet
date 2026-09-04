using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace TeaQL.Core;

public abstract record Value
{
    private Value() { }

    public sealed record NullValue : Value;
    public sealed record BoolValue(bool Value) : Value;
    public sealed record I64Value(long Value) : Value;
    public sealed record U64Value(ulong Value) : Value;
    public sealed record F64Value(double Value) : Value;
    public sealed record DecimalValue(decimal Value) : Value;
    public sealed record TextValue(string Value) : Value;
    public sealed record JsonValue(JsonNode? Value) : Value;
    public sealed record DateValue(DateTime Value) : Value;
    public sealed record TimestampValue(long Milliseconds) : Value;
    public sealed record DateTimeValue(DateTime Value) : Value
    {
        public DateTimeValue(DateTimeOffset value) : this(value.UtcDateTime) { }
    }
    public sealed record TimeValue(TimeSpan Value) : Value;
    public sealed record ObjectValue(Record Value) : Value;
    public sealed record ListValue(List<Value> Values) : Value;
    public sealed record TypedNullValue(DataType Type) : Value;

    /// <summary>Language-native representation used by generated checkers and diagnostics.</summary>
    public object? Raw => this switch
    {
        NullValue or TypedNullValue => null,
        BoolValue(var value) => value,
        I64Value(var value) => value,
        U64Value(var value) => value,
        F64Value(var value) => value,
        DecimalValue(var value) => value,
        TextValue(var value) => value,
        JsonValue(var value) => value,
        DateValue(var value) => value,
        TimestampValue(var value) => value,
        DateTimeValue(var value) => value,
        TimeValue(var value) => value,
        ObjectValue(var value) => value,
        ListValue(var values) => values,
        _ => null
    };

    public static Value Object(Record record) => new ObjectValue(record);

    public static Value FromObject(object? value) => value switch
    {
        null => new NullValue(),
        Value existing => existing,
        bool item => new BoolValue(item),
        byte item => new I64Value(item),
        short item => new I64Value(item),
        int item => new I64Value(item),
        long item => new I64Value(item),
        uint item => new U64Value(item),
        ulong item => new U64Value(item),
        float item => new F64Value(item),
        double item => new F64Value(item),
        decimal item => new DecimalValue(item),
        string item => new TextValue(item),
        DateTime item => new TimestampValue(new DateTimeOffset(item).ToUnixTimeMilliseconds()),
        DateTimeOffset item => new TimestampValue(item.ToUnixTimeMilliseconds()),
        Record item => new ObjectValue(item),
        IEnumerable<object?> items => new ListValue(items.Select(FromObject).ToList()),
        _ => throw new ArgumentException($"Unsupported TeaQL value type: {value.GetType().FullName}", nameof(value))
    };

    public long? TryI64() => this switch
    {
        I64Value(var v) => v,
        U64Value(var v) => v <= long.MaxValue ? (long)v : null,
        DecimalValue(var v) => (long)v,
        _ => null
    };

    public ulong? TryU64() => this switch
    {
        U64Value(var v) => v,
        I64Value(var v) => v >= 0 ? (ulong)v : null,
        DecimalValue(var v) => v >= 0 ? (ulong)v : null,
        _ => null
    };

    public decimal? TryDecimal() => this switch
    {
        DecimalValue(var v) => v,
        I64Value(var v) => v,
        U64Value(var v) => v,
        TextValue(var v) => decimal.TryParse(v, out var result) ? result : null,
        _ => null
    };

    public double? TryF64() => this switch
    {
        F64Value(var v) => v,
        I64Value(var v) => v,
        U64Value(var v) => v,
        DecimalValue(var v) => (double)v,
        _ => null
    };

    public string? TryText() => this switch
    {
        TextValue(var v) => v,
        _ => null
    };

    public bool? TryBool() => this switch
    {
        BoolValue(var v) => v,
        _ => null
    };

    public DateTime? TryDate() => this switch
    {
        DateValue(var v) => v,
        TextValue(var v) => DateTime.TryParseExact(v, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var dt) ? dt : null,
        I64Value(var v) => DateTimeOffset.FromUnixTimeMilliseconds(v).UtcDateTime.Date,
        U64Value(var v) => v <= long.MaxValue ? DateTimeOffset.FromUnixTimeMilliseconds((long)v).UtcDateTime.Date : null,
        _ => null
    };

    public long? TryTimestamp() => this switch
    {
        TimestampValue(var v) => v,
        DateTimeValue(var v) => new DateTimeOffset(v).ToUnixTimeMilliseconds(),
        TextValue(var v) => DateTime.TryParse(v, out var dt) ? new DateTimeOffset(dt).ToUnixTimeMilliseconds() : null,
        I64Value(var v) => v,
        U64Value(var v) => v <= long.MaxValue ? (long)v : null,
        _ => null
    };

    public JsonNode? ToJsonValue() => this switch
    {
        NullValue => null,
        BoolValue(var v) => System.Text.Json.Nodes.JsonValue.Create(v),
        I64Value(var v) => System.Text.Json.Nodes.JsonValue.Create(v),
        U64Value(var v) => System.Text.Json.Nodes.JsonValue.Create(v),
        F64Value(var v) => System.Text.Json.Nodes.JsonValue.Create(v),
        DecimalValue(var v) => System.Text.Json.Nodes.JsonValue.Create(v.ToString()),
        TextValue(var v) => System.Text.Json.Nodes.JsonValue.Create(v),
        Value.JsonValue(var v) => v?.DeepClone(),
        DateValue(var v) => System.Text.Json.Nodes.JsonValue.Create(v.ToString("yyyy-MM-dd")),
        TimestampValue(var v) => System.Text.Json.Nodes.JsonValue.Create(v),
        DateTimeValue(var v) => System.Text.Json.Nodes.JsonValue.Create(v),
        TimeValue(var v) => System.Text.Json.Nodes.JsonValue.Create(v.ToString()),
        ObjectValue(var v) => v.ToJsonValue(),
        ListValue(var v) => new JsonArray(v.Select(item => item.ToJsonValue()).ToArray()),
        TypedNullValue => null,
        _ => null
    };

    public bool TeaqlIsEmpty() => this switch
    {
        NullValue => true,
        TypedNullValue => true,
        TextValue(var v) => string.IsNullOrEmpty(v),
        ObjectValue(var v) => v.Count == 0,
        ListValue(var v) => v.Count == 0,
        _ => false
    };

    public Value EntityIdValue() => this switch
    {
        I64Value(var v) => new U64Value((ulong)v),
        DecimalValue(var v) => new U64Value((ulong)v),
        TextValue(var v) => ulong.TryParse(v, out var result) ? new U64Value(result) : this,
        _ => this
    };
}
