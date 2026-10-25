using System.Globalization;
using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;

var database = Environment.GetEnvironmentVariable("TEAQL_SCALAR_DB")
    ?? Path.Combine(Path.GetTempPath(), $"teaql-dotnet-scalar-{Guid.NewGuid():N}.sqlite");
await using var connection = new SqliteConnection($"Data Source={database}");
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var diagnostics = new CountingDiagnosticSink();
var audit = new CapturingAuditSink();
var context = module.IntoContext()
    .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
        new MetadataSchemaProvider(module.Metadata.GetEntity)))
    .WithDiagnosticSqlLogSink(diagnostics).WithAppAuditEventSink(audit);
await context.EnsureSchemaAsync();
await context.EnsureSchemaAsync();
var roots = await Q.TypePlatforms().Limit(10).Comment("check scalar bootstrap")
    .Purpose("verify idempotent root seed").ExecuteForListAsync(context);
Require(roots.Count == 1 && roots[0].Id == 1, "bootstrap must keep exactly one root");

var invalid = Q.ScalarTypeProbes().Comment("prepare missing required values")
    .Purpose("verify checker rejects before SQL").NewEntity(context);
var sqlBefore = diagnostics.Count;
try
{
    await invalid.AuditAs("reject incomplete scalar fixture").SaveAsync(context);
    throw new Exception("incomplete fixture was saved");
}
catch (CheckException error)
{
    Require(error.Violations.Any(v => v.RuleId == "required" && v.ModelPath == "required_string"),
        "canonical required_string checker path missing");
}
Require(sqlBefore == diagnostics.Count, "rejected mutation reached SQL");

const decimal precise = 123456789012.3456789m;
var high = await Q.ScalarTypeProbes().WithRequiredStringIs("exact-high").Limit(1)
    .Comment("find retained exact Decimal fixture").Purpose("repeat without deleting database")
    .ExecuteForOneAsync(context);
var fresh = high is null;
if (fresh)
{
    foreach (var (name, amount) in new[] { ("exact-high", precise), ("numeric-two", 2m), ("numeric-ten", 10m) })
    {
        var entity = NewProbe(name, amount);
        await entity.AuditAs("seed scalar fixture through generated mutation API").SaveAsync(context);
    }
}
var rows = await Q.ScalarTypeProbes().OrderByRequiredDecimalAscending().Limit(10)
    .SelectTypePlatformWith(Q.TypePlatformsWithMinimalFields().SelectName())
    .Comment("load numerically ordered exact scalars").Purpose("verify generated Q and E").ExecuteForListAsync(context);
Require(rows.Count == 3 && rows.Select(r => r.RequiredDecimal).SequenceEqual(new decimal?[] { 2m, 10m, precise }),
    "Decimal SQL ordering is not numeric or row count drifted");
high = rows[2];
Require(E.ScalarTypeProbe(high).RequiredDecimal().Eval() == precise, "E Decimal lost digits");
Require(E.ScalarTypeProbe(high).RequiredString().Eval() == "exact-high", "ordinary text was retyped");
Require(E.ScalarTypeProbe(high).TypePlatform().Name().Eval() == "Scalar Type Conformance", "forward E relation mismatch");
Require(high.RequiredTime == new TimeSpan(12, 30, 0) && high.OptionalTime is null, "typed time/null mismatch");
Require(high.RequiredDate == new DateTime(2026, 10, 20)
    && high.RequiredDatetime == new DateTime(2026, 10, 20, 12, 30, 0, DateTimeKind.Utc), "date/timestamp mismatch");
var matches = await Q.ScalarTypeProbes().WithRequiredDecimalIs(precise).Limit(10)
    .Comment("match exact base ten Decimal").Purpose("verify typed query equality").ExecuteForListAsync(context);
Require(matches.Count == 1 && matches[0].Id == high.Id, "exact Decimal equality failed");
var summary = await Q.ScalarTypeProbes().SumRequiredDecimalAs("_total").MinRequiredDecimalAs("_minimum")
    .MaxRequiredDecimalAs("_maximum").Limit(1).Comment("summarize exact Decimal fixture")
    .Purpose("verify generated aggregate API").ExecuteForRowsAsync(context);
