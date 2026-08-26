using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class SchoolTypeRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public SchoolTypeRequest()
        {
            _query = new SelectQuery("SchoolType");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

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
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableSchoolTypeRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
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
            _query.Project("platform");
            _query.Project("id");
            _query.Project("name");
            _query.Project("code");
            _query.Project("display_order");
            _query.Project("version");
            return this;
        }

                public SchoolTypeRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public SchoolTypeRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public SchoolTypeRequest SelectCode()
                {
                    _query.Project("code");
                    return this;
                }

                public SchoolTypeRequest SelectDisplayOrder()
                {
                    _query.Project("display_order");
                    return this;
                }

                public SchoolTypeRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public SchoolTypeRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public SchoolTypeRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project("platform");
                    _query.ForwardRelationQuery("Platform", "Platform", "platform", related.GetQuery());
                    return this;
                }

                public SchoolTypeRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("platform", val));
                    return this;
                }

                public SchoolTypeRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("platform", vals));
                    return this;
                }

                public SchoolTypeRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public SchoolTypeRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public SchoolTypeRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public SchoolTypeRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public SchoolTypeRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public SchoolTypeRequest WithCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("code", vals));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIs(object val)
                {
                    _query.AndFilter(Expr.Eq("display_order", val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("display_order", vals));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("display_order", val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("display_order", val));
                    return this;
                }

                public SchoolTypeRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public SchoolTypeRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public SchoolTypeRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public SchoolTypeRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public SchoolTypeRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByCodeAscending()
                {
                    _query.OrderBy("code", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByCodeDescending()
                {
                    _query.OrderBy("code", "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByDisplayOrderAscending()
                {
                    _query.OrderBy("display_order", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByDisplayOrderDescending()
                {
                    _query.OrderBy("display_order", "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public SchoolTypeRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public SchoolTypeRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public SchoolTypeRequest minDisplayOrder()
                {
                    return minDisplayOrderAs("minOfDisplayOrder");
                }

                public SchoolTypeRequest minDisplayOrderAs(string retName)
                {
                    _query.Aggregate("min", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest maxDisplayOrder()
                {
                    return maxDisplayOrderAs("maxOfDisplayOrder");
                }

                public SchoolTypeRequest maxDisplayOrderAs(string retName)
                {
                    _query.Aggregate("max", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest sumDisplayOrder()
                {
                    return sumDisplayOrderAs("sumOfDisplayOrder");
                }

                public SchoolTypeRequest sumDisplayOrderAs(string retName)
                {
                    _query.Aggregate("sum", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest avgDisplayOrder()
                {
                    return avgDisplayOrderAs("avgOfDisplayOrder");
                }

                public SchoolTypeRequest avgDisplayOrderAs(string retName)
                {
                    _query.Aggregate("avg", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest standardDeviationDisplayOrder()
                {
                    return standardDeviationDisplayOrderAs("standardDeviationOfDisplayOrder");
                }

                public SchoolTypeRequest standardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest squareRootOfPopulationStandardDeviationDisplayOrder()
                {
                    return squareRootOfPopulationStandardDeviationDisplayOrderAs("squareRootOfPopulationStandardDeviationOfDisplayOrder");
                }

                public SchoolTypeRequest squareRootOfPopulationStandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest sampleVarianceDisplayOrder()
                {
                    return sampleVarianceDisplayOrderAs("sampleVarianceOfDisplayOrder");
                }

                public SchoolTypeRequest sampleVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_samp", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest samplePopulationVarianceDisplayOrder()
                {
                    return samplePopulationVarianceDisplayOrderAs("samplePopulationVarianceOfDisplayOrder");
                }

                public SchoolTypeRequest samplePopulationVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_pop", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest GroupByPlatform()
                {
                    _query.GroupBy("platform");
                    return this;
                }

                public SchoolTypeRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy("platform"); 
                    return this;
                }
                public SchoolTypeRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public SchoolTypeRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public SchoolTypeRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public SchoolTypeRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public SchoolTypeRequest GroupByCode()
                {
                    _query.GroupBy("code");
                    return this;
                }

                public SchoolTypeRequest GroupByCodeAs(string retName)
                {
                    _query.GroupBy("code"); 
                    return this;
                }
                public SchoolTypeRequest GroupByDisplayOrder()
                {
                    _query.GroupBy("display_order");
                    return this;
                }

                public SchoolTypeRequest GroupByDisplayOrderAs(string retName)
                {
                    _query.GroupBy("display_order"); 
                    return this;
                }
                public SchoolTypeRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public SchoolTypeRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public SchoolTypeRequest SelectSchoolList()
                {
                    return SelectSchoolListWith(new SchoolRequest());
                }

                public SchoolTypeRequest SelectSchoolListWith(SchoolRequest child)
                {
                    _query.RelationQuery("SchoolList", "School", "school_type", true, child.GetQuery());
                    return this;
                }

        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireDataService();
            var req = new QueryRequest(_query);
            return await service.QueryAsync(context, req);
        }

        private async Task<SchoolTypePage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireDataService();
            var countQuery = new SelectQuery("SchoolType");
            foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
            countQuery.Aggregate("Count", "id", "count");
            var countResult = await service.QueryAsync(context, new QueryRequest(countQuery));
            var totalCount = countResult.Rows.Count == 0
                ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);

            _query.Offset(offset);
            _query.Limit(limit);
            var result = await service.QueryAsync(context, new QueryRequest(_query));
            var rows = new SmartList<Generated.Models.SchoolType>();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.SchoolType.FromRecord(row));
            return new SchoolTypePage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.SchoolType> ExecuteForStreamInternalAsync(
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
                    yield return Generated.Models.SchoolType.FromRecord(row);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableSchoolTypeRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<SchoolTypePage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.SchoolType>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableSchoolTypeRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<SchoolTypePage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.SchoolType>> executeForStream,
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

        public ExecutableSchoolTypeRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.SchoolType NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.SchoolType();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.SchoolType>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.SchoolType>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.SchoolType.FromRecord(row));
            return entities;
        }

        public Task<SchoolTypePage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.SchoolType> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.SchoolType> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class SchoolTypePage
    {
        public SmartList<Generated.Models.SchoolType> Rows { get; }
        public long TotalCount { get; }
        public SchoolTypePage(
            SmartList<Generated.Models.SchoolType> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}