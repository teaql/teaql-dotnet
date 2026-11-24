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
    public class CustomerRequest
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

        public CustomerRequest()
        {
            _query = new SelectQuery("Customer");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public CustomerRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public CustomerRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public CustomerRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public CustomerRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCustomerRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public CustomerRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public CustomerRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public CustomerRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public CustomerRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public CustomerRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public CustomerRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public CustomerRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public CustomerRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public CustomerRequest SelectId()
                {
                    _query.Project(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public CustomerRequest SelectName()
                {
                    _query.Project(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public CustomerRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public CustomerRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public CustomerRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, "Platform", global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public CustomerRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public CustomerRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public CustomerRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY, vals));
                    return this;
                }

                public CustomerRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public CustomerRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public CustomerRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public CustomerRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }

                public CustomerRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }
                public CustomerRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public CustomerRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public CustomerRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public CustomerRequest OrderByNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerRequest OrderByNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public CustomerRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public CustomerRequest Count()
        {
            return CountAs("count");
        }

        public CustomerRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public CustomerRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public CustomerRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Customer.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public CustomerRequest GroupByPlatform()
                {
                    _query.GroupBy(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }

                public CustomerRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }
                public CustomerRequest GroupByName()
                {
                    _query.GroupBy(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public CustomerRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Customer.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }
                public CustomerRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public CustomerRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Customer.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public CustomerRequest SelectCustomerOrderList()
                {
                    return SelectCustomerOrderListWith(new CustomerOrderRequest());
                }

                public CustomerRequest SelectCustomerOrderListWith(CustomerOrderRequest child)
                {
                    _query.RelationQuery("CustomerOrderList", "CustomerOrder", "customer", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderListWith", Relation: "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerRequest HaveCustomerOrders()
                    => WithCustomerOrderListMatching(new CustomerOrderRequest());

                public CustomerRequest HaveNoCustomerOrders()
                    => WithoutCustomerOrderListMatching(new CustomerOrderRequest());

                public CustomerRequest WithCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "customer"));
                    return this;
                }

                public CustomerRequest WithoutCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, child.GetQuery(), "customer"));
                    return this;
                }
                public CustomerRequest CountCustomerOrders()
                    => CountCustomerOrdersAs("countCustomerOrders");

                public CustomerRequest CountCustomerOrdersAs(string alias)
                {
                    CountCustomerOrdersWith(alias, new CustomerOrderRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }

                public CustomerRequest CountCustomerOrdersWith(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountCustomerOrdersWith", alias, "CustomerOrderList")
                    };
                    return this;
                }

                public CustomerRequest MinTotalAmountOfCustomerOrders()
                    => MinTotalAmountOfCustomerOrdersAs("minOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest MinTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerRequest MaxTotalAmountOfCustomerOrders()
                    => MaxTotalAmountOfCustomerOrdersAs("maxOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest MaxTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerRequest SumTotalAmountOfCustomerOrders()
                    => SumTotalAmountOfCustomerOrdersAs("sumOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest SumTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerRequest AvgTotalAmountOfCustomerOrders()
                    => AvgTotalAmountOfCustomerOrdersAs("avgOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest AvgTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerRequest StandardDeviationTotalAmountOfCustomerOrders()
                    => StandardDeviationTotalAmountOfCustomerOrdersAs("standardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest StandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrders()
                    => SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs("squareRootOfPopulationStandardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerRequest SampleVarianceTotalAmountOfCustomerOrders()
                    => SampleVarianceTotalAmountOfCustomerOrdersAs("sampleVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest SampleVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerRequest SamplePopulationVarianceTotalAmountOfCustomerOrders()
                    => SamplePopulationVarianceTotalAmountOfCustomerOrdersAs("samplePopulationVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest SamplePopulationVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "total_amount", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceTotalAmountOfCustomerOrdersAs", alias, "CustomerOrderList")
                    };
                    return this;
                }
                public CustomerRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, global::Generated.Models.Customer.__TeaqlFields.PLATFORM_PROPERTY_QUERY)
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

        private async Task<Generated.Models.Customer?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.Customer>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.Customer>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.Customer>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Customer.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Customer> ExecuteForStreamInternalAsync(
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
                    Generated.Models.Customer.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableCustomerRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Customer?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Customer>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Customer>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCustomerRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Customer?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Customer>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Customer>> executeForStream,
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

        public ExecutableCustomerRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.Customer> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Customer").ConfigureAwait(false));
            return context.InitializeEntity("Customer", Generated.Models.Customer.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Customer>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Customer> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Customer?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}