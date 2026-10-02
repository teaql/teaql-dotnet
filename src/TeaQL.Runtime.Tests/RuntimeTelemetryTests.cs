using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Logging;
using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class RuntimeTelemetryTests
{
    [Fact]
    public void ClassifiesNativeErrorTypesWithoutInspectingMessages()
    {
        Assert.Equal("timeout", RuntimeErrorClassifier.Category("DatabaseTimeoutException"));
        Assert.Equal("authorization", RuntimeErrorClassifier.Category("PermissionException"));
        Assert.Equal("internal", RuntimeErrorClassifier.Category("UnknownTeaQLError"));
    }

    [Fact]
    public async Task LifecycleIsSafeBalancedAndFailOpen()
    {
        var events = new List<string>();
        var telemetry = new RecordingTelemetry(events);
        var result = await telemetry.ObserveAsync(
            RuntimeOperation.Create("query", "School.list", new Dictionary<string, object>
            {
                ["teaql.entity.type"] = "School",
                ["teaql.entity.id"] = 42L
            }),
            () => Task.FromResult(new[] { "school" }),
            rows => new Dictionary<string, object> { ["teaql.result.cardinality"] = rows.Length });
        Assert.Single(result);
        Assert.Equal(new[] { "start", "success" }, events);
        Assert.DoesNotContain("teaql.entity.id", telemetry.Operation!.Attributes.Keys);

        var broken = new BrokenTelemetry();
        Assert.Equal(42, await broken.ObserveAsync(
            RuntimeOperation.Create("cache", "get"), () => Task.FromResult(42)));
    }

    [Fact]
    public async Task DelegatesExplicitApplicationOwnedLifecycle()
    {
        var calls = new List<string>();
        using var telemetry = new OpenTelemetryRuntimeTelemetry(
            flush: () => { calls.Add("flush"); return Task.CompletedTask; },
            shutdown: () => { calls.Add("shutdown"); return Task.CompletedTask; });

        await telemetry.FlushAsync();
        await telemetry.ShutdownAsync();

        Assert.Equal(new[] { "flush", "shutdown" }, calls);
    }

    [Fact]
    public void OfficialSdkExportsNestedSpansAndMetrics()
    {
        var spans = new List<Activity>();
        var metrics = new List<Metric>();
        var logs = new List<LogRecord>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource("io.teaql.runtime")
            .AddInMemoryExporter(spans)
            .Build();
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("io.teaql.runtime")
            .AddInMemoryExporter(metrics)
            .Build();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(
            options => options.AddInMemoryExporter(logs)));
        using var telemetry = new OpenTelemetryRuntimeTelemetry(
            logger: loggerFactory.CreateLogger("TeaQL.Runtime"));
        var query = telemetry.StartSafely(RuntimeOperation.Create("query", "School.list",
            new Dictionary<string, object> { ["teaql.entity.type"] = "School" }));
        var provider = telemetry.StartSafely(RuntimeOperation.Create("provider", "sqlite.query"));
        provider.Success();
        query.Success(new Dictionary<string, object> { ["teaql.result.cardinality"] = 1 });
        tracerProvider.ForceFlush();
        meterProvider.ForceFlush();

        var querySpan = spans.Single(span => span.OperationName == "teaql.query");
        var providerSpan = spans.Single(span => span.OperationName == "teaql.provider");
        Assert.Equal(querySpan.SpanId, providerSpan.ParentSpanId);
        Assert.Contains(metrics, metric => metric.Name == "teaql.runtime.operation.duration");
        Assert.Contains(metrics, metric => metric.Name == "teaql.runtime.operation.count");
        Assert.Equal(2, logs.Count);
        var queryLog = logs.Single(log => log.Attributes?.Any(attribute =>
            attribute.Key == "teaql.operation.family" && Equals(attribute.Value, "query")) == true);
        Assert.Equal(querySpan.TraceId, queryLog.TraceId);
        Assert.Equal(querySpan.SpanId, queryLog.SpanId);
        Assert.DoesNotContain(queryLog.Attributes ?? [], attribute =>
            attribute.Key == "teaql.entity.id");
    }

    [Fact]
    public void OfficialSdkFailureDoesNotExportDriverErrorMessages()
    {
        var spans = new List<Activity>();
        var logs = new List<LogRecord>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource("io.teaql.runtime")
            .AddInMemoryExporter(spans)
            .Build();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(
            options => options.AddInMemoryExporter(logs)));
        using var telemetry = new OpenTelemetryRuntimeTelemetry(
            logger: loggerFactory.CreateLogger("TeaQL.Runtime"));

        var scope = telemetry.StartSafely(RuntimeOperation.Create("provider", "sqlite.query"));
        scope.Failure(new InvalidOperationException(
            "SQL failed for password=OTEL-FAILURE-CANARY"));
        tracerProvider.ForceFlush();

        var span = Assert.Single(spans);
        var log = Assert.Single(logs);
        Assert.Contains(span.Tags, tag =>
            tag.Key == "teaql.error.type" && tag.Value == "InvalidOperationException");
        Assert.Contains(log.Attributes ?? [], attribute =>
            attribute.Key == "teaql.operation.outcome" && Equals(attribute.Value, "failure"));
        Assert.DoesNotContain("OTEL-FAILURE-CANARY", string.Join(" ",
            span.TagObjects.Select(tag => $"{tag.Key}={tag.Value}")));
        Assert.DoesNotContain("OTEL-FAILURE-CANARY", span.StatusDescription ?? "");
        Assert.DoesNotContain("OTEL-FAILURE-CANARY", string.Join(" ", span.Events));
        Assert.DoesNotContain("OTEL-FAILURE-CANARY", log.FormattedMessage ?? "");
        Assert.DoesNotContain("OTEL-FAILURE-CANARY", string.Join(" ",
            (log.Attributes ?? []).Select(attribute => $"{attribute.Key}={attribute.Value}")));
    }

    [Fact]
    public async Task RuntimeDataServiceProducesQueryAndNestedProviderSpans()
    {
        var spans = new List<Activity>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource("io.teaql.runtime")
            .AddInMemoryExporter(spans)
            .Build();
        using var telemetry = new OpenTelemetryRuntimeTelemetry();
        var context = new UserContext()
            .WithDataService(new StubDataService())
            .WithRuntimeTelemetry(telemetry);

        var result = await context.RequireResource<IDataService>().QueryAsync(
            new QueryRequest(new SelectQuery("School"), new QueryIntent("load school rows", "verify query and provider spans")));
        tracerProvider.ForceFlush();

        Assert.Single(result.Rows);
        var query = spans.Single(span => span.OperationName == "teaql.query");
        var provider = spans.Single(span => span.OperationName == "teaql.provider");
        Assert.Equal(query.SpanId, provider.ParentSpanId);
    }

    [Fact]
    public async Task DiagnosticSqlLogDefaultsOnAndSupportsIndependentSwitches()
    {
        var output = new StringWriter();
        var context = new UserContext().WithDataService(new StubDataService());
        Assert.True(context.QuerySqlLogEnabled);
        Assert.True(context.MutationSqlLogEnabled);

        context.WithDiagnosticSqlLogSink(new TextDiagnosticSqlLogSink(output));
        await context.RequireResource<IDataService>().QueryAsync(
            new QueryRequest(new SelectQuery("School"), new QueryIntent("load school rows", "verify safe SQL diagnostics")));
        Assert.DoesNotContain("Parameterized SQL:", output.ToString());
        Assert.Contains("SQL: -- TeaQL SAFE", output.ToString());
        Assert.Contains("name = '[REDACTED]' /* masked */", output.ToString());
        Assert.DoesNotContain("O''Brien", output.ToString());
        Assert.DoesNotContain("Debug SQL:", output.ToString());

        var sensitive = new StringWriter();
        context.WithSensitiveDiagnosticSqlLogSink(new SensitiveDiagnosticSqlLogSink(sensitive));
        await context.RequireResource<IDataService>().QueryAsync(
            new QueryRequest(new SelectQuery("School"), new QueryIntent("load school rows", "verify sensitive sink remains masked")));
        Assert.DoesNotContain("O''Brien", sensitive.ToString());
        Assert.DoesNotContain("O'Brien", sensitive.ToString());
        Assert.Contains("Debug SQL:", sensitive.ToString());

        var before = output.ToString();
        context.DisableQuerySqlLog();
        await context.RequireResource<IDataService>().QueryAsync(
            new QueryRequest(new SelectQuery("School"), new QueryIntent("load school rows", "verify logging switch does not bypass query execution")));
        Assert.Equal(before, output.ToString());
        Assert.True(context.MutationSqlLogEnabled);
        context.EnableQuerySqlLog().DisableMutationSqlLog();
        Assert.True(context.QuerySqlLogEnabled);
        Assert.False(context.MutationSqlLogEnabled);
    }

    [Fact]
    public async Task RuntimeDataServiceCarriesObserverIntoActualRelationLoad()
    {
        var spans = new List<Activity>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource("io.teaql.runtime").AddInMemoryExporter(spans).Build();
        using var telemetry = new OpenTelemetryRuntimeTelemetry();
        var context = new UserContext()
            .WithDataService(new StubDataService(observeRelation: true))
            .WithRuntimeTelemetry(telemetry);
        var queryRequest = new QueryRequest(new SelectQuery("School"),
            new QueryIntent("load school and students", "verify relation observer propagation"));
        queryRequest.Query.Relation("students");

        await context.RequireResource<IDataService>().QueryAsync(queryRequest);
        tracerProvider.ForceFlush();

        var provider = spans.Single(span => span.OperationName == "teaql.provider");
        var relation = spans.Single(span => span.OperationName == "teaql.relation_load");
        Assert.Equal(provider.SpanId, relation.ParentSpanId);
        Assert.Contains(relation.Tags, tag =>
            tag.Key == "teaql.relation.name" && Equals(tag.Value, "students"));
    }

    [Fact]
    public async Task BrokenSqlDiagnosticSinkDoesNotFailSuccessfulQueryOrSkipOtherSink()
    {
        var ordinary = new CountingDiagnosticSink();
        var context = new UserContext()
            .WithDataService(new StubDataService())
            .WithSensitiveDiagnosticSqlLogSink(new ThrowingSensitiveDiagnosticSink())
            .WithDiagnosticSqlLogSink(ordinary);

        var result = await context.RequireResource<IDataService>().QueryAsync(
            new QueryRequest(new SelectQuery("School"), new QueryIntent("load school rows", "verify independent SQL diagnostic sinks")));

        Assert.Single(result.Rows);
        Assert.Equal(1, ordinary.Count);

        context.WithDiagnosticSqlLogSink(new ThrowingDiagnosticSink());
        result = await context.RequireResource<IDataService>().QueryAsync(
            new QueryRequest(new SelectQuery("School"), new QueryIntent("load school rows", "verify failed SQL diagnostic sink is fail open")));
        Assert.Single(result.Rows);
    }

    private sealed class CountingDiagnosticSink : IDiagnosticSqlLogSink
    {
        public int Count { get; private set; }
        public void Write(ExecutionMetadata metadata) => Count++;
    }

    private sealed class ThrowingDiagnosticSink : IDiagnosticSqlLogSink
    {
        public void Write(ExecutionMetadata metadata) =>
            throw new InvalidOperationException("DIAGNOSTIC-SINK-FAILURE");
    }

    private sealed class ThrowingSensitiveDiagnosticSink : ISensitiveDiagnosticSqlLogSink
    {
        public void Write(ExecutionMetadata metadata) =>
            throw new InvalidOperationException("SENSITIVE-SINK-FAILURE");
    }

    private sealed class RecordingTelemetry(List<string> events) : IRuntimeTelemetry
    {
        public RuntimeOperation? Operation { get; private set; }
        public IRuntimeTelemetryScope Start(RuntimeOperation operation)
        {
            Operation = operation; events.Add("start");
            return new Scope(events);
        }
        private sealed class Scope(List<string> events) : IRuntimeTelemetryScope
        {
            public void Success(IReadOnlyDictionary<string, object>? attributes = null) => events.Add("success");
            public void Failure(Exception error) => events.Add("failure");
        }
    }

    private sealed class BrokenTelemetry : IRuntimeTelemetry
    {
        public IRuntimeTelemetryScope Start(RuntimeOperation operation) => throw new InvalidOperationException();
    }

    private sealed class StubDataService(bool observeRelation = false) : IDataService
    {
        public DataServiceCapabilities Capabilities { get; } = new() { Query = true, Mutation = true };
        public async Task<QueryResult> QueryAsync(QueryRequest request)
        {
            if (observeRelation && request.Query.RelationLoads.Count > 0)
                await request.RelationLoadObserver!.ObserveAsync(
                    request.Query.Entity, request.Query.RelationLoads[0].Name,
                    new Dictionary<string, object>(),
                    () => Task.CompletedTask);
            return new QueryResult {
                Rows = new List<TeaQL.Core.Record> { new() },
                Metadata = new ExecutionMetadata {
                    Operation = DataServiceOperation.Query,
                    StartedAt = DateTimeOffset.UnixEpoch,
                    EndedAt = DateTimeOffset.UnixEpoch.AddMilliseconds(1),
                    ResultCount = 1,
                    ParameterizedQuery = "SELECT * FROM school_data WHERE name = ?",
                    Parameters = new Value[] { new Value.TextValue("O'Brien 学校") },
                    DebugQuery = "SELECT * FROM school_data WHERE name = 'O''Brien 学校'"
                }
            };
        }
        public Task<MutationResult> MutateAsync(MutationRequest request) =>
            Task.FromResult(new MutationResult { AffectedRows = 1 });
    }
}
