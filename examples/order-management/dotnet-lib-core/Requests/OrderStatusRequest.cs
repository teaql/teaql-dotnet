using System;
using System.Collections.Generic;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class OrderStatusRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public OrderStatusRequest()
        {
            _query = new SelectQuery("OrderStatus");
        }

        public SelectQuery GetQuery() => _query;

        public OrderStatusRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderStatusRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new InvalidOperationException("Purpose() requires a non-empty Comment() set earlier on the request");
            }
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderStatusRequest(
                ExecuteForListInternalAsync, () => Limit(1));
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

                public OrderStatusRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public OrderStatusRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public OrderStatusRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public OrderStatusRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
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

                public OrderStatusRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
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

                public OrderStatusRequest WithCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("code", vals));
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

                public OrderStatusRequest WithColorIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("color", vals));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIs(object val)
                {
                    _query.AndFilter(Expr.Eq("display_order", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("display_order", vals));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("display_order", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("display_order", val));
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

                public OrderStatusRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public OrderStatusRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public OrderStatusRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public OrderStatusRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
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

                public OrderStatusRequest minDisplayOrder()
                {
                    return minDisplayOrderAs("minOfDisplayOrder");
                }

                public OrderStatusRequest minDisplayOrderAs(string retName)
                {
                    _query.Aggregate("min", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest maxDisplayOrder()
                {
                    return maxDisplayOrderAs("maxOfDisplayOrder");
                }

                public OrderStatusRequest maxDisplayOrderAs(string retName)
                {
                    _query.Aggregate("max", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest sumDisplayOrder()
                {
                    return sumDisplayOrderAs("sumOfDisplayOrder");
                }

                public OrderStatusRequest sumDisplayOrderAs(string retName)
                {
                    _query.Aggregate("sum", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest avgDisplayOrder()
                {
                    return avgDisplayOrderAs("avgOfDisplayOrder");
                }

                public OrderStatusRequest avgDisplayOrderAs(string retName)
                {
                    _query.Aggregate("avg", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest standardDeviationDisplayOrder()
                {
                    return standardDeviationDisplayOrderAs("standardDeviationOfDisplayOrder");
                }

                public OrderStatusRequest standardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest squareRootOfPopulationStandardDeviationDisplayOrder()
                {
                    return squareRootOfPopulationStandardDeviationDisplayOrderAs("squareRootOfPopulationStandardDeviationOfDisplayOrder");
                }

                public OrderStatusRequest squareRootOfPopulationStandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest sampleVarianceDisplayOrder()
                {
                    return sampleVarianceDisplayOrderAs("sampleVarianceOfDisplayOrder");
                }

                public OrderStatusRequest sampleVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_samp", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest samplePopulationVarianceDisplayOrder()
                {
                    return samplePopulationVarianceDisplayOrderAs("samplePopulationVarianceOfDisplayOrder");
                }

                public OrderStatusRequest samplePopulationVarianceDisplayOrderAs(string retName)
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

    public sealed class ExecutableOrderStatusRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForList;
        private readonly Action _limitOne;

        internal ExecutableOrderStatusRequest(
            Func<UserContext, Task<QueryResult>> executeForList,
            Action limitOne)
        {
            _executeForList = executeForList;
            _limitOne = limitOne;
        }

        public Generated.Models.OrderStatus NewEntity(UserContext ctx)
            => new Generated.Models.OrderStatus();

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

        public async Task<List<Generated.Models.OrderStatus>> ExecuteEntitiesForListAsync(
            UserContext ctx)
        {
            var result = await ExecuteForListAsync(ctx);
            var entities = new List<Generated.Models.OrderStatus>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.OrderStatus.FromRecord(row));
            return entities;
        }

        public async Task<Generated.Models.OrderStatus> ExecuteEntityForOneAsync(
            UserContext ctx)
        {
            _limitOne();
            var entities = await ExecuteEntitiesForListAsync(ctx);
            return entities.Count > 0 ? entities[0] : null;
        }
    }
}