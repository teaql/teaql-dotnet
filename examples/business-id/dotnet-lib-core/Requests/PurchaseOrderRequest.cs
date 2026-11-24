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
    public class PurchaseOrderRequest
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

        public PurchaseOrderRequest()
        {
            _query = new SelectQuery("PurchaseOrder");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public PurchaseOrderRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public PurchaseOrderRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public PurchaseOrderRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public PurchaseOrderRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutablePurchaseOrderRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutablePurchaseOrderRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public PurchaseOrderRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public PurchaseOrderRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public PurchaseOrderRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public PurchaseOrderRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public PurchaseOrderRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public PurchaseOrderRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public PurchaseOrderRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public PurchaseOrderRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public PurchaseOrderRequest SelectId()
                {
                    _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public PurchaseOrderRequest SelectSupplier()
                {
                    _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest SelectOrderNumber()
                {
                    _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public PurchaseOrderRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, "Platform", global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public PurchaseOrderRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public PurchaseOrderRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public PurchaseOrderRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY, vals));
                    return this;
                }

                public PurchaseOrderRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public PurchaseOrderRequest WithSupplierIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY));
                    return this;
                }
                public PurchaseOrderRequest WithSupplierNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithSupplierSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithOrderNumberIs(OrderNumber val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, val.Value));
                    return this;
                }

                public PurchaseOrderRequest WithOrderNumberIsNot(OrderNumber val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, val.Value));
                    return this;
                }

                public PurchaseOrderRequest WithOrderNumberIn(params OrderNumber[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, vals.Select(val => (object)val.Value).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithOrderNumberNotIn(params OrderNumber[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, vals.Select(val => (object)val.Value).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public PurchaseOrderRequest OrderBySupplierAscending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderBySupplierDescending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY, "desc");
                    return this;
                }

                public PurchaseOrderRequest OrderByOrderNumberAscending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByOrderNumberDescending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, "desc");
                    return this;
                }

                public PurchaseOrderRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public PurchaseOrderRequest Count()
        {
            return CountAs("count");
        }

        public PurchaseOrderRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public PurchaseOrderRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public PurchaseOrderRequest GroupByPlatform()
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }
                public PurchaseOrderRequest GroupBySupplier()
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest GroupBySupplierAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.SUPPLIER_PROPERTY_QUERY);
                    return this;
                }
                public PurchaseOrderRequest GroupByOrderNumber()
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest GroupByOrderNumberAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY);
                    return this;
                }
                public PurchaseOrderRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public PurchaseOrderRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, global::Generated.Models.PurchaseOrder.__TeaqlFields.PLATFORM_PROPERTY_QUERY)
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

        private async Task<Generated.Models.PurchaseOrder?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.PurchaseOrder>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.PurchaseOrder>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.PurchaseOrder>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.PurchaseOrder.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.PurchaseOrder> ExecuteForStreamInternalAsync(
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
                    Generated.Models.PurchaseOrder.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutablePurchaseOrderRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.PurchaseOrder?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.PurchaseOrder>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.PurchaseOrder>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePurchaseOrderRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.PurchaseOrder?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.PurchaseOrder>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.PurchaseOrder>> executeForStream,
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

        public ExecutablePurchaseOrderRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.PurchaseOrder> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("PurchaseOrder").ConfigureAwait(false));
            return context.InitializeEntity("PurchaseOrder", Generated.Models.PurchaseOrder.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.PurchaseOrder>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.PurchaseOrder> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.PurchaseOrder?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}