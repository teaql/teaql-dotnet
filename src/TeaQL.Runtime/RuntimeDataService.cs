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
public sealed class RuntimeDataService : IDataService
{
    private readonly IDataService _provider;
    private readonly UserContext _context;

    public RuntimeDataService(IDataService provider, UserContext context)
    {
        _provider = provider;
        _context = context;
    }

    public DataServiceCapabilities Capabilities => _provider.Capabilities;

    public Task<QueryResult> QueryAsync(QueryRequest request)
    {
        request.RelationLoadObserver = new RuntimeRelationLoadObserver(_context.RuntimeTelemetry);
        return _context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("query", $"{request.Query.Entity}.list",
                new Dictionary<string, object> { ["teaql.entity.type"] = request.Query.Entity }),
            () => ExecuteQueryAsync(request),
            result => new Dictionary<string, object>
            {
                ["teaql.result.cardinality"] = result.Rows.Count
            });
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
                    idQuery.IdSetPagination = null;
                    var idResult = await ObserveProviderQueryAsync(CopyRequest(request, idQuery)).ConfigureAwait(false);
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

    private QueryRequest CopyRequest(QueryRequest source, SelectQuery query) => new()
    {
        Query = query,
        TraceChain = new List<TraceNode>(source.TraceChain),
        Comment = source.Comment,
        Purpose = source.Purpose,
        RelationLoadObserver = new RuntimeRelationLoadObserver(_context.RuntimeTelemetry)
    };

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

    public Task<MutationResult> MutateAsync(MutationRequest request)
    {
        _context.CheckAndFix(request);
        var entity = EntityName(request);
        return _context.RuntimeTelemetry.ObserveAsync(
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
            });
    }

    private async Task<QueryResult> ObserveProviderQueryAsync(QueryRequest request)
    {
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
        _context.RecordExecutionMetadata(result.Metadata);
        return result;
    }

    private async Task<MutationResult> ObserveProviderMutationAsync(MutationRequest request)
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
