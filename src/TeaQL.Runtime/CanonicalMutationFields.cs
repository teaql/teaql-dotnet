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
            Validate(context, entity, root.Change(key), request is UpdateMutationRequest);
        var values = request switch {
            InsertMutationRequest insert => insert.Command.Values, UpdateMutationRequest update => update.Command.Values,
            _ => null
        };
        if (values != null) Validate(context, entity, values, request is UpdateMutationRequest);
    }

    internal static void Validate(UserContext context, string entity, IReadOnlyDictionary<string, Value> values,
        bool updating = false)
    {
        var descriptor = context.Metadata?.GetEntity(entity);
        // Legacy/manual entities without fixed-field metadata retain their
        // contract; do not invent one from member names or global suffix bans.
        if (descriptor == null || descriptor.Properties.Count == 0) return;
        var properties = descriptor.Properties.ToDictionary(property => property.Name, StringComparer.Ordinal);
        var violations = new List<CheckResult>();
        foreach (var field in values.Keys)
        {
            if (!properties.TryGetValue(field, out var property)) violations.Add(new CheckResult {
                RuleId = "unknown_field", EntityType = entity, Location = ObjectLocation.Property(field),
                Message = "Expected a canonical model field name"
            });
            else if (updating && property.ImmutableAfterCreateRuleId is { } ruleId) violations.Add(new CheckResult {
                RuleId = ruleId, EntityType = entity, Location = ObjectLocation.Property(field),
                Message = "Field is immutable after create"
            });
        }
        if (violations.Count == 0) return;
        context.TranslateCheckResults(violations);
        throw new CheckException(violations);
    }
}
