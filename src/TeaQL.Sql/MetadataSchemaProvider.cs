using TeaQL.Core;

namespace TeaQL.Sql;

/// <summary>Adapts installed runtime metadata to the SQL executor schema contract.</summary>
public sealed class MetadataSchemaProvider(Func<string, EntityDescriptor?> lookup) : ISchemaProvider
{
    public EntityDescriptor? GetEntity(string name) => lookup(name);
}
