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
            p = QueryIntent.RequirePurpose(p);
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
            _query.Project(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public PaymentAttemptRequest SelectId()
                {
                    _query.Project(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public PaymentAttemptRequest SelectReferenceCode()
                {
                    _query.Project(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
                    return this;
                }

                public PaymentAttemptRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public PaymentAttemptRequest SelectPayment()
                {
                    return SelectPaymentWith(new PaymentRequest());
                }

                public PaymentAttemptRequest SelectPaymentWith(PaymentRequest related)
                {
                    _query.Project(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_MEMBER, "Payment", global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPaymentWith", Relation: global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public PaymentAttemptRequest WithPaymentMatching(PaymentRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Payment")!, related.GetQuery(), "id"));
                    return this;
                }

                public PaymentAttemptRequest WithoutPaymentMatching(PaymentRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Payment")!, related.GetQuery(), "id"));
                    return this;
                }

                public PaymentAttemptRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentAttemptRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public PaymentAttemptRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public PaymentAttemptRequest FilterByPayment(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentAttemptRequest FilterByPaymentIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY, vals));
                    return this;
                }

                public PaymentAttemptRequest WithPaymentIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY));
                    return this;
                }

                public PaymentAttemptRequest WithPaymentIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public PaymentAttemptRequest WithReferenceCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY));
                    return this;
                }
                public PaymentAttemptRequest WithReferenceCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentAttemptRequest WithReferenceCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentAttemptRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public PaymentAttemptRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public PaymentAttemptRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PaymentAttemptRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public PaymentAttemptRequest OrderByReferenceCodeAscending()
                {
                    _query.OrderBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PaymentAttemptRequest OrderByReferenceCodeDescending()
                {
                    _query.OrderBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public PaymentAttemptRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PaymentAttemptRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
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
                    _query.GroupBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public PaymentAttemptRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public PaymentAttemptRequest GroupByPayment()
                {
                    _query.GroupBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY);
                    return this;
                }

                public PaymentAttemptRequest GroupByPaymentAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY);
                    return this;
                }
                public PaymentAttemptRequest GroupByReferenceCode()
                {
                    _query.GroupBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
                    return this;
                }

                public PaymentAttemptRequest GroupByReferenceCodeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
                    return this;
                }
                public PaymentAttemptRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public PaymentAttemptRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.PaymentAttempt.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public PaymentAttemptRequest FacetByPaymentAs(
                    string name, PaymentRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPaymentAs", name, global::Generated.Models.PaymentAttempt.__TeaqlFields.PAYMENT_PROPERTY_QUERY)
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

        private async Task<Generated.Models.PaymentAttempt?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.PaymentAttempt>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.PaymentAttempt>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.PaymentAttempt>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.PaymentAttempt.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
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
            var request = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            if (request.Query.AggregateItems.Count > 0 || request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Entity streams require entity rows; use a record or aggregation query");
            return RuntimeDataService.StreamEntities(context, request,
                row => global::TeaQL.Runtime.EntityQueryProperties.Capture(
                    Generated.Models.PaymentAttempt.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutablePaymentAttemptRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.PaymentAttempt?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.PaymentAttempt>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.PaymentAttempt>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePaymentAttemptRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.PaymentAttempt?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.PaymentAttempt>>> executeForList,
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

        public async Task<Generated.Models.PaymentAttempt> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("PaymentAttempt").ConfigureAwait(false));
            return context.InitializeEntity("PaymentAttempt", Generated.Models.PaymentAttempt.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.PaymentAttempt>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.PaymentAttempt> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.PaymentAttempt?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}