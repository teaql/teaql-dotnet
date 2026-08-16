using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public class UserContext
{
    private sealed record LocalCacheEntry(object Value, DateTimeOffset? ExpiresAt);
    private static readonly ConcurrentDictionary<string, LocalCacheEntry> LocalCache = new();
    private sealed record LocalLockEntry(UserContext Owner, DateTimeOffset? ExpiresAt);
    private static readonly object LocalLockGate = new();
    private static readonly Dictionary<string, LocalLockEntry> LocalLocks = new();
    public IMetadataStore? Metadata { get; set; }
    public IEntityRegistry? EntityRegistry { get; set; }
    
    private readonly ConcurrentDictionary<Type, object> _typedResources = new();
    private readonly ConcurrentDictionary<string, object> _namedResources = new();
    private readonly ConcurrentDictionary<string, Value> _locals = new();
    private readonly ConcurrentDictionary<string, List<Action<UserContext, object>>> _entityInitializers = new();
    private readonly ConcurrentBag<object> _managedEntities = new();
    
    public string TraceId { get; set; }
    public string? UserIdentifier { get; set; }
    public string? Timezone { get; set; } = "UTC";
    
    public IServiceProvider? ServiceProvider { get; set; }

    public UserContext()
    {
        var pid = Environment.ProcessId;
        var threadId = Environment.CurrentManagedThreadId;
        var user = Environment.UserName;
        UserIdentifier = $"{user}@pid-{pid}.tid-{threadId}";
        TraceId = $"req-{pid}-{threadId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    }

    public UserContext WithMetadata(IMetadataStore metadata)
    {
        Metadata = metadata;
        return this;
    }

    public UserContext WithEntityRegistry(IEntityRegistry registry)
    {
        EntityRegistry = registry;
        return this;
    }

    public UserContext WithRequestPolicy(IRequestPolicy policy)
    {
        InsertResource(policy);
        return this;
    }

    public UserContext WithAppAuditEventSink(IAppAuditEventSink sink)
    {
        InsertResource(sink);
        return this;
    }

    public UserContext WithTrustedTenant(string tenant)
    {
        if (string.IsNullOrWhiteSpace(tenant))
            throw new ArgumentException("A trusted tenant is required", nameof(tenant));
        InsertNamedResource("trustedTenant", tenant);
        return this;
    }

    public SelectQuery ApplyRequestPolicy(SelectQuery query) =>
        RequireResource<IRequestPolicy>().Apply(query);

    public Task PublishAppAuditEventAsync(IReadOnlyDictionary<string, object?> safeEvent,
        CancellationToken cancellationToken = default) =>
        RequireResource<IAppAuditEventSink>().RecordAsync(safeEvent, cancellationToken);

    public async Task EnsureSchemaAsync()
    {
        var provider = RequireResource<ISchemaExecutor>();
        var metadata = Metadata ?? throw new InvalidOperationException("Missing metadata");
        foreach (var entity in metadata.GetAllEntities())
            await provider.EnsureSchemaAsync(new SchemaRequest { EntityName = entity.Name });
    }

    public UserContext WithModule(RuntimeModule module)
    {
        module.ApplyTo(this);
        return this;
    }

    public UserContext RegisterEntityInitializer(string entityName, Action<UserContext, object> initializer)
    {
        if (string.IsNullOrWhiteSpace(entityName)) throw new ArgumentException("Entity name is required", nameof(entityName));
        ArgumentNullException.ThrowIfNull(initializer);
        var initializers = _entityInitializers.GetOrAdd(entityName, _ => new List<Action<UserContext, object>>());
        lock (initializers) initializers.Add(initializer);
        return this;
    }

    /// <summary>Applies trusted local defaults while preserving the concrete generated type.</summary>
    public T InitializeEntity<T>(string entityName, T entity) where T : class
    {
        if (string.IsNullOrWhiteSpace(entityName)) throw new ArgumentException("Entity name is required", nameof(entityName));
        ArgumentNullException.ThrowIfNull(entity);
        ApplyEntityInitializers("*", entity);
        ApplyEntityInitializers(entityName, entity);
        _managedEntities.Add(entity);
        return entity;
    }

    public IReadOnlyCollection<object> ManagedEntities => _managedEntities.ToArray();

    private void ApplyEntityInitializers(string entityName, object entity)
    {
        if (!_entityInitializers.TryGetValue(entityName, out var initializers)) return;
        Action<UserContext, object>[] snapshot;
        lock (initializers) snapshot = initializers.ToArray();
        foreach (var initializer in snapshot) initializer(this, entity);
    }

    public EntityDescriptor? GetEntity(string name)
    {
        return Metadata?.GetEntity(name);
    }

    public EntityDescriptor RequireEntity(string name)
    {
        return GetEntity(name) ?? throw new InvalidOperationException($"Missing entity: {name}");
    }

    public void InsertResource<T>(T resource) where T : class
    {
        _typedResources[typeof(T)] = resource;
    }

    public T? GetResource<T>() where T : class
    {
        if (_typedResources.TryGetValue(typeof(T), out var resource))
        {
            return (T)resource;
        }
        
        return ServiceProvider?.GetService<T>();
    }

    public T RequireResource<T>() where T : class
    {
        return GetResource<T>() ?? throw new InvalidOperationException($"Missing resource of type {typeof(T).Name}");
    }

    public void InsertNamedResource<T>(string name, T resource) where T : class
    {
        _namedResources[name] = resource;
    }

    public T? GetNamedResource<T>(string name) where T : class
    {
        return _namedResources.TryGetValue(name, out var resource) ? (T)resource : null;
    }

    public T RequireNamedResource<T>(string name) where T : class
    {
        return GetNamedResource<T>(name) ?? throw new InvalidOperationException($"Missing named resource: {name}");
    }

    public void PutLocal(string key, Value value)
    {
        _locals[key] = value;
    }

    public Value? GetLocal(string key)
    {
        return _locals.TryGetValue(key, out var value) ? value : null;
    }

    public Value? RemoveLocal(string key)
    {
        return _locals.TryRemove(key, out var value) ? value : null;
    }

    // ==========================================
    // Context Attribute
    // ==========================================
    public void PutAttribute(string key, object value) { }
    public T? GetAttribute<T>(string key) { return default; }
    public object? GetAttribute(string key) { return null; }

    // ==========================================
    // Local Cache
    // ==========================================
    public void PutToLocalCache(string key, object value, int? timeToLiveInSeconds = null)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        var expiresAt = timeToLiveInSeconds > 0
            ? DateTimeOffset.UtcNow.AddSeconds(timeToLiveInSeconds.Value)
            : (DateTimeOffset?)null;
        LocalCache[key] = new LocalCacheEntry(value, expiresAt);
    }
    public T? GetFromLocalCache<T>(string key)
    {
        if (!LocalCache.TryGetValue(key, out var entry)) return default;
        if (entry.ExpiresAt is not null && DateTimeOffset.UtcNow >= entry.ExpiresAt)
        {
            LocalCache.TryRemove(new KeyValuePair<string, LocalCacheEntry>(key, entry));
            return default;
        }
        return entry.Value is T value ? value : default;
    }
    public void RemoveFromLocalCache(string key) { LocalCache.TryRemove(key, out _); }

    // ==========================================
    // Remote Cache
    // ==========================================
    public void PutToRemoteCache(string key, object value, int? timeToLiveInSeconds = null)
    {
        GetResource<IRemoteCacheProvider>()?.Put(key, value, timeToLiveInSeconds);
    }
    public T? GetFromRemoteCache<T>(string key)
    {
        var provider = GetResource<IRemoteCacheProvider>();
        return provider != null ? provider.Get<T>(key) : default;
    }
    public void RemoveFromRemoteCache(string key)
    {
        GetResource<IRemoteCacheProvider>()?.Remove(key);
    }

    // ==========================================
    // Local Lock
    // ==========================================
    public bool TryLocalLock(string key, long timeoutMillis, long expireMillis)
    {
        var deadline = DateTimeOffset.UtcNow.AddMilliseconds(Math.Max(timeoutMillis, 0));
        lock (LocalLockGate)
        {
            while (true)
            {
                var now = DateTimeOffset.UtcNow;
                if (!LocalLocks.TryGetValue(key, out var current)
                    || current.ExpiresAt is not null && now >= current.ExpiresAt
                    || ReferenceEquals(current.Owner, this))
                {
                    var expiresAt = expireMillis > 0 ? now.AddMilliseconds(expireMillis) : (DateTimeOffset?)null;
                    LocalLocks[key] = new LocalLockEntry(this, expiresAt);
                    return true;
                }
                var remaining = deadline - now;
                if (timeoutMillis <= 0 || remaining <= TimeSpan.Zero) return false;
                var leaseRemaining = current.ExpiresAt is null ? remaining : current.ExpiresAt.Value - now;
                Monitor.Wait(LocalLockGate, leaseRemaining < remaining ? leaseRemaining : remaining);
            }
        }
    }
    public void UnlockLocal(string key)
    {
        lock (LocalLockGate)
        {
            if (LocalLocks.TryGetValue(key, out var current) && ReferenceEquals(current.Owner, this))
            {
                LocalLocks.Remove(key);
                Monitor.PulseAll(LocalLockGate);
            }
        }
    }

    // ==========================================
    // Remote Lock
    // ==========================================
    public bool TryRemoteLock(string key, long timeoutMillis, long expireMillis)
    {
        var provider = GetResource<IRemoteLockProvider>();
        return provider != null ? provider.TryLock(key, timeoutMillis, expireMillis) : true;
    }
    public void UnlockRemote(string key)
    {
        GetResource<IRemoteLockProvider>()?.Unlock(key);
    }
}
