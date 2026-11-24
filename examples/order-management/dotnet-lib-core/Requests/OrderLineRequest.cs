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
                ? time.ToString(@"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture)
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
            _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public OrderLineRequest SelectId()
                {
                    _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }



                public OrderLineRequest SelectProductName()
                {
                    _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest SelectSku()
                {
                    _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest SelectQuantity()
                {
                    _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY);
                    return this;
                }


                public OrderLineRequest SelectCreateTime()
                {
                    _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest SelectCustomerOrder()
                {
                    return SelectCustomerOrderWith(new CustomerOrderRequest());
                }

                public OrderLineRequest SelectCustomerOrderWith(CustomerOrderRequest related)
                {
                    _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, "CustomerOrder", global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderWith", Relation: global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public OrderLineRequest SelectProduct()
                {
                    return SelectProductWith(new ProductRequest());
                }

                public OrderLineRequest SelectProductWith(ProductRequest related)
                {
                    _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_MEMBER, "Product", global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectProductWith", Relation: global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public OrderLineRequest SelectCommercePlatform()
                {
                    return SelectCommercePlatformWith(new CommercePlatformRequest());
                }

                public OrderLineRequest SelectCommercePlatformWith(CommercePlatformRequest related)
                {
                    _query.Project(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, "CommercePlatform", global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCommercePlatformWith", Relation: global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public OrderLineRequest WithCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithoutCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }
                public OrderLineRequest WithProductMatching(ProductRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Product")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithoutProductMatching(ProductRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Product")!, related.GetQuery(), "id"));
                    return this;
                }
                public OrderLineRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderLineRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest FilterByCustomerOrder(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest FilterByCustomerOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, vals));
                    return this;
                }

                public OrderLineRequest WithCustomerOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithCustomerOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest FilterByProduct(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest FilterByProductIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY, vals));
                    return this;
                }

                public OrderLineRequest WithProductIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithProductIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithProductNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithProductNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderLineRequest WithProductNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithProductNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithProductNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithProductNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithProductNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithProductNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY));
                    return this;
                }
                public OrderLineRequest WithProductNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithProductNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithProductNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithProductNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithProductNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithProductNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithSkuContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithSkuIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderLineRequest WithSkuIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithSkuNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithSkuGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithSkuBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithSkuIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithSkuIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY));
                    return this;
                }
                public OrderLineRequest WithSkuNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithSkuStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithSkuNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithSkuEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithSkuNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithSkuSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest WithQuantityIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithQuantityNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithQuantityGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithQuantityBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithQuantityIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithQuantityIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderLineRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY, vals));
                    return this;
                }

                public OrderLineRequest WithCommercePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithCommercePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderLineRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderLineRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderLineRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public OrderLineRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderLineRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }



                public OrderLineRequest OrderByProductNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderLineRequest OrderByProductNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderLineRequest OrderBySkuAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderLineRequest OrderBySkuDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderLineRequest OrderByQuantityAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderLineRequest OrderByQuantityDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, "desc");
                    return this;
                }


                public OrderLineRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderLineRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderLineRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderLineRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
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
                    _query.Aggregate("min", global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderLineRequest MaxQuantity()
                {
                    return MaxQuantityAs("maxOfQuantity");
                }

                public OrderLineRequest MaxQuantityAs(string retName)
                {
                    _query.Aggregate("max", global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderLineRequest SumQuantity()
                {
                    return SumQuantityAs("sumOfQuantity");
                }

                public OrderLineRequest SumQuantityAs(string retName)
                {
                    _query.Aggregate("sum", global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderLineRequest AvgQuantity()
                {
                    return AvgQuantityAs("avgOfQuantity");
                }

                public OrderLineRequest AvgQuantityAs(string retName)
                {
                    _query.Aggregate("avg", global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderLineRequest StandardDeviationQuantity()
                {
                    return StandardDeviationQuantityAs("standardDeviationOfQuantity");
                }

                public OrderLineRequest StandardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev", global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderLineRequest SquareRootOfPopulationStandardDeviationQuantity()
                {
                    return SquareRootOfPopulationStandardDeviationQuantityAs("squareRootOfPopulationStandardDeviationOfQuantity");
                }

                public OrderLineRequest SquareRootOfPopulationStandardDeviationQuantityAs(string retName)
                {
                    _query.Aggregate("stddev_pop", global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderLineRequest SampleVarianceQuantity()
                {
                    return SampleVarianceQuantityAs("sampleVarianceOfQuantity");
                }

                public OrderLineRequest SampleVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_samp", global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderLineRequest SamplePopulationVarianceQuantity()
                {
                    return SamplePopulationVarianceQuantityAs("samplePopulationVarianceOfQuantity");
                }

                public OrderLineRequest SamplePopulationVarianceQuantityAs(string retName)
                {
                    _query.Aggregate("var_pop", global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderLineRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public OrderLineRequest GroupByCustomerOrder()
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest GroupByCustomerOrderAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }
                public OrderLineRequest GroupByProduct()
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest GroupByProductAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY);
                    return this;
                }
                public OrderLineRequest GroupByProductName()
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest GroupByProductNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_NAME_PROPERTY_QUERY);
                    return this;
                }
                public OrderLineRequest GroupBySku()
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest GroupBySkuAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.SKU_PROPERTY_QUERY);
                    return this;
                }
                public OrderLineRequest GroupByQuantity()
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest GroupByQuantityAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.QUANTITY_PROPERTY_QUERY);
                    return this;
                }
                public OrderLineRequest GroupByCommercePlatform()
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY);
                    return this;
                }
                public OrderLineRequest GroupByCreateTime()
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }
                public OrderLineRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public OrderLineRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderLine.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public OrderLineRequest FacetByCustomerOrderAs(
                    string name, CustomerOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerOrderAs", name, global::Generated.Models.OrderLine.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY)
                    });
                    return this;
                }

                public OrderLineRequest FacetByProductAs(
                    string name, ProductRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByProductAs", name, global::Generated.Models.OrderLine.__TeaqlFields.PRODUCT_PROPERTY_QUERY)
                    });
                    return this;
                }

                public OrderLineRequest FacetByCommercePlatformAs(
                    string name, CommercePlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCommercePlatformAs", name, global::Generated.Models.OrderLine.__TeaqlFields.COMMERCE_PLATFORM_PROPERTY_QUERY)
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
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.OrderLine>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.OrderLine>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            if (request.Query.AggregateItems.Count > 0 || request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Entity streams require entity rows; use a record or aggregation query");
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
            return context.InitializeEntity("OrderLine", Generated.Models.OrderLine.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
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