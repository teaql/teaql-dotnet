using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class ContextFixTests
{
    [Fact]
    public void ExistingCompiledGeneratedBridgesRetainTheirMethodSignatures()
    {
        Assert.NotNull(typeof(GraphMutationSession).GetMethod("PrepareInput", [typeof(object), typeof(MutationRequest)]));
        Assert.Equal(typeof(void), typeof(GraphMutationSession).GetMethod("DiscardRestoredFields")!.ReturnType);
    }
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2032-02-29T10:15:30Z");
    private static RuntimeModule Module() => new RuntimeModule().Entity(EntityDescriptor.New("Probe")
        .Property(PropertyDescriptor.New("name", DataType.Text))
        .Property(PropertyDescriptor.New("note", DataType.Text))
        .Property(PropertyDescriptor.New("born_at", DataType.Timestamp).OnCreateFromContext("now"))
        .Property(PropertyDescriptor.New("touched_at", DataType.Timestamp).OnCreateFromContext("now").OnUpdateFromContext("now"))
        .Property(PropertyDescriptor.New("source_ip", DataType.Text).OnCreateFromContext("ip").OnUpdateFromContext("ip")))
        .Checker("Probe", new Checker());
    private static UserContext Context() => Module().IntoContext().WithBusinessClock(new FixedBusinessClock(Now));
    private static UpdateMutationRequest Update(Record values, Record? old = null) => new(new UpdateCommand("Probe", new Value.I64Value(1)) {
        Values = values, OldValues = old, ExpectedVersionValue = 7
    }, "check frozen mutation intent");

    [Theory]
    [InlineData("NORMALIZE", true)]
    [InlineData("REPAIR_OTHER", false)]
    [InlineData("UNKNOWN_BASELINE", false)]
    [InlineData("MUTATE_BASELINE", false)]
    public void FinalBusinessIntentNotAnEarlierDirtyFlagControlsContextRetention(string mode, bool noOp)
    {
        var ctx = Context();
        ctx.InsertResource<IContextFixResolver>(new Resolver((_, input) => input.Function == "now"
            ? new Value.TimestampValue(input.Now.ToUnixTimeMilliseconds()) : new Value.TextValue("prepared IP")));
        var values = new Record { ["name"] = new Value.TextValue(mode) };
        ctx.CheckAndFix(Update(values, mode == "UNKNOWN_BASELINE" ? null : new Record {
            ["name"] = new Value.TextValue(mode == "MUTATE_BASELINE" ? "different" : "same") }));
        Assert.Equal(new Value.TextValue("same"), values["name"]);
        Assert.Equal(!noOp, values.ContainsKey("touched_at"));
        Assert.Equal(!noOp, values.ContainsKey("source_ip"));
        if (mode == "REPAIR_OTHER") Assert.Equal(new Value.TextValue("business repair"), values["note"]);
    }

    [Theory]
    [InlineData("empty")]
    [InlineData("managed")]
    [InlineData("restored")]
    [InlineData("readonly")]
    public void NoBusinessIntentDoesNotReadTheContextProviderOrManufactureAssignments(string kind)
    {
        var ctx = Context(); var resolver = new Resolver((_, _) => throw new Exception("must not resolve no-op"));
        ctx.InsertResource<IContextFixResolver>(resolver);
        var values = kind switch {
            "managed" => new Record { ["touched_at"] = new Value.TimestampValue(1) },
            "restored" => new Record { ["name"] = new Value.TextValue("same") },
            "readonly" => new Record { ["_count"] = new Value.I64Value(1) }, _ => new Record()
        };
        if (kind == "readonly")
            Assert.Throws<CheckException>(() => ctx.CheckAndFix(Update(values)));
        else ctx.CheckAndFix(Update(values, new Record { ["name"] = new Value.TextValue("same") }));
        Assert.Equal(0, resolver.Calls); Assert.False(values.ContainsKey("source_ip"));
        if (kind == "managed") Assert.Equal(new Value.TimestampValue(1), values["touched_at"]);
    }

    [Fact]
    public void CreateWithoutCallerFieldsAppliesDefaultsAndUpdatePreservesCreationTime()
    {
        var ctx = Context(); var resolver = new Resolver((_, input) => input.Function == "now"
            ? new Value.TimestampValue(input.Now.ToUnixTimeMilliseconds()) : new Value.TextValue("prepared IP"));
        ctx.InsertResource<IContextFixResolver>(resolver);
        var insert = new InsertMutationRequest(new InsertCommand("Probe"), "create default fields");
        ctx.CheckAndFix(insert); Assert.Equal(3, resolver.Calls);
        Assert.Equal(new Value.TimestampValue(Now.ToUnixTimeMilliseconds()), insert.Command.Values["born_at"]);
        var values = new Record { ["name"] = new Value.TextValue("edit"), ["born_at"] = new Value.TimestampValue(17) };
        ctx.CheckAndFix(Update(values)); Assert.Equal(5, resolver.Calls);
        Assert.Equal(new Value.TimestampValue(17), values["born_at"]);
        Assert.Equal(new Value.TextValue("prepared IP"), values["source_ip"]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ProviderFailureIsSafeAndFieldLocalAndRestoresEarlierAutomaticAssignments(bool wrongType)
    {
        var ctx = Context(); var resolver = new Resolver((_, input) => input.Function == "now"
            ? new Value.TimestampValue(input.Now.ToUnixTimeMilliseconds())
            : wrongType ? new Value.I64Value(4) : throw new InvalidOperationException("private-provider-secret"));
        ctx.InsertResource<IContextFixResolver>(resolver);
        var values = new Record { ["name"] = new Value.TextValue("caller edit"), ["touched_at"] = new Value.TimestampValue(5) };
        var error = Assert.Throws<CheckException>(() => ctx.CheckAndFix(Update(values)));
        Assert.Equal("/sourceIp", Assert.Single(error.Violations).InstancePath);
        Assert.Equal("source_ip", Assert.Single(error.Violations).ModelPath);
        Assert.Null(error.InnerException); Assert.DoesNotContain("private-provider-secret", error.ToString());
        Assert.Equal(new Value.TimestampValue(5), values["touched_at"]); Assert.False(values.ContainsKey("source_ip"));
        Assert.Equal(new Value.TextValue("caller edit"), values["name"]);
        ctx.InsertResource<IContextFixResolver>(new Resolver((_, input) => input.Function == "now"
            ? new Value.TimestampValue(input.Now.ToUnixTimeMilliseconds()) : new Value.TextValue("retry IP")));
        ctx.CheckAndFix(Update(values)); Assert.Equal(new Value.TextValue("retry IP"), values["source_ip"]);
    }

    [Fact]
    public void RejectedCheckRestoresContextFieldsButKeepsCustomBusinessRepair()
    {
        var ctx = Context(); ctx.InsertResource<IContextFixResolver>(new Resolver((_, input) => input.Function == "now"
            ? new Value.TimestampValue(input.Now.ToUnixTimeMilliseconds()) : new Value.TextValue("IP")));
        ctx.Install(new RuntimeModule().Entity(Module().Metadata.GetEntity("Probe")!).Checker("Probe", new Checker(reject: true)));
        var values = new Record { ["name"] = new Value.TextValue("before") };
        Assert.Throws<CheckException>(() => ctx.CheckAndFix(Update(values)));
        Assert.Equal("name", Assert.Single(values).Key); Assert.Equal(new Value.TextValue("custom repair"), values["name"]);
    }

    [Fact]
    public void DefaultClockAndUniqueStringAreTypedAndNoFrameEscapesTheInvocation()
    {
        var module = new RuntimeModule().Entity(EntityDescriptor.New("Token")
            .Property(PropertyDescriptor.New("when", DataType.Timestamp).OnCreateFromContext("now"))
            .Property(PropertyDescriptor.New("token", DataType.Text).OnCreateFromContext("uniStr")))
            .Checker("Token", new TokenChecker());
        var ctx = module.IntoContext().WithBusinessClock(new FixedBusinessClock(Now));
        var request = new InsertMutationRequest(new InsertCommand("Token"), "assign typed defaults");
        ctx.CheckAndFix(request); Assert.Equal(new Value.TimestampValue(Now.ToUnixTimeMilliseconds()), request.Command.Values["when"]);
        Assert.StartsWith("prefix-", ((Value.TextValue)request.Command.Values["token"]).Value);
        Assert.False(ContextFix.Allowed(ctx, request));
        Assert.Throws<CheckException>(() => ContextFix.Apply(ctx, request, Now, "when", "now"));
    }

    [Fact]
    public void OutOfRangeTimestampFailsAtTheFieldBeforeProviderOrNativeHydration()
    {
        var ctx = Context(); ctx.InsertResource<IContextFixResolver>(new Resolver((_, _) => new Value.TimestampValue(long.MaxValue)));
        var values = new Record { ["name"] = new Value.TextValue("caller edit") };
        var error = Assert.Throws<CheckException>(() => ctx.CheckAndFix(Update(values)));
        Assert.Equal("touched_at", Assert.Single(error.Violations).ModelPath);
        Assert.Equal("name", Assert.Single(values).Key); Assert.Null(error.InnerException);
    }

    private sealed class Checker(bool reject = false) : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext ctx, MutationRequest request, DateTimeOffset now)
        {
            if (request is InsertMutationRequest) ContextFix.Apply(ctx, request, now, "born_at", "now");
            ContextFix.Apply(ctx, request, now, "touched_at", "now");
            ContextFix.Apply(ctx, request, now, "source_ip", "ip");
            if (request is UpdateMutationRequest update && update.Command.Values.TryGetValue("name", out var name)
                && name is Value.TextValue text && text.Value is "NORMALIZE" or "REPAIR_OTHER" or "UNKNOWN_BASELINE" or "MUTATE_BASELINE") {
                update.Command.Values["name"] = new Value.TextValue("same");
                if (text.Value == "MUTATE_BASELINE") update.Command.OldValues!["name"] = new Value.TextValue("same");
                if (text.Value == "REPAIR_OTHER") update.Command.Values["note"] = new Value.TextValue("business repair");
            }
            if (!reject) return [];
            ((UpdateMutationRequest)request).Command.Values["name"] = new Value.TextValue("custom repair");
            return [new CheckResult {RuleId = "required", Location = ObjectLocation.Property("name")}];
        }
    }
    private sealed class TokenChecker : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext ctx, MutationRequest request, DateTimeOffset now) {
            ContextFix.Apply(ctx, request, now, "when", "now");
            ContextFix.Apply(ctx, request, now, "token", "uniStr", ["prefix-"]); return [];
        }
    }
    private sealed class Resolver(Func<UserContext, ContextFixInput, Value> resolve) : IContextFixResolver
    {
        public int Calls { get; private set; }
        public Value Resolve(UserContext ctx, ContextFixInput input) { Calls++; return resolve(ctx, input); }
    }
}
