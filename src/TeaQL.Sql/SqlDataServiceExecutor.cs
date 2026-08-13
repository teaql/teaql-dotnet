using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Sql;

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
        request.Query.PrepareForList();
        var entityDesc = SchemaProvider.GetEntity(request.Query.Entity)
            ?? throw new SqlExecutorException($"SQL compile error: unknown entity {request.Query.Entity}");

        CompiledQuery compiled;
        try
        {
            compiled = Dialect.CompileSelect(entityDesc, request.Query);
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
        await RelationQueryLoader.EnhanceAsync(SchemaProvider, QueryAsync, rows, request);
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

    public async Task<List<StreamChunk>> QueryStreamAsync(QueryRequest request, int chunkSize)
    {
        var queryResult = await QueryAsync(request);
        var chunks = new List<StreamChunk>();
        var currentChunk = new List<Record>();
        int chunkIndex = 0;

        foreach (var row in queryResult.Rows)
        {
            currentChunk.Add(row);
            if (currentChunk.Count >= chunkSize)
            {
                chunks.Add(new StreamChunk
                {
                    Rows = currentChunk,
                    ChunkIndex = chunkIndex,
                    IsLast = false
                });
                currentChunk = new List<Record>();
                chunkIndex++;
            }
        }

        chunks.Add(new StreamChunk
        {
            Rows = currentChunk,
            ChunkIndex = chunkIndex,
            IsLast = true
        });

        return chunks;
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
        if (parents.Count == 0 || request.Query.Relations.Count == 0)
        {
            return;
        }
        var parentDescriptor = schemaProvider.GetEntity(request.Query.Entity)
            ?? throw new SqlExecutorException($"SQL compile error: unknown entity {request.Query.Entity}");
        foreach (var load in request.Query.Relations)
        {
            var relation = parentDescriptor.RelationByName(load.Name)
                ?? throw new SqlExecutorException($"SQL compile error: missing relation {request.Query.Entity}.{load.Name}");
            var parentIds = parents
                .Where(parent => parent.ContainsKey(relation.LocalKey))
                .Select(parent => parent[relation.LocalKey])
                .ToList();
            if (parentIds.Count == 0)
            {
                Attach(parents, new List<Record>(), load.Name, relation);
                continue;
            }
            var childQuery = Clone(load.Query ?? new SelectQuery(relation.TargetEntity));
            childQuery.Entity = relation.TargetEntity;
            if (!childQuery.Projection.Contains(relation.ForeignKey))
            {
                childQuery.Projection.Add(relation.ForeignKey);
            }
            childQuery.AndFilter(Expr.InList(relation.ForeignKey, parentIds));
            if (childQuery.Slice != null)
            {
                childQuery.PartitionByField(relation.ForeignKey);
            }
            var childResult = await queryAsync(new QueryRequest
            {
                Query = childQuery,
                TraceChain = request.TraceChain,
                Comment = request.Comment
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
        OrderBy = new List<OrderBy>(query.OrderBy),
        Aggregates = new List<Aggregate>(query.Aggregates),
        GroupBy = new List<string>(query.GroupBy),
        Relations = new List<RelationLoad>(query.Relations),
        TraceChain = new List<TraceNode>(query.TraceChain),
        RawSqlSearchCriteria = new List<string>(query.RawSqlSearchCriteria),
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
            .Where(child => child.ContainsKey(relation.ForeignKey))
            .GroupBy(child => child[relation.ForeignKey])
            .ToDictionary(group => group.Key, group => group.ToList());
        foreach (var parent in parents)
        {
            var related = parent.TryGetValue(relation.LocalKey, out var localKey)
                && buckets.TryGetValue(localKey, out var bucket)
                ? bucket
                : new List<Record>();
            parent[relationName] = relation.Many
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
        request.Query.PrepareForList();
        var entityDesc = SchemaProvider.GetEntity(request.Query.Entity)
            ?? throw new SqlExecutorException($"SQL compile error: unknown entity {request.Query.Entity}");

        CompiledQuery compiled;
        try
        {
            compiled = Dialect.CompileSelect(entityDesc, request.Query);
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
        await RelationQueryLoader.EnhanceAsync(SchemaProvider, QueryAsync, rows, request);
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

    public async Task<List<StreamChunk>> QueryStreamAsync(QueryRequest request, int chunkSize)
    {
        var queryResult = await QueryAsync(request);
        var chunks = new List<StreamChunk>();
        var currentChunk = new List<Record>();
        int chunkIndex = 0;

        foreach (var row in queryResult.Rows)
        {
            currentChunk.Add(row);
            if (currentChunk.Count >= chunkSize)
            {
                chunks.Add(new StreamChunk
                {
                    Rows = currentChunk,
                    ChunkIndex = chunkIndex,
                    IsLast = false
                });
                currentChunk = new List<Record>();
                chunkIndex++;
            }
        }

        chunks.Add(new StreamChunk
        {
            Rows = currentChunk,
            ChunkIndex = chunkIndex,
            IsLast = true
        });

        return chunks;
    }
}
