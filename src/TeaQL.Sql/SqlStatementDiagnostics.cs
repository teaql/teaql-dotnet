using System.Runtime.CompilerServices;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Sql;

internal static class SqlStatementDiagnostics
{
    // Parent SQL can finish before a descendant does. Capture declared child
    // bindings/policies before emitting parent prose, without executing a query,
    // changing caller builders, or sharing provenance through Context.
    internal static ExecutionMetadata CaptureQueryIntent(SqlDialect dialect, ISchemaProvider schema,
        QueryRequest request, CompiledQuery compiled)
    {
        var current = InheritedIntent(request, compiled);
        var values = current.Parameters.ToList();
        var policies = current.ParameterLogPolicies.ToList();
        var pending = new Stack<(SelectQuery Query, string Entity)>();
        var seen = new Dictionary<SelectQuery, HashSet<string>>(ReferenceEqualityComparer.Instance);
        void Children(SelectQuery query, EntityDescriptor entity)
        {
            foreach (var load in query.RelationLoads)
                if (load.Query != null && entity.RelationByName(load.Name) is { } relation)
                    pending.Push((load.Query, relation.TargetEntity));
            foreach (var aggregate in query.RelationAggregates)
                if (entity.RelationByName(aggregate.RelationName) is { } relation)
                    pending.Push((aggregate.Query, relation.TargetEntity));
            foreach (var facet in query.Facets)
                pending.Push((facet.Query, entity.RelationByName(facet.RelationName)?.TargetEntity ?? facet.Query.Entity));
            foreach (var child in query.ChildEnhancements) pending.Push((child, child.Entity));
            foreach (var group in query.ObjectGroupBys) pending.Push((group.Query, group.Query.Entity));
            if (query.DiagnosticOrigin is { } origin) pending.Push((origin, origin.Entity));
        }
        if (schema.GetEntity(request.Query.Entity) is { } root) Children(request.Query, root);
        while (pending.TryPop(out var item))
        {
            if (!seen.TryGetValue(item.Query, out var entities)) seen[item.Query] = entities = new();
            if (!entities.Add(item.Entity)) continue;
            var entity = schema.GetEntity(item.Entity) ?? throw new SqlExecutorException("Unknown relation entity");
            var query = item.Query.CloneForExecution(); query.Entity = item.Entity;
            // Collect binding policies only; never execute or add physical trace edges.
            query.NormalizeGeneratedFilters();
            var child = dialect.CompileSelect(entity, query);
            var intent = InheritedIntent(new QueryRequest(query, request.Intent), child);
            values.AddRange(intent.Parameters); policies.AddRange(intent.ParameterLogPolicies);
            Children(item.Query, entity);
        }
        return new ExecutionMetadata { Parameters = values.ToArray(), ParameterLogPolicies = policies.ToArray(),
            IntentValues = request.IntentSource?.IntentValues ?? Array.Empty<Value>(), GeneratedSql = true };
    }

    // Flatten only binding provenance, not SQL or free-form intent. A grandchild
    // needs both its parent's and earlier ancestors' policies. No global cache.
    internal static ExecutionMetadata InheritedIntent(QueryRequest request, CompiledQuery compiled)
    {
        var values = new List<Value>();
        var policies = new List<SqlParameterLogPolicy>();
        void Append(IReadOnlyList<Value> parameters, IReadOnlyList<SqlParameterLogPolicy> sourcePolicies,
            bool generated, string? sql)
        {
            bool credential = (!generated || sourcePolicies.Count == 0) && SensitiveLogNames.IsCredential(sql ?? "");
            for (int i = 0; i < parameters.Count; i++)
            {
                values.Add(parameters[i]);
                policies.Add(credential ? SqlParameterLogPolicy.Credential
                    : sourcePolicies.Count == parameters.Count ? sourcePolicies[i] : SqlParameterLogPolicy.Unknown);
            }
        }
        if (request.IntentSource is { } inherited)
            Append(inherited.Parameters, inherited.ParameterLogPolicies, inherited.GeneratedSql, inherited.ParameterizedQuery);
        Append(compiled.Params, compiled.ParameterLogPolicies, compiled.GeneratedSql, compiled.Sql);
        return new ExecutionMetadata { Parameters = values, ParameterLogPolicies = policies, GeneratedSql = true };
    }

    internal static ExecutionMetadata Metadata(SqlDialect dialect, object request, CompiledQuery compiled,
        DateTimeOffset start, string outcome, int? count = null, EntityDescriptor? descriptor = null)
    {
        var query = request as QueryRequest;
        var mutation = request as MutationRequest;
        var operation = request switch {
            QueryRequest => DataServiceOperation.Query,
            InsertMutationRequest => DataServiceOperation.Insert,
            UpdateMutationRequest => DataServiceOperation.Update,
            DeleteMutationRequest => DataServiceOperation.Delete,
            RecoverMutationRequest => DataServiceOperation.Recover,
            _ => throw new InvalidOperationException("Unknown SQL request")
        };
        var entity = request switch {
            QueryRequest q => q.Query.Entity,
            InsertMutationRequest i => i.Command.Entity,
            UpdateMutationRequest u => u.Command.Entity,
            DeleteMutationRequest d => d.Command.Entity,
            RecoverMutationRequest r => r.Command.Entity,
            _ => "unknown"
        };
        return new ExecutionMetadata {
            Backend = dialect.Kind.ToString().ToLowerInvariant(), Operation = operation,
            ExecutionOutcome = outcome, StartedAt = start, EndedAt = DateTimeOffset.UtcNow,
            ResultCount = count, ParameterizedQuery = compiled.Sql, Parameters = compiled.Params.ToList(),
            ParameterLogPolicies = compiled.ParameterLogPolicies, GeneratedSql = compiled.GeneratedSql,
            IntentSource = query?.IntentSource,
            IntentValues = mutation == null ? Array.Empty<Value>() : MutationTargetIds(mutation, descriptor),
            Comment = query?.Comment ?? mutation?.Comment, Purpose = query?.Purpose, AuditReason = mutation?.Comment,
            MutationLineage = mutation?.AuditLineage(entity) ?? Array.Empty<TraceNode>(),
            TraceChain = query != null ? SqlDataServiceTransaction.QueryTracePath(query, dialect.Kind.ToString())
                : SqlDataServiceTransaction.MutationTracePath(mutation!, entity, operation, dialect.Kind.ToString())
        };
    }

