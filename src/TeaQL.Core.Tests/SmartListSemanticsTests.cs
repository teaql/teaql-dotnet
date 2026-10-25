using System.Globalization;
using System.Text.Json.Nodes;

namespace TeaQL.Core.Tests;

public class SmartListSemanticsTests
{
    private static JsonNode Vectors() => JsonNode.Parse(File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "fixtures", "smart-list-semantics.json")))!;
    private static int[] Ints(JsonNode node) => node.AsArray().Select(value => value!.GetValue<int>()).ToArray();

    [Fact] public void StatesTotalsAndFallbackRemainDistinct()
    {
        foreach (var item in Vectors()["states"]!.AsArray())
        {
            var state = item!;
            var loaded = state["loaded"]!.GetValue<bool>();
            var rows = loaded ? new SmartList<int>(Ints(state["rows"]!)) : SmartList<int>.Empty();
            rows.TotalCount = state["total"]?.GetValue<long>();
            Assert.Equal(loaded, rows.IsLoaded); Assert.Equal(Ints(state["rows"]!), rows);
            Assert.Equal(state["total"]?.GetValue<long>(), rows.TotalCount);
            Assert.Equal(rows.TotalCount ?? rows.Count, rows.TotalCountOrLen());
            Assert.Equal(state["total"]?.GetValue<long>(), rows.TotalCount);
        }
        Assert.Throws<ArgumentOutOfRangeException>(() => new SmartList<int>().WithTotalCount(-1));
        Assert.False(SmartList<int>.Empty().Map(value => value).IsLoaded);
        Assert.False(SmartList<int>.Empty().Filter(_ => true).IsLoaded);
        Assert.False(SmartList<int>.Empty().FlatMap(value => new[] { value }).IsLoaded);
    }

    private static Record Nested(params long[] values) => new() {
        ["values"] = new Value.ListValue(values.Select(value => (Value)new Value.I64Value(value)).ToList()) };

    [Fact] public void MappingOwnsNestedValueTreesFacetsAndRecordFacetMetadata()
    {
        var exact = decimal.Parse(Vectors()["metadata"]!["decimal"]!["value"]!.GetValue<string>(), CultureInfo.InvariantCulture);
        var source = new SmartList<int>(new[] { 1, 2, 3 }).WithTotalCount(9)
            .WithAggregation("nested", new Value.ObjectValue(Nested(1, 2)))
            .WithAggregation("decimal", new Value.DecimalValue(exact)).WithAggregation("nullable", new Value.NullValue())
            .WithSummary("nested", new Value.ObjectValue(Nested(3)))
            .WithFacet("status", new SmartList<Record>(new[] { new Record { ["count"] = new Value.I64Value(3) } }))
            .WithFacet("empty", new SmartList<Record>());
        source.Facets["status"][0].QueryFacets.Add("nested", new SmartList<Record>());
        var mapped = source.Map(value => value * 10);
        Assert.Equal(new[] { 10, 20, 30 }, mapped); Assert.Equal(source.TotalCount, mapped.TotalCount);
        Assert.Equal(exact, Assert.IsType<Value.DecimalValue>(mapped.Aggregations["decimal"]).Value);
        Assert.IsType<Value.NullValue>(mapped.Aggregations["nullable"]); Assert.False(mapped.Aggregations.ContainsKey("absent"));
        Assert.IsType<Value.ListValue>(Assert.IsType<Value.ObjectValue>(mapped.Aggregations["nested"]).Value["values"]).Values.Add(new Value.I64Value(99));
        Assert.IsType<Value.ListValue>(Assert.IsType<Value.ObjectValue>(mapped.Summary["nested"]).Value["values"]).Values.Add(new Value.I64Value(99));
        mapped.Facets["status"][0]["count"] = new Value.I64Value(99);
        mapped.Facets["status"][0].QueryFacets.Clear(); mapped.RemoveFacet("empty");
        Assert.Equal(2, Assert.IsType<Value.ListValue>(Assert.IsType<Value.ObjectValue>(source.Aggregations["nested"]).Value["values"]).Values.Count);
        Assert.Single(Assert.IsType<Value.ListValue>(Assert.IsType<Value.ObjectValue>(source.Summary["nested"]).Value["values"]).Values);
        Assert.Equal(new Value.I64Value(3), source.Facets["status"][0]["count"]);
        Assert.True(source.Facets["status"][0].QueryFacets.ContainsKey("nested")); Assert.NotNull(source.Facet("empty"));
        var copy = source.ToList(value => value); copy[0] = 99; Assert.Equal(1, source[0]);
        foreach (var result in new[] { source.Filter(value => value % 2 == 1), source.Filter(_ => true), source.FlatMap(value => new[] { value, value }) })
        {
            Assert.True(result.IsLoaded); Assert.Null(result.TotalCount);
            Assert.Empty(result.Aggregations); Assert.Empty(result.Summary); Assert.Empty(result.Facets);
        }
        Assert.Equal(new[] { 1, 3 }, source.Filter(value => value % 2 == 1));
        Assert.Equal(new[] { 1, 1, 2, 2, 3, 3 }, source.FlatMap(value => new[] { value, value }));
    }

    private sealed record Keyed(string Key, int Value);
    private static Keyed[] KeyedRows(JsonNode node) => node.AsArray().Select(row =>
        new Keyed(row!["key"]!.GetValue<string>(), row["value"]!.GetValue<int>())).ToArray();

    [Fact] public void MergeDuplicatesAndNativeGroupOrderMatchSharedVectors()
    {
        var merge = Vectors()["merge"]!;
        var source = new SmartList<Keyed>(KeyedRows(merge["target"]!)).WithTotalCount(9);
        source.MergeBy(KeyedRows(merge["incoming"]!), item => item.Key);
        var expected = KeyedRows(merge["expected"]!); Assert.Equal(expected, source);
        Assert.Equal(new Keyed("a", 4), source.IdentityMap(item => item.Key)["a"]);
        Assert.Equal(expected.Take(2), source.GroupBy(item => item.Key)["a"]); Assert.Equal(9, source.TotalCount);
        foreach (var index in new[] { -1, source.Count, 100 }) Assert.False(source.TryGet(index, out _));
        Assert.Throws<ArgumentOutOfRangeException>(() => source[-1] = new Keyed("x", 1));
        Assert.False(SmartList<Keyed>.Empty().TryGet(0, out _));
        source.WithFacet("empty", new SmartList<Record>()); Assert.Null(source.Facet("absent"));
        Assert.NotNull(source.Facet("empty")); Assert.Null(source.RemoveFacet("absent"));
        Assert.Single(source.TakeFacets()); Assert.Empty(source.Facets);
    }

    private sealed record IdentityItem(Value Id, long Revision) : IIdentifiableEntity, IVersionedEntity
    {
        public static string EntityName => "IdentityItem";
        public static EntityDescriptor EntityDescriptor() => TeaQL.Core.EntityDescriptor.New("IdentityItem");
        public Record IntoRecord() => new() { ["id"] = Id, ["version"] = new Value.I64Value(Revision) };
        public Value IdValue() => Id;
        public long Version() => Revision;
    }

    [Fact] public void MissingIdsAreExtractedButNotIndexedAndRecordConversionKeepsMetadata()
    {
        var identity = Vectors()["identity"]!;
        var source = new SmartList<IdentityItem>(identity["rows"]!.AsArray().Select(row => {
            var id = row!["id"];
            Value value = id is null ? new Value.NullValue() :
                id.AsValue().TryGetValue<string>(out var text) ? new Value.TextValue(text) : new Value.I64Value(id.GetValue<long>());
            return new IdentityItem(value, row["version"]!.GetValue<long>());
        })).WithTotalCount(9).WithSummary("source", new Value.BoolValue(true));
        Assert.IsType<Value.NullValue>(source.Ids()[1]); Assert.Equal(new long[] { 1, 2, 3, 4, 5, 6 }, source.Versions());
        var indexed = source.MapById(); Assert.Equal(4, indexed.Count); Assert.False(indexed.ContainsKey("null"));
        var keys = new[] { "i:0", "i:1", "t:1", "t:" }; var positions = Ints(identity["indexedPositions"]!);
        for (var index = 0; index < keys.Length; index++) Assert.Same(source[positions[index]], indexed[keys[index]]);
        var records = source.IntoRecords(); Assert.Equal(9, records.TotalCount); records.Summary.Clear(); Assert.Single(source.Summary);
        Assert.Null(SmartList<IdentityItem>.Empty().IntoValue());
        Assert.Empty(Assert.IsType<Value.ListValue>(new SmartList<IdentityItem>().IntoValue()).Values);
    }
}
