using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

[Collection("Log privacy environment")]
public sealed class RuntimeLoggingPresentationsTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "teaql-log-presentations-" + Guid.NewGuid().ToString("N"));
    public RuntimeLoggingPresentationsTests() => Directory.CreateDirectory(_directory);
    public void Dispose() => Directory.Delete(_directory, true);
    private RuntimeLogger New(string format, Dictionary<string, string>? env = null, string? directory = null) => new(new()
    { LogFormat = format, Environment = env ?? new(), WorkingDirectory = directory ?? _directory });
    private static readonly Regex Header = new(@"^(\S+) \[TeaQL\]\[(.*)\]\[(INFO|WARN|ERROR)\] (.*) runtime=(\S+)$");
    private static string Scalar(string value) => JsonSerializer.Deserialize<string>("\"" + value.Replace("\"", "\\\"") + "\"")!;
    private static JsonObject[] Parse(string text, string format)
    {
        Assert.EndsWith("\n", text); var rows = new List<JsonObject>(); List<string>? sql = null;
        foreach (var line in text[..^1].Split('\n'))
        {
            if (format == "json") { rows.Add(JsonNode.Parse(line)!.AsObject()); continue; }
            Assert.False(line.Any(c => char.IsControl(c) || c is '\u2028' or '\u2029'));
            var header = Header.Match(line);
            if (header.Success)
            {
                rows.Add(new() { ["timestamp"] = header.Groups[1].Value, ["log_name"] = Scalar(header.Groups[2].Value),
                    ["level"] = header.Groups[3].Value, ["event"] = Scalar(header.Groups[4].Value), ["runtime"] = header.Groups[5].Value }); sql = null;
            }
            else
            {
                Assert.NotEmpty(rows); Assert.StartsWith("  ", line); var row = rows[^1];
                if (!row.ContainsKey("message")) row["message"] = Scalar(line[2..]);
                else if (line == "  sql:") { sql = []; row["sql"] = ""; }
                else if (line.StartsWith("    ", StringComparison.Ordinal) && sql is not null) { sql.Add(Scalar(line[4..])); row["sql"] = string.Join('\n', sql); }
                else { var parts = line[2..].Split('=', 2); Assert.Equal(2, parts.Length); row[Scalar(parts[0])] = JsonNode.Parse(parts[1]); sql = null; }
            }
        }
        foreach (var row in rows)
        {
            foreach (var key in new[] { "timestamp", "level", "log_name", "runtime", "event", "message" }) Assert.NotNull(row[key]!.GetValue<string>());
            Assert.Equal(TimeSpan.Zero, DateTimeOffset.Parse(row["timestamp"]!.GetValue<string>(), CultureInfo.InvariantCulture).Offset);
        }
        return rows.ToArray();
    }
    private static JsonObject[] Read(string format, params string[] paths) => paths.SelectMany(path => Parse(File.ReadAllText(path), format)).ToArray();

    [Theory][InlineData("json")][InlineData("human")]
    public void SelectionNamesOverrideFrozenContextsAndTwoStarts(string format)
    {
        var defaults = new RuntimeLogger(new() { Environment = new Dictionary<string,string>(), WorkingDirectory = Path.Combine(_directory,"default") }); Assert.Equal("json", defaults.LogFormat);
        var cases = new (string? App,string Domain,string[] Models,string Name)[] {
            ("application","domain",["model"],"application"),(null,"domain",["model"],"domain"),(null,"",["model"],"model"),(null,"",[],"teaql"),(null,"",["one","two"],"teaql") };
        for (int i=0;i<cases.Length;i++)
        {
            var test=cases[i]; var directory=Path.Combine(_directory,i.ToString());var env=new Dictionary<string,string> { ["TEAQL_DOMAIN"]=test.Domain,["TEAQL_LOG_FORMAT"]=format };
            var log=new RuntimeLogger(new() { LogName=test.App,ModelNames=test.Models,Environment=env,WorkingDirectory=directory });
            Assert.Equal(format,log.LogFormat); Assert.Equal(test.Name,log.LogName);Assert.Equal(Path.Combine(directory,"logs",test.Name+".log"),log.FilePath);
            env["TEAQL_LOG_FORMAT"]="invalid-unused";env["TEAQL_DOMAIN"]="changed";var context=new UserContext(log);Assert.Same(log,context.Logger);
            Assert.True(log.Write("INFO","frozen","startup-owned"));new RuntimeLogger(new() { LogName=test.Name,LogFormat=format,Environment=new Dictionary<string,string>(),WorkingDirectory=directory });
            Assert.Equal(3,Read(format,log.FilePath!).Length);
            var other=New(format,new() { ["TEAQL_LOG_FORMAT"]="invalid-unused",["TEAQL_LOG_ENDPOINT"]="override/custom.log" },directory);
            Assert.Equal(Path.Combine(directory,"override","custom.log"),other.FilePath);Assert.Equal(format,other.LogFormat);
        }
    }

    [Theory][InlineData("json")][InlineData("human")]
    public void InvalidFormatsFailWithoutEchoAndAppWins(string format)
    {
        var saved=Console.Error;using var capture=new StringWriter();
        try
        {
            Console.SetError(capture);
            foreach(var value in new[] { "","debug","JSON","human\nFORMAT-CANARY" }) foreach(var app in new[] {false,true})
                Assert.Throws<InvalidOperationException>(() => new RuntimeLogger(new() { LogFormat=app?value:null,Environment=app?new Dictionary<string,string>():new Dictionary<string,string> { ["TEAQL_LOG_FORMAT"]=value },WorkingDirectory=_directory }));
        }
        finally { Console.SetError(saved); }
        Assert.DoesNotContain("CANARY",capture.ToString());Assert.All(Parse(capture.ToString(),"json"),row=>Assert.Equal("ERROR",row["level"]!.GetValue<string>()));
        Assert.Empty(Directory.GetFileSystemEntries(_directory));Assert.Equal(format,New(format,new() { ["TEAQL_LOG_FORMAT"]="invalid-unused" }).LogFormat);
    }

    [Theory][InlineData("json")][InlineData("human")]
    public void StreamsKeepProjectedTypesAndEscapeHumanControlCharacters(string format)
    {
        foreach(var endpoint in new[] {"stdout","stderr"})
        {
            var saved=endpoint=="stdout"?Console.Out:Console.Error;using var capture=new StringWriter();
            var fields=new Dictionary<string,object?> { ["category"]="SQL",["trace_id"]="0123456789abcdef0123456789abcdef",["outcome"]="retrying",["error_code"]="connection_timeout",
                ["entity_id"]=9007199254740993L,["enabled"]=true,["result_count"]=0,["omission_reason"]=null,["changes"]=new[]{new {field="password",value="***"}},["sql"]="SELECT id\nFROM customer\n\u001b\r\u0085\u2028" };
            const string message="Readable \\u001b\nFORGED\u001b\r\u0085\u2029";
            try {if(endpoint=="stdout")Console.SetOut(capture);else Console.SetError(capture);var log=New(format,new() { ["TEAQL_LOG_ENDPOINT"]=endpoint });Assert.True(log.Write("WARN","unknown.kind",message,fields));}
            finally {if(endpoint=="stdout")Console.SetOut(saved);else Console.SetError(saved);}
            var rows=Parse(capture.ToString(),format);Assert.Equal(2,rows.Length);rows[1].Remove("timestamp");
            foreach(var (key,value) in new Dictionary<string,object?> { ["level"]="WARN",["log_name"]="teaql",["runtime"]="dotnet",["event"]="unknown.kind",["message"]=message }) fields[key]=value;
            Assert.True(JsonNode.DeepEquals(JsonSerializer.SerializeToNode(fields),rows[1]));Assert.Empty(Directory.GetFileSystemEntries(_directory));
        }
    }

    [Theory][InlineData("json")][InlineData("human")]
    public async Task NativeOutcomesPrivacyCorrelationAndTypedAudit(string format)
    {
        var log=New(format);var context=new UserContext(log);using var activity=new Activity("probe").SetIdFormat(ActivityIdFormat.W3C).Start();
        Assert.Equal(42,await context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query","probe"),()=>Task.FromResult(42)));
        foreach(var error in new Exception[] {new MutationPolicyException("POLICY-CANARY"),new IOException("DRIVER-CANARY")})
            Assert.Same(error,await Assert.ThrowsAsync(error.GetType(),()=>context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query","probe"),()=>Task.FromException<int>(error))));
        var rows=Read(format,log.FilePath!);Assert.Equal(new[]{"INFO","INFO","WARN","ERROR"},rows.Select(r=>r["level"]!.GetValue<string>()));
        var previous=Environment.GetEnvironmentVariable("TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS");
        try {foreach(var ack in new[]{"","true","I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK"})
        {
            Environment.SetEnvironmentVariable("TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS",ack);
            var metadata=new ExecutionMetadata {Operation=DataServiceOperation.Query,ExecutionOutcome="retrying",ParameterizedQuery="SELECT id FROM customer WHERE password = ?",
                Parameters=new Value[]{new Value.TextValue("PASSWORD-CANARY")},DebugQuery="PASSWORD-CANARY",Comment="read PASSWORD-CANARY",SqlCorrelation=new SqlExecutionCorrelation(activity.TraceId.ToString(),activity.SpanId.ToString())};
            context.DiagnosticSqlLogSink!.Write(metadata);Assert.Equal("PASSWORD-CANARY",metadata.Parameters[0].TryText());
        }} finally {Environment.SetEnvironmentVariable("TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS",previous);}
        await context.PublishAppAuditEventAsync(new Dictionary<string,object?> { ["entityType"]="Customer",["mutationKind"]="update",["entity_id"]=9007199254740993L,["fields"]=new {password="***"},["reason"]="projected audit" });
        rows=Read(format,log.FilePath!);var sql=rows.Where(r=>r["category"]?.GetValue<string>()=="SQL").ToArray();Assert.Equal(3,sql.Length);
        Assert.All(sql,row=>{Assert.Equal("WARN",row["level"]!.GetValue<string>());Assert.Equal(activity.TraceId.ToString(),row["trace_id"]!.GetValue<string>());});
        var audit=Assert.Single(rows.Where(r=>r["event"]!.GetValue<string>()=="audit.event"));Assert.Equal(9007199254740993L,audit["audit"]!["entity_id"]!.GetValue<long>());
        Assert.DoesNotContain("CANARY",File.ReadAllText(log.FilePath!));
    }

    [Theory][InlineData("json")][InlineData("human")]
    public void PositiveRetentionKeepsNumericNewestOrderAndWholeBlocks(string format)
    {
        var log=New(format,new() { ["TEAQL_LOG_MAX_SIZE"]="256",["TEAQL_LOG_MAX_FILES"]="2" });
        for(var i=0;i<5;i++)Assert.True(log.Write("INFO","retention",new string('界',200),new Dictionary<string,object?> { ["sequence"]=i,["sql"]="SELECT\n"+i }));
        for(var i=0;i<3;i++){var row=Assert.Single(Read(format,log.FilePath+(i==0?"":"."+i)));Assert.Equal(4-i,row["sequence"]!.GetValue<int>());Assert.Equal("SELECT\n"+(4-i),row["sql"]!.GetValue<string>());}
        Assert.Equal(3,Directory.GetFiles(Path.GetDirectoryName(log.FilePath!)!).Length);Assert.False(File.Exists(log.FilePath+".3"));
    }

    [Theory][InlineData("json")][InlineData("human")]
    public void WholeUtf8EventsSizeDayRestartRetentionOversizeAndOverride(string format)
    {
        foreach(var other in new[]{false,true})
        {
            var directory=Path.Combine(_directory,other.ToString());var env=new Dictionary<string,string> { ["TEAQL_LOG_MAX_SIZE"]="1K",["TEAQL_LOG_MAX_FILES"]="100" };if(other)env["TEAQL_LOG_ENDPOINT"]="override/custom.log";
            var log=New(format,env,directory);for(var index=0;index<40;index++)Assert.True(log.Write("INFO","regular",new string('界',25),new Dictionary<string,object?> { ["index"]=index,["sql"]="SELECT id\nFROM customer" }));
            var files=Directory.GetFiles(Path.GetDirectoryName(log.FilePath!)!);Assert.True(files.Length>1);Assert.All(files,file=>Assert.InRange(new FileInfo(file).Length,1,1024));
            var rows=Read(format,files);Assert.Equal(41,rows.Length);Assert.Equal(Enumerable.Range(0,40),rows.Where(r=>r.ContainsKey("index")).Select(r=>r["index"]!.GetValue<int>()).Order());
            Assert.All(rows.Where(r=>r.ContainsKey("index")),row=>Assert.Equal("SELECT id\nFROM customer",row["sql"]!.GetValue<string>()));
            Assert.True(log.Write("WARN","oversize","whole",new Dictionary<string,object?> { ["sql"]=new string('界',2000)+"\nLAST" }));var before=File.ReadAllBytes(log.FilePath!);Assert.True(before.Length>1024);Assert.Single(Read(format,log.FilePath!));
            Assert.True(log.Write("INFO","after","next"));Assert.Equal(before,File.ReadAllBytes(log.FilePath+".1"));
            foreach(var restart in new[]{false,true}) {File.SetLastWriteTimeUtc(log.FilePath!,DateTime.UtcNow.AddDays(-2));before=File.ReadAllBytes(log.FilePath!);if(restart)New(format,env,directory);else Assert.True(log.Write("INFO","day","UTC rotation"));Assert.Equal(before,File.ReadAllBytes(log.FilePath+".1"));Assert.Single(Read(format,log.FilePath!));}
            env["TEAQL_LOG_MAX_FILES"]="0";var zero=New(format,env,directory);Assert.True(zero.Write("INFO","last",new string('界',2000)));Assert.Single(Directory.GetFiles(Path.GetDirectoryName(zero.FilePath!)!));Assert.Single(Read(format,zero.FilePath!));
        }
    }

    [Theory][InlineData("json")][InlineData("human")]
    public void ConcurrentInstancesKeepMultilineBlocksWhole(string format)
    {
        var loggers=Enumerable.Range(0,4).Select(_=>New(format,new() { ["TEAQL_LOG_MAX_SIZE"]="2K",["TEAQL_LOG_MAX_FILES"]="100" })).ToArray();
        Parallel.For(0,120,index=>Assert.True(loggers[index%4].Write("INFO","parallel","多线程",new Dictionary<string,object?> { ["index"]=index,["sql"]="SELECT\n"+index })));
        var rows=Read(format,Directory.GetFiles(Path.GetDirectoryName(loggers[0].FilePath!)!));Assert.Equal(124,rows.Length);
        Assert.Equal(Enumerable.Range(0,120),rows.Where(r=>r.ContainsKey("index")).Select(r=>r["index"]!.GetValue<int>()).Order());
        Assert.All(rows.Where(r=>r.ContainsKey("index")),row=>Assert.Equal("SELECT\n"+row["index"]!.GetValue<int>(),row["sql"]!.GetValue<string>()));
    }

    [Theory][InlineData("json")][InlineData("human")]
    public async Task MixedFormatAndIoFailuresPreserveFilesAndBusiness(string format)
    {
        var log=New(format);var other=format=="json"?"human":"json";var archive=log.FilePath+".10";File.WriteAllText(archive,"preserve archive bytes");var before=File.ReadAllBytes(log.FilePath!);
        Assert.Throws<InvalidOperationException>(()=>New(other,new() { ["TEAQL_LOG_MAX_SIZE"]="1",["TEAQL_LOG_MAX_FILES"]="0" }));Assert.Equal(before,File.ReadAllBytes(log.FilePath!));Assert.Equal("preserve archive bytes",File.ReadAllText(archive));
        var replacement=New(other,directory:Path.Combine(_directory,"replacement"));before=File.ReadAllBytes(replacement.FilePath!);File.WriteAllBytes(log.FilePath!,before);
        var context=new UserContext(log);Assert.Equal(42,await context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query","probe"),()=>Task.FromResult(42)));
        var error=new IOException("SECRET-BUSINESS-ERROR");Assert.Same(error,await Assert.ThrowsAsync<IOException>(()=>context.OperationTelemetry.ObserveAsync<int>(RuntimeOperation.Create("query","probe"),()=>Task.FromException<int>(error))));
        Assert.False(log.Write("INFO","later","business completion"));Assert.Equal(before,File.ReadAllBytes(log.FilePath!));Assert.Equal("preserve archive bytes",File.ReadAllText(archive));
        File.Delete(log.FilePath!);Directory.CreateDirectory(log.FilePath!);Assert.False(log.Write("ERROR","later","I/O failure"));Assert.Throws<InvalidOperationException>(()=>New(format));
    }
}
