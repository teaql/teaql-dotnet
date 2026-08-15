using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

public interface IRequestPolicy
{
    SelectQuery Apply(SelectQuery query);
}

public interface IAppAuditEventSink
{
    Task RecordAsync(IReadOnlyDictionary<string, object?> safeEvent,
        CancellationToken cancellationToken = default);
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
