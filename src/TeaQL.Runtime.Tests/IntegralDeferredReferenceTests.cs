using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public class IntegralDeferredReferenceTests
{
    [Theory]
    [InlineData("valid")]
    [InlineData("wrong-type")]
    [InlineData("other-ledger")]
    [InlineData("not-preparing")]
    [InlineData("changed-by-fix")]
    [InlineData("introduced-by-fix")]
    [InlineData("target-no-longer-new")]
    public void IntegralDeferredReferenceRequiresOriginalTypedLedgerOwnedNewTarget(string scenario)
    {
        var descriptor = EntityDescriptor.New("Child")
            .Property(PropertyDescriptor.New("platform", DataType.I64))
            .Relation(RelationDescriptor.New("platform", "Platform").LocalKey("platform").ForeignKey("id"));
        var ledger = new EntityRoot(); var key = new EntityKey("Child", -1);
        var target = new EntityKey(scenario == "wrong-type" ? "OtherPlatform" : "Platform", -2);
        ledger.MarkAsNew(key);
        if (scenario == "other-ledger") new EntityRoot().MarkAsNew(target); else ledger.MarkAsNew(target);
        var command = new InsertCommand("Child");
        if (scenario != "introduced-by-fix") command.Value("platform", new Value.I64Value(-2));
        var request = new InsertMutationRequest(command, "prepare typed reference") { LedgerKey = key, LedgerRoot = ledger };
        var captured = IntegralScalarAdmission.CaptureDeferred(descriptor, request, scenario != "not-preparing");
        if (scenario == "changed-by-fix") { ledger.MarkAsNew(new EntityKey("Platform", -3)); command.Values["platform"] = new Value.I64Value(-3); }
        if (scenario == "introduced-by-fix") command.Values["platform"] = new Value.I64Value(-2);
        if (scenario == "target-no-longer-new") ledger.MarkAsPersisted(target);
        var errors = IntegralScalarAdmission.Check(descriptor, request, captured).ToArray();
        if (scenario == "valid") Assert.Empty(errors);
        else Assert.Equal("invalid_type", Assert.Single(errors).RuleId);
    }
}
