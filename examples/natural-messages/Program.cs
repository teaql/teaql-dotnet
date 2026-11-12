using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

static void Require(bool value, string reason) { if (!value) throw new InvalidOperationException(reason); }
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder {
    DataSource = Environment.GetEnvironmentVariable("TEAQL_NATURAL_MESSAGES_DB") ?? ":memory:"
}.ToString());
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
module.ContextField("Entry", "source_ip", create: "ip").ContextField("Entry", "last_ip", create: "ip", update: "ip")
    .ContextField("Entry", "opened_by", create: "actor").ContextField("Entry", "changed_by", create: "actor", update: "actor");
// Passive application metadata until automatic generator emission is qualified.
module.CheckerPresentation(CheckerPresentationModel.FromJson("""
    {"schema":"teaql.checker-model/v1","revision":"natural-sqlite-v1","entities":{
      "Catalog":{"labels":{"en":"catalog","zh-CN":"目录"},"fields":[
        {"name":"name","labels":{"en":"name","zh-CN":"名称"}},
        {"name":"entry_list","targetType":"Entry","labels":{"en":"entry","zh-CN":"明细"},"classifiers":{"zh-CN":"条"},"aliases":["EntryList"]}]},
      "Entry":{"labels":{"en":"entry","zh-CN":"明细"},"fields":[
        {"name":"name","labels":{"en":"name","zh-CN":"名称"}},
        {"name":"guarded_token","labels":{"en":"private token","zh-CN":"私密标记"}}]}}}
    """));
var checkers = (Dictionary<string,IEntityChecker>)typeof(RuntimeModule).GetProperty("Checkers", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(module)!;
module.Checker("Entry", new AppChecker(checkers["Entry"]));
var transport = new ObservedTransport(new SqliteTransport(connection)); var audit = new Audit();
var context = module.IntoContext().WithBusinessClock(new FixedBusinessClock(DateTimeOffset.Parse("2032-02-29T10:15:30Z")))
    .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module))).WithAppAuditEventSink(audit);
context.InsertResource<IContextFixResolver>(new Resolver());
await context.EnsureSchemaAsync();
var suffix = Guid.NewGuid().ToString("N")[..8];
var parent = Q.Catalogs().Comment("Prepare a natural-message graph").Purpose("Verify generated mutation diagnostics").NewEntity(context).UpdateName("graph-" + suffix);
Entry Child(string name) => Q.Entries().Comment("Prepare an entry").Purpose("Verify natural Checker messages").NewEntity(context).UpdateName(name);
parent.EntryList.Add(Child("first-" + suffix)); parent.EntryList.Add(Child("second-" + suffix));
await parent.AuditAs("Create the natural-message example graph").SaveAsync(context);
async Task<Catalog> Load() => (await Q.Catalogs().WithIdIs(parent.Id!.Value)
    .SelectEntryListWith(Q.Entries().OrderByIdAscending().Limit(10)).Limit(1)
    .Comment("Load the complete natural-message graph").Purpose("Use Q and E to verify stored values").ExecuteForListAsync(context)).Single();
