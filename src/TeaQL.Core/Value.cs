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
    public sealed record ObjectValue(Record Value) : Value;
    public sealed record ListValue(List<Value> Values) : Value;
    public sealed record TypedNullValue(DataType Type) : Value;
}
