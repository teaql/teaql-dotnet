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

    public EntityKey(string entityType, long id) : this(entityType, new Value.I64Value(id)) { }
}

/// <summary>Pending mutation ledger shared by one generated object graph.</summary>
public sealed class EntityRoot
{
    private readonly ConcurrentDictionary<EntityKey, ConcurrentDictionary<string, Value>> _changes = new();
    private readonly ConcurrentDictionary<EntityKey, long> _originalVersions = new();
    private readonly ConcurrentDictionary<EntityKey, byte> _newKeys = new();
    private readonly ConcurrentDictionary<EntityKey, byte> _deletedKeys = new();
    private readonly ConcurrentDictionary<EntityKey, IReadOnlyList<TraceNode>> _traces = new();
    private readonly object _gate = new();

    /// <summary>Complete per-entity replacement, not a suffix or a Context trace.</summary>
    public void SetTraceChain(EntityKey key, IEnumerable<TraceNode> chain)
    {
        var snapshot = Array.AsReadOnly(chain.Select(node => node with { }).ToArray());
        lock (_gate) _traces[key] = snapshot;
    }
    public IReadOnlyList<TraceNode> TraceChain(EntityKey key)
    {
        lock (_gate) return _traces.TryGetValue(key, out var chain) ? chain : Array.Empty<TraceNode>();
    }

    public void Set(EntityKey key, string field, Value value)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field is required", nameof(field));
        lock (_gate) _changes.GetOrAdd(key, _ => new ConcurrentDictionary<string, Value>())[field] = value;
    }

    public IReadOnlyDictionary<EntityKey, IReadOnlyDictionary<string, Value>> Changes()
    {
        lock (_gate) return _changes.ToDictionary(item => item.Key,
            item => (IReadOnlyDictionary<string, Value>)new Dictionary<string, Value>(item.Value));
    }

    public Record Change(EntityKey key)
    {
        lock (_gate) return _changes.TryGetValue(key, out var values) ? new Record(values) : new Record();
    }

    /// <summary>Import only a reached entity; neither drain nor adopt its source graph.</summary>
    public void MergeEntityFrom(EntityRoot other, EntityKey key)
    {
        if (ReferenceEquals(this, other)) return;
        var snapshot = other.Snapshot(key);
        lock (_gate) { ValidateVersion(key, snapshot.Version); Import(snapshot); }
    }

    public void MergeFrom(EntityRoot other)
    {
        if (ReferenceEquals(this, other)) return;
        var snapshots = other.Snapshots();
        lock (_gate)
        {
            foreach (var snapshot in snapshots) ValidateVersion(snapshot.Key, snapshot.Version);
            foreach (var snapshot in snapshots) Import(snapshot);
        }
    }

    public void Rekey(EntityKey oldKey, EntityKey newKey)
    {
        if (oldKey == newKey) return;
        lock (_gate)
        {
            ValidateVersion(newKey, OriginalVersion(oldKey));
            if (_changes.TryRemove(oldKey, out var values))
                foreach (var (field, value) in values) Set(newKey, field, value);
            if (_originalVersions.TryRemove(oldKey, out var version)) _originalVersions[newKey] = version;
            if (_newKeys.TryRemove(oldKey, out _)) _newKeys[newKey] = 0;
            if (_deletedKeys.TryRemove(oldKey, out _)) _deletedKeys[newKey] = 0;
            if (_traces.TryRemove(oldKey, out var trace)) _traces[newKey] = trace;
        }
    }

    public void ClearEntity(EntityKey key)
    {
        lock (_gate)
        {
            _changes.TryRemove(key, out _); _newKeys.TryRemove(key, out _); _deletedKeys.TryRemove(key, out _);
            _traces.TryRemove(key, out _);
        }
    }

    public void SetOriginalVersion(EntityKey key, long version)
    {
        lock (_gate) { ValidateVersion(key, version); _originalVersions[key] = version; }
    }
    /// <summary>Advance authoritative state only after this entity's commit has cleared pending changes.</summary>
    public void AcceptCommittedVersion(EntityKey key, long version)
    {
        lock (_gate)
        {
            if (HasPending(key)) throw new InvalidOperationException("ENTITY_PENDING_CHANGES: clear committed changes before accepting a version");
            _originalVersions[key] = version;
        }
    }
    public long? OriginalVersion(EntityKey key)
    {
        lock (_gate) return _originalVersions.TryGetValue(key, out var value) ? value : null;
    }
    public void MarkAsNew(EntityKey key) { lock (_gate) _newKeys[key] = 0; }
    public void MarkAsDeleted(EntityKey key)
    {
        lock (_gate) { _changes.TryRemove(key, out _); _deletedKeys[key] = 0; }
    }
    public bool IsNew(EntityKey key) { lock (_gate) return _newKeys.ContainsKey(key); }
    public bool IsDeleted(EntityKey key) { lock (_gate) return _deletedKeys.ContainsKey(key); }
    public bool HasPending(EntityKey key)
    {
        lock (_gate) return _changes.ContainsKey(key) || _newKeys.ContainsKey(key) || _deletedKeys.ContainsKey(key);
    }
    public bool IsEmpty { get { lock (_gate) return _changes.IsEmpty && _newKeys.IsEmpty && _deletedKeys.IsEmpty; } }

    public void MarkAsPersisted(EntityKey key)
    {
        lock (_gate)
        {
            _newKeys.TryRemove(key, out _);
            _deletedKeys.TryRemove(key, out _);
        }
    }

    public void ClearCommitted()
    {
        lock (_gate)
        {
            _changes.Clear();
            _newKeys.Clear();
            _deletedKeys.Clear();
            _traces.Clear();
        }
    }

    private sealed record Entry(EntityKey Key, Record Values, long? Version, bool New, bool Deleted,
        IReadOnlyList<TraceNode>? Trace);

    private Entry Snapshot(EntityKey key)
    {
        lock (_gate) return new Entry(key, Change(key), OriginalVersion(key), IsNew(key), IsDeleted(key),
            _traces.TryGetValue(key, out var trace) ? trace : null);
    }
    private Entry[] Snapshots()
    {
        lock (_gate) return _changes.Keys.Concat(_originalVersions.Keys).Concat(_newKeys.Keys)
            .Concat(_deletedKeys.Keys).Concat(_traces.Keys).Distinct().Select(Snapshot).ToArray();
    }
    private void ValidateVersion(EntityKey key, long? incoming)
    {
        if (incoming.HasValue && _originalVersions.TryGetValue(key, out var original) && original != incoming.Value)
            throw new InvalidOperationException($"ENTITY_VERSION_CONFLICT: {key.EntityType} has loaded versions {original} and {incoming.Value}");
    }
    private void Import(Entry entry)
    {
        foreach (var (field, value) in entry.Values) Set(entry.Key, field, value);
        if (entry.New) MarkAsNew(entry.Key);
        if (entry.Deleted) MarkAsDeleted(entry.Key);
        if (entry.Version.HasValue) _originalVersions[entry.Key] = entry.Version.Value;
        if (entry.Trace != null) _traces[entry.Key] = entry.Trace;
    }
}
