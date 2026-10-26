using System.Globalization;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class IntegralExactnessTests
{
    [Theory]
    [InlineData("12.75")]
    [InlineData("-0.5")]
    [InlineData("9223372036854775808")]
    [InlineData("-9223372036854775809")]
    [InlineData("79228162514264337593543950335")]
    public void IntegralExactnessSignedConversionNeverTruncatesOrThrows(string text)
    {
        Assert.Null(new Value.DecimalValue(decimal.Parse(text, CultureInfo.InvariantCulture)).TryI64());
    }

    [Theory]
    [InlineData("1.5")]
    [InlineData("0.0001")]
    [InlineData("18446744073709551616")]
    [InlineData("79228162514264337593543950335")]
    public void IntegralExactnessUnsignedConversionNeverTruncatesOrThrows(string text)
    {
        Assert.Null(new Value.DecimalValue(decimal.Parse(text, CultureInfo.InvariantCulture)).TryU64());
    }

    [Fact]
    public void IntegralExactnessKeepsBothSignedAndUnsignedEndpoints()
    {
        Assert.Equal(long.MinValue, new Value.DecimalValue((decimal)long.MinValue).TryI64());
        Assert.Equal(long.MaxValue, new Value.DecimalValue((decimal)long.MaxValue).TryI64());
        Assert.Equal(ulong.MaxValue, new Value.DecimalValue((decimal)ulong.MaxValue).TryU64());
        Assert.Equal(0ul, new Value.DecimalValue(0m).TryU64());
    }

    [Theory]
    [InlineData("1.5")]
    [InlineData("18446744073709551616")]
    public void IntegralExactnessEntityIdentityCannotBeFabricatedByNarrowing(string text)
    {
        var value = new Value.DecimalValue(decimal.Parse(text, CultureInfo.InvariantCulture));
        Assert.Same(value, value.EntityIdValue());
    }

    [Fact]
    public void IntegralExactnessNegativeIdentityIsNotWrappedToUnsignedMaximum()
    {
        var value = new Value.I64Value(-1);
        Assert.Same(value, value.EntityIdValue());
    }
}
