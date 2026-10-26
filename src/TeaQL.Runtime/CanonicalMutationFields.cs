using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

/// <summary>Trusted model vocabulary at mutation admission, not a wire-name converter.</summary>
internal static class CanonicalMutationFields
{
    internal static void Validate(UserContext context, MutationRequest request)
    {
        var entity = request switch {
            InsertMutationRequest insert => insert.Command.Entity, UpdateMutationRequest update => update.Command.Entity,
            DeleteMutationRequest delete => delete.Command.Entity, RecoverMutationRequest recover => recover.Command.Entity,
            _ => ""
        };
        // A generated create may materialize a command from fixed members.
        // Its original ledger must not lose an invalid key at that boundary.
        if (request.LedgerRoot is { } root && request.LedgerKey is { } key && key.EntityType == entity)
            Validate(context, entity, root.Change(key));
        var values = request switch {
            InsertMutationRequest insert => insert.Command.Values, UpdateMutationRequest update => update.Command.Values,
            _ => null
        };
        if (values != null) Validate(context, entity, values);
    }

    internal static void Validate(UserContext context, string entity, IReadOnlyDictionary<string, Value> values)
    {
        var descriptor = context.Metadata?.GetEntity(entity);
        // Legacy/manual entities without fixed-field metadata retain their
        // contract; do not invent one from member names or global suffix bans.
        if (descriptor == null || descriptor.Properties.Count == 0) return;
        var names = descriptor.Properties.Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
        var violations = values.Keys.Where(field => !names.Contains(field)).Select(field => new CheckResult {
            RuleId = "unknown_field", EntityType = entity, Location = ObjectLocation.Property(field),
            Message = "Expected a canonical model field name"
        }).ToList();
        if (violations.Count == 0) return;
        context.TranslateCheckResults(violations);
        throw new CheckException(violations);
    }
}
