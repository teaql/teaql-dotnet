using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Sql;

/// <summary>Facet membership and materialization are derived work of the selecting request.</summary>
internal static class FacetQueryLoader
{
    private const string CountAlias = "__teaql_facet_count";

    internal static async Task<Dictionary<string, SmartList<Record>>> LoadAsync(
        ISchemaProvider schema, Func<QueryRequest, Task<QueryResult>> queryAsync,
        QueryRequest request, CompiledQuery compiled)
    {
        var facets = new Dictionary<string, SmartList<Record>>();
        if (request.Query.Facets.Count == 0) return facets;
        var source = schema.GetEntity(request.Query.Entity)
            ?? throw new SqlExecutorException($"SQL compile error: unknown entity {request.Query.Entity}");
        var inheritedIntent = SqlStatementDiagnostics.InheritedIntent(request, compiled);
        foreach (var facet in request.Query.Facets)
        {
            var parent = facet.Query.RequestScope?.Parent ?? request.Scope.Facet(facet.RelationName, facet.Name, facet.Operation);
            var relationLabel = parent.Relation ?? facet.RelationName;
            var aliases = facet.Query.AggregateItems.Where(item => item.Function == AggregateFunction.Count)
                .Select(item => item.Alias).Distinct().ToArray();
            var countAlias = aliases.FirstOrDefault() ?? CountAlias;
            var relation = source.RelationByName(facet.RelationName)
                ?? throw new SqlExecutorException($"SQL compile error: missing relation {source.Name}.{facet.RelationName}");
            var target = schema.GetEntity(relation.TargetEntity)
                ?? throw new SqlExecutorException($"SQL compile error: unknown entity {relation.TargetEntity}");

            // Counts describe the authorized membership, not the root's
            // current page. Do not recursively execute enhancements while counting.
            var membership = request.Query.CloneForExecution();
            membership.Projection.Clear(); membership.ExprProjection.Clear();
            membership.RawProjections.Clear(); membership.DynamicProperties.Clear();
            membership.OrderByItems.Clear(); membership.AggregateItems.Clear(); membership.GroupByItems.Clear();
            membership.RelationLoads.Clear(); membership.RelationAggregates.Clear(); membership.Facets.Clear();
            membership.ChildEnhancements.Clear(); membership.ObjectGroupBys.Clear();
            membership.Slice = null; membership.PartitionBy = null; membership.IdSetPagination = null;
            membership.Project(relation.LocalKeyValue).GroupBy(relation.LocalKeyValue).Count(countAlias);
            foreach (var alias in aliases.Skip(1)) membership.Count(alias);
            var countRequest = request.DeriveAt(membership, parent.Helper(membership.Entity, $"aggregate {relationLabel}"), facet.RelationName);
            countRequest.IntentSource = inheritedIntent;
            var countResult = await queryAsync(countRequest);
            var localColumn = source.PropertyByName(relation.LocalKeyValue)?.ColumnNameString ?? relation.LocalKeyValue;
            var counts = new Dictionary<Value, long>();
            foreach (var row in countResult.Rows)
                if ((row.TryGetValue(relation.LocalKeyValue, out var key) || row.TryGetValue(localColumn, out key))
                    && key is not Value.NullValue and not Value.TypedNullValue)
                    counts[key] = row[countAlias].TryI64() ?? 0;

            var materialization = facet.Query.CloneForExecution();
            materialization.Entity = relation.TargetEntity;
            if (aliases.Length == 0) aliases = new[] { RequestConstants.COUNT_ALIAS };
            materialization.AggregateItems.Clear(); materialization.GroupByItems.Clear();
            // A reverse facet sum covers the complete authorized outer
            // membership, not only the page and not unrelated rows in that status.
            materialization.RelationAggregates = materialization.RelationAggregates.Select(aggregate => {
                if (schema.GetEntity(materialization.Entity)?.RelationByName(aggregate.RelationName)?.TargetEntity != source.Name) return aggregate;
                var ids = membership.CloneForExecution();
                ids.AggregateItems.Clear(); ids.GroupByItems.Clear(); ids.Projection.Clear(); ids.Slice = null;
                var child = aggregate.Query.CloneForExecution();
                child.AndFilter(Expr.InSubquery("id", source, ids, "id"));
                return aggregate with { Query = child };
            }).ToList();
            if (materialization.Projection.Count > 0 && !materialization.Projection.Contains(relation.ForeignKeyValue))
                materialization.Projection.Add(relation.ForeignKeyValue);
            if (!facet.IncludeAllFacets)
                // Keep requested nested Facets even when this candidate list is
                // empty; an explicit false membership also excludes unrelated rows.
                materialization.AndFilter(counts.Count == 0 ? Expr.Value(new Value.BoolValue(false))
                    : Expr.InList(relation.ForeignKeyValue, counts.Keys.ToList()));
            var derived = request.Derive(materialization, facet.RelationName, facet.Operation);
            derived.IntentSource = inheritedIntent;
            var result = await queryAsync(derived);
            var foreignColumn = target.PropertyByName(relation.ForeignKeyValue)?.ColumnNameString ?? relation.ForeignKeyValue;
            foreach (var row in result.Rows)
            {
                var count = (row.TryGetValue(relation.ForeignKeyValue, out var key) || row.TryGetValue(foreignColumn, out key))
                    && counts.TryGetValue(key, out var found) ? found : 0;
                foreach (var alias in aliases) row[alias] = new Value.I64Value(count);
            }
            facets[facet.Name] = new SmartList<Record>(result.Rows) { Facets = result.Facets };
        }
        return facets;
    }
}
