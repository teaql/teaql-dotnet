using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using TeaQL.Core;

namespace TeaQL.DataService;

public class DataServiceCapabilities
{
    public bool Query { get; set; }
    public bool Mutation { get; set; }
    public bool Transaction { get; set; }
    public bool Schema { get; set; }
    public bool IdGeneration { get; set; }
    public bool BatchMutation { get; set; }
    public bool Returning { get; set; }
}

public class QueryRequest
{
    public SelectQuery Query { get; set; } = new();
    public List<TraceNode> TraceChain { get; set; } = new();
    public string? Comment { get; set; }
    public string? Purpose { get; set; }
    /// <summary>Runtime-only observer; providers must not serialize it.</summary>
    public IRelationLoadObserver? RelationLoadObserver { get; set; }
}

public interface IRelationLoadObserver
{
    Task ObserveAsync(string entity, string relation,
        IReadOnlyDictionary<string, object> attributes, Func<Task> body);
}

public class QueryResult
{
    public List<Record> Rows { get; set; } = new();
    public ExecutionMetadata Metadata { get; set; } = new();
    public Dictionary<string, SmartList<Record>> Facets { get; set; } = new();
}

public abstract class MutationRequest
{
    public abstract IReadOnlyList<TraceNode> TraceChain { get; }
    public abstract string? Comment { get; }
}

public class InsertMutationRequest : MutationRequest
{
    public InsertCommand Command { get; }
    
    public InsertMutationRequest(InsertCommand command)
    {
        Command = command;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Command.TraceChain;
    public override string? Comment => Command.TraceChain.LastOrDefault()?.Comment;
}

public class UpdateMutationRequest : MutationRequest
{
    public UpdateCommand Command { get; }
    
    public UpdateMutationRequest(UpdateCommand command)
    {
        Command = command;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Command.TraceChain;
    public override string? Comment => Command.TraceChain.LastOrDefault()?.Comment;
}

public class DeleteMutationRequest : MutationRequest
{
    public DeleteCommand Command { get; }
    
    public DeleteMutationRequest(DeleteCommand command)
    {
        Command = command;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Command.TraceChain;
    public override string? Comment => Command.TraceChain.LastOrDefault()?.Comment;
}

public class RecoverMutationRequest : MutationRequest
{
    public RecoverCommand Command { get; }
    
    public RecoverMutationRequest(RecoverCommand command)
    {
        Command = command;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Command.TraceChain;
    public override string? Comment => Command.TraceChain.LastOrDefault()?.Comment;
}

public class BatchMutationRequest : MutationRequest
{
    public List<MutationRequest> Requests { get; }
    
    public BatchMutationRequest(List<MutationRequest> requests)
    {
        Requests = requests;
    }

    public override IReadOnlyList<TraceNode> TraceChain => Array.Empty<TraceNode>();
    public override string? Comment => null;
}

public class MutationResult
{
    public ulong AffectedRows { get; set; }
    public Record GeneratedValues { get; set; } = new();
    public Record? PersistedRecord { get; set; }
    public ExecutionMetadata Metadata { get; set; } = new();
}

public enum DataServiceOperation
{
    Query,
    Insert,
    Update,
    Delete,
    Recover,
    Batch,
    Schema
}

public class ExecutionMetadata
{
    public string Backend { get; set; } = string.Empty;
    public DataServiceOperation Operation { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset EndedAt { get; set; }
    public ulong? AffectedRows { get; set; }
    public int? ResultCount { get; set; }
    public List<TraceNode> TraceChain { get; set; } = new();
    public string? Comment { get; set; }
    public string? Purpose { get; set; }
    public string? AuditReason { get; set; }
    public string? BackendRequestId { get; set; }
    /// <summary>Provider-native request text with placeholders, never interpolated bind values.</summary>
    public string? ParameterizedQuery { get; set; }
    /// <summary>Structured bind values for trusted runtime diagnostics.</summary>
    public IReadOnlyList<Value> Parameters { get; set; } = Array.Empty<Value>();
    public int ParameterCount => Parameters.Count;
    public string? DebugQuery { get; set; }
}

public interface IDataService
{
    DataServiceCapabilities Capabilities { get; }
    Task<QueryResult> QueryAsync(QueryRequest request);
    Task<MutationResult> MutateAsync(MutationRequest request);
}

public class StreamChunk
{
    public List<Record> Rows { get; set; } = new();
    public int ChunkIndex { get; set; }
    public bool IsLast { get; set; }
}

public interface IStreamQueryExecutor : IDataService
{
    IAsyncEnumerable<StreamChunk> QueryStreamAsync(QueryRequest request, int chunkSize, CancellationToken cancellationToken = default);
}

public interface ITransaction : IDataService, IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}

public interface ITransactionExecutor : IDataService
{
    Task<ITransaction> BeginTransactionAsync();
}

internal class SchemaRequest
{
    public string EntityName { get; set; } = string.Empty;
}

internal class SchemaResult
{
    public bool Changed { get; set; }
}

internal interface ISchemaExecutor : IDataService
{
    Task<SchemaResult> EnsureSchemaAsync(SchemaRequest request);
}

public interface IIdGeneratorExecutor : IDataService
{
    Task<ulong> NextIdAsync(string entity);
    Task EnsureIdFloorAsync(string entity, ulong floor);
}
