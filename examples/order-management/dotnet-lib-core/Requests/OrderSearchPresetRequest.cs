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
                ? time.ToString(@"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture)
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
            _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public OrderSearchPresetRequest SelectId()
                {
                    _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest SelectName()
                {
                    _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest SelectFilterJson()
                {
                    _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest SelectRequestId()
                {
                    _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest SelectOwnerUserId()
                {
                    _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY);
                    return this;
                }


                public OrderSearchPresetRequest SelectCreateTime()
                {
                    _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest SelectUpdateTime()
                {
                    _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest SelectCommercePlatform()
                {
                    return SelectCommercePlatformWith(new CommercePlatformRequest());
                }

                public OrderSearchPresetRequest SelectCommercePlatformWith(CommercePlatformRequest related)
                {
                    _query.Project(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, "CommercePlatform", global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCommercePlatformWith", Relation: global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public OrderSearchPresetRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderSearchPresetRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderSearchPresetRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }
                public OrderSearchPresetRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderSearchPresetRequest WithFilterJsonIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY));
                    return this;
                }
                public OrderSearchPresetRequest WithFilterJsonNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithFilterJsonSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderSearchPresetRequest WithRequestIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY));
                    return this;
                }
                public OrderSearchPresetRequest WithRequestIdNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithRequestIdSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderSearchPresetRequest WithOwnerUserIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY));
                    return this;
                }
                public OrderSearchPresetRequest WithOwnerUserIdNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest WithOwnerUserIdSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderSearchPresetRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, vals));
                    return this;
                }

                public OrderSearchPresetRequest WithCommercePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithCommercePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public OrderSearchPresetRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByFilterJsonAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByFilterJsonDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByRequestIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByRequestIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByOwnerUserIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByOwnerUserIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public OrderSearchPresetRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderSearchPresetRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
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
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public OrderSearchPresetRequest GroupByName()
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }
                public OrderSearchPresetRequest GroupByFilterJson()
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest GroupByFilterJsonAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.FILTER_JSON_PROPERTY_QUERY);
                    return this;
                }
                public OrderSearchPresetRequest GroupByRequestId()
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest GroupByRequestIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.REQUEST_ID_PROPERTY_QUERY);
                    return this;
                }
                public OrderSearchPresetRequest GroupByOwnerUserId()
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest GroupByOwnerUserIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.OWNER_USER_ID_PROPERTY_QUERY);
                    return this;
                }
                public OrderSearchPresetRequest GroupByCommercePlatform()
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY);
                    return this;
                }
                public OrderSearchPresetRequest GroupByCreateTime()
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }
                public OrderSearchPresetRequest GroupByUpdateTime()
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
                    return this;
                }
                public OrderSearchPresetRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public OrderSearchPresetRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderSearchPreset.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public OrderSearchPresetRequest FacetByCommercePlatformAs(
                    string name, CommercePlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCommercePlatformAs", name, global::Generated.Models.OrderSearchPreset.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY)
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
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.OrderSearchPreset>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.OrderSearchPreset>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            if (limitOne) query.Limit(1);
            var request = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            if (request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Grouped projections require ExecuteForRowsAsync");
            SmartList<Record> result;
            if (_countRequested) result = await RuntimeDataService.FetchPageAsync(service, context, request);
            else {
                var rows = await service.QueryAsync(request);
                result = new SmartList<Record>(rows.Rows) { Facets = rows.Facets };
            }
            var alias = _countRequested ? request.Query.PageCountOperation?.Alias : null;
            var aggregateOnly = request.Query.AggregateItems.Any(item =>
                !(alias != null && item.Function == AggregateFunction.Count && item.Alias == alias));
            if (aggregateOnly) {
                if (result.Count > 0) foreach (var pair in result[0]) result.Aggregations[pair.Key] = pair.Value;
                result.Clear();
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
            if (request.Query.AggregateItems.Count > 0 || request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Entity streams require entity rows; use a record or aggregation query");
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
            return context.InitializeEntity("OrderSearchPreset", Generated.Models.OrderSearchPreset.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
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