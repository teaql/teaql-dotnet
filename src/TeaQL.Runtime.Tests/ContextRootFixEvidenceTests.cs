using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public sealed class ContextRootFixEvidenceTests
{
    [Theory]
    [InlineData("absent")]
    [InlineData("null")]
    [InlineData("typed-null")]
    public void ContextDefaultsPublishSafeProvenanceEvenWithoutAGeneratedChecker(string kind)
    {
        var context = ContextRootAdmissionTests.Module().IntoContext().WithActiveRoot("DomainRoot", 17);
        var command = new InsertCommand("OwnedRecord").Value("name", "ordinary");
        if (kind != "absent") command.Values["managed_root"] = kind == "null"
            ? new Value.NullValue() : new Value.TypedNullValue(DataType.I64);
        context.CheckAndFix(new InsertMutationRequest(command, "supply the modeled context default"));
        Assert.Equal(new Value.I64Value(17), command.Values["managed_root"]);
        Assert.Equal(Expected, Assert.Single(context.LastFixEvidence));
        Assert.DoesNotContain("17", context.LastFixEvidence.ToString()!);
    }

    [Fact]
    public void ASecondFixDoesNotInventAnAdditionalContextDefaultOrMutateTheOldReceipt()
    {
        var context = ContextRootAdmissionTests.Module().IntoContext().WithActiveRoot("DomainRoot", 17);
        var request = new InsertMutationRequest(new InsertCommand("OwnedRecord"), "fix once only");
        context.CheckAndFix(request); var first = context.LastFixEvidence;
        Assert.Equal(Expected, Assert.Single(first));
        context.CheckAndFix(request);
        Assert.Same(first, context.LastFixEvidence); Assert.Equal(Expected, Assert.Single(first));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AnExplicitValueIsNotFalselyAttributedToContext(bool unsigned)
    {
        var context = ContextRootAdmissionTests.Module().IntoContext().WithActiveRoot("DomainRoot", 17);
        context.CheckAndFix(new InsertMutationRequest(new InsertCommand("OwnedRecord")
            .Value("managed_root", unsigned ? new Value.U64Value(17) : new Value.I64Value(17)), "preserve explicit provenance"));
        Assert.Empty(context.LastFixEvidence);
    }

    [Fact]
    public void CustomCheckerFailureRetainsOnlyTheActualSafeContextSource()
    {
        var context = ContextRootAdmissionTests.Module(new ThrowingChecker()).IntoContext().WithActiveRoot("DomainRoot", 17);
        Assert.Throws<InvalidOperationException>(() => context.CheckAndFix(
            new InsertMutationRequest(new InsertCommand("OwnedRecord"), "record the applied default before checker failure")));
        Assert.Equal(Expected, Assert.Single(context.LastFixEvidence));
    }

    [Fact]
    public void RejectedOwnershipDoesNotReplaceThePreviousContextFixReceipt()
    {
        var context = ContextRootAdmissionTests.Module().IntoContext().WithActiveRoot("DomainRoot", 17);
        context.CheckAndFix(new InsertMutationRequest(new InsertCommand("OwnedRecord"), "prepare a known receipt"));
        var first = context.LastFixEvidence; Assert.Equal(Expected, Assert.Single(first));
        context.WithActiveRoot("DomainRoot", 18);
        Assert.Throws<CheckException>(() => context.CheckAndFix(ContextRootAdmissionTests.Update()));
        Assert.Same(first, context.LastFixEvidence);
    }

    private static FixEvidence Expected => new("OwnedRecord", "managed_root", FixEvidenceSource.Context, "activeRoot:DomainRoot");
    private sealed class ThrowingChecker : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now) =>
            throw new InvalidOperationException("synthetic checker failure");
    }
}
