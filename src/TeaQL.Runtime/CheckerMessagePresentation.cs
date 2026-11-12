using System.Globalization;
using System.Text;
using System.Text.Json;

namespace TeaQL.Runtime;

public enum CheckerMessageWarning { LabelFallback, GenericRule, RenderFallback }
public sealed record CheckerMessageOutput(string Message, IReadOnlyList<CheckerMessageWarning> Warnings);
internal sealed record CheckerMessageField(string Name, string? Target, IReadOnlyDictionary<TeaQLLocale, string> Labels, IReadOnlyDictionary<TeaQLLocale, string> Classifiers);
internal sealed record CheckerMessageEntity(IReadOnlyDictionary<TeaQLLocale, string> Labels, IReadOnlyDictionary<string, CheckerMessageField> Fields, string Fingerprint);

internal static class CheckerMessageWords
{
    private static readonly IReadOnlyDictionary<TeaQLLocale, IReadOnlyDictionary<string, string>> Data = Load();
    internal static string Get(TeaQLLocale locale, string key) => Data[locale][key];
    internal static string Sanitize(string value)
    {
        var result = new StringBuilder();
        foreach (var rune in value.EnumerateRunes()) result.Append(Rune.GetUnicodeCategory(rune) is UnicodeCategory.Control or UnicodeCategory.Format or UnicodeCategory.LineSeparator or UnicodeCategory.ParagraphSeparator ? " " : rune.ToString());
        return result.ToString().Trim();
    }
    internal static IReadOnlyDictionary<TeaQLLocale, string> Labels(JsonElement value)
    {
        var result = new Dictionary<TeaQLLocale, string>();
        if (value.ValueKind == JsonValueKind.Undefined) return CheckerMessageJson.Freeze(result);
        foreach (var property in value.EnumerateObject())
        { var locale = TeaQLLocales.Parse(property.Name); if (!result.TryAdd(locale, CheckerMessageJson.Text(property.Value))) throw CheckerMessageJson.Invalid(); }
        return CheckerMessageJson.Freeze(result);
    }
    private static IReadOnlyDictionary<TeaQLLocale, IReadOnlyDictionary<string, string>> Load()
    {
        using var document = JsonDocument.Parse(CheckerMessageJson.Resource("checker-presentation-v1.json"));
        if (document.RootElement.GetProperty("schema").GetString() != "teaql.checker-presentation/v1") throw CheckerMessageJson.Invalid();
        var result = new Dictionary<TeaQLLocale, IReadOnlyDictionary<string, string>>();
        foreach (var entry in document.RootElement.GetProperty("locales").EnumerateObject())
            result.Add(TeaQLLocales.Parse(entry.Name), CheckerMessageJson.Freeze(entry.Value.EnumerateObject().ToDictionary(p => p.Name, p => CheckerMessageJson.Text(p.Value), StringComparer.Ordinal)));
        foreach (var locale in Enum.GetValues<TeaQLLocale>()) foreach (var key in new[] { "currentObject", "utf16", "scalar", "byte" })
            if (!result.TryGetValue(locale, out var words) || !words.TryGetValue(key, out var text) || Sanitize(text) != text) throw CheckerMessageJson.Invalid();
        return CheckerMessageJson.Freeze(result);
    }
}

