using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Sql;

internal static class SqlMutationReadback
{
    internal static async Task<(Record Row, ExecutionMetadata Read)> ExecuteAsync(SqlDialect dialect, ISqlTransport transport,
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
            var row = rows.SingleOrDefault()
                ?? throw new SqlExecutorException($"Authoritative persisted row not found for {refresh.Entity}");
            return (row, ReadMetadata("success"));
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
                    var outcome = count.HasValue ? "success" : error is OperationCanceledException ? "cancelled" : "failure";
                    Report(observer, ReadMetadata(outcome));
                }
            }
            throw;
        }

        ExecutionMetadata ReadMetadata(string outcome)
        {
            var query = QueryRequest.Readback(refresh, request);
            var read = SqlStatementDiagnostics.Metadata(dialect, query, compiled!, started, outcome, count);
            read.AuditReason = write.AuditReason;
            read.MutationLineage = write.MutationLineage;
            read.IntentSource = write;
            return read;
        }
    }

    private static void Report(Action<ExecutionMetadata> observer, ExecutionMetadata metadata)
    {
        try { observer(metadata); }
        catch { /* Preserve the in-flight readback error; attempt the other record as well. */ }
    }
}
