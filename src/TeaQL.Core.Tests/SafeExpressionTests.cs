using System;
using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class SafeExpressionTests
{
    [Fact]
    public void SafeExpression_EvalWith_UsesTheSuppliedRoot()
    {
        var expression = new SafeExpression<int, int>(2, root => (true, root * 3));
        
        Assert.Equal((true, 6), expression.Eval());
        Assert.Equal((true, 12), expression.EvalWith(4));
    }

    [Fact]
    public void SafeExpression_ApplyOptional_ShortCircuitsRemainingMappers()
    {
        int optionalCalls = 0;
        int remainingCalls = 0;

        var expression = SafeExpression.Value(5)
            .ApplyOptional<int>(value => 
            {
                optionalCalls++;
                return (false, 0);
            })
            .Apply(value => 
            {
                remainingCalls++;
                return value * 2;
            });

        Assert.Equal((false, 0), expression.Eval());
        Assert.Equal(1, optionalCalls);
        Assert.Equal(0, remainingCalls);
    }

    [Fact]
    public void SafeExpression_LazyFallbackAndError_OnlyRunForMissingValues()
    {
        int presentFallbackCalls = 0;
        var present = SafeExpression.Value(7);
        Assert.Equal(7, present.OrElseWith(() => 
        {
            presentFallbackCalls++;
            return 9;
        }));
        Assert.Equal(0, presentFallbackCalls);
        Assert.Equal(7, present.OrElseThrow(() => new Exception("unused error")));

        var missing = new SafeExpression<object, int>(new object(), _ => (false, 0));
        int missingFallbackCalls = 0;
        Assert.Equal(9, missing.OrElseWith(() => 
        {
            missingFallbackCalls++;
            return 9;
        }));
        Assert.Equal(1, missingFallbackCalls);
        Assert.Throws<Exception>(() => missing.OrElseThrow(() => new Exception("missing value")));
    }

    [Fact]
    public void SafeExpression_OrIfNull_ReturnsValueOrFallback()
    {
        Assert.Equal(7, SafeExpression.Value(7).OrIfNull(9));
        var missing = new SafeExpression<object, int>(new object(), _ => (false, 0));
        Assert.Equal(9, missing.OrIfNull(9));
    }

    [Fact]
    public void SafeExpression_CallbacksOnlyRunForTheirMatchingBranch()
    {
        var present = SafeExpression.Value("teaql");
        int presentNullCalls = 0;
        string? presentValue = null;
        present.WhenIsNull(() => presentNullCalls++);
        present.WhenIsNotNull(value => presentValue = value);
        Assert.Equal(0, presentNullCalls);
        Assert.Equal("teaql", presentValue);

        var missing = new SafeExpression<object, string>(new object(), _ => (false, null!));
        int missingNullCalls = 0;
        int missingValueCalls = 0;
        missing.WhenIsNull(() => missingNullCalls++);
        missing.WhenIsNotNull(_ => missingValueCalls++);
        Assert.Equal(1, missingNullCalls);
        Assert.Equal(0, missingValueCalls);
    }
}
