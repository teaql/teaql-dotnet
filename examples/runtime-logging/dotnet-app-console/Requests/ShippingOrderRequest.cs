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
    public class ShippingOrderRequest
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

        public ShippingOrderRequest()
        {
            _query = new SelectQuery("ShippingOrder");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public ShippingOrderRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public ShippingOrderRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public ShippingOrderRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public ShippingOrderRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableShippingOrderRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableShippingOrderRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public ShippingOrderRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public ShippingOrderRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public ShippingOrderRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public ShippingOrderRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public ShippingOrderRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public ShippingOrderRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public ShippingOrderRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public ShippingOrderRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("warehouse");
            _query.Project("reference");
            _query.Project("customer_reference");
            _query.Project("instructions");
            _query.Project("version");
            return this;
        }

                public ShippingOrderRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public ShippingOrderRequest SelectReference()
                {
                    _query.Project("reference");
                    return this;
                }

                public ShippingOrderRequest SelectCustomerReference()
                {
                    _query.Project("customer_reference");
                    return this;
                }

                public ShippingOrderRequest SelectInstructions()
                {
                    _query.Project("instructions");
                    return this;
                }

                public ShippingOrderRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public ShippingOrderRequest SelectWarehouse()
                {
                    return SelectWarehouseWith(new WarehouseRequest());
                }

                public ShippingOrderRequest SelectWarehouseWith(WarehouseRequest related)
                {
                    _query.Project("warehouse");
                    _query.ForwardRelationQuery("Warehouse", "Warehouse", "warehouse", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectWarehouseWith", Relation: "Warehouse")
                    };
                    return this;
                }
                public ShippingOrderRequest WithWarehouseMatching(WarehouseRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("warehouse", GeneratedRuntimeModule.Module.Metadata.GetEntity("Warehouse")!, related.GetQuery(), "id"));
                    return this;
                }

                public ShippingOrderRequest WithoutWarehouseMatching(WarehouseRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("warehouse", GeneratedRuntimeModule.Module.Metadata.GetEntity("Warehouse")!, related.GetQuery(), "id"));
                    return this;
                }

                public ShippingOrderRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShippingOrderRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public ShippingOrderRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public ShippingOrderRequest FilterByWarehouse(object val)
                {
                    _query.AndFilter(Expr.Eq("warehouse", val));
                    return this;
                }

                public ShippingOrderRequest FilterByWarehouseIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("warehouse", vals));
                    return this;
                }

                public ShippingOrderRequest WithWarehouseIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("warehouse"));
                    return this;
                }

                public ShippingOrderRequest WithWarehouseIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("warehouse"));
                    return this;
                }

                public ShippingOrderRequest WithReferenceContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("reference", val));
                    return this;
                }

                public ShippingOrderRequest WithReferenceIs(string val)
                {
                    _query.AndFilter(Expr.Eq("reference", TeaqlQueryValue(val)));
                    return this;
                }
                public ShippingOrderRequest WithReferenceIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithReferenceIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("reference", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithReferenceNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("reference", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithReferenceGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithReferenceGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithReferenceLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithReferenceLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithReferenceBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("reference", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShippingOrderRequest WithReferenceIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("reference"));
                    return this;
                }

                public ShippingOrderRequest WithReferenceIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("reference"));
                    return this;
                }
                public ShippingOrderRequest WithReferenceNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("reference", val));
                    return this;
                }

                public ShippingOrderRequest WithReferenceStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("reference", val));
                    return this;
                }

                public ShippingOrderRequest WithReferenceNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("reference", val));
                    return this;
                }

                public ShippingOrderRequest WithReferenceEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("reference", val));
                    return this;
                }

                public ShippingOrderRequest WithReferenceNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("reference", val));
                    return this;
                }

                public ShippingOrderRequest WithReferenceSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("reference", val));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("customer_reference", val));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceIs(string val)
                {
                    _query.AndFilter(Expr.Eq("customer_reference", TeaqlQueryValue(val)));
                    return this;
                }
                public ShippingOrderRequest WithCustomerReferenceIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("customer_reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("customer_reference", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("customer_reference", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("customer_reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("customer_reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("customer_reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("customer_reference", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("customer_reference", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("customer_reference"));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("customer_reference"));
                    return this;
                }
                public ShippingOrderRequest WithCustomerReferenceNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("customer_reference", val));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("customer_reference", val));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("customer_reference", val));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("customer_reference", val));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("customer_reference", val));
                    return this;
                }

                public ShippingOrderRequest WithCustomerReferenceSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("customer_reference", val));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("instructions", val));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsIs(string val)
                {
                    _query.AndFilter(Expr.Eq("instructions", TeaqlQueryValue(val)));
                    return this;
                }
                public ShippingOrderRequest WithInstructionsIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("instructions", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("instructions", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("instructions", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("instructions"));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("instructions"));
                    return this;
                }
                public ShippingOrderRequest WithInstructionsNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("instructions", val));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("instructions", val));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("instructions", val));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("instructions", val));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("instructions", val));
                    return this;
                }

                public ShippingOrderRequest WithInstructionsSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("instructions", val));
                    return this;
                }

                public ShippingOrderRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingOrderRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingOrderRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShippingOrderRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public ShippingOrderRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public ShippingOrderRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public ShippingOrderRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public ShippingOrderRequest OrderByReferenceAscending()
                {
                    _query.OrderBy("reference", "asc");
                    return this;
                }

                public ShippingOrderRequest OrderByReferenceDescending()
                {
                    _query.OrderBy("reference", "desc");
                    return this;
                }

                public ShippingOrderRequest OrderByCustomerReferenceAscending()
                {
                    _query.OrderBy("customer_reference", "asc");
                    return this;
                }

                public ShippingOrderRequest OrderByCustomerReferenceDescending()
                {
                    _query.OrderBy("customer_reference", "desc");
                    return this;
                }

                public ShippingOrderRequest OrderByInstructionsAscending()
                {
                    _query.OrderBy("instructions", "asc");
                    return this;
                }

                public ShippingOrderRequest OrderByInstructionsDescending()
                {
                    _query.OrderBy("instructions", "desc");
                    return this;
                }

                public ShippingOrderRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public ShippingOrderRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public ShippingOrderRequest Count()
        {
            return CountAs("count");
        }

        public ShippingOrderRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public ShippingOrderRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public ShippingOrderRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public ShippingOrderRequest GroupByWarehouse()
                {
                    _query.GroupBy("warehouse");
                    return this;
                }

                public ShippingOrderRequest GroupByWarehouseAs(string retName)
                {
                    _query.GroupBy("warehouse"); 
                    return this;
                }
                public ShippingOrderRequest GroupByReference()
                {
                    _query.GroupBy("reference");
                    return this;
                }

                public ShippingOrderRequest GroupByReferenceAs(string retName)
                {
                    _query.GroupBy("reference"); 
                    return this;
                }
                public ShippingOrderRequest GroupByCustomerReference()
                {
                    _query.GroupBy("customer_reference");
                    return this;
                }

                public ShippingOrderRequest GroupByCustomerReferenceAs(string retName)
                {
                    _query.GroupBy("customer_reference"); 
                    return this;
                }
                public ShippingOrderRequest GroupByInstructions()
                {
                    _query.GroupBy("instructions");
                    return this;
                }

                public ShippingOrderRequest GroupByInstructionsAs(string retName)
                {
                    _query.GroupBy("instructions"); 
                    return this;
                }
                public ShippingOrderRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public ShippingOrderRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public ShippingOrderRequest SelectShippingItemList()
                {
                    return SelectShippingItemListWith(new ShippingItemRequest());
                }

                public ShippingOrderRequest SelectShippingItemListWith(ShippingItemRequest child)
                {
                    _query.RelationQuery("ShippingItemList", "ShippingItem", "shipping_order", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectShippingItemListWith", Relation: "ShippingItemList")
                    };
                    return this;
                }
                public ShippingOrderRequest HaveShippingItems()
                    => WithShippingItemListMatching(new ShippingItemRequest());

                public ShippingOrderRequest HaveNoShippingItems()
                    => WithoutShippingItemListMatching(new ShippingItemRequest());

                public ShippingOrderRequest WithShippingItemListMatching(ShippingItemRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("ShippingItem")!, child.GetQuery(), "shipping_order"));
                    return this;
                }

                public ShippingOrderRequest WithoutShippingItemListMatching(ShippingItemRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("ShippingItem")!, child.GetQuery(), "shipping_order"));
                    return this;
                }
                public ShippingOrderRequest CountShippingItems()
                    => CountShippingItemsAs("countShippingItems");

                public ShippingOrderRequest CountShippingItemsAs(string alias)
                {
                    CountShippingItemsWith(alias, new ShippingItemRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountShippingItemsAs", alias, "ShippingItemList")
                    };
                    return this;
                }

                public ShippingOrderRequest CountShippingItemsWith(string alias, ShippingItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("ShippingItemList", "ShippingItem", "shipping_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountShippingItemsWith", alias, "ShippingItemList")
                    };
                    return this;
                }

                public ShippingOrderRequest MinQuantityOfShippingItems()
                    => MinQuantityOfShippingItemsAs("minOfQuantityOfShippingItems", new ShippingItemRequest());

                public ShippingOrderRequest MinQuantityOfShippingItemsAs(string alias, ShippingItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "quantity", alias);
                    _query.RelationAggregate("ShippingItemList", "ShippingItem", "shipping_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinQuantityOfShippingItemsAs", alias, "ShippingItemList")
                    };
                    return this;
                }
                public ShippingOrderRequest MaxQuantityOfShippingItems()
                    => MaxQuantityOfShippingItemsAs("maxOfQuantityOfShippingItems", new ShippingItemRequest());

                public ShippingOrderRequest MaxQuantityOfShippingItemsAs(string alias, ShippingItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "quantity", alias);
                    _query.RelationAggregate("ShippingItemList", "ShippingItem", "shipping_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxQuantityOfShippingItemsAs", alias, "ShippingItemList")
                    };
                    return this;
                }
                public ShippingOrderRequest SumQuantityOfShippingItems()
                    => SumQuantityOfShippingItemsAs("sumOfQuantityOfShippingItems", new ShippingItemRequest());

                public ShippingOrderRequest SumQuantityOfShippingItemsAs(string alias, ShippingItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "quantity", alias);
                    _query.RelationAggregate("ShippingItemList", "ShippingItem", "shipping_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumQuantityOfShippingItemsAs", alias, "ShippingItemList")
                    };
                    return this;
                }
                public ShippingOrderRequest AvgQuantityOfShippingItems()
                    => AvgQuantityOfShippingItemsAs("avgOfQuantityOfShippingItems", new ShippingItemRequest());

                public ShippingOrderRequest AvgQuantityOfShippingItemsAs(string alias, ShippingItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "quantity", alias);
                    _query.RelationAggregate("ShippingItemList", "ShippingItem", "shipping_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgQuantityOfShippingItemsAs", alias, "ShippingItemList")
                    };
                    return this;
                }
                public ShippingOrderRequest StandardDeviationQuantityOfShippingItems()
                    => StandardDeviationQuantityOfShippingItemsAs("standardDeviationOfQuantityOfShippingItems", new ShippingItemRequest());

                public ShippingOrderRequest StandardDeviationQuantityOfShippingItemsAs(string alias, ShippingItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "quantity", alias);
                    _query.RelationAggregate("ShippingItemList", "ShippingItem", "shipping_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationQuantityOfShippingItemsAs", alias, "ShippingItemList")
                    };
                    return this;
                }
                public ShippingOrderRequest SquareRootOfPopulationStandardDeviationQuantityOfShippingItems()
                    => SquareRootOfPopulationStandardDeviationQuantityOfShippingItemsAs("squareRootOfPopulationStandardDeviationOfQuantityOfShippingItems", new ShippingItemRequest());

                public ShippingOrderRequest SquareRootOfPopulationStandardDeviationQuantityOfShippingItemsAs(string alias, ShippingItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "quantity", alias);
                    _query.RelationAggregate("ShippingItemList", "ShippingItem", "shipping_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationQuantityOfShippingItemsAs", alias, "ShippingItemList")
                    };
                    return this;
                }
                public ShippingOrderRequest SampleVarianceQuantityOfShippingItems()
                    => SampleVarianceQuantityOfShippingItemsAs("sampleVarianceOfQuantityOfShippingItems", new ShippingItemRequest());

                public ShippingOrderRequest SampleVarianceQuantityOfShippingItemsAs(string alias, ShippingItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "quantity", alias);
                    _query.RelationAggregate("ShippingItemList", "ShippingItem", "shipping_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceQuantityOfShippingItemsAs", alias, "ShippingItemList")
                    };
                    return this;
                }
                public ShippingOrderRequest SamplePopulationVarianceQuantityOfShippingItems()
                    => SamplePopulationVarianceQuantityOfShippingItemsAs("samplePopulationVarianceOfQuantityOfShippingItems", new ShippingItemRequest());

                public ShippingOrderRequest SamplePopulationVarianceQuantityOfShippingItemsAs(string alias, ShippingItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "quantity", alias);
                    _query.RelationAggregate("ShippingItemList", "ShippingItem", "shipping_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceQuantityOfShippingItemsAs", alias, "ShippingItemList")
                    };
                    return this;
                }
                public ShippingOrderRequest FacetByWarehouseAs(
                    string name, WarehouseRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Warehouse", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByWarehouseAs", name, "warehouse")
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

        private async Task<Generated.Models.ShippingOrder?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.ShippingOrder>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.ShippingOrder>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.ShippingOrder>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.ShippingOrder.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.ShippingOrder> ExecuteForStreamInternalAsync(
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
                    Generated.Models.ShippingOrder.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableShippingOrderRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.ShippingOrder?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.ShippingOrder>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.ShippingOrder>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableShippingOrderRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.ShippingOrder?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.ShippingOrder>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.ShippingOrder>> executeForStream,
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

        public ExecutableShippingOrderRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.ShippingOrder> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("ShippingOrder").ConfigureAwait(false));
            return context.InitializeEntity("ShippingOrder", Generated.Models.ShippingOrder.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.ShippingOrder>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.ShippingOrder> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.ShippingOrder?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}