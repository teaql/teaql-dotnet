using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Sql;

internal static class QueryClone
{
    public static SelectQuery Clone(SelectQuery query) => query with
    {
        Projection = new List<string>(query.Projection),
        ExprProjection = new List<NamedExpr>(query.ExprProjection),
        OrderByItems = new List<OrderBy>(query.OrderByItems),
        AggregateItems = new List<Aggregate>(query.AggregateItems),
        GroupByItems = new List<string>(query.GroupByItems),
        RelationLoads = new List<RelationLoad>(query.RelationLoads),
        TraceChain = new List<TraceNode>(query.TraceChain),
        RawSqlSearchCriteriaItems = new List<string>(query.RawSqlSearchCriteriaItems),
        DynamicProperties = new List<RawSqlProjection>(query.DynamicProperties),
        RawProjections = new List<RawSqlProjection>(query.RawProjections),
        ObjectGroupBys = new List<ObjectGroupBy>(query.ObjectGroupBys),
        ChildEnhancements = new List<SelectQuery>(query.ChildEnhancements)
    };
}

internal sealed record ContinuousPageExecution(
    string QueryKey, string Entity, SortDirection Direction, ulong PageSize,
    ulong OriginalOffset, int TtlSeconds, bool Optimized, string? SeekCursorId);

internal static class ContinuousPagePlanner
{
    public static async Task<ContinuousPageExecution?> PrepareAsync(ContinuousPageRuntimeContext? runtime, SelectQuery query)
    {
        var options = query.ContinuousPageFetch;
        if (runtime == null || options == null) { runtime?.Observe("DISABLED"); return null; }
        if (query.Slice?.Limit is not ulong pageSize || pageSize == 0)
        { runtime.Observe("OFFSET_FALLBACK:INVALID_SLICE"); return null; }
        if (query.PartitionBy != null || query.AggregateItems.Count != 0 || query.GroupByItems.Count != 0 ||
            query.OrderByItems.Count != 1 || query.OrderByItems[0].Field != "id" || query.OrderByItems[0].ExprValue != null)
        { runtime.Observe("OFFSET_FALLBACK:UNSUPPORTED_QUERY_SHAPE"); return null; }

        var order = query.OrderByItems[0];
        var normalized = QueryClone.Clone(query);
        normalized.Slice = normalized.Slice! with { Offset = 0 };
        normalized.CommentText = null;
        normalized.TraceChain.Clear();
        normalized.ContinuousPageFetch = null;
        var payload = JsonSerializer.Serialize(normalized);
        var digest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{options.Namespace}|{runtime.Owner}|{payload}"))).ToLowerInvariant();
        var queryKey = $"teaql:continuous-page:v1:{digest}";
        var offset = query.Slice!.Offset;
        var execution = new ContinuousPageExecution(queryKey, query.Entity, order.Direction, pageSize, offset, options.TtlSeconds, false, null);
        if (offset == 0) { runtime.Observe("OFFSET_FALLBACK:FIRST_PAGE"); return execution; }

        ContinuousPageCursor? cursor;
        try { cursor = await runtime.Store.GetAsync(queryKey, offset); }
        catch { runtime.Observe("OFFSET_FALLBACK:STORE_UNAVAILABLE"); return execution; }
        if (cursor == null) { runtime.Observe("OFFSET_FALLBACK:CACHE_MISS"); return execution; }
        if (cursor.Entity != query.Entity || cursor.Direction != order.Direction || cursor.PageSize != pageSize ||
            cursor.NextOffset != offset || cursor.ExpiresAt <= DateTimeOffset.UtcNow)
        { runtime.Observe("OFFSET_FALLBACK:CURSOR_INVALID"); return execution; }

        query.Slice = query.Slice with { Offset = 0 };
        query.AndFilter(order.Direction == SortDirection.Desc ? Expr.Lt("id", cursor.Boundary) : Expr.Gt("id", cursor.Boundary));
        runtime.Observe("CURSOR_SEEK", cursor.CursorId);
        return execution with { Optimized = true, SeekCursorId = cursor.CursorId };
    }

