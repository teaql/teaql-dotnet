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
    public class OrderStatusRequest
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

        public OrderStatusRequest()
        {
            _query = new SelectQuery("OrderStatus");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public OrderStatusRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public OrderStatusRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public OrderStatusRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public OrderStatusRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderStatusRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderStatusRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public OrderStatusRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public OrderStatusRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public OrderStatusRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public OrderStatusRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public OrderStatusRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public OrderStatusRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public OrderStatusRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public OrderStatusRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("code");
            _query.Project("color");
            _query.Project("display_order");
            _query.Project("commerce_platform");
            _query.Project("version");
            return this;
        }

                public OrderStatusRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public OrderStatusRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public OrderStatusRequest SelectCode()
                {
                    _query.Project("code");
                    return this;
                }

                public OrderStatusRequest SelectColor()
                {
                    _query.Project("color");
                    return this;
                }

                public OrderStatusRequest SelectDisplayOrder()
                {
                    _query.Project("display_order");
                    return this;
                }


                public OrderStatusRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public OrderStatusRequest SelectCommercePlatform()
                {
                    return SelectCommercePlatformWith(new CommercePlatformRequest());
                }

                public OrderStatusRequest SelectCommercePlatformWith(CommercePlatformRequest related)
                {
                    _query.Project("commerce_platform");
                    _query.ForwardRelationQuery("CommercePlatform", "CommercePlatform", "commerce_platform", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCommercePlatformWith", Relation: "CommercePlatform")
                    };
                    return this;
                }
                public OrderStatusRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderStatusRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("CommercePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderStatusRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderStatusRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public OrderStatusRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public OrderStatusRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderStatusRequest WithNameIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithNameIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithNameNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithNameGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithNameGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithNameLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithNameLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithNameBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderStatusRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public OrderStatusRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public OrderStatusRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public OrderStatusRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public OrderStatusRequest WithCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq("code", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderStatusRequest WithCodeIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("code", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithCodeIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("code", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithCodeNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("code", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithCodeGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("code", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithCodeGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("code", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithCodeLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("code", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithCodeLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("code", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithCodeBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("code", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderStatusRequest WithCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("code"));
                    return this;
                }

                public OrderStatusRequest WithCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("code"));
                    return this;
                }
                public OrderStatusRequest WithCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("code", val));
                    return this;
                }

                public OrderStatusRequest WithCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("code", val));
                    return this;
                }

                public OrderStatusRequest WithColorContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorIs(string val)
                {
                    _query.AndFilter(Expr.Eq("color", TeaqlQueryValue(val)));
                    return this;
                }
                public OrderStatusRequest WithColorIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("color", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithColorIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("color", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithColorNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("color", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithColorGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("color", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithColorGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("color", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithColorLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("color", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithColorLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("color", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithColorBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("color", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderStatusRequest WithColorIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("color"));
                    return this;
                }

                public OrderStatusRequest WithColorIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("color"));
                    return this;
                }
                public OrderStatusRequest WithColorNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("color", val));
                    return this;
                }

                public OrderStatusRequest WithColorSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("color", val));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIs(decimal val)
                {
                    _query.AndFilter(Expr.Eq("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIsNot(decimal val)
                {
                    _query.AndFilter(Expr.Ne("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.In("display_order", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderNotIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.NotIn("display_order", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderGreaterThan(decimal val)
                {
                    _query.AndFilter(Expr.Gt("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderGreaterThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Gte("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderLessThan(decimal val)
                {
                    _query.AndFilter(Expr.Lt("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderLessThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Lte("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderBetween(decimal lower, decimal upper)
                {
                    _query.AndFilter(Expr.Between("display_order", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("display_order"));
                    return this;
                }

                public OrderStatusRequest WithDisplayOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("display_order"));
                    return this;
                }

                public OrderStatusRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public OrderStatusRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public OrderStatusRequest WithCommercePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("commerce_platform"));
                    return this;
                }

                public OrderStatusRequest WithCommercePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("commerce_platform"));
                    return this;
                }

                public OrderStatusRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public OrderStatusRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public OrderStatusRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderStatusRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public OrderStatusRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public OrderStatusRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public OrderStatusRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public OrderStatusRequest OrderByCodeAscending()
                {
                    _query.OrderBy("code", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByCodeDescending()
                {
                    _query.OrderBy("code", "desc");
                    return this;
                }

                public OrderStatusRequest OrderByColorAscending()
                {
                    _query.OrderBy("color", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByColorDescending()
                {
                    _query.OrderBy("color", "desc");
                    return this;
                }

                public OrderStatusRequest OrderByDisplayOrderAscending()
                {
                    _query.OrderBy("display_order", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByDisplayOrderDescending()
                {
                    _query.OrderBy("display_order", "desc");
                    return this;
                }


                public OrderStatusRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public OrderStatusRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public OrderStatusRequest Count()
        {
            return CountAs("count");
        }

        public OrderStatusRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public OrderStatusRequest MinDisplayOrder()
                {
                    return MinDisplayOrderAs("minOfDisplayOrder");
                }

                public OrderStatusRequest MinDisplayOrderAs(string retName)
                {
                    _query.Aggregate("min", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest MaxDisplayOrder()
                {
                    return MaxDisplayOrderAs("maxOfDisplayOrder");
                }

                public OrderStatusRequest MaxDisplayOrderAs(string retName)
                {
                    _query.Aggregate("max", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest SumDisplayOrder()
                {
                    return SumDisplayOrderAs("sumOfDisplayOrder");
                }

                public OrderStatusRequest SumDisplayOrderAs(string retName)
                {
                    _query.Aggregate("sum", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest AvgDisplayOrder()
                {
                    return AvgDisplayOrderAs("avgOfDisplayOrder");
                }

                public OrderStatusRequest AvgDisplayOrderAs(string retName)
                {
                    _query.Aggregate("avg", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest StandardDeviationDisplayOrder()
                {
                    return StandardDeviationDisplayOrderAs("standardDeviationOfDisplayOrder");
                }

                public OrderStatusRequest StandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest SquareRootOfPopulationStandardDeviationDisplayOrder()
                {
                    return SquareRootOfPopulationStandardDeviationDisplayOrderAs("squareRootOfPopulationStandardDeviationOfDisplayOrder");
                }

                public OrderStatusRequest SquareRootOfPopulationStandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest SampleVarianceDisplayOrder()
                {
                    return SampleVarianceDisplayOrderAs("sampleVarianceOfDisplayOrder");
                }

                public OrderStatusRequest SampleVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_samp", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest SamplePopulationVarianceDisplayOrder()
                {
                    return SamplePopulationVarianceDisplayOrderAs("samplePopulationVarianceOfDisplayOrder");
                }

                public OrderStatusRequest SamplePopulationVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_pop", "display_order", retName);
                    return this;
                }
                public OrderStatusRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public OrderStatusRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id");
                    _query.ProjectExpr(retName, Expr.Column("id"));
                    return this;
                }
                public OrderStatusRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public OrderStatusRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name");
                    _query.ProjectExpr(retName, Expr.Column("name"));
                    return this;
                }
                public OrderStatusRequest GroupByCode()
                {
                    _query.GroupBy("code");
                    return this;
                }

                public OrderStatusRequest GroupByCodeAs(string retName)
                {
                    _query.GroupBy("code");
                    _query.ProjectExpr(retName, Expr.Column("code"));
                    return this;
                }
                public OrderStatusRequest GroupByColor()
                {
                    _query.GroupBy("color");
                    return this;
                }

                public OrderStatusRequest GroupByColorAs(string retName)
                {
                    _query.GroupBy("color");
                    _query.ProjectExpr(retName, Expr.Column("color"));
                    return this;
                }
                public OrderStatusRequest GroupByDisplayOrder()
                {
                    _query.GroupBy("display_order");
                    return this;
                }

                public OrderStatusRequest GroupByDisplayOrderAs(string retName)
                {
                    _query.GroupBy("display_order");
                    _query.ProjectExpr(retName, Expr.Column("display_order"));
                    return this;
                }
                public OrderStatusRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public OrderStatusRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform");
                    _query.ProjectExpr(retName, Expr.Column("commerce_platform"));
                    return this;
                }
                public OrderStatusRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public OrderStatusRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version");
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public OrderStatusRequest SelectCustomerOrderList()
                {
                    return SelectCustomerOrderListWith(new CustomerOrderRequest());
                }

                public OrderStatusRequest SelectCustomerOrderListWith(CustomerOrderRequest child)
                {
                    _query.RelationQuery("CustomerOrderList", "CustomerOrder", "status", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderListWith", Relation: "CustomerOrderList")
                    };
                    return this;
                }
                public OrderStatusRequest HaveCustomerOrders()
                    => WithCustomerOrderListMatching(new CustomerOrderRequest());

                public OrderStatusRequest HaveNoCustomerOrders()
                    => WithoutCustomerOrderListMatching(new CustomerOrderRequest());

                public OrderStatusRequest WithCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "status"));
                    return this;
                }

                public OrderStatusRequest WithoutCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "status"));
                    return this;
                }
                public OrderStatusRequest CountCustomerOrders()
                    => CountCustomerOrdersAs("countCustomerOrders");

                public OrderStatusRequest CountCustomerOrdersAs(string alias)
                {
                    CountCustomerOrdersWith(alias, new CustomerOrderRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }

                public OrderStatusRequest CountCustomerOrdersWith(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersWith", alias, "CustomerOrderList")
                    };
                    return this;
                }

                public OrderStatusRequest MinTotalAmountOfCustomerOrders()
                    => MinTotalAmountOfCustomerOrdersAs("minOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest MinTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public OrderStatusRequest MaxTotalAmountOfCustomerOrders()
                    => MaxTotalAmountOfCustomerOrdersAs("maxOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest MaxTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public OrderStatusRequest SumTotalAmountOfCustomerOrders()
                    => SumTotalAmountOfCustomerOrdersAs("sumOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest SumTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public OrderStatusRequest AvgTotalAmountOfCustomerOrders()
                    => AvgTotalAmountOfCustomerOrdersAs("avgOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest AvgTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public OrderStatusRequest StandardDeviationTotalAmountOfCustomerOrders()
                    => StandardDeviationTotalAmountOfCustomerOrdersAs("standardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest StandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public OrderStatusRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrders()
                    => SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs("squareRootOfPopulationStandardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public OrderStatusRequest SampleVarianceTotalAmountOfCustomerOrders()
                    => SampleVarianceTotalAmountOfCustomerOrdersAs("sampleVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest SampleVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public OrderStatusRequest SamplePopulationVarianceTotalAmountOfCustomerOrders()
                    => SamplePopulationVarianceTotalAmountOfCustomerOrdersAs("samplePopulationVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public OrderStatusRequest SamplePopulationVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public OrderStatusRequest FacetByCommercePlatformAs(
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

        private async Task<Generated.Models.OrderStatus?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.OrderStatus.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.OrderStatus>> ExecuteForListWithMetadataInternalAsync(
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
            var entities = new SmartList<Generated.Models.OrderStatus>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.OrderStatus.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.OrderStatus> ExecuteForStreamInternalAsync(
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
                    Generated.Models.OrderStatus.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableOrderStatusRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.OrderStatus?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.OrderStatus>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderStatus>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableOrderStatusRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.OrderStatus?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.OrderStatus>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderStatus>> executeForStream,
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

        public ExecutableOrderStatusRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.OrderStatus> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("OrderStatus").ConfigureAwait(false));
            var entity = Generated.Models.OrderStatus.TeaqlNewWithReservedId(identity);
            return context.InitializeEntity("OrderStatus", entity,
                initialized => initialized.TeaqlValidateReservedIdentity(identity));
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.OrderStatus>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.OrderStatus> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.OrderStatus?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}