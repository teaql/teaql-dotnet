using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public class UserContext
{
    public IMetadataStore? Metadata { get; set; }
    public IEntityRegistry? EntityRegistry { get; set; }
    
    private readonly ConcurrentDictionary<Type, object> _typedResources = new();
    private readonly ConcurrentDictionary<string, object> _namedResources = new();
    private readonly ConcurrentDictionary<string, Value> _locals = new();
    private IRawAuditEventSink? _standardAuditSink;
    private IAppAuditEventSink? _appAuditSink;
    
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

    public UserContext WithModule(RuntimeModule module)
    {
        module.ApplyTo(this);
        return this;
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

    internal void SetStandardAuditEventSink(IRawAuditEventSink sink) => _standardAuditSink = sink;

    public UserContext WithAppAuditEventSink(IAppAuditEventSink sink)
    {
        _appAuditSink = sink;
        return this;
    }

    public IDataService Audit(IDataService dataService) => new AuditedDataService(dataService, this);

    internal async Task EmitMutationAuditAsync(MutationRequest request, MutationResult result)
    {
        var raw = request switch
        {
            InsertMutationRequest insert => new RawAuditEvent(MutationAuditKind.Created, insert.Command.Entity,
                result.GeneratedValues.TryGetValue("id", out var generatedId) ? generatedId : insert.Command.Values.GetValueOrDefault("id"),
                insert.Command.Values.Select(pair => new AuditFieldChange(pair.Key, null, pair.Value)).ToList(), insert.Command.TraceChain),
            UpdateMutationRequest update => new RawAuditEvent(MutationAuditKind.Updated, update.Command.Entity, update.Command.Id,
                update.Command.Values.Select(pair => new AuditFieldChange(pair.Key, update.Command.OldValues?.GetValueOrDefault(pair.Key), pair.Value)).ToList(), update.Command.TraceChain),
            DeleteMutationRequest delete => new RawAuditEvent(MutationAuditKind.Deleted, delete.Command.Entity, delete.Command.Id,
                Array.Empty<AuditFieldChange>(), delete.Command.TraceChain),
            RecoverMutationRequest recover => new RawAuditEvent(MutationAuditKind.Recovered, recover.Command.Entity, recover.Command.Id,
                Array.Empty<AuditFieldChange>(), recover.Command.TraceChain),
            _ => throw new InvalidOperationException("Unsupported mutation audit request")
        };
        if (_standardAuditSink is not null) await _standardAuditSink.OnEventAsync(this, raw);
        if (_appAuditSink is not null) await _appAuditSink.OnSafeEventAsync(this, BuildSafeEvent(raw));
    }

    private SafeAuditEvent BuildSafeEvent(RawAuditEvent raw)
    {
        var descriptor = GetEntity(raw.Entity);
        var mask = descriptor?.AuditMaskFieldList ?? new List<string>();
        var maxLength = descriptor?.AuditValueMaxLenValue;
        var fields = raw.Changes.Select(change =>
        {
            var value = ValueText(change.NewValue);
            var masked = mask.Contains(change.Field);
            if (value is not null && masked) value = Mask(value);
            var truncated = value is not null && maxLength is not null && value.Length > maxLength.Value;
            if (truncated)
            {
                var limit = maxLength!.Value;
                value = limit <= 3 ? new string('*', limit) : value![..(limit - 3)] + "...";
            }
            return new SafeAuditField(change.Field, value, masked, truncated);
        }).ToList();
        return new SafeAuditEvent(raw.Kind, raw.Entity, ValueText(raw.EntityId), fields, raw.TraceChain);
    }

    private static string? ValueText(Value? value) => value switch
    {
        null or Value.NullValue => null,
        Value.TextValue text => text.Value,
        Value.I64Value number => number.Value.ToString(),
        Value.U64Value number => number.Value.ToString(),
        _ => value.ToString()
    };

    private static string Mask(string value) => value.Length < 8
        ? new string('*', value.Length)
        : value[..2] + new string('*', value.Length - 4) + value[^2..];

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
    public void PutToLocalCache(string key, object value, int? timeToLiveInSeconds = null) { }
    public T? GetFromLocalCache<T>(string key) { return default; }
    public void RemoveFromLocalCache(string key) { }

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
    public bool TryLocalLock(string key, long timeoutMillis, long expireMillis) { return true; }
    public void UnlockLocal(string key) { }

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
