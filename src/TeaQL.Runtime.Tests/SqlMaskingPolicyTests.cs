using TeaQL.Core;
using TeaQL.DataService;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

[Collection("Log privacy environment")]
public class SqlMaskingPolicyTests
{
    private static ExecutionMetadata Entry(string sql, params Value[] values) => new()
    {
        Backend = "sqlite", ParameterizedQuery = sql, Parameters = values,
        Comment = "what: verify projections", Purpose = "why: preserve field policy", ResultCount = 1
    };

    [Fact]
    public void MixedPoliciesKeepKnownValuesAndHideUnknownAndCredentialsUnderDebug()
    {
        var source = Entry("SELECT ?, ?, ?, ?", Value.FromObject("Ordinary"), Value.FromObject("Riverside"),
            Value.FromObject("UNKNOWN-CANARY"), Value.FromObject("PASSWORD-CANARY"));
        source.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Plain, SqlParameterLogPolicy.Masked,
            SqlParameterLogPolicy.Unknown, SqlParameterLogPolicy.Credential };
        source.Comment = "what: locate UNKNOWN-CANARY PASSWORD-CANARY";
        var safe = LogPrivacy.Project(source);
        Assert.Equal(new[] { false, true, true, true }, safe.MaskedParameters);
        Assert.Contains("'Ordinary', 'Ri*****de' /* masked */", safe.DebugQuery);
        Assert.DoesNotContain("CANARY", safe.Comment);
        var debug = LogPrivacy.Project(source, true);
        Assert.Equal(new[] { false, false, true, true }, debug.MaskedParameters);
        Assert.Contains("'Riverside'", debug.DebugQuery);
        Assert.Contains("DEBUG PLAINTEXT; EXPLICIT OPT-IN", debug.DebugQuery);
        Assert.Contains("NOT REPLAYABLE", debug.DebugQuery);
        Assert.DoesNotContain("CANARY", debug.DebugQuery);
        Assert.Equal("UNKNOWN-CANARY", source.Parameters[2].TryText());
        Assert.Equal(safe.DebugQuery, LogPrivacy.Project(safe).DebugQuery);
        Assert.DoesNotContain("Riverside", LogPrivacy.Project(safe, true).DebugQuery);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MissingOrInvalidPoliciesNeverAuthorizeDebug(bool invalid)
    {
        var source = Entry("SELECT ?", Value.FromObject("UNKNOWN-CANARY"));
        if (invalid) source.ParameterLogPolicies = new[] { (SqlParameterLogPolicy)999 };
        var safe = LogPrivacy.Project(source, true);
        Assert.Equal("[REDACTED]", safe.Parameters[0].TryText());
        Assert.Contains("SELECT '[REDACTED]' /* masked */", safe.DebugQuery);
    }

