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
    public class PurchaseOrderRequest
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

        public PurchaseOrderRequest()
        {
            _query = new SelectQuery("PurchaseOrder");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public PurchaseOrderRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public PurchaseOrderRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public PurchaseOrderRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public PurchaseOrderRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutablePurchaseOrderRequest Purpose(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                throw new ArgumentException("query purpose must not be empty", nameof(p));
            _query.Purpose(p);
            _purpose = p;
            return new ExecutablePurchaseOrderRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public PurchaseOrderRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public PurchaseOrderRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public PurchaseOrderRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public PurchaseOrderRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public PurchaseOrderRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public PurchaseOrderRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public PurchaseOrderRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public PurchaseOrderRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("tenant");
            _query.Project("contact_phone");
            _query.Project("internal_note");
            _query.Project("create_time");
            _query.Project("update_time");
            _query.Project("version");
            return this;
        }

                public PurchaseOrderRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public PurchaseOrderRequest SelectContactPhone()
                {
                    _query.Project("contact_phone");
                    return this;
                }

                public PurchaseOrderRequest SelectInternalNote()
                {
                    _query.Project("internal_note");
                    return this;
                }

                public PurchaseOrderRequest SelectCreateTime()
                {
                    _query.Project("create_time");
                    return this;
                }

                public PurchaseOrderRequest SelectUpdateTime()
                {
                    _query.Project("update_time");
                    return this;
                }

                public PurchaseOrderRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public PurchaseOrderRequest SelectTenant()
                {
                    return SelectTenantWith(new TenantRequest());
                }

                public PurchaseOrderRequest SelectTenantWith(TenantRequest related)
                {
                    _query.Project("tenant");
                    _query.ForwardRelationQuery("Tenant", "Tenant", "tenant", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTenantWith", Relation: "Tenant")
                    };
                    return this;
                }
                public PurchaseOrderRequest WithTenantMatching(TenantRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("tenant", GeneratedRuntimeModule.Module.Metadata.GetEntity("Tenant")!, related.GetQuery(), "id"));
                    return this;
                }

                public PurchaseOrderRequest WithoutTenantMatching(TenantRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("tenant", GeneratedRuntimeModule.Module.Metadata.GetEntity("Tenant")!, related.GetQuery(), "id"));
                    return this;
                }

                public PurchaseOrderRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public PurchaseOrderRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public PurchaseOrderRequest FilterByTenant(object val)
                {
                    _query.AndFilter(Expr.Eq("tenant", val));
                    return this;
                }

                public PurchaseOrderRequest FilterByTenantIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("tenant", vals));
                    return this;
                }

                public PurchaseOrderRequest WithTenantIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("tenant"));
                    return this;
                }

                public PurchaseOrderRequest WithTenantIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("tenant"));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("contact_phone", val));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneIs(string val)
                {
                    _query.AndFilter(Expr.Eq("contact_phone", TeaqlQueryValue(val)));
                    return this;
                }
                public PurchaseOrderRequest WithContactPhoneIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("contact_phone", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("contact_phone", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("contact_phone", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("contact_phone", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("contact_phone", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("contact_phone", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("contact_phone", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("contact_phone", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("contact_phone"));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("contact_phone"));
                    return this;
                }
                public PurchaseOrderRequest WithContactPhoneNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("contact_phone", val));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("contact_phone", val));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("contact_phone", val));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("contact_phone", val));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("contact_phone", val));
                    return this;
                }

                public PurchaseOrderRequest WithContactPhoneSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("contact_phone", val));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("internal_note", val));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteIs(string val)
                {
                    _query.AndFilter(Expr.Eq("internal_note", TeaqlQueryValue(val)));
                    return this;
                }
                public PurchaseOrderRequest WithInternalNoteIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("internal_note", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("internal_note", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("internal_note", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("internal_note", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("internal_note", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("internal_note", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("internal_note", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("internal_note", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("internal_note"));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("internal_note"));
                    return this;
                }
                public PurchaseOrderRequest WithInternalNoteNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("internal_note", val));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("internal_note", val));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("internal_note", val));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("internal_note", val));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("internal_note", val));
                    return this;
                }

                public PurchaseOrderRequest WithInternalNoteSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("internal_note", val));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("create_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("create_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("create_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("create_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("create_time"));
                    return this;
                }

                public PurchaseOrderRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("create_time"));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("update_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("update_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("update_time", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("update_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("update_time"));
                    return this;
                }

                public PurchaseOrderRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("update_time"));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public PurchaseOrderRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public PurchaseOrderRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public PurchaseOrderRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public PurchaseOrderRequest OrderByContactPhoneAscending()
                {
                    _query.OrderBy("contact_phone", "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByContactPhoneDescending()
                {
                    _query.OrderBy("contact_phone", "desc");
                    return this;
                }

                public PurchaseOrderRequest OrderByInternalNoteAscending()
                {
                    _query.OrderBy("internal_note", "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByInternalNoteDescending()
                {
                    _query.OrderBy("internal_note", "desc");
                    return this;
                }

                public PurchaseOrderRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy("create_time", "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy("create_time", "desc");
                    return this;
                }

                public PurchaseOrderRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy("update_time", "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy("update_time", "desc");
                    return this;
                }

                public PurchaseOrderRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public PurchaseOrderRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public PurchaseOrderRequest Count()
        {
            return CountAs("count");
        }

        public PurchaseOrderRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public PurchaseOrderRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public PurchaseOrderRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public PurchaseOrderRequest GroupByTenant()
                {
                    _query.GroupBy("tenant");
                    return this;
                }

                public PurchaseOrderRequest GroupByTenantAs(string retName)
                {
                    _query.GroupBy("tenant"); 
                    return this;
                }
                public PurchaseOrderRequest GroupByContactPhone()
                {
                    _query.GroupBy("contact_phone");
                    return this;
                }

                public PurchaseOrderRequest GroupByContactPhoneAs(string retName)
                {
                    _query.GroupBy("contact_phone"); 
                    return this;
                }
                public PurchaseOrderRequest GroupByInternalNote()
                {
                    _query.GroupBy("internal_note");
                    return this;
                }

                public PurchaseOrderRequest GroupByInternalNoteAs(string retName)
                {
                    _query.GroupBy("internal_note"); 
                    return this;
                }
                public PurchaseOrderRequest GroupByCreateTime()
                {
                    _query.GroupBy("create_time");
                    return this;
                }

                public PurchaseOrderRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy("create_time"); 
                    return this;
                }
                public PurchaseOrderRequest GroupByUpdateTime()
                {
                    _query.GroupBy("update_time");
                    return this;
                }

                public PurchaseOrderRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy("update_time"); 
                    return this;
                }
                public PurchaseOrderRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public PurchaseOrderRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public PurchaseOrderRequest SelectOrderItemList()
                {
                    return SelectOrderItemListWith(new OrderItemRequest());
                }

                public PurchaseOrderRequest SelectOrderItemListWith(OrderItemRequest child)
                {
                    _query.RelationQuery("OrderItemList", "OrderItem", "purchase_order", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectOrderItemListWith", Relation: "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest HaveOrderItems()
                    => WithOrderItemListMatching(new OrderItemRequest());

                public PurchaseOrderRequest HaveNoOrderItems()
                    => WithoutOrderItemListMatching(new OrderItemRequest());

                public PurchaseOrderRequest WithOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "purchase_order"));
                    return this;
                }

                public PurchaseOrderRequest WithoutOrderItemListMatching(OrderItemRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("OrderItem")!, child.GetQuery(), "purchase_order"));
                    return this;
                }
                public PurchaseOrderRequest CountOrderItems()
                    => CountOrderItemsAs("countOrderItems");

                public PurchaseOrderRequest CountOrderItemsAs(string alias)
                {
                    CountOrderItemsWith(alias, new OrderItemRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }

                public PurchaseOrderRequest CountOrderItemsWith(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountOrderItemsWith", alias, "OrderItemList")
                    };
                    return this;
                }

                public PurchaseOrderRequest MinUnitPriceOfOrderItems()
                    => MinUnitPriceOfOrderItemsAs("minOfUnitPriceOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest MinUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest MaxUnitPriceOfOrderItems()
                    => MaxUnitPriceOfOrderItemsAs("maxOfUnitPriceOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest MaxUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest SumUnitPriceOfOrderItems()
                    => SumUnitPriceOfOrderItemsAs("sumOfUnitPriceOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest SumUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest AvgUnitPriceOfOrderItems()
                    => AvgUnitPriceOfOrderItemsAs("avgOfUnitPriceOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest AvgUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest StandardDeviationUnitPriceOfOrderItems()
                    => StandardDeviationUnitPriceOfOrderItemsAs("standardDeviationOfUnitPriceOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest StandardDeviationUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItems()
                    => SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItemsAs("squareRootOfPopulationStandardDeviationOfUnitPriceOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest SampleVarianceUnitPriceOfOrderItems()
                    => SampleVarianceUnitPriceOfOrderItemsAs("sampleVarianceOfUnitPriceOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest SampleVarianceUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest SamplePopulationVarianceUnitPriceOfOrderItems()
                    => SamplePopulationVarianceUnitPriceOfOrderItemsAs("samplePopulationVarianceOfUnitPriceOfOrderItems", new OrderItemRequest());

                public PurchaseOrderRequest SamplePopulationVarianceUnitPriceOfOrderItemsAs(string alias, OrderItemRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "unit_price", alias);
                    _query.RelationAggregate("OrderItemList", "OrderItem", "purchase_order", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceUnitPriceOfOrderItemsAs", alias, "OrderItemList")
                    };
                    return this;
                }
                public PurchaseOrderRequest FacetByTenantAs(
                    string name, TenantRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Tenant", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByTenantAs", name, "tenant")
                    });
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

        private IAsyncEnumerable<Generated.Models.PurchaseOrder> ExecuteForStreamInternalAsync(
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
                row => Generated.Models.PurchaseOrder.FromRecord(row, new EntityRoot()),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            if (string.IsNullOrWhiteSpace(_purpose) || string.IsNullOrWhiteSpace(_comment))
                throw new Exception("Security audit failure: Comment() and Purpose() must be non-empty before execution or NewEntity()");
        }

    }

    public sealed class ExecutablePurchaseOrderRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<QueryResult>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Record>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.PurchaseOrder>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePurchaseOrderRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<QueryResult>> executeForOne,
            Func<UserContext, Task<SmartList<Record>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.PurchaseOrder>> executeForStream,
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

        public ExecutablePurchaseOrderRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public Generated.Models.PurchaseOrder NewEntity(UserContext context)
        {
            _ensureIntent();
            return new Generated.Models.PurchaseOrder();
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public async Task<SmartList<Generated.Models.PurchaseOrder>> ExecuteForListAsync(UserContext context)
        {
            var result = await _executeForList(context);
            var entities = new SmartList<Generated.Models.PurchaseOrder>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            var queryRoot = new EntityRoot();
            foreach (var row in result)
                entities.Add(Generated.Models.PurchaseOrder.FromRecord(row, queryRoot));
            entities.Facets = result.Facets;
            return entities;
        }

        public IAsyncEnumerable<Generated.Models.PurchaseOrder> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public async Task<Generated.Models.PurchaseOrder?> ExecuteForOneAsync(
            UserContext context)
        {
            var result = await _executeForOne(context);
            if (result.Rows.Count == 0) return null;
            return Generated.Models.PurchaseOrder.FromRecord(
                result.Rows[0], new EntityRoot());
        }
    }

}