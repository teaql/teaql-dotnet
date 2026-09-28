using Moq;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Sql.Tests;

[CollectionDefinition("Readback log environment", DisableParallelization = true)]
public class ReadbackLogEnvironment { }

[Collection("Readback log environment")]
public class SqlReadbackDiagnosticTests
{
    private sealed class Transport : ISqlTransaction
    {
        public Exception? ReadFailure;
        public Exception? WriteFailure;
        public List<Record> Rows = new();
        public List<CompiledQuery> Writes = new();
        public long? IdSpaceLevel;
        public int Reads;
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query)
        {
            Writes.Add(query);
            if (WriteFailure != null && !query.Sql.Contains("teaql_id_space", StringComparison.Ordinal))
                return Task.FromException<ulong>(WriteFailure);
            return Task.FromResult(1UL);
        }
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
        {
            Reads++;
            if (IdSpaceLevel is long level && query.Sql.Contains("teaql_id_space", StringComparison.Ordinal))
                return Task.FromResult(new List<Record> { new() { ["current_level"] = new Value.I64Value(level) } });
            return ReadFailure == null ? Task.FromResult(Rows) : Task.FromException<List<Record>>(ReadFailure);
        }
        public Task CommitSqlAsync() => Task.CompletedTask;
        public Task RollbackSqlAsync() => Task.CompletedTask;
        public void Dispose() { }
    }

    private sealed class Dialect : TestSqlDialect
    {
        public override DatabaseKind Kind => DatabaseKind.PostgreSql;
        public Exception? CompileFailure;
        public override CompiledQuery CompileSelect(EntityDescriptor entity, SelectQuery query)
            => CompileFailure == null ? base.CompileSelect(entity, query) : throw CompileFailure;
    }

    private sealed class Sink : IDiagnosticSqlLogSink
    {
        public readonly List<ExecutionMetadata> Entries = new();
        public readonly StringWriter Text = new();
        public bool Broken;
        public void Write(ExecutionMetadata metadata)
        {
            Entries.Add(metadata);
            if (Broken) throw new InvalidOperationException("sink failed");
            new TextDiagnosticSqlLogSink(Text).Write(metadata);
        }
    }

    private static RuntimeDataService Service(Transport transport, Sink sink, Dialect? dialect = null, bool disabled = false,
        ISensitiveDiagnosticSqlLogSink? sensitiveSink = null)
    {
        var entity = EntityDescriptor.New("Customer")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("name", DataType.Text))
            .Property(PropertyDescriptor.New("address", DataType.Text))
            .Property(PropertyDescriptor.New("password", DataType.Text))
            .AuditMaskFields(new() { "name" });
        var schema = new Mock<ISchemaProvider>();
        schema.Setup(s => s.GetEntity("Customer")).Returns(entity);
        var context = new UserContext().WithDiagnosticSqlLogSink(sink);
        if (sensitiveSink != null) context.WithSensitiveDiagnosticSqlLogSink(sensitiveSink);
        if (disabled) context.DisableQuerySqlLog().DisableMutationSqlLog();
        return new RuntimeDataService(new SqlDataServiceTransaction(dialect ?? new Dialect(), transport, schema.Object), context);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task QueryOwnedIntentSurvivesRuntimeCopyAndSafeOutput(bool failure)
    {
        var transport = new Transport();
        if (failure) transport.ReadFailure = new InvalidOperationException("DRIVER-CANARY");
        var sink = new Sink();
        var query = new SelectQuery("Customer")
            .Filter(Expr.Eq("name", Value.FromObject("Riverside"))).Limit(1)
            .Comment("what: locate Riverside").Purpose("why: test inherited query intent");
        var error = await Xunit.Record.ExceptionAsync(() => Service(transport, sink)
            .QueryAsync(new QueryRequest(query)));
        if (failure) Assert.Same(transport.ReadFailure, Assert.IsType<SqlExecutorException>(error).InnerException);
        else Assert.Null(error);
        var entry = Assert.Single(sink.Entries);
        Assert.Equal("what: locate [REDACTED]", entry.Comment);
        Assert.Equal("why: test inherited query intent", entry.Purpose);
        Assert.Equal(failure ? "failure" : "success", entry.ExecutionOutcome);
        Assert.Contains("Ri*****de", sink.Text.ToString());
        Assert.DoesNotContain("Riverside", sink.Text.ToString());
        Assert.Contains(entry.TraceChain, node => node.Comment == "what: locate [REDACTED]");
        Assert.Equal("what: locate Riverside", query.CommentText);
    }

    private static UpdateMutationRequest Update()
    {
        var command = new UpdateCommand("Customer", Value.FromObject(1L))
            .Value("name", "Riverside").Value("address", "1 Runtime Road").Value("password", "PASSWORD-CANARY");
        command.TraceChain.Add(new TraceNode("Customer", null, "why: preserve authoritative snapshot Riverside PASSWORD-CANARY"));
        return new UpdateMutationRequest(command);
    }

    private static Record Row() => new() { ["id"] = Value.FromObject(1L), ["name"] = Value.FromObject("Riverside") };

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task CreateTargetIdMasksIntentButRemainsPlainInCompiledSql(bool failure)
    {
        var transport = new Transport { IdSpaceLevel = 1001,
            WriteFailure = failure ? new InvalidOperationException("driver failed") : null,
            Rows = new() { new Record { ["id"] = new Value.I64Value(1001), ["name"] = new Value.TextValue("Riverside") } } };
        var sink = new Sink();
        var command = new InsertCommand("Customer").Value("id", new Value.I64Value(1001))
            .Value("name", "Riverside");
        command.TraceChain.Add(new TraceNode("Customer", 1001, "what: create customer 1001"));
        var service = Service(transport, sink);
        if (failure) await Assert.ThrowsAsync<SqlExecutorException>(() => service.MutateAsync(new InsertMutationRequest(command)));
        else Assert.Equal(1UL, (await service.MutateAsync(new InsertMutationRequest(command))).AffectedRows);
        var entry = Assert.Single(sink.Entries);
        Assert.Equal(failure ? "failure" : "success", entry.ExecutionOutcome);
        Assert.Equal("what: create customer [REDACTED]", entry.AuditReason);
        Assert.DoesNotContain("customer 1001", sink.Text.ToString());
        Assert.Contains("1001", sink.Text.ToString());
        Assert.Contains(transport.Writes.Last().Params, value => value.TryI64() == 1001);
        Assert.Equal("what: create customer 1001", command.TraceChain.Last().Comment);
    }

    [Fact]
    public async Task UpdateTargetIdMasksIntentButRemainsPlainInCompiledSql()
    {
        var transport = new Transport { Rows = new() {
            new Record { ["id"] = new Value.I64Value(1001), ["name"] = new Value.TextValue("Riverside") } } };
        var sink = new Sink();
        var command = new UpdateCommand("Customer", new Value.I64Value(1001)).Value("name", "Riverside");
        command.TraceChain.Add(new TraceNode("Customer", 1001, "what: update customer 1001"));
        var result = await Service(transport, sink).MutateAsync(new UpdateMutationRequest(command));
        Assert.Equal(1UL, result.AffectedRows);
        var entry = Assert.Single(sink.Entries);
        Assert.Equal("what: update customer [REDACTED]", entry.AuditReason);
        Assert.DoesNotContain("customer 1001", sink.Text.ToString());
        Assert.Contains("1001", sink.Text.ToString());
        Assert.Contains(transport.Writes.Last().Params, value => value.TryI64() == 1001);
        Assert.Equal("what: update customer 1001", command.TraceChain.Last().Comment);
    }

    [Fact]
    public async Task ExplicitSqlDebugStillHidesTargetIdFromIntent()
    {
        const string flag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
        var previous = Environment.GetEnvironmentVariable(flag);
        try
        {
            Environment.SetEnvironmentVariable(flag, "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK");
            var transport = new Transport { Rows = new() {
                new Record { ["id"] = new Value.I64Value(1001), ["name"] = new Value.TextValue("Riverside") } } };
            var sensitive = new SensitiveSink();
            var command = new UpdateCommand("Customer", new Value.I64Value(1001)).Value("name", "Riverside");
            command.TraceChain.Add(new TraceNode("Customer", 1001, "what: update customer 1001"));
            await Service(transport, new Sink(), sensitiveSink: sensitive).MutateAsync(new UpdateMutationRequest(command));
            var entry = Assert.Single(sensitive.Entries);
            Assert.Equal("what: update customer [REDACTED]", entry.AuditReason);
            Assert.Contains("1001", entry.DebugQuery);
            Assert.DoesNotContain("IntentValues", System.Text.Json.JsonSerializer.Serialize(entry));
        }
        finally { Environment.SetEnvironmentVariable(flag, previous); }
    }

    [Theory]
    [InlineData("error", false)][InlineData("error", true)]
    [InlineData("cancelled", false)][InlineData("cancelled", true)]
    [InlineData("empty", false)][InlineData("empty", true)]
    [InlineData("multiple", false)][InlineData("multiple", true)]
    public async Task FailedReadbackPreservesWriteAndReadFacts(string mode, bool batch)
    {
        var transport = new Transport();
        if (mode == "error") transport.ReadFailure = new InvalidOperationException("DRIVER-CANARY Riverside PASSWORD-CANARY");
        if (mode == "cancelled") transport.ReadFailure = new OperationCanceledException("DRIVER-CANARY");
        if (mode == "multiple") transport.Rows = new() { Row(), Row() };
        var sink = new Sink();
        var service = Service(transport, sink);
        MutationRequest request = Update();
        if (batch) request = new BatchMutationRequest(new() {
            new DeleteMutationRequest(new DeleteCommand("Customer", Value.FromObject(9L)).HardDelete()),
            new BatchMutationRequest(new() { request, Update() })
        });
        var error = await Xunit.Record.ExceptionAsync(() => service.MutateAsync(request));
        Assert.NotNull(error);
        if (transport.ReadFailure != null) Assert.Same(transport.ReadFailure, error);
        else if (mode == "empty") Assert.IsType<SqlExecutorException>(error);
        else Assert.IsType<InvalidOperationException>(error);
        Assert.Equal(batch ? 2 : 1, transport.Writes.Count);
        Assert.Equal(1, transport.Reads);
        Assert.Equal(batch ? 3 : 2, sink.Entries.Count);
        var write = sink.Entries[^2];
        var read = sink.Entries[^1];
        Assert.Equal(DataServiceOperation.Update, write.Operation);
        Assert.Equal("success", write.ExecutionOutcome);
        Assert.Equal(1UL, write.AffectedRows);
        Assert.Equal(DataServiceOperation.Query, read.Operation);
        Assert.Equal(mode == "error" ? "failure" : mode == "cancelled" ? "cancelled" : "success", read.ExecutionOutcome);
        Assert.Equal(mode == "empty" ? 0 : mode == "multiple" ? 2 : (int?)null, read.ResultCount);
        Assert.Null(read.AffectedRows);
        Assert.Equal("why: preserve authoritative snapshot [REDACTED] [REDACTED]", read.AuditReason);
        Assert.NotEmpty(read.TraceChain);
        var text = sink.Text.ToString();
        Assert.Contains("Ri*****de", text);
        Assert.Contains("1 Runtime Road", text);
        Assert.DoesNotContain("Riverside", text);
        Assert.DoesNotContain("PASSWORD-CANARY", text);
        Assert.DoesNotContain("DRIVER-CANARY", text);
        Assert.Contains(transport.Writes.Last().Params, value => value.TryText() == "Riverside");
        Assert.Contains(transport.Writes.Last().Params, value => value.TryText() == "PASSWORD-CANARY");
    }

    [Fact]
    public async Task SuccessfulReadbackKeepsSnapshotAndExistingLogScope()
    {
        var row = Row();
        var transport = new Transport { Rows = new() { row } };
        var sink = new Sink();
        var result = await Service(transport, sink).MutateAsync(Update());
        Assert.Same(row, result.PersistedRecord);
        Assert.Equal("Riverside", result.PersistedRecord!["name"].TryText());
        Assert.Single(sink.Entries);
        Assert.Equal(DataServiceOperation.Update, sink.Entries[0].Operation);
    }

    [Fact]
    public async Task ReadbackCompileFailureDoesNotInventQueryExecution()
    {
        var failure = new SqlCompileException("compile canary");
        var transport = new Transport();
        var sink = new Sink();
        var caught = await Xunit.Record.ExceptionAsync(() => Service(transport, sink,
            new Dialect { CompileFailure = failure }).MutateAsync(Update()));
        Assert.Same(failure, caught);
        Assert.Equal(0, transport.Reads);
        Assert.Single(sink.Entries);
        Assert.Equal("success", sink.Entries[0].ExecutionOutcome);
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task BrokenSinkCannotReplaceReadbackFailure(bool empty)
    {
        var failure = new InvalidOperationException("DRIVER-CANARY");
        var transport = new Transport { ReadFailure = empty ? null : failure };
        var sink = new Sink { Broken = true };
        var caught = await Xunit.Record.ExceptionAsync(() => Service(transport, sink).MutateAsync(Update()));
        if (empty) Assert.IsType<SqlExecutorException>(caught);
        else Assert.Same(failure, caught);
        Assert.Equal(2, sink.Entries.Count);
    }

    [Fact]
    public async Task DisabledLogsPreserveReadbackFailure()
    {
        var failure = new OperationCanceledException();
        var transport = new Transport { ReadFailure = failure };
        var sink = new Sink();
        var caught = await Xunit.Record.ExceptionAsync(() => Service(transport, sink, disabled: true).MutateAsync(Update()));
        Assert.Same(failure, caught);
        Assert.Equal(1, transport.Reads);
        Assert.Single(transport.Writes);
        Assert.Empty(sink.Entries);
    }

    private sealed class SensitiveSink : ISensitiveDiagnosticSqlLogSink
    {
        public readonly List<ExecutionMetadata> Entries = new();
        public void Write(ExecutionMetadata metadata) => Entries.Add(metadata);
    }

    [Fact]
    public async Task RetainedReadbackDebugSafelyReemitsAfterRevocation()
    {
        const string flag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
        var previous = Environment.GetEnvironmentVariable(flag);
        try
        {
            Environment.SetEnvironmentVariable(flag, "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK");
            var captured = new SensitiveSink();
            await Assert.ThrowsAsync<SqlExecutorException>(() => Service(new Transport(), new Sink(),
                sensitiveSink: captured).MutateAsync(Update()));
            Assert.Equal(2, captured.Entries.Count);
            Assert.Contains("Riverside", captured.Entries[1].Comment);
            Environment.SetEnvironmentVariable(flag, null);
            using var output = new StringWriter();
            var sink = new SensitiveDiagnosticSqlLogSink(output);
            foreach (var entry in captured.Entries) sink.Write(entry);
            Assert.DoesNotContain("Riverside", output.ToString());
            Assert.DoesNotContain("PASSWORD-CANARY", output.ToString());
            Assert.Contains("why: preserve authoritative snapshot", output.ToString());
            Assert.Contains("SELECT", output.ToString());
            Assert.DoesNotContain("DEBUG PLAINTEXT", output.ToString());
        }
        finally { Environment.SetEnvironmentVariable(flag, previous); }
    }

    [Theory]
    [InlineData(false)][InlineData(true)]
    public async Task InheritedIntentRespectsDebugButNeverExposesCredentials(bool debug)
    {
        const string flag = "TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS";
        var previous = Environment.GetEnvironmentVariable(flag);
        try
        {
            Environment.SetEnvironmentVariable(flag, debug ? "I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK" : null);
            var sink = new Sink();
            var sensitive = new SensitiveSink();
            await Assert.ThrowsAsync<SqlExecutorException>(() => Service(new Transport(), sink,
                sensitiveSink: sensitive).MutateAsync(Update()));
            Assert.Equal(2, sensitive.Entries.Count);
            foreach (var entry in sensitive.Entries)
            {
                var serialized = System.Text.Json.JsonSerializer.Serialize(entry);
                Assert.DoesNotContain("PASSWORD-CANARY", serialized);
                Assert.DoesNotContain("IntentSource", serialized);
                Assert.Equal(debug, entry.Comment!.Contains("Riverside"));
                if (debug) Assert.Contains("DEBUG PLAINTEXT; EXPLICIT OPT-IN", entry.DebugQuery);
            }
            Assert.DoesNotContain("Riverside", sink.Text.ToString());
            Assert.DoesNotContain("PASSWORD-CANARY", sink.Text.ToString());
        }
        finally { Environment.SetEnvironmentVariable(flag, previous); }
    }
}
