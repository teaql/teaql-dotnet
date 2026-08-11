using System;
using System.Collections.Generic;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public interface IMetadataStore
{
    EntityDescriptor? GetEntity(string name);
    IReadOnlyList<EntityDescriptor> GetAllEntities();
    void RecordMetadataLog(ExecutionMetadata metadata) { }
}

public interface IEntityRegistry
{
    bool Contains(string entity);
}

public interface ITeaqlRuntime
{
    UserContext UserContext { get; }
}

public interface IRemoteCacheProvider
{
    void Put(string key, object value, int? timeToLiveInSeconds = null);
    T? Get<T>(string key);
    void Remove(string key);
}

public interface IRemoteLockProvider
{
    bool TryLock(string key, long timeoutMillis, long expireMillis);
    void Unlock(string key);
}
