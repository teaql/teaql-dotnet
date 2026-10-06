using System.Text.Json;
using Xunit;

namespace TeaQL.Core.Tests;

public class SqlTraceChainTests
{
    public static IEnumerable<object[]> Cases()
    {
        using var input = JsonDocument.Parse(File.ReadAllText(
            Path.Combine(AppContext.BaseDirectory, "fixtures", "sql-trace-path-v1.json")));
        foreach (var item in input.RootElement.GetProperty("cases").EnumerateArray())
            yield return new object[] { item.GetProperty("id").GetString()!, item.GetRawText() };
    }

    private static TraceNode Node(JsonElement node) => new(node.GetProperty("name").GetString()!,
        node.GetProperty("entityId").ValueKind == JsonValueKind.Null ? null : node.GetProperty("entityId").GetUInt64(), "")
    {
        Kind = node.GetProperty("kind").GetString()!, Name = node.GetProperty("name").GetString()!,
        Detail = node.GetProperty("detail").GetString()!
    };

    private static void EqualPath(IEnumerable<TraceNode> expected, IEnumerable<TraceNode> actual) =>
        Assert.Equal(expected.Select(node => (node.Kind.ToLowerInvariant(), node.Name, node.EntityId, node.Detail)),
            actual.Select(node => (node.Kind.ToLowerInvariant(), node.Name, node.EntityId, node.Detail)));

    [Theory, MemberData(nameof(Cases))]
    public void FrozenRustAlgorithm(string id, string json)
    {
        using var input = JsonDocument.Parse(json);
        var item = input.RootElement;
        Assert.Equal(id, item.GetProperty("id").GetString());
        var source = item.GetProperty("source").EnumerateArray().Select(Node).ToArray();
        var path = SqlTraceChain.Canonical(source, item.GetProperty("backend").GetString()!,
            item.GetProperty("operation").GetString()!);
        EqualPath(item.GetProperty("expectedPath").EnumerateArray().Select(Node), path.TracePath);
        var intent = item.GetProperty("expectedIntent");
        Assert.Equal(intent.GetProperty("comment").GetString(), path.Comment);
        Assert.Equal(intent.GetProperty("purpose").GetString(), path.Purpose);
        Assert.Equal(intent.GetProperty("auditReason").GetString(), path.AuditReason);
        EqualPath(path.TracePath, SqlTraceChain.Canonical(path.TracePath, "ignored", "select").TracePath);
        if (source.Length > 0) source[0] = source[0] with { Name = "caller changed source" };
        Assert.DoesNotContain(path.TracePath, node => node.Name == "caller changed source");
    }
}
