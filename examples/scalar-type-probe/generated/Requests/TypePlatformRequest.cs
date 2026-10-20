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
    public class TypePlatformRequest
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

        public TypePlatformRequest()
        {
            _query = new SelectQuery("TypePlatform");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public TypePlatformRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public TypePlatformRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public TypePlatformRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public TypePlatformRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableTypePlatformRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableTypePlatformRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public TypePlatformRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public TypePlatformRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public TypePlatformRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public TypePlatformRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public TypePlatformRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public TypePlatformRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public TypePlatformRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public TypePlatformRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("version");
            return this;
        }

                public TypePlatformRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public TypePlatformRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public TypePlatformRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }


                public TypePlatformRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TypePlatformRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TypePlatformRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TypePlatformRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public TypePlatformRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public TypePlatformRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public TypePlatformRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public TypePlatformRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TypePlatformRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TypePlatformRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TypePlatformRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public TypePlatformRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public TypePlatformRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public TypePlatformRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public TypePlatformRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public TypePlatformRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public TypePlatformRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public TypePlatformRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public TypePlatformRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TypePlatformRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public TypePlatformRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public TypePlatformRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public TypePlatformRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public TypePlatformRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public TypePlatformRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public TypePlatformRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public TypePlatformRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public TypePlatformRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public TypePlatformRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public TypePlatformRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public TypePlatformRequest Count()
        {
            return CountAs("count");
        }

        public TypePlatformRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public TypePlatformRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public TypePlatformRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public TypePlatformRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public TypePlatformRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public TypePlatformRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public TypePlatformRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public TypePlatformRequest SelectScalarTypeProbeList()
                {
                    return SelectScalarTypeProbeListWith(new ScalarTypeProbeRequest());
                }

                public TypePlatformRequest SelectScalarTypeProbeListWith(ScalarTypeProbeRequest child)
                {
                    _query.RelationQuery("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectScalarTypeProbeListWith", Relation: "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest HaveScalarTypeProbes()
                    => WithScalarTypeProbeListMatching(new ScalarTypeProbeRequest());

                public TypePlatformRequest HaveNoScalarTypeProbes()
                    => WithoutScalarTypeProbeListMatching(new ScalarTypeProbeRequest());

                public TypePlatformRequest WithScalarTypeProbeListMatching(ScalarTypeProbeRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("ScalarTypeProbe")!, child.GetQuery(), "type_platform"));
                    return this;
                }

                public TypePlatformRequest WithoutScalarTypeProbeListMatching(ScalarTypeProbeRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("ScalarTypeProbe")!, child.GetQuery(), "type_platform"));
                    return this;
                }
                public TypePlatformRequest CountScalarTypeProbes()
                    => CountScalarTypeProbesAs("countScalarTypeProbes");

                public TypePlatformRequest CountScalarTypeProbesAs(string alias)
                {
                    CountScalarTypeProbesWith(alias, new ScalarTypeProbeRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }

                public TypePlatformRequest CountScalarTypeProbesWith(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountScalarTypeProbesWith", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }

                public TypePlatformRequest MinRequiredIntegerOfScalarTypeProbes()
                    => MinRequiredIntegerOfScalarTypeProbesAs("minOfRequiredIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinRequiredIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "required_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinRequiredIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxRequiredIntegerOfScalarTypeProbes()
                    => MaxRequiredIntegerOfScalarTypeProbesAs("maxOfRequiredIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxRequiredIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "required_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxRequiredIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumRequiredIntegerOfScalarTypeProbes()
                    => SumRequiredIntegerOfScalarTypeProbesAs("sumOfRequiredIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumRequiredIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "required_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumRequiredIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgRequiredIntegerOfScalarTypeProbes()
                    => AvgRequiredIntegerOfScalarTypeProbesAs("avgOfRequiredIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgRequiredIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "required_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgRequiredIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationRequiredIntegerOfScalarTypeProbes()
                    => StandardDeviationRequiredIntegerOfScalarTypeProbesAs("standardDeviationOfRequiredIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationRequiredIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "required_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationRequiredIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredIntegerOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationRequiredIntegerOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfRequiredIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "required_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationRequiredIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceRequiredIntegerOfScalarTypeProbes()
                    => SampleVarianceRequiredIntegerOfScalarTypeProbesAs("sampleVarianceOfRequiredIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceRequiredIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "required_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceRequiredIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceRequiredIntegerOfScalarTypeProbes()
                    => SamplePopulationVarianceRequiredIntegerOfScalarTypeProbesAs("samplePopulationVarianceOfRequiredIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceRequiredIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "required_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceRequiredIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MinOptionalIntegerOfScalarTypeProbes()
                    => MinOptionalIntegerOfScalarTypeProbesAs("minOfOptionalIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinOptionalIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "optional_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinOptionalIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxOptionalIntegerOfScalarTypeProbes()
                    => MaxOptionalIntegerOfScalarTypeProbesAs("maxOfOptionalIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxOptionalIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "optional_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxOptionalIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumOptionalIntegerOfScalarTypeProbes()
                    => SumOptionalIntegerOfScalarTypeProbesAs("sumOfOptionalIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumOptionalIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "optional_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumOptionalIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgOptionalIntegerOfScalarTypeProbes()
                    => AvgOptionalIntegerOfScalarTypeProbesAs("avgOfOptionalIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgOptionalIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "optional_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgOptionalIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationOptionalIntegerOfScalarTypeProbes()
                    => StandardDeviationOptionalIntegerOfScalarTypeProbesAs("standardDeviationOfOptionalIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationOptionalIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "optional_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationOptionalIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalIntegerOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationOptionalIntegerOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfOptionalIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "optional_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationOptionalIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceOptionalIntegerOfScalarTypeProbes()
                    => SampleVarianceOptionalIntegerOfScalarTypeProbesAs("sampleVarianceOfOptionalIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceOptionalIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "optional_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceOptionalIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceOptionalIntegerOfScalarTypeProbes()
                    => SamplePopulationVarianceOptionalIntegerOfScalarTypeProbesAs("samplePopulationVarianceOfOptionalIntegerOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceOptionalIntegerOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "optional_integer", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceOptionalIntegerOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MinRequiredLongOfScalarTypeProbes()
                    => MinRequiredLongOfScalarTypeProbesAs("minOfRequiredLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinRequiredLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "required_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinRequiredLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxRequiredLongOfScalarTypeProbes()
                    => MaxRequiredLongOfScalarTypeProbesAs("maxOfRequiredLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxRequiredLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "required_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxRequiredLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumRequiredLongOfScalarTypeProbes()
                    => SumRequiredLongOfScalarTypeProbesAs("sumOfRequiredLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumRequiredLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "required_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumRequiredLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgRequiredLongOfScalarTypeProbes()
                    => AvgRequiredLongOfScalarTypeProbesAs("avgOfRequiredLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgRequiredLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "required_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgRequiredLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationRequiredLongOfScalarTypeProbes()
                    => StandardDeviationRequiredLongOfScalarTypeProbesAs("standardDeviationOfRequiredLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationRequiredLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "required_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationRequiredLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredLongOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationRequiredLongOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfRequiredLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "required_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationRequiredLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceRequiredLongOfScalarTypeProbes()
                    => SampleVarianceRequiredLongOfScalarTypeProbesAs("sampleVarianceOfRequiredLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceRequiredLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "required_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceRequiredLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceRequiredLongOfScalarTypeProbes()
                    => SamplePopulationVarianceRequiredLongOfScalarTypeProbesAs("samplePopulationVarianceOfRequiredLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceRequiredLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "required_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceRequiredLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MinOptionalLongOfScalarTypeProbes()
                    => MinOptionalLongOfScalarTypeProbesAs("minOfOptionalLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinOptionalLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "optional_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinOptionalLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxOptionalLongOfScalarTypeProbes()
                    => MaxOptionalLongOfScalarTypeProbesAs("maxOfOptionalLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxOptionalLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "optional_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxOptionalLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumOptionalLongOfScalarTypeProbes()
                    => SumOptionalLongOfScalarTypeProbesAs("sumOfOptionalLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumOptionalLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "optional_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumOptionalLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgOptionalLongOfScalarTypeProbes()
                    => AvgOptionalLongOfScalarTypeProbesAs("avgOfOptionalLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgOptionalLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "optional_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgOptionalLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationOptionalLongOfScalarTypeProbes()
                    => StandardDeviationOptionalLongOfScalarTypeProbesAs("standardDeviationOfOptionalLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationOptionalLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "optional_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationOptionalLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalLongOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationOptionalLongOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfOptionalLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "optional_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationOptionalLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceOptionalLongOfScalarTypeProbes()
                    => SampleVarianceOptionalLongOfScalarTypeProbesAs("sampleVarianceOfOptionalLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceOptionalLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "optional_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceOptionalLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceOptionalLongOfScalarTypeProbes()
                    => SamplePopulationVarianceOptionalLongOfScalarTypeProbesAs("samplePopulationVarianceOfOptionalLongOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceOptionalLongOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "optional_long", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceOptionalLongOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MinRequiredDecimalOfScalarTypeProbes()
                    => MinRequiredDecimalOfScalarTypeProbesAs("minOfRequiredDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinRequiredDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "required_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinRequiredDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxRequiredDecimalOfScalarTypeProbes()
                    => MaxRequiredDecimalOfScalarTypeProbesAs("maxOfRequiredDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxRequiredDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "required_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxRequiredDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumRequiredDecimalOfScalarTypeProbes()
                    => SumRequiredDecimalOfScalarTypeProbesAs("sumOfRequiredDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumRequiredDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "required_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumRequiredDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgRequiredDecimalOfScalarTypeProbes()
                    => AvgRequiredDecimalOfScalarTypeProbesAs("avgOfRequiredDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgRequiredDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "required_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgRequiredDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationRequiredDecimalOfScalarTypeProbes()
                    => StandardDeviationRequiredDecimalOfScalarTypeProbesAs("standardDeviationOfRequiredDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationRequiredDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "required_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationRequiredDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredDecimalOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationRequiredDecimalOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfRequiredDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "required_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationRequiredDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceRequiredDecimalOfScalarTypeProbes()
                    => SampleVarianceRequiredDecimalOfScalarTypeProbesAs("sampleVarianceOfRequiredDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceRequiredDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "required_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceRequiredDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceRequiredDecimalOfScalarTypeProbes()
                    => SamplePopulationVarianceRequiredDecimalOfScalarTypeProbesAs("samplePopulationVarianceOfRequiredDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceRequiredDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "required_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceRequiredDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MinOptionalDecimalOfScalarTypeProbes()
                    => MinOptionalDecimalOfScalarTypeProbesAs("minOfOptionalDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinOptionalDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "optional_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinOptionalDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxOptionalDecimalOfScalarTypeProbes()
                    => MaxOptionalDecimalOfScalarTypeProbesAs("maxOfOptionalDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxOptionalDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "optional_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxOptionalDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumOptionalDecimalOfScalarTypeProbes()
                    => SumOptionalDecimalOfScalarTypeProbesAs("sumOfOptionalDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumOptionalDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "optional_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumOptionalDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgOptionalDecimalOfScalarTypeProbes()
                    => AvgOptionalDecimalOfScalarTypeProbesAs("avgOfOptionalDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgOptionalDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "optional_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgOptionalDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationOptionalDecimalOfScalarTypeProbes()
                    => StandardDeviationOptionalDecimalOfScalarTypeProbesAs("standardDeviationOfOptionalDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationOptionalDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "optional_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationOptionalDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalDecimalOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationOptionalDecimalOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfOptionalDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "optional_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationOptionalDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceOptionalDecimalOfScalarTypeProbes()
                    => SampleVarianceOptionalDecimalOfScalarTypeProbesAs("sampleVarianceOfOptionalDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceOptionalDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "optional_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceOptionalDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceOptionalDecimalOfScalarTypeProbes()
                    => SamplePopulationVarianceOptionalDecimalOfScalarTypeProbesAs("samplePopulationVarianceOfOptionalDecimalOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceOptionalDecimalOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "optional_decimal", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceOptionalDecimalOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MinRequiredFloatOfScalarTypeProbes()
                    => MinRequiredFloatOfScalarTypeProbesAs("minOfRequiredFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinRequiredFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "required_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinRequiredFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxRequiredFloatOfScalarTypeProbes()
                    => MaxRequiredFloatOfScalarTypeProbesAs("maxOfRequiredFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxRequiredFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "required_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxRequiredFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumRequiredFloatOfScalarTypeProbes()
                    => SumRequiredFloatOfScalarTypeProbesAs("sumOfRequiredFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumRequiredFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "required_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumRequiredFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgRequiredFloatOfScalarTypeProbes()
                    => AvgRequiredFloatOfScalarTypeProbesAs("avgOfRequiredFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgRequiredFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "required_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgRequiredFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationRequiredFloatOfScalarTypeProbes()
                    => StandardDeviationRequiredFloatOfScalarTypeProbesAs("standardDeviationOfRequiredFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationRequiredFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "required_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationRequiredFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredFloatOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationRequiredFloatOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfRequiredFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "required_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationRequiredFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceRequiredFloatOfScalarTypeProbes()
                    => SampleVarianceRequiredFloatOfScalarTypeProbesAs("sampleVarianceOfRequiredFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceRequiredFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "required_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceRequiredFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceRequiredFloatOfScalarTypeProbes()
                    => SamplePopulationVarianceRequiredFloatOfScalarTypeProbesAs("samplePopulationVarianceOfRequiredFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceRequiredFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "required_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceRequiredFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MinOptionalFloatOfScalarTypeProbes()
                    => MinOptionalFloatOfScalarTypeProbesAs("minOfOptionalFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinOptionalFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "optional_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinOptionalFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxOptionalFloatOfScalarTypeProbes()
                    => MaxOptionalFloatOfScalarTypeProbesAs("maxOfOptionalFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxOptionalFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "optional_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxOptionalFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumOptionalFloatOfScalarTypeProbes()
                    => SumOptionalFloatOfScalarTypeProbesAs("sumOfOptionalFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumOptionalFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "optional_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumOptionalFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgOptionalFloatOfScalarTypeProbes()
                    => AvgOptionalFloatOfScalarTypeProbesAs("avgOfOptionalFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgOptionalFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "optional_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgOptionalFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationOptionalFloatOfScalarTypeProbes()
                    => StandardDeviationOptionalFloatOfScalarTypeProbesAs("standardDeviationOfOptionalFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationOptionalFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "optional_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationOptionalFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalFloatOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationOptionalFloatOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfOptionalFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "optional_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationOptionalFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceOptionalFloatOfScalarTypeProbes()
                    => SampleVarianceOptionalFloatOfScalarTypeProbesAs("sampleVarianceOfOptionalFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceOptionalFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "optional_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceOptionalFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceOptionalFloatOfScalarTypeProbes()
                    => SamplePopulationVarianceOptionalFloatOfScalarTypeProbesAs("samplePopulationVarianceOfOptionalFloatOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceOptionalFloatOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "optional_float", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceOptionalFloatOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MinRequiredDoubleOfScalarTypeProbes()
                    => MinRequiredDoubleOfScalarTypeProbesAs("minOfRequiredDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinRequiredDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "required_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinRequiredDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxRequiredDoubleOfScalarTypeProbes()
                    => MaxRequiredDoubleOfScalarTypeProbesAs("maxOfRequiredDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxRequiredDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "required_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxRequiredDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumRequiredDoubleOfScalarTypeProbes()
                    => SumRequiredDoubleOfScalarTypeProbesAs("sumOfRequiredDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumRequiredDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "required_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumRequiredDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgRequiredDoubleOfScalarTypeProbes()
                    => AvgRequiredDoubleOfScalarTypeProbesAs("avgOfRequiredDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgRequiredDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "required_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgRequiredDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationRequiredDoubleOfScalarTypeProbes()
                    => StandardDeviationRequiredDoubleOfScalarTypeProbesAs("standardDeviationOfRequiredDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationRequiredDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "required_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationRequiredDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredDoubleOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationRequiredDoubleOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfRequiredDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationRequiredDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "required_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationRequiredDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceRequiredDoubleOfScalarTypeProbes()
                    => SampleVarianceRequiredDoubleOfScalarTypeProbesAs("sampleVarianceOfRequiredDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceRequiredDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "required_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceRequiredDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceRequiredDoubleOfScalarTypeProbes()
                    => SamplePopulationVarianceRequiredDoubleOfScalarTypeProbesAs("samplePopulationVarianceOfRequiredDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceRequiredDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "required_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceRequiredDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MinOptionalDoubleOfScalarTypeProbes()
                    => MinOptionalDoubleOfScalarTypeProbesAs("minOfOptionalDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MinOptionalDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "optional_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinOptionalDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest MaxOptionalDoubleOfScalarTypeProbes()
                    => MaxOptionalDoubleOfScalarTypeProbesAs("maxOfOptionalDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest MaxOptionalDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "optional_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxOptionalDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SumOptionalDoubleOfScalarTypeProbes()
                    => SumOptionalDoubleOfScalarTypeProbesAs("sumOfOptionalDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SumOptionalDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "optional_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumOptionalDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest AvgOptionalDoubleOfScalarTypeProbes()
                    => AvgOptionalDoubleOfScalarTypeProbesAs("avgOfOptionalDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest AvgOptionalDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "optional_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgOptionalDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest StandardDeviationOptionalDoubleOfScalarTypeProbes()
                    => StandardDeviationOptionalDoubleOfScalarTypeProbesAs("standardDeviationOfOptionalDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest StandardDeviationOptionalDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "optional_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationOptionalDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalDoubleOfScalarTypeProbes()
                    => SquareRootOfPopulationStandardDeviationOptionalDoubleOfScalarTypeProbesAs("squareRootOfPopulationStandardDeviationOfOptionalDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SquareRootOfPopulationStandardDeviationOptionalDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "optional_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationOptionalDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SampleVarianceOptionalDoubleOfScalarTypeProbes()
                    => SampleVarianceOptionalDoubleOfScalarTypeProbesAs("sampleVarianceOfOptionalDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SampleVarianceOptionalDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "optional_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceOptionalDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }
                public TypePlatformRequest SamplePopulationVarianceOptionalDoubleOfScalarTypeProbes()
                    => SamplePopulationVarianceOptionalDoubleOfScalarTypeProbesAs("samplePopulationVarianceOfOptionalDoubleOfScalarTypeProbes", new ScalarTypeProbeRequest());

                public TypePlatformRequest SamplePopulationVarianceOptionalDoubleOfScalarTypeProbesAs(string alias, ScalarTypeProbeRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "optional_double", alias);
                    _query.RelationAggregate("ScalarTypeProbeList", "ScalarTypeProbe", "type_platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceOptionalDoubleOfScalarTypeProbesAs", alias, "ScalarTypeProbeList")
                    };
                    return this;
                }

        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var request = context.PrepareQueryRequest(new QueryRequest(_query));
            return await service.QueryAsync(request);
        }

        private async Task<QueryResult> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            return await service.QueryAsync(context.PrepareQueryRequest(new QueryRequest(query)));
        }

        private async Task<SmartList<Record>> ExecuteForListWithMetadataInternalAsync(
            UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            var request = context.PrepareQueryRequest(new QueryRequest(query));
            if (_countRequested)
                return await RuntimeDataService.FetchPageAsync(service, context, request);
            var result = await service.QueryAsync(request);
            return new SmartList<Record>(result.Rows) { Facets = result.Facets };
        }

        private IAsyncEnumerable<Generated.Models.TypePlatform> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var request = context.PrepareQueryRequest(new QueryRequest(_query));
            return RuntimeDataService.StreamEntities(context, request,
                row => Generated.Models.TypePlatform.FromRecord(row, new EntityRoot()),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableTypePlatformRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<QueryResult>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Record>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.TypePlatform>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableTypePlatformRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<QueryResult>> executeForOne,
            Func<UserContext, Task<SmartList<Record>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.TypePlatform>> executeForStream,
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

        public ExecutableTypePlatformRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.TypePlatform NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.TypePlatform();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.TypePlatform>> ExecuteForListAsync(UserContext context)
        {
            var result = await _executeForList(context);
            var entities = new SmartList<Generated.Models.TypePlatform>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            var queryRoot = new EntityRoot();
            foreach (var row in result)
                entities.Add(Generated.Models.TypePlatform.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public IAsyncEnumerable<Generated.Models.TypePlatform> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.TypePlatform?> ExecuteForOneAsync(
            UserContext context)
        {
            var result = await _executeForOne(context);
            if (result.Rows.Count == 0) return null;
            return Generated.Models.TypePlatform.FromRecord(
                result.Rows[0], new EntityRoot());
        }
    }

}