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
    public class ProductRequest
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

        public ProductRequest()
        {
            _query = new SelectQuery("Product");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public ProductRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

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
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableProductRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
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
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCommercePlatformWith", Relation: "CommercePlatform")
                    };
                    return this;
                }
                public ProductRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public ProductRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public ProductRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public ProductRequest WithNameIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithNameIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithNameNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithNameGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithNameGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithNameLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithNameLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithNameBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
                    _query.AndFilter(Expr.Eq("sku", TeaqlQueryValue(val)));
                    return this;
                }
                public ProductRequest WithSkuIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithSkuIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("sku", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithSkuNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("sku", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithSkuGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithSkuGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithSkuLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithSkuLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("sku", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithSkuBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("sku", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
                    _query.AndFilter(Expr.Eq("image_url", TeaqlQueryValue(val)));
                    return this;
                }
                public ProductRequest WithImageUrlIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("image_url", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithImageUrlIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("image_url", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithImageUrlNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("image_url", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithImageUrlGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("image_url", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithImageUrlGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("image_url", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithImageUrlLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("image_url", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithImageUrlLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("image_url", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithImageUrlBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("image_url", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public ProductRequest WithCreateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithCreateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithCreateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithCreateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithCreateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithCreateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithCreateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithCreateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithCreateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("create_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public ProductRequest WithUpdateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithUpdateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithUpdateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithUpdateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithUpdateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithUpdateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithUpdateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithUpdateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithUpdateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("update_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public ProductRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ProductRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ProductRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
            return CountAs("count");
        }

        public ProductRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
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
                    _query.ProjectExpr(retName, Expr.Column("id"));
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
                    _query.ProjectExpr(retName, Expr.Column("name"));
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
                    _query.ProjectExpr(retName, Expr.Column("sku"));
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
                    _query.ProjectExpr(retName, Expr.Column("image_url"));
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
                    _query.ProjectExpr(retName, Expr.Column("commerce_platform"));
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
                    _query.ProjectExpr(retName, Expr.Column("create_time"));
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
                    _query.ProjectExpr(retName, Expr.Column("update_time"));
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
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public ProductRequest SelectOrderLineList()
                {
                    return SelectOrderLineListWith(new OrderLineRequest());
                }

                public ProductRequest SelectOrderLineListWith(OrderLineRequest child)
                {
                    _query.RelationQuery("OrderLineList", "OrderLine", "product", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectOrderLineListWith", Relation: "OrderLineList")
                    };
                    return this;
                }
                public ProductRequest HaveOrderLines()
                    => WithOrderLineListMatching(new OrderLineRequest());

                public ProductRequest HaveNoOrderLines()
                    => WithoutOrderLineListMatching(new OrderLineRequest());

                public ProductRequest WithOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderLine")!, child.GetQuery(), "product"));
                    return this;
                }

                public ProductRequest WithoutOrderLineListMatching(OrderLineRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderLine")!, child.GetQuery(), "product"));
                    return this;
                }
                public ProductRequest CountOrderLines()
                    => CountOrderLinesAs("countOrderLines");

                public ProductRequest CountOrderLinesAs(string alias)
                {
                    CountOrderLinesWith(alias, new OrderLineRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }

                public ProductRequest CountOrderLinesWith(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderLinesWith", alias, "OrderLineList")
                    };
                    return this;
                }

                public ProductRequest MinQuantityOfOrderLines()
                    => MinQuantityOfOrderLinesAs("minOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest MinQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public ProductRequest MaxQuantityOfOrderLines()
                    => MaxQuantityOfOrderLinesAs("maxOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest MaxQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public ProductRequest SumQuantityOfOrderLines()
                    => SumQuantityOfOrderLinesAs("sumOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest SumQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public ProductRequest AvgQuantityOfOrderLines()
                    => AvgQuantityOfOrderLinesAs("avgOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest AvgQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public ProductRequest StandardDeviationQuantityOfOrderLines()
                    => StandardDeviationQuantityOfOrderLinesAs("standardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest StandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public ProductRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLines()
                    => SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs("squareRootOfPopulationStandardDeviationOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public ProductRequest SampleVarianceQuantityOfOrderLines()
                    => SampleVarianceQuantityOfOrderLinesAs("sampleVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest SampleVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public ProductRequest SamplePopulationVarianceQuantityOfOrderLines()
                    => SamplePopulationVarianceQuantityOfOrderLinesAs("samplePopulationVarianceOfQuantityOfOrderLines", new OrderLineRequest());

                public ProductRequest SamplePopulationVarianceQuantityOfOrderLinesAs(string alias, OrderLineRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "quantity", alias);
                    _query.RelationAggregate("OrderLineList", "OrderLine", "product", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceQuantityOfOrderLinesAs", alias, "OrderLineList")
                    };
                    return this;
                }
                public ProductRequest FacetByCommercePlatformAs(
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

        private async Task<Generated.Models.Product?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.Product.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.Product>> ExecuteForListWithMetadataInternalAsync(
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
            var entities = new SmartList<Generated.Models.Product>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Product.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Product> ExecuteForStreamInternalAsync(
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
                    Generated.Models.Product.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableProductRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Product?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Product>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Product>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableProductRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Product?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Product>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Product>> executeForStream,
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

        public ExecutableProductRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.Product> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Product").ConfigureAwait(false));
            var entity = Generated.Models.Product.TeaqlNewWithReservedId(identity);
            return context.InitializeEntity("Product", entity,
                initialized => initialized.TeaqlValidateReservedIdentity(identity));
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Product>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Product> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Product?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}