using System.Text.Json;
using System.Text.Json.Serialization;

namespace TeaQL.Runtime;

/// <summary>Value-free generated native calls; canonical paths are never parsed from strings.</summary>
public sealed class ExpressionPath
{
    private sealed record Step(string Call, ObjectLocation Location);
    private readonly string _root;
    private readonly JsonFieldNamingProfile _profile;
    private readonly IReadOnlyList<Step> _steps;
    private readonly int? _missing;
    public ObjectLocation Location { get; }

    private ExpressionPath(string root, JsonFieldNamingProfile profile, ObjectLocation location,
        IReadOnlyList<Step> steps, int? missing = null)
    { _root = root; _profile = profile; Location = location; _steps = steps; _missing = missing; }

    public static ExpressionPath Root(string expression, JsonFieldNamingProfile profile = JsonFieldNamingProfile.CamelCase) =>
        new(expression, profile, ObjectLocation.Root(), []);
    public ExpressionPath Member(string name, string method) => Append(method + "()", Location.PropertyAt(name));
    public ExpressionPath At(int index, bool first = false) => index < 0
        ? Operation($"Get({index})")
        : Append(first ? "First()" : $"Get({index})", Location.Index(index));
    public ExpressionPath Operation(string call) => Append(call, Location);
    private ExpressionPath Append(string call, ObjectLocation location) =>
        new(_root, _profile, location, _steps.Concat([new Step(call, location)]).ToArray(), _missing);
    public ExpressionPath MarkMissing() => new(_root, _profile, Location, _steps, _missing ?? _steps.Count - 1);
    public ExpressionPath WithFailureFrom(ExpressionPath failed) => new(_root, _profile, Location, _steps, failed._missing);
    public string Render(bool broken = false) => _root + string.Concat(_steps.Select((step, index) =>
        "." + step.Call + (broken && index == _missing ? "<broken>" : "")));

    public NotLoadedDiagnostic Diagnostic(string fallback)
    {
        var failure = _missing is int index ? _steps[index].Location : Location;
        var last = failure.Segments.LastOrDefault();
        var location = WireLocation(failure);
        return new("NOT_LOADED", location, failure.InstancePathWith(_profile),
            last is ObjectLocationSegment.Property property ? property.Name : fallback,
            WireLocation(Location), Location.InstancePathWith(_profile), Math.Max(0, location.Count - 1),
            Render(), Render(true), new("preload", location));
    }

    private static IReadOnlyList<ExpressionWireLocationSegment> WireLocation(ObjectLocation location) =>
        location.Segments.Select(segment => segment switch
        {
            ObjectLocationSegment.Property property => new ExpressionWireLocationSegment("property", property.Name, null),
            ObjectLocationSegment.Index index => new ExpressionWireLocationSegment("index", null, index.Value),
            _ => throw new InvalidOperationException("Unsupported expression location segment")
        }).ToArray();
}

public sealed record ExpressionWireLocationSegment(
    [property: JsonPropertyName("kind")] string Kind,
    [property: JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Name,
    [property: JsonPropertyName("index"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? Index);
public sealed record ExpressionPreloadGuidance(
    [property: JsonPropertyName("action")] string Action,
    [property: JsonPropertyName("location")] IReadOnlyList<ExpressionWireLocationSegment> Location);
public sealed record NotLoadedDiagnostic(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("location")] IReadOnlyList<ExpressionWireLocationSegment> Location,
    [property: JsonPropertyName("instancePath")] string InstancePath,
    [property: JsonPropertyName("breakPoint")] string BreakPoint,
    [property: JsonPropertyName("attemptedLocation")] IReadOnlyList<ExpressionWireLocationSegment> AttemptedLocation,
    [property: JsonPropertyName("attemptedInstancePath")] string AttemptedInstancePath,
    [property: JsonPropertyName("breakPointIndex")] int BreakPointIndex,
    [property: JsonPropertyName("originalExpression")] string OriginalExpression,
    [property: JsonPropertyName("expressionWithBrokenPoint")] string ExpressionWithBrokenPoint,
    [property: JsonPropertyName("suggestedFix")] ExpressionPreloadGuidance SuggestedFix);

public sealed class TeaQLNotLoadedException : InvalidOperationException
{
    public ExpressionPath Path { get; }
    public string BreakPoint { get; }
    public TeaQLNotLoadedException(ExpressionPath path, string breakPoint)
        : base("TeaQLNotLoadedError: " + path.Render(true) + "\ndiagnostic: " + JsonSerializer.Serialize(path.Diagnostic(breakPoint)))
    { Path = path; BreakPoint = breakPoint; }
    public TeaQLNotLoadedException WithPath(ExpressionPath path) => new(path.WithFailureFrom(Path), BreakPoint);
    public TeaQLNotLoadedException Terminal(string call) => new(Path.Operation(call), BreakPoint);
    public NotLoadedDiagnostic ToWire() => Path.Diagnostic(BreakPoint);
}