    internal static IReadOnlyList<Value> MutationTargetIds(MutationRequest request, EntityDescriptor? descriptor)
    {
        Value? target = request switch {
            InsertMutationRequest insert when descriptor?.IdProperty() is { } id
                && insert.Command.Values.TryGetValue(id.Name, out var value) => value,
            UpdateMutationRequest update => update.Command.Id,
            DeleteMutationRequest delete => delete.Command.Id,
            RecoverMutationRequest recover => recover.Command.Id,
            _ => null
        };
        return target == null || target is Value.NullValue or Value.TypedNullValue
            ? request.InheritedIntentValues
            : request.InheritedIntentValues.Concat(new[] { target }).ToArray();
    }

    private static void Record(object request, ExecutionMetadata metadata)
    {
        var observer = request is QueryRequest q ? q.DiagnosticObserver : ((MutationRequest)request).DiagnosticObserver;
        if (observer == null) return;
        metadata.DiagnosticReported = true;
        try { observer?.Invoke(metadata); }
        catch when (metadata.ExecutionOutcome != "success") {
            // A failing sink must not replace an in-flight provider failure/cancellation.
        }
    }

    internal static void QuerySucceeded(QueryRequest request, ExecutionMetadata metadata)
    {
        try { Record(request, metadata); }
        catch (Exception error) when (error is not OutOfMemoryException) { }
    }

    internal static void Failure(SqlDialect dialect, object request, CompiledQuery compiled,
        DateTimeOffset start, Exception error, EntityDescriptor? descriptor = null) => Record(request, Metadata(dialect, request, compiled, start,
            error is OperationCanceledException ? "cancelled" : "failure", descriptor: descriptor));

    internal static async IAsyncEnumerable<StreamChunk> Stream(SqlDialect dialect, ISqlTransport transport,
        ISchemaProvider schema, QueryRequest request, int chunkSize,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
        if (request.Query.RelationLoads.Count != 0 || request.Query.ChildEnhancements.Count != 0 || request.Query.ObjectGroupBys.Count != 0)
            throw new NotSupportedException("streaming relation or aggregate enhancement is not supported; stream a root query or use ExecuteForListAsync");
        if (transport is not IStreamingSqlTransport streaming) throw new NotSupportedException("streaming query is not supported by this transport");
        var entity = schema.GetEntity(request.Query.Entity) ?? throw new SqlExecutorException($"unknown entity {request.Query.Entity}");
        var compiled = dialect.CompileSelect(entity, request.Query);
        var start = DateTimeOffset.UtcNow;
        var outcome = "cancelled";
        int delivered = 0, index = 0;
        IAsyncEnumerator<Record>? cursor = null;
        var current = new List<Record>();
        List<Record>? pending = null;
        async Task<bool> MoveNext()
        {
            try {
                cancellationToken.ThrowIfCancellationRequested();
                return await cursor!.MoveNextAsync().ConfigureAwait(false);
            }
            catch (Exception ex) { outcome = ex is OperationCanceledException ? "cancelled" : "failure"; throw; }
        }
        try
        {
            try { cursor = streaming.StreamSqlAsync(compiled, cancellationToken).GetAsyncEnumerator(cancellationToken); }
            catch (Exception ex) { outcome = ex is OperationCanceledException ? "cancelled" : "failure"; throw; }
            while (await MoveNext().ConfigureAwait(false))
            {
                current.Add(cursor.Current);
                if (current.Count >= chunkSize)
                {
                    if (pending != null) {
                        delivered += pending.Count;
                        yield return new StreamChunk { Rows = pending, ChunkIndex = index++, IsLast = false };
                    }
                    pending = current;
                    current = new List<Record>();
                }
            }
            if (current.Count > 0)
            {
                if (pending != null) {
                    delivered += pending.Count;
                    yield return new StreamChunk { Rows = pending, ChunkIndex = index++, IsLast = false };
                }
                delivered += current.Count;
                yield return new StreamChunk { Rows = current, ChunkIndex = index, IsLast = true };
            }
            else if (pending != null) {
                delivered += pending.Count;
                yield return new StreamChunk { Rows = pending, ChunkIndex = index, IsLast = true };
            }
            outcome = "success";
        }
        finally
        {
            try { if (cursor != null) await cursor.DisposeAsync().ConfigureAwait(false); }
            catch (Exception ex) {
                if (outcome == "success") {
                    outcome = ex is OperationCanceledException ? "cancelled" : "failure";
                    throw;
                }
            }
            finally { Record(request, Metadata(dialect, request, compiled, start, outcome, delivered)); }
        }
    }
}
