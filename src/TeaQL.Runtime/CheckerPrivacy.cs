using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

// Error-owned, safe projection provenance only. Never retain raw checked values,
// a Context, a ledger, or an invocation's redaction strings here.
internal sealed record CheckerPresentation(object? SafeInput, string? Fingerprint, bool Masked,
    bool Credential, bool Safe, int? OriginalLength);

internal static class CheckerPrivacy
{
    private const string Redacted = "[REDACTED]";
    private sealed record Inspected(object? Value, bool Credential);

    internal static IReadOnlyList<CheckResult> Translate(IReadOnlyList<CheckResult> results,
        I18nCatalog catalog, TeaQLLocale locale, UserContext? context = null, IEnumerable<string>? additional = null,
        string? requestEntityType = null)
    {
        var secrets = new HashSet<string>(additional ?? [], StringComparer.Ordinal);
        var prepared = results.Select(result => Prepare(result, context, secrets, requestEntityType)).ToArray();
        foreach (var result in prepared)
        {
            result.Message = Scrub(result.Message, secrets);
            catalog.TranslatePrepared(result, locale, secrets);
        }
        return results;
    }

    internal static CheckResult ForWire(CheckResult source)
    {
        var copy = source.Copy(); var secrets = new HashSet<string>(StringComparer.Ordinal);
        Prepare(copy, null, secrets); copy.Message = Scrub(copy.Message, secrets); return copy;
    }

    internal static IEnumerable<string> RequestSecrets(UserContext context, MutationRequest request)
    {
        var (entity, values) = request switch {
            InsertMutationRequest insert => (insert.Command.Entity, insert.Command.Values.AsEnumerable()),
            UpdateMutationRequest update => (update.Command.Entity, update.Command.Values
                .Concat(update.Command.OldValues ?? []).Concat(update.Command.Guards)),
            DeleteMutationRequest delete => (delete.Command.Entity, delete.Command.Guards.AsEnumerable()),
            RecoverMutationRequest recover => (recover.Command.Entity, recover.Command.Guards.AsEnumerable()),
            _ => ("", Enumerable.Empty<KeyValuePair<string, Value>>())
        };
        if (request.LoadedSnapshot != null) values = values.Concat(request.LoadedSnapshot.CopyValues());
        var secrets = new HashSet<string>(StringComparer.Ordinal); var debug = LogPrivacy.PlaintextEnabled();
        foreach (var (name, value) in values)
        {
            var inspected = Inspect(value); var policy = FieldPolicy(context, entity, [name]);
            if (inspected.Credential || policy.Credential || policy.Masked && !debug) Strings(inspected.Value, secrets);
        }
        return secrets.ToArray();
    }

    private static CheckResult Prepare(CheckResult result, UserContext? context, HashSet<string> secrets, string? requestEntityType = null)
    {
        var input = Inspect(result.InputValue); var expected = Inspect(result.SystemValue);
        var fields = result.Location.Segments.OfType<ObjectLocationSegment.Property>().Select(p => p.Name).ToArray();
        var policy = FieldPolicy(context, result.EntityType, fields);
        // The declared type is diagnostic identity, not authority to weaken the
        // actual mutation owner's policy. This owner is invocation-local.
        var owner = FieldPolicy(context, requestEntityType, fields, allowLeafShortcut: false);
        var previous = result.Presentation;
        var credential = input.Credential || policy.Credential || owner.Credential || previous?.Credential == true;
        var masked = policy.Masked || owner.Masked || previous?.Masked == true
            || HasModeledMask(context, policy.ValueDescriptor, input.Value) || HasModeledMask(context, owner.ValueDescriptor, input.Value);
        var hide = credential || masked && !LogPrivacy.PlaintextEnabled();
        var same = previous?.Fingerprint != null && previous.Fingerprint == Fingerprint(input.Value);
        int? length = same ? previous!.OriginalLength : InputLength(input.Value);
        if (hide) Strings(input.Value, secrets);
        if (expected.Credential || credential && expected.Value is not null && !IsNumericOrBoolean(expected.Value))
        { Strings(expected.Value, secrets); expected = new Inspected(expected.Value == null ? null : Redacted, false); }
        object? safe = credential && input.Value != null ? Redacted
            : same && previous!.Safe ? Clone(input.Value)
            : same && hide && previous!.Masked ? Clone(previous.SafeInput)
            : hide ? Mask(input.Value) : Clone(input.Value);
        var fallback = credential && input.Value != null ? Redacted : masked ? Mask(input.Value) : Clone(input.Value);
        if (same && (previous!.Masked || !masked)) fallback = Clone(previous.SafeInput);
        result.SetPresentedValues(safe, expected.Value);
        result.Presentation = new CheckerPresentation(fallback, Fingerprint(safe), masked, credential, hide || same && previous!.Safe, length);
        return result;
    }

