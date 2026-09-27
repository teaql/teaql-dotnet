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
        metadata = LogPrivacy.Project(metadata);
        var elapsed = metadata.EndedAt - metadata.StartedAt;
        var elapsedMicros = (long)(elapsed.TotalMilliseconds * 1_000);
        var summary = metadata.ResultCount is not null
            ? $"{metadata.ResultCount} rows returned"
            : metadata.AffectedRows is not null ? $"{metadata.AffectedRows} rows affected" : "";
        lock (_gate)
        {
            _writer.WriteLine($"[TeaQL SQL][{metadata.Operation.ToString().ToLowerInvariant()}][{elapsedMicros}us] {summary}");
            _writer.WriteLine($"comment={metadata.Comment} purpose={metadata.Purpose} auditReason={metadata.AuditReason} tracePath={string.Join(" -> ", metadata.TraceChain)}");
            _writer.WriteLine($"Parameterized SQL: {metadata.ParameterizedQuery} parameterCount={metadata.ParameterCount}");
        }
    }
}

public sealed class SensitiveDiagnosticSqlLogSink : ISensitiveDiagnosticSqlLogSink
{
    private readonly TextWriter _writer;
    private readonly object _gate = new();

    public SensitiveDiagnosticSqlLogSink(TextWriter? writer = null) => _writer = writer ?? Console.Error;

    public void Write(ExecutionMetadata metadata)
    {
        metadata = LogPrivacy.Project(metadata, LogPrivacy.PlaintextEnabled());
        var elapsedMicros = (long)((metadata.EndedAt - metadata.StartedAt).TotalMilliseconds * 1_000);
        lock (_gate)
        {
            _writer.WriteLine($"[TeaQL SENSITIVE SQL][{metadata.Operation.ToString().ToLowerInvariant()}][{elapsedMicros}us]");
            _writer.WriteLine($"Parameterized SQL: {metadata.ParameterizedQuery} params=[{string.Join(", ", metadata.Parameters)}]");
            _writer.WriteLine($"Debug SQL: {metadata.DebugQuery}");
        }
    }
}