    public static async Task RegisterAsync(ContinuousPageRuntimeContext? runtime, ContinuousPageExecution? execution, List<Record> rows)
    {
        if (runtime == null || execution == null || (ulong)rows.Count != execution.PageSize || rows.Count == 0 ||
            !rows[^1].TryGetValue("id", out var boundary)) return;
        var cursor = new ContinuousPageCursor(
            $"cpg_{Guid.NewGuid():N}", execution.QueryKey, execution.Entity, execution.Direction, boundary,
            execution.PageSize, execution.OriginalOffset + (ulong)rows.Count,
            DateTimeOffset.UtcNow.AddSeconds(execution.TtlSeconds));
        try { await runtime.Store.PutAsync(cursor); }
        catch { runtime.Observe("OFFSET_FALLBACK:STORE_UNAVAILABLE"); return; }
        if (execution.Optimized) runtime.Observe("CURSOR_SEEK", execution.SeekCursorId);
    }
}

public class SqlExecutorException : Exception
{
    public SqlExecutorException(string message) : base(message) { }
    public SqlExecutorException(string message, Exception innerException) : base(message, innerException) { }
}

public class SqlDataServiceExecutor : IDataService, ITransactionExecutor, IStreamQueryExecutor
{
    public SqlDialect Dialect { get; }
    public ISqlTransport Transport { get; }
    public ISchemaProvider SchemaProvider { get; }

    public SqlDataServiceExecutor(SqlDialect dialect, ISqlTransport transport, ISchemaProvider schemaProvider)
    {
        Dialect = dialect;
        Transport = transport;
        SchemaProvider = schemaProvider;
    }

    public DataServiceCapabilities Capabilities => new DataServiceCapabilities
    {
        Query = true,
        Mutation = true,
        Transaction = Transport is ISqlTransactionTransport,
        Schema = false,
        IdGeneration = false,
        BatchMutation = true,
        Returning = false
    };

    public async Task<QueryResult> QueryAsync(QueryRequest request)
    {
        var query = (request.Query.ContinuousPageFetch == null ? request.Query : QueryClone.Clone(request.Query)).PrepareForList();
        var continuous = await ContinuousPagePlanner.PrepareAsync(request.ContinuousPageRuntime, query);
        var entityDesc = SchemaProvider.GetEntity(query.Entity)
            ?? throw new SqlExecutorException($"SQL compile error: unknown entity {query.Entity}");

        CompiledQuery compiled;
        try
        {
            compiled = Dialect.CompileSelect(entityDesc, query);
        }
        catch (SqlCompileException ex)
        {
            throw new SqlExecutorException($"SQL compile error: {ex.Message}", ex);
        }

        var start = DateTimeOffset.UtcNow;
        List<Record> rows;
        try
        {
            rows = await Transport.FetchAllSqlAsync(compiled);
        }
        catch (Exception ex)
        {
            throw new SqlExecutorException($"Transport error: {ex.Message}", ex);
        }
        var effectiveRequest = new QueryRequest { Query = query, TraceChain = request.TraceChain, Comment = request.Comment, ContinuousPageRuntime = request.ContinuousPageRuntime };
        await RelationQueryLoader.EnhanceAsync(SchemaProvider, QueryAsync, rows, effectiveRequest);
        var end = DateTimeOffset.UtcNow;

        var metadata = new ExecutionMetadata
        {
            Backend = "sql",
            Operation = DataServiceOperation.Query,
            StartedAt = start,
            EndedAt = end,
            AffectedRows = null,
            ResultCount = rows.Count,
            TraceChain = request.TraceChain ?? new List<TraceNode>(),
            Comment = request.Comment,
            BackendRequestId = null,
            DebugQuery = compiled.DebugSql(Dialect.Kind)
        };

        await ContinuousPagePlanner.RegisterAsync(request.ContinuousPageRuntime, continuous, rows);
        return new QueryResult { Rows = rows, Metadata = metadata };
    }

