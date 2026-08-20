using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.TfpEndpoint;
using Xunit;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace TeaQL.Runtime.Tests;

public class TfpEndpointTelemetryTests
{
    [Fact]
    public async Task RecordsServerQueryMutationAndOriginalFailure()
    {
        var telemetry = new RecordingTelemetry();
        var dataService = new StubDataService();
        var handler = new TfpEndpointHandler(dataService, telemetry);

        var query = await handler.HandleQueryAsync(Trusted(), QueryPayload());
        await handler.HandleMutationAsync(Trusted(),
            "{\"entity\":\"Probe\",\"action\":\"Create\",\"payload\":{},\"comment\":\"create probe\"}");

        Assert.Empty((List<Dictionary<string, object?>>)query["data"]);
        Assert.Collection(telemetry.Events,
            item =>
            {
                Assert.Equal("tfp", item.Operation.Family);
                Assert.Equal("server.query", item.Operation.Name);
                Assert.Equal("server", item.Operation.Attributes["teaql.tfp.role"]);
                Assert.Equal(0, item.Completion!["teaql.result.cardinality"]);
                Assert.Null(item.Error);
            },
            item =>
            {
                Assert.Equal("server.mutation", item.Operation.Name);
                Assert.Null(item.Error);
            });

        var original = new InvalidOperationException("provider failed");
        dataService.Error = original;
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleQueryAsync(Trusted(), QueryPayload()));
        Assert.Same(original, thrown);
        Assert.Same(original, telemetry.Events[^1].Error);
    }

    [Fact]
    public async Task ExtractsCaseInsensitiveW3cCarrierAsDirectServerParent()
    {
        var spans = new List<Activity>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource("io.teaql.runtime").AddInMemoryExporter(spans).Build();
        using var telemetry = new OpenTelemetryRuntimeTelemetry();
        var handler = new TfpEndpointHandler(new StubDataService(), telemetry);
        const string traceId = "0af7651916cd43dd8448eb211c80319c";
        const string parentSpanId = "b7ad6b7169203331";

        await handler.HandleQueryAsync(Trusted(), QueryPayload(),
            new Dictionary<string, string>
            {
                ["TraceParent"] = $"00-{traceId}-{parentSpanId}-01"
            });
        tracerProvider.ForceFlush();

        var server = Assert.Single(spans.Where(span => span.OperationName == "teaql.tfp"));
        Assert.Equal(ActivityTraceId.CreateFromString(traceId), server.TraceId);
        Assert.Equal(ActivitySpanId.CreateFromString(parentSpanId), server.ParentSpanId);
    }

    [Fact]
    public async Task FailsClosedForMissingPolicyForbiddenFilterAndMutation()
    {
        var handler = new TfpEndpointHandler(new StubDataService());
        var unauthorized = await Assert.ThrowsAsync<TfpEndpointException>(() =>
            handler.HandleQueryAsync(QueryPayload()));
        Assert.Equal("TFP_UNAUTHORIZED", unauthorized.Code);

        var forbidden = await Assert.ThrowsAsync<TfpEndpointException>(() =>
            handler.HandleQueryAsync(Trusted(),
                "{\"entity\":\"Probe\",\"filterCondition\":{\"secret\":{\"$eq\":1}},\"commentText\":\"x\",\"purposeText\":\"x\"}"));
        Assert.Equal("TFP_FORBIDDEN_FIELD", forbidden.Code);

        var audit = await Assert.ThrowsAsync<TfpEndpointException>(() =>
            handler.HandleMutationAsync(Trusted(),
                "{\"entity\":\"Probe\",\"action\":\"Create\",\"payload\":{},\"comment\":\" \"}"));
        Assert.Equal("TFP_AUDIT_REASON_REQUIRED", audit.Code);
    }

    private static string QueryPayload() =>
        "{\"entity\":\"Probe\",\"limitValue\":10,\"commentText\":\"test query\",\"purposeText\":\"test\"}";

    private static TrustedFederalContext Trusted() => new()
    {
        TenantField = "tenant_id",
        TenantId = new Value.I64Value(7),
        AuthenticatedUser = "tester",
        ApprovedPurpose = "tests",
        AllowedEntities = new HashSet<string> { "Probe" },
        ReadableFields = new Dictionary<string, IReadOnlyDictionary<string, string>> {
            ["Probe"] = new Dictionary<string, string> { ["id"] = "id" }
        },
        WritableFields = new Dictionary<string, IReadOnlyDictionary<string, string>> {
            ["Probe"] = new Dictionary<string, string>()
        },
        AllowedActions = new Dictionary<string, ISet<string>> {
            ["Probe"] = new HashSet<string> { "Create" }
        },
        MaxPageSize = 100
    };

    private sealed class StubDataService : IDataService
    {
        public Exception? Error { get; set; }
        public DataServiceCapabilities Capabilities { get; } = new() { Query = true, Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => Error is null
            ? Task.FromResult(new QueryResult())
            : Task.FromException<QueryResult>(Error);
        public Task<MutationResult> MutateAsync(MutationRequest request) => Error is null
            ? Task.FromResult(new MutationResult { AffectedRows = 1 })
            : Task.FromException<MutationResult>(Error);
    }

    private sealed class RecordingTelemetry : IRuntimeTelemetry
    {
        public List<Event> Events { get; } = [];
        public IRuntimeTelemetryScope Start(RuntimeOperation operation)
        {
            var item = new Event(operation);
            Events.Add(item);
            return new Scope(item);
        }
        private sealed class Scope(Event item) : IRuntimeTelemetryScope
        {
            public void Success(IReadOnlyDictionary<string, object>? attributes = null) =>
                item.Completion = attributes ?? new Dictionary<string, object>();
            public void Failure(Exception error) => item.Error = error;
        }
    }

    private sealed class Event(RuntimeOperation operation)
    {
        public RuntimeOperation Operation { get; } = operation;
        public IReadOnlyDictionary<string, object>? Completion { get; set; }
        public Exception? Error { get; set; }
    }
}
