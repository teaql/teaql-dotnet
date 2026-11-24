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
    public class TaskExecutionLogRequest
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

        public TaskExecutionLogRequest()
        {
            _query = new SelectQuery("TaskExecutionLog");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public TaskExecutionLogRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public TaskExecutionLogRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public TaskExecutionLogRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public TaskExecutionLogRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableTaskExecutionLogRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableTaskExecutionLogRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public TaskExecutionLogRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public TaskExecutionLogRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public TaskExecutionLogRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public TaskExecutionLogRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public TaskExecutionLogRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public TaskExecutionLogRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public TaskExecutionLogRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public TaskExecutionLogRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public TaskExecutionLogRequest SelectId()
                {
                    _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public TaskExecutionLogRequest SelectAction()
                {
                    _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY);
                    return this;
                }

                public TaskExecutionLogRequest SelectDetail()
                {
                    _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY);
                    return this;
                }

                public TaskExecutionLogRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public TaskExecutionLogRequest SelectTask()
                {
                    return SelectTaskWith(new TaskRequest());
                }

                public TaskExecutionLogRequest SelectTaskWith(TaskRequest related)
                {
                    _query.Project(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_MEMBER, "Task", global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTaskWith", Relation: global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public TaskExecutionLogRequest WithTaskMatching(TaskRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskExecutionLogRequest WithoutTaskMatching(TaskRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public TaskExecutionLogRequest FilterByTask(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest FilterByTaskIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY, vals));
                    return this;
                }

                public TaskExecutionLogRequest WithTaskIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY));
                    return this;
                }

                public TaskExecutionLogRequest WithTaskIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY));
                    return this;
                }

                public TaskExecutionLogRequest WithActionContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public TaskExecutionLogRequest WithActionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithActionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithActionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY));
                    return this;
                }

                public TaskExecutionLogRequest WithActionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY));
                    return this;
                }
                public TaskExecutionLogRequest WithActionNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public TaskExecutionLogRequest WithDetailIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY));
                    return this;
                }
                public TaskExecutionLogRequest WithDetailNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public TaskExecutionLogRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public TaskExecutionLogRequest OrderByActionAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByActionDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY, "desc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByDetailAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByDetailDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY, "desc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public TaskExecutionLogRequest Count()
        {
            return CountAs("count");
        }

        public TaskExecutionLogRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public TaskExecutionLogRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public TaskExecutionLogRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public TaskExecutionLogRequest GroupByTask()
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY);
                    return this;
                }

                public TaskExecutionLogRequest GroupByTaskAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY);
                    return this;
                }
                public TaskExecutionLogRequest GroupByAction()
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY);
                    return this;
                }

                public TaskExecutionLogRequest GroupByActionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.ACTION_PROPERTY_QUERY);
                    return this;
                }
                public TaskExecutionLogRequest GroupByDetail()
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY);
                    return this;
                }

                public TaskExecutionLogRequest GroupByDetailAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.DETAIL_PROPERTY_QUERY);
                    return this;
                }
                public TaskExecutionLogRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public TaskExecutionLogRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.TaskExecutionLog.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public TaskExecutionLogRequest FacetByTaskAs(
                    string name, TaskRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByTaskAs", name, global::Generated.Models.TaskExecutionLog.__TeaqlFields.TASK_PROPERTY_QUERY)
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

        private async Task<Generated.Models.TaskExecutionLog?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.TaskExecutionLog>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.TaskExecutionLog>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.TaskExecutionLog>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.TaskExecutionLog.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.TaskExecutionLog> ExecuteForStreamInternalAsync(
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
                    Generated.Models.TaskExecutionLog.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableTaskExecutionLogRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.TaskExecutionLog?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.TaskExecutionLog>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.TaskExecutionLog>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableTaskExecutionLogRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.TaskExecutionLog?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.TaskExecutionLog>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.TaskExecutionLog>> executeForStream,
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

        public ExecutableTaskExecutionLogRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.TaskExecutionLog> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("TaskExecutionLog").ConfigureAwait(false));
            return context.InitializeEntity("TaskExecutionLog", Generated.Models.TaskExecutionLog.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.TaskExecutionLog>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.TaskExecutionLog> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.TaskExecutionLog?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}