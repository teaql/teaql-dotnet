namespace TeaQL.Runtime;

/// <summary>
/// One synchronous standalone Checker/Fix invocation. The execution-local stack
/// is not a mutation ledger or a Context-wide graph. Batch leaves share their
/// caller's invocation; a nested independent request opens its own one.
/// </summary>
internal sealed class CheckerFixInvocation : IDisposable
{
    private static readonly AsyncLocal<CheckerFixInvocation?> Active = new();
    private readonly UserContext _context;
    private readonly CheckerFixInvocation? _parent;
    private readonly List<FixEvidence> _evidence = new();
    private DateTimeOffset? _now;
    private bool _closed;

    private CheckerFixInvocation(UserContext context)
    {
        _context = context; _parent = Active.Value; Active.Value = this;
    }

    internal static CheckerFixInvocation Open(UserContext context) => new(context);

    internal static CheckerFixInvocation? Current(UserContext context)
    {
        for (var item = Active.Value; item != null; item = item._parent)
        {
            // A fork may retain a closed child while its original parent still
            // executes elsewhere. Never tunnel through that expired boundary.
            if (item._closed) return null;
            if (ReferenceEquals(item._context, context)) return item;
        }
        return null;
    }

    // Preserve admission-before-clock ordering, including rejected requests and
    // batches with no Checker. Merely opening a scope does not read the clock.
    internal DateTimeOffset Now
    {
        get { _now ??= _context.BusinessTime; return _now.Value; }
    }
    internal void Record(FixEvidence evidence) => _evidence.Add(evidence);

    public void Dispose()
    {
        if (_closed) return;
        if (!ReferenceEquals(Active.Value, this))
            throw new InvalidOperationException("Checker/Fix invocations must close in reverse execution order");
        try
        {
            if (_now != null || _evidence.Count != 0)
                _context.CompleteStandaloneFixEvidence(Array.AsReadOnly(_evidence.ToArray()));
        }
        finally
        {
            _evidence.Clear(); _closed = true; Active.Value = _parent;
        }
    }
}
