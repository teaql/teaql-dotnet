using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using TeaQL.Sql;
using TeaQL.Core;
using TeaQL.DataService;
using Record = TeaQL.Core.Record;

namespace TeaQL.Sql.Tests
{
    public class SqlDataServiceExecutorTests
    {
        private readonly Mock<SqlDialect> _mockDialect;
        private readonly Mock<ISqlTransactionTransport> _mockTransport;
        private readonly Mock<IStreamingSqlTransport> _mockStreamingTransport;
        private readonly Mock<ISchemaProvider> _mockSchemaProvider;
        private readonly SqlDataServiceExecutor _executor;

        public SqlDataServiceExecutorTests()
        {
            _mockDialect = new Mock<SqlDialect>();
            _mockTransport = new Mock<ISqlTransactionTransport>();
            _mockStreamingTransport = _mockTransport.As<IStreamingSqlTransport>();
            _mockSchemaProvider = new Mock<ISchemaProvider>();

            _executor = new SqlDataServiceExecutor(_mockDialect.Object, _mockTransport.Object, _mockSchemaProvider.Object);
        }

        [Fact]
        public void Capabilities_ReturnsExpected()
        {
            var caps = _executor.Capabilities;
            Assert.True(caps.Query);
            Assert.True(caps.Mutation);
            Assert.True(caps.Transaction);
            Assert.True(caps.Schema);
        }

        [Fact]
        public async Task QueryAsync_CallsTransport_AndReturnsResult()
        {
            var req = new QueryRequest { Query = new SelectQuery { Entity = "TestEntity" } };
            var ed = new EntityDescriptor { Name = "TestEntity", TableNameValue = "test" };
            _mockSchemaProvider.Setup(s => s.GetEntity("TestEntity")).Returns(ed);

            var compiled = new CompiledQuery(
                "SELECT $1", new List<Value> { new Value.TextValue("secret-customer-value") }, null);
            _mockDialect.Setup(d => d.CompileSelect(ed, req.Query)).Returns(compiled);

            var rows = new List<Record> { new Record() };
            _mockTransport.Setup(t => t.FetchAllSqlAsync(compiled)).ReturnsAsync(rows);

            var result = await _executor.QueryAsync(req);

            Assert.Single(result.Rows);
            Assert.Equal(DataServiceOperation.Query, result.Metadata.Operation);
            Assert.Equal(1, result.Metadata.ResultCount);
            Assert.Equal("SELECT $1", result.Metadata.ParameterizedQuery);
            Assert.DoesNotContain("secret-customer-value", result.Metadata.ParameterizedQuery);
            Assert.Single(result.Metadata.Parameters);
            Assert.Equal(1, result.Metadata.ParameterCount);
        }

        [Fact]
        public async Task QueryAsync_ThrowsIfEntityNotFound()
        {
            var req = new QueryRequest { Query = new SelectQuery { Entity = "Unknown" } };
            _mockSchemaProvider.Setup(s => s.GetEntity("Unknown")).Returns((EntityDescriptor?)null);

            await Assert.ThrowsAsync<SqlExecutorException>(() => _executor.QueryAsync(req));
        }

        [Fact]
        public async Task QueryAsync_ObservesActualRelationFetchAndAttach()
        {
            var school = new EntityDescriptor { Name = "School", TableNameValue = "school" }
                .Relation(RelationDescriptor.New("students", "Student")
                    .ForeignKey("schoolId").Many());
            var student = new EntityDescriptor { Name = "Student", TableNameValue = "student" };
            _mockSchemaProvider.Setup(s => s.GetEntity("School")).Returns(school);
            _mockSchemaProvider.Setup(s => s.GetEntity("Student")).Returns(student);
            var parentSql = new CompiledQuery("SELECT school", new List<Value>());
            var childSql = new CompiledQuery("SELECT student", new List<Value>());
            _mockDialect.Setup(d => d.CompileSelect(school, It.IsAny<SelectQuery>()))
                .Returns(parentSql);
            _mockDialect.Setup(d => d.CompileSelect(student, It.IsAny<SelectQuery>()))
                .Returns(childSql);
            _mockTransport.SetupSequence(t => t.FetchAllSqlAsync(It.IsAny<CompiledQuery>()))
                .ReturnsAsync(new List<Record> { new() { ["id"] = new Value.I64Value(1) } })
                .ReturnsAsync(new List<Record>
                    { new() { ["id"] = new Value.I64Value(2), ["schoolId"] = new Value.I64Value(1) } });
            var observer = new RecordingRelationObserver();
            var query = new SelectQuery("School").Relation("students");

            var result = await _executor.QueryAsync(new QueryRequest
                { Query = query, RelationLoadObserver = observer });

            Assert.Equal("School", observer.Entity);
            Assert.Equal("students", observer.Relation);
            Assert.Equal(1, observer.Invocations);
            var related = Assert.IsType<Value.ListValue>(result.Rows[0]["students"]);
            Assert.Single(related.Values);
        }

