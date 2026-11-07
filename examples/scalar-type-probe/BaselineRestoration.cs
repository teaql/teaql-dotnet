using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;

// Application-owned probe. Raw ledger writes are explicit conformance fault
// injection; all business reads and saves use the generated Q/E/mutation APIs.
internal static class BaselineRestoration
{
    internal static async Task RunAsync(string database)
    {
        await using var connection = new SqliteConnection($"Data Source={database}");
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module;
        var provider = new Observed(new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
            new MetadataSchemaProvider(module.Metadata.GetEntity)));
        var policy = new Policy(); var audit = new Audit(); var sql = new SqlLog();
        var context = module.IntoContext().WithDataService(provider)
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => policy))
            .WithAppAuditEventSink(audit).WithDiagnosticSqlLogSink(sql)
            .WithBusinessClock(new FixedBusinessClock(DateTimeOffset.Parse("2032-03-01T12:30:00Z")));
        await context.EnsureSchemaAsync();
        var marker = "restore-" + Guid.NewGuid().ToString("N");
        var created = (await Q.ScalarTypeProbes().Comment("prepare restoration fixture")
            .Purpose("verify known persisted baseline").NewEntityAsync(context))
            .UpdateTypePlatform(1).UpdateRequiredString(marker).UpdateRequiredBoolean(true)
            .UpdateRequiredInteger(37).UpdateRequiredLong(8).UpdateRequiredDecimal(37m)
            .UpdateRequiredFloat(1.25).UpdateRequiredDouble(0.0)
            .UpdateRequiredDate(new DateTime(2026, 10, 20)).UpdateRequiredTime(new TimeSpan(12, 30, 0))
            .UpdateRequiredDatetime(new DateTime(2026, 10, 20, 12, 30, 0, DateTimeKind.Utc));
        await created.AuditAs("seed restoration through generated mutation API").SaveAsync(context);
        var id = created.Id!.Value;
        var cases = new List<string>();
        var loaded = await Load(context, id);
        var baseline = loaded.Version!.Value;
        loaded.UpdateRequiredInteger(99).UpdateRequiredInteger(37);
        await NoOp(loaded, "known-value");

        loaded = await Load(context, id);
        Require(E.ScalarTypeProbe(loaded).OptionalString().Eval() is null, "fixture null is not loaded null");
        loaded.UpdateOptionalString("temporary").UpdateOptionalString(null!);
        await NoOp(loaded, "known-null");

        loaded = await Load(context, id);
        loaded.TeaqlMutationLedger.Set(new EntityKey("ScalarTypeProbe", id), "required_integer", new Value.I64Value(99));
        loaded.TeaqlMutationLedger.Set(new EntityKey("ScalarTypeProbe", id), "required_integer", new Value.I64Value(37));
        await NoOp(loaded, "raw-canonical-value");

        loaded = await Q.ScalarTypeProbesWithMinimalFields().WithIdIs(id).SelectRequiredInteger().Limit(1)
            .Comment("load known value in sparse view").Purpose("verify no-op preserves sparse geometry").ExecuteForOneAsync(context)
            ?? throw new Exception("sparse restoration fixture missing");
        loaded.UpdateRequiredInteger(99).UpdateRequiredInteger(37);
        await NoOp(loaded, "sparse-known-value");
        NotLoaded(() => E.ScalarTypeProbe(loaded).OptionalString().Eval());

        loaded = await Q.ScalarTypeProbesWithMinimalFields().WithIdIs(id).SelectRequiredInteger().Limit(1)
            .Comment("load unknown optional original").Purpose("do not mistake backing null for loaded null").ExecuteForOneAsync(context)
            ?? throw new Exception("unknown-original fixture missing");
        loaded.UpdateOptionalString(null!);
        var rejectedBefore = provider.Snapshot(); var rejectedHooks = Hooks();
        try { await loaded.AuditAs("reject changed incomplete view").SaveAsync(context); throw new Exception("partial edit was saved"); }
        catch (CheckException) { }
        Require(provider.Snapshot() == rejectedBefore && Hooks() == rejectedHooks
            && loaded.TeaqlMutationLedger.Change(new EntityKey("ScalarTypeProbe", id)).ContainsKey("optional_string"),
            "unknown original was discarded or rejection reached provider");
        cases.Add("unknown-original-retained");

        loaded = await Load(context, id);
        loaded.UpdateRequiredInteger(99).UpdateRequiredInteger(37);
        loaded.TeaqlMutationLedger.Set(new EntityKey("ScalarTypeProbe", id), "requiredInteger", new Value.I64Value(37));
        rejectedBefore = provider.Snapshot(); rejectedHooks = Hooks();
        try { await loaded.AuditAs("reject equal malformed raw key").SaveAsync(context); throw new Exception("invalid alias was saved"); }
        catch (CheckException error) { Require(error.Violations.Any(v => v.RuleId == "unknown_field"), "raw namespace rejection missing"); }
        Require(provider.Snapshot() == rejectedBefore && Hooks() == rejectedHooks
            && loaded.TeaqlMutationLedger.Change(new EntityKey("ScalarTypeProbe", id)).Count == 2,
            "normalization erased malformed input before admission");
        cases.Add("invalid-namespace-before-discard");

        context.WithBusinessClock(new FixedBusinessClock(DateTimeOffset.Parse("2032-04-01T09:15:00Z")));
        loaded = await Load(context, id);
        loaded.UpdateRequiredInteger(99).UpdateRequiredInteger(37).UpdateRequiredLong(9);
        var realBefore = provider.Snapshot(); var realHooks = Hooks();
        await loaded.AuditAs("persist the other genuine edit").SaveAsync(context);
        Require(loaded.Version == baseline + 1 && E.ScalarTypeProbe(loaded).RequiredLong().Eval() == 9
            && E.ScalarTypeProbe(loaded).RequiredInteger().Eval() == 37
            && E.ScalarTypeProbe(loaded).UpdateTime().Eval() == DateTimeOffset.Parse("2032-04-01T09:15:00Z").UtcDateTime,
            "restoring one field lost the other edit, fixed business time or version");
        RealWrite(realBefore, realHooks); cases.Add("remaining-edit-and-business-clock");

        loaded = await Load(context, id); loaded.UpdateRequiredDecimal(37.00m);
        realBefore = provider.Snapshot(); realHooks = Hooks();
        await loaded.AuditAs("retain exact Decimal scale intent").SaveAsync(context);
        Require(loaded.Version == baseline + 2, "Decimal scale change was normalized away");
        RealWrite(realBefore, realHooks); cases.Add("decimal-scale-intent");

        loaded = await Load(context, id); loaded.UpdateRequiredDouble(-0.0);
        realBefore = provider.Snapshot(); realHooks = Hooks();
        await loaded.AuditAs("retain signed floating zero intent").SaveAsync(context);
        Require(loaded.Version == baseline + 3, "signed zero change was normalized away");
        RealWrite(realBefore, realHooks); cases.Add("signed-zero-intent-not-storage-bits");

        var graph = await Q.TypePlatforms().WithIdIs(1).Limit(1)
            .SelectScalarTypeProbeListWith(Q.ScalarTypeProbes().WithIdIs(id).Limit(1))
            .Comment("load parent and one bounded child").Purpose("verify restored parent still visits dirty child")
            .ExecuteForOneAsync(context) ?? throw new Exception("restoration graph missing");
        var parentVersion = graph.Version; var parentName = graph.Name;
        graph.UpdateName("temporary parent").UpdateName(parentName!);
        var child = graph.ScalarTypeProbeList.Single(); child.UpdateRequiredInteger(38);
        realBefore = provider.Snapshot(); realHooks = Hooks();
        await graph.AuditAs("save child under restored parent").SaveAsync(context);
        Require(graph.Version == parentVersion && child.Version == baseline + 4
            && E.ScalarTypeProbe(child).RequiredInteger().Eval() == 38, "clean parent discarded dirty child or bumped its own version");
        RealWrite(realBefore, realHooks); cases.Add("restored-parent-dirty-child");

        loaded = await Load(context, id); loaded.UpdateRequiredInteger(99).UpdateRequiredInteger(38).MarkForDeletion();
        realBefore = provider.Snapshot(); realHooks = Hooks();
        await loaded.AuditAs("keep pending deletion after field restoration").SaveAsync(context);
        RealWrite(realBefore, realHooks); cases.Add("pending-deletion");
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT required_integer,required_long,version,update_time FROM scalar_type_probe_data WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            using var reader = await command.ExecuteReaderAsync();
            Require(await reader.ReadAsync() && reader.GetInt64(0) == 38 && reader.GetInt64(1) == 9
                && reader.GetInt64(2) < 0, "physical restoration/deletion oracle mismatch");
            Console.WriteLine("BASELINE_RESTORE_PHYSICAL " + JsonSerializer.Serialize(new {
                id, requiredInteger = reader.GetInt64(0), requiredLong = reader.GetInt64(1), version = reader.GetInt64(2),
                updateTime = reader.GetValue(3).ToString()
            }));
        }
        Require(provider.Snapshot() is { Query: > 0, Mutate: > 0, Begin: > 0, NextId: > 0 }
            && policy.Reviews > 0 && audit.Events > 0 && sql.Calls > 0, "uncalibrated observers");
        Console.WriteLine("BASELINE_RESTORE_RESULT " + JsonSerializer.Serialize(new { cases, baselineVersion = baseline, passed = true }));
        Console.WriteLine($"PASS .NET known-baseline restoration cases={cases.Count}");

        (int, int, int) Hooks() => (policy.Reviews, audit.Events, sql.Calls);
        void RealWrite(Counts before, (int, int, int) hooks)
        {
            var after = provider.Snapshot();
            Require(after.Mutate == before.Mutate + 1 && after.Begin == before.Begin + 1
                && policy.Reviews == hooks.Item1 + 1 && audit.Events == hooks.Item2 + 1 && sql.Calls > hooks.Item3,
                "genuine write did not cross the calibrated provider/policy/audit/SQL boundaries once");
        }
        async Task NoOp(ScalarTypeProbe row, string name)
        {
            var before = provider.Snapshot(); var hooks = Hooks(); var version = row.Version;
            var json = row.ToJson(); var key = new EntityKey("ScalarTypeProbe", id);
            var lineage = row.TeaqlMutationLedger.TraceChain(key).ToArray();
            var baselineVersion = row.TeaqlMutationLedger.OriginalVersion(key);
            await row.AuditAs("save restored " + name).SaveAsync(context);
            Require(provider.Snapshot() == before && Hooks() == hooks && row.Version == version
                && row.ToJson() == json && row.TeaqlMutationLedger.OriginalVersion(key) == baselineVersion
                && row.TeaqlMutationLedger.TraceChain(key).SequenceEqual(lineage)
                && !row.TeaqlMutationLedger.HasPending(key), "restored " + name + " changed view/version/lineage or reached I/O");
            Console.WriteLine("BASELINE_RESTORE_NOOP " + JsonSerializer.Serialize(new {
                @case = name, id, before, after = provider.Snapshot(), versionBefore = version, versionAfter = row.Version,
                policyDelta = policy.Reviews - hooks.Item1, auditDelta = audit.Events - hooks.Item2,
                sqlDelta = sql.Calls - hooks.Item3, viewPreserved = row.ToJson() == json,
                lineagePreserved = row.TeaqlMutationLedger.TraceChain(key).SequenceEqual(lineage),
                optimisticBaselinePreserved = row.TeaqlMutationLedger.OriginalVersion(key) == baselineVersion,
                pending = row.TeaqlMutationLedger.HasPending(key)
            }));
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT required_integer,version FROM scalar_type_probe_data WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            using var reader = await command.ExecuteReaderAsync();
            Require(await reader.ReadAsync() && reader.GetInt64(0) == 37 && reader.GetInt64(1) == baseline,
                "readonly physical no-op oracle differs from generated Q/E");
            cases.Add(name);
        }
    }

    private static async Task<ScalarTypeProbe> Load(UserContext context, long id) =>
        await Q.ScalarTypeProbes().WithIdIs(id).Limit(1).Comment("load restoration baseline")
            .Purpose("capture actual generated Q provenance").ExecuteForOneAsync(context)
        ?? throw new Exception("restoration fixture missing");
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void NotLoaded(Func<object?> access)
    { try { access(); } catch (TeaQLNotLoadedException) { return; } throw new Exception("missing field became loaded"); }
    private sealed class Policy : IMutationPolicy
    {
        public int Reviews { get; private set; }
        public MutationPolicyIdentity Identity { get; } = new("baseline-restoration", "1", "sha256:baseline-restoration");
        public MutationDecision Review(UserContext context, MutationPlan plan) { Reviews++; return MutationDecision.Allowed(); }
    }
    private sealed class Audit : IAppAuditEventSink
    {
        public int Events { get; private set; }
        public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken cancellationToken = default)
        { Events++; return Task.CompletedTask; }
    }
    private sealed class SqlLog : IDiagnosticSqlLogSink
    {
        public int Calls { get; private set; }
        public void Write(ExecutionMetadata metadata) => Calls++;
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
}
