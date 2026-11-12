using System.Globalization;
using System.Text.RegularExpressions;

namespace TeaQL.Runtime;

public sealed record CheckerMessageRequest(CheckerMessagePresentation Presentation, CheckerMessageCatalog Catalog,
    CheckerMessageSelection Selection, IReadOnlyDictionary<string, string> Arguments);
public delegate CheckerMessageOutput CheckerMessageRenderer(CheckerMessageRequest request);
internal sealed record CheckerMessageConfiguration(CheckerPresentationModel Model, CheckerMessageCatalog Catalog,
    I18nCatalog Legacy, TeaQLLocale Locale, CheckerMessageRenderer Renderer, Action<CheckerMessageWarning> Warning);
internal sealed record CheckerValidationOrigin(CheckerPresentationModel Model, string? Entity);
internal sealed record CheckerNaturalBinding(CheckerMessagePresentation Snapshot, string? AutomaticMessage = null, CheckerMessagePlan? Plan = null);
internal sealed record CheckerPreparedMessage(CheckerMessageSelection? Selection, CheckerMessageCatalog Catalog,
    CheckerMessageTemplate? Legacy, bool Generic);

internal sealed class CheckerMessagePlan
{
    private readonly IReadOnlyDictionary<(TeaQLLocale, CheckerMessageShape), CheckerPreparedMessage>? _entries;
    private readonly string? _literal;
    internal CheckerMessagePlan(string literal) => _literal = literal;
    private CheckerMessagePlan(Dictionary<(TeaQLLocale, CheckerMessageShape), CheckerPreparedMessage> entries) => _entries = CheckerMessageJson.Freeze(entries);
    internal CheckerPreparedMessage Entry(TeaQLLocale locale, CheckerMessageShape shape) => _entries![(locale, shape)];
    internal static CheckerMessagePlan Prepare(CheckerMessageConfiguration config, string rule, Func<string, string> project)
    {
        var entries = new Dictionary<(TeaQLLocale, CheckerMessageShape), CheckerPreparedMessage>();
        var patterns = new Dictionary<TeaQLLocale, CheckerMessageCatalog>();
        foreach (var locale in Enum.GetValues<TeaQLLocale>()) foreach (var shape in Enum.GetValues<CheckerMessageShape>())
        {
            CheckerMessageSelection? selection = null; CheckerMessageTemplate? legacy = null;
            foreach (var candidate in new[] { locale, TeaQLLocale.English })
            {
                selection = config.Catalog.Override(candidate, rule, shape);
                if (selection != null) { selection = selection.Project(project); break; }
                if (config.Legacy.ApplicationMessage(candidate, rule) is { } raw)
                {
                    var nodes = Regex.Split(raw, @"(\{(?:location|system|input|input_len)\})").Select(part =>
                        Regex.IsMatch(part, @"^\{(?:location|system|input|input_len)\}$")
                            ? new CheckerTemplateNode(null, part[1..^1]) : new CheckerTemplateNode(project(part), null)).ToArray();
                    legacy = new(nodes); break;
                }
                selection = CheckerMessageCatalog.Builtin.Lookup(candidate, rule, shape); if (selection != null) break;
            }
            var generic = selection == null && legacy == null;
            if (generic) selection = CheckerMessageCatalog.Builtin.Select(locale, "checker.invalid", shape);
            var effective = selection?.Locale ?? locale;
            if (!patterns.ContainsKey(effective)) patterns[effective] = config.Catalog.ProjectPatterns(effective, project);
            entries[(locale, shape)] = new(selection, patterns[effective], legacy, generic);
        }
        return new(entries);
    }
    internal CheckerMessagePlan Project(Func<string, string> project)
    {
        if (_literal != null) return new(project(_literal));
        return new(_entries!.ToDictionary(pair => pair.Key, pair => {
            var item = pair.Value;
            return item with { Selection = item.Selection?.Application == true ? item.Selection.Project(project) : item.Selection,
                Catalog = item.Catalog.ProjectPatterns(item.Selection?.Locale ?? pair.Key.Item1, project), Legacy = item.Legacy?.Project(project) };
        }));
    }
    internal CheckerMessageOutput Render(CheckerMessagePresentation snapshot, CheckResult result, TeaQLLocale locale)
    {
        if (_literal != null) return new(_literal, []);
        var item = Entry(locale, snapshot.Shape); var arguments = CheckerMessages.Arguments(result, item.Selection?.Locale ?? locale).ToDictionary(p => p.Key, p => p.Value);
        if (item.Legacy != null) { arguments["location"] = result.NativePath; return new(item.Legacy.Render(arguments), []); }
        var output = snapshot.Render(item.Catalog, item.Selection!, arguments);
        return item.Generic ? output with { Warnings = Array.AsReadOnly(output.Warnings.Append(CheckerMessageWarning.GenericRule).ToArray()) } : output;
    }
}

