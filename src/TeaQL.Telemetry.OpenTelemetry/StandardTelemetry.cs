using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TeaQL.DataService;
using TeaQL.Runtime;

namespace TeaQL.Telemetry.OpenTelemetry;

public sealed class StandardTelemetryFactory : IStandardTelemetryFactory
{
    public IRuntimeTelemetry Create(TelemetryStartupConfiguration configuration) => new StandardTelemetry(configuration);
}

public sealed record TelemetryExportHealth(long FailedTraceSpans, long FailedLogRecords, long FailedMetricObjects, long DroppedTraceSpans, long DroppedLogRecords);

public sealed class StandardTelemetry : IRuntimeTelemetry, IRuntimeSqlTelemetry, IStandardTelemetryLifecycle
{
    private const string ScopeName = "io.teaql.runtime";
    private const string OwnerKey = "teaql.internal.telemetry.owner";
    private readonly object _owner = new();
    private readonly ActivitySource _source = new(ScopeName);
    private readonly Meter _meter = new(ScopeName);
    private readonly Histogram<double> _duration;
    private readonly Counter<long> _operations;
    private readonly TracerProvider _traces;
    private readonly MeterProvider _metrics;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private readonly BatchActivityExportProcessor _traceBatch;
    private readonly BatchLogRecordExportProcessor _logBatch;
    private readonly OwnedLogProcessor _logProcessor;
    private readonly ExportHealthState _health = new();
    private readonly Admission _traceAdmission, _logAdmission;
    private readonly object _lifecycleGate = new(), _completionGate = new();
    private readonly HashSet<Task> _flushes = new();
    private readonly System.Collections.Concurrent.ConcurrentBag<OwnedHttpTransport> _transports = new();
    private Task? _shutdownTask, _metricFlushTask;
    private int _closed;
    private readonly AsyncLocal<ActivityContext?> _remoteParent = new();
    public bool IsShutdown => Volatile.Read(ref _closed) != 0;

    public StandardTelemetry(TelemetryStartupConfiguration config)
        : this(config, null, null, null) { }

