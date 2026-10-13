using Xunit;

namespace TeaQL.Core.Tests;

public class DynamicPropertySnapshotTests
{
    [Fact]
    public void SnapshotKeepsDerivedValuesWithoutMakingThemMutationFields()
    {
        var nested = new Record { ["value"] = new Value.I64Value(5) };
        var record = new Record { ["id"] = new Value.I64Value(1), ["name"] = new Value.TextValue("School"),
            ["itemCount"] = new Value.I64Value(5), ["emptySum"] = new Value.NullValue(), ["nested"] = new Value.ObjectValue(nested) };
        var snapshot = DynamicPropertySnapshot.Capture(record,new[]{"id","name"});
        nested["value"] = new Value.I64Value(99); record["itemCount"] = new Value.I64Value(99);
        Assert.Equal(5L,snapshot.Get("itemCount")!.TryI64());
        Assert.False(snapshot.Contains("name")); Assert.Null(snapshot.Get("absent"));
        Assert.True(snapshot.Contains("emptySum")); Assert.IsType<Value.NullValue>(snapshot.Get("emptySum"));
        var first = Assert.IsType<Value.ObjectValue>(snapshot.Get("nested")); first.Value["value"] = new Value.I64Value(99);
        Assert.Equal(5L,Assert.IsType<Value.ObjectValue>(snapshot.Get("nested")).Value["value"].TryI64());
    }
}