    public async Task<MutationResult> MutateAsync(MutationRequest request)
    {
        if (request is BatchMutationRequest batchReq)
        {
            ulong totalAffected = 0;
            var batchStart = DateTimeOffset.UtcNow;
            foreach (var req in batchReq.Requests)
            {
                var res = await MutateAsync(req);
                totalAffected += res.AffectedRows;
            }
            var batchEnd = DateTimeOffset.UtcNow;

            return new MutationResult
            {
                AffectedRows = totalAffected,
                GeneratedValues = new Record(),
                Metadata = new ExecutionMetadata
                {
                    Backend = "sql",
                    Operation = DataServiceOperation.Batch,
                    StartedAt = batchStart,
                    EndedAt = batchEnd,
                    AffectedRows = totalAffected,
                    ResultCount = null,
                    TraceChain = new List<TraceNode>(),
                    Comment = null,
                    BackendRequestId = null,
                    DebugQuery = null
                }
            };
        }

        string entityName = request switch
        {
            InsertMutationRequest i => i.Command.Entity,
            UpdateMutationRequest u => u.Command.Entity,
            DeleteMutationRequest d => d.Command.Entity,
            RecoverMutationRequest r => r.Command.Entity,
            _ => throw new NotSupportedException($"Unsupported mutation request: {request.GetType()}")
        };

        var entityDesc = SchemaProvider.GetEntity(entityName)
            ?? throw new SqlExecutorException($"SQL compile error: unknown entity {entityName}");

        CompiledQuery compiled;
        try
        {
            compiled = request switch
            {
                InsertMutationRequest i => Dialect.CompileInsert(entityDesc, i.Command),
                UpdateMutationRequest u => Dialect.CompileUpdate(entityDesc, u.Command),
                DeleteMutationRequest d => Dialect.CompileDelete(entityDesc, d.Command),
                RecoverMutationRequest r => Dialect.CompileRecover(entityDesc, r.Command),
                _ => throw new InvalidOperationException()
            };
        }
        catch (SqlCompileException ex)
        {
            throw new SqlExecutorException($"SQL compile error: {ex.Message}", ex);
        }

        var start = DateTimeOffset.UtcNow;
        ulong affectedRows;
        try
        {
            affectedRows = await Transport.ExecuteSqlAsync(compiled);
        }
        catch (Exception ex)
        {
            throw new SqlExecutorException($"Transport error: {ex.Message}", ex);
        }
        var end = DateTimeOffset.UtcNow;

        var operation = request switch
        {
            InsertMutationRequest => DataServiceOperation.Insert,
            UpdateMutationRequest => DataServiceOperation.Update,
            DeleteMutationRequest => DataServiceOperation.Delete,
            RecoverMutationRequest => DataServiceOperation.Recover,
            _ => throw new InvalidOperationException()
        };

        var metadata = new ExecutionMetadata
        {
            Backend = "sql",
            Operation = operation,
            StartedAt = start,
            EndedAt = end,
            AffectedRows = affectedRows,
            ResultCount = null,
            TraceChain = request.TraceChain?.ToList() ?? new List<TraceNode>(),
            Comment = request.Comment,
            BackendRequestId = null,
            DebugQuery = compiled.DebugSql(Dialect.Kind)
        };

        return new MutationResult
        {
            AffectedRows = affectedRows,
            GeneratedValues = new Record(),
            Metadata = metadata
        };
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        if (Transport is not ISqlTransactionTransport txTransport)
        {
            throw new NotSupportedException("Transport does not support transactions");
        }

        ISqlTransaction tx;
        try
        {
            tx = await txTransport.BeginSqlAsync();
        }
        catch (Exception ex)
        {
            throw new SqlExecutorException($"Transport error: {ex.Message}", ex);
        }

        return new SqlDataServiceTransaction(Dialect, tx, SchemaProvider);
    }

    public async IAsyncEnumerable<StreamChunk> QueryStreamAsync(QueryRequest request, int chunkSize, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
        if (request.Query.RelationLoads.Count != 0 || request.Query.ChildEnhancements.Count != 0 || request.Query.ObjectGroupBys.Count != 0)
            throw new NotSupportedException("streaming relation or aggregate enhancement is not supported; stream a root query or use ExecuteForListAsync");
        if (Transport is not IStreamingSqlTransport streaming) throw new NotSupportedException("streaming query is not supported by this transport");
        var entityDesc = SchemaProvider.GetEntity(request.Query.Entity) ?? throw new SqlExecutorException($"unknown entity {request.Query.Entity}");
        var compiled = Dialect.CompileSelect(entityDesc, request.Query);
        var currentChunk = new List<Record>();
        List<Record>? pendingChunk = null;
        int chunkIndex = 0;
        await foreach (var row in streaming.StreamSqlAsync(compiled, cancellationToken).WithCancellation(cancellationToken))
        {
            currentChunk.Add(row);
            if (currentChunk.Count >= chunkSize)
            {
                if (pendingChunk != null)
                {
                    yield return new StreamChunk { Rows = pendingChunk, ChunkIndex = chunkIndex++, IsLast = false };
                }
                pendingChunk = currentChunk;
                currentChunk = new List<Record>();
            }
        }

        if (currentChunk.Count > 0)
        {
            if (pendingChunk != null) yield return new StreamChunk { Rows = pendingChunk, ChunkIndex = chunkIndex++, IsLast = false };
            yield return new StreamChunk { Rows = currentChunk, ChunkIndex = chunkIndex, IsLast = true };
        }
        else if (pendingChunk != null)
        {
            yield return new StreamChunk { Rows = pendingChunk, ChunkIndex = chunkIndex, IsLast = true };
        }
    }
}

