using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using TeaQL.Core;

namespace TeaQL.Runtime;

/// <summary>Generated, passive access to selected model fields; never a data-service call.</summary>
public interface IEntityJsonDocument
{
    EntityJsonSchema EntityJsonSchema { get; }
    IEnumerable<KeyValuePair<string, object?>> LoadedJsonFields();
}

public enum EntityJsonKind
{
    Id, Version, Integer, Long, Float, Double, Decimal, Text, Boolean,
    Date, Time, DateTime, Reference, Many
}

public sealed record EntityJsonField(string CanonicalName, EntityJsonKind Kind,
    string? RecordName = null, Func<EntityJsonSchema>? TargetSchema = null)
{
    internal string Key => RecordName ?? CanonicalName;
}

public sealed class EntityJsonException(string code, ObjectLocation location, string instancePath, string message)
    : ArgumentException(message)
{
    public string Code { get; } = code;
    public ObjectLocation Location { get; } = location;
    public string InstancePath { get; } = instancePath;
}

/// <summary>
/// Model-owned JSON without reflection, persistence or transport authorization.
/// The generated FromRecord bridge supplies fresh mutation-boundary ownership.
/// </summary>
public sealed class EntityJsonSchema
{
    private const int MaxDepth = 64;
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);
    private static readonly Regex Integral = new(@"\A-?(?:0|[1-9][0-9]*)\z", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
    private static readonly Regex PlainDecimal = new(@"\A-?(?:0|[1-9][0-9]*)(?:\.[0-9]+)?\z", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
    private static readonly Regex ClockTime = new(@"\A[0-9]{2}:[0-9]{2}:[0-9]{2}(?:\.[0-9]{1,7})?\z", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
    private static readonly Regex Instant = new(@"\A[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}(?:\.[0-9]{1,7})?(?:Z|[+-][0-9]{2}:[0-9]{2})\z", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
    private readonly IReadOnlyDictionary<string, EntityJsonField> _byKey;
    private readonly IReadOnlyDictionary<JsonFieldNamingProfile, WireEntityMetadata> _wire;
    private readonly IReadOnlyDictionary<JsonFieldNamingProfile, IReadOnlyDictionary<string, EntityJsonField>> _names;
    public string EntityType { get; }
    public JsonFieldNamingProfile Profile { get; }
    public int MaxJsonBytes { get; }
    public IReadOnlyList<EntityJsonField> Fields { get; }

    public EntityJsonSchema(string entityType, IEnumerable<EntityJsonField> fields,
        JsonFieldNamingProfile profile = JsonFieldNamingProfile.CamelCase, int maxJsonBytes = 1024 * 1024)
    {
        if (string.IsNullOrWhiteSpace(entityType) || !Enum.IsDefined(profile) || maxJsonBytes < 1)
            throw new ArgumentException("Invalid generated entity JSON schema");
        EntityType = entityType; Profile = profile; MaxJsonBytes = maxJsonBytes;
        var definitions = fields.ToArray();
        if (definitions.Any(field => string.IsNullOrWhiteSpace(field.CanonicalName) || string.IsNullOrWhiteSpace(field.Key)
            || !Enum.IsDefined(field.Kind) || (field.Kind is EntityJsonKind.Reference or EntityJsonKind.Many && field.TargetSchema is null))
            || definitions.Select(field => field.CanonicalName).Distinct(StringComparer.Ordinal).Count() != definitions.Length
            || definitions.Select(field => field.Key).Distinct(StringComparer.Ordinal).Count() != definitions.Length)
            throw new ArgumentException("Invalid generated entity JSON field metadata");
        Fields = Array.AsReadOnly(definitions);
        _byKey = new ReadOnlyDictionary<string, EntityJsonField>(definitions.ToDictionary(field => field.Key, StringComparer.Ordinal));
        var aliases = definitions.ToDictionary(field => field.CanonicalName,
            field => (IReadOnlyList<string>)new[] { field.CanonicalName });
        _wire = Enum.GetValues<JsonFieldNamingProfile>().ToDictionary(item => item,
            item => WireFields.CreateMetadata(entityType, definitions.Select(field => field.CanonicalName), item, aliases));
        _names = _wire.ToDictionary(pair => pair.Key, pair => (IReadOnlyDictionary<string, EntityJsonField>)
            pair.Value.Fields.Values.SelectMany(field => new[] { field.WireName }.Concat(field.Aliases).Distinct(StringComparer.Ordinal)
                .Select(name => (name, field: definitions.Single(definition => definition.CanonicalName == field.CanonicalName))))
                .ToDictionary(item => item.name, item => item.field, StringComparer.Ordinal));
    }

    public string Serialize(IEntityJsonDocument entity, JsonFieldNamingProfile? profile = null)
    {
        var actual = CheckedProfile(profile);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
            WriteEntity(writer, entity, actual, ObjectLocation.Root(), "", new(ReferenceEqualityComparer.Instance), 0, MaxJsonBytes);
        if (stream.Length > MaxJsonBytes) throw Error(ObjectLocation.Root(), "", "WIRE_JSON_LIMIT", "JSON byte limit exceeded");
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public Record Parse(string json, JsonFieldNamingProfile? profile = null)
    {
        if (json is null || Utf8Bytes(json, ObjectLocation.Root(), "") > MaxJsonBytes)
            throw Error(ObjectLocation.Root(), "", "WIRE_JSON_LIMIT", "Expected a bounded JSON object");
        try
        {
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = MaxDepth });
            return ReadEntity(document.RootElement, CheckedProfile(profile), ObjectLocation.Root(), "", 0);
        }
        catch (EntityJsonException) { throw; }
        catch (JsonException) { throw Error(ObjectLocation.Root(), "", "WIRE_INVALID_JSON", "Invalid JSON document"); }
    }

    private JsonFieldNamingProfile CheckedProfile(JsonFieldNamingProfile? value)
    {
        var actual = value ?? Profile;
        if (!Enum.IsDefined(actual)) throw new ArgumentException("Unsupported entity JSON naming profile");
        return actual;
    }

    private void WriteEntity(Utf8JsonWriter writer, IEntityJsonDocument entity, JsonFieldNamingProfile profile,
        ObjectLocation location, string pointer, HashSet<IEntityJsonDocument> active, int depth, int maxBytes)
    {
        if (writer.CurrentDepth >= MaxDepth) throw Error(location, pointer, "WIRE_JSON_LIMIT", "JSON depth limit exceeded");
        if (!ReferenceEquals(entity.EntityJsonSchema, this)) throw Error(location, pointer);
        if (!active.Add(entity)) throw Error(location, pointer, "WIRE_ENTITY_CYCLE", "Cyclic entities require an explicit reference adapter");
        try
        {
            writer.WriteStartObject();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var (key, value) in entity.LoadedJsonFields())
            {
                if (!_byKey.TryGetValue(key, out var field)) throw Error(location, pointer, "WIRE_UNKNOWN_FIELD", "Unknown generated JSON field");
                var wireName = _wire[profile].Fields[field.CanonicalName].WireName;
                var childLocation = location.PropertyAt(field.CanonicalName); var childPointer = Pointer(pointer, wireName);
                if (!seen.Add(key)) throw Error(childLocation, childPointer, "WIRE_FIELD_COLLISION", "Duplicate generated JSON field");
                writer.WritePropertyName(wireName);
                WriteValue(writer, field, value, profile, childLocation, childPointer, active, depth + 1, maxBytes);
                if (writer.BytesCommitted + writer.BytesPending > maxBytes) throw Error(childLocation, childPointer, "WIRE_JSON_LIMIT", "JSON byte limit exceeded");
            }
            writer.WriteEndObject();
        }
        finally { active.Remove(entity); }
    }

    private void WriteValue(Utf8JsonWriter writer, EntityJsonField field, object? value, JsonFieldNamingProfile profile,
        ObjectLocation location, string pointer, HashSet<IEntityJsonDocument> active, int depth, int maxBytes)
    {
        if (value is null)
        {
            if (field.Kind == EntityJsonKind.Many) throw Error(location, pointer);
            writer.WriteNullValue(); return;
        }
        switch (field.Kind)
        {
            case EntityJsonKind.Id: case EntityJsonKind.Version: case EntityJsonKind.Integer: case EntityJsonKind.Long:
                if (value is not long && value is not int) throw Error(location, pointer);
                var number = Convert.ToInt64(value, Invariant);
                CheckInteger(number, field.Kind, location, pointer);
                if (field.Kind == EntityJsonKind.Integer) writer.WriteNumberValue(number);
                else writer.WriteStringValue(number.ToString(Invariant));
                return;
            case EntityJsonKind.Text:
                if (value is not string text) throw Error(location, pointer);
                if (Utf8Bytes(text, location, pointer) > maxBytes) throw Error(location, pointer, "WIRE_JSON_LIMIT");
                writer.WriteStringValue(text); return;
            case EntityJsonKind.Boolean:
                if (value is not bool boolean) throw Error(location, pointer);
                writer.WriteBooleanValue(boolean); return;
            case EntityJsonKind.Decimal:
                if (value is not decimal amount) throw Error(location, pointer);
                writer.WriteStringValue(amount.ToString(Invariant)); return;
            case EntityJsonKind.Float: case EntityJsonKind.Double:
                if (value is not double && value is not float) throw Error(location, pointer);
                var floating = Convert.ToDouble(value, Invariant);
                if (!double.IsFinite(floating)) throw Error(location, pointer);
                writer.WriteNumberValue(floating); return;
            case EntityJsonKind.Date:
                if (value is not DateTime date || date.TimeOfDay != TimeSpan.Zero) throw Error(location, pointer, "WIRE_TEMPORAL_PRECISION");
                writer.WriteStringValue(date.ToString("yyyy-MM-dd", Invariant)); return;
            case EntityJsonKind.Time:
                if (value is not TimeSpan time || time < TimeSpan.Zero || time >= TimeSpan.FromDays(1)) throw Error(location, pointer);
                writer.WriteStringValue(time.ToString(@"hh\:mm\:ss\.fffffff", Invariant).TrimEnd('0').TrimEnd('.')); return;
            case EntityJsonKind.DateTime:
                var instant = value switch { DateTimeOffset item => item.UtcDateTime,
                    DateTime item when item.Kind == DateTimeKind.Utc => item, _ => throw Error(location, pointer) };
                if (instant.Ticks % TimeSpan.TicksPerMillisecond != 0) throw Error(location, pointer, "WIRE_TEMPORAL_PRECISION");
                writer.WriteStringValue(instant.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", Invariant)); return;
            case EntityJsonKind.Reference:
                if (value is IEntityJsonDocument target)
                { field.TargetSchema!().WriteEntity(writer, target, profile, location, pointer, active, depth, maxBytes); return; }
                if (value is not long && value is not int) throw Error(location, pointer);
                var identity = Convert.ToInt64(value, Invariant); CheckInteger(identity, EntityJsonKind.Id, location, pointer);
                writer.WriteStringValue(identity.ToString(Invariant)); return;
            case EntityJsonKind.Many:
                if (value is not IEnumerable children || value is string) throw Error(location, pointer);
                if (writer.CurrentDepth >= MaxDepth) throw Error(location, pointer, "WIRE_JSON_LIMIT");
                writer.WriteStartArray(); var index = 0;
                foreach (var child in children)
                {
                    if (child is not IEntityJsonDocument document) throw Error(location.Index(index), Pointer(pointer, index), "WIRE_INVALID_TYPE");
                    field.TargetSchema!().WriteEntity(writer, document, profile, location.Index(index), Pointer(pointer, index), active, depth, maxBytes);
                    if (writer.BytesCommitted + writer.BytesPending > maxBytes) throw Error(location.Index(index), Pointer(pointer, index), "WIRE_JSON_LIMIT");
                    index++;
                }
                writer.WriteEndArray(); return;
            default: throw Error(location, pointer);
        }
    }

    private Record ReadEntity(JsonElement element, JsonFieldNamingProfile profile, ObjectLocation location, string pointer, int depth)
    {
        if (depth > MaxDepth) throw Error(location, pointer, "WIRE_JSON_LIMIT");
        if (element.ValueKind != JsonValueKind.Object) throw Error(location, pointer, "WIRE_ENTITY_OBJECT_REQUIRED");
        var record = new Record();
        foreach (var property in element.EnumerateObject())
        {
            string name;
            try { name = property.Name; }
            catch (InvalidOperationException) { throw Error(location, pointer, "WIRE_INVALID_TEXT"); }
            Utf8Bytes(name, location, pointer);
            var childPointer = Pointer(pointer, name);
            if (!_names[profile].TryGetValue(name, out var field))
                throw Error(string.IsNullOrWhiteSpace(name) ? location : location.PropertyAt(name), childPointer, "WIRE_UNKNOWN_FIELD");
            var childLocation = location.PropertyAt(field.CanonicalName);
            if (record.ContainsKey(field.Key)) throw Error(childLocation, childPointer, "WIRE_FIELD_COLLISION");
            record[field.Key] = ReadValue(field, property.Value, profile, childLocation, childPointer, depth + 1);
        }
        return record;
    }

    private Value ReadValue(EntityJsonField field, JsonElement value, JsonFieldNamingProfile profile, ObjectLocation location, string pointer, int depth)
    {
        if (value.ValueKind == JsonValueKind.Null)
        {
            if (field.Kind == EntityJsonKind.Many) throw Error(location, pointer);
            return new Value.NullValue();
        }
        string? text;
        try { text = value.ValueKind == JsonValueKind.String ? value.GetString()! : null; }
        catch (InvalidOperationException) { throw Error(location, pointer, "WIRE_INVALID_TEXT"); }
        if (text is not null) Utf8Bytes(text, location, pointer);
        switch (field.Kind)
        {
            case EntityJsonKind.Id: case EntityJsonKind.Version: case EntityJsonKind.Long:
                if (text is null || text.Length > 20 || !Integral.IsMatch(text)
                    || !long.TryParse(text, NumberStyles.AllowLeadingSign, Invariant, out var integer)) throw Error(location, pointer);
                CheckInteger(integer, field.Kind, location, pointer); return new Value.I64Value(integer);
            case EntityJsonKind.Integer:
                if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out var small)) throw Error(location, pointer);
                return new Value.I64Value(small);
            case EntityJsonKind.Text:
                if (text is null) throw Error(location, pointer);
                return new Value.TextValue(text);
            case EntityJsonKind.Boolean:
                if (value.ValueKind is not (JsonValueKind.True or JsonValueKind.False)) throw Error(location, pointer);
                return new Value.BoolValue(value.GetBoolean());
            case EntityJsonKind.Decimal:
                return new Value.DecimalValue(ExactDecimal(text, location, pointer));
            case EntityJsonKind.Float: case EntityJsonKind.Double:
                if (value.ValueKind != JsonValueKind.Number || !value.TryGetDouble(out var floating) || !double.IsFinite(floating)) throw Error(location, pointer);
                return new Value.F64Value(floating);
            case EntityJsonKind.Date:
                if (text is null || !DateTime.TryParseExact(text, "yyyy-MM-dd", Invariant, DateTimeStyles.None, out var date)) throw Error(location, pointer);
                return new Value.DateValue(date);
            case EntityJsonKind.Time:
                if (text is null || !ClockTime.IsMatch(text) || !TimeSpan.TryParseExact(text, "c", Invariant, out var time)
                    || time < TimeSpan.Zero || time >= TimeSpan.FromDays(1)) throw Error(location, pointer);
                return new Value.TimeValue(time);
            case EntityJsonKind.DateTime:
                if (text is null || !Instant.IsMatch(text) || !DateTimeOffset.TryParse(text, Invariant, DateTimeStyles.None, out var instant)) throw Error(location, pointer);
                if (instant.Ticks % TimeSpan.TicksPerMillisecond != 0) throw Error(location, pointer, "WIRE_TEMPORAL_PRECISION");
                return new Value.DateTimeValue(instant.UtcDateTime);
            case EntityJsonKind.Reference:
                if (value.ValueKind == JsonValueKind.Object)
                    return new Value.ObjectValue(field.TargetSchema!().ReadEntity(value, profile, location, pointer, depth));
                if (text is null || text.Length > 20 || !Integral.IsMatch(text)
                    || !long.TryParse(text, NumberStyles.AllowLeadingSign, Invariant, out var identity)) throw Error(location, pointer);
                CheckInteger(identity, EntityJsonKind.Id, location, pointer); return new Value.I64Value(identity);
            case EntityJsonKind.Many:
                if (value.ValueKind != JsonValueKind.Array) throw Error(location, pointer);
                var index = 0; var children = new List<Value>();
                foreach (var child in value.EnumerateArray())
                { children.Add(new Value.ObjectValue(field.TargetSchema!().ReadEntity(child, profile, location.Index(index), Pointer(pointer, index), depth))); index++; }
                return new Value.ListValue(children);
            default: throw Error(location, pointer);
        }
    }

    private static void CheckInteger(long number, EntityJsonKind kind, ObjectLocation location, string pointer)
    {
        if ((kind == EntityJsonKind.Integer && (number < int.MinValue || number > int.MaxValue)) || (kind == EntityJsonKind.Id && number < 0))
            throw Error(location, pointer, "WIRE_INTEGER_RANGE");
    }

    private static decimal ExactDecimal(string? text, ObjectLocation location, string pointer)
    {
        if (text is null || text.Length > 4096 || !PlainDecimal.IsMatch(text)) throw Error(location, pointer);
        var point = text.IndexOf('.'); var scale = point < 0 ? 0 : text.Length - point - 1;
        if (scale > 28 || !decimal.TryParse(text, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, Invariant, out var result))
            throw Error(location, pointer, "WIRE_DECIMAL_PRECISION");
        var bits = decimal.GetBits(result); var actualScale = (bits[3] >> 16) & 0xff;
        var coefficient = (BigInteger)(uint)bits[0] + ((BigInteger)(uint)bits[1] << 32) + ((BigInteger)(uint)bits[2] << 64);
        var expected = BigInteger.Parse(text.Replace(".", "").TrimStart('-'), Invariant);
        if (actualScale != scale || coefficient != expected) throw Error(location, pointer, "WIRE_DECIMAL_PRECISION");
        return result;
    }

    private static string Pointer(string parent, object name) => parent + "/" + Convert.ToString(name, Invariant)!.Replace("~", "~0").Replace("/", "~1");
    private static int Utf8Bytes(string value, ObjectLocation location, string pointer)
    {
        try { return StrictUtf8.GetByteCount(value); }
        catch (EncoderFallbackException) { throw Error(location, pointer, "WIRE_INVALID_TEXT"); }
    }
    private static EntityJsonException Error(ObjectLocation location, string pointer, string code = "WIRE_INVALID_TYPE", string message = "Expected the declared model JSON type") =>
        new(code, location, pointer, message);
}
