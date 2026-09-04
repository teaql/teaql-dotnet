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
    public class CustomerRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;

        public CustomerRequest()
        {
            _query = new SelectQuery("Customer");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

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
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerRequest(
                ExecuteForListInternalAsync,
                ExecuteForPageInternalAsync,
                ExecuteForStreamInternalAsync,
                () => Limit(1),
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
            _query.Project("id");
            _query.Project("name");
            _query.Project("email");
            _query.Project("commerce_platform");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public CustomerRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public CustomerRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public CustomerRequest SelectEmail()
                {
                    _query.Project("email");
                    return this;
                }


                public CustomerRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public CustomerRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public CustomerRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public CustomerRequest SelectCommercePlatform()
                {
                    return SelectCommercePlatformWith(new CommercePlatformRequest());
                }

                public CustomerRequest SelectCommercePlatformWith(CommercePlatformRequest related)
                {
                    _query.Project("commerce_platform");
                    _query.ForwardRelationQuery("CommercePlatform", "CommercePlatform", "commerce_platform", related.GetQuery());
                    return this;
                }
                public CustomerRequest WithCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public CustomerRequest WithoutCommercePlatformMatching(CommercePlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("commerce_platform", EntityDescriptor.New("CommercePlatform"), related.GetQuery(), "id"));
                    return this;
                }

                public CustomerRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", val));
                    return this;
                }

                public CustomerRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", val));
                    return this;
                }

                public CustomerRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals));
                    return this;
                }

                public CustomerRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals));
                    return this;
                }

                public CustomerRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", val));
                    return this;
                }

                public CustomerRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", val));
                    return this;
                }

                public CustomerRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", val));
                    return this;
                }

                public CustomerRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", val));
                    return this;
                }

                public CustomerRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", lower, upper));
                    return this;
                }

                public CustomerRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public CustomerRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public CustomerRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public CustomerRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", val));
                    return this;
                }
                public CustomerRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("name", val));
                    return this;
                }

                public CustomerRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals));
                    return this;
                }

                public CustomerRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals));
                    return this;
                }

                public CustomerRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("name", val));
                    return this;
                }

                public CustomerRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("name", val));
                    return this;
                }

                public CustomerRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("name", val));
                    return this;
                }

                public CustomerRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("name", val));
                    return this;
                }

                public CustomerRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("name", lower, upper));
                    return this;
                }

                public CustomerRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public CustomerRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public CustomerRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public CustomerRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public CustomerRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public CustomerRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public CustomerRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public CustomerRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public CustomerRequest WithEmailContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("email", val));
                    return this;
                }

                public CustomerRequest WithEmailIs(string val)
                {
                    _query.AndFilter(Expr.Eq("email", val));
                    return this;
                }
                public CustomerRequest WithEmailIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("email", val));
                    return this;
                }

                public CustomerRequest WithEmailIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("email", vals));
                    return this;
                }

                public CustomerRequest WithEmailNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("email", vals));
                    return this;
                }

                public CustomerRequest WithEmailGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("email", val));
                    return this;
                }

                public CustomerRequest WithEmailGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("email", val));
                    return this;
                }

                public CustomerRequest WithEmailLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("email", val));
                    return this;
                }

                public CustomerRequest WithEmailLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("email", val));
                    return this;
                }

                public CustomerRequest WithEmailBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("email", lower, upper));
                    return this;
                }

                public CustomerRequest WithEmailIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("email"));
                    return this;
                }

                public CustomerRequest WithEmailIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("email"));
                    return this;
                }
                public CustomerRequest WithEmailNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("email", val));
                    return this;
                }

                public CustomerRequest WithEmailStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("email", val));
                    return this;
                }

                public CustomerRequest WithEmailNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("email", val));
                    return this;
                }

                public CustomerRequest WithEmailEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("email", val));
                    return this;
                }

                public CustomerRequest WithEmailNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("email", val));
                    return this;
                }

                public CustomerRequest WithEmailSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("email", val));
                    return this;
                }

                public CustomerRequest FilterByCommercePlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("commerce_platform", val));
                    return this;
                }

                public CustomerRequest FilterByCommercePlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("commerce_platform", vals));
                    return this;
                }

                public CustomerRequest WithCommercePlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("commerce_platform"));
                    return this;
                }

                public CustomerRequest WithCommercePlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("commerce_platform"));
                    return this;
                }

                public CustomerRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", val));
                    return this;
                }

                public CustomerRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", val));
                    return this;
                }

                public CustomerRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals));
                    return this;
                }

                public CustomerRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals));
                    return this;
                }

                public CustomerRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", val));
                    return this;
                }

                public CustomerRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", val));
                    return this;
                }

                public CustomerRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", val));
                    return this;
                }

                public CustomerRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", val));
                    return this;
                }

                public CustomerRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", lower, upper));
                    return this;
                }

                public CustomerRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public CustomerRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public CustomerRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", val));
                    return this;
                }

                public CustomerRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("update_time", val));
                    return this;
                }

                public CustomerRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals));
                    return this;
                }

                public CustomerRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals));
                    return this;
                }

                public CustomerRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("update_time", val));
                    return this;
                }

                public CustomerRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", val));
                    return this;
                }

                public CustomerRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("update_time", val));
                    return this;
                }

                public CustomerRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", val));
                    return this;
                }

                public CustomerRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("update_time", lower, upper));
                    return this;
                }

                public CustomerRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public CustomerRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public CustomerRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", val));
                    return this;
                }

                public CustomerRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", val));
                    return this;
                }

                public CustomerRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals));
                    return this;
                }

                public CustomerRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals));
                    return this;
                }

                public CustomerRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", val));
                    return this;
                }

                public CustomerRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", val));
                    return this;
                }

                public CustomerRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", val));
                    return this;
                }

                public CustomerRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", val));
                    return this;
                }

                public CustomerRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", lower, upper));
                    return this;
                }

                public CustomerRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public CustomerRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public CustomerRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public CustomerRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public CustomerRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public CustomerRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public CustomerRequest OrderByEmailAscending()
                {
                    _query.OrderBy("email", "asc");
                    return this;
                }

                public CustomerRequest OrderByEmailDescending()
                {
                    _query.OrderBy("email", "desc");
                    return this;
                }


                public CustomerRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public CustomerRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public CustomerRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public CustomerRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public CustomerRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public CustomerRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public CustomerRequest Count()
        {
            _query.Aggregate("Count", "id", "count");
            return this;
        }

        public CustomerRequest CountAs(string retName)
        {
            _query.Aggregate("Count", "id", retName);
            return this;
        }

                public CustomerRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public CustomerRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public CustomerRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public CustomerRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name"); 
                    return this;
                }
                public CustomerRequest GroupByEmail()
                {
                    _query.GroupBy("email");
                    return this;
                }

                public CustomerRequest GroupByEmailAs(string retName)
                {
                    _query.GroupBy("email"); 
                    return this;
                }
                public CustomerRequest GroupByCommercePlatform()
                {
                    _query.GroupBy("commerce_platform");
                    return this;
                }

                public CustomerRequest GroupByCommercePlatformAs(string retName)
                {
                    _query.GroupBy("commerce_platform"); 
                    return this;
                }
                public CustomerRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public CustomerRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public CustomerRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public CustomerRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
                    return this;
                }
                public CustomerRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public CustomerRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public CustomerRequest SelectCustomerOrderList()
                {
                    return SelectCustomerOrderListWith(new CustomerOrderRequest());
                }

                public CustomerRequest SelectCustomerOrderListWith(CustomerOrderRequest child)
                {
                    _query.RelationQuery("CustomerOrderList", "CustomerOrder", "customer", true, child.GetQuery());
                    return this;
                }
                public CustomerRequest HaveCustomerOrders()
                    => WithCustomerOrderListMatching(new CustomerOrderRequest());

                public CustomerRequest HaveNoCustomerOrders()
                    => WithoutCustomerOrderListMatching(new CustomerOrderRequest());

                public CustomerRequest WithCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", EntityDescriptor.New("CustomerOrder"), child.GetQuery(), "customer"));
                    return this;
                }

                public CustomerRequest WithoutCustomerOrderListMatching(CustomerOrderRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", EntityDescriptor.New("CustomerOrder"), child.GetQuery(), "customer"));
                    return this;
                }
                public CustomerRequest CountCustomerOrders()
                    => CountCustomerOrdersAs("countCustomerOrders");

                public CustomerRequest CountCustomerOrdersAs(string alias)
                    => CountCustomerOrdersWith(alias, new CustomerOrderRequest());

                public CustomerRequest CountCustomerOrdersWith(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("Count", "id", alias);
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, child.GetQuery(), true);
                    return this;
                }

                public CustomerRequest MinTotalAmountOfCustomerOrders()
                    => MinTotalAmountOfCustomerOrdersAs("minOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest MinTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("min", "total_amount", "min_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerRequest MaxTotalAmountOfCustomerOrders()
                    => MaxTotalAmountOfCustomerOrdersAs("maxOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest MaxTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("max", "total_amount", "max_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerRequest SumTotalAmountOfCustomerOrders()
                    => SumTotalAmountOfCustomerOrdersAs("sumOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest SumTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("sum", "total_amount", "sum_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerRequest AvgTotalAmountOfCustomerOrders()
                    => AvgTotalAmountOfCustomerOrdersAs("avgOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest AvgTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("avg", "total_amount", "avg_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerRequest StandardDeviationTotalAmountOfCustomerOrders()
                    => StandardDeviationTotalAmountOfCustomerOrdersAs("standardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest StandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("stddev", "total_amount", "standardDeviation_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrders()
                    => SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs("squareRootOfPopulationStandardDeviationOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest SquareRootOfPopulationStandardDeviationTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("stddev_pop", "total_amount", "squareRootOfPopulationStandardDeviation_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerRequest SampleVarianceTotalAmountOfCustomerOrders()
                    => SampleVarianceTotalAmountOfCustomerOrdersAs("sampleVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest SampleVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("var_samp", "total_amount", "sampleVariance_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerRequest SamplePopulationVarianceTotalAmountOfCustomerOrders()
                    => SamplePopulationVarianceTotalAmountOfCustomerOrdersAs("samplePopulationVarianceOfTotalAmountOfCustomerOrders", new CustomerOrderRequest());

                public CustomerRequest SamplePopulationVarianceTotalAmountOfCustomerOrdersAs(string alias, CustomerOrderRequest child)
                {
                    child.GetQuery().Aggregate("var_pop", "total_amount", "samplePopulationVariance_total_amount");
                    _query.RelationAggregate("CustomerOrderList", "CustomerOrder", "customer", alias, child.GetQuery(), true);
                    return this;
                }
                public CustomerRequest FacetByCommercePlatformAs(
                    string name, CommercePlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "commerce_platform", request.GetQuery(), includeAllFacets));
                    return this;
                }


        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var req = new QueryRequest(_query);
            var result = await service.QueryAsync(req);
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
                var membershipRows = (await service.QueryAsync(new QueryRequest(membership))).Rows;
                var counts = membershipRows
                    .Where(row => row.TryGetValue(facet.RelationName, out var value) && value.Raw != null)
                    .GroupBy(row => Convert.ToString(row[facet.RelationName].Raw)!)
                    .ToDictionary(group => group.Key, group => group.Count());

                var nested = facet.Query.Copy();
                nested.Facets.Clear();
                var countAliases = nested.Aggregates
                    .Where(aggregate => aggregate.Function == AggregateFunction.Count)
                    .Select(aggregate => aggregate.Alias).ToArray();
                nested.Aggregates.Clear();
                nested.GroupFields.Clear();
                var facetRows = (await service.QueryAsync(new QueryRequest(nested))).Rows;
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

        private async Task<CustomerPage> ExecuteForPageInternalAsync(
            UserContext context, int offset, int limit)
        {
            EnsureIntent();
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (limit is < 1 or > 10_000) throw new ArgumentOutOfRangeException(nameof(limit));
            var service = context.RequireResource<IDataService>();
            _query.Offset(offset);
            _query.Limit(limit);
            var result = await service.QueryAsync(new QueryRequest(_query));
            long totalCount;
            if (_query.IdSetPagination != null && context.IdSetCountAccuracy == "EXACT")
            {
                totalCount = checked((long)context.IdSetCount);
            }
            else
            {
                var countQuery = new SelectQuery("Customer");
                foreach (var filter in _query.Filters) countQuery.Filters.Add(filter);
                countQuery.Aggregate("Count", "id", "count");
                var countResult = await service.QueryAsync(new QueryRequest(countQuery));
                totalCount = countResult.Rows.Count == 0
                    ? 0L : Convert.ToInt64(countResult.Rows[0]["count"].Raw);
            }
            var rows = new SmartList<Generated.Models.Customer>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                rows.Add(Generated.Models.Customer.FromRecord(row, queryRoot));
            return new CustomerPage(rows, totalCount);
        }

        private async IAsyncEnumerable<Generated.Models.Customer> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            await foreach (var chunk in streaming.QueryStreamAsync(
                new QueryRequest(_query), chunkSize, cancellationToken).WithCancellation(cancellationToken))
            {
                var queryRoot = new EntityRoot();
                foreach (var row in chunk.Rows)
                    yield return Generated.Models.Customer.FromRecord(row, queryRoot);
            }
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutableCustomerRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, int, int, Task<CustomerPage>> _executeForPage;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Customer>> _executeForStream;
        private readonly Action _limitOne;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCustomerRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, int, int, Task<CustomerPage>> executeForPage,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Customer>> executeForStream,
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

        public ExecutableCustomerRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.Customer NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.Customer();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.Customer>> ExecuteForListAsync(UserContext context)
        {
            var result = await ExecuteForRowsAsync(context);
            var entities = new SmartList<Generated.Models.Customer>();
            var queryRoot = new EntityRoot();
            foreach (var row in result.Rows)
                entities.Add(Generated.Models.Customer.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public Task<CustomerPage> ExecuteForPageAsync(
            UserContext context, int offset, int limit)
            => _executeForPage(context, offset, limit);

        public IAsyncEnumerable<Generated.Models.Customer> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.Customer?> ExecuteForOneAsync(
            UserContext context)
        {
            _limitOne();
            var entities = await ExecuteForListAsync(context);
            return entities.Count > 0 ? entities[0] : null;
        }
    }

    public sealed class CustomerPage
    {
        public SmartList<Generated.Models.Customer> Rows { get; }
        public long TotalCount { get; }
        public CustomerPage(
            SmartList<Generated.Models.Customer> rows, long totalCount)
        { Rows = rows; TotalCount = totalCount; }
    }
}