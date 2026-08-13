using System;
using System.Collections.Generic;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class CustomerOrderRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public CustomerOrderRequest()
        {
            _query = new SelectQuery("CustomerOrder");
        }

        public SelectQuery GetQuery() => _query;

        public CustomerOrderRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCustomerOrderRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new InvalidOperationException("Purpose() requires a non-empty Comment() set earlier on the request");
            }
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerOrderRequest(
                ExecuteForListInternalAsync, () => Limit(1));
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

                public CustomerOrderRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public CustomerOrderRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public CustomerOrderRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public CustomerOrderRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
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

                public CustomerOrderRequest WithOrderNumberIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("order_number", vals));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIs(object val)
                {
                    _query.AndFilter(Expr.Eq("order_date", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("order_date", vals));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("order_date", val));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("order_date", val));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIs(object val)
                {
                    _query.AndFilter(Expr.Eq("total_amount", val));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("total_amount", vals));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("total_amount", val));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("total_amount", val));
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

                public CustomerOrderRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public CustomerOrderRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public CustomerOrderRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public CustomerOrderRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public CustomerOrderRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
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

                public CustomerOrderRequest minTotalAmount()
                {
                    return minTotalAmountAs("minOfTotalAmount");
                }

                public CustomerOrderRequest minTotalAmountAs(string retName)
                {
                    _query.Aggregate("min", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest maxTotalAmount()
                {
                    return maxTotalAmountAs("maxOfTotalAmount");
                }

                public CustomerOrderRequest maxTotalAmountAs(string retName)
                {
                    _query.Aggregate("max", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest sumTotalAmount()
                {
                    return sumTotalAmountAs("sumOfTotalAmount");
                }

                public CustomerOrderRequest sumTotalAmountAs(string retName)
                {
                    _query.Aggregate("sum", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest avgTotalAmount()
                {
                    return avgTotalAmountAs("avgOfTotalAmount");
                }

                public CustomerOrderRequest avgTotalAmountAs(string retName)
                {
                    _query.Aggregate("avg", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest standardDeviationTotalAmount()
                {
                    return standardDeviationTotalAmountAs("standardDeviationOfTotalAmount");
                }

                public CustomerOrderRequest standardDeviationTotalAmountAs(string retName)
                {
                    _query.Aggregate("stddev", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest squareRootOfPopulationStandardDeviationTotalAmount()
                {
                    return squareRootOfPopulationStandardDeviationTotalAmountAs("squareRootOfPopulationStandardDeviationOfTotalAmount");
                }

                public CustomerOrderRequest squareRootOfPopulationStandardDeviationTotalAmountAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest sampleVarianceTotalAmount()
                {
                    return sampleVarianceTotalAmountAs("sampleVarianceOfTotalAmount");
                }

                public CustomerOrderRequest sampleVarianceTotalAmountAs(string retName)
                {
                    _query.Aggregate("var_samp", "total_amount", retName);
                    return this;
                }
                public CustomerOrderRequest samplePopulationVarianceTotalAmount()
                {
                    return samplePopulationVarianceTotalAmountAs("samplePopulationVarianceOfTotalAmount");
                }

                public CustomerOrderRequest samplePopulationVarianceTotalAmountAs(string retName)
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

        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext ctx)
        {
            if (string.IsNullOrEmpty(_purpose) || string.IsNullOrEmpty(_comment))
            {
                throw new Exception("Security audit failure: Comment() and Purpose() must be called before ExecuteForListAsync()");
            }
            var service = ctx.RequireDataService();
            var req = new QueryRequest(_query);
            return await service.QueryAsync(ctx, req);
        }

    }

    public sealed class ExecutableCustomerOrderRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForList;
        private readonly Action _limitOne;

        internal ExecutableCustomerOrderRequest(
            Func<UserContext, Task<QueryResult>> executeForList,
            Action limitOne)
        {
            _executeForList = executeForList;
            _limitOne = limitOne;
        }

        public Generated.Models.CustomerOrder NewEntity(UserContext ctx)
            => new Generated.Models.CustomerOrder();

        public Task<QueryResult> ExecuteForListAsync(UserContext ctx)
            => _executeForList(ctx);

        public async Task<Record> ExecuteForOneAsync(UserContext ctx)
        {
            _limitOne();
            var res = await ExecuteForListAsync(ctx);
            if (res.Rows != null && res.Rows.Count > 0)
            {
                return res.Rows[0];
            }
            return null;
        }

        public async Task<List<Generated.Models.CustomerOrder>> ExecuteEntitiesForListAsync(
            UserContext ctx)
        {
            var result = await ExecuteForListAsync(ctx);
            var entities = new List<Generated.Models.CustomerOrder>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.CustomerOrder.FromRecord(row));
            return entities;
        }

        public async Task<Generated.Models.CustomerOrder> ExecuteEntityForOneAsync(
            UserContext ctx)
        {
            _limitOne();
            var entities = await ExecuteEntitiesForListAsync(ctx);
            return entities.Count > 0 ? entities[0] : null;
        }
    }
}