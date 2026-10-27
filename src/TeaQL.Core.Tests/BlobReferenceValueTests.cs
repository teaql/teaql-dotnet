using System.Text.Json;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class BlobReferenceValueTests
{
    [Fact]
    public void SharedStableKeyAndFormatVectorsMatchSqlAndJsonBoundaries()
    {
        using var vectors = JsonDocument.Parse(File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory, "fixtures/blob-value-cases.json")));
        var root = vectors.RootElement;
        foreach (var element in root.GetProperty("valid").EnumerateArray())
        {
            var key = element.GetString()!;
            var reference = JsonSerializer.Deserialize<BlobRef>(element.GetRawText())!;
            Assert.Equal(key, reference.Key);
            Assert.Equal(reference, BlobRef.FromSqlText(key));
            Assert.Equal(key, Value.FromObject(reference).TryText());
            Assert.Equal("BlobRef[masked]", reference.ToString());
            Assert.Throws<JsonException>(() => JsonSerializer.Serialize(reference));
        }
        foreach (var element in root.GetProperty("invalid").EnumerateArray())
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<BlobRef>(element.GetRawText()));
        foreach (var element in root.GetProperty("invalidJson").EnumerateArray())
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<BlobRef>(element.GetRawText()));

        var formats = root.GetProperty("formats").EnumerateObject().ToArray();
        var spec = new BlobFieldSpec("Asset", "document", formats.Select(x => x.Name).Reverse(),
            false, "default");
        Assert.Equal(formats.Select(x => x.Name).OrderBy(x => x, StringComparer.Ordinal),
            spec.AllowedExtensions);
        Assert.Equal(formats.Select(x => x.Value.GetString()).Distinct().OrderBy(x => x, StringComparer.Ordinal),
            spec.AllowedContentTypes);
        Assert.Throws<ArgumentException>(() => new BlobFieldSpec("Asset", "document",
            ["application.docx"], false, "default"));
    }
}
