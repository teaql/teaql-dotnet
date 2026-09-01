namespace TeaQL.Core;

public record TraceNode(string EntityType, ulong? EntityId, string Comment)
{
    public int Level { get; init; }
    public string Kind { get; init; } = "entity";
    public string Name { get; init; } = EntityType;
}
