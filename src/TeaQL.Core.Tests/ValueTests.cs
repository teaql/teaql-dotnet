using System;
using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class ValueTests
{
    [Fact]
    public void Value_TryI64_AcceptsRepresentableNumericVariants()
    {
        Assert.Equal(long.MinValue, new Value.I64Value(long.MinValue).TryI64());
        Assert.Equal(long.MaxValue, new Value.I64Value(long.MaxValue).TryI64());
        Assert.Equal(long.MaxValue, new Value.U64Value((ulong)long.MaxValue).TryI64());
        Assert.Equal(-42L, new Value.DecimalValue(-42m).TryI64());
    }

    [Fact]
    public void Value_TryI64_RejectsUnsignedOverflowAndUnrelatedVariants()
    {
        Assert.Null(new Value.U64Value((ulong)long.MaxValue + 1).TryI64());
        Assert.Null(new Value.U64Value(ulong.MaxValue).TryI64());
        Assert.Null(new Value.F64Value(42.0).TryI64());
        Assert.Null(new Value.TextValue("42").TryI64());
        Assert.Null(new Value.NullValue().TryI64());
    }

    [Fact]
    public void Value_TryU64_AcceptsRepresentableNumericVariants()
    {
        Assert.Equal(0ul, new Value.U64Value(0).TryU64());
        Assert.Equal(ulong.MaxValue, new Value.U64Value(ulong.MaxValue).TryU64());
        Assert.Equal((ulong)long.MaxValue, new Value.I64Value(long.MaxValue).TryU64());
        Assert.Equal(42ul, new Value.DecimalValue(42m).TryU64());
    }

    [Fact]
    public void Value_TryU64_RejectsNegativeAndUnrelatedVariants()
    {
        Assert.Null(new Value.I64Value(-1).TryU64());
        Assert.Null(new Value.DecimalValue(-1m).TryU64());
        Assert.Null(new Value.F64Value(42.0).TryU64());
        Assert.Null(new Value.TextValue("42").TryU64());
        Assert.Null(new Value.NullValue().TryU64());
    }

    [Fact]
    public void Value_TryDecimal_AcceptsDecimalIntegerAndTextVariants()
    {
        var decimalVal = 123.450m;

        Assert.Equal(decimalVal, new Value.DecimalValue(decimalVal).TryDecimal());
        Assert.Equal((decimal)long.MinValue, new Value.I64Value(long.MinValue).TryDecimal());
        Assert.Equal((decimal)ulong.MaxValue, new Value.U64Value(ulong.MaxValue).TryDecimal());
        Assert.Equal(decimalVal, new Value.TextValue("123.450").TryDecimal());
    }

    [Fact]
    public void Value_TryDecimal_RejectsInvalidTextAndUnrelatedVariants()
    {
        Assert.Null(new Value.TextValue("not-a-decimal").TryDecimal());
        Assert.Null(new Value.BoolValue(true).TryDecimal());
        Assert.Null(new Value.F64Value(1.5).TryDecimal());
        Assert.Null(new Value.NullValue().TryDecimal());
    }

    [Fact]
    public void Value_TryF64_AcceptsSupportedNumericVariants()
    {
        Assert.Equal(1.25, new Value.F64Value(1.25).TryF64());
        Assert.Equal(-2.0, new Value.I64Value(-2).TryF64());
        Assert.Equal(2.0, new Value.U64Value(2).TryF64());
        Assert.Equal(1.5, new Value.DecimalValue(1.5m).TryF64());
    }

    [Fact]
    public void Value_TryF64_RejectsUnrelatedVariants()
    {
        Assert.Null(new Value.TextValue("1.5").TryF64());
        Assert.Null(new Value.BoolValue(true).TryF64());
        Assert.Null(new Value.NullValue().TryF64());
    }

    [Fact]
    public void Value_TryDate_AcceptsDateAndIsoDateText()
    {
        var leapDay = new DateTime(2024, 2, 29, 0, 0, 0, DateTimeKind.Utc);

        Assert.Equal(leapDay, new Value.DateValue(leapDay).TryDate());
        Assert.Equal(leapDay, new Value.TextValue("2024-02-29").TryDate());
        
        long millis = ((DateTimeOffset)leapDay).ToUnixTimeMilliseconds();
        Assert.Equal(leapDay, new Value.I64Value(millis).TryDate());
        Assert.Equal(leapDay, new Value.U64Value((ulong)millis).TryDate());
    }

    [Fact]
    public void Value_TeaqlIsEmpty_WorksCorrectly()
    {
        Assert.True(new Value.NullValue().TeaqlIsEmpty());
        Assert.True(new Value.TypedNullValue(DataType.Text).TeaqlIsEmpty());
        Assert.True(new Value.TextValue("").TeaqlIsEmpty());
        Assert.False(new Value.TextValue("1").TeaqlIsEmpty());
        Assert.True(new Value.ObjectValue(new Record()).TeaqlIsEmpty());
        Assert.True(new Value.ListValue(new System.Collections.Generic.List<Value>()).TeaqlIsEmpty());
    }
}
