using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Core.Tests;

public class LoadedScalarSnapshotTests
{
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
