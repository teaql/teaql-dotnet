using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class ShipmentRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;
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
                ExecuteForPageInternalAsync,
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
            _query.Project("id");
            _query.Project("customer_order");
            _query.Project("reference_code");
            _query.Project("version");
            return this;
        }

                public ShipmentRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public ShipmentRequest SelectReferenceCode()
                {
                    _query.Project("reference_code");
                    return this;
                }

                public ShipmentRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public ShipmentRequest SelectCustomerOrder()
                {
                    return SelectCustomerOrderWith(new CustomerOrderRequest());
                }

                public ShipmentRequest SelectCustomerOrderWith(CustomerOrderRequest related)
                {
                    _query.Project("customer_order");
                    _query.ForwardRelationQuery("CustomerOrder", "CustomerOrder", "customer_order", related.GetQuery());
                    return this;
                }
                public ShipmentRequest WithCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("customer_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public ShipmentRequest WithoutCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("customer_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public ShipmentRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShipmentRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public ShipmentRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public ShipmentRequest FilterByCustomerOrder(object val)
                {
                    _query.AndFilter(Expr.Eq("customer_order", val));
                    return this;
                }

                public ShipmentRequest FilterByCustomerOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("customer_order", vals));
                    return this;
                }

                public ShipmentRequest WithCustomerOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("customer_order"));
                    return this;
                }

                public ShipmentRequest WithCustomerOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("customer_order"));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("reference_code", val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq("reference_code", TeaqlQueryValue(val)));
                    return this;
                }
                public ShipmentRequest WithReferenceCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("reference_code", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("reference_code", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("reference_code", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("reference_code"));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("reference_code"));
                    return this;
                }
                public ShipmentRequest WithReferenceCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("reference_code", val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("reference_code", val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("reference_code", val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("reference_code", val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("reference_code", val));
                    return this;
                }

                public ShipmentRequest WithReferenceCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("reference_code", val));
                    return this;
                }

                public ShipmentRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShipmentRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShipmentRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShipmentRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public ShipmentRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public ShipmentRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public ShipmentRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public ShipmentRequest OrderByReferenceCodeAscending()
                {
                    _query.OrderBy("reference_code", "asc");
                    return this;
                }

                public ShipmentRequest OrderByReferenceCodeDescending()
                {
                    _query.OrderBy("reference_code", "desc");
                    return this;
                }

                public ShipmentRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public ShipmentRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public ShipmentRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public ShipmentRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public ShipmentRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public ShipmentRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public ShipmentRequest GroupByCustomerOrder()
                {
                    _query.GroupBy("customer_order");
                    return this;
                }

                public ShipmentRequest GroupByCustomerOrderAs(string retName)
                {
                    _query.GroupBy("customer_order"); 
                    return this;
                }
                public ShipmentRequest GroupByReferenceCode()
                {
                    _query.GroupBy("reference_code");
                    return this;
                }

                public ShipmentRequest GroupByReferenceCodeAs(string retName)
                {
                    _query.GroupBy("reference_code"); 
                    return this;
                }
                public ShipmentRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public ShipmentRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public ShipmentRequest FacetByCustomerOrderAs(
                    string name, CustomerOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "customer_order", request.GetQuery(), includeAllFacets));
                    return this;
                }


        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var req = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            var authorized = req.Query;
            var result = await service.QueryAsync(req);
            foreach (var facet in authorized.Facets)
            {
                var membership = authorized.CloneForExecution();
                membership.Facets.Clear();
                membership.Relations.Clear();
                membership.Orders.Clear();
                membership.Aggregates.Clear();
                membership.GroupFields.Clear();
                membership.Projections.Clear();
                membership.Project(facet.RelationName);
                var membershipRows = (await service.QueryAsync(req.WithQuery(membership))).Rows;
                var counts = membershipRows
                    .Where(row => row.TryGetValue(facet.RelationName, out var value) && value.Raw != null)
                    .GroupBy(row => Convert.ToString(row[facet.RelationName].Raw)!)
                    .ToDictionary(group => group.Key, group => group.Count());

                var nested = facet.Query.CloneForExecution();
                nested.Facets.Clear();
                var countAliases = nested.Aggregates
                    .Where(aggregate => aggregate.Function == AggregateFunction.Count)
                    .Select(aggregate => aggregate.Alias).ToArray();
                nested.Aggregates.Clear();
                nested.GroupFields.Clear();
                var facetRows = (await service.QueryAsync(req.WithQuery(nested))).Rows;
                var decorated = new SmartList<Record>();
                foreach (var row in facetRows)
                {
                    var key = row.TryGetValue("id", out var id) ? Convert.ToString(id.Raw) : null;
                    var count = key != null && counts.TryGetValue(key, out var value) ? value : 0;
                    if (!facet.IncludeAllFacets && count == 0) continue;
                    foreach (var alias in countAliases.Length == 0 ? new[] { "count" } : countAliases)
                        row[alias] = new Value.I64Value(count);
                    decorated.Add(row);
                }
                result.Facets[facet.Name] = decorated;
            }
            return result;
        }

        private async Task<QueryResult> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            return await service.QueryAsync(req);
        }

        private async Task<ShipmentPage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Offset(offset);
            query.Limit(limit);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var authorized = req.Query;
            var result = await service.QueryAsync(req);
            long totalCount;
            if (authorized.IdSetPagination != null && context.IdSetCountAccuracy == "EXACT")
            {
                totalCount = checked((long)context.IdSetCount);
            }
            else
            {
                var countQuery = authorized.CloneForExecution();
                countQuery.Projection.Clear();
                countQuery.ExprProjection.Clear();
                countQuery.RelationLoads.Clear();
                countQuery.RelationAggregates.Clear();
                countQuery.OrderByItems.Clear();
                countQuery.GroupByItems.Clear();
                countQuery.AggregateItems.Clear();
                countQuery.Slice = null;
                countQuery.IdSetPagination = null;
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(req.WithQuery(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.Shipment>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.Shipment.FromRecord(row, queryRoot));
            return new ShipmentPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.Shipment> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var req = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            await foreach (var chunk in streaming.QueryStreamAsync(
                req, chunkSize, cancellationToken).WithCancellation(cancellationToken))
            {
                var queryRoot = new EntityRoot();
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.Shipment.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableShipmentRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<QueryResult>> _executeForOne;
        private readonly Func<UserContext, int, int, Task<ShipmentPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Shipment>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableShipmentRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<QueryResult>> executeForOne,
            Func<UserContext, int, int, Task<ShipmentPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Shipment>> executeForStream,
            Action<string> comment,
            Action ensureIntent)
        {
            _executeForRows = executeForRows;
            _executeForOne = executeForOne;
            _executeForPage = executeForPage;
            _executeForStream = executeForStream;
            _comment = comment;
            _ensureIntent = ensureIntent;
        }

        public ExecutableShipmentRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.Shipment NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.Shipment();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.Shipment>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.Shipment>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.Shipment.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<ShipmentPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.Shipment> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.Shipment?> ExecuteForOneAsync(
            UserContext context)
        {
            var result = await _executeForOne(context);
            if (result.Rows.Count == 0) return null;
            return Generated.Models.Shipment.FromRecord(
                result.Rows[0], new EntityRoot());
        }
    }

    public sealed class ShipmentPage
    {
        public SmartList<Generated.Models.Shipment> Rows { get; }
        public long TotalCount { get; }
        public ShipmentPage(
            SmartList<Generated.Models.Shipment> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}