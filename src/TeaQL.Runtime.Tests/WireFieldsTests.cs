namespace TeaQL.Runtime.Tests;
public class WireFieldsTests
{
    private static WireEntityMetadata Metadata()=>WireFields.CreateMetadata("School",["name","school_type"],aliases:new Dictionary<string,IReadOnlyList<string>>{{"school_type",["school_type"]}});
    [Fact] public void NormalizesDeclaredAliasAndRetainsProvenance(){var normalized=WireFields.Normalize(new Dictionary<string,object?>{{"school_type",1001}},Metadata(),"/school");Assert.Equal(1001,normalized.Values["school_type"]);var result=WireFields.RetainSubmittedPaths([new CheckResult{RuleId="required",Location=ObjectLocation.Property("school_type")}],normalized);Assert.Equal("/school/school_type",result[0].SourceInstancePath);}
    [Fact] public void RejectsUnknownAndCollision(){var unknown=Assert.Throws<WireInputException>(()=>WireFields.Normalize(new Dictionary<string,object?>{{"bad/name",1}},Metadata()));Assert.Equal(("WIRE_UNKNOWN_FIELD","/bad~1name"),(unknown.Code,unknown.InstancePath));var collision=Assert.Throws<WireInputException>(()=>WireFields.Normalize(new Dictionary<string,object?>{{"schoolType",1},{"school_type",2}},Metadata()));Assert.Equal("WIRE_FIELD_COLLISION",collision.Code);}
}
