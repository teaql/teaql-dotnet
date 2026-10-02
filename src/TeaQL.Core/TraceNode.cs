namespace TeaQL.Core;

public record TraceNode(string EntityType, ulong? EntityId, string Comment)
{
    public int Level { get; init; }
    public string Kind { get; init; } = "entity";
    public string Name { get; init; } = EntityType;
    /// <summary>Operation or qualified property, independent of intent prose.</summary>
    public string Detail { get; init; } = "";
}
