using System.Collections.Generic;
using System.Threading.Tasks;
using TeaQL.DataService;

namespace TeaQL.Runtime;

/// <summary>
/// Adds TeaQL semantic operation spans around a provider data service. Provider
/// internals remain independently instrumentable and become children through
/// the active OpenTelemetry context.
/// </summary>
public sealed class RuntimeDataService : IDataService
{
    private readonly IDataService _provider;
    private readonly UserContext _context;

    public RuntimeDataService(IDataService provider, UserContext context)
    {
        _provider = provider;
        _context = context;
    }

    public DataServiceCapabilities Capabilities => _provider.Capabilities;

    public Task<QueryResult> QueryAsync(QueryRequest request) =>
        _context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("query", $"{request.Query.Entity}.list",
                new Dictionary<string, object> { ["teaql.entity.type"] = request.Query.Entity }),
            () => ObserveProviderQueryAsync(request),
            result => new Dictionary<string, object>
            {
                ["teaql.result.cardinality"] = result.Rows.Count
            });

    public Task<MutationResult> MutateAsync(MutationRequest request)
    {
        var entity = EntityName(request);
        return _context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("mutation", $"{entity}.mutate",
                new Dictionary<string, object> { ["teaql.entity.type"] = entity }),
            () => ObserveProviderMutationAsync(request),
            result => new Dictionary<string, object>
            {
                ["teaql.result.cardinality"] = result.AffectedRows
            });
    }

    private Task<QueryResult> ObserveProviderQueryAsync(QueryRequest request) =>
        _context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("provider", "data-service.query"),
            () => _provider.QueryAsync(request),
            result => new Dictionary<string, object>
            {
                ["teaql.result.cardinality"] = result.Rows.Count
            });

    private Task<MutationResult> ObserveProviderMutationAsync(MutationRequest request) =>
        _context.RuntimeTelemetry.ObserveAsync(
            RuntimeOperation.Create("provider", "data-service.mutate"),
            () => _provider.MutateAsync(request),
            result => new Dictionary<string, object>
            {
                ["teaql.result.cardinality"] = result.AffectedRows
            });

    private static string EntityName(MutationRequest request) => request switch
    {
        InsertMutationRequest insert => insert.Command.Entity,
        UpdateMutationRequest update => update.Command.Entity,
        DeleteMutationRequest delete => delete.Command.Entity,
        RecoverMutationRequest recover => recover.Command.Entity,
        BatchMutationRequest => "batch",
        _ => "unknown"
    };
}
