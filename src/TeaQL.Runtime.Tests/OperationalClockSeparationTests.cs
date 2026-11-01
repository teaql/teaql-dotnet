using System.Text.Json;
using TeaQL.Runtime;
using Xunit;
using Xunit.Abstractions;

namespace TeaQL.Runtime.Tests;

// Raw-reference opt-in is process-wide. Keep these cases out of parallel
// collections and restore the exact previous environment value on exit.
[CollectionDefinition("Operational clock environment", DisableParallelization = true)]
public sealed class OperationalClockEnvironmentCollection { }

[Collection("Operational clock environment")]
public sealed class OperationalClockSeparationTests(ITestOutputHelper output)
{
    private const string RawEnvironment = "TEAQL_UNSAFE_RAW_ENTITY_REFERENCES";
    private const string RawAcknowledgement = "I_UNDERSTAND_RAW_ENTITY_IDS_ARE_VISIBLE_FOR_LOCAL_DEVELOPMENT_ONLY";
    private static readonly DateTimeOffset BusinessInstant = new(2132, 2, 29, 10, 15, 30, TimeSpan.Zero);

    private sealed class CountingClock : IBusinessClock
    {
        public DateTimeOffset Value { get; set; } = BusinessInstant;
        public int Reads { get; private set; }
        public bool Broken { get; set; }
        public DateTimeOffset Now
        {
            get
            {
                Reads++;
                if (Broken) throw new InvalidOperationException("synthetic unavailable business time");
                return Value;
            }
        }
    }

    private static (UserContext Context, CountingClock Clock) Fixture(bool opaque = true)
    {
        var clock = new CountingClock();
        var context = new UserContext().WithBusinessClock(clock);
        Assert.Equal(BusinessInstant, context.BusinessTime); // Calibrate the installed clock.
        if (opaque)
            context.WithEntityReferenceCodec(new AeadEntityReferenceCodec(1,
                new Dictionary<uint, byte[]> { [1] = Enumerable.Repeat((byte)0x51, 32).ToArray() }));
        return (context, clock);
    }

    private void Observe(string name, CountingClock clock, int expectedReads, params string[] facts)
    {
        Assert.Equal(expectedReads, clock.Reads);
        output.WriteLine("DOTNET_OPERATIONAL_CLOCK_CASE=" + JsonSerializer.Serialize(new
        {
            @case = name, passed = true, businessReads = clock.Reads,
            facts = facts.ToDictionary(n => n, _ => true)
        }));
    }

    private static void Invalid(Action work) =>
        Assert.Equal("ENTITY_REFERENCE_INVALID", Assert.Throws<EntityReferenceTokenException>(work).Code);

    private sealed class RawScope : IDisposable
    {
        private readonly string? previous = Environment.GetEnvironmentVariable(RawEnvironment);
        public RawScope() => Environment.SetEnvironmentVariable(RawEnvironment, RawAcknowledgement);
        public void Dispose() => Environment.SetEnvironmentVariable(RawEnvironment, previous);
    }

    [Fact]
    public void ReferenceIssueUsesSystemTimeWithFutureBusinessClock()
    {
        var (context, clock) = Fixture();
        var before = DateTimeOffset.UtcNow;
        var token = context.EncodeEntityReference("OrderItem", 42, 7, "clock-probe", TimeSpan.FromMinutes(1));
        Assert.StartsWith("tqr1.", token);
        var claims = context.DecodeEntityReference(token, "OrderItem", "clock-probe");
        var after = DateTimeOffset.UtcNow;
        Assert.InRange(claims.IssuedAt, before.AddSeconds(-1), after);
        Assert.InRange(claims.ExpiresAt, before.AddMinutes(1).AddSeconds(-1), after.AddMinutes(1));
        Assert.Equal((ulong)42, claims.Id);
        Observe("reference-system-issue", clock, 1, "systemIssuedAt", "systemExpiresAt", "opaqueRoundtrip");
    }

