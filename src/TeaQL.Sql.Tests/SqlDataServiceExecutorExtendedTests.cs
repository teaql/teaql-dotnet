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
    public class SqlDataServiceExecutorExtendedTests
    {
        private readonly Mock<SqlDialect> _mockDialect;
        private readonly Mock<ISqlTransactionTransport> _mockTransport;
        private readonly Mock<ISchemaProvider> _mockSchemaProvider;
        private readonly SqlDataServiceExecutor _executor;

        public SqlDataServiceExecutorExtendedTests()
        {
            _mockDialect = new Mock<SqlDialect>();
            _mockTransport = new Mock<ISqlTransactionTransport>();
            _mockSchemaProvider = new Mock<ISchemaProvider>();

            _executor = new SqlDataServiceExecutor(_mockDialect.Object, _mockTransport.Object, _mockSchemaProvider.Object);
        }

        [Fact]
        public async Task QueryAsync_Throws_WhenDialectThrows()
        {
            var req = new QueryRequest { Query = new SelectQuery { Entity = "TestEntity" } };
            var ed = new EntityDescriptor { Name = "TestEntity" };
            _mockSchemaProvider.Setup(s => s.GetEntity("TestEntity")).Returns(ed);

            _mockDialect.Setup(d => d.CompileSelect(ed, req.Query)).Throws(new SqlCompileException("bad"));

            await Assert.ThrowsAsync<SqlExecutorException>(() => _executor.QueryAsync(req));
        }

        [Fact]
        public async Task QueryAsync_Throws_WhenTransportThrows()
        {
            var req = new QueryRequest { Query = new SelectQuery { Entity = "TestEntity" } };
            var ed = new EntityDescriptor { Name = "TestEntity" };
            _mockSchemaProvider.Setup(s => s.GetEntity("TestEntity")).Returns(ed);

            var compiled = new CompiledQuery("SELECT 1", new List<Value>());
            _mockDialect.Setup(d => d.CompileSelect(ed, req.Query)).Returns(compiled);

            _mockTransport.Setup(t => t.FetchAllSqlAsync(compiled)).ThrowsAsync(new Exception("db error"));

            await Assert.ThrowsAsync<SqlExecutorException>(() => _executor.QueryAsync(req));
        }

        [Fact]
        public async Task MutateAsync_Batch_CallsSubRequests()
        {
            var insertCmd = new InsertCommand { Entity = "TestEntity" };
            var req1 = new InsertMutationRequest(insertCmd);
            var req2 = new InsertMutationRequest(insertCmd);
            var batchReq = new BatchMutationRequest(new List<MutationRequest> { req1, req2 });

            var ed = new EntityDescriptor { Name = "TestEntity" };
            _mockSchemaProvider.Setup(s => s.GetEntity("TestEntity")).Returns(ed);

            var compiled = new CompiledQuery("INSERT ...", new List<Value>());
            _mockDialect.Setup(d => d.CompileInsert(ed, insertCmd)).Returns(compiled);
            
            _mockTransport.Setup(t => t.ExecuteSqlAsync(compiled)).ReturnsAsync(5ul);

            var result = await _executor.MutateAsync(batchReq);

            Assert.Equal(10ul, result.AffectedRows);
        }
        
        [Fact]
        public async Task QueryStreamAsync_Throws_WhenTransportThrows()
        {
            var req = new QueryRequest { Query = new SelectQuery { Entity = "TestEntity" } };
            var ed = new EntityDescriptor { Name = "TestEntity" };
            _mockSchemaProvider.Setup(s => s.GetEntity("TestEntity")).Returns(ed);

            var compiled = new CompiledQuery("SELECT 1", new List<Value>());
            _mockDialect.Setup(d => d.CompileSelect(ed, req.Query)).Returns(compiled);

            _mockTransport.Setup(t => t.FetchAllSqlAsync(compiled)).ThrowsAsync(new Exception("db error"));

            await Assert.ThrowsAsync<SqlExecutorException>(() => _executor.QueryStreamAsync(req, 10));
        }

        [Fact]
        public void SqlExecutorException_Constructors_Work()
        {
            var ex1 = new SqlExecutorException("msg");
            Assert.Equal("msg", ex1.Message);

            var inner = new Exception("inner");
            var ex2 = new SqlExecutorException("msg", inner);
            Assert.Same(inner, ex2.InnerException);
        }
    }
}
