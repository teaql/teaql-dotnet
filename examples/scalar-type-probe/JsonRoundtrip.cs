using System.Text.Json;
using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;

internal static class EntityJsonExample
{
    public static async Task RunAsync(string database)
    {
        await using var connection = new SqliteConnection($"Data Source={database}");
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module;
        var observed = new JsonObservedProvider(new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
            new MetadataSchemaProvider(module.Metadata.GetEntity)));
        var context = module.IntoContext().WithDataService(observed);
        await context.EnsureSchemaAsync();
        var checks = new List<string>();
        void Check(bool condition, string name) { if (!condition) throw new Exception(name); checks.Add(name); }
        void NotLoaded(Action access, string name)
        {
            var before = observed.Snapshot();
            try { access(); throw new Exception(name + ": unselected value became loaded"); }
            catch (TeaQLNotLoadedException) { Check(observed.Snapshot() == before, name); }
        }
        async Task<ScalarTypeProbe> Load(long id) => await Q.ScalarTypeProbes().WithIdIs(id).Limit(1)
            .Comment("reload exact entity JSON values").Purpose("verify the generated SQLite JSON contract")
            .ExecuteForOneAsync(context) ?? throw new Exception("JSON fixture was not persisted");

        var original = Q.ScalarTypeProbes().Comment("prepare exact entity JSON values")
            .Purpose("verify the generated audited mutation contract").NewEntity(context)
            .UpdateTypePlatform(1).UpdateRequiredString("entity-json-" + Guid.NewGuid().ToString("N"))
            .UpdateOptionalString("").UpdateLargeText("Unicode scalar fixture: 精确数值")
            .UpdateRequiredBoolean(false).UpdateOptionalBoolean(false)
            .UpdateRequiredInteger(123).UpdateOptionalInteger(int.MinValue)
            .UpdateRequiredLong(9007199254740993).UpdateOptionalLong(long.MinValue)
            .UpdateRequiredDecimal(9007199254740993.123456789m).UpdateOptionalDecimal(0.10m)
            .UpdateRequiredFloat(1.25).UpdateRequiredDouble(2.5)
            .UpdateRequiredDate(new DateTime(2024, 2, 29))
            .UpdateRequiredTime(new TimeSpan(23, 59, 59).Add(TimeSpan.FromTicks(1234567)))
            .UpdateRequiredDatetime(new DateTime(2024, 2, 29, 12, 34, 56, 789, DateTimeKind.Utc));
        await original.AuditAs("create generated entity JSON fixture").SaveAsync(context);
        var loaded = await Load(original.Id!.Value);
        var expected = ReadValues(loaded);
        Check((long?)expected["required_long"] == 9007199254740993, "SQLite exact Long");
        Check((TimeSpan?)expected["required_time"] == new TimeSpan(23, 59, 59).Add(TimeSpan.FromTicks(1234567)), "SQLite native Time ticks");
        Check((DateTime?)expected["required_datetime"] == new DateTime(2024, 2, 29, 12, 34, 56, 789, DateTimeKind.Utc), "SQLite UTC milliseconds");
        var calibration = observed.Snapshot();
        Check(calibration.Query > 0 && calibration.Mutate > 0 && calibration.Begin > 0 && calibration.NextId > 0, "calibrated provider observer");

