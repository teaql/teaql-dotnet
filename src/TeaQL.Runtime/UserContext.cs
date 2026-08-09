using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using TeaQL.Core;

namespace TeaQL.Runtime;

public class UserContext
{
    public IMetadataStore? Metadata { get; set; }
    public IEntityRegistry? EntityRegistry { get; set; }
    
    private readonly ConcurrentDictionary<Type, object> _typedResources = new();
    private readonly ConcurrentDictionary<string, object> _namedResources = new();
    private readonly ConcurrentDictionary<string, Value> _locals = new();
    
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
}