internal static class RelationQueryLoader
{
    public static async Task EnhanceAsync(
        ISchemaProvider schemaProvider,
        Func<QueryRequest, Task<QueryResult>> queryAsync,
        List<Record> parents,
        QueryRequest request)
    {
        if (parents.Count == 0 || request.Query.RelationLoads.Count == 0)
        {
            return;
        }
        var parentDescriptor = schemaProvider.GetEntity(request.Query.Entity)
            ?? throw new SqlExecutorException($"SQL compile error: unknown entity {request.Query.Entity}");
        foreach (var load in request.Query.RelationLoads)
        {
            var relation = parentDescriptor.RelationByName(load.Name)
                ?? throw new SqlExecutorException($"SQL compile error: missing relation {request.Query.Entity}.{load.Name}");
            var parentIds = parents
                .Where(parent => parent.ContainsKey(relation.LocalKeyValue))
                .Select(parent => parent[relation.LocalKeyValue])
                .ToList();
            if (parentIds.Count == 0)
            {
                Attach(parents, new List<Record>(), load.Name, relation);
                continue;
            }
            var childQuery = Clone(load.Query ?? new SelectQuery(relation.TargetEntity));
            childQuery.ContinuousPageFetch = null;
            childQuery.Entity = relation.TargetEntity;
            if (!childQuery.Projection.Contains(relation.ForeignKeyValue))
            {
                childQuery.Projection.Add(relation.ForeignKeyValue);
            }
            childQuery.AndFilter(Expr.InList(relation.ForeignKeyValue, parentIds));
            if (childQuery.Slice != null)
            {
                childQuery.PartitionByField(relation.ForeignKeyValue);
            }
            var childResult = await queryAsync(new QueryRequest
            {
                Query = childQuery,
                TraceChain = request.TraceChain,
                Comment = request.Comment,
                ContinuousPageRuntime = request.ContinuousPageRuntime
            });
            foreach (var child in childResult.Rows)
            {
                child.Remove("__teaql_partition_rank");
            }
            Attach(parents, childResult.Rows, load.Name, relation);
        }
    }

    private static SelectQuery Clone(SelectQuery query) => query with
    {
        Projection = new List<string>(query.Projection),
        ExprProjection = new List<NamedExpr>(query.ExprProjection),
        OrderByItems = new List<OrderBy>(query.OrderByItems),
        AggregateItems = new List<Aggregate>(query.AggregateItems),
        GroupByItems = new List<string>(query.GroupByItems),
        RelationLoads = new List<RelationLoad>(query.RelationLoads),
        TraceChain = new List<TraceNode>(query.TraceChain),
        RawSqlSearchCriteriaItems = new List<string>(query.RawSqlSearchCriteriaItems),
        DynamicProperties = new List<RawSqlProjection>(query.DynamicProperties),
        RawProjections = new List<RawSqlProjection>(query.RawProjections),
        ObjectGroupBys = new List<ObjectGroupBy>(query.ObjectGroupBys),
        ChildEnhancements = new List<SelectQuery>(query.ChildEnhancements)
    };

    private static void Attach(
        List<Record> parents,
        List<Record> children,
        string relationName,
        RelationDescriptor relation)
    {
        var buckets = children
            .Where(child => child.ContainsKey(relation.ForeignKeyValue))
            .GroupBy(child => child[relation.ForeignKeyValue])
            .ToDictionary(group => group.Key, group => group.ToList());
        foreach (var parent in parents)
        {
            var related = parent.TryGetValue(relation.LocalKeyValue, out var localKey)
                && buckets.TryGetValue(localKey, out var bucket)
                ? bucket
                : new List<Record>();
            parent[relationName] = relation.IsMany
                ? new Value.ListValue(related.Select(row => (Value)new Value.ObjectValue(row)).ToList())
                : related.Count > 0 ? new Value.ObjectValue(related[0]) : new Value.NullValue();
        }
    }
}