    private static (bool Masked, bool Credential, EntityDescriptor? ValueDescriptor) FieldPolicy(UserContext? context, string? entity,
        IReadOnlyList<string> fields, bool allowLeafShortcut = true)
    {
        bool masked = false, credential = fields.Any(SensitiveLogNames.IsCredential);
        var descriptor = entity == null ? null : context?.GetEntity(entity);
        // An explicit declaring child type can accompany an already-prefixed path.
        var start = allowLeafShortcut && fields.Count > 0 && descriptor?.RelationByName(fields[0]) == null
            && descriptor?.PropertyByName(fields[^1]) != null ? fields.Count - 1 : 0;
        for (var i = start; i < fields.Count; i++)
        {
            var field = fields[i]; var property = descriptor?.PropertyByName(field);
            var relation = descriptor?.RelationByName(field);
            var stored = relation == null ? null : descriptor?.PropertyByName(relation.LocalKeyValue);
            var aliases = new[] { field, property?.Name, property?.ColumnNameString, relation?.LocalKeyValue, stored?.ColumnNameString };
            credential |= aliases.Any(a => a != null && SensitiveLogNames.IsCredential(a));
            masked |= aliases.Any(a => a != null && descriptor?.AuditMaskFieldList.Contains(a) == true);
            // A scalar (including JSON) is not another namespace of this entity.
            descriptor = relation == null ? null : context?.GetEntity(relation.TargetEntity);
        }
        return (masked, credential, descriptor);
    }

    private static bool HasModeledMask(UserContext? context, EntityDescriptor? descriptor, object? value)
    {
        if (context == null || descriptor == null) return false;
        if (value is IList<object?> list) return list.Any(item => HasModeledMask(context, descriptor, item));
        if (value is not IDictionary<string, object?> fields) return false;
        foreach (var (key, item) in fields)
        {
            var canonical = ModelValueField(descriptor, key);
            if (canonical == null) continue;
            var policy = FieldPolicy(context, descriptor.Name, [canonical], allowLeafShortcut: false);
            if (policy.Masked || policy.Credential || HasModeledMask(context, policy.ValueDescriptor, item)) return true;
        }
        return false;
    }

    private static string? ModelValueField(EntityDescriptor descriptor, string key)
    {
        var names = descriptor.Properties.Select(p => p.Name).Concat(descriptor.Relations.Select(r => r.Name))
            .Concat(descriptor.AuditMaskFieldList).Distinct(StringComparer.Ordinal);
        foreach (var name in names)
        {
            if (key == name || descriptor.PropertyByName(name)?.ColumnNameString == key) return name;
            foreach (var profile in Enum.GetValues<JsonFieldNamingProfile>())
            {
                var rendered = ObjectLocation.Property(name).InstancePathWith(profile)[1..].Replace("~1", "/").Replace("~0", "~");
                if (key == rendered) return name;
            }
        }
        return null;
    }

