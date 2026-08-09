using System.Collections.Generic;

namespace TeaQL.Core;

public enum MutationKind { Insert, Update, Delete, Recover }

public record InsertCommand
{
    public string Entity { get; init; } = "";
    public Record Values { get; init; } = new();
    public List<TraceNode> TraceChain { get; init; } = new();
}

public record UpdateCommand
{
    public string Entity { get; init; } = "";
    public Value Id { get; init; } = new Value.NullValue();
    public long? ExpectedVersion { get; init; }
    public Record Values { get; init; } = new();
    public List<TraceNode> TraceChain { get; init; } = new();
    public Record? OldValues { get; init; }
}

public record BatchInsertCommand
{
    public string Entity { get; init; } = "";
    public List<Record> BatchValues { get; init; } = new();
    public List<List<TraceNode>> TraceChains { get; init; } = new();
}

public record BatchUpdateCommand
{
    public string Entity { get; init; } = "";
    public List<Value> BatchIds { get; init; } = new();
    public List<long?> BatchExpectedVersions { get; init; } = new();
    public List<Record> BatchValues { get; init; } = new();
    public List<string> UpdateFields { get; init; } = new();
    public List<List<TraceNode>> TraceChains { get; init; } = new();
    public List<Record?> BatchOldValues { get; init; } = new();
}

public record DeleteCommand
{
    public string Entity { get; init; } = "";
    public Value Id { get; init; } = new Value.NullValue();
    public long? ExpectedVersion { get; init; }
    public bool SoftDelete { get; init; } = true;
    public List<TraceNode> TraceChain { get; init; } = new();
}

public record RecoverCommand
{
    public string Entity { get; init; } = "";
    public Value Id { get; init; } = new Value.NullValue();
    public long ExpectedVersion { get; init; }
    public List<TraceNode> TraceChain { get; init; } = new();
}
