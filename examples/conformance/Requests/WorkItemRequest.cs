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
    public class WorkItemRequest
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

        public WorkItemRequest()
        {
            _query = new SelectQuery("WorkItem");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public WorkItemRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public WorkItemRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public WorkItemRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public WorkItemRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableWorkItemRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableWorkItemRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public WorkItemRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public WorkItemRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public WorkItemRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public WorkItemRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public WorkItemRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public WorkItemRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public WorkItemRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public WorkItemRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("title");
            _query.Project("description");
            _query.Project("platform");
            _query.Project("version");
            return this;
        }

                public WorkItemRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public WorkItemRequest SelectTitle()
                {
                    _query.Project("title");
                    return this;
                }

                public WorkItemRequest SelectDescription()
                {
                    _query.Project("description");
                    return this;
                }


                public WorkItemRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public WorkItemRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public WorkItemRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project("platform");
                    _query.ForwardRelationQuery("Platform", "Platform", "platform", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: "Platform")
                    };
                    return this;
                }
                public WorkItemRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public WorkItemRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public WorkItemRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public WorkItemRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public WorkItemRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public WorkItemRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public WorkItemRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public WorkItemRequest WithTitleContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleIs(string val)
                {
                    _query.AndFilter(Expr.Eq("title", TeaqlQueryValue(val)));
                    return this;
                }
                public WorkItemRequest WithTitleIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("title", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("title", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public WorkItemRequest WithTitleNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("title", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public WorkItemRequest WithTitleGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("title", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("title", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("title", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("title", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("title", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public WorkItemRequest WithTitleIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("title"));
                    return this;
                }

                public WorkItemRequest WithTitleIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("title"));
                    return this;
                }
                public WorkItemRequest WithTitleNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("title", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionIs(string val)
                {
                    _query.AndFilter(Expr.Eq("description", TeaqlQueryValue(val)));
                    return this;
                }
                public WorkItemRequest WithDescriptionIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("description", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("description", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public WorkItemRequest WithDescriptionNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("description", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public WorkItemRequest WithDescriptionGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("description", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("description", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("description", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("description", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("description", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public WorkItemRequest WithDescriptionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("description"));
                    return this;
                }

                public WorkItemRequest WithDescriptionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("description"));
                    return this;
                }
                public WorkItemRequest WithDescriptionNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("description", val));
                    return this;
                }

                public WorkItemRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("platform", val));
                    return this;
                }

                public WorkItemRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("platform", vals));
                    return this;
                }

                public WorkItemRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("platform"));
                    return this;
                }

                public WorkItemRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("platform"));
                    return this;
                }

                public WorkItemRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public WorkItemRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public WorkItemRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public WorkItemRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public WorkItemRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public WorkItemRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public WorkItemRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public WorkItemRequest OrderByTitleAscending()
                {
                    _query.OrderBy("title", "asc");
                    return this;
                }

                public WorkItemRequest OrderByTitleDescending()
                {
                    _query.OrderBy("title", "desc");
                    return this;
                }

                public WorkItemRequest OrderByDescriptionAscending()
                {
                    _query.OrderBy("description", "asc");
                    return this;
                }

                public WorkItemRequest OrderByDescriptionDescending()
                {
                    _query.OrderBy("description", "desc");
                    return this;
                }


                public WorkItemRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public WorkItemRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public WorkItemRequest Count()
        {
            return CountAs("count");
        }

        public WorkItemRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public WorkItemRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public WorkItemRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id");
                    _query.ProjectExpr(retName, Expr.Column("id"));
                    return this;
                }
                public WorkItemRequest GroupByTitle()
                {
                    _query.GroupBy("title");
                    return this;
                }

                public WorkItemRequest GroupByTitleAs(string retName)
                {
                    _query.GroupBy("title");
                    _query.ProjectExpr(retName, Expr.Column("title"));
                    return this;
                }
                public WorkItemRequest GroupByDescription()
                {
                    _query.GroupBy("description");
                    return this;
                }

                public WorkItemRequest GroupByDescriptionAs(string retName)
                {
                    _query.GroupBy("description");
                    _query.ProjectExpr(retName, Expr.Column("description"));
                    return this;
                }
                public WorkItemRequest GroupByPlatform()
                {
                    _query.GroupBy("platform");
                    return this;
                }

                public WorkItemRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy("platform");
                    _query.ProjectExpr(retName, Expr.Column("platform"));
                    return this;
                }
                public WorkItemRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public WorkItemRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version");
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public WorkItemRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Platform", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, "platform")
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

        private async Task<Generated.Models.WorkItem?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.WorkItem.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.WorkItem>> ExecuteForListWithMetadataInternalAsync(
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
            var entities = new SmartList<Generated.Models.WorkItem>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.WorkItem.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.WorkItem> ExecuteForStreamInternalAsync(
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
                    Generated.Models.WorkItem.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableWorkItemRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.WorkItem?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.WorkItem>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.WorkItem>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableWorkItemRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.WorkItem?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.WorkItem>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.WorkItem>> executeForStream,
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

        public ExecutableWorkItemRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.WorkItem> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("WorkItem").ConfigureAwait(false));
            var entity = Generated.Models.WorkItem.TeaqlNewWithReservedId(identity);
            return context.InitializeEntity("WorkItem", entity,
                initialized => initialized.TeaqlValidateReservedIdentity(identity));
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.WorkItem>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.WorkItem> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.WorkItem?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}