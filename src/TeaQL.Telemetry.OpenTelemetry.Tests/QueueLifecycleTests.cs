using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using TeaQL.Runtime;
using Xunit;

namespace TeaQL.Telemetry.OpenTelemetry.Tests;

[Collection("Owned SDK")]
public class QueueLifecycleTests
{
    private static StandardTelemetry Pipeline(BaseExporter<Activity> spans, BaseExporter<LogRecord> logs) =>
        new(new(true, "queue-lifecycle", "http://localhost:4318"), spans, logs, new InMemoryExporter<Metric>(new List<Metric>()));

    [Fact]
    public async Task FlushIncludesAlreadyDequeuedButStillExportingRecords()
    {
        using var spans = new Held<Activity>(); using var logs = new Held<LogRecord>(); var pipeline = Pipeline(spans, logs);
        try {
            for (var index = 0; index < 128; index++) pipeline.Start(RuntimeOperation.Create("query", "held-prefix")).Success();
            Assert.True(spans.Started.Wait(TimeSpan.FromSeconds(3))); Assert.True(logs.Started.Wait(TimeSpan.FromSeconds(3)));
            var flush = pipeline.FlushAsync(); await Task.Delay(150);
            Assert.False(flush.IsCompleted, "Dequeuing must not substitute for actual exporter completion");
            spans.Release.Set(); logs.Release.Set(); await flush.WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal(128, spans.Completed); Assert.Equal(128, logs.Completed);
        }
        finally { spans.Release.Set(); logs.Release.Set(); await pipeline.ShutdownAsync(); }
    }

    [Fact]
    public async Task ConcurrentShutdownCallersJoinTheSameCleanup()
    {
        using var spans = new Held<Activity>(); using var logs = new Held<LogRecord>(); var pipeline = Pipeline(spans, logs);
        try {
            for (var index = 0; index < 128; index++) pipeline.Start(RuntimeOperation.Create("query", "held-shutdown")).Success();
            Assert.True(spans.Started.Wait(TimeSpan.FromSeconds(3))); Assert.True(logs.Started.Wait(TimeSpan.FromSeconds(3)));
            var first = pipeline.ShutdownAsync(); var second = pipeline.ShutdownAsync(); await Task.Delay(150);
            Assert.False(second.IsCompleted, "A second shutdown must not report cleanup before the first one completes");
            spans.Release.Set(); logs.Release.Set(); await Task.WhenAll(first, second).WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal(128, spans.Completed); Assert.Equal(128, logs.Completed);
        }
        finally { spans.Release.Set(); logs.Release.Set(); await pipeline.ShutdownAsync(); }
    }

    [Fact]
    public async Task AllConcurrentFlushesWaitForTheSameActualExports()
    {
        using var spans = new Held<Activity>(); using var logs = new Held<LogRecord>(); var pipeline = Pipeline(spans, logs);
        try {
            for (var index = 0; index < 128; index++) pipeline.Start(RuntimeOperation.Create("query", "parallel-flush")).Success();
            Assert.True(spans.Started.Wait(TimeSpan.FromSeconds(3))); Assert.True(logs.Started.Wait(TimeSpan.FromSeconds(3)));
            var flushes = Enumerable.Range(0, 12).Select(_ => pipeline.FlushAsync()).ToArray();
            await Task.Delay(150); Assert.All(flushes, task => Assert.False(task.IsCompleted));
            spans.Release.Set(); logs.Release.Set(); await Task.WhenAll(flushes).WaitAsync(TimeSpan.FromSeconds(3));
            Assert.Equal(128, pipeline.ExportQueues["traces"].Completed); Assert.Equal(128, pipeline.ExportQueues["logs"].Completed);
        }
        finally { spans.Release.Set(); logs.Release.Set(); await pipeline.ShutdownAsync(); }
    }

    [Fact]
    public async Task NativeSdkSnapshotsRespectTheDefaultQuotaAndRecoverCapacity()
    {
        using var spans = new Held<Activity>(); using var logs = new Held<LogRecord>(); var pipeline = Pipeline(spans, logs);
        try {
            for (var index = 0; index < 128; index++) pipeline.Start(RuntimeOperation.Create("query", "quota")).Success();
            Assert.True(spans.Started.Wait(TimeSpan.FromSeconds(3))); Assert.True(logs.Started.Wait(TimeSpan.FromSeconds(3)));
            Parallel.For(0, 4096, _ => pipeline.Start(RuntimeOperation.Create("query", "quota")).Success());
            foreach (var signal in new[] { "traces", "logs" }) {
                var queue = pipeline.ExportQueues[signal]; Assert.Equal(4224, queue.Offered);
                Assert.Equal(2048, queue.Admitted); Assert.Equal(2048, queue.Outstanding); Assert.Equal(2176, queue.Overflow);
                Assert.Equal(128, queue.Exporting); Assert.Equal(0, queue.HandoffFailed);
            }
            spans.Release.Set(); logs.Release.Set(); await pipeline.FlushAsync();
            Assert.Equal(2048, spans.Completed); Assert.Equal(2048, logs.Completed);
            pipeline.Start(RuntimeOperation.Create("query", "recovered")).Success(); await pipeline.FlushAsync();
            foreach (var signal in new[] { "traces", "logs" }) {
                var queue = pipeline.ExportQueues[signal]; Assert.Equal(2049, queue.Completed);
                Assert.Equal(0, queue.Outstanding); Assert.Equal(2048, queue.PeakOutstanding);
            }
        }
        finally { spans.Release.Set(); logs.Release.Set(); await pipeline.ShutdownAsync(); }
    }

    [Fact]
    public async Task LateScopesCountLossWithoutAdvertisingSqlCorrelationOrExporting()
    {
        using var spans = new Held<Activity>(); using var logs = new Held<LogRecord>(); spans.Release.Set(); logs.Release.Set();
        var pipeline = Pipeline(spans, logs); var existing = pipeline.Start(RuntimeOperation.Create("query", "before-close"));
        await pipeline.ShutdownAsync(); existing.Success();
        using var statement = pipeline.StartStatement("sqlite", TeaQL.DataService.DataServiceOperation.Query);
        Assert.Null(statement.Correlation); statement.Success();
        var late = pipeline.Start(RuntimeOperation.Create("query", "late")); late.Success(); ((IDisposable)late).Dispose();
        ((IDisposable)pipeline.Start(RuntimeOperation.Create("query", "abandoned"))).Dispose();
        Assert.Equal(4, pipeline.ExportQueues["traces"].PostShutdown); Assert.Equal(3, pipeline.ExportQueues["logs"].PostShutdown);
        Assert.Equal(0, spans.Completed); Assert.Equal(0, logs.Completed);
        Assert.Equal(0, pipeline.ExportQueues["traces"].Outstanding); Assert.Equal(0, pipeline.ExportQueues["logs"].Outstanding);
    }

    private sealed class Held<T> : BaseExporter<T> where T : class
    {
        internal readonly ManualResetEventSlim Started = new(false), Release = new(false);
        internal long Completed;
        public override ExportResult Export(in Batch<T> batch) {
            long count = 0; foreach (var record in batch) count++;
            Started.Set(); Release.Wait(); Interlocked.Add(ref Completed, count); return ExportResult.Success;
        }
    }
}
