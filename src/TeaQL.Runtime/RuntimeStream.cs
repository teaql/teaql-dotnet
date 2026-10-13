using System.Runtime.CompilerServices;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

// Lifecycle belongs to runtime, not generated model-specific SDK code. Each
// cold enumeration owns its counters, cursor and one root; activation is local
// to iterator execution so suspended streams cannot parent unrelated queries.
internal static class RuntimeStream
{
    internal static async IAsyncEnumerable<T> Entities<T>(IRuntimeTelemetry telemetry, QueryRequest template,
        Func<Record, T> materialize, Func<QueryRequest, CancellationToken, IAsyncEnumerable<StreamChunk>> open,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var request = template.WithQuery(template.Query); var delivery = new StreamDelivery(); request.StreamDelivery = delivery;
        var scope = telemetry.StartSafely(Operation(request));
        IAsyncEnumerator<StreamChunk>? cursor = null; bool complete = false;
        try
        {
            try { cursor = open(request, cancellationToken).GetAsyncEnumerator(cancellationToken); }
            catch (Exception error) { delivery.Error = error; throw; }
            while (await MoveNext(cursor, delivery, cancellationToken).ConfigureAwait(false))
            {
                foreach (var row in cursor.Current.Rows)
                {
                    T entity;
                    try { cancellationToken.ThrowIfCancellationRequested(); entity = materialize(row); }
                    catch (Exception error) { delivery.Error = error; throw; }
                    delivery.Delivered(); yield return entity;
                }
            }
            complete = true;
        }
        finally
        {
            try { if (cursor != null) await cursor.DisposeAsync().ConfigureAwait(false); }
            catch (Exception error) { if (delivery.Error == null) { delivery.Error = error; throw; } }
            finally { Finish(scope, delivery.Count, complete, delivery.Error); }
        }
    }

    internal static async IAsyncEnumerable<StreamChunk> Chunks(IRuntimeTelemetry telemetry, QueryRequest template,
        Func<QueryRequest, CancellationToken, IAsyncEnumerable<StreamChunk>> open,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var request = template.WithQuery(template.Query); var terminal = new StreamDelivery();
        var scope = telemetry.StartSafely(Operation(request));
        IAsyncEnumerator<StreamChunk>? cursor = null; bool complete = false; int delivered = 0;
        try
        {
            try { cursor = open(request, cancellationToken).GetAsyncEnumerator(cancellationToken); }
            catch (Exception error) { terminal.Error = error; throw; }
            while (await MoveNext(cursor, terminal, cancellationToken).ConfigureAwait(false))
            { delivered += cursor.Current.Rows.Count; yield return cursor.Current; }
            complete = true;
        }
        finally
        {
            try { if (cursor != null) await cursor.DisposeAsync().ConfigureAwait(false); }
            catch (Exception error) { if (terminal.Error == null) { terminal.Error = error; throw; } }
            finally { Finish(scope, delivered, complete, terminal.Error); }
        }
    }

    private static async Task<bool> MoveNext(IAsyncEnumerator<StreamChunk> cursor, StreamDelivery terminal, CancellationToken cancellationToken)
    {
        try { cancellationToken.ThrowIfCancellationRequested(); return await cursor.MoveNextAsync().ConfigureAwait(false); }
        catch (Exception error) { terminal.Error = error; throw; }
    }
    private static RuntimeOperation Operation(QueryRequest request) => RuntimeOperation.Create("query", request.Query.Entity + ".stream",
        new Dictionary<string, object> { ["teaql.entity.type"] = request.Query.Entity });
    private static void Finish(IRuntimeTelemetryScope scope, int delivered, bool complete, Exception? error)
    {
        var attributes = new Dictionary<string, object> { ["teaql.result.cardinality"] = delivered };
        if (complete && error == null) scope.Success(attributes);
        else scope.Failure(error ?? new OperationCanceledException(), attributes);
    }
}
