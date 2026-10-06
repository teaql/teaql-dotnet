using System.Text.Json;
using TeaQL.Core;
using Xunit;

namespace TeaQL.DataService.Tests;

public class GraphLineageVectorTests
{
    public static IEnumerable<object[]> Cases()
    {
        using var input = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
            "fixtures", "graph-mutation-lineage-v1.json")));
        foreach (var item in input.RootElement.GetProperty("cases").EnumerateArray())
            yield return new object[] { item.GetProperty("id").GetString()!, item.GetRawText() };
    }

    [Theory, MemberData(nameof(Cases))]
    public void FrozenGraphExpectationsArePerTypedEntity(string id, string json)
    {
        using var input = JsonDocument.Parse(json);
        var item = input.RootElement;
        Assert.Equal(id, item.GetProperty("id").GetString());
        var scopes = new Dictionary<string, MutationTraceScope>();
        var requests = new Dictionary<string, MutationRequest>();
        var ledger = new EntityRoot();
        foreach (var node in item.GetProperty("nodes").EnumerateArray())
        {
            var name = node.GetProperty("entityType").GetString()!;
            var entityId = node.TryGetProperty("assignedEntityId", out var assigned) ? assigned.GetUInt64()
                : node.GetProperty("entityId").GetUInt64();
            var parent = node.GetProperty("parent").ValueKind == JsonValueKind.Null ? null
                : scopes[node.GetProperty("parent").GetString()!];
            var reason = parent == null ? item.GetProperty("requestComment").GetString()
                : node.GetProperty("localComment").GetString();
            MutationTraceScope scope;
            try { scope = new(name, entityId, new MutationIntent(reason).Comment, parent); }
            catch (RequestIntentException) { scope = parent!; }
            var handle = node.GetProperty("nodeId").GetString()!;
            scopes[handle] = scope;
            var key = new EntityKey(name, new Value.U64Value(entityId));
            if (node.TryGetProperty("ledgerLineage", out var chain))
                ledger.SetTraceChain(key, chain.EnumerateArray().Select(value =>
                    new TraceNode(value.GetProperty("name").GetString()!, value.GetProperty("entityId").GetUInt64(), "") {
                        Kind = "auditReason", Detail = value.GetProperty("detail").GetString()! }));
            requests[handle] = new InsertMutationRequest(new InsertCommand(name), item.GetProperty("requestComment").GetString()) {
                LedgerKey = key, LedgerRoot = ledger, GraphScope = scope };
        }
        foreach (var expected in item.GetProperty("expected").EnumerateArray())
        {
            var actual = requests[expected.GetProperty("nodeId").GetString()!].AuditLineage("unused");
            Assert.Equal(expected.GetProperty("lineage").EnumerateArray().Select(value =>
                (value.GetProperty("name").GetString(), (ulong?)value.GetProperty("entityId").GetUInt64(), value.GetProperty("detail").GetString())),
                actual.Select(node => ((string?)node.Name, node.EntityId, (string?)node.Detail)));
            Assert.All(actual, node => Assert.Equal("auditReason", node.Kind));
        }
    }
}
