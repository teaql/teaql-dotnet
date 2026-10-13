namespace TeaQL.Telemetry.OpenTelemetry;

public sealed record TelemetryQueueState(long Offered, long Admitted, long Completed, long Retired,
    long Overflow, long PostShutdown, long HandoffFailed, int Outstanding, int Exporting, int PeakOutstanding);

// A bounded identity ledger, not another payload queue. The SDK still owns
// snapshots, batching and its queue. Only nonblocking handoff runs under _gate.
internal sealed class Admission(string signal, ExportHealthState health, int capacity = 2048)
{
    private readonly object _gate = new();
    private readonly Dictionary<string, Entry> _pending = new();
    private sealed class Entry(long sequence) { internal readonly long Sequence = sequence; internal bool Exporting; }
    private sealed record Waiter(long Prefix, TaskCompletionSource<bool> Completion);
    private readonly List<Waiter> _waiters = new();
    private long _offered, _admitted, _completed, _retired, _overflow, _late, _handoffFailed;
    private int _peak;
    private bool _closed;
    internal long Prefix { get { lock (_gate) return _admitted; } }
    internal TelemetryQueueState Snapshot {
        get { lock (_gate) return new(_offered, _admitted, _completed, _retired, _overflow, _late, _handoffFailed,
            _pending.Count, _pending.Values.Count(row => row.Exporting), _peak); }
    }
    internal bool Submit(string identity, Func<bool> handoff)
    {
        string? reason = null;
        lock (_gate) {
            _offered++;
            if (_closed) { _late++; reason = "shutdown"; }
            else if (_pending.ContainsKey(identity)) { _handoffFailed++; reason = "handoff_failure"; }
            else if (_pending.Count >= capacity) { _overflow++; reason = "queue_overflow"; }
            else {
                _pending[identity] = new(_admitted + 1);
                bool accepted; try { accepted = handoff(); } catch { accepted = false; }
                if (accepted) { _admitted++; _peak = Math.Max(_peak, _pending.Count); }
                else { _pending.Remove(identity); _handoffFailed++; reason = "handoff_failure"; }
            }
        }
        if (reason != null) health.QueueLoss(signal, reason, 1);
        return reason == null;
    }
    internal bool BeginExport(string identity)
    {
        lock (_gate) {
            if (!_pending.TryGetValue(identity, out var entry) || entry.Exporting) return false;
            entry.Exporting = true; return true;
        }
    }
    internal void Complete(IEnumerable<string> identities)
    {
        List<Waiter> ready;
        lock (_gate) {
            foreach (var identity in identities)
                if (_pending.TryGetValue(identity, out var entry) && entry.Exporting && _pending.Remove(identity)) _completed++;
            ready = ReadyLocked();
        }
        foreach (var waiter in ready) waiter.Completion.TrySetResult(true);
    }
    internal Task<bool> Observe(long prefix, TimeSpan budget)
    {
        Waiter waiter;
        lock (_gate) {
            if (!_pending.Values.Any(entry => entry.Sequence <= prefix)) return Task.FromResult(true);
            waiter = new(prefix, new(TaskCreationOptions.RunContinuationsAsynchronously)); _waiters.Add(waiter);
        }
        return ExpireAsync(waiter, budget);
    }
    private async Task<bool> ExpireAsync(Waiter waiter, TimeSpan budget)
    {
        try { return await waiter.Completion.Task.WaitAsync(budget > TimeSpan.Zero ? budget : TimeSpan.Zero).ConfigureAwait(false); }
        catch (TimeoutException) { return false; }
        finally { lock (_gate) _waiters.Remove(waiter); }
    }
    private List<Waiter> ReadyLocked()
    {
        var ready = _waiters.Where(waiter => !_pending.Values.Any(entry => entry.Sequence <= waiter.Prefix)).ToList();
        foreach (var waiter in ready) _waiters.Remove(waiter); return ready;
    }
    internal void Close() { lock (_gate) _closed = true; }
    internal void RecordLateOffer() {
        lock (_gate) { _offered++; _late++; }
        health.QueueLoss(signal, "shutdown", 1);
    }
    internal void RetireQueued()
    {
        int count; List<Waiter> ready;
        lock (_gate) {
            _closed = true;
            var queued = _pending.Where(row => !row.Value.Exporting).Select(row => row.Key).ToArray();
            count = queued.Length; foreach (var identity in queued) _pending.Remove(identity); _retired += count; ready = ReadyLocked();
        }
        health.QueueLoss(signal, "shutdown", count);
        foreach (var waiter in ready) waiter.Completion.TrySetResult(true);
    }
}
