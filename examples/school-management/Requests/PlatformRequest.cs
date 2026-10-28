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
    public class PlatformRequest
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

        public PlatformRequest()
        {
            _query = new SelectQuery("Platform");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public PlatformRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public PlatformRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public PlatformRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public PlatformRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutablePlatformRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutablePlatformRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public PlatformRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public PlatformRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public PlatformRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public PlatformRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public PlatformRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public PlatformRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public PlatformRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public PlatformRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("base_url");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public PlatformRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public PlatformRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public PlatformRequest SelectBaseUrl()
                {
                    _query.Project("base_url");
                    return this;
                }

                public PlatformRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public PlatformRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public PlatformRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }


                public PlatformRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public PlatformRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public PlatformRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public PlatformRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public PlatformRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public PlatformRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public PlatformRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public PlatformRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlIs(string val)
                {
                    _query.AndFilter(Expr.Eq("base_url", TeaqlQueryValue(val)));
                    return this;
                }
                public PlatformRequest WithBaseUrlIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("base_url", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithBaseUrlIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("base_url", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithBaseUrlNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("base_url", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithBaseUrlGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("base_url", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithBaseUrlGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("base_url", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithBaseUrlLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("base_url", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithBaseUrlLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("base_url", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithBaseUrlBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("base_url", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithBaseUrlIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("base_url"));
                    return this;
                }

                public PlatformRequest WithBaseUrlIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("base_url"));
                    return this;
                }
                public PlatformRequest WithBaseUrlNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("base_url", val));
                    return this;
                }

                public PlatformRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public PlatformRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public PlatformRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("update_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public PlatformRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public PlatformRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PlatformRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public PlatformRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public PlatformRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public PlatformRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public PlatformRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public PlatformRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public PlatformRequest OrderByBaseUrlAscending()
                {
                    _query.OrderBy("base_url", "asc");
                    return this;
                }

                public PlatformRequest OrderByBaseUrlDescending()
                {
                    _query.OrderBy("base_url", "desc");
                    return this;
                }

                public PlatformRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public PlatformRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public PlatformRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public PlatformRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public PlatformRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public PlatformRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public PlatformRequest Count()
        {
            return CountAs("count");
        }

        public PlatformRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public PlatformRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public PlatformRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public PlatformRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public PlatformRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public PlatformRequest GroupByBaseUrl()
                {
                    _query.GroupBy("base_url");
                    return this;
                }

                public PlatformRequest GroupByBaseUrlAs(string retName)
                {
                    _query.GroupBy("base_url"); 
                    return this;
                }
                public PlatformRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public PlatformRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public PlatformRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public PlatformRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
                    return this;
                }
                public PlatformRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public PlatformRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public PlatformRequest SelectSchoolTypeList()
                {
                    return SelectSchoolTypeListWith(new SchoolTypeRequest());
                }

                public PlatformRequest SelectSchoolTypeListWith(SchoolTypeRequest child)
                {
                    _query.RelationQuery("SchoolTypeList", "SchoolType", "platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectSchoolTypeListWith", Relation: "SchoolTypeList")
                    };
                    return this;
                }
                public PlatformRequest SelectSchoolList()
                {
                    return SelectSchoolListWith(new SchoolRequest());
                }

                public PlatformRequest SelectSchoolListWith(SchoolRequest child)
                {
                    _query.RelationQuery("SchoolList", "School", "platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectSchoolListWith", Relation: "SchoolList")
                    };
                    return this;
                }
                public PlatformRequest HaveSchoolTypes()
                    => WithSchoolTypeListMatching(new SchoolTypeRequest());

                public PlatformRequest HaveNoSchoolTypes()
                    => WithoutSchoolTypeListMatching(new SchoolTypeRequest());

                public PlatformRequest WithSchoolTypeListMatching(SchoolTypeRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("SchoolType")!, child.GetQuery(), "platform"));
                    return this;
                }

                public PlatformRequest WithoutSchoolTypeListMatching(SchoolTypeRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("SchoolType")!, child.GetQuery(), "platform"));
                    return this;
                }
                public PlatformRequest HaveSchools()
                    => WithSchoolListMatching(new SchoolRequest());

                public PlatformRequest HaveNoSchools()
                    => WithoutSchoolListMatching(new SchoolRequest());

                public PlatformRequest WithSchoolListMatching(SchoolRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("School")!, child.GetQuery(), "platform"));
                    return this;
                }

                public PlatformRequest WithoutSchoolListMatching(SchoolRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("School")!, child.GetQuery(), "platform"));
                    return this;
                }
                public PlatformRequest CountSchoolTypes()
                    => CountSchoolTypesAs("countSchoolTypes");

                public PlatformRequest CountSchoolTypesAs(string alias)
                {
                    CountSchoolTypesWith(alias, new SchoolTypeRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountSchoolTypesAs", alias, "SchoolTypeList")
                    };
                    return this;
                }

                public PlatformRequest CountSchoolTypesWith(string alias, SchoolTypeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountSchoolTypesWith", alias, "SchoolTypeList")
                    };
                    return this;
                }

                public PlatformRequest MinDisplayOrderOfSchoolTypes()
                    => MinDisplayOrderOfSchoolTypesAs("minOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest MinDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "display_order", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinDisplayOrderOfSchoolTypesAs", alias, "SchoolTypeList")
                    };
                    return this;
                }
                public PlatformRequest MaxDisplayOrderOfSchoolTypes()
                    => MaxDisplayOrderOfSchoolTypesAs("maxOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest MaxDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "display_order", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxDisplayOrderOfSchoolTypesAs", alias, "SchoolTypeList")
                    };
                    return this;
                }
                public PlatformRequest SumDisplayOrderOfSchoolTypes()
                    => SumDisplayOrderOfSchoolTypesAs("sumOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest SumDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "display_order", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumDisplayOrderOfSchoolTypesAs", alias, "SchoolTypeList")
                    };
                    return this;
                }
                public PlatformRequest AvgDisplayOrderOfSchoolTypes()
                    => AvgDisplayOrderOfSchoolTypesAs("avgOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest AvgDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "display_order", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgDisplayOrderOfSchoolTypesAs", alias, "SchoolTypeList")
                    };
                    return this;
                }
                public PlatformRequest StandardDeviationDisplayOrderOfSchoolTypes()
                    => StandardDeviationDisplayOrderOfSchoolTypesAs("standardDeviationOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest StandardDeviationDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "display_order", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationDisplayOrderOfSchoolTypesAs", alias, "SchoolTypeList")
                    };
                    return this;
                }
                public PlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfSchoolTypes()
                    => SquareRootOfPopulationStandardDeviationDisplayOrderOfSchoolTypesAs("squareRootOfPopulationStandardDeviationOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "display_order", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationDisplayOrderOfSchoolTypesAs", alias, "SchoolTypeList")
                    };
                    return this;
                }
                public PlatformRequest SampleVarianceDisplayOrderOfSchoolTypes()
                    => SampleVarianceDisplayOrderOfSchoolTypesAs("sampleVarianceOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest SampleVarianceDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "display_order", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceDisplayOrderOfSchoolTypesAs", alias, "SchoolTypeList")
                    };
                    return this;
                }
                public PlatformRequest SamplePopulationVarianceDisplayOrderOfSchoolTypes()
                    => SamplePopulationVarianceDisplayOrderOfSchoolTypesAs("samplePopulationVarianceOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest SamplePopulationVarianceDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "display_order", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceDisplayOrderOfSchoolTypesAs", alias, "SchoolTypeList")
                    };
                    return this;
                }
                public PlatformRequest CountSchools()
                    => CountSchoolsAs("countSchools");

                public PlatformRequest CountSchoolsAs(string alias)
                {
                    CountSchoolsWith(alias, new SchoolRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }

                public PlatformRequest CountSchoolsWith(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountSchoolsWith", alias, "SchoolList")
                    };
                    return this;
                }

                public PlatformRequest MinStudentCapacityOfSchools()
                    => MinStudentCapacityOfSchoolsAs("minOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest MinStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public PlatformRequest MaxStudentCapacityOfSchools()
                    => MaxStudentCapacityOfSchoolsAs("maxOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest MaxStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public PlatformRequest SumStudentCapacityOfSchools()
                    => SumStudentCapacityOfSchoolsAs("sumOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest SumStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public PlatformRequest AvgStudentCapacityOfSchools()
                    => AvgStudentCapacityOfSchoolsAs("avgOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest AvgStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public PlatformRequest StandardDeviationStudentCapacityOfSchools()
                    => StandardDeviationStudentCapacityOfSchoolsAs("standardDeviationOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest StandardDeviationStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public PlatformRequest SquareRootOfPopulationStandardDeviationStudentCapacityOfSchools()
                    => SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs("squareRootOfPopulationStandardDeviationOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public PlatformRequest SampleVarianceStudentCapacityOfSchools()
                    => SampleVarianceStudentCapacityOfSchoolsAs("sampleVarianceOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest SampleVarianceStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
                    return this;
                }
                public PlatformRequest SamplePopulationVarianceStudentCapacityOfSchools()
                    => SamplePopulationVarianceStudentCapacityOfSchoolsAs("samplePopulationVarianceOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest SamplePopulationVarianceStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "student_capacity", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceStudentCapacityOfSchoolsAs", alias, "SchoolList")
                    };
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

        private async Task<Generated.Models.Platform?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.Platform.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.Platform>> ExecuteForListWithMetadataInternalAsync(
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
            var entities = new SmartList<Generated.Models.Platform>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Platform.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamInternalAsync(
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
                    Generated.Models.Platform.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutablePlatformRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Platform?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Platform>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePlatformRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Platform?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Platform>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> executeForStream,
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

        public ExecutablePlatformRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.Platform NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.Platform();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Platform>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Platform?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}