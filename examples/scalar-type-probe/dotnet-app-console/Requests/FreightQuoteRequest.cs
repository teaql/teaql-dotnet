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
    public class FreightQuoteRequest
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

        public FreightQuoteRequest()
        {
            _query = new SelectQuery("FreightQuote");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public FreightQuoteRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public FreightQuoteRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public FreightQuoteRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public FreightQuoteRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableFreightQuoteRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableFreightQuoteRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public FreightQuoteRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public FreightQuoteRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public FreightQuoteRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public FreightQuoteRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public FreightQuoteRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public FreightQuoteRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public FreightQuoteRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public FreightQuoteRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("platform");
            _query.Project("reference");
            _query.Project("amount");
            _query.Project("shipment_number");
            _query.Project("package_count");
            _query.Project("delivery_date");
            _query.Project("pickup_time");
            _query.Project("quoted_at");
            _query.Project("instructions");
            _query.Project("version");
            return this;
        }

                public FreightQuoteRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }


                public FreightQuoteRequest SelectReference()
                {
                    _query.Project("reference");
                    return this;
                }

                public FreightQuoteRequest SelectAmount()
                {
                    _query.Project("amount");
                    return this;
                }

                public FreightQuoteRequest SelectShipmentNumber()
                {
                    _query.Project("shipment_number");
                    return this;
                }

                public FreightQuoteRequest SelectPackageCount()
                {
                    _query.Project("package_count");
                    return this;
                }

                public FreightQuoteRequest SelectDeliveryDate()
                {
                    _query.Project("delivery_date");
                    return this;
                }

                public FreightQuoteRequest SelectPickupTime()
                {
                    _query.Project("pickup_time");
                    return this;
                }

                public FreightQuoteRequest SelectQuotedAt()
                {
                    _query.Project("quoted_at");
                    return this;
                }

                public FreightQuoteRequest SelectInstructions()
                {
                    _query.Project("instructions");
                    return this;
                }

                public FreightQuoteRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }

                public FreightQuoteRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public FreightQuoteRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project("platform");
                    _query.ForwardRelationQuery("Platform", "Platform", "platform", related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: "Platform")
                    };
                    return this;
                }
                public FreightQuoteRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public FreightQuoteRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery("platform", GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public FreightQuoteRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public FreightQuoteRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public FreightQuoteRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq("platform", val));
                    return this;
                }

                public FreightQuoteRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("platform", vals));
                    return this;
                }

                public FreightQuoteRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("platform"));
                    return this;
                }

                public FreightQuoteRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("platform"));
                    return this;
                }

                public FreightQuoteRequest WithReferenceContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("reference", val));
                    return this;
                }

                public FreightQuoteRequest WithReferenceIs(string val)
                {
                    _query.AndFilter(Expr.Eq("reference", TeaqlQueryValue(val)));
                    return this;
                }
                public FreightQuoteRequest WithReferenceIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithReferenceIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("reference", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithReferenceNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("reference", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithReferenceGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithReferenceGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithReferenceLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithReferenceLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("reference", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithReferenceBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("reference", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithReferenceIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("reference"));
                    return this;
                }

                public FreightQuoteRequest WithReferenceIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("reference"));
                    return this;
                }
                public FreightQuoteRequest WithReferenceNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("reference", val));
                    return this;
                }

                public FreightQuoteRequest WithReferenceStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("reference", val));
                    return this;
                }

                public FreightQuoteRequest WithReferenceNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("reference", val));
                    return this;
                }

                public FreightQuoteRequest WithReferenceEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("reference", val));
                    return this;
                }

                public FreightQuoteRequest WithReferenceNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("reference", val));
                    return this;
                }

                public FreightQuoteRequest WithReferenceSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("reference", val));
                    return this;
                }

                public FreightQuoteRequest WithAmountIs(object val)
                {
                    _query.AndFilter(Expr.Eq("amount", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithAmountIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("amount", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithAmountIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("amount", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithAmountNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("amount", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithAmountGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("amount", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithAmountGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("amount", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithAmountLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("amount", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithAmountLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("amount", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithAmountBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("amount", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithAmountIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("amount"));
                    return this;
                }

                public FreightQuoteRequest WithAmountIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("amount"));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberIs(object val)
                {
                    _query.AndFilter(Expr.Eq("shipment_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("shipment_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("shipment_number", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("shipment_number", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("shipment_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("shipment_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("shipment_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("shipment_number", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("shipment_number", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("shipment_number"));
                    return this;
                }

                public FreightQuoteRequest WithShipmentNumberIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("shipment_number"));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountIs(object val)
                {
                    _query.AndFilter(Expr.Eq("package_count", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("package_count", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("package_count", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("package_count", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("package_count", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("package_count", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("package_count", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("package_count", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("package_count", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("package_count"));
                    return this;
                }

                public FreightQuoteRequest WithPackageCountIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("package_count"));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateIs(object val)
                {
                    _query.AndFilter(Expr.Eq("delivery_date", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("delivery_date", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("delivery_date", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("delivery_date", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("delivery_date", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("delivery_date", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("delivery_date", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("delivery_date", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("delivery_date", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("delivery_date"));
                    return this;
                }

                public FreightQuoteRequest WithDeliveryDateIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("delivery_date"));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq("pickup_time", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("pickup_time", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("pickup_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("pickup_time", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("pickup_time", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("pickup_time", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("pickup_time", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("pickup_time", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("pickup_time", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("pickup_time"));
                    return this;
                }

                public FreightQuoteRequest WithPickupTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("pickup_time"));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtIs(object val)
                {
                    _query.AndFilter(Expr.Eq("quoted_at", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("quoted_at", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("quoted_at", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("quoted_at", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("quoted_at", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("quoted_at", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("quoted_at", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("quoted_at", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("quoted_at", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("quoted_at"));
                    return this;
                }

                public FreightQuoteRequest WithQuotedAtIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("quoted_at"));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("instructions", val));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsIs(string val)
                {
                    _query.AndFilter(Expr.Eq("instructions", TeaqlQueryValue(val)));
                    return this;
                }
                public FreightQuoteRequest WithInstructionsIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("instructions", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("instructions", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("instructions", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("instructions", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("instructions"));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("instructions"));
                    return this;
                }
                public FreightQuoteRequest WithInstructionsNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("instructions", val));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("instructions", val));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("instructions", val));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("instructions", val));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("instructions", val));
                    return this;
                }

                public FreightQuoteRequest WithInstructionsSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("instructions", val));
                    return this;
                }

                public FreightQuoteRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public FreightQuoteRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public FreightQuoteRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public FreightQuoteRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public FreightQuoteRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public FreightQuoteRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }


                public FreightQuoteRequest OrderByReferenceAscending()
                {
                    _query.OrderBy("reference", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByReferenceDescending()
                {
                    _query.OrderBy("reference", "desc");
                    return this;
                }

                public FreightQuoteRequest OrderByAmountAscending()
                {
                    _query.OrderBy("amount", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByAmountDescending()
                {
                    _query.OrderBy("amount", "desc");
                    return this;
                }

                public FreightQuoteRequest OrderByShipmentNumberAscending()
                {
                    _query.OrderBy("shipment_number", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByShipmentNumberDescending()
                {
                    _query.OrderBy("shipment_number", "desc");
                    return this;
                }

                public FreightQuoteRequest OrderByPackageCountAscending()
                {
                    _query.OrderBy("package_count", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByPackageCountDescending()
                {
                    _query.OrderBy("package_count", "desc");
                    return this;
                }

                public FreightQuoteRequest OrderByDeliveryDateAscending()
                {
                    _query.OrderBy("delivery_date", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByDeliveryDateDescending()
                {
                    _query.OrderBy("delivery_date", "desc");
                    return this;
                }

                public FreightQuoteRequest OrderByPickupTimeAscending()
                {
                    _query.OrderBy("pickup_time", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByPickupTimeDescending()
                {
                    _query.OrderBy("pickup_time", "desc");
                    return this;
                }

                public FreightQuoteRequest OrderByQuotedAtAscending()
                {
                    _query.OrderBy("quoted_at", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByQuotedAtDescending()
                {
                    _query.OrderBy("quoted_at", "desc");
                    return this;
                }

                public FreightQuoteRequest OrderByInstructionsAscending()
                {
                    _query.OrderBy("instructions", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByInstructionsDescending()
                {
                    _query.OrderBy("instructions", "desc");
                    return this;
                }

                public FreightQuoteRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public FreightQuoteRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public FreightQuoteRequest Count()
        {
            return CountAs("count");
        }

        public FreightQuoteRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public FreightQuoteRequest MinAmount()
                {
                    return MinAmountAs("minOfAmount");
                }

                public FreightQuoteRequest MinAmountAs(string retName)
                {
                    _query.Aggregate("min", "amount", retName);
                    return this;
                }
                public FreightQuoteRequest MaxAmount()
                {
                    return MaxAmountAs("maxOfAmount");
                }

                public FreightQuoteRequest MaxAmountAs(string retName)
                {
                    _query.Aggregate("max", "amount", retName);
                    return this;
                }
                public FreightQuoteRequest SumAmount()
                {
                    return SumAmountAs("sumOfAmount");
                }

                public FreightQuoteRequest SumAmountAs(string retName)
                {
                    _query.Aggregate("sum", "amount", retName);
                    return this;
                }
                public FreightQuoteRequest AvgAmount()
                {
                    return AvgAmountAs("avgOfAmount");
                }

                public FreightQuoteRequest AvgAmountAs(string retName)
                {
                    _query.Aggregate("avg", "amount", retName);
                    return this;
                }
                public FreightQuoteRequest StandardDeviationAmount()
                {
                    return StandardDeviationAmountAs("standardDeviationOfAmount");
                }

                public FreightQuoteRequest StandardDeviationAmountAs(string retName)
                {
                    _query.Aggregate("stddev", "amount", retName);
                    return this;
                }
                public FreightQuoteRequest SquareRootOfPopulationStandardDeviationAmount()
                {
                    return SquareRootOfPopulationStandardDeviationAmountAs("squareRootOfPopulationStandardDeviationOfAmount");
                }

                public FreightQuoteRequest SquareRootOfPopulationStandardDeviationAmountAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "amount", retName);
                    return this;
                }
                public FreightQuoteRequest SampleVarianceAmount()
                {
                    return SampleVarianceAmountAs("sampleVarianceOfAmount");
                }

                public FreightQuoteRequest SampleVarianceAmountAs(string retName)
                {
                    _query.Aggregate("var_samp", "amount", retName);
                    return this;
                }
                public FreightQuoteRequest SamplePopulationVarianceAmount()
                {
                    return SamplePopulationVarianceAmountAs("samplePopulationVarianceOfAmount");
                }

                public FreightQuoteRequest SamplePopulationVarianceAmountAs(string retName)
                {
                    _query.Aggregate("var_pop", "amount", retName);
                    return this;
                }
                public FreightQuoteRequest MinShipmentNumber()
                {
                    return MinShipmentNumberAs("minOfShipmentNumber");
                }

                public FreightQuoteRequest MinShipmentNumberAs(string retName)
                {
                    _query.Aggregate("min", "shipment_number", retName);
                    return this;
                }
                public FreightQuoteRequest MaxShipmentNumber()
                {
                    return MaxShipmentNumberAs("maxOfShipmentNumber");
                }

                public FreightQuoteRequest MaxShipmentNumberAs(string retName)
                {
                    _query.Aggregate("max", "shipment_number", retName);
                    return this;
                }
                public FreightQuoteRequest SumShipmentNumber()
                {
                    return SumShipmentNumberAs("sumOfShipmentNumber");
                }

                public FreightQuoteRequest SumShipmentNumberAs(string retName)
                {
                    _query.Aggregate("sum", "shipment_number", retName);
                    return this;
                }
                public FreightQuoteRequest AvgShipmentNumber()
                {
                    return AvgShipmentNumberAs("avgOfShipmentNumber");
                }

                public FreightQuoteRequest AvgShipmentNumberAs(string retName)
                {
                    _query.Aggregate("avg", "shipment_number", retName);
                    return this;
                }
                public FreightQuoteRequest StandardDeviationShipmentNumber()
                {
                    return StandardDeviationShipmentNumberAs("standardDeviationOfShipmentNumber");
                }

                public FreightQuoteRequest StandardDeviationShipmentNumberAs(string retName)
                {
                    _query.Aggregate("stddev", "shipment_number", retName);
                    return this;
                }
                public FreightQuoteRequest SquareRootOfPopulationStandardDeviationShipmentNumber()
                {
                    return SquareRootOfPopulationStandardDeviationShipmentNumberAs("squareRootOfPopulationStandardDeviationOfShipmentNumber");
                }

                public FreightQuoteRequest SquareRootOfPopulationStandardDeviationShipmentNumberAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "shipment_number", retName);
                    return this;
                }
                public FreightQuoteRequest SampleVarianceShipmentNumber()
                {
                    return SampleVarianceShipmentNumberAs("sampleVarianceOfShipmentNumber");
                }

                public FreightQuoteRequest SampleVarianceShipmentNumberAs(string retName)
                {
                    _query.Aggregate("var_samp", "shipment_number", retName);
                    return this;
                }
                public FreightQuoteRequest SamplePopulationVarianceShipmentNumber()
                {
                    return SamplePopulationVarianceShipmentNumberAs("samplePopulationVarianceOfShipmentNumber");
                }

                public FreightQuoteRequest SamplePopulationVarianceShipmentNumberAs(string retName)
                {
                    _query.Aggregate("var_pop", "shipment_number", retName);
                    return this;
                }
                public FreightQuoteRequest MinPackageCount()
                {
                    return MinPackageCountAs("minOfPackageCount");
                }

                public FreightQuoteRequest MinPackageCountAs(string retName)
                {
                    _query.Aggregate("min", "package_count", retName);
                    return this;
                }
                public FreightQuoteRequest MaxPackageCount()
                {
                    return MaxPackageCountAs("maxOfPackageCount");
                }

                public FreightQuoteRequest MaxPackageCountAs(string retName)
                {
                    _query.Aggregate("max", "package_count", retName);
                    return this;
                }
                public FreightQuoteRequest SumPackageCount()
                {
                    return SumPackageCountAs("sumOfPackageCount");
                }

                public FreightQuoteRequest SumPackageCountAs(string retName)
                {
                    _query.Aggregate("sum", "package_count", retName);
                    return this;
                }
                public FreightQuoteRequest AvgPackageCount()
                {
                    return AvgPackageCountAs("avgOfPackageCount");
                }

                public FreightQuoteRequest AvgPackageCountAs(string retName)
                {
                    _query.Aggregate("avg", "package_count", retName);
                    return this;
                }
                public FreightQuoteRequest StandardDeviationPackageCount()
                {
                    return StandardDeviationPackageCountAs("standardDeviationOfPackageCount");
                }

                public FreightQuoteRequest StandardDeviationPackageCountAs(string retName)
                {
                    _query.Aggregate("stddev", "package_count", retName);
                    return this;
                }
                public FreightQuoteRequest SquareRootOfPopulationStandardDeviationPackageCount()
                {
                    return SquareRootOfPopulationStandardDeviationPackageCountAs("squareRootOfPopulationStandardDeviationOfPackageCount");
                }

                public FreightQuoteRequest SquareRootOfPopulationStandardDeviationPackageCountAs(string retName)
                {
                    _query.Aggregate("stddev_pop", "package_count", retName);
                    return this;
                }
                public FreightQuoteRequest SampleVariancePackageCount()
                {
                    return SampleVariancePackageCountAs("sampleVarianceOfPackageCount");
                }

                public FreightQuoteRequest SampleVariancePackageCountAs(string retName)
                {
                    _query.Aggregate("var_samp", "package_count", retName);
                    return this;
                }
                public FreightQuoteRequest SamplePopulationVariancePackageCount()
                {
                    return SamplePopulationVariancePackageCountAs("samplePopulationVarianceOfPackageCount");
                }

                public FreightQuoteRequest SamplePopulationVariancePackageCountAs(string retName)
                {
                    _query.Aggregate("var_pop", "package_count", retName);
                    return this;
                }
                public FreightQuoteRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public FreightQuoteRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id"); 
                    return this;
                }
                public FreightQuoteRequest GroupByPlatform()
                {
                    _query.GroupBy("platform");
                    return this;
                }

                public FreightQuoteRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy("platform"); 
                    return this;
                }
                public FreightQuoteRequest GroupByReference()
                {
                    _query.GroupBy("reference");
                    return this;
                }

                public FreightQuoteRequest GroupByReferenceAs(string retName)
                {
                    _query.GroupBy("reference"); 
                    return this;
                }
                public FreightQuoteRequest GroupByAmount()
                {
                    _query.GroupBy("amount");
                    return this;
                }

                public FreightQuoteRequest GroupByAmountAs(string retName)
                {
                    _query.GroupBy("amount"); 
                    return this;
                }
                public FreightQuoteRequest GroupByShipmentNumber()
                {
                    _query.GroupBy("shipment_number");
                    return this;
                }

                public FreightQuoteRequest GroupByShipmentNumberAs(string retName)
                {
                    _query.GroupBy("shipment_number"); 
                    return this;
                }
                public FreightQuoteRequest GroupByPackageCount()
                {
                    _query.GroupBy("package_count");
                    return this;
                }

                public FreightQuoteRequest GroupByPackageCountAs(string retName)
                {
                    _query.GroupBy("package_count"); 
                    return this;
                }
                public FreightQuoteRequest GroupByDeliveryDate()
                {
                    _query.GroupBy("delivery_date");
                    return this;
                }

                public FreightQuoteRequest GroupByDeliveryDateAs(string retName)
                {
                    _query.GroupBy("delivery_date"); 
                    return this;
                }
                public FreightQuoteRequest GroupByPickupTime()
                {
                    _query.GroupBy("pickup_time");
                    return this;
                }

                public FreightQuoteRequest GroupByPickupTimeAs(string retName)
                {
                    _query.GroupBy("pickup_time"); 
                    return this;
                }
                public FreightQuoteRequest GroupByQuotedAt()
                {
                    _query.GroupBy("quoted_at");
                    return this;
                }

                public FreightQuoteRequest GroupByQuotedAtAs(string retName)
                {
                    _query.GroupBy("quoted_at"); 
                    return this;
                }
                public FreightQuoteRequest GroupByInstructions()
                {
                    _query.GroupBy("instructions");
                    return this;
                }

                public FreightQuoteRequest GroupByInstructionsAs(string retName)
                {
                    _query.GroupBy("instructions"); 
                    return this;
                }
                public FreightQuoteRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public FreightQuoteRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version"); 
                    return this;
                }
                public FreightQuoteRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, "Platform", request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, "platform")
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

        private async Task<Generated.Models.FreightQuote?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.FreightQuote>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.FreightQuote>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.FreightQuote>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.FreightQuote.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.FreightQuote> ExecuteForStreamInternalAsync(
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
                    Generated.Models.FreightQuote.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableFreightQuoteRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.FreightQuote?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.FreightQuote>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.FreightQuote>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableFreightQuoteRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.FreightQuote?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.FreightQuote>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.FreightQuote>> executeForStream,
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

        public ExecutableFreightQuoteRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.FreightQuote> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("FreightQuote").ConfigureAwait(false));
            return context.InitializeEntity("FreightQuote", Generated.Models.FreightQuote.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.FreightQuote>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.FreightQuote> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.FreightQuote?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}