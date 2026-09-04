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
    public class OrderStatusRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;

        public OrderStatusRequest()
        {
            _query = new SelectQuery("OrderStatus");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public OrderStatusRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public OrderStatusRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public OrderStatusRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderStatusRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderStatusRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
                c => Comment(c),
                EnsureIntent);
        }

        public OrderStatusRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public OrderStatusRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public OrderStatusRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public OrderStatusRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public OrderStatusRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public OrderStatusRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public OrderStatusRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public OrderStatusRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("code");
            _query.Project("color");
            _query.Project("display_order");
            _query.Project("commerce_platform");
            _query.Project("version");
            return this;
        }

                public OrderStatusRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public OrderStatusRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public OrderStatusRequest SelectCode()
                {
                    _query.Project("code");
                    return this;
                }

                public OrderStatusRequest SelectColor()
                {
                    _query.Project("color");
                    return this;
                }

                public OrderStatusRequest SelectDisplayOrder()
                {
                    _query.Project("display_order");
                    return this;
                }


                public OrderStatusRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public OrderStatusRequest SelectCommercePlatform()
                {
                    return SelectCommercePlatformWith(new CommercePlatformRequest());
                }

                public OrderStatusRequest SelectCommercePlatformWith(CommercePlatformRequest related)
                {
                    _query.Project("commerce_platform");
                    _query.ForwardRelationQuery("CommercePlatform", "CommercePlatform", "commerce_platform", related.GetQuery());
                    return this;
                }
                public OrderStatusRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public OrderStatusRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public OrderStatusRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public OrderStatusRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public OrderStatusRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public OrderStatusRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public OrderStatusRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public OrderStatusRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public OrderStatusRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public OrderStatusRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public OrderStatusRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
                    return this;
                }

                public OrderStatusRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public OrderStatusRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public OrderStatusRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }
                public OrderStatusRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public OrderStatusRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals));
                    return this;
                }

                public OrderStatusRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", lower, upper));
                    return this;
                }

                public OrderStatusRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public OrderStatusRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public OrderStatusRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public OrderStatusRequest WithCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq("code", val));
                    return this;
                }
                public OrderStatusRequest WithCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("code", vals));
                    return this;
                }

                public OrderStatusRequest WithCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("code", vals));
                    return this;
                }

                public OrderStatusRequest WithCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("code", lower, upper));
                    return this;
                }

                public OrderStatusRequest WithCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("code"));
                    return this;
                }

                public OrderStatusRequest WithCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("code"));
                    return this;
                }
                public OrderStatusRequest WithCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("code", val));
                    return this;
                }

                public OrderStatusRequest WithColorContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorIs(string val)
                {
                    _query.AndFilter(Expr.Eq("color", val));
                    return this;
                }
                public OrderStatusRequest WithColorIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("color", vals));
                    return this;
                }

                public OrderStatusRequest WithColorNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("color", vals));
                    return this;
                }

                public OrderStatusRequest WithColorGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("color", lower, upper));
                    return this;
                }

                public OrderStatusRequest WithColorIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("color"));
                    return this;
                }

                public OrderStatusRequest WithColorIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("color"));
                    return this;
                }
                public OrderStatusRequest WithColorNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("color", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIs(object val)
                {
                    _query.AndFilter(Expr.Eq("display_order", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("display_order", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("display_order", vals));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("display_order", vals));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("display_order", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("display_order", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("display_order", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("display_order", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("display_order", lower, upper));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("display_order"));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("display_order"));
                    return this;
                }

                public OrderStatusRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public OrderStatusRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public OrderStatusRequest WithCommercePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("commerce_platform"));
                    return this;
                }

                public OrderStatusRequest WithCommercePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("commerce_platform"));
                    return this;
                }

                public OrderStatusRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public OrderStatusRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public OrderStatusRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public OrderStatusRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public OrderStatusRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public OrderStatusRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public OrderStatusRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public OrderStatusRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public OrderStatusRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
                    return this;
                }

                public OrderStatusRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public OrderStatusRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public OrderStatusRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public OrderStatusRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public OrderStatusRequest OrderByCodeAscending()
                {
                    _query.OrderBy("code", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByCodeDescending()
                {
                    _query.OrderBy("code", "desc");
                    return this;
                }

                public OrderStatusRequest OrderByColorAscending()
                {
                    _query.OrderBy("color", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByColorDescending()
                {
                    _query.OrderBy("color", "desc");
                    return this;
                }

                public OrderStatusRequest OrderByDisplayOrderAscending()
                {
                    _query.OrderBy("display_order", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByDisplayOrderDescending()
                {
                    _query.OrderBy("display_order", "desc");
                    return this;
                }


                public OrderStatusRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public OrderStatusRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public OrderStatusRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public OrderStatusRequest MinDisplayOrder()
                {
                    return MinDisplayOrderAs("minOfDisplayOrder");
                }

                public OrderStatusRequest MinDisplayOrderAs(string retName)
                {
                    _query.Aggregate("min", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest MaxDisplayOrder()
                {
                    return MaxDisplayOrderAs("maxOfDisplayOrder");
                }

                public OrderStatusRequest MaxDisplayOrderAs(string retName)
                {
                    _query.Aggregate("max", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest SumDisplayOrder()
                {
                    return SumDisplayOrderAs("sumOfDisplayOrder");
                }

                public OrderStatusRequest SumDisplayOrderAs(string retName)
                {
                    _query.Aggregate("sum", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest AvgDisplayOrder()
                {
                    return AvgDisplayOrderAs("avgOfDisplayOrder");
                }

                public OrderStatusRequest AvgDisplayOrderAs(string retName)
                {
                    _query.Aggregate("avg", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest StandardDeviationDisplayOrder()
                {
                    return StandardDeviationDisplayOrderAs("standardDeviationOfDisplayOrder");
                }

                public OrderStatusRequest StandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest SquareRootOfPopulationStandardDeviationDisplayOrder()
                {
                    return SquareRootOfPopulationStandardDeviationDisplayOrderAs("squareRootOfPopulationStandardDeviationOfDisplayOrder");
                }

                public OrderStatusRequest SquareRootOfPopulationStandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest SampleVarianceDisplayOrder()
                {
                    return SampleVarianceDisplayOrderAs("sampleVarianceOfDisplayOrder");
                }

                public OrderStatusRequest SampleVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_samp", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest SamplePopulationVarianceDisplayOrder()
                {
                    return SamplePopulationVarianceDisplayOrderAs("samplePopulationVarianceOfDisplayOrder");
                }

                public OrderStatusRequest SamplePopulationVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_pop", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public OrderStatusRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public OrderStatusRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public OrderStatusRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public OrderStatusRequest GroupByCode()
                {
                    _query.GroupBy("code");
                    return this;
                }

                public OrderStatusRequest GroupByCodeAs(string retName)
                {
                    _query.GroupBy("code"); 
                    return this;
                }
                public OrderStatusRequest GroupByColor()
                {
                    _query.GroupBy("color");
                    return this;
                }

                public OrderStatusRequest GroupByColorAs(string retName)
                {
                    _query.GroupBy("color"); 
                    return this;
                }
                public OrderStatusRequest GroupByDisplayOrder()
                {
                    _query.GroupBy("display_order");
                    return this;
                }

                public OrderStatusRequest GroupByDisplayOrderAs(string retName)
                {
                    _query.GroupBy("display_order"); 
                    return this;
                }
                public OrderStatusRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public OrderStatusRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform"); 
                    return this;
                }
                public OrderStatusRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public OrderStatusRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public OrderStatusRequest SelectCustomerOrderList()
                {
                    return SelectCustomerOrderListWith(new CustomerOrderRequest());
                }

                public OrderStatusRequest SelectCustomerOrderListWith(CustomerOrderRequest child)
                {
                    _query.RelationQuery("CustomerOrderList", "CustomerOrder", "status", true, child.GetQuery());
                    return this;
                }
                public OrderStatusRequest HaveCustomerOrders()
                    => WithCustomerOrderListMatching(new CustomerOrderRequest());

                public OrderStatusRequest HaveNoCustomerOrders()
                    => WithoutCustomerOrderListMatching(new CustomerOrderRequest());

                public OrderStatusRequest WithCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("CustomerOrder"), child.GetQuery(), "status"));
                    return this;
                }

                public OrderStatusRequest WithoutCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("CustomerOrder"), child.GetQuery(), "status"));
                    return this;
                }
                public OrderStatusRequest CountCustomerOrders()
                    => CountCustomerOrdersAs("countCustomerOrders");

                public OrderStatusRequest CountCustomerOrdersAs(string alias)
                    => CountCustomerOrdersWith(alias, new CustomerOrderRequest());

                public OrderStatusRequest CountCustomerOrdersWith(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, child.GetQuery(), true);
                    return this;
                }

                public OrderStatusRequest MinTotalAmountOfCustomerOrders()
                    => MinTotalAmountOfCustomerOrdersAs("minOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest MinTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("min", "total_amount", "min_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, child.GetQuery(), true);
                    return this;
                }
                public OrderStatusRequest MaxTotalAmountOfCustomerOrders()
                    => MaxTotalAmountOfCustomerOrdersAs("maxOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest MaxTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("max", "total_amount", "max_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, child.GetQuery(), true);
                    return this;
                }
                public OrderStatusRequest SumTotalAmountOfCustomerOrders()
                    => SumTotalAmountOfCustomerOrdersAs("sumOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest SumTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("sum", "total_amount", "sum_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, child.GetQuery(), true);
                    return this;
                }
                public OrderStatusRequest AvgTotalAmountOfCustomerOrders()
                    => AvgTotalAmountOfCustomerOrdersAs("avgOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest AvgTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("avg", "total_amount", "avg_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, child.GetQuery(), true);
                    return this;
                }
                public OrderStatusRequest StandardDeviationTotalAmountOfCustomerOrders()
                    => StandardDeviationTotalAmountOfCustomerOrdersAs("standardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest StandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "total_amount", "standardDeviation_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, child.GetQuery(), true);
                    return this;
                }
                public OrderStatusRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrders()
                    => SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs("squareRootOfPopulationStandardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "total_amount", "squareRootOfPopulationStandardDeviation_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, child.GetQuery(), true);
                    return this;
                }
                public OrderStatusRequest SampleVarianceTotalAmountOfCustomerOrders()
                    => SampleVarianceTotalAmountOfCustomerOrdersAs("sampleVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest SampleVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "total_amount", "sampleVariance_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, child.GetQuery(), true);
                    return this;
                }
                public OrderStatusRequest SamplePopulationVarianceTotalAmountOfCustomerOrders()
                    => SamplePopulationVarianceTotalAmountOfCustomerOrdersAs("samplePopulationVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest SamplePopulationVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "total_amount", "samplePopulationVariance_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, child.GetQuery(), true);
                    return this;
                }
                public OrderStatusRequest FacetByCommercePlatformAs(
                    string name, CommercePlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "commerce_platform", request.GetQuery(), includeAllFacets));
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

        private async Task<OrderStatusPage> ExecuteForPageInternalAsync(
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
                var countQuery = new SelectQuery("OrderStatus");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.OrderStatus>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.OrderStatus.FromRecord(row, queryRoot));
            return new OrderStatusPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.OrderStatus> ExecuteForStreamInternalAsync(
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
                    yield return Generated.Models.OrderStatus.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableOrderStatusRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<OrderStatusPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderStatus>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableOrderStatusRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<OrderStatusPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderStatus>> executeForStream,
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

        public ExecutableOrderStatusRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.OrderStatus NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.OrderStatus();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.OrderStatus>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.OrderStatus>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.OrderStatus.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<OrderStatusPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.OrderStatus> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.OrderStatus?> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class OrderStatusPage
    {
        public SmartList<Generated.Models.OrderStatus> Rows { get; }
        public long TotalCount { get; }
        public OrderStatusPage(
            SmartList<Generated.Models.OrderStatus> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}