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
    public class OrderItemRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;
        private bool _countRequested;
        private readonly List<DynamicSearchWarning> _dynamicSearchWarnings = new();
        private static object TeaqlQueryValue(object value) =>
            value is TimeSpan time
                ? time.ToString("c", System.Globalization.CultureInfo.InvariantCulture)
                : value;

        public OrderItemRequest()
        {
            _query = new SelectQuery("OrderItem");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public OrderItemRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public OrderItemRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public OrderItemRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public OrderItemRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderItemRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderItemRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public OrderItemRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public OrderItemRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public OrderItemRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public OrderItemRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public OrderItemRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public OrderItemRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public OrderItemRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public OrderItemRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("purchase_order");
            _query.Project("tenant");
            _query.Project("product_name");
            _query.Project("unit_price");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public OrderItemRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }



                public OrderItemRequest SelectProductName()
                {
                    _query.Project("product_name");
                    return this;
                }

                public OrderItemRequest SelectUnitPrice()
                {
                    _query.Project("unit_price");
                    return this;
                }

                public OrderItemRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public OrderItemRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public OrderItemRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public OrderItemRequest SelectPurchaseOrder()
                {
                    return SelectPurchaseOrderWith(new PurchaseOrderRequest());
                }

                public OrderItemRequest SelectPurchaseOrderWith(PurchaseOrderRequest related)
                {
                    _query.Project("purchase_order");
                    _query.ForwardRelationQuery("PurchaseOrder", "PurchaseOrder", "purchase_order", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPurchaseOrderWith", Relation: "PurchaseOrder")
                    };
                    return this;
                }
                public OrderItemRequest SelectTenant()
                {
                    return SelectTenantWith(new TenantRequest());
                }

                public OrderItemRequest SelectTenantWith(TenantRequest related)
                {
                    _query.Project("tenant");
                    _query.ForwardRelationQuery("Tenant", "Tenant", "tenant", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTenantWith", Relation: "Tenant")
                    };
                    return this;
                }
                public OrderItemRequest WithPurchaseOrderMatching(PurchaseOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("purchase_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("PurchaseOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderItemRequest WithoutPurchaseOrderMatching(PurchaseOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("purchase_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("PurchaseOrder")!, related.GetQuery(), "id"));
                    return this;
                }
                public OrderItemRequest WithTenantMatching(TenantRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("tenant", GeneratedRuntimeModule.Module.Metadata.GetEntity("Tenant")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderItemRequest WithoutTenantMatching(TenantRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("tenant", GeneratedRuntimeModule.Module.Metadata.GetEntity("Tenant")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderItemRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public OrderItemRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public OrderItemRequest FilterByPurchaseOrder(object val)
                {
                    _query.AndFilter(Expr.Eq("purchase_order", val));
                    return this;
                }

                public OrderItemRequest FilterByPurchaseOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("purchase_order", vals));
                    return this;
                }

                public OrderItemRequest WithPurchaseOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("purchase_order"));
                    return this;
                }

                public OrderItemRequest WithPurchaseOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("purchase_order"));
                    return this;
                }

                public OrderItemRequest FilterByTenant(object val)
                {
                    _query.AndFilter(Expr.Eq("tenant", val));
                    return this;
                }

                public OrderItemRequest FilterByTenantIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("tenant", vals));
                    return this;
                }

                public OrderItemRequest WithTenantIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("tenant"));
                    return this;
                }

                public OrderItemRequest WithTenantIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("tenant"));
                    return this;
                }

                public OrderItemRequest WithProductNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("product_name", val));
                    return this;
                }

                public OrderItemRequest WithProductNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("product_name", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderItemRequest WithProductNameIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithProductNameIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("product_name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithProductNameNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("product_name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithProductNameGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithProductNameGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithProductNameLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithProductNameLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithProductNameBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("product_name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithProductNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("product_name"));
                    return this;
                }

                public OrderItemRequest WithProductNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("product_name"));
                    return this;
                }
                public OrderItemRequest WithProductNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("product_name", val));
                    return this;
                }

                public OrderItemRequest WithProductNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("product_name", val));
                    return this;
                }

                public OrderItemRequest WithProductNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("product_name", val));
                    return this;
                }

                public OrderItemRequest WithProductNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("product_name", val));
                    return this;
                }

                public OrderItemRequest WithProductNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("product_name", val));
                    return this;
                }

                public OrderItemRequest WithProductNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("product_name", val));
                    return this;
                }

                public OrderItemRequest WithUnitPriceIs(decimal val)
                {
                    _query.AndFilter(Expr.Eq("unit_price", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUnitPriceIsNot(decimal val)
                {
                    _query.AndFilter(Expr.Ne("unit_price", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUnitPriceIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.In("unit_price", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithUnitPriceNotIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.NotIn("unit_price", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithUnitPriceGreaterThan(decimal val)
                {
                    _query.AndFilter(Expr.Gt("unit_price", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUnitPriceGreaterThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Gte("unit_price", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUnitPriceLessThan(decimal val)
                {
                    _query.AndFilter(Expr.Lt("unit_price", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUnitPriceLessThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Lte("unit_price", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUnitPriceBetween(decimal lower, decimal upper)
                {
                    _query.AndFilter(Expr.Between("unit_price", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithUnitPriceIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("unit_price"));
                    return this;
                }

                public OrderItemRequest WithUnitPriceIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("unit_price"));
                    return this;
                }

                public OrderItemRequest WithCreateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithCreateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithCreateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithCreateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithCreateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithCreateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithCreateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithCreateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithCreateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("create_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public OrderItemRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("update_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public OrderItemRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public OrderItemRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderItemRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderItemRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderItemRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public OrderItemRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public OrderItemRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public OrderItemRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }



                public OrderItemRequest OrderByProductNameAscending()
                {
                    _query.OrderBy("product_name", "asc");
                    return this;
                }

                public OrderItemRequest OrderByProductNameDescending()
                {
                    _query.OrderBy("product_name", "desc");
                    return this;
                }

                public OrderItemRequest OrderByUnitPriceAscending()
                {
                    _query.OrderBy("unit_price", "asc");
                    return this;
                }

                public OrderItemRequest OrderByUnitPriceDescending()
                {
                    _query.OrderBy("unit_price", "desc");
                    return this;
                }

                public OrderItemRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public OrderItemRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public OrderItemRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public OrderItemRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public OrderItemRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public OrderItemRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public OrderItemRequest Count()
        {
            return CountAs("count");
        }

        public OrderItemRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public OrderItemRequest MinUnitPrice()
                {
                    return MinUnitPriceAs("minOfUnitPrice");
                }

                public OrderItemRequest MinUnitPriceAs(string retName)
                {
                    _query.Aggregate("min", "unit_price", retName);
                    return this;
                }
                public OrderItemRequest MaxUnitPrice()
                {
                    return MaxUnitPriceAs("maxOfUnitPrice");
                }

                public OrderItemRequest MaxUnitPriceAs(string retName)
                {
                    _query.Aggregate("max", "unit_price", retName);
                    return this;
                }
                public OrderItemRequest SumUnitPrice()
                {
                    return SumUnitPriceAs("sumOfUnitPrice");
                }

                public OrderItemRequest SumUnitPriceAs(string retName)
                {
                    _query.Aggregate("sum", "unit_price", retName);
                    return this;
                }
                public OrderItemRequest AvgUnitPrice()
                {
                    return AvgUnitPriceAs("avgOfUnitPrice");
                }

                public OrderItemRequest AvgUnitPriceAs(string retName)
                {
                    _query.Aggregate("avg", "unit_price", retName);
                    return this;
                }
                public OrderItemRequest StandardDeviationUnitPrice()
                {
                    return StandardDeviationUnitPriceAs("standardDeviationOfUnitPrice");
                }

                public OrderItemRequest StandardDeviationUnitPriceAs(string retName)
                {
                    _query.Aggregate("stddev", "unit_price", retName);
                    return this;
                }
                public OrderItemRequest SquareRootOfPopulationStandardDeviationUnitPrice()
                {
                    return SquareRootOfPopulationStandardDeviationUnitPriceAs("squareRootOfPopulationStandardDeviationOfUnitPrice");
                }

                public OrderItemRequest SquareRootOfPopulationStandardDeviationUnitPriceAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "unit_price", retName);
                    return this;
                }
                public OrderItemRequest SampleVarianceUnitPrice()
                {
                    return SampleVarianceUnitPriceAs("sampleVarianceOfUnitPrice");
                }

                public OrderItemRequest SampleVarianceUnitPriceAs(string retName)
                {
                    _query.Aggregate("var_samp", "unit_price", retName);
                    return this;
                }
                public OrderItemRequest SamplePopulationVarianceUnitPrice()
                {
                    return SamplePopulationVarianceUnitPriceAs("samplePopulationVarianceOfUnitPrice");
                }

                public OrderItemRequest SamplePopulationVarianceUnitPriceAs(string retName)
                {
                    _query.Aggregate("var_pop", "unit_price", retName);
                    return this;
                }
                public OrderItemRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public OrderItemRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id");
                    _query.ProjectExpr(retName, Expr.Column("id"));
                    return this;
                }
                public OrderItemRequest GroupByPurchaseOrder()
                {
                    _query.GroupBy("purchase_order");
                    return this;
                }

                public OrderItemRequest GroupByPurchaseOrderAs(string retName)
                {
                    _query.GroupBy("purchase_order");
                    _query.ProjectExpr(retName, Expr.Column("purchase_order"));
                    return this;
                }
                public OrderItemRequest GroupByTenant()
                {
                    _query.GroupBy("tenant");
                    return this;
                }

                public OrderItemRequest GroupByTenantAs(string retName)
                {
                    _query.GroupBy("tenant");
                    _query.ProjectExpr(retName, Expr.Column("tenant"));
                    return this;
                }
                public OrderItemRequest GroupByProductName()
                {
                    _query.GroupBy("product_name");
                    return this;
                }

                public OrderItemRequest GroupByProductNameAs(string retName)
                {
                    _query.GroupBy("product_name");
                    _query.ProjectExpr(retName, Expr.Column("product_name"));
                    return this;
                }
                public OrderItemRequest GroupByUnitPrice()
                {
                    _query.GroupBy("unit_price");
                    return this;
                }

                public OrderItemRequest GroupByUnitPriceAs(string retName)
                {
                    _query.GroupBy("unit_price");
                    _query.ProjectExpr(retName, Expr.Column("unit_price"));
                    return this;
                }
                public OrderItemRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public OrderItemRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time");
                    _query.ProjectExpr(retName, Expr.Column("create_time"));
                    return this;
                }
                public OrderItemRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public OrderItemRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time");
                    _query.ProjectExpr(retName, Expr.Column("update_time"));
                    return this;
                }
                public OrderItemRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public OrderItemRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version");
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public OrderItemRequest FacetByPurchaseOrderAs(
                    string name, PurchaseOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "PurchaseOrder", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPurchaseOrderAs", name, "purchase_order")
                    });
                    return this;
                }

                public OrderItemRequest FacetByTenantAs(
                    string name, TenantRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Tenant", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByTenantAs", name, "tenant")
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

        private async Task<Generated.Models.OrderItem?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.OrderItem.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.OrderItem>> ExecuteForListWithMetadataInternalAsync(
            UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            var request = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            SmartList<Record> result;
            if (_countRequested) result = await RuntimeDataService.FetchPageAsync(service, context, request);
            else {
                var rows = await service.QueryAsync(request);
                result = new SmartList<Record>(rows.Rows) { Facets = rows.Facets };
            }
            var entities = new SmartList<Generated.Models.OrderItem>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.OrderItem.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.OrderItem> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var request = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            return RuntimeDataService.StreamEntities(context, request,
                row => global::TeaQL.Runtime.EntityQueryProperties.Capture(
                    Generated.Models.OrderItem.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableOrderItemRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.OrderItem?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.OrderItem>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderItem>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableOrderItemRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.OrderItem?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.OrderItem>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderItem>> executeForStream,
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

        public ExecutableOrderItemRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.OrderItem> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("OrderItem").ConfigureAwait(false));
            var entity = context.InitializeEntity("OrderItem", new Generated.Models.OrderItem());
            entity.TeaqlInitializeNewId(identity);
            return entity;
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.OrderItem>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.OrderItem> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.OrderItem?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}