    private static Inspected Inspect(object? value) => Inspect(value, new HashSet<object>(ReferenceEqualityComparer.Instance), 0);
    private static Inspected Inspect(object? value, HashSet<object> active, int depth)
    {
        if (depth > 48) return new(Redacted, true);
        if (value is Value core) return Inspect(core.Raw, active, depth + 1);
        if (value is double d && !double.IsFinite(d) || value is float f && !float.IsFinite(f)) return new(Redacted, true);
        if (value == null || value is string || IsNumericOrBoolean(value) || value is DateTime or DateTimeOffset or DateOnly or TimeOnly or TimeSpan or Guid or char)
            return new(value, false);
        if (!active.Add(value)) return new(Redacted, true);
        try
        {
            if (value is JsonNode node) return InspectNode(node, depth);
            if (value is JsonElement element) return InspectNode(JsonNode.Parse(element.GetRawText()), depth);
            if (value is IDictionary dictionary)
            {
                var copy = new Dictionary<string, object?>(StringComparer.Ordinal); bool credential = false;
                foreach (DictionaryEntry pair in dictionary)
                {
                    if (pair.Key is not string name) return new(Redacted, true);
                    var child = Inspect(pair.Value, active, depth + 1); copy[name] = child.Value;
                    credential |= SensitiveLogNames.IsCredential(name) || child.Credential;
                }
                return new(copy, credential);
            }
            if (value.GetType().GetInterfaces().Any(i => i.IsGenericType
                && i.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)))
                return InspectNode(JsonSerializer.SerializeToNode(value, value.GetType()), depth);
            if (value is IEnumerable sequence)
            {
                var copy = new List<object?>(); bool credential = false;
                foreach (var item in sequence)
                {
                    if (copy.Count >= 10000) return new(Redacted, true);
                    var child = Inspect(item, active, depth + 1); copy.Add(child.Value); credential |= child.Credential;
                }
                return new(copy, credential);
            }
            // Respect JSON names/converters without invoking an arbitrary ToString.
            return InspectNode(JsonSerializer.SerializeToNode(value, value.GetType(), new JsonSerializerOptions { MaxDepth = 48 }), depth);
        }
        catch (Exception error) when (error is not OutOfMemoryException and not OperationCanceledException)
        { return new(Redacted, true); }
        finally { active.Remove(value); }
    }

    private static Inspected InspectNode(JsonNode? value, int depth)
    {
        if (depth > 48) return new(Redacted, true);
        if (value is JsonObject obj)
        {
            var copy = new Dictionary<string, object?>(StringComparer.Ordinal); bool credential = false;
            foreach (var pair in obj)
            { var child = InspectNode(pair.Value, depth + 1); copy[pair.Key] = child.Value; credential |= SensitiveLogNames.IsCredential(pair.Key) || child.Credential; }
            return new(copy, credential);
        }
        if (value is JsonArray array)
        {
            var children = array.Select(v => InspectNode(v, depth + 1)).ToArray();
            return new(children.Select(v => v.Value).ToList(), children.Any(v => v.Credential));
        }
        if (value == null) return new(null, false);
        using var document = JsonDocument.Parse(value.ToJsonString());
        var element = document.RootElement;
        return element.ValueKind switch {
            JsonValueKind.String => new(element.GetString(), false), JsonValueKind.True => new(true, false),
            JsonValueKind.False => new(false, false), JsonValueKind.Null => new(null, false),
            JsonValueKind.Number when element.TryGetDecimal(out var number) => new(number, false),
            _ => new(Redacted, true)
        };
    }

    private static bool IsNumericOrBoolean(object value) => value is bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal;
    private static object? Clone(object? value) => Inspect(value).Value;
    private static object? Mask(object? value) => value switch {
        null => null,
        IDictionary<string, object?> => Redacted,
        IList<object?> list => list.Select(Mask).ToList(),
        string text => LogPrivacy.MaskAuditValue(text),
        _ => LogPrivacy.MaskAuditValue(Display(value))
    };
    internal static string Display(object? value) => value switch {
        null => "[omitted]", string text => text,
        IFormattable scalar => scalar.ToString(null, CultureInfo.InvariantCulture),
        _ => JsonSerializer.Serialize(value)
    };
    private static int? InputLength(object? value) => value == null ? null : Display(value).Length;
    private static string? Fingerprint(object? value)
    {
        try { return Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(new { Type = value?.GetType().FullName, Value = value }))); }
        catch (Exception error) when (error is not OutOfMemoryException and not OperationCanceledException) { return null; }
    }
    private static void Strings(object? value, HashSet<string> output)
    {
        if (value is string text) { if (text.Length != 0) output.Add(text); }
        else if (value is IDictionary<string, object?> map) foreach (var item in map.Values) Strings(item, output);
        else if (value is IList<object?> list) foreach (var item in list) Strings(item, output);
    }
    internal static string? Scrub(string? text, IEnumerable<string> secrets)
    {
        if (text == null) return null;
        var variants = secrets.Where(s => s.Length != 0).SelectMany(s => new[] {
            s, JsonSerializer.Serialize(s), JsonSerializer.Serialize(s)[1..^1],
            JsonSerializer.Serialize(s, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping })[1..^1]
        }).Distinct(StringComparer.Ordinal).OrderByDescending(s => s.Length);
        foreach (var secret in variants) text = text.Replace(secret, Redacted, StringComparison.Ordinal);
        return text;
    }
}
