using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using TeaQL.Provider.Sqlite;
using System.Text.Json;
using Record = TeaQL.Core.Record;

const string Secret = "DOTNET-FACET-PRIVATE-SCHOOL";
const string Future = "Campus Learning Platform";
var database = Path.GetFullPath(Environment.GetEnvironmentVariable("TEAQL_FACET_TRACE_DB")
    ?? throw new InvalidOperationException("Set TEAQL_FACET_TRACE_DB or run examples/facet-trace/verify.sh"));
await using var connection = new SqliteConnection("Data Source=" + database);
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var transport = new Transport(new SqliteTransport(connection));
var provider = new SqlDataServiceExecutor(new SqliteDialect(), transport,
    new MetadataSchemaProvider(module.Metadata.GetEntity));
var audit = new Audits(); var sink = new Sink();
var context = module.IntoContext().WithDataService(provider).WithAppAuditEventSink(audit)
    .WithDiagnosticSqlLogSink(sink);
await context.EnsureSchemaAsync();
var existing = await Q.Schools().WithNameIs(Secret).Limit(10).Comment("check retained fixture")
    .Purpose("reuse one database without cleanup").ExecuteForListAsync(context);
if (existing.Count == 0) {
    for (var i = 0; i < 2; i++) {
        var school = Q.Schools().Comment("seed facet membership").Purpose("prepare generated fixture").NewEntity(context);
        school.UpdatePlatformId(1).UpdateSchoolTypeToPrimary().UpdateName(Secret)
            .UpdateAddress("Facet Road").UpdateEstablishedDate(new DateTime(2001, 1, 1))
            .UpdateStudentCapacity(10).UpdateActive(true);
        await school.AuditAs("seed generated facet school").SaveAsync(context);
    }
} else Check(existing.Count == 2, "retained fixture membership changed");
var observed = new Observed(provider);
context.WithDataService(observed);
audit.Events.Clear();
transport.Reset();
var scenarios = 0;
foreach (var logging in new[] { false, true })
foreach (var mode in new[] { "root", "nested", "loaded" })
foreach (var empty in new[] { false, true })
foreach (var includeAll in new[] { false, true }) {
    context.EnableQuerySqlLog(logging);
    sink.Entries.Clear(); observed.Statements.Clear(); transport.Reads.Clear();
    var nested = mode != "root";
    var comment = "load " + Secret + (nested ? " via " + Future : "");
    const string purpose = "verify generated Facet ownership";
    var types = Q.SchoolTypesWithMinimalFields().SelectCode().OrderByIdAscending().Limit(10).CountAs("schoolCount");
    if (nested) types.FacetByPlatformAs("platforms", Q.PlatformsWithMinimalFields().SelectName()
        .WithNameIs(Future).Limit(10).CountAs("typeCount"), includeAll);
    var schools = Q.SchoolsWithMinimalFields().SelectName().WithNameIs(Secret)
        .OrderByIdAscending().Limit(1).FacetBySchoolTypeAs("types", types, includeAll);
    if (empty) schools.WithIdIs(-1);
    var results = new List<object>();
    if (mode == "loaded") {
        var parents = await Q.SchoolTypesWithMinimalFields().SelectCode().OrderByIdAscending().Limit(10)
            .SelectSchoolListWith(schools).Comment(comment).Purpose(purpose).ExecuteForListAsync(context);
        Check(parents.Count == 2, "loaded parent count");
        foreach (var parent in parents) {
            var member = !empty && parent.Id == 1001;
            Check(parent.IsLoaded("SchoolList"), "selected collection is NotLoaded");
            var list = (object)parent.SchoolList as SmartList<School>;
            Check(list != null && list.IsLoaded, "typed collection lost query-only SmartList carrier");
            Check(list!.Count == (member ? 1 : 0), "bounded loaded collection count");
            Check(E.SchoolType(parent).SchoolList().Size().Eval() == list.Count, "E collection size");
            VerifyChoices(list.Facets["types"], member ? 2 : 0, includeAll, true);
            results.Add(new { parentId = parent.Id, visibleRows = list.Count,
                facets = DescribeChoices(list.Facets["types"], true) });
            await parent.AuditAs("retain read-only Facet state").SaveAsync(context);
            Check(!parent.HasQueryProjection("types"), "Facet became a scalar query alias");
            Check(!JsonSerializer.Serialize(parent.ToUpdateCommand().Values).Contains("schoolCount"), "Facet became persistent values");
        }
    } else {
        var rows = await schools.Comment(comment).Purpose(purpose).ExecuteForListAsync(context);
        Check(rows.Count == (empty ? 0 : 1), "root bounded rows");
        if (!empty) Check(E.School(rows[0]).Name().Eval() == Secret, "E lost real value");
        VerifyChoices(rows.Facets["types"], empty ? 0 : 2, includeAll, nested);
        results.Add(new { visibleRows = rows.Count, facets = DescribeChoices(rows.Facets["types"], nested) });
    }
    var root = mode == "loaded" ? "SchoolType" : "School";
    var branch = new[] { "SchoolList", "SchoolList/SchoolType", "SchoolList/SchoolType",
        "SchoolList/SchoolType/Platform", "SchoolList/SchoolType/Platform" };
    var expectedRoutes = mode == "loaded" ? new[] { "" }.Concat(branch).Concat(branch).ToArray()
        : nested ? new[] { "", "SchoolType", "SchoolType", "SchoolType/Platform", "SchoolType/Platform" }
        : new[] { "", "SchoolType", "SchoolType" };
    Check(transport.Reads.Count == expectedRoutes.Length, "physical SQL count changed");
    var countSql = transport.Reads.Count(read => read.Sql.Contains("COUNT(", StringComparison.OrdinalIgnoreCase));
    Check(countSql == (mode == "loaded" ? 4 : nested ? 2 : 1), "physical membership count SQL changed");
    Check(observed.Statements.All(statement => statement.Comment == comment && statement.Purpose == purpose), "root intent changed");
    foreach (var statement in sink.Entries) {
        Check(statement.ExecutionOutcome == "success", "physical SQL failed");
        Check(statement.Comment == "load [REDACTED]" + (nested ? " via [REDACTED]" : "")
            && statement.Purpose == purpose, "physical inherited safe intent changed");
        var route = statement.TraceChain.Where(node => node.Kind == "relation").Select(node => node.Name).ToArray();
        var allowed = mode == "loaded"
            ? new[] { "", "SchoolList", "SchoolList/SchoolType", "SchoolList/SchoolType/Platform" }
            : new[] { "", "SchoolType", "SchoolType/Platform" };
        Check(allowed.Contains(string.Join("/", route)), "unexpected physical route " + string.Join("/", route));
        Check(statement.TraceChain.Select(node => node.Kind).SequenceEqual(
            new[] { "operation", "request" }.Concat(route.Select(_ => "relation")).Concat(new[] { "provider", "sql" })), "noncanonical path");
        Check(statement.TraceChain[0].Name == root && statement.TraceChain[1].Name == root, "root reset");
        Check(statement.TraceChain[^2].Name == "sqlite" && statement.TraceChain[^1].Name == "select", "physical tail");
    }
    Check(sink.Entries.Count == (logging ? transport.Reads.Count : 0), "diagnostic logging mode");
    Check(transport.Reads.Any(read => read.Params.Any(value => value.Raw is string text && text == Secret)), "private SQL binding changed");
    Check(transport.Reads.Any(read => read.Sql.Contains("COUNT(", StringComparison.OrdinalIgnoreCase)), "missing membership count SQL");
    if (logging) {
        Check(sink.Entries.Select(statement => string.Join("/", statement.TraceChain
            .Where(node => node.Kind == "relation").Select(node => node.Name)))
            .SequenceEqual(expectedRoutes), "ordered physical Facet ancestry changed");
        foreach (var statement in sink.Entries) {
            var previous = root;
            foreach (var relation in statement.TraceChain.Where(node => node.Kind == "relation")) {
                Check(relation.Detail == previous + "." + relation.Name, "logical relation edge changed");
                previous = relation.Name == "SchoolList" ? "School" : relation.Name;
            }
        }
        var paths = sink.Entries.Select(statement => string.Join("/", statement.TraceChain
            .Where(node => node.Kind == "relation").Select(node => node.Name))).ToHashSet();
        var prefix = mode == "loaded" ? "SchoolList/" : "";
        var expected = new HashSet<string> { "", prefix + "SchoolType" };
        if (mode == "loaded") expected.Add("SchoolList");
        if (nested) expected.Add(prefix + "SchoolType/Platform");
        Check(paths.SetEquals(expected), "missing physical Facet route");
        Check(sink.Entries[0].Comment!.Contains("[REDACTED]"), "first SQL intent not masked");
        var safe = JsonSerializer.Serialize(sink.Entries);
        Check(!safe.Contains(Secret) && (!nested || !safe.Contains(Future)), "safe emitted intent leaked");
        Console.WriteLine("SAFE_FACET_SQL " + safe);
    }
    Console.WriteLine($"FACET mode={mode} empty={empty} includeAll={includeAll} logging={logging} physical={transport.Reads.Count}");
    Console.WriteLine("FACET_OBSERVED " + JsonSerializer.Serialize(new {
        mode, empty, includeAll, logging, physical = transport.Reads.Count, countSql, results, sql = sink.Entries }));
    scenarios++;
}
context.EnableQuerySqlLog(); sink.Entries.Clear();
var unselected = await Q.SchoolTypesWithMinimalFields().Limit(1).Comment("independent " + Secret + " " + Future)
    .Purpose("no retained private state").ExecuteForOneAsync(context);
