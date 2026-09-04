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
    public class ProductRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;

        public ProductRequest()
        {
            _query = new SelectQuery("Product");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public ProductRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public ProductRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public ProductRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableProductRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableProductRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
                c => Comment(c),
                EnsureIntent);
        }

        public ProductRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public ProductRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public ProductRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public ProductRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public ProductRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public ProductRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public ProductRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public ProductRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("sku");
            _query.Project("image_url");
            _query.Project("commerce_platform");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public ProductRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public ProductRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public ProductRequest SelectSku()
                {
                    _query.Project("sku");
                    return this;
                }

                public ProductRequest SelectImageUrl()
                {
                    _query.Project("image_url");
                    return this;
                }


                public ProductRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public ProductRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public ProductRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public ProductRequest SelectCommercePlatform()
                {
                    return SelectCommercePlatformWith(new CommercePlatformRequest());
                }

                public ProductRequest SelectCommercePlatformWith(CommercePlatformRequest related)
                {
                    _query.Project("commerce_platform");
                    _query.ForwardRelationQuery("CommercePlatform", "CommercePlatform", "commerce_platform", related.GetQuery());
                    return this;
                }
                public ProductRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public ProductRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public ProductRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public ProductRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public ProductRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public ProductRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public ProductRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public ProductRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public ProductRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public ProductRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public ProductRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
                    return this;
                }

                public ProductRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public ProductRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public ProductRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public ProductRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }
                public ProductRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", val));
                    return this;
                }

                public ProductRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public ProductRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals));
                    return this;
                }

                public ProductRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", val));
                    return this;
                }

                public ProductRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", val));
                    return this;
                }

                public ProductRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", val));
                    return this;
                }

                public ProductRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", val));
                    return this;
                }

                public ProductRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", lower, upper));
                    return this;
                }

                public ProductRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public ProductRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public ProductRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public ProductRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public ProductRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public ProductRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public ProductRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public ProductRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public ProductRequest WithSkuContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("sku", val));
                    return this;
                }

                public ProductRequest WithSkuIs(string val)
                {
                    _query.AndFilter(Expr.Eq("sku", val));
                    return this;
                }
                public ProductRequest WithSkuIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("sku", val));
                    return this;
                }

                public ProductRequest WithSkuIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("sku", vals));
                    return this;
                }

                public ProductRequest WithSkuNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("sku", vals));
                    return this;
                }

                public ProductRequest WithSkuGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("sku", val));
                    return this;
                }

                public ProductRequest WithSkuGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("sku", val));
                    return this;
                }

                public ProductRequest WithSkuLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("sku", val));
                    return this;
                }

                public ProductRequest WithSkuLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("sku", val));
                    return this;
                }

                public ProductRequest WithSkuBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("sku", lower, upper));
                    return this;
                }

                public ProductRequest WithSkuIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("sku"));
                    return this;
                }

                public ProductRequest WithSkuIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("sku"));
                    return this;
                }
                public ProductRequest WithSkuNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("sku", val));
                    return this;
                }

                public ProductRequest WithSkuStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("sku", val));
                    return this;
                }

                public ProductRequest WithSkuNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("sku", val));
                    return this;
                }

                public ProductRequest WithSkuEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("sku", val));
                    return this;
                }

                public ProductRequest WithSkuNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("sku", val));
                    return this;
                }

                public ProductRequest WithSkuSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("sku", val));
                    return this;
                }

                public ProductRequest WithImageUrlContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlIs(string val)
                {
                    _query.AndFilter(Expr.Eq("image_url", val));
                    return this;
                }
                public ProductRequest WithImageUrlIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("image_url", vals));
                    return this;
                }

                public ProductRequest WithImageUrlNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("image_url", vals));
                    return this;
                }

                public ProductRequest WithImageUrlGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("image_url", lower, upper));
                    return this;
                }

                public ProductRequest WithImageUrlIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("image_url"));
                    return this;
                }

                public ProductRequest WithImageUrlIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("image_url"));
                    return this;
                }
                public ProductRequest WithImageUrlNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("image_url", val));
                    return this;
                }

                public ProductRequest WithImageUrlSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("image_url", val));
                    return this;
                }

                public ProductRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public ProductRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public ProductRequest WithCommercePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("commerce_platform"));
                    return this;
                }

                public ProductRequest WithCommercePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("commerce_platform"));
                    return this;
                }

                public ProductRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public ProductRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", val));
                    return this;
                }

                public ProductRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public ProductRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals));
                    return this;
                }

                public ProductRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", val));
                    return this;
                }

                public ProductRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public ProductRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", val));
                    return this;
                }

                public ProductRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public ProductRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", lower, upper));
                    return this;
                }

                public ProductRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public ProductRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public ProductRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public ProductRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("update_time", val));
                    return this;
                }

                public ProductRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public ProductRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals));
                    return this;
                }

                public ProductRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("update_time", val));
                    return this;
                }

                public ProductRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public ProductRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("update_time", val));
                    return this;
                }

                public ProductRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public ProductRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("update_time", lower, upper));
                    return this;
                }

                public ProductRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public ProductRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public ProductRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public ProductRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public ProductRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public ProductRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public ProductRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public ProductRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public ProductRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public ProductRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public ProductRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
                    return this;
                }

                public ProductRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public ProductRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public ProductRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public ProductRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public ProductRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public ProductRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public ProductRequest OrderBySkuAscending()
                {
                    _query.OrderBy("sku", "asc");
                    return this;
                }

                public ProductRequest OrderBySkuDescending()
                {
                    _query.OrderBy("sku", "desc");
                    return this;
                }

                public ProductRequest OrderByImageUrlAscending()
                {
                    _query.OrderBy("image_url", "asc");
                    return this;
                }

                public ProductRequest OrderByImageUrlDescending()
                {
                    _query.OrderBy("image_url", "desc");
                    return this;
                }


                public ProductRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public ProductRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public ProductRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public ProductRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public ProductRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public ProductRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public ProductRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public ProductRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public ProductRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public ProductRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public ProductRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public ProductRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public ProductRequest GroupBySku()
                {
                    _query.GroupBy("sku");
                    return this;
                }

                public ProductRequest GroupBySkuAs(string retName)
                {
                    _query.GroupBy("sku"); 
                    return this;
                }
                public ProductRequest GroupByImageUrl()
                {
                    _query.GroupBy("image_url");
                    return this;
                }

                public ProductRequest GroupByImageUrlAs(string retName)
                {
                    _query.GroupBy("image_url"); 
                    return this;
                }
                public ProductRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public ProductRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform"); 
                    return this;
                }
                public ProductRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public ProductRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public ProductRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public ProductRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
                    return this;
                }
                public ProductRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public ProductRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public ProductRequest SelectOrderLineList()
                {
                    return SelectOrderLineListWith(new OrderLineRequest());
                }

                public ProductRequest SelectOrderLineListWith(OrderLineRequest child)
                {
                    _query.RelationQuery("OrderLineList", "OrderLine", "product", true, child.GetQuery());
                    return this;
                }
                public ProductRequest HaveOrderLines()
                    => WithOrderLineListMatching(new OrderLineRequest());

                public ProductRequest HaveNoOrderLines()
                    => WithoutOrderLineListMatching(new OrderLineRequest());

                public ProductRequest WithOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("OrderLine"), child.GetQuery(), "product"));
                    return this;
                }

                public ProductRequest WithoutOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("OrderLine"), child.GetQuery(), "product"));
                    return this;
                }
                public ProductRequest CountOrderLines()
                    => CountOrderLinesAs("countOrderLines");

                public ProductRequest CountOrderLinesAs(string alias)
                    => CountOrderLinesWith(alias, new OrderLineRequest());

                public ProductRequest CountOrderLinesWith(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, child.GetQuery(), true);
                    return this;
                }

                public ProductRequest MinQuantityOfOrderLines()
                    => MinQuantityOfOrderLinesAs("minOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest MinQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("min", "quantity", "min_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, child.GetQuery(), true);
                    return this;
                }
                public ProductRequest MaxQuantityOfOrderLines()
                    => MaxQuantityOfOrderLinesAs("maxOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest MaxQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("max", "quantity", "max_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, child.GetQuery(), true);
                    return this;
                }
                public ProductRequest SumQuantityOfOrderLines()
                    => SumQuantityOfOrderLinesAs("sumOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest SumQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("sum", "quantity", "sum_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, child.GetQuery(), true);
                    return this;
                }
                public ProductRequest AvgQuantityOfOrderLines()
                    => AvgQuantityOfOrderLinesAs("avgOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest AvgQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("avg", "quantity", "avg_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, child.GetQuery(), true);
                    return this;
                }
                public ProductRequest StandardDeviationQuantityOfOrderLines()
                    => StandardDeviationQuantityOfOrderLinesAs("standardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest StandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "quantity", "standardDeviation_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, child.GetQuery(), true);
                    return this;
                }
                public ProductRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLines()
                    => SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs("squareRootOfPopulationStandardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "quantity", "squareRootOfPopulationStandardDeviation_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, child.GetQuery(), true);
                    return this;
                }
                public ProductRequest SampleVarianceQuantityOfOrderLines()
                    => SampleVarianceQuantityOfOrderLinesAs("sampleVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest SampleVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "quantity", "sampleVariance_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, child.GetQuery(), true);
                    return this;
                }
                public ProductRequest SamplePopulationVarianceQuantityOfOrderLines()
                    => SamplePopulationVarianceQuantityOfOrderLinesAs("samplePopulationVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest SamplePopulationVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "quantity", "samplePopulationVariance_quantity");
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, child.GetQuery(), true);
                    return this;
                }
                public ProductRequest FacetByCommercePlatformAs(
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

        private async Task<ProductPage> ExecuteForPageInternalAsync(
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
                var countQuery = new SelectQuery("Product");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.Product>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.Product.FromRecord(row, queryRoot));
            return new ProductPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.Product> ExecuteForStreamInternalAsync(
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
                    yield return Generated.Models.Product.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableProductRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<ProductPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Product>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableProductRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<ProductPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Product>> executeForStream,
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

        public ExecutableProductRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.Product NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.Product();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.Product>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.Product>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.Product.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<ProductPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.Product> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.Product?> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class ProductPage
    {
        public SmartList<Generated.Models.Product> Rows { get; }
        public long TotalCount { get; }
        public ProductPage(
            SmartList<Generated.Models.Product> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}