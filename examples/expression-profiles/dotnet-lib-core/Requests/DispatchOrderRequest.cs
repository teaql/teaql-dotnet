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
    public class DispatchOrderRequest
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

        public DispatchOrderRequest()
        {
            _query = new SelectQuery("DispatchOrder");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public DispatchOrderRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public DispatchOrderRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public DispatchOrderRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public DispatchOrderRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableDispatchOrderRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableDispatchOrderRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public DispatchOrderRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public DispatchOrderRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public DispatchOrderRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public DispatchOrderRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public DispatchOrderRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public DispatchOrderRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public DispatchOrderRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public DispatchOrderRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public DispatchOrderRequest SelectId()
                {
                    _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public DispatchOrderRequest SelectOrderReference()
                {
                    _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY);
                    return this;
                }

                public DispatchOrderRequest SelectHandlingNote()
                {
                    _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY);
                    return this;
                }

                public DispatchOrderRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public DispatchOrderRequest SelectDistributionCenter()
                {
                    return SelectDistributionCenterWith(new DistributionCenterRequest());
                }

                public DispatchOrderRequest SelectDistributionCenterWith(DistributionCenterRequest related)
                {
                    _query.Project(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER, "DistributionCenter", global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectDistributionCenterWith", Relation: global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public DispatchOrderRequest WithDistributionCenterMatching(DistributionCenterRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("DistributionCenter")!, related.GetQuery(), "id"));
                    return this;
                }

                public DispatchOrderRequest WithoutDistributionCenterMatching(DistributionCenterRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("DistributionCenter")!, related.GetQuery(), "id"));
                    return this;
                }

                public DispatchOrderRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DispatchOrderRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DispatchOrderRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public DispatchOrderRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public DispatchOrderRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public DispatchOrderRequest FilterByDistributionCenter(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest FilterByDistributionCenterIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY, vals));
                    return this;
                }

                public DispatchOrderRequest WithDistributionCenterIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY));
                    return this;
                }

                public DispatchOrderRequest WithDistributionCenterIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public DispatchOrderRequest WithOrderReferenceIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY));
                    return this;
                }
                public DispatchOrderRequest WithOrderReferenceNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithOrderReferenceSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public DispatchOrderRequest WithHandlingNoteIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY));
                    return this;
                }
                public DispatchOrderRequest WithHandlingNoteNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithHandlingNoteSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, val));
                    return this;
                }

                public DispatchOrderRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DispatchOrderRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public DispatchOrderRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public DispatchOrderRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public DispatchOrderRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public DispatchOrderRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public DispatchOrderRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public DispatchOrderRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public DispatchOrderRequest OrderByOrderReferenceAscending()
                {
                    _query.OrderBy(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public DispatchOrderRequest OrderByOrderReferenceDescending()
                {
                    _query.OrderBy(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public DispatchOrderRequest OrderByHandlingNoteAscending()
                {
                    _query.OrderBy(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public DispatchOrderRequest OrderByHandlingNoteDescending()
                {
                    _query.OrderBy(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public DispatchOrderRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public DispatchOrderRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public DispatchOrderRequest Count()
        {
            return CountAs("count");
        }

        public DispatchOrderRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public DispatchOrderRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public DispatchOrderRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public DispatchOrderRequest GroupByDistributionCenter()
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY);
                    return this;
                }

                public DispatchOrderRequest GroupByDistributionCenterAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY);
                    return this;
                }
                public DispatchOrderRequest GroupByOrderReference()
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY);
                    return this;
                }

                public DispatchOrderRequest GroupByOrderReferenceAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.ORDER_REFERENCE_PROPERTY_QUERY);
                    return this;
                }
                public DispatchOrderRequest GroupByHandlingNote()
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY);
                    return this;
                }

                public DispatchOrderRequest GroupByHandlingNoteAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.HANDLING_NOTE_PROPERTY_QUERY);
                    return this;
                }
                public DispatchOrderRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public DispatchOrderRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.DispatchOrder.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public DispatchOrderRequest FacetByDistributionCenterAs(
                    string name, DistributionCenterRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByDistributionCenterAs", name, global::Generated.Models.DispatchOrder.__TeaqlFields.DISTRIBUTION_CENTER_PROPERTY_QUERY)
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

        private async Task<Generated.Models.DispatchOrder?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.DispatchOrder>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.DispatchOrder>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.DispatchOrder>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.DispatchOrder.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.DispatchOrder> ExecuteForStreamInternalAsync(
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
                    Generated.Models.DispatchOrder.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableDispatchOrderRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.DispatchOrder?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.DispatchOrder>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.DispatchOrder>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableDispatchOrderRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.DispatchOrder?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.DispatchOrder>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.DispatchOrder>> executeForStream,
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

        public ExecutableDispatchOrderRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.DispatchOrder> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("DispatchOrder").ConfigureAwait(false));
            return context.InitializeEntity("DispatchOrder", Generated.Models.DispatchOrder.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.DispatchOrder>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.DispatchOrder> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.DispatchOrder?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}