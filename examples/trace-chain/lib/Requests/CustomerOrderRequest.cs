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
    public class CustomerOrderRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;
        private static object TeaqlQueryValue(object value) =>
            value is TimeSpan time
                ? time.ToString(@"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture)
                : value;

        public CustomerOrderRequest()
        {
            _query = new SelectQuery("CustomerOrder");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public CustomerOrderRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public CustomerOrderRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public CustomerOrderRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCustomerOrderRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerOrderRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public CustomerOrderRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public CustomerOrderRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public CustomerOrderRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public CustomerOrderRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public CustomerOrderRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public CustomerOrderRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public CustomerOrderRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public CustomerOrderRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("platform");
            _query.Project("order_number");
            _query.Project("description");
            _query.Project("version");
            return this;
        }

                public CustomerOrderRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public CustomerOrderRequest SelectOrderNumber()
                {
                    _query.Project("order_number");
                    return this;
                }

                public CustomerOrderRequest SelectDescription()
                {
                    _query.Project("description");
                    return this;
                }

                public CustomerOrderRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public CustomerOrderRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public CustomerOrderRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project("platform");
                    _query.ForwardRelationQuery("Platform", "Platform", "platform", related.GetQuery());
                    return this;
                }
                public CustomerOrderRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public CustomerOrderRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public CustomerOrderRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("platform", val));
                    return this;
                }

                public CustomerOrderRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("platform", vals));
                    return this;
                }

                public CustomerOrderRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("platform"));
                    return this;
                }

                public CustomerOrderRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("platform"));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIs(string val)
                {
                    _query.AndFilter(Expr.Eq("order_number", TeaqlQueryValue(val)));
                    return this;
                }
                public CustomerOrderRequest WithOrderNumberIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("order_number", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("order_number", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("order_number", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("order_number", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("order_number"));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("order_number"));
                    return this;
                }
                public CustomerOrderRequest WithOrderNumberNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("description", val));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionIs(string val)
                {
                    _query.AndFilter(Expr.Eq("description", TeaqlQueryValue(val)));
                    return this;
                }
                public CustomerOrderRequest WithDescriptionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("description", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("description", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("description", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("description", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("description", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("description", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("description", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("description", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("description"));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("description"));
                    return this;
                }
                public CustomerOrderRequest WithDescriptionNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("description", val));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("description", val));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("description", val));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("description", val));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("description", val));
                    return this;
                }

                public CustomerOrderRequest WithDescriptionSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("description", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public CustomerOrderRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public CustomerOrderRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public CustomerOrderRequest OrderByOrderNumberAscending()
                {
                    _query.OrderBy("order_number", "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByOrderNumberDescending()
                {
                    _query.OrderBy("order_number", "desc");
                    return this;
                }

                public CustomerOrderRequest OrderByDescriptionAscending()
                {
                    _query.OrderBy("description", "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByDescriptionDescending()
                {
                    _query.OrderBy("description", "desc");
                    return this;
                }

                public CustomerOrderRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public CustomerOrderRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public CustomerOrderRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public CustomerOrderRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public CustomerOrderRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public CustomerOrderRequest GroupByPlatform()
                {
                    _query.GroupBy("platform");
                    return this;
                }

                public CustomerOrderRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy("platform"); 
                    return this;
                }
                public CustomerOrderRequest GroupByOrderNumber()
                {
                    _query.GroupBy("order_number");
                    return this;
                }

                public CustomerOrderRequest GroupByOrderNumberAs(string retName)
                {
                    _query.GroupBy("order_number"); 
                    return this;
                }
                public CustomerOrderRequest GroupByDescription()
                {
                    _query.GroupBy("description");
                    return this;
                }

                public CustomerOrderRequest GroupByDescriptionAs(string retName)
                {
                    _query.GroupBy("description"); 
                    return this;
                }
                public CustomerOrderRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public CustomerOrderRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public CustomerOrderRequest SelectOrderItemList()
                {
                    return SelectOrderItemListWith(new OrderItemRequest());
                }

                public CustomerOrderRequest SelectOrderItemListWith(OrderItemRequest child)
                {
                    _query.RelationQuery("OrderItemList", "OrderItem", "customer_order", true, child.GetQuery());
                    return this;
                }
                public CustomerOrderRequest SelectPaymentList()
                {
                    return SelectPaymentListWith(new PaymentRequest());
                }

                public CustomerOrderRequest SelectPaymentListWith(PaymentRequest child)
                {
                    _query.RelationQuery("PaymentList", "Payment", "customer_order", true, child.GetQuery());
                    return this;
                }
                public CustomerOrderRequest SelectShipmentList()
                {
                    return SelectShipmentListWith(new ShipmentRequest());
                }

                public CustomerOrderRequest SelectShipmentListWith(ShipmentRequest child)
                {
                    _query.RelationQuery("ShipmentList", "Shipment", "customer_order", true, child.GetQuery());
                    return this;
                }
                public CustomerOrderRequest HaveOrderItems()
                    => WithOrderItemListMatching(new OrderItemRequest());

                public CustomerOrderRequest HaveNoOrderItems()
                    => WithoutOrderItemListMatching(new OrderItemRequest());

                public CustomerOrderRequest WithOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "customer_order"));
                    return this;
                }

                public CustomerOrderRequest WithoutOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "customer_order"));
                    return this;
                }
                public CustomerOrderRequest HavePayments()
                    => WithPaymentListMatching(new PaymentRequest());

                public CustomerOrderRequest HaveNoPayments()
                    => WithoutPaymentListMatching(new PaymentRequest());

                public CustomerOrderRequest WithPaymentListMatching(PaymentRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Payment")!, child.GetQuery(), "customer_order"));
                    return this;
                }

                public CustomerOrderRequest WithoutPaymentListMatching(PaymentRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Payment")!, child.GetQuery(), "customer_order"));
                    return this;
                }
                public CustomerOrderRequest HaveShipments()
                    => WithShipmentListMatching(new ShipmentRequest());

                public CustomerOrderRequest HaveNoShipments()
                    => WithoutShipmentListMatching(new ShipmentRequest());

                public CustomerOrderRequest WithShipmentListMatching(ShipmentRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Shipment")!, child.GetQuery(), "customer_order"));
                    return this;
                }

                public CustomerOrderRequest WithoutShipmentListMatching(ShipmentRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Shipment")!, child.GetQuery(), "customer_order"));
                    return this;
                }
                public CustomerOrderRequest CountOrderItems()
                    => CountOrderItemsAs("countOrderItems");

                public CustomerOrderRequest CountOrderItemsAs(string alias)
                    => CountOrderItemsWith(alias, new OrderItemRequest());

                public CustomerOrderRequest CountOrderItemsWith(string alias, OrderItemRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }


                public CustomerOrderRequest CountPayments()
                    => CountPaymentsAs("countPayments");

                public CustomerOrderRequest CountPaymentsAs(string alias)
                    => CountPaymentsWith(alias, new PaymentRequest());

                public CustomerOrderRequest CountPaymentsWith(string alias, PaymentRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("PaymentList", "Payment", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }


                public CustomerOrderRequest CountShipments()
                    => CountShipmentsAs("countShipments");

                public CustomerOrderRequest CountShipmentsAs(string alias)
                    => CountShipmentsWith(alias, new ShipmentRequest());

                public CustomerOrderRequest CountShipmentsWith(string alias, ShipmentRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("ShipmentList", "Shipment", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }


                public CustomerOrderRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "platform", request.GetQuery(), includeAllFacets));
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

        private async Task<CustomerOrderPage> ExecuteForPageInternalAsync(
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
                var countQuery = authorized.ForExactCount();
                var countResult = await service.QueryAsync(req.WithQuery(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.CustomerOrder>();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.CustomerOrder.FromRecord(row, new EntityRoot()));
            return new CustomerOrderPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.CustomerOrder> ExecuteForStreamInternalAsync(
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
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.CustomerOrder.FromRecord(row, new EntityRoot());
            }
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableCustomerOrderRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<QueryResult>> _executeForOne;
        private readonly Func<UserContext, int, int, Task<CustomerOrderPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerOrder>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCustomerOrderRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<QueryResult>> executeForOne,
            Func<UserContext, int, int, Task<CustomerOrderPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerOrder>> executeForStream,
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

        public ExecutableCustomerOrderRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.CustomerOrder NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.CustomerOrder();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.CustomerOrder>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.CustomerOrder>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.CustomerOrder.FromRecord(row, new EntityRoot()));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<CustomerOrderPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.CustomerOrder> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.CustomerOrder?> ExecuteForOneAsync(
            UserContext context)
        {
            var result = await _executeForOne(context);
            if (result.Rows.Count == 0) return null;
            return Generated.Models.CustomerOrder.FromRecord(
                result.Rows[0], new EntityRoot());
        }
    }

    public sealed class CustomerOrderPage
    {
        public SmartList<Generated.Models.CustomerOrder> Rows { get; }
        public long TotalCount { get; }
        public CustomerOrderPage(
            SmartList<Generated.Models.CustomerOrder> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}