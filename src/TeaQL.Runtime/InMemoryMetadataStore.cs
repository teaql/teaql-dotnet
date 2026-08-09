using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public class InMemoryMetadataStore : IMetadataStore
{
    private readonly ConcurrentDictionary<string, EntityDescriptor> _entities = new();

    public void Register(EntityDescriptor entity)
    {
        _entities[entity.Name] = entity;
    }

    public InMemoryMetadataStore WithEntity(EntityDescriptor entity)
    {
        Register(entity);
        return this;
    }

    public EntityDescriptor? GetEntity(string name)
    {
        return _entities.TryGetValue(name, out var entity) ? entity : null;
    }

    public IReadOnlyList<EntityDescriptor> GetAllEntities()
    {
        return _entities.Values.ToList();
    }

    public void RecordMetadataLog(ExecutionMetadata metadata)
    {
        // Default implementation does nothing, as in Rust
    }
}
