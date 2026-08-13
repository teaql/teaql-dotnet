using System;
using System.Collections.Generic;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class CustomerRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public CustomerRequest()
        {
            _query = new SelectQuery("Customer");
        }

        public SelectQuery GetQuery() => _query;

        public CustomerRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCustomerRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new InvalidOperationException("Purpose() requires a non-empty Comment() set earlier on the request");
            }
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerRequest(
                ExecuteForListInternalAsync, () => Limit(1));
        }

        public CustomerRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public CustomerRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

                public CustomerRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public CustomerRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public CustomerRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public CustomerRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public CustomerRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public CustomerRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }

                public CustomerRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public CustomerRequest WithEmailContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("email", val));
                    return this;
                }

                public CustomerRequest WithEmailIs(string val)
                {
                    _query.AndFilter(Expr.Eq("email", val));
                    return this;
                }

                public CustomerRequest WithEmailIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("email", vals));
                    return this;
                }

                public CustomerRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public CustomerRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public CustomerRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public CustomerRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public CustomerRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public CustomerRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public CustomerRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public CustomerRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public CustomerRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public CustomerRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public CustomerRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public CustomerRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public CustomerRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public CustomerRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public CustomerRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public CustomerRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public CustomerRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public CustomerRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public CustomerRequest OrderByEmailAscending()
                {
                    _query.OrderBy("email", "asc");
                    return this;
                }

                public CustomerRequest OrderByEmailDescending()
                {
                    _query.OrderBy("email", "desc");
                    return this;
                }


                public CustomerRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public CustomerRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public CustomerRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public CustomerRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public CustomerRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public CustomerRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public CustomerRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public CustomerRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public CustomerRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public CustomerRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public CustomerRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public CustomerRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public CustomerRequest GroupByEmail()
                {
                    _query.GroupBy("email");
                    return this;
                }

                public CustomerRequest GroupByEmailAs(string retName)
                {
                    _query.GroupBy("email"); 
                    return this;
                }
                public CustomerRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public CustomerRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform"); 
                    return this;
                }
                public CustomerRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public CustomerRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public CustomerRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public CustomerRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
                    return this;
                }
                public CustomerRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public CustomerRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public CustomerRequest SelectCustomerOrderList()
                {
                    return SelectCustomerOrderListWith(new CustomerOrderRequest());
                }

                public CustomerRequest SelectCustomerOrderListWith(CustomerOrderRequest child)
                {
                    _query.RelationQuery("CustomerOrderList", "CustomerOrder", "customer", true, child.GetQuery());
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

    public sealed class ExecutableCustomerRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForList;
        private readonly Action _limitOne;

        internal ExecutableCustomerRequest(
            Func<UserContext, Task<QueryResult>> executeForList,
            Action limitOne)
        {
            _executeForList = executeForList;
            _limitOne = limitOne;
        }

        public Generated.Models.Customer NewEntity(UserContext ctx)
            => new Generated.Models.Customer();

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

        public async Task<List<Generated.Models.Customer>> ExecuteEntitiesForListAsync(
            UserContext ctx)
        {
            var result = await ExecuteForListAsync(ctx);
            var entities = new List<Generated.Models.Customer>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.Customer.FromRecord(row));
            return entities;
        }

        public async Task<Generated.Models.Customer> ExecuteEntityForOneAsync(
            UserContext ctx)
        {
            _limitOne();
            var entities = await ExecuteEntitiesForListAsync(ctx);
            return entities.Count > 0 ? entities[0] : null;
        }
    }
}