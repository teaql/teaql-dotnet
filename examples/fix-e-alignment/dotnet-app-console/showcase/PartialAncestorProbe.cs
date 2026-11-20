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

// Only application-owned tests: generated Q/E, native JSON and audited graph save.
static class PartialAncestorProbe
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static async Task Verify(string filename)
    {
        var first = new DateTimeOffset(2032, 2, 29, 10, 15, 30, TimeSpan.Zero);
        var second = first.AddDays(1);
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module; var transport = new Transport(new SqliteTransport(connection));
        var audit = new Audit(); var policies = 0;
        var context = module.IntoContext()
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module)))
            .WithBusinessClock(new FixedBusinessClock(first))
            .WithAppAuditEventSink(audit)
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => { policies++; return null; }));
        await context.EnsureSchemaAsync();
        Effects Counts() => new(transport.Statements, transport.Begins, transport.IdStatements, transport.Mutations, policies, audit.Calls);
        void Reset() { transport.Reset(); policies = 0; audit.Calls = 0; }
        Reset();
        var parent = (await Q.PurchaseOrders().Comment("Prepare the partial-ancestor fixture")
            .Purpose("Validate only modified business objects for graph saves").NewEntityAsync(context));
        parent.UpdateContactPhone("5550600");
        var child = (await Q.OrderItems().Comment("Prepare one complete leaf")
            .Purpose("Use the generated relation and audited save").NewEntityAsync(context));
        child.UpdateProductName("Partial graph original"); child.UpdateUnitPrice(10.00m);
        parent.OrderItemList.Add(child);
        await parent.AuditAs("Create the partial-ancestor graph").SaveAsync(context);
        var calibration = Counts();
        Require(calibration.statements > 0 && calibration.transactions == 1 && calibration.idStatements > 0
            && calibration.mutations == 2 && calibration.policies > 0 && calibration.audits == 2,
            "Every observer must be calibrated by actual generated creation");
        var parentId = parent.Id ?? throw new InvalidOperationException("No persisted parent identity");
        var seeded = (await Q.PurchaseOrders().WithIdIs(parentId)
            .SelectOrderItemListWith(Q.OrderItems().Limit(1).Comment("Read the persisted fixture leaf"))
            .Limit(1).Comment("Read the saved graph identity").Purpose("Never infer a child ID from allocation order")
            .ExecuteForListAsync(context)).Single();
        var childId = E.PurchaseOrder(seeded).OrderItemList().First().Id().Eval()
            ?? throw new InvalidOperationException("No persisted child identity");
        var tenantId = E.PurchaseOrder(seeded).TenantId().Eval()
            ?? throw new InvalidOperationException("No context root reference");
        async Task<(Tenant root, PurchaseOrder parent, OrderItem leaf)> Load(bool complete)
        {
            var leaf = (complete ? Q.OrderItems() : Q.OrderItemsWithMinimalFields()).WithIdIs(childId)
                .Limit(1).Comment(complete ? "Load every scalar of the modified leaf" : "Load an incomplete modified leaf");
            var root = (await Q.TenantsWithMinimalFields().WithIdIs(tenantId)
                .SelectPurchaseOrderListWith(Q.PurchaseOrdersWithMinimalFields().WithIdIs(parentId)
                    .SelectOrderItemListWith(leaf).Limit(1).Comment("Keep the untouched parent partial"))
                .Limit(1).Comment("Load two untouched partial ancestors above one leaf")
                .Purpose("Exercise Q E JSON and graph mutation in one bounded example").ExecuteForListAsync(context)).Single();
            Require(E.Tenant(root).PurchaseOrderList().Size().Eval() == 1, "Selected parent disappeared");
            var order = root.PurchaseOrderList.Single();
            Require(E.PurchaseOrder(order).OrderItemList().Size().Eval() == 1, "Selected leaf disappeared");
            return (root, order, order.OrderItemList.Single());
        }
        var graph = await Load(true);
        var empty = (await Q.PurchaseOrdersWithMinimalFields().WithIdIs(parentId)
            .SelectOrderItemListWith(Q.OrderItemsWithMinimalFields().WithIdIs(0).Limit(1).Comment("Explicitly select an empty child list"))
            .Limit(1).Comment("Distinguish selected empty from unselected reverse relations")
            .Purpose("Native JSON preserves availability without implicit queries").ExecuteForListAsync(context)).Single();
        Require(E.PurchaseOrder(empty).OrderItemList().Size().Eval() == 0, "Selected list should genuinely be empty");
        Reset();
        void Missing(Func<object?> read, string pointer)
        {
            try { _ = read(); }
            catch (TeaQLNotLoadedException error) { Require(error.ToWire().InstancePath == pointer, "Wrong missing-field location"); return; }
            throw new InvalidOperationException("Missing fixed field became loaded null");
        }
        Missing(() => E.Tenant(graph.root).Name().Eval(), "/name");
        Missing(() => E.Tenant(graph.root).PurchaseOrderList().First().ContactPhone().Eval(), "/purchaseOrderList/0/contactPhone");
        Require(E.OrderItem(graph.leaf).UnitPrice().Eval() == 10.00m, "Complete leaf lost its Decimal");
        using (var document = JsonDocument.Parse(graph.root.ToJson()))
        {
            var json = document.RootElement;
            Require(!json.TryGetProperty("name", out _) && !json.TryGetProperty("orderItemList", out _), "JSON fabricated unselected fields");
            var orders = json.GetProperty("purchaseOrderList"); Require(orders.GetArrayLength() == 1, "JSON lost the selected parent");
            Require(!orders[0].TryGetProperty("contactPhone", out _), "JSON fabricated a partial parent scalar");
            Require(orders[0].GetProperty("orderItemList").GetArrayLength() == 1, "JSON lost the selected grandchild");
        }
        using (var document = JsonDocument.Parse(empty.ToJson()))
            Require(document.RootElement.GetProperty("orderItemList").GetArrayLength() == 0, "JSON lost the loaded empty list");
        var zero = new Effects(0, 0, 0, 0, 0, 0); var guardEffects = Counts(); Require(guardEffects == zero, "E or native JSON performed hidden work");
        context.WithBusinessClock(new FixedBusinessClock(second));
        graph.leaf.UpdateProductName("Partial graph updated");
        await graph.root.AuditAs("Save the complete modified leaf through untouched partial ancestors").SaveAsync(context);
        var saveEffects = Counts();
        Require(saveEffects.transactions == 1 && saveEffects.idStatements == 0 && saveEffects.mutations == 1
            && saveEffects.policies > 0 && saveEffects.audits == 1, "Saving one modified leaf performed unrelated work");
        async Task<string> Physical()
        {
            await using var oracle = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename, Mode = SqliteOpenMode.ReadOnly }.ToString());
            await oracle.OpenAsync(); var result = new Dictionary<string, List<Dictionary<string, object?>>>();
            foreach (var table in new[] { "tenant_data", "purchase_order_data", "order_item_data" })
            {
                using var command = oracle.CreateCommand(); command.CommandText = "SELECT * FROM " + table + " ORDER BY id";
                using var reader = await command.ExecuteReaderAsync(); var rows = new List<Dictionary<string, object?>>();
                while (await reader.ReadAsync())
                {
                    var row = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => reader.IsDBNull(i) ? null : reader.GetValue(i));
                    Require(Convert.ToInt64(row["version"], CultureInfo.InvariantCulture) == (table == "order_item_data" ? 2 : 1), "Wrong physical version");
                    if (table == "purchase_order_data") Require((string?)row["contact_phone"] == "5550600" && row["internal_note"] is null, "Ancestor changed");
                    if (table == "order_item_data")
                    {
                        Require((string?)row["product_name"] == "Partial graph updated", "Wrong persisted leaf");
                        Require(Convert.ToDecimal(row["unit_price"], CultureInfo.InvariantCulture) == 10.00m, "Decimal changed");
                        Require(Convert.ToInt64(row["purchase_order"], CultureInfo.InvariantCulture) == Convert.ToInt64(row["id"], CultureInfo.InvariantCulture), "Broken FK");
                    }
                    rows.Add(row);
                }
                result[table] = rows;
            }
            return JsonSerializer.Serialize(result);
        }
        var stored = await Physical();
        var partial = await Load(false); partial.leaf.UpdateProductName("Rejected partial leaf");
        // Read-only test seam, identical to the existing E boundary observer.
        // No generated implementation is inspected or changed for API discovery.
        var ledgerField = typeof(OrderItem).GetField("_entityRoot", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("The ledger observation seam changed");
        var ledger = (EntityRoot)ledgerField.GetValue(partial.leaf)!;
        string Pending() => ledger.Change(new EntityKey("OrderItem", childId)).ToJsonValue().ToJsonString();
        var before = Pending(); var rejections = new List<WireCheckResult[]>(); var rejectionEffects = new List<Effects>();
        for (var attempt = 0; attempt < 2; attempt++)
        {
            Reset();
            try { await partial.root.AuditAs("Reject the incomplete modified leaf before provider entry").SaveAsync(context);
                throw new InvalidOperationException("Incomplete modified leaf was accepted"); }
            catch (CheckException error)
            {
                var violations = error.Violations.Select(v => v.ToWire()).ToArray();
                Require(violations.Length > 0 && violations.All(v => v.RuleId == "invalid_type" && v.EntityType == "OrderItem"
                    && v.InstancePath.StartsWith("/purchaseOrderList/0/orderItemList/0/", StringComparison.Ordinal)), "Lost modified-leaf diagnostic ancestry");
                rejections.Add(violations);
            }
            Require(Counts() == zero, "Rejected incomplete leaf entered provider, policy or audit");
            Require(ReferenceEquals(ledgerField.GetValue(partial.leaf), ledger) && Pending() == before, "Rejection changed pending mutation intent");
            Require(await Physical() == stored, "Rejected mutation changed the physical graph"); rejectionEffects.Add(Counts());
        }
        Console.WriteLine("CORE_DOTNET_PARTIAL_ANCESTORS_RESULT=" + JsonSerializer.Serialize(new {
            language = "dotnet", passed = true, orderId = parentId, leafId = childId, ancestorVersions = new[] { 1, 1 }, leafVersion = 2,
            calibration, guardEffects, saveEffects, cases = new[] {
                "partial-ancestor-guards-and-JSON-no-I/O", "complete-leaf-saved-through-two-untouched-partial-ancestors",
                "partial-leaf-rejected-twice-before-provider-with-intent-retained" }, rejections, rejectionEffects,
            intentRetained = true, selectedEmptySerialized = true, unselectedListOmitted = true, generatedImplementationsModified = false
        }));
    }

    sealed record Effects(int statements, int transactions, int idStatements, int mutations, int policies, int audits);
    sealed class Schema(RuntimeModule module) : ISchemaProvider
    { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
    sealed class Audit : IAppAuditEventSink
    {
        public int Calls;
        public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default)
        { Calls++; return Task.CompletedTask; }
    }
    sealed class Transport(SqliteTransport inner) : IAutomaticMutationTransactionTransport
    {
        public int Statements, Begins, IdStatements, Mutations;
        public void Reset() { Statements = 0; Begins = 0; IdStatements = 0; Mutations = 0; }
        void Observe(CompiledQuery query)
        {
            Statements++; var sql = query.Sql.Replace("\"", "");
            if (sql.Contains("teaql_id_space", StringComparison.OrdinalIgnoreCase)) IdStatements++;
            if (Regex.IsMatch(sql, @"^\s*(?:INSERT INTO|UPDATE|DELETE FROM)\s+(?:tenant_data|purchase_order_data|order_item_data)\b", RegexOptions.IgnoreCase)) Mutations++;
        }
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Observe(query); return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Observe(query); return inner.ExecuteSqlAsync(query); }
        public async Task<ISqlTransaction> BeginSqlAsync() { Begins++; return new Transaction(await inner.BeginSqlAsync(), this); }
        sealed class Transaction(ISqlTransaction transaction, Transport owner) : ISqlTransaction
        {
            public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { owner.Observe(query); return transaction.FetchAllSqlAsync(query); }
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { owner.Observe(query); return transaction.ExecuteSqlAsync(query); }
            public Task CommitSqlAsync() => transaction.CommitSqlAsync();
            public Task RollbackSqlAsync() => transaction.RollbackSqlAsync();
            public void Dispose() => transaction.Dispose();
        }
    }
}
