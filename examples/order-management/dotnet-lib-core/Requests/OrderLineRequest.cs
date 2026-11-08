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
    public class OrderLineRequest
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

        public OrderLineRequest()
        {
            _query = new SelectQuery("OrderLine");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public OrderLineRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public OrderLineRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public OrderLineRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public OrderLineRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderLineRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderLineRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public OrderLineRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public OrderLineRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public OrderLineRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public OrderLineRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public OrderLineRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public OrderLineRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public OrderLineRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public OrderLineRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("customer_order");
            _query.Project("product");
            _query.Project("product_name");
            _query.Project("sku");
            _query.Project("quantity");
            _query.Project("commerce_platform");
            _query.Project("create_time");
            _query.Project("version");
            return this;
        }

                public OrderLineRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }



                public OrderLineRequest SelectProductName()
                {
                    _query.Project("product_name");
                    return this;
                }

                public OrderLineRequest SelectSku()
                {
                    _query.Project("sku");
                    return this;
                }

                public OrderLineRequest SelectQuantity()
                {
                    _query.Project("quantity");
                    return this;
                }


                public OrderLineRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public OrderLineRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public OrderLineRequest SelectCustomerOrder()
                {
                    return SelectCustomerOrderWith(new CustomerOrderRequest());
                }

                public OrderLineRequest SelectCustomerOrderWith(CustomerOrderRequest related)
                {
                    _query.Project("customer_order");
                    _query.ForwardRelationQuery("CustomerOrder", "CustomerOrder", "customer_order", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderWith", Relation: "CustomerOrder")
                    };
                    return this;
                }
                public OrderLineRequest SelectProduct()
                {
                    return SelectProductWith(new ProductRequest());
                }

                public OrderLineRequest SelectProductWith(ProductRequest related)
                {
                    _query.Project("product");
                    _query.ForwardRelationQuery("Product", "Product", "product", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectProductWith", Relation: "Product")
                    };
                    return this;
                }
                public OrderLineRequest SelectCommercePlatform()
                {
                    return SelectCommercePlatformWith(new CommercePlatformRequest());
                }

                public OrderLineRequest SelectCommercePlatformWith(CommercePlatformRequest related)
                {
                    _query.Project("commerce_platform");
                    _query.ForwardRelationQuery("CommercePlatform", "CommercePlatform", "commerce_platform", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCommercePlatformWith", Relation: "CommercePlatform")
                    };
                    return this;
                }
                public OrderLineRequest WithCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("customer_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithoutCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("customer_order", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }
                public OrderLineRequest WithProductMatching(ProductRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("product", GeneratedRuntimeModule.Module.Metadata.GetEntity("Product")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithoutProductMatching(ProductRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("product", GeneratedRuntimeModule.Module.Metadata.GetEntity("Product")!, related.GetQuery(), "id"));
                    return this;
                }
                public OrderLineRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public OrderLineRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public OrderLineRequest FilterByCustomerOrder(object val)
                {
                    _query.AndFilter(Expr.Eq("customer_order", val));
                    return this;
                }

                public OrderLineRequest FilterByCustomerOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("customer_order", vals));
                    return this;
                }

                public OrderLineRequest WithCustomerOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("customer_order"));
                    return this;
                }

                public OrderLineRequest WithCustomerOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("customer_order"));
                    return this;
                }

                public OrderLineRequest FilterByProduct(object val)
                {
                    _query.AndFilter(Expr.Eq("product", val));
                    return this;
                }

                public OrderLineRequest FilterByProductIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("product", vals));
                    return this;
                }

                public OrderLineRequest WithProductIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("product"));
                    return this;
                }

                public OrderLineRequest WithProductIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("product"));
                    return this;
                }

                public OrderLineRequest WithProductNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("product_name", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderLineRequest WithProductNameIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("product_name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithProductNameNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("product_name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithProductNameGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("product_name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("product_name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithProductNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("product_name"));
                    return this;
                }

                public OrderLineRequest WithProductNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("product_name"));
                    return this;
                }
                public OrderLineRequest WithProductNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("product_name", val));
                    return this;
                }

                public OrderLineRequest WithSkuContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuIs(string val)
                {
                    _query.AndFilter(Expr.Eq("sku", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderLineRequest WithSkuIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("sku", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithSkuNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("sku", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithSkuGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("sku", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithSkuIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("sku"));
                    return this;
                }

                public OrderLineRequest WithSkuIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("sku"));
                    return this;
                }
                public OrderLineRequest WithSkuNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("sku", val));
                    return this;
                }

                public OrderLineRequest WithQuantityIs(long val)
                {
                    _query.AndFilter(Expr.Eq("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("quantity", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithQuantityNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("quantity", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithQuantityGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("quantity", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("quantity", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithQuantityIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("quantity"));
                    return this;
                }

                public OrderLineRequest WithQuantityIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("quantity"));
                    return this;
                }

                public OrderLineRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public OrderLineRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public OrderLineRequest WithCommercePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("commerce_platform"));
                    return this;
                }

                public OrderLineRequest WithCommercePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("commerce_platform"));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithCreateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithCreateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("create_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public OrderLineRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderLineRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public OrderLineRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public OrderLineRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public OrderLineRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }



                public OrderLineRequest OrderByProductNameAscending()
                {
                    _query.OrderBy("product_name", "asc");
                    return this;
                }

                public OrderLineRequest OrderByProductNameDescending()
                {
                    _query.OrderBy("product_name", "desc");
                    return this;
                }

                public OrderLineRequest OrderBySkuAscending()
                {
                    _query.OrderBy("sku", "asc");
                    return this;
                }

                public OrderLineRequest OrderBySkuDescending()
                {
                    _query.OrderBy("sku", "desc");
                    return this;
                }

                public OrderLineRequest OrderByQuantityAscending()
                {
                    _query.OrderBy("quantity", "asc");
                    return this;
                }

                public OrderLineRequest OrderByQuantityDescending()
                {
                    _query.OrderBy("quantity", "desc");
                    return this;
                }


                public OrderLineRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public OrderLineRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public OrderLineRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public OrderLineRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public OrderLineRequest Count()
        {
            return CountAs("count");
        }

        public OrderLineRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public OrderLineRequest MinQuantity()
                {
                    return MinQuantityAs("minOfQuantity");
                }

                public OrderLineRequest MinQuantityAs(string retName)
                {
                    _query.Aggregate("min", "quantity", retName);
                    return this;
                }
                public OrderLineRequest MaxQuantity()
                {
                    return MaxQuantityAs("maxOfQuantity");
                }

                public OrderLineRequest MaxQuantityAs(string retName)
                {
                    _query.Aggregate("max", "quantity", retName);
                    return this;
                }
                public OrderLineRequest SumQuantity()
                {
                    return SumQuantityAs("sumOfQuantity");
                }

                public OrderLineRequest SumQuantityAs(string retName)
                {
                    _query.Aggregate("sum", "quantity", retName);
                    return this;
                }
                public OrderLineRequest AvgQuantity()
                {
                    return AvgQuantityAs("avgOfQuantity");
                }

                public OrderLineRequest AvgQuantityAs(string retName)
                {
                    _query.Aggregate("avg", "quantity", retName);
                    return this;
                }
                public OrderLineRequest StandardDeviationQuantity()
                {
                    return StandardDeviationQuantityAs("standardDeviationOfQuantity");
                }

                public OrderLineRequest StandardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev", "quantity", retName);
                    return this;
                }
                public OrderLineRequest SquareRootOfPopulationStandardDeviationQuantity()
                {
                    return SquareRootOfPopulationStandardDeviationQuantityAs("squareRootOfPopulationStandardDeviationOfQuantity");
                }

                public OrderLineRequest SquareRootOfPopulationStandardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "quantity", retName);
                    return this;
                }
                public OrderLineRequest SampleVarianceQuantity()
                {
                    return SampleVarianceQuantityAs("sampleVarianceOfQuantity");
                }

                public OrderLineRequest SampleVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_samp", "quantity", retName);
                    return this;
                }
                public OrderLineRequest SamplePopulationVarianceQuantity()
                {
                    return SamplePopulationVarianceQuantityAs("samplePopulationVarianceOfQuantity");
                }

                public OrderLineRequest SamplePopulationVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_pop", "quantity", retName);
                    return this;
                }
                public OrderLineRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public OrderLineRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id");
                    _query.ProjectExpr(retName, Expr.Column("id"));
                    return this;
                }
                public OrderLineRequest GroupByCustomerOrder()
                {
                    _query.GroupBy("customer_order");
                    return this;
                }

                public OrderLineRequest GroupByCustomerOrderAs(string retName)
                {
                    _query.GroupBy("customer_order");
                    _query.ProjectExpr(retName, Expr.Column("customer_order"));
                    return this;
                }
                public OrderLineRequest GroupByProduct()
                {
                    _query.GroupBy("product");
                    return this;
                }

                public OrderLineRequest GroupByProductAs(string retName)
                {
                    _query.GroupBy("product");
                    _query.ProjectExpr(retName, Expr.Column("product"));
                    return this;
                }
                public OrderLineRequest GroupByProductName()
                {
                    _query.GroupBy("product_name");
                    return this;
                }

                public OrderLineRequest GroupByProductNameAs(string retName)
                {
                    _query.GroupBy("product_name");
                    _query.ProjectExpr(retName, Expr.Column("product_name"));
                    return this;
                }
                public OrderLineRequest GroupBySku()
                {
                    _query.GroupBy("sku");
                    return this;
                }

                public OrderLineRequest GroupBySkuAs(string retName)
                {
                    _query.GroupBy("sku");
                    _query.ProjectExpr(retName, Expr.Column("sku"));
                    return this;
                }
                public OrderLineRequest GroupByQuantity()
                {
                    _query.GroupBy("quantity");
                    return this;
                }

                public OrderLineRequest GroupByQuantityAs(string retName)
                {
                    _query.GroupBy("quantity");
                    _query.ProjectExpr(retName, Expr.Column("quantity"));
                    return this;
                }
                public OrderLineRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public OrderLineRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform");
                    _query.ProjectExpr(retName, Expr.Column("commerce_platform"));
                    return this;
                }
                public OrderLineRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public OrderLineRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time");
                    _query.ProjectExpr(retName, Expr.Column("create_time"));
                    return this;
                }
                public OrderLineRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public OrderLineRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version");
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public OrderLineRequest FacetByCustomerOrderAs(
                    string name, CustomerOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "CustomerOrder", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerOrderAs", name, "customer_order")
                    });
                    return this;
                }

                public OrderLineRequest FacetByProductAs(
                    string name, ProductRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Product", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByProductAs", name, "product")
                    });
                    return this;
                }

                public OrderLineRequest FacetByCommercePlatformAs(
                    string name, CommercePlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "CommercePlatform", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCommercePlatformAs", name, "commerce_platform")
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

        private async Task<Generated.Models.OrderLine?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.OrderLine.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.OrderLine>> ExecuteForListWithMetadataInternalAsync(
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
            var entities = new SmartList<Generated.Models.OrderLine>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.OrderLine.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.OrderLine> ExecuteForStreamInternalAsync(
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
                    Generated.Models.OrderLine.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableOrderLineRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.OrderLine?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.OrderLine>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderLine>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableOrderLineRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.OrderLine?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.OrderLine>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderLine>> executeForStream,
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

        public ExecutableOrderLineRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.OrderLine> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("OrderLine").ConfigureAwait(false));
            var entity = Generated.Models.OrderLine.TeaqlNewWithReservedId(identity);
            return context.InitializeEntity("OrderLine", entity,
                initialized => initialized.TeaqlValidateReservedIdentity(identity));
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.OrderLine>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.OrderLine> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.OrderLine?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}