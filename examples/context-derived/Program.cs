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

static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
var now = DateTimeOffset.Parse("2032-02-29T10:15:30Z");
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder {
    DataSource = Environment.GetEnvironmentVariable("TEAQL_CONTEXT_FIX_DB") ?? ":memory:"
}.ToString());
await connection.OpenAsync();
var transport = new ObservedTransport(new SqliteTransport(connection));
var module = GeneratedRuntimeModule.Module;
module.ContextField("Entry", "source_ip", create: "ip")
    .ContextField("Entry", "last_ip", create: "ip", update: "ip")
    .ContextField("Entry", "opened_by", create: "actor")
    .ContextField("Entry", "changed_by", create: "actor", update: "actor");
// Decorate the actual generated checker, not a replacement implementation.
var registry = (Dictionary<string, IEntityChecker>)typeof(RuntimeModule).GetProperty("Checkers", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(module)!;
var wrapper = new Checker(registry["Entry"]); module.Checker("Entry", wrapper);
var audit = new Audit(); var resolver = new Resolver();
var context = module.IntoContext().WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module)))
    .WithBusinessClock(new FixedBusinessClock(now)).WithAppAuditEventSink(audit);
context.InsertResource<IContextFixResolver>(resolver);
await context.EnsureSchemaAsync();
var suffix = Guid.NewGuid().ToString("N")[..10];
Entry New(string name) => Q.Entries().Comment("Prepare a Context Fix subject")
    .Purpose("Verify generated typed mutation boundaries").NewEntity(context).UpdateCatalogId(1).UpdateName(name);
async Task<Entry> Load(long id) => (await Q.Entries().WithIdIs(id).Limit(1)
    .Comment("Reload the persisted Context Fix subject").Purpose("Assert real SQLite values using E").ExecuteForListAsync(context)).Single();
async Task<CheckException> Reject(Func<Task> work) {
    try { await work(); } catch (CheckException error) { return error; }
    throw new InvalidOperationException("Expected a rejected save");
}
void Zero(string label) => Require(transport.Calls == 0 && audit.Calls == 0, label + " must not call provider/audit");
void Reset() { transport.Calls = 0; audit.Calls = 0; resolver.Calls = 0; }

var entry = New("initial-" + suffix);
await entry.AuditAs("Create with Context defaults").SaveAsync(context);
var loaded = await Load(entry.Id!.Value);
Require(E.Entry(loaded).BornAt().Eval() == now.UtcDateTime && E.Entry(loaded).TouchedAt().Eval() == now.UtcDateTime, "Business Clock create values");
Require(E.Entry(loaded).SourceIp().Eval() == "ip-A" && E.Entry(loaded).OpenedBy().Eval() == "actor-A", "create Context values");
Require(resolver.TokenCalls == 1, "unique create function must run once");
var version = loaded.Version; var token = loaded.GuardedToken;
resolver.Phase = "B"; context.WithBusinessClock(new FixedBusinessClock(now.AddDays(1)));
Reset();
for (var attempt = 0; attempt < 2; attempt++) await loaded.AuditAs("Do not turn changed Context into an update").SaveAsync(context);
Zero("clean entity"); Require(resolver.Calls == 0 && loaded.Version == version, "no-op changed Context");
wrapper.NormalizeTo = loaded.Name!;
loaded.UpdateName("NORMALIZE"); Reset();
await loaded.AuditAs("Normalize the last business edit back to its persisted value").SaveAsync(context);
Zero("normalized business edit"); Require(loaded.Version == version && loaded.Name == wrapper.NormalizeTo, "normalized native values/version");
Require(E.Entry(loaded).TouchedAt().Eval() == now.UtcDateTime && loaded.LastIp == "ip-A", "normalized Context values");
loaded.UpdateName("updated-" + suffix); Reset(); var clocksBeforeUpdate = resolver.NowCalls;
await loaded.AuditAs("Apply Context only alongside a real business edit").SaveAsync(context);
Require(audit.Calls == 1 && resolver.NowCalls == clocksBeforeUpdate + 1, "one update and once-only Fix");
loaded = await Load(loaded.Id!.Value);
Require(loaded.Version == version + 1 && E.Entry(loaded).TouchedAt().Eval() == now.AddDays(1).UtcDateTime, "real update version/clock");
Require(E.Entry(loaded).BornAt().Eval() == now.UtcDateTime && loaded.SourceIp == "ip-A" && loaded.OpenedBy == "actor-A", "preserve create-only Context");
Require(loaded.LastIp == "ip-B" && loaded.ChangedBy == "actor-B" && loaded.GuardedToken == token, "update-only scoped fields");

