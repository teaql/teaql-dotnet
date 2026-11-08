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
    public class SchoolRequest
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

        public SchoolRequest()
        {
            _query = new SelectQuery("School");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public SchoolRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

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
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableSchoolRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
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

        public SchoolRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public SchoolRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public SchoolRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
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
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: "Platform")
                    };
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
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectSchoolTypeWith", Relation: "SchoolType")
                    };
                    return this;
                }
                public SchoolRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public SchoolRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }
                public SchoolRequest WithSchoolTypeMatching(SchoolTypeRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("school_type", GeneratedRuntimeModule.Module.Metadata.GetEntity("SchoolType")!, related.GetQuery(), "id"));
                    return this;
                }

                public SchoolRequest WithoutSchoolTypeMatching(SchoolTypeRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("school_type", GeneratedRuntimeModule.Module.Metadata.GetEntity("SchoolType")!, related.GetQuery(), "id"));
                    return this;
                }

                public SchoolRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public SchoolRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("platform"));
                    return this;
                }

                public SchoolRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("platform"));
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

                public SchoolRequest WithSchoolTypeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("school_type"));
                    return this;
                }

                public SchoolRequest WithSchoolTypeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("school_type"));
                    return this;
                }
                public SchoolRequest WithSchoolTypeIsPrimary()
                {
                    _query.AndFilter(Expr.Eq("school_type", 1001L));
                    return this;
                }

                public SchoolRequest WithSchoolTypeIsSecondary()
                {
                    _query.AndFilter(Expr.Eq("school_type", 1002L));
                    return this;
                }


                public SchoolRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public SchoolRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public SchoolRequest WithNameIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithNameNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithNameGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public SchoolRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public SchoolRequest WithAddressContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("address", val));
                    return this;
                }

                public SchoolRequest WithAddressIs(string val)
                {
                    _query.AndFilter(Expr.Eq("address", TeaqlQueryValue(val)));
                    return this;
                }
                public SchoolRequest WithAddressIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("address", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("address", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithAddressNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("address", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithAddressGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("address", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("address", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("address", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("address", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("address", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public SchoolRequest WithAddressSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("address", val));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIs(DateTime val)
                {
                    _query.AndFilter(Expr.Eq("established_date", new Value.DateValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIsNot(DateTime val)
                {
                    _query.AndFilter(Expr.Ne("established_date", new Value.DateValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.In("established_date", vals.Select(val => new Value.DateValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithEstablishedDateNotIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.NotIn("established_date", vals.Select(val => new Value.DateValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithEstablishedDateGreaterThan(DateTime val)
                {
                    _query.AndFilter(Expr.Gt("established_date", new Value.DateValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateGreaterThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Gte("established_date", new Value.DateValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateLessThan(DateTime val)
                {
                    _query.AndFilter(Expr.Lt("established_date", new Value.DateValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateLessThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Lte("established_date", new Value.DateValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateBetween(DateTime lower, DateTime upper)
                {
                    _query.AndFilter(Expr.Between("established_date", new Value.DateValue(lower), new Value.DateValue(upper)));
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

                public SchoolRequest WithStudentCapacityIs(long val)
                {
                    _query.AndFilter(Expr.Eq("student_capacity", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("student_capacity", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("student_capacity", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithStudentCapacityNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("student_capacity", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithStudentCapacityGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("student_capacity", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("student_capacity", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("student_capacity", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("student_capacity", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("student_capacity", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
                public SchoolRequest WithActiveIsNot(bool val)
                {
                    _query.AndFilter(Expr.Ne("active", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveIn(params bool[] vals)
                {
                    _query.AndFilter(Expr.In("active", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithActiveNotIn(params bool[] vals)
                {
                    _query.AndFilter(Expr.NotIn("active", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithActiveGreaterThan(bool val)
                {
                    _query.AndFilter(Expr.Gt("active", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveGreaterThanOrEqualTo(bool val)
                {
                    _query.AndFilter(Expr.Gte("active", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveLessThan(bool val)
                {
                    _query.AndFilter(Expr.Lt("active", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveLessThanOrEqualTo(bool val)
                {
                    _query.AndFilter(Expr.Lte("active", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveBetween(bool lower, bool upper)
                {
                    _query.AndFilter(Expr.Between("active", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public SchoolRequest WithCreateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithCreateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithCreateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("create_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public SchoolRequest WithUpdateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithUpdateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithUpdateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("update_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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

                public SchoolRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public SchoolRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
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
            return CountAs("count");
        }

        public SchoolRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public SchoolRequest MinStudentCapacity()
                {
                    return MinStudentCapacityAs("minOfStudentCapacity");
                }

                public SchoolRequest MinStudentCapacityAs(string retName)
                {
                    _query.Aggregate("min", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest MaxStudentCapacity()
                {
                    return MaxStudentCapacityAs("maxOfStudentCapacity");
                }

                public SchoolRequest MaxStudentCapacityAs(string retName)
                {
                    _query.Aggregate("max", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest SumStudentCapacity()
                {
                    return SumStudentCapacityAs("sumOfStudentCapacity");
                }

                public SchoolRequest SumStudentCapacityAs(string retName)
                {
                    _query.Aggregate("sum", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest AvgStudentCapacity()
                {
                    return AvgStudentCapacityAs("avgOfStudentCapacity");
                }

                public SchoolRequest AvgStudentCapacityAs(string retName)
                {
                    _query.Aggregate("avg", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest StandardDeviationStudentCapacity()
                {
                    return StandardDeviationStudentCapacityAs("standardDeviationOfStudentCapacity");
                }

                public SchoolRequest StandardDeviationStudentCapacityAs(string retName)
                {
                    _query.Aggregate("stddev", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest SquareRootOfPopulationStandardDeviationStudentCapacity()
                {
                    return SquareRootOfPopulationStandardDeviationStudentCapacityAs("squareRootOfPopulationStandardDeviationOfStudentCapacity");
                }

                public SchoolRequest SquareRootOfPopulationStandardDeviationStudentCapacityAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest SampleVarianceStudentCapacity()
                {
                    return SampleVarianceStudentCapacityAs("sampleVarianceOfStudentCapacity");
                }

                public SchoolRequest SampleVarianceStudentCapacityAs(string retName)
                {
                    _query.Aggregate("var_samp", "student_capacity", retName);
                    return this;
                }
                public SchoolRequest SamplePopulationVarianceStudentCapacity()
                {
                    return SamplePopulationVarianceStudentCapacityAs("samplePopulationVarianceOfStudentCapacity");
                }

                public SchoolRequest SamplePopulationVarianceStudentCapacityAs(string retName)
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
                    _query.ProjectExpr(retName, Expr.Column("id"));
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
                    _query.ProjectExpr(retName, Expr.Column("platform"));
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
                    _query.ProjectExpr(retName, Expr.Column("school_type"));
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
                    _query.ProjectExpr(retName, Expr.Column("name"));
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
                    _query.ProjectExpr(retName, Expr.Column("address"));
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
                    _query.ProjectExpr(retName, Expr.Column("established_date"));
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
                    _query.ProjectExpr(retName, Expr.Column("student_capacity"));
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
                    _query.ProjectExpr(retName, Expr.Column("active"));
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
                    _query.ProjectExpr(retName, Expr.Column("create_time"));
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
                    _query.ProjectExpr(retName, Expr.Column("update_time"));
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
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public SchoolRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Platform", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, "platform")
                    });
                    return this;
                }

                public SchoolRequest FacetBySchoolTypeAs(
                    string name, SchoolTypeRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "SchoolType", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetBySchoolTypeAs", name, "school_type")
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

        private async Task<Generated.Models.School?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.School.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.School>> ExecuteForListWithMetadataInternalAsync(
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
            var entities = new SmartList<Generated.Models.School>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.School.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.School> ExecuteForStreamInternalAsync(
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
                    Generated.Models.School.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableSchoolRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.School?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.School>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.School>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableSchoolRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.School?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.School>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.School>> executeForStream,
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

        public ExecutableSchoolRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.School> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("School").ConfigureAwait(false));
            var entity = Generated.Models.School.TeaqlNewWithReservedId(identity);
            return context.InitializeEntity("School", entity,
                initialized => initialized.TeaqlValidateReservedIdentity(identity));
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.School>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.School> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.School?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}