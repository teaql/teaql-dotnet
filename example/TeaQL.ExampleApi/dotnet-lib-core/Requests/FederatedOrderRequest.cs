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
    public class FederatedOrderRequest
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

        public FederatedOrderRequest()
        {
            _query = new SelectQuery("FederatedOrder");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public FederatedOrderRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public FederatedOrderRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public FederatedOrderRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public FederatedOrderRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableFederatedOrderRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableFederatedOrderRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public FederatedOrderRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public FederatedOrderRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public FederatedOrderRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public FederatedOrderRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public FederatedOrderRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public FederatedOrderRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public FederatedOrderRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public FederatedOrderRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("merchant");
            _query.Project("order_number");
            _query.Project("fulfillment_status");
            _query.Project("version");
            return this;
        }

                public FederatedOrderRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public FederatedOrderRequest SelectOrderNumber()
                {
                    _query.Project("order_number");
                    return this;
                }

                public FederatedOrderRequest SelectFulfillmentStatus()
                {
                    _query.Project("fulfillment_status");
                    return this;
                }

                public FederatedOrderRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public FederatedOrderRequest SelectMerchant()
                {
                    return SelectMerchantWith(new MerchantRequest());
                }

                public FederatedOrderRequest SelectMerchantWith(MerchantRequest related)
                {
                    _query.Project("merchant");
                    _query.ForwardRelationQuery("Merchant", "Merchant", "merchant", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectMerchantWith", Relation: "Merchant")
                    };
                    return this;
                }
                public FederatedOrderRequest WithMerchantMatching(MerchantRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("merchant", GeneratedRuntimeModule.Module.Metadata.GetEntity("Merchant")!, related.GetQuery(), "id"));
                    return this;
                }

                public FederatedOrderRequest WithoutMerchantMatching(MerchantRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("merchant", GeneratedRuntimeModule.Module.Metadata.GetEntity("Merchant")!, related.GetQuery(), "id"));
                    return this;
                }

                public FederatedOrderRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FederatedOrderRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FederatedOrderRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FederatedOrderRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public FederatedOrderRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public FederatedOrderRequest FilterByMerchant(object val)
                {
                    _query.AndFilter(Expr.Eq("merchant", val));
                    return this;
                }

                public FederatedOrderRequest FilterByMerchantIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("merchant", vals));
                    return this;
                }

                public FederatedOrderRequest WithMerchantIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("merchant"));
                    return this;
                }

                public FederatedOrderRequest WithMerchantIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("merchant"));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("order_number", val));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberIs(string val)
                {
                    _query.AndFilter(Expr.Eq("order_number", TeaqlQueryValue(val)));
                    return this;
                }
                public FederatedOrderRequest WithOrderNumberIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("order_number", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("order_number", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("order_number", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("order_number"));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("order_number"));
                    return this;
                }
                public FederatedOrderRequest WithOrderNumberNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("order_number", val));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("order_number", val));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("order_number", val));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("order_number", val));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("order_number", val));
                    return this;
                }

                public FederatedOrderRequest WithOrderNumberSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("order_number", val));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("fulfillment_status", val));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusIs(string val)
                {
                    _query.AndFilter(Expr.Eq("fulfillment_status", TeaqlQueryValue(val)));
                    return this;
                }
                public FederatedOrderRequest WithFulfillmentStatusIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("fulfillment_status", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("fulfillment_status", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("fulfillment_status", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("fulfillment_status", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("fulfillment_status", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("fulfillment_status", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("fulfillment_status", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("fulfillment_status", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("fulfillment_status"));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("fulfillment_status"));
                    return this;
                }
                public FederatedOrderRequest WithFulfillmentStatusNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("fulfillment_status", val));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("fulfillment_status", val));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("fulfillment_status", val));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("fulfillment_status", val));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("fulfillment_status", val));
                    return this;
                }

                public FederatedOrderRequest WithFulfillmentStatusSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("fulfillment_status", val));
                    return this;
                }

                public FederatedOrderRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FederatedOrderRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FederatedOrderRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FederatedOrderRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FederatedOrderRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public FederatedOrderRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public FederatedOrderRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public FederatedOrderRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public FederatedOrderRequest OrderByOrderNumberAscending()
                {
                    _query.OrderBy("order_number", "asc");
                    return this;
                }

                public FederatedOrderRequest OrderByOrderNumberDescending()
                {
                    _query.OrderBy("order_number", "desc");
                    return this;
                }

                public FederatedOrderRequest OrderByFulfillmentStatusAscending()
                {
                    _query.OrderBy("fulfillment_status", "asc");
                    return this;
                }

                public FederatedOrderRequest OrderByFulfillmentStatusDescending()
                {
                    _query.OrderBy("fulfillment_status", "desc");
                    return this;
                }

                public FederatedOrderRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public FederatedOrderRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public FederatedOrderRequest Count()
        {
            return CountAs("count");
        }

        public FederatedOrderRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public FederatedOrderRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public FederatedOrderRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public FederatedOrderRequest GroupByMerchant()
                {
                    _query.GroupBy("merchant");
                    return this;
                }

                public FederatedOrderRequest GroupByMerchantAs(string retName)
                {
                    _query.GroupBy("merchant"); 
                    return this;
                }
                public FederatedOrderRequest GroupByOrderNumber()
                {
                    _query.GroupBy("order_number");
                    return this;
                }

                public FederatedOrderRequest GroupByOrderNumberAs(string retName)
                {
                    _query.GroupBy("order_number"); 
                    return this;
                }
                public FederatedOrderRequest GroupByFulfillmentStatus()
                {
                    _query.GroupBy("fulfillment_status");
                    return this;
                }

                public FederatedOrderRequest GroupByFulfillmentStatusAs(string retName)
                {
                    _query.GroupBy("fulfillment_status"); 
                    return this;
                }
                public FederatedOrderRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public FederatedOrderRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public FederatedOrderRequest FacetByMerchantAs(
                    string name, MerchantRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Merchant", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByMerchantAs", name, "merchant")
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

        private async Task<Generated.Models.FederatedOrder?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.FederatedOrder>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.FederatedOrder>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.FederatedOrder>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.FederatedOrder.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.FederatedOrder> ExecuteForStreamInternalAsync(
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
                    Generated.Models.FederatedOrder.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableFederatedOrderRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.FederatedOrder?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.FederatedOrder>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.FederatedOrder>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableFederatedOrderRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.FederatedOrder?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.FederatedOrder>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.FederatedOrder>> executeForStream,
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

        public ExecutableFederatedOrderRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.FederatedOrder> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("FederatedOrder").ConfigureAwait(false));
            return context.InitializeEntity("FederatedOrder", Generated.Models.FederatedOrder.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.FederatedOrder>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.FederatedOrder> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.FederatedOrder?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}