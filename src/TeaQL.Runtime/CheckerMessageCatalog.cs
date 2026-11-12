using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace TeaQL.Runtime;

public enum CheckerMessageShape { One, Two, Deep }

internal sealed record CheckerTemplateNode(string? Literal, string? Argument);
internal sealed class CheckerMessageTemplate(IReadOnlyList<CheckerTemplateNode> nodes)
{
    internal IReadOnlyList<CheckerTemplateNode> Nodes { get; } = Array.AsReadOnly(nodes.ToArray());
    internal static CheckerMessageTemplate Compile(string source, IEnumerable<string> allowed, IEnumerable<string> required)
    {
        CheckerMessageJson.Text(source);
        var accepted = allowed.ToHashSet(StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<CheckerTemplateNode>(); var literal = new StringBuilder();
        for (var offset = 0; offset < source.Length;)
        {
            var value = source[offset];
            if ((value == '{' || value == '}') && offset + 1 < source.Length && source[offset + 1] == value)
            { literal.Append(value); offset += 2; continue; }
            if (value == '}') throw CheckerMessageJson.Invalid();
            if (value != '{') { literal.Append(value); offset++; continue; }
            var end = source.IndexOf('}', offset + 1); if (end < 0) throw CheckerMessageJson.Invalid();
            var name = source[(offset + 1)..end]; if (!accepted.Contains(name)) throw CheckerMessageJson.Invalid();
            if (literal.Length != 0) result.Add(new(literal.ToString(), null));
            result.Add(new(null, name)); literal.Clear(); seen.Add(name); offset = end + 1;
        }
        if (literal.Length != 0) result.Add(new(literal.ToString(), null));
        if (required.Any(name => !seen.Contains(name))) throw CheckerMessageJson.Invalid();
        return new(result);
    }
    internal CheckerMessageTemplate Project(Func<string, string> project) =>
        new(Nodes.Select(node => node.Literal is { } literal ? new CheckerTemplateNode(project(literal), null) : node).ToArray());
    internal string Render(IReadOnlyDictionary<string, string> arguments) => string.Concat(Nodes.Select(node =>
        node.Literal ?? (arguments.TryGetValue(node.Argument!, out var value) ? value : "-")));
}

internal static class CheckerMessageJson
{
    internal static ArgumentException Invalid() => new("Invalid Checker message resource");
    internal static string Text(string? value) => !string.IsNullOrWhiteSpace(value) ? value : throw Invalid();
    internal static string Text(JsonElement value) => value.ValueKind == JsonValueKind.String ? Text(value.GetString()) : throw Invalid();
    internal static void Object(JsonElement value, params string[] allowed)
    {
        if (value.ValueKind != JsonValueKind.Object || value.EnumerateObject().Any(p => !allowed.Contains(p.Name, StringComparer.Ordinal))) throw Invalid();
    }
    internal static void Unique(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Object)
        {
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in value.EnumerateObject())
            { if (!keys.Add(property.Name)) throw Invalid(); Unique(property.Value); }
        }
        else if (value.ValueKind == JsonValueKind.Array) foreach (var item in value.EnumerateArray()) Unique(item);
    }
    internal static IReadOnlyDictionary<TKey, TValue> Freeze<TKey, TValue>(Dictionary<TKey, TValue> values) where TKey : notnull =>
        new ReadOnlyDictionary<TKey, TValue>(values);
    internal static string Resource(string name)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TeaQL.Runtime." + name)
            ?? throw new InvalidOperationException("Missing built-in Checker resource");
        using var reader = new StreamReader(stream); return reader.ReadToEnd();
    }
}

public sealed class CheckerMessageSelection
{
    public TeaQLLocale Locale { get; }
    internal CheckerMessageTemplate Template { get; }
    internal bool Application { get; }
    internal CheckerMessageSelection(TeaQLLocale locale, CheckerMessageTemplate template, bool application = false)
    { Locale = locale; Template = template; Application = application; }
    public string Render(IReadOnlyDictionary<string, string> arguments) => Template.Render(arguments);
    internal CheckerMessageSelection Project(Func<string, string> project) => new(Locale, Template.Project(project), Application);
}

