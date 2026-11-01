using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

// Application-owned proof: generated queries, loaded E expressions and audited
// saves. Direct SQL is a read-only, independently observed storage oracle.
static class UnchangedViewsProbe
{
    static void Require(bool value, string message)
    { if (!value) throw new InvalidOperationException(message); }
    static EntityRoot Ledger(PurchaseOrder row) => (EntityRoot)(typeof(PurchaseOrder)
        .GetField("_entityRoot", BindingFlags.NonPublic | BindingFlags.Instance)
        ?? throw new InvalidOperationException("The read-only ledger observation seam changed")).GetValue(row)!;
    static EntityKey Key(PurchaseOrder row) => new("PurchaseOrder", row.Id!.Value);
    static void NotLoaded(Func<object?> access)
    {
        try { _ = access(); }
        catch (TeaQLNotLoadedException) { return; }
        throw new InvalidOperationException("An unchanged sparse view acquired an unselected field");
    }
    static void Aliases(SelectQuery query)
    {
        query.ProjectExpr("_null", Expr.Value(new Value.NullValue()));
        query.ProjectExpr("_zero", Expr.Value(new Value.I64Value(0)));
        query.ProjectExpr("_empty", Expr.Value(new Value.TextValue("")));
        query.ProjectExpr("_row_id", Expr.Column("id"));
        query.ProjectExpr("_phone", Expr.Column("contact_phone"));
    }
    static void Properties(PurchaseOrder row)
    {
        var values = row.DynamicProperties;
        Require(values.Contains("_null") && values.Get("_null") is Value.NullValue
            && !values.Contains("_missing") && values.Get("_missing") is null,
            "Absent and selected null lost their separate presence");
        Require(values.Get("_zero")?.TryI64() == 0 && values.Get("_empty")?.TryText() == ""
            && values.Get("_row_id")?.TryI64() == row.Id
            && values.Get("_phone")?.TryText() == E.PurchaseOrder(row).ContactPhone().Eval(),
            "Query properties lost their values or borrowed another row's data");
        using var json = JsonDocument.Parse(row.ToJson()); var root = json.RootElement;
        Require(root.GetProperty("_null").ValueKind == JsonValueKind.Null
            && root.GetProperty("_zero").GetInt64() == 0 && root.GetProperty("_empty").GetString() == ""
            && root.GetProperty("_row_id").GetInt64() == row.Id && !root.TryGetProperty("_missing", out _),
            "The selected query-property JSON view changed");
    }

