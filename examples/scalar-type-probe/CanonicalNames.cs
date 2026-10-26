using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Text.Json;

// Raw ledger access and the SQLite trigger are explicit conformance fault
// injection. Application queries/mutations still use the generated Q/E APIs.
internal static class CanonicalNames
{
    internal static async Task RunAsync(string database)
    {
        await using var connection = new SqliteConnection($"Data Source={database}");
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module;
        var sql = new ObservedSql(new SqliteTransport(connection));
        var provider = new Observed(new SqlDataServiceExecutor(new SqliteDialect(), sql,
            new MetadataSchemaProvider(module.Metadata.GetEntity)));
        var checker = new CheckerRecorder(new ScalarTypeProbeChecker());
        var policy = new PolicyRecorder(); var audit = new AuditRecorder();
        var context = module.And(new RuntimeModule().Checker("ScalarTypeProbe", checker)).IntoContext()
            .WithDataService(provider).WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => policy))
            .WithAppAuditEventSink(audit);
        await context.EnsureSchemaAsync();
        var marker = "canonical-" + Guid.NewGuid().ToString("N");
        var cases = new List<object>(); var parents = new List<TypePlatform>();
        foreach (var suffix in new[] { "original", "replacement" })
        {
            var parent = Q.TypePlatforms().Comment("prepare canonical parent").Purpose("verify relation identity").NewEntity(context)
                .UpdateName(marker + "-" + suffix);
            await parent.AuditAs("create canonical naming parent").SaveAsync(context); parents.Add(parent);
        }
        var rows = new List<ScalarTypeProbe>();
        foreach (var suffix in new[] { "first", "second" })
        {
            var row = New(context, marker + "-" + suffix, parents[0].Id!.Value);
            var pending = Pending(row);
            Require(pending.ContainsKey("required_string") && pending.ContainsKey("type_platform")
                && !pending.ContainsKey("requiredString") && !pending.ContainsKey("type_platform_id"), "native updaters lost canonical keys");
            await row.AuditAs("create canonical naming row").SaveAsync(context); rows.Add(row);
        }
        cases.Add(new { @case = "NAME-01", canonicalScalar = true });
        var first = await Load(context, rows[0].Id!.Value, true);
        first.UpdateTypePlatform(parents[0].Id!.Value);
        Require(E.ScalarTypeProbe(first).TypePlatform().Name().Eval() == marker + "-original", "same identity discarded loaded detail");
        var relationProbe = await Load(context, first.Id!.Value, true);
        relationProbe.UpdateTypePlatformId(parents[1].Id!.Value);
        NotLoaded(() => E.ScalarTypeProbe(relationProbe).TypePlatform().Name().Eval());
        relationProbe.UpdateTypePlatform(parents[0]);
        Require(E.ScalarTypeProbe(relationProbe).TypePlatform().Name().Eval() == marker + "-original", "typed setter lost explicit detail");
        relationProbe.UpdateTypePlatformId(null);
        Require(E.ScalarTypeProbe(relationProbe).TypePlatform().Eval() is null
            && E.ScalarTypeProbe(relationProbe).TypePlatformId().Eval() is null, "known null became NotLoaded or retained old detail");
        first.UpdateTypePlatform(parents[1].Id!.Value);
        Require(Pending(first)["type_platform"].TryI64() == parents[1].Id && !Pending(first).ContainsKey("type_platform_id"), "relation update key");
        NotLoaded(() => E.ScalarTypeProbe(first).TypePlatform().Name().Eval());
        await first.AuditAs("change canonical forward identity").SaveAsync(context);
        cases.Add(new { @case = "NAME-02", createAndUpdate = true, invalidatedOldDetail = true });

        var invalid = new (string Name, Record Fields)[] {
            ("storage-alias", new() { ["type_platform_id"] = new Value.I64Value(parents[0].Id!.Value) }),
            ("native-alias", new() { ["typePlatform"] = new Value.I64Value(parents[0].Id!.Value) }),
            ("scalar-alias", new() { ["requiredString"] = new Value.TextValue("PRIVATE_BAD_ALIAS") }),
            ("unknown", new() { ["unknown_field"] = new Value.TextValue("PRIVATE_BAD_UNKNOWN") }),
            ("duplicate-equal", new() { ["type_platform"] = new Value.I64Value(parents[0].Id!.Value), ["type_platform_id"] = new Value.I64Value(parents[0].Id!.Value) }),
            ("duplicate-unequal", new() { ["type_platform"] = new Value.I64Value(parents[0].Id!.Value), ["type_platform_id"] = new Value.I64Value(parents[1].Id!.Value) })
        };
        var rejected = new List<string>();
        foreach (var update in new[] { false, true }) foreach (var item in invalid)
        {
            var candidate = update ? await Load(context, first.Id!.Value) : New(context, marker, parents[0].Id!.Value);
            foreach (var pair in item.Fields) candidate.TeaqlMutationLedger.Set(Key(candidate), pair.Key, pair.Value);
            var before = provider.Snapshot(); var beforeSql = sql.Calls;
            var hooks = (checker.Calls, policy.Plans.Count, audit.Events.Count);
            try { await candidate.AuditAs("reject malformed canonical ledger").SaveAsync(context); throw new Exception("malformed namespace saved: " + item.Name); }
            catch (CheckException error)
            {
                Require(error.Violations.Any(v => v.RuleId == "unknown_field") && !error.Message.Contains("PRIVATE_BAD"), "namespace diagnostic missing or leaked a value");
            }
            Require(provider.Snapshot() == before && sql.Calls == beforeSql
                && hooks == (checker.Calls, policy.Plans.Count, audit.Events.Count), "malformed ledger crossed checker/policy/provider/SQL");
            rejected.Add((update ? "update/" : "create/") + item.Name);
        }
        foreach (var update in new[] { false, true })
        {
            var candidate = update ? await Load(context, first.Id!.Value) : New(context, marker, parents[0].Id!.Value);
            candidate.UpdateRequiredString(null!); var before = provider.Snapshot(); var beforeSql = sql.Calls; var audits = audit.Events.Count;
            try { await candidate.AuditAs("reject canonical required value").SaveAsync(context); throw new Exception("missing required value saved"); }
            catch (CheckException error)
            {
                Require(error.Violations.Any(v => v.RuleId == "required" && v.ModelPath == "required_string"
                    && v.InstancePath == "/requiredString"), "required value lost canonical and JSON paths");
            }
            Require(provider.Snapshot() == before && sql.Calls == beforeSql && audit.Events.Count == audits, "required-value rejection reached provider");
        }
        Require(checker.Keys.Any(keys => keys.Contains("required_string") && keys.Contains("type_platform"))
            && checker.Keys.All(keys => !keys.Contains("type_platform_id") && !keys.Contains("requiredString")), "actual Checker vocabulary");
        var calibrated = provider.Snapshot();
        Require(calibrated.Query > 0 && calibrated.Mutate > 0 && calibrated.Begin > 0 && calibrated.NextId > 0 && sql.Calls > 0, "unused boundary observer");
        cases.Add(new { @case = "NAME-03", namespaceChecks = rejected, namespaceRejections = 12, requiredRejections = 2,
            rejectionProviderCalls = new Counts(0, 0, 0, 0), rejectionTransportCalls = 0, beforeCheckerAndPolicy = true });

        var columns = new List<string>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA table_info(scalar_type_probe_data)";
            using var reader = await command.ExecuteReaderAsync(); while (await reader.ReadAsync()) columns.Add(reader.GetString(1));
        }
        Require(columns.Contains("type_platform") && columns.Contains("required_string") && !columns.Contains("type_platform_id"), "physical columns renamed");
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT version,type_platform,required_string FROM scalar_type_probe_data WHERE id=@id";
            command.Parameters.AddWithValue("@id", first.Id!.Value); using var reader = await command.ExecuteReaderAsync();
            Require(await reader.ReadAsync() && reader.GetInt64(0) == 2 && reader.GetInt64(1) == parents[1].Id
                && reader.GetString(2) == marker + "-first", "physical relation/scalar write mismatch");
        }
        cases.Add(new { @case = "NAME-04", physicalColumns = columns, logicalForeignColumn = "type_platform" });
        first = await Load(context, first.Id!.Value, true);
        Require(E.ScalarTypeProbe(first).TypePlatform().Name().Eval() == marker + "-replacement"
            && E.ScalarTypeProbe(first).OptionalString().Eval() is null, "loaded reference/null mismatch");
        var sparse = await Q.ScalarTypeProbesWithMinimalFields().WithIdIs(first.Id.Value).SelectRequiredString().Limit(1)
            .Comment("read canonical sparse projection").Purpose("verify NotLoaded semantics").ExecuteForOneAsync(context)
            ?? throw new Exception("canonical sparse fixture missing");
        var noIo = provider.Snapshot(); var noSql = sql.Calls;
        NotLoaded(() => E.ScalarTypeProbe(sparse).OptionalString().Eval());
        Require(provider.Snapshot() == noIo && sql.Calls == noSql, "E performed implicit I/O");
        cases.Add(new { @case = "NAME-05", loadedNullAndNotLoaded = true, foreignIdentity = parents[1].Id!.Value.ToString(CultureInfo.InvariantCulture) });
        cases.Add(new { @case = "NAME-06", nativeGeneratedQEMutation = true });
        foreach (var profile in Enum.GetValues<JsonFieldNamingProfile>())
        {
            var (reference, scalar, id) = profile switch {
                JsonFieldNamingProfile.SnakeCase => ("type_platform", "required_string", "id"),
                JsonFieldNamingProfile.PascalCase => ("TypePlatform", "RequiredString", "Id"),
                _ => ("typePlatform", "requiredString", "id") };
            var wire = first.ToJson(profile); using var document = JsonDocument.Parse(wire); var data = document.RootElement;
            Require(data.GetProperty(reference).GetProperty(id).GetString() == parents[1].Id!.Value.ToString(CultureInfo.InvariantCulture)
                && data.GetProperty(scalar).GetString() == marker + "-first" && !data.TryGetProperty("type_platform_id", out _), "wire profile leaked storage names or lost target");
            var decoded = ScalarTypeProbe.FromJson(wire, profile);
            Require(E.ScalarTypeProbe(decoded).TypePlatform().Id().Eval() == parents[1].Id, "typed JSON target identity");
            var duplicate = wire[..wire.LastIndexOf('}')] + "," + JsonSerializer.Serialize(reference) + ":" + data.GetProperty(reference).GetRawText() + "}";
            try { ScalarTypeProbe.FromJson(duplicate, profile); throw new Exception("equal wire collision accepted"); }
            catch (EntityJsonException error) { Require(error.Code == "WIRE_FIELD_COLLISION" && error.Location.ModelPath == "type_platform", "wire collision code/path"); }
        }
        cases.Add(new { @case = "NAME-07", defaultWire = new[] { "requiredString", "typePlatform" }, profiles = 3,
            equalCollisionRejected = true, typedTargetPreserved = true });

        first.UpdateTypePlatform(parents[0].Id!.Value);
        await first.AuditAs("restore canonical graph membership").SaveAsync(context);
        var graph = await Q.TypePlatforms().WithIdIs(parents[0].Id!.Value).Limit(1)
            .SelectScalarTypeProbeListWith(Q.ScalarTypeProbes().OrderByIdAscending().Limit(2))
            .Comment("load canonical two-child graph").Purpose("verify rollback and same-graph retry").ExecuteForOneAsync(context)
            ?? throw new Exception("canonical graph missing");
        var children = graph.ScalarTypeProbeList.ToArray();
        Require(children.Length == 2 && children.Select(child => child.Id).Order().SequenceEqual(rows.Select(row => row.Id).Order()), "loaded graph identity drift");
        var versions = children.ToDictionary(child => child.Id!.Value, child => child.Version!.Value);
        // Keep explicit expected values per typed identity, independent of list position after save.
        var expectedValues = children.Select((child, index) => (Id: child.Id!.Value, Text: marker + "-graph-" + index)).ToDictionary(pair => pair.Id, pair => pair.Text);
        foreach (var child in children)
        {
            child.UpdateRequiredString(expectedValues[child.Id!.Value]).AuditAs("canonical child update");
            Require(Pending(child).Keys.SequenceEqual(new[] { "required_string" }), "unexpected initial pending fields");
        }
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"CREATE TEMP TRIGGER deny_name_second BEFORE UPDATE ON scalar_type_probe_data WHEN NEW.id={children[1].Id!.Value} BEGIN SELECT RAISE(ABORT,'NAME_SECOND_WRITE'); END";
            await command.ExecuteNonQueryAsync();
        }
        var successful = sql.SuccessfulTargetUpdates; var rollbacks = sql.Rollbacks; var beforeAudits = audit.Events.Count;
        try { await graph.AuditAs("save canonical two-child graph").SaveAsync(context); throw new Exception("second real SQLite update did not fail"); }
        catch (SqlExecutorException error) when (error.ToString().Contains("NAME_SECOND_WRITE")) { }
        finally
        {
            using var command = connection.CreateCommand(); command.CommandText = "DROP TRIGGER deny_name_second"; await command.ExecuteNonQueryAsync();
        }
        Require(sql.SuccessfulTargetUpdates == successful + 1 && sql.Rollbacks == rollbacks + 1 && audit.Events.Count == beforeAudits,
            "first write not rolled back or premature commit audit escaped");
        foreach (var child in children)
        {
            var untouched = await Load(context, child.Id!.Value);
            Require(untouched.Version == versions[child.Id.Value] && E.ScalarTypeProbe(untouched).RequiredString().Eval() != expectedValues[child.Id.Value]
                && Pending(child).ContainsKey("required_string"), "rollback lost persisted or pending state");
        }
        await graph.AuditAs("save canonical two-child graph").SaveAsync(context);
        var committed = audit.Events.Skip(beforeAudits).ToArray();
        Require(committed.Length == 2 && committed.Select(item => Convert.ToInt64(item["entityId"], CultureInfo.InvariantCulture)).Order().SequenceEqual(versions.Keys.Order()), "retry audit targets");
        foreach (var item in committed)
        {
            Require((string?)item["entityType"] == "ScalarTypeProbe" && item["changedFields"] is string[] fields
                && fields.Contains("required_string") && !fields.Contains("type_platform_id"), "canonical audit fields");
            Require(item["traceChain"] is TraceNode[] chain && chain.Any(node => node.Detail == "canonical child update"), "local child reason lost");
        }
        foreach (var child in children)
        {
            var changed = await Load(context, child.Id!.Value);
            Require(changed.Version == versions[child.Id.Value] + 1 && E.ScalarTypeProbe(changed).RequiredString().Eval() == expectedValues[child.Id.Value]
                && E.ScalarTypeProbe(changed).TypePlatformId().Eval() == parents[0].Id && Pending(child).Count == 0, "retry value/version/identity/pending mismatch");
        }
        Require(policy.Plans.SelectMany(plan => plan.Operations).All(operation => !operation.ChangedValues.ContainsKey("type_platform_id")
            && !operation.ChangedValues.ContainsKey("requiredString")), "normalized policy vocabulary");
        cases.Add(new { @case = "NAME-08", firstUpdateRolledBack = true, failedAuditCount = 0, pendingIntentRetained = true,
            retryAudits = 2, typedTargets = true, canonicalPolicyAndAudit = true, versionsBefore = versions,
            rollbackSQL = new { executedUpdates = 1, rollbacks = 1 } });
        var report = new { schema = "teaql.dotnet-generated-canonical-name-example/v1", passed = true, cases,
            childIds = rows.Select(row => row.Id!.Value.ToString(CultureInfo.InvariantCulture)),
            parentIds = parents.Select(row => row.Id!.Value.ToString(CultureInfo.InvariantCulture)),
            calibratedProviderCalls = calibrated, physicalSchemaCrossRuntimeAccepted = false };
        var reportJson = JsonSerializer.Serialize(report);
        await File.WriteAllTextAsync(Path.Combine(Path.GetDirectoryName(database)!, marker + ".report.json"), reportJson + Environment.NewLine);
        Console.WriteLine("DOTNET_CANONICAL_NAME=" + reportJson);
    }

    private static ScalarTypeProbe New(UserContext context, string name, long parent) => Q.ScalarTypeProbes()
        .Comment("prepare canonical naming fixture").Purpose("verify generated mutation boundaries").NewEntity(context)
        .UpdateTypePlatform(parent).UpdateRequiredString(name).UpdateRequiredBoolean(true).UpdateRequiredInteger(1).UpdateRequiredLong(1)
        .UpdateRequiredDecimal(3.50m).UpdateRequiredFloat(1.25).UpdateRequiredDouble(2.5)
        .UpdateRequiredDate(new DateTime(2026, 10, 20)).UpdateRequiredTime(new TimeSpan(12, 30, 0))
        .UpdateRequiredDatetime(new DateTime(2026, 10, 20, 12, 30, 0, DateTimeKind.Utc));
    private static async Task<ScalarTypeProbe> Load(UserContext context, long id, bool forward = false)
    {
        var query = Q.ScalarTypeProbes().WithIdIs(id).Limit(1);
        if (forward) query = query.SelectTypePlatformWith(Q.TypePlatforms().Limit(1));
        return await query.Comment("reload canonical naming fixture").Purpose("verify Q E persisted state").ExecuteForOneAsync(context)
            ?? throw new Exception("canonical fixture missing");
    }
    private static EntityKey Key(ScalarTypeProbe row) => row.Id.HasValue ? new("ScalarTypeProbe", row.Id.Value)
        : row.TeaqlMutationLedger.Changes().Keys.Single(key => key.EntityType == "ScalarTypeProbe");
    private static Record Pending(ScalarTypeProbe row) => row.TeaqlMutationLedger.Change(Key(row));
    private static void NotLoaded(Func<object?> access) { try { access(); } catch (TeaQLNotLoadedException) { return; } throw new Exception("missing field became loaded"); }
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private sealed class CheckerRecorder(IEntityChecker original) : IEntityChecker
    {
        public int Calls { get; private set; }
        public List<string[]> Keys { get; } = [];
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        {
            Calls++; Keys.Add((request is InsertMutationRequest insert ? insert.Command.Values : ((UpdateMutationRequest)request).Command.Values).Keys.ToArray());
            return original.CheckAndFix(context, request, now);
        }
    }
    private sealed class PolicyRecorder : IMutationPolicy
    {
        public MutationPolicyIdentity Identity { get; } = new("canonical-name", "1", "sha256:canonical-name");
        public List<MutationPlan> Plans { get; } = [];
        public MutationDecision Review(UserContext context, MutationPlan plan) { Plans.Add(plan); return MutationDecision.Allowed(); }
    }
    private sealed class AuditRecorder : IAppAuditEventSink
    {
        public List<IReadOnlyDictionary<string, object?>> Events { get; } = [];
        public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken cancellationToken = default) { Events.Add(value); return Task.CompletedTask; }
    }
    private sealed record Counts(int Query, int Mutate, int Begin, int NextId);
    private sealed class Observed(SqlDataServiceExecutor inner) : IDataService, ITransactionExecutor, IIdGeneratorExecutor, ISchemaExecutor
    {
        private int query, mutate, begin, nextId;
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Counts Snapshot() => new(query, mutate, begin, nextId);
        public Task<QueryResult> QueryAsync(QueryRequest request) { query++; return inner.QueryAsync(request); }
        public Task<MutationResult> MutateAsync(MutationRequest request) { mutate++; return inner.MutateAsync(request); }
        public async Task<ITransaction> BeginTransactionAsync() { begin++; return new Transaction(await inner.BeginTransactionAsync(), this); }
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
    private sealed class ObservedSql(SqliteTransport inner) : IStreamingSqlTransport, IAutomaticMutationTransactionTransport,
        ISchemaConnectionInitializer, ISchemaCompatibilityVerifier
    {
        public int Calls { get; private set; }
        public int SuccessfulTargetUpdates { get; private set; }
        public int Rollbacks { get; private set; }
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Calls++; return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Calls++; return inner.ExecuteSqlAsync(query); }
        public IAsyncEnumerable<Record> StreamSqlAsync(CompiledQuery query, CancellationToken cancellationToken = default) { Calls++; return inner.StreamSqlAsync(query, cancellationToken); }
        public async Task<ISqlTransaction> BeginSqlAsync() { Calls++; return new Transaction(await inner.BeginSqlAsync(), this); }
        public Task EnsureSchemaFunctionsAsync() => inner.EnsureSchemaFunctionsAsync();
        public Task VerifySchemaCompatibilityAsync(SqlDialect dialect, EntityDescriptor entity) => inner.VerifySchemaCompatibilityAsync(dialect, entity);
        private sealed class Transaction(ISqlTransaction inner, ObservedSql owner) : ISqlTransaction
        {
            public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { owner.Calls++; return inner.FetchAllSqlAsync(query); }
            public async Task<ulong> ExecuteSqlAsync(CompiledQuery query)
            {
                owner.Calls++; var rows = await inner.ExecuteSqlAsync(query);
                if (rows > 0 && query.Sql.TrimStart().StartsWith("UPDATE scalar_type_probe_data", StringComparison.OrdinalIgnoreCase)) owner.SuccessfulTargetUpdates++;
                return rows;
            }
            public Task CommitSqlAsync() => inner.CommitSqlAsync();
            public Task RollbackSqlAsync() { owner.Rollbacks++; return inner.RollbackSqlAsync(); }
            public void Dispose() => inner.Dispose();
        }
    }
}
