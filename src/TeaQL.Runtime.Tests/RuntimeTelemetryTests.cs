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
            new QueryRequest { Query = new SelectQuery("School") });
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
            new QueryRequest { Query = new SelectQuery("School") });
        Assert.Contains("SELECT * FROM school_data WHERE name = 'O''Brien 学校'", output.ToString());
        Assert.Contains("Parameterized SQL:", output.ToString());
        Assert.Contains("Debug SQL:", output.ToString());

        var before = output.ToString();
        context.DisableQuerySqlLog();
        await context.RequireResource<IDataService>().QueryAsync(
            new QueryRequest { Query = new SelectQuery("School") });
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
        var queryRequest = new QueryRequest { Query = new SelectQuery("School") };
        queryRequest.Query.Relation("students");

        await context.RequireResource<IDataService>().QueryAsync(queryRequest);
        tracerProvider.ForceFlush();

        var provider = spans.Single(span => span.OperationName == "teaql.provider");
        var relation = spans.Single(span => span.OperationName == "teaql.relation_load");
        Assert.Equal(provider.SpanId, relation.ParentSpanId);
        Assert.Contains(relation.Tags, tag =>
            tag.Key == "teaql.relation.name" && Equals(tag.Value, "students"));
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
                    DebugQuery = "SELECT * FROM school_data WHERE name = 'O''Brien 学校'"
                }
            };
        }
        public Task<MutationResult> MutateAsync(MutationRequest request) =>
            Task.FromResult(new MutationResult { AffectedRows = 1 });
    }
}
