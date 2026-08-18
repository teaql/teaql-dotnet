using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace TeaQL.Runtime;

public sealed record RuntimeOperation(
    string Family,
    string Name,
    IReadOnlyDictionary<string, object> Attributes)
{
    private static readonly HashSet<string> Forbidden = new(StringComparer.Ordinal)
    {
        "teaql.entity.id", "teaql.user.id", "teaql.tenant.id",
        "teaql.query.parameters", "teaql.field.values", "teaql.audit.reason",
        "db.query.parameter_values", "http.request.body", "url.full"
    };

    public static RuntimeOperation Create(
        string family, string name, IReadOnlyDictionary<string, object>? attributes = null)
    {
        var safe = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["teaql.operation.family"] = family,
            ["teaql.operation.name"] = name
        };
        foreach (var pair in attributes ?? new Dictionary<string, object>())
            if (!Forbidden.Contains(pair.Key) && IsSafeValue(pair.Value)) safe[pair.Key] = pair.Value;
        return new RuntimeOperation(family, name, safe);
    }

    private static bool IsSafeValue(object value) =>
        value is string or bool or byte or sbyte or short or ushort or int or uint
            or long or ulong or float or double or decimal;
}

public interface IRuntimeTelemetryScope
{
    void Success(IReadOnlyDictionary<string, object>? attributes = null);
    void Failure(Exception error);
}

public interface IRuntimeTelemetry
{
    IRuntimeTelemetryScope Start(RuntimeOperation operation);
    Task FlushAsync() => Task.CompletedTask;
    Task ShutdownAsync() => Task.CompletedTask;
}

public sealed class NoopRuntimeTelemetry : IRuntimeTelemetry
{
    public static readonly NoopRuntimeTelemetry Instance = new();
    private NoopRuntimeTelemetry() { }
    public IRuntimeTelemetryScope Start(RuntimeOperation operation) => NoopScope.Instance;
    private sealed class NoopScope : IRuntimeTelemetryScope
    {
        public static readonly NoopScope Instance = new();
        public void Success(IReadOnlyDictionary<string, object>? attributes = null) { }
        public void Failure(Exception error) { }
    }
}

public static class RuntimeTelemetryExtensions
{
    public static IRuntimeTelemetryScope StartSafely(
        this IRuntimeTelemetry? telemetry, RuntimeOperation operation)
    {
        try { return new FailOpenScope((telemetry ?? NoopRuntimeTelemetry.Instance).Start(operation)); }
        catch { return NoopRuntimeTelemetry.Instance.Start(operation); }
    }

    public static async Task<T> ObserveAsync<T>(
        this IRuntimeTelemetry? telemetry, RuntimeOperation operation,
        Func<Task<T>> work, Func<T, IReadOnlyDictionary<string, object>?>? completion = null)
    {
        var scope = telemetry.StartSafely(operation);
        try
        {
            var result = await work().ConfigureAwait(false);
            scope.Success(completion?.Invoke(result));
            return result;
        }
        catch (Exception error)
        {
            scope.Failure(error);
            throw;
        }
    }

    private sealed class FailOpenScope : IRuntimeTelemetryScope
    {
        private IRuntimeTelemetryScope? _delegate;
        public FailOpenScope(IRuntimeTelemetryScope @delegate) => _delegate = @delegate;
        public void Success(IReadOnlyDictionary<string, object>? attributes = null) =>
            Finish(scope => scope.Success(attributes));
        public void Failure(Exception error) => Finish(scope => scope.Failure(error));
        private void Finish(Action<IRuntimeTelemetryScope> action)
        {
            var scope = System.Threading.Interlocked.Exchange(ref _delegate, null);
            if (scope is null) return;
            try { action(scope); } catch { }
        }
    }
}

public sealed class OpenTelemetryRuntimeTelemetry : IRuntimeTelemetry, IDisposable
{
    private readonly ActivitySource _activities;
    private readonly Meter _meter;
    private readonly Histogram<double> _duration;
    private readonly Counter<long> _operations;

    public OpenTelemetryRuntimeTelemetry(string instrumentationScope = "io.teaql.runtime")
    {
        _activities = new ActivitySource(instrumentationScope);
        _meter = new Meter(instrumentationScope);
        _duration = _meter.CreateHistogram<double>(
            "teaql.runtime.operation.duration", "ms", "TeaQL runtime operation duration");
        _operations = _meter.CreateCounter<long>(
            "teaql.runtime.operation.count", "{operation}", "Completed TeaQL runtime operations");
    }

    public IRuntimeTelemetryScope Start(RuntimeOperation operation)
    {
        var activity = _activities.StartActivity($"teaql.{operation.Family}", ActivityKind.Internal);
        if (activity is not null)
            foreach (var pair in operation.Attributes) activity.SetTag(pair.Key, pair.Value);
        return new OpenTelemetryScope(activity, operation, _duration, _operations);
    }

    public void Dispose()
    {
        _activities.Dispose();
        _meter.Dispose();
    }

    private sealed class OpenTelemetryScope : IRuntimeTelemetryScope
    {
        private Activity? _activity;
        private readonly RuntimeOperation _operation;
        private readonly Histogram<double> _duration;
        private readonly Counter<long> _operations;
        private readonly long _startedAt = Stopwatch.GetTimestamp();

        public OpenTelemetryScope(Activity? activity, RuntimeOperation operation,
            Histogram<double> duration, Counter<long> operations)
        {
            _activity = activity;
            _operation = operation;
            _duration = duration;
            _operations = operations;
        }

        public void Success(IReadOnlyDictionary<string, object>? attributes = null)
        {
            if (_activity is null) { Finish("success"); return; }
            foreach (var pair in attributes ?? new Dictionary<string, object>())
                if (pair.Key is "teaql.result.cardinality" or "teaql.cache.result")
                    _activity.SetTag(pair.Key, pair.Value);
            _activity.SetStatus(ActivityStatusCode.Ok);
            Finish("success");
        }

        public void Failure(Exception error)
        {
            _activity?.SetTag("teaql.error.type", error.GetType().Name);
            _activity?.SetStatus(ActivityStatusCode.Error);
            Finish("failure");
        }

        private void Finish(string outcome)
        {
            var activity = System.Threading.Interlocked.Exchange(ref _activity, null);
            var tags = new TagList
            {
                { "teaql.operation.family", _operation.Family },
                { "teaql.operation.outcome", outcome }
            };
            _duration.Record(Stopwatch.GetElapsedTime(_startedAt).TotalMilliseconds, tags);
            _operations.Add(1, tags);
            activity?.Stop();
        }
    }
}
