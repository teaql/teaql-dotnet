using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class FixEvidenceInvocationTests
{
    private static InsertMutationRequest Request() => new(new InsertCommand("ClockNode"), "observe standalone Checker invocation");
    private static FixEvidence Evidence(string label) => new("ClockNode", "create_time", FixEvidenceSource.Clock, label);

    [Fact]
    public void StandaloneReceiptsAreBoundedImmutableAndInvocationScoped()
    {
        var clock = new AdvancingClock(); var times = new List<long>(); var count = 0;
        var context = new UserContext().WithBusinessClock(clock).Install(new RuntimeModule().Checker("ClockNode",
            new DelegateChecker((ctx, _, now) => { times.Add(now.ToUnixTimeMilliseconds()); ctx.RecordFixEvidence(Evidence("fix-" + ++count)); })));
        context.CheckAndFix(Request()); var previous = context.LastFixEvidence;
        context.CheckAndFix(Request());
        Assert.Equal(new[] { "fix-1" }, previous.Select(e => e.SourceLabel));
        Assert.Equal(new[] { "fix-2" }, context.LastFixEvidence.Select(e => e.SourceLabel));
        Assert.Equal(new long[] { 1961662530000, 1961662530001 }, times); Assert.Equal(2, clock.Calls);
        Assert.Null(CheckerFixInvocation.Current(context));
        Assert.Throws<NotSupportedException>(() => ((IList<FixEvidence>)previous)[0] = Evidence("mutated"));
    }

    [Fact]
    public void AStandaloneBatchSharesOneClockAcrossLeaves()
    {
        var clock = new AdvancingClock(); var times = new List<long>();
        var context = new UserContext().WithBusinessClock(clock).Install(new RuntimeModule().Checker("ClockNode",
            new DelegateChecker((ctx, _, now) => { times.Add(now.ToUnixTimeMilliseconds()); ctx.RecordFixEvidence(Evidence("leaf")); })));
        context.CheckAndFix(new BatchMutationRequest([Request(), Request()], "check one batch"));
        Assert.Equal(new long[] { 1961662530000, 1961662530000 }, times); Assert.Equal(1, clock.Calls);
        Assert.Equal(2, context.LastFixEvidence.Count); Assert.Null(CheckerFixInvocation.Current(context));
    }

    [Fact]
    public void AnIndependentNestedRequestRestoresTheOuterInvocation()
    {
        var clock = new AdvancingClock(); var depth = 0; var times = new List<long>();
        IReadOnlyList<FixEvidence>? innerReceipt = null;
        var context = new UserContext().WithBusinessClock(clock);
        context.Install(new RuntimeModule().Checker("ClockNode", new DelegateChecker((ctx, _, now) =>
        {
            times.Add(now.ToUnixTimeMilliseconds());
            if (depth++ == 0)
            {
                ctx.RecordFixEvidence(Evidence("outer-before"));
                var outer = CheckerFixInvocation.Current(ctx);
                ctx.CheckAndFix(Request()); innerReceipt = ctx.LastFixEvidence;
                Assert.Same(outer, CheckerFixInvocation.Current(ctx));
                ctx.RecordFixEvidence(Evidence("outer-after"));
            }
            else ctx.RecordFixEvidence(Evidence("inner"));
        })));
        context.CheckAndFix(Request());
        Assert.Equal(new long[] { 1961662530000, 1961662530001 }, times); Assert.Equal(2, clock.Calls);
        Assert.Equal(new[] { "inner" }, innerReceipt!.Select(e => e.SourceLabel));
        Assert.Equal(new[] { "outer-before", "outer-after" }, context.LastFixEvidence.Select(e => e.SourceLabel));
        Assert.Null(CheckerFixInvocation.Current(context));
    }

    [Fact]
    public void FailurePublishesOnlyItsOwnReceiptAndRetryCapturesFreshTime()
    {
        var original = new InvalidOperationException("checker failure"); var calls = 0; var clock = new AdvancingClock();
        var context = new UserContext().WithBusinessClock(clock).Install(new RuntimeModule().Checker("ClockNode",
            new DelegateChecker((ctx, _, _) => { ctx.RecordFixEvidence(Evidence("attempt-" + ++calls)); if (calls == 1) throw original; })));
        Assert.Same(original, Assert.Throws<InvalidOperationException>(() => context.CheckAndFix(Request())));
        var failedReceipt = context.LastFixEvidence; Assert.Null(CheckerFixInvocation.Current(context));
        context.CheckAndFix(Request());
        Assert.Equal(new[] { "attempt-1" }, failedReceipt.Select(e => e.SourceLabel));
        Assert.Equal(new[] { "attempt-2" }, context.LastFixEvidence.Select(e => e.SourceLabel));
        Assert.Equal(2, clock.Calls); Assert.Null(CheckerFixInvocation.Current(context));
    }

    [Fact]
    public void ClockFailureReleasesTheScopeWithoutReplacingTheOriginalFailure()
    {
        var clock = new AdvancingClock { FailFirst = true }; var context = new UserContext().WithBusinessClock(clock)
            .Install(new RuntimeModule().Checker("ClockNode", new DelegateChecker((ctx, _, _) => ctx.RecordFixEvidence(Evidence("retry")))));
        Assert.Same(clock.Original, Assert.Throws<InvalidOperationException>(() => context.CheckAndFix(Request())));
        Assert.Null(CheckerFixInvocation.Current(context)); context.CheckAndFix(Request());
        Assert.Equal(2, clock.Calls); Assert.Single(context.LastFixEvidence); Assert.Null(CheckerFixInvocation.Current(context));
    }

    [Fact]
    public void EmptyOrRejectedAdmissionDoesNotReadTheClockOrReplaceThePreviousReceipt()
    {
        var clock = new AdvancingClock(); var context = new UserContext().WithBusinessClock(clock)
            .Install(new RuntimeModule().Checker("ClockNode", new DelegateChecker((ctx, _, _) => ctx.RecordFixEvidence(Evidence("before")))));
        context.CheckAndFix(Request()); var previous = context.LastFixEvidence;
        context.CheckAndFix(new BatchMutationRequest([], "empty batch has no Checker work"));
        var invalid = new UpdateCommand("ClockNode", new Value.I64Value(1)) { ExpectedVersionValue = 0 };
        Assert.Throws<CheckException>(() => context.CheckAndFix(new UpdateMutationRequest(invalid, "reject invalid version before Fix")));
        Assert.Equal(1, clock.Calls); Assert.Same(previous, context.LastFixEvidence); Assert.Null(CheckerFixInvocation.Current(context));
    }

    [Fact]
    public void IndependentContextsNeverBorrowTheEnclosingStandaloneClockOrEvidence()
    {
        var firstClock = new AdvancingClock(); var secondClock = new AdvancingClock();
        var second = new UserContext().WithBusinessClock(secondClock).Install(new RuntimeModule().Checker("ClockNode",
            new DelegateChecker((ctx, _, _) => ctx.RecordFixEvidence(Evidence("second")))));
        var first = new UserContext().WithBusinessClock(firstClock).Install(new RuntimeModule().Checker("ClockNode",
            new DelegateChecker((ctx, _, now) =>
            {
                ctx.RecordFixEvidence(Evidence("first-before")); var owner = CheckerFixInvocation.Current(ctx);
                second.CheckAndFix(Request()); Assert.Same(owner, CheckerFixInvocation.Current(ctx));
                Assert.Equal(now, owner!.Now); ctx.RecordFixEvidence(Evidence("first-after"));
            })));
        first.CheckAndFix(Request());
        Assert.Equal(new[] { "first-before", "first-after" }, first.LastFixEvidence.Select(e => e.SourceLabel));
        Assert.Equal(new[] { "second" }, second.LastFixEvidence.Select(e => e.SourceLabel));
        Assert.Equal(1, firstClock.Calls); Assert.Equal(1, secondClock.Calls);
        Assert.Null(CheckerFixInvocation.Current(first)); Assert.Null(CheckerFixInvocation.Current(second));
    }

    [Fact]
    public async Task AnInheritedClosedScopeCannotBeReusedByLaterAsyncWork()
    {
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var clock = new AdvancingClock(); Task? worker = null; var calls = 0;
        var context = new UserContext().WithBusinessClock(clock);
        context.Install(new RuntimeModule().Checker("ClockNode", new DelegateChecker((ctx, _, _) =>
        {
            ctx.RecordFixEvidence(Evidence("fix-" + ++calls));
            if (calls == 1) worker = Task.Run(async () =>
            {
                await release.Task; Assert.Null(CheckerFixInvocation.Current(ctx)); ctx.CheckAndFix(Request());
            });
        })));
        context.CheckAndFix(Request()); var previous = context.LastFixEvidence;
        release.SetResult(); await worker!.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Equal(new[] { "fix-1" }, previous.Select(e => e.SourceLabel));
        Assert.Equal(new[] { "fix-2" }, context.LastFixEvidence.Select(e => e.SourceLabel));
        Assert.Equal(2, clock.Calls); Assert.Null(CheckerFixInvocation.Current(context));
    }

    [Fact]
    public async Task ConcurrentStandaloneInvocationsOnOneContextKeepTheirOwnClockAndReceipt()
    {
        using var entered = new ManualResetEventSlim(); using var release = new ManualResetEventSlim();
        var clock = new AdvancingClock(); var context = new UserContext().WithBusinessClock(clock);
        context.Install(new RuntimeModule().Checker("ClockNode", new DelegateChecker((ctx, _, now) =>
        {
            var first = now.ToUnixTimeMilliseconds() == 1961662530000;
            ctx.RecordFixEvidence(Evidence(first ? "first" : "second"));
            if (first) { entered.Set(); Assert.True(release.Wait(TimeSpan.FromSeconds(10))); }
            Assert.Equal(now, CheckerFixInvocation.Current(ctx)!.Now);
        })));
        var worker = Task.Run(() => context.CheckAndFix(Request()));
        IReadOnlyList<FixEvidence>? secondReceipt = null;
        try
        {
            Assert.True(entered.Wait(TimeSpan.FromSeconds(10))); context.CheckAndFix(Request());
            secondReceipt = context.LastFixEvidence;
            Assert.Equal(new[] { "second" }, secondReceipt.Select(e => e.SourceLabel));
        }
        finally { release.Set(); await worker.WaitAsync(TimeSpan.FromSeconds(10)); }
        Assert.Equal(new[] { "second" }, secondReceipt!.Select(e => e.SourceLabel));
        Assert.Equal(new[] { "first" }, context.LastFixEvidence.Select(e => e.SourceLabel));
        Assert.Equal(2, clock.Calls); Assert.Null(CheckerFixInvocation.Current(context));
    }

    [Fact]
    public void AClosedNestedForkCannotFallThroughToTheStillActiveParent()
    {
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task? worker = null; var depth = 0; var clock = new AdvancingClock();
        var context = new UserContext().WithBusinessClock(clock);
        context.Install(new RuntimeModule().Checker("ClockNode", new DelegateChecker((ctx, _, _) =>
        {
            if (depth++ == 0)
            {
                ctx.RecordFixEvidence(Evidence("outer-before")); ctx.CheckAndFix(Request());
                release.SetResult(); worker!.WaitAsync(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
                ctx.RecordFixEvidence(Evidence("outer-after"));
            }
            else
            {
                ctx.RecordFixEvidence(Evidence("inner"));
                worker = Task.Run(async () => { await release.Task; Assert.Null(CheckerFixInvocation.Current(ctx)); });
            }
        })));
        context.CheckAndFix(Request());
        Assert.Equal(new[] { "outer-before", "outer-after" }, context.LastFixEvidence.Select(e => e.SourceLabel));
        Assert.Null(CheckerFixInvocation.Current(context)); Assert.Equal(2, clock.Calls);
    }

    [Fact]
    public void CancellationReleasesItsInvocationAndPreservesTheExactError()
    {
        using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
        var original = new OperationCanceledException(cancelled.Token); var count = 0; var clock = new AdvancingClock();
        var context = new UserContext().WithBusinessClock(clock).Install(new RuntimeModule().Checker("ClockNode",
            new DelegateChecker((ctx, _, _) => { ctx.RecordFixEvidence(Evidence("attempt")); if (++count == 1) throw original; })));
        Assert.Same(original, Assert.Throws<OperationCanceledException>(() => context.CheckAndFix(Request())));
        Assert.Null(CheckerFixInvocation.Current(context)); context.CheckAndFix(Request());
        Assert.Single(context.LastFixEvidence); Assert.Equal(2, clock.Calls); Assert.Null(CheckerFixInvocation.Current(context));
    }

    private sealed class DelegateChecker(Action<UserContext, MutationRequest, DateTimeOffset> callback) : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        { callback(context, request, now); return []; }
    }
    private sealed class AdvancingClock : IBusinessClock
    {
        public int Calls; public bool FailFirst;
        public InvalidOperationException Original { get; } = new("clock failure");
        public DateTimeOffset Now
        {
            get { var i = Interlocked.Increment(ref Calls) - 1; if (FailFirst && i == 0) throw Original; return DateTimeOffset.FromUnixTimeMilliseconds(1961662530000 + i); }
        }
    }
}
