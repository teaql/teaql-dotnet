using System.Text.Json.Nodes;
using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class MutationFixLedgerTests
{
    private static readonly DateTimeOffset Now = new(2032, 2, 29, 10, 15, 30, TimeSpan.Zero);
    private static UserContext Context(IEntityChecker checker) => new UserContext()
        .WithBusinessClock(new FixedBusinessClock(Now)).Install(new RuntimeModule().Checker("Probe", checker));
    private static UpdateMutationRequest Request(EntityRoot ledger, EntityKey key, Record values) =>
        (UpdateMutationRequest)MutationRequest.Create(new UpdateCommand("Probe", new Value.I64Value(1))
            { Values = values, ExpectedVersionValue = 7 }, "verify checked Fix writeback", key, ledger);

    [Fact]
    public void SuccessfulFixUsesCanonicalKeysWithoutCommittingOrChangingTypedVersions()
    {
        var ledger = new EntityRoot(); var key = new EntityKey("Probe", 1); var other = new EntityKey("Other", 1);
        ledger.SetOriginalVersion(key, 7); ledger.SetOriginalVersion(other, 19);
        ledger.Set(key, "internal_note", new Value.TextValue("before"));
        ledger.Set(other, "internal_note", new Value.TextValue("other pending"));
        var context = Context(new Checker((_, request, now) => {
            var values = ((UpdateMutationRequest)request).Command.Values;
            values["internal_note"] = new Value.TextValue("normalized");
            values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds()); return [];
        }));
        context.CheckAndFix(Request(ledger, key, ledger.Change(key)));
        Assert.Equal(new[] { "internal_note", "update_time" }, ledger.Change(key).Keys.Order());
        Assert.Equal(new Value.TextValue("normalized"), ledger.Change(key)["internal_note"]);
        Assert.Equal(new Value.TimestampValue(Now.ToUnixTimeMilliseconds()), ledger.Change(key)["update_time"]);
        Assert.Equal(7, ledger.OriginalVersion(key)); Assert.Equal(19, ledger.OriginalVersion(other));
        Assert.Equal(new Value.TextValue("other pending"), Assert.Single(ledger.Change(other)).Value);
        Assert.True(ledger.HasPending(key)); // No provider or resource is installed.
    }

    [Fact]
    public void AThrowingCheckerCannotPublishPartialFixValues()
    {
        var ledger = new EntityRoot(); var key = new EntityKey("Probe", 1); ledger.Set(key, "internal_note", new Value.TextValue("before"));
        var original = new InvalidOperationException("injected Checker failure");
        var context = Context(new Checker((_, request, now) => {
            ((UpdateMutationRequest)request).Command.Values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds()); throw original;
        }));
        Assert.Same(original, Assert.Throws<InvalidOperationException>(() => context.CheckAndFix(Request(ledger, key, ledger.Change(key)))));
        Assert.Equal("internal_note", Assert.Single(ledger.Change(key)).Key);
    }

    [Fact]
    public void ARejectedCheckerCannotPublishPartialFixValues()
    {
        var ledger = new EntityRoot(); var key = new EntityKey("Probe", 1); ledger.Set(key, "internal_note", new Value.TextValue("before"));
        var context = Context(new Checker((_, request, now) => {
            ((UpdateMutationRequest)request).Command.Values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            return [new CheckResult { RuleId = "invalid_type", Location = ObjectLocation.Property("internal_note"), Message = "rejected fixture" }];
        }));
        Assert.Throws<CheckException>(() => context.CheckAndFix(Request(ledger, key, ledger.Change(key))));
        Assert.Equal("internal_note", Assert.Single(ledger.Change(key)).Key);
    }

    [Fact]
    public void InPlaceJsonFixIsDetectedAndCannotRetainAMutableAliasIntoTheLedger()
    {
        var ledger = new EntityRoot(); var key = new EntityKey("Probe", 1); var json = JsonNode.Parse("{\"code\":\"before\"}")!;
        var request = Request(ledger, key, new Record { ["payload"] = new Value.JsonValue(json) });
        var context = Context(new Checker((_, mutation, _) => {
            Assert.IsType<Value.JsonValue>(((UpdateMutationRequest)mutation).Command.Values["payload"]).Value!["code"] = "fixed"; return [];
        }));
        context.CheckAndFix(request); json["code"] = "changed after Fix";
        Assert.Equal("fixed", Assert.IsType<Value.JsonValue>(ledger.Change(key)["payload"]).Value!["code"]!.GetValue<string>());
    }

    [Fact]
    public void AMismatchedTypedLedgerKeyDoesNotReceiveAnotherEntitysFix()
    {
        var ledger = new EntityRoot(); var key = new EntityKey("Other", 1);
        var context = Context(new Checker((_, request, now) => {
            ((UpdateMutationRequest)request).Command.Values["update_time"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds()); return [];
        }));
        context.CheckAndFix(Request(ledger, key, new Record()));
        Assert.Empty(ledger.Changes());
    }

    [Fact]
    public void AnUnchangedDetachedCommandDoesNotFabricatePendingBusinessFields()
    {
        var ledger = new EntityRoot(); var key = new EntityKey("Probe", 1);
        var context = Context(new Checker((_, _, _) => []));
        context.CheckAndFix(Request(ledger, key, new Record { ["internal_note"] = new Value.TextValue("unchanged") }));
        Assert.Empty(ledger.Changes());
    }

    private sealed class Checker(Func<UserContext, MutationRequest, DateTimeOffset, IReadOnlyList<CheckResult>> run) : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now) => run(context, request, now);
    }
}