var before = loaded.Name; var beforeVersion = loaded.Version;
loaded.UpdateName(new string('x', 300)); resolver.Phase = "C"; Reset();
var rejected = await Reject(async () => await loaded.AuditAs("Reject an invalid business edit").SaveAsync(context));
Require(rejected.Violations.Any(v => v.ModelPath == "name"), "business error ObjectLocation"); Zero("rejected check");
Require(loaded.LastIp == "ip-B" && loaded.Version == beforeVersion, "rejected Context cannot alter native entity");
loaded.UpdateName(before!); Reset();
await loaded.AuditAs("Undo the failed edit without retaining automatic Context intent").SaveAsync(context);
Zero("restored business edit"); Require(resolver.Calls == 0, "failed Fix must not poison a no-op retry");
loaded.UpdateName("retry-" + suffix); Reset();
await loaded.AuditAs("Retry a corrected business edit").SaveAsync(context);
loaded = await Load(loaded.Id!.Value); Require(loaded.LastIp == "ip-C", "retry resolved Context");

var failing = New("provider-fail-" + suffix); resolver.FailToken = true; Reset();
var providerError = await Reject(async () => await failing.AuditAs("Reject a failed automatic provider").SaveAsync(context));
Require(providerError.Violations.Any(v => v.ModelPath == "guarded_token"), "safe provider error field");
Require(providerError.InnerException == null && !providerError.ToString().Contains("PRIVATE_PROVIDER_SECRET"), "no raw provider exception");
Zero("failed Context provider"); resolver.FailToken = false;
await failing.AuditAs("Retry after fixing the automatic provider").SaveAsync(context);

var parent = Q.Catalogs().Comment("Prepare a complete parent graph").Purpose("Verify per-node Context undo").NewEntity(context).UpdateName("graph-" + suffix);
var first = New("first-" + suffix); var second = New("second-" + suffix);
parent.EntryList.Add(first); parent.EntryList.Add(second);
await parent.AuditAs("Create a graph with two Context-bearing children").SaveAsync(context);
parent = (await Q.Catalogs().WithIdIs(parent.Id!.Value).SelectEntryListWith(Q.Entries().OrderByIdAscending().Limit(10)).Limit(1)
    .Comment("Load the complete Context regression graph").Purpose("Verify later-child failure and retry").ExecuteForListAsync(context)).Single();
first = parent.EntryList[0]; second = parent.EntryList[1];
var parentVersion = parent.Version; var secondVersion = second.Version;
var firstOriginal = first.Name; var secondOriginal = second.Name;
wrapper.NormalizeTo = firstOriginal!;
first.UpdateName("NORMALIZE"); Reset();
await parent.AuditAs("Normalize one child without dirtying its clean relatives").SaveAsync(context);
Zero("normalized child-only graph");
Require(first.Name == firstOriginal && first.Version == 1 && parent.Version == parentVersion, "normalized graph native state");
first.UpdateName("NORMALIZE"); second.UpdateName("real-sibling-" + suffix); Reset();
await parent.AuditAs("Normalize one child while saving an independently changed sibling").SaveAsync(context);
Require(audit.Calls == 1 && first.Name == firstOriginal && first.Version == 1 && parent.Version == parentVersion,
    "mixed graph must not write the normalized child");
