using Google.Protobuf;
using OpenTelemetry.Proto.Collector.Logs.V1;
using OpenTelemetry.Proto.Collector.Metrics.V1;
using OpenTelemetry.Proto.Collector.Trace.V1;
using System.Text;
using ProtoMetric = global::OpenTelemetry.Proto.Metrics.V1.Metric;

namespace TeaQL.Telemetry.OpenTelemetry;

internal readonly record struct AckSummary(long Rejected, bool Warning);

internal static class OtlpAcknowledgement
{
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);
    internal static long CountSubmitted(byte[] request, string signal) {
        using var stream = new MemoryStream(request, writable: false);
        using var input = CodedInputStream.CreateWithLimits(stream, 16 * 1024 * 1024, 16);
        return signal switch {
            "traces" => ExportTraceServiceRequest.Parser.ParseFrom(input).ResourceSpans.SelectMany(row => row.ScopeSpans).Sum(row => (long)row.Spans.Count),
            "logs" => ExportLogsServiceRequest.Parser.ParseFrom(input).ResourceLogs.SelectMany(row => row.ScopeLogs).Sum(row => (long)row.LogRecords.Count),
            "metrics" => ExportMetricsServiceRequest.Parser.ParseFrom(input).ResourceMetrics.SelectMany(row => row.ScopeMetrics).SelectMany(row => row.Metrics)
                .Sum(row => row.DataCase switch {
                    ProtoMetric.DataOneofCase.Gauge => (long)row.Gauge.DataPoints.Count,
                    ProtoMetric.DataOneofCase.Sum => row.Sum.DataPoints.Count,
                    ProtoMetric.DataOneofCase.Histogram => row.Histogram.DataPoints.Count,
                    ProtoMetric.DataOneofCase.ExponentialHistogram => row.ExponentialHistogram.DataPoints.Count,
                    ProtoMetric.DataOneofCase.Summary => row.Summary.DataPoints.Count,
                    _ => 0L
                }),
            _ => throw new FormatException("invalid-signal")
        };
    }
    internal static AckSummary Decode(byte[] body, string signal, long submitted)
    {
        if (body.Length > OwnedHttpTransport.MaximumResponseBytes || submitted < 0) throw new FormatException("invalid-ack");
        ValidateKnownWires(body, partial: false);
        using var stream = new MemoryStream(body, writable: false);
        using var input = CodedInputStream.CreateWithLimits(stream, OwnedHttpTransport.MaximumResponseBytes, 16);
        AckSummary result = signal switch {
            "traces" => Trace(ExportTraceServiceResponse.Parser.ParseFrom(input)),
            "logs" => Log(ExportLogsServiceResponse.Parser.ParseFrom(input)),
            "metrics" => Metric(ExportMetricsServiceResponse.Parser.ParseFrom(input)),
            _ => throw new FormatException("invalid-signal")
        };
        if (result.Rejected < 0 || result.Rejected > submitted) throw new FormatException("invalid-rejection-count");
        return result;
    }
    private static AckSummary Trace(ExportTraceServiceResponse value) => new(value.PartialSuccess?.RejectedSpans ?? 0, value.PartialSuccess?.ErrorMessage.Length > 0);
    private static AckSummary Log(ExportLogsServiceResponse value) => new(value.PartialSuccess?.RejectedLogRecords ?? 0, value.PartialSuccess?.ErrorMessage.Length > 0);
    private static AckSummary Metric(ExportMetricsServiceResponse value) => new(value.PartialSuccess?.RejectedDataPoints ?? 0, value.PartialSuccess?.ErrorMessage.Length > 0);

    // Protobuf normally treats a known field with another wire type as an
    // unknown field. ACKs must not silently turn this into full acceptance.
    // Use the official wire reader, not an alternative protobuf decoder.
    private static void ValidateKnownWires(byte[] body, bool partial)
    {
        using var stream = new MemoryStream(body, writable: false);
        using var input = CodedInputStream.CreateWithLimits(stream, OwnedHttpTransport.MaximumResponseBytes, 16);
        uint tag;
        while ((tag = input.ReadTag()) != 0) {
            var field = tag >> 3; var wire = tag & 7;
            if (wire is 3 or 4 or 6 or 7) throw new FormatException("invalid-ack-wire");
            if (field == 1) {
                if (wire != (partial ? 0u : 2u)) throw new FormatException("invalid-ack-wire");
                if (partial) _ = input.ReadInt64();
                else ValidateKnownWires(input.ReadBytes().ToByteArray(), partial: true);
            }
            else if (partial && field == 2) {
                if (wire != 2) throw new FormatException("invalid-ack-wire");
                _ = StrictUtf8.GetCharCount(input.ReadBytes().Span);
            }
            else input.SkipLastField();
        }
    }
}