/// <summary>Immutable, compiled, data-only templates. No EL, format execution or recursive interpolation.</summary>
public sealed class CheckerMessageCatalog
{
    private sealed record Patterns(CheckerMessageTemplate Reference, CheckerMessageTemplate Item, string Separator);
    private sealed record Entry(IReadOnlyDictionary<string, IReadOnlyDictionary<CheckerMessageShape, CheckerMessageTemplate>> Rules, Patterns? Patterns);
    private readonly IReadOnlyDictionary<TeaQLLocale, Entry> _locales;
    private readonly bool _application;
    public static CheckerMessageCatalog Builtin { get; } = Parse(CheckerMessageJson.Resource("natural-messages-v1.json"), true);
    private CheckerMessageCatalog(IReadOnlyDictionary<TeaQLLocale, Entry> locales, bool application = false)
    { _locales = locales; _application = application; }
    public static CheckerMessageCatalog FromJson(string source) => Parse(source, false);
    private static CheckerMessageCatalog Parse(string source, bool complete)
    {
        using var document = JsonDocument.Parse(source); var root = document.RootElement; CheckerMessageJson.Unique(root);
        CheckerMessageJson.Object(root, "schema", "defaultLocale", "locales");
        if (root.GetProperty("schema").GetString() != "teaql.checker-messages/v1" || root.GetProperty("defaultLocale").GetString() != "en") throw CheckerMessageJson.Invalid();
        var entries = new Dictionary<TeaQLLocale, Entry>();
        foreach (var property in root.GetProperty("locales").EnumerateObject())
        {
            var locale = TeaQLLocales.Parse(property.Name); if (entries.ContainsKey(locale)) throw CheckerMessageJson.Invalid();
            var entry = property.Value; CheckerMessageJson.Object(entry, "rules", "locationPatterns");
            var rules = new Dictionary<string, IReadOnlyDictionary<CheckerMessageShape, CheckerMessageTemplate>>(StringComparer.Ordinal);
            if (entry.TryGetProperty("rules", out var rawRules)) foreach (var rule in rawRules.EnumerateObject())
            {
                if (!rule.Name.StartsWith("checker.", StringComparison.Ordinal) || rule.Name.Length == 8) throw CheckerMessageJson.Invalid();
                var shapes = new Dictionary<CheckerMessageShape, CheckerMessageTemplate>();
                foreach (var shapeEntry in rule.Value.EnumerateObject())
                {
                    var shape = shapeEntry.Name switch { "one" => CheckerMessageShape.One, "two" => CheckerMessageShape.Two, "deep" => CheckerMessageShape.Deep, _ => throw CheckerMessageJson.Invalid() };
                    var allowed = new HashSet<string>(["field", "system", "input", "input_len", "unit"], StringComparer.Ordinal);
                    var required = new HashSet<string>(["field"], StringComparer.Ordinal);
                    if (rule.Name is "checker.required" or "checker.invalid") allowed = ["field"];
                    else if (rule.Name is "checker.min" or "checker.max") { allowed = ["field", "system", "input"]; required.Add("system"); }
                    else if (rule.Name is "checker.minLength" or "checker.maxLength")
                    { allowed = ["field", "system", "input_len", "unit"]; required.UnionWith(["system", "input_len", "unit"]); }
                    if (shape != CheckerMessageShape.One) { var owner = shape == CheckerMessageShape.Two ? "object" : "objectPath"; allowed.Add(owner); required.Add(owner); }
                    shapes[shape] = CheckerMessageTemplate.Compile(CheckerMessageJson.Text(shapeEntry.Value), allowed, required);
                }
                rules[rule.Name] = CheckerMessageJson.Freeze(shapes);
            }
            Patterns? patterns = null;
            if (entry.TryGetProperty("locationPatterns", out var raw))
            {
                CheckerMessageJson.Object(raw, "reference", "item", "separator");
                var reference = CheckerMessageTemplate.Compile(CheckerMessageJson.Text(raw.GetProperty("reference")), ["relation"], ["relation"]);
                var item = CheckerMessageTemplate.Compile(CheckerMessageJson.Text(raw.GetProperty("item")), ["relation", "position", "ordinal", "classifier"], ["relation"]);
                if (!item.Nodes.Any(node => node.Argument is "position" or "ordinal")) throw CheckerMessageJson.Invalid();
                var separator = CheckerMessageJson.Text(raw.GetProperty("separator"));
                if (separator.Any(c => c is '{' or '}' || char.IsControl(c))) throw CheckerMessageJson.Invalid();
                patterns = new(reference, item, separator);
            }
            entries[locale] = new(CheckerMessageJson.Freeze(rules), patterns);
        }
        if (complete) foreach (var locale in Enum.GetValues<TeaQLLocale>())
        {
            if (!entries.TryGetValue(locale, out var entry) || entry.Patterns == null) throw CheckerMessageJson.Invalid();
            foreach (var rule in new[] { "checker.required", "checker.min", "checker.max", "checker.minLength", "checker.maxLength", "checker.invalid" })
                foreach (var shape in Enum.GetValues<CheckerMessageShape>()) if (!entry.Rules.TryGetValue(rule, out var shapes) || !shapes.ContainsKey(shape)) throw CheckerMessageJson.Invalid();
        }
        return new(CheckerMessageJson.Freeze(entries), !complete);
    }
    public CheckerMessageSelection? Lookup(TeaQLLocale locale, string rule, CheckerMessageShape shape) =>
        _locales.TryGetValue(locale, out var entry) && entry.Rules.TryGetValue(rule, out var shapes) && shapes.TryGetValue(shape, out var template)
            ? new(locale, template, _application) : null;
    internal CheckerMessageSelection? Override(TeaQLLocale locale, string rule, CheckerMessageShape shape) => _application ? Lookup(locale, rule, shape) : null;
    public CheckerMessageSelection Select(TeaQLLocale locale, string rule, CheckerMessageShape shape)
    {
        foreach (var candidate in new[] { locale, TeaQLLocale.English })
            if ((Lookup(candidate, rule, shape) ?? Builtin.Lookup(candidate, rule, shape)) is { } selected) return selected;
        return Builtin.Lookup(locale, "checker.invalid", shape)!;
    }
    private Patterns LocationPatterns(TeaQLLocale locale) => _locales.TryGetValue(locale, out var entry) && entry.Patterns != null ? entry.Patterns : Builtin._locales[locale].Patterns!;
    internal CheckerMessageCatalog ProjectPatterns(TeaQLLocale locale, Func<string, string> project)
    {
        if (ReferenceEquals(this, Builtin) || !_locales.TryGetValue(locale, out var entry) || entry.Patterns is not { } patterns) return Builtin;
        var safe = new Patterns(patterns.Reference.Project(project), patterns.Item.Project(project), project(patterns.Separator));
        return new(CheckerMessageJson.Freeze(new Dictionary<TeaQLLocale, Entry> { [locale] = new(new ReadOnlyDictionary<string, IReadOnlyDictionary<CheckerMessageShape, CheckerMessageTemplate>>(new Dictionary<string, IReadOnlyDictionary<CheckerMessageShape, CheckerMessageTemplate>>()), safe) }));
    }
    public string Reference(TeaQLLocale locale, string relation) => LocationPatterns(locale).Reference.Render(new Dictionary<string, string> { ["relation"] = relation });
    public string Item(TeaQLLocale locale, string relation, int index, string classifier)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index)); var position = (long)index + 1;
        return LocationPatterns(locale).Item.Render(new Dictionary<string, string> { ["relation"] = relation, ["position"] = position.ToString(CultureInfo.InvariantCulture), ["ordinal"] = Ordinal(position), ["classifier"] = classifier });
    }
    public string Separator(TeaQLLocale locale) => LocationPatterns(locale).Separator;
    public static string Ordinal(long position) => position.ToString(CultureInfo.InvariantCulture) +
        (position % 100 is >= 11 and <= 13 ? "th" : (position % 10) switch { 1 => "st", 2 => "nd", 3 => "rd", _ => "th" });
}
