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
    public class TaskStatusRequest
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

        public TaskStatusRequest()
        {
            _query = new SelectQuery("TaskStatus");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public TaskStatusRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public TaskStatusRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public TaskStatusRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public TaskStatusRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableTaskStatusRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableTaskStatusRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public TaskStatusRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public TaskStatusRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public TaskStatusRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public TaskStatusRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public TaskStatusRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public TaskStatusRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public TaskStatusRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public TaskStatusRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public TaskStatusRequest SelectId()
                {
                    _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest SelectName()
                {
                    _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest SelectCode()
                {
                    _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest SelectColor()
                {
                    _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest SelectDisplayOrder()
                {
                    _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest SelectProgress()
                {
                    _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY);
                    return this;
                }


                public TaskStatusRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public TaskStatusRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, "Platform", global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public TaskStatusRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskStatusRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskStatusRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public TaskStatusRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }
                public TaskStatusRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public TaskStatusRequest WithCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY));
                    return this;
                }
                public TaskStatusRequest WithCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithColorContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithColorIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public TaskStatusRequest WithColorIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithColorNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithColorGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithColorIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithColorIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY));
                    return this;
                }
                public TaskStatusRequest WithColorNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithColorStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithColorNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithColorEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithColorNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithColorSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithProgressIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithProgressNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithProgressGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithProgressIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithProgressIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskStatusRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY, vals));
                    return this;
                }

                public TaskStatusRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public TaskStatusRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskStatusRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }

                public TaskStatusRequest OrderByNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskStatusRequest OrderByNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public TaskStatusRequest OrderByCodeAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskStatusRequest OrderByCodeDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public TaskStatusRequest OrderByColorAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskStatusRequest OrderByColorDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY, "desc");
                    return this;
                }

                public TaskStatusRequest OrderByDisplayOrderAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskStatusRequest OrderByDisplayOrderDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, "desc");
                    return this;
                }

                public TaskStatusRequest OrderByProgressAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskStatusRequest OrderByProgressDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, "desc");
                    return this;
                }


                public TaskStatusRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskStatusRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public TaskStatusRequest Count()
        {
            return CountAs("count");
        }

        public TaskStatusRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public TaskStatusRequest MinDisplayOrder()
                {
                    return MinDisplayOrderAs("minOfDisplayOrder");
                }

                public TaskStatusRequest MinDisplayOrderAs(string retName)
                {
                    _query.Aggregate("min", global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest MaxDisplayOrder()
                {
                    return MaxDisplayOrderAs("maxOfDisplayOrder");
                }

                public TaskStatusRequest MaxDisplayOrderAs(string retName)
                {
                    _query.Aggregate("max", global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest SumDisplayOrder()
                {
                    return SumDisplayOrderAs("sumOfDisplayOrder");
                }

                public TaskStatusRequest SumDisplayOrderAs(string retName)
                {
                    _query.Aggregate("sum", global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest AvgDisplayOrder()
                {
                    return AvgDisplayOrderAs("avgOfDisplayOrder");
                }

                public TaskStatusRequest AvgDisplayOrderAs(string retName)
                {
                    _query.Aggregate("avg", global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest StandardDeviationDisplayOrder()
                {
                    return StandardDeviationDisplayOrderAs("standardDeviationOfDisplayOrder");
                }

                public TaskStatusRequest StandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev", global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest SquareRootOfPopulationStandardDeviationDisplayOrder()
                {
                    return SquareRootOfPopulationStandardDeviationDisplayOrderAs("squareRootOfPopulationStandardDeviationOfDisplayOrder");
                }

                public TaskStatusRequest SquareRootOfPopulationStandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev_pop", global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest SampleVarianceDisplayOrder()
                {
                    return SampleVarianceDisplayOrderAs("sampleVarianceOfDisplayOrder");
                }

                public TaskStatusRequest SampleVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_samp", global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest SamplePopulationVarianceDisplayOrder()
                {
                    return SamplePopulationVarianceDisplayOrderAs("samplePopulationVarianceOfDisplayOrder");
                }

                public TaskStatusRequest SamplePopulationVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_pop", global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest MinProgress()
                {
                    return MinProgressAs("minOfProgress");
                }

                public TaskStatusRequest MinProgressAs(string retName)
                {
                    _query.Aggregate("min", global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest MaxProgress()
                {
                    return MaxProgressAs("maxOfProgress");
                }

                public TaskStatusRequest MaxProgressAs(string retName)
                {
                    _query.Aggregate("max", global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest SumProgress()
                {
                    return SumProgressAs("sumOfProgress");
                }

                public TaskStatusRequest SumProgressAs(string retName)
                {
                    _query.Aggregate("sum", global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest AvgProgress()
                {
                    return AvgProgressAs("avgOfProgress");
                }

                public TaskStatusRequest AvgProgressAs(string retName)
                {
                    _query.Aggregate("avg", global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest StandardDeviationProgress()
                {
                    return StandardDeviationProgressAs("standardDeviationOfProgress");
                }

                public TaskStatusRequest StandardDeviationProgressAs(string retName)
                {
                    _query.Aggregate("stddev", global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest SquareRootOfPopulationStandardDeviationProgress()
                {
                    return SquareRootOfPopulationStandardDeviationProgressAs("squareRootOfPopulationStandardDeviationOfProgress");
                }

                public TaskStatusRequest SquareRootOfPopulationStandardDeviationProgressAs(string retName)
                {
                    _query.Aggregate("stddev_pop", global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest SampleVarianceProgress()
                {
                    return SampleVarianceProgressAs("sampleVarianceOfProgress");
                }

                public TaskStatusRequest SampleVarianceProgressAs(string retName)
                {
                    _query.Aggregate("var_samp", global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest SamplePopulationVarianceProgress()
                {
                    return SamplePopulationVarianceProgressAs("samplePopulationVarianceOfProgress");
                }

                public TaskStatusRequest SamplePopulationVarianceProgressAs(string retName)
                {
                    _query.Aggregate("var_pop", global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY, retName);
                    return this;
                }
                public TaskStatusRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public TaskStatusRequest GroupByName()
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }
                public TaskStatusRequest GroupByCode()
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest GroupByCodeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.CODE_PROPERTY_QUERY);
                    return this;
                }
                public TaskStatusRequest GroupByColor()
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest GroupByColorAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.COLOR_PROPERTY_QUERY);
                    return this;
                }
                public TaskStatusRequest GroupByDisplayOrder()
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest GroupByDisplayOrderAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.DISPLAY_ORDER_PROPERTY_QUERY);
                    return this;
                }
                public TaskStatusRequest GroupByProgress()
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest GroupByProgressAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.PROGRESS_PROPERTY_QUERY);
                    return this;
                }
                public TaskStatusRequest GroupByPlatform()
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }
                public TaskStatusRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public TaskStatusRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskStatus.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public TaskStatusRequest SelectTaskList()
                {
                    return SelectTaskListWith(new TaskRequest());
                }

                public TaskStatusRequest SelectTaskListWith(TaskRequest child)
                {
                    _query.RelationQuery("TaskList", "Task", "status", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTaskListWith", Relation: "TaskList")
                    };
                    return this;
                }
                public TaskStatusRequest HaveTasks()
                    => WithTaskListMatching(new TaskRequest());

                public TaskStatusRequest HaveNoTasks()
                    => WithoutTaskListMatching(new TaskRequest());

                public TaskStatusRequest WithTaskListMatching(TaskRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, child.GetQuery(), "status"));
                    return this;
                }

                public TaskStatusRequest WithoutTaskListMatching(TaskRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, child.GetQuery(), "status"));
                    return this;
                }
                public TaskStatusRequest CountTasks()
                    => CountTasksAs("countTasks");

                public TaskStatusRequest CountTasksAs(string alias)
                {
                    CountTasksWith(alias, new TaskRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTasksAs", alias, "TaskList")
                    };
                    return this;
                }

                public TaskStatusRequest CountTasksWith(string alias, TaskRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("TaskList", "Task", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTasksWith", alias, "TaskList")
                    };
                    return this;
                }


                public TaskStatusRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, global::Generated.Models.TaskStatus.__TeaqlFields.PLATFORM_PROPERTY_QUERY)
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

        private async Task<Generated.Models.TaskStatus?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.TaskStatus>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.TaskStatus>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.TaskStatus>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.TaskStatus.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.TaskStatus> ExecuteForStreamInternalAsync(
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
                    Generated.Models.TaskStatus.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableTaskStatusRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.TaskStatus?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.TaskStatus>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.TaskStatus>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableTaskStatusRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.TaskStatus?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.TaskStatus>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.TaskStatus>> executeForStream,
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

        public ExecutableTaskStatusRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.TaskStatus> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("TaskStatus").ConfigureAwait(false));
            return context.InitializeEntity("TaskStatus", Generated.Models.TaskStatus.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.TaskStatus>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.TaskStatus> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.TaskStatus?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}