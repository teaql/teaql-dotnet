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
    public class CustomerOrderRequest
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

        public CustomerOrderRequest()
        {
            _query = new SelectQuery("CustomerOrder");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public CustomerOrderRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public CustomerOrderRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public CustomerOrderRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public CustomerOrderRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableCustomerOrderRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableCustomerOrderRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public CustomerOrderRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public CustomerOrderRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public CustomerOrderRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public CustomerOrderRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public CustomerOrderRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public CustomerOrderRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public CustomerOrderRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public CustomerOrderRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public CustomerOrderRequest SelectId()
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }



                public CustomerOrderRequest SelectOrderNumber()
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest SelectFulfillmentStatus()
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest SelectMerchant()
                {
                    return SelectMerchantWith(new MerchantRequest());
                }

                public CustomerOrderRequest SelectMerchantWith(MerchantRequest related)
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_MEMBER, "Merchant", global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectMerchantWith", Relation: global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public CustomerOrderRequest SelectCustomerProfile()
                {
                    return SelectCustomerProfileWith(new CustomerProfileRequest());
                }

                public CustomerOrderRequest SelectCustomerProfileWith(CustomerProfileRequest related)
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_MEMBER, "CustomerProfile", global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerProfileWith", Relation: global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public CustomerOrderRequest WithMerchantMatching(MerchantRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Merchant")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithoutMerchantMatching(MerchantRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Merchant")!, related.GetQuery(), "id"));
                    return this;
                }
                public CustomerOrderRequest WithCustomerProfileMatching(CustomerProfileRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerProfile")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithoutCustomerProfileMatching(CustomerProfileRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerProfile")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest FilterByMerchant(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest FilterByMerchantIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY, vals));
                    return this;
                }

                public CustomerOrderRequest WithMerchantIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithMerchantIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest FilterByCustomerProfile(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest FilterByCustomerProfileIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY, vals));
                    return this;
                }

                public CustomerOrderRequest WithCustomerProfileIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithCustomerProfileIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public CustomerOrderRequest WithOrderNumberIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY));
                    return this;
                }
                public CustomerOrderRequest WithOrderNumberNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithOrderNumberSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public CustomerOrderRequest WithFulfillmentStatusIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY));
                    return this;
                }
                public CustomerOrderRequest WithFulfillmentStatusNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithFulfillmentStatusSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }



                public CustomerOrderRequest OrderByOrderNumberAscending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByOrderNumberDescending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY, "desc");
                    return this;
                }

                public CustomerOrderRequest OrderByFulfillmentStatusAscending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByFulfillmentStatusDescending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY, "desc");
                    return this;
                }

                public CustomerOrderRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public CustomerOrderRequest Count()
        {
            return CountAs("count");
        }

        public CustomerOrderRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public CustomerOrderRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public CustomerOrderRequest GroupByMerchant()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByMerchantAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY);
                    return this;
                }
                public CustomerOrderRequest GroupByCustomerProfile()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByCustomerProfileAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY);
                    return this;
                }
                public CustomerOrderRequest GroupByOrderNumber()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByOrderNumberAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY);
                    return this;
                }
                public CustomerOrderRequest GroupByFulfillmentStatus()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByFulfillmentStatusAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.FULFILLMENT_STATUS_PROPERTY_QUERY);
                    return this;
                }
                public CustomerOrderRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public CustomerOrderRequest FacetByMerchantAs(
                    string name, MerchantRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByMerchantAs", name, global::Generated.Models.CustomerOrder.__TeaqlFields.MERCHANT_PROPERTY_QUERY)
                    });
                    return this;
                }

                public CustomerOrderRequest FacetByCustomerProfileAs(
                    string name, CustomerProfileRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerProfileAs", name, global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROFILE_PROPERTY_QUERY)
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

        private async Task<Generated.Models.CustomerOrder?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.CustomerOrder>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.CustomerOrder>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.CustomerOrder>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.CustomerOrder.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.CustomerOrder> ExecuteForStreamInternalAsync(
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
                    Generated.Models.CustomerOrder.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableCustomerOrderRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.CustomerOrder?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.CustomerOrder>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerOrder>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableCustomerOrderRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.CustomerOrder?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.CustomerOrder>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.CustomerOrder>> executeForStream,
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

        public ExecutableCustomerOrderRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.CustomerOrder> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("CustomerOrder").ConfigureAwait(false));
            return context.InitializeEntity("CustomerOrder", Generated.Models.CustomerOrder.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.CustomerOrder>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.CustomerOrder> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.CustomerOrder?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}