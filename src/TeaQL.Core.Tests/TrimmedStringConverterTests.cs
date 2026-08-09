using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class TrimmedStringConverterTests
{
    public class TrimmedFields
    {
        [JsonConverter(typeof(TrimmedStringConverter))]
        public string Required { get; set; } = "";

        [JsonConverter(typeof(TrimmedStringConverter))]
        public string? Optional { get; set; }
    }

    [Fact]
    public void TrimmedStringHelpersTrimDuringSerialization()
    {
        var fields = new TrimmedFields
        {
            Required = "  required value\n",
            Optional = "\toptional value  "
        };

        var json = JsonSerializer.Serialize(fields, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        using var doc = JsonDocument.Parse(json);
        Assert.Equal("required value", doc.RootElement.GetProperty("required").GetString());
        Assert.Equal("optional value", doc.RootElement.GetProperty("optional").GetString());
    }

    [Fact]
    public void TrimmedOptionalStringPreservesNoneDuringSerialization()
    {
        var fields = new TrimmedFields
        {
            Required = " value ",
            Optional = null
        };

        var json = JsonSerializer.Serialize(fields, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        using var doc = JsonDocument.Parse(json);
        Assert.Equal("value", doc.RootElement.GetProperty("required").GetString());
        Assert.Equal(JsonValueKind.Null, doc.RootElement.GetProperty("optional").ValueKind);
    }

    [Fact]
    public void TrimmedStringHelpersTrimDuringDeserialization()
    {
        var json = "{\"required\": \"  required value\\n\", \"optional\": \"\\toptional value  \"}";
        var fields = JsonSerializer.Deserialize<TrimmedFields>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(fields);
        Assert.Equal("required value", fields.Required);
        Assert.Equal("optional value", fields.Optional);
    }

    [Fact]
    public void TrimmedOptionalStringKeepsWhitespaceOnlyInputAsSomeEmpty()
    {
        var json = "{\"required\": \" value \", \"optional\": \" \\t\\n \"}";
        var fields = JsonSerializer.Deserialize<TrimmedFields>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.NotNull(fields);
        Assert.Equal("value", fields.Required);
        Assert.Equal("", fields.Optional);
    }
}
