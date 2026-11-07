using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

// Business writes use only generated Q/E/audited Save. Direct SQLite reads are
// an independent physical oracle; transport counters include ID/transaction work.
static class CheckerMetadataProbe
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static async Task Verify(SqliteConnection connection)
    {
        var transport = new CountingTransport(new SqliteTransport(connection));
        var module = GeneratedRuntimeModule.Module;
        var audit = new Audit();
        UserContext Context(RuntimeModule runtimeModule) => runtimeModule.IntoContext()
            .WithActiveRoot("Tenant", 1)
            .WithBusinessClock(new FixedBusinessClock(new DateTimeOffset(2032, 3, 1, 11, 15, 30, TimeSpan.Zero)))
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new ModuleSchemaProvider(runtimeModule)))
            .WithAppAuditEventSink(audit);
        var context = Context(module);
        async Task<PurchaseOrder> ParentAsync(UserContext ctx) => (await Q.PurchaseOrders().Comment("Prepare synthetic Checker metadata")
            .Purpose("Verify model checks precede all database work").NewEntityAsync(ctx));
        async Task<OrderItem> ChildAsync(UserContext ctx, string? name, decimal price)
        {
            var child = (await Q.OrderItems().Comment("Prepare a synthetic child constraint")
                .Purpose("Verify indexed graph diagnostics").NewEntityAsync(ctx));
            if (name != null) child.UpdateProductName(name);
            child.UpdateUnitPrice(price);
            return child;
        }
        var calibrated = (await ParentAsync(context)); calibrated.UpdateContactPhone("5550970");
        transport.Reset(); audit.Calls = 0;
        calibrated = await calibrated.AuditAs("Calibrate the successful generated save").SaveAsync(context);
        Require(transport.Statements > 0 && transport.Begins > 0 && audit.Calls == 1,
            "Successful save must calibrate SQL, transaction and audit observers");
        var id = calibrated.Id ?? throw new InvalidOperationException("Calibration did not allocate an ID");
        var calibrationStatements = transport.Statements;
        var cases = new List<object>(); var missing = new List<string>();

        async Task Reject(string name, UserContext ctx, PurchaseOrder owner, string rule,
            string modelPath, string entityType, object? expected = null, bool specialized = false)
        {
            var before = await Snapshot(connection);
            transport.Reset(); audit.Calls = 0;
            CheckResult violation;
            try
            {
                await owner.AuditAs("Reject the synthetic " + name + " case before persistence").SaveAsync(ctx);
                throw new InvalidOperationException("Invalid generated save succeeded: " + name);
            }
            catch (CheckException error)
            {
                violation = error.Violations.Single(result => result.RuleId == rule && result.ModelPath == modelPath);
            }
            Require(transport.Statements == 0 && transport.Begins == 0 && audit.Calls == 0,
                "Checker rejection reached SQL, transaction or audit: " + name);
            Require(await Snapshot(connection) == before, "Rejected save changed physical data: " + name);
            if (violation.EntityType != entityType) missing.Add(name + ":entityType");
            if (expected != null && Convert.ToDecimal(violation.SystemValue, CultureInfo.InvariantCulture)
                != Convert.ToDecimal(expected, CultureInfo.InvariantCulture)) missing.Add(name + ":expectedConstraint");
            if (modelPath.Contains('[') && !violation.Message!.Contains(violation.NativePath, StringComparison.Ordinal))
                missing.Add(name + ":humanIndexedPath");
            if (specialized && violation.SourceInstancePath != "/orderItems/1/product~1name~0alias")
                missing.Add(name + ":sourceAlias");
            var wires = new List<object>();
            foreach (var profile in Enum.GetValues<JsonFieldNamingProfile>())
            {
                // Serialize the real runtime DTO. Rebuilding segments in this
                // application would conceal the abstract-record JSON defect.
                var json = JsonSerializer.SerializeToElement(violation.ToWire(profile),
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                Require(json.GetProperty("instancePath").GetString() == violation.Location.InstancePathWith(profile),
                    "Wire profile changed semantic location");
                var segments = json.GetProperty("location");
                if (segments.EnumerateArray().Any(segment => !segment.TryGetProperty("kind", out _)))
                    missing.Add(name + ":wireSegments:" + profile);
                wires.Add(new { profile = profile.ToString(), value = json });
            }
            cases.Add(new { name, rule, entityType = violation.EntityType, modelPath = violation.ModelPath,
                nativePath = violation.NativePath, message = violation.Message,
                systemValue = violation.SystemValue, violation.SourceInstancePath,
                statements = transport.Statements, begins = transport.Begins, audits = audit.Calls,
                physicalStateUnchanged = true, wires });
        }

        await Reject("required-parent", context, (await ParentAsync(context)), "required", "contact_phone", "PurchaseOrder");
        var longPhone = (await ParentAsync(context)); longPhone.UpdateContactPhone(new string('7', 33));
        await Reject("length-parent", context, longPhone, "max_length", "contact_phone", "PurchaseOrder", 32);
        var rows = await Q.PurchaseOrders().WithIdIs(id).Limit(1).Comment("Load a complete update target")
            .Purpose("Verify explicit null fails before SQL").ExecuteForListAsync(context);
        rows[0].UpdateContactPhone(null!);
        await Reject("required-null-update", context, rows[0], "required", "contact_phone", "PurchaseOrder");
        var partial = await Q.PurchaseOrdersWithMinimalFields().WithIdIs(id).Limit(1)
            .Comment("Load an identity-only update target").Purpose("Verify direct partial-owner admission metadata")
            .ExecuteForListAsync(context);
        partial[0].UpdateContactPhone("5550971");
        await Reject("partial-owner", context, partial[0], "invalid_type", "tenant", "PurchaseOrder");

        foreach (var item in new (string Name, string? Product, decimal Price, string Rule, string Field, object? Bound)[]
        {
            ("required-child", null, 10m, "required", "product_name", null),
            ("length-child", new string('p', 81), 10m, "max_length", "product_name", 80),
            ("minimum-child", "synthetic", 0m, "min", "unit_price", 0.01m),
            ("exact-below-minimum", "synthetic", 0.009999999999999999999999m, "min", "unit_price", 0.01m),
            ("maximum-child", "synthetic", 1000001m, "max", "unit_price", 1000000m),
            ("exact-above-maximum", "synthetic", 1000000.000000000000000001m, "max", "unit_price", 1000000m)
        })
        {
            var owner = (await ParentAsync(context)); owner.UpdateContactPhone("5550972");
            owner.OrderItemList.Add((await ChildAsync(context, "valid preceding item", 10m)));
            owner.OrderItemList.Add((await ChildAsync(context, item.Product, item.Price)));
            await Reject(item.Name, context, owner, item.Rule, "order_item_list[1]." + item.Field, "OrderItem", item.Bound);
        }
        await Reject("localized-required", context.SetLocaleCode("zh-CN"), (await ParentAsync(context)),
            "required", "contact_phone", "PurchaseOrder");
        context.SetLocaleCode("en");

        var specialContext = Context(module.And(new RuntimeModule().Checker("OrderItem", new SpecializedChecker())));
        var special = (await ParentAsync(specialContext)); special.UpdateContactPhone("5550973");
        special.OrderItemList.Add((await ChildAsync(specialContext, "valid preceding item", 10m)));
        special.OrderItemList.Add((await ChildAsync(specialContext, "specialized synthetic item", 10m)));
        await Reject("custom-specialized-child", specialContext, special, "max_length",
            "order_item_list[1].product_name", "ReviewedOrderItem", 80, specialized: true);

        var endpoints = new List<string>();
        foreach (var price in new[] { 0.01m, 1000000m })
        {
            var child = (await ChildAsync(context, "inclusive endpoint", price)); child.UpdatePurchaseOrderId(id);
            transport.Reset(); audit.Calls = 0;
            var saved = await child.AuditAs("Accept the exact inclusive modeled endpoint").SaveAsync(context);
            Require(E.OrderItem(saved).UnitPrice().Eval() == price && transport.Statements > 0
                && transport.Begins > 0 && audit.Calls == 1, "Inclusive bound or calibrated observers failed");
            using var stored = connection.CreateCommand();
            stored.CommandText = "SELECT CAST(unit_price AS TEXT) FROM order_item_data WHERE id=$id";
            stored.Parameters.AddWithValue("$id", saved.Id!.Value);
            Require(decimal.Parse((string)(await stored.ExecuteScalarAsync())!, CultureInfo.InvariantCulture) == price,
                "The independent physical oracle contradicts the accepted endpoint");
            endpoints.Add(price.ToString(CultureInfo.InvariantCulture));
        }
        Console.WriteLine("DOTNET_CHECKER_METADATA=" + JsonSerializer.Serialize(new {
            language = "dotnet", passed = missing.Count == 0, normalGeneratedSave = true,
            calibrationStatements, cases, acceptedEndpoints = endpoints, missing
        }));
        Require(missing.Count == 0, "Checker metadata incomplete: " + string.Join(", ", missing));
    }

    private static async Task<string> Snapshot(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 'orders',id,version,contact_phone FROM purchase_order_data UNION ALL "
            + "SELECT 'items',id,version,product_name FROM order_item_data ORDER BY 1,2";
        using var reader = await command.ExecuteReaderAsync(); var rows = new List<object[]>();
        while (await reader.ReadAsync())
            rows.Add(Enumerable.Range(0, reader.FieldCount).Select(index => reader.GetValue(index)).ToArray());
        return JsonSerializer.Serialize(rows);
    }

    private sealed class SpecializedChecker : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        {
            var values = request is InsertMutationRequest insert ? insert.Command.Values : new Record();
            if (!values.TryGetValue("product_name", out var name) || name.TryText() != "specialized synthetic item") return [];
            return [new CheckResult { RuleId = "max_length", EntityType = "ReviewedOrderItem",
                Location = ObjectLocation.Property("product_name"), SystemValue = 80,
                SourceInstancePath = "/orderItems/1/product~1name~0alias" }];
        }
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
