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
    public class CustomerContactRequest
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

        public CustomerContactRequest()
        {
            _query = new SelectQuery("CustomerContact");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public CustomerContactRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public CustomerContactRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public CustomerContactRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public CustomerContactRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCustomerContactRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerContactRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public CustomerContactRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public CustomerContactRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public CustomerContactRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public CustomerContactRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public CustomerContactRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public CustomerContactRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public CustomerContactRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public CustomerContactRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("customer");
            _query.Project("channel");
            _query.Project("version");
            return this;
        }

                public CustomerContactRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public CustomerContactRequest SelectChannel()
                {
                    _query.Project("channel");
                    return this;
                }

                public CustomerContactRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public CustomerContactRequest SelectCustomer()
                {
                    return SelectCustomerWith(new CustomerRequest());
                }

                public CustomerContactRequest SelectCustomerWith(CustomerRequest related)
                {
                    _query.Project("customer");
                    _query.ForwardRelationQuery("Customer", "Customer", "customer", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerWith", Relation: "Customer")
                    };
                    return this;
                }
                public CustomerContactRequest WithCustomerMatching(CustomerRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("customer", GeneratedRuntimeModule.Module.Metadata.GetEntity("Customer")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerContactRequest WithoutCustomerMatching(CustomerRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("customer", GeneratedRuntimeModule.Module.Metadata.GetEntity("Customer")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerContactRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerContactRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerContactRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerContactRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public CustomerContactRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public CustomerContactRequest FilterByCustomer(object val)
                {
                    _query.AndFilter(Expr.Eq("customer", val));
                    return this;
                }

                public CustomerContactRequest FilterByCustomerIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("customer", vals));
                    return this;
                }

                public CustomerContactRequest WithCustomerIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("customer"));
                    return this;
                }

                public CustomerContactRequest WithCustomerIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("customer"));
                    return this;
                }

                public CustomerContactRequest WithChannelContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("channel", val));
                    return this;
                }

                public CustomerContactRequest WithChannelIs(string val)
                {
                    _query.AndFilter(Expr.Eq("channel", TeaqlQueryValue(val)));
                    return this;
                }
                public CustomerContactRequest WithChannelIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("channel", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithChannelIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("channel", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerContactRequest WithChannelNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("channel", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerContactRequest WithChannelGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("channel", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithChannelGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("channel", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithChannelLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("channel", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithChannelLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("channel", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithChannelBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("channel", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerContactRequest WithChannelIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("channel"));
                    return this;
                }

                public CustomerContactRequest WithChannelIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("channel"));
                    return this;
                }
                public CustomerContactRequest WithChannelNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("channel", val));
                    return this;
                }

                public CustomerContactRequest WithChannelStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("channel", val));
                    return this;
                }

                public CustomerContactRequest WithChannelNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("channel", val));
                    return this;
                }

                public CustomerContactRequest WithChannelEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("channel", val));
                    return this;
                }

                public CustomerContactRequest WithChannelNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("channel", val));
                    return this;
                }

                public CustomerContactRequest WithChannelSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("channel", val));
                    return this;
                }

                public CustomerContactRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerContactRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerContactRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerContactRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerContactRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public CustomerContactRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public CustomerContactRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public CustomerContactRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public CustomerContactRequest OrderByChannelAscending()
                {
                    _query.OrderBy("channel", "asc");
                    return this;
                }

                public CustomerContactRequest OrderByChannelDescending()
                {
                    _query.OrderBy("channel", "desc");
                    return this;
                }

                public CustomerContactRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public CustomerContactRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public CustomerContactRequest Count()
        {
            return CountAs("count");
        }

        public CustomerContactRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public CustomerContactRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public CustomerContactRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public CustomerContactRequest GroupByCustomer()
                {
                    _query.GroupBy("customer");
                    return this;
                }

                public CustomerContactRequest GroupByCustomerAs(string retName)
                {
                    _query.GroupBy("customer"); 
                    return this;
                }
                public CustomerContactRequest GroupByChannel()
                {
                    _query.GroupBy("channel");
                    return this;
                }

                public CustomerContactRequest GroupByChannelAs(string retName)
                {
                    _query.GroupBy("channel"); 
                    return this;
                }
                public CustomerContactRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public CustomerContactRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public CustomerContactRequest FacetByCustomerAs(
                    string name, CustomerRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Customer", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerAs", name, "customer")
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

        private async Task<Generated.Models.CustomerContact?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.CustomerContact>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.CustomerContact>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.CustomerContact>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.CustomerContact.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.CustomerContact> ExecuteForStreamInternalAsync(
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
                    Generated.Models.CustomerContact.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableCustomerContactRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.CustomerContact?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.CustomerContact>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerContact>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCustomerContactRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.CustomerContact?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.CustomerContact>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerContact>> executeForStream,
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

        public ExecutableCustomerContactRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.CustomerContact> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("CustomerContact").ConfigureAwait(false));
            return context.InitializeEntity("CustomerContact", Generated.Models.CustomerContact.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.CustomerContact>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.CustomerContact> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.CustomerContact?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}