    [Fact]
    public void ReferenceSurvivesForwardBusinessJump()
    {
        var (context, clock) = Fixture();
        var token = context.EncodeEntityReference("OrderItem", 42, 7, "clock-probe", TimeSpan.FromMinutes(1));
        clock.Value = clock.Value.AddYears(200);
        Assert.Equal((ulong)42, context.DecodeEntityReference(token, "OrderItem", "clock-probe").Id);
        Observe("reference-business-forward", clock, 1, "validAfterForwardJump");
    }

    [Fact]
    public async Task FrozenBusinessClockCannotPreventReferenceExpiryOrReviveIt()
    {
        var (context, clock) = Fixture();
        var token = context.EncodeEntityReference("OrderItem", 42, 7, "clock-probe", TimeSpan.FromSeconds(2));
        Assert.Equal((ulong)42, context.DecodeEntityReference(token, "OrderItem", "clock-probe").Id);
        await Task.Delay(2200);
        Assert.Equal(BusinessInstant, context.BusinessTime);
        Invalid(() => context.DecodeEntityReference(token, "OrderItem", "clock-probe"));
        clock.Value = clock.Value.AddYears(-200);
        Invalid(() => context.DecodeEntityReference(token, "OrderItem", "clock-probe"));
        Observe("reference-real-expiry", clock, 2, "initiallyValid", "businessTimeUnchanged", "reallyExpired", "backwardCannotRevive");
    }

    [Fact]
    public void UnreadableBusinessClockDoesNotBreakReferenceSecurity()
    {
        var (context, clock) = Fixture();
        clock.Broken = true;
        Assert.Equal("synthetic unavailable business time", Assert.Throws<InvalidOperationException>(() => { _ = context.BusinessTime; }).Message);
        var token = context.EncodeEntityReference("OrderItem", 42, 7, "clock-probe", TimeSpan.FromMinutes(1));
        Assert.Equal((ulong)42, context.DecodeEntityReference(token, "OrderItem", "clock-probe").Id);
        Observe("reference-unreadable-business", clock, 2, "unreadableCalibrated", "valid");
    }

    [Fact]
    public void ReferenceTypePurposeAndAuthenticationRemainEnforced()
    {
        var (context, clock) = Fixture();
        var token = context.EncodeEntityReference("OrderItem", 42, 7, "clock-probe", TimeSpan.FromMinutes(1));
        Invalid(() => context.DecodeEntityReference(token, "InvoiceItem", "clock-probe"));
        Invalid(() => context.DecodeEntityReference(token, "OrderItem", "different-purpose"));
        var tampered = token[..20] + (token[20] == 'A' ? 'B' : 'A') + token[21..];
        Invalid(() => context.DecodeEntityReference(tampered, "OrderItem", "clock-probe"));
        Observe("reference-bindings-and-tamper", clock, 1, "wrongTypeRejected", "wrongPurposeRejected", "tamperRejected");
    }

    [Fact]
    public void ExplicitRawBranchAlsoUsesSystemIssueTime()
    {
        using var raw = new RawScope();
        var (context, clock) = Fixture(opaque: false);
        var before = DateTimeOffset.UtcNow;
        var token = context.EncodeEntityReference("OrderItem", 42, 7, "clock-probe", TimeSpan.FromMinutes(1));
        Assert.StartsWith("tqr0.", token);
        var claims = context.DecodeEntityReference(token, "OrderItem", "clock-probe");
        var after = DateTimeOffset.UtcNow;
        Assert.InRange(claims.IssuedAt, before.AddSeconds(-1), after);
        Assert.InRange(claims.ExpiresAt, before.AddMinutes(1).AddSeconds(-1), after.AddMinutes(1));
        Observe("raw-system-issue", clock, 1, "rawBranchExercised", "systemIssuedAt", "systemExpiresAt");
    }

    [Fact]
    public async Task ExplicitRawBranchActuallyExpiresWithFrozenBusinessClock()
    {
        using var raw = new RawScope();
        var (context, clock) = Fixture(opaque: false);
        var token = context.EncodeEntityReference("OrderItem", 42, 7, "clock-probe", TimeSpan.FromSeconds(2));
        Assert.StartsWith("tqr0.", token);
        Assert.Equal((ulong)42, context.DecodeEntityReference(token, "OrderItem", "clock-probe").Id);
        await Task.Delay(2200);
        Assert.Equal(BusinessInstant, context.BusinessTime);
        Invalid(() => context.DecodeEntityReference(token, "OrderItem", "clock-probe"));
        clock.Value = clock.Value.AddYears(-200);
        Invalid(() => context.DecodeEntityReference(token, "OrderItem", "clock-probe"));
        Observe("raw-real-expiry", clock, 2, "rawBranchExercised", "reallyExpired", "backwardCannotRevive");
    }

