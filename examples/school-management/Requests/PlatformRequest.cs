using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class PlatformRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public PlatformRequest()
        {
            _query = new SelectQuery("Platform");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public PlatformRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public PlatformRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public PlatformRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutablePlatformRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutablePlatformRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
                c => Comment(c),
                EnsureIntent);
        }

        public PlatformRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public PlatformRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public PlatformRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public PlatformRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public PlatformRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("base_url");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public PlatformRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public PlatformRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public PlatformRequest SelectBaseUrl()
                {
                    _query.Project("base_url");
                    return this;
                }

                public PlatformRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public PlatformRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public PlatformRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }


                public PlatformRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public PlatformRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public PlatformRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public PlatformRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public PlatformRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public PlatformRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }

                public PlatformRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public PlatformRequest WithBaseUrlContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlIs(string val)
                {
                    _query.AndFilter(Expr.Eq("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("base_url", vals));
                    return this;
                }

                public PlatformRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public PlatformRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public PlatformRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public PlatformRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public PlatformRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public PlatformRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public PlatformRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public PlatformRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public PlatformRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public PlatformRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public PlatformRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public PlatformRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public PlatformRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public PlatformRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public PlatformRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public PlatformRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public PlatformRequest OrderByBaseUrlAscending()
                {
                    _query.OrderBy("base_url", "asc");
                    return this;
                }

                public PlatformRequest OrderByBaseUrlDescending()
                {
                    _query.OrderBy("base_url", "desc");
                    return this;
                }

                public PlatformRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public PlatformRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public PlatformRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public PlatformRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public PlatformRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public PlatformRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public PlatformRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public PlatformRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public PlatformRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public PlatformRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public PlatformRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public PlatformRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public PlatformRequest GroupByBaseUrl()
                {
                    _query.GroupBy("base_url");
                    return this;
                }

                public PlatformRequest GroupByBaseUrlAs(string retName)
                {
                    _query.GroupBy("base_url"); 
                    return this;
                }
                public PlatformRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public PlatformRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public PlatformRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public PlatformRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
                    return this;
                }
                public PlatformRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public PlatformRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public PlatformRequest SelectSchoolTypeList()
                {
                    return SelectSchoolTypeListWith(new SchoolTypeRequest());
                }

                public PlatformRequest SelectSchoolTypeListWith(SchoolTypeRequest child)
                {
                    _query.RelationQuery("SchoolTypeList", "SchoolType", "platform", true, child.GetQuery());
                    return this;
                }
                public PlatformRequest SelectSchoolList()
                {
                    return SelectSchoolListWith(new SchoolRequest());
                }

                public PlatformRequest SelectSchoolListWith(SchoolRequest child)
                {
                    _query.RelationQuery("SchoolList", "School", "platform", true, child.GetQuery());
                    return this;
                }

        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireDataService();
            var req = new QueryRequest(_query);
            return await service.QueryAsync(context, req);
        }

        private async Task<PlatformPage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireDataService();
            var countQuery = new SelectQuery("Platform");
            foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
            countQuery.Aggregate("Count", "id", "count");
            var countResult = await service.QueryAsync(context, new QueryRequest(countQuery));
            var totalCount = countResult.Rows.Count == 0
                ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);

            _query.Offset(offset);
            _query.Limit(limit);
            var result = await service.QueryAsync(context, new QueryRequest(_query));
            var rows = new SmartList<Generated.Models.Platform>();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.Platform.FromRecord(row));
            return new PlatformPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamInternalAsync(
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
                    yield return Generated.Models.Platform.FromRecord(row);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutablePlatformRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<PlatformPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePlatformRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<PlatformPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> executeForStream,
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

        public ExecutablePlatformRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.Platform NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.Platform();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.Platform>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.Platform>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.Platform.FromRecord(row));
            return entities;
        }

        public Task<PlatformPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.Platform> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class PlatformPage
    {
        public SmartList<Generated.Models.Platform> Rows { get; }
        public long TotalCount { get; }
        public PlatformPage(
            SmartList<Generated.Models.Platform> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}