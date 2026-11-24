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
            p = QueryIntent.RequirePurpose(p);
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
            _query.Project(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public PaymentRequest SelectId()
                {
                    _query.Project(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public PaymentRequest SelectReferenceCode()
                {
                    _query.Project(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
                    return this;
                }

                public PaymentRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public PaymentRequest SelectCustomerOrder()
                {
                    return SelectCustomerOrderWith(new CustomerOrderRequest());
                }

                public PaymentRequest SelectCustomerOrderWith(CustomerOrderRequest related)
                {
                    _query.Project(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, "CustomerOrder", global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderWith", Relation: global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public PaymentRequest WithCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public PaymentRequest WithoutCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public PaymentRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public PaymentRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public PaymentRequest FilterByCustomerOrder(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentRequest FilterByCustomerOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, vals));
                    return this;
                }

                public PaymentRequest WithCustomerOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public PaymentRequest WithCustomerOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public PaymentRequest WithReferenceCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public PaymentRequest WithReferenceCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithReferenceCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithReferenceCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentRequest WithReferenceCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY));
                    return this;
                }

                public PaymentRequest WithReferenceCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY));
                    return this;
                }
                public PaymentRequest WithReferenceCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentRequest WithReferenceCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, val));
                    return this;
                }

                public PaymentRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PaymentRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public PaymentRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PaymentRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public PaymentRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public PaymentRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PaymentRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public PaymentRequest OrderByReferenceCodeAscending()
                {
                    _query.OrderBy(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PaymentRequest OrderByReferenceCodeDescending()
                {
                    _query.OrderBy(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public PaymentRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public PaymentRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
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
                    _query.GroupBy(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public PaymentRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Payment.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public PaymentRequest GroupByCustomerOrder()
                {
                    _query.GroupBy(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public PaymentRequest GroupByCustomerOrderAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }
                public PaymentRequest GroupByReferenceCode()
                {
                    _query.GroupBy(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
                    return this;
                }

                public PaymentRequest GroupByReferenceCodeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Payment.__TeaqlFields.REFERENCE_CODE_PROPERTY_QUERY);
                    return this;
                }
                public PaymentRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public PaymentRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Payment.__TeaqlFields.VERSION_PROPERTY_QUERY);
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
                        name, global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerOrderAs", name, global::Generated.Models.Payment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY)
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

        private async Task<Generated.Models.Payment?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.Payment>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.Payment>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.Payment>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Payment.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
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
            var request = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            if (request.Query.AggregateItems.Count > 0 || request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Entity streams require entity rows; use a record or aggregation query");
            return RuntimeDataService.StreamEntities(context, request,
                row => global::TeaQL.Runtime.EntityQueryProperties.Capture(
                    Generated.Models.Payment.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutablePaymentRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Payment?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Payment>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Payment>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePaymentRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Payment?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Payment>>> executeForList,
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

        public async Task<Generated.Models.Payment> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Payment").ConfigureAwait(false));
            return context.InitializeEntity("Payment", Generated.Models.Payment.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Payment>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Payment> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Payment?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}