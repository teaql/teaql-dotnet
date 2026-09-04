using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public class UserContext
{
    private static readonly IIdSetStore DefaultIdSetStore = new InMemoryIdSetStore();
    private const string ActiveRootResource = "teaql.activeRoot";
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
    private readonly ConcurrentDictionary<string, IEntityChecker> _checkers = new();
    private readonly SemaphoreSlim _graphSaveGate = new(1, 1);
    private sealed class GraphSaveSession { }
    private readonly AsyncLocal<GraphSaveSession?> _ambientGraphSave = new();
    private GraphSaveSession? _activeGraphSave;
    private List<Action> _graphCommitActions = new();
    private List<Action> _graphRollbackActions = new();
    private DateTimeOffset? _graphFixTime;
    private List<FixEvidence> _currentFixEvidence = new();
    private IReadOnlyList<FixEvidence> _lastFixEvidence = Array.Empty<FixEvidence>();
    public IReadOnlyList<FixEvidence> LastFixEvidence => _lastFixEvidence;

    public void RecordFixEvidence(FixEvidence evidence)
    {
        var normalized = (evidence.SourceLabel ?? "").ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(evidence.EntityType) || string.IsNullOrWhiteSpace(evidence.ModelPath)
            || string.IsNullOrWhiteSpace(evidence.SourceLabel) || normalized.Contains("authorization")
            || normalized.Contains("cookie") || normalized.Contains("token="))
            throw new ArgumentException("Fix evidence must contain only safe framework provenance labels");
        _currentFixEvidence.Add(evidence);
    }
    
    public string TraceId { get; set; }
    public string? UserIdentifier { get; set; }
    public string? Timezone { get; set; } = "UTC";
    
    public IServiceProvider? ServiceProvider { get; set; }
    public IRuntimeTelemetry RuntimeTelemetry { get; private set; } = NoopRuntimeTelemetry.Instance;
    public IDiagnosticSqlLogSink? DiagnosticSqlLogSink { get; private set; } = new TextDiagnosticSqlLogSink();
    public bool QuerySqlLogEnabled { get; private set; } = true;
    public bool MutationSqlLogEnabled { get; private set; } = true;
    public TeaQLLocale Locale { get; private set; } = TeaQLLocale.English;
    public I18nCatalog I18nCatalog { get; private set; } = I18nCatalog.Builtin;
    public IIdSetStore IdSetStore { get; private set; } = DefaultIdSetStore;
    public string IdSetPlan { get; private set; } = "ID_SET_DISABLED";
    public ulong IdSetCount { get; private set; }
    public string IdSetCountAccuracy { get; private set; } = "UNKNOWN";

    public UserContext SetLocaleCode(string code) { var locale = TeaQLLocales.Parse(code); Locale = locale; return this; }
    public UserContext SetLanguageCode(string code) => SetLocaleCode(code);
    public UserContext InstallI18nCatalog(I18nCatalog catalog) { I18nCatalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); return this; }
    public IReadOnlyList<CheckResult> TranslateCheckResults(IReadOnlyList<CheckResult> results) { foreach(var result in results) I18nCatalog.Translate(result, Locale); return results; }

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

    public UserContext WithRuntimeTelemetry(IRuntimeTelemetry telemetry)
    {
        RuntimeTelemetry = telemetry ?? NoopRuntimeTelemetry.Instance;
        return this;
    }

    public UserContext WithDiagnosticSqlLogSink(IDiagnosticSqlLogSink? sink)
    {
        DiagnosticSqlLogSink = sink;
        return this;
    }

    public UserContext EnableQuerySqlLog(bool enabled = true) { QuerySqlLogEnabled = enabled; return this; }
    public UserContext EnableMutationSqlLog(bool enabled = true) { MutationSqlLogEnabled = enabled; return this; }
    public UserContext DisableQuerySqlLog() => EnableQuerySqlLog(false);
    public UserContext DisableMutationSqlLog() => EnableMutationSqlLog(false);

    internal void RecordExecutionMetadata(ExecutionMetadata? metadata)
    {
        if (metadata == null) return;
        var query = metadata.Operation == DataServiceOperation.Query;
        if ((query && !QuerySqlLogEnabled) || (!query && !MutationSqlLogEnabled)) return;
        DiagnosticSqlLogSink?.Write(metadata);
    }

    public UserContext WithDataService(IDataService provider)
    {
        InsertResource<IDataService>(new RuntimeDataService(provider, this));
        if (provider is ITransactionExecutor transactionExecutor)
            InsertResource<ITransactionExecutor>(transactionExecutor);
        if (provider is ISchemaExecutor schemaExecutor)
            InsertResource<ISchemaExecutor>(schemaExecutor);
        return this;
    }

    public async Task<T> ExecuteGraphSaveAsync<T>(Func<Task<T>> work)
    {
        if (_ambientGraphSave.Value != null && ReferenceEquals(_ambientGraphSave.Value, _activeGraphSave))
            return await work().ConfigureAwait(false);
        await _graphSaveGate.WaitAsync().ConfigureAwait(false);
        var original = RequireResource<IDataService>();
        ITransaction? transaction = null;
        var session = new GraphSaveSession();
        try
        {
            transaction = await RequireResource<ITransactionExecutor>()
                .BeginTransactionAsync().ConfigureAwait(false);
            _activeGraphSave = session;
            _ambientGraphSave.Value = session;
            _graphFixTime = DateTimeOffset.UtcNow;
            _currentFixEvidence = new List<FixEvidence>();
            _graphCommitActions = new List<Action>();
            _graphRollbackActions = new List<Action>();
            InsertResource<IDataService>(new RuntimeDataService(transaction, this));
            T result;
            try
            {
                result = await work().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch
            {
                try { await transaction.RollbackAsync().ConfigureAwait(false); }
                finally
                {
                    for (var index = _graphRollbackActions.Count - 1; index >= 0; index--)
                        _graphRollbackActions[index]();
                }
                throw;
            }
            foreach (var action in _graphCommitActions) action();
            return result;
        }
        finally
        {
            _lastFixEvidence = _currentFixEvidence.AsReadOnly();
            InsertResource<IDataService>(original);
            _ambientGraphSave.Value = null;
            _activeGraphSave = null;
            _graphFixTime = null;
            _graphCommitActions = new List<Action>();
            _graphRollbackActions = new List<Action>();
            transaction?.Dispose();
            _graphSaveGate.Release();
        }
    }

    public void AfterGraphCommit(Action action)
    {
        if (_ambientGraphSave.Value == null || !ReferenceEquals(_ambientGraphSave.Value, _activeGraphSave))
            throw new InvalidOperationException("No graph save is active");
        _graphCommitActions.Add(action);
    }

    public void AfterGraphRollback(Action action)
    {
        if (_ambientGraphSave.Value == null || !ReferenceEquals(_ambientGraphSave.Value, _activeGraphSave))
            throw new InvalidOperationException("No graph save is active");
        _graphRollbackActions.Add(action);
    }

    public UserContext WithIdSetStore(IIdSetStore store)
    {
        IdSetStore = store ?? throw new ArgumentNullException(nameof(store));
        return this;
    }

    internal void ObserveIdSet(string plan, string accuracy = "UNKNOWN", ulong count = 0)
    {
        IdSetPlan = plan;
        IdSetCountAccuracy = accuracy;
        IdSetCount = count;
    }

    internal string IdSetSecurityScope()
    {
        _namedResources.TryGetValue("trustedTenant", out var tenant);
        _namedResources.TryGetValue(ActiveRootResource, out var root);
        _typedResources.TryGetValue(typeof(IRequestPolicy), out var policy);
        var policyIdentity = policy == null
            ? "none"
            : $"{policy.GetType().FullName}:{System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(policy)}";
        return $"{UserIdentifier}|{tenant}|{root}|{policyIdentity}";
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

    public UserContext WithActiveRoot(string entityType, long id)
    {
        if (string.IsNullOrWhiteSpace(entityType)) throw new ArgumentException("Active root entity type is required", nameof(entityType));
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Active root id must be positive");
        InsertNamedResource(ActiveRootResource, new ContextEntityRef(entityType, id));
        return this;
    }

    public ContextEntityRef RequireActiveRoot(string expectedEntityType)
    {
        if (!_namedResources.TryGetValue(ActiveRootResource, out var value) || value is not ContextEntityRef root)
            throw new ContextRootException(expectedEntityType, null, "Active root is missing from UserContext");
        if (!string.Equals(root.EntityType, expectedEntityType, StringComparison.Ordinal))
            throw new ContextRootException(expectedEntityType, root, $"Active root type is {root.EntityType}, expected {expectedEntityType}");
        return root;
    }

    /// <summary>
    /// Generator integration scope for generated bootstrap mutations. It supplies the stable audit
    /// actor/category and active root, then restores the caller's context exactly on disposal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IDisposable EnterGeneratedBootstrap(string rootEntityType, long rootId)
    {
        var previousUser = UserIdentifier;
        var hadRoot = _namedResources.TryGetValue(ActiveRootResource, out var previousRoot);
        var hadCategory = _namedResources.TryGetValue("bootstrapCategory", out var previousCategory);
        UserIdentifier = "teaql-generated-bootstrap";
        WithActiveRoot(rootEntityType, rootId);
        InsertNamedResource("bootstrapCategory", "runtime-bootstrap");
        return new DelegateDisposable(() =>
        {
            UserIdentifier = previousUser;
            RestoreNamedResource(ActiveRootResource, hadRoot, previousRoot);
            RestoreNamedResource("bootstrapCategory", hadCategory, previousCategory);
        });
    }

    private void RestoreNamedResource(string name, bool existed, object? value)
    {
        if (existed && value != null) _namedResources[name] = value;
        else _namedResources.TryRemove(name, out _);
    }

    private sealed class DelegateDisposable(Action dispose) : IDisposable
    {
        private Action? _dispose = dispose;
        public void Dispose() => Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }

    public SelectQuery ApplyRequestPolicy(SelectQuery query) =>
        RequireResource<IRequestPolicy>().Apply(query);

    public Task PublishAppAuditEventAsync(IReadOnlyDictionary<string, object?> safeEvent,
        CancellationToken cancellationToken = default) => PublishAppAuditEventAsync(
            safeEvent.TryGetValue("entityType", out var entity) ? entity?.ToString() ?? "unknown" : "unknown",
            safeEvent.TryGetValue("mutationKind", out var kind) ? kind?.ToString() ?? "unknown" : "unknown",
            safeEvent.TryGetValue("changedFieldCount", out var count) && count is IConvertible convertible
                ? convertible.ToInt32(System.Globalization.CultureInfo.InvariantCulture) : 0,
            safeEvent, cancellationToken);

    public Task PublishAppAuditEventAsync(string entityType, string mutationKind,
        int changedFieldCount, IReadOnlyDictionary<string, object?> safeEvent,
        CancellationToken cancellationToken = default) => RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("audit", $"{entityType}.audit",
                new Dictionary<string, object>
                {
                    ["teaql.entity.type"] = entityType,
                    ["teaql.mutation.kind"] = mutationKind,
                    ["teaql.audit.changed_field_count"] = changedFieldCount
                }),
            async () =>
            {
                await RequireResource<IAppAuditEventSink>()
                    .RecordAsync(safeEvent, cancellationToken).ConfigureAwait(false);
                return true;
            });

    public async Task EnsureSchemaAsync()
    {
        var provider = RequireResource<ISchemaExecutor>();
        var metadata = Metadata ?? throw new InvalidOperationException("Missing metadata");
        foreach (var entity in metadata.GetAllEntities())
            await provider.EnsureSchemaAsync(new SchemaRequest { EntityName = entity.Name });
        if (_generatedBootstrap != null)
            await _generatedBootstrap(this).ConfigureAwait(false);
    }

    private Func<UserContext, Task>? _generatedBootstrap;

    internal void InstallGeneratedBootstrap(Func<UserContext, Task>? callback) =>
        _generatedBootstrap = callback;

    public UserContext WithModule(RuntimeModule module)
    {
        module.ApplyTo(this);
        return this;
    }

    /// <summary>Installs a passive runtime manifest. Schema changes remain explicit.</summary>
    public UserContext Install(RuntimeModule module) => WithModule(module);

    internal void InstallCheckers(IReadOnlyDictionary<string, IEntityChecker> checkers)
    {
        foreach (var checker in checkers) _checkers[checker.Key] = checker.Value;
    }

    internal void CheckAndFix(MutationRequest request)
    {
        if (request is BatchMutationRequest batch)
        {
            foreach (var item in batch.Requests) CheckAndFix(item);
            return;
        }
        var entity = request switch
        {
            InsertMutationRequest insert => insert.Command.Entity,
            UpdateMutationRequest update => update.Command.Entity,
            DeleteMutationRequest delete => delete.Command.Entity,
            RecoverMutationRequest recover => recover.Command.Entity,
            _ => ""
        };
        if (!_checkers.TryGetValue(entity, out var checker)) return;
        var violations = checker.CheckAndFix(this, request, _graphFixTime ?? DateTimeOffset.UtcNow).ToList();
        TranslateCheckResults(violations);
        if (violations.Count != 0) throw new CheckException(violations);
    }

    /// <summary>Generated graph infrastructure validates/fixes every node before its first provider mutation.</summary>
    public void PreflightMutation(MutationRequest request) => CheckAndFix(request);

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
        var scope = RuntimeTelemetry.StartSafely(RuntimeOperation.Create("cache", "local.put",
            new Dictionary<string, object> { ["teaql.cache.operation"] = "put" }));
        try
        {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        var expiresAt = timeToLiveInSeconds > 0
            ? DateTimeOffset.UtcNow.AddSeconds(timeToLiveInSeconds.Value)
            : (DateTimeOffset?)null;
        LocalCache[key] = new LocalCacheEntry(value, expiresAt);
            scope.Success(new Dictionary<string, object> { ["teaql.cache.result"] = "stored" });
        }
        catch (Exception error) { scope.Failure(error); throw; }
    }
    public T? GetFromLocalCache<T>(string key)
    {
        var scope = RuntimeTelemetry.StartSafely(RuntimeOperation.Create("cache", "local.get",
            new Dictionary<string, object> { ["teaql.cache.operation"] = "get" }));
        if (!LocalCache.TryGetValue(key, out var entry))
        {
            scope.Success(new Dictionary<string, object> { ["teaql.cache.result"] = "miss" });
            return default;
        }
        if (entry.ExpiresAt is not null && DateTimeOffset.UtcNow >= entry.ExpiresAt)
        {
            LocalCache.TryRemove(new KeyValuePair<string, LocalCacheEntry>(key, entry));
            scope.Success(new Dictionary<string, object> { ["teaql.cache.result"] = "miss" });
            return default;
        }
        var result = entry.Value is T value ? value : default;
        scope.Success(new Dictionary<string, object>
            { ["teaql.cache.result"] = result is null ? "miss" : "hit" });
        return result;
    }
    public void RemoveFromLocalCache(string key)
    {
        var scope = RuntimeTelemetry.StartSafely(RuntimeOperation.Create("cache", "local.remove",
            new Dictionary<string, object> { ["teaql.cache.operation"] = "remove" }));
        try
        {
            LocalCache.TryRemove(key, out _);
            scope.Success(new Dictionary<string, object> { ["teaql.cache.result"] = "removed" });
        }
        catch (Exception error) { scope.Failure(error); throw; }
    }

    // ==========================================
    // Remote Cache
    // ==========================================
    public void PutToRemoteCache(string key, object value, int? timeToLiveInSeconds = null)
    {
        var scope = RuntimeTelemetry.StartSafely(RuntimeOperation.Create("cache", "remote.put",
            new Dictionary<string, object> { ["teaql.cache.operation"] = "put" }));
        try
        {
            GetResource<IRemoteCacheProvider>()?.Put(key, value, timeToLiveInSeconds);
            scope.Success(new Dictionary<string, object> { ["teaql.cache.result"] = "stored" });
        }
        catch (Exception error) { scope.Failure(error); throw; }
    }
    public T? GetFromRemoteCache<T>(string key)
    {
        var scope = RuntimeTelemetry.StartSafely(RuntimeOperation.Create("cache", "remote.get",
            new Dictionary<string, object> { ["teaql.cache.operation"] = "get" }));
        try
        {
            var provider = GetResource<IRemoteCacheProvider>();
            var result = provider != null ? provider.Get<T>(key) : default;
            scope.Success(new Dictionary<string, object>
                { ["teaql.cache.result"] = result is null ? "miss" : "hit" });
            return result;
        }
        catch (Exception error) { scope.Failure(error); throw; }
    }
    public void RemoveFromRemoteCache(string key)
    {
        var scope = RuntimeTelemetry.StartSafely(RuntimeOperation.Create("cache", "remote.remove",
            new Dictionary<string, object> { ["teaql.cache.operation"] = "remove" }));
        try
        {
            GetResource<IRemoteCacheProvider>()?.Remove(key);
            scope.Success(new Dictionary<string, object> { ["teaql.cache.result"] = "removed" });
        }
        catch (Exception error) { scope.Failure(error); throw; }
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
