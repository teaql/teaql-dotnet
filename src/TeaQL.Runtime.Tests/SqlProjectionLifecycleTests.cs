using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

[Collection("Log privacy environment")]
public class SqlProjectionLifecycleTests
{
    private static ExecutionMetadata Entry() => new()
    {
        Backend = "sqlite", GeneratedSql = true, ParameterizedQuery = "SELECT ? LIMIT 10000",
        Parameters = new[] { Value.FromObject("Riverside") },
        ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked },
        Comment = "what: find Riverside", Purpose = "why: check privacy",
        TraceChain = new() { new("Customer", 1, "Riverside") { Kind = "Riverside" } }
    };

    [Fact]
    public void SafeProjectionCannotBeRelabeledAsDebug()
    {
        var safe = LogPrivacy.Project(Entry());
        var again = LogPrivacy.Project(safe, true);
        Assert.Equal("SAFE", again.LogMode);
        Assert.Equal(safe.DebugQuery, again.DebugQuery);
        Assert.DoesNotContain("Riverside", JsonSerializer.Serialize(again));
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void MalformedMaskFlagsFailClosed(bool debug)
    {
        var source = Entry();
        source.MaskedParameters = new[] { false, false };
        var safe = LogPrivacy.Project(source, debug);
        Assert.Equal("mask_count_mismatch", safe.SqlOmissionReason);
        Assert.DoesNotContain("Riverside", JsonSerializer.Serialize(safe));
    }

    [Theory]
    [InlineData("copy")] [InlineData("modified")] [InlineData("header")]
    public void DebugWithoutReliableProvenanceHidesFreeFormIntent(string mode)
    {
        var debug = LogPrivacy.Project(Entry(), true);
        if (mode != "modified") debug = new ExecutionMetadata
        {
            Backend = debug.Backend, GeneratedSql = true, ParameterizedQuery = debug.ParameterizedQuery,
            Parameters = debug.Parameters, ParameterLogPolicies = debug.ParameterLogPolicies,
            MaskedParameters = debug.MaskedParameters, DebugQuery = debug.DebugQuery,
            LogMode = mode == "header" ? null : debug.LogMode
        };
        debug.Comment = "what: inherited ORIGINAL-WRITE-CANARY";
        debug.Purpose = "why: ORIGINAL-WRITE-CANARY";
        debug.AuditReason = "ORIGINAL-WRITE-CANARY";
        debug.BackendRequestId = "ORIGINAL-WRITE-CANARY";
        debug.TraceChain = new() { new("ORIGINAL-WRITE-CANARY", 1, "ORIGINAL-WRITE-CANARY")
            { Name = "ORIGINAL-WRITE-CANARY", Kind = "ORIGINAL-WRITE-CANARY" } };
        var safe = LogPrivacy.Project(debug);
        Assert.DoesNotContain("CANARY", JsonSerializer.Serialize(safe));
        Assert.Contains("SELECT 'Ri*****de' /* masked */ LIMIT 10000", safe.DebugQuery);
    }

    [Fact]
    public void RetainedDebugHasIndependentSafeCopiesAndSupportsConcurrentReads()
    {
        var source = Entry();
        source.Parameters = new Value[] { new Value.ListValue(new() { Value.FromObject("Riverside") }) };
        source.Backend = "postgresql";
        var debug = LogPrivacy.Project(source, true);
        var first = LogPrivacy.Project(debug);
        ((Value.ListValue)first.Parameters[0]).Values[0] = Value.FromObject("POISON");
        first.TraceChain.Clear();
        Parallel.For(0, 16, _ =>
        {
            var safe = LogPrivacy.Project(debug);
            Assert.Equal("Ri*****de", ((Value.ListValue)safe.Parameters[0]).Values[0].TryText());
            Assert.Single(safe.TraceChain);
            Assert.Equal("what: find [REDACTED]", safe.Comment);
            Assert.DoesNotContain("Riverside", JsonSerializer.Serialize(safe));
        });
    }
}
