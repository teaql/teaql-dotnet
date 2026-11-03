using System.ComponentModel;

namespace TeaQL.Runtime;

/// <summary>Generated adapter traversal phases, not business mutation actions.</summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public enum GraphMutationTraversal { Prepare, Reserve, Preflight, Write }

public sealed partial class GraphMutationSession
{
    // Invocation-owned reference identity. Distinct writable views must not be
    // mistaken for the same object merely because their database IDs are equal.
    private readonly Dictionary<object, byte> _traversal = new(ReferenceEqualityComparer.Instance);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool VisitEntity(object entity, GraphMutationTraversal phase)
    {
        RequireActive();
        ArgumentNullException.ThrowIfNull(entity);
        if ((uint)phase > (uint)GraphMutationTraversal.Write)
            throw new ArgumentOutOfRangeException(nameof(phase));
        var bit = (byte)(1 << (int)phase);
        _traversal.TryGetValue(entity, out var prior);
        if ((prior & bit) != 0)
        {
            if (phase == GraphMutationTraversal.Prepare) ValidatePreparedInputUnchanged(entity);
            return false;
        }
        _traversal[entity] = (byte)(prior | bit);
        return true;
    }
}
