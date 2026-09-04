namespace TeaQL.Runtime;

public sealed record WireFieldMetadata(string CanonicalName, string WireName, IReadOnlyList<string> Aliases);
public sealed record WireEntityMetadata(string EntityType, JsonFieldNamingProfile Profile,
    IReadOnlyDictionary<string, WireFieldMetadata> Fields);
public sealed record NormalizedWireInput(IReadOnlyDictionary<string, object?> Values,
    IReadOnlyDictionary<string, string> SourceInstancePaths);

public sealed class WireInputException(string code, string instancePath, string message) : ArgumentException(message)
{
    public string Code { get; } = code;
    public string InstancePath { get; } = instancePath;
}

public static class WireFields
{
    public static WireEntityMetadata CreateMetadata(string entityType, IEnumerable<string> canonicalFields,
        JsonFieldNamingProfile profile=JsonFieldNamingProfile.CamelCase,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? aliases=null)
    {
        var fields=new Dictionary<string, WireFieldMetadata>(); var spellings=new Dictionary<string,string>();
        foreach(var canonical in canonicalFields){var field=new WireFieldMetadata(canonical,Render(canonical,profile),aliases?.GetValueOrDefault(canonical) ?? []);
            foreach(var spelling in new[]{field.WireName}.Concat(field.Aliases)){if(spellings.TryGetValue(spelling,out var previous)&&previous!=canonical)throw new ArgumentException($"Wire field spelling '{spelling}' maps to both '{previous}' and '{canonical}'");spellings[spelling]=canonical;}fields[canonical]=field;}
        return new WireEntityMetadata(entityType,profile,fields);
    }

    public static NormalizedWireInput Normalize(IReadOnlyDictionary<string,object?> input,WireEntityMetadata metadata,string parentPointer="")
    {
        var lookup=new Dictionary<string,WireFieldMetadata>();foreach(var field in metadata.Fields.Values){lookup[field.WireName]=field;foreach(var alias in field.Aliases)lookup[alias]=field;}
        var values=new Dictionary<string,object?>();var paths=new Dictionary<string,string>();var submitted=new Dictionary<string,string>();
        foreach(var (name,value) in input){var pointer=$"{parentPointer}/{Escape(name)}";if(!lookup.TryGetValue(name,out var field))throw new WireInputException("WIRE_UNKNOWN_FIELD",pointer,$"Unknown {metadata.EntityType} field '{name}'");if(submitted.TryGetValue(field.CanonicalName,out var previous))throw new WireInputException("WIRE_FIELD_COLLISION",pointer,$"Fields '{previous}' and '{name}' both map to canonical field '{field.CanonicalName}'");submitted[field.CanonicalName]=name;values[field.CanonicalName]=value;if(name!=field.WireName)paths[field.CanonicalName]=pointer;}
        return new NormalizedWireInput(values,paths);
    }

    public static IReadOnlyList<CheckResult> RetainSubmittedPaths(IEnumerable<CheckResult> results,NormalizedWireInput normalized)=>results.Select(result=>{var canonical=result.Location.Segments.OfType<ObjectLocationSegment.Property>().FirstOrDefault()?.Name;return new CheckResult{RuleId=result.RuleId,Location=result.Location,EntityType=result.EntityType,SourceInstancePath=canonical is not null&&normalized.SourceInstancePaths.TryGetValue(canonical,out var path)?path:result.SourceInstancePath,InputValue=result.InputValue,SystemValue=result.SystemValue,Message=result.Message};}).ToList();
    private static string Escape(string value)=>value.Replace("~","~0").Replace("/","~1");
    private static string Render(string value,JsonFieldNamingProfile profile){if(profile==JsonFieldNamingProfile.SnakeCase)return value;var parts=value.Split('_');var camel=parts[0]+string.Concat(parts.Skip(1).Select(x=>char.ToUpperInvariant(x[0])+x[1..]));return profile==JsonFieldNamingProfile.PascalCase?char.ToUpperInvariant(camel[0])+camel[1..]:camel;}
}
