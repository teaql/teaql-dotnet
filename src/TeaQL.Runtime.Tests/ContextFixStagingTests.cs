using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public class ContextFixStagingTests
{
    [Fact]
    public async Task LaterRejectedChildCannotRetainAnEarlierAutomaticContextAssignment()
    {
        var root = new EntityRoot(); var first = new EntityKey("Probe", 1); var second = new EntityKey("Probe", 2);
        root.Set(first, "name", new Value.TextValue("first edit"));
        root.Set(second, "name", new Value.TextValue("bad"));
        var property = PropertyDescriptor.New("source_ip", DataType.Text);
        // This baseline replay uses reflection solely so it also compiles before
        // Context metadata is introduced. It does not bypass runtime validation.
        typeof(PropertyDescriptor).GetProperty("UpdateFunction")?.SetValue(property, "clientIp");
        var module = new RuntimeModule().Entity(EntityDescriptor.New("Probe")
            .Property(PropertyDescriptor.New("name", DataType.Text)).Property(property)).Checker("Probe", new Checker());
        var ctx = module.IntoContext();
        var error = await Assert.ThrowsAsync<CheckException>(() => ctx.ExecuteGraphSaveAsync("validate all before writes", graph => {
            graph.PrepareInput(new object(), Request(first));
            graph.PrepareInput(new object(), Request(second));
            graph.SealPreparedGraph(); return Task.CompletedTask;
        }, _ => Task.FromResult(true)));
        Assert.Equal("/name", Assert.Single(error.Violations).InstancePath);
        Assert.Equal("name", Assert.Single(root.Change(first)).Key);
        Assert.Equal("name", Assert.Single(root.Change(second)).Key);
        Assert.Equal(new Value.TextValue("first edit"), root.Change(first)["name"]);

        MutationRequest Request(EntityKey key) => MutationRequest.Create(new UpdateCommand("Probe", key.Id) {
            Values = root.Change(key), ExpectedVersionValue = 1
        }, "check detached child", key, root);
    }

    private sealed class Checker : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        {
            var values = ((UpdateMutationRequest)request).Command.Values;
            values["source_ip"] = new Value.TextValue("prepared client IP");
            return values["name"] == new Value.TextValue("bad")
                ? [new CheckResult {RuleId = "required", Location = ObjectLocation.Property("name")}] : [];
        }
    }
}
