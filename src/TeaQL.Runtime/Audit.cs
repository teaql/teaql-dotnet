using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public interface IRawAuditEventSink
{
    Task OnEventAsync(UserContext context, RawAuditEvent auditEvent);
}

public interface IAppAuditEventSink
{
    Task OnSafeEventAsync(UserContext context, SafeAuditEvent auditEvent);
}

public sealed class AuditedDataService : IDataService
{
    private readonly IDataService _inner;
    private readonly UserContext _context;

    public AuditedDataService(IDataService inner, UserContext context)
    {
        _inner = inner;
        _context = context;
    }

    public DataServiceCapabilities Capabilities => _inner.Capabilities;
    public Task<QueryResult> QueryAsync(QueryRequest request) => _inner.QueryAsync(request);

    public async Task<MutationResult> MutateAsync(MutationRequest request)
    {
        var result = await _inner.MutateAsync(request);
        if (result.AffectedRows > 0)
            await _context.EmitMutationAuditAsync(request, result);
        return result;
    }
}
