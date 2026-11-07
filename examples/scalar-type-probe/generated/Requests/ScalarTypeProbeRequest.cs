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
    public class ScalarTypeProbeRequest
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

        public ScalarTypeProbeRequest()
        {
            _query = new SelectQuery("ScalarTypeProbe");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public ScalarTypeProbeRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public ScalarTypeProbeRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public ScalarTypeProbeRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public ScalarTypeProbeRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableScalarTypeProbeRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableScalarTypeProbeRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public ScalarTypeProbeRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public ScalarTypeProbeRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public ScalarTypeProbeRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public ScalarTypeProbeRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public ScalarTypeProbeRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public ScalarTypeProbeRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public ScalarTypeProbeRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public ScalarTypeProbeRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("type_platform");
            _query.Project("required_string");
            _query.Project("optional_string");
            _query.Project("large_text");
            _query.Project("required_boolean");
            _query.Project("optional_boolean");
            _query.Project("required_integer");
            _query.Project("optional_integer");
            _query.Project("required_long");
            _query.Project("optional_long");
            _query.Project("required_decimal");
            _query.Project("optional_decimal");
            _query.Project("required_float");
            _query.Project("optional_float");
            _query.Project("required_double");
            _query.Project("optional_double");
            _query.Project("required_date");
            _query.Project("optional_date");
            _query.Project("required_time");
            _query.Project("optional_time");
            _query.Project("required_datetime");
            _query.Project("optional_datetime");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public ScalarTypeProbeRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public ScalarTypeProbeRequest SelectRequiredString()
                {
                    _query.Project("required_string");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalString()
                {
                    _query.Project("optional_string");
                    return this;
                }

                public ScalarTypeProbeRequest SelectLargeText()
                {
                    _query.Project("large_text");
                    return this;
                }

                public ScalarTypeProbeRequest SelectRequiredBoolean()
                {
                    _query.Project("required_boolean");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalBoolean()
                {
                    _query.Project("optional_boolean");
                    return this;
                }

                public ScalarTypeProbeRequest SelectRequiredInteger()
                {
                    _query.Project("required_integer");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalInteger()
                {
                    _query.Project("optional_integer");
                    return this;
                }

                public ScalarTypeProbeRequest SelectRequiredLong()
                {
                    _query.Project("required_long");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalLong()
                {
                    _query.Project("optional_long");
                    return this;
                }

                public ScalarTypeProbeRequest SelectRequiredDecimal()
                {
                    _query.Project("required_decimal");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalDecimal()
                {
                    _query.Project("optional_decimal");
                    return this;
                }

                public ScalarTypeProbeRequest SelectRequiredFloat()
                {
                    _query.Project("required_float");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalFloat()
                {
                    _query.Project("optional_float");
                    return this;
                }

                public ScalarTypeProbeRequest SelectRequiredDouble()
                {
                    _query.Project("required_double");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalDouble()
                {
                    _query.Project("optional_double");
                    return this;
                }

                public ScalarTypeProbeRequest SelectRequiredDate()
                {
                    _query.Project("required_date");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalDate()
                {
                    _query.Project("optional_date");
                    return this;
                }

                public ScalarTypeProbeRequest SelectRequiredTime()
                {
                    _query.Project("required_time");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalTime()
                {
                    _query.Project("optional_time");
                    return this;
                }

                public ScalarTypeProbeRequest SelectRequiredDatetime()
                {
                    _query.Project("required_datetime");
                    return this;
                }

                public ScalarTypeProbeRequest SelectOptionalDatetime()
                {
                    _query.Project("optional_datetime");
                    return this;
                }

                public ScalarTypeProbeRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public ScalarTypeProbeRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public ScalarTypeProbeRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public ScalarTypeProbeRequest SelectTypePlatform()
                {
                    return SelectTypePlatformWith(new TypePlatformRequest());
                }

                public ScalarTypeProbeRequest SelectTypePlatformWith(TypePlatformRequest related)
                {
                    _query.Project("type_platform");
                    _query.ForwardRelationQuery("TypePlatform", "TypePlatform", "type_platform", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTypePlatformWith", Relation: "TypePlatform")
                    };
                    return this;
                }
                public ScalarTypeProbeRequest WithTypePlatformMatching(TypePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("type_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("TypePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public ScalarTypeProbeRequest WithoutTypePlatformMatching(TypePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("type_platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("TypePlatform")!, related.GetQuery(), "id"));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public ScalarTypeProbeRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public ScalarTypeProbeRequest FilterByTypePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("type_platform", val));
                    return this;
                }

                public ScalarTypeProbeRequest FilterByTypePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("type_platform", vals));
                    return this;
                }

                public ScalarTypeProbeRequest WithTypePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("type_platform"));
                    return this;
                }

                public ScalarTypeProbeRequest WithTypePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("type_platform"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("required_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringIs(string val)
                {
                    _query.AndFilter(Expr.Eq("required_string", TeaqlQueryValue(val)));
                    return this;
                }
                public ScalarTypeProbeRequest WithRequiredStringIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("required_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("required_string", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_string", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("required_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("required_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("required_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("required_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("required_string", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_string"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_string"));
                    return this;
                }
                public ScalarTypeProbeRequest WithRequiredStringNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("required_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("required_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("required_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("required_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("required_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredStringSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("required_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("optional_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringIs(string val)
                {
                    _query.AndFilter(Expr.Eq("optional_string", TeaqlQueryValue(val)));
                    return this;
                }
                public ScalarTypeProbeRequest WithOptionalStringIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("optional_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("optional_string", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_string", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("optional_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("optional_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("optional_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("optional_string", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("optional_string", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_string"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_string"));
                    return this;
                }
                public ScalarTypeProbeRequest WithOptionalStringNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("optional_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("optional_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("optional_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("optional_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("optional_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalStringSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("optional_string", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("large_text", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextIs(string val)
                {
                    _query.AndFilter(Expr.Eq("large_text", TeaqlQueryValue(val)));
                    return this;
                }
                public ScalarTypeProbeRequest WithLargeTextIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("large_text", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("large_text", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("large_text", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("large_text", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("large_text", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("large_text", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("large_text", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("large_text", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("large_text"));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("large_text"));
                    return this;
                }
                public ScalarTypeProbeRequest WithLargeTextNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("large_text", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("large_text", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("large_text", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("large_text", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("large_text", val));
                    return this;
                }

                public ScalarTypeProbeRequest WithLargeTextSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("large_text", val));
                    return this;
                }

                public ScalarTypeProbeRequest WhichAreRequiredBoolean()
                {
                    _query.AndFilter(Expr.Eq("required_boolean", true));
                    return this;
                }

                public ScalarTypeProbeRequest WhichAreNotRequiredBoolean()
                {
                    _query.AndFilter(Expr.Eq("required_boolean", false));
                    return this;
                }
                public ScalarTypeProbeRequest WithRequiredBooleanIsNot(bool val)
                {
                    _query.AndFilter(Expr.Ne("required_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredBooleanIn(params bool[] vals)
                {
                    _query.AndFilter(Expr.In("required_boolean", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredBooleanNotIn(params bool[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_boolean", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredBooleanGreaterThan(bool val)
                {
                    _query.AndFilter(Expr.Gt("required_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredBooleanGreaterThanOrEqualTo(bool val)
                {
                    _query.AndFilter(Expr.Gte("required_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredBooleanLessThan(bool val)
                {
                    _query.AndFilter(Expr.Lt("required_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredBooleanLessThanOrEqualTo(bool val)
                {
                    _query.AndFilter(Expr.Lte("required_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredBooleanBetween(bool lower, bool upper)
                {
                    _query.AndFilter(Expr.Between("required_boolean", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredBooleanIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_boolean"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredBooleanIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_boolean"));
                    return this;
                }

                public ScalarTypeProbeRequest WhichAreOptionalBoolean()
                {
                    _query.AndFilter(Expr.Eq("optional_boolean", true));
                    return this;
                }

                public ScalarTypeProbeRequest WhichAreNotOptionalBoolean()
                {
                    _query.AndFilter(Expr.Eq("optional_boolean", false));
                    return this;
                }
                public ScalarTypeProbeRequest WithOptionalBooleanIsNot(bool val)
                {
                    _query.AndFilter(Expr.Ne("optional_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalBooleanIn(params bool[] vals)
                {
                    _query.AndFilter(Expr.In("optional_boolean", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalBooleanNotIn(params bool[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_boolean", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalBooleanGreaterThan(bool val)
                {
                    _query.AndFilter(Expr.Gt("optional_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalBooleanGreaterThanOrEqualTo(bool val)
                {
                    _query.AndFilter(Expr.Gte("optional_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalBooleanLessThan(bool val)
                {
                    _query.AndFilter(Expr.Lt("optional_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalBooleanLessThanOrEqualTo(bool val)
                {
                    _query.AndFilter(Expr.Lte("optional_boolean", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalBooleanBetween(bool lower, bool upper)
                {
                    _query.AndFilter(Expr.Between("optional_boolean", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalBooleanIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_boolean"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalBooleanIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_boolean"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerIs(long val)
                {
                    _query.AndFilter(Expr.Eq("required_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("required_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("required_integer", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_integer", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("required_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("required_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("required_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("required_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("required_integer", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_integer"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredIntegerIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_integer"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerIs(long val)
                {
                    _query.AndFilter(Expr.Eq("optional_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("optional_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("optional_integer", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_integer", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("optional_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("optional_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("optional_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("optional_integer", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("optional_integer", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_integer"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalIntegerIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_integer"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongIs(long val)
                {
                    _query.AndFilter(Expr.Eq("required_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("required_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("required_long", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_long", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("required_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("required_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("required_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("required_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("required_long", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_long"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredLongIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_long"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongIs(long val)
                {
                    _query.AndFilter(Expr.Eq("optional_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("optional_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("optional_long", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_long", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("optional_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("optional_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("optional_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("optional_long", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("optional_long", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_long"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalLongIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_long"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalIs(decimal val)
                {
                    _query.AndFilter(Expr.Eq("required_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalIsNot(decimal val)
                {
                    _query.AndFilter(Expr.Ne("required_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.In("required_decimal", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalNotIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_decimal", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalGreaterThan(decimal val)
                {
                    _query.AndFilter(Expr.Gt("required_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalGreaterThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Gte("required_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalLessThan(decimal val)
                {
                    _query.AndFilter(Expr.Lt("required_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalLessThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Lte("required_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalBetween(decimal lower, decimal upper)
                {
                    _query.AndFilter(Expr.Between("required_decimal", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_decimal"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDecimalIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_decimal"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalIs(decimal val)
                {
                    _query.AndFilter(Expr.Eq("optional_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalIsNot(decimal val)
                {
                    _query.AndFilter(Expr.Ne("optional_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.In("optional_decimal", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalNotIn(params decimal[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_decimal", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalGreaterThan(decimal val)
                {
                    _query.AndFilter(Expr.Gt("optional_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalGreaterThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Gte("optional_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalLessThan(decimal val)
                {
                    _query.AndFilter(Expr.Lt("optional_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalLessThanOrEqualTo(decimal val)
                {
                    _query.AndFilter(Expr.Lte("optional_decimal", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalBetween(decimal lower, decimal upper)
                {
                    _query.AndFilter(Expr.Between("optional_decimal", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_decimal"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDecimalIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_decimal"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatIs(double val)
                {
                    _query.AndFilter(Expr.Eq("required_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatIsNot(double val)
                {
                    _query.AndFilter(Expr.Ne("required_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatIn(params double[] vals)
                {
                    _query.AndFilter(Expr.In("required_float", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatNotIn(params double[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_float", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatGreaterThan(double val)
                {
                    _query.AndFilter(Expr.Gt("required_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatGreaterThanOrEqualTo(double val)
                {
                    _query.AndFilter(Expr.Gte("required_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatLessThan(double val)
                {
                    _query.AndFilter(Expr.Lt("required_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatLessThanOrEqualTo(double val)
                {
                    _query.AndFilter(Expr.Lte("required_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatBetween(double lower, double upper)
                {
                    _query.AndFilter(Expr.Between("required_float", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_float"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredFloatIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_float"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatIs(double val)
                {
                    _query.AndFilter(Expr.Eq("optional_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatIsNot(double val)
                {
                    _query.AndFilter(Expr.Ne("optional_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatIn(params double[] vals)
                {
                    _query.AndFilter(Expr.In("optional_float", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatNotIn(params double[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_float", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatGreaterThan(double val)
                {
                    _query.AndFilter(Expr.Gt("optional_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatGreaterThanOrEqualTo(double val)
                {
                    _query.AndFilter(Expr.Gte("optional_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatLessThan(double val)
                {
                    _query.AndFilter(Expr.Lt("optional_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatLessThanOrEqualTo(double val)
                {
                    _query.AndFilter(Expr.Lte("optional_float", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatBetween(double lower, double upper)
                {
                    _query.AndFilter(Expr.Between("optional_float", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_float"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalFloatIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_float"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleIs(double val)
                {
                    _query.AndFilter(Expr.Eq("required_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleIsNot(double val)
                {
                    _query.AndFilter(Expr.Ne("required_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleIn(params double[] vals)
                {
                    _query.AndFilter(Expr.In("required_double", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleNotIn(params double[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_double", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleGreaterThan(double val)
                {
                    _query.AndFilter(Expr.Gt("required_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleGreaterThanOrEqualTo(double val)
                {
                    _query.AndFilter(Expr.Gte("required_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleLessThan(double val)
                {
                    _query.AndFilter(Expr.Lt("required_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleLessThanOrEqualTo(double val)
                {
                    _query.AndFilter(Expr.Lte("required_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleBetween(double lower, double upper)
                {
                    _query.AndFilter(Expr.Between("required_double", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_double"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDoubleIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_double"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleIs(double val)
                {
                    _query.AndFilter(Expr.Eq("optional_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleIsNot(double val)
                {
                    _query.AndFilter(Expr.Ne("optional_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleIn(params double[] vals)
                {
                    _query.AndFilter(Expr.In("optional_double", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleNotIn(params double[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_double", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleGreaterThan(double val)
                {
                    _query.AndFilter(Expr.Gt("optional_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleGreaterThanOrEqualTo(double val)
                {
                    _query.AndFilter(Expr.Gte("optional_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleLessThan(double val)
                {
                    _query.AndFilter(Expr.Lt("optional_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleLessThanOrEqualTo(double val)
                {
                    _query.AndFilter(Expr.Lte("optional_double", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleBetween(double lower, double upper)
                {
                    _query.AndFilter(Expr.Between("optional_double", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_double"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDoubleIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_double"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateIs(DateTime val)
                {
                    _query.AndFilter(Expr.Eq("required_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateIsNot(DateTime val)
                {
                    _query.AndFilter(Expr.Ne("required_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.In("required_date", vals.Select(val => new Value.DateValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateNotIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_date", vals.Select(val => new Value.DateValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateGreaterThan(DateTime val)
                {
                    _query.AndFilter(Expr.Gt("required_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateGreaterThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Gte("required_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateLessThan(DateTime val)
                {
                    _query.AndFilter(Expr.Lt("required_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateLessThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Lte("required_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateBetween(DateTime lower, DateTime upper)
                {
                    _query.AndFilter(Expr.Between("required_date", new Value.DateValue(lower), new Value.DateValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_date"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDateIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_date"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateIs(DateTime val)
                {
                    _query.AndFilter(Expr.Eq("optional_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateIsNot(DateTime val)
                {
                    _query.AndFilter(Expr.Ne("optional_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.In("optional_date", vals.Select(val => new Value.DateValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateNotIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_date", vals.Select(val => new Value.DateValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateGreaterThan(DateTime val)
                {
                    _query.AndFilter(Expr.Gt("optional_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateGreaterThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Gte("optional_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateLessThan(DateTime val)
                {
                    _query.AndFilter(Expr.Lt("optional_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateLessThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Lte("optional_date", new Value.DateValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateBetween(DateTime lower, DateTime upper)
                {
                    _query.AndFilter(Expr.Between("optional_date", new Value.DateValue(lower), new Value.DateValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_date"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDateIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_date"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeIs(TimeSpan val)
                {
                    _query.AndFilter(Expr.Eq("required_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeIsNot(TimeSpan val)
                {
                    _query.AndFilter(Expr.Ne("required_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeIn(params TimeSpan[] vals)
                {
                    _query.AndFilter(Expr.In("required_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeNotIn(params TimeSpan[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeGreaterThan(TimeSpan val)
                {
                    _query.AndFilter(Expr.Gt("required_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeGreaterThanOrEqualTo(TimeSpan val)
                {
                    _query.AndFilter(Expr.Gte("required_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeLessThan(TimeSpan val)
                {
                    _query.AndFilter(Expr.Lt("required_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeLessThanOrEqualTo(TimeSpan val)
                {
                    _query.AndFilter(Expr.Lte("required_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeBetween(TimeSpan lower, TimeSpan upper)
                {
                    _query.AndFilter(Expr.Between("required_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_time"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_time"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeIs(TimeSpan val)
                {
                    _query.AndFilter(Expr.Eq("optional_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeIsNot(TimeSpan val)
                {
                    _query.AndFilter(Expr.Ne("optional_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeIn(params TimeSpan[] vals)
                {
                    _query.AndFilter(Expr.In("optional_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeNotIn(params TimeSpan[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeGreaterThan(TimeSpan val)
                {
                    _query.AndFilter(Expr.Gt("optional_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeGreaterThanOrEqualTo(TimeSpan val)
                {
                    _query.AndFilter(Expr.Gte("optional_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeLessThan(TimeSpan val)
                {
                    _query.AndFilter(Expr.Lt("optional_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeLessThanOrEqualTo(TimeSpan val)
                {
                    _query.AndFilter(Expr.Lte("optional_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeBetween(TimeSpan lower, TimeSpan upper)
                {
                    _query.AndFilter(Expr.Between("optional_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_time"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_time"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeIs(DateTime val)
                {
                    _query.AndFilter(Expr.Eq("required_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeIsNot(DateTime val)
                {
                    _query.AndFilter(Expr.Ne("required_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.In("required_datetime", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeNotIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.NotIn("required_datetime", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeGreaterThan(DateTime val)
                {
                    _query.AndFilter(Expr.Gt("required_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeGreaterThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Gte("required_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeLessThan(DateTime val)
                {
                    _query.AndFilter(Expr.Lt("required_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeLessThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Lte("required_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeBetween(DateTime lower, DateTime upper)
                {
                    _query.AndFilter(Expr.Between("required_datetime", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("required_datetime"));
                    return this;
                }

                public ScalarTypeProbeRequest WithRequiredDatetimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("required_datetime"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeIs(DateTime val)
                {
                    _query.AndFilter(Expr.Eq("optional_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeIsNot(DateTime val)
                {
                    _query.AndFilter(Expr.Ne("optional_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.In("optional_datetime", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeNotIn(params DateTime[] vals)
                {
                    _query.AndFilter(Expr.NotIn("optional_datetime", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeGreaterThan(DateTime val)
                {
                    _query.AndFilter(Expr.Gt("optional_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeGreaterThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Gte("optional_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeLessThan(DateTime val)
                {
                    _query.AndFilter(Expr.Lt("optional_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeLessThanOrEqualTo(DateTime val)
                {
                    _query.AndFilter(Expr.Lte("optional_datetime", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeBetween(DateTime lower, DateTime upper)
                {
                    _query.AndFilter(Expr.Between("optional_datetime", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("optional_datetime"));
                    return this;
                }

                public ScalarTypeProbeRequest WithOptionalDatetimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("optional_datetime"));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("create_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public ScalarTypeProbeRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeIs(long val)
                {
                    _query.AndFilter(Expr.Eq("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("update_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public ScalarTypeProbeRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public ScalarTypeProbeRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public ScalarTypeProbeRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public ScalarTypeProbeRequest OrderByRequiredStringAscending()
                {
                    _query.OrderBy("required_string", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredStringDescending()
                {
                    _query.OrderBy("required_string", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalStringAscending()
                {
                    _query.OrderBy("optional_string", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalStringDescending()
                {
                    _query.OrderBy("optional_string", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByLargeTextAscending()
                {
                    _query.OrderBy("large_text", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByLargeTextDescending()
                {
                    _query.OrderBy("large_text", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredBooleanAscending()
                {
                    _query.OrderBy("required_boolean", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredBooleanDescending()
                {
                    _query.OrderBy("required_boolean", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalBooleanAscending()
                {
                    _query.OrderBy("optional_boolean", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalBooleanDescending()
                {
                    _query.OrderBy("optional_boolean", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredIntegerAscending()
                {
                    _query.OrderBy("required_integer", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredIntegerDescending()
                {
                    _query.OrderBy("required_integer", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalIntegerAscending()
                {
                    _query.OrderBy("optional_integer", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalIntegerDescending()
                {
                    _query.OrderBy("optional_integer", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredLongAscending()
                {
                    _query.OrderBy("required_long", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredLongDescending()
                {
                    _query.OrderBy("required_long", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalLongAscending()
                {
                    _query.OrderBy("optional_long", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalLongDescending()
                {
                    _query.OrderBy("optional_long", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredDecimalAscending()
                {
                    _query.OrderBy("required_decimal", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredDecimalDescending()
                {
                    _query.OrderBy("required_decimal", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalDecimalAscending()
                {
                    _query.OrderBy("optional_decimal", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalDecimalDescending()
                {
                    _query.OrderBy("optional_decimal", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredFloatAscending()
                {
                    _query.OrderBy("required_float", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredFloatDescending()
                {
                    _query.OrderBy("required_float", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalFloatAscending()
                {
                    _query.OrderBy("optional_float", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalFloatDescending()
                {
                    _query.OrderBy("optional_float", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredDoubleAscending()
                {
                    _query.OrderBy("required_double", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredDoubleDescending()
                {
                    _query.OrderBy("required_double", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalDoubleAscending()
                {
                    _query.OrderBy("optional_double", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalDoubleDescending()
                {
                    _query.OrderBy("optional_double", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredDateAscending()
                {
                    _query.OrderBy("required_date", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredDateDescending()
                {
                    _query.OrderBy("required_date", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalDateAscending()
                {
                    _query.OrderBy("optional_date", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalDateDescending()
                {
                    _query.OrderBy("optional_date", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredTimeAscending()
                {
                    _query.OrderBy("required_time", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredTimeDescending()
                {
                    _query.OrderBy("required_time", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalTimeAscending()
                {
                    _query.OrderBy("optional_time", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalTimeDescending()
                {
                    _query.OrderBy("optional_time", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredDatetimeAscending()
                {
                    _query.OrderBy("required_datetime", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByRequiredDatetimeDescending()
                {
                    _query.OrderBy("required_datetime", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalDatetimeAscending()
                {
                    _query.OrderBy("optional_datetime", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByOptionalDatetimeDescending()
                {
                    _query.OrderBy("optional_datetime", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public ScalarTypeProbeRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public ScalarTypeProbeRequest Count()
        {
            return CountAs("count");
        }

        public ScalarTypeProbeRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public ScalarTypeProbeRequest MinRequiredInteger()
                {
                    return MinRequiredIntegerAs("minOfRequiredInteger");
                }

                public ScalarTypeProbeRequest MinRequiredIntegerAs(string retName)
                {
                    _query.Aggregate("min", "required_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxRequiredInteger()
                {
                    return MaxRequiredIntegerAs("maxOfRequiredInteger");
                }

                public ScalarTypeProbeRequest MaxRequiredIntegerAs(string retName)
                {
                    _query.Aggregate("max", "required_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumRequiredInteger()
                {
                    return SumRequiredIntegerAs("sumOfRequiredInteger");
                }

                public ScalarTypeProbeRequest SumRequiredIntegerAs(string retName)
                {
                    _query.Aggregate("sum", "required_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgRequiredInteger()
                {
                    return AvgRequiredIntegerAs("avgOfRequiredInteger");
                }

                public ScalarTypeProbeRequest AvgRequiredIntegerAs(string retName)
                {
                    _query.Aggregate("avg", "required_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationRequiredInteger()
                {
                    return StandardDeviationRequiredIntegerAs("standardDeviationOfRequiredInteger");
                }

                public ScalarTypeProbeRequest StandardDeviationRequiredIntegerAs(string retName)
                {
                    _query.Aggregate("stddev", "required_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredInteger()
                {
                    return SquareRootOfPopulationStandardDeviationRequiredIntegerAs("squareRootOfPopulationStandardDeviationOfRequiredInteger");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredIntegerAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "required_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceRequiredInteger()
                {
                    return SampleVarianceRequiredIntegerAs("sampleVarianceOfRequiredInteger");
                }

                public ScalarTypeProbeRequest SampleVarianceRequiredIntegerAs(string retName)
                {
                    _query.Aggregate("var_samp", "required_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredInteger()
                {
                    return SamplePopulationVarianceRequiredIntegerAs("samplePopulationVarianceOfRequiredInteger");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredIntegerAs(string retName)
                {
                    _query.Aggregate("var_pop", "required_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MinOptionalInteger()
                {
                    return MinOptionalIntegerAs("minOfOptionalInteger");
                }

                public ScalarTypeProbeRequest MinOptionalIntegerAs(string retName)
                {
                    _query.Aggregate("min", "optional_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxOptionalInteger()
                {
                    return MaxOptionalIntegerAs("maxOfOptionalInteger");
                }

                public ScalarTypeProbeRequest MaxOptionalIntegerAs(string retName)
                {
                    _query.Aggregate("max", "optional_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumOptionalInteger()
                {
                    return SumOptionalIntegerAs("sumOfOptionalInteger");
                }

                public ScalarTypeProbeRequest SumOptionalIntegerAs(string retName)
                {
                    _query.Aggregate("sum", "optional_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgOptionalInteger()
                {
                    return AvgOptionalIntegerAs("avgOfOptionalInteger");
                }

                public ScalarTypeProbeRequest AvgOptionalIntegerAs(string retName)
                {
                    _query.Aggregate("avg", "optional_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationOptionalInteger()
                {
                    return StandardDeviationOptionalIntegerAs("standardDeviationOfOptionalInteger");
                }

                public ScalarTypeProbeRequest StandardDeviationOptionalIntegerAs(string retName)
                {
                    _query.Aggregate("stddev", "optional_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalInteger()
                {
                    return SquareRootOfPopulationStandardDeviationOptionalIntegerAs("squareRootOfPopulationStandardDeviationOfOptionalInteger");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalIntegerAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "optional_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceOptionalInteger()
                {
                    return SampleVarianceOptionalIntegerAs("sampleVarianceOfOptionalInteger");
                }

                public ScalarTypeProbeRequest SampleVarianceOptionalIntegerAs(string retName)
                {
                    _query.Aggregate("var_samp", "optional_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalInteger()
                {
                    return SamplePopulationVarianceOptionalIntegerAs("samplePopulationVarianceOfOptionalInteger");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalIntegerAs(string retName)
                {
                    _query.Aggregate("var_pop", "optional_integer", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MinRequiredLong()
                {
                    return MinRequiredLongAs("minOfRequiredLong");
                }

                public ScalarTypeProbeRequest MinRequiredLongAs(string retName)
                {
                    _query.Aggregate("min", "required_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxRequiredLong()
                {
                    return MaxRequiredLongAs("maxOfRequiredLong");
                }

                public ScalarTypeProbeRequest MaxRequiredLongAs(string retName)
                {
                    _query.Aggregate("max", "required_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumRequiredLong()
                {
                    return SumRequiredLongAs("sumOfRequiredLong");
                }

                public ScalarTypeProbeRequest SumRequiredLongAs(string retName)
                {
                    _query.Aggregate("sum", "required_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgRequiredLong()
                {
                    return AvgRequiredLongAs("avgOfRequiredLong");
                }

                public ScalarTypeProbeRequest AvgRequiredLongAs(string retName)
                {
                    _query.Aggregate("avg", "required_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationRequiredLong()
                {
                    return StandardDeviationRequiredLongAs("standardDeviationOfRequiredLong");
                }

                public ScalarTypeProbeRequest StandardDeviationRequiredLongAs(string retName)
                {
                    _query.Aggregate("stddev", "required_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredLong()
                {
                    return SquareRootOfPopulationStandardDeviationRequiredLongAs("squareRootOfPopulationStandardDeviationOfRequiredLong");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredLongAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "required_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceRequiredLong()
                {
                    return SampleVarianceRequiredLongAs("sampleVarianceOfRequiredLong");
                }

                public ScalarTypeProbeRequest SampleVarianceRequiredLongAs(string retName)
                {
                    _query.Aggregate("var_samp", "required_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredLong()
                {
                    return SamplePopulationVarianceRequiredLongAs("samplePopulationVarianceOfRequiredLong");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredLongAs(string retName)
                {
                    _query.Aggregate("var_pop", "required_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MinOptionalLong()
                {
                    return MinOptionalLongAs("minOfOptionalLong");
                }

                public ScalarTypeProbeRequest MinOptionalLongAs(string retName)
                {
                    _query.Aggregate("min", "optional_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxOptionalLong()
                {
                    return MaxOptionalLongAs("maxOfOptionalLong");
                }

                public ScalarTypeProbeRequest MaxOptionalLongAs(string retName)
                {
                    _query.Aggregate("max", "optional_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumOptionalLong()
                {
                    return SumOptionalLongAs("sumOfOptionalLong");
                }

                public ScalarTypeProbeRequest SumOptionalLongAs(string retName)
                {
                    _query.Aggregate("sum", "optional_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgOptionalLong()
                {
                    return AvgOptionalLongAs("avgOfOptionalLong");
                }

                public ScalarTypeProbeRequest AvgOptionalLongAs(string retName)
                {
                    _query.Aggregate("avg", "optional_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationOptionalLong()
                {
                    return StandardDeviationOptionalLongAs("standardDeviationOfOptionalLong");
                }

                public ScalarTypeProbeRequest StandardDeviationOptionalLongAs(string retName)
                {
                    _query.Aggregate("stddev", "optional_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalLong()
                {
                    return SquareRootOfPopulationStandardDeviationOptionalLongAs("squareRootOfPopulationStandardDeviationOfOptionalLong");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalLongAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "optional_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceOptionalLong()
                {
                    return SampleVarianceOptionalLongAs("sampleVarianceOfOptionalLong");
                }

                public ScalarTypeProbeRequest SampleVarianceOptionalLongAs(string retName)
                {
                    _query.Aggregate("var_samp", "optional_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalLong()
                {
                    return SamplePopulationVarianceOptionalLongAs("samplePopulationVarianceOfOptionalLong");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalLongAs(string retName)
                {
                    _query.Aggregate("var_pop", "optional_long", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MinRequiredDecimal()
                {
                    return MinRequiredDecimalAs("minOfRequiredDecimal");
                }

                public ScalarTypeProbeRequest MinRequiredDecimalAs(string retName)
                {
                    _query.Aggregate("min", "required_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxRequiredDecimal()
                {
                    return MaxRequiredDecimalAs("maxOfRequiredDecimal");
                }

                public ScalarTypeProbeRequest MaxRequiredDecimalAs(string retName)
                {
                    _query.Aggregate("max", "required_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumRequiredDecimal()
                {
                    return SumRequiredDecimalAs("sumOfRequiredDecimal");
                }

                public ScalarTypeProbeRequest SumRequiredDecimalAs(string retName)
                {
                    _query.Aggregate("sum", "required_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgRequiredDecimal()
                {
                    return AvgRequiredDecimalAs("avgOfRequiredDecimal");
                }

                public ScalarTypeProbeRequest AvgRequiredDecimalAs(string retName)
                {
                    _query.Aggregate("avg", "required_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationRequiredDecimal()
                {
                    return StandardDeviationRequiredDecimalAs("standardDeviationOfRequiredDecimal");
                }

                public ScalarTypeProbeRequest StandardDeviationRequiredDecimalAs(string retName)
                {
                    _query.Aggregate("stddev", "required_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredDecimal()
                {
                    return SquareRootOfPopulationStandardDeviationRequiredDecimalAs("squareRootOfPopulationStandardDeviationOfRequiredDecimal");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredDecimalAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "required_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceRequiredDecimal()
                {
                    return SampleVarianceRequiredDecimalAs("sampleVarianceOfRequiredDecimal");
                }

                public ScalarTypeProbeRequest SampleVarianceRequiredDecimalAs(string retName)
                {
                    _query.Aggregate("var_samp", "required_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredDecimal()
                {
                    return SamplePopulationVarianceRequiredDecimalAs("samplePopulationVarianceOfRequiredDecimal");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredDecimalAs(string retName)
                {
                    _query.Aggregate("var_pop", "required_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MinOptionalDecimal()
                {
                    return MinOptionalDecimalAs("minOfOptionalDecimal");
                }

                public ScalarTypeProbeRequest MinOptionalDecimalAs(string retName)
                {
                    _query.Aggregate("min", "optional_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxOptionalDecimal()
                {
                    return MaxOptionalDecimalAs("maxOfOptionalDecimal");
                }

                public ScalarTypeProbeRequest MaxOptionalDecimalAs(string retName)
                {
                    _query.Aggregate("max", "optional_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumOptionalDecimal()
                {
                    return SumOptionalDecimalAs("sumOfOptionalDecimal");
                }

                public ScalarTypeProbeRequest SumOptionalDecimalAs(string retName)
                {
                    _query.Aggregate("sum", "optional_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgOptionalDecimal()
                {
                    return AvgOptionalDecimalAs("avgOfOptionalDecimal");
                }

                public ScalarTypeProbeRequest AvgOptionalDecimalAs(string retName)
                {
                    _query.Aggregate("avg", "optional_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationOptionalDecimal()
                {
                    return StandardDeviationOptionalDecimalAs("standardDeviationOfOptionalDecimal");
                }

                public ScalarTypeProbeRequest StandardDeviationOptionalDecimalAs(string retName)
                {
                    _query.Aggregate("stddev", "optional_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalDecimal()
                {
                    return SquareRootOfPopulationStandardDeviationOptionalDecimalAs("squareRootOfPopulationStandardDeviationOfOptionalDecimal");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalDecimalAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "optional_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceOptionalDecimal()
                {
                    return SampleVarianceOptionalDecimalAs("sampleVarianceOfOptionalDecimal");
                }

                public ScalarTypeProbeRequest SampleVarianceOptionalDecimalAs(string retName)
                {
                    _query.Aggregate("var_samp", "optional_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalDecimal()
                {
                    return SamplePopulationVarianceOptionalDecimalAs("samplePopulationVarianceOfOptionalDecimal");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalDecimalAs(string retName)
                {
                    _query.Aggregate("var_pop", "optional_decimal", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MinRequiredFloat()
                {
                    return MinRequiredFloatAs("minOfRequiredFloat");
                }

                public ScalarTypeProbeRequest MinRequiredFloatAs(string retName)
                {
                    _query.Aggregate("min", "required_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxRequiredFloat()
                {
                    return MaxRequiredFloatAs("maxOfRequiredFloat");
                }

                public ScalarTypeProbeRequest MaxRequiredFloatAs(string retName)
                {
                    _query.Aggregate("max", "required_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumRequiredFloat()
                {
                    return SumRequiredFloatAs("sumOfRequiredFloat");
                }

                public ScalarTypeProbeRequest SumRequiredFloatAs(string retName)
                {
                    _query.Aggregate("sum", "required_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgRequiredFloat()
                {
                    return AvgRequiredFloatAs("avgOfRequiredFloat");
                }

                public ScalarTypeProbeRequest AvgRequiredFloatAs(string retName)
                {
                    _query.Aggregate("avg", "required_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationRequiredFloat()
                {
                    return StandardDeviationRequiredFloatAs("standardDeviationOfRequiredFloat");
                }

                public ScalarTypeProbeRequest StandardDeviationRequiredFloatAs(string retName)
                {
                    _query.Aggregate("stddev", "required_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredFloat()
                {
                    return SquareRootOfPopulationStandardDeviationRequiredFloatAs("squareRootOfPopulationStandardDeviationOfRequiredFloat");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredFloatAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "required_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceRequiredFloat()
                {
                    return SampleVarianceRequiredFloatAs("sampleVarianceOfRequiredFloat");
                }

                public ScalarTypeProbeRequest SampleVarianceRequiredFloatAs(string retName)
                {
                    _query.Aggregate("var_samp", "required_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredFloat()
                {
                    return SamplePopulationVarianceRequiredFloatAs("samplePopulationVarianceOfRequiredFloat");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredFloatAs(string retName)
                {
                    _query.Aggregate("var_pop", "required_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MinOptionalFloat()
                {
                    return MinOptionalFloatAs("minOfOptionalFloat");
                }

                public ScalarTypeProbeRequest MinOptionalFloatAs(string retName)
                {
                    _query.Aggregate("min", "optional_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxOptionalFloat()
                {
                    return MaxOptionalFloatAs("maxOfOptionalFloat");
                }

                public ScalarTypeProbeRequest MaxOptionalFloatAs(string retName)
                {
                    _query.Aggregate("max", "optional_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumOptionalFloat()
                {
                    return SumOptionalFloatAs("sumOfOptionalFloat");
                }

                public ScalarTypeProbeRequest SumOptionalFloatAs(string retName)
                {
                    _query.Aggregate("sum", "optional_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgOptionalFloat()
                {
                    return AvgOptionalFloatAs("avgOfOptionalFloat");
                }

                public ScalarTypeProbeRequest AvgOptionalFloatAs(string retName)
                {
                    _query.Aggregate("avg", "optional_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationOptionalFloat()
                {
                    return StandardDeviationOptionalFloatAs("standardDeviationOfOptionalFloat");
                }

                public ScalarTypeProbeRequest StandardDeviationOptionalFloatAs(string retName)
                {
                    _query.Aggregate("stddev", "optional_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalFloat()
                {
                    return SquareRootOfPopulationStandardDeviationOptionalFloatAs("squareRootOfPopulationStandardDeviationOfOptionalFloat");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalFloatAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "optional_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceOptionalFloat()
                {
                    return SampleVarianceOptionalFloatAs("sampleVarianceOfOptionalFloat");
                }

                public ScalarTypeProbeRequest SampleVarianceOptionalFloatAs(string retName)
                {
                    _query.Aggregate("var_samp", "optional_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalFloat()
                {
                    return SamplePopulationVarianceOptionalFloatAs("samplePopulationVarianceOfOptionalFloat");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalFloatAs(string retName)
                {
                    _query.Aggregate("var_pop", "optional_float", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MinRequiredDouble()
                {
                    return MinRequiredDoubleAs("minOfRequiredDouble");
                }

                public ScalarTypeProbeRequest MinRequiredDoubleAs(string retName)
                {
                    _query.Aggregate("min", "required_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxRequiredDouble()
                {
                    return MaxRequiredDoubleAs("maxOfRequiredDouble");
                }

                public ScalarTypeProbeRequest MaxRequiredDoubleAs(string retName)
                {
                    _query.Aggregate("max", "required_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumRequiredDouble()
                {
                    return SumRequiredDoubleAs("sumOfRequiredDouble");
                }

                public ScalarTypeProbeRequest SumRequiredDoubleAs(string retName)
                {
                    _query.Aggregate("sum", "required_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgRequiredDouble()
                {
                    return AvgRequiredDoubleAs("avgOfRequiredDouble");
                }

                public ScalarTypeProbeRequest AvgRequiredDoubleAs(string retName)
                {
                    _query.Aggregate("avg", "required_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationRequiredDouble()
                {
                    return StandardDeviationRequiredDoubleAs("standardDeviationOfRequiredDouble");
                }

                public ScalarTypeProbeRequest StandardDeviationRequiredDoubleAs(string retName)
                {
                    _query.Aggregate("stddev", "required_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredDouble()
                {
                    return SquareRootOfPopulationStandardDeviationRequiredDoubleAs("squareRootOfPopulationStandardDeviationOfRequiredDouble");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationRequiredDoubleAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "required_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceRequiredDouble()
                {
                    return SampleVarianceRequiredDoubleAs("sampleVarianceOfRequiredDouble");
                }

                public ScalarTypeProbeRequest SampleVarianceRequiredDoubleAs(string retName)
                {
                    _query.Aggregate("var_samp", "required_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredDouble()
                {
                    return SamplePopulationVarianceRequiredDoubleAs("samplePopulationVarianceOfRequiredDouble");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceRequiredDoubleAs(string retName)
                {
                    _query.Aggregate("var_pop", "required_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MinOptionalDouble()
                {
                    return MinOptionalDoubleAs("minOfOptionalDouble");
                }

                public ScalarTypeProbeRequest MinOptionalDoubleAs(string retName)
                {
                    _query.Aggregate("min", "optional_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest MaxOptionalDouble()
                {
                    return MaxOptionalDoubleAs("maxOfOptionalDouble");
                }

                public ScalarTypeProbeRequest MaxOptionalDoubleAs(string retName)
                {
                    _query.Aggregate("max", "optional_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SumOptionalDouble()
                {
                    return SumOptionalDoubleAs("sumOfOptionalDouble");
                }

                public ScalarTypeProbeRequest SumOptionalDoubleAs(string retName)
                {
                    _query.Aggregate("sum", "optional_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest AvgOptionalDouble()
                {
                    return AvgOptionalDoubleAs("avgOfOptionalDouble");
                }

                public ScalarTypeProbeRequest AvgOptionalDoubleAs(string retName)
                {
                    _query.Aggregate("avg", "optional_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest StandardDeviationOptionalDouble()
                {
                    return StandardDeviationOptionalDoubleAs("standardDeviationOfOptionalDouble");
                }

                public ScalarTypeProbeRequest StandardDeviationOptionalDoubleAs(string retName)
                {
                    _query.Aggregate("stddev", "optional_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalDouble()
                {
                    return SquareRootOfPopulationStandardDeviationOptionalDoubleAs("squareRootOfPopulationStandardDeviationOfOptionalDouble");
                }

                public ScalarTypeProbeRequest SquareRootOfPopulationStandardDeviationOptionalDoubleAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "optional_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SampleVarianceOptionalDouble()
                {
                    return SampleVarianceOptionalDoubleAs("sampleVarianceOfOptionalDouble");
                }

                public ScalarTypeProbeRequest SampleVarianceOptionalDoubleAs(string retName)
                {
                    _query.Aggregate("var_samp", "optional_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalDouble()
                {
                    return SamplePopulationVarianceOptionalDoubleAs("samplePopulationVarianceOfOptionalDouble");
                }

                public ScalarTypeProbeRequest SamplePopulationVarianceOptionalDoubleAs(string retName)
                {
                    _query.Aggregate("var_pop", "optional_double", retName);
                    return this;
                }
                public ScalarTypeProbeRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id");
                    _query.ProjectExpr(retName, Expr.Column("id"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByTypePlatform()
                {
                    _query.GroupBy("type_platform");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByTypePlatformAs(string retName)
                {
                    _query.GroupBy("type_platform");
                    _query.ProjectExpr(retName, Expr.Column("type_platform"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredString()
                {
                    _query.GroupBy("required_string");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredStringAs(string retName)
                {
                    _query.GroupBy("required_string");
                    _query.ProjectExpr(retName, Expr.Column("required_string"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalString()
                {
                    _query.GroupBy("optional_string");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalStringAs(string retName)
                {
                    _query.GroupBy("optional_string");
                    _query.ProjectExpr(retName, Expr.Column("optional_string"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByLargeText()
                {
                    _query.GroupBy("large_text");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByLargeTextAs(string retName)
                {
                    _query.GroupBy("large_text");
                    _query.ProjectExpr(retName, Expr.Column("large_text"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredBoolean()
                {
                    _query.GroupBy("required_boolean");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredBooleanAs(string retName)
                {
                    _query.GroupBy("required_boolean");
                    _query.ProjectExpr(retName, Expr.Column("required_boolean"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalBoolean()
                {
                    _query.GroupBy("optional_boolean");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalBooleanAs(string retName)
                {
                    _query.GroupBy("optional_boolean");
                    _query.ProjectExpr(retName, Expr.Column("optional_boolean"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredInteger()
                {
                    _query.GroupBy("required_integer");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredIntegerAs(string retName)
                {
                    _query.GroupBy("required_integer");
                    _query.ProjectExpr(retName, Expr.Column("required_integer"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalInteger()
                {
                    _query.GroupBy("optional_integer");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalIntegerAs(string retName)
                {
                    _query.GroupBy("optional_integer");
                    _query.ProjectExpr(retName, Expr.Column("optional_integer"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredLong()
                {
                    _query.GroupBy("required_long");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredLongAs(string retName)
                {
                    _query.GroupBy("required_long");
                    _query.ProjectExpr(retName, Expr.Column("required_long"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalLong()
                {
                    _query.GroupBy("optional_long");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalLongAs(string retName)
                {
                    _query.GroupBy("optional_long");
                    _query.ProjectExpr(retName, Expr.Column("optional_long"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredDecimal()
                {
                    _query.GroupBy("required_decimal");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredDecimalAs(string retName)
                {
                    _query.GroupBy("required_decimal");
                    _query.ProjectExpr(retName, Expr.Column("required_decimal"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalDecimal()
                {
                    _query.GroupBy("optional_decimal");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalDecimalAs(string retName)
                {
                    _query.GroupBy("optional_decimal");
                    _query.ProjectExpr(retName, Expr.Column("optional_decimal"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredFloat()
                {
                    _query.GroupBy("required_float");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredFloatAs(string retName)
                {
                    _query.GroupBy("required_float");
                    _query.ProjectExpr(retName, Expr.Column("required_float"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalFloat()
                {
                    _query.GroupBy("optional_float");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalFloatAs(string retName)
                {
                    _query.GroupBy("optional_float");
                    _query.ProjectExpr(retName, Expr.Column("optional_float"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredDouble()
                {
                    _query.GroupBy("required_double");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredDoubleAs(string retName)
                {
                    _query.GroupBy("required_double");
                    _query.ProjectExpr(retName, Expr.Column("required_double"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalDouble()
                {
                    _query.GroupBy("optional_double");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalDoubleAs(string retName)
                {
                    _query.GroupBy("optional_double");
                    _query.ProjectExpr(retName, Expr.Column("optional_double"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredDate()
                {
                    _query.GroupBy("required_date");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredDateAs(string retName)
                {
                    _query.GroupBy("required_date");
                    _query.ProjectExpr(retName, Expr.Column("required_date"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalDate()
                {
                    _query.GroupBy("optional_date");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalDateAs(string retName)
                {
                    _query.GroupBy("optional_date");
                    _query.ProjectExpr(retName, Expr.Column("optional_date"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredTime()
                {
                    _query.GroupBy("required_time");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredTimeAs(string retName)
                {
                    _query.GroupBy("required_time");
                    _query.ProjectExpr(retName, Expr.Column("required_time"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalTime()
                {
                    _query.GroupBy("optional_time");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalTimeAs(string retName)
                {
                    _query.GroupBy("optional_time");
                    _query.ProjectExpr(retName, Expr.Column("optional_time"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByRequiredDatetime()
                {
                    _query.GroupBy("required_datetime");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByRequiredDatetimeAs(string retName)
                {
                    _query.GroupBy("required_datetime");
                    _query.ProjectExpr(retName, Expr.Column("required_datetime"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByOptionalDatetime()
                {
                    _query.GroupBy("optional_datetime");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByOptionalDatetimeAs(string retName)
                {
                    _query.GroupBy("optional_datetime");
                    _query.ProjectExpr(retName, Expr.Column("optional_datetime"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time");
                    _query.ProjectExpr(retName, Expr.Column("create_time"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time");
                    _query.ProjectExpr(retName, Expr.Column("update_time"));
                    return this;
                }
                public ScalarTypeProbeRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public ScalarTypeProbeRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version");
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public ScalarTypeProbeRequest FacetByTypePlatformAs(
                    string name, TypePlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "TypePlatform", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByTypePlatformAs", name, "type_platform")
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

        private async Task<Generated.Models.ScalarTypeProbe?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.ScalarTypeProbe.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.ScalarTypeProbe>> ExecuteForListWithMetadataInternalAsync(
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
            var entities = new SmartList<Generated.Models.ScalarTypeProbe>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.ScalarTypeProbe.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.ScalarTypeProbe> ExecuteForStreamInternalAsync(
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
                    Generated.Models.ScalarTypeProbe.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableScalarTypeProbeRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.ScalarTypeProbe?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.ScalarTypeProbe>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.ScalarTypeProbe>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableScalarTypeProbeRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.ScalarTypeProbe?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.ScalarTypeProbe>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.ScalarTypeProbe>> executeForStream,
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

        public ExecutableScalarTypeProbeRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.ScalarTypeProbe> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("ScalarTypeProbe").ConfigureAwait(false));
            var entity = context.InitializeEntity("ScalarTypeProbe", new Generated.Models.ScalarTypeProbe());
            entity.TeaqlInitializeNewId(identity);
            return entity;
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.ScalarTypeProbe>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.ScalarTypeProbe> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.ScalarTypeProbe?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}