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
    public class OrderSearchPresetRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;

        public OrderSearchPresetRequest()
        {
            _query = new SelectQuery("OrderSearchPreset");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public OrderSearchPresetRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public OrderSearchPresetRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public OrderSearchPresetRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderSearchPresetRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderSearchPresetRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
                c => Comment(c),
                EnsureIntent);
        }

        public OrderSearchPresetRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public OrderSearchPresetRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public OrderSearchPresetRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public OrderSearchPresetRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public OrderSearchPresetRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
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

        public OrderSearchPresetRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("filter_json");
            _query.Project("request_id");
            _query.Project("owner_user_id");
            _query.Project("commerce_platform");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public OrderSearchPresetRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public OrderSearchPresetRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public OrderSearchPresetRequest SelectFilterJson()
                {
                    _query.Project("filter_json");
                    return this;
                }

                public OrderSearchPresetRequest SelectRequestId()
                {
                    _query.Project("request_id");
                    return this;
                }

                public OrderSearchPresetRequest SelectOwnerUserId()
                {
                    _query.Project("owner_user_id");
                    return this;
                }


                public OrderSearchPresetRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public OrderSearchPresetRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public OrderSearchPresetRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public OrderSearchPresetRequest SelectCommercePlatform()
                {
                    return SelectCommercePlatformWith(new CommercePlatformRequest());
                }

                public OrderSearchPresetRequest SelectCommercePlatformWith(CommercePlatformRequest related)
                {
                    _query.Project("commerce_platform");
                    _query.ForwardRelationQuery("CommercePlatform", "CommercePlatform", "commerce_platform", related.GetQuery());
                    return this;
                }
                public OrderSearchPresetRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public OrderSearchPresetRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
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
                public OrderSearchPresetRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", lower, upper));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public OrderSearchPresetRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
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
                public OrderSearchPresetRequest WithFilterJsonIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("filter_json", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("filter_json", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("filter_json", lower, upper));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("filter_json"));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("filter_json"));
                    return this;
                }
                public OrderSearchPresetRequest WithFilterJsonNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("filter_json", val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("filter_json", val));
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
                public OrderSearchPresetRequest WithRequestIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("request_id", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("request_id", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("request_id", lower, upper));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("request_id"));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("request_id"));
                    return this;
                }
                public OrderSearchPresetRequest WithRequestIdNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("request_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("request_id", val));
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
                public OrderSearchPresetRequest WithOwnerUserIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("owner_user_id", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("owner_user_id", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("owner_user_id", lower, upper));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("owner_user_id"));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("owner_user_id"));
                    return this;
                }
                public OrderSearchPresetRequest WithOwnerUserIdNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("owner_user_id", val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("owner_user_id", val));
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

                public OrderSearchPresetRequest WithCommercePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("commerce_platform"));
                    return this;
                }

                public OrderSearchPresetRequest WithCommercePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("commerce_platform"));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", lower, upper));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("update_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("update_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("update_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("update_time", lower, upper));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
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
                public OrderSearchPresetRequest FacetByCommercePlatformAs(
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

        private async Task<OrderSearchPresetPage> ExecuteForPageInternalAsync(
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
                var countQuery = new SelectQuery("OrderSearchPreset");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.OrderSearchPreset>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.OrderSearchPreset.FromRecord(row, queryRoot));
            return new OrderSearchPresetPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.OrderSearchPreset> ExecuteForStreamInternalAsync(
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
                    yield return Generated.Models.OrderSearchPreset.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableOrderSearchPresetRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<OrderSearchPresetPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderSearchPreset>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableOrderSearchPresetRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<OrderSearchPresetPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderSearchPreset>> executeForStream,
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

        public ExecutableOrderSearchPresetRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.OrderSearchPreset NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.OrderSearchPreset();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.OrderSearchPreset>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.OrderSearchPreset>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.OrderSearchPreset.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<OrderSearchPresetPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.OrderSearchPreset> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.OrderSearchPreset?> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class OrderSearchPresetPage
    {
        public SmartList<Generated.Models.OrderSearchPreset> Rows { get; }
        public long TotalCount { get; }
        public OrderSearchPresetPage(
            SmartList<Generated.Models.OrderSearchPreset> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}