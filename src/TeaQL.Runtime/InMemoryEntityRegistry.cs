using System.Collections.Concurrent;

namespace TeaQL.Runtime;

public class InMemoryEntityRegistry : IEntityRegistry
{
    private readonly ConcurrentDictionary<string, string> _entities = new();

    public void Register(string entity)
    {
        _entities[entity] = entity;
    }

    public InMemoryEntityRegistry WithEntity(string entity)
    {
        Register(entity);
        return this;
    }

    public bool Contains(string entity)
    {
        return _entities.ContainsKey(entity);
    }
}