var aggregate = summary.Rows.Single();
Require(aggregate["_total"] is Value.DecimalValue total && total.Value == precise + 12m
    && aggregate["_minimum"] is Value.DecimalValue { Value: 2m }
    && aggregate["_maximum"] is Value.DecimalValue { Value: precise }, "typed exact aggregates failed");
var streamed = 0;
await foreach (var row in Q.ScalarTypeProbes().WithRequiredDecimalIs(precise).Limit(1)
    .Comment("stream exact Decimal fixture").Purpose("verify cursor hydration").ExecuteForStreamAsync(context))
{
    Require(E.ScalarTypeProbe(row).RequiredDecimal().Eval() == precise, "stream lost Decimal digits");
    streamed++;
}
Require(streamed == 1, "stream result count mismatch");
var projection = await Q.ScalarTypeProbesWithMinimalFields().SelectRequiredString().WithIdIs(high.Id!.Value)
    .Limit(1).Comment("load a bounded partial scalar projection").Purpose("verify E NotLoaded")
    .ExecuteForOneAsync(context);
try
{
    E.ScalarTypeProbe(projection).RequiredDecimal().Eval();
    throw new Exception("unselected Decimal was treated as loaded");
}
catch (TeaQLNotLoadedException) { }

var previousVersion = high.Version!.Value;
var nextOptional = high.OptionalDecimal is null ? (decimal?)0.10m : null;
high.UpdateOptionalDecimal(nextOptional);
await high.AuditAs("update exact optional Decimal without changing required precision").SaveAsync(context);
var reloaded = await Q.ScalarTypeProbes().WithIdIs(high.Id.Value).Limit(1)
    .Comment("reload audited Decimal update").Purpose("verify exact value and optimistic version")
    .ExecuteForOneAsync(context);
Require(reloaded?.RequiredDecimal == precise && reloaded.OptionalDecimal == nextOptional
    && reloaded.Version == previousVersion + 1, "Decimal update/null/version mismatch");
Require(audit.Events.Any(e => e.TryGetValue("changedFields", out var fields)
    && fields is string[] names && names.Contains("optional_decimal")), "canonical mutation audit missing");
Console.WriteLine($"PASS .NET generated scalar Decimal fresh={fresh} rows=3 version={reloaded!.Version} "
    + $"exact={precise.ToString(CultureInfo.InvariantCulture)} Q=PASS E=PASS Checker=PASS Mutation=PASS");

ScalarTypeProbe NewProbe(string name, decimal amount) => Q.ScalarTypeProbes()
    .Comment("prepare one generated scalar fixture").Purpose("verify exact provider storage").NewEntity(context)
    .UpdateTypePlatform(1).UpdateRequiredString(name).UpdateRequiredBoolean(true)
    .UpdateRequiredInteger(7).UpdateRequiredLong(8).UpdateRequiredDecimal(amount)
    .UpdateRequiredFloat(1.25).UpdateRequiredDouble(2.5).UpdateRequiredDate(new DateTime(2026, 10, 20))
    .UpdateRequiredTime(new TimeSpan(12, 30, 0))
    .UpdateRequiredDatetime(new DateTime(2026, 10, 20, 12, 30, 0, DateTimeKind.Utc));

static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

sealed class CountingDiagnosticSink : IDiagnosticSqlLogSink
{
    public int Count { get; private set; }
    public void Write(ExecutionMetadata metadata) => Count++;
}
sealed class CapturingAuditSink : IAppAuditEventSink
{
    public List<IReadOnlyDictionary<string, object?>> Events { get; } = [];
    public Task RecordAsync(IReadOnlyDictionary<string, object?> safeEvent, CancellationToken cancellationToken = default)
    {
        Events.Add(new Dictionary<string, object?>(safeEvent));
        return Task.CompletedTask;
    }
}
