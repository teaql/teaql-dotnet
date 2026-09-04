using TeaQL.Core;
using TeaQL.DataService;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Runtime.Tests;

public class CheckerTests
{
    [Fact]
    public void GeneratedBootstrapScopeSuppliesAndRestoresContextIdentity()
    {
        var context = new UserContext { UserIdentifier = "application-user" }
            .WithActiveRoot("Tenant", 42);
        context.InsertNamedResource("bootstrapCategory", "application-category");

        using (context.EnterGeneratedBootstrap("Platform", 1))
        {
            Assert.Equal("teaql-generated-bootstrap", context.UserIdentifier);
            Assert.Equal(new ContextEntityRef("Platform", 1), context.RequireActiveRoot("Platform"));
            Assert.Equal("runtime-bootstrap", context.RequireNamedResource<string>("bootstrapCategory"));
        }

        Assert.Equal("application-user", context.UserIdentifier);
        Assert.Equal(new ContextEntityRef("Tenant", 42), context.RequireActiveRoot("Tenant"));
        Assert.Equal("application-category", context.RequireNamedResource<string>("bootstrapCategory"));
    }

    [Fact]
    public void ActiveRootIsTypedAndFailsClosed()
    {
        var context = new UserContext().WithActiveRoot("Tenant", 42);
        Assert.Equal(new ContextEntityRef("Tenant", 42), context.RequireActiveRoot("Tenant"));
        Assert.Throws<ContextRootException>(() => context.RequireActiveRoot("Organization"));
        Assert.Throws<ContextRootException>(() => new UserContext().RequireActiveRoot("Tenant"));
    }

    [Fact]
    public async Task CheckerFailureIsStructuredSaveScopedAndPrecedesProvider()
    {
        var provider = new CountingDataService();
        var checker = new RequiredNameChecker();
        var context = new UserContext()
            .Install(new RuntimeModule().Checker("School", checker))
            .WithDataService(provider);
        var service = context.RequireResource<IDataService>();
        var request = new InsertMutationRequest(new InsertCommand("School"));

        for (var attempt = 0; attempt < 2; attempt++)
        {
            var error = await Assert.ThrowsAsync<CheckException>(() => service.MutateAsync(request));
            var violation = Assert.Single(error.Violations);
            Assert.Equal("name", violation.ModelPath);
            Assert.Equal("Name", violation.NativePath);
            Assert.Equal("/name", violation.InstancePath);
        }
        Assert.Equal(2, checker.Calls);
        Assert.Equal(0, provider.MutationCalls);
    }

    [Fact]
    public async Task SuccessfulGeneratedBootstrapMutationPublishesSafeAuditEvent()
    {
        var provider = new CountingDataService
        {
            Result = new MutationResult
            {
                AffectedRows = 1,
                PersistedRecord = new Record
                {
                    ["id"] = new Value.I64Value(1001),
                    ["version"] = new Value.I64Value(2)
                }
            }
        };
        var sink = new CapturingAuditSink();
        var context = new UserContext { UserIdentifier = "application-user" }
            .WithDataService(provider)
            .WithAppAuditEventSink(sink);
        var command = new UpdateCommand("SchoolType", new Value.I64Value(1001))
            .Value("name", new Value.TextValue("Primary School"))
            .Value("version", new Value.I64Value(1));
        command.TraceChain.Add(new TraceNode(
            "SchoolType", 1001, "reconcile generated constant"));

        using (context.EnterGeneratedBootstrap("Platform", 1))
            await context.RequireResource<IDataService>()
                .MutateAsync(new UpdateMutationRequest(command));

        var audit = Assert.Single(sink.Events);
        Assert.Equal("teaql-generated-bootstrap", audit["actor"]);
        Assert.Equal("runtime-bootstrap", audit["category"]);
        Assert.Equal("reconcile generated constant", audit["reason"]);
        Assert.Equal("SchoolType", audit["entityType"]);
        Assert.Equal(1001L, audit["entityId"]);
        Assert.Equal("update", audit["mutationKind"]);
        Assert.Equal(new[] { "name", "version" }, Assert.IsType<string[]>(audit["changedFields"]));
        Assert.Equal(2, audit["changedFieldCount"]);
        Assert.Equal(2L, audit["resultVersion"]);
        Assert.Equal("application-user", context.UserIdentifier);
    }

    [Fact]
    public async Task SuccessfulMutationWithoutAuditSinkIsNoOp()
    {
        var provider = new CountingDataService();
        var context = new UserContext().WithDataService(provider);
        var command = new InsertCommand("School").Value("name", new Value.TextValue("Riverside"));
        command.TraceChain.Add(new TraceNode("School", null, "create school"));

        var result = await context.RequireResource<IDataService>()
            .MutateAsync(new InsertMutationRequest(command));

        Assert.Equal(1ul, result.AffectedRows);
        Assert.Equal(1, provider.MutationCalls);
    }

    private sealed class RequiredNameChecker : IEntityChecker
    {
        public int Calls { get; private set; }
        public IReadOnlyList<CheckResult> CheckAndFix(
            UserContext context, MutationRequest mutation, DateTimeOffset now)
        {
            Calls++;
            var insert = Assert.IsType<InsertMutationRequest>(mutation);
            return insert.Command.Values.ContainsKey("name")
                ? [] : [new CheckResult { RuleId = "required", Location = ObjectLocation.Property("name") }];
        }
    }

    private sealed class CountingDataService : IDataService
    {
        public MutationResult Result { get; init; } = new() { AffectedRows = 1 };
        public int MutationCalls { get; private set; }
        public DataServiceCapabilities Capabilities { get; } = new() { Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) =>
            Task.FromResult(new QueryResult());
        public Task<MutationResult> MutateAsync(MutationRequest request)
        {
            MutationCalls++;
            return Task.FromResult(Result);
        }
    }

    private sealed class CapturingAuditSink : IAppAuditEventSink
    {
        public List<IReadOnlyDictionary<string, object?>> Events { get; } = [];
        public Task RecordAsync(IReadOnlyDictionary<string, object?> safeEvent,
            CancellationToken cancellationToken = default)
        {
            Events.Add(safeEvent);
            return Task.CompletedTask;
        }
    }
}
