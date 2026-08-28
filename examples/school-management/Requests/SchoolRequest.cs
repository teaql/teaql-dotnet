using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TeaQL.Core;
using Generated.Models;

namespace Generated.Requests
{
    public class SchoolRequest
    {
        private SelectQuery _query;
        private string _purpose;
        private string _comment;

        public SchoolRequest()
        {
            _query = new SelectQuery("School");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public SchoolRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public SchoolRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public SchoolRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableSchoolRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableSchoolRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
                c => Comment(c),
                EnsureIntent);
        }

        public SchoolRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public SchoolRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public SchoolRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public SchoolRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public SchoolRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("platform");
            _query.Project("school_type");
            _query.Project("name");
            _query.Project("address");
            _query.Project("established_date");
            _query.Project("student_capacity");
            _query.Project("active");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public SchoolRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }



                public SchoolRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public SchoolRequest SelectAddress()
                {
                    _query.Project("address");
                    return this;
                }

                public SchoolRequest SelectEstablishedDate()
                {
                    _query.Project("established_date");
                    return this;
                }

                public SchoolRequest SelectStudentCapacity()
                {
                    _query.Project("student_capacity");
                    return this;
                }

                public SchoolRequest SelectActive()
                {
                    _query.Project("active");
                    return this;
                }

                public SchoolRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public SchoolRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public SchoolRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public SchoolRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public SchoolRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project("platform");
                    _query.ForwardRelationQuery("Platform", "Platform", "platform", related.GetQuery());
                    return this;
                }
                public SchoolRequest SelectSchoolType()
                {
                    return SelectSchoolTypeWith(new SchoolTypeRequest());
                }

                public SchoolRequest SelectSchoolTypeWith(SchoolTypeRequest related)
                {
                    _query.Project("school_type");
                    _query.ForwardRelationQuery("SchoolType", "SchoolType", "school_type", related.GetQuery());
                    return this;
                }

                public SchoolRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public SchoolRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public SchoolRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public SchoolRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public SchoolRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public SchoolRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public SchoolRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public SchoolRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public SchoolRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
                    return this;
                }

                public SchoolRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public SchoolRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public SchoolRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("platform", val));
                    return this;
                }

                public SchoolRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("platform", vals));
                    return this;
                }

                public SchoolRequest FilterBySchoolType(object val)
                {
                    _query.AndFilter(Expr.Eq("school_type", val));
                    return this;
                }

                public SchoolRequest FilterBySchoolTypeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("school_type", vals));
                    return this;
                }
                public SchoolRequest WithSchoolTypeIsPrimary()
                {
                    _query.AndFilter(Expr.Eq("school_type", 1001L));
                    return this;
                }


                public SchoolRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public SchoolRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }
                public SchoolRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", val));
                    return this;
                }

                public SchoolRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public SchoolRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals));
                    return this;
                }

                public SchoolRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", val));
                    return this;
                }

                public SchoolRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", val));
                    return this;
                }

                public SchoolRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", val));
                    return this;
                }

                public SchoolRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", val));
                    return this;
                }

                public SchoolRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", lower, upper));
                    return this;
                }

                public SchoolRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public SchoolRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public SchoolRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public SchoolRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public SchoolRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public SchoolRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public SchoolRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public SchoolRequest WithAddressContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("address", val));
                    return this;
                }

                public SchoolRequest WithAddressIs(string val)
                {
                    _query.AndFilter(Expr.Eq("address", val));
                    return this;
                }
                public SchoolRequest WithAddressIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("address", val));
                    return this;
                }

                public SchoolRequest WithAddressIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("address", vals));
                    return this;
                }

                public SchoolRequest WithAddressNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("address", vals));
                    return this;
                }

                public SchoolRequest WithAddressGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("address", val));
                    return this;
                }

                public SchoolRequest WithAddressGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("address", val));
                    return this;
                }

                public SchoolRequest WithAddressLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("address", val));
                    return this;
                }

                public SchoolRequest WithAddressLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("address", val));
                    return this;
                }

                public SchoolRequest WithAddressBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("address", lower, upper));
                    return this;
                }

                public SchoolRequest WithAddressIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("address"));
                    return this;
                }

                public SchoolRequest WithAddressIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("address"));
                    return this;
                }
                public SchoolRequest WithAddressNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("address", val));
                    return this;
                }

                public SchoolRequest WithAddressStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("address", val));
                    return this;
                }

                public SchoolRequest WithAddressNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("address", val));
                    return this;
                }

                public SchoolRequest WithAddressEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("address", val));
                    return this;
                }

                public SchoolRequest WithAddressNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("address", val));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIs(object val)
                {
                    _query.AndFilter(Expr.Eq("established_date", val));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("established_date", val));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("established_date", vals));
                    return this;
                }

                public SchoolRequest WithEstablishedDateNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("established_date", vals));
                    return this;
                }

                public SchoolRequest WithEstablishedDateGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("established_date", val));
                    return this;
                }

                public SchoolRequest WithEstablishedDateGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("established_date", val));
                    return this;
                }

                public SchoolRequest WithEstablishedDateLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("established_date", val));
                    return this;
                }

                public SchoolRequest WithEstablishedDateLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("established_date", val));
                    return this;
                }

                public SchoolRequest WithEstablishedDateBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("established_date", lower, upper));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("established_date"));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("established_date"));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIs(object val)
                {
                    _query.AndFilter(Expr.Eq("student_capacity", val));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("student_capacity", val));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("student_capacity", vals));
                    return this;
                }

                public SchoolRequest WithStudentCapacityNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("student_capacity", vals));
                    return this;
                }

                public SchoolRequest WithStudentCapacityGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("student_capacity", val));
                    return this;
                }

                public SchoolRequest WithStudentCapacityGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("student_capacity", val));
                    return this;
                }

                public SchoolRequest WithStudentCapacityLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("student_capacity", val));
                    return this;
                }

                public SchoolRequest WithStudentCapacityLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("student_capacity", val));
                    return this;
                }

                public SchoolRequest WithStudentCapacityBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("student_capacity", lower, upper));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("student_capacity"));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("student_capacity"));
                    return this;
                }

                public SchoolRequest WhichAreActive()
                {
                    _query.AndFilter(Expr.Eq("active", true));
                    return this;
                }

                public SchoolRequest WhichAreNotActive()
                {
                    _query.AndFilter(Expr.Eq("active", false));
                    return this;
                }
                public SchoolRequest WithActiveIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("active", val));
                    return this;
                }

                public SchoolRequest WithActiveIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("active", vals));
                    return this;
                }

                public SchoolRequest WithActiveNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("active", vals));
                    return this;
                }

                public SchoolRequest WithActiveGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("active", val));
                    return this;
                }

                public SchoolRequest WithActiveGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("active", val));
                    return this;
                }

                public SchoolRequest WithActiveLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("active", val));
                    return this;
                }

                public SchoolRequest WithActiveLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("active", val));
                    return this;
                }

                public SchoolRequest WithActiveBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("active", lower, upper));
                    return this;
                }

                public SchoolRequest WithActiveIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("active"));
                    return this;
                }

                public SchoolRequest WithActiveIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("active"));
                    return this;
                }

                public SchoolRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public SchoolRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", val));
                    return this;
                }

                public SchoolRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public SchoolRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals));
                    return this;
                }

                public SchoolRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", val));
                    return this;
                }

                public SchoolRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public SchoolRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", val));
                    return this;
                }

                public SchoolRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public SchoolRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", lower, upper));
                    return this;
                }

                public SchoolRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public SchoolRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("update_time", val));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public SchoolRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals));
                    return this;
                }

                public SchoolRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("update_time", val));
                    return this;
                }

                public SchoolRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public SchoolRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("update_time", val));
                    return this;
                }

                public SchoolRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public SchoolRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("update_time", lower, upper));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public SchoolRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public SchoolRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public SchoolRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public SchoolRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public SchoolRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public SchoolRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public SchoolRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public SchoolRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public SchoolRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
                    return this;
                }

                public SchoolRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public SchoolRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public SchoolRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public SchoolRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }



                public SchoolRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public SchoolRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public SchoolRequest OrderByAddressAscending()
                {
                    _query.OrderBy("address", "asc");
                    return this;
                }

                public SchoolRequest OrderByAddressDescending()
                {
                    _query.OrderBy("address", "desc");
                    return this;
                }

                public SchoolRequest OrderByEstablishedDateAscending()
                {
                    _query.OrderBy("established_date", "asc");
                    return this;
                }

                public SchoolRequest OrderByEstablishedDateDescending()
                {
                    _query.OrderBy("established_date", "desc");
                    return this;
                }

                public SchoolRequest OrderByStudentCapacityAscending()
                {
                    _query.OrderBy("student_capacity", "asc");
                    return this;
                }

                public SchoolRequest OrderByStudentCapacityDescending()
                {
                    _query.OrderBy("student_capacity", "desc");
                    return this;
                }

                public SchoolRequest OrderByActiveAscending()
                {
                    _query.OrderBy("active", "asc");
                    return this;
                }

                public SchoolRequest OrderByActiveDescending()
                {
                    _query.OrderBy("active", "desc");
                    return this;
                }

                public SchoolRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public SchoolRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public SchoolRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public SchoolRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public SchoolRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public SchoolRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public SchoolRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public SchoolRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public SchoolRequest minStudentCapacity()
                {
                    return minStudentCapacityAs("minOfStudentCapacity");
                }

                public SchoolRequest minStudentCapacityAs(string retName)
                {
                    _query.Aggregate("min", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest maxStudentCapacity()
                {
                    return maxStudentCapacityAs("maxOfStudentCapacity");
                }

                public SchoolRequest maxStudentCapacityAs(string retName)
                {
                    _query.Aggregate("max", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest sumStudentCapacity()
                {
                    return sumStudentCapacityAs("sumOfStudentCapacity");
                }

                public SchoolRequest sumStudentCapacityAs(string retName)
                {
                    _query.Aggregate("sum", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest avgStudentCapacity()
                {
                    return avgStudentCapacityAs("avgOfStudentCapacity");
                }

                public SchoolRequest avgStudentCapacityAs(string retName)
                {
                    _query.Aggregate("avg", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest standardDeviationStudentCapacity()
                {
                    return standardDeviationStudentCapacityAs("standardDeviationOfStudentCapacity");
                }

                public SchoolRequest standardDeviationStudentCapacityAs(string retName)
                {
                    _query.Aggregate("stddev", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest squareRootOfPopulationStandardDeviationStudentCapacity()
                {
                    return squareRootOfPopulationStandardDeviationStudentCapacityAs("squareRootOfPopulationStandardDeviationOfStudentCapacity");
                }

                public SchoolRequest squareRootOfPopulationStandardDeviationStudentCapacityAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest sampleVarianceStudentCapacity()
                {
                    return sampleVarianceStudentCapacityAs("sampleVarianceOfStudentCapacity");
                }

                public SchoolRequest sampleVarianceStudentCapacityAs(string retName)
                {
                    _query.Aggregate("var_samp", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest samplePopulationVarianceStudentCapacity()
                {
                    return samplePopulationVarianceStudentCapacityAs("samplePopulationVarianceOfStudentCapacity");
                }

                public SchoolRequest samplePopulationVarianceStudentCapacityAs(string retName)
                {
                    _query.Aggregate("var_pop", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public SchoolRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public SchoolRequest GroupByPlatform()
                {
                    _query.GroupBy("platform");
                    return this;
                }

                public SchoolRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy("platform"); 
                    return this;
                }
                public SchoolRequest GroupBySchoolType()
                {
                    _query.GroupBy("school_type");
                    return this;
                }

                public SchoolRequest GroupBySchoolTypeAs(string retName)
                {
                    _query.GroupBy("school_type"); 
                    return this;
                }
                public SchoolRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public SchoolRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public SchoolRequest GroupByAddress()
                {
                    _query.GroupBy("address");
                    return this;
                }

                public SchoolRequest GroupByAddressAs(string retName)
                {
                    _query.GroupBy("address"); 
                    return this;
                }
                public SchoolRequest GroupByEstablishedDate()
                {
                    _query.GroupBy("established_date");
                    return this;
                }

                public SchoolRequest GroupByEstablishedDateAs(string retName)
                {
                    _query.GroupBy("established_date"); 
                    return this;
                }
                public SchoolRequest GroupByStudentCapacity()
                {
                    _query.GroupBy("student_capacity");
                    return this;
                }

                public SchoolRequest GroupByStudentCapacityAs(string retName)
                {
                    _query.GroupBy("student_capacity"); 
                    return this;
                }
                public SchoolRequest GroupByActive()
                {
                    _query.GroupBy("active");
                    return this;
                }

                public SchoolRequest GroupByActiveAs(string retName)
                {
                    _query.GroupBy("active"); 
                    return this;
                }
                public SchoolRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public SchoolRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public SchoolRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public SchoolRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
                    return this;
                }
                public SchoolRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public SchoolRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }

        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireDataService();
            var req = new QueryRequest(_query);
            return await service.QueryAsync(context, req);
        }

        private async Task<SchoolPage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireDataService();
            var countQuery = new SelectQuery("School");
            foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
            countQuery.Aggregate("Count", "id", "count");
            var countResult = await service.QueryAsync(context, new QueryRequest(countQuery));
            var totalCount = countResult.Rows.Count == 0
                ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);

            _query.Offset(offset);
            _query.Limit(limit);
            var result = await service.QueryAsync(context, new QueryRequest(_query));
            var rows = new SmartList<Generated.Models.School>();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.School.FromRecord(row));
            return new SchoolPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.School> ExecuteForStreamInternalAsync(
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
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.School.FromRecord(row);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableSchoolRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<SchoolPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.School>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableSchoolRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<SchoolPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.School>> executeForStream,
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

        public ExecutableSchoolRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.School NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.School();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.School>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.School>();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.School.FromRecord(row));
            return entities;
        }

        public Task<SchoolPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.School> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.School> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class SchoolPage
    {
        public SmartList<Generated.Models.School> Rows { get; }
        public long TotalCount { get; }
        public SchoolPage(
            SmartList<Generated.Models.School> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}