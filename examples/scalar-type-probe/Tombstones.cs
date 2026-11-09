using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;

// Business seeding and reads use generated Q/E/audited saves. Native requests
// below are explicitly separate provider-SPI transaction/lifecycle controls.
internal static class Tombstones
{
    internal static async Task RunAsync(string database)
    {
        await using var connection = new SqliteConnection($"Data Source={database}"); await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module;
        var provider = new Observed(new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
            new MetadataSchemaProvider(module.Metadata.GetEntity)));
        var audit = new Audit(provider); var policy = new Policy();
        var context = module.IntoContext().WithDataService(provider).WithAppAuditEventSink(audit)
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => policy));
        await context.EnsureSchemaAsync(); await context.EnsureSchemaAsync();
        var marker = "tombstone-" + Guid.NewGuid().ToString("N");
        var round = int.Parse(Environment.GetEnvironmentVariable("TEAQL_TOMBSTONE_ROUND")
            ?? Environment.GetEnvironmentVariable("TEAQL_EMPTY_ROUND") ?? "1");
        var template = (await NewAsync(marker + "-template")).UpdateRequiredDate(new DateTime(2040, 2, 29))
            .UpdateRequiredDatetime(new DateTime(2040, 2, 29, 14, 15, 16, DateTimeKind.Utc));
        await template.AuditAs("seed typed trigger reference values").SaveAsync(context);
        foreach (var kind in new[] { "ordinary", "trigger", "commit-retry", "provider-commit", "provider-rollback-retry", "provider-roles" })
        {
            var row = await NewAsync(marker + "-" + kind); await row.AuditAs("seed an existing tombstone fixture").SaveAsync(context);
            var id = row.Id!.Value; row = await Load(id); var triggered = kind != "ordinary";
            var originalName = E.ScalarTypeProbe(row).RequiredString().Eval();
            if (triggered)
            {
                using var ddl = connection.CreateCommand();
                ddl.CommandText = $"CREATE TRIGGER tombstone_{id} AFTER UPDATE OF version ON scalar_type_probe_data "
                    + $"WHEN NEW.id={id} AND NEW.version<0 AND OLD.version>0 BEGIN "
                    + "UPDATE scalar_type_probe_data SET version=NEW.version-40,"
                    + "required_string='DB-TOMBSTONE-'||NEW.id,optional_string=NULL,"
                    + $"required_date=(SELECT required_date FROM scalar_type_probe_data WHERE id={template.Id}),"
                    + $"required_datetime=(SELECT required_datetime FROM scalar_type_probe_data WHERE id={template.Id}) "
                    + "WHERE id=NEW.id; END";
                await ddl.ExecuteNonQueryAsync();
            }
            var expected = triggered ? -42L : -2L; var beforeAudit = audit.Events.Count;
            long? version; string? returnedName; bool rolledBack = false, absent = false;
            if (kind.StartsWith("provider-", StringComparison.Ordinal) && kind != "provider-roles")
            {
                var request = new DeleteMutationRequest(new DeleteCommand("ScalarTypeProbe", Value.FromObject(id)).ExpectedVersion(1),
                    "verify explicit provider transaction readback");
                using (var tx = await provider.BeginTransactionAsync())
                {
                    var result = await tx.MutateAsync(request); AssertRecord(result.PersistedRecord, id, expected);
                    Require(audit.Events.Count == beforeAudit, "raw provider transaction fabricated a runtime audit");
                    if (kind == "provider-rollback-retry") { await tx.RollbackAsync(); rolledBack = true; }
                    else await tx.CommitAsync();
                }
                if (rolledBack)
                {
                    await Physical(id, 1, originalName); Require((await Load(id)).Version == 1, "provider rollback leaked a tombstone");
                    using var retry = await provider.BeginTransactionAsync(); var result = await retry.MutateAsync(request);
                    AssertRecord(result.PersistedRecord, id, expected); await retry.CommitAsync();
                }
                version = expected; returnedName = "DB-TOMBSTONE-" + id;
            }
            else
            {
                row.MarkForDeletion();
                if (kind == "commit-retry")
                {
                    provider.FailCommitOnce = true;
                    try { await row.AuditAs("rollback a generated deletion before physical commit").SaveAsync(context); throw new Exception("commit fault did not fire"); }
                    catch (InvalidOperationException e) when (e.Message == "INTENTIONAL_TOMBSTONE_COMMIT_FAILURE") { rolledBack = true; }
                    Require(audit.Events.Count == beforeAudit && row.Version == 1, "failed commit leaked audit/version");
                    Require(E.ScalarTypeProbe(row).RequiredString().Eval() == originalName
                        && E.ScalarTypeProbe(row).OptionalString().Eval() == "before deletion", "failed commit did not restore values");
                    Require(row.TeaqlMutationLedger.HasPending(new EntityKey("ScalarTypeProbe", id)), "failed commit lost pending deletion");
                    await Physical(id, 1, originalName);
                }
                await row.AuditAs("save deletion with authoritative persisted values").SaveAsync(context);
                Require(row.Version == expected, "generated save inferred a version instead of reading it");
                returnedName = triggered ? "DB-TOMBSTONE-" + id : originalName;
                Require(E.ScalarTypeProbe(row).RequiredString().Eval() == returnedName, "generated readback lost trigger text");
                if (triggered)
                {
                    Require(E.ScalarTypeProbe(row).OptionalString().Eval() is null, "trigger null is not loaded null");
                    Require(E.ScalarTypeProbe(row).RequiredDate().Eval() == new DateTime(2040, 2, 29)
                        && E.ScalarTypeProbe(row).RequiredDatetime().Eval() == new DateTime(2040, 2, 29, 14, 15, 16, DateTimeKind.Utc),
                        "trigger temporal values were not read back");
                }
                Require(audit.Events.Count == beforeAudit + 1 && Convert.ToInt64(audit.Events[^1]["resultVersion"]) == expected,
                    "committed audit did not use the authoritative version");
                version = row.Version;
                var counts = provider.Snapshot(); var policies = policy.Reviews; var audits = audit.Events.Count;
                try { row.MarkForDeletion(); await row.AuditAs("reject repeated deletion").SaveAsync(context); throw new Exception("repeated deletion succeeded"); }
                catch (CheckException) { }
                Require(provider.Snapshot() == counts && policy.Reviews == policies && audit.Events.Count == audits,
                    "repeated deletion reached a service/policy/audit boundary");
                if (kind == "provider-roles")
                {
                    var service = context.RequireResource<IDataService>();
                    var blind = await service.MutateAsync(new DeleteMutationRequest(new DeleteCommand("ScalarTypeProbe", Value.FromObject(id)),
                        "a trusted versionless soft delete cannot rewrite a tombstone"));
                    Require(blind.AffectedRows == 0 && audit.Events.Count == audits, "blind delete rewrote a tombstone or audited a no-op");
                    var recovered = await service.MutateAsync(new RecoverMutationRequest(new RecoverCommand {
                        Entity = "ScalarTypeProbe", Id = Value.FromObject(id), ExpectedVersionValue = expected }, "explicit provider recovery"));
                    Require(recovered.PersistedRecord?["version"].TryI64() == 43, "explicit recovery did not return live version");
                    var live = await Load(id); Require(live.Version == 43, "generated Q did not see recovered row");
                    live.MarkForDeletion(); await live.AuditAs("delete the recovered row before explicit physical removal").SaveAsync(context);
                    Require(live.Version == -84, "recovered row deletion lost trigger version");
                    var hard = await service.MutateAsync(new DeleteMutationRequest(new DeleteCommand("ScalarTypeProbe", Value.FromObject(id))
                        .ExpectedVersion(-84).HardDelete(), "explicit trusted physical deletion"));
                    Require(hard.AffectedRows == 1 && hard.PersistedRecord is null, "hard deletion returned a fictitious persisted entity");
                    using var count = connection.CreateCommand(); count.CommandText = "SELECT count(*) FROM scalar_type_probe_data WHERE id=@id";
                    count.Parameters.AddWithValue("@id", id); Require(Convert.ToInt64(await count.ExecuteScalarAsync()) == 0, "hard delete left a row");
                    version = null; absent = true;
                }
            }
            if (!absent) await Physical(id, expected, returnedName);
            var visible = await Q.ScalarTypeProbes().WithIdIs(id).Limit(1).Comment("check deleted row visibility")
                .Purpose("ordinary generated Q must hide tombstones").ExecuteForListAsync(context);
            Require(visible.Count == 0, "ordinary generated Q exposed a tombstone");
            Console.WriteLine("DOTNET_TOMBSTONE=" + JsonSerializer.Serialize(new {
                @case = kind, round, id, version, triggered, name = returnedName,
                auditDelta = audit.Events.Count - beforeAudit, rolledBack, physicalAbsent = absent,
                providerTransaction = kind is "provider-commit" or "provider-rollback-retry"
            }));
        }
        Console.WriteLine("PASS .NET generated authoritative tombstone and provider transaction controls cases=6");

        async Task<ScalarTypeProbe> NewAsync(string name) => (await Q.ScalarTypeProbes().Comment("prepare typed tombstone fixture")
            .Purpose("verify persisted deletion readback").NewEntityAsync(context)).UpdateTypePlatform(1).UpdateRequiredString(name)
            .UpdateOptionalString("before deletion").UpdateRequiredBoolean(true).UpdateRequiredInteger(7).UpdateRequiredLong(8)
            .UpdateRequiredDecimal(3.50m).UpdateRequiredFloat(1.25).UpdateRequiredDouble(2.5)
            .UpdateRequiredDate(new DateTime(2026, 10, 20)).UpdateRequiredTime(new TimeSpan(12, 30, 0))
            .UpdateRequiredDatetime(new DateTime(2026, 10, 20, 12, 30, 0, DateTimeKind.Utc));
        async Task<ScalarTypeProbe> Load(long id) => await Q.ScalarTypeProbes().WithIdIs(id).Limit(1)
            .Comment("load a persisted tombstone baseline").Purpose("retain ordinary Q/E semantics").ExecuteForOneAsync(context)
            ?? throw new Exception("live tombstone fixture missing");
        async Task Physical(long id, long expected, string? name)
        {
            using var command = connection.CreateCommand(); command.CommandText = "SELECT version,required_string FROM scalar_type_probe_data WHERE id=@id";
            command.Parameters.AddWithValue("@id", id); await using var values = await command.ExecuteReaderAsync();
            Require(await values.ReadAsync() && values.GetInt64(0) == expected && values.GetString(1) == name && !await values.ReadAsync(),
                "physical row differs from the reviewed tombstone outcome");
        }
    }
    private static void AssertRecord(TeaQL.Core.Record? row, long id, long version)
    {
        Require(row is not null && row["id"].TryI64() == id && row["version"].TryI64() == version
            && row["required_string"].TryText() == "DB-TOMBSTONE-" + id
            && row.ContainsKey("optional_string") && row["optional_string"] is Value.NullValue,
            "native transaction lost authoritative values or loaded null");
    }
    private static void Require(bool ok, string message) { if (!ok) throw new Exception(message); }
    private sealed class Policy : IMutationPolicy
    {
        public int Reviews { get; private set; }
        public MutationPolicyIdentity Identity { get; } = new("tombstone-example", "1", "sha256:tombstone-example");
        public MutationDecision Review(UserContext context, MutationPlan plan) { Reviews++; return MutationDecision.Allowed(); }
    }
    private sealed class Audit(Observed owner) : IAppAuditEventSink
    {
        public List<IReadOnlyDictionary<string, object?>> Events { get; } = [];
        public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default)
        { Require(owner.Active == 0, "audit preceded physical commit"); Events.Add(value); return Task.CompletedTask; }
    }
    private sealed record Counts(int Query, int Mutate, int Begin, int NextId);
    private sealed class Observed(SqlDataServiceExecutor inner) : IDataService, ITransactionExecutor, IIdGeneratorExecutor, ISchemaExecutor
    {
        private int query, mutate, begin, nextId;
        public int Active { get; private set; }
        public bool FailCommitOnce { get; set; }
        public Counts Snapshot() => new(query, mutate, begin, nextId);
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Task<QueryResult> QueryAsync(QueryRequest request) { query++; return inner.QueryAsync(request); }
        public Task<MutationResult> MutateAsync(MutationRequest request) { mutate++; return inner.MutateAsync(request); }
        public async Task<ITransaction> BeginTransactionAsync() { begin++; var tx = await inner.BeginTransactionAsync(); Active++; return new Transaction(tx, this); }
        public Task<SchemaResult> EnsureSchemaAsync(SchemaRequest request) => ((ISchemaExecutor)inner).EnsureSchemaAsync(request);
        public Task<ulong> NextIdAsync(string entity) { nextId++; return inner.NextIdAsync(entity); }
        public Task EnsureIdFloorAsync(string entity, ulong floor) => inner.EnsureIdFloorAsync(entity, floor);
        private sealed class Transaction(ITransaction inner, Observed owner) : ITransaction, IInternalIdAllocator
        {
            private bool ended;
            private void Finish() { if (!ended) { owner.Active--; ended = true; } }
            public DataServiceCapabilities Capabilities => inner.Capabilities;
            public Task<QueryResult> QueryAsync(QueryRequest request) { owner.query++; return inner.QueryAsync(request); }
            public Task<MutationResult> MutateAsync(MutationRequest request) { owner.mutate++; return inner.MutateAsync(request); }
            public Task<ulong> NextIdAsync(string entity) { owner.nextId++; return ((IInternalIdAllocator)inner).NextIdAsync(entity); }
            public Task EnsureIdFloorAsync(string entity, ulong floor) => ((IInternalIdAllocator)inner).EnsureIdFloorAsync(entity, floor);
            public async Task CommitAsync() { if (owner.FailCommitOnce) { owner.FailCommitOnce = false; throw new InvalidOperationException("INTENTIONAL_TOMBSTONE_COMMIT_FAILURE"); } await inner.CommitAsync(); Finish(); }
            public async Task RollbackAsync() { await inner.RollbackAsync(); Finish(); }
            public void Dispose() { inner.Dispose(); Finish(); }
        }
    }
}
