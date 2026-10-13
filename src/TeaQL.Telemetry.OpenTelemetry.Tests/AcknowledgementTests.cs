using System.Collections.Concurrent;
using Google.Protobuf;
using OpenTelemetry.Exporter;
using OpenTelemetry.Proto.Collector.Logs.V1;
using OpenTelemetry.Proto.Collector.Metrics.V1;
using OpenTelemetry.Proto.Collector.Trace.V1;
using TeaQL.Runtime;
using Xunit;

namespace TeaQL.Telemetry.OpenTelemetry.Tests;

[Collection("Owned SDK")]
public class AcknowledgementTests
{
    [Theory]
    [InlineData("traces")]
    [InlineData("logs")]
    [InlineData("metrics")]
    public void EmptyDefaultAndUnknownFieldsDoNotInventRejections(string signal)
    {
        foreach (var body in new[] { Array.Empty<byte>(), new byte[] { 0x0a, 0x00 },
            new byte[] { 0x78, 0x01, 0x82, 0x01, 0x02, 0xff, 0xff } })
            Assert.Equal(new AckSummary(0, false), OtlpAcknowledgement.Decode(body, signal, 3));
    }

    [Fact]
    public void MetricSubmissionUsesDataPointsNotMetricObjects()
    {
        var metric = new global::OpenTelemetry.Proto.Metrics.V1.Metric {
            Name = "seven-points", Sum = new global::OpenTelemetry.Proto.Metrics.V1.Sum()
        };
        for (var index = 0; index < 7; index++)
            metric.Sum.DataPoints.Add(new global::OpenTelemetry.Proto.Metrics.V1.NumberDataPoint { AsInt = index });
        var scope = new global::OpenTelemetry.Proto.Metrics.V1.ScopeMetrics(); scope.Metrics.Add(metric);
        var resource = new global::OpenTelemetry.Proto.Metrics.V1.ResourceMetrics(); resource.ScopeMetrics.Add(scope);
        var request = new ExportMetricsServiceRequest(); request.ResourceMetrics.Add(resource);
        Assert.Equal(7, OtlpAcknowledgement.CountSubmitted(request.ToByteArray(), "metrics"));
        var response = new ExportMetricsServiceResponse { PartialSuccess = new ExportMetricsPartialSuccess { RejectedDataPoints = 7 } };
        Assert.Equal(new AckSummary(7, false), OtlpAcknowledgement.Decode(response.ToByteArray(), "metrics", 7));
        Assert.Throws<FormatException>(() => OtlpAcknowledgement.Decode(response.ToByteArray(), "metrics", 6));
    }

