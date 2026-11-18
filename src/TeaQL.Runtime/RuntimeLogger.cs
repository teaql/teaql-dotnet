using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public sealed record RuntimeLoggingOptions
{
    public string? LogName { get; init; }
    public string? LogFormat { get; init; }
    public IReadOnlyList<string> ModelNames { get; init; } = Array.Empty<string>();
    public IReadOnlyDictionary<string, string>? Environment { get; init; }
    public string? WorkingDirectory { get; init; }
}

/// <summary>Application-owned JSON/human logging. Fields must be safely projected
/// before Write; the default SQL and audit paths retain their privacy policies.</summary>
public sealed class RuntimeLogger
{
    private static readonly string[] Settings = ["TEAQL_DOMAIN", "TEAQL_LOG_ENDPOINT", "TEAQL_LOG_MAX_SIZE", "TEAQL_LOG_MAX_FILES", "TEAQL_LOG_FORMAT"];
    private static readonly ConcurrentDictionary<string, object> Gates = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, RuntimeLogger> Defaults = new(StringComparer.Ordinal);
    private static readonly object DefaultGate = new();
    private static readonly Regex SizePattern = new(@"^([0-9]+)\s*(K|KB|M|MB|G|GB)?$", RegexOptions.CultureInvariant);
    private static readonly Regex Device = new(@"^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex Archive = new(@"^[1-9][0-9]*$", RegexOptions.CultureInvariant);
    private static readonly Regex HumanHeader = new(@"^\d{4}-\d\d-\d\dT\S+(?:Z|\+00:00) \[TeaQL\]\[.*\]\[(?:INFO|WARN|ERROR)\] .+ runtime=\S+$", RegexOptions.CultureInvariant);
    private static readonly HashSet<string> EventFields = ["timestamp", "level", "log_name", "runtime", "event", "message"];
    private static readonly UTF8Encoding Utf8 = new(false, true);
    private readonly object _gate;
    private readonly string? _stream;
    public string LogName { get; }
    public string LogFormat { get; }
    public string? FilePath { get; }
    public long MaxSize { get; }
    public int MaxFiles { get; }

    internal static Dictionary<string, string> ReadEnvironment() => Settings
        .Select(key => (key, value: System.Environment.GetEnvironmentVariable(key)))
        .Where(pair => pair.value is not null).ToDictionary(pair => pair.key, pair => pair.value!);

    public RuntimeLogger(RuntimeLoggingOptions? options = null)
    {
        options ??= new();
        var name = "teaql";
        try
        {
            var environment = options.Environment ?? ReadEnvironment();
            string? Setting(string key) => environment.TryGetValue(key, out var value) ? value : null;
            name = options.LogName ?? (Setting("TEAQL_DOMAIN") is { Length: > 0 } domain ? domain :
                options.ModelNames.Count == 1 ? options.ModelNames[0] : "teaql");
            if (name.Length == 0 || name is "." or ".." || name.EndsWith(' ') || name.EndsWith('.')
                || name.Any(c => char.IsControl(c) || "<>:\"/\\|?*".Contains(c)) || Device.IsMatch(name.Split('.')[0]))
                throw new ArgumentException();
            _ = Utf8.GetBytes(name);
            LogName = name;
            LogFormat = options.LogFormat ?? Setting("TEAQL_LOG_FORMAT") ?? "json";
            if (LogFormat is not ("json" or "human")) throw new ArgumentException();
            MaxSize = ParseSize(Setting("TEAQL_LOG_MAX_SIZE") ?? "50M");
            var files = Setting("TEAQL_LOG_MAX_FILES") ?? "7";
            if (!Regex.IsMatch(files, @"^[0-9]+$", RegexOptions.CultureInvariant)
                || !int.TryParse(files, NumberStyles.None, CultureInfo.InvariantCulture, out var retention)) throw new ArgumentException();
            MaxFiles = retention;
            var endpoint = Setting("TEAQL_LOG_ENDPOINT");
            if (string.IsNullOrEmpty(endpoint)) endpoint = System.IO.Path.Combine("logs", name + ".log");
            if (endpoint is "stdout" or "stderr" or "-")
            {
                _stream = endpoint;
                _gate = new();
            }
            else
            {
                if (endpoint.StartsWith("file:", StringComparison.Ordinal))
                {
                    var uri = new Uri(endpoint, UriKind.Absolute);
                    if (!uri.IsFile || (uri.Host.Length != 0 && uri.Host != "localhost") || uri.Query.Length != 0 || uri.Fragment.Length != 0) throw new ArgumentException();
                    endpoint = uri.LocalPath;
                }
                else if (endpoint.Contains("://", StringComparison.Ordinal)) throw new ArgumentException();
                var path = System.IO.Path.GetFullPath(endpoint, options.WorkingDirectory ?? System.Environment.CurrentDirectory);
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path)!);
                // Resolve existing links before selecting the shared destination.
                var parent = new DirectoryInfo(System.IO.Path.GetDirectoryName(path)!);
                var resolvedParent = parent.ResolveLinkTarget(true)?.FullName ?? parent.FullName;
                path = System.IO.Path.Combine(resolvedParent, System.IO.Path.GetFileName(path));
                // Another writer can rotate between an existence check and
                // link resolution. Absence here is normal; append reopens
                // the active path under the shared lock below.
                FileSystemInfo? target = null;
                try { target = new FileInfo(path).ResolveLinkTarget(true); }
                catch (FileNotFoundException) { }
                FilePath = target?.FullName ?? path;
                _gate = Gates.GetOrAdd(FilePath, _ => new object());
            }
            Emit(Record("INFO", "runtime.start", "TeaQL runtime started", null));
        }
        catch
        {
            // Never include paths, raw exceptions, environment values or names
            // that failed validation in diagnostic failure messages.
            Failure(LogName ?? "teaql", "log_destination_unavailable");
            throw new InvalidOperationException("Runtime logging initialization failed");
        }
    }

    internal static RuntimeLogger Default()
    {
        var environment = ReadEnvironment();
        var cwd = System.Environment.CurrentDirectory;
        var key = JsonSerializer.Serialize(new object[] { cwd, environment });
        lock (DefaultGate)
        {
            if (!Defaults.TryGetValue(key, out var logger))
                Defaults[key] = logger = new(new() { Environment = environment, WorkingDirectory = cwd });
            return logger;
        }
    }

    public static long ParseSize(string value)
    {
        var match = SizePattern.Match(value.Trim().ToUpperInvariant());
        if (!match.Success || !long.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var size) || size <= 0)
            throw new ArgumentException("Invalid log size");
        var multiplier = match.Groups[2].Value switch { "K" or "KB" => 1024L, "M" or "MB" => 1024L * 1024, "G" or "GB" => 1024L * 1024 * 1024, _ => 1 };
        return checked(size * multiplier);
    }

    private Dictionary<string, object?> Record(string level, string @event, string message, IReadOnlyDictionary<string, object?>? fields)
    {
        var record = fields is null ? new Dictionary<string, object?>() : new(fields);
        record["timestamp"] = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
        record["level"] = level; record["log_name"] = LogName; record["runtime"] = "dotnet";
        record["event"] = @event; record["message"] = message;
        return record;
    }

    public bool Write(string level, string @event, string message, IReadOnlyDictionary<string, object?>? safeFields = null)
    {
        try
        {
            if (level is not ("INFO" or "WARN" or "ERROR")) throw new ArgumentException();
            Emit(Record(level, @event, message, safeFields));
            return true;
        }
        catch { Failure(LogName, "log_write_failed"); return false; }
    }

    internal static void Failure(string name, string code)
    {
        try
        {
            Console.Error.WriteLine(JsonSerializer.Serialize(new { timestamp = DateTimeOffset.UtcNow.ToString("O"), level = "ERROR", log_name = name,
                runtime = "dotnet", @event = "logging.failure", message = "Runtime logging failed", error_code = code }));
        }
        catch { }
    }

    private IReadOnlyList<(int Number, string Path)> Archives() => Directory.EnumerateFileSystemEntries(System.IO.Path.GetDirectoryName(FilePath!)!)
        .Where(path => System.IO.Path.GetFileName(path).StartsWith(System.IO.Path.GetFileName(FilePath) + ".", StringComparison.Ordinal))
        .Select(path => (Path: path, Suffix: path[(FilePath!.Length + 1)..]))
        .Where(item => Archive.IsMatch(item.Suffix))
        .Select(item => (Number: int.Parse(item.Suffix, CultureInfo.InvariantCulture), item.Path))
        .Select(item => (File.GetAttributes(item.Path) & (FileAttributes.Directory | FileAttributes.ReparsePoint)) != 0
            ? throw new IOException() : item)
        .OrderByDescending(item => item.Number).ToArray();

    private void Emit(Dictionary<string, object?> record)
    {
        var encoded = JsonSerializer.Serialize(record);
        var line = encoded + "\n";
        if (LogFormat == "human")
        {
            using var shared = JsonDocument.Parse(encoded);
            line = HumanRecord(shared.RootElement);
        }
        var bytes = Utf8.GetBytes(line);
        lock (_gate)
        {
            if (_stream is not null)
            {
                var writer = _stream == "stderr" ? Console.Error : Console.Out;
                writer.Write(line); writer.Flush(); return;
            }
            var lockPath = FilePath + ".lock";
            FileStream? ownership = null;
            var timer = Stopwatch.StartNew();
            while (ownership is null)
            {
                try { ownership = new FileStream(lockPath, FileMode.CreateNew, FileAccess.Write, FileShare.None); }
                // The owner may release between CreateNew failing and an
                // existence check. Preserve the OS's already-exists result.
                catch (IOException error) when (timer.Elapsed < TimeSpan.FromSeconds(30)
                    && ((error.HResult & 0xffff) is 17 or 80 or 183 || File.Exists(lockPath))) { Thread.Sleep(5); }
            }
            try
            {
                CheckPresentation(); // A mismatch cannot prune or rotate existing files.
                var archives = Archives();
                foreach (var archive in archives.Where(item => item.Number > MaxFiles)) File.Delete(archive.Path);
                var active = new FileInfo(FilePath!);
                if (Directory.Exists(FilePath)) throw new IOException();
                if (active.Exists && active.Length > 0 && (bytes.LongLength > MaxSize - active.Length || active.LastWriteTimeUtc.Date != DateTime.UtcNow.Date))
                {
                    foreach (var archive in archives.Where(item => item.Number <= MaxFiles))
                        if (archive.Number == MaxFiles) File.Delete(archive.Path);
                        else File.Move(archive.Path, FilePath + "." + (archive.Number + 1).ToString(CultureInfo.InvariantCulture));
                    if (MaxFiles == 0) File.Delete(FilePath!); else File.Move(FilePath!, FilePath + ".1");
                }
                // Reopen after every writer's rotation while ownership is held.
                using var destination = new FileStream(FilePath!, FileMode.Append, FileAccess.Write, FileShare.Read);
                destination.Write(bytes);
            }
            finally { ownership.Dispose(); File.Delete(lockPath); }
        }
    }

    private static string HumanText(string value, bool escapeBackslash)
    {
        var output = new StringBuilder();
        foreach (var c in value)
            if (escapeBackslash && c == '\\') output.Append("\\\\");
            else if (char.IsControl(c) || c is '\u2028' or '\u2029')
                output.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
            else output.Append(c);
        return output.ToString();
    }

    private static string HumanRecord(JsonElement record)
    {
        string Text(string key) => HumanText(record.GetProperty(key).GetString()!, true);
        var output = new StringBuilder().Append(Text("timestamp")).Append(" [TeaQL][")
            .Append(Text("log_name")).Append("][").Append(Text("level")).Append("] ")
            .Append(Text("event")).Append(" runtime=").Append(Text("runtime"))
            .Append("\n  ").Append(Text("message")).Append('\n');
        foreach (var field in record.EnumerateObject().Where(field => !EventFields.Contains(field.Name)).OrderBy(field => field.Name, StringComparer.Ordinal))
        {
            if (field.Name == "sql" && field.Value.ValueKind == JsonValueKind.String)
            {
                output.Append("  sql:\n");
                foreach (var line in field.Value.GetString()!.Split('\n')) output.Append("    ").Append(HumanText(line, true)).Append('\n');
            }
            else output.Append("  ").Append(HumanText(field.Name, true)).Append('=')
                .Append(HumanText(field.Value.GetRawText(), false)).Append('\n');
        }
        return output.ToString();
    }

    private void CheckPresentation()
    {
        if (Directory.Exists(FilePath)) throw new IOException();
        var active = new FileInfo(FilePath!);
        if (!active.Exists || active.Length == 0) return;
        using var input = new StreamReader(FilePath!, Utf8, detectEncodingFromByteOrderMarks: false);
        var first = input.ReadLine();
        if (first is not null && LogFormat == "human" && HumanHeader.IsMatch(first)) return;
        if (first is not null && LogFormat == "json")
        {
            try
            {
                using var document = JsonDocument.Parse(first);
                var record = document.RootElement;
                if (record.ValueKind == JsonValueKind.Object
                    && EventFields.All(key => record.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String)
                    && record.GetProperty("level").GetString() is "INFO" or "WARN" or "ERROR") return;
            }
            catch (JsonException) { }
        }
        throw new IOException("incompatible_log_format");
    }
}

