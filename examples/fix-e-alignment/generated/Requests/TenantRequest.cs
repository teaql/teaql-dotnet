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
    public class TenantRequest
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

        public TenantRequest()
        {
            _query = new SelectQuery("Tenant");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public TenantRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public TenantRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public TenantRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public TenantRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableTenantRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableTenantRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public TenantRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public TenantRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public TenantRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public TenantRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public TenantRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public TenantRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public TenantRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public TenantRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("version");
            return this;
        }

                public TenantRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public TenantRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public TenantRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }


                public TenantRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TenantRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TenantRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TenantRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public TenantRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public TenantRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public TenantRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public TenantRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TenantRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TenantRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TenantRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public TenantRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public TenantRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public TenantRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public TenantRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public TenantRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public TenantRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public TenantRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public TenantRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TenantRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TenantRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TenantRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TenantRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public TenantRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public TenantRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public TenantRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public TenantRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public TenantRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public TenantRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public TenantRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public TenantRequest Count()
        {
            return CountAs("count");
        }

        public TenantRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public TenantRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public TenantRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public TenantRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public TenantRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public TenantRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public TenantRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public TenantRequest SelectPurchaseOrderList()
                {
                    return SelectPurchaseOrderListWith(new PurchaseOrderRequest());
                }

                public TenantRequest SelectPurchaseOrderListWith(PurchaseOrderRequest child)
                {
                    _query.RelationQuery("PurchaseOrderList", "PurchaseOrder", "tenant", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPurchaseOrderListWith", Relation: "PurchaseOrderList")
                    };
                    return this;
                }
                public TenantRequest SelectOrderItemList()
                {
                    return SelectOrderItemListWith(new OrderItemRequest());
                }

                public TenantRequest SelectOrderItemListWith(OrderItemRequest child)
                {
                    _query.RelationQuery("OrderItemList", "OrderItem", "tenant", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectOrderItemListWith", Relation: "OrderItemList")
                    };
                    return this;
                }
                public TenantRequest HavePurchaseOrders()
                    => WithPurchaseOrderListMatching(new PurchaseOrderRequest());

                public TenantRequest HaveNoPurchaseOrders()
                    => WithoutPurchaseOrderListMatching(new PurchaseOrderRequest());

                public TenantRequest WithPurchaseOrderListMatching(PurchaseOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("PurchaseOrder")!, child.GetQuery(), "tenant"));
                    return this;
                }

                public TenantRequest WithoutPurchaseOrderListMatching(PurchaseOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("PurchaseOrder")!, child.GetQuery(), "tenant"));
                    return this;
                }
                public TenantRequest HaveOrderItems()
                    => WithOrderItemListMatching(new OrderItemRequest());

                public TenantRequest HaveNoOrderItems()
                    => WithoutOrderItemListMatching(new OrderItemRequest());

                public TenantRequest WithOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "tenant"));
                    return this;
                }

                public TenantRequest WithoutOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "tenant"));
                    return this;
                }
                public TenantRequest CountPurchaseOrders()
                    => CountPurchaseOrdersAs("countPurchaseOrders");

                public TenantRequest CountPurchaseOrdersAs(string alias)
                {
                    CountPurchaseOrdersWith(alias, new PurchaseOrderRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountPurchaseOrdersAs", alias, "PurchaseOrderList")
                    };
                    return this;
                }

                public TenantRequest CountPurchaseOrdersWith(string alias, PurchaseOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("PurchaseOrderList", "PurchaseOrder", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountPurchaseOrdersWith", alias, "PurchaseOrderList")
                    };
                    return this;
                }


                public TenantRequest CountOrderItems()
                    => CountOrderItemsAs("countOrderItems");

                public TenantRequest CountOrderItemsAs(string alias)
                {
                    CountOrderItemsWith(alias, new OrderItemRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }

                public TenantRequest CountOrderItemsWith(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderItemsWith", alias, "OrderItemList")
                    };
                    return this;
                }

                public TenantRequest MinUnitPriceOfOrderItems()
                    => MinUnitPriceOfOrderItemsAs("minOfUnitPriceOfOrderItems", new OrderItemRequest());

                public TenantRequest MinUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public TenantRequest MaxUnitPriceOfOrderItems()
                    => MaxUnitPriceOfOrderItemsAs("maxOfUnitPriceOfOrderItems", new OrderItemRequest());

                public TenantRequest MaxUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public TenantRequest SumUnitPriceOfOrderItems()
                    => SumUnitPriceOfOrderItemsAs("sumOfUnitPriceOfOrderItems", new OrderItemRequest());

                public TenantRequest SumUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public TenantRequest AvgUnitPriceOfOrderItems()
                    => AvgUnitPriceOfOrderItemsAs("avgOfUnitPriceOfOrderItems", new OrderItemRequest());

                public TenantRequest AvgUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public TenantRequest StandardDeviationUnitPriceOfOrderItems()
                    => StandardDeviationUnitPriceOfOrderItemsAs("standardDeviationOfUnitPriceOfOrderItems", new OrderItemRequest());

                public TenantRequest StandardDeviationUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public TenantRequest SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItems()
                    => SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItemsAs("squareRootOfPopulationStandardDeviationOfUnitPriceOfOrderItems", new OrderItemRequest());

                public TenantRequest SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public TenantRequest SampleVarianceUnitPriceOfOrderItems()
                    => SampleVarianceUnitPriceOfOrderItemsAs("sampleVarianceOfUnitPriceOfOrderItems", new OrderItemRequest());

                public TenantRequest SampleVarianceUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public TenantRequest SamplePopulationVarianceUnitPriceOfOrderItems()
                    => SamplePopulationVarianceUnitPriceOfOrderItemsAs("samplePopulationVarianceOfUnitPriceOfOrderItems", new OrderItemRequest());

                public TenantRequest SamplePopulationVarianceUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "tenant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceUnitPriceOfOrderItemsAs", alias, "OrderItemList")
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

        private async Task<Generated.Models.Tenant?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.Tenant.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.Tenant>> ExecuteForListWithMetadataInternalAsync(
            UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            var request = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            SmartList<Record> result;
            if (_countRequested) result = await RuntimeDataService.FetchPageAsync(service, context, request);
            else {
                var rows = await service.QueryAsync(request);
                result = new SmartList<Record>(rows.Rows) { Facets = rows.Facets };
            }
            var entities = new SmartList<Generated.Models.Tenant>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Tenant.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Tenant> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var request = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            return RuntimeDataService.StreamEntities(context, request,
                row => global::TeaQL.Runtime.EntityQueryProperties.Capture(
                    Generated.Models.Tenant.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableTenantRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Tenant?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Tenant>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Tenant>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableTenantRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Tenant?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Tenant>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Tenant>> executeForStream,
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

        public ExecutableTenantRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.Tenant NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.Tenant();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Tenant>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Tenant> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Tenant?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}