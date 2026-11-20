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
    public class CustomerRequest
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

        public CustomerRequest()
        {
            _query = new SelectQuery("Customer");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public CustomerRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public CustomerRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public CustomerRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public CustomerRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCustomerRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public CustomerRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public CustomerRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public CustomerRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public CustomerRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public CustomerRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public CustomerRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public CustomerRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public CustomerRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("platform");
            _query.Project("email");
            _query.Project("version");
            return this;
        }

                public CustomerRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public CustomerRequest SelectEmail()
                {
                    _query.Project("email");
                    return this;
                }

                public CustomerRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public CustomerRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public CustomerRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project("platform");
                    _query.ForwardRelationQuery("Platform", "Platform", "platform", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: "Platform")
                    };
                    return this;
                }
                public CustomerRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public CustomerRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public CustomerRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("platform", val));
                    return this;
                }

                public CustomerRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("platform", vals));
                    return this;
                }

                public CustomerRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("platform"));
                    return this;
                }

                public CustomerRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("platform"));
                    return this;
                }

                public CustomerRequest WithEmailContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("email", val));
                    return this;
                }

                public CustomerRequest WithEmailIs(string val)
                {
                    _query.AndFilter(Expr.Eq("email", TeaqlQueryValue(val)));
                    return this;
                }
                public CustomerRequest WithEmailIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("email", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithEmailIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("email", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithEmailNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("email", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithEmailGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("email", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithEmailGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("email", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithEmailLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("email", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithEmailLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("email", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithEmailBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("email", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerRequest WithEmailIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("email"));
                    return this;
                }

                public CustomerRequest WithEmailIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("email"));
                    return this;
                }
                public CustomerRequest WithEmailNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("email", val));
                    return this;
                }

                public CustomerRequest WithEmailStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("email", val));
                    return this;
                }

                public CustomerRequest WithEmailNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("email", val));
                    return this;
                }

                public CustomerRequest WithEmailEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("email", val));
                    return this;
                }

                public CustomerRequest WithEmailNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("email", val));
                    return this;
                }

                public CustomerRequest WithEmailSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("email", val));
                    return this;
                }

                public CustomerRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public CustomerRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public CustomerRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public CustomerRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public CustomerRequest OrderByEmailAscending()
                {
                    _query.OrderBy("email", "asc");
                    return this;
                }

                public CustomerRequest OrderByEmailDescending()
                {
                    _query.OrderBy("email", "desc");
                    return this;
                }

                public CustomerRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public CustomerRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public CustomerRequest Count()
        {
            return CountAs("count");
        }

        public CustomerRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public CustomerRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public CustomerRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public CustomerRequest GroupByPlatform()
                {
                    _query.GroupBy("platform");
                    return this;
                }

                public CustomerRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy("platform"); 
                    return this;
                }
                public CustomerRequest GroupByEmail()
                {
                    _query.GroupBy("email");
                    return this;
                }

                public CustomerRequest GroupByEmailAs(string retName)
                {
                    _query.GroupBy("email"); 
                    return this;
                }
                public CustomerRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public CustomerRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public CustomerRequest SelectCustomerContactList()
                {
                    return SelectCustomerContactListWith(new CustomerContactRequest());
                }

                public CustomerRequest SelectCustomerContactListWith(CustomerContactRequest child)
                {
                    _query.RelationQuery("CustomerContactList", "CustomerContact", "customer", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerContactListWith", Relation: "CustomerContactList")
                    };
                    return this;
                }
                public CustomerRequest HaveCustomerContacts()
                    => WithCustomerContactListMatching(new CustomerContactRequest());

                public CustomerRequest HaveNoCustomerContacts()
                    => WithoutCustomerContactListMatching(new CustomerContactRequest());

                public CustomerRequest WithCustomerContactListMatching(CustomerContactRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerContact")!, child.GetQuery(), "customer"));
                    return this;
                }

                public CustomerRequest WithoutCustomerContactListMatching(CustomerContactRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerContact")!, child.GetQuery(), "customer"));
                    return this;
                }
                public CustomerRequest CountCustomerContacts()
                    => CountCustomerContactsAs("countCustomerContacts");

                public CustomerRequest CountCustomerContactsAs(string alias)
                {
                    CountCustomerContactsWith(alias, new CustomerContactRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerContactsAs", alias, "CustomerContactList")
                    };
                    return this;
                }

                public CustomerRequest CountCustomerContactsWith(string alias, CustomerContactRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("CustomerContactList", "CustomerContact", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerContactsWith", alias, "CustomerContactList")
                    };
                    return this;
                }


                public CustomerRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Platform", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, "platform")
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

        private async Task<Generated.Models.Customer?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.Customer>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.Customer>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.Customer>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Customer.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Customer> ExecuteForStreamInternalAsync(
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
                    Generated.Models.Customer.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableCustomerRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Customer?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Customer>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Customer>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCustomerRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Customer?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Customer>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Customer>> executeForStream,
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

        public ExecutableCustomerRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.Customer> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Customer").ConfigureAwait(false));
            return context.InitializeEntity("Customer", Generated.Models.Customer.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Customer>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Customer> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Customer?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}