using System.Collections.Generic;

namespace TeaQL.Core;

public enum MutationKind { Insert, Update, Delete, Recover }

public record InsertCommand
{
    public string Entity { get; init; } = "";
    public Record Values { get; init; } = new();
    public List<TraceNode> TraceChain { get; init; } = new();

    public InsertCommand() { }

    public InsertCommand(string entity)
    {
        Entity = entity;
    }

    public InsertCommand Value(string field, Value value)
    {
        Values[field] = value;
        return this;
    }
}

public record UpdateCommand
{
    public string Entity { get; init; } = "";
    public Value Id { get; init; } = new Value.NullValue();
    public long? ExpectedVersionValue { get; set; }
    public Record Values { get; init; } = new();
    public List<TraceNode> TraceChain { get; init; } = new();
    public Record? OldValues { get; init; }

    public UpdateCommand() { }

    public UpdateCommand(string entity, Value id)
    {
        Entity = entity;
        Id = id;
    }

    public UpdateCommand ExpectedVersion(long version)
    {
        ExpectedVersionValue = version;
        return this;
    }

    public UpdateCommand Value(string field, Value value)
    {
        Values[field] = value;
        return this;
    }
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
    public long? ExpectedVersionValue { get; set; }
    public bool SoftDelete { get; set; } = true;
    public List<TraceNode> TraceChain { get; init; } = new();

    public DeleteCommand() { }

    public DeleteCommand(string entity, Value id)
    {
        Entity = entity;
        Id = id;
    }

    public DeleteCommand ExpectedVersion(long version)
    {
        ExpectedVersionValue = version;
        return this;
    }

    public DeleteCommand HardDelete()
    {
        SoftDelete = false;
        return this;
    }
}

public record RecoverCommand
{
    public string Entity { get; init; } = "";
    public Value Id { get; init; } = new Value.NullValue();
    public long ExpectedVersionValue { get; init; }
    public List<TraceNode> TraceChain { get; init; } = new();
}
