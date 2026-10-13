using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TeaQL.Runtime;

public sealed record TelemetryStartupConfiguration(bool Enabled, string ServiceName = "", string Endpoint = "", string ErrorCode = "")
{
    public const string EnabledVariable = "TEAQL_TELEMETRY_ENABLED";
    public const string ServiceVariable = "TEAQL_TELEMETRY_SERVICE_NAME";
    public const string EndpointVariable = "TEAQL_TELEMETRY_OTLP_ENDPOINT";
    public const string Hints = "TEAQL_TELEMETRY_ENABLED=true TEAQL_TELEMETRY_SERVICE_NAME=<application-name> TEAQL_TELEMETRY_OTLP_ENDPOINT=<collector-url>";

    public static TelemetryStartupConfiguration Read(IReadOnlyDictionary<string, string?> environment)
    {
        string Get(string name) => environment.TryGetValue(name, out var value) ? value ?? "" : "";
        var flag = Get(EnabledVariable).Trim();
        if (flag.Length == 0 || flag.Equals("false", StringComparison.OrdinalIgnoreCase)) return new(false);
        if (!flag.Equals("true", StringComparison.OrdinalIgnoreCase)) return new(false, ErrorCode: "invalid-enable");
        var service = Get(ServiceVariable).Trim();
        if (!Regex.IsMatch(service, @"\A[a-zA-Z0-9_.-]{1,120}\z")) return new(false, ErrorCode: "invalid-service");
        var endpoint = Get(EndpointVariable);
        if (endpoint.IndexOfAny(['?', '#']) >= 0 || !Uri.TryCreate(endpoint, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https") || uri.Host.Length == 0 || uri.UserInfo.Length != 0
            || endpoint.Contains('@')) return new(false, ErrorCode: "invalid-endpoint");
        return new(true, service, uri.AbsoluteUri.TrimEnd('/'));
    }

    public string Message => ErrorCode.Length > 0
        ? $"TeaQL telemetry: disabled; configuration={ErrorCode}. {Hints}"
        : !Enabled ? $"TeaQL telemetry: supported, disabled. Enable full masked diagnostics with {Hints}"
        : $"TeaQL telemetry: starting standard module; full recording, masked SQL; {EnabledVariable}=true {ServiceVariable}={ServiceName} {EndpointVariable}={Endpoint}";
}

// Optional SDK assembly implements this small factory; Core and Runtime do not
// reference any SDK package. Discovery occurs only after explicit enablement.
public interface IStandardTelemetryFactory
{
    IRuntimeTelemetry Create(TelemetryStartupConfiguration configuration);
}
public interface IStandardTelemetryLifecycle { bool IsShutdown { get; } }

public static class TelemetryStartup
{
    private static readonly ConcurrentDictionary<string, byte> Announced = new();
    private static readonly ConcurrentDictionary<string, Lazy<Task<IRuntimeTelemetry>>> Pipelines = new();
    private static readonly object FactoryGate = new();
    private static IStandardTelemetryFactory? _factory;

    public static void RegisterFactory(IStandardTelemetryFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        lock (FactoryGate) _factory = factory;
    }
    internal static IStandardTelemetryFactory? RegisteredFactory { get { lock (FactoryGate) return _factory; } }
    internal static void RestoreFactory(IStandardTelemetryFactory? factory) { lock (FactoryGate) _factory = factory; }

    public static void Notice(string fixedSafeMessage)
    {
        try { Console.Error.WriteLine(fixedSafeMessage); } catch (Exception error) when (error is not OutOfMemoryException) { }
    }
    private static void Announce(string message) { if (Announced.TryAdd(message, 0)) Notice(message); }

    internal static IRuntimeTelemetry Default()
    {
        var environment = new Dictionary<string, string?>();
        foreach (var key in new[] { TelemetryStartupConfiguration.EnabledVariable, TelemetryStartupConfiguration.ServiceVariable, TelemetryStartupConfiguration.EndpointVariable })
            environment[key] = Environment.GetEnvironmentVariable(key);
        return Initialize(TelemetryStartupConfiguration.Read(environment));
    }

    internal static IRuntimeTelemetry Initialize(TelemetryStartupConfiguration configuration)
    {
        Announce(configuration.Message);
        if (!configuration.Enabled) return NoopRuntimeTelemetry.Instance;
        var key = configuration.ServiceName + "\0" + configuration.Endpoint;
        var lazy = Pipelines.GetOrAdd(key, _ => new(() => BuildWithDeadline(configuration), LazyThreadSafetyMode.ExecutionAndPublication));
        try
        {
            if (!lazy.Value.Wait(TimeSpan.FromSeconds(6)))
            {
                Announce("TeaQL telemetry: initialization timed out; business continues without export.");
                return NoopRuntimeTelemetry.Instance;
            }
            var pipeline = lazy.Value.GetAwaiter().GetResult();
            if (pipeline is IStandardTelemetryLifecycle { IsShutdown: true })
            {
                if (Pipelines.TryRemove(new KeyValuePair<string, Lazy<Task<IRuntimeTelemetry>>>(key, lazy))) return Initialize(configuration);
                return NoopRuntimeTelemetry.Instance;
            }
            return pipeline;
        }
        catch (Exception error) when (error is not OutOfMemoryException)
        {
            Announce("TeaQL telemetry: initialization failed; business continues without export.");
            return NoopRuntimeTelemetry.Instance;
        }
    }

    private static async Task<IRuntimeTelemetry> BuildWithDeadline(TelemetryStartupConfiguration configuration)
    {
        var construction = Task.Run(() => Build(configuration));
        try { return await construction.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false); }
        catch (TimeoutException)
        {
            Announce("TeaQL telemetry: initialization timed out; business continues without export.");
            // All callers share this timed result, not the raw constructor task.
            // No later context can adopt a pipeline scheduled for cleanup.
            _ = construction.ContinueWith(async completed => {
                if (completed.Status == TaskStatus.RanToCompletion)
                    try { await completed.Result.ShutdownAsync().WaitAsync(TimeSpan.FromSeconds(6)).ConfigureAwait(false); } catch { }
            }, TaskScheduler.Default);
            return NoopRuntimeTelemetry.Instance;
        }
    }

    private static IRuntimeTelemetry Build(TelemetryStartupConfiguration configuration)
    {
        IStandardTelemetryFactory? factory;
        lock (FactoryGate) factory = _factory;
        if (factory == null)
        {
            try
            {
                var assembly = Assembly.Load("TeaQL.Telemetry.OpenTelemetry");
                var type = assembly.GetType("TeaQL.Telemetry.OpenTelemetry.StandardTelemetryFactory", throwOnError: true)!;
                factory = Activator.CreateInstance(type) as IStandardTelemetryFactory;
            }
            catch (Exception error) when (error is not OutOfMemoryException)
            {
                Announce("TeaQL telemetry: standard module unavailable; add TeaQL.Telemetry.OpenTelemetry. " + TelemetryStartupConfiguration.Hints);
                return NoopRuntimeTelemetry.Instance;
            }
        }
        try
        {
            var pipeline = factory?.Create(configuration) ?? NoopRuntimeTelemetry.Instance;
            if (pipeline is not NoopRuntimeTelemetry) Notice("TeaQL telemetry: enabled; full recording, traces, metrics, logs and masked SQL.");
            return pipeline;
        }
        catch (Exception error) when (error is not OutOfMemoryException)
        {
            Announce("TeaQL telemetry: initialization failed; business continues without export.");
            return NoopRuntimeTelemetry.Instance;
        }
    }
}