/// <summary>One immutable presentation model shared by all rows of a model revision.</summary>
public sealed class CheckerPresentationModel
{
    public string Revision { get; }
    private readonly IReadOnlyDictionary<string, CheckerMessageEntity> _entities;
    internal IReadOnlyDictionary<TeaQLLocale, string> Current { get; }
    public static CheckerPresentationModel Empty { get; } = new("unconfigured", new Dictionary<string, CheckerMessageEntity>(),
        Enum.GetValues<TeaQLLocale>().ToDictionary(locale => locale, locale => CheckerMessageWords.Get(locale, "currentObject")));
    private CheckerPresentationModel(string revision, Dictionary<string, CheckerMessageEntity> entities, Dictionary<TeaQLLocale, string> current)
    { Revision = revision; _entities = CheckerMessageJson.Freeze(entities); Current = CheckerMessageJson.Freeze(current); }
    internal CheckerMessageEntity? Entity(string? identity) => identity != null && _entities.TryGetValue(identity, out var entity) ? entity : null;
    public static CheckerPresentationModel FromJson(string source)
    {
        using var document = JsonDocument.Parse(source); var root = document.RootElement; CheckerMessageJson.Unique(root);
        CheckerMessageJson.Object(root, "schema", "revision", "entities", "currentObject");
        if (root.GetProperty("schema").GetString() != "teaql.checker-model/v1") throw CheckerMessageJson.Invalid();
        var entities = new Dictionary<string, CheckerMessageEntity>(StringComparer.Ordinal);
        object Sorted(IReadOnlyDictionary<TeaQLLocale, string> labels) => labels.OrderBy(p => p.Key.Code(), StringComparer.Ordinal).Select(p => new[] { p.Key.Code(), p.Value }).ToArray();
        foreach (var property in root.GetProperty("entities").EnumerateObject())
        {
            CheckerMessageJson.Text(property.Name); var raw = property.Value; CheckerMessageJson.Object(raw, "labels", "fields");
            var labels = CheckerMessageWords.Labels(raw.TryGetProperty("labels", out var rawLabels) ? rawLabels : default);
            var fields = new Dictionary<string, CheckerMessageField>(StringComparer.Ordinal); var canonical = new List<(string Name, object Value)>();
            foreach (var item in raw.GetProperty("fields").EnumerateArray())
            {
                CheckerMessageJson.Object(item, "name", "targetType", "labels", "classifiers", "aliases");
                var name = CheckerMessageJson.Text(item.GetProperty("name")); var target = item.TryGetProperty("targetType", out var targetValue) ? CheckerMessageJson.Text(targetValue) : null;
                var fieldLabels = CheckerMessageWords.Labels(item.TryGetProperty("labels", out var value) ? value : default);
                var classifiers = CheckerMessageWords.Labels(item.TryGetProperty("classifiers", out value) ? value : default);
                var aliases = item.TryGetProperty("aliases", out value) ? value.EnumerateArray().Select(CheckerMessageJson.Text).ToArray() : [];
                var field = new CheckerMessageField(name, target, fieldLabels, classifiers);
                foreach (var identity in new[] { name }.Concat(aliases)) if (!fields.TryAdd(identity, field)) throw CheckerMessageJson.Invalid();
                canonical.Add((name, new object?[] { name, target, Sorted(fieldLabels), Sorted(classifiers), aliases.Order(StringComparer.Ordinal).ToArray() }));
            }
            var fingerprint = JsonSerializer.Serialize(new[] { Sorted(labels), canonical.OrderBy(p => p.Name, StringComparer.Ordinal).Select(p => p.Value).ToArray() });
            entities[property.Name] = new(labels, CheckerMessageJson.Freeze(fields), fingerprint);
        }
        var current = Empty.Current.ToDictionary(p => p.Key, p => p.Value);
        foreach (var pair in CheckerMessageWords.Labels(root.TryGetProperty("currentObject", out var objectLabels) ? objectLabels : default)) current[pair.Key] = pair.Value;
        return new(CheckerMessageJson.Text(root.GetProperty("revision")), entities, current);
    }
    public CheckerPresentationModel Merge(CheckerPresentationModel other)
    {
        ArgumentNullException.ThrowIfNull(other); if (ReferenceEquals(this, Empty)) return other;
        if (ReferenceEquals(other, Empty) || ReferenceEquals(this, other)) return this;
        var entities = _entities.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal);
        foreach (var (key, entity) in other._entities)
        { if (entities.TryGetValue(key, out var prior) && prior.Fingerprint != entity.Fingerprint) throw CheckerMessageJson.Invalid(); entities[key] = entity; }
        if (Current.Any(pair => other.Current[pair.Key] != pair.Value)) throw CheckerMessageJson.Invalid();
        return entities.Count == _entities.Count ? this : new(Revision + "+" + other.Revision, entities, Current.ToDictionary(p => p.Key, p => p.Value));
    }
}

