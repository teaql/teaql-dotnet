using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Data.Sqlite;
using TeaQL.Provider.Sqlite;
using TeaQL.Core;
using TeaQL.Sql;
using TeaQL.DataService;
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
