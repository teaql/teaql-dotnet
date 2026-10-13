using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

[Collection("Runtime telemetry SDK")]
public class RuntimeSqlTelemetryTests
{
    [Fact]
    public void SqlDiagnosticUsesExistingMaskAndOmitsIdentifiersEvenWithPlaintextDebugEnabled()
    {
        var previous = Environment.GetEnvironmentVariable(LogPrivacy.EnvironmentName);
        Environment.SetEnvironmentVariable(LogPrivacy.EnvironmentName, LogPrivacy.Acknowledgement);
        try {
            var telemetry = new DiagnosticProbe();
            var context = new UserContext().WithRuntimeTelemetry(telemetry).DisableQuerySqlLog().DisableMutationSqlLog();
            var node = new TraceNode("School", 9988776655, "load PRIVATE-VALUE") { Kind = "comment", Detail = "load PRIVATE-VALUE" };
            var source = new ExecutionMetadata { Backend = "sqlite", Operation = DataServiceOperation.Query,
                SqlCorrelation = new("11111111111111111111111111111111", "2222222222222222"),
                ParameterizedQuery = "SELECT name FROM school_data WHERE name = ?", Parameters = new[] { new Value.TextValue("PRIVATE-VALUE") },
                ParameterLogPolicies = new[] { SqlParameterLogPolicy.Masked }, GeneratedSql = true,
                Comment = node.Comment, Purpose = node.Comment, TraceChain = new() { node }, RequestTraceChain = new[] { node }, MutationLineage = new[] { node } };
            context.RecordExecutionMetadata(source);
            var diagnostic = Assert.Single(telemetry.Events);
            var output = JsonSerializer.Serialize(diagnostic);
            Assert.DoesNotContain("PRIVATE-VALUE", output); Assert.DoesNotContain("9988776655", output); Assert.DoesNotContain("EntityId", output);
            Assert.Equal(source.SqlCorrelation, diagnostic.Correlation); Assert.Equal("masked", diagnostic.Attributes["teaql.sql.log_mode"]);
            Assert.Equal("PRIVATE-VALUE", ((Value.TextValue)source.Parameters[0]).Value);
            Assert.Equal("load PRIVATE-VALUE", source.TraceChain[0].Comment);
        }
        finally { Environment.SetEnvironmentVariable(LogPrivacy.EnvironmentName, previous); }
    }
    [Fact]
    public void ThrowingDiagnosticDoesNotAffectBusinessOutcome()
    {
        var context = new UserContext().WithRuntimeTelemetry(new DiagnosticProbe { Throw = true });
        context.RecordExecutionMetadata(new ExecutionMetadata { SqlCorrelation = new("11111111111111111111111111111111", "2222222222222222") });
    }
    private sealed class DiagnosticProbe : IRuntimeTelemetry, IRuntimeSqlTelemetry {
        internal bool Throw;
        internal List<RuntimeDiagnosticEvent> Events = new();
        public IRuntimeTelemetryScope Start(RuntimeOperation operation) => NoopRuntimeTelemetry.Instance.Start(operation);
        public ISqlExecutionObservation StartStatement(string backend, DataServiceOperation operation) => throw new NotSupportedException();
        public void RecordDiagnostic(RuntimeDiagnosticEvent diagnostic) { if (Throw) throw new Exception("PRIVATE-SINK-FAILURE"); Events.Add(diagnostic); }
    }
}