public class SqlDataServiceTransaction : ITransaction, IStreamQueryExecutor
{
    public SqlDialect Dialect { get; }
    public ISqlTransaction Transport { get; }
    public ISchemaProvider SchemaProvider { get; }

    public SqlDataServiceTransaction(SqlDialect dialect, ISqlTransaction transport, ISchemaProvider schemaProvider)
    {
        Dialect = dialect;
        Transport = transport;
        SchemaProvider = schemaProvider;
    }

    public DataServiceCapabilities Capabilities => new DataServiceCapabilities
    {
        Query = true,
        Mutation = true,
        Transaction = false,
        Schema = false,
        IdGeneration = false,
        BatchMutation = true,
        Returning = false
    };

    public async Task<QueryResult> QueryAsync(QueryRequest request)
    {
        var query = (request.Query.ContinuousPageFetch == null ? request.Query : QueryClone.Clone(request.Query)).PrepareForList();
        var continuous = await ContinuousPagePlanner.PrepareAsync(request.ContinuousPageRuntime, query);
        var entityDesc = SchemaProvider.GetEntity(query.Entity)
            ?? throw new SqlExecutorException($"SQL compile error: unknown entity {query.Entity}");

        CompiledQuery compiled;
        try
        {
            compiled = Dialect.CompileSelect(entityDesc, query);
        }
        catch (SqlCompileException ex)
        {
            throw new SqlExecutorException($"SQL compile error: {ex.Message}", ex);
        }

        var start = DateTimeOffset.UtcNow;
        List<Record> rows;
        try
        {
            rows = await Transport.FetchAllSqlAsync(compiled);
        }
        catch (Exception ex)
        {
            throw new SqlExecutorException($"Transport error: {ex.Message}", ex);
        }
        var effectiveRequest = new QueryRequest { Query = query, TraceChain = request.TraceChain, Comment = request.Comment, ContinuousPageRuntime = request.ContinuousPageRuntime };
        await RelationQueryLoader.EnhanceAsync(SchemaProvider, QueryAsync, rows, effectiveRequest);
        var end = DateTimeOffset.UtcNow;

        var metadata = new ExecutionMetadata
        {
            Backend = "sql",
            Operation = DataServiceOperation.Query,
            StartedAt = start,
            EndedAt = end,
            AffectedRows = null,
            ResultCount = rows.Count,
            TraceChain = request.TraceChain ?? new List<TraceNode>(),
            Comment = request.Comment,
            BackendRequestId = null,
            DebugQuery = compiled.DebugSql(Dialect.Kind)
        };

        await ContinuousPagePlanner.RegisterAsync(request.ContinuousPageRuntime, continuous, rows);
        return new QueryResult { Rows = rows, Metadata = metadata };
    }

