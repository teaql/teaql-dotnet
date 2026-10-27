using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

/// <summary>
/// Adds TeaQL semantic operation spans around a provider data service. Provider
/// internals remain independently instrumentable and become children through
/// the active OpenTelemetry context.
/// </summary>
public sealed class RuntimeDataService : IStreamQueryExecutor
{
    private readonly IDataService _provider;
    private readonly UserContext _context;
    private readonly GraphMutationSession? _graph;

    public RuntimeDataService(IDataService provider, UserContext context)
    {
        _provider = provider;
        _context = context;
    }

    internal RuntimeDataService(IDataService provider, UserContext context, GraphMutationSession graph)
        : this(provider, context) => _graph = graph;

    public DataServiceCapabilities Capabilities => _provider.Capabilities;

    /// <summary>Execute one authorized page snapshot. Generated libraries bind
    /// native operation metadata and hydrate rows; this runtime owns counting.</summary>
    public static Task<SmartList<Record>> FetchPageAsync(IDataService service, UserContext context, QueryRequest request) =>
        context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("query", $"{request.Query.Entity}.list",
                new Dictionary<string, object> { ["teaql.entity.type"] = request.Query.Entity }),
            () => FetchCountedListCoreAsync(service, context, request),
            result => new Dictionary<string, object> { ["teaql.result.cardinality"] = result.Count });

    private static async Task<SmartList<Record>> FetchCountedListCoreAsync(IDataService service, UserContext context, QueryRequest request)
    {
        // The explicit count and list are branches of one generated invocation.
        // Dispatch only these owned branches beneath that root, rather than
        // suppressing arbitrary nested/concurrent queries via ambient state.
        Task<QueryResult> QueryBranchAsync(QueryRequest branch) =>
            service is RuntimeDataService runtime && ReferenceEquals(runtime._context, context)
                ? runtime.ExecuteOwnedQueryAsync(branch) : service.QueryAsync(branch);
        var source = request.Query.CloneForExecution();
        var operation = source.PageCountOperation;
        var alias = operation?.Alias ?? RequestConstants.COUNT_ALIAS;
        if (operation != null)
            source.AggregateItems.RemoveAll(item => item.Function == AggregateFunction.Count && item.Alias == alias);
        source.PrepareForList();
        async Task<long> CountAsync()
        {
            var matchingEntities = source.CloneForExecution();
            matchingEntities.AggregateItems.Clear();
            var count = matchingEntities.ForExactCount();
            count.AggregateItems.Clear(); count.CountField("id", alias);
            var scope = operation == null ? request.Scope.Helper(source.Entity, "count matching rows")
                : request.Scope.DeriveOperation(operation);
            var result = await QueryBranchAsync(request.DeriveAt(count, scope));
            if (result.Rows.Count != 1 || !result.Rows[0].TryGetValue(alias, out var value))
                throw new InvalidOperationException("Exact entity count did not return one integer result");
            var exact = value switch {
                Value.I64Value(var number) when number >= 0 => (long?)number,
                Value.U64Value(var number) when number <= long.MaxValue => (long)number,
                Value.DecimalValue(var number) when number >= 0 && number <= long.MaxValue && decimal.Truncate(number) == number => (long)number,
                _ => null
            };
            if (exact is not long total)
                throw new InvalidOperationException("Exact entity count must be a non-negative integer without rounding");
            return total;
        }
        QueryResult rows;
        long total;
        if (source.IdSetPagination != null)
        {
            rows = await QueryBranchAsync(request.WithQuery(source));
            total = context.IdSetCountAccuracy == "EXACT" ? checked((long)context.IdSetCount) : await CountAsync();
        }
        else
        {
            total = await CountAsync();
            rows = await QueryBranchAsync(request.WithQuery(source));
        }
        var page = new SmartList<Record>(rows.Rows) { TotalCount = total, Facets = rows.Facets };
        if (operation?.Alias != null) page.Aggregations[alias] = new Value.I64Value(total);
        return page;
    }

    public IAsyncEnumerable<StreamChunk> QueryStreamAsync(QueryRequest request, int chunkSize,
        System.Threading.CancellationToken cancellationToken = default)
        => RuntimeStream.Chunks(_context.RuntimeTelemetry, request.WithQuery(request.Query),
            (captured, token) => QueryStreamCoreAsync(captured, chunkSize, token), cancellationToken);

    private IAsyncEnumerable<StreamChunk> QueryStreamCoreAsync(QueryRequest request, int chunkSize,
        System.Threading.CancellationToken cancellationToken)
    {
        if (_provider is not IStreamQueryExecutor streaming)
            throw new NotSupportedException("The installed provider does not support streaming queries");
        var execution = CopyRequest(request, request.Query.CloneForExecution());
        execution.DiagnosticObserver = _context.RecordExecutionMetadata;
        execution.StatementObserver = _context.RuntimeTelemetry is IRuntimeSqlTelemetry telemetry ? telemetry.StartStatement : null;
        return streaming.QueryStreamAsync(execution, chunkSize, cancellationToken);
    }

    /// <summary>Generated-library bridge: capture a typed stream now, execute
    /// lazily, own one logical root and acknowledge each delivered entity.</summary>
    public static IAsyncEnumerable<T> StreamEntities<T>(UserContext context, QueryRequest request,
        Func<Record, T> materialize, int chunkSize, System.Threading.CancellationToken cancellationToken = default)
    {
        if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
        var service = context.RequireResource<IDataService>();
        if (service is not IStreamQueryExecutor streaming)
            throw new NotSupportedException("The configured data service does not provide a local streaming cursor");
        var captured = request.WithQuery(request.Query);
        return RuntimeStream.Entities(context.RuntimeTelemetry, captured, materialize,
            (execution, token) => service is RuntimeDataService runtime && ReferenceEquals(runtime._context, context)
                ? runtime.QueryStreamCoreAsync(execution, chunkSize, token) : streaming.QueryStreamAsync(execution, chunkSize, token),
            cancellationToken);
    }

    public Task<QueryResult> QueryAsync(QueryRequest request)
    {
        return _context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("query", $"{request.Query.Entity}.list",
                new Dictionary<string, object> { ["teaql.entity.type"] = request.Query.Entity }),
            () => ExecuteOwnedQueryAsync(request),
            result => new Dictionary<string, object>
            {
                ["teaql.result.cardinality"] = result.Rows.Count
            });
    }

    private Task<QueryResult> ExecuteOwnedQueryAsync(QueryRequest request)
    {
        request.RelationLoadObserver = new RuntimeRelationLoadObserver(_context.RuntimeTelemetry);
        return ExecuteQueryAsync(request);
    }

    private async Task<QueryResult> ExecuteQueryAsync(QueryRequest request)
    {
        var source = request.Query.CloneForExecution().PrepareForList();
        if (source.IdSetPagination == null)
        {
            _context.ObserveIdSet("ID_SET_DISABLED");
            return await ObserveProviderQueryAsync(CopyRequest(request, source)).ConfigureAwait(false);
        }
        if (source.Slice?.Limit is not ulong limit || limit == 0 || source.PartitionBy != null ||
            source.AggregateItems.Count != 0 || source.GroupByItems.Count != 0 || source.RawSqlText != null)
        {
            _context.ObserveIdSet("ID_SET_FALLBACK_UNSUPPORTED_SHAPE");
            return await ObserveProviderQueryAsync(CopyRequest(request, source)).ConfigureAwait(false);
        }
        if (source.OrderByItems.Any(order => order.ExprValue != null || string.IsNullOrEmpty(order.Field)))
        {
            _context.ObserveIdSet("ID_SET_FALLBACK_NON_DETERMINISTIC_ORDER");
            return await ObserveProviderQueryAsync(CopyRequest(request, source)).ConfigureAwait(false);
        }

        var stable = source.CloneForExecution();
        if (stable.OrderByItems.All(order => order.Field != "id")) stable.OrderAsc("id");
        var options = source.IdSetPagination;
        var queryKey = IdSetQueryKey(stable, options.Namespace);
        RetainedIdSet? retained;
        try { retained = await _context.IdSetStore.GetAsync(queryKey).ConfigureAwait(false); }
        catch
        {
            _context.ObserveIdSet("ID_SET_FALLBACK_STORE_UNAVAILABLE");
            return await ObserveProviderQueryAsync(CopyRequest(request, source)).ConfigureAwait(false);
        }
        var plan = "ID_SET_HIT";
        if (retained == null)
        {
            using (await IdSetBuildLocks.AcquireAsync(queryKey).ConfigureAwait(false))
            {
                try { retained = await _context.IdSetStore.GetAsync(queryKey).ConfigureAwait(false); }
                catch
                {
                    _context.ObserveIdSet("ID_SET_FALLBACK_STORE_UNAVAILABLE");
                    return await ObserveProviderQueryAsync(CopyRequest(request, source)).ConfigureAwait(false);
                }
                if (retained == null)
                {
                    var idQuery = stable.CloneForExecution();
                    idQuery.Projection = new List<string> { "id" };
                    idQuery.ExprProjection.Clear();
                    idQuery.RelationLoads.Clear();
                    idQuery.RelationAggregates.Clear();
                    idQuery.ChildEnhancements.Clear();
                    idQuery.Slice = new Slice(options.MaxIds == ulong.MaxValue ? ulong.MaxValue : options.MaxIds + 1, 0);
                    idQuery.HardLimit(options.MaxIds == ulong.MaxValue ? ulong.MaxValue : options.MaxIds + 1);
                    idQuery.IdSetPagination = null;
                    var helper = request.DeriveAt(idQuery, request.Scope.Helper(idQuery.Entity,
                        $"collect {idQuery.Entity} IDs for pagination"));
                    var idResult = await ObserveProviderQueryAsync(helper).ConfigureAwait(false);
                    var ids = new List<ulong>(idResult.Rows.Count);
                    foreach (var row in idResult.Rows)
                    {
                        if (!row.TryGetValue("id", out var value) || value.TryU64() is not ulong id)
                        {
                            _context.ObserveIdSet("ID_SET_FALLBACK_UNSUPPORTED_SHAPE");
                            return await ObserveProviderQueryAsync(CopyRequest(request, source)).ConfigureAwait(false);
                        }
                        ids.Add(id);
                    }
                    if ((ulong)ids.Count > options.MaxIds)
                    {
                        _context.ObserveIdSet("ID_SET_FALLBACK_LIMIT_EXCEEDED", "LOWER_BOUND", (ulong)ids.Count);
                        return await ObserveProviderQueryAsync(CopyRequest(request, source)).ConfigureAwait(false);
                    }
                    retained = new RetainedIdSet(queryKey, ids.ToArray(), DateTimeOffset.UtcNow.AddSeconds(options.TtlSeconds));
                    try { await _context.IdSetStore.PutAsync(retained).ConfigureAwait(false); }
                    catch
                    {
                        _context.ObserveIdSet("ID_SET_FALLBACK_STORE_UNAVAILABLE");
                        return await ObserveProviderQueryAsync(CopyRequest(request, source)).ConfigureAwait(false);
                    }
                    plan = "ID_SET_BUILD";
                }
            }
        }

        _context.ObserveIdSet(plan, "EXACT", (ulong)retained.Ids.LongLength);
        var offset = source.Slice!.Offset;
        if (offset >= (ulong)retained.Ids.LongLength) return new QueryResult();
        var count = (int)Math.Min(limit, (ulong)retained.Ids.LongLength - offset);
        var pageIds = retained.Ids.AsSpan((int)offset, count).ToArray();
        var page = source.CloneForExecution();
        page.Slice = null;
        page.IdSetPagination = null;
        page.AndFilter(Expr.InList("id", pageIds.Select(id => (Value)new Value.U64Value(id))));
        var result = await ObserveProviderQueryAsync(CopyRequest(request, page)).ConfigureAwait(false);
        var byId = result.Rows.Where(row => row.TryGetValue("id", out var value) && value.TryU64().HasValue)
            .ToDictionary(row => row["id"].TryU64()!.Value);
        result.Rows = pageIds.Where(byId.ContainsKey).Select(id => byId[id]).ToList();
        return result;
    }

    private QueryRequest CopyRequest(QueryRequest source, SelectQuery query)
    {
        var copy = source.WithQuery(query);
        copy.RelationLoadObserver = new RuntimeRelationLoadObserver(_context.RuntimeTelemetry);
        return copy;
    }

    private string IdSetQueryKey(SelectQuery source, string namespaceName)
    {
        var normalized = source.CloneForExecution();
        normalized.Slice = null;
        normalized.Projection.Clear();
        normalized.ExprProjection.Clear();
        normalized.RelationLoads.Clear();
        normalized.RelationAggregates.Clear();
        normalized.CommentText = null;
        normalized.TraceChain.Clear();
        normalized.IdSetPagination = null;
        var identity = string.Join("|", namespaceName, _context.IdSetSecurityScope(),
            _provider.GetType().FullName,
            System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(_provider),
            normalized.Entity, normalized.SearchText, normalized.FilterCondition?.ToString(),
            string.Join(",", normalized.OrderByItems.Select(order => order.ToString())),
            string.Join(",", normalized.RawSqlSearchCriteriaItems));
        return "teaql:id-set:v1:" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity))).ToLowerInvariant();
    }

    public async Task<MutationResult> MutateAsync(MutationRequest request)
    {
        try
        {
        _context.ValidateGraphMutation(request, _graph);
        _context.CheckAndFix(request);
        request.InheritedIntentValues = request.InheritedIntentValues
            .Concat(LoadedPrivateValues(request, _context)).ToArray();
        using var mutationGovernance = _context.EnterMutationPolicyExecution(request);
        var entity = EntityName(request);
        var result = await _context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("mutation", $"{entity}.mutate",
                new Dictionary<string, object>
                {
                    ["teaql.entity.type"] = entity,
                    ["teaql.mutation.kind"] = MutationKind(request)
                }),
            () => ObserveProviderMutationAsync(request),
            result => new Dictionary<string, object>
            {
                ["teaql.result.cardinality"] = result.AffectedRows
            }).ConfigureAwait(false);
        await PublishMutationAuditAsync(request, result).ConfigureAwait(false);
        return result;
        }
        catch
        {
            if (_context.GetEntity(EntityName(request))?.HasBlobFields() == true)
                _graph?.Poison();
            throw;
        }
    }

    private async Task PublishMutationAuditAsync(MutationRequest request, MutationResult result)
    {
        var deliveries = new List<Func<Task>>();
        CollectMutationAudits(request, result, deliveries);
        if (_graph != null)
        {
            foreach (var publish in deliveries) _graph.AfterCommit(publish);
            return;
        }
        // Project EVERY leaf before the first callback can mutate caller-owned
        // commands/ledgers, including leaves in later nested batch containers.
        var failures = new List<Exception>();
        foreach (var publish in deliveries)
            try { await publish().ConfigureAwait(false); }
            catch (Exception error) { failures.Add(error); }
        // Provider execution already succeeded. Delivery failure cannot justify
        // re-running writes or skipping later committed item notifications.
        if (failures.Count == 1) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failures[0]).Throw();
        if (failures.Count > 1) throw new AggregateException("Batch audit delivery failed", failures);
    }

    private void CollectMutationAudits(MutationRequest request, MutationResult result, List<Func<Task>> deliveries)
    {
        if (request is BatchMutationRequest && result.BatchItems is { } items)
        {
            // These are the actual execution requests, with captured root intent
            // and sibling privacy provenance. Nested groups recurse once in
            // request order; each successful leaf retains its own persisted row.
            foreach (var item in items)
                CollectMutationAudits(item.Request, item.Result, deliveries);
            return;
        }
        if (result.AffectedRows == 0 || _context.GetResource<IAppAuditEventSink>() == null) return;
        var entity = EntityName(request); var mutationKind = MutationKind(request);
        var changedFields = ChangedFields(request).OrderBy(field => field, StringComparer.Ordinal).ToArray();
        var safeEvent = new Dictionary<string, object?>
        {
            ["actor"] = _context.UserIdentifier,
            ["category"] = _context.GetNamedResource<string>("bootstrapCategory") ?? "mutation",
            ["entityType"] = entity,
            ["entityId"] = EntityId(request, result),
            ["mutationKind"] = mutationKind,
            ["changedFields"] = changedFields,
            ["changedFieldCount"] = changedFields.Length,
            ["resultVersion"] = PersistedValue(result, "version"),
            ["affectedRows"] = result.AffectedRows
        };
        var values = PrivateMutationValues(request, _context).Concat(request.InheritedIntentValues).ToArray();
        safeEvent["traceChain"] = request.AuditLineage(EntityName(request))
            .Select(node => node with { Comment = "", Detail = LogPrivacy.ScrubAuditText(node.Detail, values) ?? "" })
            .ToArray();
        safeEvent["reason"] = LogPrivacy.ScrubAuditText(request.Comment, values);
        if (_context.CurrentMutationGovernance is { } governance)
            safeEvent["mutationGovernance"] = GovernanceEvidence(governance);
        deliveries.Add(() => _context.PublishAppAuditEventAsync(
            entity, mutationKind, changedFields.Length, safeEvent));
    }

    internal static IEnumerable<Value> LoadedPrivateValues(MutationRequest request, UserContext context)
    {
        if (request is BatchMutationRequest batch)
            return batch.Requests.SelectMany(item => LoadedPrivateValues(item, context));
        if (request.LoadedSnapshot == null) return [];
        return PrivateFieldValues(request.LoadedSnapshot.CopyValues(), EntityName(request), context);
    }

    private static IEnumerable<Value> PrivateFieldValues(IEnumerable<KeyValuePair<string, Value>> fields,
        string entity, UserContext context)
    {
        var descriptor = context.GetEntity(entity);
        return fields.Where(pair => {
            var property = descriptor?.PropertyByName(pair.Key);
            return property == null || descriptor?.HasExplicitSqlLogPolicyMetadata != true
                || LogPrivacy.HasCredentials(pair.Value.ToJsonValue())
                || SensitiveLogNames.IsCredential(property.Name)
                || SensitiveLogNames.IsCredential(property.ColumnNameString)
                || descriptor.AuditMaskFieldList.Contains(property.Name);
        }).Select(pair => pair.Value).ToArray();
    }

    internal static IEnumerable<Value> PrivateMutationValues(MutationRequest request, UserContext context)
    {
        if (request is BatchMutationRequest batch)
            return batch.Requests.SelectMany(item => PrivateMutationValues(item, context));
        IEnumerable<KeyValuePair<string, Value>> fields = request switch
        {
            InsertMutationRequest insert => insert.Command.Values,
            UpdateMutationRequest update => update.Command.Values.Concat(update.Command.Guards)
                .Concat(update.Command.OldValues ?? new Record())
                .Append(new("id", update.Command.Id)),
            DeleteMutationRequest delete => delete.Command.Guards.Append(new("id", delete.Command.Id)),
            RecoverMutationRequest recover => recover.Command.Guards.Append(new("id", recover.Command.Id)),
            _ => []
        };
        return PrivateFieldValues(fields, EntityName(request), context);
    }

    private static IReadOnlyDictionary<string, object?> GovernanceEvidence(
        MutationGovernanceSnapshot governance) => new Dictionary<string, object?>
    {
        ["executionId"] = governance.ExecutionId,
        ["requestKey"] = governance.RequestKey,
        ["source"] = governance.Source.ToString(),
        ["policyId"] = governance.Policy?.Id,
        ["policyVersion"] = governance.Policy?.Version,
        ["policyFingerprint"] = governance.Policy?.Fingerprint,
        ["approvalStatus"] = governance.ApprovalStatus.ToString(),
        ["warningCodes"] = governance.WarningCodes.ToArray(),
        ["operationCount"] = governance.Operations.Count
    };

    private static IEnumerable<string> ChangedFields(MutationRequest request) => request switch
    {
        InsertMutationRequest insert => insert.Command.Values.Keys,
        UpdateMutationRequest update => update.Command.Values.Keys,
        DeleteMutationRequest => ["version"],
        RecoverMutationRequest => ["version"],
        BatchMutationRequest batch => batch.Requests.SelectMany(ChangedFields).Distinct(StringComparer.Ordinal),
        _ => []
    };

    private static object? EntityId(MutationRequest request, MutationResult result) =>
        PersistedValue(result, "id") ?? request switch
        {
            InsertMutationRequest insert when insert.Command.Values.TryGetValue("id", out var id) => SafeValue(id),
            UpdateMutationRequest update => SafeValue(update.Command.Id),
            DeleteMutationRequest delete => SafeValue(delete.Command.Id),
            RecoverMutationRequest recover => SafeValue(recover.Command.Id),
            _ => null
        };

    private static object? PersistedValue(MutationResult result, string field) =>
        result.PersistedRecord != null && result.PersistedRecord.TryGetValue(field, out var value)
            ? SafeValue(value) : null;

    private static object? SafeValue(Value value) => value switch
    {
        Value.BoolValue(var item) => item,
        Value.I64Value(var item) => item,
        Value.U64Value(var item) => item,
        Value.F64Value(var item) => item,
        Value.DecimalValue(var item) => item,
        Value.TextValue(var item) => item,
        Value.DateValue(var item) => item,
        Value.TimestampValue(var item) => item,
        Value.NullValue or Value.TypedNullValue => null,
        _ => null
    };

    private async Task<QueryResult> ObserveProviderQueryAsync(QueryRequest request)
    {
        request.DiagnosticObserver = _context.RecordExecutionMetadata;
        request.StatementObserver = _context.RuntimeTelemetry is IRuntimeSqlTelemetry telemetry ? telemetry.StartStatement : null;
        var result = await _context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("provider", "data-service.query",
                new Dictionary<string, object>
                {
                    ["teaql.provider.kind"] = _provider.GetType().Name,
                    ["teaql.provider.operation"] = "query"
                }),
            () => _provider.QueryAsync(request),
            result => new Dictionary<string, object>
            {
                ["teaql.result.cardinality"] = result.Rows.Count
            }).ConfigureAwait(false);
        if (!result.Metadata.DiagnosticReported) _context.RecordExecutionMetadata(result.Metadata);
        return result;
    }

    private async Task<MutationResult> ObserveProviderMutationAsync(MutationRequest request)
    {
        var previous = request.DiagnosticObserver;
        var previousStatement = request.StatementObserver;
        var previousAllocator = request.InternalIdAllocator;
        request.DiagnosticObserver = _context.RecordExecutionMetadata;
        request.StatementObserver = _context.RuntimeTelemetry is IRuntimeSqlTelemetry telemetry ? telemetry.StartStatement : null;
        request.InternalIdAllocator = _context.InternalIdAllocator;
        try
        {
        var result = await _context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("provider", "data-service.mutate",
                new Dictionary<string, object>
                {
                    ["teaql.provider.kind"] = _provider.GetType().Name,
                    ["teaql.provider.operation"] = MutationKind(request)
                }),
            () => _provider.MutateAsync(request),
            result => new Dictionary<string, object>
            {
                ["teaql.result.cardinality"] = result.AffectedRows
            }).ConfigureAwait(false);
        _context.RecordExecutionMetadata(result.Metadata);
        return result;
        }
        finally
        {
            request.DiagnosticObserver = previous;
            request.StatementObserver = previousStatement;
            request.InternalIdAllocator = previousAllocator;
        }
    }

    private static string EntityName(MutationRequest request) => request switch
    {
        InsertMutationRequest insert => insert.Command.Entity,
        UpdateMutationRequest update => update.Command.Entity,
        DeleteMutationRequest delete => delete.Command.Entity,
        RecoverMutationRequest recover => recover.Command.Entity,
        BatchMutationRequest => "batch",
        _ => "unknown"
    };

    private static string MutationKind(MutationRequest request) => request switch
    {
        InsertMutationRequest => "create",
        UpdateMutationRequest => "update",
        DeleteMutationRequest => "delete",
        RecoverMutationRequest => "recover",
        BatchMutationRequest => "batch",
        _ => "unknown"
    };

    private sealed class RuntimeRelationLoadObserver(IRuntimeTelemetry telemetry)
        : IRelationLoadObserver
    {
        public async Task ObserveAsync(string entity, string relation,
            IReadOnlyDictionary<string, object> attributes, Func<Task> body)
        {
            var operationAttributes = new Dictionary<string, object>(attributes)
            {
                ["teaql.entity.type"] = entity,
                ["teaql.relation.name"] = relation
            };
            await telemetry.ObserveAsync(
                RuntimeOperation.Create("relation_load", $"{entity}.{relation}",
                    operationAttributes),
                async () => { await body().ConfigureAwait(false); return true; })
                .ConfigureAwait(false);
        }
    }
}
