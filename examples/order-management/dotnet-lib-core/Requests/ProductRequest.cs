using System;
using System.Collections.Generic;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class ProductRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public ProductRequest()
        {
            _query = new SelectQuery("Product");
        }

        public SelectQuery GetQuery() => _query;

        public ProductRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableProductRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new InvalidOperationException("Purpose() requires a non-empty Comment() set earlier on the request");
            }
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableProductRequest(
                ExecuteForListInternalAsync, () => Limit(1));
        }

        public ProductRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public ProductRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

                public ProductRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public ProductRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public ProductRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public ProductRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public ProductRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public ProductRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }

                public ProductRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public ProductRequest WithSkuContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("sku", val));
                    return this;
                }

                public ProductRequest WithSkuIs(string val)
                {
                    _query.AndFilter(Expr.Eq("sku", val));
                    return this;
                }

                public ProductRequest WithSkuIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("sku", vals));
                    return this;
                }

                public ProductRequest WithImageUrlContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlIs(string val)
                {
                    _query.AndFilter(Expr.Eq("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("image_url", vals));
                    return this;
                }

                public ProductRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public ProductRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public ProductRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public ProductRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public ProductRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public ProductRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public ProductRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public ProductRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public ProductRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public ProductRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public ProductRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public ProductRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public ProductRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public ProductRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public ProductRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public ProductRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public ProductRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public ProductRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public ProductRequest OrderBySkuAscending()
                {
                    _query.OrderBy("sku", "asc");
                    return this;
                }

                public ProductRequest OrderBySkuDescending()
                {
                    _query.OrderBy("sku", "desc");
                    return this;
                }

                public ProductRequest OrderByImageUrlAscending()
                {
                    _query.OrderBy("image_url", "asc");
                    return this;
                }

                public ProductRequest OrderByImageUrlDescending()
                {
                    _query.OrderBy("image_url", "desc");
                    return this;
                }


                public ProductRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public ProductRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public ProductRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public ProductRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public ProductRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public ProductRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public ProductRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public ProductRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public ProductRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public ProductRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public ProductRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public ProductRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public ProductRequest GroupBySku()
                {
                    _query.GroupBy("sku");
                    return this;
                }

                public ProductRequest GroupBySkuAs(string retName)
                {
                    _query.GroupBy("sku"); 
                    return this;
                }
                public ProductRequest GroupByImageUrl()
                {
                    _query.GroupBy("image_url");
                    return this;
                }

                public ProductRequest GroupByImageUrlAs(string retName)
                {
                    _query.GroupBy("image_url"); 
                    return this;
                }
                public ProductRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public ProductRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform"); 
                    return this;
                }
                public ProductRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public ProductRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public ProductRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public ProductRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
                    return this;
                }
                public ProductRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public ProductRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public ProductRequest SelectOrderLineList()
                {
                    return SelectOrderLineListWith(new OrderLineRequest());
                }

                public ProductRequest SelectOrderLineListWith(OrderLineRequest child)
                {
                    _query.RelationQuery("OrderLineList", "OrderLine", "product", true, child.GetQuery());
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

    public sealed class ExecutableProductRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForList;
        private readonly Action _limitOne;

        internal ExecutableProductRequest(
            Func<UserContext, Task<QueryResult>> executeForList,
            Action limitOne)
        {
            _executeForList = executeForList;
            _limitOne = limitOne;
        }

        public Generated.Models.Product NewEntity(UserContext context)
            => new Generated.Models.Product();

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

        public async Task<List<Generated.Models.Product>> ExecuteEntitiesForListAsync(
            UserContext context)
        {
            var result = await ExecuteForListAsync(context);
            var entities = new List<Generated.Models.Product>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.Product.FromRecord(row));
            return entities;
        }

        public async Task<Generated.Models.Product> ExecuteEntityForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteEntitiesForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }
}