using TeaQL.Core;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Core.Tests;

public class QueryProjectionSnapshotTests
{
    [Fact]
    public void QueryAliasesOwnInputAndReturnedMutableValues()
    {
        var counts = new List<Value> { new Value.I64Value(2) };
        var json = System.Text.Json.Nodes.JsonNode.Parse("{\"count\":2}");
        var input = new Record {
            ["id"] = new Value.I64Value(1), ["OrderNumber"] = new Value.TextValue("private"),
            ["order_number"] = new Value.TextValue("private"), ["Items"] = new Value.ListValue(new()),
            ["summary"] = new Value.ObjectValue(new Record { ["counts"] = new Value.ListValue(counts) }),
            ["json"] = new Value.JsonValue(json), ["SaveAsync"] = new Value.I64Value(3)
        };
        var snapshot = new QueryProjectionSnapshot(input, new[] { "id", "OrderNumber", "order_number", "Items" });
        counts.Add(new Value.I64Value(99)); json!["count"] = 99;
        var returned = Assert.IsType<Value.ObjectValue>(snapshot.Get("summary")).Value;
        Assert.IsType<Value.ListValue>(returned["counts"]).Values.Clear();
        Assert.IsType<Value.JsonValue>(snapshot.Get("json")).Value!["count"] = 100;
        Assert.Single(Assert.IsType<Value.ListValue>(Assert.IsType<Value.ObjectValue>(snapshot.Get("summary")).Value["counts"]).Values);
        Assert.Equal(2, Assert.IsType<Value.JsonValue>(snapshot.Get("json")).Value!["count"]!.GetValue<int>());
        Assert.Equal(3L, snapshot.Get("SaveAsync").TryI64());
        foreach (var name in new[] { "id", "OrderNumber", "order_number", "Items" }) {
            Assert.False(snapshot.Contains(name));
            Assert.Throws<KeyNotFoundException>(() => snapshot.Get(name));
        }
        Assert.DoesNotContain("private", snapshot.ToString());
    }

    [Fact]
    public void MissingIsDifferentFromZeroAndNull()
    {
        var snapshot = new QueryProjectionSnapshot(new Record {
            ["zero"] = new Value.I64Value(0), ["null"] = new Value.NullValue()
        }, Array.Empty<string>());
        Assert.True(snapshot.Contains("zero")); Assert.Equal(0L, snapshot.Get("zero").TryI64());
        Assert.True(snapshot.Contains("null")); Assert.IsType<Value.NullValue>(snapshot.Get("null"));
        Assert.False(snapshot.Contains("missing"));
        Assert.Throws<KeyNotFoundException>(() => snapshot.Get("missing"));
        Assert.False(QueryProjectionSnapshot.Empty.Contains("zero"));
    }
}
