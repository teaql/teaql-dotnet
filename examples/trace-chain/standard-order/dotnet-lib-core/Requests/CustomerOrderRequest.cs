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
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_NUMBER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY);
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

                public CustomerOrderRequest SelectOrderDate()
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest SelectTotalAmount()
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest SelectCustomer()
                {
                    return SelectCustomerWith(new CustomerRequest());
                }

                public CustomerOrderRequest SelectCustomerWith(CustomerRequest related)
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER, "Customer", global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerWith", Relation: global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public CustomerOrderRequest SelectStatus()
                {
                    return SelectStatusWith(new OrderStatusRequest());
                }

                public CustomerOrderRequest SelectStatusWith(OrderStatusRequest related)
                {
                    _query.Project(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER, "OrderStatus", global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectStatusWith", Relation: global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public CustomerOrderRequest WithCustomerMatching(CustomerRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Customer")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithoutCustomerMatching(CustomerRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Customer")!, related.GetQuery(), "id"));
                    return this;
                }
                public CustomerOrderRequest WithStatusMatching(OrderStatusRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderStatus")!, related.GetQuery(), "id"));
                    return this;
                }

                public CustomerOrderRequest WithoutStatusMatching(OrderStatusRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderStatus")!, related.GetQuery(), "id"));
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

                public CustomerOrderRequest FilterByCustomer(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest FilterByCustomerIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY, vals));
                    return this;
                }

                public CustomerOrderRequest WithCustomerIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithCustomerIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest FilterByStatus(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY, val));
                    return this;
                }

                public CustomerOrderRequest FilterByStatusIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY, vals));
                    return this;
                }

                public CustomerOrderRequest WithStatusIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithStatusIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY));
                    return this;
                }
                public CustomerOrderRequest WithStatusIsNew()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY, 1001L));
                    return this;
                }

                public CustomerOrderRequest WithStatusIsPaid()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY, 1002L));
                    return this;
                }

                public CustomerOrderRequest WithStatusIsCancelled()
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY, 1003L));
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

                public CustomerOrderRequest WithOrderDateIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithOrderDateIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY));
                    return this;
                }

                public CustomerOrderRequest WithTotalAmountIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY));
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

                public CustomerOrderRequest OrderByOrderDateAscending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByOrderDateDescending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public CustomerOrderRequest OrderByTotalAmountAscending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, "asc");
                    return this;
                }

                public CustomerOrderRequest OrderByTotalAmountDescending()
                {
                    _query.OrderBy(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, "desc");
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

                public CustomerOrderRequest MinTotalAmount()
                {
                    return MinTotalAmountAs("minOfTotalAmount");
                }

                public CustomerOrderRequest MinTotalAmountAs(string retName)
                {
                    _query.Aggregate("min", global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, retName);
                    return this;
                }
                public CustomerOrderRequest MaxTotalAmount()
                {
                    return MaxTotalAmountAs("maxOfTotalAmount");
                }

                public CustomerOrderRequest MaxTotalAmountAs(string retName)
                {
                    _query.Aggregate("max", global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, retName);
                    return this;
                }
                public CustomerOrderRequest SumTotalAmount()
                {
                    return SumTotalAmountAs("sumOfTotalAmount");
                }

                public CustomerOrderRequest SumTotalAmountAs(string retName)
                {
                    _query.Aggregate("sum", global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, retName);
                    return this;
                }
                public CustomerOrderRequest AvgTotalAmount()
                {
                    return AvgTotalAmountAs("avgOfTotalAmount");
                }

                public CustomerOrderRequest AvgTotalAmountAs(string retName)
                {
                    _query.Aggregate("avg", global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, retName);
                    return this;
                }
                public CustomerOrderRequest StandardDeviationTotalAmount()
                {
                    return StandardDeviationTotalAmountAs("standardDeviationOfTotalAmount");
                }

                public CustomerOrderRequest StandardDeviationTotalAmountAs(string retName)
                {
                    _query.Aggregate("stddev", global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, retName);
                    return this;
                }
                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationTotalAmount()
                {
                    return SquareRootOfPopulationStandardDeviationTotalAmountAs("squareRootOfPopulationStandardDeviationOfTotalAmount");
                }

                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationTotalAmountAs(string retName)
                {
                    _query.Aggregate("stddev_pop", global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, retName);
                    return this;
                }
                public CustomerOrderRequest SampleVarianceTotalAmount()
                {
                    return SampleVarianceTotalAmountAs("sampleVarianceOfTotalAmount");
                }

                public CustomerOrderRequest SampleVarianceTotalAmountAs(string retName)
                {
                    _query.Aggregate("var_samp", global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, retName);
                    return this;
                }
                public CustomerOrderRequest SamplePopulationVarianceTotalAmount()
                {
                    return SamplePopulationVarianceTotalAmountAs("samplePopulationVarianceOfTotalAmount");
                }

                public CustomerOrderRequest SamplePopulationVarianceTotalAmountAs(string retName)
                {
                    _query.Aggregate("var_pop", global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY, retName);
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
                public CustomerOrderRequest GroupByCustomer()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByCustomerAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY);
                    return this;
                }
                public CustomerOrderRequest GroupByStatus()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByStatusAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY);
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
                public CustomerOrderRequest GroupByOrderDate()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByOrderDateAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.ORDER_DATE_PROPERTY_QUERY);
                    return this;
                }
                public CustomerOrderRequest GroupByTotalAmount()
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY);
                    return this;
                }

                public CustomerOrderRequest GroupByTotalAmountAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.CustomerOrder.__TeaqlFields.TOTAL_AMOUNT_PROPERTY_QUERY);
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
                public CustomerOrderRequest SelectOrderItemList()
                {
                    return SelectOrderItemListWith(new OrderItemRequest());
                }

                public CustomerOrderRequest SelectOrderItemListWith(OrderItemRequest child)
                {
                    _query.RelationQuery("OrderItemList", "OrderItem", "customer_order", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectOrderItemListWith", Relation: "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest HaveOrderItems()
                    => WithOrderItemListMatching(new OrderItemRequest());

                public CustomerOrderRequest HaveNoOrderItems()
                    => WithoutOrderItemListMatching(new OrderItemRequest());

                public CustomerOrderRequest WithOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "customer_order"));
                    return this;
                }

                public CustomerOrderRequest WithoutOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "customer_order"));
                    return this;
                }
                public CustomerOrderRequest CountOrderItems()
                    => CountOrderItemsAs("countOrderItems");

                public CustomerOrderRequest CountOrderItemsAs(string alias)
                {
                    CountOrderItemsWith(alias, new OrderItemRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }

                public CustomerOrderRequest CountOrderItemsWith(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderItemsWith", alias, "OrderItemList")
                    };
                    return this;
                }

                public CustomerOrderRequest MinQuantityOfOrderItems()
                    => MinQuantityOfOrderItemsAs("minOfQuantityOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest MinQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest MaxQuantityOfOrderItems()
                    => MaxQuantityOfOrderItemsAs("maxOfQuantityOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest MaxQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest SumQuantityOfOrderItems()
                    => SumQuantityOfOrderItemsAs("sumOfQuantityOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest SumQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest AvgQuantityOfOrderItems()
                    => AvgQuantityOfOrderItemsAs("avgOfQuantityOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest AvgQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest StandardDeviationQuantityOfOrderItems()
                    => StandardDeviationQuantityOfOrderItemsAs("standardDeviationOfQuantityOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest StandardDeviationQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderItems()
                    => SquareRootOfPopulationStandardDeviationQuantityOfOrderItemsAs("squareRootOfPopulationStandardDeviationOfQuantityOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest SampleVarianceQuantityOfOrderItems()
                    => SampleVarianceQuantityOfOrderItemsAs("sampleVarianceOfQuantityOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest SampleVarianceQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest SamplePopulationVarianceQuantityOfOrderItems()
                    => SamplePopulationVarianceQuantityOfOrderItemsAs("samplePopulationVarianceOfQuantityOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest SamplePopulationVarianceQuantityOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "quantity", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceQuantityOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest MinUnitPriceOfOrderItems()
                    => MinUnitPriceOfOrderItemsAs("minOfUnitPriceOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest MinUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest MaxUnitPriceOfOrderItems()
                    => MaxUnitPriceOfOrderItemsAs("maxOfUnitPriceOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest MaxUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest SumUnitPriceOfOrderItems()
                    => SumUnitPriceOfOrderItemsAs("sumOfUnitPriceOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest SumUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest AvgUnitPriceOfOrderItems()
                    => AvgUnitPriceOfOrderItemsAs("avgOfUnitPriceOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest AvgUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest StandardDeviationUnitPriceOfOrderItems()
                    => StandardDeviationUnitPriceOfOrderItemsAs("standardDeviationOfUnitPriceOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest StandardDeviationUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItems()
                    => SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItemsAs("squareRootOfPopulationStandardDeviationOfUnitPriceOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest SampleVarianceUnitPriceOfOrderItems()
                    => SampleVarianceUnitPriceOfOrderItemsAs("sampleVarianceOfUnitPriceOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest SampleVarianceUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest SamplePopulationVarianceUnitPriceOfOrderItems()
                    => SamplePopulationVarianceUnitPriceOfOrderItemsAs("samplePopulationVarianceOfUnitPriceOfOrderItems", new OrderItemRequest());

                public CustomerOrderRequest SamplePopulationVarianceUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "customer_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public CustomerOrderRequest FacetByCustomerAs(
                    string name, CustomerRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerAs", name, global::Generated.Models.CustomerOrder.__TeaqlFields.CUSTOMER_PROPERTY_QUERY)
                    });
                    return this;
                }

                public CustomerOrderRequest FacetByStatusAs(
                    string name, OrderStatusRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByStatusAs", name, global::Generated.Models.CustomerOrder.__TeaqlFields.STATUS_PROPERTY_QUERY)
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