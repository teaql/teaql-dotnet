using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using TeaQL.Provider.Sqlite;
using TeaQL.Core;
using TeaQL.Sql;
using TeaQL.DataService;
using TeaQL.Runtime;
using Record = TeaQL.Core.Record;

namespace TeaQL.Provider.Sqlite.Tests
{
    public class SqliteTransportTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly SqliteTransport _transport;

        public SqliteTransportTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _transport = new SqliteTransport(_connection);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Fact]
        public async Task OptimisticIdSpaceIsSharedAcrossExecutors()
        {
            var path = Path.Combine(Path.GetTempPath(), $"teaql-id-space-{Guid.NewGuid():N}.db");
            try
            {
                await using var firstConnection = new SqliteConnection($"Data Source={path};Default Timeout=5");
                await using var secondConnection = new SqliteConnection($"Data Source={path};Default Timeout=5");
                await firstConnection.OpenAsync();
                await secondConnection.OpenAsync();
                var first = new SqlDataServiceExecutor(
                    new SqliteDialect(), new SqliteTransport(firstConnection), new EmptySchemaProvider());
                var second = new SqlDataServiceExecutor(
                    new SqliteDialect(), new SqliteTransport(secondConnection), new EmptySchemaProvider());

                Assert.Equal(1ul, await first.NextIdAsync("Order"));
                Assert.Equal(2ul, await second.NextIdAsync("Order"));
                Assert.Equal(1ul, await first.NextIdAsync("Customer"));
                await first.EnsureIdFloorAsync("SeededType", 1001);
                Assert.Equal(1002ul, await second.NextIdAsync("SeededType"));
                var tasks = Enumerable.Range(0, 20)
                    .Select(index => (index % 2 == 0 ? first : second).NextIdAsync("Order"));
                var ids = (await Task.WhenAll(tasks)).OrderBy(value => value).ToArray();
                Assert.Equal(Enumerable.Range(3, 20).Select(value => (ulong)value), ids);
                Assert.Equal(20, ids.Distinct().Count());
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public async Task SchoolBootstrapIsIdempotentPreservesRootAndReconcilesConstants()
        {
            var platform = EntityDescriptor.New("Platform").TableName("platform_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .Property(PropertyDescriptor.New("version", DataType.I64).Version());
            var schoolType = EntityDescriptor.New("SchoolType").TableName("school_type_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .Property(PropertyDescriptor.New("code", DataType.Text))
                .Property(PropertyDescriptor.New("version", DataType.I64).Version());
            var primaryValues = new Record
                { ["name"] = new Value.TextValue("Primary"), ["code"] = new Value.TextValue("PRIMARY") };
            var module = new RuntimeModule().Entity(platform).Entity(schoolType)
                .RootEntity(new BootstrapEntity("Platform", 1, new Record
                    { ["name"] = new Value.TextValue("Campus Learning Platform") }))
                .ConstantEntity(new BootstrapEntity("SchoolType", 1001, primaryValues))
                .ConstantEntity(new BootstrapEntity("SchoolType", 1002, new Record
                    { ["name"] = new Value.TextValue("Secondary"), ["code"] = new Value.TextValue("SECONDARY") }));
            var executor = new SqlDataServiceExecutor(
                new SqliteDialect(), _transport, new ModuleSchemaProvider(module));
            var context = module.IntoContext();
            context.WithDataService(executor);
            Assert.Null(typeof(SqlDataServiceExecutor).GetMethod("EnsureSchemaAsync"));

            await context.EnsureSchemaAsync();
            await executor.MutateAsync(new UpdateMutationRequest(
                new UpdateCommand("Platform", new Value.I64Value(1))
                    .Value("name", new Value.TextValue("Customer Name"))));
            primaryValues["name"] = new Value.TextValue("Primary School");
            await context.EnsureSchemaAsync();

            var roots = await executor.QueryAsync(new QueryRequest { Query = new SelectQuery("Platform") });
            var constants = await executor.QueryAsync(new QueryRequest { Query = new SelectQuery("SchoolType") });
            Assert.Single(roots.Rows);
            Assert.Equal("Customer Name", roots.Rows[0]["name"].TryText());
            Assert.Equal(2, constants.Rows.Count);
            Assert.Equal("Primary School", constants.Rows.Single(row => row["id"].TryI64() == 1001)["name"].TryText());
            Assert.Equal(2, constants.Rows.Single(row => row["id"].TryI64() == 1001)["version"].TryI64());
            Assert.Equal(1, constants.Rows.Single(row => row["id"].TryI64() == 1002)["version"].TryI64());
            Assert.True(await executor.NextIdAsync("SchoolType") > 1002);
        }

        private sealed class ModuleSchemaProvider : ISchemaProvider
        {
            private readonly RuntimeModule _module;
            public ModuleSchemaProvider(RuntimeModule module) => _module = module;
            public EntityDescriptor? GetEntity(string name) => _module.Metadata.GetEntity(name);
        }

        private sealed class EmptySchemaProvider : ISchemaProvider
        {
            public EntityDescriptor? GetEntity(string name) => null;
        }

        [Fact]
        public async Task ExecuteSqlAsync_ReturnsAffectedRows()
        {
            var create = new CompiledQuery("CREATE TABLE test (id INTEGER PRIMARY KEY, name TEXT)", new List<Value>());
            await _transport.ExecuteSqlAsync(create);

            var insert = new CompiledQuery("INSERT INTO test (id, name) VALUES (@p0, @p1)", new List<Value> { new Value.I64Value(1), new Value.TextValue("A") });
            var rows = await _transport.ExecuteSqlAsync(insert);
            Assert.Equal(1ul, rows);
        }

        [Fact]
        public async Task FetchAllSqlAsync_ReadsVariousTypes()
        {
            var create = new CompiledQuery("CREATE TABLE types (id INTEGER, b BOOLEAN, f REAL, s TEXT, d DATE, t TIMESTAMP, dec NUMERIC, j JSON)", new List<Value>());
            await _transport.ExecuteSqlAsync(create);

            var insert = new CompiledQuery(
                "INSERT INTO types VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7)",
                new List<Value> {
                    new Value.I64Value(1),
                    new Value.BoolValue(true),
                    new Value.F64Value(3.14),
                    new Value.TextValue("hello"),
                    new Value.DateValue(new DateTime(2023, 1, 1)),
                    new Value.TimestampValue(1672531200000),
                    new Value.DecimalValue(10.5m),
                    new Value.JsonValue(System.Text.Json.Nodes.JsonNode.Parse("{\"a\":1}"))
                });
            await _transport.ExecuteSqlAsync(insert);

            var select = new CompiledQuery("SELECT * FROM types", new List<Value>());
            var results = await _transport.FetchAllSqlAsync(select);

            Assert.Single(results);
            var record = results[0];
            Assert.Equal(1, ((Value.I64Value)record["id"]).Value);
            Assert.True(((Value.BoolValue)record["b"]).Value);
            Assert.Equal(3.14, ((Value.F64Value)record["f"]).Value);
            Assert.Equal("hello", ((Value.TextValue)record["s"]).Value);
            Assert.Equal(new DateTime(2023, 1, 1), ((Value.DateValue)record["d"]).Value);
            Assert.Equal(1672531200000, ((Value.TimestampValue)record["t"]).Milliseconds);
            Assert.Equal(10.5m, ((Value.DecimalValue)record["dec"]).Value);
            Assert.NotNull(((Value.JsonValue)record["j"]).Value);
        }

        [Fact]
        public async Task NullValues_AreHandled()
        {
            var create = new CompiledQuery("CREATE TABLE nulls (id INTEGER, v TEXT)", new List<Value>());
            await _transport.ExecuteSqlAsync(create);

            var insert = new CompiledQuery(
                "INSERT INTO nulls VALUES (@p0, @p1)",
                new List<Value> { new Value.I64Value(2), new Value.NullValue() });
            await _transport.ExecuteSqlAsync(insert);

            var select = new CompiledQuery("SELECT * FROM nulls", new List<Value>());
            var results = await _transport.FetchAllSqlAsync(select);
            var record = results[0];
            Assert.IsType<Value.NullValue>(record["v"]);
        }

        [Fact]
        public async Task TemporalDebugSql_IsExecutableAndMatchesPreparedStorage()
        {
            await _transport.ExecuteSqlAsync(new CompiledQuery(
                "CREATE TABLE temporal_fixture (id INTEGER PRIMARY KEY, d DATE, t TIMESTAMP)",
                new List<Value>()));
            var prepared = new CompiledQuery(
                "INSERT INTO temporal_fixture VALUES (@p0, @p1, @p2) /* ignored @p9 */",
                new List<Value> {
                    new Value.I64Value(1),
                    new Value.DateValue(new DateTime(2024, 2, 29)),
                    new Value.TimestampValue(1787110200123)
                });
            await _transport.ExecuteSqlAsync(prepared);
            var literal = new CompiledQuery(
                prepared.DebugSql(DatabaseKind.Sqlite).Replace("VALUES (1,", "VALUES (2,"),
                new List<Value>());
            await _transport.ExecuteSqlAsync(literal);

            var rows = await _transport.FetchAllSqlAsync(new CompiledQuery(
                "SELECT d, t, typeof(t) AS storage_type FROM temporal_fixture ORDER BY id",
                new List<Value>()));
            Assert.Equal(rows[0]["d"], rows[1]["d"]);
            Assert.Equal(rows[0]["t"], rows[1]["t"]);
            Assert.Equal("integer", ((Value.TextValue)rows[0]["storage_type"]).Value);
        }
        
        [Fact]
        public async Task UnsupportedType_Throws()
        {
            var create = new CompiledQuery("CREATE TABLE test (id INTEGER)", new List<Value>());
            await _transport.ExecuteSqlAsync(create);

            // Using typed null but no mapped sqlite bind
            var insert = new CompiledQuery(
                "INSERT INTO test VALUES (@p0)",
                new List<Value> { new Value.ListValue(new List<Value>()) }); // ListValue is unsupported
                
            await Assert.ThrowsAsync<NotSupportedException>(() => _transport.ExecuteSqlAsync(insert));
        }

        [Fact]
        public async Task MutationReturnsAuthoritativeDatabaseDefaultsInItsTransaction()
        {
            await _transport.ExecuteSqlAsync(new CompiledQuery(
                "CREATE TABLE widget (id INTEGER PRIMARY KEY, name TEXT DEFAULT 'database-default', version INTEGER DEFAULT 1)",
                new List<Value>()));
            var descriptor = EntityDescriptor.New("Widget").TableName("widget")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .Property(PropertyDescriptor.New("version", DataType.I64).Version());
            var executor = new SqlDataServiceExecutor(
                new SqliteDialect(), _transport, new SingleSchemaProvider(descriptor));
            var command = new InsertCommand("Widget").Value("id", new Value.I64Value(7));

            var result = await executor.MutateAsync(new InsertMutationRequest(command));

            Assert.NotNull(result.PersistedRecord);
            Assert.Equal(7, ((Value.I64Value)result.PersistedRecord!["id"]).Value);
            Assert.Equal("database-default", ((Value.TextValue)result.PersistedRecord["name"]).Value);
            Assert.Equal(1, ((Value.I64Value)result.PersistedRecord["version"]).Value);
        }

        private sealed class SingleSchemaProvider : ISchemaProvider
        {
            private readonly EntityDescriptor _descriptor;
            public SingleSchemaProvider(EntityDescriptor descriptor) => _descriptor = descriptor;
            public EntityDescriptor? GetEntity(string name) => name == _descriptor.Name ? _descriptor : null;
        }
    }
}
