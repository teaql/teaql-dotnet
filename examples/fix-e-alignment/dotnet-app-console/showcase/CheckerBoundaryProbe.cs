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

// Generated Q/E/audited saves only. SQL below is a read-only physical oracle.
// The reflection seam observes RuntimeModule's native checker registry solely
// to wrap the real generated Checker; it never discovers generated APIs or
// replaces model constraints with application-written validation.
static class CheckerBoundaryProbe
{
    private sealed class SchemaFreeIdentityAllocator : IInternalIdAllocator
    {
        private ulong level;
        public Task<ulong> NextIdAsync(string entity) => Task.FromResult(++level);
        public Task EnsureIdFloorAsync(string entity, ulong floor)
        { level = Math.Max(level, floor); return Task.CompletedTask; }
    }
    static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    static string Profile(JsonFieldNamingProfile profile) => profile switch {
        JsonFieldNamingProfile.CamelCase => "camelCase", JsonFieldNamingProfile.SnakeCase => "snake_case", _ => "PascalCase" };

    public static async Task Verify(string filename)
    {
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
        await connection.OpenAsync();
        var transport = new Transport(new SqliteTransport(connection)); var state = new State(); var audit = new Audit();
        var original = GeneratedRuntimeModule.Module;
        var property = typeof(RuntimeModule).GetProperty("Checkers", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("Native checker observation seam changed");
        var checkers = (Dictionary<string, IEntityChecker>)property.GetValue(original)!;
        var overrides = new RuntimeModule();
        foreach (var (type, checker) in checkers) overrides.Checker(type, new ObservedChecker(type, checker, state));
        var module = original.And(overrides);
        UserContext Context(Transport observed) => module.IntoContext().WithActiveRoot("Tenant", 1)
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), observed, new Schema(module)))
            .WithBusinessClock(new FixedBusinessClock(new DateTimeOffset(2032, 2, 29, 10, 15, 30, TimeSpan.Zero)))
            .WithAppAuditEventSink(audit)
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => { state.Policies++; return null; }));
        var context = Context(transport); await context.EnsureSchemaAsync();
        using var count = connection.CreateCommand(); count.CommandText = "SELECT count(*) FROM purchase_order_data";
        var previousParents = Convert.ToInt32(await count.ExecuteScalarAsync(), CultureInfo.InvariantCulture);
        Require(previousParents % 5 == 0, "Use the same complete unreset boundary store");
        var round = previousParents / 5 + 1; Require(round is 1 or 2, "This retained fixture is a two-round example");
        var observations = 0;
        void Emit(string name, object facts, string? profile = null)
        {
            var row = new Dictionary<string, object?> { ["round"] = round, ["case"] = name, ["passed"] = true };
            if (profile != null) row["profile"] = profile;
            foreach (var p in JsonSerializer.SerializeToElement(facts, Json).EnumerateObject()) row[p.Name] = p.Value;
            Console.WriteLine("DOTNET_CHECKER_BOUNDARY_CASE=" + JsonSerializer.Serialize(row, Json)); observations++;
        }
        Effects Counts() => new(transport.Statements, transport.Begins, transport.ParentInserts, transport.ChildInserts,
            state.CheckerCalls, state.Mapped, state.Saves, state.Policies, audit.Calls);
        void Reset() { transport.Reset(); state.ResetCounts(); audit.Calls = 0; }
        async Task<PurchaseOrder> ParentAsync(UserContext ctx) => (await Q.PurchaseOrders().Comment("Prepare a bounded Checker fixture")
            .Purpose("Verify generated graph admission before persistence").NewEntityAsync(ctx));
        async Task<OrderItem> ChildAsync(UserContext ctx, string? name = "Boundary item")
        {
            var row = (await Q.OrderItems().Comment("Prepare a real child constraint")
                .Purpose("Keep business writes in generated mutation APIs").NewEntityAsync(ctx));
            if (name != null) row.UpdateProductName(name);
            row.UpdateUnitPrice(10.00m); return row;
        }
        async Task<CheckResult> Reject(string name, Func<Task> save, string rule, string field)
        {
            var before = await Physical(connection); Reset();
            try { await save(); throw new InvalidOperationException("Invalid save succeeded: " + name); }
            catch (CheckException error)
            {
                Require(Counts().ZeroProvider, "Rejection reached provider/transaction/policy/audit: " + name);
                Require(await Physical(connection) == before, "Rejection changed the stored graph: " + name);
                return error.Violations.Single(v => v.RuleId == rule && v.ModelPath == field);
            }
        }
        // A separate newly opened memory connection has no tables and never
        // calls EnsureSchema. Installation alone must provide generated checks.
        await using (var empty = new SqliteConnection("Data Source=:memory:"))
        {
            await empty.OpenAsync(); var observed = new Transport(new SqliteTransport(empty));
            // Creation reserves an ID before returning; this schema-free probe
            // deliberately selects a memory allocator, not an uninitialized SQL ID space.
            var isolated = Context(observed).WithInternalIdAllocator(new SchemaFreeIdentityAllocator());
            var row = (await ParentAsync(isolated)); Reset(); observed.Reset();
            try { await row.AuditAs("Reject invalid input without schema initialization").SaveAsync(isolated);
                throw new InvalidOperationException("Schema-free Checker did not reject"); }
            catch (CheckException error)
            {
                Require(error.Violations.Any(v => v.RuleId == "required" && v.ModelPath == "contact_phone"), "Wrong schema-free rule");
                Require(observed.Statements == 0 && observed.Begins == 0 && state.CheckerCalls > 0
                    && state.Policies == 0 && audit.Calls == 0, "Schema-free rejection used SQL/policy/audit or no Checker");
            }
            using var tables = empty.CreateCommand(); tables.CommandText = "SELECT count(*) FROM sqlite_master WHERE type='table'";
            Require(Convert.ToInt32(await tables.ExecuteScalarAsync(), CultureInfo.InvariantCulture) == 0, "Schema-free check created tables");
            Emit("module-checker-without-schema", new { schemaCalled = false, tablesCreated = false, checkerCalls = state.CheckerCalls, zeroProviderIO = true });
        }

        var parent = (await ParentAsync(context)); parent.UpdateContactPhone("5550100"); var child = (await ChildAsync(context)); parent.OrderItemList.Add(child);
        Reset(); await parent.AuditAs("Calibrate exactly-once graph persistence").SaveAsync(context); var calibration = Counts();
        Require(calibration.parentInserts == 1 && calibration.childInserts == 1 && calibration.transactions == 1
            && calibration.audits == 2 && calibration.checkers > 0 && calibration.policies > 0, "Positive observers were not calibrated");
        var parentId = parent.Id ?? throw new InvalidOperationException("No parent ID");
        var childId = child.Id ?? throw new InvalidOperationException("No child ID");
        Emit("exact-provider-calibration", new { parentId, childId, effects = calibration });

        var missingRelation = (await ChildAsync(context));
        var relationError = await Reject("required-relation", async () => {
            await missingRelation.AuditAs("Reject a missing required parent reference").SaveAsync(context);
        }, "required", "purchase_order");
        Require(relationError.EntityType == "OrderItem", "Required relation lost its declaring type");
        Emit("required-relation", new { rule = relationError.RuleId, modelPath = relationError.ModelPath, zeroProviderIO = true });

        var root = (await Q.TenantsWithMinimalFields().WithIdIs(1)
            .SelectPurchaseOrderListWith(Q.PurchaseOrdersWithMinimalFields().WithIdIs(parentId)
                .SelectTenant().SelectContactPhone().SelectCreateTime().SelectUpdateTime().Limit(1)
                .Comment("Load every fixed scalar except the optional note"))
            .Limit(1).Comment("Load the actual partial mutation target beneath an untouched root")
            .Purpose("Distinguish completeness from required-root validation").ExecuteForListAsync(context)).Single();
        var partial = root.PurchaseOrderList.Single();
        Require(E.PurchaseOrder(partial).TenantId().Eval() == 1, "Completeness fixture has an absent root reference");
        try { _ = E.PurchaseOrder(partial).InternalNote().Eval(); throw new InvalidOperationException("Optional note was loaded"); }
        catch (TeaQLNotLoadedException) { }
        partial.UpdateContactPhone("5550101");
        var partialError = await Reject("nonroot-partial-update", async () => {
            await root.AuditAs("Reject an incomplete modified object below an untouched root").SaveAsync(context);
        }, "invalid_type", "purchase_order_list[0].internal_note");
        Require(partialError.EntityType == "PurchaseOrder", "Completeness error belongs to another object");
        Emit("nonroot-partial-update", new { loadedRoot = 1, absentField = "internal_note", nativePath = partialError.NativePath, zeroProviderIO = true });

        var completeRoot = (await Q.TenantsWithMinimalFields().WithIdIs(1)
            .SelectPurchaseOrderListWith(Q.PurchaseOrders().WithIdIs(parentId).Limit(1).Comment("Load every field of the modified child"))
            .Limit(1).Comment("Keep the unmodified root partial")
            .Purpose("Require completeness only for actual mutations").ExecuteForListAsync(context)).Single();
        completeRoot.PurchaseOrderList.Single().UpdateContactPhone("5550102"); Reset();
        await completeRoot.AuditAs("Save a complete mutation through an unmodified partial root").SaveAsync(context);
        Require(transport.Begins == 1 && transport.ParentInserts == 0 && transport.ChildInserts == 0 && audit.Calls == 1,
            "Valid partial-ancestor save performed unrelated inserts/audits");
        Require(E.PurchaseOrder(completeRoot.PurchaseOrderList.Single()).Version().Eval() == 2
            && E.Tenant(completeRoot).Version().Eval() == 1, "Partial-root save modified an untouched ancestor");
        Emit("unmodified-partial-ancestor", new { parentId, childVersion = E.PurchaseOrder(completeRoot.PurchaseOrderList.Single()).Version().Eval(),
            rootVersion = E.Tenant(completeRoot).Version().Eval(), effects = Counts() });

        var invalidParent = (await ParentAsync(context)); invalidParent.UpdateContactPhone("5550100"); var invalidChild = (await ChildAsync(context));
        invalidParent.OrderItemList.Add(invalidChild); state.FixMode = "reject";
        var fixError = await Reject("post-fix-rejection", async () => {
            await invalidParent.AuditAs("Reject a range violation introduced by customer Fix").SaveAsync(context);
        }, "min", "order_item_list[0].unit_price");
        Require(fixError.EntityType == "OrderItem" && Convert.ToDecimal(fixError.SystemValue, CultureInfo.InvariantCulture) == 0.01m,
            "Post-Fix range check lost actual model metadata");
        Require(E.OrderItem(invalidChild).UnitPrice().Eval() == 10.00m, "Rejected Fix destroyed pending input");
        Emit("post-fix-rejection", new { originalPrice = "10.00", fixedPrice = "0", originalInputRetained = true, zeroProviderIO = true });

        var fixedParent = (await ParentAsync(context)); fixedParent.UpdateContactPhone("5550100"); var fixedChild = (await ChildAsync(context, null));
        fixedParent.OrderItemList.Add(fixedChild); state.FixMode = "repair"; Reset();
        await fixedParent.AuditAs("Allow customer Fix to supply required and exact Decimal values").SaveAsync(context);
        var fixedEffects = Counts(); state.FixMode = null;
        Require(fixedEffects.parentInserts == 1 && fixedEffects.childInserts == 1 && fixedEffects.transactions == 1, "Fixed graph persisted more than once");
        var fixedRow = (await Q.OrderItems().WithIdIs(fixedChild.Id!.Value).Limit(1)
            .Comment("Read the actual persisted Fix output").Purpose("Reject stale pre-Fix Create rebuilding")
            .ExecuteForListAsync(context)).Single();
        Require(E.OrderItem(fixedRow).ProductName().Eval() == "Fixed child" && E.OrderItem(fixedRow).UnitPrice().Eval() == 12.34m, "Create ignored Fix output");
        Emit("post-fix-repair", new { parentId = fixedParent.Id, childId = fixedChild.Id, storedName = "Fixed child", storedPrice = "12.34", effects = fixedEffects });

        foreach (var profile in Enum.GetValues<JsonFieldNamingProfile>())
        {
            var metadata = WireFields.CreateMetadata("PurchaseOrder", ["contact_phone"], profile,
                new Dictionary<string, IReadOnlyList<string>> { ["contact_phone"] = ["legacy/phone~"] });
            var standard = metadata.Fields["contact_phone"].WireName;
            async Task<(PurchaseOrder row, NormalizedWireInput normalized)> Submit(Dictionary<string, object?> leaf)
            {
                // Parse a real JSON document before normalization; unknown and
                // colliding spellings cannot reach mapping, Checker or Save.
                var text = JsonSerializer.Serialize(new Dictionary<string, object?> { ["payload/order"] = leaf });
                using var document = JsonDocument.Parse(text);
                var input = document.RootElement.GetProperty("payload/order").EnumerateObject()
                    .ToDictionary(p => p.Name, p => (object?)p.Value.Clone());
                var normalized = WireFields.Normalize(input, metadata, "/payload~1order"); state.Mapped++;
                var value = (JsonElement)normalized.Values["contact_phone"]!;
                var row = (await ParentAsync(context)); row.UpdateContactPhone(value.ValueKind == JsonValueKind.Null ? null! : value.GetString()!); state.Saves++;
                // The factory has already reserved identity. From here measure
                // Checker/Save effects, not that earlier allocation transaction.
                transport.Reset();
                try { await row.AuditAs("Save a normalized object-local JSON request").SaveAsync(context); }
                catch (CheckException error) { throw new SubmittedViolation(error, normalized); }
                return (row, normalized);
            }
            var beforeAlias = await Physical(connection); Reset();
            try { await Submit(new Dictionary<string, object?> { ["legacy/phone~"] = null }); throw new InvalidOperationException("Invalid alias input succeeded"); }
            catch (SubmittedViolation failure)
            {
                Require(Counts().ZeroProvider && state.Mapped == 1 && state.Saves == 1 && state.CheckerCalls > 0,
                    "Accepted alias never reached the real generated Checker");
                Require(await Physical(connection) == beforeAlias, "Invalid alias input persisted");
                var violation = WireFields.RetainSubmittedPaths(failure.Original.Violations, failure.Normalized)
                    .Single(v => v.RuleId == "required" && v.ModelPath == "contact_phone");
                Require(violation.EntityType == "PurchaseOrder" && violation.SourceInstancePath == "/payload~1order/legacy~1phone~0", "Alias lost its exact submitted path");
                var wires = Enum.GetValues<JsonFieldNamingProfile>().ToDictionary(Profile, p => violation.ToWire(p));
                Emit("accepted-input-alias", new { wires, zeroProviderIO = true,
                    measurementScope = "save-after-identity-reservation" }, Profile(profile));
            }
            foreach (var name in new[] { "unknown", "equal-forward", "equal-reverse", "unequal-forward", "unequal-reverse" })
            {
                var input = new Dictionary<string, object?>(); var reverse = name.EndsWith("reverse", StringComparison.Ordinal);
                if (name == "unknown") input["unknown/field~"] = new Dictionary<string, object?> { ["not_a_phone"] = 42 };
                else
                {
                    input[reverse ? "legacy/phone~" : standard] = "5550100";
                    input[reverse ? standard : "legacy/phone~"] = name.StartsWith("unequal", StringComparison.Ordinal) ? "5550101" : "5550100";
                }
                var before = await Physical(connection); Reset();
                try { await Submit(input); throw new InvalidOperationException("Invalid wire input succeeded"); }
                catch (WireInputException error)
                {
                    Require(error.Code == (name == "unknown" ? "WIRE_UNKNOWN_FIELD" : "WIRE_FIELD_COLLISION"), "Wrong wire admission code");
                    Require(Counts().ZeroProvider && state.Mapped == 0 && state.Saves == 0 && state.CheckerCalls == 0,
                        "Unknown or colliding input reached domain work");
                    Require(await Physical(connection) == before, "Rejected normalization changed data");
                    Emit("input-" + name, new { code = error.Code, pointer = error.InstancePath,
                        mapperCalls = state.Mapped, saveCalls = state.Saves, checkerCalls = state.CheckerCalls, zeroProviderIO = true }, Profile(profile));
                }
            }
            Reset(); var positive = await Submit(new Dictionary<string, object?> { [standard] = "55509" + round + (int)profile });
            Require(transport.ParentInserts == 1 && transport.ChildInserts == 0 && transport.Begins == 1, "Positive wire control did not insert exactly once");
            Emit("wire-positive-control", new { id = positive.row.Id, parentInserts = 1 }, Profile(profile));
        }
        Require(observations == 28, "Boundary inventory changed");
        using var physicalReport = JsonDocument.Parse(await Physical(connection));
        Console.WriteLine("DOTNET_CHECKER_BOUNDARY_RESULT=" + JsonSerializer.Serialize(new { language = "dotnet", passed = true,
            round, observations, sameDatabase = true, requiredAndRangeObserveActualFix = true,
            realJsonInputAdapter = "application-owned-object-local-composition", recursiveGeneratedHttpAdapterAccepted = false,
            fullCheckerContractAccepted = false, allP0P1Complete = false, physical = physicalReport.RootElement }, Json));
    }

    sealed class SubmittedViolation(CheckException original, NormalizedWireInput normalized) : Exception
    { public CheckException Original { get; } = original; public NormalizedWireInput Normalized { get; } = normalized; }
    sealed class State
    {
        public string? FixMode; public int CheckerCalls, Policies, Mapped, Saves;
        public void ResetCounts() { CheckerCalls = 0; Policies = 0; Mapped = 0; Saves = 0; }
    }
    sealed class ObservedChecker(string type, IEntityChecker original, State state) : IEntityChecker
    {
        public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
        {
            state.CheckerCalls++;
            if (type == "OrderItem" && request is InsertMutationRequest insert)
            {
                if (state.FixMode == "reject") insert.Command.Values["unit_price"] = new Value.DecimalValue(0m);
                if (state.FixMode == "repair")
                {
                    insert.Command.Values["unit_price"] = new Value.DecimalValue(12.34m);
                    insert.Command.Values["product_name"] = new Value.TextValue("Fixed child");
                }
            }
            return original.CheckAndFix(context, request, now);
        }
    }
    static async Task<string> Physical(SqliteConnection connection)
    {
        var tables = new Dictionary<string, List<Dictionary<string, object?>>>();
        foreach (var table in new[] { "tenant_data", "purchase_order_data", "order_item_data" })
        {
            using var command = connection.CreateCommand(); command.CommandText = "SELECT * FROM " + table + " ORDER BY id";
            using var reader = await command.ExecuteReaderAsync(); var rows = new List<Dictionary<string, object?>>();
            while (await reader.ReadAsync()) rows.Add(Enumerable.Range(0, reader.FieldCount)
                .ToDictionary(reader.GetName, i => reader.IsDBNull(i) ? null : reader.GetValue(i)));
            tables[table] = rows;
        }
        return JsonSerializer.Serialize(tables, Json);
    }
    sealed record Effects(int statements, int transactions, int parentInserts, int childInserts,
        int checkers, int mapped, int saves, int policies, int audits)
    { public bool ZeroProvider => statements == 0 && transactions == 0 && parentInserts == 0 && childInserts == 0 && policies == 0 && audits == 0; }
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
        public int Statements, Begins, ParentInserts, ChildInserts;
        public void Reset() { Statements = 0; Begins = 0; ParentInserts = 0; ChildInserts = 0; }
        void Observe(CompiledQuery query)
        {
            Statements++; var sql = query.Sql.Replace("\"", "");
            if (Regex.IsMatch(sql, @"^\s*INSERT\s+INTO\s+purchase_order_data\b", RegexOptions.IgnoreCase)) ParentInserts++;
            if (Regex.IsMatch(sql, @"^\s*INSERT\s+INTO\s+order_item_data\b", RegexOptions.IgnoreCase)) ChildInserts++;
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