    public async Task<MutationResult> MutateAsync(MutationRequest request)
    {
        if (request is BatchMutationRequest batchReq)
        {
            ulong totalAffected = 0;
            var batchStart = DateTimeOffset.UtcNow;
            foreach (var req in batchReq.Requests)
            {
                var res = await MutateAsync(req);
                totalAffected += res.AffectedRows;
            }
            var batchEnd = DateTimeOffset.UtcNow;

            return new MutationResult
            {
                AffectedRows = totalAffected,
                GeneratedValues = new Record(),
                Metadata = new ExecutionMetadata
                {
                    Backend = "sql",
                    Operation = DataServiceOperation.Batch,
                    StartedAt = batchStart,
                    EndedAt = batchEnd,
                    AffectedRows = totalAffected,
                    ResultCount = null,
                    TraceChain = new List<TraceNode>(),
                    Comment = null,
                    BackendRequestId = null,
                    DebugQuery = null
                }
            };
        }

        string entityName = request switch
        {
            InsertMutationRequest i => i.Command.Entity,
            UpdateMutationRequest u => u.Command.Entity,
            DeleteMutationRequest d => d.Command.Entity,
            RecoverMutationRequest r => r.Command.Entity,
            _ => throw new NotSupportedException($"Unsupported mutation request: {request.GetType()}")
        };

        var entityDesc = SchemaProvider.GetEntity(entityName)
            ?? throw new SqlExecutorException($"SQL compile error: unknown entity {entityName}");

        CompiledQuery compiled;
        try
        {
            compiled = request switch
            {
                InsertMutationRequest i => Dialect.CompileInsert(entityDesc, i.Command),
                UpdateMutationRequest u => Dialect.CompileUpdate(entityDesc, u.Command),
                DeleteMutationRequest d => Dialect.CompileDelete(entityDesc, d.Command),
                RecoverMutationRequest r => Dialect.CompileRecover(entityDesc, r.Command),
                _ => throw new InvalidOperationException()
            };
        }
        catch (SqlCompileException ex)
        {
            throw new SqlExecutorException($"SQL compile error: {ex.Message}", ex);
        }

        var start = DateTimeOffset.UtcNow;
        ulong affectedRows;
        try
        {
            affectedRows = await Transport.ExecuteSqlAsync(compiled);
        }
        catch (Exception ex)
        {
            throw new SqlExecutorException($"Transport error: {ex.Message}", ex);
        }
        var end = DateTimeOffset.UtcNow;

        var operation = request switch
        {
            InsertMutationRequest => DataServiceOperation.Insert,
            UpdateMutationRequest => DataServiceOperation.Update,
            DeleteMutationRequest => DataServiceOperation.Delete,
            RecoverMutationRequest => DataServiceOperation.Recover,
            _ => throw new InvalidOperationException()
        };

        var metadata = new ExecutionMetadata
        {
            Backend = "sql",
            Operation = operation,
            StartedAt = start,
            EndedAt = end,
            AffectedRows = affectedRows,
            ResultCount = null,
            TraceChain = request.TraceChain?.ToList() ?? new List<TraceNode>(),
            Comment = request.Comment,
            BackendRequestId = null,
            DebugQuery = compiled.DebugSql(Dialect.Kind)
        };

        return new MutationResult
        {
            AffectedRows = affectedRows,
            GeneratedValues = new Record(),
            Metadata = metadata
        };
    }

    public async Task CommitAsync()
    {
        try
        {
            await Transport.CommitSqlAsync();
        }
        catch (Exception ex)
        {
            throw new SqlExecutorException($"Transport error: {ex.Message}", ex);
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            await Transport.RollbackSqlAsync();
        }
        catch (Exception ex)
        {
            throw new SqlExecutorException($"Transport error: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        Transport.Dispose();
    }

    public async IAsyncEnumerable<StreamChunk> QueryStreamAsync(QueryRequest request, int chunkSize, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
        if (request.Query.RelationLoads.Count != 0 || request.Query.ChildEnhancements.Count != 0 || request.Query.ObjectGroupBys.Count != 0)
            throw new NotSupportedException("streaming relation or aggregate enhancement is not supported; stream a root query or use ExecuteForListAsync");
        if (Transport is not IStreamingSqlTransport streaming) throw new NotSupportedException("streaming query is not supported by this transport");
        var entityDesc = SchemaProvider.GetEntity(request.Query.Entity) ?? throw new SqlExecutorException($"unknown entity {request.Query.Entity}");
        var compiled = Dialect.CompileSelect(entityDesc, request.Query);
        var currentChunk = new List<Record>();
        List<Record>? pendingChunk = null;
        int chunkIndex = 0;

        await foreach (var row in streaming.StreamSqlAsync(compiled, cancellationToken).WithCancellation(cancellationToken))
        {
            currentChunk.Add(row);
            if (currentChunk.Count >= chunkSize)
            {
                if (pendingChunk != null)
                {
                    yield return new StreamChunk { Rows = pendingChunk, ChunkIndex = chunkIndex++, IsLast = false };
                }
                pendingChunk = currentChunk;
                currentChunk = new List<Record>();
            }
        }

        if (currentChunk.Count > 0)
        {
            if (pendingChunk != null) yield return new StreamChunk { Rows = pendingChunk, ChunkIndex = chunkIndex++, IsLast = false };
            yield return new StreamChunk { Rows = currentChunk, ChunkIndex = chunkIndex, IsLast = true };
        }
        else if (pendingChunk != null)
        {
            yield return new StreamChunk { Rows = pendingChunk, ChunkIndex = chunkIndex, IsLast = true };
        }
    }
}
