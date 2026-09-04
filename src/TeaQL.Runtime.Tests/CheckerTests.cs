using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

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
        public int MutationCalls { get; private set; }
        public DataServiceCapabilities Capabilities { get; } = new() { Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) =>
            Task.FromResult(new QueryResult());
        public Task<MutationResult> MutateAsync(MutationRequest request)
        {
            MutationCalls++;
            return Task.FromResult(new MutationResult { AffectedRows = 1 });
        }
    }
}
