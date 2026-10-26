using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class KnownBaselineRestorationTests
{
    [Fact]
    public async Task RestoresOnlyKnownFieldsAndKeepsVersionLineageAndOtherTypedOwners()
    {
        var key = new EntityKey("Probe", 1); var other = new EntityKey("Other", 1); var root = new EntityRoot();
        root.SetOriginalVersion(key, 7); root.SetOriginalVersion(other, 3);
        root.SetTraceChain(key, [new TraceNode("Probe", 1, "loaded origin")]);
        root.Set(key, "amount", new Value.I64Value(37)); root.Set(key, "name", new Value.TextValue("real edit"));
        root.Set(other, "amount", new Value.I64Value(37));
        await Prepare(graph => graph.DiscardRestoredFields("Probe", root, key, Snapshot(key)));
        Assert.Equal("real edit", Assert.Single(root.Change(key)).Value.TryText());
        Assert.Equal(37, root.Change(other)["amount"].TryI64()); Assert.Equal(7, root.OriginalVersion(key));
        Assert.Equal("loaded origin", Assert.Single(root.TraceChain(key)).Comment);
    }

    [Fact]
    public async Task LoadedNullIsKnownButAnAbsentOriginalIsNeverAssumedNull()
    {
        var key = new EntityKey("Probe", 1); var root = new EntityRoot(); root.SetOriginalVersion(key, 7);
        root.Set(key, "name", new Value.NullValue()); root.Set(key, "unknown_original", new Value.NullValue());
        await Prepare(graph => graph.DiscardRestoredFields("Probe", root, key, Snapshot(key)));
        Assert.False(root.Change(key).ContainsKey("name"));
        Assert.IsType<Value.NullValue>(root.Change(key)["unknown_original"]);
    }

    [Theory]
    [InlineData("Other", 1, 7)] [InlineData("Probe", 2, 7)] [InlineData("Probe", 1, 8)]
    public async Task AForeignOwnerOrVersionNeverProvidesTheBaseline(string owner, long id, long version)
    {
        var key = new EntityKey("Probe", 1); var root = new EntityRoot(); root.SetOriginalVersion(key, 7);
        root.Set(key, "amount", new Value.I64Value(37));
        await Prepare(graph => graph.DiscardRestoredFields("Probe", root, key, Snapshot(new EntityKey(owner, id), version)));
        Assert.True(root.HasPending(key));
    }

    [Fact]
    public async Task UnknownEqualRawKeysAreRejectedBeforeAnyDiscard()
    {
        var key = new EntityKey("Probe", 1); var root = new EntityRoot(); root.SetOriginalVersion(key, 7);
        root.Set(key, "amount", new Value.I64Value(37)); root.Set(key, "Amount", new Value.I64Value(37));
        var error = await Assert.ThrowsAsync<CheckException>(() => Prepare(graph =>
            graph.DiscardRestoredFields("Probe", root, key, Snapshot(key))));
        Assert.Equal("unknown_field", Assert.Single(error.Violations).RuleId);
        Assert.Equal(2, root.Change(key).Count);
    }

    [Theory]
    [InlineData(true)] [InlineData(false)]
    public async Task NewAndDeletedOperationsRemainPending(bool creating)
    {
        var key = new EntityKey("Probe", 1); var root = new EntityRoot(); root.SetOriginalVersion(key, 7);
        if (creating) root.MarkAsNew(key); else root.MarkAsDeleted(key);
        root.Set(key, "amount", new Value.I64Value(37));
        await Prepare(graph => graph.DiscardRestoredFields("Probe", root, key, Snapshot(key)));
        Assert.True(root.HasPending(key)); Assert.True(root.Change(key).ContainsKey("amount"));
    }

    [Fact]
    public async Task ASealedEmptyTraversalDoesNotRequireOrTouchAProvider()
    {
        var context = Context();
        Assert.Equal(42, await context.ExecuteGraphSaveAsync("save proven empty graph", graph => {
            graph.SealPreparedGraph();
            Assert.Equal("GRAPH_INPUT_PREPARATION_CLOSED", Assert.Throws<InvalidOperationException>(() =>
                graph.PrepareInput(new object(), MutationRequest.Create(new InsertCommand("Probe"),
                    "too late", new EntityKey("Probe", -1), new EntityRoot()))).Message);
            return Task.CompletedTask;
        }, async graph => {
            await Assert.ThrowsAsync<InvalidOperationException>(() => graph.MutateAsync(
                new InsertMutationRequest(new InsertCommand("Probe"), "no empty-plan write")));
            return 42;
        }));
    }

    private static UserContext Context() => new RuntimeModule().Entity(EntityDescriptor.New("Probe")
        .Property(PropertyDescriptor.New("id", DataType.I64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version())
        .Property(PropertyDescriptor.New("amount", DataType.I64))
        .Property(PropertyDescriptor.New("name", DataType.Text))
        .Property(PropertyDescriptor.New("unknown_original", DataType.Text))).IntoContext();
    private static Task<int> Prepare(Action<GraphMutationSession> prepare) => Context().ExecuteGraphSaveAsync(
        "normalize one trusted generated entity", graph => { prepare(graph); graph.SealPreparedGraph(); return Task.CompletedTask; },
        _ => Task.FromResult(1));
    private static LoadedScalarSnapshot Snapshot(EntityKey key, long version = 7) => new(key, new Record {
        ["id"] = key.Id, ["version"] = new Value.I64Value(version), ["amount"] = new Value.I64Value(37), ["name"] = new Value.NullValue()
    });
}
