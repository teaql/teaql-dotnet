using System.Text.Json;
using TeaQL.Core;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class EntityQueryPropertyTests
{
    private sealed class Node : IEntityJsonDocument
    {
        public readonly Dictionary<string, object?> Data = new(StringComparer.Ordinal);
        public DynamicPropertySnapshot Properties = DynamicPropertySnapshot.Empty;
        public EntityJsonSchema EntityJsonSchema { get; init; } = Schema;
        public DynamicPropertySnapshot QueryProperties => Properties;
        public IEnumerable<KeyValuePair<string, object?>> LoadedJsonFields() => Data;
        public void Values(params (string Key, Value Value)[] values) => Properties =
            DynamicPropertySnapshot.Capture(new Record(values.ToDictionary(v => v.Key, v => v.Value)), Array.Empty<string>());
    }
    private static readonly EntityJsonSchema Schema = new("Node", new EntityJsonField[] {
        new("id", EntityJsonKind.Id), new("version", EntityJsonKind.Version),
        new("display_name", EntityJsonKind.Text, "DisplayName"),
        new("parent", EntityJsonKind.Reference, "Parent", () => Schema!),
        new("children", EntityJsonKind.Many, "ChildRows", () => Schema!) });
    private static SelectQuery Selected(params string[] aliases) => new("Node") {
        AggregateItems = aliases.Select(alias => Aggregate.Sum("value", alias)).ToList() };
    private static string Text(Node node, JsonFieldNamingProfile profile = JsonFieldNamingProfile.CamelCase) => node.EntityJsonSchema.Serialize(node, profile);

    [Theory]
    [InlineData(JsonFieldNamingProfile.CamelCase)] [InlineData(JsonFieldNamingProfile.SnakeCase)] [InlineData(JsonFieldNamingProfile.PascalCase)]
    public void SelectedValuesKeepNullZeroExactDecimalAndLargeIntegerWithoutChangingLoadedFields(JsonFieldNamingProfile profile)
    {
        var node = new Node(); node.Data["id"] = 1L; node.Data["DisplayName"] = "original";
        node.Values(("total", new Value.NullValue()), ("rows", new Value.I64Value(0)),
            ("amount", new Value.DecimalValue(123456789012.345678900m)), ("exact", new Value.I64Value(long.MaxValue)),
            ("private", new Value.TextValue("must remain private")));
        var query = Selected("total", "rows", "amount", "exact"); var before = JsonSerializer.Serialize(query);
        Assert.Same(node, EntityQueryProperties.Capture(node, query));
        using var json = JsonDocument.Parse(Text(node, profile));
        Assert.Equal(JsonValueKind.Null, json.RootElement.GetProperty("total").ValueKind);
        Assert.Equal(0, json.RootElement.GetProperty("rows").GetInt64());
        Assert.Equal("123456789012.345678900", json.RootElement.GetProperty("amount").GetString());
        Assert.Equal("9223372036854775807", json.RootElement.GetProperty("exact").GetRawText());
        Assert.False(json.RootElement.TryGetProperty("private", out _)); Assert.Equal(6, json.RootElement.EnumerateObject().Count());
        Assert.Equal(before, JsonSerializer.Serialize(query)); Assert.Equal(2, node.Data.Count);
        Assert.Equal("original", node.Data["DisplayName"]); Assert.False(node.Properties.Contains("not_selected"));
    }

    [Fact] public void UnknownPropertiesStayPrivateAndSelectedAliasesAreOutputOnly()
    {
        var node = new Node(); node.Values(("total", new Value.NullValue()), ("private", new Value.TextValue("secret")));
        Assert.Equal("{}", Text(node)); EntityQueryProperties.Capture(node, Selected("total"));
        Assert.Equal("{\"total\":null}", Text(node));
        Assert.Equal("WIRE_UNKNOWN_FIELD", Assert.Throws<EntityJsonException>(() => Schema.Parse(Text(node))).Code);
        EntityQueryProperties.Capture(node, new SelectQuery("Node")); Assert.Equal("{}", Text(node));
    }

    [Fact] public void MissingSelectedAliasIsNotFabricatedAsNullAndDoesNotReplacePreviousView()
    {
        var node = new Node(); node.Values(("total", new Value.I64Value(7)));
        EntityQueryProperties.Capture(node, Selected("total")); var before = Text(node);
        Assert.Throws<ArgumentException>(() => EntityQueryProperties.Capture(node, Selected("total", "absent")));
        Assert.Equal(before, Text(node)); Assert.Null(node.Properties.Get("absent"));
    }

    [Fact] public void DuplicateAliasAcrossRootAndRelationIsRejected()
    {
        var node = new Node(); node.Values(("total", new Value.I64Value(7)));
        var query = Selected("total"); query.RelationAggregates.Add(new("ChildRows", "total", new SelectQuery("Node"), true));
        Assert.Throws<ArgumentException>(() => EntityQueryProperties.Capture(node, query)); Assert.Equal("{}", Text(node));
    }

    [Theory]
    [InlineData("display_name")] [InlineData("displayName")] [InlineData("DisplayName")]
    [InlineData("id")] [InlineData("Id")] [InlineData("children")] [InlineData("Children")] [InlineData("ChildRows")]
    public void EveryCanonicalNativeAndWireModelNameIsReserved(string alias)
    {
        var node = new Node(); node.Values((alias, new Value.I64Value(7)));
        Assert.Equal("WIRE_QUERY_PROPERTY_COLLISION", Assert.Throws<EntityJsonException>(() => EntityQueryProperties.Capture(node, Selected(alias))).Code);
        Assert.Equal("{}", Text(node));
    }

    [Theory] [InlineData("")] [InlineData(" ")] [InlineData("\t")]
    public void EmptyAliasesAreRejected(string alias)
    {
        var node = new Node(); node.Values((alias, new Value.NullValue()));
        Assert.Throws<EntityJsonException>(() => EntityQueryProperties.Capture(node, Selected(alias)));
    }

    [Fact] public void OversizedAliasIsRejected()
    {
        var alias = new string('a', 4097); var node = new Node(); node.Values((alias, new Value.NullValue()));
        Assert.Throws<EntityJsonException>(() => EntityQueryProperties.Capture(node, Selected(alias)));
    }

    [Fact] public void OnlyLoadedSelectedRelationsAreVisited()
    {
        var root = new Node(); var child = new Node(); child.Values(("total", new Value.NullValue()));
        root.Data["ChildRows"] = new[] { child }; var query = new SelectQuery("Node");
        EntityQueryProperties.Capture(root, query); Assert.Equal("{}", Text(child));
        query.RelationLoads.Add(new("children", Selected("total"))); EntityQueryProperties.Capture(root, query);
        Assert.Contains("\"total\":null", Text(root));
        var unloaded = new Node(); query.RelationLoads[0] = new("ChildRows", Selected("missing"));
        EntityQueryProperties.Capture(unloaded, query); Assert.Equal("{}", Text(unloaded));
    }

    [Fact] public void ForwardSelectionAcceptsItsNativeNameWithoutChangingCanonicalStorage()
    {
        var schema = new EntityJsonSchema("Forward", new[] { new EntityJsonField("type_platform", EntityJsonKind.Reference,
            "type_platform", () => Schema) });
        var root = new Node { EntityJsonSchema = schema }; var child = new Node();
        root.Data["type_platform"] = child; child.Values(("total", new Value.NullValue()));
        var query = new SelectQuery("Forward"); query.RelationLoads.Add(new("TypePlatform", Selected("total")));
        EntityQueryProperties.Capture(root, query); Assert.Equal("{\"typePlatform\":{\"total\":null}}", Text(root));
    }

    [Fact] public void LateChildCollisionDoesNotPartiallyReplaceAnyGraphView()
    {
        var root = new Node(); var child = new Node(); root.Data["ChildRows"] = new[] { child };
        root.Values(("old", new Value.I64Value(1)), ("new", new Value.I64Value(2)));
        child.Values(("old", new Value.I64Value(3)), ("displayName", new Value.I64Value(4)));
        EntityQueryProperties.Capture(root, Selected("old")); EntityQueryProperties.Capture(child, Selected("old"));
        var before = Text(root); var query = Selected("new"); query.RelationLoads.Add(new("ChildRows", Selected("displayName")));
        Assert.Throws<EntityJsonException>(() => EntityQueryProperties.Capture(root, query)); Assert.Equal(before, Text(root));
    }

    [Fact] public void LateRootFailureDoesNotPartiallyReplaceListViews()
    {
        var first = new Node(); first.Values(("old", new Value.I64Value(1)), ("new", new Value.I64Value(2)));
        var second = new Node(); EntityQueryProperties.Capture(first, Selected("old")); var before = Text(first);
        Assert.Throws<ArgumentException>(() => EntityQueryProperties.CaptureList(new[] { first, second }, Selected("new")));
        Assert.Equal(before, Text(first)); Assert.Equal("{}", Text(second));
    }

    [Fact] public void GraphDepthFailureKeepsPreviousViewsUnchanged()
    {
        var root = new Node(); root.Values(("old", new Value.I64Value(1)), ("new", new Value.I64Value(2)));
        EntityQueryProperties.Capture(root, Selected("old")); var query = Selected("new"); var q = query; var node = root;
        for (var i = 0; i < 66; i++) { var child = new Node(); node.Data["Parent"] = child; node = child;
            var next = new SelectQuery("Node"); q.RelationLoads.Add(new("Parent", next)); q = next; }
        Assert.Throws<ArgumentException>(() => EntityQueryProperties.Capture(root, query)); root.Data.Remove("Parent");
        Assert.Equal("{\"old\":1}", Text(root));
    }

    [Fact] public void PageCountMetadataDoesNotBecomeARequiredEntityProperty()
    {
        var node = new Node(); var query = new SelectQuery("Node"); query.AggregateItems.Add(Aggregate.Count("count"));
        query.PageCountOperation = new("count", "CountAs", "count"); EntityQueryProperties.Capture(node, query);
        Assert.Equal("{}", Text(node));
    }

    [Theory] [InlineData(double.NaN)] [InlineData(double.PositiveInfinity)] [InlineData(double.NegativeInfinity)]
    public void QueryValuesCannotBypassNonfiniteGuards(double value)
    {
        var node = new Node(); node.Values(("total", new Value.F64Value(value))); EntityQueryProperties.Capture(node, Selected("total"));
        Assert.Equal("WIRE_INVALID_TYPE", Assert.Throws<EntityJsonException>(() => Text(node)).Code);
    }

    [Fact] public void SelectedCompositeValuesAreDefensiveAndByteBounded()
    {
        var source = new Record { ["amount"] = new Value.DecimalValue(3.50m) };
        var node = new Node(); node.Values(("total", new Value.ObjectValue(source))); EntityQueryProperties.Capture(node, Selected("total"));
        source["amount"] = new Value.DecimalValue(9m);
        ((Value.ObjectValue)node.Properties.Get("total")!).Value["amount"] = new Value.DecimalValue(10m);
        Assert.Equal("{\"total\":{\"amount\":\"3.50\"}}", Text(node));
        var bounded = new Node { EntityJsonSchema = new("Bounded", Array.Empty<EntityJsonField>(), maxJsonBytes: 16) };
        bounded.Values(("total", new Value.TextValue(new string('a', 32)))); EntityQueryProperties.Capture(bounded, Selected("total"));
        Assert.Equal("WIRE_JSON_LIMIT", Assert.Throws<EntityJsonException>(() => Text(bounded)).Code);
    }

    [Fact] public void QueryTemporalValuesKeepTheirNativePrecisionAndFormat()
    {
        var node = new Node(); node.Values(("date", new Value.DateValue(new DateTime(2024, 2, 29))),
            ("time", new Value.TimeValue(new TimeSpan(12, 30, 0).Add(TimeSpan.FromTicks(1234567)))),
            ("instant", new Value.TimestampValue(1709210096789)),
            ("datetime", new Value.DateTimeValue(new DateTime(2024, 2, 29, 12, 34, 56, 789, DateTimeKind.Utc))));
        EntityQueryProperties.Capture(node, Selected("date", "time", "instant", "datetime"));
        using var json = JsonDocument.Parse(Text(node)); Assert.Equal("2024-02-29", json.RootElement.GetProperty("date").GetString());
        Assert.Equal("12:30:00.1234567", json.RootElement.GetProperty("time").GetString());
        Assert.Equal("2024-02-29T12:34:56.789Z", json.RootElement.GetProperty("instant").GetString());
        Assert.Equal("2024-02-29T12:34:56.789Z", json.RootElement.GetProperty("datetime").GetString());
    }

    [Fact] public void CompositeQueryValuesRespectTheContainerDepthLimit()
    {
        Value value = new Value.I64Value(1); for (var i = 0; i < 64; i++) value = new Value.ListValue(new List<Value> { value });
        var node = new Node(); node.Values(("nested", value)); EntityQueryProperties.Capture(node, Selected("nested"));
        Assert.Equal("WIRE_JSON_LIMIT", Assert.Throws<EntityJsonException>(() => Text(node)).Code);
    }
}
