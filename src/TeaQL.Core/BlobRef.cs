using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace TeaQL.Core;

/// <summary>Stable catalog Key. Possession grants neither read nor bind authority.</summary>
[JsonConverter(typeof(BlobRefJsonConverter))]
public sealed class BlobRef : IEquatable<BlobRef>
{
    private static readonly Regex KeyPattern = new(@"\Ablob_[A-Za-z0-9_-]{1,123}\z",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private readonly string _key;

    public BlobRef(string key)
    {
        if (key is null || !KeyPattern.IsMatch(key))
            throw new ArgumentException("BLOB_REFERENCE_INVALID", nameof(key));
        _key = key;
    }

    [JsonIgnore]
    public string Key => _key;
    public string ToSqlText() => _key;
    public static BlobRef FromSqlText(object value) => value is string key
        ? new BlobRef(key) : throw new ArgumentException("BLOB_REFERENCE_INVALID", nameof(value));
    public bool Equals(BlobRef? other) => other is not null
        && string.Equals(_key, other._key, StringComparison.Ordinal);
    public override bool Equals(object? obj) => Equals(obj as BlobRef);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(_key);
    public override string ToString() => "BlobRef[masked]";
}

public sealed class BlobRefJsonConverter : JsonConverter<BlobRef>
{
    public override BlobRef Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("BLOB_REFERENCE_INVALID");
        try { return new BlobRef(reader.GetString()!); }
        catch (ArgumentException) { throw new JsonException("BLOB_REFERENCE_INVALID"); }
    }

    public override void Write(Utf8JsonWriter writer, BlobRef value,
        JsonSerializerOptions options) => throw new JsonException("BLOB_PRESENTATION_REQUIRED");
}
