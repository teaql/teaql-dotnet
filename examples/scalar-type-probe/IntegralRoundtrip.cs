using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;

internal static class IntegralRoundtrip
{
    public static async Task RunAsync(string database)
    {
        await using var connection = new SqliteConnection($"Data Source={database}");
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module;
        var provider = new Observed(new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
            new MetadataSchemaProvider(module.Metadata.GetEntity)));
        var context = module.IntoContext().WithDataService(provider);
        await context.EnsureSchemaAsync();
        var vectors = new (string Name, long Integer, long Long)[] {
            ("integral-min", int.MinValue, long.MinValue), ("integral-max", int.MaxValue, long.MaxValue),
            ("integral-zero", 0, 0), ("integral-beyond-js", 1, 9007199254740993) };
        var retained = new List<ScalarTypeProbe>();
        foreach (var vector in vectors)
        {
            var row = await Q.ScalarTypeProbes().WithRequiredStringIs(vector.Name).Limit(1)
                .Comment("find retained exact integers").Purpose("repeat the generated example without cleanup").ExecuteForOneAsync(context);
            if (row is null)
            {
                row = (await NewAsync(context, vector.Name)).UpdateRequiredInteger(vector.Integer).UpdateRequiredLong(vector.Long);
                await row.AuditAs("seed exact integer endpoints through generated mutation").SaveAsync(context);
            }
            row = await Load(context, row.Id!.Value);
            Require(E.ScalarTypeProbe(row).RequiredInteger().Eval() == vector.Integer
                && E.ScalarTypeProbe(row).RequiredLong().Eval() == vector.Long && row.Version == 1, "exact integer endpoint/version");
            var calls = provider.Snapshot();
            var decoded = ScalarTypeProbe.FromJson(row.ToJson());
            Require(E.ScalarTypeProbe(decoded).RequiredLong().Eval() == vector.Long
                && E.ScalarTypeProbe(decoded).RequiredInteger().Eval() == vector.Integer
                && E.ScalarTypeProbe(decoded).OptionalLong().Eval() is null
                && provider.Snapshot() == calls, "typed JSON integer/null precision or unexpected I/O");
            retained.Add(row);
        }
        var calibration = (await NewAsync(context, "integral-calibration-" + Guid.NewGuid().ToString("N")));
        await calibration.AuditAs("calibrate generated create on every retained example round").SaveAsync(context);
        var calibrated = provider.Snapshot();
        Require(calibrated.Query > 0 && calibrated.Begin > 0 && calibrated.Mutate > 0 && calibrated.NextId > 0,
            "provider observer did not see normal Q/save work");
        var cases = new List<object>();
        var bad = new (string Field, Value Value)[] {
            ("required_long", new Value.DecimalValue(1.5m)), ("required_long", new Value.DecimalValue(9223372036854775808m)),
            ("required_long", new Value.F64Value(1)), ("required_long", new Value.TextValue("PRIVATE_INVALID_INTEGER")),
            ("required_long", new Value.BoolValue(true)),
            ("required_integer", new Value.DecimalValue(1.5m)), ("required_integer", new Value.I64Value((long)int.MaxValue + 1)),
            ("required_integer", new Value.F64Value(1)), ("required_integer", new Value.TextValue("PRIVATE_INVALID_INTEGER")),
            ("required_integer", new Value.BoolValue(true)),
            ("type_platform", new Value.DecimalValue(1.5m)), ("type_platform", new Value.I64Value(-1)),
            ("type_platform", new Value.DecimalValue(18446744073709551616m)), ("type_platform", new Value.U64Value(ulong.MaxValue)),
            ("type_platform", new Value.F64Value(1)), ("type_platform", new Value.TextValue("PRIVATE_INVALID_REFERENCE")) };
        foreach (var update in new[] { false, true })
        for (var index = 0; index < bad.Length; index++)
        {
            var (field, value) = bad[index];
            var fix = new ReplacingChecker(field, value);
            var candidateContext = module.And(new RuntimeModule().Checker("ScalarTypeProbe", fix)).IntoContext().WithDataService(provider);
            var candidate = update ? await Load(context, retained[0].Id!.Value) : (await NewAsync(candidateContext, "invalid-integral-create"));
            if (update) candidate.UpdateRequiredString("pending-invalid-integral-update");
            var before = provider.Snapshot();
            try { await candidate.AuditAs("reject invalid customer integral Fix output").SaveAsync(candidateContext); throw new Exception("invalid integral Fix saved"); }
            catch (CheckException error)
            {
                var violation = error.Violations.Single();
                Require(violation.RuleId == "invalid_type" && violation.ModelPath == field && violation.InputValue is null
                    && !error.Message.Contains("PRIVATE_INVALID") && fix.Calls == 1 && provider.Snapshot() == before,
                    "invalid Fix output reached provider, leaked values or lost canonical diagnostic");
            }
            cases.Add(new { kind = "post-fix", field, index, update, providerCalls = new Counts(0, 0, 0, 0) });
        }
        foreach (var (field, value) in new[] {
            ("required_integer", (long)int.MaxValue + 1), ("required_integer", (long)int.MinValue - 1),
            ("optional_integer", (long)int.MaxValue + 1), ("optional_integer", (long)int.MinValue - 1) })
        {
            var candidate = (await NewAsync(context, "invalid-integer-width"));
            if (field == "required_integer") candidate.UpdateRequiredInteger(value); else candidate.UpdateOptionalInteger(value);
            var before = provider.Snapshot();
            try { await candidate.AuditAs("reject out of range native Integer").SaveAsync(context); throw new Exception("native Integer overflow saved"); }
            catch (CheckException error) {
                Require(error.Violations.Any(v => v.RuleId == "invalid_type" && v.ModelPath == field)
                    && provider.Snapshot() == before, "modeled Integer width was not enforced before provider");
            }
            cases.Add(new { kind = "native-width", field, value, providerCalls = new Counts(0, 0, 0, 0) });
        }
        foreach (var (field, value) in new (string, object)[] {
            ("required_integer", (long)int.MaxValue + 1), ("optional_integer", (long)int.MinValue - 1),
            ("required_long", "PRIVATE_CORRUPT_INTEGER"), ("optional_long", 1.5d) })
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"UPDATE scalar_type_probe_data SET {field}=@bad WHERE id=@id";
            command.Parameters.AddWithValue("@bad", value); command.Parameters.AddWithValue("@id", retained[0].Id!.Value);
            await command.ExecuteNonQueryAsync();
            try {
                try { await Load(context, retained[0].Id!.Value); throw new Exception("corrupt stored Integer hydrated"); }
                catch (SqlExecutorException error) {
                    Require(error.Message.Contains("ScalarTypeProbe." + field) && !error.Message.Contains("PRIVATE_CORRUPT"), "corrupt Q read lost field context or leaked value");
                }
            } finally {
                command.Parameters["@bad"].Value = field switch {
                    "required_integer" => int.MinValue, "required_long" => long.MinValue, _ => DBNull.Value };
                await command.ExecuteNonQueryAsync();
            }
            cases.Add(new { kind = "corrupt-read", field });
        }
        foreach (var row in retained) Require((await Load(context, row.Id!.Value)).Version == 1, "rejected mutation changed a stored version");
        Require(cases.Count == 40, "integral case inventory drift");
        var report = new { schema = "teaql.dotnet-generated-integral-example/v1", passed = true, cases,
            checkCount = cases.Count, calibrated, calibrationEntityId = calibration.Id,
            rows = retained.Select(row => new { id = row.Id, version = row.Version }).ToArray(),
            generatedQ = true, generatedE = true, typedJson = true, providerCallsBeforeAdmission = 0 };
        Console.WriteLine("DOTNET_INTEGRAL=" + JsonSerializer.Serialize(report));
    }

    private static async Task<ScalarTypeProbe> NewAsync(UserContext context, string name) => (await Q.ScalarTypeProbes()
        .Comment("prepare complete generated integer fixture").Purpose("verify trusted model ranges").NewEntityAsync(context))
        .UpdateTypePlatform(1).UpdateRequiredString(name).UpdateRequiredBoolean(true).UpdateRequiredInteger(1).UpdateRequiredLong(1)
        .UpdateRequiredDecimal(1m).UpdateRequiredFloat(1.25).UpdateRequiredDouble(2.5)
        .UpdateRequiredDate(new DateTime(2026, 10, 20)).UpdateRequiredTime(new TimeSpan(12, 30, 0))
        .UpdateRequiredDatetime(new DateTime(2026, 10, 20, 12, 30, 0, DateTimeKind.Utc));
    private static async Task<ScalarTypeProbe> Load(UserContext context, long id) => await Q.ScalarTypeProbes().WithIdIs(id).Limit(1)
        .Comment("load exact generated integer fixture").Purpose("verify Q E and JSON without numeric narrowing")
        .ExecuteForOneAsync(context) ?? throw new Exception("integral fixture missing");
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private sealed class ReplacingChecker(string field, Value value) : IEntityChecker
    {
        internal int Calls { get; private set; }
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        {
            Calls++;
            var values = request is InsertMutationRequest insert ? insert.Command.Values : ((UpdateMutationRequest)request).Command.Values;
            values[field] = value; return [];
        }
    }
    private sealed record Counts(int Query, int Mutate, int Begin, int NextId);
    private sealed class Observed(SqlDataServiceExecutor inner) : IDataService, ITransactionExecutor, IIdGeneratorExecutor, ISchemaExecutor
    {
        private int query, mutate, begin, nextId;
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Counts Snapshot() => new(query, mutate, begin, nextId);
        public Task<QueryResult> QueryAsync(QueryRequest request) { query++; return inner.QueryAsync(request); }
        public Task<MutationResult> MutateAsync(MutationRequest request) { mutate++; return inner.MutateAsync(request); }
        public async Task<ITransaction> BeginTransactionAsync() {
            begin++; var transaction = await inner.BeginTransactionAsync();
            return new Transaction(transaction, this);
        }
        public Task<SchemaResult> EnsureSchemaAsync(SchemaRequest request) => ((ISchemaExecutor)inner).EnsureSchemaAsync(request);
        public Task<ulong> NextIdAsync(string entity) { nextId++; return inner.NextIdAsync(entity); }
        public Task EnsureIdFloorAsync(string entity, ulong floor) => inner.EnsureIdFloorAsync(entity, floor);
        private sealed class Transaction(ITransaction inner, Observed owner) : ITransaction, IInternalIdAllocator
        {
            public DataServiceCapabilities Capabilities => inner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) { owner.query++; return inner.QueryAsync(request); }
            public Task<MutationResult> MutateAsync(MutationRequest request) { owner.mutate++; return inner.MutateAsync(request); }
            public Task<ulong> NextIdAsync(string entity) { owner.nextId++; return ((IInternalIdAllocator)inner).NextIdAsync(entity); }
            public Task EnsureIdFloorAsync(string entity, ulong floor) => ((IInternalIdAllocator)inner).EnsureIdFloorAsync(entity, floor);
            public Task CommitAsync() => inner.CommitAsync(); public Task RollbackAsync() => inner.RollbackAsync(); public void Dispose() => inner.Dispose();
        }
    }
}
