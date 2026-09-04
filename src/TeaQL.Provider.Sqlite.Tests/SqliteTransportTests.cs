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
        public async Task GeneratedBootstrapRunsAfterSchemaThroughInstalledContext()
        {
            var platform = EntityDescriptor.New("Platform").TableName("platform_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .Property(PropertyDescriptor.New("version", DataType.I64).Version());
            var calls = new List<string>();
            var module = new RuntimeModule().Entity(platform)
                .GeneratedBootstrap(async context =>
                {
                    calls.Add("first");
                    var service = context.RequireResource<IDataService>();
                    var before = await service.QueryAsync(new QueryRequest
                    {
                        Query = new SelectQuery("Platform"),
                        Comment = "what: inspect generated bootstrap state",
                        Purpose = "why: verify schema exists before generated bootstrap"
                    });
                    if (before.Rows.Count == 0)
                    {
                        var command = new InsertCommand("Platform")
                            .Value("id", new Value.I64Value(1))
                            .Value("name", new Value.TextValue("Campus Learning Platform"))
                            .Value("version", new Value.I64Value(1));
                        command.TraceChain.Add(new TraceNode(
                            "Platform", 1, "initialize generated Platform root"));
                        await service.MutateAsync(new InsertMutationRequest(command));
                    }
                })
                .GeneratedBootstrap(context =>
                {
                    calls.Add("second");
                    return Task.CompletedTask;
                });
            var executor = new SqlDataServiceExecutor(
                new SqliteDialect(), _transport, new ModuleSchemaProvider(module));
            var context = module.IntoContext();
            context.WithDataService(executor);
            Assert.Null(typeof(SqlDataServiceExecutor).GetMethod("EnsureSchemaAsync"));

            await context.EnsureSchemaAsync();
            await context.EnsureSchemaAsync();
            Assert.Equal(new[] { "first", "second", "first", "second" }, calls);
            using (var soundex = _connection.CreateCommand())
            {
                soundex.CommandText = "SELECT soundex('Robert'), soundex('Robert') = soundex('Rupert'), soundex(NULL)";
                using var reader = await soundex.ExecuteReaderAsync();
                Assert.True(await reader.ReadAsync());
                Assert.Equal("R163", reader.GetString(0));
                Assert.Equal(1L, reader.GetInt64(1));
                Assert.Equal("?000", reader.GetString(2));
            }
            var roots = await executor.QueryAsync(new QueryRequest { Query = new SelectQuery("Platform") });
            Assert.Single(roots.Rows);
            Assert.Equal("Campus Learning Platform", roots.Rows[0]["name"].TryText());
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
        public async Task RelationSubqueriesExecutePositiveAndNegativePredicates()
        {
            var group = EntityDescriptor.New("QueryGroup").TableName("query_group_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .Property(PropertyDescriptor.New("version", DataType.I64).Version());
            var record = EntityDescriptor.New("QueryRecord").TableName("query_record_data")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("queryGroup", DataType.I64).ColumnName("query_group"))
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .Property(PropertyDescriptor.New("version", DataType.I64).Version());
            var module = new RuntimeModule().Entity(group).Entity(record);
            var executor = new SqlDataServiceExecutor(
                new SqliteDialect(), _transport, new ModuleSchemaProvider(module));
            var context = module.IntoContext().WithDataService(executor);
            await context.EnsureSchemaAsync();
            await _transport.ExecuteSqlAsync(new CompiledQuery(
                "INSERT INTO query_group_data(id,name,version) VALUES (1,'Core',1),(2,'Other',1),(3,'Empty',1)", []));
            await _transport.ExecuteSqlAsync(new CompiledQuery(
                "INSERT INTO query_record_data(id,query_group,name,version) VALUES " +
                "(11,1,'included',1),(12,2,'excluded',1),(13,NULL,'orphan',1)", []));
            var child = new SelectQuery("QueryGroup")
            {
                FilterCondition = Expr.Eq("name", new Value.TextValue("Core"))
            };
            var included = new SelectQuery("QueryRecord")
            {
                FilterCondition = Expr.InSubquery("queryGroup", group, child, "id")
            };
            var excluded = new SelectQuery("QueryRecord")
            {
                FilterCondition = Expr.NotInSubquery("queryGroup", group, child, "id")
            };

            Assert.Equal("included", (await executor.QueryAsync(
                new QueryRequest { Query = included })).Rows.Single()["name"].TryText());
            Assert.Equal("excluded", (await executor.QueryAsync(
                new QueryRequest { Query = excluded })).Rows.Single()["name"].TryText());

            async Task<long[]> Ids(string entityName, Expr filter) =>
                (await executor.QueryAsync(new QueryRequest
                {
                    Query = new SelectQuery(entityName)
                        .Filter(filter).OrderAsc("id")
                })).Rows.Select(row => row["id"].TryI64()!.Value).ToArray();

            Assert.Equal([11L, 12L], await Ids("QueryRecord", Expr.IsNotNull("queryGroup")));
            Assert.Equal([13L], await Ids("QueryRecord", Expr.IsNull("queryGroup")));
            Assert.Equal([11L], await Ids("QueryRecord",
                Expr.InSubquery("queryGroup", group, child, "id")));
            Assert.Equal([12L], await Ids("QueryRecord",
                Expr.NotInSubquery("queryGroup", group, child, "id")));

            var allRecords = new SelectQuery("QueryRecord");
            Assert.Equal([1L, 2L], await Ids("QueryGroup",
                Expr.InSubquery("id", record, allRecords, "queryGroup")));
            Assert.Equal([3L], await Ids("QueryGroup",
                Expr.NotInSubquery("id", record, allRecords, "queryGroup")));
        }

        [Fact]
        public async Task TopNRelationPlansAreEquivalentStableAndObservable()
        {
            var order = EntityDescriptor.New("Order").TableName("orders")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("version", DataType.I64).Version())
                .Relation(RelationDescriptor.New("lines", "OrderLine").LocalKey("id").ForeignKey("orderId").Many());
            var line = EntityDescriptor.New("OrderLine").TableName("orderline")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("orderId", DataType.I64).ColumnName("order_id"))
                .Property(PropertyDescriptor.New("name", DataType.Text))
                .Property(PropertyDescriptor.New("state", DataType.Text))
                .Property(PropertyDescriptor.New("version", DataType.I64).Version());
            var module = new RuntimeModule().Entity(order).Entity(line);
            var recording = new RecordingTransport(_transport);
            var executor = new SqlDataServiceExecutor(new SqliteDialect(), recording, new ModuleSchemaProvider(module));
            var context = module.IntoContext().WithDataService(executor);
            await context.EnsureSchemaAsync();
            await context.EnsureSchemaAsync();
            await _transport.ExecuteSqlAsync(new CompiledQuery("INSERT INTO orders VALUES (11,1),(12,1),(13,1)", []));
            await _transport.ExecuteSqlAsync(new CompiledQuery(
                "INSERT INTO orderline VALUES " +
                "(1101,11,'same','visible',1),(1102,11,'same','visible',1),(1103,11,'same','visible',1),(1104,11,'same','visible',1)," +
                "(1201,12,'same','visible',1),(1202,12,'same','visible',1),(1203,12,'same','visible',1),(1204,12,'same','visible',1)," +
                "(9999,11,'same','hidden',1)", []));

            SelectQuery Query(ulong? threshold = null)
            {
                var child = new SelectQuery("OrderLine").Project("id").Project("name")
                    .Filter(Expr.Eq("state", new Value.TextValue("visible")))
                    .AndFilter(Expr.Gt("version", new Value.I64Value(0))).OrderDesc("name").Limit(3);
                if (threshold is not null) child.TopNProbeParentThreshold(threshold.Value);
                return new SelectQuery("Order").OrderAsc("id").RelationQuery("lines", child);
            }
            static string Ids(List<Record> rows) => string.Join(";", rows.Select(row =>
                $"{row["id"].TryI64()}:{string.Join(',', ((Value.ListValue)row["lines"]).Values
                    .Cast<Value.ObjectValue>().Select(child => child.Value["id"].TryI64()))}"));

            var observer = new RecordingTopNObserver();
            recording.Queries.Clear();
            var probes = await executor.QueryAsync(new QueryRequest { Query = Query(), RelationLoadObserver = observer });
            Assert.Equal([3, 3, 0], probes.Rows.Select(row => ((Value.ListValue)row["lines"]).Values.Count).ToArray());
            Assert.Equal(4, recording.Queries.Count);
            Assert.DoesNotContain(recording.Queries, sql => sql.Contains("COUNT(", StringComparison.OrdinalIgnoreCase));
            Assert.All(recording.Queries.Skip(1), sql =>
            {
                Assert.Contains("state", sql);
                Assert.Contains("version", sql);
                Assert.Contains("id ASC", sql);
            });

            recording.Queries.Clear();
            var window = await executor.QueryAsync(new QueryRequest { Query = Query(0), RelationLoadObserver = observer });
            Assert.Equal(2, recording.Queries.Count);
            Assert.Contains("ROW_NUMBER() OVER", recording.Queries[1]);
            Assert.Contains("state", recording.Queries[1]);
            Assert.Contains("version", recording.Queries[1]);
            Assert.DoesNotContain("COUNT(", recording.Queries[1], StringComparison.OrdinalIgnoreCase);
            Assert.Equal(Ids(probes.Rows), Ids(window.Rows));
            Assert.Equal("window", observer.Attributes!["teaql.relation.selected_plan"]);
            Assert.Equal(3, observer.Attributes["teaql.relation.parent_count"]);

            foreach (var (threshold, expected) in new[] { (3ul, 4), (2ul, 2) })
            {
                recording.Queries.Clear();
                var first = await executor.QueryAsync(new QueryRequest { Query = Query(threshold) });
                var sql = recording.Queries.ToArray();
                recording.Queries.Clear();
                var second = await executor.QueryAsync(new QueryRequest { Query = Query(threshold) });
                Assert.Equal(expected, recording.Queries.Count);
                Assert.Equal(sql, recording.Queries);
                Assert.Equal(Ids(first.Rows), Ids(second.Rows));
            }
            using var indexCommand = _connection.CreateCommand();
            indexCommand.CommandText = "SELECT count(*) FROM pragma_index_list('orderline') WHERE name='IDX_ORDERLINE_ORDER_ID_ID_DESC'";
            Assert.Equal(1L, (long)(await indexCommand.ExecuteScalarAsync())!);
        }

        [Fact]
        public async Task CompleteScalarFixtureIncludingNullableBooleanExecutes()
        {
            var record = EntityDescriptor.New("QueryRecord").TableName("query_record_scalar")
                .Property(PropertyDescriptor.New("id", DataType.I64).Id())
                .Property(PropertyDescriptor.New("requiredText", DataType.Text).ColumnName("required_text"))
                .Property(PropertyDescriptor.New("optionalText", DataType.Text).ColumnName("optional_text"))
                .Property(PropertyDescriptor.New("requiredInteger", DataType.I64).ColumnName("required_integer"))
                .Property(PropertyDescriptor.New("optionalLong", DataType.I64).ColumnName("optional_long"))
                .Property(PropertyDescriptor.New("requiredDecimal", DataType.Decimal).ColumnName("required_decimal"))
                .Property(PropertyDescriptor.New("requiredFloat", DataType.F64).ColumnName("required_float"))
                .Property(PropertyDescriptor.New("requiredDouble", DataType.F64).ColumnName("required_double"))
                .Property(PropertyDescriptor.New("requiredDate", DataType.Date).ColumnName("required_date"))
                .Property(PropertyDescriptor.New("requiredTime", DataType.I64).ColumnName("required_time"))
                .Property(PropertyDescriptor.New("requiredTimestamp", DataType.Timestamp).ColumnName("required_timestamp"))
                .Property(PropertyDescriptor.New("active", DataType.Bool))
                .Property(PropertyDescriptor.New("reviewed", DataType.Bool))
                .Property(PropertyDescriptor.New("version", DataType.I64).Version());
            var module = new RuntimeModule().Entity(record);
            var executor = new SqlDataServiceExecutor(
                new SqliteDialect(), _transport, new ModuleSchemaProvider(module));
            var context = module.IntoContext().WithDataService(executor);
            await context.EnsureSchemaAsync();
            await _transport.ExecuteSqlAsync(new CompiledQuery(
                "INSERT INTO query_record_scalar VALUES " +
                "(1,'Alpha','optional',42,42000000000,42.125,42.5,42.75,'2026-08-29',34200000,1777632600000,1,0,1)," +
                "(2,'Beta',NULL,7,NULL,7.500,7.5,7.75,'2026-08-30',36000000,1777720400000,0,NULL,1)," +
                "(3,'Gamma','tail',99,99000000000,99.875,99.5,99.75,'2026-08-31',37800000,1777808200000,1,1,1)", []));

            async Task<long[]> Ids(Expr expression)
            {
                var query = new SelectQuery("QueryRecord")
                {
                    Projection = ["id"],
                    FilterCondition = expression,
                    OrderByItems = [OrderBy.Asc("id")]
                };
                return (await executor.QueryAsync(new QueryRequest { Query = query })).Rows
                    .Select(row => row["id"].TryI64() ?? -1).ToArray();
            }
            Assert.Equal([1], await Ids(Expr.Eq("requiredText", new Value.TextValue("Alpha"))));
            Assert.Equal([2, 3], await Ids(Expr.Ne("requiredText", new Value.TextValue("Alpha"))));
            Assert.Equal([1, 3], await Ids(Expr.InList("requiredText",
                [new Value.TextValue("Alpha"), new Value.TextValue("Gamma")])));
            Assert.Equal([2], await Ids(Expr.Contain("requiredText", "et")));
            Assert.Equal([1, 3], await Ids(Expr.Between("requiredInteger", new Value.I64Value(40), new Value.I64Value(100))));
            Assert.Equal([3], await Ids(Expr.Gt("requiredDecimal", new Value.DecimalValue(50m))));
            Assert.Equal([2], await Ids(Expr.Lte("requiredFloat", new Value.F64Value(7.5))));
            Assert.Equal([3], await Ids(Expr.Gte("requiredDouble", new Value.F64Value(99.75))));
            Assert.Equal([2, 3], await Ids(Expr.Between("requiredDate",
                new Value.DateValue(new DateTime(2026, 8, 30)), new Value.DateValue(new DateTime(2026, 8, 31)))));
            Assert.Equal([3], await Ids(Expr.Gt("requiredTime", new Value.I64Value(36_000_000))));
            Assert.Equal([1, 2], await Ids(Expr.Lt("requiredTimestamp", new Value.TimestampValue(1_777_750_000_000))));
            Assert.Equal([2], await Ids(Expr.IsNull("optionalText")));
            Assert.Equal([1, 3], await Ids(Expr.IsNotNull("optionalLong")));
            Assert.Equal([2], await Ids(Expr.Eq("active", new Value.BoolValue(false))));
            Assert.Equal([3], await Ids(Expr.Eq("reviewed", new Value.BoolValue(true))));
            Assert.Equal([1], await Ids(Expr.Eq("reviewed", new Value.BoolValue(false))));
            Assert.Equal([2], await Ids(Expr.IsNull("reviewed")));
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

        private sealed class RecordingTransport(ISqlTransport inner) : ISqlTransport
        {
            public List<string> Queries { get; } = new();
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
            public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
            {
                Queries.Add(query.Sql);
                return await inner.FetchAllSqlAsync(query);
            }
        }

        private sealed class RecordingTopNObserver : IRelationLoadObserver
        {
            public IReadOnlyDictionary<string, object>? Attributes { get; private set; }
            public async Task ObserveAsync(string entity, string relation,
                IReadOnlyDictionary<string, object> attributes, Func<Task> body)
            {
                Attributes = attributes;
                await body();
            }
        }
    }
}
