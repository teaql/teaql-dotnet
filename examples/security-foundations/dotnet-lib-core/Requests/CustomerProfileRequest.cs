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
    public class CustomerProfileRequest
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

        public CustomerProfileRequest()
        {
            _query = new SelectQuery("CustomerProfile");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public CustomerProfileRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public CustomerProfileRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public CustomerProfileRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public CustomerProfileRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCustomerProfileRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerProfileRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public CustomerProfileRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public CustomerProfileRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public CustomerProfileRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public CustomerProfileRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public CustomerProfileRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public CustomerProfileRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public CustomerProfileRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public CustomerProfileRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public CustomerProfileRequest SelectId()
                {
                    _query.Project(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public CustomerProfileRequest SelectEmail()
                {
                    _query.Project(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY);
                    return this;
                }

                public CustomerProfileRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public CustomerProfileRequest SelectMerchant()
                {
                    return SelectMerchantWith(new MerchantRequest());
                }

                public CustomerProfileRequest SelectMerchantWith(MerchantRequest related)
                {
                    _query.Project(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_MEMBER, "Merchant", global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectMerchantWith", Relation: global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public CustomerProfileRequest WithMerchantMatching(MerchantRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Merchant")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerProfileRequest WithoutMerchantMatching(MerchantRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Merchant")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerProfileRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerProfileRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerProfileRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerProfileRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public CustomerProfileRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public CustomerProfileRequest FilterByMerchant(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerProfileRequest FilterByMerchantIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY, vals));
                    return this;
                }

                public CustomerProfileRequest WithMerchantIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY));
                    return this;
                }

                public CustomerProfileRequest WithMerchantIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY));
                    return this;
                }

                public CustomerProfileRequest WithEmailContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerProfileRequest WithEmailIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public CustomerProfileRequest WithEmailIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithEmailIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerProfileRequest WithEmailNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerProfileRequest WithEmailGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithEmailGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithEmailLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithEmailLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithEmailBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerProfileRequest WithEmailIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY));
                    return this;
                }

                public CustomerProfileRequest WithEmailIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY));
                    return this;
                }
                public CustomerProfileRequest WithEmailNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerProfileRequest WithEmailStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerProfileRequest WithEmailNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerProfileRequest WithEmailEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerProfileRequest WithEmailNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerProfileRequest WithEmailSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerProfileRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerProfileRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerProfileRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerProfileRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerProfileRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public CustomerProfileRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public CustomerProfileRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerProfileRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public CustomerProfileRequest OrderByEmailAscending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerProfileRequest OrderByEmailDescending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY, "desc");
                    return this;
                }

                public CustomerProfileRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerProfileRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public CustomerProfileRequest Count()
        {
            return CountAs("count");
        }

        public CustomerProfileRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public CustomerProfileRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public CustomerProfileRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerProfile.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public CustomerProfileRequest GroupByMerchant()
                {
                    _query.GroupBy(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY);
                    return this;
                }

                public CustomerProfileRequest GroupByMerchantAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY);
                    return this;
                }
                public CustomerProfileRequest GroupByEmail()
                {
                    _query.GroupBy(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY);
                    return this;
                }

                public CustomerProfileRequest GroupByEmailAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerProfile.__TeaqlFields.EMAIL_PROPERTY_QUERY);
                    return this;
                }
                public CustomerProfileRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public CustomerProfileRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerProfile.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public CustomerProfileRequest SelectCustomerOrderList()
                {
                    return SelectCustomerOrderListWith(new CustomerOrderRequest());
                }

                public CustomerProfileRequest SelectCustomerOrderListWith(CustomerOrderRequest child)
                {
                    _query.RelationQuery("CustomerOrderList", "CustomerOrder", "customer_profile", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderListWith", Relation: "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerProfileRequest HaveCustomerOrders()
                    => WithCustomerOrderListMatching(new CustomerOrderRequest());

                public CustomerProfileRequest HaveNoCustomerOrders()
                    => WithoutCustomerOrderListMatching(new CustomerOrderRequest());

                public CustomerProfileRequest WithCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "customer_profile"));
                    return this;
                }

                public CustomerProfileRequest WithoutCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "customer_profile"));
                    return this;
                }
                public CustomerProfileRequest CountCustomerOrders()
                    => CountCustomerOrdersAs("countCustomerOrders");

                public CustomerProfileRequest CountCustomerOrdersAs(string alias)
                {
                    CountCustomerOrdersWith(alias, new CustomerOrderRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }

                public CustomerProfileRequest CountCustomerOrdersWith(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer_profile", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersWith", alias, "CustomerOrderList")
                    };
                    return this;
                }


                public CustomerProfileRequest FacetByMerchantAs(
                    string name, MerchantRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByMerchantAs", name, global::Generated.Models.CustomerProfile.__TeaqlFields.MERCHANT_PROPERTY_QUERY)
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

        private async Task<Generated.Models.CustomerProfile?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.CustomerProfile>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.CustomerProfile>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.CustomerProfile>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.CustomerProfile.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.CustomerProfile> ExecuteForStreamInternalAsync(
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
                    Generated.Models.CustomerProfile.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableCustomerProfileRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.CustomerProfile?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.CustomerProfile>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerProfile>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCustomerProfileRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.CustomerProfile?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.CustomerProfile>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerProfile>> executeForStream,
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

        public ExecutableCustomerProfileRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.CustomerProfile> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("CustomerProfile").ConfigureAwait(false));
            return context.InitializeEntity("CustomerProfile", Generated.Models.CustomerProfile.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.CustomerProfile>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.CustomerProfile> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.CustomerProfile?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}