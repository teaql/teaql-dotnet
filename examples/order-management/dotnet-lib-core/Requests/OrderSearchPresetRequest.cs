using System;
using System.Collections.Generic;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class OrderSearchPresetRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public OrderSearchPresetRequest()
        {
            _query = new SelectQuery("OrderSearchPreset");
        }

        public SelectQuery GetQuery() => _query;

        public OrderSearchPresetRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderSearchPresetRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(_comment))
            {
                throw new InvalidOperationException("Purpose() requires a non-empty Comment() set earlier on the request");
            }
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderSearchPresetRequest(
                ExecuteForListInternalAsync, () => Limit(1));
        }

        public OrderSearchPresetRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public OrderSearchPresetRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

                public OrderSearchPresetRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIs(string val)
                {
                    _query.AndFilter(Expr.Eq("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("filter_json", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIs(string val)
                {
                    _query.AndFilter(Expr.Eq("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("request_id", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIs(string val)
                {
                    _query.AndFilter(Expr.Eq("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("owner_user_id", vals));
                    return this;
                }

                public OrderSearchPresetRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public OrderSearchPresetRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public OrderSearchPresetRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByFilterJsonAscending()
                {
                    _query.OrderBy("filter_json", "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByFilterJsonDescending()
                {
                    _query.OrderBy("filter_json", "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByRequestIdAscending()
                {
                    _query.OrderBy("request_id", "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByRequestIdDescending()
                {
                    _query.OrderBy("request_id", "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByOwnerUserIdAscending()
                {
                    _query.OrderBy("owner_user_id", "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByOwnerUserIdDescending()
                {
                    _query.OrderBy("owner_user_id", "desc");
                    return this;
                }


                public OrderSearchPresetRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public OrderSearchPresetRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public OrderSearchPresetRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public OrderSearchPresetRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public OrderSearchPresetRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public OrderSearchPresetRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public OrderSearchPresetRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public OrderSearchPresetRequest GroupByFilterJson()
                {
                    _query.GroupBy("filter_json");
                    return this;
                }

                public OrderSearchPresetRequest GroupByFilterJsonAs(string retName)
                {
                    _query.GroupBy("filter_json"); 
                    return this;
                }
                public OrderSearchPresetRequest GroupByRequestId()
                {
                    _query.GroupBy("request_id");
                    return this;
                }

                public OrderSearchPresetRequest GroupByRequestIdAs(string retName)
                {
                    _query.GroupBy("request_id"); 
                    return this;
                }
                public OrderSearchPresetRequest GroupByOwnerUserId()
                {
                    _query.GroupBy("owner_user_id");
                    return this;
                }

                public OrderSearchPresetRequest GroupByOwnerUserIdAs(string retName)
                {
                    _query.GroupBy("owner_user_id"); 
                    return this;
                }
                public OrderSearchPresetRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public OrderSearchPresetRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform"); 
                    return this;
                }
                public OrderSearchPresetRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public OrderSearchPresetRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public OrderSearchPresetRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public OrderSearchPresetRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
                    return this;
                }
                public OrderSearchPresetRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public OrderSearchPresetRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
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

    public sealed class ExecutableOrderSearchPresetRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForList;
        private readonly Action _limitOne;

        internal ExecutableOrderSearchPresetRequest(
            Func<UserContext, Task<QueryResult>> executeForList,
            Action limitOne)
        {
            _executeForList = executeForList;
            _limitOne = limitOne;
        }

        public Generated.Models.OrderSearchPreset NewEntity(UserContext ctx)
            => new Generated.Models.OrderSearchPreset();

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

        public async Task<List<Generated.Models.OrderSearchPreset>> ExecuteEntitiesForListAsync(
            UserContext ctx)
        {
            var result = await ExecuteForListAsync(ctx);
            var entities = new List<Generated.Models.OrderSearchPreset>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.OrderSearchPreset.FromRecord(row));
            return entities;
        }

        public async Task<Generated.Models.OrderSearchPreset> ExecuteEntityForOneAsync(
            UserContext ctx)
        {
            _limitOne();
            var entities = await ExecuteEntitiesForListAsync(ctx);
            return entities.Count > 0 ? entities[0] : null;
        }
    }
}