    public static async Task Verify(string filename)
    {
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module; var transport = new Transport(new SqliteTransport(connection));
        var audit = new Audit(); var policy = new Policy(); var sql = new SqlLog(); var resolutions = 0;
        var initial = new DateTimeOffset(2032, 2, 29, 10, 15, 30, TimeSpan.Zero);
        var context = module.IntoContext()
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module)))
            .WithBusinessClock(new FixedBusinessClock(initial)).WithAppAuditEventSink(audit)
            .WithDiagnosticSqlLogSink(sql)
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => { resolutions++; return policy; }));
        await context.EnsureSchemaAsync();
        var previous = (await Q.PurchaseOrders().Limit(5).Comment("Determine the unchanged-view regression round")
            .Purpose("Run twice without resetting the same SQLite database").ExecuteForListAsync(context)).Count;
        Require(previous is 0 or 2, "Unexpected retained unchanged-view round boundary");
        var round = previous / 2 + 1; var first = initial.AddDays(round - 1); var next = first.AddHours(2);
        context.WithBusinessClock(new FixedBusinessClock(first));
        Effects Counts() => new(transport.Queries, transport.Mutations, transport.Begins, transport.Commits,
            transport.Rollbacks, transport.IdStatements, resolutions, policy.Reviews, audit.Calls, sql.Calls);
        void RealWrite(Effects before)
        {
            var delta = Counts() - before;
            Require(delta.queries > 0 && delta.mutations == 1 && delta.begins == 1 && delta.commits == 1
                && delta.rollbacks == 0 && delta.resolutions == 1 && delta.reviews == 1 && delta.audits == 1 && delta.sqlLogs > 0,
                "The genuine write did not calibrate the provider, policy, audit and diagnostic observers");
        }
        async Task<PurchaseOrder> Seed()
        {
            var row = Q.PurchaseOrders().Comment("Prepare an unchanged-view fixture")
                .Purpose("Verify no-op preservation using generated mutation APIs").NewEntity(context);
            row.UpdateContactPhone("5550800").UpdateInternalNote(null!);
            var before = Counts(); await row.AuditAs("Create an unchanged-view fixture").SaveAsync(context); RealWrite(before);
            return row;
        }
        var one = await Seed(); var two = await Seed();
        var request = Q.PurchaseOrdersWithMinimalFields().WithIdIs(one.Id!.Value)
            .SelectContactPhone().SelectInternalNote()
            .SelectTenantWith(Q.TenantsWithMinimalFields().Limit(1).Comment("Retain only the loaded root identity"))
            .Limit(1).Comment("Load a sparse view with query-only properties and one loaded edge");
        Aliases(request.GetQuery());
        var sparse = (await request.Purpose("Preserve the exact view when all fixed edits are restored")
            .ExecuteForListAsync(context)).Single();
        Properties(sparse); NotLoaded(() => E.PurchaseOrder(sparse).CreateTime().Eval());
        NotLoaded(() => E.PurchaseOrder(sparse).Tenant().Name().Eval());
        var rootId = E.PurchaseOrder(sparse).Tenant().Id().Eval();
        Require(rootId == 1 && E.PurchaseOrder(sparse).InternalNote().HasValue
            && E.PurchaseOrder(sparse).InternalNote().Eval() is null, "The initial sparse relation/null view is not calibrated");
        sparse.UpdateContactPhone("5550899").UpdateContactPhone("5550800")
            .UpdateInternalNote("temporary null replacement").UpdateInternalNote(null!);
        await NoOp(sparse, "sparse-restored-values-null-properties-and-edge");
        NotLoaded(() => E.PurchaseOrder(sparse).CreateTime().Eval());
        NotLoaded(() => E.PurchaseOrder(sparse).Tenant().Name().Eval());
        Require(E.PurchaseOrder(sparse).Tenant().Id().Eval() == rootId, "A no-op lost a loaded relation edge");
        await NoOp(sparse, "repeat-clean-sparse-save");

        var pairRequest = Q.PurchaseOrders().Limit(2).Comment("Load two row-owned property values with the same query geometry");
        pairRequest.GetQuery().AndFilter(Expr.In("id", new[] { one.Id.Value, two.Id!.Value }));
        Aliases(pairRequest.GetQuery());
        var pair = (await pairRequest.Purpose("Saving one row must not consume another row's values or pending intent")
            .ExecuteForListAsync(context)).OrderBy(row => row.Id).ToArray();
        Require(pair.Length == 2 && pair[0].Id == one.Id && pair[1].Id == two.Id, "The bounded pair query is not calibrated");
        foreach (var row in pair) Properties(row);
        var left = pair[0]; var right = pair[1]; var rightJson = right.ToJson();
        right.UpdateInternalNote("independent pending value");
        left.UpdateContactPhone("5550899").UpdateContactPhone("5550800");
        var rightIntent = JsonSerializer.Serialize(Ledger(right).Change(Key(right)));
        await NoOp(left, "other-row-pending-intent-and-values-retained");
        Require(JsonSerializer.Serialize(Ledger(right).Change(Key(right))) == rightIntent
            && right.DynamicProperties.Get("_row_id")?.TryI64() == two.Id
            && E.PurchaseOrder(right).InternalNote().Eval() == "independent pending value",
            "Saving one row consumed another row's pending work or query property");
        right.UpdateInternalNote(null!);
        Require(right.ToJson() == rightJson, "Restoring a fixed null changed the other row's JSON view");
        await NoOp(right, "row-owned-properties-and-loaded-null-retained");

        var collisionBefore = Counts(); var collisionJson = left.ToJson();
        var collision = new SelectQuery("PurchaseOrder").ProjectExpr("contact_phone", Expr.Value(new Value.I64Value(0)));
        try { EntityQueryProperties.Capture(left, collision); throw new InvalidOperationException("A model-field collision was accepted"); }
        catch (EntityJsonException error) { Require(error.Code == "WIRE_QUERY_PROPERTY_COLLISION", "Unexpected alias admission diagnostic"); }
        var missing = new SelectQuery("PurchaseOrder").ProjectExpr("_missing", Expr.Value(new Value.NullValue()));
        try { EntityQueryProperties.Capture(left, missing); throw new InvalidOperationException("An absent alias was invented"); }
        catch (ArgumentException error) { Require(error.Message.Contains("Selected query-property alias is absent"), "Unexpected missing-property diagnostic"); }
        Require(left.ToJson() == collisionJson && Counts() == collisionBefore, "Rejected property selection changed the validated view or performed I/O");
        await NoOp(left, "rejected-property-selection-retains-prior-view");

        context.WithBusinessClock(new FixedBusinessClock(next));
        right.UpdateContactPhone("5550801"); var realBefore = Counts();
        var saved = await right.AuditAs("Persist the genuine fixed-field control after no-op saves").SaveAsync(context);
        RealWrite(realBefore);
        Require(E.PurchaseOrder(saved).ContactPhone().Eval() == "5550801" && E.PurchaseOrder(saved).Version().Eval() == 2
            && E.PurchaseOrder(saved).CreateTime().Eval() == first.UtcDateTime
            && E.PurchaseOrder(saved).UpdateTime().Eval() == next.UtcDateTime,
            "The genuine save reinstalled an old fixed-field view over authoritative readback");
        Require(!saved.DynamicProperties.Contains("_phone") || saved.DynamicProperties.Get("_phone")?.TryText() == "5550801",
            "The genuine result reinstalled stale derived values");
        Console.WriteLine("DOTNET_UNCHANGED_VIEW_WRITE=" + JsonSerializer.Serialize(new {
            round, orderId = two.Id.Value, before = realBefore, after = Counts(), effects = Counts() - realBefore,
            phone = E.PurchaseOrder(saved).ContactPhone().Eval(), version = E.PurchaseOrder(saved).Version().Eval(),
            stalePropertyReinstalled = false, passed = true }));
        Properties(left); Require(E.PurchaseOrder(left).Version().Eval() == 1, "Another row's write changed the unchanged row");

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id,contact_phone,internal_note,tenant,version,create_time,update_time FROM purchase_order_data ORDER BY id";
        using var reader = await command.ExecuteReaderAsync(); var physical = new List<object>();
        var index = 0;
        while (await reader.ReadAsync())
        {
            var written = index % 2 == 1; var time = initial.AddDays(index / 2);
            Require(reader.GetInt64(0) == index + 1 && reader.GetString(1) == (written ? "5550801" : "5550800")
                && reader.IsDBNull(2) && reader.GetInt64(3) == 1 && reader.GetInt64(4) == (written ? 2 : 1)
                && reader.GetInt64(5) == time.ToUnixTimeMilliseconds()
                && reader.GetInt64(6) == (written ? time.AddHours(2) : time).ToUnixTimeMilliseconds(),
                "Independent SQLite readback contradicts the generated no-op/write result");
            physical.Add(new { id = reader.GetInt64(0), phone = reader.GetString(1), note = (string?)null,
                tenant = reader.GetInt64(3), version = reader.GetInt64(4), createTime = reader.GetInt64(5), updateTime = reader.GetInt64(6) }); index++;
        }
        Require(index == round * 2 && transport.IdStatements > 0, "The retained store or ID observer is not calibrated");
        Console.WriteLine("DOTNET_UNCHANGED_VIEW_PHYSICAL=" + JsonSerializer.Serialize(new { round, rows = physical, passed = true }));

        async Task NoOp(PurchaseOrder row, string name)
        {
            var json = row.ToJson(); var fields = ((IEntityJsonDocument)row).LoadedJsonFields().Select(p => p.Key).ToArray();
            var ledger = Ledger(row); var key = Key(row); var version = row.Version; var originalVersion = ledger.OriginalVersion(key);
            var lineage = ledger.TraceChain(key).ToArray(); var before = Counts();
            var result = await row.AuditAs("Save unchanged view: " + name).SaveAsync(context);
            Require(Counts() == before && row.ToJson() == json && result.ToJson() == json
                && ((IEntityJsonDocument)result).LoadedJsonFields().Select(p => p.Key).SequenceEqual(fields)
                && result.Version == version && ledger.OriginalVersion(key) == originalVersion
                && ledger.TraceChain(key).SequenceEqual(lineage) && !ledger.HasPending(key),
                "An unchanged save changed view/version/lineage/intent or crossed an execution boundary: " + name);
            Properties(row); Properties(result);
            Console.WriteLine("DOTNET_UNCHANGED_VIEW_CASE=" + JsonSerializer.Serialize(new {
                round, @case = name, orderId = row.Id!.Value, passed = true, before, after = Counts(), effects = Counts() - before,
                versionBefore = version, versionAfter = result.Version, loadedFields = fields,
                jsonPreserved = true, lineagePreserved = true, originalVersionPreserved = true, pending = false,
                selectedNullPresent = true, absentPropertyPresent = false, zero = 0, empty = "", rowOwnedProperty = row.Id.Value }));
        }
    }

    sealed record Effects(int queries, int mutations, int begins, int commits, int rollbacks, int idStatements,
        int resolutions, int reviews, int audits, int sqlLogs)
    {
        public static Effects operator -(Effects a, Effects b) => new(a.queries-b.queries, a.mutations-b.mutations,
            a.begins-b.begins, a.commits-b.commits, a.rollbacks-b.rollbacks, a.idStatements-b.idStatements,
            a.resolutions-b.resolutions, a.reviews-b.reviews, a.audits-b.audits, a.sqlLogs-b.sqlLogs);
    }
    sealed class Schema(RuntimeModule module) : ISchemaProvider { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
    sealed class Audit : IAppAuditEventSink
    { public int Calls; public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default) { Calls++; return Task.CompletedTask; } }
    sealed class SqlLog : IDiagnosticSqlLogSink { public int Calls; public void Write(ExecutionMetadata value) => Calls++; }
    sealed class Policy : IMutationPolicy
    {
        public int Reviews; public MutationPolicyIdentity Identity { get; } = new("unchanged-view", "1", "sha256:unchanged-view");
        public MutationDecision Review(UserContext context, MutationPlan plan) { Reviews++; return MutationDecision.Allowed(); }
    }
    sealed class Transport(SqliteTransport inner) : IAutomaticMutationTransactionTransport
    {
        public int Queries, Mutations, Begins, Commits, Rollbacks, IdStatements;
        void Observe(CompiledQuery query)
        {
            var text = query.Sql.Replace("\"", "");
            if (text.Contains("teaql_id_space", StringComparison.OrdinalIgnoreCase)) IdStatements++;
            if (Regex.IsMatch(text, @"^\s*(?:INSERT INTO|UPDATE|DELETE FROM)\s+(?:tenant_data|purchase_order_data|order_item_data)\b", RegexOptions.IgnoreCase)) Mutations++;
            else Queries++;
        }
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Observe(query); return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Observe(query); return inner.ExecuteSqlAsync(query); }
        public async Task<ISqlTransaction> BeginSqlAsync() { Begins++; return new Transaction(await inner.BeginSqlAsync(), this); }
        sealed class Transaction(ISqlTransaction transaction, Transport owner) : ISqlTransaction
        {
            public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { owner.Observe(query); return transaction.FetchAllSqlAsync(query); }
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { owner.Observe(query); return transaction.ExecuteSqlAsync(query); }
            public Task CommitSqlAsync() { owner.Commits++; return transaction.CommitSqlAsync(); }
            public Task RollbackSqlAsync() { owner.Rollbacks++; return transaction.RollbackSqlAsync(); }
            public void Dispose() => transaction.Dispose();
        }
    }
}
