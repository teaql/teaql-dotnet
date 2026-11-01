using TeaQL.Runtime;

internal static class OperationalClockProbe
{
    private sealed class UnavailableBusinessClock : IBusinessClock
    {
        public int Reads { get; private set; }
        public DateTimeOffset Now
        {
            get { Reads++; throw new InvalidOperationException("synthetic unavailable business time"); }
        }
    }

    public static async Task RunAsync()
    {
        static void Require(bool condition) { if (!condition) throw new InvalidOperationException("operational clock example failed"); }
        var future = new DateTimeOffset(2132, 2, 29, 10, 15, 30, TimeSpan.Zero);
        var context = new UserContext().WithBusinessClock(new FixedBusinessClock(future))
            .WithEntityReferenceCodec(new AeadEntityReferenceCodec(1,
                new Dictionary<uint, byte[]> { [1] = Enumerable.Repeat((byte)0x51, 32).ToArray() }));
        Require(context.BusinessTime == future);
        var before = DateTimeOffset.UtcNow;
        var token = context.EncodeEntityReference("OrderItem", 42, 7, "example-clock", TimeSpan.FromMinutes(1));
        var claims = context.DecodeEntityReference(token, "OrderItem", "example-clock");
        Require(claims.IssuedAt >= before.AddSeconds(-1) && claims.IssuedAt <= DateTimeOffset.UtcNow);
        var broken = new UnavailableBusinessClock();
        context.WithBusinessClock(broken);
        var calibrated = false;
        try { _ = context.BusinessTime; }
        catch (InvalidOperationException error) when (error.Message == "synthetic unavailable business time") { calibrated = true; }
        Require(calibrated && broken.Reads == 1);
        Require(context.DecodeEntityReference(token, "OrderItem", "example-clock").Id == 42);
        var key = "operational-example-" + Guid.NewGuid();
        try
        {
            context.PutToLocalCache(key, Array.Empty<int>(), 1);
            Require(context.GetFromLocalCache<int[]>(key) is { Length: 0 });
            await Task.Delay(1100);
            Require(context.GetFromLocalCache<int[]>(key) is null);
            context.WithBusinessClock(new FixedBusinessClock(future.AddYears(-200)));
            Require(context.GetFromLocalCache<int[]>(key) is null && broken.Reads == 1);
        }
        finally { context.RemoveFromLocalCache(key); }
        Console.WriteLine("DOTNET_OPERATIONAL_CLOCK_EXAMPLE={\"passed\":true,\"systemIssueTime\":true,\"unreadableBusinessCalibrated\":true,\"emptyHitIsNotMissing\":true,\"reallyExpired\":true,\"backwardCannotRevive\":true,\"additionalBusinessReads\":0}");
    }
}
