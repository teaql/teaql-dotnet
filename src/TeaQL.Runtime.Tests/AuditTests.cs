using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime.Tests;

public class AuditTests
{
    private sealed class RawSink : IRawAuditEventSink
    {
        public List<RawAuditEvent> Events { get; } = new();
        public Task OnEventAsync(UserContext context, RawAuditEvent auditEvent) { Events.Add(auditEvent); return Task.CompletedTask; }
    }
    private sealed class AppSink : IAppAuditEventSink
    {
        public List<SafeAuditEvent> Events { get; } = new();
        public Task OnSafeEventAsync(UserContext context, SafeAuditEvent auditEvent) { Events.Add(auditEvent); return Task.CompletedTask; }
    }
    private sealed class Service : IDataService
    {
        public DataServiceCapabilities Capabilities { get; } = new() { Mutation = true };
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new NotSupportedException();
        public Task<MutationResult> MutateAsync(MutationRequest request) => Task.FromResult(new MutationResult { AffectedRows = 1 });
    }

    [Fact]
    public async Task SuccessfulMutationEmitsRawAndIndependentMaskedAppEvent()
    {
        var raw = new RawSink();
        var app = new AppSink();
        var descriptor = EntityDescriptor.New("User").AuditMaskFields(new List<string> { "email" });
        var context = new RuntimeModule().Entity(descriptor).AuditEventSink(raw).IntoContext().WithAppAuditEventSink(app);
        var command = new InsertCommand("User").Value("email", new Value.TextValue("person@example.invalid"));
        command.TraceChain.Add(new TraceNode("User", null, "approved change"));

        await context.Audit(new Service()).MutateAsync(new InsertMutationRequest(command));

        Assert.Single(raw.Events);
        Assert.Equal("person@example.invalid", Assert.IsType<Value.TextValue>(raw.Events[0].Changes[0].NewValue).Value);
        Assert.Equal("approved change", raw.Events[0].TraceChain[0].Comment);
        Assert.Single(app.Events);
        Assert.True(app.Events[0].Fields[0].Masked);
        Assert.NotEqual("person@example.invalid", app.Events[0].Fields[0].Value);
    }
}
