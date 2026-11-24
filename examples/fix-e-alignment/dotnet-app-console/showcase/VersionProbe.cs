using Generated;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

// The raw SQL below establishes otherwise impractical test boundaries only.
// Every business write under test uses the generated audited Save, Q and E APIs.
static class VersionProbe
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static async Task Verify(UserContext source, SqliteConnection connection)
    {
        var transport = new CountingTransport(new SqliteTransport(connection));
        var module = GeneratedRuntimeModule.Module;
        var policy = new Policy(); var audit = new Audit();
        var context = source
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new ModuleSchemaProvider(module)))
            .WithMutationPolicyRegistry(policy).WithAppAuditEventSink(audit);
        var cases = new List<object>();
        foreach (var lastValidUpdate in new[] { true, false })
        {
            var item = (await Q.PurchaseOrders().Comment("Prepare a signed version boundary")
                .Purpose("Verify checked generated mutation admission").NewEntityAsync(context));
            item.UpdateContactPhone(lastValidUpdate ? "5550950" : "5550951");
            item = await item.AuditAs("Create the version boundary calibration").SaveAsync(context);
            var id = item.Id ?? throw new InvalidOperationException("Missing boundary ID");
            using (var fixture = connection.CreateCommand())
            {
                fixture.CommandText = "UPDATE purchase_order_data SET version=$version WHERE id=$id";
                fixture.Parameters.AddWithValue("$id", id);
                fixture.Parameters.AddWithValue("$version", lastValidUpdate ? long.MaxValue - 1 : long.MaxValue);
                Require(await fixture.ExecuteNonQueryAsync() == 1, "Boundary fixture did not update exactly one row");
            }
            var rows = await Q.PurchaseOrders().WithIdIs(id).Limit(1)
                .Comment("Reload the physical signed version boundary")
                .Purpose("Use actual loaded generated objects, not synthetic commands").ExecuteForListAsync(context);
            Require(rows.Count == 1, "Missing version boundary record");
            var loaded = rows[0]; int? validUpdateStatements = null;
            if (lastValidUpdate)
            {
                loaded.UpdateInternalNote("last permitted update");
                transport.Reset(); policy.Reset(); audit.Calls = 0;
                loaded = await loaded.AuditAs("Perform the last representable live update").SaveAsync(context);
                validUpdateStatements = transport.Statements;
                Require(validUpdateStatements > 0 && transport.Begins > 0 && policy.Reviews > 0 && audit.Calls == 1,
                    "The successful boundary must calibrate SQL, transaction, policy and audit observers");
            }
            var beforeVersion = Convert.ToInt64(E.PurchaseOrder(loaded).Version().Eval());
            Require(beforeVersion == long.MaxValue, "Last representable version was lost or rounded");
            var before = await Stored(connection, id);
            loaded.UpdateInternalNote("must remain pending");
            transport.Reset(); policy.Reset(); audit.Calls = 0;
            CheckResult violation;
            try
            {
                await loaded.AuditAs("Reject the exhausted live update").SaveAsync(context);
                throw new InvalidOperationException("Exhausted update unexpectedly succeeded");
            }
            catch (CheckException error)
            {
                Require(error.Violations.Count == 1, "Expected one version admission violation");
                violation = error.Violations[0];
                Require(violation.RuleId == "version_exhausted" && violation.InstancePath == "/version"
                    && violation.EntityType == "PurchaseOrder", "Expected canonical version exhaustion diagnostics");
            }
            Require(transport.Statements == 0 && transport.Begins == 0 && policy.Resolutions == 0 && policy.Reviews == 0 && audit.Calls == 0,
                "Exhausted Save reached SQL, transaction, policy or mutation audit");
            Require(E.PurchaseOrder(loaded).InternalNote().Eval() == "must remain pending", "Rejected Save discarded pending intent");
            Require(E.PurchaseOrder(loaded).Version().Eval() == beforeVersion, "Rejected Save changed the loaded version");
            var rejectionStatements = transport.Statements; var rejectionBegins = transport.Begins;
            var rejectionPolicyResolutions = policy.Resolutions; var rejectionPolicyReviews = policy.Reviews; var rejectionAudits = audit.Calls;
            var after = await Stored(connection, id);
            Require(before == after && after.Version == long.MaxValue.ToString(CultureInfo.InvariantCulture)
                && after.Note == (lastValidUpdate ? "last permitted update" : null), "Rejected Save changed physical data");
            var persisted = await Q.PurchaseOrders().WithIdIs(id).Limit(1)
                .Comment("Reload after rejected version exhaustion")
                .Purpose("Verify persisted Q and E remain at the exact legal endpoint").ExecuteForListAsync(context);
            Require(persisted.Count == 1 && E.PurchaseOrder(persisted[0]).Version().Eval() == long.MaxValue
                && E.PurchaseOrder(persisted[0]).InternalNote().Eval() == after.Note, "Q/E contradict physical boundary data");
            cases.Add(new {
                @case = lastValidUpdate ? "last-representable-update" : "loaded-exhausted-update", id,
                beforeVersion = beforeVersion.ToString(CultureInfo.InvariantCulture), version = after.Version,
                storedNote = after.Note, validUpdateStatements,
                rejectionRule = violation.RuleId, rejectionPath = violation.InstancePath,
                rejectionStatements, rejectionBegins, rejectionPolicyResolutions, rejectionPolicyReviews, rejectionAudits,
                loadedVersionPreserved = true, pendingIntentPreserved = true
            });
        }
        Console.WriteLine("DOTNET_VERSION_BOUNDARIES=" + JsonSerializer.Serialize(new { language = "dotnet", passed = true, cases }));
    }

    private sealed record StoredRow(string Version, string? Note);
    private static async Task<StoredRow> Stored(SqliteConnection connection, long id)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT CAST(version AS TEXT),internal_note FROM purchase_order_data WHERE id=$id";
        command.Parameters.AddWithValue("$id", id);
        using var reader = await command.ExecuteReaderAsync();
        Require(await reader.ReadAsync(), "Missing physical boundary row");
        return new StoredRow(reader.GetString(0), reader.IsDBNull(1) ? null : reader.GetString(1));
    }
    private sealed class Policy : IMutationPolicy, IMutationPolicyRegistry
    {
        internal int Resolutions { get; private set; }
        internal int Reviews { get; private set; }
        internal void Reset() { Resolutions = 0; Reviews = 0; }
        public MutationPolicyIdentity Identity { get; } = new("version-boundary-example", "1", "checked-v1");
        public IMutationPolicy? Resolve(string key) { Resolutions++; return this; }
        public MutationDecision Review(UserContext context, MutationPlan plan) { Reviews++; return MutationDecision.Allowed(); }
    }
    private sealed class Audit : IAppAuditEventSink
    {
        internal int Calls { get; set; }
        public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default)
        { Calls++; return Task.CompletedTask; }
    }
    private sealed class CountingTransport(SqliteTransport inner) : IAutomaticMutationTransactionTransport
    {
        internal int Statements { get; private set; }
        internal int Begins { get; private set; }
        internal void Reset() { Statements = 0; Begins = 0; }
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Statements++; return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Statements++; return inner.ExecuteSqlAsync(query); }
        public async Task<ISqlTransaction> BeginSqlAsync() { Begins++; return new Transaction(await inner.BeginSqlAsync(), this); }
        private sealed class Transaction(ISqlTransaction transaction, CountingTransport owner) : ISqlTransaction
        {
            public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { owner.Statements++; return transaction.FetchAllSqlAsync(query); }
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { owner.Statements++; return transaction.ExecuteSqlAsync(query); }
            public Task CommitSqlAsync() => transaction.CommitSqlAsync();
            public Task RollbackSqlAsync() => transaction.RollbackSqlAsync();
            public void Dispose() => transaction.Dispose();
        }
    }
}
