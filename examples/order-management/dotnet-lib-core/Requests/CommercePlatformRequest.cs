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
    public class CommercePlatformRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;
        private bool _countRequested;
        private readonly List<DynamicSearchWarning> _dynamicSearchWarnings = new();
        private static object TeaqlQueryValue(object value) =>
            value is TimeSpan time
                ? time.ToString("c", System.Globalization.CultureInfo.InvariantCulture)
                : value;

        public CommercePlatformRequest()
        {
            _query = new SelectQuery("CommercePlatform");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public CommercePlatformRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public CommercePlatformRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public CommercePlatformRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public CommercePlatformRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCommercePlatformRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCommercePlatformRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public CommercePlatformRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public CommercePlatformRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public CommercePlatformRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public CommercePlatformRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public CommercePlatformRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public CommercePlatformRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public CommercePlatformRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public CommercePlatformRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public CommercePlatformRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public CommercePlatformRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public CommercePlatformRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public CommercePlatformRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public CommercePlatformRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }


                public CommercePlatformRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CommercePlatformRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public CommercePlatformRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public CommercePlatformRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public CommercePlatformRequest WithNameIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithNameIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithNameNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithNameGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithNameGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithNameLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithNameLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithNameBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CommercePlatformRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public CommercePlatformRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public CommercePlatformRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("create_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("update_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public CommercePlatformRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public CommercePlatformRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CommercePlatformRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CommercePlatformRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public CommercePlatformRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public CommercePlatformRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public CommercePlatformRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public CommercePlatformRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public CommercePlatformRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public CommercePlatformRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public CommercePlatformRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public CommercePlatformRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public CommercePlatformRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public CommercePlatformRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public CommercePlatformRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public CommercePlatformRequest Count()
        {
            return CountAs("count");
        }

        public CommercePlatformRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public CommercePlatformRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public CommercePlatformRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id");
                    _query.ProjectExpr(retName, Expr.Column("id"));
                    return this;
                }
                public CommercePlatformRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public CommercePlatformRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name");
                    _query.ProjectExpr(retName, Expr.Column("name"));
                    return this;
                }
                public CommercePlatformRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public CommercePlatformRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time");
                    _query.ProjectExpr(retName, Expr.Column("create_time"));
                    return this;
                }
                public CommercePlatformRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public CommercePlatformRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time");
                    _query.ProjectExpr(retName, Expr.Column("update_time"));
                    return this;
                }
                public CommercePlatformRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public CommercePlatformRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version");
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public CommercePlatformRequest SelectCustomerList()
                {
                    return SelectCustomerListWith(new CustomerRequest());
                }

                public CommercePlatformRequest SelectCustomerListWith(CustomerRequest child)
                {
                    _query.RelationQuery("CustomerList", "Customer", "commerce_platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerListWith", Relation: "CustomerList")
                    };
                    return this;
                }
                public CommercePlatformRequest SelectOrderStatusList()
                {
                    return SelectOrderStatusListWith(new OrderStatusRequest());
                }

                public CommercePlatformRequest SelectOrderStatusListWith(OrderStatusRequest child)
                {
                    _query.RelationQuery("OrderStatusList", "OrderStatus", "commerce_platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectOrderStatusListWith", Relation: "OrderStatusList")
                    };
                    return this;
                }
                public CommercePlatformRequest SelectCustomerOrderList()
                {
                    return SelectCustomerOrderListWith(new CustomerOrderRequest());
                }

                public CommercePlatformRequest SelectCustomerOrderListWith(CustomerOrderRequest child)
                {
                    _query.RelationQuery("CustomerOrderList", "CustomerOrder", "commerce_platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderListWith", Relation: "CustomerOrderList")
                    };
                    return this;
                }
                public CommercePlatformRequest SelectProductList()
                {
                    return SelectProductListWith(new ProductRequest());
                }

                public CommercePlatformRequest SelectProductListWith(ProductRequest child)
                {
                    _query.RelationQuery("ProductList", "Product", "commerce_platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectProductListWith", Relation: "ProductList")
                    };
                    return this;
                }
                public CommercePlatformRequest SelectOrderLineList()
                {
                    return SelectOrderLineListWith(new OrderLineRequest());
                }

                public CommercePlatformRequest SelectOrderLineListWith(OrderLineRequest child)
                {
                    _query.RelationQuery("OrderLineList", "OrderLine", "commerce_platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectOrderLineListWith", Relation: "OrderLineList")
                    };
                    return this;
                }
                public CommercePlatformRequest SelectOrderSearchPresetList()
                {
                    return SelectOrderSearchPresetListWith(new OrderSearchPresetRequest());
                }

                public CommercePlatformRequest SelectOrderSearchPresetListWith(OrderSearchPresetRequest child)
                {
                    _query.RelationQuery("OrderSearchPresetList", "OrderSearchPreset", "commerce_platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectOrderSearchPresetListWith", Relation: "OrderSearchPresetList")
                    };
                    return this;
                }
                public CommercePlatformRequest HaveCustomers()
                    => WithCustomerListMatching(new CustomerRequest());

                public CommercePlatformRequest HaveNoCustomers()
                    => WithoutCustomerListMatching(new CustomerRequest());

                public CommercePlatformRequest WithCustomerListMatching(CustomerRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Customer")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutCustomerListMatching(CustomerRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Customer")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveOrderStatuses()
                    => WithOrderStatusListMatching(new OrderStatusRequest());

                public CommercePlatformRequest HaveNoOrderStatuses()
                    => WithoutOrderStatusListMatching(new OrderStatusRequest());

                public CommercePlatformRequest WithOrderStatusListMatching(OrderStatusRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderStatus")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutOrderStatusListMatching(OrderStatusRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderStatus")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveCustomerOrders()
                    => WithCustomerOrderListMatching(new CustomerOrderRequest());

                public CommercePlatformRequest HaveNoCustomerOrders()
                    => WithoutCustomerOrderListMatching(new CustomerOrderRequest());

                public CommercePlatformRequest WithCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveProducts()
                    => WithProductListMatching(new ProductRequest());

                public CommercePlatformRequest HaveNoProducts()
                    => WithoutProductListMatching(new ProductRequest());

                public CommercePlatformRequest WithProductListMatching(ProductRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Product")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutProductListMatching(ProductRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Product")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveOrderLines()
                    => WithOrderLineListMatching(new OrderLineRequest());

                public CommercePlatformRequest HaveNoOrderLines()
                    => WithoutOrderLineListMatching(new OrderLineRequest());

                public CommercePlatformRequest WithOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderLine")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderLine")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveOrderSearchPresets()
                    => WithOrderSearchPresetListMatching(new OrderSearchPresetRequest());

                public CommercePlatformRequest HaveNoOrderSearchPresets()
                    => WithoutOrderSearchPresetListMatching(new OrderSearchPresetRequest());

                public CommercePlatformRequest WithOrderSearchPresetListMatching(OrderSearchPresetRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderSearchPreset")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutOrderSearchPresetListMatching(OrderSearchPresetRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderSearchPreset")!, child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest CountCustomers()
                    => CountCustomersAs("countCustomers");

                public CommercePlatformRequest CountCustomersAs(string alias)
                {
                    CountCustomersWith(alias, new CustomerRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomersAs", alias, "CustomerList")
                    };
                    return this;
                }

                public CommercePlatformRequest CountCustomersWith(string alias, CustomerRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("CustomerList", "Customer", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomersWith", alias, "CustomerList")
                    };
                    return this;
                }


                public CommercePlatformRequest CountOrderStatuses()
                    => CountOrderStatusesAs("countOrderStatuses");

                public CommercePlatformRequest CountOrderStatusesAs(string alias)
                {
                    CountOrderStatusesWith(alias, new OrderStatusRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }

                public CommercePlatformRequest CountOrderStatusesWith(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderStatusesWith", alias, "OrderStatusList")
                    };
                    return this;
                }

                public CommercePlatformRequest MinDisplayOrderOfOrderStatuses()
                    => MinDisplayOrderOfOrderStatusesAs("minOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest MinDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public CommercePlatformRequest MaxDisplayOrderOfOrderStatuses()
                    => MaxDisplayOrderOfOrderStatusesAs("maxOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest MaxDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public CommercePlatformRequest SumDisplayOrderOfOrderStatuses()
                    => SumDisplayOrderOfOrderStatusesAs("sumOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest SumDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public CommercePlatformRequest AvgDisplayOrderOfOrderStatuses()
                    => AvgDisplayOrderOfOrderStatusesAs("avgOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest AvgDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public CommercePlatformRequest StandardDeviationDisplayOrderOfOrderStatuses()
                    => StandardDeviationDisplayOrderOfOrderStatusesAs("standardDeviationOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest StandardDeviationDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatuses()
                    => SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatusesAs("squareRootOfPopulationStandardDeviationOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public CommercePlatformRequest SampleVarianceDisplayOrderOfOrderStatuses()
                    => SampleVarianceDisplayOrderOfOrderStatusesAs("sampleVarianceOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest SampleVarianceDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public CommercePlatformRequest SamplePopulationVarianceDisplayOrderOfOrderStatuses()
                    => SamplePopulationVarianceDisplayOrderOfOrderStatusesAs("samplePopulationVarianceOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest SamplePopulationVarianceDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public CommercePlatformRequest CountCustomerOrders()
                    => CountCustomerOrdersAs("countCustomerOrders");

                public CommercePlatformRequest CountCustomerOrdersAs(string alias)
                {
                    CountCustomerOrdersWith(alias, new CustomerOrderRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }

                public CommercePlatformRequest CountCustomerOrdersWith(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersWith", alias, "CustomerOrderList")
                    };
                    return this;
                }

                public CommercePlatformRequest MinTotalAmountOfCustomerOrders()
                    => MinTotalAmountOfCustomerOrdersAs("minOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest MinTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CommercePlatformRequest MaxTotalAmountOfCustomerOrders()
                    => MaxTotalAmountOfCustomerOrdersAs("maxOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest MaxTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CommercePlatformRequest SumTotalAmountOfCustomerOrders()
                    => SumTotalAmountOfCustomerOrdersAs("sumOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest SumTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CommercePlatformRequest AvgTotalAmountOfCustomerOrders()
                    => AvgTotalAmountOfCustomerOrdersAs("avgOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest AvgTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CommercePlatformRequest StandardDeviationTotalAmountOfCustomerOrders()
                    => StandardDeviationTotalAmountOfCustomerOrdersAs("standardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest StandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrders()
                    => SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs("squareRootOfPopulationStandardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CommercePlatformRequest SampleVarianceTotalAmountOfCustomerOrders()
                    => SampleVarianceTotalAmountOfCustomerOrdersAs("sampleVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest SampleVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CommercePlatformRequest SamplePopulationVarianceTotalAmountOfCustomerOrders()
                    => SamplePopulationVarianceTotalAmountOfCustomerOrdersAs("samplePopulationVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest SamplePopulationVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CommercePlatformRequest CountProducts()
                    => CountProductsAs("countProducts");

                public CommercePlatformRequest CountProductsAs(string alias)
                {
                    CountProductsWith(alias, new ProductRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountProductsAs", alias, "ProductList")
                    };
                    return this;
                }

                public CommercePlatformRequest CountProductsWith(string alias, ProductRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("ProductList", "Product", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountProductsWith", alias, "ProductList")
                    };
                    return this;
                }


                public CommercePlatformRequest CountOrderLines()
                    => CountOrderLinesAs("countOrderLines");

                public CommercePlatformRequest CountOrderLinesAs(string alias)
                {
                    CountOrderLinesWith(alias, new OrderLineRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }

                public CommercePlatformRequest CountOrderLinesWith(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderLinesWith", alias, "OrderLineList")
                    };
                    return this;
                }

                public CommercePlatformRequest MinQuantityOfOrderLines()
                    => MinQuantityOfOrderLinesAs("minOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest MinQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public CommercePlatformRequest MaxQuantityOfOrderLines()
                    => MaxQuantityOfOrderLinesAs("maxOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest MaxQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public CommercePlatformRequest SumQuantityOfOrderLines()
                    => SumQuantityOfOrderLinesAs("sumOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest SumQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public CommercePlatformRequest AvgQuantityOfOrderLines()
                    => AvgQuantityOfOrderLinesAs("avgOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest AvgQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public CommercePlatformRequest StandardDeviationQuantityOfOrderLines()
                    => StandardDeviationQuantityOfOrderLinesAs("standardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest StandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLines()
                    => SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs("squareRootOfPopulationStandardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public CommercePlatformRequest SampleVarianceQuantityOfOrderLines()
                    => SampleVarianceQuantityOfOrderLinesAs("sampleVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest SampleVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public CommercePlatformRequest SamplePopulationVarianceQuantityOfOrderLines()
                    => SamplePopulationVarianceQuantityOfOrderLinesAs("samplePopulationVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest SamplePopulationVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public CommercePlatformRequest CountOrderSearchPresets()
                    => CountOrderSearchPresetsAs("countOrderSearchPresets");

                public CommercePlatformRequest CountOrderSearchPresetsAs(string alias)
                {
                    CountOrderSearchPresetsWith(alias, new OrderSearchPresetRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderSearchPresetsAs", alias, "OrderSearchPresetList")
                    };
                    return this;
                }

                public CommercePlatformRequest CountOrderSearchPresetsWith(string alias, OrderSearchPresetRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("OrderSearchPresetList", "OrderSearchPreset", "commerce_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderSearchPresetsWith", alias, "OrderSearchPresetList")
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

        private async Task<Generated.Models.CommercePlatform?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.CommercePlatform.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.CommercePlatform>> ExecuteForListWithMetadataInternalAsync(
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
            var entities = new SmartList<Generated.Models.CommercePlatform>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.CommercePlatform.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.CommercePlatform> ExecuteForStreamInternalAsync(
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
                    Generated.Models.CommercePlatform.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableCommercePlatformRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.CommercePlatform?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.CommercePlatform>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CommercePlatform>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCommercePlatformRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.CommercePlatform?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.CommercePlatform>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CommercePlatform>> executeForStream,
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

        public ExecutableCommercePlatformRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.CommercePlatform> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("CommercePlatform").ConfigureAwait(false));
            var entity = Generated.Models.CommercePlatform.TeaqlNewWithReservedId(identity);
            return context.InitializeEntity("CommercePlatform", entity,
                initialized => initialized.TeaqlValidateReservedIdentity(identity));
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.CommercePlatform>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.CommercePlatform> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.CommercePlatform?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}