internal static class CheckerMessages
{
    internal static CheckerMessageOutput Standard(CheckerMessageRequest request) => request.Presentation.Render(request.Catalog, request.Selection, request.Arguments);
    internal static void DefaultWarning(CheckerMessageWarning warning) => Console.Error.WriteLine(warning switch {
        CheckerMessageWarning.LabelFallback => "CHECKER_MESSAGE_LABEL_FALLBACK", CheckerMessageWarning.GenericRule => "CHECKER_MESSAGE_GENERIC_RULE", _ => "CHECKER_MESSAGE_RENDER_FALLBACK" });
    internal static bool Automatic(CheckResult result) => string.IsNullOrWhiteSpace(result.Message)
        || result.NaturalPresentation is { AutomaticMessage: not null } state && state.AutomaticMessage == result.Message;
    internal static string Rule(string rule) => rule.ToUpperInvariant() switch {
        "REQUIRED" => "checker.required", "MIN" => "checker.min", "MAX" => "checker.max",
        "MIN_STR_LEN" or "MIN_LENGTH" => "checker.minLength", "MAX_STR_LEN" or "MAX_LENGTH" => "checker.maxLength", _ => "checker." + rule.ToLowerInvariant() };
    internal static void Bind(CheckResult result, CheckerPresentationModel model, string origin) =>
        result.NaturalPresentation = new(CheckerMessagePresentation.Capture(model, origin, result.Location), Automatic(result) ? result.Message : null,
            Automatic(result) ? result.NaturalPresentation?.Plan : null);
    private static string Scalar(object? value) => value switch {
        null => "[omitted]", string text => CheckerMessageWords.Sanitize(text),
        bool boolean => boolean.ToString(CultureInfo.InvariantCulture),
        byte or sbyte or short or ushort or int or uint or long or ulong or decimal or float or double => Convert.ToString(value, CultureInfo.InvariantCulture)!,
        DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), DateTime date => date.ToString("O", CultureInfo.InvariantCulture),
        DateTimeOffset date => date.ToString("O", CultureInfo.InvariantCulture), _ => "[value omitted]" };
    internal static IReadOnlyDictionary<string, string> Arguments(CheckResult result, TeaQLLocale locale) => CheckerMessageJson.Freeze(new Dictionary<string, string> {
        ["input"] = Scalar(result.InputValue), ["system"] = Scalar(result.SystemValue),
        ["input_len"] = result.Presentation?.OriginalLength?.ToString(CultureInfo.InvariantCulture) ?? "[omitted]", ["unit"] = CheckerMessageWords.Get(locale, "utf16") });
    internal static void Render(CheckerMessageConfiguration config, CheckResult result, IEnumerable<string> secrets, bool automatic,
        HashSet<CheckerMessageWarning> seen, CheckerValidationOrigin? forced = null)
    {
        if (!automatic) return; // A projected per-violation customer literal is authoritative.
        void Warn(CheckerMessageWarning code)
        {
            if (!Enum.IsDefined(code)) code = CheckerMessageWarning.RenderFallback;
            if (seen.Add(code)) try { config.Warning(code); } catch (Exception error) when (error is not OutOfMemoryException) { }
        }
        var previous = result.NaturalPresentation;
        CheckerMessagePresentation snapshot;
        try { snapshot = CheckerMessagePresentation.Capture(forced?.Model ?? previous?.Snapshot.Model ?? config.Model,
            forced?.Entity ?? previous?.Snapshot.Origin, result.Location); }
        catch (Exception error) when (error is not OutOfMemoryException)
        { Warn(CheckerMessageWarning.RenderFallback); snapshot = CheckerMessagePresentation.Capture(CheckerPresentationModel.Empty, null, ObjectLocation.Root()); }
        // Validate even a configuration supplied through an internal/native path.
        _ = config.Locale.Code();
        string Project(string literal) => CheckerPrivacy.Scrub(literal, secrets)!;
        var plan = previous?.Plan?.Project(Project);
        CheckerMessageOutput output;
        try
        {
            if (plan == null)
            {
                plan = CheckerMessagePlan.Prepare(config, Rule(result.RuleId), Project);
                var item = plan.Entry(config.Locale, snapshot.Shape);
                if (config.Renderer != Standard && item.Legacy == null)
                {
                    if (item.Generic) Warn(CheckerMessageWarning.GenericRule);
                    output = config.Renderer(new(snapshot, item.Catalog, item.Selection!, Arguments(result, item.Selection!.Locale)));
                    output = output with { Message = Project(output.Message) }; plan = new(output.Message);
                }
                else output = plan.Render(snapshot, result, config.Locale);
            }
            else output = plan.Render(snapshot, result, config.Locale);
            if (string.IsNullOrWhiteSpace(output.Message)) throw new InvalidOperationException("Empty Checker message");
            foreach (var code in output.Warnings) Warn(code);
        }
        catch (Exception error) when (error is not OutOfMemoryException)
        {
            Warn(CheckerMessageWarning.RenderFallback);
            output = snapshot.Render(CheckerMessageCatalog.Builtin, CheckerMessageCatalog.Builtin.Select(config.Locale, "checker.invalid", snapshot.Shape), new Dictionary<string, string>());
            foreach (var code in output.Warnings) Warn(code); plan = new(output.Message);
        }
        result.Message = output.Message; result.NaturalPresentation = new(snapshot, output.Message, plan);
    }
}
