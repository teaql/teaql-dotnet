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
    public class WorkItemRequest
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

        public WorkItemRequest()
        {
            _query = new SelectQuery("WorkItem");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public WorkItemRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public WorkItemRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public WorkItemRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public WorkItemRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableWorkItemRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableWorkItemRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public WorkItemRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public WorkItemRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public WorkItemRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public WorkItemRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public WorkItemRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public WorkItemRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public WorkItemRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public WorkItemRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public WorkItemRequest SelectId()
                {
                    _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public WorkItemRequest SelectTitle()
                {
                    _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY);
                    return this;
                }

                public WorkItemRequest SelectDescription()
                {
                    _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY);
                    return this;
                }


                public WorkItemRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public WorkItemRequest SelectPlatform()
                {
                    return SelectPlatformWith(new PlatformRequest());
                }

                public WorkItemRequest SelectPlatformWith(PlatformRequest related)
                {
                    _query.Project(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, "Platform", global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectPlatformWith", Relation: global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public WorkItemRequest WithPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public WorkItemRequest WithoutPlatformMatching(PlatformRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Platform")!, related.GetQuery(), "id"));
                    return this;
                }

                public WorkItemRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public WorkItemRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public WorkItemRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public WorkItemRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public WorkItemRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public WorkItemRequest WithTitleContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithTitleIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public WorkItemRequest WithTitleIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public WorkItemRequest WithTitleNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public WorkItemRequest WithTitleGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithTitleBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public WorkItemRequest WithTitleIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY));
                    return this;
                }

                public WorkItemRequest WithTitleIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY));
                    return this;
                }
                public WorkItemRequest WithTitleNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithTitleStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithTitleNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithTitleEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithTitleNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithTitleSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithDescriptionContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithDescriptionIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public WorkItemRequest WithDescriptionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public WorkItemRequest WithDescriptionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public WorkItemRequest WithDescriptionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithDescriptionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public WorkItemRequest WithDescriptionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY));
                    return this;
                }

                public WorkItemRequest WithDescriptionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY));
                    return this;
                }
                public WorkItemRequest WithDescriptionNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithDescriptionStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithDescriptionNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithDescriptionEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithDescriptionNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest WithDescriptionSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest FilterByPlatform(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY, val));
                    return this;
                }

                public WorkItemRequest FilterByPlatformIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY, vals));
                    return this;
                }

                public WorkItemRequest WithPlatformIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public WorkItemRequest WithPlatformIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY));
                    return this;
                }

                public WorkItemRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public WorkItemRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public WorkItemRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public WorkItemRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public WorkItemRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public WorkItemRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public WorkItemRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public WorkItemRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }

                public WorkItemRequest OrderByTitleAscending()
                {
                    _query.OrderBy(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public WorkItemRequest OrderByTitleDescending()
                {
                    _query.OrderBy(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public WorkItemRequest OrderByDescriptionAscending()
                {
                    _query.OrderBy(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public WorkItemRequest OrderByDescriptionDescending()
                {
                    _query.OrderBy(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY, "desc");
                    return this;
                }


                public WorkItemRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public WorkItemRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public WorkItemRequest Count()
        {
            return CountAs("count");
        }

        public WorkItemRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public WorkItemRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public WorkItemRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public WorkItemRequest GroupByTitle()
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY);
                    return this;
                }

                public WorkItemRequest GroupByTitleAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.TITLE_PROPERTY_QUERY);
                    return this;
                }
                public WorkItemRequest GroupByDescription()
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY);
                    return this;
                }

                public WorkItemRequest GroupByDescriptionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.DESCRIPTION_PROPERTY_QUERY);
                    return this;
                }
                public WorkItemRequest GroupByPlatform()
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }

                public WorkItemRequest GroupByPlatformAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY);
                    return this;
                }
                public WorkItemRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public WorkItemRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.WorkItem.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public WorkItemRequest FacetByPlatformAs(
                    string name, PlatformRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByPlatformAs", name, global::Generated.Models.WorkItem.__TeaqlFields.PLATFORM_PROPERTY_QUERY)
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

        private async Task<Generated.Models.WorkItem?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.WorkItem>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.WorkItem>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.WorkItem>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.WorkItem.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.WorkItem> ExecuteForStreamInternalAsync(
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
                    Generated.Models.WorkItem.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableWorkItemRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.WorkItem?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.WorkItem>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.WorkItem>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableWorkItemRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.WorkItem?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.WorkItem>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.WorkItem>> executeForStream,
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

        public ExecutableWorkItemRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.WorkItem> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("WorkItem").ConfigureAwait(false));
            return context.InitializeEntity("WorkItem", Generated.Models.WorkItem.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.WorkItem>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.WorkItem> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.WorkItem?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}