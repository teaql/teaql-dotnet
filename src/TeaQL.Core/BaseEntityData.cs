namespace TeaQL.Core;

public record BaseEntityData
{
    public ulong Id { get; init; }
    public long Version { get; init; }
    public Record Dynamic { get; init; } = new();
}