        foreach (var profile in Enum.GetValues<JsonFieldNamingProfile>())
        {
            var before = observed.Snapshot();
            var json = loaded.ToJson(profile);
            using var document = JsonDocument.Parse(json);
            var restored = ScalarTypeProbe.FromJson(json, profile);
            var actual = ReadValues(restored);
            foreach (var (field, value) in expected)
                Check(value?.GetType() == actual[field]?.GetType() && Equals(value, actual[field]), $"{profile} native {field}");
            var names = profile switch {
                JsonFieldNamingProfile.SnakeCase => new[] { "required_long", "required_decimal", "optional_decimal", "optional_date", "optional_boolean", "optional_string" },
                JsonFieldNamingProfile.PascalCase => new[] { "RequiredLong", "RequiredDecimal", "OptionalDecimal", "OptionalDate", "OptionalBoolean", "OptionalString" },
                _ => new[] { "requiredLong", "requiredDecimal", "optionalDecimal", "optionalDate", "optionalBoolean", "optionalString" }
            };
            Check(document.RootElement.GetProperty(names[0]).GetString() == "9007199254740993", profile + " exact Long string");
            Check(document.RootElement.GetProperty(names[1]).GetString() == "9007199254740993.123456789", profile + " exact Decimal string");
            Check(document.RootElement.GetProperty(names[2]).GetString() == "0.10", profile + " Decimal scale");
            Check(document.RootElement.GetProperty(names[3]).ValueKind == JsonValueKind.Null, profile + " loaded null");
            Check(!document.RootElement.GetProperty(names[4]).GetBoolean(), profile + " false Boolean");
            Check(document.RootElement.GetProperty(names[5]).GetString() == "", profile + " empty string");
            Check(!document.RootElement.EnumerateObject().Any(field => field.Name.StartsWith('_')
                || field.Name.Contains("Ledger") || field.Name.Contains("Comment") || field.Name.Contains("DynamicProperties")), profile + " no private state");
            Check(observed.Snapshot() == before, profile + " JSON and E perform zero provider calls");
        }

        var sparse = await Q.ScalarTypeProbesWithMinimalFields().WithIdIs(original.Id.Value).SelectRequiredString().Limit(1)
            .Comment("load sparse entity JSON").Purpose("preserve omitted fields as NotLoaded").ExecuteForOneAsync(context)
            ?? throw new Exception("sparse fixture absent");
        var sparseJson = sparse.ToJson();
        using (var document = JsonDocument.Parse(sparseJson))
            Check(document.RootElement.EnumerateObject().Select(field => field.Name).Order().SequenceEqual(new[] { "id", "requiredString", "version" }), "only selected sparse fields encoded");
        var restoredSparse = ScalarTypeProbe.FromJson(sparseJson);
        NotLoaded(() => E.ScalarTypeProbe(restoredSparse).OptionalDecimal().Eval(), "sparse optional field is NotLoaded, not null");
        restoredSparse.UpdateRequiredString("forbidden-partial-edit");
        var beforeSparseSave = observed.Snapshot();
        try { await restoredSparse.AuditAs("reject sparse JSON edit").SaveAsync(context); throw new Exception("sparse JSON save accepted"); }
        catch (CheckException) { Check(observed.Snapshot() == beforeSparseSave, "partial JSON save rejected before provider I/O"); }

        var reverse = await Q.TypePlatformsWithMinimalFields().WithIdIs(1).Limit(1)
            .SelectScalarTypeProbeListWith(Q.ScalarTypeProbesWithMinimalFields().WithIdIs(original.Id.Value)
                .SelectRequiredString().SelectRequiredDecimal().OrderByIdAscending().Limit(1))
            .Comment("load bounded reverse JSON tree").Purpose("verify typed nested projection state").ExecuteForOneAsync(context)
            ?? throw new Exception("reverse fixture absent");
        var beforeReverse = observed.Snapshot();
        var reverseE = E.TypePlatform(TypePlatform.FromJson(reverse.ToJson()));
        Check(reverseE.ScalarTypeProbeList().Size().Eval() == 1, "typed reverse JSON list");
        Check(reverseE.ScalarTypeProbeList().First().RequiredDecimal().Eval() == 9007199254740993.123456789m, "typed nested Decimal");
        NotLoaded(() => reverseE.ScalarTypeProbeList().First().RequiredLong().Eval(), "nested sparse Long remains NotLoaded");
        NotLoaded(() => reverseE.Name().Eval(), "parent sparse name remains NotLoaded");
        Check(observed.Snapshot() == beforeReverse, "nested JSON uses zero provider calls");