secondVersion = second.Version; secondOriginal = second.Name;
first.UpdateName("NORMALIZE"); second.UpdateName(new string('x', 300)); Reset();
var normalizedGraphError = await Reject(async () => await parent.AuditAs("Reject a later child after an earlier normalized edit").SaveAsync(context));
Require(normalizedGraphError.Violations.Any(v => v.InstancePath == "/entryList/1/name"), "normalized mixed graph location");
Zero("normalized child before invalid sibling");
first.UpdateName(firstOriginal!); second.UpdateName(secondOriginal!); Reset();
await parent.AuditAs("Restore the rejected mixed graph without stale automatic changes").SaveAsync(context);
Zero("restored normalized graph");
first.UpdateName("changed-" + suffix); second.UpdateName(new string('x', 300)); resolver.Phase = "D"; Reset();
var graphError = await Reject(async () => await parent.AuditAs("Reject the second child before writing either child").SaveAsync(context));
Require(graphError.Violations.Any(v => v.InstancePath == "/entryList/1/name"), "indexed graph ObjectLocation"); Zero("invalid graph");
first.UpdateName(firstOriginal!); second.UpdateName(secondOriginal!); Reset();
await parent.AuditAs("Restore both edits after failed graph Fix").SaveAsync(context);
Zero("graph Context undo"); Require(resolver.Calls == 0, "earlier sibling automatic fields must not remain in ledger");
first.UpdateName("accepted-child-" + suffix); Reset();
await parent.AuditAs("Save only one changed child").SaveAsync(context);
Require(audit.Calls == 1 && parent.Version == parentVersion && second.Version == secondVersion, "child-only mutation boundary");
var persisted = await Load(first.Id!.Value);
Require(persisted.Name == "accepted-child-" + suffix && persisted.LastIp == "ip-D", "child retry persisted");
// Independent read-only database oracle; no mutation SQL alternative.
await using var oracle = connection.CreateCommand(); oracle.CommandText = "SELECT name,last_ip,source_ip,version FROM entry_data WHERE id=$id";
oracle.Parameters.AddWithValue("$id", persisted.Id); await using var reader = await oracle.ExecuteReaderAsync();
Require(await reader.ReadAsync() && reader.GetString(0) == persisted.Name && reader.GetString(1) == "ip-D"
    && reader.GetInt64(3) == persisted.Version, "physical SQLite state");
Console.WriteLine("DOTNET_CONTEXT_FIX_SQLITE=" + JsonSerializer.Serialize(new {passed=true, typedQ=true, typedE=true,
    noOpProviderCalls=0, failedCheckProviderCalls=0, updateAudits=1, objectLocation=true, graphRollback=true, fixOnce=true}));

sealed class Checker(IEntityChecker generated) : IEntityChecker
{
    public string NormalizeTo = "";
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now) {
        if (request is InsertMutationRequest) {
            ContextFix.Apply(context, request, now, "source_ip", "ip");
            ContextFix.Apply(context, request, now, "opened_by", "actor");
        }
        ContextFix.Apply(context, request, now, "last_ip", "ip");
        ContextFix.Apply(context, request, now, "changed_by", "actor");
        var results = generated.CheckAndFix(context, request, now);
        if (request is UpdateMutationRequest update && update.Command.Values.TryGetValue("name", out var name)
            && name == new Value.TextValue("NORMALIZE")) update.Command.Values["name"] = new Value.TextValue(NormalizeTo);
        return results;
    }
}
sealed class Resolver : IContextFixResolver
{
    public string Phase = "A"; public bool FailToken; public int Calls, TokenCalls, NowCalls;
    public Value Resolve(UserContext context, ContextFixInput input) {
        Calls++;
        if (input.Function == "now") { NowCalls++; return new Value.TimestampValue(input.Now.ToUnixTimeMilliseconds()); }
        if (input.Function == "uniStr") {
            TokenCalls++; if (FailToken) throw new InvalidOperationException("PRIVATE_PROVIDER_SECRET");
            return new Value.TextValue("token-" + Guid.NewGuid().ToString("N"));
        }
        return new Value.TextValue(input.Function + "-" + Phase);
    }
}
sealed class Schema(RuntimeModule module) : ISchemaProvider { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
sealed class Audit : IAppAuditEventSink {
    public int Calls;
    public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default) { Calls++; return Task.CompletedTask; }
}
sealed class ObservedTransport(SqliteTransport actual) : IAutomaticMutationTransactionTransport
{
    public int Calls;
    public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Calls++; return actual.FetchAllSqlAsync(query); }
    public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Calls++; return actual.ExecuteSqlAsync(query); }
    public async Task<ISqlTransaction> BeginSqlAsync() { Calls++; return new Transaction(await actual.BeginSqlAsync(), this); }
    sealed class Transaction(ISqlTransaction inner, ObservedTransport owner) : ISqlTransaction {
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { owner.Calls++; return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { owner.Calls++; return inner.ExecuteSqlAsync(query); }
        public Task CommitSqlAsync() { owner.Calls++; return inner.CommitSqlAsync(); }
        public Task RollbackSqlAsync() { owner.Calls++; return inner.RollbackSqlAsync(); }
        public void Dispose() => inner.Dispose();
    }
}
