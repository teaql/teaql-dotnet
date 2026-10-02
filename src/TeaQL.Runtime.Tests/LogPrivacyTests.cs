using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

[CollectionDefinition("Log privacy environment", DisableParallelization = true)]
public class LogPrivacyEnvironmentCollection { }

[Collection("Log privacy environment")]
public class LogPrivacyTests
{
    private const string Flag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
    private const string Ack = "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK";

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("true", false)]
    [InlineData(Ack + " ", false)]
    [InlineData(Ack, true)]
    public void FileSinkRequiresExactAcknowledgement(string? setting, bool reveal)
    {
        var previous = Environment.GetEnvironmentVariable(Flag);
        var path = Path.Combine(Path.GetTempPath(), "teaql-log-privacy-" + Guid.NewGuid() + ".log");
        try
        {
            Environment.SetEnvironmentVariable(Flag, setting);
            var metadata = Entry("name", "PRIVATE-CUSTOMER-CANARY");
            metadata.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked };
            using (var writer = new StreamWriter(path))
            {
                new SensitiveDiagnosticSqlLogSink(writer).Write(metadata);
                new SensitiveDiagnosticSqlLogSink(writer).Write(Entry("password", "PASSWORD-CANARY"));
            }
            var output = File.ReadAllText(path);
            Assert.Equal(reveal, output.Contains("PRIVATE-CUSTOMER-CANARY"));
            Assert.DoesNotContain("PASSWORD-CANARY", output);
            Assert.Equal("PRIVATE-CUSTOMER-CANARY", metadata.Parameters[0].TryText());
            using var normal = new StringWriter();
            new TextDiagnosticSqlLogSink(normal).Write(metadata);
            Assert.DoesNotContain("PRIVATE-CUSTOMER-CANARY", normal.ToString());
        }
        finally { Environment.SetEnvironmentVariable(Flag, previous); }
    }

    [Fact]
    public void DefaultSinkHidesLiteralSqlAndScrubsTrace()
    {
        var metadata = Entry("name", "PRIVATE-CUSTOMER-CANARY");
        metadata.ParameterizedQuery = "select * from customer where name='PRIVATE-CUSTOMER-CANARY'";
        metadata.TraceChain.Add(new TraceNode("customer", 1, "load PRIVATE-CUSTOMER-CANARY")
            { Detail = "custom PRIVATE-CUSTOMER-CANARY detail" });
        metadata.MutationLineage = new[] { new TraceNode("customer", 1, "")
            { Kind = "auditReason", Detail = "change PRIVATE-CUSTOMER-CANARY" } };
        using var writer = new StringWriter();
        new TextDiagnosticSqlLogSink(writer).Write(metadata);
        Assert.DoesNotContain("PRIVATE-CUSTOMER-CANARY", writer.ToString());
        Assert.Contains("NOT REPLAYABLE", writer.ToString());
        Assert.Contains("PRIVATE-CUSTOMER-CANARY", metadata.TraceChain[0].Comment);
        Assert.Contains("PRIVATE-CUSTOMER-CANARY", metadata.TraceChain[0].Detail);
        Assert.Equal(1UL, metadata.TraceChain[0].EntityId);
        Assert.Contains("PRIVATE-CUSTOMER-CANARY", metadata.MutationLineage[0].Detail);
    }

    private static ExecutionMetadata Entry(string field, string value) => new()
    {
        Operation = DataServiceOperation.Query,
        ParameterizedQuery = $"select * from customer where {field} = ?",
        Parameters = new Value[] { new Value.TextValue(value) },
        DebugQuery = $"select * from customer where {field} = '{value}'",
        Comment = "load " + value, Purpose = "test privacy"
    };
}