Check(sink.Entries.Single().Comment == "independent " + Secret + " " + Future, "privacy leaked into next request");
Check(!unselected!.IsLoaded("SchoolList"), "unselected collection became loaded");
try { _ = E.SchoolType(unselected).SchoolList().Size().Eval(); throw new Exception("NotLoaded was hidden"); }
catch (TeaQLNotLoadedException) { }
Check(audit.Events.Count == 0, "query emitted mutation audits");
Check(observed.Mutations == 0 && transport.Writes == 0, "query polluted mutation ownership");
var graph = await Q.SchoolTypes().WithIdIs(1001).SelectSchoolListWith(Q.Schools()
    .WithNameIs(Secret).OrderByIdAscending().Limit(10)).Comment("load complete reverse graph")
    .Purpose("verify generated collection mutation compatibility").ExecuteForOneAsync(context);
Check(graph != null && graph.SchoolList.Count == 2, "complete reverse graph fixture");
var child = graph!.SchoolList[0];
var oldVersion = child.Version;
var address = E.School(child).Address().Eval() == "Facet Road A" ? "Facet Road B" : "Facet Road A";
child.UpdateAddress(address);
await graph.AuditAs("update one reverse-list child").SaveAsync(context);
var persisted = await Q.Schools().WithIdIs(child.Id!.Value).Comment("read committed reverse child")
    .Purpose("verify graph save reached SQLite").ExecuteForOneAsync(context);
