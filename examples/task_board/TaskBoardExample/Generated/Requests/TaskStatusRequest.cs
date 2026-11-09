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
    public class TaskStatusRequest
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

        public TaskStatusRequest()
        {
            _query = new SelectQuery("TaskStatus");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public TaskStatusRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public TaskStatusRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public TaskStatusRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public TaskStatusRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableTaskStatusRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableTaskStatusRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public TaskStatusRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public TaskStatusRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public TaskStatusRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public TaskStatusRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public TaskStatusRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public TaskStatusRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public TaskStatusRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public TaskStatusRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("code");
            _query.Project("color");
            _query.Project("display_order");
            _query.Project("progress");
            _query.Project("platform");
            _query.Project("version");
            return this;
        }

                public TaskStatusRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public TaskStatusRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public TaskStatusRequest SelectCode()
                {
                    _query.Project("code");
                    return this;
                }

                public TaskStatusRequest SelectColor()
                {
                    _query.Project("color");
                    return this;
                }

                public TaskStatusRequest SelectDisplayOrder()
                {
                    _query.Project("display_order");
                    return this;
                }

                public TaskStatusRequest SelectProgress()
                {
                    _query.Project("progress");
                    return this;
                }


                public TaskStatusRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public TaskStatusRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public TaskStatusRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project("platform");
                    _query.ForwardRelationQuery("Platform", "Platform", "platform", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: "Platform")
                    };
                    return this;
                }
                public TaskStatusRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskStatusRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public TaskStatusRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public TaskStatusRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public TaskStatusRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public TaskStatusRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public TaskStatusRequest WithNameIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithNameNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithNameGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithNameBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public TaskStatusRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public TaskStatusRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public TaskStatusRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public TaskStatusRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public TaskStatusRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public TaskStatusRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public TaskStatusRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public TaskStatusRequest WithCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("code", val));
                    return this;
                }

                public TaskStatusRequest WithCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq("code", TeaqlQueryValue(val)));
                    return this;
                }
                public TaskStatusRequest WithCodeIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("code", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("code", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithCodeNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("code", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithCodeGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("code", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("code", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("code", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("code", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithCodeBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("code", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("code"));
                    return this;
                }

                public TaskStatusRequest WithCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("code"));
                    return this;
                }
                public TaskStatusRequest WithCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("code", val));
                    return this;
                }

                public TaskStatusRequest WithCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("code", val));
                    return this;
                }

                public TaskStatusRequest WithCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("code", val));
                    return this;
                }

                public TaskStatusRequest WithCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("code", val));
                    return this;
                }

                public TaskStatusRequest WithCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("code", val));
                    return this;
                }

                public TaskStatusRequest WithCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("code", val));
                    return this;
                }

                public TaskStatusRequest WithColorContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("color", val));
                    return this;
                }

                public TaskStatusRequest WithColorIs(string val)
                {
                    _query.AndFilter(Expr.Eq("color", TeaqlQueryValue(val)));
                    return this;
                }
                public TaskStatusRequest WithColorIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("color", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("color", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithColorNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("color", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithColorGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("color", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("color", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("color", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("color", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithColorBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("color", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithColorIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("color"));
                    return this;
                }

                public TaskStatusRequest WithColorIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("color"));
                    return this;
                }
                public TaskStatusRequest WithColorNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("color", val));
                    return this;
                }

                public TaskStatusRequest WithColorStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("color", val));
                    return this;
                }

                public TaskStatusRequest WithColorNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("color", val));
                    return this;
                }

                public TaskStatusRequest WithColorEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("color", val));
                    return this;
                }

                public TaskStatusRequest WithColorNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("color", val));
                    return this;
                }

                public TaskStatusRequest WithColorSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("color", val));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIs(decimal val)
                {
                    _query.AndFilter(Expr.Eq("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIsNot(decimal val)
                {
                    _query.AndFilter(Expr.Ne("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.In("display_order", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderNotIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.NotIn("display_order", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderGreaterThan(decimal val)
                {
                    _query.AndFilter(Expr.Gt("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderGreaterThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Gte("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderLessThan(decimal val)
                {
                    _query.AndFilter(Expr.Lt("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderLessThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Lte("display_order", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderBetween(decimal lower, decimal upper)
                {
                    _query.AndFilter(Expr.Between("display_order", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("display_order"));
                    return this;
                }

                public TaskStatusRequest WithDisplayOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("display_order"));
                    return this;
                }

                public TaskStatusRequest WithProgressIs(decimal val)
                {
                    _query.AndFilter(Expr.Eq("progress", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressIsNot(decimal val)
                {
                    _query.AndFilter(Expr.Ne("progress", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.In("progress", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithProgressNotIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.NotIn("progress", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithProgressGreaterThan(decimal val)
                {
                    _query.AndFilter(Expr.Gt("progress", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressGreaterThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Gte("progress", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressLessThan(decimal val)
                {
                    _query.AndFilter(Expr.Lt("progress", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressLessThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Lte("progress", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithProgressBetween(decimal lower, decimal upper)
                {
                    _query.AndFilter(Expr.Between("progress", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithProgressIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("progress"));
                    return this;
                }

                public TaskStatusRequest WithProgressIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("progress"));
                    return this;
                }

                public TaskStatusRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("platform", val));
                    return this;
                }

                public TaskStatusRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("platform", vals));
                    return this;
                }

                public TaskStatusRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("platform"));
                    return this;
                }

                public TaskStatusRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("platform"));
                    return this;
                }

                public TaskStatusRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public TaskStatusRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TaskStatusRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TaskStatusRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public TaskStatusRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public TaskStatusRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public TaskStatusRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public TaskStatusRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public TaskStatusRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public TaskStatusRequest OrderByCodeAscending()
                {
                    _query.OrderBy("code", "asc");
                    return this;
                }

                public TaskStatusRequest OrderByCodeDescending()
                {
                    _query.OrderBy("code", "desc");
                    return this;
                }

                public TaskStatusRequest OrderByColorAscending()
                {
                    _query.OrderBy("color", "asc");
                    return this;
                }

                public TaskStatusRequest OrderByColorDescending()
                {
                    _query.OrderBy("color", "desc");
                    return this;
                }

                public TaskStatusRequest OrderByDisplayOrderAscending()
                {
                    _query.OrderBy("display_order", "asc");
                    return this;
                }

                public TaskStatusRequest OrderByDisplayOrderDescending()
                {
                    _query.OrderBy("display_order", "desc");
                    return this;
                }

                public TaskStatusRequest OrderByProgressAscending()
                {
                    _query.OrderBy("progress", "asc");
                    return this;
                }

                public TaskStatusRequest OrderByProgressDescending()
                {
                    _query.OrderBy("progress", "desc");
                    return this;
                }


                public TaskStatusRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public TaskStatusRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public TaskStatusRequest Count()
        {
            return CountAs("count");
        }

        public TaskStatusRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public TaskStatusRequest MinDisplayOrder()
                {
                    return MinDisplayOrderAs("minOfDisplayOrder");
                }

                public TaskStatusRequest MinDisplayOrderAs(string retName)
                {
                    _query.Aggregate("min", "display_order", retName);
                    return this;
                }
                public TaskStatusRequest MaxDisplayOrder()
                {
                    return MaxDisplayOrderAs("maxOfDisplayOrder");
                }

                public TaskStatusRequest MaxDisplayOrderAs(string retName)
                {
                    _query.Aggregate("max", "display_order", retName);
                    return this;
                }
                public TaskStatusRequest SumDisplayOrder()
                {
                    return SumDisplayOrderAs("sumOfDisplayOrder");
                }

                public TaskStatusRequest SumDisplayOrderAs(string retName)
                {
                    _query.Aggregate("sum", "display_order", retName);
                    return this;
                }
                public TaskStatusRequest AvgDisplayOrder()
                {
                    return AvgDisplayOrderAs("avgOfDisplayOrder");
                }

                public TaskStatusRequest AvgDisplayOrderAs(string retName)
                {
                    _query.Aggregate("avg", "display_order", retName);
                    return this;
                }
                public TaskStatusRequest StandardDeviationDisplayOrder()
                {
                    return StandardDeviationDisplayOrderAs("standardDeviationOfDisplayOrder");
                }

                public TaskStatusRequest StandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev", "display_order", retName);
                    return this;
                }
                public TaskStatusRequest SquareRootOfPopulationStandardDeviationDisplayOrder()
                {
                    return SquareRootOfPopulationStandardDeviationDisplayOrderAs("squareRootOfPopulationStandardDeviationOfDisplayOrder");
                }

                public TaskStatusRequest SquareRootOfPopulationStandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "display_order", retName);
                    return this;
                }
                public TaskStatusRequest SampleVarianceDisplayOrder()
                {
                    return SampleVarianceDisplayOrderAs("sampleVarianceOfDisplayOrder");
                }

                public TaskStatusRequest SampleVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_samp", "display_order", retName);
                    return this;
                }
                public TaskStatusRequest SamplePopulationVarianceDisplayOrder()
                {
                    return SamplePopulationVarianceDisplayOrderAs("samplePopulationVarianceOfDisplayOrder");
                }

                public TaskStatusRequest SamplePopulationVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_pop", "display_order", retName);
                    return this;
                }
                public TaskStatusRequest MinProgress()
                {
                    return MinProgressAs("minOfProgress");
                }

                public TaskStatusRequest MinProgressAs(string retName)
                {
                    _query.Aggregate("min", "progress", retName);
                    return this;
                }
                public TaskStatusRequest MaxProgress()
                {
                    return MaxProgressAs("maxOfProgress");
                }

                public TaskStatusRequest MaxProgressAs(string retName)
                {
                    _query.Aggregate("max", "progress", retName);
                    return this;
                }
                public TaskStatusRequest SumProgress()
                {
                    return SumProgressAs("sumOfProgress");
                }

                public TaskStatusRequest SumProgressAs(string retName)
                {
                    _query.Aggregate("sum", "progress", retName);
                    return this;
                }
                public TaskStatusRequest AvgProgress()
                {
                    return AvgProgressAs("avgOfProgress");
                }

                public TaskStatusRequest AvgProgressAs(string retName)
                {
                    _query.Aggregate("avg", "progress", retName);
                    return this;
                }
                public TaskStatusRequest StandardDeviationProgress()
                {
                    return StandardDeviationProgressAs("standardDeviationOfProgress");
                }

                public TaskStatusRequest StandardDeviationProgressAs(string retName)
                {
                    _query.Aggregate("stddev", "progress", retName);
                    return this;
                }
                public TaskStatusRequest SquareRootOfPopulationStandardDeviationProgress()
                {
                    return SquareRootOfPopulationStandardDeviationProgressAs("squareRootOfPopulationStandardDeviationOfProgress");
                }

                public TaskStatusRequest SquareRootOfPopulationStandardDeviationProgressAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "progress", retName);
                    return this;
                }
                public TaskStatusRequest SampleVarianceProgress()
                {
                    return SampleVarianceProgressAs("sampleVarianceOfProgress");
                }

                public TaskStatusRequest SampleVarianceProgressAs(string retName)
                {
                    _query.Aggregate("var_samp", "progress", retName);
                    return this;
                }
                public TaskStatusRequest SamplePopulationVarianceProgress()
                {
                    return SamplePopulationVarianceProgressAs("samplePopulationVarianceOfProgress");
                }

                public TaskStatusRequest SamplePopulationVarianceProgressAs(string retName)
                {
                    _query.Aggregate("var_pop", "progress", retName);
                    return this;
                }
                public TaskStatusRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public TaskStatusRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id");
                    _query.ProjectExpr(retName, Expr.Column("id"));
                    return this;
                }
                public TaskStatusRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public TaskStatusRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name");
                    _query.ProjectExpr(retName, Expr.Column("name"));
                    return this;
                }
                public TaskStatusRequest GroupByCode()
                {
                    _query.GroupBy("code");
                    return this;
                }

                public TaskStatusRequest GroupByCodeAs(string retName)
                {
                    _query.GroupBy("code");
                    _query.ProjectExpr(retName, Expr.Column("code"));
                    return this;
                }
                public TaskStatusRequest GroupByColor()
                {
                    _query.GroupBy("color");
                    return this;
                }

                public TaskStatusRequest GroupByColorAs(string retName)
                {
                    _query.GroupBy("color");
                    _query.ProjectExpr(retName, Expr.Column("color"));
                    return this;
                }
                public TaskStatusRequest GroupByDisplayOrder()
                {
                    _query.GroupBy("display_order");
                    return this;
                }

                public TaskStatusRequest GroupByDisplayOrderAs(string retName)
                {
                    _query.GroupBy("display_order");
                    _query.ProjectExpr(retName, Expr.Column("display_order"));
                    return this;
                }
                public TaskStatusRequest GroupByProgress()
                {
                    _query.GroupBy("progress");
                    return this;
                }

                public TaskStatusRequest GroupByProgressAs(string retName)
                {
                    _query.GroupBy("progress");
                    _query.ProjectExpr(retName, Expr.Column("progress"));
                    return this;
                }
                public TaskStatusRequest GroupByPlatform()
                {
                    _query.GroupBy("platform");
                    return this;
                }

                public TaskStatusRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy("platform");
                    _query.ProjectExpr(retName, Expr.Column("platform"));
                    return this;
                }
                public TaskStatusRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public TaskStatusRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version");
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public TaskStatusRequest SelectTaskList()
                {
                    return SelectTaskListWith(new TaskRequest());
                }

                public TaskStatusRequest SelectTaskListWith(TaskRequest child)
                {
                    _query.RelationQuery("TaskList", "Task", "status", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTaskListWith", Relation: "TaskList")
                    };
                    return this;
                }
                public TaskStatusRequest HaveTasks()
                    => WithTaskListMatching(new TaskRequest());

                public TaskStatusRequest HaveNoTasks()
                    => WithoutTaskListMatching(new TaskRequest());

                public TaskStatusRequest WithTaskListMatching(TaskRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, child.GetQuery(), "status"));
                    return this;
                }

                public TaskStatusRequest WithoutTaskListMatching(TaskRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, child.GetQuery(), "status"));
                    return this;
                }
                public TaskStatusRequest CountTasks()
                    => CountTasksAs("countTasks");

                public TaskStatusRequest CountTasksAs(string alias)
                {
                    CountTasksWith(alias, new TaskRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTasksAs", alias, "TaskList")
                    };
                    return this;
                }

                public TaskStatusRequest CountTasksWith(string alias, TaskRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("TaskList", "Task", "status", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTasksWith", alias, "TaskList")
                    };
                    return this;
                }


                public TaskStatusRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Platform", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, "platform")
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

        private async Task<Generated.Models.TaskStatus?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.TaskStatus.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.TaskStatus>> ExecuteForListWithMetadataInternalAsync(
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
            var entities = new SmartList<Generated.Models.TaskStatus>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.TaskStatus.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.TaskStatus> ExecuteForStreamInternalAsync(
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
                    Generated.Models.TaskStatus.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableTaskStatusRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.TaskStatus?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.TaskStatus>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.TaskStatus>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableTaskStatusRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.TaskStatus?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.TaskStatus>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.TaskStatus>> executeForStream,
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

        public ExecutableTaskStatusRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.TaskStatus> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("TaskStatus").ConfigureAwait(false));
            var entity = Generated.Models.TaskStatus.TeaqlNewWithReservedId(identity);
            return context.InitializeEntity("TaskStatus", entity,
                initialized => initialized.TeaqlValidateReservedIdentity(identity));
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.TaskStatus>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.TaskStatus> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.TaskStatus?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}