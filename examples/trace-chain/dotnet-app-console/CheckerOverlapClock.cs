using TeaQL.Runtime;

// Test-only admission barrier: rejected graphs deliberately never reach Begin.
sealed class CheckerOverlapClock(IBusinessClock original) : IBusinessClock, IDisposable
{
    private readonly ManualResetEventSlim _release = new(false);
    private int _reads;
    public TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public DateTimeOffset Now
    {
        get
        {
            if (Interlocked.Increment(ref _reads) == 1)
            {
                Entered.TrySetResult();
                if (!_release.Wait(TimeSpan.FromSeconds(15)))
                    throw new TimeoutException("Checker overlap admission barrier was not released");
            }
            return original.Now;
        }
    }
    public void Release() => _release.Set();
    public void Dispose() { _release.Set(); _release.Dispose(); }
}
