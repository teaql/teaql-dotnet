using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using TeaQL.Runtime;
using System.Diagnostics.Metrics;

namespace TeaQL.Telemetry.OpenTelemetry;

internal sealed class ExportHealthState
{
    private long _traces, _logs, _metrics, _traceDrops, _logDrops;
    private readonly object _gate = new();
    private readonly Dictionary<string, long> _rejected = new(), _responses = new(), _queueLosses = new();
    private readonly AsyncLocal<Submission?> _submission = new();
    private sealed record Submission(string Signal, long Count);
    internal IReadOnlyDictionary<string, long> Rejected => Copy(_rejected);
    internal IReadOnlyDictionary<string, long> Responses => Copy(_responses);
    internal IReadOnlyDictionary<string, long> QueueLosses => Copy(_queueLosses);
    private Dictionary<string, long> Copy(Dictionary<string, long> values) { lock (_gate) return new(values); }
    internal long Submitted(string signal) => _submission.Value is { } current && current.Signal == signal ? current.Count : -1;
    internal IDisposable Enter(string signal, long count) {
        var previous = _submission.Value; _submission.Value = new(signal, count);
        return new RestoreSubmission(this, previous);
    }
    private sealed class RestoreSubmission(ExportHealthState owner, Submission? previous) : IDisposable {
        private int _disposed;
        public void Dispose() { if (Interlocked.Exchange(ref _disposed, 1) == 0) owner._submission.Value = previous; }
    }
    internal void Response(string signal, string reason, long rejected = 0) {
        bool first;
        lock (_gate) {
            var key = signal + ":" + reason; first = !_responses.ContainsKey(key);
            _responses[key] = _responses.GetValueOrDefault(key) + 1;
            if (rejected > 0) _rejected[signal] = _rejected.GetValueOrDefault(signal) + rejected;
        }
        if (first) TelemetryStartup.Notice($"TeaQL telemetry: {signal} response={reason}; business outcome unchanged.");
    }
    private static Measurement<long> Measure(long value, string signal, string? reason = null) => reason == null
        ? new(value, new KeyValuePair<string, object?>("teaql.telemetry.signal", signal))
        : new(value, new KeyValuePair<string, object?>("teaql.telemetry.signal", signal), new("teaql.telemetry.reason", reason));
    internal IEnumerable<Measurement<long>> LossMeasurements() {
        var values = QueueLosses;
        return new[] { Measure(Traces, "traces", "export_failure"), Measure(Logs, "logs", "export_failure"), Measure(Metrics, "metrics", "export_failure") }
            .Concat(new[] { "traces", "logs" }.SelectMany(signal => new[] { "queue_overflow", "shutdown", "handoff_failure" }
                .Select(reason => Measure(values.GetValueOrDefault(signal + ":" + reason), signal, reason)))).ToArray();
    }
    internal IEnumerable<Measurement<long>> RejectionMeasurements() {
        var values = Rejected;
        return new[] { "traces", "logs", "metrics" }.Select(signal => Measure(values.GetValueOrDefault(signal), signal)).ToArray();
    }
    internal IEnumerable<Measurement<long>> ResponseMeasurements() {
        var values = Responses;
        return new[] { "traces", "logs", "metrics" }.SelectMany(signal => new[] { "partial_success", "warning", "invalid_response" }
            .Select(reason => Measure(values.GetValueOrDefault(signal + ":" + reason), signal, reason))).ToArray();
    }
    public long Traces => Interlocked.Read(ref _traces);
    public long Logs => Interlocked.Read(ref _logs);
    public long Metrics => Interlocked.Read(ref _metrics);
    public long TraceDrops => Interlocked.Read(ref _traceDrops);
    public long LogDrops => Interlocked.Read(ref _logDrops);
    internal void QueueLoss(string signal, string reason, long count) {
        if (count <= 0) return;
        bool first;
        lock (_gate) { var key = signal + ":" + reason; first = !_queueLosses.ContainsKey(key); _queueLosses[key] = _queueLosses.GetValueOrDefault(key) + count; }
        if (reason == "queue_overflow") { if (signal == "traces") Interlocked.Add(ref _traceDrops, count); else Interlocked.Add(ref _logDrops, count); }
        if (first) TelemetryStartup.Notice($"TeaQL telemetry: {signal} loss={reason}; business outcome unchanged.");
    }
    internal void Failed(string signal, long count) {
        var total = signal switch { "traces" => Interlocked.Add(ref _traces, count), "logs" => Interlocked.Add(ref _logs, count), _ => Interlocked.Add(ref _metrics, count) };
        if (total == count) TelemetryStartup.Notice($"TeaQL telemetry: {signal} export failed; delivery loss counted; business outcome unchanged.");
    }
}
// Register resource ownership through the public SDK extension point, without
// exporting twice or reflecting into SDK-internal ParentProvider setters.
internal sealed class ExporterRegistration<T>(BaseExporter<T> exporter) : BaseExportProcessor<T>(exporter) where T : class {
    protected override void OnExport(T data) { }
    protected override bool OnForceFlush(int timeoutMilliseconds) => true;
    protected override bool OnShutdown(int timeoutMilliseconds) => true;
    protected override void Dispose(bool disposing) { }
}
internal sealed class HealthAwareOtlpMetricExporter(OtlpExporterOptions options, ExportHealthState health) : OtlpMetricExporter(options) {
    public override ExportResult Export(in Batch<Metric> batch) {
        long points = 0;
        foreach (var metric in batch) foreach (ref readonly var point in metric.GetMetricPoints()) points++;
        using var submission = health.Enter("metrics", points);
        ExportResult result; try { result = base.Export(in batch); } catch { result = ExportResult.Failure; }
        if (result == ExportResult.Failure) health.Failed("metrics", batch.Count);
        return result;
    }
}
internal sealed class SafeExporter<T>(BaseExporter<T> exporter, ExportHealthState health, string signal, Admission? admission = null) : BaseExporter<T> where T : class
{
    public override ExportResult Export(in Batch<T> batch) {
        if (admission == null) return ExportSelected(in batch);
        var selected = new List<T>(); var identities = new List<string>();
        // Native Batch is single-use and dequeues while iterating. Materialize
        // it once, never enumerate it for counting and then enumerate it again.
        // Our wrapped log processor makes the SDK take its thread-static Copy
        // path, so these log snapshots are not returned to its shared pool here.
        foreach (var record in batch) {
            var identity = RecordIdentity(record);
            if (identity == null || !admission.BeginExport(identity)) continue;
            if (record is LogRecord log)
                log.Attributes = log.Attributes?.Where(pair => pair.Key != LogTicketKey).ToArray();
            selected.Add(record); identities.Add(identity);
        }
        if (selected.Count == 0) return ExportResult.Success;
        try { using var outbound = new Batch<T>(selected.ToArray(), selected.Count); return ExportSelected(in outbound); }
        finally { admission.Complete(identities); }
    }
    internal const string LogTicketKey = "teaql.internal.telemetry.ticket";
    internal static string? RecordIdentity(T record) => record switch {
        System.Diagnostics.Activity span => span.TraceId.ToHexString() + span.SpanId.ToHexString(),
        LogRecord log => log.Attributes?.FirstOrDefault(pair => pair.Key == LogTicketKey).Value as string,
        _ => null
    };
    private ExportResult ExportSelected(in Batch<T> batch) {
        using var submission = health.Enter(signal, batch.Count);
        ExportResult result;
        try { result = exporter.Export(in batch); } catch { result = ExportResult.Failure; }
        if (result == ExportResult.Failure) health.Failed(signal, batch.Count);
        return result;
    }
    protected override bool OnForceFlush(int timeoutMilliseconds) { try { return exporter.ForceFlush(timeoutMilliseconds); } catch { return false; } }
    protected override bool OnShutdown(int timeoutMilliseconds) { try { return exporter.Shutdown(timeoutMilliseconds); } catch { return false; } }
    protected override void Dispose(bool disposing) { if (disposing) exporter.Dispose(); base.Dispose(disposing); }
}
