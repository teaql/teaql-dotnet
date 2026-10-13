using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using OpenTelemetry.Exporter;
using TeaQL.Runtime;
using Xunit;

namespace TeaQL.Telemetry.OpenTelemetry.Tests;

[Collection("Owned SDK")]
public class HttpExportPrivacyTests
{
    private const string Canary = "REMOTE-COLLECTOR-PRIVATE-CANARY";
    private static StandardTelemetry Pipeline(HttpMessageHandler handler) => new(
        new(true, "dotnet-private-http", "http://localhost:4318"), null, null, null,
        signal => new OtlpExporterOptions {
            Endpoint = new Uri("http://localhost:4318/v1/" + signal),
            Protocol = OtlpExportProtocol.HttpProtobuf, TimeoutMilliseconds = 1000, Headers = "",
            HttpClientFactory = () => new HttpClient(handler, disposeHandler: false)
        });

    [Fact]
    public async Task CustomerEnabledSdkDiagnosticsMustNotRevealCollectorBodyOrReason()
    {
        using var events = new ExporterEvents();
        using var handler = new ControlledHttp(503, Canary);
        var pipeline = Pipeline(handler);
        try {
            pipeline.Start(RuntimeOperation.Create("query", "private-http-failure")).Success();
            await pipeline.FlushAsync();
            Assert.Equal(1, pipeline.ExportHealth.FailedTraceSpans);
            Assert.Equal(1, pipeline.ExportHealth.FailedLogRecords);
            Assert.True(handler.Calls >= 3, "All real native exporters must reach the controlled receiver");
            Assert.NotEmpty(events.Events);
            Assert.DoesNotContain(Canary, string.Join("\n", events.Events));
        }
        finally { await pipeline.ShutdownAsync(); }
    }

    [Fact]
    public async Task OversizedHttp200AcknowledgementCannotBeReportedAsDelivered()
    {
        using var handler = new ControlledHttp(200, new string('x', 4 * 1024 * 1024 + 1));
        var pipeline = Pipeline(handler);
        try {
            pipeline.Start(RuntimeOperation.Create("query", "oversized-http-ack")).Success();
            await pipeline.FlushAsync();
            Assert.Equal(1, pipeline.ExportHealth.FailedTraceSpans);
            Assert.Equal(1, pipeline.ExportHealth.FailedLogRecords);
        }
        finally { await pipeline.ShutdownAsync(); }
    }

    [Fact]
    public async Task StreamingWithoutContentLengthStopsAtTheConfiguredByteLimit()
    {
        using var handler = new StreamingHttp();
        var pipeline = Pipeline(handler);
        try {
            pipeline.Start(RuntimeOperation.Create("query", "unknown-length-http-ack")).Success();
            await pipeline.FlushAsync();
            Assert.Equal(1, pipeline.ExportHealth.FailedTraceSpans);
            Assert.Equal(1, pipeline.ExportHealth.FailedLogRecords);
            Assert.NotEmpty(handler.Streams);
            Assert.All(handler.Streams, stream => {
                Assert.Equal(OwnedHttpTransport.MaximumResponseBytes + 1, stream.ReadBytes);
                Assert.True(stream.Disposed);
            });
        }
        finally { await pipeline.ShutdownAsync(); }
    }

    [Fact]
    public async Task OriginalTransportExceptionNeverEntersCustomerSdkDiagnostics()
    {
        using var events = new ExporterEvents();
        using var handler = new ThrowingHttp();
        var pipeline = Pipeline(handler);
        try {
            pipeline.Start(RuntimeOperation.Create("query", "private-transport-exception")).Success();
            await pipeline.FlushAsync();
            Assert.Equal(1, pipeline.ExportHealth.FailedTraceSpans);
            Assert.Equal(1, pipeline.ExportHealth.FailedLogRecords);
            Assert.DoesNotContain(Canary, string.Join("\n", events.Events));
        }
        finally { await pipeline.ShutdownAsync(); }
    }

    private sealed class ExporterEvents : EventListener
    {
        internal ConcurrentQueue<string> Events { get; } = new();
        protected override void OnEventSourceCreated(EventSource source) {
            if (source.Name == "OpenTelemetry-Exporter-OpenTelemetryProtocol")
                EnableEvents(source, EventLevel.Verbose, EventKeywords.All);
        }
        protected override void OnEventWritten(EventWrittenEventArgs data) {
            Events.Enqueue(string.Join(" ", data.Payload?.Select(value => value?.ToString()) ?? []));
        }
    }
    private sealed class ControlledHttp(int status, string body) : HttpMessageHandler
    {
        internal int Calls;
        private HttpResponseMessage Response() {
            Interlocked.Increment(ref Calls);
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(body));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
            return new((HttpStatusCode)status) { ReasonPhrase = Canary, Content = content };
        }
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) => Response();
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(Response());
    }
    private sealed class ThrowingHttp : HttpMessageHandler {
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) => throw new HttpRequestException(Canary);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => throw new HttpRequestException(Canary);
    }
    private sealed class StreamingHttp : HttpMessageHandler {
        internal ConcurrentQueue<EndlessStream> Streams = new();
        private HttpResponseMessage Response() {
            var stream = new EndlessStream(); Streams.Enqueue(stream);
            var content = new UnknownLengthContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
            return new(HttpStatusCode.OK) { Content = content };
        }
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) => Response();
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(Response());
    }
    private sealed class UnknownLengthContent(EndlessStream stream) : HttpContent {
        protected override bool TryComputeLength(out long length) { length = 0; return false; }
        protected override Task SerializeToStreamAsync(Stream target, TransportContext? context) => throw new InvalidOperationException("must stream, not buffer");
        protected override Task<Stream> CreateContentReadStreamAsync() => Task.FromResult<Stream>(stream);
        protected override Task<Stream> CreateContentReadStreamAsync(CancellationToken token) => Task.FromResult<Stream>(stream);
        protected override void Dispose(bool disposing) { if (disposing) stream.Dispose(); base.Dispose(disposing); }
    }
    private sealed class EndlessStream : Stream {
        internal int ReadBytes; internal bool Disposed;
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => ReadBytes; set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count) { Array.Fill(buffer, (byte)'x', offset, count); ReadBytes += count; return count; }
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default) {
            token.ThrowIfCancellationRequested(); buffer.Span.Fill((byte)'x'); ReadBytes += buffer.Length; return ValueTask.FromResult(buffer.Length);
        }
        protected override void Dispose(bool disposing) { Disposed = true; base.Dispose(disposing); }
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
