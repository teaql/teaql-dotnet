using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

/// <summary>Request intent must fail before customer code, provider IO or committed audit.</summary>
public class RequestIntentGateTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BlankRootIntentCannotReachPolicyProviderOrGraphCallback(bool logging)
    {
        var provider = new CountingProvider();
        var policy = new CountingPolicy();
        var audit = new CountingAudit();
        var registryCalls = 0;
        var graphCalls = 0;
        var context = new UserContext().WithDataService(provider).WithRequestPolicy(policy)
            .WithAppAuditEventSink(audit)
            .WithMutationPolicyRegistry(new DelegatingMutationPolicyRegistry(_ =>
            {
                registryCalls++;
                return null;
            }))
            .EnableQuerySqlLog(logging).EnableMutationSqlLog(logging);
        var service = new RuntimeDataService(provider, context);

        foreach (var blank in new string?[] { null, "", " \t\r\n", "\u0085", "\u00a0", "\u2003" })
        {
            foreach (var missingComment in new[] { true, false })
            {
                var query = new SelectQuery("Probe")
                    .Comment(missingComment ? blank! : "load SECRET-CANARY")
                    .Purpose(missingComment ? "render SECRET-CANARY" : blank!);
                var code = missingComment ? "REQUEST_COMMENT_REQUIRED" : "QUERY_PURPOSE_REQUIRED";
                var field = missingComment ? "comment" : "purpose";
                Check(Assert.Throws<RequestIntentException>(() => context.ApplyRequestPolicy(query)), code, field, "query");
                // Construction is itself the terminal gate: provider entry is unreachable.
                Check(await Assert.ThrowsAsync<RequestIntentException>(() =>
                    service.QueryAsync(context.PrepareQueryRequest(new QueryRequest(query)))), code, field, "query");
            }
            Check(await Assert.ThrowsAsync<RequestIntentException>(() =>
                context.ExecuteGraphSaveAsync(blank!, async graph =>
                {
                    graphCalls++;
                    // A valid child reason must not rescue a missing root reason.
                    return await graph.MutateAsync(new InsertMutationRequest(
                        new InsertCommand("Probe").Value("id", Value.FromObject(1L)), "valid child comment"));
                })), "REQUEST_COMMENT_REQUIRED", "comment", "mutation");
        }
        Assert.Equal(0, policy.Calls);
        Assert.Equal(0, registryCalls);
        Assert.Equal(0, graphCalls);
        Assert.Equal(0, provider.Calls);
        Assert.Equal(0, audit.Calls);
    }

    private static void Check(RequestIntentException error, string code, string field, string kind)
    {
        Assert.Equal(code, error.Code);
        Assert.Equal(field, error.Field);
        Assert.Equal(kind, error.RequestKind);
        Assert.DoesNotContain("SECRET-CANARY", error.Message);
    }

    private sealed class CountingPolicy : IRequestPolicy
    {
        public int Calls;
        public SelectQuery Apply(SelectQuery query) { Calls++; return query; }
    }

    private sealed class CountingAudit : IAppAuditEventSink
    {
        public int Calls;
        public Task RecordAsync(IReadOnlyDictionary<string, object?> item, CancellationToken token = default)
        { Calls++; return Task.CompletedTask; }
    }

    private sealed class CountingProvider : ITransactionExecutor
    {
        public int Calls;
        public DataServiceCapabilities Capabilities { get; } = new() { Transaction = true };
        public Task<QueryResult> QueryAsync(QueryRequest request)
        { Calls++; throw new InvalidOperationException("unexpected provider query"); }
        public Task<MutationResult> MutateAsync(MutationRequest request)
        { Calls++; throw new InvalidOperationException("unexpected provider mutation"); }
        public Task<ITransaction> BeginTransactionAsync()
        { Calls++; throw new InvalidOperationException("unexpected transaction allocation"); }
    }
}
