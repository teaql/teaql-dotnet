using System;

namespace TeaQL.Core;

public readonly struct Timestamp : IEquatable<Timestamp>, IComparable<Timestamp>
{
    public long Value { get; }

    public Timestamp(long value)
    {
        Value = value;
    }

    public static Timestamp Now()
    {
        return new Timestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }

    public long AsMillis()
    {
        return Value;
    }

    public DateTimeOffset ToDateTime()
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(Value);
    }

    public static implicit operator Timestamp(long value) => new Timestamp(value);
    public static implicit operator Timestamp(ulong value) => new Timestamp((long)value);

    public bool Equals(Timestamp other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is Timestamp other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public int CompareTo(Timestamp other) => Value.CompareTo(other.Value);

    public static bool operator ==(Timestamp left, Timestamp right) => left.Equals(right);
    public static bool operator !=(Timestamp left, Timestamp right) => !(left == right);
    public static bool operator <(Timestamp left, Timestamp right) => left.CompareTo(right) < 0;
    public static bool operator <=(Timestamp left, Timestamp right) => left.CompareTo(right) <= 0;
    public static bool operator >(Timestamp left, Timestamp right) => left.CompareTo(right) > 0;
    public static bool operator >=(Timestamp left, Timestamp right) => left.CompareTo(right) >= 0;
}