        var forward = await Q.ScalarTypeProbesWithMinimalFields().WithIdIs(original.Id.Value).Limit(1)
            .SelectTypePlatformWith(Q.TypePlatformsWithMinimalFields().Limit(1))
            .Comment("load sparse forward JSON detail").Purpose("preserve foreign identity and target projection")
            .ExecuteForOneAsync(context) ?? throw new Exception("forward fixture absent");
        var beforeForward = observed.Snapshot();
        var forwardE = E.ScalarTypeProbe(ScalarTypeProbe.FromJson(forward.ToJson()));
        Check(forwardE.TypePlatformId().Eval() == 1 && forwardE.TypePlatform().Id().Eval() == 1, "forward JSON identity and typed target");
        NotLoaded(() => forwardE.TypePlatform().Name().Eval(), "forward sparse name remains NotLoaded");
        Check(observed.Snapshot() == beforeForward, "forward JSON uses zero provider calls");
        var filtered = await Q.ScalarTypeProbesWithMinimalFields().WithIdIs(original.Id.Value).Limit(1)
            .SelectTypePlatformWith(Q.TypePlatformsWithMinimalFields().WithIdIs(999999999).Limit(1))
            .Comment("exclude detail without changing foreign identity").Purpose("verify filtered state survives JSON")
            .ExecuteForOneAsync(context) ?? throw new Exception("filtered owner absent");
        var filteredE = E.ScalarTypeProbe(ScalarTypeProbe.FromJson(filtered.ToJson()));
        Check(filteredE.TypePlatformId().Eval() == 1, "filtered FK remains loaded");
        NotLoaded(() => filteredE.TypePlatform().Name().Eval(), "filtered detail remains NotLoaded");

        var nullReference = ScalarTypeProbe.FromJson("{\"id\":\"42\",\"version\":\"1\",\"typePlatform\":null}");
        Check(E.ScalarTypeProbe(nullReference).TypePlatformId().Eval() is null
            && E.ScalarTypeProbe(nullReference).TypePlatform().Eval() is null, "null FK is a loaded null target");

        foreach (var field in new[] { "required_float", "required_double" })
        foreach (var number in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
        {
            var candidate = ScalarTypeProbe.FromJson(loaded.ToJson());
            if (field == "required_float") candidate.UpdateRequiredFloat(number); else candidate.UpdateRequiredDouble(number);
            var before = observed.Snapshot();
            try { candidate.ToJson(); throw new Exception("nonfinite JSON accepted"); }
            catch (EntityJsonException error) { Check(error.Location.ModelPath == field && observed.Snapshot() == before, $"{field} nonfinite zero provider {number}"); }
        }

        var left = ScalarTypeProbe.FromJson(loaded.ToJson());
        var right = ScalarTypeProbe.FromJson(loaded.ToJson());
        right.UpdateOptionalString("pending-independent-right"); left.UpdateOptionalString("committed-independent-left");
        await left.AuditAs("persist independent decoded JSON object").SaveAsync(context);
        var committed = await Load(original.Id.Value);
        Check(committed.Version == loaded.Version + 1 && E.ScalarTypeProbe(committed).OptionalString().Eval() == "committed-independent-left", "decoded JSON audited save and version");
        try { await right.AuditAs("reject stale independent JSON edit").SaveAsync(context); throw new Exception("stale JSON save accepted or discarded"); }
        catch (InvalidOperationException error) { Check(error.Message.Contains("did not return authoritative persisted state"), "independent pending JSON mutation retained"); }
        var final = await Load(original.Id.Value);
        Check(final.Version == committed.Version && E.ScalarTypeProbe(final).OptionalString().Eval() == "committed-independent-left", "stale JSON edit preserves committed state");

        var counters = observed.Snapshot();
        var report = new { schema = "teaql.dotnet-generated-entity-json-example/v1", passed = true,
            profiles = Enum.GetValues<JsonFieldNamingProfile>().Select(value => value.ToString()).ToArray(),
            businessScalarFields = 21, checkCount = checks.Count, checks,
            providerCalls = new { query = counters.Query, mutate = counters.Mutate, begin = counters.Begin, next_id = counters.NextId },
            entityId = original.Id.Value.ToString(System.Globalization.CultureInfo.InvariantCulture), finalVersion = final.Version,
            limitations = new[] { "Entity JSON is not transport authorization or a context-bound reference adapter",
                "Query/facet metadata is not an entity document", "Stale-save diagnostic taxonomy remains separate" } };
        var reportDirectory = Environment.GetEnvironmentVariable("TEAQL_EXAMPLES_EVIDENCE_DIR")
            ?? Environment.GetEnvironmentVariable("TEAQL_SCALAR_EVIDENCE_DIR") ?? Path.GetDirectoryName(database)!;
        Directory.CreateDirectory(reportDirectory);
        await File.WriteAllTextAsync(Path.Combine(reportDirectory, "entity-json-" + Guid.NewGuid().ToString("N") + ".report.json"), JsonSerializer.Serialize(report) + "\n");
        Console.WriteLine("DOTNET_ENTITY_JSON=" + JsonSerializer.Serialize(report));
    }

