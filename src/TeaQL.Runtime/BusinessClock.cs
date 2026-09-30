namespace TeaQL.Runtime;

/// <summary>
/// Supplies time for domain behavior and Checker/Fix decisions. Operational
/// clocks such as cache expiry and telemetry duration are intentionally separate.
/// </summary>
public interface IBusinessClock
{
    DateTimeOffset Now { get; }
}

public sealed class SystemBusinessClock : IBusinessClock
{
    public static SystemBusinessClock Instance { get; } = new();

    private SystemBusinessClock() { }

    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}

public sealed class FixedBusinessClock(DateTimeOffset value) : IBusinessClock
{
    public DateTimeOffset Now { get; } = value;
}
