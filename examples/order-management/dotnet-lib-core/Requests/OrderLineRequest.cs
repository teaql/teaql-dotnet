using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class OrderLineRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;

        public OrderLineRequest()
        {
            _query = new SelectQuery("OrderLine");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

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
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderLineRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
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
                    return this;
                }
                public OrderLineRequest WithCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("customer_order", EntityDescriptor.New("CustomerOrder"), related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithoutCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("customer_order", EntityDescriptor.New("CustomerOrder"), related.GetQuery(), "id"));
                    return this;
                }
                public OrderLineRequest WithProductMatching(ProductRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("product", EntityDescriptor.New("Product"), related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithoutProductMatching(ProductRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("product", EntityDescriptor.New("Product"), related.GetQuery(), "id"));
                    return this;
                }
                public OrderLineRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public OrderLineRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public OrderLineRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public OrderLineRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public OrderLineRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public OrderLineRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public OrderLineRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public OrderLineRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public OrderLineRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
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
                    _query.AndFilter(Expr.Eq("product_name", val));
                    return this;
                }
                public OrderLineRequest WithProductNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("product_name", vals));
                    return this;
                }

                public OrderLineRequest WithProductNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("product_name", vals));
                    return this;
                }

                public OrderLineRequest WithProductNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("product_name", val));
                    return this;
                }

                public OrderLineRequest WithProductNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("product_name", lower, upper));
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
                    _query.AndFilter(Expr.Eq("sku", val));
                    return this;
                }
                public OrderLineRequest WithSkuIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("sku", vals));
                    return this;
                }

                public OrderLineRequest WithSkuNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("sku", vals));
                    return this;
                }

                public OrderLineRequest WithSkuGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("sku", val));
                    return this;
                }

                public OrderLineRequest WithSkuBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("sku", lower, upper));
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

                public OrderLineRequest WithQuantityIs(object val)
                {
                    _query.AndFilter(Expr.Eq("quantity", val));
                    return this;
                }

                public OrderLineRequest WithQuantityIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("quantity", val));
                    return this;
                }

                public OrderLineRequest WithQuantityIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("quantity", vals));
                    return this;
                }

                public OrderLineRequest WithQuantityNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("quantity", vals));
                    return this;
                }

                public OrderLineRequest WithQuantityGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("quantity", val));
                    return this;
                }

                public OrderLineRequest WithQuantityGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("quantity", val));
                    return this;
                }

                public OrderLineRequest WithQuantityLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("quantity", val));
                    return this;
                }

                public OrderLineRequest WithQuantityLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("quantity", val));
                    return this;
                }

                public OrderLineRequest WithQuantityBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("quantity", lower, upper));
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

                public OrderLineRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", val));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public OrderLineRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals));
                    return this;
                }

                public OrderLineRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", val));
                    return this;
                }

                public OrderLineRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public OrderLineRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", val));
                    return this;
                }

                public OrderLineRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public OrderLineRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", lower, upper));
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

                public OrderLineRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public OrderLineRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public OrderLineRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public OrderLineRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public OrderLineRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public OrderLineRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public OrderLineRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public OrderLineRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public OrderLineRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
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
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public OrderLineRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
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
                    return this;
                }
                public OrderLineRequest FacetByCustomerOrderAs(
                    string name, CustomerOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "customer_order", request.GetQuery(), includeAllFacets));
                    return this;
                }

                public OrderLineRequest FacetByProductAs(
                    string name, ProductRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "product", request.GetQuery(), includeAllFacets));
                    return this;
                }

                public OrderLineRequest FacetByCommercePlatformAs(
                    string name, CommercePlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "commerce_platform", request.GetQuery(), includeAllFacets));
                    return this;
                }


        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var req = new QueryRequest(_query);
            var result = await service.QueryAsync(req);
            foreach (var facet in _query.Facets)
            {
                var membership = _query.Copy();
                membership.Facets.Clear();
                membership.Relations.Clear();
                membership.Orders.Clear();
                membership.Aggregates.Clear();
                membership.GroupFields.Clear();
                membership.Projections.Clear();
                membership.Project(facet.RelationName);
                var membershipRows = (await service.QueryAsync(new QueryRequest(membership))).Rows;
                var counts = membershipRows
                    .Where(row => row.TryGetValue(facet.RelationName, out var value) && value.Raw != null)
                    .GroupBy(row => Convert.ToString(row[facet.RelationName].Raw)!)
                    .ToDictionary(group => group.Key, group => group.Count());

                var nested = facet.Query.Copy();
                nested.Facets.Clear();
                var countAliases = nested.Aggregates
                    .Where(aggregate => aggregate.Function == AggregateFunction.Count)
                    .Select(aggregate => aggregate.Alias).ToArray();
                nested.Aggregates.Clear();
                nested.GroupFields.Clear();
                var facetRows = (await service.QueryAsync(new QueryRequest(nested))).Rows;
                var decorated = new SmartList<Record>();
                foreach (var row in facetRows)
                {
                    var key = row.TryGetValue("id", out var id) ? Convert.ToString(id.Raw) : null;
                    var count = key != null && counts.TryGetValue(key, out var value) ? value : 0;
                    if (!facet.IncludeAllFacets && count == 0) continue;
                    foreach (var alias in countAliases.Length == 0 ? new[] { "count" } : countAliases)
                        row[alias] = new Value.I64Value(count);
                    decorated.Add(row);
                }
                result.Facets[facet.Name] = decorated;
            }
            return result;
        }

        private async Task<OrderLinePage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireResource<IDataService>();
            _query.Offset(offset);
            _query.Limit(limit);
            var result = await service.QueryAsync(new QueryRequest(_query));
            long totalCount;
            if (_query.IdSetPagination != null && context.IdSetCountAccuracy == "EXACT")
            {
                totalCount = checked((long)context.IdSetCount);
            }
            else
            {
                var countQuery = new SelectQuery("OrderLine");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.OrderLine>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.OrderLine.FromRecord(row, queryRoot));
            return new OrderLinePage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.OrderLine> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            await foreach (var chunk in streaming.QueryStreamAsync(
                new QueryRequest(_query), chunkSize, cancellationToken).WithCancellation(cancellationToken))
            {
                var queryRoot = new EntityRoot();
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.OrderLine.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableOrderLineRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<OrderLinePage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderLine>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableOrderLineRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<OrderLinePage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderLine>> executeForStream,
            Action limitOne,
            Action<string> comment,
            Action ensureIntent)
        {
            _executeForRows = executeForRows;
            _executeForPage = executeForPage;
            _executeForStream = executeForStream;
            _limitOne = limitOne;
            _comment = comment;
            _ensureIntent = ensureIntent;
        }

        public ExecutableOrderLineRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.OrderLine NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.OrderLine();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.OrderLine>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.OrderLine>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.OrderLine.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<OrderLinePage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.OrderLine> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.OrderLine?> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class OrderLinePage
    {
        public SmartList<Generated.Models.OrderLine> Rows { get; }
        public long TotalCount { get; }
        public OrderLinePage(
            SmartList<Generated.Models.OrderLine> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}