using Xunit;

namespace TeaQL.Telemetry.OpenTelemetry.Tests;

public class AdmissionTests
{
    [Fact]
    public async Task NewerCompletionCannotSubstituteForAnOlderPrefix()
    {
        var queue = new Admission("traces", new(), 4);
        Assert.True(queue.Submit("older", () => true)); var prefix = queue.Prefix;
        Assert.True(queue.Submit("newer", () => true));
        Assert.True(queue.BeginExport("older")); Assert.True(queue.BeginExport("newer"));
        var observed = queue.Observe(prefix, TimeSpan.FromSeconds(2));
        queue.Complete(["newer"]); Assert.False(observed.IsCompleted);
        queue.Complete(["older"]); Assert.True(await observed);
    }
    [Fact]
    public async Task FailedHandoffDoesNotEnterTheAcceptedPrefix()
    {
        var health = new ExportHealthState(); var queue = new Admission("logs", health, 1);
        Assert.False(queue.Submit("failed", () => false)); Assert.Equal(0, queue.Prefix);
        Assert.False(queue.Submit("throwing", () => throw new InvalidOperationException("PRIVATE-HANDOFF-CANARY")));
        Assert.Equal(0, queue.Prefix); Assert.Equal(2, queue.Snapshot.HandoffFailed);
        Assert.True(await queue.Observe(queue.Prefix, TimeSpan.Zero));
        Assert.True(queue.Submit("accepted", () => true)); Assert.Equal(1, queue.Prefix);
        Assert.Equal(2, health.QueueLosses["logs:handoff_failure"]);
    }
    [Fact]
    public void ExportingAndQueuedRecordsUseTheSameBudget()
    {
        var queue = new Admission("traces", new(), 2); var invoked = false;
        Assert.True(queue.Submit("exporting", () => true)); Assert.True(queue.BeginExport("exporting"));
        Assert.True(queue.Submit("queued", () => true));
        Assert.False(queue.Submit("overflow", () => { invoked = true; return true; })); Assert.False(invoked);
        Assert.Equal(2, queue.Snapshot.Outstanding); Assert.Equal(1, queue.Snapshot.Exporting);
        queue.Complete(["exporting"]); Assert.True(queue.Submit("recovered", () => true));
        Assert.Equal(2, queue.Snapshot.PeakOutstanding);
    }
    [Fact]
    public async Task RetirementExcludesAttemptedRecordsAndCompletesOnlyOnce()
    {
        var health = new ExportHealthState(); var queue = new Admission("logs", health, 3);
        Assert.True(queue.Submit("exporting", () => true)); Assert.True(queue.BeginExport("exporting"));
        Assert.True(queue.Submit("queued", () => true)); var prefix = queue.Prefix;
        var observed = queue.Observe(prefix, TimeSpan.FromSeconds(2)); queue.Close(); queue.RetireQueued(); queue.RetireQueued();
        Assert.False(observed.IsCompleted); Assert.False(queue.BeginExport("queued"));
        queue.Complete(["exporting", "exporting", "queued"]); Assert.True(await observed);
        Assert.Equal(1, queue.Snapshot.Completed); Assert.Equal(1, queue.Snapshot.Retired);
        Assert.Equal(0, queue.Snapshot.Outstanding);
        Assert.False(queue.Submit("late", () => true)); Assert.Equal(1, queue.Snapshot.PostShutdown);
        Assert.Equal(2, health.QueueLosses["logs:shutdown"]);
    }
    [Fact]
    public void DuplicateHandoffAndExportCannotReleaseAnotherRecordsPermit()
    {
        var queue = new Admission("traces", new(), 2);
        Assert.True(queue.Submit("first", () => true)); Assert.False(queue.Submit("first", () => true));
        Assert.True(queue.Submit("second", () => true));
        Assert.True(queue.BeginExport("first")); Assert.False(queue.BeginExport("first"));
        queue.Complete(["first", "first", "unknown"]);
        Assert.Equal(1, queue.Snapshot.Outstanding); Assert.Equal(1, queue.Snapshot.Completed); Assert.Equal(1, queue.Snapshot.HandoffFailed);
    }
}
