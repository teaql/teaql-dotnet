using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class CheckerTests
{
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
            Assert.Equal("name", Assert.Single(error.Violations).Location);
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
                ? [] : [new CheckResult { RuleId = "required", Location = "name" }];
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
