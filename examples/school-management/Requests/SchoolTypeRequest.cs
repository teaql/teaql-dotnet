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
    public class SchoolTypeRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public SchoolTypeRequest()
        {
            _query = new SelectQuery("SchoolType");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public SchoolTypeRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public SchoolTypeRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public SchoolTypeRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableSchoolTypeRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableSchoolTypeRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
                c => Comment(c),
                EnsureIntent);
        }

        public SchoolTypeRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public SchoolTypeRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public SchoolTypeRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public SchoolTypeRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public SchoolTypeRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public SchoolTypeRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public SchoolTypeRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public SchoolTypeRequest SelectSelfFields()
        {
            _query.Project("platform");
            _query.Project("id");
            _query.Project("name");
            _query.Project("code");
            _query.Project("display_order");
            _query.Project("version");
            return this;
        }

                public SchoolTypeRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public SchoolTypeRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public SchoolTypeRequest SelectCode()
                {
                    _query.Project("code");
                    return this;
                }

                public SchoolTypeRequest SelectDisplayOrder()
                {
                    _query.Project("display_order");
                    return this;
                }

                public SchoolTypeRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public SchoolTypeRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public SchoolTypeRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project("platform");
                    _query.ForwardRelationQuery("Platform", "Platform", "platform", related.GetQuery());
                    return this;
                }
                public SchoolTypeRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("platform", EntityDescriptor.New("Platform"), related.GetQuery(), "id"));
                    return this;
                }

                public SchoolTypeRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("platform", EntityDescriptor.New("Platform"), related.GetQuery(), "id"));
                    return this;
                }

                public SchoolTypeRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("platform", val));
                    return this;
                }

                public SchoolTypeRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("platform", vals));
                    return this;
                }

                public SchoolTypeRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("platform"));
                    return this;
                }

                public SchoolTypeRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("platform"));
                    return this;
                }

                public SchoolTypeRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public SchoolTypeRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public SchoolTypeRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public SchoolTypeRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public SchoolTypeRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public SchoolTypeRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public SchoolTypeRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public SchoolTypeRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public SchoolTypeRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
                    return this;
                }

                public SchoolTypeRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public SchoolTypeRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public SchoolTypeRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }
                public SchoolTypeRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public SchoolTypeRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals));
                    return this;
                }

                public SchoolTypeRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", lower, upper));
                    return this;
                }

                public SchoolTypeRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public SchoolTypeRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public SchoolTypeRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public SchoolTypeRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeIs(string val)
                {
                    _query.AndFilter(Expr.Eq("code", val));
                    return this;
                }
                public SchoolTypeRequest WithCodeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("code", vals));
                    return this;
                }

                public SchoolTypeRequest WithCodeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("code", vals));
                    return this;
                }

                public SchoolTypeRequest WithCodeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("code", lower, upper));
                    return this;
                }

                public SchoolTypeRequest WithCodeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("code"));
                    return this;
                }

                public SchoolTypeRequest WithCodeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("code"));
                    return this;
                }
                public SchoolTypeRequest WithCodeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("code", val));
                    return this;
                }

                public SchoolTypeRequest WithCodeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("code", val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIs(object val)
                {
                    _query.AndFilter(Expr.Eq("display_order", val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("display_order", val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("display_order", vals));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("display_order", vals));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("display_order", val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("display_order", val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("display_order", val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("display_order", val));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("display_order", lower, upper));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("display_order"));
                    return this;
                }

                public SchoolTypeRequest WithDisplayOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("display_order"));
                    return this;
                }

                public SchoolTypeRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public SchoolTypeRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public SchoolTypeRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public SchoolTypeRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public SchoolTypeRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public SchoolTypeRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public SchoolTypeRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public SchoolTypeRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public SchoolTypeRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
                    return this;
                }

                public SchoolTypeRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public SchoolTypeRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public SchoolTypeRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByCodeAscending()
                {
                    _query.OrderBy("code", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByCodeDescending()
                {
                    _query.OrderBy("code", "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByDisplayOrderAscending()
                {
                    _query.OrderBy("display_order", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByDisplayOrderDescending()
                {
                    _query.OrderBy("display_order", "desc");
                    return this;
                }

                public SchoolTypeRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public SchoolTypeRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public SchoolTypeRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public SchoolTypeRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public SchoolTypeRequest MinDisplayOrder()
                {
                    return MinDisplayOrderAs("minOfDisplayOrder");
                }

                public SchoolTypeRequest MinDisplayOrderAs(string retName)
                {
                    _query.Aggregate("min", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest MaxDisplayOrder()
                {
                    return MaxDisplayOrderAs("maxOfDisplayOrder");
                }

                public SchoolTypeRequest MaxDisplayOrderAs(string retName)
                {
                    _query.Aggregate("max", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest SumDisplayOrder()
                {
                    return SumDisplayOrderAs("sumOfDisplayOrder");
                }

                public SchoolTypeRequest SumDisplayOrderAs(string retName)
                {
                    _query.Aggregate("sum", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest AvgDisplayOrder()
                {
                    return AvgDisplayOrderAs("avgOfDisplayOrder");
                }

                public SchoolTypeRequest AvgDisplayOrderAs(string retName)
                {
                    _query.Aggregate("avg", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest StandardDeviationDisplayOrder()
                {
                    return StandardDeviationDisplayOrderAs("standardDeviationOfDisplayOrder");
                }

                public SchoolTypeRequest StandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest SquareRootOfPopulationStandardDeviationDisplayOrder()
                {
                    return SquareRootOfPopulationStandardDeviationDisplayOrderAs("squareRootOfPopulationStandardDeviationOfDisplayOrder");
                }

                public SchoolTypeRequest SquareRootOfPopulationStandardDeviationDisplayOrderAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest SampleVarianceDisplayOrder()
                {
                    return SampleVarianceDisplayOrderAs("sampleVarianceOfDisplayOrder");
                }

                public SchoolTypeRequest SampleVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_samp", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest SamplePopulationVarianceDisplayOrder()
                {
                    return SamplePopulationVarianceDisplayOrderAs("samplePopulationVarianceOfDisplayOrder");
                }

                public SchoolTypeRequest SamplePopulationVarianceDisplayOrderAs(string retName)
                {
                    _query.Aggregate("var_pop", "display_order", retName);
                    return this;
                }
                public SchoolTypeRequest GroupByPlatform()
                {
                    _query.GroupBy("platform");
                    return this;
                }

                public SchoolTypeRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy("platform"); 
                    return this;
                }
                public SchoolTypeRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public SchoolTypeRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public SchoolTypeRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public SchoolTypeRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public SchoolTypeRequest GroupByCode()
                {
                    _query.GroupBy("code");
                    return this;
                }

                public SchoolTypeRequest GroupByCodeAs(string retName)
                {
                    _query.GroupBy("code"); 
                    return this;
                }
                public SchoolTypeRequest GroupByDisplayOrder()
                {
                    _query.GroupBy("display_order");
                    return this;
                }

                public SchoolTypeRequest GroupByDisplayOrderAs(string retName)
                {
                    _query.GroupBy("display_order"); 
                    return this;
                }
                public SchoolTypeRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public SchoolTypeRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public SchoolTypeRequest SelectSchoolList()
                {
                    return SelectSchoolListWith(new SchoolRequest());
                }

                public SchoolTypeRequest SelectSchoolListWith(SchoolRequest child)
                {
                    _query.RelationQuery("SchoolList", "School", "school_type", true, child.GetQuery());
                    return this;
                }
                public SchoolTypeRequest HaveSchools()
                    => WithSchoolListMatching(new SchoolRequest());

                public SchoolTypeRequest HaveNoSchools()
                    => WithoutSchoolListMatching(new SchoolRequest());

                public SchoolTypeRequest WithSchoolListMatching(SchoolRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("School"), child.GetQuery(), "school_type"));
                    return this;
                }

                public SchoolTypeRequest WithoutSchoolListMatching(SchoolRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("School"), child.GetQuery(), "school_type"));
                    return this;
                }
                public SchoolTypeRequest CountSchools()
                    => CountSchoolsAs("countSchools");

                public SchoolTypeRequest CountSchoolsAs(string alias)
                    => CountSchoolsWith(alias, new SchoolRequest());

                public SchoolTypeRequest CountSchoolsWith(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, child.GetQuery(), true);
                    return this;
                }

                public SchoolTypeRequest MinStudentCapacityOfSchools()
                    => MinStudentCapacityOfSchoolsAs("minOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest MinStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("min", "student_capacity", "min_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, child.GetQuery(), true);
                    return this;
                }
                public SchoolTypeRequest MaxStudentCapacityOfSchools()
                    => MaxStudentCapacityOfSchoolsAs("maxOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest MaxStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("max", "student_capacity", "max_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, child.GetQuery(), true);
                    return this;
                }
                public SchoolTypeRequest SumStudentCapacityOfSchools()
                    => SumStudentCapacityOfSchoolsAs("sumOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest SumStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("sum", "student_capacity", "sum_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, child.GetQuery(), true);
                    return this;
                }
                public SchoolTypeRequest AvgStudentCapacityOfSchools()
                    => AvgStudentCapacityOfSchoolsAs("avgOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest AvgStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("avg", "student_capacity", "avg_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, child.GetQuery(), true);
                    return this;
                }
                public SchoolTypeRequest StandardDeviationStudentCapacityOfSchools()
                    => StandardDeviationStudentCapacityOfSchoolsAs("standardDeviationOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest StandardDeviationStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "student_capacity", "standardDeviation_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, child.GetQuery(), true);
                    return this;
                }
                public SchoolTypeRequest SquareRootOfPopulationStandardDeviationStudentCapacityOfSchools()
                    => SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs("squareRootOfPopulationStandardDeviationOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest SquareRootOfPopulationStandardDeviationStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "student_capacity", "squareRootOfPopulationStandardDeviation_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, child.GetQuery(), true);
                    return this;
                }
                public SchoolTypeRequest SampleVarianceStudentCapacityOfSchools()
                    => SampleVarianceStudentCapacityOfSchoolsAs("sampleVarianceOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest SampleVarianceStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "student_capacity", "sampleVariance_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, child.GetQuery(), true);
                    return this;
                }
                public SchoolTypeRequest SamplePopulationVarianceStudentCapacityOfSchools()
                    => SamplePopulationVarianceStudentCapacityOfSchoolsAs("samplePopulationVarianceOfStudentCapacityOfSchools", new SchoolRequest());

                public SchoolTypeRequest SamplePopulationVarianceStudentCapacityOfSchoolsAs(string alias, SchoolRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "student_capacity", "samplePopulationVariance_student_capacity");
                    _query.RelationAggregate("SchoolList", "School", "school_type", alias, child.GetQuery(), true);
                    return this;
                }
                public SchoolTypeRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "platform", request.GetQuery(), includeAllFacets));
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

        private async Task<SchoolTypePage> ExecuteForPageInternalAsync(
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
                var countQuery = new SelectQuery("SchoolType");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(context, new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.SchoolType>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.SchoolType.FromRecord(row, queryRoot));
            return new SchoolTypePage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.SchoolType> ExecuteForStreamInternalAsync(
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
                    yield return Generated.Models.SchoolType.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableSchoolTypeRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<SchoolTypePage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.SchoolType>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableSchoolTypeRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<SchoolTypePage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.SchoolType>> executeForStream,
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

        public ExecutableSchoolTypeRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.SchoolType NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.SchoolType();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.SchoolType>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.SchoolType>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.SchoolType.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<SchoolTypePage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.SchoolType> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.SchoolType> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class SchoolTypePage
    {
        public SmartList<Generated.Models.SchoolType> Rows { get; }
        public long TotalCount { get; }
        public SchoolTypePage(
            SmartList<Generated.Models.SchoolType> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}