using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class TraceChainSqliteTests
{
    // Native-test observation only; do not add public runtime introspection.
    // Retain scalar values and service identities, not copies of SQL sink data.
    private static Dictionary<string, object?> ContextFields(UserContext context) =>
        typeof(UserContext).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .ToDictionary(field => field.Name, field => field.GetValue(context));

    private static Dictionary<TKey, object> ContextResources<TKey>(UserContext context, string field)
        where TKey : notnull =>
        ((ConcurrentDictionary<TKey, object>)typeof(UserContext)
            .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(context)!)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

    private sealed class Sink : IDiagnosticSqlLogSink
    {
        public readonly ConcurrentQueue<ExecutionMetadata> Entries = new();
        public void Write(ExecutionMetadata metadata) => Entries.Enqueue(metadata);
    }

    private static EntityDescriptor Entity(string name) => EntityDescriptor.New(name)
        .TableName(name.ToLowerInvariant() + "_data")
        .Property(PropertyDescriptor.New("id", DataType.I64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version());

    private static async Task<(SqliteConnection Connection, SqlDataServiceExecutor Provider,
        UserContext Context, Sink Log)> Fixture(Func<ISqlTransport, ISqlTransport>? wrap = null)
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var region = Entity("Region");
        var organization = Entity("Organization")
            .Property(PropertyDescriptor.New("regionId", DataType.I64))
            .Relation(RelationDescriptor.New("region", "Region").LocalKey("regionId").ForeignKey("id"));
        var platform = Entity("Platform")
            .Property(PropertyDescriptor.New("organizationId", DataType.I64))
            .Relation(RelationDescriptor.New("organization", "Organization").LocalKey("organizationId").ForeignKey("id"));
        var school = Entity("School")
            .Property(PropertyDescriptor.New("platformId", DataType.I64))
            .Relation(RelationDescriptor.New("platform", "Platform").LocalKey("platformId").ForeignKey("id"));
        var module = new RuntimeModule().Entity(region).Entity(organization).Entity(platform).Entity(school);
        var descriptors = new[] { region, organization, platform, school };
        var schemas = new MetadataSchemaProvider(name => descriptors.FirstOrDefault(entity => entity.Name == name));
        ISqlTransport transport = new SqliteTransport(connection);
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), wrap?.Invoke(transport) ?? transport, schemas);
        var sink = new Sink();
        var context = module.IntoContext().WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        foreach (var (entity, foreign) in new[] { ("Region", ""), ("Organization", "regionId"),
            ("Platform", "organizationId"), ("School", "platformId") })
        {
            var command = new InsertCommand(entity).Value("id", Value.FromObject(1L))
                .Value("version", Value.FromObject(1L));
            if (foreign.Length > 0) command.Value(foreign, Value.FromObject(1L));
            await service.MutateAsync(new InsertMutationRequest(command, "prepare trace fixture"));
        }
        sink.Entries.Clear();
        return (connection, provider, context, sink);
    }

    private sealed class PausedRoots(ISqlTransport inner) : ISqlTransport
    {
        // The connection is not thread-safe. Serialize only the actual database
        // calls, never the request lifetime or the wait at the root barrier.
        private readonly SemaphoreSlim connectionGate = new(1);
        private int roots;
        public bool Enabled;
        public int RootCount => Volatile.Read(ref roots);
        public readonly TaskCompletionSource<bool> BothEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public readonly TaskCompletionSource<bool> Release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public async Task<List<TeaQL.Core.Record>> FetchAllSqlAsync(CompiledQuery query)
        {
            List<TeaQL.Core.Record> rows;
            await connectionGate.WaitAsync();
            try { rows = await inner.FetchAllSqlAsync(query); }
            finally { connectionGate.Release(); }
            if (Enabled && Regex.IsMatch(query.Sql.Replace("\"", ""), @"\bFROM\s+school_data\b", RegexOptions.IgnoreCase))
            {
                if (Interlocked.Increment(ref roots) == 2) BothEntered.TrySetResult(true);
                await Release.Task.WaitAsync(TimeSpan.FromSeconds(10));
            }
            return rows;
        }
    }

    private sealed class ObservedService(IDataService inner) : IDataService
    {
        public readonly ConcurrentQueue<QueryRequest> Requests = new();
        public readonly ConcurrentQueue<ExecutionMetadata> Statements = new();
        public DataServiceCapabilities Capabilities => inner.Capabilities;
        public Task<MutationResult> MutateAsync(MutationRequest request) => inner.MutateAsync(request);
        public Task<QueryResult> QueryAsync(QueryRequest request)
        {
            Requests.Enqueue(request);
            var previous = request.DiagnosticObserver;
            request.DiagnosticObserver = metadata => {
                Statements.Enqueue(metadata);
                previous?.Invoke(metadata);
            };
            return inner.QueryAsync(request);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TwoLiveGraphsKeepIntentAndPhysicalPathsIsolated(bool logging)
    {
        PausedRoots? barrier = null;
        var fixture = await Fixture(inner => barrier = new PausedRoots(inner));
        await using var connection = fixture.Connection;
        var observer = new ObservedService(fixture.Provider);
        fixture.Context.WithDataService(observer).EnableQuerySqlLog(logging);
        var service = fixture.Context.RequireResource<IDataService>();
        barrier!.Enabled = true;
        var contextFieldsBefore = ContextFields(fixture.Context);
        var typedResourcesBefore = ContextResources<Type>(fixture.Context, "_typedResources");
        var namedResourcesBefore = ContextResources<string>(fixture.Context, "_namedResources");
        void AssertContextUnchanged()
        {
            var fields = ContextFields(fixture.Context);
            Assert.Equal(contextFieldsBefore.Keys.Order(), fields.Keys.Order());
            foreach (var (key, original) in contextFieldsBefore)
                if (original is null || original.GetType().IsValueType) Assert.Equal(original, fields[key]);
                else Assert.Same(original, fields[key]);
            void AssertResources<TKey>(Dictionary<TKey, object> before, Dictionary<TKey, object> after)
                where TKey : notnull
            {
                Assert.True(before.Count == after.Count && before.Keys.All(after.ContainsKey),
                    "shared Context resource keys changed during independent queries");
                foreach (var key in before.Keys) Assert.Same(before[key], after[key]);
            }
            AssertResources(typedResourcesBefore, ContextResources<Type>(fixture.Context, "_typedResources"));
            AssertResources(namedResourcesBefore, ContextResources<string>(fixture.Context, "_namedResources"));
        }
        QueryRequest Request(string label) => new(new SelectQuery("School").Limit(1)
            .RelationQuery("platform", new SelectQuery("Platform").Project("organizationId").Limit(1)
                .RelationQuery("organization", new SelectQuery("Organization").Project("regionId").Limit(1)
                    .RelationQuery("region", new SelectQuery("Region").Limit(1))))
            .Comment($"load {label} graph").Purpose($"render {label} graph"));
        var pending = new[] { service.QueryAsync(Request("alpha")), service.QueryAsync(Request("beta")) };
        var completed = Task.WhenAll(pending);
        try
        {
            var first = await Task.WhenAny(barrier.BothEntered.Task, completed).WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Same(barrier.BothEntered.Task, first);
            Assert.Equal(2, barrier.RootCount);
            Assert.All(pending, task => Assert.False(task.IsCompleted));
            Assert.Empty(observer.Statements);
            Assert.Empty(fixture.Log.Entries);
            Assert.Equal(new[] { "load alpha graph", "load beta graph" }, observer.Requests.Select(r => r.Comment));
            Assert.Equal(new[] { "render alpha graph", "render beta graph" }, observer.Requests.Select(r => r.Purpose));
            Assert.All(observer.Requests, request => Assert.Empty(request.TraceChain));
            AssertContextUnchanged();
            barrier.Release.TrySetResult(true);
            var results = await completed.WaitAsync(TimeSpan.FromSeconds(5));
            foreach (var result in results)
            {
                var platform = Assert.IsType<Value.ObjectValue>(Assert.Single(result.Rows)["platform"]).Value;
                var organization = Assert.IsType<Value.ObjectValue>(platform["organization"]).Value;
                var region = Assert.IsType<Value.ObjectValue>(organization["region"]).Value;
                Assert.Equal(1L, region["id"].TryI64());
            }
            Assert.Equal(8, observer.Statements.Count);
            Assert.Equal(logging ? 8 : 0, fixture.Log.Entries.Count);
            AssertContextUnchanged();
            foreach (var entries in new[] { observer.Statements, fixture.Log.Entries })
            foreach (var label in new[] { "alpha", "beta" })
            {
                var own = entries.Where(e => e.Comment == $"load {label} graph").ToArray();
                Assert.Equal(ReferenceEquals(entries, observer.Statements) || logging ? 4 : 0, own.Length);
                for (var depth = 0; depth < own.Length; depth++)
                {
                    Assert.Equal($"render {label} graph", own[depth].Purpose);
                    Assert.Equal("success", own[depth].ExecutionOutcome);
                    Assert.Equal(new[] { "School", "School" }
                        .Concat(new[] { "platform", "organization", "region" }.Take(depth))
                        .Concat(new[] { "sqlite", "select" }), own[depth].TraceChain.Select(n => n.Name));
                    Assert.Equal(new[] { "operation", "request" }.Concat(Enumerable.Repeat("relation", depth))
                        .Concat(new[] { "provider", "sql" }), own[depth].TraceChain.Select(n => n.Kind));
                    Assert.Equal(new[] { "School.platform", "Platform.organization", "Organization.region" }.Take(depth),
                        own[depth].TraceChain.Where(n => n.Kind == "relation").Select(n => n.Detail));
                }
            }
        }
        finally
        {
            barrier.Release.TrySetResult(true);
            try { await completed.WaitAsync(TimeSpan.FromSeconds(10)); }
            catch { /* Preserve the first assertion/provider failure rather than the cleanup error. */ }
        }
    }

    [Fact]
    public async Task PhysicalRootPathNamesTheEntityWithoutDuplicatingIntent()
    {
        var fixture = await Fixture();
        await using var connection = fixture.Connection;
        var result = await fixture.Provider.QueryAsync(new QueryRequest(new SelectQuery("School").Limit(1)
            .Comment("load school").Purpose("verify physical path")));
        Assert.Single(result.Rows);
        Assert.Equal(new[] { "School", "School", "sqlite", "select" },
            result.Metadata.TraceChain.Select(node => node.Name));
        Assert.All(result.Metadata.TraceChain, node => Assert.Equal("", node.Comment));
        Assert.Equal("load school", result.Metadata.Comment);
        Assert.Equal("verify physical path", result.Metadata.Purpose);
    }

    [Fact]
    public async Task RealThreeLevelFailureKeepsTheOriginatingRootAndLocalRelationNames()
    {
        var fixture = await Fixture();
        await using var connection = fixture.Connection;
        // Fault injection changes schema only; data was created through Mutation API.
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "DROP TABLE region_data";
            await command.ExecuteNonQueryAsync();
        }
        var query = new SelectQuery("School").Limit(1)
            .RelationQuery("platform", new SelectQuery("Platform").Project("organizationId").Limit(1)
                .RelationQuery("organization", new SelectQuery("Organization").Project("regionId").Limit(1)
                    .RelationQuery("region", new SelectQuery("Region").Limit(1))))
            .Comment("load school graph").Purpose("verify inherited failure route");
        await Assert.ThrowsAsync<SqlExecutorException>(() => fixture.Context.RequireResource<IDataService>()
            .QueryAsync(new QueryRequest(query)));
        Assert.Equal(4, fixture.Log.Entries.Count);
        Assert.Equal(3, fixture.Log.Entries.Count(entry => entry.ExecutionOutcome == "success"));
        var failed = Assert.Single(fixture.Log.Entries.Where(entry => entry.ExecutionOutcome == "failure"));
        Assert.Equal("failure", failed.ExecutionOutcome);
        Assert.Equal(new[] { "School", "School", "platform", "organization", "region", "sqlite", "select" },
            failed.TraceChain.Select(node => node.Name));
        Assert.All(failed.TraceChain, node => Assert.Equal("", node.Comment));
        Assert.Equal(new[] { "School.platform", "Platform.organization", "Organization.region" },
            failed.TraceChain.Where(node => node.Kind == "relation").Select(node => node.Detail));
        Assert.Equal("load school graph", failed.Comment);
        Assert.Equal("verify inherited failure route", failed.Purpose);
    }

    [Fact]
    public async Task SuccessfulThreeLevelLoadEmitsEveryPhysicalStatementExactlyOnce()
    {
        var fixture = await Fixture();
        await using var connection = fixture.Connection;
        var query = new SelectQuery("School").Limit(1)
            .RelationQuery("platform", new SelectQuery("Platform").Project("organizationId").Limit(1)
                .RelationQuery("organization", new SelectQuery("Organization").Project("regionId").Limit(1)
                    .RelationQuery("region", new SelectQuery("Region").Limit(1))))
            .Comment("load school graph").Purpose("observe every physical statement");
        await fixture.Context.RequireResource<IDataService>().QueryAsync(new QueryRequest(query));
        Assert.Equal(4, fixture.Log.Entries.Count);
        Assert.Equal(new[] { 0, 1, 2, 3 }, fixture.Log.Entries.Select(entry => entry.TraceChain.Count(node => node.Kind == "relation")));
        Assert.All(fixture.Log.Entries, entry => {
            Assert.Equal("School", entry.TraceChain[0].Name);
            Assert.Equal("success", entry.ExecutionOutcome);
            Assert.Equal("load school graph", entry.Comment);
            Assert.Equal("observe every physical statement", entry.Purpose);
        });
    }

    [Fact]
    public async Task RequestCapturesMutableNestedQueriesBeforeExecution()
    {
        var fixture=await Fixture(); await using var connection=fixture.Connection;
        var region=new SelectQuery("Region").Filter(Expr.Eq("id",1L)).Limit(1);
        var query=new SelectQuery("School").Limit(1).Comment("captured graph").Purpose("snapshot nested request")
            .RelationQuery("platform",new SelectQuery("Platform").Project("organizationId").Limit(1)
                .RelationQuery("organization",new SelectQuery("Organization").Project("regionId").Limit(1).RelationQuery("region",region)));
        var request=new QueryRequest(query);
        region.Filter(Expr.Eq("id",999L));query.Comment("changed caller");
        var result=await fixture.Context.RequireResource<IDataService>().QueryAsync(request);
        var platform=Assert.IsType<Value.ObjectValue>(Assert.Single(result.Rows)["platform"]).Value;
        var organization=Assert.IsType<Value.ObjectValue>(platform["organization"]).Value;
        var actualRegion=Assert.IsType<Value.ObjectValue>(organization["region"]).Value;
        Assert.Equal(1L,actualRegion["id"].TryI64());
        Assert.Equal(4,fixture.Log.Entries.Count);
        Assert.All(fixture.Log.Entries,entry=>Assert.Equal("captured graph",entry.Comment));
    }
}
