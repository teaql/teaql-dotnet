using System;
using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class TimestampTests
{
    [Fact]
    public void TestTimestampConversions()
    {
        Timestamp ts = 1000L;
        Assert.Equal(1000L, ts.AsMillis());
        var dt = ts.ToDateTime();
        Assert.Equal(1000L, dt.ToUnixTimeMilliseconds());

        Timestamp ts2 = 2000UL;
        Assert.Equal(2000L, ts2.AsMillis());
    }

    [Fact]
    public void TestTimestampNow()
    {
        var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var ts = Timestamp.Now();
        var after = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Assert.True(ts.AsMillis() >= before);
        Assert.True(ts.AsMillis() <= after);
    }
}
