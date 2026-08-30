namespace TeaQL.Runtime;

public abstract record ObjectLocationSegment
{
    private ObjectLocationSegment() { }

    public sealed record Property(string Name) : ObjectLocationSegment;
    public sealed record Index(int Value) : ObjectLocationSegment;
}

/// <summary>A casing-neutral checker location expressed with canonical KSML names.</summary>
public sealed record ObjectLocation
{
    private readonly IReadOnlyList<ObjectLocationSegment> _segments;

    public ObjectLocation() : this([]) { }

    private ObjectLocation(IReadOnlyList<ObjectLocationSegment> segments) => _segments = segments;

    public IReadOnlyList<ObjectLocationSegment> Segments => _segments;

    public static ObjectLocation Root() => new();

    public static ObjectLocation Property(string name) => Root().PropertyAt(name);

    public ObjectLocation PropertyAt(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("A KSML property name is required", nameof(name));
        return Append(new ObjectLocationSegment.Property(name));
    }

    public ObjectLocation Index(int index)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        return Append(new ObjectLocationSegment.Index(index));
    }

    public ObjectLocation PrefixedBy(ObjectLocation prefix)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        return new(prefix._segments.Concat(_segments).ToArray());
    }

    public string ModelPath => Render(static name => name);

    public string NativePath => Render(ToPascalCase);

    public string InstancePath => string.Concat(_segments.Select(segment => segment switch
    {
        ObjectLocationSegment.Property property => "/" + EscapePointer(ToLowerCamelCase(property.Name)),
        ObjectLocationSegment.Index index => "/" + index.Value,
        _ => throw new InvalidOperationException("Unsupported object-location segment")
    }));

    public override string ToString() => NativePath;

    public static implicit operator ObjectLocation(string property) => Property(property);

    private ObjectLocation Append(ObjectLocationSegment segment) =>
        new(_segments.Concat([segment]).ToArray());

    private string Render(Func<string, string> propertyName)
    {
        var result = "";
        foreach (var segment in _segments)
        {
            switch (segment)
            {
                case ObjectLocationSegment.Property property:
                    result += (result.Length == 0 ? "" : ".") + propertyName(property.Name);
                    break;
                case ObjectLocationSegment.Index index:
                    result += $"[{index.Value}]";
                    break;
            }
        }
        return result;
    }

    private static string ToPascalCase(string name)
    {
        var parts = name.Split('_');
        return string.Concat(parts.Select(Capitalize));
    }

    private static string ToLowerCamelCase(string name)
    {
        var parts = name.Split('_');
        if (parts.Length == 0) return "";
        return parts[0] + string.Concat(parts.Skip(1).Select(Capitalize));
    }

    private static string Capitalize(string value) => value.Length == 0
        ? ""
        : char.ToUpperInvariant(value[0]) + value[1..];

    private static string EscapePointer(string value) => value.Replace("~", "~0").Replace("/", "~1");
}
