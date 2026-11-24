using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Generated;
using Generated.Models;
using Generated.Requests;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using TeaQL.Provider.Sqlite;

internal static class EmptyAggregates
{
    private const string Purpose = "verify generated empty aggregate contract";
    private static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public static async Task Main()
    {
        var database = Environment.GetEnvironmentVariable("TEAQL_EMPTY_DB") ?? throw new Exception("TEAQL_EMPTY_DB required");
        var vectorFile = Environment.GetEnvironmentVariable("TEAQL_EMPTY_VECTORS") ?? throw new Exception("TEAQL_EMPTY_VECTORS required");
        using var vectors = JsonDocument.Parse(await File.ReadAllBytesAsync(vectorFile));
        var hash = Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(vectorFile))).ToLowerInvariant();
        var allPassed = true;
        var selectedRound = Environment.GetEnvironmentVariable("TEAQL_EMPTY_ROUND");
        var rounds = selectedRound is null ? new[] { 1, 2 } : new[] { int.Parse(selectedRound) };
        Require(rounds.All(round => round is 1 or 2), "TEAQL_EMPTY_ROUND must be 1 or 2");
        foreach (var round in rounds) allPassed &= await Round(database, vectors.RootElement, hash, round);
        if (!allPassed) Environment.ExitCode = 1;
    }

    private static async Task<bool> Round(string database, JsonElement vectors, string hash, int round)
    {
        await using var connection = new SqliteConnection($"Data Source={database}");
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module;
        var transport = new ObservedTransport(new SqliteTransport(connection));
        var sink = new Sink();
        var telemetry = new Telemetry();
        var context = module.IntoContext().WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport,
            new MetadataSchemaProvider(module.Metadata.GetEntity))).WithDiagnosticSqlLogSink(sink).WithRuntimeTelemetry(telemetry);
        await context.EnsureSchemaAsync();
        var scalarIds = new List<long>(); var bucketIds = new List<long>();
        foreach (var vector in vectors.GetProperty("scalarCases").EnumerateArray())
        {
            var parent = await Parent($"EA236-{round}-scalar-{vector.GetProperty("id").GetString()}"); scalarIds.Add(parent);
            foreach (var value in vector.GetProperty("values").EnumerateArray()) await Child(parent, value.ValueKind == JsonValueKind.Null ? null : value.GetInt32());
        }
        for (var i = 1; i <= 4; i++) bucketIds.Add(await Parent($"EA236-{round}-bucket-{i}"));
        foreach (var child in vectors.GetProperty("bucketCases")[1].GetProperty("children").EnumerateArray())
            await Child(bucketIds[child.GetProperty("parent").GetInt32() - 1], child.GetProperty("value").ValueKind == JsonValueKind.Null ? null : child.GetProperty("value").GetInt32());
        var rows = new List<Dictionary<string, object?>>(); var errors = new List<string>();
        foreach (var logging in new[] { true, false })
        {
            context.EnableQuerySqlLog(logging);
            for (var i = 0; i < scalarIds.Count; i++)
            {
                var vector = vectors.GetProperty("scalarCases")[i]; var id = vector.GetProperty("id").GetString()!;
                var query = Q.ScalarTypeProbesWithMinimalFields().FilterByTypePlatform(scalarIds[i])
                    .CountAs("rows").SumOptionalIntegerAs("total").Limit(1).Comment($"inspect {id} nullable aggregate");
                var execute = query.Purpose(Purpose);
                Reset(); var before = Snapshot(query.GetQuery()); var result = await execute.ExecuteForRowsAsync(context);
                var record = result.Rows.Single();
                Require(record["rows"].TryI64() == vector.GetProperty("countStar").GetInt64(), "COUNT(*) mismatch");
                var sum = NullableInteger(record["total"]); Require(sum == OptionalInteger(vector.GetProperty("sum")), "SUM mismatch");
                Add(id, new { countStar = record["rows"].TryI64(), sum }, query.GetQuery(), before, result.Metadata);
                var known = Q.ScalarTypeProbesWithMinimalFields().FilterByTypePlatform(scalarIds[i]).WithOptionalIntegerIsKnown()
                    .CountAs("values_count").Limit(1).Comment($"inspect {id} non-null contributions");
                var executeKnown = known.Purpose(Purpose);
                Reset(); before = Snapshot(known.GetQuery()); result = await executeKnown.ExecuteForRowsAsync(context);
                var count = result.Rows.Single()["values_count"].TryI64(); Require(count == vector.GetProperty("countValue").GetInt64(), "COUNT(value) mismatch");
                Add(id + "-COUNT-VALUE", count, known.GetQuery(), before, result.Metadata);
            }
            var related = Q.TypePlatformsWithMinimalFields().SelectName().WithIdIs(bucketIds[3])
                .CountScalarTypeProbesWith("rows", Contributions()).SumOptionalIntegerOfScalarTypeProbesAs("total", Contributions())
                .OrderByIdAscending().Limit(1).Comment("inspect EA-05 empty related SUM");
            var executeRelated = related.Purpose(Purpose);
            Reset(); var relatedBefore = Snapshot(related.GetQuery()); var parents = await executeRelated.ExecuteForListAsync(context);
            var entity = parents.Single(); Require(E.TypePlatform(entity).Id().Eval() == bucketIds[3], "generated E identity mismatch");
            Require(entity.DynamicProperties.Contains("rows") && entity.DynamicProperties.Get("rows")?.TryI64() == 0, "related empty COUNT missing");
            Require(entity.DynamicProperties.Contains("total") && IsNull(entity.DynamicProperties.Get("total")), "related empty SUM missing");
            var entityJSON = JsonSerializer.Deserialize<JsonElement>(entity.ToJson());
            if (!entityJSON.TryGetProperty("rows", out var jsonCount) || jsonCount.GetInt64() != 0) errors.Add($"round {round} logging {logging}: selected COUNT missing from entity JSON");
            if (!entityJSON.TryGetProperty("total", out var jsonSum) || jsonSum.ValueKind != JsonValueKind.Null) errors.Add($"round {round} logging {logging}: selected null SUM missing from entity JSON");
            var relatedRow = Add("EA-05", new { count = 0, sum = (long?)null }, related.GetQuery(), relatedBefore);
            relatedRow["entityJSON"] = entityJSON;
            foreach (var includeAll in new[] { true, false })
            {
                var facet = Q.ScalarTypeProbesWithMinimalFields().FilterByTypePlatformIn(bucketIds.Select(id => (object)id).ToArray())
                    .FacetByTypePlatformAs("buckets", Q.TypePlatformsWithMinimalFields().WithIdIn(bucketIds.Select(id => (object)id).ToArray()).SelectName()
                        .CountAs("rows").SumOptionalIntegerOfScalarTypeProbesAs("total", Contributions()).OrderByIdAscending().Limit(10)
                        .Comment("load facet candidate buckets"), includeAll)
                    .OrderByIdAscending().Limit(1).Comment("inspect EA-06 complete membership behind one visible row");
                var executeFacet = facet.Purpose(Purpose);
                Reset(); var before = Snapshot(facet.GetQuery()); var result = await executeFacet.ExecuteForRowsAsync(context);
                Require(result.Rows.Count == 1, "facet visible page must contain one row");
                var values = result.Facets["buckets"].Select(record => new { parent = bucketIds.IndexOf(Identity(record)) + 1,
                    count = record["rows"].TryI64(), sum = NullableInteger(record["total"]) }).OrderBy(x => x.parent).ToArray();
                var expected = vectors.GetProperty("bucketCases")[1].GetProperty("expected").EnumerateArray()
                    .Where(v => includeAll || v.GetProperty("count").GetInt64() > 0).Select(v => new { parent = v.GetProperty("parent").GetInt32(),
                        count = (long?)v.GetProperty("count").GetInt64(), sum = OptionalInteger(v.GetProperty("sum")) }).ToArray();
                Require(JsonSerializer.Serialize(values) == JsonSerializer.Serialize(expected), "facet membership or nullable SUM mismatch");
                var row = Add("EA-06", values, facet.GetQuery(), before, result.Metadata); row["includeAll"] = includeAll; row["visibleRows"] = 1;
            }
            var fallback = Q.ScalarTypeProbesWithMinimalFields().FilterByTypePlatform(scalarIds[0]).SumOptionalIntegerAs("total")
                .Limit(1).Comment("inspect EA-07 explicit fallback");
            var executeFallback = fallback.Purpose(Purpose);
            Reset(); var fallbackBefore = Snapshot(fallback.GetQuery()); var fallbackResult = await executeFallback.ExecuteForRowsAsync(context);
            var raw = NullableInteger(fallbackResult.Rows.Single()["total"]); var zero = raw ?? 0;
            Require(raw is null && zero == 0 && IsNull(fallbackResult.Rows.Single()["total"]), "fallback changed raw null");
            Add("EA-07", new { raw, fallback = zero, rawRemains = NullableInteger(fallbackResult.Rows.Single()["total"]) }, fallback.GetQuery(), fallbackBefore, fallbackResult.Metadata);
            var grouped = Q.ScalarTypeProbesWithMinimalFields().FilterByTypePlatform(scalarIds[0]).GroupByOptionalInteger()
                .SumOptionalIntegerAs("total").Limit(10).Comment("inspect EA-08 empty grouped input");
            var executeGrouped = grouped.Purpose(Purpose);
            Reset(); var groupedBefore = Snapshot(grouped.GetQuery()); var groups = await executeGrouped.ExecuteForRowsAsync(context);
            Require(groups.Rows.Count == 0, "empty group query fabricated a row");
            Add("EA-08", Array.Empty<object>(), grouped.GetQuery(), groupedBefore, groups.Metadata);

            // Test-only fault injection. Generated SQL remains unchanged; SQLite
            // really encounters a missing table, then the savepoint restores it.
            var failing = Q.ScalarTypeProbesWithMinimalFields().FilterByTypePlatform(scalarIds[0]).SumOptionalIntegerAs("total")
                .Limit(1).Comment("inspect actual missing-table SQLite failure");
            var executeFailing = failing.Purpose(Purpose);
            Reset(); var failingBefore = Snapshot(failing.GetQuery()); SqliteException? nativeFailure = null;
            using (var command = connection.CreateCommand()) { command.CommandText = "SAVEPOINT core236_failure; DROP TABLE scalar_type_probe_data;"; await command.ExecuteNonQueryAsync(); }
            try { await executeFailing.ExecuteForRowsAsync(context); throw new Exception("missing table query unexpectedly succeeded"); }
            catch (Exception error) { for (Exception? inner = error; inner is not null; inner = inner.InnerException) if (inner is SqliteException sqlite) nativeFailure = sqlite;
                if (nativeFailure is null) throw; }
            finally { using var command = connection.CreateCommand(); command.CommandText = "ROLLBACK TO core236_failure; RELEASE core236_failure;"; await command.ExecuteNonQueryAsync(); }
            Require(nativeFailure!.SqliteErrorCode == 1 && nativeFailure.Message.Contains("no such table"), "failure was not SQLite prepare failure");
            var failureRow = Add("REAL-SQLITE-FAILURE", new { type = nativeFailure.GetType().Name, code = nativeFailure.SqliteErrorCode,
                extendedCode = nativeFailure.SqliteExtendedErrorCode, faultInjection = "savepoint-drop-table", restored = true }, failing.GetQuery(), failingBefore);
            Require(telemetry.Events.All(e => (string)e.Attributes["teaql.sql.outcome"] == "failure"), "failure diagnostic reported success");
            var recovery = Q.TypePlatformsWithMinimalFields().WithIdIs(bucketIds[3]).SelectName().Limit(1)
                .Comment("load independent detail after aggregate failure");
            var executeRecovery = recovery.Purpose("verify independent request isolation");
            Reset(); var recoveryBefore = Snapshot(recovery.GetQuery()); var recovered = await executeRecovery.ExecuteForOneAsync(context);
            Require(E.TypePlatform(recovered).Name().Eval() == $"EA236-{round}-bucket-4", "independent request failed to recover");
            Add("QUERY-RECOVERY", new { recovered = true }, recovery.GetQuery(), recoveryBefore);

            Dictionary<string, object?> Add(string name, object? value, SelectQuery query, string before, ExecutionMetadata? metadata = null)
            {
                var after = Snapshot(query); Require(before == after, "query or request options mutated");
                Require(logging ? transport.Reads.SequenceEqual(sink.Entries.Select(e => e.ParameterizedQuery)) : sink.Entries.Count == 0,
                    "ordinary logging toggle or sink SQL differs from actual driver reads");
                Require(telemetry.Events.Count == transport.Reads.Count && telemetry.Statements.Count == transport.Reads.Count,
                    "fine-grained telemetry must remain complete with ordinary logging on or off");
                Require(telemetry.Events.All(e => !string.IsNullOrWhiteSpace((string)e.Attributes["teaql.intent.comment"])
                    && !string.IsNullOrWhiteSpace((string)e.Attributes["teaql.intent.purpose"]) && (string)e.Attributes["teaql.sql.log_mode"] == "masked"), "statement lost safe intent");
                Require(telemetry.Statements.All(s => s.Disposed && s.Outcome is "success" or "failure"), "statement observation did not terminate");
                var row = new Dictionary<string, object?> { ["case"] = name, ["logging"] = logging, ["values"] = value,
                    ["requestBefore"] = before, ["requestAfter"] = after, ["driverSQL"] = transport.Reads.ToArray(),
                    ["sql"] = sink.Entries.ToArray(), ["telemetry"] = telemetry.Events.ToArray(),
                    ["statementObservations"] = telemetry.Statements.ToArray(), ["metadata"] = metadata };
                rows.Add(row); return row;
            }
        }
        var report = new { schema = "teaql.dotnet-generated-empty-aggregate/v1", passed = errors.Count == 0, round,
            vectorsSha256 = hash, scalarIds, bucketIds, rows, errors, fullQueryTraceAccepted = false, allP0P1Complete = false };
        var text = JsonSerializer.Serialize(report, Json); await File.WriteAllTextAsync(database + $".round-{round}.json", text + "\n");
        Console.WriteLine("DOTNET_GENERATED_EMPTY_AGGREGATE=" + text); return errors.Count == 0;

        void Reset() { sink.Entries.Clear(); transport.Reads.Clear(); telemetry.Events.Clear(); telemetry.Statements.Clear(); }
        async Task<long> Parent(string name)
        {
            var parent = (await Q.TypePlatforms().Comment("prepare aggregate bucket").Purpose("seed evaluated scalar model").NewEntityAsync(context)).UpdateName(name);
            await parent.AuditAs("seed aggregate parent through generated mutation API").SaveAsync(context); return parent.Id!.Value;
        }
        async Task Child(long parent, int? value)
        {
            var child = (await Q.ScalarTypeProbes().Comment("prepare aggregate contribution").Purpose("seed evaluated scalar model").NewEntityAsync(context))
                .UpdateTypePlatform(parent).UpdateRequiredString("EA236 contribution").UpdateRequiredBoolean(true).UpdateRequiredInteger(7)
                .UpdateOptionalInteger(value).UpdateRequiredLong(8).UpdateRequiredDecimal(1m).UpdateRequiredFloat(1.25).UpdateRequiredDouble(2.5)
                .UpdateRequiredDate(new DateTime(2026, 10, 20)).UpdateRequiredTime(new TimeSpan(12, 30, 0))
                .UpdateRequiredDatetime(new DateTime(2026, 10, 20, 12, 30, 0, DateTimeKind.Utc));
            await child.AuditAs("seed aggregate contribution through generated mutation API").SaveAsync(context);
        }
    }

    private static ScalarTypeProbeRequest Contributions() => Q.ScalarTypeProbesWithMinimalFields().Limit(100).Comment("load aggregate contributions");
    private static string Snapshot(SelectQuery query) => JsonSerializer.Serialize(query, Json);
    private static bool IsNull(Value? value) => value is Value.NullValue or Value.TypedNullValue;
    private static long? NullableInteger(Value value) => IsNull(value) ? null : value.TryI64() ?? throw new Exception("non-integral aggregate value");
    private static long? OptionalInteger(JsonElement value) => value.ValueKind == JsonValueKind.Null ? null : value.GetInt64();
    private static long Identity(Record record) => (record.TryGetValue("id", out var id) ? id : record["Id"]).TryI64()!.Value;
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private sealed class Sink : IDiagnosticSqlLogSink
    {
        internal readonly List<ExecutionMetadata> Entries = new();
        public void Write(ExecutionMetadata metadata) => Entries.Add(metadata);
    }
    private sealed class Telemetry : IRuntimeTelemetry, IRuntimeSqlTelemetry
    {
        internal readonly List<RuntimeDiagnosticEvent> Events = new();
        internal readonly List<Statement> Statements = new();
        private readonly string traceId = Guid.NewGuid().ToString("N");
        private long nextSpan;
        public IRuntimeTelemetryScope Start(RuntimeOperation operation) => NoopRuntimeTelemetry.Instance.Start(operation);
        public ISqlExecutionObservation StartStatement(string backend, DataServiceOperation operation)
        {
            var statement = new Statement(new(traceId, (++nextSpan).ToString("x16")), backend, operation.ToString());
            Statements.Add(statement); return statement;
        }
        public void RecordDiagnostic(RuntimeDiagnosticEvent diagnostic) => Events.Add(diagnostic);
    }
    private sealed class Statement(SqlExecutionCorrelation correlation, string backend, string operation) : ISqlExecutionObservation
    {
        public SqlExecutionCorrelation Correlation => correlation;
        public string Backend => backend;
        public string Operation => operation;
        public string? Outcome { get; private set; }
        public bool Disposed { get; private set; }
        public void Success() => Outcome = "success";
        public void Failure(Exception error) => Outcome = "failure";
        public void Dispose() => Disposed = true;
    }
    private sealed class ObservedTransport(SqliteTransport inner) : ISqlTransport, IAutomaticMutationTransactionTransport, ISchemaConnectionInitializer, ISchemaCompatibilityVerifier
    {
        internal readonly List<string> Reads = new();
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Reads.Add(query.Sql); return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public Task<ISqlTransaction> BeginSqlAsync() => inner.BeginSqlAsync();
        public Task EnsureSchemaFunctionsAsync() => inner.EnsureSchemaFunctionsAsync();
        public Task VerifySchemaCompatibilityAsync(SqlDialect dialect, EntityDescriptor entity) => inner.VerifySchemaCompatibilityAsync(dialect, entity);
    }
}
