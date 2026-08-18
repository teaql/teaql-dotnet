using Microsoft.Extensions.Logging;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class RuntimeTelemetryOtlpSmokeTests
{
    [Fact]
    public void ExportsQueryTraceMetricAndLogThroughOtlpHttp()
    {
        var serviceName = Environment.GetEnvironmentVariable("TEAQL_OTLP_SERVICE_NAME");
        if (string.IsNullOrWhiteSpace(serviceName)) return;

        var endpoint = new Uri(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
            ?? "http://localhost:4318");
        var runId = serviceName[(serviceName.LastIndexOf('-') + 1)..];
        var resource = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceInstanceId: runId)
            .AddAttributes(new Dictionary<string, object>
            {
                ["teaql.runtime.language"] = "dotnet",
                ["teaql.conformance.run_id"] = runId
            });

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resource)
            .AddSource("io.teaql.runtime")
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(endpoint, "/v1/traces");
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                options.ExportProcessorType = ExportProcessorType.Batch;
                options.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
                {
                    MaxQueueSize = 64,
                    MaxExportBatchSize = 16
                };
            }).Build();
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(resource)
            .AddMeter("io.teaql.runtime")
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(endpoint, "/v1/metrics");
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                options.ExportProcessorType = ExportProcessorType.Simple;
            }).Build();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resource);
            options.AddOtlpExporter(exporter =>
            {
                exporter.Endpoint = new Uri(endpoint, "/v1/logs");
                exporter.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                exporter.ExportProcessorType = ExportProcessorType.Batch;
                exporter.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
                {
                    MaxQueueSize = 64,
                    MaxExportBatchSize = 16
                };
            });
        }));
        using var telemetry = new OpenTelemetryRuntimeTelemetry(
            logger: loggerFactory.CreateLogger("TeaQL.Runtime"));

        Complete(telemetry, "query", "ConformanceProbe.list",
            new() { ["teaql.entity.type"] = "ConformanceProbe" });
        Complete(telemetry, "mutation", "ConformanceProbe.update",
            new() { ["teaql.entity.type"] = "ConformanceProbe", ["teaql.mutation.kind"] = "update" });
        Complete(telemetry, "relation_load", "ConformanceProbe.children",
            new() { ["teaql.entity.type"] = "ConformanceProbe", ["teaql.relation.name"] = "children" });
        Complete(telemetry, "provider", "sqlite.query",
            new() { ["teaql.provider.kind"] = "sqlite", ["teaql.provider.operation"] = "query" });
        Complete(telemetry, "cache", "local.get",
            new() { ["teaql.cache.operation"] = "get" });
        Complete(telemetry, "tfp", "server.query",
            new() { ["teaql.tfp.role"] = "server" });
        Complete(telemetry, "audit", "ConformanceProbe.audit", new()
        {
            ["teaql.entity.type"] = "ConformanceProbe", ["teaql.mutation.kind"] = "update",
            ["teaql.audit.changed_field_count"] = 1
        });

        Assert.True(tracerProvider.ForceFlush());
        Assert.True(meterProvider.ForceFlush());
        loggerFactory.Dispose();
    }

    private static void Complete(OpenTelemetryRuntimeTelemetry telemetry, string family,
        string name, Dictionary<string, object> attributes)
    {
        attributes["teaql.entity.id"] = "must-not-export";
        var scope = telemetry.StartSafely(RuntimeOperation.Create(family, name, attributes));
        var completion = new Dictionary<string, object> { ["teaql.result.cardinality"] = 1 };
        if (family == "cache") completion["teaql.cache.result"] = "hit";
        scope.Success(completion);
    }
}
