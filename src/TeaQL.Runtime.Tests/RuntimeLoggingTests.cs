using System.Text.Json;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

public sealed class RuntimeLoggingTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "teaql-logging-test-" + Guid.NewGuid().ToString("N"));
    public RuntimeLoggingTests() => Directory.CreateDirectory(_directory);
    public void Dispose() => Directory.Delete(_directory, true);
    private RuntimeLogger New(Dictionary<string, string>? environment = null) => new(new()
    { Environment = environment ?? new(), WorkingDirectory = _directory });
    private static JsonElement[] Records(params string[] paths) => paths.SelectMany(path =>
    {
        var content = File.ReadAllText(path);
        Assert.EndsWith("\n", content);
        return content.TrimEnd('\n').Split('\n').Select(line =>
        {
            using var document = JsonDocument.Parse(line);
            var record = document.RootElement.Clone();
            foreach (var field in new[] { "timestamp", "level", "log_name", "runtime", "event", "message" })
                Assert.False(string.IsNullOrEmpty(record.GetProperty(field).GetString()));
            Assert.Equal("dotnet", record.GetProperty("runtime").GetString());
            Assert.Equal(TimeSpan.Zero, DateTimeOffset.Parse(record.GetProperty("timestamp").GetString()!).Offset);
            return record;
        }).ToArray();
    }).ToArray();

    [Fact]
    public void DefaultStartupBeforeQueryAndAppendAcrossTwoStarts()
    {
        var logger = New();
        Assert.Equal(Path.Combine(_directory, "logs", "teaql.log"), logger.FilePath);
        Assert.Equal(50 * 1024 * 1024, logger.MaxSize); Assert.Equal(7, logger.MaxFiles);
        Assert.Equal("runtime.start", Assert.Single(Records(logger.FilePath!)).GetProperty("event").GetString());
        var second = New(); second.Write("INFO", "probe", "Completed");
        Assert.Equal(3, Records(logger.FilePath!).Length);
    }

    [Theory]
    [InlineData("application", "domain", "model", "application")]
    [InlineData(null, "domain", "model", "domain")]
    [InlineData(null, null, "model", "model")]
    [InlineData(null, null, null, "teaql")]
    [InlineData("订单", null, null, "订单")]
    public void NamePrecedenceAndOverride(string? explicitName, string? domain, string? model, string expected)
    {
        var environment = new Dictionary<string, string> { ["TEAQL_LOG_ENDPOINT"] = "other/output.jsonl" };
        if (domain is not null) environment["TEAQL_DOMAIN"] = domain;
        var logger = new RuntimeLogger(new() { LogName = explicitName, ModelNames = model is null ? [] : [model], Environment = environment, WorkingDirectory = _directory });
        Assert.Equal(expected, logger.LogName);
        Assert.Equal(expected, Assert.Single(Records(logger.FilePath!)).GetProperty("log_name").GetString());
        Assert.Equal(Path.Combine(_directory, "other", "output.jsonl"), logger.FilePath);
        var multiple = new RuntimeLogger(new() { ModelNames = ["one", "two"], Environment = new Dictionary<string, string>(), WorkingDirectory = _directory });
        Assert.Equal("teaql", multiple.LogName);
    }

    [Theory]
    [InlineData("")][InlineData(".")][InlineData("..")][InlineData("../secret")][InlineData("a/b")][InlineData("a\\b")]
    [InlineData("a\nname")][InlineData("NUL")][InlineData("COM1.txt")][InlineData("trailing.")][InlineData("trailing ")]
    public void RejectsInvalidNames(string name) => Assert.Throws<InvalidOperationException>(() =>
        new RuntimeLogger(new() { LogName = name, Environment = new Dictionary<string, string>(), WorkingDirectory = _directory }));

    [Theory]
    [InlineData("1", 1L)][InlineData("2K", 2048L)][InlineData("3KB", 3072L)][InlineData("2m", 2097152L)]
    [InlineData("1MB", 1048576L)][InlineData("1G", 1073741824L)][InlineData("1GB", 1073741824L)]
    public void BinarySizeUnits(string value, long expected) => Assert.Equal(expected, RuntimeLogger.ParseSize(value));

    [Theory]
    [InlineData("0")][InlineData("-1")][InlineData("1.2M")][InlineData("1TB")][InlineData("999999999999999999999G")]
    public void RejectsInvalidSizes(string value) => Assert.ThrowsAny<Exception>(() => RuntimeLogger.ParseSize(value));

    [Fact]
    public void OversizedWholeUtf8RecordsArchiveOrderAndRetention()
    {
        var logger = New(new() { ["TEAQL_LOG_MAX_SIZE"] = "256", ["TEAQL_LOG_MAX_FILES"] = "2" });
        for (var i = 0; i < 5; i++) Assert.True(logger.Write("INFO", "probe", new string('界', 200), new Dictionary<string, object?> { ["sequence"] = i }));
        for (var i = 0; i < 3; i++)
        {
            var record = Assert.Single(Records(logger.FilePath + (i == 0 ? "" : "." + i)));
            Assert.Equal(4 - i, record.GetProperty("sequence").GetInt32());
            Assert.Equal(new string('界', 200), record.GetProperty("message").GetString());
        }
        Assert.False(File.Exists(logger.FilePath + ".3"));
        var zero = New(new() { ["TEAQL_LOG_MAX_SIZE"] = "256", ["TEAQL_LOG_MAX_FILES"] = "0" });
        zero.Write("INFO", "probe", new string('x', 400));
        Assert.Single(Directory.GetFiles(Path.GetDirectoryName(zero.FilePath!)!));
        Assert.Single(Records(zero.FilePath!));
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public void UtcDayRotationAndRestart(bool restart)
    {
        var logger = New(); var previous = File.ReadAllBytes(logger.FilePath!);
        File.SetLastWriteTimeUtc(logger.FilePath!, DateTime.UtcNow.AddDays(-1));
        if (restart) logger = New(); else logger.Write("INFO", "next.day", "Next day");
        Assert.Equal(previous, File.ReadAllBytes(logger.FilePath + ".1"));
        Assert.Single(Records(logger.FilePath!));
    }

    [Fact]
    public void OrdinaryUtf8RecordsRespectWholeLineByteLimit()
    {
        var logger = New(new() { ["TEAQL_LOG_MAX_SIZE"] = "1K", ["TEAQL_LOG_MAX_FILES"] = "1000" });
        for (var i = 0; i < 40; i++) Assert.True(logger.Write("INFO", "regular", new string('界', 30), new Dictionary<string, object?> { ["index"] = i }));
        var files = Directory.GetFiles(Path.GetDirectoryName(logger.FilePath!)!);
        Assert.True(files.Length > 1);
        foreach (var file in files) Assert.InRange(new FileInfo(file).Length, 1, 1024);
        var all = Records(files); Assert.Equal(41, all.Length);
        Assert.Equal(Enumerable.Range(0, 40), all.Where(row => row.TryGetProperty("index", out _)).Select(row => row.GetProperty("index").GetInt32()).Order());
    }

    [Fact]
    public async Task RotationFailurePreservesForeignArchiveAndBusinessOutcome()
    {
        var logger = New(); var before = File.ReadAllBytes(logger.FilePath!);
        var archive = logger.FilePath + ".1"; Directory.CreateDirectory(archive);
        File.WriteAllText(Path.Combine(archive, "marker"), "preserve");
        File.SetLastWriteTimeUtc(logger.FilePath!, DateTime.UtcNow.AddDays(-1));
        var context = new UserContext(logger); context.PutToLocalCache("probe", 42);
        Assert.Equal(42, context.GetFromLocalCache<int>("probe"));
        var original = new IOException("private-original-error");
        Assert.Same(original, await Assert.ThrowsAsync<IOException>(() => context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query", "probe"), () => Task.FromException<int>(original))));
        Assert.False(logger.Write("INFO", "probe", "Completed"));
        Assert.Equal(before, File.ReadAllBytes(logger.FilePath!));
        Assert.Equal("preserve", File.ReadAllText(Path.Combine(archive, "marker")));
    }

    [Fact]
    public async Task NativeRuleRejectionsAreWarningsAndPreserveOriginalErrors()
    {
        var logger = New(); var context = new UserContext(logger);
        Exception[] errors = [new CheckException([]), new TeaQL.Core.RequestIntentException("QUERY_PURPOSE_REQUIRED", "purpose", "query"), new MutationPolicyException("private-policy-value")];
        foreach (var error in errors)
        {
            try { await context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query", "probe"), () => Task.FromException<int>(error)); Assert.Fail("Expected rejection"); }
            catch (Exception actual) { Assert.Same(error, actual); }
        }
        var all = Records(logger.FilePath!); Assert.Equal(4, all.Length);
        Assert.All(all.Skip(1), row => Assert.Equal("WARN", row.GetProperty("level").GetString()));
        Assert.DoesNotContain("private-policy-value", File.ReadAllText(logger.FilePath!));
    }

    [Fact]
    public async Task OperationAndSqlFilesKeepAvailableTraceCorrelation()
    {
        var logger = New(); var context = new UserContext(logger);
        using var activity = new System.Diagnostics.Activity("probe").SetIdFormat(System.Diagnostics.ActivityIdFormat.W3C).Start();
        Assert.Equal(42, await context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query", "probe"), () => Task.FromResult(42)));
        context.DiagnosticSqlLogSink!.Write(new ExecutionMetadata { Operation = DataServiceOperation.Query, ExecutionOutcome = "success", SqlCorrelation = new SqlExecutionCorrelation(activity.TraceId.ToString(), activity.SpanId.ToString()) });
        var all = Records(logger.FilePath!); Assert.Equal(3, all.Length);
        Assert.All(all.Skip(1), row => Assert.Equal(activity.TraceId.ToString(), row.GetProperty("trace_id").GetString()));
    }

    [Fact]
    public async Task StartupFatalPostStartupPreservesActualBusinessResultsAndExtensionIdentity()
    {
        Directory.CreateDirectory(Path.Combine(_directory, "logs", "teaql.log"));
        Assert.Throws<InvalidOperationException>(() => New());
        Directory.Delete(Path.Combine(_directory, "logs", "teaql.log"));
        var logger = New(); var telemetry = NoopRuntimeTelemetry.Instance;
        var context = new UserContext(logger).WithRuntimeTelemetry(telemetry);
        Assert.Same(telemetry, context.RuntimeTelemetry);
        context.PutToLocalCache("safe-probe-key", 42);
        File.Delete(logger.FilePath!); Directory.CreateDirectory(logger.FilePath!);
        Assert.Equal(42, context.GetFromLocalCache<int>("safe-probe-key"));
        var failure = new IOException("private-original-error");
        var actual = await Assert.ThrowsAsync<IOException>(() => context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query", "probe"), () => Task.FromException<int>(failure)));
        Assert.Same(failure, actual);
        Assert.False(logger.Write("INFO", "probe", "Completed"));
    }

    private sealed class ValidationException(string message) : Exception(message);

    [Fact]
    public async Task OutcomeSeverityAuditAndSqlUseSafeProjection()
    {
        var logger = New(); var context = new UserContext(logger);
        context.PutToLocalCache("private-cache-key", 7);
        await Assert.ThrowsAsync<ValidationException>(() => context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query", "probe"), () => Task.FromException<int>(new ValidationException("secret-error-value"))));
        await Assert.ThrowsAsync<IOException>(() => context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query", "probe"), () => Task.FromException<int>(new IOException("secret-error-value"))));
        context.DiagnosticSqlLogSink!.Write(new ExecutionMetadata { Operation = DataServiceOperation.Query, DebugQuery = "unprojected-sql-secret", ExecutionOutcome = "retrying" });
        await context.PublishAppAuditEventAsync(new Dictionary<string, object?> { ["entityType"] = "Probe", ["mutationKind"] = "insert", ["reason"] = "already projected" });
        var records = Records(logger.FilePath!);
        Assert.Contains(records, r => r.GetProperty("level").GetString() == "INFO" && r.GetProperty("event").GetString() == "operation.success");
        Assert.Contains(records, r => r.GetProperty("level").GetString() == "WARN" && r.GetProperty("event").GetString() == "operation.failure");
        Assert.Contains(records, r => r.GetProperty("level").GetString() == "ERROR" && r.GetProperty("event").GetString() == "operation.failure");
        Assert.Contains(records, r => r.GetProperty("level").GetString() == "WARN" && r.GetProperty("category").GetString() == "SQL");
        Assert.Contains(records, r => r.GetProperty("event").GetString() == "audit.event" && r.GetProperty("category").GetString() == "AUDIT");
        var text = File.ReadAllText(logger.FilePath!);
        Assert.DoesNotContain("secret-error-value", text); Assert.DoesNotContain("unprojected-sql-secret", text); Assert.DoesNotContain("private-cache-key", text);
    }

    [Fact]
    public void ConcurrentStartupWhileOtherInstancesRotate()
    {
        Parallel.For(0, 100, index =>
        {
            var logger = New(new() { ["TEAQL_LOG_MAX_SIZE"] = "1", ["TEAQL_LOG_MAX_FILES"] = "1000" });
            Assert.True(logger.Write("INFO", "startup.race", "Completed", new Dictionary<string, object?> { ["id"] = index }));
        });
        var records = Records(Directory.GetFiles(Path.Combine(_directory, "logs")));
        Assert.Equal(200, records.Length);
        Assert.Equal(100, records.Count(record => record.GetProperty("event").GetString() == "runtime.start"));
    }

    [Fact]
    public void ConcurrentInstancesRotateAndReopenTheActivePath()
    {
        var loggers = Enumerable.Range(0, 8).Select(_ => New(new() { ["TEAQL_LOG_MAX_SIZE"] = "1K", ["TEAQL_LOG_MAX_FILES"] = "1000" })).ToArray();
        Parallel.For(0, 240, index => Assert.True(loggers[index % 8].Write("INFO", "concurrent", "Completed", new Dictionary<string, object?> { ["id"] = index })));
        var records = Records(Directory.GetFiles(Path.GetDirectoryName(loggers[0].FilePath!)!));
        Assert.Equal(248, records.Length);
        Assert.Equal(240, records.Where(r => r.TryGetProperty("id", out _)).Select(r => r.GetProperty("id").GetInt32()).Distinct().Count());
    }
}
