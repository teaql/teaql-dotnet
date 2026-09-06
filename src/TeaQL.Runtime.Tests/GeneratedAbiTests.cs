using TeaQL.Core;
using TeaQL.DataService;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class GeneratedAbiTests
{
    [Fact]
    public void GeneratedMutationFactoryKeepsTypedRequestAuditAndLedger()
    {
        var ledger = new EntityRoot();
        var key = new EntityKey("School", -1);
        var command = new InsertCommand("School").Value("name", new Value.TextValue("Riverside"));

        var request = MutationRequest.Create(command, "create generated school", key, ledger);

        var insert = Assert.IsType<InsertMutationRequest>(request);
        Assert.Same(command, insert.Command);
        Assert.Same(ledger, insert.LedgerRoot);
        Assert.Equal(key, insert.LedgerKey);
        Assert.Equal("create generated school", insert.Comment);
    }

    [Fact]
    public void GeneratedManifestConstructorBuildsOfficialMetadata()
    {
        var module = new RuntimeModule(
            new[] { "School" },
            new Dictionary<string, IEntityChecker>(),
            new Dictionary<string, Record>
            {
                ["School"] = new() { ["id"] = new Value.I64Value(0), ["name"] = new Value.TextValue("") }
            },
            new Dictionary<string, IReadOnlyDictionary<string, bool>>
            {
                ["School"] = new Dictionary<string, bool> { ["id"] = true, ["name"] = true }
            },
            tableNames: new Dictionary<string, string> { ["School"] = "legacy_school" });

        var descriptor = Assert.Single(module.Metadata.GetAllEntities());
        Assert.Equal("School", descriptor.Name);
        Assert.Equal("legacy_school", descriptor.TableNameValue);
        Assert.True(descriptor.PropertyByName("id")!.IsId);
        Assert.False(descriptor.PropertyByName("name")!.Nullable);
    }
}
