using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

// Application-owned example. It never changes generated Q/E/entity sources.
static class ExpressionBoundaryProbe
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static async Task Verify(string filename)
    {
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
        await connection.OpenAsync();
        var transport = new Transport(new SqliteTransport(connection));
        var module = GeneratedRuntimeModule.Module; var audit = new Audit(); var policies = 0;
        var context = module.IntoContext()
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module)))
            .WithBusinessClock(new FixedBusinessClock(new DateTimeOffset(2032, 2, 29, 10, 15, 30, TimeSpan.Zero)))
            .WithAppAuditEventSink(audit)
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ => { policies++; return null; }));
        await context.EnsureSchemaAsync();
        var parent = Q.PurchaseOrders().Comment("Create the generated E boundary parent")
            .Purpose("Verify expression diagnostics on persisted rows").NewEntity(context);
        parent.UpdateContactPhone("PRIVATE_DOTNET_ROOT_123");
        for (var index = 0; index < 3; index++)
        {
            var child = Q.OrderItems().Comment("Create an indexed E boundary child")
                .Purpose("Exercise actual loaded list positions").NewEntity(context);
            child.UpdateProductName("Expression item " + index); child.UpdateUnitPrice(10.00m);
            parent.OrderItemList.Add(child);
        }
        await parent.AuditAs("Persist the generated E boundary graph").SaveAsync(context);
        var empty = Q.PurchaseOrders().Comment("Create the loaded-empty E boundary parent")
            .Purpose("Distinguish loaded empty from NotLoaded").NewEntity(context);
        empty.UpdateContactPhone("PRIVATE_DOTNET_ROOT_EMPTY");
        await empty.AuditAs("Persist a parent without children").SaveAsync(context);
        var parentId = parent.Id ?? throw new InvalidOperationException("Missing persisted parent ID");
        var emptyId = empty.Id ?? throw new InvalidOperationException("Missing persisted empty parent ID");
        var calibration = new { statements = transport.Statements, transactions = transport.Begins, policies, audits = audit.Calls };
        Require(calibration.statements > 0 && calibration.transactions > 0 && calibration.policies > 0 && calibration.audits > 0,
            "All SQL, transaction, policy and audit observers must be calibrated by real work");
        var lateRows = await Q.PurchaseOrdersWithMinimalFields().WithIdIs(parentId)
            .SelectTenantWith(Q.TenantsWithMinimalFields().Limit(1).Comment("Load target identity without its name"))
            .Limit(1).Comment("Load a real relation with a missing late scalar")
            .Purpose("Distinguish a missing leaf from its loaded relation").ExecuteForListAsync(context);
        var indexedRows = await Q.PurchaseOrders().WithIdIs(parentId)
            .SelectOrderItemListWith(Q.OrderItemsWithMinimalFields().SelectUnitPrice().OrderByIdAscending().Limit(3)
                .Comment("Load child identities and prices, without product names"))
            .Limit(1).Comment("Load the sparse indexed child graph")
            .Purpose("Verify numeric E locations against real rows").ExecuteForListAsync(context);
        var emptyRows = await Q.PurchaseOrders().WithIdIs(emptyId)
            .SelectOrderItemListWith(Q.OrderItems().Limit(3).Comment("Load the genuinely empty list"))
            .Limit(1).Comment("Load a parent with no child records")
            .Purpose("Preserve native loaded-empty behavior").ExecuteForListAsync(context);
        var fkRows = await Q.PurchaseOrders().WithIdIs(parentId).Limit(1)
            .Comment("Load the FK without target details").Purpose("Never fabricate a target from its ID").ExecuteForListAsync(context);
        Require(lateRows.Count == 1 && indexedRows.Count == 1 && emptyRows.Count == 1 && fkRows.Count == 1, "Missing generated query results");
        var late = lateRows[0]; var indexed = indexedRows[0]; var loadedEmpty = emptyRows[0]; var fkOnly = fkRows[0];
        Require(E.PurchaseOrder(late).Tenant().Id().Eval() == 1, "The late-leaf relation identity must actually be loaded");
        Require(E.PurchaseOrder(indexed).OrderItemList().Size().Eval() == 3, "The real list must contain three children");
        var childIds = Enumerable.Range(0, 3).Select(i => E.PurchaseOrder(indexed).OrderItemList().Get(i).Id().Eval()).ToArray();
        Require(childIds.All(id => id.HasValue && id > 0) && childIds.Distinct().Count() == 3, "Child identities must be real and distinct");
        var rows = new[] { late, indexed, loadedEmpty, fkOnly };
        // Reflection is only an observation of private state, never an API-discovery
        // or mutation path. Reading the backing field does not create a ledger.
        var ledgerField = typeof(PurchaseOrder).GetField("_entityRoot", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("The private ledger observation seam changed");
        var ledgers = rows.Select(row => (EntityRoot)ledgerField.GetValue(row)!).ToArray();
        string Snapshot() => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new {
            values = rows.Select(row => row.ToJson()),
            loaded = rows.Select(row => new[] { "Id", "Version", "Tenant", "ContactPhone", "InternalNote", "OrderItemList" }.Select(row.IsLoaded)),
            dirty = ledgers.Select(ledger => ledger.Changes().Count)
        })))).ToLowerInvariant();
        var before = Snapshot(); transport.Reset(); policies = 0; audit.Calls = 0;
        var cases = new List<string>();
        void Case(string name, object value)
        { cases.Add(name); Console.WriteLine("CORE_DOTNET_E_CASE=" + JsonSerializer.Serialize(new { name, value })); }
        object Missing(Func<object?> read, string pointer, string attempted, string chain)
        {
            try { read(); }
            catch (TeaQLNotLoadedException error)
            {
                var wire = error.ToWire();
                Require(wire.InstancePath == pointer && wire.AttemptedInstancePath == attempted, "Wrong canonical location");
                Require(wire.ExpressionWithBrokenPoint == chain && wire.OriginalExpression == chain.Replace("<broken>", ""), "Lost native call chain");
                Require(chain.Split("<broken>").Length == 2 && error.Message.Contains(chain, StringComparison.Ordinal), "Wrong human breakpoint");
                Require(!error.Message.Contains("PRIVATE_DOTNET_") && !JsonSerializer.Serialize(wire).Contains("PRIVATE_DOTNET_"), "Diagnostics disclosed input");
                return new { wire, human = error.Message };
            }
            throw new InvalidOperationException("NotLoaded became loaded null, absence or fallback");
        }
        Case("late-leaf", Missing(() => E.PurchaseOrder(late).Tenant().Name().Eval(), "/tenant/name", "/tenant/name",
            "E.PurchaseOrder(<root>).Tenant().Name()<broken>.Eval()"));
        Case("indexed-leaf", Missing(() => E.PurchaseOrder(indexed).OrderItemList().Get(2).ProductName().Eval(),
            "/orderItemList/2/productName", "/orderItemList/2/productName",
            "E.PurchaseOrder(<root>).OrderItemList().Get(2).ProductName()<broken>.Eval()"));
        var nullValue = E.PurchaseOrder(fkOnly).InternalNote();
        Require(nullValue.Eval() is null && nullValue.HasValue && nullValue.OrIfNull("PRIVATE_DOTNET_FALLBACK") == "PRIVATE_DOTNET_FALLBACK", "Loaded null changed");
        Case("loaded-null", new { value = nullValue.Eval(), present = nullValue.HasValue, fallbackUsed = true });
        var emptyValue = E.PurchaseOrder(loadedEmpty).OrderItemList().First().ProductName();
        Require(E.PurchaseOrder(loadedEmpty).OrderItemList().Size().Eval() == 0 && emptyValue.Eval() is null && !emptyValue.HasValue
            && emptyValue.OrIfNull("PRIVATE_DOTNET_FALLBACK") == "PRIVATE_DOTNET_FALLBACK", "Loaded-empty absence changed");
        Case("loaded-empty", new { size = 0, value = emptyValue.Eval(), present = emptyValue.HasValue, fallbackUsed = true });
        var beyond = E.PurchaseOrder(indexed).OrderItemList().Get(3).ProductName();
        Require(beyond.Eval() is null && !beyond.HasValue && beyond.OrIfNull("PRIVATE_DOTNET_FALLBACK") == "PRIVATE_DOTNET_FALLBACK"
            && E.PurchaseOrder(indexed).OrderItemList().Get(-1).ProductName().Eval() is null, "Out-of-range absence changed");
        Case("out-of-range", new { index = 3, negativeIndex = -1, value = beyond.Eval(), present = beyond.HasValue, fallbackUsed = true });
        var leaf = E.PurchaseOrder(fkOnly).Tenant().Name();
        var terminals = new[] {
            Missing(() => leaf.Eval(), "/tenant", "/tenant/name", "E.PurchaseOrder(<root>).Tenant()<broken>.Name().Eval()"),
            Missing(() => leaf.HasValue, "/tenant", "/tenant/name", "E.PurchaseOrder(<root>).Tenant()<broken>.Name().HasValue"),
            Missing(() => leaf.OrIfNull("PRIVATE_DOTNET_FALLBACK"), "/tenant", "/tenant/name", "E.PurchaseOrder(<root>).Tenant()<broken>.Name().OrIfNull(<fallback>)")
        };
        Case("terminal-not-loaded", new { terminals });
        var branch = E.PurchaseOrder(fkOnly).Tenant(); var left = branch.Name(); var right = branch.PurchaseOrderList().First().ContactPhone();
        const string leftChain = "E.PurchaseOrder(<root>).Tenant()<broken>.Name().Eval()";
        var leftBefore = Missing(() => left.Eval(), "/tenant", "/tenant/name", leftChain);
        var rightFailure = Missing(() => right.Eval(), "/tenant", "/tenant/purchaseOrderList/0/contactPhone",
            "E.PurchaseOrder(<root>).Tenant()<broken>.PurchaseOrderList().First().ContactPhone().Eval()");
        var leftAfter = Missing(() => left.Eval(), "/tenant", "/tenant/name", leftChain);
        Require(JsonSerializer.Serialize(leftBefore) == JsonSerializer.Serialize(leftAfter), "Sibling expressions changed each other");
        Case("sibling-paths", new { leftBefore, right = rightFailure, leftAfter });
        Require(E.PurchaseOrder(fkOnly).TenantId().Eval() == 1, "FK was lost");
        Case("fk-only-not-loaded", new { fk = 1, failure = Missing(() => E.PurchaseOrder(fkOnly).Tenant().Eval(), "/tenant", "/tenant",
            "E.PurchaseOrder(<root>).Tenant()<broken>.Eval()") });
        var after = Snapshot(); Require(after == before, "E changed projection or pending mutation state");
        Require(rows.Select((row, index) => ReferenceEquals(ledgerField.GetValue(row), ledgers[index])).All(value => value), "E replaced a mutation ledger");
        var effects = new { statements = transport.Statements, transactions = transport.Begins, policies, audits = audit.Calls };
        Require(effects.statements == 0 && effects.transactions == 0 && effects.policies == 0 && effects.audits == 0, "E performed implicit work");
        Case("no-side-effects", new { before, after, effects, calibration, ledgerIdentitiesUnchanged = true });
        var profiles = Enum.GetValues<JsonFieldNamingProfile>().Select(profile => {
            var path = ExpressionPath.Root("E.Native(<root>)", profile).Member("a~/b.c", "SafeMethod").MarkMissing();
            var wire = new TeaQLNotLoadedException(path, "a~/b.c").Terminal("Eval()").ToWire();
            Require(wire.InstancePath == (profile == JsonFieldNamingProfile.PascalCase ? "/A~0~1b.c" : "/a~0~1b.c"), "Native pointer escaping or naming changed");
            return new { profile = profile.ToString(), wire };
        }).ToArray();
        Case("native-punctuation", new { nativeOnly = true, profiles });
        Console.WriteLine("CORE_DOTNET_E_BOUNDARY_RESULT=" + JsonSerializer.Serialize(new {
            language = "dotnet", passed = true, parentId, emptyParentId = emptyId, childIds, cases,
            generatedCases = 9, nativeLocationCases = 1, terminalObservations = 3,
            fullPunctuationGeneratedPathAccepted = false, physicalVersionsUnchanged = true, effects
        }));
    }

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
        public int Statements, Begins;
        public void Reset() { Statements = 0; Begins = 0; }
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { Statements++; return inner.FetchAllSqlAsync(query); }
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { Statements++; return inner.ExecuteSqlAsync(query); }
        public async Task<ISqlTransaction> BeginSqlAsync() { Begins++; return new Transaction(await inner.BeginSqlAsync(), this); }
        sealed class Transaction(ISqlTransaction transaction, Transport owner) : ISqlTransaction
        {
            public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) { owner.Statements++; return transaction.FetchAllSqlAsync(query); }
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) { owner.Statements++; return transaction.ExecuteSqlAsync(query); }
            public Task CommitSqlAsync() => transaction.CommitSqlAsync();
            public Task RollbackSqlAsync() => transaction.RollbackSqlAsync();
            public void Dispose() => transaction.Dispose();
        }
    }
}