        [Fact]
        public async Task MutateAsync_Insert_CallsTransport()
        {
            var cmd = new InsertCommand { Entity = "TestEntity" };
            var req = new InsertMutationRequest(cmd);
            var ed = new EntityDescriptor { Name = "TestEntity", TableNameValue = "test" };
            _mockSchemaProvider.Setup(s => s.GetEntity("TestEntity")).Returns(ed);

            var compiled = new CompiledQuery("INSERT ...", new List<Value>(), null);
            _mockDialect.Setup(d => d.CompileInsert(ed, cmd)).Returns(compiled);

            _mockTransport.Setup(t => t.ExecuteSqlAsync(compiled)).ReturnsAsync(1ul);

            var result = await _executor.MutateAsync(req);

            Assert.Equal(1ul, result.AffectedRows);
            Assert.Equal(DataServiceOperation.Insert, result.Metadata.Operation);
            Assert.Equal("INSERT ...", result.Metadata.ParameterizedQuery);
            Assert.Empty(result.Metadata.Parameters);
        }

        [Fact]
        public async Task QueryStreamAsync_ChunksResults()
        {
            var req = new QueryRequest { Query = new SelectQuery { Entity = "TestEntity" } };
            var ed = new EntityDescriptor { Name = "TestEntity", TableNameValue = "test" };
            _mockSchemaProvider.Setup(s => s.GetEntity("TestEntity")).Returns(ed);

            var compiled = new CompiledQuery("SELECT 1", new List<Value>(), null);
            _mockDialect.Setup(d => d.CompileSelect(ed, req.Query)).Returns(compiled);

            var rows = new List<Record> { new Record(), new Record(), new Record() };
            _mockStreamingTransport.Setup(t => t.StreamSqlAsync(compiled, default)).Returns(StreamRows(rows));

            var chunks = new List<StreamChunk>();
            await foreach (var chunk in _executor.QueryStreamAsync(req, 2))
                chunks.Add(chunk);

            Assert.Equal(2, chunks.Count);
            Assert.Equal(2, chunks[0].Rows.Count);
            Assert.Single(chunks[1].Rows);
            Assert.True(chunks[1].IsLast);
        }

        private static async IAsyncEnumerable<Record> StreamRows(IEnumerable<Record> rows)
        {
            foreach (var row in rows)
            {
                await Task.Yield();
                yield return row;
            }
        }

        private sealed class RecordingRelationObserver : IRelationLoadObserver
        {
            public string? Entity { get; private set; }
            public string? Relation { get; private set; }
            public int Invocations { get; private set; }
            public async Task ObserveAsync(string entity, string relation, Func<Task> body)
            {
                Entity = entity;
                Relation = relation;
                Invocations++;
                await body();
            }
        }

        [Fact]
        public async Task BeginTransactionAsync_ReturnsTransaction()
        {
            var mockTx = new Mock<ISqlTransaction>();
            _mockTransport.Setup(t => t.BeginSqlAsync()).ReturnsAsync(mockTx.Object);

            var tx = await _executor.BeginTransactionAsync();

            Assert.NotNull(tx);
            Assert.IsType<SqlDataServiceTransaction>(tx);
        }
    }

    public class SqlDataServiceTransactionTests
    {
        [Fact]
        public async Task CommitAsync_CallsTransport()
        {
            var mockDialect = new Mock<SqlDialect>();
            var mockTx = new Mock<ISqlTransaction>();
            var mockSchemaProvider = new Mock<ISchemaProvider>();

            var tx = new SqlDataServiceTransaction(mockDialect.Object, mockTx.Object, mockSchemaProvider.Object);

            await tx.CommitAsync();

            mockTx.Verify(t => t.CommitSqlAsync(), Times.Once);
        }

        [Fact]
        public async Task RollbackAsync_CallsTransport()
        {
            var mockDialect = new Mock<SqlDialect>();
            var mockTx = new Mock<ISqlTransaction>();
            var mockSchemaProvider = new Mock<ISchemaProvider>();

            var tx = new SqlDataServiceTransaction(mockDialect.Object, mockTx.Object, mockSchemaProvider.Object);

            await tx.RollbackAsync();

            mockTx.Verify(t => t.RollbackSqlAsync(), Times.Once);
        }

        [Fact]
        public void Dispose_CallsTransport()
        {
            var mockDialect = new Mock<SqlDialect>();
            var mockTx = new Mock<ISqlTransaction>();
            var mockSchemaProvider = new Mock<ISchemaProvider>();

            var tx = new SqlDataServiceTransaction(mockDialect.Object, mockTx.Object, mockSchemaProvider.Object);

            tx.Dispose();

            mockTx.Verify(t => t.Dispose(), Times.Once);
        }
    }
}
