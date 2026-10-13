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
    public class PaymentAttemptRequest
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

        public PaymentAttemptRequest()
        {
            _query = new SelectQuery("PaymentAttempt");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public PaymentAttemptRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public PaymentAttemptRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public PaymentAttemptRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public PaymentAttemptRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutablePaymentAttemptRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutablePaymentAttemptRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public PaymentAttemptRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public PaymentAttemptRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public PaymentAttemptRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public PaymentAttemptRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public PaymentAttemptRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public PaymentAttemptRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public PaymentAttemptRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public PaymentAttemptRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("payment");
            _query.Project("reference_code");
            _query.Project("version");
            return this;
        }

                public PaymentAttemptRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public PaymentAttemptRequest SelectReferenceCode()
                {
                    _query.Project("reference_code");
                    return this;
                }

                public PaymentAttemptRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public PaymentAttemptRequest SelectPayment()
                {
                    return SelectPaymentWith(new PaymentRequest());
                }

                public PaymentAttemptRequest SelectPaymentWith(PaymentRequest related)
                {
                    _query.Project("payment");
                    _query.ForwardRelationQuery("Payment", "Payment", "payment", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPaymentWith", Relation: "Payment")
                    };
                    return this;
                }
                public PaymentAttemptRequest WithPaymentMatching(PaymentRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("payment", GeneratedRuntimeModule.Module.Metadata.GetEntity("Payment")!, related.GetQuery(), "id"));
                    return this;
                }

                public PaymentAttemptRequest WithoutPaymentMatching(PaymentRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("payment", GeneratedRuntimeModule.Module.Metadata.GetEntity("Payment")!, related.GetQuery(), "id"));
                    return this;
                }

                public PaymentAttemptRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentAttemptRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public PaymentAttemptRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public PaymentAttemptRequest FilterByPayment(object val)
                {
                    _query.AndFilter(Expr.Eq("payment", val));
                    return this;
                }

                public PaymentAttemptRequest FilterByPaymentIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("payment", vals));
                    return this;
                }

                public PaymentAttemptRequest WithPaymentIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("payment"));
                    return this;
                }

                public PaymentAttemptRequest WithPaymentIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("payment"));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("reference_code", val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq("reference_code", TeaqlQueryValue(val)));
                    return this;
                }
                public PaymentAttemptRequest WithReferenceCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("reference_code", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("reference_code", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("reference_code", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("reference_code"));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("reference_code"));
                    return this;
                }
                public PaymentAttemptRequest WithReferenceCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("reference_code", val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("reference_code", val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("reference_code", val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("reference_code", val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("reference_code", val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("reference_code", val));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public PaymentAttemptRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public PaymentAttemptRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public PaymentAttemptRequest OrderByReferenceCodeAscending()
                {
                    _query.OrderBy("reference_code", "asc");
                    return this;
                }

                public PaymentAttemptRequest OrderByReferenceCodeDescending()
                {
                    _query.OrderBy("reference_code", "desc");
                    return this;
                }

                public PaymentAttemptRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public PaymentAttemptRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public PaymentAttemptRequest Count()
        {
            return CountAs("count");
        }

        public PaymentAttemptRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public PaymentAttemptRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public PaymentAttemptRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public PaymentAttemptRequest GroupByPayment()
                {
                    _query.GroupBy("payment");
                    return this;
                }

                public PaymentAttemptRequest GroupByPaymentAs(string retName)
                {
                    _query.GroupBy("payment"); 
                    return this;
                }
                public PaymentAttemptRequest GroupByReferenceCode()
                {
                    _query.GroupBy("reference_code");
                    return this;
                }

                public PaymentAttemptRequest GroupByReferenceCodeAs(string retName)
                {
                    _query.GroupBy("reference_code"); 
                    return this;
                }
                public PaymentAttemptRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public PaymentAttemptRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public PaymentAttemptRequest FacetByPaymentAs(
                    string name, PaymentRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Payment", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPaymentAs", name, "payment")
                    });
                    return this;
                }


        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var request = context.PrepareQueryRequest(new QueryRequest(_query));
            return await service.QueryAsync(request);
        }

        private async Task<QueryResult> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            return await service.QueryAsync(context.PrepareQueryRequest(new QueryRequest(query)));
        }

        private async Task<SmartList<Record>> ExecuteForListWithMetadataInternalAsync(
            UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            var request = context.PrepareQueryRequest(new QueryRequest(query));
            if (_countRequested)
                return await RuntimeDataService.FetchPageAsync(service, context, request);
            var result = await service.QueryAsync(request);
            return new SmartList<Record>(result.Rows) { Facets = result.Facets };
        }

        private IAsyncEnumerable<Generated.Models.PaymentAttempt> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var request = context.PrepareQueryRequest(new QueryRequest(_query));
            return RuntimeDataService.StreamEntities(context, request,
                row => Generated.Models.PaymentAttempt.FromRecord(row, new EntityRoot()),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutablePaymentAttemptRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<QueryResult>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Record>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.PaymentAttempt>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePaymentAttemptRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<QueryResult>> executeForOne,
            Func<UserContext, Task<SmartList<Record>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.PaymentAttempt>> executeForStream,
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

        public ExecutablePaymentAttemptRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.PaymentAttempt NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.PaymentAttempt();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.PaymentAttempt>> ExecuteForListAsync(UserContext context)
        {
            var result = await _executeForList(context);
            var entities = new SmartList<Generated.Models.PaymentAttempt>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            var queryRoot = new EntityRoot();
            foreach (var row in result)
                entities.Add(Generated.Models.PaymentAttempt.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public IAsyncEnumerable<Generated.Models.PaymentAttempt> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.PaymentAttempt?> ExecuteForOneAsync(
            UserContext context)
        {
            var result = await _executeForOne(context);
            if (result.Rows.Count == 0) return null;
            return Generated.Models.PaymentAttempt.FromRecord(
                result.Rows[0], new EntityRoot());
        }
    }

}