using Generated;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Text.RegularExpressions;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

// Real generated Save entry points, not a handwritten replacement persistence path.
static class GraphSaveReentrancyProbe
{
    static readonly TimeSpan Bound = TimeSpan.FromSeconds(5);
    public static readonly string[] Scenarios = ["prepare", "work", "after-write", "after-commit"];
    static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }

    public static async Task Verify(string filename, int round)
    {
        await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = filename }.ToString());
        await connection.OpenAsync();
        var transport = new Transport(new SqliteTransport(connection));
        var audit = new Audit();
        var module = GeneratedRuntimeModule.Module;
        var context = module.IntoContext().WithActiveRoot("Tenant", 1)
            .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), transport, new Schema(module)))
            .WithBusinessClock(new FixedBusinessClock(new DateTimeOffset(2037, 6, 1, 0, 0, 0, TimeSpan.Zero)))
            .WithAppAuditEventSink(audit);
        await context.EnsureSchemaAsync();
        foreach (var scenario in Scenarios)
        {
            var order = await Q.PurchaseOrders().Comment("Prepare a root Save reentry fixture")
                .Purpose("Verify bounded rejection and same-object retry").NewEntityAsync(context);
            order.UpdateContactPhone("5550199");
            var originalId = order.Id;
            var originalVersion = order.Version;
            transport.Reset(); audit.Calls = 0;
            async Task<int> NestedSave()
            {
                await order.AuditAs("Reject an independent nested root Save").SaveAsync(context);
                return 2;
            }
            Exception? failure = null;
            try
            {
                Task<int> operation;
                if (scenario == "prepare")
                    operation = context.ExecuteGraphSaveAsync("Outer preparation owner", async _ => { await NestedSave(); }, _ => Task.FromResult(1));
                else if (scenario == "work")
                    operation = context.ExecuteGraphSaveAsync("Outer transaction owner", async _ => { await Task.Yield(); return await NestedSave(); });
                else if (scenario == "after-write")
                {
                    transport.AfterInsert = async () => { transport.AfterInsert = null; await NestedSave(); };
                    operation = SaveOuter();
                    async Task<int> SaveOuter() { await order.AuditAs("Roll back a real write before nested admission").SaveAsync(context); return 1; }
                }
                else
                    operation = context.ExecuteGraphSaveAsync("Outer postcommit owner", graph =>
                    {
                        graph.AfterCommit(() => NestedSave().GetAwaiter().GetResult());
                        return Task.FromResult(1);
                    });
                await operation.WaitAsync(Bound);
            }
            catch (Exception error) { failure = error; }
            var code = failure is GraphCommittedException committed ? committed.InnerExceptions.Single().Message : failure?.Message;
            Require(code == "GRAPH_SAVE_REENTRANT", "Expected prompt nested root rejection, not a timeout or unrelated error");
            Require((failure is GraphCommittedException) == (scenario == "after-commit"), "Wrong committed failure classification");
            var rejected = transport.Counts();
            Require(rejected == (scenario switch
            {
                "prepare" => new Effects(0, 0, 0, 0),
                "work" => new Effects(1, 0, 1, 0),
                "after-write" => new Effects(1, 0, 1, 1),
                _ => new Effects(1, 1, 0, 0)
            }), "Wrong provider effects for rejected reentry");
            Require(audit.Calls == 0 && order.Id == originalId && order.Version == originalVersion,
                "Reentry changed original intent/version or published an uncommitted audit");
            Require((await Q.PurchaseOrders().WithIdIs(originalId!.Value).Limit(1)
                .Comment("Verify rejected Save left no row").Purpose("Check rollback through generated Q").ExecuteForListAsync(context)).Count == 0,
                "Rejected graph persisted a row");
            transport.Reset();
            await order.AuditAs("Retry the same object after bounded reentry rejection").SaveAsync(context).WaitAsync(Bound);
            Require(transport.Counts() == new Effects(1, 1, 0, 1) && audit.Calls == 1 && order.Version == 1,
                "Retry must commit exactly one row and audit");
            var loaded = (await Q.PurchaseOrders().WithIdIs(originalId.Value).Limit(1)
                .Comment("Reload the successful same-object retry").Purpose("Verify persisted values through generated E").ExecuteForListAsync(context)).Single();
            Require(E.PurchaseOrder(loaded).ContactPhone().Eval() == "5550199", "Retry did not preserve the business input");
            Console.WriteLine("DOTNET_GRAPH_REENTRY=" + JsonSerializer.Serialize(new
            {
                round, scenario, passed = true, id = originalId, code, rejected,
                committedFailure = failure is GraphCommittedException, retry = true, version = order.Version,
                auditCount = audit.Calls, runtimeAssembly = typeof(UserContext).Assembly.Location
            }));
        }
    }

    sealed class Schema(RuntimeModule module) : ISchemaProvider { public EntityDescriptor? GetEntity(string name) => module.Metadata.GetEntity(name); }
    sealed class Audit : IAppAuditEventSink
    {
        public int Calls;
        public Task RecordAsync(IReadOnlyDictionary<string, object?> value, CancellationToken token = default) { Calls++; return Task.CompletedTask; }
    }
    sealed record Effects(int Begins, int Commits, int Rollbacks, int Inserts);
    sealed class Transport(SqliteTransport inner) : IAutomaticMutationTransactionTransport
    {
        int begins, commits, rollbacks, inserts;
        public Func<Task>? AfterInsert;
        public void Reset() { begins = commits = rollbacks = inserts = 0; }
        public Effects Counts() => new(begins, commits, rollbacks, inserts);
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) => inner.FetchAllSqlAsync(query);
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public async Task<ISqlTransaction> BeginSqlAsync() { begins++; return new Transaction(await inner.BeginSqlAsync(), this); }
        async Task Observe(CompiledQuery query)
        {
            if (!Regex.IsMatch(query.Sql.Replace("\"", ""), @"^\s*INSERT INTO\s+purchase_order_data\b", RegexOptions.IgnoreCase)) return;
            inserts++;
            if (AfterInsert is { } callback) await callback();
        }
        sealed class Transaction(ISqlTransaction actual, Transport owner) : ISqlTransaction
        {
            public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
            { var rows = await actual.FetchAllSqlAsync(query); await owner.Observe(query); return rows; }
            public async Task<ulong> ExecuteSqlAsync(CompiledQuery query)
            { var rows = await actual.ExecuteSqlAsync(query); await owner.Observe(query); return rows; }
            public Task CommitSqlAsync() { owner.commits++; return actual.CommitSqlAsync(); }
            public Task RollbackSqlAsync() { owner.rollbacks++; return actual.RollbackSqlAsync(); }
            public void Dispose() => actual.Dispose();
        }
    }
}