    private static Dictionary<string, object?> ReadValues(ScalarTypeProbe entity)
    {
        var e = E.ScalarTypeProbe(entity);
        return new() {
            ["required_string"] = e.RequiredString().Eval(), ["optional_string"] = e.OptionalString().Eval(), ["large_text"] = e.LargeText().Eval(),
            ["required_boolean"] = e.RequiredBoolean().Eval(), ["optional_boolean"] = e.OptionalBoolean().Eval(),
            ["required_integer"] = e.RequiredInteger().Eval(), ["optional_integer"] = e.OptionalInteger().Eval(),
            ["required_long"] = e.RequiredLong().Eval(), ["optional_long"] = e.OptionalLong().Eval(),
            ["required_decimal"] = e.RequiredDecimal().Eval(), ["optional_decimal"] = e.OptionalDecimal().Eval(),
            ["required_float"] = e.RequiredFloat().Eval(), ["optional_float"] = e.OptionalFloat().Eval(),
            ["required_double"] = e.RequiredDouble().Eval(), ["optional_double"] = e.OptionalDouble().Eval(),
            ["required_date"] = e.RequiredDate().Eval(), ["optional_date"] = e.OptionalDate().Eval(),
            ["required_time"] = e.RequiredTime().Eval(), ["optional_time"] = e.OptionalTime().Eval(),
            ["required_datetime"] = e.RequiredDatetime().Eval(), ["optional_datetime"] = e.OptionalDatetime().Eval(),
            ["id"] = e.Id().Eval(), ["version"] = e.Version().Eval(), ["type_platform"] = e.TypePlatformId().Eval(),
            ["create_time"] = e.CreateTime().Eval(), ["update_time"] = e.UpdateTime().Eval()
        };
    }

    private sealed record Counts(int Query, int Mutate, int Begin, int NextId);
    private sealed class JsonObservedProvider(SqlDataServiceExecutor inner) : IDataService, ITransactionExecutor, IIdGeneratorExecutor, ISchemaExecutor
    {
        private int query, mutate, begin, nextId;
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Counts Snapshot() => new(query, mutate, begin, nextId);
        public Task<QueryResult> QueryAsync(QueryRequest request) { query++; return inner.QueryAsync(request); }
        public Task<MutationResult> MutateAsync(MutationRequest request) { mutate++; return inner.MutateAsync(request); }
        public async Task<ITransaction> BeginTransactionAsync()
        {
            begin++; var transaction = await inner.BeginTransactionAsync();
            if (transaction is not IInternalIdAllocator) { await transaction.RollbackAsync(); transaction.Dispose(); throw new Exception("SQLite transaction lost ID allocator"); }
            return new ObservedTransaction(transaction, this);
        }
        public Task<SchemaResult> EnsureSchemaAsync(SchemaRequest request) => ((ISchemaExecutor)inner).EnsureSchemaAsync(request);
        public Task<ulong> NextIdAsync(string entity) { nextId++; return inner.NextIdAsync(entity); }
        public Task EnsureIdFloorAsync(string entity, ulong floor) => inner.EnsureIdFloorAsync(entity, floor);
        private sealed class ObservedTransaction(ITransaction inner, JsonObservedProvider owner) : ITransaction, IInternalIdAllocator
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
