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
    public class SchoolTypeRequest
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

        public SchoolTypeRequest()
        {
            _query = new SelectQuery("SchoolType");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public SchoolTypeRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public SchoolTypeRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public SchoolTypeRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public SchoolTypeRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableSchoolTypeRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableSchoolTypeRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public SchoolTypeRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public SchoolTypeRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public SchoolTypeRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public SchoolTypeRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public SchoolTypeRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public SchoolTypeRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public SchoolTypeRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public SchoolTypeRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public SchoolTypeRequest SelectId()
                {
                    _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest SelectName()
                {
                    _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest SelectCode()
                {
                    _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest SelectDisplayOrder()
                {
                    _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public SchoolTypeRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, "Platform", global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public SchoolTypeRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public SchoolTypeRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public SchoolTypeRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY, vals));
                    return this;
                }

                public SchoolTypeRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolTypeRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public SchoolTypeRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolTypeRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }
                public SchoolTypeRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public SchoolTypeRequest WithCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolTypeRequest WithCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest WithCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY));
                    return this;
                }
                public SchoolTypeRequest WithCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolTypeRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolTypeRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolTypeRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public SchoolTypeRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByCodeAscending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByCodeDescending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByDisplayOrderAscending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByDisplayOrderDescending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public SchoolTypeRequest Count()
        {
            return CountAs("count");
        }

        public SchoolTypeRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public SchoolTypeRequest MinDisplayOrder()
                {
                    return MinDisplayOrderAs("minOfDisplayOrder");
                }

                public SchoolTypeRequest MinDisplayOrderAs(string retName)
                {
                    _query.Aggregate("min", global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolTypeRequest MaxDisplayOrder()
                {
                    return MaxDisplayOrderAs("maxOfDisplayOrder");
                }

                public SchoolTypeRequest MaxDisplayOrderAs(string retName)
                {
                    _query.Aggregate("max", global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolTypeRequest SumDisplayOrder()
                {
                    return SumDisplayOrderAs("sumOfDisplayOrder");
                }

                public SchoolTypeRequest SumDisplayOrderAs(string retName)
                {
                    _query.Aggregate("sum", global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolTypeRequest AvgDisplayOrder()
                {
                    return AvgDisplayOrderAs("avgOfDisplayOrder");
                }

                public SchoolTypeRequest AvgDisplayOrderAs(string retName)
                {
                    _query.Aggregate("avg", global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolTypeRequest StandardDeviationDisplayOrder()
                {
                    return StandardDeviationDisplayOrderAs("standardDeviationOfDisplayOrder");
                }

                public SchoolTypeRequest StandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev", global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolTypeRequest SquareRootOfPopulationStandardDeviationDisplayOrder()
                {
                    return SquareRootOfPopulationStandardDeviationDisplayOrderAs("squareRootOfPopulationStandardDeviationOfDisplayOrder");
                }

                public SchoolTypeRequest SquareRootOfPopulationStandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev_pop", global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolTypeRequest SampleVarianceDisplayOrder()
                {
                    return SampleVarianceDisplayOrderAs("sampleVarianceOfDisplayOrder");
                }

                public SchoolTypeRequest SampleVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_samp", global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolTypeRequest SamplePopulationVarianceDisplayOrder()
                {
                    return SamplePopulationVarianceDisplayOrderAs("samplePopulationVarianceOfDisplayOrder");
                }

                public SchoolTypeRequest SamplePopulationVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_pop", global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolTypeRequest GroupByPlatform()
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }
                public SchoolTypeRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public SchoolTypeRequest GroupByName()
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }
                public SchoolTypeRequest GroupByCode()
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest GroupByCodeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.CODE_PROPERTY_QUERY);
                    return this;
                }
                public SchoolTypeRequest GroupByDisplayOrder()
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest GroupByDisplayOrderAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY);
                    return this;
                }
                public SchoolTypeRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public SchoolTypeRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.SchoolType.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public SchoolTypeRequest SelectSchoolList()
                {
                    return SelectSchoolListWith(new SchoolRequest());
                }

                public SchoolTypeRequest SelectSchoolListWith(SchoolRequest child)
                {
                    _query.RelationQuery("SchoolList", "School", "school_type", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectSchoolListWith", Relation: "SchoolList")
                    };
                    return this;
                }
                public SchoolTypeRequest HaveSchools()
                    => WithSchoolListMatching(new SchoolRequest());

                public SchoolTypeRequest HaveNoSchools()
                    => WithoutSchoolListMatching(new SchoolRequest());

                public SchoolTypeRequest WithSchoolListMatching(SchoolRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("School")!, child.GetQuery(), "school_type"));
                    return this;
                }

                public SchoolTypeRequest WithoutSchoolListMatching(SchoolRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("School")!, child.GetQuery(), "school_type"));
                    return this;
                }
                public SchoolTypeRequest CountSchools()
                    => CountSchoolsAs("countSchools");

                public SchoolTypeRequest CountSchoolsAs(string alias)
                {
                    CountSchoolsWith(alias, new SchoolRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }

                public SchoolTypeRequest CountSchoolsWith(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountSchoolsWith", alias, "SchoolList")
                    };
                    return this;
                }

                public SchoolTypeRequest MinStudentCapacityOfSchools()
                    => MinStudentCapacityOfSchoolsAs("minOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest MinStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public SchoolTypeRequest MaxStudentCapacityOfSchools()
                    => MaxStudentCapacityOfSchoolsAs("maxOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest MaxStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public SchoolTypeRequest SumStudentCapacityOfSchools()
                    => SumStudentCapacityOfSchoolsAs("sumOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest SumStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public SchoolTypeRequest AvgStudentCapacityOfSchools()
                    => AvgStudentCapacityOfSchoolsAs("avgOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest AvgStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public SchoolTypeRequest StandardDeviationStudentCapacityOfSchools()
                    => StandardDeviationStudentCapacityOfSchoolsAs("standardDeviationOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest StandardDeviationStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public SchoolTypeRequest SquareRootOfPopulationStandardDeviationStudentCapacityOfSchools()
                    => SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs("squareRootOfPopulationStandardDeviationOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public SchoolTypeRequest SampleVarianceStudentCapacityOfSchools()
                    => SampleVarianceStudentCapacityOfSchoolsAs("sampleVarianceOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest SampleVarianceStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public SchoolTypeRequest SamplePopulationVarianceStudentCapacityOfSchools()
                    => SamplePopulationVarianceStudentCapacityOfSchoolsAs("samplePopulationVarianceOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest SamplePopulationVarianceStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public SchoolTypeRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, global::Generated.Models.SchoolType.__TeaqlFields.PLATFORM_PROPERTY_QUERY)
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

        private async Task<Generated.Models.SchoolType?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.SchoolType>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.SchoolType>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.SchoolType>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.SchoolType.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.SchoolType> ExecuteForStreamInternalAsync(
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
                    Generated.Models.SchoolType.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableSchoolTypeRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.SchoolType?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.SchoolType>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.SchoolType>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableSchoolTypeRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.SchoolType?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.SchoolType>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.SchoolType>> executeForStream,
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

        public ExecutableSchoolTypeRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.SchoolType> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("SchoolType").ConfigureAwait(false));
            return context.InitializeEntity("SchoolType", Generated.Models.SchoolType.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.SchoolType>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.SchoolType> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.SchoolType?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}