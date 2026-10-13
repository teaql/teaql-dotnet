namespace TeaQL.DataService;

public sealed record SqlExecutionCorrelation(string TraceId, string SpanId);
public interface ISqlExecutionObservation : IDisposable
{
    SqlExecutionCorrelation? Correlation { get; }
    void Success();
    void Failure(Exception error);
}

internal static class SqlExecutionObservation
{
    internal static ISqlExecutionObservation Start(object request, string backend, DataServiceOperation operation)
    {
        var factory = request is QueryRequest query ? query.StatementObserver : (request as MutationRequest)?.StatementObserver;
        if (factory == null) return Noop.Instance;
        try { return new FailOpen(factory(backend, operation)); }
        catch (Exception error) when (error is not OutOfMemoryException) { return Noop.Instance; }
    }
    private sealed class Noop : ISqlExecutionObservation
    {
        internal static readonly Noop Instance = new();
        public SqlExecutionCorrelation? Correlation => null;
        public void Success() { }
        public void Failure(Exception error) { }
        public void Dispose() { }
    }
    private sealed class FailOpen(ISqlExecutionObservation? scope) : ISqlExecutionObservation
    {
        public SqlExecutionCorrelation? Correlation { get { try { return scope?.Correlation; } catch { return null; } } }
        public void Success() { try { scope?.Success(); } catch { } }
        public void Failure(Exception error) { try { scope?.Failure(error); } catch { } }
        public void Dispose() { try { scope?.Dispose(); } catch { } }
    }
}