internal sealed class RuntimeDiagnosticSqlLogSink(RuntimeLogger logger) : IDiagnosticSqlLogSink
{
    public void Write(ExecutionMetadata metadata)
    {
        if (metadata.Statements.Count > 0) { foreach (var statement in metadata.Statements) Write(statement); return; }
        var safe = LogPrivacy.Project(metadata);
        var outcome = safe.ExecutionOutcome ?? "unknown";
        logger.Write(outcome is "failure" or "error" ? "ERROR" : outcome == "retrying" ? "WARN" : "INFO", "sql." + outcome, "TeaQL SQL execution",
            new Dictionary<string, object?> { ["category"] = "SQL", ["outcome"] = outcome, ["sql"] = safe.DebugQuery,
                ["comment"] = safe.Comment, ["purpose"] = safe.Purpose, ["audit_reason"] = safe.AuditReason,
                ["trace_chain"] = safe.RequestTraceChain, ["trace_path"] = safe.TraceChain, ["mutation_lineage"] = safe.MutationLineage,
                ["result_count"] = safe.ResultCount, ["affected_rows"] = safe.AffectedRows, ["omission_reason"] = safe.SqlOmissionReason,
                ["elapsed_us"] = (long)((safe.EndedAt - safe.StartedAt).TotalMicroseconds), ["trace_id"] = safe.SqlCorrelation?.TraceId });
    }
}