parent = await Load(); var first = parent.EntryList[0]; var second = parent.EntryList[1];
Require(E.Entry(first).Name().Eval() == "first-" + suffix && E.Entry(second).Name().Eval() == "second-" + suffix, "Q/E hydration");
var parentVersion = parent.Version; var firstVersion = first.Version; var secondVersion = second.Version;
const string secret = "密钥🔑abcdef"; second.UpdateGuardedToken(secret);
transport.Reset(); audit.Calls = 0;
CheckException? failure = null;
try { await parent.AuditAs("Reject a private value on the second child").SaveAsync(context); }
catch (CheckException error) { failure = error; }
Require(failure != null, "Expected a located validation error");
var violation = failure!.Violations.Single();
Require(violation.ModelPath == "entry_list[1].guarded_token" && violation.InstancePath == "/entryList/1/guardedToken", "Canonical/wire location");
Require(violation.EntityType == "Entry", "Declaring type is not the graph origin");
const string english = "The private token of the 2nd entry must not exceed 8 UTF-16 code units; its current length is 10.";
Require(violation.Message == english, "Natural graph message and original UTF-16 length");
Require(!failure.ToString().Contains(secret) && !JsonSerializer.Serialize(violation.ToWire()).Contains(secret), "Private value cannot enter diagnostics");
Require(transport.Calls == 0 && audit.Calls == 0, "Rejected graph cannot query, begin, write or audit");
Require(parent.Version == parentVersion && first.Version == firstVersion && second.Version == secondVersion, "Rejected versions unchanged");
Console.WriteLine("DOTNET_NATURAL_EN=" + JsonSerializer.Serialize(violation.ToWire()));
context.SetLocaleCode("zh-CN").TranslateCheckResults(failure.Violations);
Require(violation.Message == "第2条明细的私密标记最多允许8个 UTF-16 代码单元，当前长度为10。", "Chinese graph grammar");
Console.WriteLine("DOTNET_NATURAL_ZH=" + JsonSerializer.Serialize(violation.ToWire()));
context.SetLocaleCode("en"); second.UpdateGuardedToken("safe-B");
transport.Reset(); audit.Calls = 0;
await parent.AuditAs("Save the corrected second child only").SaveAsync(context);
Require(audit.Calls == 1 && transport.Writes == 1, "Only the changed child writes and audits");
var reloaded = await Load();
Require(reloaded.Version == parentVersion && reloaded.EntryList[0].Version == firstVersion && reloaded.EntryList[1].Version == secondVersion + 1, "Only modified version advances");
Require(E.Entry(reloaded.EntryList[1]).GuardedToken().Eval() == "safe-B", "Corrected field persisted");
Console.WriteLine("DOTNET_NATURAL_SQLITE=" + JsonSerializer.Serialize(new {
    passed=true, typedQ=true, typedE=true, originalLength=10, unchangedParent=true,
    failedCheckProviderCalls=0, correctedWrites=1, correctedAudits=1, english, pointer=violation.InstancePath,
    parentId=parent.Id, firstId=first.Id, secondId=second.Id, parentVersion, firstVersion, secondVersion=reloaded.EntryList[1].Version }));

sealed class AppChecker(IEntityChecker generated) : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        if (request is InsertMutationRequest) {
            ContextFix.Apply(context, request, now, "source_ip", "ip");
            ContextFix.Apply(context, request, now, "opened_by", "actor");
        }
        ContextFix.Apply(context, request, now, "last_ip", "ip");
        ContextFix.Apply(context, request, now, "changed_by", "actor");
        var results = generated.CheckAndFix(context, request, now).ToList();
        var values = request is InsertMutationRequest insert ? insert.Command.Values : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        if (values.TryGetValue("guarded_token", out var token) && token.Raw is string text && text.Length > 8)
            results.Add(new CheckResult { RuleId="max_length", EntityType="Entry", Location=ObjectLocation.Property("guarded_token"), SystemValue=8 });
        return results;
    }
}
sealed class Resolver : IContextFixResolver
{
    public Value Resolve(UserContext context, ContextFixInput input) => input.Function == "now"
        ? new Value.TimestampValue(input.Now.ToUnixTimeMilliseconds()) : new Value.TextValue(input.Function == "uniStr" ? "seed-A" : input.Function + "-A");
}
sealed class Schema(RuntimeModule module) : ISchemaProvider { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
sealed class Audit : IAppAuditEventSink
{
    public int Calls;
    public Task RecordAsync(IReadOnlyDictionary<string,object?> value, CancellationToken token=default) { Calls++; return Task.CompletedTask; }
}
sealed class ObservedTransport(SqliteTransport actual) : IAutomaticMutationTransactionTransport
{
    public int Calls, Writes;
    public void Reset() { Calls=0; Writes=0; }
    public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Calls++; return actual.FetchAllSqlAsync(query); }
    public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Calls++; Writes++; return actual.ExecuteSqlAsync(query); }
    public async Task<ISqlTransaction> BeginSqlAsync() { Calls++; return new Transaction(await actual.BeginSqlAsync(), this); }
    sealed class Transaction(ISqlTransaction inner, ObservedTransport owner) : ISqlTransaction
    {
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { owner.Calls++; return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { owner.Calls++; owner.Writes++; return inner.ExecuteSqlAsync(query); }
        public Task CommitSqlAsync() { owner.Calls++; return inner.CommitSqlAsync(); }
        public Task RollbackSqlAsync() { owner.Calls++; return inner.RollbackSqlAsync(); }
        public void Dispose() => inner.Dispose();
    }
}
