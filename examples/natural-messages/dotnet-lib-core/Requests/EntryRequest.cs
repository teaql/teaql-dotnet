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
    public class EntryRequest
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

        public EntryRequest()
        {
            _query = new SelectQuery("Entry");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public EntryRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public EntryRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public EntryRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public EntryRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableEntryRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableEntryRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public EntryRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public EntryRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public EntryRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public EntryRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public EntryRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public EntryRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public EntryRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public EntryRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public EntryRequest SelectId()
                {
                    _query.Project(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public EntryRequest SelectName()
                {
                    _query.Project(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest SelectCreateTime()
                {
                    _query.Project(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest SelectUpdateTime()
                {
                    _query.Project(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest SelectCatalog()
                {
                    return SelectCatalogWith(new CatalogRequest());
                }

                public EntryRequest SelectCatalogWith(CatalogRequest related)
                {
                    _query.Project(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER, "Catalog", global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCatalogWith", Relation: global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public EntryRequest WithCatalogMatching(CatalogRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Catalog")!, related.GetQuery(), "id"));
                    return this;
                }

                public EntryRequest WithoutCatalogMatching(CatalogRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("Catalog")!, related.GetQuery(), "id"));
                    return this;
                }

                public EntryRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public EntryRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest FilterByCatalog(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY, val));
                    return this;
                }

                public EntryRequest FilterByCatalogIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY, vals));
                    return this;
                }

                public EntryRequest WithCatalogIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest WithCatalogIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public EntryRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public EntryRequest WithNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public EntryRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY));
                    return this;
                }
                public EntryRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public EntryRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public EntryRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public EntryRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public EntryRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public EntryRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public EntryRequest WithCreateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithCreateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithCreateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithCreateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithCreateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithCreateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithCreateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithCreateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithCreateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public EntryRequest WithCreateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest WithCreateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest WithUpdateTimeIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithUpdateTimeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithUpdateTimeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithUpdateTimeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithUpdateTimeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithUpdateTimeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithUpdateTimeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithUpdateTimeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithUpdateTimeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public EntryRequest WithUpdateTimeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest WithUpdateTimeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public EntryRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public EntryRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public EntryRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public EntryRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public EntryRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public EntryRequest OrderByNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public EntryRequest OrderByNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public EntryRequest OrderByCreateTimeAscending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public EntryRequest OrderByCreateTimeDescending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public EntryRequest OrderByUpdateTimeAscending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public EntryRequest OrderByUpdateTimeDescending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public EntryRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public EntryRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public EntryRequest Count()
        {
            return CountAs("count");
        }

        public EntryRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public EntryRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public EntryRequest GroupByCatalog()
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest GroupByCatalogAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY);
                    return this;
                }
                public EntryRequest GroupByName()
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.NAME_PROPERTY_QUERY);
                    return this;
                }
                public EntryRequest GroupByCreateTime()
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest GroupByCreateTimeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.CREATE_TIME_PROPERTY_QUERY);
                    return this;
                }
                public EntryRequest GroupByUpdateTime()
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest GroupByUpdateTimeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.UPDATE_TIME_PROPERTY_QUERY);
                    return this;
                }
                public EntryRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public EntryRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.Entry.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public EntryRequest FacetByCatalogAs(
                    string name, CatalogRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCatalogAs", name, global::Generated.Models.Entry.__TeaqlFields.CATALOG_PROPERTY_QUERY)
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

        private async Task<Generated.Models.Entry?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.Entry>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.Entry>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
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
            var entities = new SmartList<Generated.Models.Entry>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Entry.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Entry> ExecuteForStreamInternalAsync(
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
                    Generated.Models.Entry.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableEntryRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Entry?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Entry>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Entry>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableEntryRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Entry?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Entry>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Entry>> executeForStream,
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

        public ExecutableEntryRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.Entry> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Entry").ConfigureAwait(false));
            return context.InitializeEntity("Entry", Generated.Models.Entry.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Entry>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Entry> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Entry?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}