    private static StandardTelemetry Pipeline(AckHttp handler) => new(new(true, "dotnet-ack-test", "http://localhost:4318"),
        null, null, null, signal => new OtlpExporterOptions {
            Endpoint = new Uri("http://localhost:4318/v1/" + signal), Protocol = OtlpExportProtocol.HttpProtobuf,
            TimeoutMilliseconds = 1000, HttpClientFactory = () => new HttpClient(handler, false)
        });
    [Theory]
    [InlineData("malformed")]
    [InlineData("negative")]
    [InlineData("above-submitted")]
    [InlineData("wrong-wire")]
    [InlineData("deep-group")]
    [InlineData("invalid-utf8")]
    public async Task InvalidHttp200AcknowledgementsCountAsFailedExports(string mode)
    {
        using var handler = new AckHttp(mode); var pipeline = Pipeline(handler);
        try {
            pipeline.Start(RuntimeOperation.Create("query", "invalid-ack")).Success();
            await pipeline.FlushAsync();
            Assert.Equal(1, pipeline.ExportHealth.FailedTraceSpans);
            Assert.Equal(1, pipeline.ExportHealth.FailedLogRecords);
        }
        finally { await pipeline.ShutdownAsync(); }
    }
    [Theory]
    [InlineData("partial", "teaql.telemetry.export.rejected")]
    [InlineData("warning", "teaql.telemetry.export.response")]
    public async Task ReceiverRejectionAndWarningsHaveSeparateCumulativeMetrics(string mode, string metricName)
    {
        using var handler = new AckHttp(mode); var pipeline = Pipeline(handler);
        try {
            pipeline.Start(RuntimeOperation.Create("query", "partial-ack")).Success();
            await pipeline.FlushAsync(); await pipeline.FlushAsync();
            Assert.Equal(0, pipeline.ExportHealth.FailedTraceSpans); Assert.Equal(0, pipeline.ExportHealth.FailedLogRecords);
            if (mode == "partial") {
                Assert.Equal(1, pipeline.RejectedRecords["traces"]); Assert.Equal(1, pipeline.RejectedRecords["logs"]);
                Assert.Equal(14, pipeline.RejectedRecords["metrics"]);
            }
            else Assert.Empty(pipeline.RejectedRecords);
            var reason = mode == "partial" ? "partial_success" : "warning";
            Assert.Equal(1, pipeline.ExportResponses["traces:" + reason]);
            Assert.Equal(1, pipeline.ExportResponses["logs:" + reason]);
            Assert.Equal(2, pipeline.ExportResponses["metrics:" + reason]);
            Assert.DoesNotContain("PRIVATE-ACK-CANARY", System.Text.Json.JsonSerializer.Serialize(pipeline.ExportResponses));
            var metrics = handler.Requests.Where(row => row.Signal == "metrics")
                .Select(row => ExportMetricsServiceRequest.Parser.ParseFrom(row.Body)).ToArray();
            Assert.Contains(metrics.SelectMany(row => row.ResourceMetrics).SelectMany(row => row.ScopeMetrics).SelectMany(row => row.Metrics), row => row.Name == metricName);
        }
        finally { await pipeline.ShutdownAsync(); }
    }
    [Fact]
    public async Task FailedMetricExportCountsMetricObjectsNotOneBatch()
    {
        using var handler = new AckHttp("http-failure"); var pipeline = Pipeline(handler);
        try {
            pipeline.Start(RuntimeOperation.Create("query", "metric-units")).Success(); await pipeline.FlushAsync();
            var request = handler.Requests.First(row => row.Signal == "metrics");
            var submitted = ExportMetricsServiceRequest.Parser.ParseFrom(request.Body).ResourceMetrics
                .SelectMany(row => row.ScopeMetrics).Sum(row => row.Metrics.Count);
            Assert.True(submitted > 1); Assert.Equal(submitted, pipeline.ExportHealth.FailedMetricObjects);
        }
        finally { await pipeline.ShutdownAsync(); }
    }

    private sealed class AckHttp(string mode) : HttpMessageHandler
    {
        internal ConcurrentQueue<(string Signal, byte[] Body)> Requests = new();
        private HttpResponseMessage Response(HttpRequestMessage request, CancellationToken token) {
            var signal = request.RequestUri!.AbsolutePath.Split('/').Last();
            Requests.Enqueue((signal, request.Content!.ReadAsByteArrayAsync(token).GetAwaiter().GetResult()));
            var rejected = mode == "negative" ? -1 : mode == "above-submitted" ? 99999 : mode == "partial" ? (signal == "metrics" ? 7 : 1) : 0;
            var text = mode is "partial" or "warning" ? "PRIVATE-ACK-CANARY" : "";
            var body = mode switch {
                "malformed" => Enumerable.Repeat((byte)0xff, 8).ToArray(),
                "wrong-wire" => new byte[] { 0x08, 0x01 },
                "deep-group" => Enumerable.Repeat((byte)0x7b, 64).Concat(Enumerable.Repeat((byte)0x7c, 64)).ToArray(),
                "invalid-utf8" => new byte[] { 0x0a, 0x03, 0x12, 0x01, 0xff },
                _ => signal switch {
                    "traces" => new ExportTraceServiceResponse { PartialSuccess = new ExportTracePartialSuccess { RejectedSpans = rejected, ErrorMessage = text } }.ToByteArray(),
                    "logs" => new ExportLogsServiceResponse { PartialSuccess = new ExportLogsPartialSuccess { RejectedLogRecords = rejected, ErrorMessage = text } }.ToByteArray(),
                    _ => new ExportMetricsServiceResponse { PartialSuccess = new ExportMetricsPartialSuccess { RejectedDataPoints = rejected, ErrorMessage = text } }.ToByteArray()
                }
            };
            var content = new ByteArrayContent(body); content.Headers.ContentType = new("application/x-protobuf");
            return new(mode == "http-failure" ? System.Net.HttpStatusCode.ServiceUnavailable : System.Net.HttpStatusCode.OK) { Content = content };
        }
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken token) => Response(request, token);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => Task.FromResult(Response(request, token));
    }
}
