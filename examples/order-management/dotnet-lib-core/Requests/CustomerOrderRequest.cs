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
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerOrderRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
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
            _query.Project("order_number");
            _query.Project("order_date");
            _query.Project("total_amount");
            _query.Project("status");
            _query.Project("customer");
            _query.Project("commerce_platform");
            _query.Project("create_time");
            _query.Project("update_time");
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

                public CustomerOrderRequest SelectOrderDate()
                {
                    _query.Project("order_date");
                    return this;
                }

                public CustomerOrderRequest SelectTotalAmount()
                {
                    _query.Project("total_amount");
                    return this;
                }




                public CustomerOrderRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public CustomerOrderRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public CustomerOrderRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public CustomerOrderRequest SelectStatus()
                {
                    return SelectStatusWith(new OrderStatusRequest());
                }

                public CustomerOrderRequest SelectStatusWith(OrderStatusRequest related)
                {
                    _query.Project("status");
                    _query.ForwardRelationQuery("Status", "OrderStatus", "status", related.GetQuery());
                    return this;
                }
                public CustomerOrderRequest SelectCustomer()
                {
                    return SelectCustomerWith(new CustomerRequest());
                }

                public CustomerOrderRequest SelectCustomerWith(CustomerRequest related)
                {
                    _query.Project("customer");
                    _query.ForwardRelationQuery("Customer", "Customer", "customer", related.GetQuery());
                    return this;
                }
                public CustomerOrderRequest SelectCommercePlatform()
                {
                    return SelectCommercePlatformWith(new CommercePlatformRequest());
                }

                public CustomerOrderRequest SelectCommercePlatformWith(CommercePlatformRequest related)
                {
                    _query.Project("commerce_platform");
                    _query.ForwardRelationQuery("CommercePlatform", "CommercePlatform", "commerce_platform", related.GetQuery());
                    return this;
                }
                public CustomerOrderRequest WithStatusMatching(OrderStatusRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("status", EntityDescriptor.New("OrderStatus"), related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithoutStatusMatching(OrderStatusRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("status", EntityDescriptor.New("OrderStatus"), related.GetQuery(), "id"));
                    return this;
                }
                public CustomerOrderRequest WithCustomerMatching(CustomerRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("customer", EntityDescriptor.New("Customer"), related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithoutCustomerMatching(CustomerRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("customer", EntityDescriptor.New("Customer"), related.GetQuery(), "id"));
                    return this;
                }
                public CustomerOrderRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public CustomerOrderRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public CustomerOrderRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public CustomerOrderRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public CustomerOrderRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public CustomerOrderRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public CustomerOrderRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public CustomerOrderRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public CustomerOrderRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
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

                public CustomerOrderRequest WithOrderNumberContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIs(string val)
                {
                    _query.AndFilter(Expr.Eq("order_number", val));
                    return this;
                }
                public CustomerOrderRequest WithOrderNumberIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("order_number", vals));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("order_number", vals));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("order_number", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("order_number", lower, upper));
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

                public CustomerOrderRequest WithOrderDateIs(object val)
                {
                    _query.AndFilter(Expr.Eq("order_date", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("order_date", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("order_date", vals));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("order_date", vals));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("order_date", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("order_date", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("order_date", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("order_date", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("order_date", lower, upper));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("order_date"));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("order_date"));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIs(object val)
                {
                    _query.AndFilter(Expr.Eq("total_amount", val));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("total_amount", val));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("total_amount", vals));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("total_amount", vals));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("total_amount", val));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("total_amount", val));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("total_amount", val));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("total_amount", val));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("total_amount", lower, upper));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("total_amount"));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("total_amount"));
                    return this;
                }

                public CustomerOrderRequest FilterByStatus(object val)
                {
                    _query.AndFilter(Expr.Eq("status", val));
                    return this;
                }

                public CustomerOrderRequest FilterByStatusIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("status", vals));
                    return this;
                }

                public CustomerOrderRequest WithStatusIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("status"));
                    return this;
                }

                public CustomerOrderRequest WithStatusIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("status"));
                    return this;
                }
                public CustomerOrderRequest WithStatusIsPending()
                {
                    _query.AndFilter(Expr.Eq("status", 1001L));
                    return this;
                }

                public CustomerOrderRequest WithStatusIsConfirmed()
                {
                    _query.AndFilter(Expr.Eq("status", 1002L));
                    return this;
                }


                public CustomerOrderRequest FilterByCustomer(object val)
                {
                    _query.AndFilter(Expr.Eq("customer", val));
                    return this;
                }

                public CustomerOrderRequest FilterByCustomerIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("customer", vals));
                    return this;
                }

                public CustomerOrderRequest WithCustomerIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("customer"));
                    return this;
                }

                public CustomerOrderRequest WithCustomerIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("customer"));
                    return this;
                }

                public CustomerOrderRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public CustomerOrderRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public CustomerOrderRequest WithCommercePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("commerce_platform"));
                    return this;
                }

                public CustomerOrderRequest WithCommercePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("commerce_platform"));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", val));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", val));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", val));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", lower, upper));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("update_time", val));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("update_time", val));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("update_time", val));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("update_time", lower, upper));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public CustomerOrderRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public CustomerOrderRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public CustomerOrderRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
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

                public CustomerOrderRequest OrderByOrderDateAscending()
                {
                    _query.OrderBy("order_date", "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByOrderDateDescending()
                {
                    _query.OrderBy("order_date", "desc");
                    return this;
                }

                public CustomerOrderRequest OrderByTotalAmountAscending()
                {
                    _query.OrderBy("total_amount", "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByTotalAmountDescending()
                {
                    _query.OrderBy("total_amount", "desc");
                    return this;
                }




                public CustomerOrderRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public CustomerOrderRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
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

                public CustomerOrderRequest MinTotalAmount()
                {
                    return MinTotalAmountAs("minOfTotalAmount");
                }

                public CustomerOrderRequest MinTotalAmountAs(string retName)
                {
                    _query.Aggregate("min", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest MaxTotalAmount()
                {
                    return MaxTotalAmountAs("maxOfTotalAmount");
                }

                public CustomerOrderRequest MaxTotalAmountAs(string retName)
                {
                    _query.Aggregate("max", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest SumTotalAmount()
                {
                    return SumTotalAmountAs("sumOfTotalAmount");
                }

                public CustomerOrderRequest SumTotalAmountAs(string retName)
                {
                    _query.Aggregate("sum", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest AvgTotalAmount()
                {
                    return AvgTotalAmountAs("avgOfTotalAmount");
                }

                public CustomerOrderRequest AvgTotalAmountAs(string retName)
                {
                    _query.Aggregate("avg", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest StandardDeviationTotalAmount()
                {
                    return StandardDeviationTotalAmountAs("standardDeviationOfTotalAmount");
                }

                public CustomerOrderRequest StandardDeviationTotalAmountAs(string retName)
                {
                    _query.Aggregate("stddev", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationTotalAmount()
                {
                    return SquareRootOfPopulationStandardDeviationTotalAmountAs("squareRootOfPopulationStandardDeviationOfTotalAmount");
                }

                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationTotalAmountAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest SampleVarianceTotalAmount()
                {
                    return SampleVarianceTotalAmountAs("sampleVarianceOfTotalAmount");
                }

                public CustomerOrderRequest SampleVarianceTotalAmountAs(string retName)
                {
                    _query.Aggregate("var_samp", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest SamplePopulationVarianceTotalAmount()
                {
                    return SamplePopulationVarianceTotalAmountAs("samplePopulationVarianceOfTotalAmount");
                }

                public CustomerOrderRequest SamplePopulationVarianceTotalAmountAs(string retName)
                {
                    _query.Aggregate("var_pop", "total_amount", retName);
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
                public CustomerOrderRequest GroupByOrderDate()
                {
                    _query.GroupBy("order_date");
                    return this;
                }

                public CustomerOrderRequest GroupByOrderDateAs(string retName)
                {
                    _query.GroupBy("order_date"); 
                    return this;
                }
                public CustomerOrderRequest GroupByTotalAmount()
                {
                    _query.GroupBy("total_amount");
                    return this;
                }

                public CustomerOrderRequest GroupByTotalAmountAs(string retName)
                {
                    _query.GroupBy("total_amount"); 
                    return this;
                }
                public CustomerOrderRequest GroupByStatus()
                {
                    _query.GroupBy("status");
                    return this;
                }

                public CustomerOrderRequest GroupByStatusAs(string retName)
                {
                    _query.GroupBy("status"); 
                    return this;
                }
                public CustomerOrderRequest GroupByCustomer()
                {
                    _query.GroupBy("customer");
                    return this;
                }

                public CustomerOrderRequest GroupByCustomerAs(string retName)
                {
                    _query.GroupBy("customer"); 
                    return this;
                }
                public CustomerOrderRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public CustomerOrderRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform"); 
                    return this;
                }
                public CustomerOrderRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public CustomerOrderRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public CustomerOrderRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public CustomerOrderRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
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
                public CustomerOrderRequest SelectOrderLineList()
                {
                    return SelectOrderLineListWith(new OrderLineRequest());
                }

                public CustomerOrderRequest SelectOrderLineListWith(OrderLineRequest child)
                {
                    _query.RelationQuery("OrderLineList", "OrderLine", "customer_order", true, child.GetQuery());
                    return this;
                }
                public CustomerOrderRequest HaveOrderLines()
                    => WithOrderLineListMatching(new OrderLineRequest());

                public CustomerOrderRequest HaveNoOrderLines()
                    => WithoutOrderLineListMatching(new OrderLineRequest());

                public CustomerOrderRequest WithOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("OrderLine"), child.GetQuery(), "customer_order"));
                    return this;
                }

                public CustomerOrderRequest WithoutOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("OrderLine"), child.GetQuery(), "customer_order"));
                    return this;
                }
                public CustomerOrderRequest CountOrderLines()
                    => CountOrderLinesAs("countOrderLines");

                public CustomerOrderRequest CountOrderLinesAs(string alias)
                    => CountOrderLinesWith(alias, new OrderLineRequest());

                public CustomerOrderRequest CountOrderLinesWith(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }

                public CustomerOrderRequest MinQuantityOfOrderLines()
                    => MinQuantityOfOrderLinesAs("minOfQuantityOfOrderLines", new OrderLineRequest());

                public CustomerOrderRequest MinQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("min", "quantity", "min_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerOrderRequest MaxQuantityOfOrderLines()
                    => MaxQuantityOfOrderLinesAs("maxOfQuantityOfOrderLines", new OrderLineRequest());

                public CustomerOrderRequest MaxQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("max", "quantity", "max_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerOrderRequest SumQuantityOfOrderLines()
                    => SumQuantityOfOrderLinesAs("sumOfQuantityOfOrderLines", new OrderLineRequest());

                public CustomerOrderRequest SumQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("sum", "quantity", "sum_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerOrderRequest AvgQuantityOfOrderLines()
                    => AvgQuantityOfOrderLinesAs("avgOfQuantityOfOrderLines", new OrderLineRequest());

                public CustomerOrderRequest AvgQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("avg", "quantity", "avg_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerOrderRequest StandardDeviationQuantityOfOrderLines()
                    => StandardDeviationQuantityOfOrderLinesAs("standardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public CustomerOrderRequest StandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "quantity", "standardDeviation_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLines()
                    => SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs("squareRootOfPopulationStandardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "quantity", "squareRootOfPopulationStandardDeviation_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerOrderRequest SampleVarianceQuantityOfOrderLines()
                    => SampleVarianceQuantityOfOrderLinesAs("sampleVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public CustomerOrderRequest SampleVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "quantity", "sampleVariance_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerOrderRequest SamplePopulationVarianceQuantityOfOrderLines()
                    => SamplePopulationVarianceQuantityOfOrderLinesAs("samplePopulationVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public CustomerOrderRequest SamplePopulationVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "quantity", "samplePopulationVariance_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "customer_order", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerOrderRequest FacetByStatusAs(
                    string name, OrderStatusRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "status", request.GetQuery(), includeAllFacets));
                    return this;
                }

                public CustomerOrderRequest FacetByCustomerAs(
                    string name, CustomerRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "customer", request.GetQuery(), includeAllFacets));
                    return this;
                }

                public CustomerOrderRequest FacetByCommercePlatformAs(
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

        private async Task<CustomerOrderPage> ExecuteForPageInternalAsync(
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
                var countQuery = new SelectQuery("CustomerOrder");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.CustomerOrder>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.CustomerOrder.FromRecord(row, queryRoot));
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
            await foreach (var chunk in streaming.QueryStreamAsync(
                new QueryRequest(_query), chunkSize, cancellationToken).WithCancellation(cancellationToken))
            {
                var queryRoot = new EntityRoot();
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.CustomerOrder.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableCustomerOrderRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<CustomerOrderPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerOrder>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCustomerOrderRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<CustomerOrderPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerOrder>> executeForStream,
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
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.CustomerOrder.FromRecord(row, queryRoot));
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
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
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