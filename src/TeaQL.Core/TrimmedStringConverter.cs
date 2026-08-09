using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TeaQL.Core;

public class TrimmedStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var val = reader.GetString();
        return val?.Trim();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Trim());
    }
}
