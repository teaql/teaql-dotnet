using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

// Application-owned regression; generated implementations remain hash-checked.
static class PartialRootProbe
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static async Task Verify(string filename)
    {
        var first = new DateTimeOffset(2032, 4, 1, 10, 15, 30, TimeSpan.Zero);
        var second = first.AddDays(1);
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module;
        var transport = new Transport(new SqliteTransport(connection)); var audit = new Audit(); var policy = new Policy(); var lookups = 0;
        var context = module.IntoContext()
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module)))
            .WithBusinessClock(new FixedBusinessClock(first)).WithAppAuditEventSink(audit)
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => { lookups++; return policy; }));
        await context.EnsureSchemaAsync();
        var active = context.RequireActiveRoot("Tenant");
        Effects Counts() => new(transport.Statements, transport.Begins, transport.Commits, transport.Rollbacks,
            transport.IdStatements, transport.Mutations, transport.RootLookups, lookups, policy.Calls, audit.Calls);
        void Reset() { transport.Reset(); lookups = 0; policy.Calls = 0; audit.Calls = 0; }
        async Task<Dictionary<string, List<Dictionary<string, object?>>>> Physical()
        {
            await using var oracle = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename, Mode = SqliteOpenMode.ReadOnly }.ToString());
            await oracle.OpenAsync(); var result = new Dictionary<string, List<Dictionary<string, object?>>>();
            foreach (var table in new[] { "tenant_data", "purchase_order_data", "order_item_data" })
            {
                using var command = oracle.CreateCommand(); command.CommandText = "SELECT * FROM " + table + " ORDER BY id";
                using var reader = await command.ExecuteReaderAsync(); var rows = new List<Dictionary<string, object?>>();
                while (await reader.ReadAsync()) rows.Add(Enumerable.Range(0, reader.FieldCount)
                    .ToDictionary(reader.GetName, i => reader.IsDBNull(i) ? null : reader.GetValue(i)));
                result[table] = rows;
            }
            return result;
        }
        Reset();
        var other = Q.Tenants().Comment("Prepare another persisted domain root")
            .Purpose("Test another real context rather than an invented ID").NewEntity(context);
        other.UpdateName("Independent partial root");
        await other.AuditAs("Create an independent root for partial-ancestor admission").SaveAsync(context);
        var otherId = other.Id ?? throw new InvalidOperationException("No persisted independent root ID");
        Require(otherId != active.Id && context.RequireActiveRoot("Tenant").Id == active.Id, "Creating another root changed the caller scope");
        var order = Q.PurchaseOrders().Comment("Prepare the managed partial-ancestor fixture")
            .Purpose("Verify unchanged ownership at the modified leaf").NewEntity(context);
        order.UpdateContactPhone("5550801"); order.UpdateInternalNote(null!); // Deliberate NULL for the optional modeled field.
        var child = Q.OrderItems().Comment("Prepare the complete managed leaf")
            .Purpose("Persist the leaf under its original active root").NewEntity(context);
        child.UpdateProductName("Managed partial original"); child.UpdateUnitPrice(10.00m);
        order.OrderItemList.Add(child);
        await order.AuditAs("Create the owned graph for partial-root verification").SaveAsync(context);
        var orderId = order.Id ?? throw new InvalidOperationException("No persisted order ID");
        var childId = child.Id ?? throw new InvalidOperationException("No persisted leaf ID");
        _ = await Q.Tenants().WithIdIs(otherId).Limit(1).Comment("Calibrate the root SELECT observer")
            .Purpose("Detect provider lookups during root admission").ExecuteForListAsync(context);
        using (var transaction = await transport.BeginSqlAsync()) await transaction.RollbackSqlAsync();
        var calibration = Counts();
        Require(calibration.statements > 0 && calibration.begins > 0 && calibration.commits > 0 && calibration.rollbacks > 0
            && calibration.idStatements > 0 && calibration.mutations > 0 && calibration.rootLookups > 0
            && calibration.policies > 0 && calibration.reviews > 0 && calibration.audits > 0, "Every side-effect observer must be positively calibrated");
        var root = (await Q.TenantsWithMinimalFields().WithIdIs(active.Id)
            .SelectPurchaseOrderListWith(Q.PurchaseOrdersWithMinimalFields().WithIdIs(orderId)
                .SelectOrderItemListWith(Q.OrderItems().WithIdIs(childId).Limit(1).Comment("Load the complete owned leaf"))
                .Limit(1).Comment("Keep the unmodified parent partial"))
            .Limit(1).Comment("Load two untouched partial ancestors above the complete leaf")
            .Purpose("Verify root ownership when it is absent from the mutation delta").ExecuteForListAsync(context)).Single();
        Require(E.Tenant(root).PurchaseOrderList().Size().Eval() == 1, "Selected parent disappeared");
        var parent = root.PurchaseOrderList.Single();
        Require(E.PurchaseOrder(parent).OrderItemList().Size().Eval() == 1, "Selected leaf disappeared");
        var leaf = parent.OrderItemList.Single();
        Require(E.OrderItem(leaf).TenantId().Eval() == active.Id, "Loaded leaf lost original ownership");
        string Sparse()
        {
            foreach (var read in new Func<object?>[] { () => E.Tenant(root).Name().Eval(), () => E.PurchaseOrder(parent).ContactPhone().Eval() })
            {
                var missing = false; try { _ = read(); } catch (TeaQLNotLoadedException) { missing = true; }
                Require(missing, "An untouched partial ancestor became complete");
            }
            using var json = JsonDocument.Parse(root.ToJson()); var value = json.RootElement;
            Require(!value.TryGetProperty("name", out _) && !value.GetProperty("purchaseOrderList")[0].TryGetProperty("contactPhone", out _), "JSON promoted a sparse ancestor");
            return JsonSerializer.Serialize(new { rootVersion = E.Tenant(root).Version().Eval(), parentVersion = E.PurchaseOrder(parent).Version().Eval(),
                keys = value.EnumerateObject().Select(p => p.Name).OrderBy(n => n).ToArray(),
                parentKeys = value.GetProperty("purchaseOrderList")[0].EnumerateObject().Select(p => p.Name).OrderBy(n => n).ToArray() });
        }
        var projection = Sparse(); var beforeRows = await Physical(); var beforeStorage = JsonSerializer.Serialize(beforeRows);
        context.WithBusinessClock(new FixedBusinessClock(second)); leaf.UpdateProductName("Managed partial retry");
        var field = typeof(OrderItem).GetField("_entityRoot", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("The existing readonly ledger observation seam changed");
        var ledger = (EntityRoot)field.GetValue(leaf)!; var key = new EntityKey("OrderItem", childId);
        string Pending() => ledger.Change(key).ToJsonValue().ToJsonString();
        var intent = Pending(); Require(!ledger.Change(key).ContainsKey("tenant"), "Fixture must leave ownership outside the writable delta");
        context.WithActiveRoot("Tenant", otherId); var zero = new Effects(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        var denials = new List<object>();
        for (var attempt = 1; attempt <= 2; attempt++)
        {
            Reset(); Exception? failure = null;
            try { await root.AuditAs("Reject the complete leaf under another active root").SaveAsync(context); }
            catch (Exception error) { failure = error; }
            var effects = Counts(); var storageUnchanged = JsonSerializer.Serialize(await Physical()) == beforeStorage;
            var violations = failure is CheckException check ? check.Violations.Select(v => v.ToWire()).ToArray() : Array.Empty<WireCheckResult>();
            var observation = new { attempt, effects, intentRetained = Pending() == intent,
                sparseViewUnchanged = Sparse() == projection, storageUnchanged, violations };
            denials.Add(observation); Console.WriteLine("CORE_DOTNET_PARTIAL_ROOT_DENIAL=" + JsonSerializer.Serialize(observation));
            Require(failure is CheckException, "Wrong-context leaf save was not rejected by Checker");
            Require(effects == zero && observation.intentRetained && observation.sparseViewUnchanged && storageUnchanged, "Rejected save changed intent, projection, storage or services");
            Require(violations.Length == 1 && violations[0].RuleId == "context_root_mismatch" && violations[0].EntityType == "OrderItem"
                && violations[0].InstancePath == "/purchaseOrderList/0/orderItemList/0/tenant", "Ownership failure lost its declaring type or graph ancestry");
        }
        context.WithActiveRoot(active.EntityType, active.Id); Reset();
        await root.AuditAs("Save the same complete leaf under its original active root").SaveAsync(context);
        var acceptedEffects = Counts();
        Require(acceptedEffects.begins == 1 && acceptedEffects.commits == 1 && acceptedEffects.rollbacks == 0 && acceptedEffects.idStatements == 0
            && acceptedEffects.mutations == 1 && acceptedEffects.rootLookups == 0 && acceptedEffects.policies > 0 && acceptedEffects.reviews > 0 && acceptedEffects.audits == 1,
            "Same-graph retry performed unrelated work");
        Require(ledger.Change(key).Count == 0 && Sparse() == projection, "Committed retry did not clear intent or preserve sparse ancestors");
        var stored = await Physical();
        foreach (var table in new[] { "tenant_data", "purchase_order_data" }) Require(JsonSerializer.Serialize(stored[table]) == JsonSerializer.Serialize(beforeRows[table]), "Untouched ancestor was persisted");
        var row = stored["order_item_data"].Single(v => Convert.ToInt64(v["id"], CultureInfo.InvariantCulture) == childId);
        Require(Convert.ToInt64(row["version"], CultureInfo.InvariantCulture) == 2 && Convert.ToInt64(row["tenant"], CultureInfo.InvariantCulture) == active.Id
            && Convert.ToInt64(row["purchase_order"], CultureInfo.InvariantCulture) == orderId && (string?)row["product_name"] == "Managed partial retry"
            && Convert.ToDecimal(row["unit_price"], CultureInfo.InvariantCulture) == 10.00m
            && Convert.ToInt64(row["create_time"], CultureInfo.InvariantCulture) == first.ToUnixTimeMilliseconds()
            && Convert.ToInt64(row["update_time"], CultureInfo.InvariantCulture) == second.ToUnixTimeMilliseconds(), "Stored leaf differs from the accepted business command");
        Console.WriteLine("CORE_DOTNET_PARTIAL_ROOT_RESULT=" + JsonSerializer.Serialize(new { language = "dotnet", passed = true,
            orderId, leafId = childId, activeRootId = active.Id, otherRootId = otherId, calibration, denials, acceptedEffects,
            ancestorVersions = new[] { E.Tenant(root).Version().Eval(), E.PurchaseOrder(parent).Version().Eval() },
            leafVersion = E.OrderItem(leaf).Version().Eval(), sameGraphRetry = true, intentClearedAfterCommit = true, generatedImplementationsModified = false }));
    }
    sealed record Effects(int statements, int begins, int commits, int rollbacks, int idStatements, int mutations, int rootLookups, int policies, int reviews, int audits);
    sealed class Schema(RuntimeModule module) : ISchemaProvider { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
    sealed class Audit : IAppAuditEventSink
    { public int Calls; public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default) { Calls++; return Task.CompletedTask; } }
    sealed class Policy : IMutationPolicy
    { public int Calls; public MutationPolicyIdentity Identity { get; } = new("partial-root-verification", "1", "partial-root-v1");
      public MutationDecision Review(UserContext context, MutationPlan plan) { Calls++; return MutationDecision.Allowed(); } }
    sealed class Transport(SqliteTransport inner) : IAutomaticMutationTransactionTransport
    {
        public int Statements, Begins, Commits, Rollbacks, IdStatements, Mutations, RootLookups;
        public void Reset() { Statements = Begins = Commits = Rollbacks = IdStatements = Mutations = RootLookups = 0; }
        void Observe(CompiledQuery query)
        {
            Statements++; var sql = query.Sql.Replace("\"", "");
            if (sql.Contains("teaql_id_space", StringComparison.OrdinalIgnoreCase)) IdStatements++;
            if (Regex.IsMatch(sql, @"^\s*(?:INSERT INTO|UPDATE|DELETE FROM)\s+(?:tenant_data|purchase_order_data|order_item_data)\b", RegexOptions.IgnoreCase)) Mutations++;
            if (Regex.IsMatch(sql, @"\bFROM\s+tenant_data\b", RegexOptions.IgnoreCase)) RootLookups++;
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