    internal StandardTelemetry(TelemetryStartupConfiguration config, BaseExporter<Activity>? traceExporter,
        BaseExporter<LogRecord>? logExporter, BaseExporter<Metric>? metricExporter, Func<string, OtlpExporterOptions>? optionsFactory = null)
    {
        var valid = TelemetryStartupConfiguration.Read(new Dictionary<string, string?> {
            [TelemetryStartupConfiguration.EnabledVariable] = "true", [TelemetryStartupConfiguration.ServiceVariable] = config.ServiceName,
            [TelemetryStartupConfiguration.EndpointVariable] = config.Endpoint
        });
        if (!valid.Enabled) throw new ArgumentException("invalid-telemetry-configuration");
        _traceAdmission = new("traces", _health); _logAdmission = new("logs", _health);
        OtlpExporterOptions Options(string signal) {
            var options = optionsFactory?.Invoke(signal) ?? new OtlpExporterOptions {
                Endpoint = new Uri(valid.Endpoint + "/v1/" + signal)
            };
            var factory = optionsFactory == null ? OwnedHttpTransport.PrivateClient : options.HttpClientFactory;
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
            options.Headers = "";
            options.TimeoutMilliseconds = Math.Min(3000, Math.Max(1, options.TimeoutMilliseconds));
            options.HttpClientFactory = () => {
                var transport = new OwnedHttpTransport(factory(), options.TimeoutMilliseconds, signal, _health);
                _transports.Add(transport);
                return new HttpClient(transport) { Timeout = TimeSpan.FromMilliseconds(options.TimeoutMilliseconds) };
            };
            return options;
        }
        var resource = ResourceBuilder.CreateEmpty().AddService(valid.ServiceName).AddAttributes(
            new[] { new KeyValuePair<string, object>("teaql.runtime.language", "dotnet") });
        traceExporter ??= new OtlpTraceExporter(Options("traces"));
        logExporter ??= new OtlpLogExporter(Options("logs"));
        _traceBatch = new(new SafeExporter<Activity>(traceExporter, _health, "traces", _traceAdmission),
            maxQueueSize: 2048, scheduledDelayMilliseconds: 1000, exporterTimeoutMilliseconds: 5000, maxExportBatchSize: 128);
        // The platform's ActivitySource registry is shared. Sample/export only
        // this instance's operations, without modifying another SDK's listeners.
        _traces = Sdk.CreateTracerProviderBuilder().SetResourceBuilder(resource).AddSource(ScopeName)
            .SetSampler(new OwnedAlwaysOnSampler(_owner)).AddProcessor(new ExporterRegistration<Activity>(traceExporter))
            .AddProcessor(new OwnedTraceProcessor(_traceBatch, _owner, _traceAdmission)).Build();
        _duration = _meter.CreateHistogram<double>("teaql.runtime.operation.duration", "ms");
        _operations = _meter.CreateCounter<long>("teaql.runtime.operation.count", "{operation}");
        _meter.CreateObservableCounter("teaql.telemetry.export.lost", _health.LossMeasurements);
        _meter.CreateObservableCounter("teaql.telemetry.export.rejected", _health.RejectionMeasurements);
        _meter.CreateObservableCounter("teaql.telemetry.export.response", _health.ResponseMeasurements);
        _metrics = Sdk.CreateMeterProviderBuilder().SetResourceBuilder(resource).AddMeter(ScopeName)
            .AddView(instrument => ReferenceEquals(instrument.Meter, _meter) ? null : MetricStreamConfiguration.Drop)
            .AddReader(new PeriodicExportingMetricReader(metricExporter == null ? new HealthAwareOtlpMetricExporter(Options("metrics"), _health)
                : new SafeExporter<Metric>(metricExporter, _health, "metrics"), 10000, 5000)).Build();
        _logBatch = new(new SafeExporter<LogRecord>(logExporter, _health, "logs", _logAdmission),
            maxQueueSize: 2048, scheduledDelayMilliseconds: 1000, exporterTimeoutMilliseconds: 5000, maxExportBatchSize: 128);
        _logProcessor = new(_logBatch, _logAdmission);
        _loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(options => {
            options.SetResourceBuilder(resource);
            options.IncludeScopes = false;
            options.IncludeFormattedMessage = true;
            options.AddProcessor(new ExporterRegistration<LogRecord>(logExporter));
            options.AddProcessor(new CarriedLogCorrelationProcessor());
            options.AddProcessor(_logProcessor);
        }));
        _logger = _loggerFactory.CreateLogger(ScopeName);
    }

    public IRuntimeTelemetryScope Start(RuntimeOperation operation)
    {
        lock (_completionGate) {
            if (IsShutdown) return new LateScope(this);
            var tags = new ActivityTagsCollection(operation.Attributes.Select(pair => new KeyValuePair<string, object?>(pair.Key, pair.Value))) { [OwnerKey] = _owner };
            var activity = _source.StartActivity("teaql." + operation.Family, ActivityKind.Internal, Activity.Current?.Context ?? _remoteParent.Value ?? default, tags);
            activity?.SetCustomProperty(OwnerKey, _owner);
            activity?.SetTag(OwnerKey, null); // internal ownership is never an exported attribute
            return new OperationScope(this, activity, operation);
        }
    }
    public IDisposable ExtractAndActivate(IReadOnlyDictionary<string, string> carrier)
    {
        var previous = _remoteParent.Value;
        var traceparent = carrier.FirstOrDefault(pair => pair.Key.Equals("traceparent", StringComparison.OrdinalIgnoreCase)).Value;
        var tracestate = carrier.FirstOrDefault(pair => pair.Key.Equals("tracestate", StringComparison.OrdinalIgnoreCase)).Value;
        if (ActivityContext.TryParse(traceparent, tracestate, isRemote: true, out var parsed)) _remoteParent.Value = parsed;
        return new RestoreRemoteParent(_remoteParent, previous);
    }
    private sealed class RestoreRemoteParent(AsyncLocal<ActivityContext?> storage, ActivityContext? previous) : IDisposable
    {
        private int _closed;
        public void Dispose() { if (Interlocked.Exchange(ref _closed, 1) == 0) storage.Value = previous; }
    }
    public ISqlExecutionObservation StartStatement(string backend, DataServiceOperation operation)
    {
        return (ISqlExecutionObservation)Start(RuntimeOperation.Create("provider", backend + ".statement", new Dictionary<string, object> {
            ["teaql.provider.kind"] = backend, ["teaql.provider.operation"] = operation.ToString().ToLowerInvariant(), ["teaql.provider.execution"] = "statement"
        }));
    }
    public void RecordDiagnostic(RuntimeDiagnosticEvent diagnostic)
    {
        lock (_completionGate) {
        if (IsShutdown) { _logAdmission.RecordLateOffer(); return; }
        if (!ValidCorrelation(diagnostic.Correlation)) {
            TelemetryStartup.Notice("TeaQL telemetry: SQL correlation unavailable; no replacement span created."); return;
        }
        Emit("TeaQL SQL execution", diagnostic.Attributes, diagnostic.Correlation);
        }
    }
    private static bool ValidCorrelation(SqlExecutionCorrelation correlation) =>
        correlation.TraceId.Length == 32 && correlation.SpanId.Length == 16
        && correlation.TraceId.All(Uri.IsHexDigit) && correlation.SpanId.All(Uri.IsHexDigit)
        && correlation.TraceId.Any(c => c != '0') && correlation.SpanId.Any(c => c != '0');

    private void Emit(string body, IReadOnlyDictionary<string, object> attributes, SqlExecutionCorrelation? correlation)
    {
        if (IsShutdown) { _logAdmission.RecordLateOffer(); return; }
        var values = attributes.Select(pair => new KeyValuePair<string, object?>(pair.Key, pair.Value)).ToList();
        values.Add(new("{OriginalFormat}", body));
        if (correlation != null) { values.Add(new(CarriedLogCorrelationProcessor.TraceKey, correlation.TraceId)); values.Add(new(CarriedLogCorrelationProcessor.SpanKey, correlation.SpanId)); }
        // The private processor admits the SDK's log record, before its native
        // snapshot/queue handoff. Thread-static logger records are never keys.
        values.Add(new(SafeExporter<LogRecord>.LogTicketKey, Guid.NewGuid().ToString("N")));
        _logger.Log(LogLevel.Information, new EventId(0), values, null, (_, _) => body);
    }
    public TelemetryExportHealth ExportHealth => new(_health.Traces, _health.Logs, _health.Metrics, _health.TraceDrops, _health.LogDrops);
    public IReadOnlyDictionary<string, long> RejectedRecords => _health.Rejected;
    public IReadOnlyDictionary<string, long> ExportResponses => _health.Responses;
    public IReadOnlyDictionary<string, long> QueueLosses => _health.QueueLosses;
    public IReadOnlyDictionary<string, int> ExportResources => new Dictionary<string, int> {
        ["ownedTransports"] = _transports.Count, ["closedTransports"] = _transports.Count(row => row.IsClosed),
        ["activeHTTP"] = _transports.Sum(row => row.Active)
    };
    public IReadOnlyDictionary<string, TelemetryQueueState> ExportQueues => new Dictionary<string, TelemetryQueueState> {
        ["traces"] = _traceAdmission.Snapshot, ["logs"] = _logAdmission.Snapshot
    };
    public Task FlushAsync() {
        lock (_lifecycleGate) {
            if (_shutdownTask != null) return _shutdownTask;
            var task = FlushCore(_traceAdmission.Prefix, _logAdmission.Prefix);
            _flushes.Add(task);
            _ = task.ContinueWith(completed => { lock (_lifecycleGate) _flushes.Remove(completed); },
                CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return task;
        }
    }
    public Task ShutdownAsync() {
        lock (_lifecycleGate) {
            if (_shutdownTask != null) return _shutdownTask;
            lock (_completionGate) {
                Interlocked.Exchange(ref _closed, 1); _traceAdmission.Close(); _logAdmission.Close();
            }
            return _shutdownTask = ShutdownCore(_traceAdmission.Prefix, _logAdmission.Prefix, _flushes.ToArray());
        }
    }
    private async Task FlushCore(long tracePrefix, long logPrefix) {
        var deadline = Stopwatch.StartNew();
        try {
            // ForceFlush(0) only wakes official workers; its dequeue counter is
            // not our completion oracle and does not create per-caller threads.
            _traceBatch.ForceFlush(0); _logBatch.ForceFlush(0);
            var results = await Task.WhenAll(_traceAdmission.Observe(tracePrefix, TimeSpan.FromSeconds(5)),
                _logAdmission.Observe(logPrefix, TimeSpan.FromSeconds(5))).ConfigureAwait(false);
            if (results.Any(result => !result)) { Incomplete(); return; }
            var remaining = TimeSpan.FromSeconds(5) - deadline.Elapsed;
            if (remaining <= TimeSpan.Zero) { Incomplete(); return; }
            Task metrics;
            lock (_lifecycleGate) {
                if (_metricFlushTask == null || _metricFlushTask.IsCompleted)
                    _metricFlushTask = Task.Run(() => _metrics.ForceFlush(Math.Max(1, (int)remaining.TotalMilliseconds)));
                metrics = _metricFlushTask;
            }
            await metrics.WaitAsync(remaining).ConfigureAwait(false);
        }
        catch { Incomplete(); }
    }
    private async Task ShutdownCore(long tracePrefix, long logPrefix, Task[] acceptedFlushes) {
        var elapsed = Stopwatch.StartNew();
        try {
            _traceBatch.ForceFlush(0); _logBatch.ForceFlush(0);
            var settled = await Task.WhenAll(_traceAdmission.Observe(tracePrefix, TimeSpan.FromSeconds(5)),
                _logAdmission.Observe(logPrefix, TimeSpan.FromSeconds(5))).ConfigureAwait(false);
            if (settled.Any(result => !result)) {
                _traceAdmission.RetireQueued(); _logAdmission.RetireQueued();
                TelemetryStartup.Notice("TeaQL telemetry: shutdown drain expired; queued records retired; business outcome unchanged.");
                foreach (var transport in _transports.Where(row => row.Signal != "metrics")) transport.CancelPending();
            }
            var joinBudget = TimeSpan.FromSeconds(5) - elapsed.Elapsed;
            if (acceptedFlushes.Length > 0 && joinBudget > TimeSpan.Zero)
                try { await Task.WhenAll(acceptedFlushes).WaitAsync(joinBudget).ConfigureAwait(false); } catch { Incomplete(); }
            var remaining = TimeSpan.FromSeconds(6) - elapsed.Elapsed;
            var timeout = Math.Max(1, (int)remaining.TotalMilliseconds);
            await Task.WhenAll(Task.Run(() => _traces.Shutdown(timeout)), Task.Run(() => _logProcessor.Shutdown(timeout)))
                .WaitAsync(remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero).ConfigureAwait(false);
            remaining = TimeSpan.FromMilliseconds(5900) - elapsed.Elapsed;
            foreach (var transport in _transports.Where(row => row.Signal == "metrics")) transport.Constrain(remaining);
            if (remaining > TimeSpan.Zero)
                await Task.Run(() => _metrics.Shutdown(Math.Max(1, (int)remaining.TotalMilliseconds))).WaitAsync(remaining).ConfigureAwait(false);
        }
        catch { Incomplete(); }
        finally {
            foreach (var transport in _transports) { transport.CancelPending(); transport.Dispose(); }
            _source.Dispose(); _meter.Dispose();
            var remaining = TimeSpan.FromSeconds(6) - elapsed.Elapsed;
            var cleanup = Task.Run(() => { _loggerFactory.Dispose(); _traces.Dispose(); _metrics.Dispose(); });
            if (remaining > TimeSpan.Zero)
                try { await cleanup.WaitAsync(remaining).ConfigureAwait(false); } catch { Incomplete(); }
        }
    }
    private static void Incomplete() => TelemetryStartup.Notice("TeaQL telemetry: lifecycle incomplete; business outcome unchanged.");
    private sealed class LateScope(StandardTelemetry pipeline) : IRuntimeTelemetryScope, ISqlExecutionObservation {
        private int _ended;
        public SqlExecutionCorrelation? Correlation => null;
        private void End(bool completion) {
            if (Interlocked.Exchange(ref _ended, 1) != 0) return;
            pipeline._traceAdmission.RecordLateOffer(); if (completion) pipeline._logAdmission.RecordLateOffer();
        }
        public void Success() => End(true); public void Success(IReadOnlyDictionary<string, object>? attributes) => End(true);
        public void Failure(Exception error) => End(true);
        public void Failure(Exception error, IReadOnlyDictionary<string, object>? attributes) => End(true);
        public void Dispose() => End(false);
    }

    private sealed class OperationScope : IRuntimeTelemetryScope, ISqlExecutionObservation
    {
        private readonly StandardTelemetry _pipeline;
        private readonly Activity? _activity;
        private readonly RuntimeOperation _operation;
        private readonly long _started = Stopwatch.GetTimestamp();
        private int _ended;
        public SqlExecutionCorrelation? Correlation { get; }
        internal OperationScope(StandardTelemetry pipeline, Activity? activity, RuntimeOperation operation) {
            _pipeline = pipeline; _activity = activity; _operation = operation;
            if (activity != null) Correlation = new(activity.TraceId.ToHexString(), activity.SpanId.ToHexString());
        }
        public void Success() => Success(null);
        public void Success(IReadOnlyDictionary<string, object>? attributes) => End("success", null, attributes);
        public void Failure(Exception error) => End(error is OperationCanceledException ? "cancelled" : "failure", error, null);
        public void Failure(Exception error, IReadOnlyDictionary<string, object>? attributes) =>
            End(error is OperationCanceledException ? "cancelled" : "failure", error, attributes);
        public void Dispose() { if (Volatile.Read(ref _ended) == 0) Failure(new OperationCanceledException()); }
        private void End(string outcome, Exception? error, IReadOnlyDictionary<string, object>? attributes)
        {
            if (Interlocked.Exchange(ref _ended, 1) != 0) return;
            lock (_pipeline._completionGate) {
            if (_pipeline.IsShutdown) {
                _pipeline._traceAdmission.RecordLateOffer(); _pipeline._logAdmission.RecordLateOffer();
                _activity?.SetCustomProperty(OwnerKey, null); _activity?.Stop(); return;
            }
            try {
                _activity?.SetTag("teaql.operation.outcome", outcome);
                _activity?.SetStatus(error == null ? ActivityStatusCode.Ok : ActivityStatusCode.Error);
                foreach (var pair in attributes ?? new Dictionary<string, object>())
                    if (pair.Key is "teaql.result.cardinality" or "teaql.cache.result") _activity?.SetTag(pair.Key, pair.Value);
                var elapsed = Stopwatch.GetElapsedTime(_started).TotalMilliseconds;
                var tags = new TagList { { "teaql.operation.family", _operation.Family }, { "teaql.operation.outcome", outcome } };
                _pipeline._duration.Record(elapsed, tags); _pipeline._operations.Add(1, tags);
                var values = new Dictionary<string, object> { ["teaql.operation.family"] = _operation.Family, ["teaql.operation.name"] = _operation.Name,
                    ["teaql.operation.outcome"] = outcome, ["teaql.operation.duration_ms"] = elapsed };
                if (error != null) {
                    var category = RuntimeErrorClassifier.Category(error);
                    _activity?.SetTag("teaql.error.type", error.GetType().Name); _activity?.SetTag("teaql.error.category", category);
                    values["teaql.error.category"] = category;
                }
                _pipeline.Emit("TeaQL runtime operation completed", values, Correlation);
            }
            finally { _activity?.Stop(); }
            }
        }
    }
    private sealed class OwnedAlwaysOnSampler(object owner) : Sampler {
        public override SamplingResult ShouldSample(in SamplingParameters parameters) => new(parameters.Tags?.Any(pair => pair.Key == OwnerKey && ReferenceEquals(pair.Value, owner)) == true
            ? SamplingDecision.RecordAndSample : SamplingDecision.Drop);
    }
    private sealed class OwnedTraceProcessor(BatchActivityExportProcessor batch, object owner, Admission admission) : BaseProcessor<Activity> {
        public override void OnEnd(Activity activity) {
            if (!ReferenceEquals(activity.GetCustomProperty(OwnerKey), owner)) return;
            if (!activity.Recorded) return;
            admission.Submit(SafeExporter<Activity>.RecordIdentity(activity)!, () => { batch.OnEnd(activity); return true; });
        }
        protected override bool OnForceFlush(int timeoutMilliseconds) => batch.ForceFlush(timeoutMilliseconds);
        protected override bool OnShutdown(int timeoutMilliseconds) => batch.Shutdown(timeoutMilliseconds);
        protected override void Dispose(bool disposing) { if (disposing) batch.Dispose(); base.Dispose(disposing); }
    }
    private sealed class OwnedLogProcessor(BatchLogRecordExportProcessor batch, Admission admission) : BaseProcessor<LogRecord> {
        public override void OnEnd(LogRecord record) {
            var identity = SafeExporter<LogRecord>.RecordIdentity(record);
            if (identity == null) return;
            admission.Submit(identity, () => { batch.OnEnd(record); return true; });
        }
        protected override bool OnForceFlush(int timeoutMilliseconds) => batch.ForceFlush(timeoutMilliseconds);
        protected override bool OnShutdown(int timeoutMilliseconds) => batch.Shutdown(timeoutMilliseconds);
        protected override void Dispose(bool disposing) { if (disposing) batch.Dispose(); base.Dispose(disposing); }
    }
    private sealed class CarriedLogCorrelationProcessor : BaseProcessor<LogRecord> {
        internal const string TraceKey = "teaql.internal.execution.trace", SpanKey = "teaql.internal.execution.span";
        public override void OnEnd(LogRecord record) {
            var values = record.Attributes ?? Array.Empty<KeyValuePair<string, object?>>();
            var trace = values.FirstOrDefault(pair => pair.Key == TraceKey).Value as string;
            var span = values.FirstOrDefault(pair => pair.Key == SpanKey).Value as string;
            if (trace != null && span != null) {
                record.TraceId = ActivityTraceId.CreateFromString(trace); record.SpanId = ActivitySpanId.CreateFromString(span); record.TraceFlags = ActivityTraceFlags.Recorded;
            }
            record.Attributes = values.Where(pair => pair.Key != TraceKey && pair.Key != SpanKey).ToArray();
        }
    }
}
