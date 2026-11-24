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
    public class TaskRequest
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

        public TaskRequest()
        {
            _query = new SelectQuery("Task");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public TaskRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public TaskRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public TaskRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public TaskRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableTaskRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableTaskRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public TaskRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public TaskRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public TaskRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public TaskRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public TaskRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public TaskRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public TaskRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public TaskRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public TaskRequest SelectId()
                {
                    _query.Project(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public TaskRequest SelectName()
                {
                    _query.Project(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }



                public TaskRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public TaskRequest SelectStatus()
                {
                    return SelectStatusWith(new TaskStatusRequest());
                }

                public TaskRequest SelectStatusWith(TaskStatusRequest related)
                {
                    _query.Project(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_MEMBER, "TaskStatus", global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectStatusWith", Relation: global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public TaskRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public TaskRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, "Platform", global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public TaskRequest WithStatusMatching(TaskStatusRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("TaskStatus")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskRequest WithoutStatusMatching(TaskStatusRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("TaskStatus")!, related.GetQuery(), "id"));
                    return this;
                }
                public TaskRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public TaskRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public TaskRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public TaskRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }

                public TaskRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }
                public TaskRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskRequest FilterByStatus(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskRequest FilterByStatusIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY, vals));
                    return this;
                }

                public TaskRequest WithStatusIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY));
                    return this;
                }

                public TaskRequest WithStatusIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY));
                    return this;
                }
                public TaskRequest WithStatusIsPlanned()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY, 1001L));
                    return this;
                }

                public TaskRequest WithStatusIsReady()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY, 1002L));
                    return this;
                }

                public TaskRequest WithStatusIsExecuting()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY, 1003L));
                    return this;
                }

                public TaskRequest WithStatusIsVerified()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY, 1004L));
                    return this;
                }


                public TaskRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY, val));
                    return this;
                }

                public TaskRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY, vals));
                    return this;
                }

                public TaskRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public TaskRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public TaskRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TaskRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public TaskRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public TaskRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public TaskRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }

                public TaskRequest OrderByNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskRequest OrderByNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY, "desc");
                    return this;
                }



                public TaskRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public TaskRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public TaskRequest Count()
        {
            return CountAs("count");
        }

        public TaskRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public TaskRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public TaskRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public TaskRequest GroupByName()
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public TaskRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }
                public TaskRequest GroupByStatus()
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY);
                    return this;
                }

                public TaskRequest GroupByStatusAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY);
                    return this;
                }
                public TaskRequest GroupByPlatform()
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }

                public TaskRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }
                public TaskRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public TaskRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Task.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public TaskRequest SelectTaskExecutionLogList()
                {
                    return SelectTaskExecutionLogListWith(new TaskExecutionLogRequest());
                }

                public TaskRequest SelectTaskExecutionLogListWith(TaskExecutionLogRequest child)
                {
                    _query.RelationQuery("TaskExecutionLogList", "TaskExecutionLog", "task", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTaskExecutionLogListWith", Relation: "TaskExecutionLogList")
                    };
                    return this;
                }
                public TaskRequest HaveTaskExecutionLogs()
                    => WithTaskExecutionLogListMatching(new TaskExecutionLogRequest());

                public TaskRequest HaveNoTaskExecutionLogs()
                    => WithoutTaskExecutionLogListMatching(new TaskExecutionLogRequest());

                public TaskRequest WithTaskExecutionLogListMatching(TaskExecutionLogRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("TaskExecutionLog")!, child.GetQuery(), "task"));
                    return this;
                }

                public TaskRequest WithoutTaskExecutionLogListMatching(TaskExecutionLogRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("TaskExecutionLog")!, child.GetQuery(), "task"));
                    return this;
                }
                public TaskRequest CountTaskExecutionLogs()
                    => CountTaskExecutionLogsAs("countTaskExecutionLogs");

                public TaskRequest CountTaskExecutionLogsAs(string alias)
                {
                    CountTaskExecutionLogsWith(alias, new TaskExecutionLogRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTaskExecutionLogsAs", alias, "TaskExecutionLogList")
                    };
                    return this;
                }

                public TaskRequest CountTaskExecutionLogsWith(string alias, TaskExecutionLogRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("TaskExecutionLogList", "TaskExecutionLog", "task", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTaskExecutionLogsWith", alias, "TaskExecutionLogList")
                    };
                    return this;
                }


                public TaskRequest FacetByStatusAs(
                    string name, TaskStatusRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByStatusAs", name, global::Generated.Models.Task.__TeaqlFields.STATUS_PROPERTY_QUERY)
                    });
                    return this;
                }

                public TaskRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, global::Generated.Models.Task.__TeaqlFields.PLATFORM_PROPERTY_QUERY)
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

        private async Task<Generated.Models.Task?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.Task>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.Task>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.Task>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Task.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Task> ExecuteForStreamInternalAsync(
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
                    Generated.Models.Task.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableTaskRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Task?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Task>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Task>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableTaskRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Task?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Task>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Task>> executeForStream,
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

        public ExecutableTaskRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.Task> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Task").ConfigureAwait(false));
            return context.InitializeEntity("Task", Generated.Models.Task.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Task>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Task> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Task?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}