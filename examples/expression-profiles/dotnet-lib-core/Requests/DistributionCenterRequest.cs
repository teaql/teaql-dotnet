using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TeaQL.Core;
using TeaQL.Runtime;
using Generated.Models;

namespace Generated.Requests
{
    public class DistributionCenterRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;
        private bool _countRequested;
        private readonly List<DynamicSearchWarning> _dynamicSearchWarnings = new();
        private static object TeaqlQueryValue(object value) =>
            value is TimeSpan time
                ? time.ToString(@"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture)
                : value;

        public DistributionCenterRequest()
        {
            _query = new SelectQuery("DistributionCenter");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public DistributionCenterRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public DistributionCenterRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public DistributionCenterRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public DistributionCenterRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableDistributionCenterRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableDistributionCenterRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public DistributionCenterRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public DistributionCenterRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public DistributionCenterRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public DistributionCenterRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public DistributionCenterRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public DistributionCenterRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public DistributionCenterRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public DistributionCenterRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public DistributionCenterRequest SelectId()
                {
                    _query.Project(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public DistributionCenterRequest SelectDockName()
                {
                    _query.Project(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY);
                    return this;
                }

                public DistributionCenterRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }


                public DistributionCenterRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DistributionCenterRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DistributionCenterRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public DistributionCenterRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public DistributionCenterRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public DistributionCenterRequest WithDockNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public DistributionCenterRequest WithDockNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public DistributionCenterRequest WithDockNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithDockNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DistributionCenterRequest WithDockNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DistributionCenterRequest WithDockNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithDockNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithDockNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithDockNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithDockNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public DistributionCenterRequest WithDockNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY));
                    return this;
                }

                public DistributionCenterRequest WithDockNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY));
                    return this;
                }
                public DistributionCenterRequest WithDockNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public DistributionCenterRequest WithDockNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public DistributionCenterRequest WithDockNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public DistributionCenterRequest WithDockNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public DistributionCenterRequest WithDockNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public DistributionCenterRequest WithDockNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public DistributionCenterRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DistributionCenterRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DistributionCenterRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DistributionCenterRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public DistributionCenterRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public DistributionCenterRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public DistributionCenterRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public DistributionCenterRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }

                public DistributionCenterRequest OrderByDockNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public DistributionCenterRequest OrderByDockNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public DistributionCenterRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public DistributionCenterRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public DistributionCenterRequest Count()
        {
            return CountAs("count");
        }

        public DistributionCenterRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public DistributionCenterRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public DistributionCenterRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.DistributionCenter.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public DistributionCenterRequest GroupByDockName()
                {
                    _query.GroupBy(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY);
                    return this;
                }

                public DistributionCenterRequest GroupByDockNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.DistributionCenter.__TeaqlFields.DOCK_NAME_PROPERTY_QUERY);
                    return this;
                }
                public DistributionCenterRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public DistributionCenterRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.DistributionCenter.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public DistributionCenterRequest SelectDispatchOrderList()
                {
                    return SelectDispatchOrderListWith(new DispatchOrderRequest());
                }

                public DistributionCenterRequest SelectDispatchOrderListWith(DispatchOrderRequest child)
                {
                    _query.RelationQuery("DispatchOrderList", "DispatchOrder", "distribution_center", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectDispatchOrderListWith", Relation: "DispatchOrderList")
                    };
                    return this;
                }
                public DistributionCenterRequest HaveDispatchOrders()
                    => WithDispatchOrderListMatching(new DispatchOrderRequest());

                public DistributionCenterRequest HaveNoDispatchOrders()
                    => WithoutDispatchOrderListMatching(new DispatchOrderRequest());

                public DistributionCenterRequest WithDispatchOrderListMatching(DispatchOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("DispatchOrder")!, child.GetQuery(), "distribution_center"));
                    return this;
                }

                public DistributionCenterRequest WithoutDispatchOrderListMatching(DispatchOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("DispatchOrder")!, child.GetQuery(), "distribution_center"));
                    return this;
                }
                public DistributionCenterRequest CountDispatchOrders()
                    => CountDispatchOrdersAs("countDispatchOrders");

                public DistributionCenterRequest CountDispatchOrdersAs(string alias)
                {
                    CountDispatchOrdersWith(alias, new DispatchOrderRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountDispatchOrdersAs", alias, "DispatchOrderList")
                    };
                    return this;
                }

                public DistributionCenterRequest CountDispatchOrdersWith(string alias, DispatchOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("DispatchOrderList", "DispatchOrder", "distribution_center", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountDispatchOrdersWith", alias, "DispatchOrderList")
                    };
                    return this;
                }



        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var req = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            // The runtime owns authorized membership, recursive Facets and SQL provenance.
            return await service.QueryAsync(req);
        }

        private async Task<Generated.Models.DistributionCenter?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.DistributionCenter>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.DistributionCenter>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            if (limitOne) query.Limit(1);
            var request = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            if (request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Grouped projections require ExecuteForRowsAsync");
            SmartList<Record> result;
            if (_countRequested) result = await RuntimeDataService.FetchPageAsync(service, context, request);
            else {
                var rows = await service.QueryAsync(request);
                result = new SmartList<Record>(rows.Rows) { Facets = rows.Facets };
            }
            var alias = _countRequested ? request.Query.PageCountOperation?.Alias : null;
            var aggregateOnly = request.Query.AggregateItems.Any(item =>
                !(alias != null && item.Function == AggregateFunction.Count && item.Alias == alias));
            if (aggregateOnly) {
                if (result.Count > 0) foreach (var pair in result[0]) result.Aggregations[pair.Key] = pair.Value;
                result.Clear();
            }
            var entities = new SmartList<Generated.Models.DistributionCenter>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.DistributionCenter.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.DistributionCenter> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var request = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            if (request.Query.AggregateItems.Count > 0 || request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Entity streams require entity rows; use a record or aggregation query");
            return RuntimeDataService.StreamEntities(context, request,
                row => global::TeaQL.Runtime.EntityQueryProperties.Capture(
                    Generated.Models.DistributionCenter.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableDistributionCenterRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.DistributionCenter?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.DistributionCenter>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.DistributionCenter>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableDistributionCenterRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.DistributionCenter?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.DistributionCenter>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.DistributionCenter>> executeForStream,
            Action<string> comment,
            Action ensureIntent)
        {
            _executeForRows = executeForRows;
            _executeForOne = executeForOne;
            _executeForList = executeForList;
            _executeForStream = executeForStream;
            _comment = comment;
            _ensureIntent = ensureIntent;
        }

        public ExecutableDistributionCenterRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.DistributionCenter> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("DistributionCenter").ConfigureAwait(false));
            return context.InitializeEntity("DistributionCenter", Generated.Models.DistributionCenter.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.DistributionCenter>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.DistributionCenter> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.DistributionCenter?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}