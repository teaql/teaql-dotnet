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