    [Fact]
    public async Task LocalCacheUsesActualExpiryAndDoesNotRevive()
    {
        var (context, clock) = Fixture();
        var key = "operational-cache-" + Guid.NewGuid();
        try
        {
            context.PutToLocalCache(key, Array.Empty<int>(), 1);
            Assert.Empty(Assert.IsType<int[]>(context.GetFromLocalCache<int[]>(key)));
            clock.Value = clock.Value.AddYears(100);
            Assert.NotNull(context.GetFromLocalCache<int[]>(key));
            await Task.Delay(1100);
            Assert.Null(context.GetFromLocalCache<int[]>(key));
            clock.Value = clock.Value.AddYears(-200);
            Assert.Null(context.GetFromLocalCache<int[]>(key));
            Observe("local-cache-real-expiry", clock, 1, "emptyHitIsNotMissing", "futureKeepsLive", "reallyExpired", "backwardCannotRevive");
        }
        finally { context.RemoveFromLocalCache(key); }
    }

    [Fact]
    public async Task LocalCachePreservesZeroNilAndEmptyValuesAcrossRealExpiry()
    {
        var (context, clock) = Fixture();
        var prefix = "operational-cache-presence-" + Guid.NewGuid();
        var suffixes = new[] { "nil", "zero", "false", "empty", "control" };
        try
        {
            context.PutToLocalCache(prefix + "nil", "", null);
            context.PutToLocalCache(prefix + "zero", 0, 0);
            context.PutToLocalCache(prefix + "false", false);
            context.PutToLocalCache(prefix + "empty", Array.Empty<int>());
            context.PutToLocalCache(prefix + "control", "finite", 1);
            await Task.Delay(1100);
            Assert.Equal("", context.GetFromLocalCache<string>(prefix + "nil"));
            Assert.Equal(0, Assert.IsType<int>(context.GetFromLocalCache<object>(prefix + "zero")));
            Assert.False(Assert.IsType<bool>(context.GetFromLocalCache<object>(prefix + "false")));
            Assert.Empty(Assert.IsType<int[]>(context.GetFromLocalCache<object>(prefix + "empty")));
            Assert.Null(context.GetFromLocalCache<string>(prefix + "control"));
            Observe("local-cache-presence-and-ttl", clock, 1, "nilTTLRemains", "zeroTTLRemains", "zeroIsHit", "falseIsHit", "emptyIsHit", "finiteControlExpires");
        }
        finally { foreach (var suffix in suffixes) context.RemoveFromLocalCache(prefix + suffix); }
    }

    [Fact]
    public async Task LocalLockExpiryIgnoresUnavailableBusinessClockAndPreservesOwnership()
    {
        var (first, clock) = Fixture();
        var second = new UserContext().WithBusinessClock(clock);
        clock.Broken = true;
        Assert.Throws<InvalidOperationException>(() => { _ = first.BusinessTime; });
        var key = "operational-lock-" + Guid.NewGuid();
        try
        {
            Assert.True(first.TryLocalLock(key, 0, 1000));
            Assert.False(second.TryLocalLock(key, 0, 1000));
            await Task.Delay(1100);
            Assert.True(second.TryLocalLock(key, 0, 1000));
            first.UnlockLocal(key);
            Assert.False(first.TryLocalLock(key, 0, 1000));
            second.UnlockLocal(key);
            Assert.True(first.TryLocalLock(key, 0, 1000));
            Observe("local-lock-unreadable-business", clock, 2, "unreadableCalibrated", "liveLeaseProtected", "reallyExpired", "ownershipPreserved", "reacquired");
        }
        finally { first.UnlockLocal(key); second.UnlockLocal(key); }
    }
}
