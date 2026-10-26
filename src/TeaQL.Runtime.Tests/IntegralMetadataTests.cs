using TeaQL.Core;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class IntegralMetadataTests
{
    [Fact]
    public void IntegralMetadataPreservesLogicalWidthWithoutChangingNativeCarrier()
    {
        var module = new RuntimeModule(["Probe"], new Dictionary<string, IEntityChecker>(),
            new Dictionary<string, Record> { ["Probe"] = new() {
                ["required_integer"] = new Value.I64Value(0), ["required_long"] = new Value.I64Value(0),
                ["id"] = new Value.I64Value(0), ["version"] = new Value.I64Value(0),
                ["type_platform"] = new Value.I64Value(0) } },
            new Dictionary<string, IReadOnlyDictionary<string, bool>>(),
            integer32Fields: new Dictionary<string, IReadOnlyList<string>> { ["Probe"] = ["required_integer"] });
        var schema = module.Metadata.GetEntity("Probe")!;
        Assert.True(schema.PropertyByName("required_integer")!.IsInt32);
        Assert.Equal(DataType.I64, schema.PropertyByName("required_integer")!.DataType);
        foreach (var field in new[] { "required_long", "id", "version", "type_platform" })
            Assert.False(schema.PropertyByName(field)!.IsInt32);
    }

    [Theory]
    [InlineData("id")]
    [InlineData("version")]
    [InlineData("type_platform")]
    [InlineData("text")]
    public void IntegralMetadataRejectsIdentityVersionReferenceOrNonIntegerNarrowing(string field)
    {
        var error = Assert.Throws<ArgumentException>(() => new RuntimeModule(["Probe"], new Dictionary<string, IEntityChecker>(),
            new Dictionary<string, Record> { ["Probe"] = new() {
                ["id"] = new Value.I64Value(0), ["version"] = new Value.I64Value(0),
                ["type_platform"] = new Value.I64Value(0), ["text"] = new Value.TextValue("") } },
            new Dictionary<string, IReadOnlyDictionary<string, bool>>(),
            new Dictionary<string, IReadOnlyList<RelationDescriptor>> { ["Probe"] = [
                RelationDescriptor.New("platform", "Platform").LocalKey("type_platform").ForeignKey("id")] },
            integer32Fields: new Dictionary<string, IReadOnlyList<string>> { ["Probe"] = [field] }));
        Assert.Equal("integer32Fields", error.ParamName);
    }
}
