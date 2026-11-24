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
                ? time.ToString(@"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture)
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
            _query.Project(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public SchoolRequest SelectId()
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }



                public SchoolRequest SelectName()
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest SelectAddress()
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest SelectEstablishedDate()
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest SelectStudentCapacity()
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest SelectActive()
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest SelectCreateTime()
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest SelectUpdateTime()
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public SchoolRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, "Platform", global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public SchoolRequest SelectSchoolType()
                {
                    return SelectSchoolTypeWith(new SchoolTypeRequest());
                }

                public SchoolRequest SelectSchoolTypeWith(SchoolTypeRequest related)
                {
                    _query.Project(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER, "SchoolType", global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectSchoolTypeWith", Relation: global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public SchoolRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public SchoolRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }
                public SchoolRequest WithSchoolTypeMatching(SchoolTypeRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("SchoolType")!, related.GetQuery(), "id"));
                    return this;
                }

                public SchoolRequest WithoutSchoolTypeMatching(SchoolTypeRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("SchoolType")!, related.GetQuery(), "id"));
                    return this;
                }

                public SchoolRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY, vals));
                    return this;
                }

                public SchoolRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest FilterBySchoolType(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest FilterBySchoolTypeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY, vals));
                    return this;
                }

                public SchoolRequest WithSchoolTypeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithSchoolTypeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY));
                    return this;
                }
                public SchoolRequest WithSchoolTypeIsPrimary()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY, 1001L));
                    return this;
                }

                public SchoolRequest WithSchoolTypeIsSecondary()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY, 1002L));
                    return this;
                }


                public SchoolRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public SchoolRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }
                public SchoolRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithAddressContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithAddressIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public SchoolRequest WithAddressIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithAddressNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithAddressGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithAddressBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolRequest WithAddressIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithAddressIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY));
                    return this;
                }
                public SchoolRequest WithAddressNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithAddressStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithAddressNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithAddressEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithAddressNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithAddressSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, val));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithEstablishedDateNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithEstablishedDateGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithEstablishedDateIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithStudentCapacityNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithStudentCapacityGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithStudentCapacityIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WhichAreActive()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, true));
                    return this;
                }

                public SchoolRequest WhichAreNotActive()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, false));
                    return this;
                }
                public SchoolRequest WithActiveIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithActiveNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithActiveGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithActiveBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolRequest WithActiveIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithActiveIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public SchoolRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public SchoolRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public SchoolRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public SchoolRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }



                public SchoolRequest OrderByNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolRequest OrderByNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolRequest OrderByAddressAscending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolRequest OrderByAddressDescending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolRequest OrderByEstablishedDateAscending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolRequest OrderByEstablishedDateDescending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolRequest OrderByStudentCapacityAscending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolRequest OrderByStudentCapacityDescending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolRequest OrderByActiveAscending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolRequest OrderByActiveDescending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public SchoolRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public SchoolRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
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
                    _query.Aggregate("min", global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolRequest MaxStudentCapacity()
                {
                    return MaxStudentCapacityAs("maxOfStudentCapacity");
                }

                public SchoolRequest MaxStudentCapacityAs(string retName)
                {
                    _query.Aggregate("max", global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolRequest SumStudentCapacity()
                {
                    return SumStudentCapacityAs("sumOfStudentCapacity");
                }

                public SchoolRequest SumStudentCapacityAs(string retName)
                {
                    _query.Aggregate("sum", global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolRequest AvgStudentCapacity()
                {
                    return AvgStudentCapacityAs("avgOfStudentCapacity");
                }

                public SchoolRequest AvgStudentCapacityAs(string retName)
                {
                    _query.Aggregate("avg", global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolRequest StandardDeviationStudentCapacity()
                {
                    return StandardDeviationStudentCapacityAs("standardDeviationOfStudentCapacity");
                }

                public SchoolRequest StandardDeviationStudentCapacityAs(string retName)
                {
                    _query.Aggregate("stddev", global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolRequest SquareRootOfPopulationStandardDeviationStudentCapacity()
                {
                    return SquareRootOfPopulationStandardDeviationStudentCapacityAs("squareRootOfPopulationStandardDeviationOfStudentCapacity");
                }

                public SchoolRequest SquareRootOfPopulationStandardDeviationStudentCapacityAs(string retName)
                {
                    _query.Aggregate("stddev_pop", global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolRequest SampleVarianceStudentCapacity()
                {
                    return SampleVarianceStudentCapacityAs("sampleVarianceOfStudentCapacity");
                }

                public SchoolRequest SampleVarianceStudentCapacityAs(string retName)
                {
                    _query.Aggregate("var_samp", global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolRequest SamplePopulationVarianceStudentCapacity()
                {
                    return SamplePopulationVarianceStudentCapacityAs("samplePopulationVarianceOfStudentCapacity");
                }

                public SchoolRequest SamplePopulationVarianceStudentCapacityAs(string retName)
                {
                    _query.Aggregate("var_pop", global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY, retName);
                    return this;
                }
                public SchoolRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupByPlatform()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupBySchoolType()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupBySchoolTypeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupByName()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupByAddress()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByAddressAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.ADDRESS_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupByEstablishedDate()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByEstablishedDateAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.ESTABLISHED_DATE_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupByStudentCapacity()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByStudentCapacityAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.STUDENT_CAPACITY_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupByActive()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByActiveAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.ACTIVE_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupByCreateTime()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupByUpdateTime()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public SchoolRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.School.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public SchoolRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, global::Generated.Models.School.__TeaqlFields.PLATFORM_PROPERTY_QUERY)
                    });
                    return this;
                }

                public SchoolRequest FacetBySchoolTypeAs(
                    string name, SchoolTypeRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetBySchoolTypeAs", name, global::Generated.Models.School.__TeaqlFields.SCHOOL_TYPE_PROPERTY_QUERY)
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
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.School>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.School>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            if (request.Query.AggregateItems.Count > 0 || request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Entity streams require entity rows; use a record or aggregation query");
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
            return context.InitializeEntity("School", Generated.Models.School.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
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