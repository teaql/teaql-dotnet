using System.Globalization;
using System.Text.Json;
using TeaQL.Core;

namespace TeaQL.Runtime.Tests;

public class EntityJsonTests
{
    private sealed class Node : IEntityJsonDocument
    {
        public Dictionary<string, object?> Data { get; } = new(StringComparer.Ordinal);
        public EntityJsonSchema EntityJsonSchema => Schema;
        public IEnumerable<KeyValuePair<string, object?>> LoadedJsonFields() => Data;
    }

    private static readonly EntityJsonSchema Schema = new("Node", [
        new("id", EntityJsonKind.Id), new("version", EntityJsonKind.Version),
        new("display_name", EntityJsonKind.Text, "DisplayNameColumn"), new("active", EntityJsonKind.Boolean),
        new("count_value", EntityJsonKind.Integer), new("long_value", EntityJsonKind.Long),
        new("amount", EntityJsonKind.Decimal), new("float_value", EntityJsonKind.Float), new("double_value", EntityJsonKind.Double),
        new("calendar_date", EntityJsonKind.Date), new("wall_time", EntityJsonKind.Time), new("occurred_at", EntityJsonKind.DateTime),
        new("parent", EntityJsonKind.Reference, TargetSchema: () => Schema!),
        new("children", EntityJsonKind.Many, "ChildRows", () => Schema!)]);

    [Theory]
    [InlineData(JsonFieldNamingProfile.CamelCase, "longValue", "displayName")]
    [InlineData(JsonFieldNamingProfile.SnakeCase, "long_value", "display_name")]
    [InlineData(JsonFieldNamingProfile.PascalCase, "LongValue", "DisplayName")]
    public void ExactNativeValuesAndPresenceRoundTrip(JsonFieldNamingProfile profile, string longName, string displayName)
    {
        var original = new Node();
        original.Data["id"] = 7L; original.Data["version"] = 2L;
        original.Data["DisplayNameColumn"] = ""; original.Data["active"] = false;
        original.Data["count_value"] = (long)int.MinValue; original.Data["long_value"] = long.MaxValue;
        original.Data["amount"] = 123456789012.3456789m;
        original.Data["float_value"] = 1.25; original.Data["double_value"] = -0.125;
        original.Data["calendar_date"] = new DateTime(2024, 2, 29);
        original.Data["wall_time"] = new TimeSpan(23, 59, 59).Add(TimeSpan.FromTicks(1234567));
        original.Data["occurred_at"] = new DateTime(2024, 2, 29, 12, 34, 56, 789, DateTimeKind.Utc);
        original.Data["parent"] = 1L; original.Data["ChildRows"] = Array.Empty<Node>();
        var json = Schema.Serialize(original, profile);
        using var wire = JsonDocument.Parse(json);
        Assert.Equal("9223372036854775807", wire.RootElement.GetProperty(longName).GetString());
        Assert.Equal("", wire.RootElement.GetProperty(displayName).GetString());
        var restored = Schema.Parse(json, profile);
        foreach (var (field, value) in original.Data)
        {
            if (field == "ChildRows") Assert.Empty(Assert.IsType<Value.ListValue>(restored[field]).Values);
            else Assert.Equal(value, restored[field].Raw);
        }
        Assert.DoesNotContain("ledger", json); Assert.DoesNotContain("Loaded", json);
    }

    [Fact] public void LoadedNullAndAbsentRemainDistinct()
    {
        var original = new Node(); original.Data["id"] = 1L; original.Data["DisplayNameColumn"] = null;
        var restored = Schema.Parse(Schema.Serialize(original));
        Assert.IsType<Value.NullValue>(restored["DisplayNameColumn"]);
        Assert.False(restored.ContainsKey("amount")); Assert.False(restored.ContainsKey("ChildRows"));
    }

