using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

static void Require(bool value, string reason) { if (!value) throw new InvalidOperationException(reason); }
static string PrivateName() => "密钥🔑" + new string('x', 98);
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder {
    DataSource = Environment.GetEnvironmentVariable("TEAQL_NATURAL_MESSAGES_DB") ?? ":memory:"
}.ToString());
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var transport = new ObservedTransport(new SqliteTransport(connection));
var audit = new Audit();
var clockA = DateTimeOffset.Parse("2026-11-12T12:00:00Z");
var clockB = clockA.AddHours(1);
// The generated module installs its original Checker and passive presentation metadata.
var context = module.IntoContext().WithBusinessClock(new FixedBusinessClock(clockA))
    .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module))).WithAppAuditEventSink(audit);
await context.EnsureSchemaAsync();
await context.EnsureSchemaAsync();
var roots = await Q.Catalogs().Limit(1)
    .Comment("Find the provisioned validation root").Purpose("Verify generated schema bootstrap").ExecuteForListAsync(context);
Require(roots.Count == 1 && roots[0].Id != null, "The generated root must be provisioned");
var rootId = roots[0].Id!.Value;
foreach (var name in new[] { "first", "second" }) {
    var entry = (await Q.Entries().Comment("Prepare an entry").Purpose("Build the bounded validation graph")
        .NewEntityAsync(context)).UpdateCatalog(rootId).UpdateName(name);
    Require(entry.Id > 0 && entry.Version is null, "Creation reserves identity before Save without claiming persistence");
    await entry.AuditAs("Create an entry with the generated audited mutation API").SaveAsync(context);
}
async Task<Catalog> Load() => (await Q.Catalogs().WithIdIs(rootId)
    .SelectEntryListWith(Q.Entries().OrderByIdDescending().Limit(2).Comment("Load the latest validation entries"))
    .Limit(1).Comment("Load the unchanged root and its bounded entry graph")
    .Purpose("Verify indexed Checker messages through generated Q and E").ExecuteForListAsync(context)).Single();
var parent = await Load();
Require(parent.EntryList.Count == 2, "Two children must be loaded");
var other = parent.EntryList[0]; var child = parent.EntryList[1];
Require(E.Entry(other).Name().Eval() == "second" && E.Entry(child).Name().Eval() == "first", "Q/E must preserve selected order");
var parentVersion = parent.Version; var otherVersion = other.Version; var childVersion = child.Version;
var previousUpdate = E.Entry(child).UpdateTime().Eval();
Require(previousUpdate == clockA.UtcDateTime, "Create/update times use the injected clock");
context.WithBusinessClock(new FixedBusinessClock(clockB));
transport.Reset(); audit.Calls = 0;
var missingAuditRejected = false;
try { await parent.AuditAs(" ").SaveAsync(context); }
catch (RequestIntentException error) { Require(error.Code == "REQUEST_COMMENT_REQUIRED", "Typed audit-intent error"); missingAuditRejected = true; }
Require(missingAuditRejected && transport.Calls == 0 && audit.Calls == 0, "Empty audit reason cannot reach provider/audit");
other.UpdateName("changed");
var responses = 0;
async Task Reject(string expected) {
    CheckException? failure = null;
    try { await parent.AuditAs("Reject the second child before SQL or committed audit").SaveAsync(context); }
    catch (CheckException error) { failure = error; }
    Require(failure != null, "The original generated Checker must reject");
    var violation = failure!.Violations.Single();
    Require(violation.RuleId is "required" or "max_length" && violation.EntityType == "Entry", "Stable rule and declaring type");
    Require(violation.ModelPath == "entry_list[1].name" && violation.InstancePath == "/entryList/1/name", "Canonical and wire locations");
    Require(violation.Message == expected, "Complete natural message at the selected child position");
    var response = JsonSerializer.Serialize(new { violations = failure.Violations.Select(v => v.ToWire()).ToArray() });
    Require(!response.Contains(PrivateName()) && !failure.ToString().Contains(PrivateName()), "Private input cannot enter diagnostics");
    Require(transport.Calls == 0 && audit.Calls == 0, "Rejection cannot query, begin, write or audit");
    Require(parent.Version == parentVersion && other.Version == otherVersion && child.Version == childVersion, "Rejected versions unchanged");
    Require(E.Entry(child).UpdateTime().Eval() == previousUpdate, "Rejected automatic Context values roll back");
    Console.WriteLine("DOTNET_NATURAL_CHECKER_RESPONSE=" + response); responses++;
}
// Deliberately violate the non-nullable input contract to test the runtime guard.
// Required rejects null, not an empty string under this model's native rule.
context.SetLocaleCode("en"); child.UpdateName(null!);
await Reject("The name of the 2nd entry is required.");
context.SetLocaleCode("zh-CN");
await Reject("请填写第2项明细的名称。");
context.SetLocaleCode("en"); child.UpdateName(PrivateName());
await Reject("The name of the 2nd entry must not exceed 100 UTF-16 code units; its current length is 102.");
var rendererCalls = 0;
context.InstallCheckerMessageRenderer(request => {
    rendererCalls++;
    Require(request.Arguments["input_len"] == "102", "Retain the original UTF-16 count");
    Require(request.Arguments["input"] != PrivateName(), "Customer renderer receives only projected input");
    return new CheckerMessageOutput("Private " + PrivateName(), []);
});
await Reject("Private [REDACTED]");
Require(rendererCalls == 1, "Graph relocation must not recall the customer renderer");
other.UpdateName("second"); child.UpdateName("first");
await parent.AuditAs("Cancel caller edits without Context-only writes").SaveAsync(context);
Require(transport.Calls == 0 && audit.Calls == 0, "Reverting edits is a true no-op");
child.UpdateName("accepted");
await parent.AuditAs("Save the corrected second child only").SaveAsync(context);
Require(audit.Calls == 1 && transport.Writes == 1, "Only the corrected child writes and audits");
var reloaded = await Load();
Require(reloaded.Version == parentVersion && reloaded.EntryList[0].Version == otherVersion && reloaded.EntryList[1].Version == childVersion + 1,
    "Only the modified version advances");
Require(E.Entry(reloaded.EntryList[1]).Name().Eval() == "accepted", "Corrected name persisted");
Require(E.Entry(reloaded.EntryList[1]).UpdateTime().Eval() == clockB.UtcDateTime, "Successful update uses the new injected clock");
Require(rendererCalls == 1, "Successful saves do not render validation failures");
Console.WriteLine("DOTNET_NATURAL_SQLITE=" + JsonSerializer.Serialize(new {
    passed=true, typedQ=true, typedE=true, automaticMetadata=true, originalGeneratedChecker=true,
    unchangedParent=true, missingAuditRejected, responses, customerRendererCalls=rendererCalls,
    originalLength=PrivateName().Length, failedCheckProviderCalls=0, correctedWrites=transport.Writes,
    correctedAudits=audit.Calls, pointer="/entryList/1/name", parentId=parent.Id, firstId=other.Id, secondId=child.Id,
    parentVersion, firstVersion=otherVersion, secondVersion=reloaded.EntryList[1].Version }));

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
