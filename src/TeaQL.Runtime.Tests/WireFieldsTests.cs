namespace TeaQL.Runtime.Tests;
public class WireFieldsTests
{
    private static WireEntityMetadata Metadata()=>WireFields.CreateMetadata("School",["name","school_type"],aliases:new Dictionary<string,IReadOnlyList<string>>{{"school_type",["school_type"]}});
    [Fact] public void NormalizesDeclaredAliasAndRetainsProvenance(){var normalized=WireFields.Normalize(new Dictionary<string,object?>{{"school_type",1001}},Metadata(),"/school");Assert.Equal(1001,normalized.Values["school_type"]);var result=WireFields.RetainSubmittedPaths([new CheckResult{RuleId="required",Location=ObjectLocation.Property("school_type")}],normalized);Assert.Equal("/school/school_type",result[0].SourceInstancePath);}
    [Fact] public void RejectsUnknownAndCollision(){var unknown=Assert.Throws<WireInputException>(()=>WireFields.Normalize(new Dictionary<string,object?>{{"bad/name",1}},Metadata()));Assert.Equal(("WIRE_UNKNOWN_FIELD","/bad~1name"),(unknown.Code,unknown.InstancePath));var collision=Assert.Throws<WireInputException>(()=>WireFields.Normalize(new Dictionary<string,object?>{{"schoolType",1},{"school_type",2}},Metadata()));Assert.Equal("WIRE_FIELD_COLLISION",collision.Code);}

    [Fact] public void ParentAliasNeverTruncatesDescendantProvenance()
    {
        var meta = WireFields.CreateMetadata("Order", ["order_item_list", "product_name"], aliases:new Dictionary<string,IReadOnlyList<string>> {
            {"order_item_list", ["legacy/items"]}, {"product_name", ["legacy~name"]} });
        var normalized = WireFields.Normalize(new Dictionary<string,object?> {{"legacy/items",null}, {"legacy~name",null}}, meta);
        var location = ObjectLocation.Property("order_item_list").Index(2).PropertyAt("product_name");
        var exact = "/legacy~1items/2/legacy~0name";
        var original = new CheckResult { RuleId="required", Location=location, SourceInstancePath=exact };
        var result = WireFields.RetainSubmittedPaths([
            original, new CheckResult {RuleId="required", Location=location},
            new CheckResult {RuleId="required", Location=ObjectLocation.Root().Index(2).PropertyAt("product_name")},
            new CheckResult {RuleId="required", Location=ObjectLocation.Property("order_item_list")}
        ], normalized);
        Assert.Equal(new string?[] {exact,null,null,"/legacy~1items"}, result.Select(r=>r.SourceInstancePath));
        Assert.Equal(exact, original.SourceInstancePath);
    }

    [Theory]
    [InlineData(JsonFieldNamingProfile.CamelCase)]
    [InlineData(JsonFieldNamingProfile.SnakeCase)]
    [InlineData(JsonFieldNamingProfile.PascalCase)]
    public void StandardChildRetainsAncestorProvenance(JsonFieldNamingProfile profile)
    {
        var parent = WireFields.CreateMetadata("Order", ["order_item_list"], aliases:new Dictionary<string,IReadOnlyList<string>> {
            {"order_item_list", ["legacy/items~"]} });
        var child = WireFields.CreateMetadata("OrderItem", ["product_name", "unit_count"], profile);
        var name = child.Fields["product_name"].WireName;
        foreach (var value in new object?[] {null, "submitted item"})
        {
            var childObject = new Dictionary<string,object?> {{name,value}};
            var parentInput = WireFields.Normalize(new Dictionary<string,object?> {{"legacy/items~",new object?[] {null,null,childObject}}}, parent);
            var boundary = parentInput.SourceInstancePaths["order_item_list"] + "/2";
            var normalized = WireFields.Normalize(childObject, child, boundary);
            Assert.Single(normalized.Values);
            Assert.Equal(value, normalized.Values["product_name"]);
            Assert.Single(normalized.SourceInstancePaths);
            Assert.Equal(boundary + "/" + name, normalized.SourceInstancePaths["product_name"]);
            var original = new CheckResult {RuleId="required", EntityType="OrderItem", Location=ObjectLocation.Property("product_name")};
            var leaf = WireFields.RetainSubmittedPaths([original], normalized)[0];
            var location = ObjectLocation.Property("order_item_list").Index(2).PropertyAt("product_name");
            var prefixed = new CheckResult {RuleId=leaf.RuleId, EntityType=leaf.EntityType, Location=location, SourceInstancePath=leaf.SourceInstancePath};
            var actual = WireFields.RetainSubmittedPaths([prefixed], parentInput)[0];
            Assert.Equal("/legacy~1items~0/2/" + name, actual.SourceInstancePath);
            Assert.Same(location, actual.Location);
            Assert.Null(original.SourceInstancePath);
        }
        Assert.Empty(WireFields.Normalize(new Dictionary<string,object?> {{name,null}}, child).SourceInstancePaths);
    }
}
