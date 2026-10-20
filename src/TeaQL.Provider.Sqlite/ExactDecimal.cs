using System.Globalization;
using System.Numerics;

namespace TeaQL.Provider.Sqlite;

/// <summary>Exact, connection-local SQLite Decimal primitives.</summary>
internal static class ExactDecimal
{
    private static readonly BigInteger MaxCoefficient = (BigInteger.One << 96) - 1;

    internal static decimal Parse(string text)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        // Decimal.TryParse can round rather than fail for a fraction beyond
        // scale 28. Canonical round-trip equality rejects that loss and keeps
        // the generated storage representation unambiguous and bounded.
        if (text.Length > 10_000 || !decimal.TryParse(text, styles, CultureInfo.InvariantCulture, out var value)
            || !string.Equals(value.ToString(CultureInfo.InvariantCulture), text, StringComparison.Ordinal))
            throw new FormatException("Invalid or unrepresentable exact SQLite Decimal.");
        return value;
    }

    internal static bool IsValid(string? text)
    {
        if (text is null) return false;
        try { Parse(text); return true; }
        catch (FormatException) { return false; }
    }

    internal readonly record struct SumState(bool HasValue, BigInteger Coefficient, int Scale)
    {
        internal SumState Add(string text)
        {
            var amount = Parse(text);
            Span<int> bits = stackalloc int[4];
            decimal.GetBits(amount, bits);
            var coefficient = (BigInteger)(uint)bits[0]
                | ((BigInteger)(uint)bits[1] << 32) | ((BigInteger)(uint)bits[2] << 64);
            if (bits[3] < 0) coefficient = -coefficient;
            var scale = (bits[3] >> 16) & 0xff;
            if (!HasValue) return new(true, coefficient, scale);
            var commonScale = Math.Max(Scale, scale);
            return new(true,
                Coefficient * BigInteger.Pow(10, commonScale - Scale)
                + coefficient * BigInteger.Pow(10, commonScale - scale), commonScale);
        }

        internal string? Finish()
        {
            if (!HasValue) return null;
            var magnitude = BigInteger.Abs(Coefficient);
            var scale = Scale;
            // Permit cancellation of an overflowing intermediate sum. Reduce
            // scale only when it can preserve the final value exactly.
            while (magnitude > MaxCoefficient && scale > 0 && magnitude % 10 == 0)
            {
                magnitude /= 10;
                scale--;
            }
            if (magnitude > MaxCoefficient)
                throw new OverflowException("SQLite Decimal sum exceeds exact runtime precision.");
            var result = new decimal(
                unchecked((int)(uint)(magnitude & uint.MaxValue)),
                unchecked((int)(uint)((magnitude >> 32) & uint.MaxValue)),
                unchecked((int)(uint)((magnitude >> 64) & uint.MaxValue)),
                Coefficient.Sign < 0, checked((byte)scale));
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }
}
