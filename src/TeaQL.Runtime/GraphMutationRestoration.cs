using TeaQL.Core;

namespace TeaQL.Runtime;

public sealed partial class GraphMutationSession
{
    /// <summary>Generated bridge: admit raw keys, then cancel only proven persisted baseline edits.</summary>
    public void DiscardRestoredFields(string entityType, EntityRoot ledger, EntityKey key,
        LoadedScalarSnapshot snapshot)
    {
        RequireActive();
        if (_transaction != null || _preparedGraphSealed)
            throw new InvalidOperationException("GRAPH_INPUT_PREPARATION_CLOSED");
        if (key.EntityType != entityType)
            throw new InvalidOperationException("GRAPH_LEDGER_ENTITY_TYPE_MISMATCH");
        var pending = ledger.Change(key);
        CanonicalMutationFields.Validate(_context, entityType, pending, !ledger.IsNew(key) && !ledger.IsDeleted(key));
        if (ledger.IsNew(key) || ledger.IsDeleted(key) || snapshot.Owner != key
            || !snapshot.TryOriginal("id", out var originalId) || !LoadedScalarSnapshot.SameValue(originalId, key.Id)
            || !snapshot.TryOriginal("version", out var version) || version is not Value.I64Value originalVersion
            || ledger.OriginalVersion(key) != originalVersion.Value) return;
        var metadata = _context.Metadata?.GetEntity(entityType);
        if (metadata == null) return;
        var restored = pending.Where(item => item.Key is not "id" and not "version"
            && metadata.PropertyByName(item.Key) is { IsId: false, IsVersion: false }
            && snapshot.TryOriginal(item.Key, out var original)
            && LoadedScalarSnapshot.SameValue(item.Value, original)).ToDictionary(item => item.Key, item => item.Value);
        ledger.DiscardFields(key, restored);
    }
}
