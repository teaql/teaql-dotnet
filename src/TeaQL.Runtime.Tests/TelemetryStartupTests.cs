using Xunit;

namespace TeaQL.Runtime.Tests;

[Collection("Runtime telemetry SDK")]
public class TelemetryStartupTests
{
    private static Dictionary<string, string?> Input(string endpoint = "http://localhost:4318/") => new() {
        [TelemetryStartupConfiguration.EnabledVariable] = "true",
        [TelemetryStartupConfiguration.ServiceVariable] = "payment-service",
        [TelemetryStartupConfiguration.EndpointVariable] = endpoint
    };

    [Fact]
    public void DisabledStartupListsThreeSettingsWithoutEchoingUnusedValues()
    {
        var config = TelemetryStartupConfiguration.Read(new Dictionary<string, string?> { [TelemetryStartupConfiguration.ServiceVariable] = "PRIVATE-SERVICE" });
        Assert.False(config.Enabled); Assert.DoesNotContain("PRIVATE-SERVICE", config.Message);
        foreach (var key in Input().Keys) Assert.Contains(key, config.Message);
    }
    [Theory]
    [InlineData("http://SECRET:password@localhost:4318")]
    [InlineData("http://localhost:4318?token=SECRET")]
    [InlineData("http://localhost:4318?")]
    [InlineData("http://localhost:4318/#SECRET")]
    [InlineData("file:///SECRET")]
    [InlineData("http://@localhost:4318")]
    public void InvalidEndpointNeverAppearsInStartup(string endpoint)
    {
        var config = TelemetryStartupConfiguration.Read(Input(endpoint));
        Assert.False(config.Enabled); Assert.Equal("invalid-endpoint", config.ErrorCode);
        Assert.DoesNotContain("SECRET", config.Message);
    }
    [Fact]
    public void ValidConfigurationShowsSafeEffectiveSettingsAndFullRecording()
    {
        var config = TelemetryStartupConfiguration.Read(Input());
        Assert.True(config.Enabled); Assert.Equal("http://localhost:4318", config.Endpoint);
        Assert.Contains("full recording", config.Message); Assert.Contains("payment-service", config.Message);
    }
    [Fact]
    public void DisabledFactoryIsNotCalledAndEnabledPipelineIsShared()
    {
        var previous = TelemetryStartup.RegisteredFactory;
        try {
            var factory = new CountingFactory(); TelemetryStartup.RegisterFactory(factory);
            Assert.Same(NoopRuntimeTelemetry.Instance, TelemetryStartup.Initialize(new(false)));
            Assert.Equal(0, factory.Calls);
            var config = new TelemetryStartupConfiguration(true, "startup-sharing-test", "http://localhost:4318");
            var first = TelemetryStartup.Initialize(config); var second = TelemetryStartup.Initialize(config);
            Assert.Same(first, second); Assert.Equal(1, factory.Calls);
        }
        finally { TelemetryStartup.RestoreFactory(previous); }
    }
    [Fact]
    public void FactoryFailurePreservesBusinessContinuity()
    {
        var previous = TelemetryStartup.RegisteredFactory;
        try {
            TelemetryStartup.RegisterFactory(new CountingFactory { Fail = true });
            Assert.Same(NoopRuntimeTelemetry.Instance, TelemetryStartup.Initialize(new(true, "startup-failure-test", "http://localhost:4318")));
        }
        finally { TelemetryStartup.RestoreFactory(previous); }
    }
    private sealed class CountingFactory : IStandardTelemetryFactory
    {
        public int Calls; public bool Fail;
        public IRuntimeTelemetry Create(TelemetryStartupConfiguration configuration) {
            Interlocked.Increment(ref Calls); if (Fail) throw new InvalidOperationException("PRIVATE-FAILURE");
            return new TestPipeline();
        }
    }
    private sealed class TestPipeline : IRuntimeTelemetry {
        public IRuntimeTelemetryScope Start(RuntimeOperation operation) => NoopRuntimeTelemetry.Instance.Start(operation);
    }
}
