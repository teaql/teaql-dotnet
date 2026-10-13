using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public sealed record RuntimeDiagnosticEvent(SqlExecutionCorrelation Correlation, IReadOnlyDictionary<string, object> Attributes);
public interface IRuntimeSqlTelemetry
{
    ISqlExecutionObservation StartStatement(string backend, DataServiceOperation operation);
    void RecordDiagnostic(RuntimeDiagnosticEvent diagnostic);
}
internal static class RuntimeSqlTelemetry
{
    internal static RuntimeDiagnosticEvent Project(ExecutionMetadata safe, SqlExecutionCorrelation correlation)
    {
        string Nodes(IEnumerable<TraceNode> nodes) => JsonSerializer.Serialize(nodes.Select(node => new {
            kind = node.Kind, name = node.Name, detail = node.Detail.Length > 0 ? node.Detail : node.Comment
        }));
        var attributes = new Dictionary<string, object> {
            ["teaql.sql.debug"] = safe.DebugQuery ?? "", ["teaql.sql.log_mode"] = safe.SqlOmissionReason == null ? "masked" : "omitted",
            ["teaql.sql.operation"] = safe.Operation == DataServiceOperation.Query ? "select" : safe.Operation.ToString().ToLowerInvariant(),
            ["teaql.sql.outcome"] = safe.ExecutionOutcome ?? "unknown", ["teaql.sql.elapsed_us"] = (long)((safe.EndedAt - safe.StartedAt).TotalMicroseconds),
            ["teaql.intent.comment"] = safe.Comment ?? safe.AuditReason ?? "", ["teaql.intent.purpose"] = safe.Purpose ?? "",
            ["teaql.trace_chain"] = Nodes(safe.RequestTraceChain), ["teaql.trace_path"] = Nodes(safe.TraceChain), ["teaql.mutation_lineage"] = Nodes(safe.MutationLineage)
        };
        if (safe.ResultCount is { } count) attributes["teaql.sql.result_count"] = count;
        if (safe.AffectedRows is { } affected) attributes["teaql.sql.affected_rows"] = affected;
        return new(correlation, attributes);
    }
}
