namespace TeaQL.Core;

/// <summary>Process-local provenance, never inferred from JSON or a missing version.</summary>
public enum EntityCreationState { Unmanaged, Reserved, Hydrated }

/// <summary>The public generated Q factory and selected Context allocator own creation.</summary>
public sealed class EntityCreationException : InvalidOperationException
{
    public string Code => "ENTITY_CREATION_CONTEXT_REQUIRED";
    public EntityCreationException() : base("ENTITY_CREATION_CONTEXT_REQUIRED: create through the generated Q factory with UserContext; hydrated mutation requires an original persisted version") { }
}

public static class EntityCreation
{
    public static void RequireReserved(EntityCreationState state, long? id)
    {
        if (state != EntityCreationState.Reserved || id is not > 0)
            throw new EntityCreationException();
    }

    /// <summary>Admit only actual mutations; unchanged partial read views remain inert.</summary>
    public static void RequireMutation(EntityCreationState state, long? id, bool creating,
        bool changed, LoadedScalarSnapshot snapshot)
    {
        if (!changed) return;
        if (creating)
        {
            RequireReserved(state, id);
            if (snapshot.Origin != LoadedScalarSnapshotOrigin.CreationBaseline)
                throw new EntityCreationException();
            return;
        }
        if (state != EntityCreationState.Hydrated || id is not > 0 || !snapshot.PersistedVersion.HasValue)
            throw new EntityCreationException();
        // Tombstones, zero and exhaustion remain the shared version validator's
        // responsibility. This guard establishes origin, not version validity.
    }
}
