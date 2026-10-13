using System.Net;
using System.Diagnostics;

namespace TeaQL.Telemetry.OpenTelemetry;

/// <summary>
/// Per-exporter HTTP boundary. Official SDK serializers and processors remain
/// unchanged, but no remote response text or original transport exception is
/// returned to SDK EventSource logging. No customer-global logging is disabled.
/// </summary>
internal sealed class OwnedHttpTransport(HttpClient inner, int timeoutMilliseconds, string signal, ExportHealthState health) : HttpMessageHandler
{
    internal const int MaximumResponseBytes = 4 * 1024 * 1024;
    private readonly CancellationTokenSource _lifetime = new();
    private int _closed;
    private int _active;
    private long _deadline = long.MaxValue;
    internal string Signal => signal;
    internal int Active => Volatile.Read(ref _active);
    internal bool IsClosed => Volatile.Read(ref _closed) != 0;
    internal void CancelPending() { try { _lifetime.Cancel(); } catch (ObjectDisposedException) { } }
    internal void Constrain(TimeSpan remaining) => Interlocked.Exchange(ref _deadline,
        Stopwatch.GetTimestamp() + (long)(Math.Max(0, remaining.TotalSeconds) * Stopwatch.Frequency));
    internal static HttpClient PrivateClient() => new(new SocketsHttpHandler {
        AllowAutoRedirect = false, UseCookies = false, Credentials = null
    }) { Timeout = Timeout.InfiniteTimeSpan };

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Exchange(request, cancellationToken, asynchronous: false).GetAwaiter().GetResult();
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Exchange(request, cancellationToken, asynchronous: true);

    private async Task<HttpResponseMessage> Exchange(HttpRequestMessage request, CancellationToken cancellationToken, bool asynchronous)
    {
        if (Volatile.Read(ref _closed) != 0) return Failure();
        Interlocked.Increment(ref _active);
        try {
        using var budget = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        var remaining = (Interlocked.Read(ref _deadline) - Stopwatch.GetTimestamp()) * 1000.0 / Stopwatch.Frequency;
        budget.CancelAfter(TimeSpan.FromMilliseconds(Math.Min(Math.Min(3000, Math.Max(1, timeoutMilliseconds)), Math.Max(0, remaining))));
        try {
            var submitted = OtlpAcknowledgement.CountSubmitted(await request.Content!.ReadAsByteArrayAsync(budget.Token).ConfigureAwait(false), signal);
            if (submitted > health.Submitted(signal) || health.Submitted(signal) < 0) return Failure();
            // HttpClient marks a request as sent before invoking its handler.
            // Forward a fresh envelope, not the already-sent SDK envelope, and
            // leave request-content lifetime with the SDK that owns it.
            using var outbound = new HttpRequestMessage(request.Method, request.RequestUri) {
                Version = request.Version, VersionPolicy = request.VersionPolicy, Content = request.Content
            };
            foreach (var header in request.Headers) outbound.Headers.TryAddWithoutValidation(header.Key, header.Value);
            HttpResponseMessage received;
            try {
                received = asynchronous
                    ? await inner.SendAsync(outbound, HttpCompletionOption.ResponseHeadersRead, budget.Token).ConfigureAwait(false)
                    : inner.Send(outbound, HttpCompletionOption.ResponseHeadersRead, budget.Token);
            }
            finally { outbound.Content = null; }
            using var response = received;
            if (!response.IsSuccessStatusCode) return Failure();
            if (response.Content.Headers.ContentLength > MaximumResponseBytes) return InvalidResponse();
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType != null && !mediaType.Equals("application/x-protobuf", StringComparison.OrdinalIgnoreCase)) return InvalidResponse();
            using var body = await response.Content.ReadAsStreamAsync(budget.Token).ConfigureAwait(false);
            var buffer = new byte[16 * 1024];
            using var acknowledgement = new MemoryStream();
            var total = 0;
            while (true) {
                var size = Math.Min(buffer.Length, MaximumResponseBytes - total + 1);
                var count = await body.ReadAsync(buffer.AsMemory(0, size), budget.Token).ConfigureAwait(false);
                if (count == 0) break;
                total += count;
                if (total > MaximumResponseBytes) return InvalidResponse();
                acknowledgement.Write(buffer, 0, count);
            }
            if (mediaType == null && total != 0) return InvalidResponse();
            AckSummary summary;
            try { summary = OtlpAcknowledgement.Decode(acknowledgement.ToArray(), signal, submitted); }
            catch { return InvalidResponse(); }
            if (summary.Rejected > 0) health.Response(signal, "partial_success", summary.Rejected);
            else if (summary.Warning) health.Response(signal, "warning");
            // Return a normalized native success without collector text.
            return new HttpResponseMessage(HttpStatusCode.OK) {
                ReasonPhrase = "TeaQL telemetry HTTP accepted",
                Content = new ByteArrayContent([])
            };
        }
        catch { return Failure(); }
        }
        catch { return Failure(); }
        finally { Interlocked.Decrement(ref _active); }
    }

    private HttpResponseMessage InvalidResponse() { health.Response(signal, "invalid_response"); return Failure(); }

    private static HttpResponseMessage Failure() => new(HttpStatusCode.BadRequest) {
        ReasonPhrase = "TeaQL telemetry HTTP failure",
        Content = new ByteArrayContent([])
    };
    protected override void Dispose(bool disposing) {
        if (disposing && Interlocked.Exchange(ref _closed, 1) == 0) {
            CancelPending(); inner.Dispose(); _lifetime.Dispose();
        }
        base.Dispose(disposing);
    }
}
