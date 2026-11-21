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
    public class SupplierRequest
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

        public SupplierRequest()
        {
            _query = new SelectQuery("Supplier");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public SupplierRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public SupplierRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public SupplierRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public SupplierRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableSupplierRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableSupplierRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public SupplierRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public SupplierRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public SupplierRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public SupplierRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public SupplierRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public SupplierRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public SupplierRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public SupplierRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("warehouse");
            _query.Project("name");
            _query.Project("minimum_order_quantity");
            _query.Project("version");
            return this;
        }

                public SupplierRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public SupplierRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public SupplierRequest SelectMinimumOrderQuantity()
                {
                    _query.Project("minimum_order_quantity");
                    return this;
                }

                public SupplierRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public SupplierRequest SelectWarehouse()
                {
                    return SelectWarehouseWith(new WarehouseRequest());
                }

                public SupplierRequest SelectWarehouseWith(WarehouseRequest related)
                {
                    _query.Project("warehouse");
                    _query.ForwardRelationQuery("Warehouse", "Warehouse", "warehouse", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectWarehouseWith", Relation: "Warehouse")
                    };
                    return this;
                }
                public SupplierRequest WithWarehouseMatching(WarehouseRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("warehouse", GeneratedRuntimeModule.Module.Metadata.GetEntity("Warehouse")!, related.GetQuery(), "id"));
                    return this;
                }

                public SupplierRequest WithoutWarehouseMatching(WarehouseRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("warehouse", GeneratedRuntimeModule.Module.Metadata.GetEntity("Warehouse")!, related.GetQuery(), "id"));
                    return this;
                }

                public SupplierRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SupplierRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SupplierRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SupplierRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public SupplierRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public SupplierRequest FilterByWarehouse(object val)
                {
                    _query.AndFilter(Expr.Eq("warehouse", val));
                    return this;
                }

                public SupplierRequest FilterByWarehouseIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("warehouse", vals));
                    return this;
                }

                public SupplierRequest WithWarehouseIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("warehouse"));
                    return this;
                }

                public SupplierRequest WithWarehouseIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("warehouse"));
                    return this;
                }

                public SupplierRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public SupplierRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public SupplierRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SupplierRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SupplierRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SupplierRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public SupplierRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public SupplierRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public SupplierRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public SupplierRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public SupplierRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public SupplierRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public SupplierRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityIs(object val)
                {
                    _query.AndFilter(Expr.Eq("minimum_order_quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("minimum_order_quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("minimum_order_quantity", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("minimum_order_quantity", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("minimum_order_quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("minimum_order_quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("minimum_order_quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("minimum_order_quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("minimum_order_quantity", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("minimum_order_quantity"));
                    return this;
                }

                public SupplierRequest WithMinimumOrderQuantityIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("minimum_order_quantity"));
                    return this;
                }

                public SupplierRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SupplierRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SupplierRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SupplierRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SupplierRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public SupplierRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public SupplierRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public SupplierRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public SupplierRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public SupplierRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public SupplierRequest OrderByMinimumOrderQuantityAscending()
                {
                    _query.OrderBy("minimum_order_quantity", "asc");
                    return this;
                }

                public SupplierRequest OrderByMinimumOrderQuantityDescending()
                {
                    _query.OrderBy("minimum_order_quantity", "desc");
                    return this;
                }

                public SupplierRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public SupplierRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public SupplierRequest Count()
        {
            return CountAs("count");
        }

        public SupplierRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public SupplierRequest MinMinimumOrderQuantity()
                {
                    return MinMinimumOrderQuantityAs("minOfMinimumOrderQuantity");
                }

                public SupplierRequest MinMinimumOrderQuantityAs(string retName)
                {
                    _query.Aggregate("min", "minimum_order_quantity", retName);
                    return this;
                }
                public SupplierRequest MaxMinimumOrderQuantity()
                {
                    return MaxMinimumOrderQuantityAs("maxOfMinimumOrderQuantity");
                }

                public SupplierRequest MaxMinimumOrderQuantityAs(string retName)
                {
                    _query.Aggregate("max", "minimum_order_quantity", retName);
                    return this;
                }
                public SupplierRequest SumMinimumOrderQuantity()
                {
                    return SumMinimumOrderQuantityAs("sumOfMinimumOrderQuantity");
                }

                public SupplierRequest SumMinimumOrderQuantityAs(string retName)
                {
                    _query.Aggregate("sum", "minimum_order_quantity", retName);
                    return this;
                }
                public SupplierRequest AvgMinimumOrderQuantity()
                {
                    return AvgMinimumOrderQuantityAs("avgOfMinimumOrderQuantity");
                }

                public SupplierRequest AvgMinimumOrderQuantityAs(string retName)
                {
                    _query.Aggregate("avg", "minimum_order_quantity", retName);
                    return this;
                }
                public SupplierRequest StandardDeviationMinimumOrderQuantity()
                {
                    return StandardDeviationMinimumOrderQuantityAs("standardDeviationOfMinimumOrderQuantity");
                }

                public SupplierRequest StandardDeviationMinimumOrderQuantityAs(string retName)
                {
                    _query.Aggregate("stddev", "minimum_order_quantity", retName);
                    return this;
                }
                public SupplierRequest SquareRootOfPopulationStandardDeviationMinimumOrderQuantity()
                {
                    return SquareRootOfPopulationStandardDeviationMinimumOrderQuantityAs("squareRootOfPopulationStandardDeviationOfMinimumOrderQuantity");
                }

                public SupplierRequest SquareRootOfPopulationStandardDeviationMinimumOrderQuantityAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "minimum_order_quantity", retName);
                    return this;
                }
                public SupplierRequest SampleVarianceMinimumOrderQuantity()
                {
                    return SampleVarianceMinimumOrderQuantityAs("sampleVarianceOfMinimumOrderQuantity");
                }

                public SupplierRequest SampleVarianceMinimumOrderQuantityAs(string retName)
                {
                    _query.Aggregate("var_samp", "minimum_order_quantity", retName);
                    return this;
                }
                public SupplierRequest SamplePopulationVarianceMinimumOrderQuantity()
                {
                    return SamplePopulationVarianceMinimumOrderQuantityAs("samplePopulationVarianceOfMinimumOrderQuantity");
                }

                public SupplierRequest SamplePopulationVarianceMinimumOrderQuantityAs(string retName)
                {
                    _query.Aggregate("var_pop", "minimum_order_quantity", retName);
                    return this;
                }
                public SupplierRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public SupplierRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public SupplierRequest GroupByWarehouse()
                {
                    _query.GroupBy("warehouse");
                    return this;
                }

                public SupplierRequest GroupByWarehouseAs(string retName)
                {
                    _query.GroupBy("warehouse"); 
                    return this;
                }
                public SupplierRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public SupplierRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public SupplierRequest GroupByMinimumOrderQuantity()
                {
                    _query.GroupBy("minimum_order_quantity");
                    return this;
                }

                public SupplierRequest GroupByMinimumOrderQuantityAs(string retName)
                {
                    _query.GroupBy("minimum_order_quantity"); 
                    return this;
                }
                public SupplierRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public SupplierRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public SupplierRequest FacetByWarehouseAs(
                    string name, WarehouseRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Warehouse", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByWarehouseAs", name, "warehouse")
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

        private async Task<Generated.Models.Supplier?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.Supplier>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.Supplier>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.Supplier>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Supplier.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Supplier> ExecuteForStreamInternalAsync(
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
                    Generated.Models.Supplier.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableSupplierRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Supplier?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Supplier>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Supplier>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableSupplierRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Supplier?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Supplier>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Supplier>> executeForStream,
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

        public ExecutableSupplierRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.Supplier> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Supplier").ConfigureAwait(false));
            return context.InitializeEntity("Supplier", Generated.Models.Supplier.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Supplier>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Supplier> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Supplier?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}