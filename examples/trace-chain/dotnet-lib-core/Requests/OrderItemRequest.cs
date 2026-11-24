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
    public class OrderItemRequest
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

        public OrderItemRequest()
        {
            _query = new SelectQuery("OrderItem");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public OrderItemRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public OrderItemRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public OrderItemRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public OrderItemRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderItemRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderItemRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public OrderItemRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public OrderItemRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public OrderItemRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public OrderItemRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public OrderItemRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public OrderItemRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public OrderItemRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public OrderItemRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public OrderItemRequest SelectId()
                {
                    _query.Project(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public OrderItemRequest SelectName()
                {
                    _query.Project(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public OrderItemRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public OrderItemRequest SelectCustomerOrder()
                {
                    return SelectCustomerOrderWith(new CustomerOrderRequest());
                }

                public OrderItemRequest SelectCustomerOrderWith(CustomerOrderRequest related)
                {
                    _query.Project(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, "CustomerOrder", global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderWith", Relation: global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public OrderItemRequest WithCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderItemRequest WithoutCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderItemRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderItemRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderItemRequest FilterByCustomerOrder(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderItemRequest FilterByCustomerOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, vals));
                    return this;
                }

                public OrderItemRequest WithCustomerOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public OrderItemRequest WithCustomerOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public OrderItemRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderItemRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderItemRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }

                public OrderItemRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }
                public OrderItemRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderItemRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderItemRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderItemRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderItemRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderItemRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderItemRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public OrderItemRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public OrderItemRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderItemRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public OrderItemRequest OrderByNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderItemRequest OrderByNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderItemRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderItemRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public OrderItemRequest Count()
        {
            return CountAs("count");
        }

        public OrderItemRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public OrderItemRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public OrderItemRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderItem.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public OrderItemRequest GroupByCustomerOrder()
                {
                    _query.GroupBy(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public OrderItemRequest GroupByCustomerOrderAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }
                public OrderItemRequest GroupByName()
                {
                    _query.GroupBy(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public OrderItemRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderItem.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }
                public OrderItemRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public OrderItemRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public OrderItemRequest FacetByCustomerOrderAs(
                    string name, CustomerOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerOrderAs", name, global::Generated.Models.OrderItem.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY)
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

        private async Task<Generated.Models.OrderItem?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.OrderItem>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.OrderItem>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.OrderItem>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.OrderItem.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.OrderItem> ExecuteForStreamInternalAsync(
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
                    Generated.Models.OrderItem.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableOrderItemRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.OrderItem?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.OrderItem>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderItem>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableOrderItemRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.OrderItem?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.OrderItem>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderItem>> executeForStream,
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

        public ExecutableOrderItemRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.OrderItem> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("OrderItem").ConfigureAwait(false));
            return context.InitializeEntity("OrderItem", Generated.Models.OrderItem.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.OrderItem>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.OrderItem> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.OrderItem?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}