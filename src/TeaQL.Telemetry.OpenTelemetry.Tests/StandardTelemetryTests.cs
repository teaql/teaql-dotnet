using System.Collections.Concurrent;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using TeaQL.DataService;
using TeaQL.Runtime;
using Xunit;

namespace TeaQL.Telemetry.OpenTelemetry.Tests;

[CollectionDefinition("Owned SDK", DisableParallelization = true)]
public class OwnedSdkCollection { }
[Collection("Owned SDK")]
public class StandardTelemetryTests
{
    private static TelemetryStartupConfiguration Config => new(true, "dotnet-standard-test", "http://localhost:4318");
    [Fact]
    public async Task RemoteParentIsInheritedAndRestoredWithoutChangingGlobalPropagation()
    {
        var spans = new List<Activity>(); var pipeline = new StandardTelemetry(Config, new InMemoryExporter<Activity>(spans), new Logs(), new InMemoryExporter<Metric>(new List<Metric>()));
        try {
            using (pipeline.ExtractAndActivate(new Dictionary<string, string> { ["TraceParent"] = "00-11111111111111111111111111111111-2222222222222222-00" }))
                pipeline.Start(RuntimeOperation.Create("query", "remote-request")).Success();
            pipeline.Start(RuntimeOperation.Create("query", "independent-request")).Success();
            await pipeline.FlushAsync();
            var remote = Assert.Single(spans.Where(span => Equals(span.GetTagItem("teaql.operation.name"), "remote-request")));
            Assert.Equal("11111111111111111111111111111111", remote.TraceId.ToHexString()); Assert.Equal("2222222222222222", remote.ParentSpanId.ToHexString());
            var next = Assert.Single(spans.Where(span => Equals(span.GetTagItem("teaql.operation.name"), "independent-request")));
            Assert.NotEqual(remote.TraceId, next.TraceId);
        }
        finally { await pipeline.ShutdownAsync(); }
    }
    [Fact]
    public async Task ActualOtlpExporterCarriesConfiguredResourceInEverySignal()
    {
        var http = new RecordingHttp();
        var pipeline = new StandardTelemetry(Config, null, null, null, signal => new OtlpExporterOptions {
            Endpoint = new Uri("http://localhost:4318/v1/" + signal), Protocol = OtlpExportProtocol.HttpProtobuf,
            HttpClientFactory = () => new HttpClient(http, disposeHandler: false), TimeoutMilliseconds = 1000
        });
        try {
            pipeline.Start(RuntimeOperation.Create("query", "resource-test")).Success();
            await pipeline.FlushAsync();
            foreach (var signal in new[] { "traces", "metrics", "logs" })
                Assert.Contains(http.Rows, row => row.Path == "/v1/" + signal && System.Text.Encoding.UTF8.GetString(row.Bytes).Contains(Config.ServiceName));
        }
        finally { await pipeline.ShutdownAsync(); http.Dispose(); }
    }
    [Fact]
    public async Task FullRecordingThreeSignalsAndExactOutOfOrderLogIdentity()
    {
        var spans = new List<Activity>(); var logs = new Logs(); var metrics = new List<Metric>();
        var pipeline = new StandardTelemetry(Config, new InMemoryExporter<Activity>(spans), logs, new InMemoryExporter<Metric>(metrics));
        try {
            using var parent = new Activity("unsampled-parent").SetParentId("00-11111111111111111111111111111111-2222222222222222-00").Start();
            var first = pipeline.StartStatement("sqlite", DataServiceOperation.Query);
            var second = pipeline.StartStatement("sqlite", DataServiceOperation.Query);
            Assert.NotEqual(first.Correlation, second.Correlation);
            second.Success(); first.Success();
            using var unrelated = new Activity("unrelated-active-span").Start();
            pipeline.RecordDiagnostic(new(second.Correlation!, new Dictionary<string, object> { ["teaql.sql.debug"] = "SELECT second" }));
            pipeline.RecordDiagnostic(new(first.Correlation!, new Dictionary<string, object> { ["teaql.sql.debug"] = "SELECT first" }));
            await pipeline.FlushAsync();
            Assert.Equal(2, spans.Count); Assert.All(spans, span => Assert.True(span.Recorded));
            Assert.Contains(metrics, metric => metric.Name == "teaql.runtime.operation.count");
            var sql = logs.Rows.Where(row => row.Body == "TeaQL SQL execution").ToArray(); Assert.Equal(2, sql.Length);
            Assert.Contains(sql, row => row.Span == first.Correlation!.SpanId && row.Trace == first.Correlation.TraceId && Equals(row.Values["teaql.sql.debug"], "SELECT first"));
            Assert.Contains(sql, row => row.Span == second.Correlation!.SpanId && Equals(row.Values["teaql.sql.debug"], "SELECT second"));
            Assert.All(spans, span => Assert.DoesNotContain(span.TagObjects, tag => tag.Key.Contains("internal")));
            Assert.All(logs.Rows, row => Assert.DoesNotContain(row.Values.Keys, key => key.Contains("internal")));
        }
        finally { await pipeline.ShutdownAsync(); }
    }
    [Fact]
    public async Task ExistingSdkRemainsUsableAndOwnedProviderDoesNotExportForeignSources()
    {
        var customerSpans = new List<Activity>(); var ownedSpans = new List<Activity>();
        using var customer = Sdk.CreateTracerProviderBuilder().AddSource("io.teaql.runtime").SetSampler(new AlwaysOnSampler()).AddInMemoryExporter(customerSpans).Build();
        using var foreign = new ActivitySource("io.teaql.runtime");
        var pipeline = new StandardTelemetry(Config, new InMemoryExporter<Activity>(ownedSpans), new Logs(), new InMemoryExporter<Metric>(new List<Metric>()));
        using (var span = foreign.StartActivity("customer-before")) { }
        pipeline.Start(RuntimeOperation.Create("query", "owned-only")).Success();
        await pipeline.FlushAsync();
        Assert.Single(ownedSpans); Assert.Equal("teaql.query", ownedSpans[0].OperationName);
        await pipeline.ShutdownAsync();
        using (var span = foreign.StartActivity("customer-after")) { }
        customer.ForceFlush();
        Assert.Contains(customerSpans, span => span.OperationName == "customer-after");
    }
    [Fact]
    public async Task SeparateOwnedPipelinesDoNotCrossExport()
    {
        var firstSpans = new List<Activity>(); var secondSpans = new List<Activity>();
        var first = new StandardTelemetry(Config, new InMemoryExporter<Activity>(firstSpans), new Logs(), new InMemoryExporter<Metric>(new List<Metric>()));
        var second = new StandardTelemetry(Config with { ServiceName = "second-service" }, new InMemoryExporter<Activity>(secondSpans), new Logs(), new InMemoryExporter<Metric>(new List<Metric>()));
        try {
            first.Start(RuntimeOperation.Create("query", "first-service")).Success();
            second.Start(RuntimeOperation.Create("query", "second-service")).Success();
            await first.FlushAsync(); await second.FlushAsync();
            Assert.Single(firstSpans); Assert.Single(secondSpans);
            Assert.Equal("first-service", firstSpans[0].GetTagItem("teaql.operation.name"));
            Assert.Equal("second-service", secondSpans[0].GetTagItem("teaql.operation.name"));
        }
        finally { await first.ShutdownAsync(); await second.ShutdownAsync(); }
    }
    [Fact]
    public async Task ExportFailureIsCountedWithoutExportingExceptionTextOrFailingBusiness()
    {
        var pipeline = new StandardTelemetry(Config, new Failing<Activity>(), new Failing<LogRecord>(), new Failing<Metric>());
        var previous = Console.Error; var notices = new StringWriter(); Console.SetError(notices);
        try {
            var operation = pipeline.Start(RuntimeOperation.Create("query", "example"));
            operation.Failure(new InvalidOperationException("PRIVATE-DRIVER-MESSAGE"));
            var started = Stopwatch.StartNew(); await pipeline.ShutdownAsync();
            Assert.True(started.Elapsed < TimeSpan.FromSeconds(8));
            Assert.Equal(1, pipeline.ExportHealth.FailedTraceSpans); Assert.Equal(1, pipeline.ExportHealth.FailedLogRecords);
            Assert.True(pipeline.ExportHealth.FailedMetricObjects > 0);
            Assert.DoesNotContain("PRIVATE", notices.ToString()); Assert.Contains("delivery loss counted", notices.ToString());
        }
        finally { Console.SetError(previous); await pipeline.ShutdownAsync(); }
    }
    [Fact]
    public async Task BoundedQueueOverflowIsVisibleAndDoesNotBlockBusiness()
    {
        var blocked = new BlockingSpans(); var pipeline = new StandardTelemetry(Config, blocked, new Logs(), new InMemoryExporter<Metric>(new List<Metric>()));
        try {
            for (var i = 0; i < 128; i++) pipeline.Start(RuntimeOperation.Create("provider", "sqlite.statement")).Success();
            Assert.True(blocked.Started.Wait(TimeSpan.FromSeconds(3)));
            for (var i = 0; i < 4096; i++) pipeline.Start(RuntimeOperation.Create("provider", "sqlite.statement")).Success();
            Assert.True(pipeline.ExportHealth.DroppedTraceSpans > 0);
        }
        finally { blocked.Release.Set(); await pipeline.ShutdownAsync(); blocked.Dispose(); }
    }
    [Theory]
    [InlineData("http://secret:password@localhost")]
    [InlineData("http://localhost?token=secret")]
    public void InvalidSettingsDoNotCreateAnExporter(string endpoint)
    {
        var exception = Assert.Throws<ArgumentException>(() => new StandardTelemetry(Config with { Endpoint = endpoint }));
        Assert.Equal("invalid-telemetry-configuration", exception.Message);
    }
    internal sealed record LogRow(string? Body, string Trace, string Span, Dictionary<string, object?> Values);
    internal sealed class Logs : BaseExporter<LogRecord>
    {
        internal ConcurrentQueue<LogRow> Rows = new();
        public override ExportResult Export(in Batch<LogRecord> batch) {
            foreach (var row in batch) Rows.Enqueue(new(row.Body, row.TraceId.ToHexString(), row.SpanId.ToHexString(), (row.Attributes ?? []).ToDictionary(p => p.Key, p => p.Value)));
            return ExportResult.Success;
        }
    }
    private sealed class Failing<T> : BaseExporter<T> where T : class {
        public override ExportResult Export(in Batch<T> batch) => throw new InvalidOperationException("PRIVATE-EXPORTER-RESPONSE");
    }
    private sealed class BlockingSpans : BaseExporter<Activity> {
        internal ManualResetEventSlim Started = new(false), Release = new(false);
        public override ExportResult Export(in Batch<Activity> batch) { Started.Set(); Release.Wait(); return ExportResult.Success; }
    }
    private sealed class RecordingHttp : HttpMessageHandler {
        internal ConcurrentQueue<(string Path, byte[] Bytes)> Rows = new();
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) {
            Rows.Enqueue((request.RequestUri!.AbsolutePath, request.Content!.ReadAsByteArrayAsync(cancellationToken).GetAwaiter().GetResult()));
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new ByteArrayContent([]) };
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            Rows.Enqueue((request.RequestUri!.AbsolutePath, await request.Content!.ReadAsByteArrayAsync(cancellationToken)));
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new ByteArrayContent([]) };
        }
    }
}