Check(persisted != null && E.School(persisted).Address().Eval() == address
    && persisted.Version == oldVersion + 1, "reverse child update was not committed");
Check(audit.Events.Count == 1, "reverse child update audit count");
Console.WriteLine("REVERSE_CHILD_UPDATE_PASS count=1");
Console.WriteLine($"DOTNET_FACET_PASS scenarios={scenarios} database={database}");

static void VerifyChoices(SmartList<Record> choices, long members, bool includeAll, bool nested) {
    Check(choices.Count == (includeAll ? 2 : members > 0 ? 1 : 0), "facet candidate count");
    var expectedIds = includeAll ? new long?[] { 1001, 1002 } : members > 0 ? new long?[] { 1001 } : Array.Empty<long?>();
    Check(choices.Select(choice => choice["id"].TryI64()).SequenceEqual(expectedIds), "facet candidate identities");
    foreach (var choice in choices)
        Check(choice["schoolCount"].TryI64() == (choice["id"].TryI64() == 1001 ? members : 0), "facet membership count");
    if (nested) {
        var platforms = choices.Facets["platforms"];
        Check(platforms.Count == (choices.Count > 0 ? 1 : 0), "nested Facet candidate count including empty");
        if (platforms.Count > 0) Check(platforms[0]["typeCount"].TryI64() == choices.Count, "nested Facet count/materialization");
    }
}
static object DescribeChoices(SmartList<Record> choices, bool nested) => new {
    types = choices.Select(choice => new { id = choice["id"].TryI64(), count = choice["schoolCount"].TryI64() }).ToArray(),
    platforms = nested ? choices.Facets["platforms"].Select(choice =>
        new { id = choice["id"].TryI64(), count = choice["typeCount"].TryI64() }).ToArray() : null,
};
static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
sealed class Sink : IDiagnosticSqlLogSink {
    public List<ExecutionMetadata> Entries { get; } = new();
    public void Write(ExecutionMetadata metadata) => Entries.Add(metadata);
}
sealed class Audits : IAppAuditEventSink {
    public List<IReadOnlyDictionary<string, object?>> Events { get; } = new();
    public Task RecordAsync(IReadOnlyDictionary<string, object?> entry, CancellationToken cancellationToken = default) {
        Events.Add(entry); return Task.CompletedTask;
    }
}
sealed class Observed(IDataService inner) : IDataService {
    public List<ExecutionMetadata> Statements { get; } = new();
    public int Mutations { get; private set; }
    public DataServiceCapabilities Capabilities => inner.Capabilities;
    public Task<MutationResult> MutateAsync(MutationRequest request) { Mutations++; return inner.MutateAsync(request); }
    public async Task<QueryResult> QueryAsync(QueryRequest request) {
        var result = await inner.QueryAsync(request); Statements.Add(result.Metadata); return result;
    }
}
sealed class Transport(ISqlTransport inner) : ISqlTransport, ISqlTransactionTransport {
    public List<CompiledQuery> Reads { get; } = new();
    public int Writes { get; private set; }
    public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Writes++; return inner.ExecuteSqlAsync(query); }
    public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Reads.Add(query); return inner.FetchAllSqlAsync(query); }
    public Task<ISqlTransaction> BeginSqlAsync() => ((ISqlTransactionTransport)inner).BeginSqlAsync();
    public void Reset() { Reads.Clear(); Writes = 0; }
}
