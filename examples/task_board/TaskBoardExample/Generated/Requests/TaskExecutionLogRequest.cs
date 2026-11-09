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
                ? time.ToString("c", System.Globalization.CultureInfo.InvariantCulture)
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
            _query.Project("id");
            _query.Project("task");
            _query.Project("action");
            _query.Project("detail");
            _query.Project("version");
            return this;
        }

                public TaskExecutionLogRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public TaskExecutionLogRequest SelectAction()
                {
                    _query.Project("action");
                    return this;
                }

                public TaskExecutionLogRequest SelectDetail()
                {
                    _query.Project("detail");
                    return this;
                }

                public TaskExecutionLogRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public TaskExecutionLogRequest SelectTask()
                {
                    return SelectTaskWith(new TaskRequest());
                }

                public TaskExecutionLogRequest SelectTaskWith(TaskRequest related)
                {
                    _query.Project("task");
                    _query.ForwardRelationQuery("Task", "Task", "task", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTaskWith", Relation: "Task")
                    };
                    return this;
                }
                public TaskExecutionLogRequest WithTaskMatching(TaskRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("task", GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskExecutionLogRequest WithoutTaskMatching(TaskRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("task", GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public TaskExecutionLogRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public TaskExecutionLogRequest FilterByTask(object val)
                {
                    _query.AndFilter(Expr.Eq("task", val));
                    return this;
                }

                public TaskExecutionLogRequest FilterByTaskIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("task", vals));
                    return this;
                }

                public TaskExecutionLogRequest WithTaskIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("task"));
                    return this;
                }

                public TaskExecutionLogRequest WithTaskIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("task"));
                    return this;
                }

                public TaskExecutionLogRequest WithActionContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("action", val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionIs(string val)
                {
                    _query.AndFilter(Expr.Eq("action", TeaqlQueryValue(val)));
                    return this;
                }
                public TaskExecutionLogRequest WithActionIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("action", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("action", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithActionNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("action", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithActionGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("action", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("action", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("action", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("action", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("action", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskExecutionLogRequest WithActionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("action"));
                    return this;
                }

                public TaskExecutionLogRequest WithActionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("action"));
                    return this;
                }
                public TaskExecutionLogRequest WithActionNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("action", val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("action", val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("action", val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("action", val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("action", val));
                    return this;
                }

                public TaskExecutionLogRequest WithActionSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("action", val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("detail", val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailIs(string val)
                {
                    _query.AndFilter(Expr.Eq("detail", TeaqlQueryValue(val)));
                    return this;
                }
                public TaskExecutionLogRequest WithDetailIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("detail", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("detail", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("detail", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("detail", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("detail", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("detail", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("detail", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("detail", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("detail"));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("detail"));
                    return this;
                }
                public TaskExecutionLogRequest WithDetailNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("detail", val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("detail", val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("detail", val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("detail", val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("detail", val));
                    return this;
                }

                public TaskExecutionLogRequest WithDetailSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("detail", val));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public TaskExecutionLogRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public TaskExecutionLogRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public TaskExecutionLogRequest OrderByActionAscending()
                {
                    _query.OrderBy("action", "asc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByActionDescending()
                {
                    _query.OrderBy("action", "desc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByDetailAscending()
                {
                    _query.OrderBy("detail", "asc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByDetailDescending()
                {
                    _query.OrderBy("detail", "desc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public TaskExecutionLogRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
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
                    _query.GroupBy("id");
                    return this;
                }

                public TaskExecutionLogRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id");
                    _query.ProjectExpr(retName, Expr.Column("id"));
                    return this;
                }
                public TaskExecutionLogRequest GroupByTask()
                {
                    _query.GroupBy("task");
                    return this;
                }

                public TaskExecutionLogRequest GroupByTaskAs(string retName)
                {
                    _query.GroupBy("task");
                    _query.ProjectExpr(retName, Expr.Column("task"));
                    return this;
                }
                public TaskExecutionLogRequest GroupByAction()
                {
                    _query.GroupBy("action");
                    return this;
                }

                public TaskExecutionLogRequest GroupByActionAs(string retName)
                {
                    _query.GroupBy("action");
                    _query.ProjectExpr(retName, Expr.Column("action"));
                    return this;
                }
                public TaskExecutionLogRequest GroupByDetail()
                {
                    _query.GroupBy("detail");
                    return this;
                }

                public TaskExecutionLogRequest GroupByDetailAs(string retName)
                {
                    _query.GroupBy("detail");
                    _query.ProjectExpr(retName, Expr.Column("detail"));
                    return this;
                }
                public TaskExecutionLogRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public TaskExecutionLogRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version");
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public TaskExecutionLogRequest FacetByTaskAs(
                    string name, TaskRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Task", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByTaskAs", name, "task")
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
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.TaskExecutionLog.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.TaskExecutionLog>> ExecuteForListWithMetadataInternalAsync(
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
            var entity = Generated.Models.TaskExecutionLog.TeaqlNewWithReservedId(identity);
            return context.InitializeEntity("TaskExecutionLog", entity,
                initialized => initialized.TeaqlValidateReservedIdentity(identity));
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