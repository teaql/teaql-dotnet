using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

[Collection("Log privacy environment")]
public class MaskingContractTests
{
    [Fact]
    public void MaskGolden()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root != null && !File.Exists(Path.Combine(root.FullName, "test-vectors/masking-v1.tsv"))) root = root.Parent;
        Assert.NotNull(root);
        foreach (var line in File.ReadAllLines(Path.Combine(root!.FullName, "test-vectors/masking-v1.tsv")).Skip(1))
        {
            var values = line.Split('\t');
            Assert.Equal(values[2], LogPrivacy.MaskAuditValue(values[1]));
        }
    }

    private const string Flag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
    private const string Ack = "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK";

    [Theory]
    [InlineData("", "")]
    [InlineData("Ada", "***")]
    [InlineData("12345678", "********")]
    [InlineData("ABCDEFGH", "AB****GH")]
    [InlineData("Riverside", "Ri*****de")]
    [InlineData("O'Reilly", "O'****ly")]
    public void MaskContractExpandedSql(string raw, string masked)
    {
        var old = Environment.GetEnvironmentVariable(Flag);
        try {
            Environment.SetEnvironmentVariable(Flag, null);
            var source = Entry(raw);
            using var output = new StringWriter();
            new TextDiagnosticSqlLogSink(output).Write(source);
            var log = output.ToString();
            Assert.Equal(raw, source.Parameters[0].TryText());
            // No field policy is attached: unknown parameters need complete masking.
            Assert.Contains("name = '", log);
            if (raw.Length >= 8 && !masked.StartsWith("*")) Assert.DoesNotContain(masked.Replace("'", "''"), log);
            Assert.Contains("masked", log.ToLowerInvariant());
            Assert.DoesNotContain("name = ?", log);
            Assert.DoesNotContain("[REDACTED SQL", log);
            if (raw.Length > 0) Assert.DoesNotContain("'" + raw.Replace("'", "''") + "'", log);
        } finally { Environment.SetEnvironmentVariable(Flag, old); }
    }

    [Fact]
    public void MaskContractDebugProvenance()
    {
        var old = Environment.GetEnvironmentVariable(Flag);
        try {
            Environment.SetEnvironmentVariable(Flag, Ack);
            for (int i = 0; i < 2; i++) {
                using var output = new StringWriter();
                var entry = Entry("Riverside");
                entry.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked };
                new SensitiveDiagnosticSqlLogSink(output).Write(entry);
                var log = output.ToString();
                Assert.Contains("'Riverside'", log);
                Assert.Contains("DEBUG", log.ToUpperInvariant());
                Assert.Contains("PLAINTEXT", log.ToUpperInvariant());
            }
        } finally { Environment.SetEnvironmentVariable(Flag, old); }
    }

    private static ExecutionMetadata Entry(string value) => new() {
        Operation = DataServiceOperation.Query,
        ParameterizedQuery = "UPDATE customer SET name = ?",
        Parameters = new Value[] { new Value.TextValue(value) },
        DebugQuery = "UPDATE customer SET name = '" + value.Replace("'", "''") + "'",
        Comment = "what: edit customer", Purpose = "why: verify mask contract"
    };
}
