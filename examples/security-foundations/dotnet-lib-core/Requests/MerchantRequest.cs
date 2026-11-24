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
    public class MerchantRequest
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

        public MerchantRequest()
        {
            _query = new SelectQuery("Merchant");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public MerchantRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public MerchantRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public MerchantRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public MerchantRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableMerchantRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableMerchantRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public MerchantRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public MerchantRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public MerchantRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public MerchantRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public MerchantRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public MerchantRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public MerchantRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public MerchantRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public MerchantRequest SelectId()
                {
                    _query.Project(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public MerchantRequest SelectName()
                {
                    _query.Project(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public MerchantRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }


                public MerchantRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public MerchantRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public MerchantRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public MerchantRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public MerchantRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public MerchantRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public MerchantRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public MerchantRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public MerchantRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public MerchantRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public MerchantRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }

                public MerchantRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }
                public MerchantRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public MerchantRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public MerchantRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public MerchantRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public MerchantRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public MerchantRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public MerchantRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public MerchantRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public MerchantRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public MerchantRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public MerchantRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public MerchantRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public MerchantRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public MerchantRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }

                public MerchantRequest OrderByNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public MerchantRequest OrderByNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public MerchantRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public MerchantRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public MerchantRequest Count()
        {
            return CountAs("count");
        }

        public MerchantRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public MerchantRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public MerchantRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Merchant.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public MerchantRequest GroupByName()
                {
                    _query.GroupBy(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public MerchantRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Merchant.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }
                public MerchantRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public MerchantRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Merchant.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public MerchantRequest SelectCustomerProfileList()
                {
                    return SelectCustomerProfileListWith(new CustomerProfileRequest());
                }

                public MerchantRequest SelectCustomerProfileListWith(CustomerProfileRequest child)
                {
                    _query.RelationQuery("CustomerProfileList", "CustomerProfile", "merchant", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerProfileListWith", Relation: "CustomerProfileList")
                    };
                    return this;
                }
                public MerchantRequest SelectCustomerOrderList()
                {
                    return SelectCustomerOrderListWith(new CustomerOrderRequest());
                }

                public MerchantRequest SelectCustomerOrderListWith(CustomerOrderRequest child)
                {
                    _query.RelationQuery("CustomerOrderList", "CustomerOrder", "merchant", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderListWith", Relation: "CustomerOrderList")
                    };
                    return this;
                }
                public MerchantRequest HaveCustomerProfiles()
                    => WithCustomerProfileListMatching(new CustomerProfileRequest());

                public MerchantRequest HaveNoCustomerProfiles()
                    => WithoutCustomerProfileListMatching(new CustomerProfileRequest());

                public MerchantRequest WithCustomerProfileListMatching(CustomerProfileRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerProfile")!, child.GetQuery(), "merchant"));
                    return this;
                }

                public MerchantRequest WithoutCustomerProfileListMatching(CustomerProfileRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerProfile")!, child.GetQuery(), "merchant"));
                    return this;
                }
                public MerchantRequest HaveCustomerOrders()
                    => WithCustomerOrderListMatching(new CustomerOrderRequest());

                public MerchantRequest HaveNoCustomerOrders()
                    => WithoutCustomerOrderListMatching(new CustomerOrderRequest());

                public MerchantRequest WithCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "merchant"));
                    return this;
                }

                public MerchantRequest WithoutCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "merchant"));
                    return this;
                }
                public MerchantRequest CountCustomerProfiles()
                    => CountCustomerProfilesAs("countCustomerProfiles");

                public MerchantRequest CountCustomerProfilesAs(string alias)
                {
                    CountCustomerProfilesWith(alias, new CustomerProfileRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerProfilesAs", alias, "CustomerProfileList")
                    };
                    return this;
                }

                public MerchantRequest CountCustomerProfilesWith(string alias, CustomerProfileRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("CustomerProfileList", "CustomerProfile", "merchant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerProfilesWith", alias, "CustomerProfileList")
                    };
                    return this;
                }


                public MerchantRequest CountCustomerOrders()
                    => CountCustomerOrdersAs("countCustomerOrders");

                public MerchantRequest CountCustomerOrdersAs(string alias)
                {
                    CountCustomerOrdersWith(alias, new CustomerOrderRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }

                public MerchantRequest CountCustomerOrdersWith(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "merchant", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersWith", alias, "CustomerOrderList")
                    };
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

        private async Task<Generated.Models.Merchant?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.Merchant>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.Merchant>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.Merchant>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Merchant.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Merchant> ExecuteForStreamInternalAsync(
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
                    Generated.Models.Merchant.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableMerchantRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Merchant?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Merchant>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Merchant>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableMerchantRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Merchant?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Merchant>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Merchant>> executeForStream,
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

        public ExecutableMerchantRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.Merchant> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Merchant").ConfigureAwait(false));
            return context.InitializeEntity("Merchant", Generated.Models.Merchant.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Merchant>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Merchant> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Merchant?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}