    [Fact]
    public void PolicyCountMismatchFailsClosedAndDebugRevocationReprojects()
    {
        var source = Entry("SELECT ?", Value.FromObject("Riverside"));
        source.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Plain, SqlParameterLogPolicy.Plain };
        var invalid = LogPrivacy.Project(source, true);
        Assert.Equal("policy_count_mismatch", invalid.SqlOmissionReason);
        Assert.DoesNotContain("Riverside", invalid.DebugQuery);
        Assert.Equal("[REDACTED]", invalid.Parameters[0].TryText());
        source.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked };
        var debug = LogPrivacy.Project(source, true);
        Assert.Contains("Riverside", debug.DebugQuery);
        var safe = LogPrivacy.Project(debug, false);
        Assert.Contains("Ri*****de", safe.DebugQuery);
        Assert.DoesNotContain("Riverside", safe.DebugQuery);
        Assert.DoesNotContain("Riverside", LogPrivacy.Project(safe, true).DebugQuery);
    }

    [Fact]
    public void RendererUsesProjectedValuesAndIgnoresSuppliedRawDebugSql()
    {
        var source = Entry("SELECT $1, $2, $1, '$1' /* $2 */", Value.FromObject("O'Reilly"), Value.FromObject(true));
        source.GeneratedSql = true; source.Backend = "postgresql";
        source.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked, SqlParameterLogPolicy.Plain };
        source.DebugQuery = "RAW-DEBUG-CANARY";
        var safe = LogPrivacy.Project(source);
        Assert.Contains("'O''****ly' /* masked */, TRUE, 'O''****ly' /* masked */, '$1'", safe.DebugQuery);
        Assert.DoesNotContain("CANARY", safe.DebugQuery);
        Assert.Null(safe.SqlOmissionReason);
    }

    [Theory]
    [InlineData("SELECT $0")]
    [InlineData("SELECT $2")]
    [InlineData("SELECT ?, ?")]
    [InlineData("SELECT 1")]
    [InlineData("SELECT 'unterminated")]
    [InlineData("SELECT :named")]
    public void MalformedOrUntrustedSqlFailsClosed(string sql)
    {
        var source = Entry(sql, Value.FromObject("BIND-CANARY"));
        source.Backend = "postgresql";
        var safe = LogPrivacy.Project(source, true);
        Assert.NotNull(safe.SqlOmissionReason);
        Assert.DoesNotContain("BIND-CANARY", safe.DebugQuery);
        Assert.DoesNotContain("SELECT", safe.DebugQuery);
        Assert.Contains("NOT REPLAYABLE", safe.DebugQuery);
    }

    [Fact]
    public void ArraysNullsAndNestedCredentialsUseIndependentCopies()
    {
        var source = Entry("SELECT $1, $2, $3", new Value.ListValue(new() { Value.FromObject("Riverside"), Value.FromObject("12345678") }),
            new Value.NullValue(), new Value.ObjectValue(new Record { ["access_token"] = Value.FromObject("TOKEN-CANARY") }));
        source.Backend = "postgresql";
        source.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked, SqlParameterLogPolicy.Masked, SqlParameterLogPolicy.Plain };
        var safe = LogPrivacy.Project(source);
        Assert.Contains("ARRAY['Ri*****de', '********']", safe.DebugQuery);
        Assert.Contains("NULL /* masked */", safe.DebugQuery);
        Assert.DoesNotContain("TOKEN-CANARY", safe.DebugQuery);
        ((Value.ListValue)safe.Parameters[0]).Values[0] = Value.FromObject("changed");
        Assert.Equal("Riverside", ((Value.ListValue)source.Parameters[0]).Values[0].TryText());
        Assert.DoesNotContain("TOKEN-CANARY", LogPrivacy.Project(source, true).DebugQuery);
    }

    [Fact]
    public void AllGoldenVectorsAreAppliedAtSqlProjectionNotOnlyHelper()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root != null && !File.Exists(Path.Combine(root.FullName, "test-vectors/masking-v1.tsv"))) root = root.Parent;
        Assert.NotNull(root);
        foreach (var line in File.ReadAllLines(Path.Combine(root!.FullName, "test-vectors/masking-v1.tsv")).Skip(1))
        {
            var parts = line.Split('\t');
            var source = Entry("SELECT ?", Value.FromObject(parts[1]));
            source.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked };
            var safe = LogPrivacy.Project(source);
            Assert.Equal(parts[2], safe.Parameters[0].TryText());
            Assert.Contains("'" + parts[2].Replace("'", "''") + "' /* masked */", safe.DebugQuery);
            Assert.Equal(parts[1], source.Parameters[0].TryText());
        }
    }

    [Fact]
    public void TextSinksRetainParameterCountWithoutReintroducingRawParameterArrays()
    {
        var source = Entry("SELECT ?, ?", Value.FromObject("Riverside"), Value.FromObject("Ordinary"));
        source.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked, SqlParameterLogPolicy.Plain };
        using var ordinary = new StringWriter();
        using var sensitive = new StringWriter();
        new TextDiagnosticSqlLogSink(ordinary).Write(source);
        var previous = Environment.GetEnvironmentVariable(LogPrivacy.EnvironmentName);
        try
        {
            Environment.SetEnvironmentVariable(LogPrivacy.EnvironmentName, null);
            new SensitiveDiagnosticSqlLogSink(sensitive).Write(source);
        }
        finally { Environment.SetEnvironmentVariable(LogPrivacy.EnvironmentName, previous); }
        foreach (var output in new[] { ordinary.ToString(), sensitive.ToString() })
        {
            Assert.Contains("parameterCount=2", output);
            Assert.Contains("'Ri*****de' /* masked */, 'Ordinary'", output);
            Assert.DoesNotContain("Riverside", output);
            Assert.DoesNotContain("Parameterized SQL", output);
        }
    }

    [Fact]
    public void BatchChildrenKeepIndependentPositionalBindingsAndIntent()
    {
        var one = Entry("UPDATE customer SET name = $1", Value.FromObject("Riverside"));
        one.Backend = "postgresql"; one.Operation = DataServiceOperation.Update;
        one.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked };
        one.AuditReason = "first change";
        var two = Entry("UPDATE customer SET name = $1", Value.FromObject("Lakeside"));
        two.Backend = "postgresql"; two.Operation = DataServiceOperation.Update;
        two.ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked };
        two.AuditReason = "second change";
        var batch = new ExecutionMetadata { Operation = DataServiceOperation.Batch, Statements = new[] { one, two } };
        using var output = new StringWriter();
        var context = new UserContext().WithDiagnosticSqlLogSink(new TextDiagnosticSqlLogSink(output));
        context.RecordExecutionMetadata(batch);
        var text = output.ToString();
        Assert.Contains("'Ri*****de' /* masked */", text);
        Assert.Contains("'La****de' /* masked */", text);
        Assert.Contains("first change", text); Assert.Contains("second change", text);
        Assert.DoesNotContain("Riverside", text); Assert.DoesNotContain("Lakeside", text);
        Assert.DoesNotContain("SQL OMITTED", text);
        context.DisableMutationSqlLog();
        context.RecordExecutionMetadata(batch);
        Assert.Equal(text, output.ToString());
    }
}
