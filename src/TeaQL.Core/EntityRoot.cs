using System.Collections.Concurrent;

namespace TeaQL.Core;

public sealed record EntityKey
{
    public string EntityType { get; }
    public Value Id { get; }

    public EntityKey(string entityType, Value id)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type is required", nameof(entityType));
        EntityType = entityType;
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }
}

/// <summary>Pending mutation ledger shared by one generated object graph.</summary>
public sealed class EntityRoot
{
    private readonly ConcurrentDictionary<EntityKey, ConcurrentDictionary<string, Value>> _changes = new();
    private readonly ConcurrentDictionary<EntityKey, long> _originalVersions = new();
    private readonly ConcurrentDictionary<EntityKey, byte> _newKeys = new();
    private readonly ConcurrentDictionary<EntityKey, byte> _deletedKeys = new();

    public void Set(EntityKey key, string field, Value value)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field is required", nameof(field));
        _changes.GetOrAdd(key, _ => new ConcurrentDictionary<string, Value>())[field] = value;
    }

    public IReadOnlyDictionary<EntityKey, IReadOnlyDictionary<string, Value>> Changes() =>
        _changes.ToDictionary(item => item.Key,
            item => (IReadOnlyDictionary<string, Value>)new Dictionary<string, Value>(item.Value));

    public void SetOriginalVersion(EntityKey key, long version) => _originalVersions[key] = version;
    public long? OriginalVersion(EntityKey key) => _originalVersions.TryGetValue(key, out var value) ? value : null;
    public void MarkAsNew(EntityKey key) => _newKeys[key] = 0;
    public void MarkAsDeleted(EntityKey key) { _changes.TryRemove(key, out _); _deletedKeys[key] = 0; }
    public bool IsNew(EntityKey key) => _newKeys.ContainsKey(key);
    public bool IsDeleted(EntityKey key) => _deletedKeys.ContainsKey(key);

    public void ClearCommitted()
    {
        _changes.Clear();
        _newKeys.Clear();
        _deletedKeys.Clear();
    }
}
