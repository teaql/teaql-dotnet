using System;
using System.IO;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public sealed class TextDiagnosticSqlLogSink : IDiagnosticSqlLogSink
{
    private readonly TextWriter _writer;
    private readonly object _gate = new();

    public TextDiagnosticSqlLogSink(TextWriter? writer = null)
    {
        _writer = writer ?? Console.Error;
    }

    public void Write(ExecutionMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(metadata.DebugQuery)) return;
        var elapsed = metadata.EndedAt - metadata.StartedAt;
        var elapsedMicros = (long)(elapsed.TotalMilliseconds * 1_000);
        var summary = metadata.ResultCount is not null
            ? $"{metadata.ResultCount} rows returned"
            : metadata.AffectedRows is not null ? $"{metadata.AffectedRows} rows affected" : "";
        lock (_gate)
        {
            _writer.WriteLine($"[TeaQL SQL][{metadata.Operation.ToString().ToLowerInvariant()}][{elapsedMicros}us] {summary}");
            _writer.WriteLine($"comment={metadata.Comment} purpose={metadata.Purpose} auditReason={metadata.AuditReason} tracePath={string.Join(" -> ", metadata.TraceChain)}");
            _writer.WriteLine($"Parameterized SQL: {metadata.ParameterizedQuery} params=[{string.Join(", ", metadata.Parameters)}]");
            _writer.WriteLine($"Debug SQL: {metadata.DebugQuery}");
        }
    }
}