/// <summary>A finite location and immutable metadata, never an entity graph or Context.</summary>
public sealed class CheckerMessagePresentation
{
    private sealed record Step(string? Identity, CheckerMessageField? Field, IReadOnlyList<int> Indices);
    private readonly IReadOnlyList<Step> _steps;
    public CheckerPresentationModel Model { get; }
    public string? Origin { get; }
    public CheckerMessageShape Shape { get; }
    private CheckerMessagePresentation(CheckerPresentationModel model, string? origin, CheckerMessageShape shape, IReadOnlyList<Step> steps)
    { Model = model; Origin = origin; Shape = shape; _steps = steps; }
    public static CheckerMessagePresentation Capture(CheckerPresentationModel model, string? origin, ObjectLocation location)
    {
        var entity = model.Entity(origin); var unusual = entity == null; var steps = new List<(string? Identity, CheckerMessageField? Field, List<int> Indices)>();
        foreach (var segment in location.Segments)
        {
            if (segment is ObjectLocationSegment.Property property)
            {
                var field = entity != null && entity.Fields.TryGetValue(property.Name, out var found) ? found : null;
                entity = model.Entity(field?.Target); steps.Add((property.Name, field, []));
            }
            else if (segment is ObjectLocationSegment.Index index)
            {
                if (index.Value < 0) throw CheckerMessageJson.Invalid();
                if (steps.Count == 0) { steps.Add((null, null, [])); unusual = true; }
                steps[^1].Indices.Add(index.Value); unusual |= steps[^1].Indices.Count > 1;
            }
        }
        unusual |= steps.Count == 0 || steps[^1].Indices.Count != 0;
        var shape = unusual || steps.Count > 2 ? CheckerMessageShape.Deep : steps.Count == 1 ? CheckerMessageShape.One : CheckerMessageShape.Two;
        return new(model, origin, shape, Array.AsReadOnly(steps.Select(step => new Step(step.Identity, step.Field, Array.AsReadOnly(step.Indices.ToArray()))).ToArray()));
    }
    public CheckerMessagePresentation Relocate(ObjectLocation location) => Capture(Model, Origin, location);
    public CheckerMessageOutput Render(CheckerMessageCatalog catalog, CheckerMessageSelection selection, IReadOnlyDictionary<string, string> safe)
    {
        var locale = selection.Locale; var warnings = new HashSet<CheckerMessageWarning>();
        string Vocabulary(IReadOnlyDictionary<TeaQLLocale, string>? labels, string identity)
        {
            var label = labels != null && labels.TryGetValue(locale, out var value) ? value : null;
            if (string.IsNullOrWhiteSpace(label)) { warnings.Add(CheckerMessageWarning.LabelFallback); label = labels != null && labels.TryGetValue(TeaQLLocale.English, out value) ? value : null; }
            label = CheckerMessageWords.Sanitize(label ?? identity.Replace('_', ' '));
            return label.Length == 0 ? CheckerMessageWords.Get(locale, "currentObject") : label;
        }
        string Current() => Vocabulary(Model.Current, "current object");
        var entity = Model.Entity(Origin); var root = entity == null ? Current() : Vocabulary(entity.Labels, Origin!);
        string Label(Step step) => Vocabulary(step.Field?.Labels, step.Field?.Name ?? step.Identity ?? "current object");
        var leaf = _steps.LastOrDefault(); var arguments = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var name in new[] { "system", "input", "input_len", "unit" }) if (safe.TryGetValue(name, out var value)) arguments[name] = value;
        arguments["field"] = leaf?.Identity != null ? Label(leaf) : root;
        var owners = new List<string>(); var count = _steps.Count - (leaf?.Identity != null && leaf.Indices.Count == 0 ? 1 : 0);
        foreach (var step in _steps.Take(count))
        {
            var relation = step.Identity != null ? Label(step) : root;
            if (step.Indices.Count == 0) owners.Add(catalog.Reference(locale, relation));
            else for (var i = 0; i < step.Indices.Count; i++)
            {
                var classifier = step.Field != null && step.Field.Classifiers.TryGetValue(locale, out var custom) ? custom
                    : locale is TeaQLLocale.ChineseSimplified or TeaQLLocale.ChineseTraditional ? "项" : "";
                owners.Add(catalog.Item(locale, i == 0 ? relation : Current(), step.Indices[i], CheckerMessageWords.Sanitize(classifier)));
            }
        }
        arguments["object"] = owners.FirstOrDefault() ?? root; arguments["objectPath"] = owners.Count == 0 ? root : string.Join(catalog.Separator(locale), owners);
        return new(selection.Render(arguments), Array.AsReadOnly(warnings.ToArray()));
    }
}
