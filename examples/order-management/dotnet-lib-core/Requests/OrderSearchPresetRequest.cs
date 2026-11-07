using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TeaQL.Core;
using TeaQL.Runtime;
using Generated.Models;

namespace Generated.Requests
{
    public class OrderSearchPresetRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;
        private bool _countRequested;
        private readonly List<DynamicSearchWarning> _dynamicSearchWarnings = new();
        private static object TeaqlQueryValue(object value) =>
            value is TimeSpan time
                ? time.ToString("c", System.Globalization.CultureInfo.InvariantCulture)
                : value;

        public OrderSearchPresetRequest()
        {
            _query = new SelectQuery("OrderSearchPreset");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public OrderSearchPresetRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

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
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderSearchPresetRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
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
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCommercePlatformWith", Relation: "CommercePlatform")
                    };
                    return this;
                }
                public OrderSearchPresetRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderSearchPresetRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderSearchPresetRequest WithNameIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithNameNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithNameGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
                    _query.AndFilter(Expr.Eq("filter_json", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderSearchPresetRequest WithFilterJsonIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("filter_json", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("filter_json", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("filter_json", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("filter_json", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("filter_json", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("filter_json", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("filter_json", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("filter_json", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
                    _query.AndFilter(Expr.Eq("request_id", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderSearchPresetRequest WithRequestIdIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("request_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("request_id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("request_id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("request_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("request_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("request_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("request_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("request_id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
                    _query.AndFilter(Expr.Eq("owner_user_id", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderSearchPresetRequest WithOwnerUserIdIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("owner_user_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("owner_user_id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("owner_user_id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("owner_user_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("owner_user_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("owner_user_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("owner_user_id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("owner_user_id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public OrderSearchPresetRequest WithCreateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("create_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public OrderSearchPresetRequest WithUpdateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("update_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public OrderSearchPresetRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
            return CountAs("count");
        }

        public OrderSearchPresetRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
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
                    _query.ProjectExpr(retName, Expr.Column("id"));
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
                    _query.ProjectExpr(retName, Expr.Column("name"));
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
                    _query.ProjectExpr(retName, Expr.Column("filter_json"));
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
                    _query.ProjectExpr(retName, Expr.Column("request_id"));
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
                    _query.ProjectExpr(retName, Expr.Column("owner_user_id"));
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
                    _query.ProjectExpr(retName, Expr.Column("commerce_platform"));
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
                    _query.ProjectExpr(retName, Expr.Column("create_time"));
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
                    _query.ProjectExpr(retName, Expr.Column("update_time"));
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
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public OrderSearchPresetRequest FacetByCommercePlatformAs(
                    string name, CommercePlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "CommercePlatform", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCommercePlatformAs", name, "commerce_platform")
                    });
                    return this;
                }


        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var req = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            // The runtime owns authorized membership, recursive Facets and SQL provenance.
            return await service.QueryAsync(req);
        }

        private async Task<Generated.Models.OrderSearchPreset?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.OrderSearchPreset.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.OrderSearchPreset>> ExecuteForListWithMetadataInternalAsync(
            UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            var request = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            SmartList<Record> result;
            if (_countRequested) result = await RuntimeDataService.FetchPageAsync(service, context, request);
            else {
                var rows = await service.QueryAsync(request);
                result = new SmartList<Record>(rows.Rows) { Facets = rows.Facets };
            }
            var entities = new SmartList<Generated.Models.OrderSearchPreset>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.OrderSearchPreset.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.OrderSearchPreset> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var request = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            return RuntimeDataService.StreamEntities(context, request,
                row => global::TeaQL.Runtime.EntityQueryProperties.Capture(
                    Generated.Models.OrderSearchPreset.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableOrderSearchPresetRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.OrderSearchPreset?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.OrderSearchPreset>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderSearchPreset>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableOrderSearchPresetRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.OrderSearchPreset?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.OrderSearchPreset>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderSearchPreset>> executeForStream,
            Action<string> comment,
            Action ensureIntent)
        {
            _executeForRows = executeForRows;
            _executeForOne = executeForOne;
            _executeForList = executeForList;
            _executeForStream = executeForStream;
            _comment = comment;
            _ensureIntent = ensureIntent;
        }

        public ExecutableOrderSearchPresetRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.OrderSearchPreset> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("OrderSearchPreset").ConfigureAwait(false));
            var entity = context.InitializeEntity("OrderSearchPreset", new Generated.Models.OrderSearchPreset());
            entity.TeaqlInitializeNewId(identity);
            return entity;
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.OrderSearchPreset>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.OrderSearchPreset> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.OrderSearchPreset?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}