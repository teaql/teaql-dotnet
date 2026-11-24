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
    public class ShipmentRequest
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

        public ShipmentRequest()
        {
            _query = new SelectQuery("Shipment");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public ShipmentRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public ShipmentRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public ShipmentRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public ShipmentRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableShipmentRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableShipmentRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public ShipmentRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public ShipmentRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public ShipmentRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public ShipmentRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public ShipmentRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public ShipmentRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public ShipmentRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public ShipmentRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public ShipmentRequest SelectId()
                {
                    _query.Project(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public ShipmentRequest SelectReferenceCode()
                {
                    _query.Project(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
                    return this;
                }

                public ShipmentRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public ShipmentRequest SelectCustomerOrder()
                {
                    return SelectCustomerOrderWith(new CustomerOrderRequest());
                }

                public ShipmentRequest SelectCustomerOrderWith(CustomerOrderRequest related)
                {
                    _query.Project(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, "CustomerOrder", global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderWith", Relation: global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public ShipmentRequest WithCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public ShipmentRequest WithoutCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public ShipmentRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShipmentRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public ShipmentRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public ShipmentRequest FilterByCustomerOrder(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, val));
                    return this;
                }

                public ShipmentRequest FilterByCustomerOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, vals));
                    return this;
                }

                public ShipmentRequest WithCustomerOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public ShipmentRequest WithCustomerOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public ShipmentRequest WithReferenceCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY));
                    return this;
                }
                public ShipmentRequest WithReferenceCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public ShipmentRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShipmentRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public ShipmentRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public ShipmentRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public ShipmentRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public ShipmentRequest OrderByReferenceCodeAscending()
                {
                    _query.OrderBy(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public ShipmentRequest OrderByReferenceCodeDescending()
                {
                    _query.OrderBy(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public ShipmentRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public ShipmentRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public ShipmentRequest Count()
        {
            return CountAs("count");
        }

        public ShipmentRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public ShipmentRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public ShipmentRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Shipment.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public ShipmentRequest GroupByCustomerOrder()
                {
                    _query.GroupBy(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public ShipmentRequest GroupByCustomerOrderAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }
                public ShipmentRequest GroupByReferenceCode()
                {
                    _query.GroupBy(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
                    return this;
                }

                public ShipmentRequest GroupByReferenceCodeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Shipment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
                    return this;
                }
                public ShipmentRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public ShipmentRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Shipment.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public ShipmentRequest FacetByCustomerOrderAs(
                    string name, CustomerOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerOrderAs", name, global::Generated.Models.Shipment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY)
                    });
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

        private async Task<Generated.Models.Shipment?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.Shipment>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.Shipment>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.Shipment>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Shipment.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Shipment> ExecuteForStreamInternalAsync(
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
                    Generated.Models.Shipment.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableShipmentRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Shipment?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Shipment>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Shipment>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableShipmentRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Shipment?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Shipment>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Shipment>> executeForStream,
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

        public ExecutableShipmentRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.Shipment> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Shipment").ConfigureAwait(false));
            return context.InitializeEntity("Shipment", Generated.Models.Shipment.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Shipment>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Shipment> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Shipment?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}