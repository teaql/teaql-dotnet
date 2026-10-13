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
    public class PaymentRequest
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

        public PaymentRequest()
        {
            _query = new SelectQuery("Payment");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public PaymentRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public PaymentRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public PaymentRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public PaymentRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutablePaymentRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutablePaymentRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public PaymentRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public PaymentRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public PaymentRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public PaymentRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public PaymentRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public PaymentRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public PaymentRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public PaymentRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("customer_order");
            _query.Project("reference_code");
            _query.Project("version");
            return this;
        }

                public PaymentRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public PaymentRequest SelectReferenceCode()
                {
                    _query.Project("reference_code");
                    return this;
                }

                public PaymentRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public PaymentRequest SelectCustomerOrder()
                {
                    return SelectCustomerOrderWith(new CustomerOrderRequest());
                }

                public PaymentRequest SelectCustomerOrderWith(CustomerOrderRequest related)
                {
                    _query.Project("customer_order");
                    _query.ForwardRelationQuery("CustomerOrder", "CustomerOrder", "customer_order", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderWith", Relation: "CustomerOrder")
                    };
                    return this;
                }
                public PaymentRequest WithCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("customer_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public PaymentRequest WithoutCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("customer_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public PaymentRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public PaymentRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public PaymentRequest FilterByCustomerOrder(object val)
                {
                    _query.AndFilter(Expr.Eq("customer_order", val));
                    return this;
                }

                public PaymentRequest FilterByCustomerOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("customer_order", vals));
                    return this;
                }

                public PaymentRequest WithCustomerOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("customer_order"));
                    return this;
                }

                public PaymentRequest WithCustomerOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("customer_order"));
                    return this;
                }

                public PaymentRequest WithReferenceCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("reference_code", val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq("reference_code", TeaqlQueryValue(val)));
                    return this;
                }
                public PaymentRequest WithReferenceCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("reference_code", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithReferenceCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("reference_code", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithReferenceCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("reference_code", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("reference_code", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("reference_code"));
                    return this;
                }

                public PaymentRequest WithReferenceCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("reference_code"));
                    return this;
                }
                public PaymentRequest WithReferenceCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("reference_code", val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("reference_code", val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("reference_code", val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("reference_code", val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("reference_code", val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("reference_code", val));
                    return this;
                }

                public PaymentRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public PaymentRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public PaymentRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public PaymentRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public PaymentRequest OrderByReferenceCodeAscending()
                {
                    _query.OrderBy("reference_code", "asc");
                    return this;
                }

                public PaymentRequest OrderByReferenceCodeDescending()
                {
                    _query.OrderBy("reference_code", "desc");
                    return this;
                }

                public PaymentRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public PaymentRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public PaymentRequest Count()
        {
            return CountAs("count");
        }

        public PaymentRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public PaymentRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public PaymentRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public PaymentRequest GroupByCustomerOrder()
                {
                    _query.GroupBy("customer_order");
                    return this;
                }

                public PaymentRequest GroupByCustomerOrderAs(string retName)
                {
                    _query.GroupBy("customer_order"); 
                    return this;
                }
                public PaymentRequest GroupByReferenceCode()
                {
                    _query.GroupBy("reference_code");
                    return this;
                }

                public PaymentRequest GroupByReferenceCodeAs(string retName)
                {
                    _query.GroupBy("reference_code"); 
                    return this;
                }
                public PaymentRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public PaymentRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public PaymentRequest SelectPaymentAttemptList()
                {
                    return SelectPaymentAttemptListWith(new PaymentAttemptRequest());
                }

                public PaymentRequest SelectPaymentAttemptListWith(PaymentAttemptRequest child)
                {
                    _query.RelationQuery("PaymentAttemptList", "PaymentAttempt", "payment", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPaymentAttemptListWith", Relation: "PaymentAttemptList")
                    };
                    return this;
                }
                public PaymentRequest HavePaymentAttempts()
                    => WithPaymentAttemptListMatching(new PaymentAttemptRequest());

                public PaymentRequest HaveNoPaymentAttempts()
                    => WithoutPaymentAttemptListMatching(new PaymentAttemptRequest());

                public PaymentRequest WithPaymentAttemptListMatching(PaymentAttemptRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("PaymentAttempt")!, child.GetQuery(), "payment"));
                    return this;
                }

                public PaymentRequest WithoutPaymentAttemptListMatching(PaymentAttemptRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("PaymentAttempt")!, child.GetQuery(), "payment"));
                    return this;
                }
                public PaymentRequest CountPaymentAttempts()
                    => CountPaymentAttemptsAs("countPaymentAttempts");

                public PaymentRequest CountPaymentAttemptsAs(string alias)
                {
                    CountPaymentAttemptsWith(alias, new PaymentAttemptRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountPaymentAttemptsAs", alias, "PaymentAttemptList")
                    };
                    return this;
                }

                public PaymentRequest CountPaymentAttemptsWith(string alias, PaymentAttemptRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("PaymentAttemptList", "PaymentAttempt", "payment", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountPaymentAttemptsWith", alias, "PaymentAttemptList")
                    };
                    return this;
                }


                public PaymentRequest FacetByCustomerOrderAs(
                    string name, CustomerOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "CustomerOrder", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerOrderAs", name, "customer_order")
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

        private IAsyncEnumerable<Generated.Models.Payment> ExecuteForStreamInternalAsync(
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
                row => Generated.Models.Payment.FromRecord(row, new EntityRoot()),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutablePaymentRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<QueryResult>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Record>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Payment>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePaymentRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<QueryResult>> executeForOne,
            Func<UserContext, Task<SmartList<Record>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Payment>> executeForStream,
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

        public ExecutablePaymentRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.Payment NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.Payment();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.Payment>> ExecuteForListAsync(UserContext context)
        {
            var result = await _executeForList(context);
            var entities = new SmartList<Generated.Models.Payment>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            var queryRoot = new EntityRoot();
            foreach (var row in result)
                entities.Add(Generated.Models.Payment.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public IAsyncEnumerable<Generated.Models.Payment> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.Payment?> ExecuteForOneAsync(
            UserContext context)
        {
            var result = await _executeForOne(context);
            if (result.Rows.Count == 0) return null;
            return Generated.Models.Payment.FromRecord(
                result.Rows[0], new EntityRoot());
        }
    }

}