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
    public class ShippingItemRequest
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

        public ShippingItemRequest()
        {
            _query = new SelectQuery("ShippingItem");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public ShippingItemRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public ShippingItemRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public ShippingItemRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public ShippingItemRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableShippingItemRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableShippingItemRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public ShippingItemRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public ShippingItemRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public ShippingItemRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public ShippingItemRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public ShippingItemRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public ShippingItemRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public ShippingItemRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public ShippingItemRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public ShippingItemRequest SelectId()
                {
                    _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public ShippingItemRequest SelectSku()
                {
                    _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY);
                    return this;
                }

                public ShippingItemRequest SelectQuantity()
                {
                    _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY);
                    return this;
                }

                public ShippingItemRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public ShippingItemRequest SelectShippingOrder()
                {
                    return SelectShippingOrderWith(new ShippingOrderRequest());
                }

                public ShippingItemRequest SelectShippingOrderWith(ShippingOrderRequest related)
                {
                    _query.Project(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_MEMBER, "ShippingOrder", global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectShippingOrderWith", Relation: global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public ShippingItemRequest WithShippingOrderMatching(ShippingOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("ShippingOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public ShippingItemRequest WithoutShippingOrderMatching(ShippingOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("ShippingOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public ShippingItemRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingItemRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingItemRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShippingItemRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public ShippingItemRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public ShippingItemRequest FilterByShippingOrder(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY, val));
                    return this;
                }

                public ShippingItemRequest FilterByShippingOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY, vals));
                    return this;
                }

                public ShippingItemRequest WithShippingOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public ShippingItemRequest WithShippingOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public ShippingItemRequest WithSkuContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public ShippingItemRequest WithSkuIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public ShippingItemRequest WithSkuIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithSkuIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingItemRequest WithSkuNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingItemRequest WithSkuGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithSkuGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithSkuLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithSkuLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithSkuBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShippingItemRequest WithSkuIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY));
                    return this;
                }

                public ShippingItemRequest WithSkuIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY));
                    return this;
                }
                public ShippingItemRequest WithSkuNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public ShippingItemRequest WithSkuStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public ShippingItemRequest WithSkuNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public ShippingItemRequest WithSkuEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public ShippingItemRequest WithSkuNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public ShippingItemRequest WithSkuSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public ShippingItemRequest WithQuantityIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithQuantityIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithQuantityIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingItemRequest WithQuantityNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingItemRequest WithQuantityGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithQuantityGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithQuantityLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithQuantityLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithQuantityBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShippingItemRequest WithQuantityIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY));
                    return this;
                }

                public ShippingItemRequest WithQuantityIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY));
                    return this;
                }

                public ShippingItemRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingItemRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public ShippingItemRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public ShippingItemRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ShippingItemRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public ShippingItemRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public ShippingItemRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public ShippingItemRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public ShippingItemRequest OrderBySkuAscending()
                {
                    _query.OrderBy(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, "asc");
                    return this;
                }

                public ShippingItemRequest OrderBySkuDescending()
                {
                    _query.OrderBy(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY, "desc");
                    return this;
                }

                public ShippingItemRequest OrderByQuantityAscending()
                {
                    _query.OrderBy(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, "asc");
                    return this;
                }

                public ShippingItemRequest OrderByQuantityDescending()
                {
                    _query.OrderBy(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, "desc");
                    return this;
                }

                public ShippingItemRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public ShippingItemRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public ShippingItemRequest Count()
        {
            return CountAs("count");
        }

        public ShippingItemRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public ShippingItemRequest MinQuantity()
                {
                    return MinQuantityAs("minOfQuantity");
                }

                public ShippingItemRequest MinQuantityAs(string retName)
                {
                    _query.Aggregate("min", global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public ShippingItemRequest MaxQuantity()
                {
                    return MaxQuantityAs("maxOfQuantity");
                }

                public ShippingItemRequest MaxQuantityAs(string retName)
                {
                    _query.Aggregate("max", global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public ShippingItemRequest SumQuantity()
                {
                    return SumQuantityAs("sumOfQuantity");
                }

                public ShippingItemRequest SumQuantityAs(string retName)
                {
                    _query.Aggregate("sum", global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public ShippingItemRequest AvgQuantity()
                {
                    return AvgQuantityAs("avgOfQuantity");
                }

                public ShippingItemRequest AvgQuantityAs(string retName)
                {
                    _query.Aggregate("avg", global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public ShippingItemRequest StandardDeviationQuantity()
                {
                    return StandardDeviationQuantityAs("standardDeviationOfQuantity");
                }

                public ShippingItemRequest StandardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev", global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public ShippingItemRequest SquareRootOfPopulationStandardDeviationQuantity()
                {
                    return SquareRootOfPopulationStandardDeviationQuantityAs("squareRootOfPopulationStandardDeviationOfQuantity");
                }

                public ShippingItemRequest SquareRootOfPopulationStandardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev_pop", global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public ShippingItemRequest SampleVarianceQuantity()
                {
                    return SampleVarianceQuantityAs("sampleVarianceOfQuantity");
                }

                public ShippingItemRequest SampleVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_samp", global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public ShippingItemRequest SamplePopulationVarianceQuantity()
                {
                    return SamplePopulationVarianceQuantityAs("samplePopulationVarianceOfQuantity");
                }

                public ShippingItemRequest SamplePopulationVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_pop", global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public ShippingItemRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public ShippingItemRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public ShippingItemRequest GroupByShippingOrder()
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public ShippingItemRequest GroupByShippingOrderAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY);
                    return this;
                }
                public ShippingItemRequest GroupBySku()
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY);
                    return this;
                }

                public ShippingItemRequest GroupBySkuAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.SKU_PROPERTY_QUERY);
                    return this;
                }
                public ShippingItemRequest GroupByQuantity()
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY);
                    return this;
                }

                public ShippingItemRequest GroupByQuantityAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.QUANTITY_PROPERTY_QUERY);
                    return this;
                }
                public ShippingItemRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public ShippingItemRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.ShippingItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public ShippingItemRequest FacetByShippingOrderAs(
                    string name, ShippingOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByShippingOrderAs", name, global::Generated.Models.ShippingItem.__TeaqlFields.SHIPPING_ORDER_PROPERTY_QUERY)
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

        private async Task<Generated.Models.ShippingItem?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.ShippingItem>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.ShippingItem>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.ShippingItem>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.ShippingItem.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.ShippingItem> ExecuteForStreamInternalAsync(
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
                    Generated.Models.ShippingItem.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableShippingItemRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.ShippingItem?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.ShippingItem>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.ShippingItem>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableShippingItemRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.ShippingItem?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.ShippingItem>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.ShippingItem>> executeForStream,
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

        public ExecutableShippingItemRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.ShippingItem> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("ShippingItem").ConfigureAwait(false));
            return context.InitializeEntity("ShippingItem", Generated.Models.ShippingItem.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.ShippingItem>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.ShippingItem> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.ShippingItem?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}