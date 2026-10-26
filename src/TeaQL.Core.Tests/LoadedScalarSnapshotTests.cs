using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Core.Tests;

public class LoadedScalarSnapshotTests
{
    [Fact]
    public void ExactEqualityPreservesDecimalScaleAndFloatingZeroAndRejectsNonFiniteValues()
    {
        Assert.True(LoadedScalarSnapshot.SameValue(new Value.DecimalValue(37m), new Value.DecimalValue(37m)));
        Assert.False(LoadedScalarSnapshot.SameValue(new Value.DecimalValue(37m), new Value.DecimalValue(37.00m)));
        Assert.False(LoadedScalarSnapshot.SameValue(new Value.F64Value(0.0), new Value.F64Value(-0.0)));
        Assert.False(LoadedScalarSnapshot.SameValue(new Value.F64Value(double.NaN), new Value.F64Value(double.NaN)));
        Assert.False(LoadedScalarSnapshot.SameValue(new Value.F64Value(double.PositiveInfinity), new Value.F64Value(double.PositiveInfinity)));
        Assert.False(LoadedScalarSnapshot.SameValue(new Value.I64Value(1), new Value.U64Value(1)));
        Assert.False(LoadedScalarSnapshot.SameValue(new Value.I64Value(1), new Value.TextValue("1")));
        Assert.True(LoadedScalarSnapshot.SameValue(new Value.NullValue(), new Value.TypedNullValue(DataType.Text)));
    }

    [Fact]
    public void ExactEqualityHandlesBoundedNestedValuesAndCalendarRepresentation()
    {
        var original = new Value.ObjectValue(new Record { ["list"] = new Value.ListValue([new Value.I64Value(1)]) });
        var equal = new Value.ObjectValue(new Record { ["list"] = new Value.ListValue([new Value.I64Value(1)]) });
        Assert.True(LoadedScalarSnapshot.SameValue(original, equal));
        equal.Value["list"] = new Value.ListValue([new Value.TextValue("1")]);
        Assert.False(LoadedScalarSnapshot.SameValue(original, equal));
        var utc = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc);
        Assert.False(LoadedScalarSnapshot.SameValue(new Value.DateTimeValue(utc),
            new Value.DateTimeValue(DateTime.SpecifyKind(utc, DateTimeKind.Unspecified))));
        Value a = new Value.I64Value(1), b = new Value.I64Value(1);
        for (var depth = 0; depth < 35; depth++) { a = new Value.ListValue([a]); b = new Value.ListValue([b]); }
        Assert.False(LoadedScalarSnapshot.SameValue(a, b));
    }

    [Fact]
    public void TypedOwnerDoesNotBecomeSerializedProvenance()
    {
        var snapshot = new LoadedScalarSnapshot(new EntityKey("Probe", 1), new Record { ["id"] = new Value.I64Value(1) });
        Assert.Equal("{}", JsonSerializer.Serialize(snapshot));
    }

    [Fact]
    public void OwnsNestedValuesWithoutExposingSerializedProvenance()
    {
        var json = JsonNode.Parse("{\"value\":\"original\"}");
        var values = new Record { ["name"] = new Value.TextValue("PRIVATEOLD"), ["details"] = new Value.JsonValue(json) };
        var snapshot = new LoadedScalarSnapshot(values);
        values["name"] = new Value.TextValue("changed"); json!["value"] = "changed";
        var copy = snapshot.CopyValues();
        Assert.Equal("PRIVATEOLD", copy["name"].TryText());
        Assert.Equal("original", ((Value.JsonValue)copy["details"]).Value!["value"]!.GetValue<string>());
        ((Value.JsonValue)copy["details"]).Value!["value"] = "second change";
        Assert.Equal("original", ((Value.JsonValue)snapshot.CopyValues()["details"]).Value!["value"]!.GetValue<string>());
        Assert.Equal("{}", JsonSerializer.Serialize(snapshot));
    }
}