internal sealed class LocalLogTelemetry(IRuntimeTelemetry telemetry, RuntimeLogger logger) : IRuntimeTelemetry
{
    public IRuntimeTelemetryScope Start(RuntimeOperation operation) => new Scope(telemetry.StartSafely(operation), operation, logger);
    public IDisposable ExtractAndActivate(IReadOnlyDictionary<string, string> carrier) => telemetry.ExtractAndActivate(carrier);
    public Task FlushAsync() => telemetry.FlushAsync();
    public Task ShutdownAsync() => telemetry.ShutdownAsync();
    private sealed class Scope(IRuntimeTelemetryScope original, RuntimeOperation operation, RuntimeLogger logger) : IRuntimeTelemetryScope
    {
        private int _ended;
        public void Success(IReadOnlyDictionary<string, object>? attributes = null)
        {
            if (Interlocked.Exchange(ref _ended, 1) != 0) return;
            Log(null); original.Success(attributes);
        }
        public void Failure(Exception error) => Failure(error, null);
        public void Failure(Exception error, IReadOnlyDictionary<string, object>? attributes)
        {
            if (Interlocked.Exchange(ref _ended, 1) != 0) return;
            Log(error); original.Failure(error, attributes);
        }
        private void Log(Exception? error)
        {
            var category = error is null ? null : RuntimeErrorClassifier.Category(error);
            if (error is CheckException or TeaQL.Core.RequestIntentException) category = "validation";
            if (error is MutationPolicyException) category = "authorization";
            var outcome = error is null ? "success" : error is OperationCanceledException ? "cancelled" : "failure";
            logger.Write(error is null ? "INFO" : category is "authorization" or "validation" or "conflict" ? "WARN" : "ERROR", "operation." + outcome, "TeaQL operation " + outcome,
                new Dictionary<string, object?> { ["category"] = operation.Family.ToUpperInvariant(), ["outcome"] = outcome,
                    ["error_code"] = category, ["trace_id"] = Activity.Current?.TraceId.ToString() });
        }
    }
}
