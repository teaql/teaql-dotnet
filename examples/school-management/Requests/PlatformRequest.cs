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
    public class PlatformRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public PlatformRequest()
        {
            _query = new SelectQuery("Platform");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

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
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutablePlatformRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
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
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public PlatformRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public PlatformRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public PlatformRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public PlatformRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public PlatformRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public PlatformRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public PlatformRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public PlatformRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
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
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }
                public PlatformRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", val));
                    return this;
                }

                public PlatformRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public PlatformRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals));
                    return this;
                }

                public PlatformRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", val));
                    return this;
                }

                public PlatformRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", val));
                    return this;
                }

                public PlatformRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", val));
                    return this;
                }

                public PlatformRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", val));
                    return this;
                }

                public PlatformRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", lower, upper));
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
                    _query.AndFilter(Expr.Eq("base_url", val));
                    return this;
                }
                public PlatformRequest WithBaseUrlIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("base_url", vals));
                    return this;
                }

                public PlatformRequest WithBaseUrlNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("base_url", vals));
                    return this;
                }

                public PlatformRequest WithBaseUrlGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("base_url", val));
                    return this;
                }

                public PlatformRequest WithBaseUrlBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("base_url", lower, upper));
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
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public PlatformRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", val));
                    return this;
                }

                public PlatformRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public PlatformRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals));
                    return this;
                }

                public PlatformRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", val));
                    return this;
                }

                public PlatformRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public PlatformRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", val));
                    return this;
                }

                public PlatformRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public PlatformRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", lower, upper));
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
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public PlatformRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("update_time", val));
                    return this;
                }

                public PlatformRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public PlatformRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals));
                    return this;
                }

                public PlatformRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("update_time", val));
                    return this;
                }

                public PlatformRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public PlatformRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("update_time", val));
                    return this;
                }

                public PlatformRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public PlatformRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("update_time", lower, upper));
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
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public PlatformRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public PlatformRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public PlatformRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public PlatformRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public PlatformRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public PlatformRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public PlatformRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public PlatformRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
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
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public PlatformRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
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
                    return this;
                }
                public PlatformRequest SelectSchoolList()
                {
                    return SelectSchoolListWith(new SchoolRequest());
                }

                public PlatformRequest SelectSchoolListWith(SchoolRequest child)
                {
                    _query.RelationQuery("SchoolList", "School", "platform", true, child.GetQuery());
                    return this;
                }
                public PlatformRequest HaveSchoolTypes()
                    => WithSchoolTypeListMatching(new SchoolTypeRequest());

                public PlatformRequest HaveNoSchoolTypes()
                    => WithoutSchoolTypeListMatching(new SchoolTypeRequest());

                public PlatformRequest WithSchoolTypeListMatching(SchoolTypeRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("SchoolType"), child.GetQuery(), "platform"));
                    return this;
                }

                public PlatformRequest WithoutSchoolTypeListMatching(SchoolTypeRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("SchoolType"), child.GetQuery(), "platform"));
                    return this;
                }
                public PlatformRequest HaveSchools()
                    => WithSchoolListMatching(new SchoolRequest());

                public PlatformRequest HaveNoSchools()
                    => WithoutSchoolListMatching(new SchoolRequest());

                public PlatformRequest WithSchoolListMatching(SchoolRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("School"), child.GetQuery(), "platform"));
                    return this;
                }

                public PlatformRequest WithoutSchoolListMatching(SchoolRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("School"), child.GetQuery(), "platform"));
                    return this;
                }
                public PlatformRequest CountSchoolTypes()
                    => CountSchoolTypesAs("countSchoolTypes");

                public PlatformRequest CountSchoolTypesAs(string alias)
                    => CountSchoolTypesWith(alias, new SchoolTypeRequest());

                public PlatformRequest CountSchoolTypesWith(string alias, SchoolTypeRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, child.GetQuery(), true);
                    return this;
                }

                public PlatformRequest MinDisplayOrderOfSchoolTypes()
                    => MinDisplayOrderOfSchoolTypesAs("minOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest MinDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    child.GetQuery().Aggregate("min", "display_order", "min_display_order");
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest MaxDisplayOrderOfSchoolTypes()
                    => MaxDisplayOrderOfSchoolTypesAs("maxOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest MaxDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    child.GetQuery().Aggregate("max", "display_order", "max_display_order");
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest SumDisplayOrderOfSchoolTypes()
                    => SumDisplayOrderOfSchoolTypesAs("sumOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest SumDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    child.GetQuery().Aggregate("sum", "display_order", "sum_display_order");
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest AvgDisplayOrderOfSchoolTypes()
                    => AvgDisplayOrderOfSchoolTypesAs("avgOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest AvgDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    child.GetQuery().Aggregate("avg", "display_order", "avg_display_order");
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest StandardDeviationDisplayOrderOfSchoolTypes()
                    => StandardDeviationDisplayOrderOfSchoolTypesAs("standardDeviationOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest StandardDeviationDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "display_order", "standardDeviation_display_order");
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfSchoolTypes()
                    => SquareRootOfPopulationStandardDeviationDisplayOrderOfSchoolTypesAs("squareRootOfPopulationStandardDeviationOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "display_order", "squareRootOfPopulationStandardDeviation_display_order");
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest SampleVarianceDisplayOrderOfSchoolTypes()
                    => SampleVarianceDisplayOrderOfSchoolTypesAs("sampleVarianceOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest SampleVarianceDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "display_order", "sampleVariance_display_order");
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest SamplePopulationVarianceDisplayOrderOfSchoolTypes()
                    => SamplePopulationVarianceDisplayOrderOfSchoolTypesAs("samplePopulationVarianceOfDisplayOrderOfSchoolTypes", new SchoolTypeRequest());

                public PlatformRequest SamplePopulationVarianceDisplayOrderOfSchoolTypesAs(string alias, SchoolTypeRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "display_order", "samplePopulationVariance_display_order");
                    _query.RelationAggregate("SchoolTypeList", "SchoolType", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest CountSchools()
                    => CountSchoolsAs("countSchools");

                public PlatformRequest CountSchoolsAs(string alias)
                    => CountSchoolsWith(alias, new SchoolRequest());

                public PlatformRequest CountSchoolsWith(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, child.GetQuery(), true);
                    return this;
                }

                public PlatformRequest MinStudentCapacityOfSchools()
                    => MinStudentCapacityOfSchoolsAs("minOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest MinStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("min", "student_capacity", "min_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest MaxStudentCapacityOfSchools()
                    => MaxStudentCapacityOfSchoolsAs("maxOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest MaxStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("max", "student_capacity", "max_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest SumStudentCapacityOfSchools()
                    => SumStudentCapacityOfSchoolsAs("sumOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest SumStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("sum", "student_capacity", "sum_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest AvgStudentCapacityOfSchools()
                    => AvgStudentCapacityOfSchoolsAs("avgOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest AvgStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("avg", "student_capacity", "avg_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest StandardDeviationStudentCapacityOfSchools()
                    => StandardDeviationStudentCapacityOfSchoolsAs("standardDeviationOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest StandardDeviationStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "student_capacity", "standardDeviation_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest SquareRootOfPopulationStandardDeviationStudentCapacityOfSchools()
                    => SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs("squareRootOfPopulationStandardDeviationOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "student_capacity", "squareRootOfPopulationStandardDeviation_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest SampleVarianceStudentCapacityOfSchools()
                    => SampleVarianceStudentCapacityOfSchoolsAs("sampleVarianceOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest SampleVarianceStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "student_capacity", "sampleVariance_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, child.GetQuery(), true);
                    return this;
                }
                public PlatformRequest SamplePopulationVarianceStudentCapacityOfSchools()
                    => SamplePopulationVarianceStudentCapacityOfSchoolsAs("samplePopulationVarianceOfStudentCapacityOfSchools", new SchoolRequest());

                public PlatformRequest SamplePopulationVarianceStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "student_capacity", "samplePopulationVariance_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "platform", alias, child.GetQuery(), true);
                    return this;
                }

        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireDataService();
            var req = new QueryRequest(_query);
            var result = await service.QueryAsync(context, req);
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
                var membershipRows = (await service.QueryAsync(context, new QueryRequest(membership))).Rows;
                var counts = membershipRows
                    .Where(row => row.TryGetValue(facet.RelationName, out var value) && value.Raw != null)
                    .GroupBy(row => Convert.ToString(row[facet.RelationName].Raw))
                    .ToDictionary(group => group.Key, group => group.Count());

                var nested = facet.Query.Copy();
                nested.Facets.Clear();
                var countAliases = nested.Aggregates
                    .Where(aggregate => string.Equals(aggregate.Function, "Count", StringComparison.OrdinalIgnoreCase))
                    .Select(aggregate => aggregate.Alias).ToArray();
                nested.Aggregates.Clear();
                nested.GroupFields.Clear();
                var facetRows = (await service.QueryAsync(context, new QueryRequest(nested))).Rows;
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

        private async Task<PlatformPage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireDataService();
            _query.Offset(offset);
            _query.Limit(limit);
            var result = await service.QueryAsync(context, new QueryRequest(_query));
            long totalCount;
            if (_query.IdSetPagination != null && context.IdSetCountAccuracy == "EXACT")
            {
                totalCount = context.IdSetCount;
            }
            else
            {
                var countQuery = new SelectQuery("Platform");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(context, new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.Platform>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.Platform.FromRecord(row, queryRoot));
            return new PlatformPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireDataService();
            if (service is not IStreamingDataService streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            await foreach (var chunk in streaming.QueryStreamAsync(
                context, new QueryRequest(_query), chunkSize, cancellationToken).WithCancellation(cancellationToken))
            {
                var queryRoot = new EntityRoot();
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.Platform.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutablePlatformRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<PlatformPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePlatformRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<PlatformPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> executeForStream,
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

        public async Task<SmartList<Generated.Models.Platform>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.Platform>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.Platform.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<PlatformPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.Platform> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class PlatformPage
    {
        public SmartList<Generated.Models.Platform> Rows { get; }
        public long TotalCount { get; }
        public PlatformPage(
            SmartList<Generated.Models.Platform> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}