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
    public class CommercePlatformRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;

        public CommercePlatformRequest()
        {
            _query = new SelectQuery("CommercePlatform");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

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
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCommercePlatformRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
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


                public CommercePlatformRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public CommercePlatformRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public CommercePlatformRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public CommercePlatformRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public CommercePlatformRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public CommercePlatformRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public CommercePlatformRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public CommercePlatformRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public CommercePlatformRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
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
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }
                public CommercePlatformRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public CommercePlatformRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals));
                    return this;
                }

                public CommercePlatformRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", val));
                    return this;
                }

                public CommercePlatformRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", lower, upper));
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

                public CommercePlatformRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", val));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", val));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", val));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", lower, upper));
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

                public CommercePlatformRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("update_time", val));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("update_time", val));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("update_time", val));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("update_time", lower, upper));
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

                public CommercePlatformRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public CommercePlatformRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public CommercePlatformRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public CommercePlatformRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public CommercePlatformRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public CommercePlatformRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public CommercePlatformRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public CommercePlatformRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public CommercePlatformRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
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
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public CommercePlatformRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
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
                    return this;
                }
                public CommercePlatformRequest SelectCustomerList()
                {
                    return SelectCustomerListWith(new CustomerRequest());
                }

                public CommercePlatformRequest SelectCustomerListWith(CustomerRequest child)
                {
                    _query.RelationQuery("CustomerList", "Customer", "commerce_platform", true, child.GetQuery());
                    return this;
                }
                public CommercePlatformRequest SelectOrderStatusList()
                {
                    return SelectOrderStatusListWith(new OrderStatusRequest());
                }

                public CommercePlatformRequest SelectOrderStatusListWith(OrderStatusRequest child)
                {
                    _query.RelationQuery("OrderStatusList", "OrderStatus", "commerce_platform", true, child.GetQuery());
                    return this;
                }
                public CommercePlatformRequest SelectCustomerOrderList()
                {
                    return SelectCustomerOrderListWith(new CustomerOrderRequest());
                }

                public CommercePlatformRequest SelectCustomerOrderListWith(CustomerOrderRequest child)
                {
                    _query.RelationQuery("CustomerOrderList", "CustomerOrder", "commerce_platform", true, child.GetQuery());
                    return this;
                }
                public CommercePlatformRequest SelectProductList()
                {
                    return SelectProductListWith(new ProductRequest());
                }

                public CommercePlatformRequest SelectProductListWith(ProductRequest child)
                {
                    _query.RelationQuery("ProductList", "Product", "commerce_platform", true, child.GetQuery());
                    return this;
                }
                public CommercePlatformRequest SelectOrderLineList()
                {
                    return SelectOrderLineListWith(new OrderLineRequest());
                }

                public CommercePlatformRequest SelectOrderLineListWith(OrderLineRequest child)
                {
                    _query.RelationQuery("OrderLineList", "OrderLine", "commerce_platform", true, child.GetQuery());
                    return this;
                }
                public CommercePlatformRequest SelectOrderSearchPresetList()
                {
                    return SelectOrderSearchPresetListWith(new OrderSearchPresetRequest());
                }

                public CommercePlatformRequest SelectOrderSearchPresetListWith(OrderSearchPresetRequest child)
                {
                    _query.RelationQuery("OrderSearchPresetList", "OrderSearchPreset", "commerce_platform", true, child.GetQuery());
                    return this;
                }
                public CommercePlatformRequest HaveCustomers()
                    => WithCustomerListMatching(new CustomerRequest());

                public CommercePlatformRequest HaveNoCustomers()
                    => WithoutCustomerListMatching(new CustomerRequest());

                public CommercePlatformRequest WithCustomerListMatching(CustomerRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("Customer"), child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutCustomerListMatching(CustomerRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("Customer"), child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveOrderStatuses()
                    => WithOrderStatusListMatching(new OrderStatusRequest());

                public CommercePlatformRequest HaveNoOrderStatuses()
                    => WithoutOrderStatusListMatching(new OrderStatusRequest());

                public CommercePlatformRequest WithOrderStatusListMatching(OrderStatusRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("OrderStatus"), child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutOrderStatusListMatching(OrderStatusRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("OrderStatus"), child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveCustomerOrders()
                    => WithCustomerOrderListMatching(new CustomerOrderRequest());

                public CommercePlatformRequest HaveNoCustomerOrders()
                    => WithoutCustomerOrderListMatching(new CustomerOrderRequest());

                public CommercePlatformRequest WithCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("CustomerOrder"), child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("CustomerOrder"), child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveProducts()
                    => WithProductListMatching(new ProductRequest());

                public CommercePlatformRequest HaveNoProducts()
                    => WithoutProductListMatching(new ProductRequest());

                public CommercePlatformRequest WithProductListMatching(ProductRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("Product"), child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutProductListMatching(ProductRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("Product"), child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveOrderLines()
                    => WithOrderLineListMatching(new OrderLineRequest());

                public CommercePlatformRequest HaveNoOrderLines()
                    => WithoutOrderLineListMatching(new OrderLineRequest());

                public CommercePlatformRequest WithOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("OrderLine"), child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("OrderLine"), child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest HaveOrderSearchPresets()
                    => WithOrderSearchPresetListMatching(new OrderSearchPresetRequest());

                public CommercePlatformRequest HaveNoOrderSearchPresets()
                    => WithoutOrderSearchPresetListMatching(new OrderSearchPresetRequest());

                public CommercePlatformRequest WithOrderSearchPresetListMatching(OrderSearchPresetRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("OrderSearchPreset"), child.GetQuery(), "commerce_platform"));
                    return this;
                }

                public CommercePlatformRequest WithoutOrderSearchPresetListMatching(OrderSearchPresetRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("OrderSearchPreset"), child.GetQuery(), "commerce_platform"));
                    return this;
                }
                public CommercePlatformRequest CountCustomers()
                    => CountCustomersAs("countCustomers");

                public CommercePlatformRequest CountCustomersAs(string alias)
                    => CountCustomersWith(alias, new CustomerRequest());

                public CommercePlatformRequest CountCustomersWith(string alias, CustomerRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("CustomerList", "Customer", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }


                public CommercePlatformRequest CountOrderStatuses()
                    => CountOrderStatusesAs("countOrderStatuses");

                public CommercePlatformRequest CountOrderStatusesAs(string alias)
                    => CountOrderStatusesWith(alias, new OrderStatusRequest());

                public CommercePlatformRequest CountOrderStatusesWith(string alias, OrderStatusRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }

                public CommercePlatformRequest MinDisplayOrderOfOrderStatuses()
                    => MinDisplayOrderOfOrderStatusesAs("minOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest MinDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    child.GetQuery().Aggregate("min", "display_order", "min_display_order");
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest MaxDisplayOrderOfOrderStatuses()
                    => MaxDisplayOrderOfOrderStatusesAs("maxOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest MaxDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    child.GetQuery().Aggregate("max", "display_order", "max_display_order");
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SumDisplayOrderOfOrderStatuses()
                    => SumDisplayOrderOfOrderStatusesAs("sumOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest SumDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    child.GetQuery().Aggregate("sum", "display_order", "sum_display_order");
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest AvgDisplayOrderOfOrderStatuses()
                    => AvgDisplayOrderOfOrderStatusesAs("avgOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest AvgDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    child.GetQuery().Aggregate("avg", "display_order", "avg_display_order");
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest StandardDeviationDisplayOrderOfOrderStatuses()
                    => StandardDeviationDisplayOrderOfOrderStatusesAs("standardDeviationOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest StandardDeviationDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "display_order", "standardDeviation_display_order");
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatuses()
                    => SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatusesAs("squareRootOfPopulationStandardDeviationOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "display_order", "squareRootOfPopulationStandardDeviation_display_order");
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SampleVarianceDisplayOrderOfOrderStatuses()
                    => SampleVarianceDisplayOrderOfOrderStatusesAs("sampleVarianceOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest SampleVarianceDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "display_order", "sampleVariance_display_order");
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SamplePopulationVarianceDisplayOrderOfOrderStatuses()
                    => SamplePopulationVarianceDisplayOrderOfOrderStatusesAs("samplePopulationVarianceOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public CommercePlatformRequest SamplePopulationVarianceDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "display_order", "samplePopulationVariance_display_order");
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest CountCustomerOrders()
                    => CountCustomerOrdersAs("countCustomerOrders");

                public CommercePlatformRequest CountCustomerOrdersAs(string alias)
                    => CountCustomerOrdersWith(alias, new CustomerOrderRequest());

                public CommercePlatformRequest CountCustomerOrdersWith(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }

                public CommercePlatformRequest MinTotalAmountOfCustomerOrders()
                    => MinTotalAmountOfCustomerOrdersAs("minOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest MinTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("min", "total_amount", "min_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest MaxTotalAmountOfCustomerOrders()
                    => MaxTotalAmountOfCustomerOrdersAs("maxOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest MaxTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("max", "total_amount", "max_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SumTotalAmountOfCustomerOrders()
                    => SumTotalAmountOfCustomerOrdersAs("sumOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest SumTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("sum", "total_amount", "sum_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest AvgTotalAmountOfCustomerOrders()
                    => AvgTotalAmountOfCustomerOrdersAs("avgOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest AvgTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("avg", "total_amount", "avg_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest StandardDeviationTotalAmountOfCustomerOrders()
                    => StandardDeviationTotalAmountOfCustomerOrdersAs("standardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest StandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "total_amount", "standardDeviation_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrders()
                    => SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs("squareRootOfPopulationStandardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "total_amount", "squareRootOfPopulationStandardDeviation_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SampleVarianceTotalAmountOfCustomerOrders()
                    => SampleVarianceTotalAmountOfCustomerOrdersAs("sampleVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest SampleVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "total_amount", "sampleVariance_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SamplePopulationVarianceTotalAmountOfCustomerOrders()
                    => SamplePopulationVarianceTotalAmountOfCustomerOrdersAs("samplePopulationVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CommercePlatformRequest SamplePopulationVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "total_amount", "samplePopulationVariance_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest CountProducts()
                    => CountProductsAs("countProducts");

                public CommercePlatformRequest CountProductsAs(string alias)
                    => CountProductsWith(alias, new ProductRequest());

                public CommercePlatformRequest CountProductsWith(string alias, ProductRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("ProductList", "Product", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }


                public CommercePlatformRequest CountOrderLines()
                    => CountOrderLinesAs("countOrderLines");

                public CommercePlatformRequest CountOrderLinesAs(string alias)
                    => CountOrderLinesWith(alias, new OrderLineRequest());

                public CommercePlatformRequest CountOrderLinesWith(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }

                public CommercePlatformRequest MinQuantityOfOrderLines()
                    => MinQuantityOfOrderLinesAs("minOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest MinQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("min", "quantity", "min_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest MaxQuantityOfOrderLines()
                    => MaxQuantityOfOrderLinesAs("maxOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest MaxQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("max", "quantity", "max_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SumQuantityOfOrderLines()
                    => SumQuantityOfOrderLinesAs("sumOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest SumQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("sum", "quantity", "sum_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest AvgQuantityOfOrderLines()
                    => AvgQuantityOfOrderLinesAs("avgOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest AvgQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("avg", "quantity", "avg_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest StandardDeviationQuantityOfOrderLines()
                    => StandardDeviationQuantityOfOrderLinesAs("standardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest StandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "quantity", "standardDeviation_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLines()
                    => SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs("squareRootOfPopulationStandardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "quantity", "squareRootOfPopulationStandardDeviation_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SampleVarianceQuantityOfOrderLines()
                    => SampleVarianceQuantityOfOrderLinesAs("sampleVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest SampleVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "quantity", "sampleVariance_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest SamplePopulationVarianceQuantityOfOrderLines()
                    => SamplePopulationVarianceQuantityOfOrderLinesAs("samplePopulationVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public CommercePlatformRequest SamplePopulationVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "quantity", "samplePopulationVariance_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }
                public CommercePlatformRequest CountOrderSearchPresets()
                    => CountOrderSearchPresetsAs("countOrderSearchPresets");

                public CommercePlatformRequest CountOrderSearchPresetsAs(string alias)
                    => CountOrderSearchPresetsWith(alias, new OrderSearchPresetRequest());

                public CommercePlatformRequest CountOrderSearchPresetsWith(string alias, OrderSearchPresetRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("OrderSearchPresetList", "OrderSearchPreset", "commerce_platform", alias, child.GetQuery(), true);
                    return this;
                }



        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var req = new QueryRequest(_query);
            var result = await service.QueryAsync(req);
            foreach (var facet in _query.Facets)
            {
                var membership = _query.Copy();
                membership.Facets.Clear();
                membership.Relations.Clear();
                membership.Orders.Clear();
                membership.Aggregates.Clear();
                membership.GroupFields.Clear();
                membership.Projections.Clear();
                membership.Project(facet.RelationName);
                var membershipRows = (await service.QueryAsync(new QueryRequest(membership))).Rows;
                var counts = membershipRows
                    .Where(row => row.TryGetValue(facet.RelationName, out var value) && value.Raw != null)
                    .GroupBy(row => Convert.ToString(row[facet.RelationName].Raw)!)
                    .ToDictionary(group => group.Key, group => group.Count());

                var nested = facet.Query.Copy();
                nested.Facets.Clear();
                var countAliases = nested.Aggregates
                    .Where(aggregate => aggregate.Function == AggregateFunction.Count)
                    .Select(aggregate => aggregate.Alias).ToArray();
                nested.Aggregates.Clear();
                nested.GroupFields.Clear();
                var facetRows = (await service.QueryAsync(new QueryRequest(nested))).Rows;
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

        private async Task<CommercePlatformPage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireResource<IDataService>();
            _query.Offset(offset);
            _query.Limit(limit);
            var result = await service.QueryAsync(new QueryRequest(_query));
            long totalCount;
            if (_query.IdSetPagination != null && context.IdSetCountAccuracy == "EXACT")
            {
                totalCount = checked((long)context.IdSetCount);
            }
            else
            {
                var countQuery = new SelectQuery("CommercePlatform");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.CommercePlatform>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.CommercePlatform.FromRecord(row, queryRoot));
            return new CommercePlatformPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.CommercePlatform> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            await foreach (var chunk in streaming.QueryStreamAsync(
                new QueryRequest(_query), chunkSize, cancellationToken).WithCancellation(cancellationToken))
            {
                var queryRoot = new EntityRoot();
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.CommercePlatform.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableCommercePlatformRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<CommercePlatformPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CommercePlatform>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCommercePlatformRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<CommercePlatformPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CommercePlatform>> executeForStream,
            Action limitOne,
            Action<string> comment,
            Action ensureIntent)
        {
            _executeForRows = executeForRows;
            _executeForPage = executeForPage;
            _executeForStream = executeForStream;
            _limitOne = limitOne;
            _comment = comment;
            _ensureIntent = ensureIntent;
        }

        public ExecutableCommercePlatformRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.CommercePlatform NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.CommercePlatform();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.CommercePlatform>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.CommercePlatform>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.CommercePlatform.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<CommercePlatformPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.CommercePlatform> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.CommercePlatform?> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class CommercePlatformPage
    {
        public SmartList<Generated.Models.CommercePlatform> Rows { get; }
        public long TotalCount { get; }
        public CommercePlatformPage(
            SmartList<Generated.Models.CommercePlatform> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}