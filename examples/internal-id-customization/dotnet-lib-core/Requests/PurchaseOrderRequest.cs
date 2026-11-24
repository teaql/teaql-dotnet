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
            _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public PurchaseOrderRequest SelectId()
                {
                    _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public PurchaseOrderRequest SelectReference()
                {
                    _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest SelectWarehouse()
                {
                    return SelectWarehouseWith(new WarehouseRequest());
                }

                public PurchaseOrderRequest SelectWarehouseWith(WarehouseRequest related)
                {
                    _query.Project(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER, "Warehouse", global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectWarehouseWith", Relation: global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public PurchaseOrderRequest WithWarehouseMatching(WarehouseRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Warehouse")!, related.GetQuery(), "id"));
                    return this;
                }

                public PurchaseOrderRequest WithoutWarehouseMatching(WarehouseRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Warehouse")!, related.GetQuery(), "id"));
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

                public PurchaseOrderRequest FilterByWarehouse(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest FilterByWarehouseIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY, vals));
                    return this;
                }

                public PurchaseOrderRequest WithWarehouseIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest WithWarehouseIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public PurchaseOrderRequest WithReferenceIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY));
                    return this;
                }
                public PurchaseOrderRequest WithReferenceNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public PurchaseOrderRequest WithReferenceSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, val));
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


                public PurchaseOrderRequest OrderByReferenceAscending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByReferenceDescending()
                {
                    _query.OrderBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY, "desc");
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
                public PurchaseOrderRequest GroupByWarehouse()
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest GroupByWarehouseAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY);
                    return this;
                }
                public PurchaseOrderRequest GroupByReference()
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY);
                    return this;
                }

                public PurchaseOrderRequest GroupByReferenceAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PurchaseOrder.__TeaqlFields.REFERENCE_PROPERTY_QUERY);
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
                public PurchaseOrderRequest SelectOrderItemList()
                {
                    return SelectOrderItemListWith(new OrderItemRequest());
                }

                public PurchaseOrderRequest SelectOrderItemListWith(OrderItemRequest child)
                {
                    _query.RelationQuery("OrderItemList", "OrderItem", "purchase_order", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectOrderItemListWith", Relation: "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest HaveOrderItems()
                    => WithOrderItemListMatching(new OrderItemRequest());

                public PurchaseOrderRequest HaveNoOrderItems()
                    => WithoutOrderItemListMatching(new OrderItemRequest());

                public PurchaseOrderRequest WithOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "purchase_order"));
                    return this;
                }

                public PurchaseOrderRequest WithoutOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "purchase_order"));
                    return this;
                }
                public PurchaseOrderRequest CountOrderItems()
                    => CountOrderItemsAs("countOrderItems");

                public PurchaseOrderRequest CountOrderItemsAs(string alias)
                {
                    CountOrderItemsWith(alias, new OrderItemRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }

                public PurchaseOrderRequest CountOrderItemsWith(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderItemsWith", alias, "OrderItemList")
                    };
                    return this;
                }

                public PurchaseOrderRequest MinQuantityOfOrderItems()
                    => MinQuantityOfOrderItemsAs("minOfQuantityOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest MinQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest MaxQuantityOfOrderItems()
                    => MaxQuantityOfOrderItemsAs("maxOfQuantityOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest MaxQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest SumQuantityOfOrderItems()
                    => SumQuantityOfOrderItemsAs("sumOfQuantityOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest SumQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest AvgQuantityOfOrderItems()
                    => AvgQuantityOfOrderItemsAs("avgOfQuantityOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest AvgQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest StandardDeviationQuantityOfOrderItems()
                    => StandardDeviationQuantityOfOrderItemsAs("standardDeviationOfQuantityOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest StandardDeviationQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderItems()
                    => SquareRootOfPopulationStandardDeviationQuantityOfOrderItemsAs("squareRootOfPopulationStandardDeviationOfQuantityOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest SampleVarianceQuantityOfOrderItems()
                    => SampleVarianceQuantityOfOrderItemsAs("sampleVarianceOfQuantityOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest SampleVarianceQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest SamplePopulationVarianceQuantityOfOrderItems()
                    => SamplePopulationVarianceQuantityOfOrderItemsAs("samplePopulationVarianceOfQuantityOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest SamplePopulationVarianceQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest FacetByWarehouseAs(
                    string name, WarehouseRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByWarehouseAs", name, global::Generated.Models.PurchaseOrder.__TeaqlFields.WAREHOUSE_PROPERTY_QUERY)
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