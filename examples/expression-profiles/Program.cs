using Generated;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
var profile = Environment.GetEnvironmentVariable("TEAQL_EXPRESSION_PROFILE")!;
var pointer = profile switch { "camelCase" => "/scalarTypeProbeList", "snake_case" => "/scalar_type_probe_list", "PascalCase" => "/ScalarTypeProbeList", _ => throw new Exception("unknown profile") };
var suffix = profile switch { "camelCase" => "/0/requiredString", "snake_case" => "/0/required_string", _ => "/0/RequiredString" };
await using var connection = new SqliteConnection($"Data Source={Environment.GetEnvironmentVariable("TEAQL_EXPRESSION_DB")}");
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var service = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection), new MetadataSchemaProvider(module.Metadata.GetEntity));
var context = module.IntoContext().WithDataService(service);
await context.EnsureSchemaAsync();
var observed = new ObservedProvider(service);
context = context.WithDataService(observed);
var row = await Q.TypePlatforms().WithIdIs(1).Limit(1)
    .Comment("Load the root without its reverse list")
    .Purpose("Verify generated E diagnostics under the modeled JSON profile").ExecuteForOneAsync(context)
    ?? throw new Exception("seeded root missing");
Require(row.Id == 1, "root identity drift");
var name = E.TypePlatform(row).Name().Eval();
var before = observed.Snapshot();
Require(before.Queries > 0, "the observer must measure the actual Q query");
NotLoadedDiagnostic wire; string human;
try { E.TypePlatform(row).ScalarTypeProbeList().First().RequiredString().Eval(); throw new Exception("unselected list became loaded"); }
catch (TeaQLNotLoadedException error) { wire = error.ToWire(); human = error.Message; }
using var dto = JsonDocument.Parse(JsonSerializer.Serialize(wire));
var location = dto.RootElement.GetProperty("location");
Require(location.GetArrayLength() == 1 && location[0].GetProperty("kind").GetString() == "property"
    && location[0].GetProperty("name").GetString() == "scalar_type_probe_list", "canonical identity drift");
Require(wire.Code == "NOT_LOADED" && wire.InstancePath == pointer && wire.AttemptedInstancePath == pointer + suffix, "model profile drift");
Require(wire.BreakPoint == "scalar_type_probe_list" && wire.BreakPointIndex == 0, "wrong first failure");
Require(wire.OriginalExpression == "E.TypePlatform(<root>).ScalarTypeProbeList().First().RequiredString().Eval()", "lost original expression");
Require(wire.ExpressionWithBrokenPoint == "E.TypePlatform(<root>).ScalarTypeProbeList()<broken>.First().RequiredString().Eval()", "wrong breakpoint");
Require(human.StartsWith("TeaQLNotLoadedError:") && human.Contains("expression=" + wire.ExpressionWithBrokenPoint)
    && !human.Contains("\"instancePath\""), "human view drift");
Require(observed.Snapshot() == before && E.TypePlatform(row).Name().Eval() == name, "E changed state or accessed the provider");
var origins = new[] { typeof(Value), typeof(QueryRequest), typeof(UserContext), typeof(SqlDataServiceExecutor), typeof(SqliteDialect) }
    .ToDictionary(t => t.Assembly.GetName().Name!, t => t.Assembly.Location);
Console.WriteLine("GENERATED_E_MODEL_PROFILE=" + JsonSerializer.Serialize(new {
    language = "dotnet", profile, round = int.Parse(Environment.GetEnvironmentVariable("TEAQL_EXPRESSION_ROUND")!), passed = true,
    wire, human, providerEntries = 0, queryCalibration = before.Queries, selectedNameUnchanged = true, runtimeOrigins = origins
}));

sealed record Counts(int Queries, int Mutations);
sealed class ObservedProvider(SqlDataServiceExecutor inner) : IDataService
{
    private int queries, mutations;
    public DataServiceCapabilities Capabilities => inner.Capabilities;
    public Counts Snapshot() => new(queries, mutations);
    public Task<QueryResult> QueryAsync(QueryRequest request) { queries++; return inner.QueryAsync(request); }
    public Task<MutationResult> MutateAsync(MutationRequest request) { mutations++; return inner.MutateAsync(request); }
}
