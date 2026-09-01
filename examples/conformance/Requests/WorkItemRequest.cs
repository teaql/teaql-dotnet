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
    public class WorkItemRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public WorkItemRequest()
        {
            _query = new SelectQuery("WorkItem");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

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
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableWorkItemRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
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
                    return this;
                }
                public WorkItemRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("platform", EntityDescriptor.New("Platform"), related.GetQuery(), "id"));
                    return this;
                }

                public WorkItemRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("platform", EntityDescriptor.New("Platform"), related.GetQuery(), "id"));
                    return this;
                }

                public WorkItemRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public WorkItemRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public WorkItemRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public WorkItemRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public WorkItemRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public WorkItemRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public WorkItemRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public WorkItemRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public WorkItemRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
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
                    _query.AndFilter(Expr.Eq("title", val));
                    return this;
                }
                public WorkItemRequest WithTitleIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("title", vals));
                    return this;
                }

                public WorkItemRequest WithTitleNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("title", vals));
                    return this;
                }

                public WorkItemRequest WithTitleGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("title", val));
                    return this;
                }

                public WorkItemRequest WithTitleBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("title", lower, upper));
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
                    _query.AndFilter(Expr.Eq("description", val));
                    return this;
                }
                public WorkItemRequest WithDescriptionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("description", vals));
                    return this;
                }

                public WorkItemRequest WithDescriptionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("description", vals));
                    return this;
                }

                public WorkItemRequest WithDescriptionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("description", val));
                    return this;
                }

                public WorkItemRequest WithDescriptionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("description", lower, upper));
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

                public WorkItemRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public WorkItemRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public WorkItemRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public WorkItemRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public WorkItemRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public WorkItemRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public WorkItemRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public WorkItemRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public WorkItemRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
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
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public WorkItemRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
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
                    return this;
                }
                public WorkItemRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "platform", request.GetQuery(), includeAllFacets));
                    return this;
                }


        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireDataService();
            var req = new QueryRequest(_query);
            var result = await service.QueryAsync(context, req);
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
                var membershipRows = (await service.QueryAsync(context, new QueryRequest(membership))).Rows;
                var counts = membershipRows
                    .Where(row => row.TryGetValue(facet.RelationName, out var value) && value.Raw != null)
                    .GroupBy(row => Convert.ToString(row[facet.RelationName].Raw))
                    .ToDictionary(group => group.Key, group => group.Count());

                var nested = facet.Query.Copy();
                nested.Facets.Clear();
                var countAliases = nested.Aggregates
                    .Where(aggregate => string.Equals(aggregate.Function, "Count", StringComparison.OrdinalIgnoreCase))
                    .Select(aggregate => aggregate.Alias).ToArray();
                nested.Aggregates.Clear();
                nested.GroupFields.Clear();
                var facetRows = (await service.QueryAsync(context, new QueryRequest(nested))).Rows;
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

        private async Task<WorkItemPage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireDataService();
            _query.Offset(offset);
            _query.Limit(limit);
            var result = await service.QueryAsync(context, new QueryRequest(_query));
            long totalCount;
            if (_query.IdSetPagination != null && context.IdSetCountAccuracy == "EXACT")
            {
                totalCount = context.IdSetCount;
            }
            else
            {
                var countQuery = new SelectQuery("WorkItem");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(context, new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.WorkItem>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.WorkItem.FromRecord(row, queryRoot));
            return new WorkItemPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.WorkItem> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireDataService();
            if (service is not IStreamingDataService streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            await foreach (var chunk in streaming.QueryStreamAsync(
                context, new QueryRequest(_query), chunkSize, cancellationToken).WithCancellation(cancellationToken))
            {
                var queryRoot = new EntityRoot();
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.WorkItem.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableWorkItemRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<WorkItemPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.WorkItem>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableWorkItemRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<WorkItemPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.WorkItem>> executeForStream,
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

        public ExecutableWorkItemRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.WorkItem NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.WorkItem();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.WorkItem>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.WorkItem>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.WorkItem.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<WorkItemPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.WorkItem> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.WorkItem> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class WorkItemPage
    {
        public SmartList<Generated.Models.WorkItem> Rows { get; }
        public long TotalCount { get; }
        public WorkItemPage(
            SmartList<Generated.Models.WorkItem> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}