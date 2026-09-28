using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Sql;

// Invocation-local diagnostic history, not an entity mutation ledger. Success
// remains on the returned metadata path; failure reports only executed SQL.
internal static class SqlMutationBatch
{
    internal static async Task<MutationResult> ExecuteAsync(SqlDialect dialect,
        BatchMutationRequest batch, Func<MutationRequest, Task<MutationResult>> execute)
    {
        ulong totalAffected = 0;
        var parameterizedQueries = new List<string>();
        var parameters = new List<Value>();
        var statements = new List<ExecutionMetadata>();
        var observer = batch.DiagnosticObserver;
        var start = DateTimeOffset.UtcNow;
        try
        {
            foreach (var request in batch.Requests)
            {
                var previous = request.DiagnosticObserver;
                request.DiagnosticObserver = observer == null ? null : statements.Add;
                MutationResult result;
                try { result = await execute(request); }
                finally { request.DiagnosticObserver = previous; }
                totalAffected += result.AffectedRows;
                if (!string.IsNullOrWhiteSpace(result.Metadata.ParameterizedQuery))
                    parameterizedQueries.Add(result.Metadata.ParameterizedQuery);
                parameters.AddRange(result.Metadata.Parameters);
                statements.Add(result.Metadata);
            }
        }
        catch
        {
            if (observer != null)
                foreach (var statement in statements) ReportFailureHistory(observer, statement);
            throw;
        }

        return new MutationResult {
            AffectedRows = totalAffected,
            GeneratedValues = new Record(),
            Metadata = new ExecutionMetadata {
                Backend = dialect.Kind.ToString().ToLowerInvariant(),
                Operation = DataServiceOperation.Batch,
                StartedAt = start, EndedAt = DateTimeOffset.UtcNow,
                AffectedRows = totalAffected,
                ParameterizedQuery = parameterizedQueries.Count == 0 ? null : string.Join("; ", parameterizedQueries),
                Parameters = parameters, Statements = statements
            }
        };
    }

    private static void ReportFailureHistory(Action<ExecutionMetadata> observer, ExecutionMetadata statement)
    {
        if (statement.Statements.Count > 0)
        {
            foreach (var child in statement.Statements) ReportFailureHistory(observer, child);
            return;
        }
        try { observer(statement); }
        catch
        {
            // A broken sink cannot replace the in-flight error or prevent
            // attempts to report later statements, even after successful SQL.
        }
    }
}
