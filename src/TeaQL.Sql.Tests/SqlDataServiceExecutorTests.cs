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
            Assert.False(caps.Schema);
        }

        [Fact]
        public async Task QueryAsync_CallsTransport_AndReturnsResult()
        {
            var req = new QueryRequest { Query = new SelectQuery { Entity = "TestEntity" } };
            var ed = new EntityDescriptor { Name = "TestEntity", TableNameValue = "test" };
            _mockSchemaProvider.Setup(s => s.GetEntity("TestEntity")).Returns(ed);

            var compiled = new CompiledQuery("SELECT 1", new List<Value>(), null);
            _mockDialect.Setup(d => d.CompileSelect(ed, req.Query)).Returns(compiled);

            var rows = new List<Record> { new Record() };
            _mockTransport.Setup(t => t.FetchAllSqlAsync(compiled)).ReturnsAsync(rows);

            var result = await _executor.QueryAsync(req);

            Assert.Single(result.Rows);
            Assert.Equal(DataServiceOperation.Query, result.Metadata.Operation);
            Assert.Equal(1, result.Metadata.ResultCount);
        }

        [Fact]
        public async Task QueryAsync_ThrowsIfEntityNotFound()
        {
            var req = new QueryRequest { Query = new SelectQuery { Entity = "Unknown" } };
            _mockSchemaProvider.Setup(s => s.GetEntity("Unknown")).Returns((EntityDescriptor?)null);

            await Assert.ThrowsAsync<SqlExecutorException>(() => _executor.QueryAsync(req));
        }

        [Fact]
        public async Task ContinuousPageFetch_UsesIdSeekForMatchingNextPage()
        {
            var entity = new EntityDescriptor { Name = "Order", TableNameValue = "orders" };
            _mockSchemaProvider.Setup(s => s.GetEntity("Order")).Returns(entity);
            SelectQuery? secondCompiledQuery = null;
            var compileCount = 0;
            _mockDialect.Setup(d => d.CompileSelect(entity, It.IsAny<SelectQuery>()))
                .Returns((EntityDescriptor _, SelectQuery query) =>
                {
                    if (++compileCount == 2) secondCompiledQuery = query;
                    return new CompiledQuery("SELECT id FROM orders", new List<Value>(), null);
                });
            _mockDialect.SetupGet(d => d.Kind).Returns(DatabaseKind.Sqlite);
            _mockTransport.SetupSequence(t => t.FetchAllSqlAsync(It.IsAny<CompiledQuery>()))
                .ReturnsAsync(Enumerable.Range(91, 10).Reverse().Select(IdRecord).ToList())
                .ReturnsAsync(Enumerable.Range(81, 10).Reverse().Select(IdRecord).ToList());
            var store = new TestContinuousPageStore();
            var runtime = new ContinuousPageRuntimeContext(store, "tenant-1:user-1");

            await _executor.QueryAsync(new QueryRequest {
                Query = new SelectQuery("Order").OrderDesc("id").Page(0, 10).OptimizeForContinuousPageFetchWith("orders", 60),
                ContinuousPageRuntime = runtime
            });
            await _executor.QueryAsync(new QueryRequest {
                Query = new SelectQuery("Order").OrderDesc("id").Page(10, 10).OptimizeForContinuousPageFetchWith("orders", 60),
                ContinuousPageRuntime = runtime
            });

            Assert.Equal("CURSOR_SEEK", runtime.Plan);
            Assert.Equal((ulong)0, secondCompiledQuery!.Slice!.Offset);
            var seek = Assert.IsType<Expr.BinaryExpr>(secondCompiledQuery.FilterCondition);
            Assert.Equal(BinaryOp.Lt, seek.Op);
        }

        [Fact]
        public async Task ContinuousPageFetch_UsesAscendingIdSeek()
        {
            var entity = new EntityDescriptor { Name = "Order", TableNameValue = "orders" };
            _mockSchemaProvider.Setup(s => s.GetEntity("Order")).Returns(entity);
            SelectQuery? secondCompiledQuery = null;
            var compileCount = 0;
            _mockDialect.Setup(d => d.CompileSelect(entity, It.IsAny<SelectQuery>()))
                .Returns((EntityDescriptor _, SelectQuery query) =>
                {
                    if (++compileCount == 2) secondCompiledQuery = query;
                    return new CompiledQuery("SELECT id FROM orders", new List<Value>(), null);
                });
            _mockDialect.SetupGet(d => d.Kind).Returns(DatabaseKind.Sqlite);
            _mockTransport.SetupSequence(t => t.FetchAllSqlAsync(It.IsAny<CompiledQuery>()))
                .ReturnsAsync(Enumerable.Range(1, 10).Select(IdRecord).ToList())
                .ReturnsAsync(Enumerable.Range(11, 10).Select(IdRecord).ToList());
            var runtime = new ContinuousPageRuntimeContext(new TestContinuousPageStore(), "tenant-1:user-1");

            await _executor.QueryAsync(new QueryRequest {
                Query = new SelectQuery("Order").OrderAsc("id").Page(0, 10).OptimizeForContinuousPageFetch(),
                ContinuousPageRuntime = runtime
            });
            await _executor.QueryAsync(new QueryRequest {
                Query = new SelectQuery("Order").OrderAsc("id").Page(10, 10).OptimizeForContinuousPageFetch(),
                ContinuousPageRuntime = runtime
            });

            Assert.Equal("CURSOR_SEEK", runtime.Plan);
            var seek = Assert.IsType<Expr.BinaryExpr>(secondCompiledQuery!.FilterCondition);
            Assert.Equal(BinaryOp.Gt, seek.Op);
        }

        private static Record IdRecord(int id) => new() { ["id"] = new Value.I64Value(id) };

        private sealed class TestContinuousPageStore : IContinuousPageCursorStore
        {
            private readonly Dictionary<(string, ulong), ContinuousPageCursor> _values = new();
            public Task<ContinuousPageCursor?> GetAsync(string queryKey, ulong offset) =>
                Task.FromResult(_values.TryGetValue((queryKey, offset), out var value) ? value : null);
            public Task PutAsync(ContinuousPageCursor cursor) { _values[(cursor.QueryKey, cursor.NextOffset)] = cursor; return Task.CompletedTask; }
            public Task InvalidateAsync(string queryKey) { _values.Clear(); return Task.CompletedTask; }
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