    [Fact] public void NestedTypedRecordsRetainSparseFieldsAndCanonicalRecordKeys()
    {
        var child = new Node(); child.Data["id"] = 8L; child.Data["amount"] = 3.50m;
        var original = new Node(); original.Data["id"] = 1L; original.Data["parent"] = child;
        original.Data["ChildRows"] = new[] { child };
        var restored = Schema.Parse(Schema.Serialize(original));
        var nested = Assert.IsType<Value.ObjectValue>(restored["parent"]).Value;
        Assert.Equal(3.50m, nested["amount"].Raw); Assert.False(nested.ContainsKey("DisplayNameColumn"));
        var reverse = Assert.IsType<Value.ListValue>(restored["ChildRows"]);
        Assert.Equal(8L, Assert.IsType<Value.ObjectValue>(Assert.Single(reverse.Values)).Value["id"].Raw);
    }

    public static IEnumerable<object[]> BadScalars() => new[] {
        ("active", "1"), ("countValue", "true"), ("countValue", "1.5"), ("countValue", "2147483648"),
        ("longValue", "9007199254740993"), ("longValue", "\"9223372036854775808\""),
        ("longValue", "\"01\""), ("longValue", "\"+1\""), ("longValue", "\" 1\""),
        ("id", "\"-1\""), ("version", "\"9223372036854775808\""),
        ("amount", "1.25"), ("amount", "\"NaN\""), ("amount", "\"1e3\""), ("amount", "\"+1\""),
        ("amount", "\"0.12345678901234567890123456789\""), ("amount", "\"1234567890123456789012345678.91\""),
        ("displayName", "3"), ("doubleValue", "\"1.25\""), ("doubleValue", "1e9999"),
        ("calendarDate", "\"2024-02-30\""), ("calendarDate", "\"2024-W09-4\""),
        ("wallTime", "\"24:00:00\""), ("wallTime", "\"12:00:00Z\""), ("wallTime", "\"12:00:00.12345678\""),
        ("occurredAt", "\"2024-01-01T00:00:00\""), ("occurredAt", "\"2024-01-01T00:00:00.0000001Z\""),
        ("parent", "4"), ("children", "{}"), ("children", "null")
    }.Select(item => new object[] { item.Item1, item.Item2 });

    [Theory] [MemberData(nameof(BadScalars))]
    public void InvalidScalarsFailWithoutCoercionOrValueDisclosure(string field, string literal)
    {
        var error = Assert.Throws<EntityJsonException>(() => Schema.Parse("{\"" + field + "\":" + literal + "}"));
        Assert.Equal("/" + field, error.InstancePath); Assert.StartsWith("WIRE_", error.Code);
        Assert.DoesNotContain(literal, error.Message);
    }

    [Theory]
    [InlineData("{\"displayName\":\"a\",\"displayName\":\"b\"}", "WIRE_FIELD_COLLISION")]
    [InlineData("{\"displayName\":\"a\",\"display_name\":\"b\"}", "WIRE_FIELD_COLLISION")]
    [InlineData("{\"_entityRoot\":{}}", "WIRE_UNKNOWN_FIELD")]
    [InlineData("{\"\":1}", "WIRE_UNKNOWN_FIELD")]
    [InlineData("{\" \":1}", "WIRE_UNKNOWN_FIELD")]
    [InlineData("{\"\\t\":1}", "WIRE_UNKNOWN_FIELD")]
    [InlineData("{\"unknown\":1}", "WIRE_UNKNOWN_FIELD")]
    [InlineData("{\"floatValue\":NaN}", "WIRE_INVALID_JSON")]
    [InlineData("[]", "WIRE_ENTITY_OBJECT_REQUIRED")]
    [InlineData("null", "WIRE_ENTITY_OBJECT_REQUIRED")]
    public void AmbiguousPrivateOrInvalidInputsFailClosed(string text, string code) =>
        Assert.Equal(code, Assert.Throws<EntityJsonException>(() => Schema.Parse(text)).Code);

