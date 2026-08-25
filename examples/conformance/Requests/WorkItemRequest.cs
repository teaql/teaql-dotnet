using System;
using System.Collections.Generic;
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

        public WorkItemRequest WithIdIs(object val)
        {
            _query.AndFilter(Expr.Eq("id", val));
            return this;
        }

        public WorkItemRequest WithIdIn(params object[] vals)
        {
            _query.AndFilter(Expr.In("id", vals));
            return this;
        }

        public WorkItemRequest WithIdGreaterThanOrEqualTo(object val)
        {
            _query.AndFilter(Expr.Gte("id", val));
            return this;
        }

        public WorkItemRequest WithIdLessThanOrEqualTo(object val)
        {
            _query.AndFilter(Expr.Lte("id", val));
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

        public WorkItemRequest WithTitleIn(params object[] vals)
        {
            _query.AndFilter(Expr.In("title", vals));
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

        public WorkItemRequest WithDescriptionIn(params object[] vals)
        {
            _query.AndFilter(Expr.In("description", vals));
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

        public WorkItemRequest WithVersionIs(object val)
        {
            _query.AndFilter(Expr.Eq("version", val));
            return this;
        }

        public WorkItemRequest WithVersionIn(params object[] vals)
        {
            _query.AndFilter(Expr.In("version", vals));
            return this;
        }

        public WorkItemRequest WithVersionGreaterThanOrEqualTo(object val)
        {
            _query.AndFilter(Expr.Gte("version", val));
            return this;
        }

        public WorkItemRequest WithVersionLessThanOrEqualTo(object val)
        {
            _query.AndFilter(Expr.Lte("version", val));
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

        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireDataService();
            var req = new QueryRequest(_query);
            return await service.QueryAsync(context, req);
        }

        private async Task<WorkItemPage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireDataService();
            var countQuery = new SelectQuery("WorkItem");
            foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
            countQuery.Aggregate("Count", "id", "count");
            var countResult = await service.QueryAsync(context, new QueryRequest(countQuery));
            var totalCount = countResult.Rows.Count == 0
                ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);

            _query.Offset(offset);
            _query.Limit(limit);
            var result = await service.QueryAsync(context, new QueryRequest(_query));
            var rows = new SmartList<Generated.Models.WorkItem>();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.WorkItem.FromRecord(row));
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
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.WorkItem.FromRecord(row);
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
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.WorkItem.FromRecord(row));
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