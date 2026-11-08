using System.Text.Json;

namespace TeaQL.Core.Tests;

public class EntityCreationTests
{
    private static LoadedScalarSnapshot Snapshot(long? version = null,
        LoadedScalarSnapshotOrigin origin = LoadedScalarSnapshotOrigin.Hydrated)
    {
        var row = new Record();
        if (version.HasValue) row["version"] = new Value.I64Value(version.Value);
        return new LoadedScalarSnapshot(new EntityKey("WorkItem", 7), row, origin);
    }

    [Fact]
    public void OnlyReservedPositiveIdentityAllowsCreation()
    {
        foreach (var state in Enum.GetValues<EntityCreationState>())
        foreach (long? id in new long?[] { null, 0, -1, 7, long.MaxValue })
        {
            if (state == EntityCreationState.Reserved && id is > 0)
                EntityCreation.RequireMutation(state, id, true, true, Snapshot(origin: LoadedScalarSnapshotOrigin.CreationBaseline));
            else Assert.Equal("ENTITY_CREATION_CONTEXT_REQUIRED", Assert.Throws<EntityCreationException>(() =>
                EntityCreation.RequireMutation(state, id, true, true, Snapshot(origin: LoadedScalarSnapshotOrigin.CreationBaseline))).Code);
        }
    }

    [Fact]
    public void UnchangedSparseHydrationIsNotANewEntity()
    {
        EntityCreation.RequireMutation(EntityCreationState.Hydrated, null, false, false, Snapshot());
        EntityCreation.RequireMutation(EntityCreationState.Hydrated, 7, false, false, Snapshot());
    }

    [Fact]
    public void HydratedMutationRequiresOriginalPersistedVersion()
    {
        Assert.Throws<EntityCreationException>(() => EntityCreation.RequireMutation(EntityCreationState.Hydrated, 7, false, true, Snapshot()));
        foreach (long version in new[] { 0L, -1L, 1L, long.MaxValue })
            EntityCreation.RequireMutation(EntityCreationState.Hydrated, 7, false, true, Snapshot(version));
        Assert.Throws<EntityCreationException>(() => EntityCreation.RequireMutation(EntityCreationState.Hydrated, null, false, true, Snapshot(1)));
        Assert.Throws<EntityCreationException>(() => EntityCreation.RequireMutation(EntityCreationState.Reserved, 7, false, true, Snapshot(1)));
    }

    [Fact]
    public void HydratedSnapshotCannotBePromotedToInsert()
    {
        Assert.Throws<EntityCreationException>(() => EntityCreation.RequireMutation(EntityCreationState.Hydrated, 7, true, true, Snapshot(1)));
        Assert.Throws<EntityCreationException>(() => EntityCreation.RequireMutation(EntityCreationState.Reserved, 7, true, true, Snapshot(1)));
    }

    [Fact]
    public void CreationBaselineNeverFabricatesPersistedVersionOrWireMetadata()
    {
        var hydrated = Snapshot(3);
        var baseline = Snapshot(3, LoadedScalarSnapshotOrigin.CreationBaseline);
        Assert.Equal(3, hydrated.PersistedVersion);
        Assert.Null(baseline.PersistedVersion);
        Assert.Throws<EntityCreationException>(() => EntityCreation.RequireMutation(EntityCreationState.Hydrated, 7, false, true, baseline));
        Assert.Equal("{}", JsonSerializer.Serialize(hydrated));
        Assert.Equal("{}", JsonSerializer.Serialize(baseline));
    }

    [Fact]
    public void MutableInputOrCopiedVersionCannotManufacturePersistedProvenance()
    {
        var row = new Record();
        var snapshot = new LoadedScalarSnapshot(new EntityKey("WorkItem", 7), row);
        row["version"] = new Value.I64Value(1);
        snapshot.CopyValues()["version"] = new Value.I64Value(1);
        Assert.Null(snapshot.PersistedVersion);
        Assert.Throws<EntityCreationException>(() => EntityCreation.RequireMutation(EntityCreationState.Hydrated, 7, false, true, snapshot));
    }
}
