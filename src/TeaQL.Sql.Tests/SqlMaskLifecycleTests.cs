using System.Runtime.CompilerServices;
using Moq;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Sql.Tests;

public class SqlMaskLifecycleTests
{
    private sealed class Transport : ISqlTransaction, IStreamingSqlTransport
    {
        public Exception? Failure;
        public int FailOnWrite = 1;
        public int WriteCount;
        public ulong AffectedRows;
        public int RowCount;
        public bool Closed;
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) => Failure == null
            ? Task.FromResult(new List<Record>()) : Task.FromException<List<Record>>(Failure);
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query)
        {
            WriteCount++;
            return Failure == null || WriteCount != FailOnWrite
                ? Task.FromResult(AffectedRows) : Task.FromException<ulong>(Failure);
        }
        public Task CommitSqlAsync() => Task.CompletedTask;
        public Task RollbackSqlAsync() => Task.CompletedTask;
        public void Dispose() { }
        public async IAsyncEnumerable<Record> StreamSqlAsync(CompiledQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            try
            {
                for (int i = 0; i < RowCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return new Record { ["name"] = Value.FromObject("Riverside") };
                    await Task.Yield();
                }
                if (Failure != null) throw Failure;
            }
            finally { Closed = true; }
        }
    }

    private static (RuntimeDataService Service, StringWriter Log) Fixture(bool transaction, Transport transport,
        Action<UserContext>? configure = null, bool legacy = false)
    {
        var entity = EntityDescriptor.New("Customer") // Compiled fixture bypasses allocator.
            .Property(PropertyDescriptor.New("name", DataType.Text))
            .Property(PropertyDescriptor.New("address", DataType.Text))
            .Property(PropertyDescriptor.New("password", DataType.Text));
        if (!legacy) entity = entity.AuditMaskFields(new() { "name" });
        var schema = new Mock<ISchemaProvider>();
        schema.Setup(s => s.GetEntity("Customer")).Returns(entity);
        var dialect = new Mock<SqlDialect>();
        var compiled = new TestSqlDialect().CompileSelect(entity,
            new SelectQuery("Customer").Filter(Expr.And(new List<Expr> {
                Expr.Eq("name", "Riverside"), Expr.Eq("address", "1 Runtime Road"), Expr.Eq("password", "PASSWORD-CANARY")
            })).Limit(5));
        dialect.Setup(d => d.CompileSelect(entity, It.IsAny<SelectQuery>())).Returns(compiled);
        dialect.Setup(d => d.CompileInsert(entity, It.IsAny<InsertCommand>())).Returns(compiled);
        dialect.Setup(d => d.CompileUpdate(entity, It.IsAny<UpdateCommand>())).Returns(compiled);
        dialect.Setup(d => d.CompileDelete(entity, It.IsAny<DeleteCommand>())).Returns(compiled);
        IDataService provider = transaction
            ? new SqlDataServiceTransaction(dialect.Object, transport, schema.Object)
            : new SqlDataServiceExecutor(dialect.Object, transport, schema.Object);
        var log = new StringWriter();
        var context = new UserContext().WithDiagnosticSqlLogSink(new TextDiagnosticSqlLogSink(log));
        configure?.Invoke(context);
        return (new RuntimeDataService(provider, context), log);
    }

    private static QueryRequest Request() => new(new SelectQuery("Customer").Limit(5)) {
        Comment = "what: inspect customers", Purpose = "why: lifecycle regression"
    };

    [Fact]
    public async Task LegacyGeneratedLibraryMasksAllParametersInDefaultSqlLog()
    {
        var (service, log) = Fixture(false, new Transport(), legacy: true);
        await service.QueryAsync(Request());
        var output = log.ToString();
        Assert.Contains("MASKED; NOT REPLAYABLE", output);
        Assert.Contains("[REDACTED]", output);
        Assert.DoesNotContain("Riverside", output);
        Assert.DoesNotContain("1 Runtime Road", output);
        Assert.DoesNotContain("PASSWORD-CANARY", output);
        Assert.Contains("comment=what: inspect customers", output);
        Assert.Contains("purpose=why: lifecycle regression", output);
    }
    private static void AssertLog(StringWriter log, string outcome)
    {
        var text = log.ToString();
        Assert.Contains("Ri*****de", text);
        Assert.Contains("1 Runtime Road", text);
        Assert.Contains("outcome=" + outcome, text);
        Assert.DoesNotContain("Riverside", text);
        Assert.DoesNotContain("PASSWORD-CANARY", text);
        Assert.DoesNotContain("DRIVER-CANARY", text);
        Assert.Equal(1, text.Split("[TeaQL SQL]").Length - 1);
    }

    [Theory]
    [InlineData(false, "query")][InlineData(true, "query")]
    [InlineData(false, "insert")][InlineData(true, "insert")]
    [InlineData(false, "update")][InlineData(true, "update")]
    [InlineData(false, "delete")][InlineData(true, "delete")]
    public async Task FailedStatementsLogWithoutReplacingOriginal(bool transaction, string operation)
    {
        var failure = new InvalidOperationException("DRIVER-CANARY Riverside PASSWORD-CANARY");
        var (service, log) = Fixture(transaction, new Transport { Failure = failure });
        var exception = await Assert.ThrowsAsync<SqlExecutorException>(async () => {
            if (operation == "query") await service.QueryAsync(Request());
            else {
                MutationRequest request = operation switch {
                    "insert" => new InsertMutationRequest(new InsertCommand("Customer")),
                    "update" => new UpdateMutationRequest(new UpdateCommand("Customer", Value.FromObject(1L))),
                    _ => new DeleteMutationRequest(new DeleteCommand("Customer", Value.FromObject(1L)))
                };
                await service.MutateAsync(request);
            }
        });
        Assert.Same(failure, exception.InnerException);
        AssertLog(log, "failure");
        Assert.DoesNotContain("0 rows affected", log.ToString());
    }

    [Theory]
    [InlineData(false, false)][InlineData(true, false)]
    [InlineData(false, true)][InlineData(true, true)]
    public async Task StreamCompleteOrDisposeLogsAndCloses(bool transaction, bool stop)
    {
        var transport = new Transport { RowCount = 3 };
        var (service, log) = Fixture(transaction, transport);
        // Interface cast also proves the context wrapper exposes the existing stream capability.
        var stream = Assert.IsAssignableFrom<IStreamQueryExecutor>(service);
        int delivered = 0;
        await foreach (var chunk in stream.QueryStreamAsync(Request(), 1)) {
            delivered += chunk.Rows.Count;
            Assert.Equal("Riverside", chunk.Rows[0]["name"].TryText());
            if (stop) break;
        }
        Assert.True(transport.Closed);
        Assert.Equal(stop ? 1 : 3, delivered);
        AssertLog(log, stop ? "cancelled" : "success");
        Assert.Contains($"{delivered} rows returned", log.ToString());
    }

    [Theory]
    [InlineData(false, 0, false)][InlineData(true, 0, false)]
    [InlineData(false, 1, false)][InlineData(true, 1, false)]
    [InlineData(false, 3, false)][InlineData(true, 3, false)]
    [InlineData(false, 0, true)][InlineData(true, 0, true)]
    public async Task StreamFailureCountsOnlyDeliveredRows(bool transaction, int rows, bool cancelled)
    {
        Exception failure = cancelled ? new OperationCanceledException() : new InvalidOperationException("DRIVER-CANARY Riverside PASSWORD-CANARY");
        var transport = new Transport { RowCount = rows, Failure = failure };
        var (service, log) = Fixture(transaction, transport);
        int delivered = 0;
        var caught = await Xunit.Record.ExceptionAsync(async () => {
            await foreach (var chunk in ((IStreamQueryExecutor)service).QueryStreamAsync(Request(), 1))
                delivered += chunk.Rows.Count;
        });
        Assert.Same(failure, caught);
        Assert.True(transport.Closed);
        Assert.Equal(Math.Max(0, rows - 1), delivered);
        AssertLog(log, cancelled ? "cancelled" : "failure");
        Assert.Contains($"{delivered} rows returned", log.ToString());
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task EmptyStreamStillProducesDiagnostic(bool transaction)
    {
        var transport = new Transport();
        var (service, log) = Fixture(transaction, transport);
        await foreach (var _ in ((IStreamQueryExecutor)service).QueryStreamAsync(Request(), 1))
            throw new Exception("unexpected row");
        Assert.True(transport.Closed);
        AssertLog(log, "success");
        Assert.Contains("0 rows returned", log.ToString());
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task DisabledStreamStillExecutesAndCloses(bool transaction)
    {
        var transport = new Transport { RowCount = 1 };
        var (service, log) = Fixture(transaction, transport, context => context.DisableQuerySqlLog());
        int count = 0;
        await foreach (var chunk in ((IStreamQueryExecutor)service).QueryStreamAsync(Request(), 1)) count += chunk.Rows.Count;
        Assert.Equal(1, count);
        Assert.True(transport.Closed);
        Assert.Equal("", log.ToString());
    }

    private sealed class BrokenSink : IDiagnosticSqlLogSink
    {
        public void Write(ExecutionMetadata metadata) => throw new Exception("sink unavailable");
    }

    private static MutationRequest Write(string label)
    {
        var command = new InsertCommand("Customer").Value("name", "Riverside");
        command.TraceChain.Add(new TraceNode("Customer", null, "why: " + label));
        return new InsertMutationRequest(command);
    }

    private static BatchMutationRequest Batch(bool nested) => new(new List<MutationRequest> {
        nested ? new BatchMutationRequest(new List<MutationRequest> { Write("first") }) : Write("first"),
        nested ? new BatchMutationRequest(new List<MutationRequest> { Write("second"), Write("unexecuted") }) : Write("second"),
        Write("unexecuted")
    });

    [Theory]
    [InlineData(false, false, false)][InlineData(true, false, false)]
    [InlineData(false, true, false)][InlineData(true, true, false)]
    [InlineData(false, true, true)][InlineData(true, true, true)]
    public async Task PartialBatchRetainsExecutedStatementsInOrder(bool transaction, bool nested, bool cancelled)
    {
        Exception failure = cancelled ? new OperationCanceledException("DRIVER-CANARY")
            : new InvalidOperationException("DRIVER-CANARY Riverside PASSWORD-CANARY");
        var transport = new Transport { Failure = failure, FailOnWrite = 2, AffectedRows = 1 };
        var (service, log) = Fixture(transaction, transport);
        var caught = await Xunit.Record.ExceptionAsync(() => service.MutateAsync(Batch(nested)));
        if (cancelled) Assert.Same(failure, caught);
        else Assert.Same(failure, Assert.IsType<SqlExecutorException>(caught).InnerException);
        Assert.Equal(2, transport.WriteCount);
        var text = log.ToString();
        Assert.Equal(2, text.Split("[TeaQL SQL]").Length - 1);
        Assert.True(text.IndexOf("outcome=success", StringComparison.Ordinal) < text.IndexOf("outcome=" + (cancelled ? "cancelled" : "failure"), StringComparison.Ordinal));
        Assert.Contains("1 rows affected", text);
        Assert.Contains("why: first", text);
        Assert.Contains("why: second", text);
        Assert.DoesNotContain("unexecuted", text);
        Assert.DoesNotContain("0 rows affected", text);
        Assert.Contains("Ri*****de", text);
        Assert.Contains("1 Runtime Road", text);
        Assert.DoesNotContain("Riverside", text);
        Assert.DoesNotContain("PASSWORD-CANARY", text);
        Assert.DoesNotContain("DRIVER-CANARY", text);
    }

    private sealed class CountingBrokenSink : IDiagnosticSqlLogSink
    {
        public int Attempts;
        public void Write(ExecutionMetadata metadata) { Attempts++; throw new Exception("sink unavailable"); }
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task PartialBatchAttemptsEveryDiagnosticDespiteBrokenSink(bool transaction)
    {
        var failure = new InvalidOperationException("DRIVER-CANARY");
        var sink = new CountingBrokenSink();
        var (service, _) = Fixture(transaction, new Transport { Failure = failure, FailOnWrite = 2, AffectedRows = 1 },
            context => context.WithDiagnosticSqlLogSink(sink));
        var caught = await Assert.ThrowsAsync<SqlExecutorException>(() => service.MutateAsync(Batch(true)));
        Assert.Same(failure, caught.InnerException);
        Assert.Equal(2, sink.Attempts);
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task SuccessfulNestedBatchDoesNotDuplicateLogs(bool transaction)
    {
        var transport = new Transport { AffectedRows = 1 };
        var (service, log) = Fixture(transaction, transport);
        var result = await service.MutateAsync(Batch(true));
        Assert.Equal(4UL, result.AffectedRows);
        Assert.Equal(4, transport.WriteCount);
        Assert.Equal(4, log.ToString().Split("[TeaQL SQL]").Length - 1);
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task LaterCompilationFailureReportsOnlyEarlierExecutedSql(bool transaction)
    {
        var transport = new Transport { AffectedRows = 1 };
        var (service, log) = Fixture(transaction, transport);
        await Assert.ThrowsAsync<SqlExecutorException>(() => service.MutateAsync(new BatchMutationRequest(new() {
            Write("first"), new InsertMutationRequest(new InsertCommand("UnknownEntity")), Write("unexecuted")
        })));
        Assert.Equal(1, transport.WriteCount);
        AssertLog(log, "success");
        Assert.Contains("1 rows affected", log.ToString());
        Assert.DoesNotContain("unexecuted", log.ToString());
        Assert.DoesNotContain("UnknownEntity", log.ToString());
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task FailedBatchCanBeReusedWithoutStaleDiagnostics(bool transaction)
    {
        var transport = new Transport { Failure = new InvalidOperationException(), FailOnWrite = 2, AffectedRows = 1 };
        var (service, log) = Fixture(transaction, transport);
        var batch = Batch(true);
        await Assert.ThrowsAsync<SqlExecutorException>(() => service.MutateAsync(batch));
        transport.Failure = null;
        log.GetStringBuilder().Clear();
        await service.MutateAsync(batch);
        Assert.Equal(4, log.ToString().Split("[TeaQL SQL]").Length - 1);
        Assert.DoesNotContain("outcome=failure", log.ToString());
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task DisabledPartialBatchStillPreservesErrorAndExecutionCount(bool transaction)
    {
        var failure = new InvalidOperationException("DRIVER-CANARY");
        var transport = new Transport { Failure = failure, FailOnWrite = 2, AffectedRows = 1 };
        var (service, log) = Fixture(transaction, transport, context => context.DisableMutationSqlLog());
        var caught = await Assert.ThrowsAsync<SqlExecutorException>(() => service.MutateAsync(Batch(true)));
        Assert.Same(failure, caught.InnerException);
        Assert.Equal(2, transport.WriteCount);
        Assert.Equal("", log.ToString());
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task FailedDiagnosticCannotReplaceDriverError(bool transaction)
    {
        var failure = new InvalidOperationException("DRIVER-CANARY");
        var (service, _) = Fixture(transaction, new Transport { Failure = failure },
            context => context.WithDiagnosticSqlLogSink(new BrokenSink()));
        var caught = await Assert.ThrowsAsync<SqlExecutorException>(() => service.QueryAsync(Request()));
        Assert.Same(failure, caught.InnerException);
    }
}