    [Theory] [InlineData(double.NaN)] [InlineData(double.PositiveInfinity)] [InlineData(double.NegativeInfinity)]
    public void NativeNonfiniteValuesCannotBeEncoded(double value)
    {
        foreach (var field in new[] { "float_value", "double_value" })
        {
            var original = new Node(); original.Data[field] = value;
            var error = Assert.Throws<EntityJsonException>(() => Schema.Serialize(original));
            Assert.Equal(field, error.Location.ModelPath);
        }
    }

    [Fact] public void NestedErrorsPreserveCanonicalLocationAndSubmittedJsonPointer()
    {
        var error = Assert.Throws<EntityJsonException>(() => Schema.Parse("{\"children\":[{\"countValue\":true}]}"));
        Assert.Equal("children[0].count_value", error.Location.ModelPath);
        Assert.Equal("/children/0/countValue", error.InstancePath);
    }

    [Fact] public void CyclesAreRejectedWithoutRecursionOverflow()
    {
        var original = new Node(); original.Data["parent"] = original;
        var error = Assert.Throws<EntityJsonException>(() => Schema.Serialize(original));
        Assert.Equal("WIRE_ENTITY_CYCLE", error.Code); Assert.Equal("/parent", error.InstancePath);
    }

    [Theory] [InlineData(false, 64)] [InlineData(true, 32)]
    public void EncoderAndDecoderShareTheSameContainerDepthBoundary(bool reverse, int maximumObjects)
    {
        static Node Chain(bool reverse, int objects)
        {
            var node = new Node(); node.Data["id"] = 1L;
            for (var index = 1; index < objects; index++)
            {
                var parent = new Node();
                if (reverse) parent.Data["ChildRows"] = new[] { node }; else parent.Data["parent"] = node;
                node = parent;
            }
            return node;
        }
        Assert.NotEmpty(Schema.Parse(Schema.Serialize(Chain(reverse, maximumObjects))));
        Assert.Equal("WIRE_JSON_LIMIT", Assert.Throws<EntityJsonException>(() => Schema.Serialize(Chain(reverse, maximumObjects + 1))).Code);
    }

    [Fact] public void DecimalScaleAndCultureAreIndependentOfMachineLocale()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            var original = new Node(); original.Data["amount"] = 3.50m;
            var json = Schema.Serialize(original); Assert.Contains("\"3.50\"", json);
            var value = Assert.IsType<Value.DecimalValue>(Schema.Parse(json)["amount"]).Value;
            Assert.Equal(decimal.GetBits(3.50m), decimal.GetBits(value));
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }

    [Fact] public void DateTimeUsesUtcAndRejectsNativePrecisionLoss()
    {
        var record = Schema.Parse("{\"occurredAt\":\"2024-02-29T20:34:56.789+08:00\"}");
        Assert.Equal(new DateTime(2024, 2, 29, 12, 34, 56, 789, DateTimeKind.Utc), record["occurred_at"].Raw);
        foreach (var value in new[] { new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(1), new DateTime(2024, 1, 1) })
        { var original = new Node(); original.Data["occurred_at"] = value; Assert.Throws<EntityJsonException>(() => Schema.Serialize(original)); }
    }

    [Fact] public void ByteLimitsAndMalformedUnicodeAreExplicit()
    {
        var small = new EntityJsonSchema("Small", [new("display_name", EntityJsonKind.Text)], maxJsonBytes: 64);
        var tooLarge = Assert.Throws<EntityJsonException>(() => small.Parse("{\"displayName\":\"" + new string('精', 30) + "\"}"));
        Assert.Equal("WIRE_JSON_LIMIT", tooLarge.Code);
        var original = new Node(); original.Data["DisplayNameColumn"] = "\ud800";
        Assert.Equal("WIRE_INVALID_TEXT", Assert.Throws<EntityJsonException>(() => Schema.Serialize(original)).Code);
        Assert.Equal("WIRE_INVALID_TEXT", Assert.Throws<EntityJsonException>(() => Schema.Parse("{\"displayName\":\"\\ud800\"}")).Code);
        Assert.Equal("WIRE_INVALID_TEXT", Assert.Throws<EntityJsonException>(() => Schema.Parse("{\"\\ud800\":1}")).Code);
    }
}
