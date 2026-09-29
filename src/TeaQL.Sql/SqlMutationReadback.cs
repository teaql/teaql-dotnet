using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Sql;

internal static class SqlMutationReadback
{
    internal static async Task<Record> ExecuteAsync(SqlDialect dialect, ISqlTransport transport,
        EntityDescriptor entity, SelectQuery refresh, MutationRequest request, ExecutionMetadata write)
    {
        CompiledQuery? compiled = null;
        DateTimeOffset started = default;
        int? count = null;
        try
        {
            compiled = dialect.CompileSelect(entity, refresh);
            started = DateTimeOffset.UtcNow;
            var rows = await transport.FetchAllSqlAsync(compiled);
            count = rows.Count;
            return rows.SingleOrDefault()
                ?? throw new SqlExecutorException($"Authoritative persisted row not found for {refresh.Entity}");
        }
        catch (Exception error)
        {
            if (request.DiagnosticObserver is { } observer)
            {
                Report(observer, write);
                // A compile failure did not execute a query. Zero/multiple rows
                // did: SQL succeeded even though the business snapshot is invalid.
                if (compiled != null)
                {
                    var query = new QueryRequest(refresh) {
                        Comment = request.Comment,
                        Purpose = "why: refresh authoritative persisted row",
                        TraceChain = request.TraceChain.ToList()
                    };
                    var outcome = count.HasValue ? "success" : error is OperationCanceledException ? "cancelled" : "failure";
                    var read = SqlStatementDiagnostics.Metadata(dialect, query, compiled, started, outcome, count);
                    read.AuditReason = write.AuditReason;
                    read.IntentSource = write;
                    Report(observer, read);
                }
            }
            throw;
        }
    }

    private static void Report(Action<ExecutionMetadata> observer, ExecutionMetadata metadata)
    {
        try { observer(metadata); }
        catch { /* Preserve the in-flight readback error; attempt the other record as well. */ }
    }
}
