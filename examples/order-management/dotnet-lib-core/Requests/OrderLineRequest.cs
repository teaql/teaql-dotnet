using System;
using System.Collections.Generic;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class OrderLineRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public OrderLineRequest()
        {
            _query = new SelectQuery("OrderLine");
        }

        public SelectQuery GetQuery() => _query;

        public OrderLineRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderLineRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new InvalidOperationException("Purpose() requires a non-empty Comment() set earlier on the request");
            }
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderLineRequest(
                ExecuteForListInternalAsync, () => Limit(1));
        }

        public OrderLineRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public OrderLineRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

                public OrderLineRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public OrderLineRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public OrderLineRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public OrderLineRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public OrderLineRequest FilterByCustomerOrder(object val)
                {
                    _query.AndFilter(Expr.Eq("customer_order", val));
                    return this;
                }

                public OrderLineRequest FilterByCustomerOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("customer_order", vals));
                    return this;
                }

                public OrderLineRequest FilterByProduct(object val)
                {
                    _query.AndFilter(Expr.Eq("product", val));
                    return this;
                }

                public OrderLineRequest FilterByProductIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("product", vals));
                    return this;
                }

                public OrderLineRequest WithProductNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("product_name", vals));
                    return this;
                }

                public OrderLineRequest WithSkuContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuIs(string val)
                {
                    _query.AndFilter(Expr.Eq("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("sku", vals));
                    return this;
                }

                public OrderLineRequest WithQuantityIs(object val)
                {
                    _query.AndFilter(Expr.Eq("quantity", val));
                    return this;
                }

                public OrderLineRequest WithQuantityIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("quantity", vals));
                    return this;
                }

                public OrderLineRequest WithQuantityGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("quantity", val));
                    return this;
                }

                public OrderLineRequest WithQuantityLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("quantity", val));
                    return this;
                }

                public OrderLineRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public OrderLineRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public OrderLineRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public OrderLineRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public OrderLineRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public OrderLineRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public OrderLineRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public OrderLineRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public OrderLineRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public OrderLineRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }



                public OrderLineRequest OrderByProductNameAscending()
                {
                    _query.OrderBy("product_name", "asc");
                    return this;
                }

                public OrderLineRequest OrderByProductNameDescending()
                {
                    _query.OrderBy("product_name", "desc");
                    return this;
                }

                public OrderLineRequest OrderBySkuAscending()
                {
                    _query.OrderBy("sku", "asc");
                    return this;
                }

                public OrderLineRequest OrderBySkuDescending()
                {
                    _query.OrderBy("sku", "desc");
                    return this;
                }

                public OrderLineRequest OrderByQuantityAscending()
                {
                    _query.OrderBy("quantity", "asc");
                    return this;
                }

                public OrderLineRequest OrderByQuantityDescending()
                {
                    _query.OrderBy("quantity", "desc");
                    return this;
                }


                public OrderLineRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public OrderLineRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public OrderLineRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public OrderLineRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public OrderLineRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public OrderLineRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public OrderLineRequest minQuantity()
                {
                    return minQuantityAs("minOfQuantity");
                }

                public OrderLineRequest minQuantityAs(string retName)
                {
                    _query.Aggregate("min", "quantity", retName);
                    return this;
                }
                public OrderLineRequest maxQuantity()
                {
                    return maxQuantityAs("maxOfQuantity");
                }

                public OrderLineRequest maxQuantityAs(string retName)
                {
                    _query.Aggregate("max", "quantity", retName);
                    return this;
                }
                public OrderLineRequest sumQuantity()
                {
                    return sumQuantityAs("sumOfQuantity");
                }

                public OrderLineRequest sumQuantityAs(string retName)
                {
                    _query.Aggregate("sum", "quantity", retName);
                    return this;
                }
                public OrderLineRequest avgQuantity()
                {
                    return avgQuantityAs("avgOfQuantity");
                }

                public OrderLineRequest avgQuantityAs(string retName)
                {
                    _query.Aggregate("avg", "quantity", retName);
                    return this;
                }
                public OrderLineRequest standardDeviationQuantity()
                {
                    return standardDeviationQuantityAs("standardDeviationOfQuantity");
                }

                public OrderLineRequest standardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev", "quantity", retName);
                    return this;
                }
                public OrderLineRequest squareRootOfPopulationStandardDeviationQuantity()
                {
                    return squareRootOfPopulationStandardDeviationQuantityAs("squareRootOfPopulationStandardDeviationOfQuantity");
                }

                public OrderLineRequest squareRootOfPopulationStandardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "quantity", retName);
                    return this;
                }
                public OrderLineRequest sampleVarianceQuantity()
                {
                    return sampleVarianceQuantityAs("sampleVarianceOfQuantity");
                }

                public OrderLineRequest sampleVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_samp", "quantity", retName);
                    return this;
                }
                public OrderLineRequest samplePopulationVarianceQuantity()
                {
                    return samplePopulationVarianceQuantityAs("samplePopulationVarianceOfQuantity");
                }

                public OrderLineRequest samplePopulationVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_pop", "quantity", retName);
                    return this;
                }
                public OrderLineRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public OrderLineRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public OrderLineRequest GroupByCustomerOrder()
                {
                    _query.GroupBy("customer_order");
                    return this;
                }

                public OrderLineRequest GroupByCustomerOrderAs(string retName)
                {
                    _query.GroupBy("customer_order"); 
                    return this;
                }
                public OrderLineRequest GroupByProduct()
                {
                    _query.GroupBy("product");
                    return this;
                }

                public OrderLineRequest GroupByProductAs(string retName)
                {
                    _query.GroupBy("product"); 
                    return this;
                }
                public OrderLineRequest GroupByProductName()
                {
                    _query.GroupBy("product_name");
                    return this;
                }

                public OrderLineRequest GroupByProductNameAs(string retName)
                {
                    _query.GroupBy("product_name"); 
                    return this;
                }
                public OrderLineRequest GroupBySku()
                {
                    _query.GroupBy("sku");
                    return this;
                }

                public OrderLineRequest GroupBySkuAs(string retName)
                {
                    _query.GroupBy("sku"); 
                    return this;
                }
                public OrderLineRequest GroupByQuantity()
                {
                    _query.GroupBy("quantity");
                    return this;
                }

                public OrderLineRequest GroupByQuantityAs(string retName)
                {
                    _query.GroupBy("quantity"); 
                    return this;
                }
                public OrderLineRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public OrderLineRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform"); 
                    return this;
                }
                public OrderLineRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public OrderLineRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public OrderLineRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public OrderLineRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }

        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            if (string.IsNullOrEmpty(_purpose) || string.IsNullOrEmpty(_comment))
            {
                throw new Exception("Security audit failure: Comment() and Purpose() must be called before ExecuteForListAsync()");
            }
            var service = context.RequireDataService();
            var req = new QueryRequest(_query);
            return await service.QueryAsync(context, req);
        }

    }

    public sealed class ExecutableOrderLineRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForList;
        private readonly Action _limitOne;

        internal ExecutableOrderLineRequest(
            Func<UserContext, Task<QueryResult>> executeForList,
            Action limitOne)
        {
            _executeForList = executeForList;
            _limitOne = limitOne;
        }

        public Generated.Models.OrderLine NewEntity(UserContext context)
            => new Generated.Models.OrderLine();

        public Task<QueryResult> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public async Task<Record> ExecuteForOneAsync(UserContext context)
        {
            _limitOne();
            var res = await ExecuteForListAsync(context);
            if (res.Rows != null && res.Rows.Count > 0)
            {
                return res.Rows[0];
            }
            return null;
        }

        public async Task<List<Generated.Models.OrderLine>> ExecuteEntitiesForListAsync(
            UserContext context)
        {
            var result = await ExecuteForListAsync(context);
            var entities = new List<Generated.Models.OrderLine>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.OrderLine.FromRecord(row));
            return entities;
        }

        public async Task<Generated.Models.OrderLine> ExecuteEntityForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteEntitiesForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }
}