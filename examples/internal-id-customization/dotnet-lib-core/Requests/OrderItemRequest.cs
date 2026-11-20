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
            _query.Project("id");
            _query.Project("purchase_order");
            _query.Project("sku");
            _query.Project("quantity");
            _query.Project("version");
            return this;
        }

                public OrderItemRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public OrderItemRequest SelectSku()
                {
                    _query.Project("sku");
                    return this;
                }

                public OrderItemRequest SelectQuantity()
                {
                    _query.Project("quantity");
                    return this;
                }

                public OrderItemRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public OrderItemRequest SelectPurchaseOrder()
                {
                    return SelectPurchaseOrderWith(new PurchaseOrderRequest());
                }

                public OrderItemRequest SelectPurchaseOrderWith(PurchaseOrderRequest related)
                {
                    _query.Project("purchase_order");
                    _query.ForwardRelationQuery("PurchaseOrder", "PurchaseOrder", "purchase_order", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPurchaseOrderWith", Relation: "PurchaseOrder")
                    };
                    return this;
                }
                public OrderItemRequest WithPurchaseOrderMatching(PurchaseOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("purchase_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("PurchaseOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderItemRequest WithoutPurchaseOrderMatching(PurchaseOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("purchase_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("PurchaseOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderItemRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public OrderItemRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public OrderItemRequest FilterByPurchaseOrder(object val)
                {
                    _query.AndFilter(Expr.Eq("purchase_order", val));
                    return this;
                }

                public OrderItemRequest FilterByPurchaseOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("purchase_order", vals));
                    return this;
                }

                public OrderItemRequest WithPurchaseOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("purchase_order"));
                    return this;
                }

                public OrderItemRequest WithPurchaseOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("purchase_order"));
                    return this;
                }

                public OrderItemRequest WithSkuContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("sku", val));
                    return this;
                }

                public OrderItemRequest WithSkuIs(string val)
                {
                    _query.AndFilter(Expr.Eq("sku", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderItemRequest WithSkuIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithSkuIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("sku", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithSkuNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("sku", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithSkuGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithSkuGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithSkuLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithSkuLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithSkuBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("sku", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithSkuIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("sku"));
                    return this;
                }

                public OrderItemRequest WithSkuIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("sku"));
                    return this;
                }
                public OrderItemRequest WithSkuNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("sku", val));
                    return this;
                }

                public OrderItemRequest WithSkuStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("sku", val));
                    return this;
                }

                public OrderItemRequest WithSkuNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("sku", val));
                    return this;
                }

                public OrderItemRequest WithSkuEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("sku", val));
                    return this;
                }

                public OrderItemRequest WithSkuNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("sku", val));
                    return this;
                }

                public OrderItemRequest WithSkuSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("sku", val));
                    return this;
                }

                public OrderItemRequest WithQuantityIs(object val)
                {
                    _query.AndFilter(Expr.Eq("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithQuantityIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithQuantityIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("quantity", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithQuantityNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("quantity", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithQuantityGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithQuantityGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithQuantityLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithQuantityLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithQuantityBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("quantity", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithQuantityIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("quantity"));
                    return this;
                }

                public OrderItemRequest WithQuantityIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("quantity"));
                    return this;
                }

                public OrderItemRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderItemRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public OrderItemRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public OrderItemRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public OrderItemRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public OrderItemRequest OrderBySkuAscending()
                {
                    _query.OrderBy("sku", "asc");
                    return this;
                }

                public OrderItemRequest OrderBySkuDescending()
                {
                    _query.OrderBy("sku", "desc");
                    return this;
                }

                public OrderItemRequest OrderByQuantityAscending()
                {
                    _query.OrderBy("quantity", "asc");
                    return this;
                }

                public OrderItemRequest OrderByQuantityDescending()
                {
                    _query.OrderBy("quantity", "desc");
                    return this;
                }

                public OrderItemRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public OrderItemRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
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

                public OrderItemRequest MinQuantity()
                {
                    return MinQuantityAs("minOfQuantity");
                }

                public OrderItemRequest MinQuantityAs(string retName)
                {
                    _query.Aggregate("min", "quantity", retName);
                    return this;
                }
                public OrderItemRequest MaxQuantity()
                {
                    return MaxQuantityAs("maxOfQuantity");
                }

                public OrderItemRequest MaxQuantityAs(string retName)
                {
                    _query.Aggregate("max", "quantity", retName);
                    return this;
                }
                public OrderItemRequest SumQuantity()
                {
                    return SumQuantityAs("sumOfQuantity");
                }

                public OrderItemRequest SumQuantityAs(string retName)
                {
                    _query.Aggregate("sum", "quantity", retName);
                    return this;
                }
                public OrderItemRequest AvgQuantity()
                {
                    return AvgQuantityAs("avgOfQuantity");
                }

                public OrderItemRequest AvgQuantityAs(string retName)
                {
                    _query.Aggregate("avg", "quantity", retName);
                    return this;
                }
                public OrderItemRequest StandardDeviationQuantity()
                {
                    return StandardDeviationQuantityAs("standardDeviationOfQuantity");
                }

                public OrderItemRequest StandardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev", "quantity", retName);
                    return this;
                }
                public OrderItemRequest SquareRootOfPopulationStandardDeviationQuantity()
                {
                    return SquareRootOfPopulationStandardDeviationQuantityAs("squareRootOfPopulationStandardDeviationOfQuantity");
                }

                public OrderItemRequest SquareRootOfPopulationStandardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "quantity", retName);
                    return this;
                }
                public OrderItemRequest SampleVarianceQuantity()
                {
                    return SampleVarianceQuantityAs("sampleVarianceOfQuantity");
                }

                public OrderItemRequest SampleVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_samp", "quantity", retName);
                    return this;
                }
                public OrderItemRequest SamplePopulationVarianceQuantity()
                {
                    return SamplePopulationVarianceQuantityAs("samplePopulationVarianceOfQuantity");
                }

                public OrderItemRequest SamplePopulationVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_pop", "quantity", retName);
                    return this;
                }
                public OrderItemRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public OrderItemRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public OrderItemRequest GroupByPurchaseOrder()
                {
                    _query.GroupBy("purchase_order");
                    return this;
                }

                public OrderItemRequest GroupByPurchaseOrderAs(string retName)
                {
                    _query.GroupBy("purchase_order"); 
                    return this;
                }
                public OrderItemRequest GroupBySku()
                {
                    _query.GroupBy("sku");
                    return this;
                }

                public OrderItemRequest GroupBySkuAs(string retName)
                {
                    _query.GroupBy("sku"); 
                    return this;
                }
                public OrderItemRequest GroupByQuantity()
                {
                    _query.GroupBy("quantity");
                    return this;
                }

                public OrderItemRequest GroupByQuantityAs(string retName)
                {
                    _query.GroupBy("quantity"); 
                    return this;
                }
                public OrderItemRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public OrderItemRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public OrderItemRequest FacetByPurchaseOrderAs(
                    string name, PurchaseOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "PurchaseOrder", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPurchaseOrderAs", name, "purchase_order")
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