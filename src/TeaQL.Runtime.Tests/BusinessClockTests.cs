using TeaQL.Runtime;

namespace TeaQL.Runtime.Tests;

public class BusinessClockTests
{
    [Fact]
    public void FixedClockDrivesBusinessTimeAndDate()
    {
        var expected = new DateTimeOffset(2026, 10, 1, 23, 45, 30, TimeSpan.FromHours(8));
        var context = new UserContext().WithBusinessClock(new FixedBusinessClock(expected));

        Assert.Equal(expected, context.BusinessTime);
        Assert.Equal(new DateOnly(2026, 10, 1), context.BusinessDate);
    }

    [Fact]
    public void SystemClockIsTheDefault()
    {
        var before = DateTimeOffset.UtcNow;
        var actual = new UserContext().BusinessTime;
        var after = DateTimeOffset.UtcNow;

        Assert.InRange(actual, before, after);
    }
}
