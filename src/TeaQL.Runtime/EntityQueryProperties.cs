using System.Collections;
using System.Runtime.CompilerServices;
using TeaQL.Core;

namespace TeaQL.Runtime;

/// <summary>Authorize query-only output at typed hydration. No Context, provider,
/// identity allocator or mutation ledger participates. Views live in the runtime,
/// not additional generated entity fields.</summary>
public static class EntityQueryProperties
{
    private sealed record View(DynamicPropertySnapshot Values, string[] Aliases);
    private sealed class Slot { internal View? View; }
    private sealed class SelectionIdentity : IEqualityComparer<(IEntityJsonDocument Entity, SelectQuery Query)>
    {
        public bool Equals((IEntityJsonDocument Entity, SelectQuery Query) a, (IEntityJsonDocument Entity, SelectQuery Query) b)
            => ReferenceEquals(a.Entity, b.Entity) && ReferenceEquals(a.Query, b.Query);
        public int GetHashCode((IEntityJsonDocument Entity, SelectQuery Query) item)
            => HashCode.Combine(RuntimeHelpers.GetHashCode(item.Entity), RuntimeHelpers.GetHashCode(item.Query));
    }
    private static readonly ConditionalWeakTable<IEntityJsonDocument, Slot> Views = new();

    public static T Capture<T>(T entity, SelectQuery query) where T : class, IEntityJsonDocument
    {
        CaptureList(new[] { entity }, query); return entity;
    }

    public static void CaptureList<T>(IEnumerable<T> entities, SelectQuery query) where T : class, IEntityJsonDocument
    {
        ArgumentNullException.ThrowIfNull(entities); ArgumentNullException.ThrowIfNull(query);
        var staged = new Dictionary<IEntityJsonDocument, Record>(ReferenceEqualityComparer.Instance);
        var visited = new HashSet<(IEntityJsonDocument Entity, SelectQuery Query)>(new SelectionIdentity());
        var selections = new Dictionary<SelectQuery, string[]>(ReferenceEqualityComparer.Instance);
        foreach (var entity in entities) Visit(entity, query, 0);
        // All ordinary validation/copy failures precede the first replacement.
        var replacements = staged.Select(pair => (Entity: pair.Key,
            View: new View(DynamicPropertySnapshot.Capture(pair.Value, Array.Empty<string>()), pair.Value.Keys.ToArray()))).ToArray();
        foreach (var item in replacements) Views.GetValue(item.Entity, _ => new Slot()).View = item.View;

        void Visit(IEntityJsonDocument entity, SelectQuery selected, int depth)
        {
            if (depth > 64) throw new ArgumentException("Selected query-property graph exceeds the depth limit");
            if (!selections.TryGetValue(selected, out var aliases)) {
                aliases = selected.AggregateItems.Where(a => !(a.Function == AggregateFunction.Count
                    && a.Alias == selected.PageCountOperation?.Alias)).Select(a => a.Alias)
                .Concat(selected.RelationAggregates.Select(a => a.Alias))
                .Concat(selected.ExprProjection.Select(a => a.Alias))
                .Concat(selected.RawProjections.Select(a => a.PropertyName))
                    .Concat(selected.DynamicProperties.Select(a => a.PropertyName)).ToArray();
                if (aliases.Distinct(StringComparer.Ordinal).Count() != aliases.Length)
                    throw new ArgumentException("Duplicate selected query-property alias");
                selections.Add(selected, aliases);
            }
            // Ordinary scalar pages allocate no per-row field/selection sets.
            if (aliases.Length == 0 && selected.RelationLoads.Count == 0) {
                if (Views.TryGetValue(entity, out _)) staged.TryAdd(entity, new());
                return;
            }
            if (selected.RelationLoads.Count > 0 && !visited.Add((entity, selected))) return;
            foreach (var alias in aliases)
            {
                if (string.IsNullOrWhiteSpace(alias) || alias.Length > 4096 || entity.EntityJsonSchema.IsReservedQueryAlias(alias))
                    throw new EntityJsonException("WIRE_QUERY_PROPERTY_COLLISION", ObjectLocation.Root(), "", "Query alias conflicts with the model namespace");
                if (!entity.QueryProperties.Contains(alias)) throw new ArgumentException("Selected query-property alias is absent: " + alias);
                if (!staged.TryGetValue(entity, out var values)) staged[entity] = values = new();
                values[alias] = entity.QueryProperties.Get(alias)!;
            }
            if (aliases.Length == 0 && Views.TryGetValue(entity, out _) && !staged.ContainsKey(entity)) staged[entity] = new();
            if (selected.RelationLoads.Count == 0) return;
            // The passive bridge enumerates loaded fields only. Never inspect
            // navigation properties by reflection or load an unrequested edge.
            var loaded = entity.LoadedJsonFields().ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
            foreach (var relation in selected.RelationLoads)
            {
                if (relation.Query is null) continue;
                var field = entity.EntityJsonSchema.QueryField(relation.Name);
                if (field?.Kind is not (EntityJsonKind.Reference or EntityJsonKind.Many)) continue;
                if (field is null || !loaded.TryGetValue(field.Key, out var value) || value is null) continue;
                if (field.Kind == EntityJsonKind.Reference && value is IEntityJsonDocument child) Visit(child, relation.Query, depth + 1);
                else if (field.Kind == EntityJsonKind.Many && value is IEnumerable children && value is not string)
                    foreach (var item in children) if (item is IEntityJsonDocument document) Visit(document, relation.Query, depth + 1);
            }
        }
    }

    internal static IEnumerable<KeyValuePair<string, Value>> SelectedValues(IEntityJsonDocument entity)
    {
        if (Views.TryGetValue(entity, out var slot) && slot.View is { } view)
            foreach (var alias in view.Aliases) yield return new(alias, view.Values.Get(alias)!);
    }
}
