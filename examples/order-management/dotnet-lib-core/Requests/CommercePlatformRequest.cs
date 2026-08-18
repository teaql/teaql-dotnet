using System;
using System.Collections.Generic;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class CommercePlatformRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public CommercePlatformRequest()
        {
            _query = new SelectQuery("CommercePlatform");
        }

        public SelectQuery GetQuery() => _query;

        public CommercePlatformRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCommercePlatformRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new InvalidOperationException("Purpose() requires a non-empty Comment() set earlier on the request");
            }
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCommercePlatformRequest(
                ExecuteForListInternalAsync, () => Limit(1));
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

                public CommercePlatformRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public CommercePlatformRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public CommercePlatformRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public CommercePlatformRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
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

                public CommercePlatformRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public CommercePlatformRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public CommercePlatformRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public CommercePlatformRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public CommercePlatformRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public CommercePlatformRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public CommercePlatformRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
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

    public sealed class ExecutableCommercePlatformRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForList;
        private readonly Action _limitOne;

        internal ExecutableCommercePlatformRequest(
            Func<UserContext, Task<QueryResult>> executeForList,
            Action limitOne)
        {
            _executeForList = executeForList;
            _limitOne = limitOne;
        }

        public Generated.Models.CommercePlatform NewEntity(UserContext context)
            => new Generated.Models.CommercePlatform();

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

        public async Task<List<Generated.Models.CommercePlatform>> ExecuteEntitiesForListAsync(
            UserContext context)
        {
            var result = await ExecuteForListAsync(context);
            var entities = new List<Generated.Models.CommercePlatform>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.CommercePlatform.FromRecord(row));
            return entities;
        }

        public async Task<Generated.Models.CommercePlatform> ExecuteEntityForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteEntitiesForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }
}