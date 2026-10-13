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
    public class PlatformRequest
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

        public PlatformRequest()
        {
            _query = new SelectQuery("Platform");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public PlatformRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

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
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
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

        public PlatformRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public PlatformRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public PlatformRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
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

                public PlatformRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }


                public PlatformRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public PlatformRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public PlatformRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public PlatformRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public PlatformRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public PlatformRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public PlatformRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public PlatformRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public PlatformRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public PlatformRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
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
            return CountAs("count");
        }

        public PlatformRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
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
                public PlatformRequest SelectOrderStatusList()
                {
                    return SelectOrderStatusListWith(new OrderStatusRequest());
                }

                public PlatformRequest SelectOrderStatusListWith(OrderStatusRequest child)
                {
                    _query.RelationQuery("OrderStatusList", "OrderStatus", "platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectOrderStatusListWith", Relation: "OrderStatusList")
                    };
                    return this;
                }
                public PlatformRequest SelectCustomerList()
                {
                    return SelectCustomerListWith(new CustomerRequest());
                }

                public PlatformRequest SelectCustomerListWith(CustomerRequest child)
                {
                    _query.RelationQuery("CustomerList", "Customer", "platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerListWith", Relation: "CustomerList")
                    };
                    return this;
                }
                public PlatformRequest HaveOrderStatuses()
                    => WithOrderStatusListMatching(new OrderStatusRequest());

                public PlatformRequest HaveNoOrderStatuses()
                    => WithoutOrderStatusListMatching(new OrderStatusRequest());

                public PlatformRequest WithOrderStatusListMatching(OrderStatusRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderStatus")!, child.GetQuery(), "platform"));
                    return this;
                }

                public PlatformRequest WithoutOrderStatusListMatching(OrderStatusRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderStatus")!, child.GetQuery(), "platform"));
                    return this;
                }
                public PlatformRequest HaveCustomers()
                    => WithCustomerListMatching(new CustomerRequest());

                public PlatformRequest HaveNoCustomers()
                    => WithoutCustomerListMatching(new CustomerRequest());

                public PlatformRequest WithCustomerListMatching(CustomerRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Customer")!, child.GetQuery(), "platform"));
                    return this;
                }

                public PlatformRequest WithoutCustomerListMatching(CustomerRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Customer")!, child.GetQuery(), "platform"));
                    return this;
                }
                public PlatformRequest CountOrderStatuses()
                    => CountOrderStatusesAs("countOrderStatuses");

                public PlatformRequest CountOrderStatusesAs(string alias)
                {
                    CountOrderStatusesWith(alias, new OrderStatusRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }

                public PlatformRequest CountOrderStatusesWith(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderStatusesWith", alias, "OrderStatusList")
                    };
                    return this;
                }

                public PlatformRequest MinDisplayOrderOfOrderStatuses()
                    => MinDisplayOrderOfOrderStatusesAs("minOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public PlatformRequest MinDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public PlatformRequest MaxDisplayOrderOfOrderStatuses()
                    => MaxDisplayOrderOfOrderStatusesAs("maxOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public PlatformRequest MaxDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public PlatformRequest SumDisplayOrderOfOrderStatuses()
                    => SumDisplayOrderOfOrderStatusesAs("sumOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public PlatformRequest SumDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public PlatformRequest AvgDisplayOrderOfOrderStatuses()
                    => AvgDisplayOrderOfOrderStatusesAs("avgOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public PlatformRequest AvgDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public PlatformRequest StandardDeviationDisplayOrderOfOrderStatuses()
                    => StandardDeviationDisplayOrderOfOrderStatusesAs("standardDeviationOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public PlatformRequest StandardDeviationDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public PlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatuses()
                    => SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatusesAs("squareRootOfPopulationStandardDeviationOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public PlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public PlatformRequest SampleVarianceDisplayOrderOfOrderStatuses()
                    => SampleVarianceDisplayOrderOfOrderStatusesAs("sampleVarianceOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public PlatformRequest SampleVarianceDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public PlatformRequest SamplePopulationVarianceDisplayOrderOfOrderStatuses()
                    => SamplePopulationVarianceDisplayOrderOfOrderStatusesAs("samplePopulationVarianceOfDisplayOrderOfOrderStatuses", new OrderStatusRequest());

                public PlatformRequest SamplePopulationVarianceDisplayOrderOfOrderStatusesAs(string alias, OrderStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "display_order", alias);
                    _query.RelationAggregate("OrderStatusList", "OrderStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceDisplayOrderOfOrderStatusesAs", alias, "OrderStatusList")
                    };
                    return this;
                }
                public PlatformRequest CountCustomers()
                    => CountCustomersAs("countCustomers");

                public PlatformRequest CountCustomersAs(string alias)
                {
                    CountCustomersWith(alias, new CustomerRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomersAs", alias, "CustomerList")
                    };
                    return this;
                }

                public PlatformRequest CountCustomersWith(string alias, CustomerRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("CustomerList", "Customer", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomersWith", alias, "CustomerList")
                    };
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

        private async IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var request = context.PrepareQueryRequest(new QueryRequest(_query));
            await foreach (var chunk in streaming.QueryStreamAsync(
                request, chunkSize, cancellationToken).WithCancellation(cancellationToken))
            {
                var queryRoot = new EntityRoot();
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.Platform.FromRecord(row, queryRoot);
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
        private readonly Func<UserContext, Task<QueryResult>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Record>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePlatformRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<QueryResult>> executeForOne,
            Func<UserContext, Task<SmartList<Record>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> executeForStream,
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
            var result = await _executeForList(context);
            var entities = new SmartList<Generated.Models.Platform>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            var queryRoot = new EntityRoot();
            foreach (var row in result)
                entities.Add(Generated.Models.Platform.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.Platform?> ExecuteForOneAsync(
            UserContext context)
        {
            var result = await _executeForOne(context);
            if (result.Rows.Count == 0) return null;
            return Generated.Models.Platform.FromRecord(
                result.Rows[0], new EntityRoot());
        }
    }

}