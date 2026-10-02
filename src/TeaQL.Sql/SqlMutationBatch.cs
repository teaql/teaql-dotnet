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
        // Root prose may mention values belonging to a different branch, even
        // one that has not executed. Keep redaction local to this invocation.
        var intentValues = batch.InheritedIntentValues.Concat(Values(batch)).ToArray();
        var start = DateTimeOffset.UtcNow;
        try
        {
            foreach (var child in batch.Requests)
            {
                var request = child.WithRootIntent(batch.Intent);
                request.InheritedIntentValues = child.InheritedIntentValues.Concat(intentValues).ToArray();
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

    private static IEnumerable<Value> Values(MutationRequest request)
    {
        switch (request)
        {
            case BatchMutationRequest batch:
                foreach (var child in batch.Requests)
                    foreach (var value in Values(child)) yield return value;
                break;
            case InsertMutationRequest insert:
                foreach (var value in insert.Command.Values.Values) yield return value;
                break;
            case UpdateMutationRequest update:
                yield return update.Command.Id;
                foreach (var value in update.Command.Values.Values) yield return value;
                foreach (var value in update.Command.Guards.Values) yield return value;
                if (update.Command.OldValues is { } old)
                    foreach (var value in old.Values) yield return value;
                break;
            case DeleteMutationRequest delete:
                yield return delete.Command.Id;
                foreach (var value in delete.Command.Guards.Values) yield return value;
                break;
            case RecoverMutationRequest recover:
                yield return recover.Command.Id;
                foreach (var value in recover.Command.Guards.Values) yield return value;
                break;
        }
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
