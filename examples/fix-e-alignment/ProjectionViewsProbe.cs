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

// Application-owned generated Q/E/audited-save regression, not custom hydration.
static class ProjectionViewsProbe
{
    static void Require(bool value, string message)
    { if (!value) throw new InvalidOperationException(message); }
    static EntityRoot Ledger(PurchaseOrder row) => (EntityRoot)(typeof(PurchaseOrder)
        .GetField("_entityRoot", BindingFlags.NonPublic | BindingFlags.Instance)
        ?? throw new InvalidOperationException("The existing read-only ledger observation seam changed")).GetValue(row)!;
    static EntityKey Key(PurchaseOrder row) => new("PurchaseOrder", row.Id ?? throw new InvalidOperationException("Missing identity"));
    static void Missing(Func<object?> read)
    {
        try { _ = read(); }
        catch (TeaQLNotLoadedException) { return; }
        throw new InvalidOperationException("An explicitly sparse view was widened");
    }

    public static async Task Verify(string filename)
    {
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module; var transport = new Transport(new SqliteTransport(connection));
        var audit = new Audit(); var policies = 0;
        var initial = new DateTimeOffset(2032, 2, 29, 10, 15, 30, TimeSpan.Zero);
        var context = module.IntoContext()
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module)))
            .WithBusinessClock(new FixedBusinessClock(initial)).WithAppAuditEventSink(audit)
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => { policies++; return null; }));
        await context.EnsureSchemaAsync();
        var previous = (await Q.PurchaseOrders().Limit(7).Comment("Determine the retained example round")
            .Purpose("Verify two rounds without resetting the projection database").ExecuteForListAsync(context)).Count;
        Require(previous is 0 or 3, "The projection store is not at an expected round boundary");
        var round = previous / 3 + 1; var first = initial.AddDays(round - 1); var second = first.AddHours(2);
        context.WithBusinessClock(new FixedBusinessClock(first));
        Effects Counts() => new(transport.Queries, transport.Mutations, transport.Begins, transport.Commits, transport.Rollbacks, transport.IdStatements, policies, audit.Calls);
        async Task<PurchaseOrder> Seed()
        {
            var row = Q.PurchaseOrders().Comment("Prepare an independent query-view fixture")
                .Purpose("Verify projection boundaries and independent mutation ledgers").NewEntity(context);
            row.UpdateContactPhone("5550700"); row.UpdateInternalNote(null!);
            var before = Counts(); await row.AuditAs("Create the projection fixture through generated mutation").SaveAsync(context);
            var after = Counts();
            Require(after.queries > before.queries && after.mutations > before.mutations && after.begins > before.begins
                && after.commits > before.commits && after.policies > before.policies && after.audits > before.audits,
                "Real successful creation must calibrate every relevant observer");
            return row;
        }
        async Task<PurchaseOrder> Full(long id) => (await Q.PurchaseOrders().WithIdIs(id).Limit(1)
            .Comment("Load a complete independent order view").Purpose("Compare explicit views of one stored identity")
            .ExecuteForListAsync(context)).Single();
        void Case(string name, PurchaseOrder row, object facts) => Console.WriteLine("DOTNET_PROJECTION_VIEW_CASE="
            + JsonSerializer.Serialize(new { round, @case = name, orderId = row.Id!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture), passed = true, facts }));

        var one = await Seed();
        var sparse = (await Q.PurchaseOrdersWithMinimalFields().WithIdIs(one.Id!.Value).Limit(1)
            .Comment("Retain an explicitly sparse order view").Purpose("A later full query must not widen it")
            .ExecuteForListAsync(context)).Single();
        var sparseJson = sparse.ToJson(); var wide = await Full(one.Id.Value); var beforeReads = Counts();
        Require(!ReferenceEquals(sparse, wide) && sparse.Id == wide.Id, "Two requested projections must remain independent views of the same identity");
        Missing(() => E.PurchaseOrder(sparse).ContactPhone().Eval()); Missing(() => E.PurchaseOrder(sparse).InternalNote().Eval());
        Require(sparseJson == sparse.ToJson(), "Full query changed an earlier sparse JSON view");
        using (var json = JsonDocument.Parse(sparseJson)) Require(!json.RootElement.TryGetProperty("contactPhone", out _)
            && !json.RootElement.TryGetProperty("internalNote", out _), "Sparse JSON exposes an unselected field");
        Require(E.PurchaseOrder(wide).InternalNote().HasValue && E.PurchaseOrder(wide).InternalNote().Eval() is null, "Selected null became NotLoaded");
        using (var json = JsonDocument.Parse(wide.ToJson())) Require(json.RootElement.GetProperty("internalNote").ValueKind == JsonValueKind.Null, "Selected null disappeared from JSON");
        Require(Counts() == beforeReads && Ledger(sparse).Changes().Count == 0 && Ledger(wide).Changes().Count == 0, "E/JSON or hydration performed implicit work");
        Require(E.PurchaseOrder(sparse).Version().Eval() == 1 && E.PurchaseOrder(wide).Version().Eval() == 1, "Read changed optimistic identity");
        Case("independent-query-projection-boundary", one, new { sameIdentity = true, separateViews = true, sparseJsonUnchanged = true,
            sparseJsonOmitsFields = true, wideNullIsLoaded = true, implicitProviderCalls = 0, hydrationDirtyKeys = 0, version = 1 });

        var two = await Seed(); var left = await Full(two.Id!.Value); var other = await Full(two.Id.Value);
        Require(!ReferenceEquals(left, other) && !ReferenceEquals(Ledger(left), Ledger(other)), "Independent query operations share a mutable view or ledger");
        left.UpdateContactPhone("5550701"); other.UpdateInternalNote("pending-other-view");
        Require(Ledger(left).Change(Key(left)).Keys.SequenceEqual(new[] { "contact_phone" })
            && Ledger(other).Change(Key(other)).Keys.SequenceEqual(new[] { "internal_note" }), "Initial pending edits leaked between views");
        context.WithBusinessClock(new FixedBusinessClock(second));
        await left.AuditAs("Save only the first independent view").SaveAsync(context);
        var stored = await Full(two.Id.Value);
        Require(E.PurchaseOrder(stored).ContactPhone().Eval() == "5550701" && E.PurchaseOrder(stored).InternalNote().Eval() is null
            && E.PurchaseOrder(stored).Version().Eval() == 2, "Saving one view persisted another view's pending field");
        Require(E.PurchaseOrder(other).Version().Eval() == 1 && Ledger(other).OriginalVersion(Key(other)) == 1
            && Ledger(other).Change(Key(other))["internal_note"] == new Value.TextValue("pending-other-view"), "Successful save changed another view's intent or version");
        var beforeReject = Counts(); Exception? failure = null;
        try { await other.AuditAs("Reject a stale independent view without losing pending intent").SaveAsync(context); }
        catch (Exception error) { failure = error; }
        var afterReject = Counts(); var rejection = afterReject - beforeReject;
        Console.WriteLine("DOTNET_STALE_VIEW_OBSERVATION=" + JsonSerializer.Serialize(new { round,
            orderId = two.Id.Value.ToString(System.Globalization.CultureInfo.InvariantCulture), errorType = failure?.GetType().FullName,
            message = failure?.Message, effects = rejection }));
        Require(failure is not null, "A stale independent view was accepted");
        Require(rejection.begins == 1 && rejection.mutations == 1 && rejection.rollbacks == 1 && rejection.commits == 0
            && rejection.audits == 0 && rejection.idStatements == 0, "Stale rejection committed, audited or performed unrelated work");
        var pending = Ledger(other).Change(Key(other));
        Require(E.PurchaseOrder(other).Version().Eval() == 1 && Ledger(other).OriginalVersion(Key(other)) == 1
            && pending["internal_note"] == new Value.TextValue("pending-other-view"), "Rejection lost pending business intent or changed its original version");
        var derived = pending.Keys.Where(k => k != "internal_note").OrderBy(k => k).ToArray();
        Console.WriteLine("DOTNET_PENDING_VIEW_OBSERVATION=" + JsonSerializer.Serialize(new { round,
            keys = pending.Keys.OrderBy(k => k).ToArray(), values = pending.ToDictionary(p => p.Key,
                p => new { type = p.Value.GetType().Name, value = p.Value.ToJsonValue() }) }));
        Require(derived.SequenceEqual(new[] { "update_time" }) && pending["update_time"] == new Value.TimestampValue(second.ToUnixTimeMilliseconds()),
            "Only the exact managed BusinessClock Fix may be added to pending intent");
        Case("same-identity-independent-mutation-ledgers", two, new { separateViews = true, otherPendingEditRetained = true,
            noCrossViewWrite = true, staleException = failure!.GetType().FullName, storedVersion = 2, otherVersion = 1,
            derivedFixFields = derived, rejectionRolledBack = true, commitAuditsOnRejection = 0 });

        context.WithBusinessClock(new FixedBusinessClock(first));
        var three = await Seed(); var tenantId = E.PurchaseOrder(three).TenantId().Eval() ?? throw new InvalidOperationException("No root reference");
        var root = (await Q.TenantsWithMinimalFields().WithIdIs(tenantId)
            .SelectPurchaseOrderListWith(Q.PurchaseOrdersWithMinimalFields().WithIdIs(three.Id!.Value)
                .SelectTenantWith(Q.Tenants().Limit(1).Comment("Load the full nested alias of the sparse root"))
                .Limit(1).Comment("Load one parent between two views of the root identity"))
            .Limit(1).Comment("Keep sparse root and complete nested alias independent")
            .Purpose("One query must preserve each explicit projection boundary").ExecuteForListAsync(context)).Single();
        var beforeNested = Counts(); var rootE = E.Tenant(root);
        Require(rootE.Id().Eval() == rootE.PurchaseOrderList().First().Tenant().Id().Eval(), "The repeated identity is not the same persisted root");
        Missing(() => rootE.Name().Eval()); Require(rootE.PurchaseOrderList().First().Tenant().Name().Eval() == "Tenant Name", "The full nested alias lost its name");
        using (var json = JsonDocument.Parse(root.ToJson())) Require(!json.RootElement.TryGetProperty("name", out _)
            && json.RootElement.GetProperty("purchaseOrderList")[0].GetProperty("tenant").GetProperty("name").GetString() == "Tenant Name", "JSON merged two distinct projection boundaries");
        Require(Counts() == beforeNested, "Nested E/JSON caused hidden provider work");
        Case("intra-query-repeated-identity-projection-boundary", three, new { sameIdentity = true, sparseRootNameNotLoaded = true,
            nestedTargetNameLoaded = true, jsonRespectsBothViews = true, implicitProviderCalls = 0 });
    }

    sealed record Effects(int queries, int mutations, int begins, int commits, int rollbacks, int idStatements, int policies, int audits)
    {
        public static Effects operator -(Effects a, Effects b) => new(a.queries-b.queries, a.mutations-b.mutations, a.begins-b.begins,
            a.commits-b.commits, a.rollbacks-b.rollbacks, a.idStatements-b.idStatements, a.policies-b.policies, a.audits-b.audits);
    }
    sealed class Schema(RuntimeModule module) : ISchemaProvider { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
    sealed class Audit : IAppAuditEventSink
    { public int Calls; public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default) { Calls++; return Task.CompletedTask; } }
    sealed class Transport(SqliteTransport inner) : IAutomaticMutationTransactionTransport
    {
        public int Queries, Mutations, Begins, Commits, Rollbacks, IdStatements;
        void Observe(CompiledQuery query)
        {
            var sql = query.Sql.Replace("\"", "");
            if (sql.Contains("teaql_id_space", StringComparison.OrdinalIgnoreCase)) IdStatements++;
            if (Regex.IsMatch(sql, @"^\s*(?:INSERT INTO|UPDATE|DELETE FROM)\s+(?:tenant_data|purchase_order_data|order_item_data)\b", RegexOptions.IgnoreCase)) Mutations++;
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
