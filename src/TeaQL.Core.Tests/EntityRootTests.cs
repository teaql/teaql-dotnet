using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class EntityRootTests
{
    [Fact]
    public void TracksFinalValuesVersionsAndLifecycle()
    {
        var root = new EntityRoot();
        var order = new EntityKey("Order", new Value.I64Value(10));
        var line = new EntityKey("OrderLine", new Value.I64Value(20));
        root.SetOriginalVersion(order, 3);
        root.Set(order, "status", new Value.TextValue("pending"));
        root.Set(order, "status", new Value.TextValue("confirmed"));
        root.Set(line, "quantity", new Value.I64Value(2));
        root.MarkAsNew(line);

        Assert.Equal("confirmed", ((Value.TextValue)root.Changes()[order]["status"]).Value);
        Assert.Equal(3, root.OriginalVersion(order));
        Assert.True(root.IsNew(line));

        root.MarkAsDeleted(line);
        Assert.True(root.IsDeleted(line));
        Assert.False(root.Changes().ContainsKey(line));
        root.ClearCommitted();
        Assert.Empty(root.Changes());
        Assert.False(root.IsNew(line));
        Assert.False(root.IsDeleted(line));
    }

    [Fact]
    public void MergesRekeysAndClearsOneEntity()
    {
        var child = new EntityRoot();
        var temporary = new EntityKey("OrderLine", new Value.I64Value(-1));
        var persisted = new EntityKey("OrderLine", new Value.I64Value(42));
        child.MarkAsNew(temporary); child.Set(temporary, "quantity", new Value.I64Value(2));
        var root = new EntityRoot(); root.MergeFrom(child); root.Rekey(temporary, persisted);
        Assert.True(root.IsNew(persisted)); Assert.True(root.Change(persisted).ContainsKey("quantity"));
        root.ClearEntity(persisted); Assert.False(root.IsNew(persisted)); Assert.Empty(root.Change(persisted));
    }

    [Fact]
    public void GeneratedEntityLifecycleCanUseNativeIdAndPersistedBoundary()
    {
        var root = new EntityRoot();
        var key = new EntityKey("School", 1001);
        Assert.True(root.IsEmpty);
        root.MarkAsNew(key);
        Assert.False(root.IsEmpty);
        root.MarkAsPersisted(key);
        Assert.True(root.IsEmpty);
    }

    [Fact]
    public void ScopedImportPreservesUnreachedAndSourceMutations()
    {
        var source = new EntityRoot();
        var parent = new EntityKey("Order", 1);
        var child = new EntityKey("OrderLine", 1);
        var sibling = new EntityKey("OrderLine", 2);
        source.SetOriginalVersion(parent, 9);
        source.SetOriginalVersion(child, 2);
        source.Set(parent, "name", new Value.TextValue("foreign parent"));
        source.Set(child, "quantity", new Value.I64Value(4));
        source.Set(sibling, "quantity", new Value.I64Value(99));
        source.SetTraceChain(child, new[] { new TraceNode("Order", 1, "") { Kind = "auditReason", Detail = "import reached child" } });
        var destination = new EntityRoot();
        destination.SetOriginalVersion(parent, 3);

        destination.MergeEntityFrom(source, child);

        Assert.Single(destination.Changes());
        Assert.Equal(new Value.I64Value(4), destination.Change(child)["quantity"]);
        Assert.Equal(2, destination.OriginalVersion(child));
        Assert.Equal(3, destination.OriginalVersion(parent));
        Assert.False(destination.HasPending(parent));
        Assert.False(destination.HasPending(sibling));
        Assert.Equal("import reached child", Assert.Single(destination.TraceChain(child)).Detail);
        Assert.Equal(3, source.Changes().Count);
        destination.ClearEntity(child);
        Assert.True(source.HasPending(child));
    }

    [Fact]
    public void ScopedImportCopiesLifecycleButDoesNotCreatePendingHydration()
    {
        var source = new EntityRoot(); var destination = new EntityRoot();
        var loaded = new EntityKey("Platform", 1);
        var added = new EntityKey("OrderLine", -1);
        var removed = new EntityKey("OrderLine", 2);
        source.SetOriginalVersion(loaded, 1);
        source.MarkAsNew(added); source.Set(added, "name", new Value.TextValue("new"));
        source.SetOriginalVersion(removed, 3); source.MarkAsDeleted(removed);
        foreach (var key in new[] { loaded, added, removed }) destination.MergeEntityFrom(source, key);
        Assert.False(destination.HasPending(loaded));
        Assert.True(destination.IsNew(added)); Assert.True(destination.HasPending(added));
        Assert.True(destination.IsDeleted(removed)); Assert.True(destination.HasPending(removed));
        Assert.True(source.IsNew(added)); Assert.True(source.IsDeleted(removed));
    }

    [Fact]
    public void ConflictingLoadedVersionRejectsScopedAndWholeImportBeforeCopying()
    {
        var destination = new EntityRoot(); var source = new EntityRoot();
        var key = new EntityKey("Order", 1); var unrelated = new EntityKey("OrderLine", 2);
        destination.SetOriginalVersion(key, 1);
        destination.Set(key, "name", new Value.TextValue("original"));
        source.SetOriginalVersion(key, 2);
        source.Set(key, "name", new Value.TextValue("replacement"));
        source.MarkAsDeleted(key);
        source.MarkAsNew(unrelated); source.Set(unrelated, "name", new Value.TextValue("unreached"));
        source.SetTraceChain(key, new[] { new TraceNode("Order", 1, "other") });

        Assert.Contains("ENTITY_VERSION_CONFLICT", Assert.Throws<InvalidOperationException>(() => destination.MergeEntityFrom(source, key)).Message);
        Assert.Throws<InvalidOperationException>(() => destination.MergeFrom(source));
        Assert.Equal(1, destination.OriginalVersion(key));
        Assert.Equal(new Value.TextValue("original"), destination.Change(key)["name"]);
        Assert.False(destination.HasPending(unrelated)); Assert.False(destination.IsDeleted(key));
        Assert.Empty(destination.TraceChain(key)); Assert.True(source.IsDeleted(key));
    }

    [Fact]
    public void ConflictingRekeyLeavesBothKeysUntouched()
    {
        var root = new EntityRoot(); var source = new EntityKey("Order", -1); var target = new EntityKey("Order", 1);
        root.SetOriginalVersion(source, 2); root.SetOriginalVersion(target, 1);
        root.MarkAsNew(source); root.Set(source, "name", new Value.TextValue("source"));
        root.Set(target, "name", new Value.TextValue("target"));
        Assert.Throws<InvalidOperationException>(() => root.Rekey(source, target));
        Assert.True(root.IsNew(source)); Assert.Equal(2, root.OriginalVersion(source));
        Assert.Equal(1, root.OriginalVersion(target));
        Assert.Equal(new Value.TextValue("source"), root.Change(source)["name"]);
        Assert.Equal(new Value.TextValue("target"), root.Change(target)["name"]);
    }

    [Fact]
    public void CommittedVersionAdvanceRequiresClearedChangesAndKeepsOtherTypesIndependent()
    {
        var root = new EntityRoot(); var order = new EntityKey("Order", 1); var payment = new EntityKey("Payment", 1);
        root.SetOriginalVersion(order, 7); root.SetOriginalVersion(payment, 1);
        root.SetOriginalVersion(order, 7);
        Assert.Throws<InvalidOperationException>(() => root.SetOriginalVersion(order, 8));
        root.Set(order, "name", new Value.TextValue("pending"));
        Assert.Contains("ENTITY_PENDING_CHANGES", Assert.Throws<InvalidOperationException>(() => root.AcceptCommittedVersion(order, 8)).Message);
        root.ClearEntity(order); root.AcceptCommittedVersion(order, 8);
        Assert.Equal(8, root.OriginalVersion(order)); Assert.Equal(1, root.OriginalVersion(payment));
        Assert.True(root.IsEmpty);
    }

    [Fact]
    public async Task ConcurrentConflictingVersionRegistrationHasOneWinner()
    {
        var root = new EntityRoot(); var key = new EntityKey("Order", 1);
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async Task<bool> Register(long version)
        {
            await start.Task;
            try { root.SetOriginalVersion(key, version); return true; }
            catch (InvalidOperationException error) when (error.Message.Contains("ENTITY_VERSION_CONFLICT")) { return false; }
        }
        var first = Task.Run(() => Register(1)); var second = Task.Run(() => Register(2));
        start.SetResult(); var results = await Task.WhenAll(first, second);
        Assert.Single(results.Where(value => value));
        Assert.Contains(root.OriginalVersion(key), new long?[] { 1, 2 });
    }
}
