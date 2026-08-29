using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TeaQL.Runtime;

public sealed record RetainedIdSet(string QueryKey, ulong[] Ids, DateTimeOffset ExpiresAt);

public interface IIdSetStore
{
    ValueTask<RetainedIdSet?> GetAsync(string queryKey);
    ValueTask PutAsync(RetainedIdSet retained);
    ValueTask InvalidateAsync(string queryKey);
}

public sealed class InMemoryIdSetStore(int maxEntries = 64, long maxBytes = 256L << 20)
    : IIdSetStore
{
    private readonly object _gate = new();
    private readonly Dictionary<string, RetainedIdSet> _sets = new();

    public ValueTask<RetainedIdSet?> GetAsync(string queryKey)
    {
        lock (_gate)
        {
            if (!_sets.TryGetValue(queryKey, out var retained))
                return ValueTask.FromResult<RetainedIdSet?>(null);
            if (retained.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _sets.Remove(queryKey);
                return ValueTask.FromResult<RetainedIdSet?>(null);
            }
            return ValueTask.FromResult<RetainedIdSet?>(retained with
            {
                Ids = (ulong[])retained.Ids.Clone()
            });
        }
    }

    public ValueTask PutAsync(RetainedIdSet retained)
    {
        var bytes = checked((long)retained.Ids.LongLength * sizeof(ulong));
        if (bytes > maxBytes) throw new InvalidOperationException("Retained ID set exceeds memory ceiling");
        lock (_gate)
        {
            while (_sets.Count > 0 && (_sets.Count >= maxEntries ||
                   _sets.Values.Sum(value => value.Ids.LongLength * sizeof(ulong)) + bytes > maxBytes))
            {
                var oldest = _sets.MinBy(pair => pair.Value.ExpiresAt).Key;
                _sets.Remove(oldest);
            }
            _sets[retained.QueryKey] = retained with { Ids = (ulong[])retained.Ids.Clone() };
        }
        return ValueTask.CompletedTask;
    }

    public ValueTask InvalidateAsync(string queryKey)
    {
        lock (_gate) _sets.Remove(queryKey);
        return ValueTask.CompletedTask;
    }
}

internal static class IdSetBuildLocks
{
    private sealed class Entry
    {
        public readonly SemaphoreSlim Gate = new(1, 1);
        public int References;
    }

    private static readonly ConcurrentDictionary<string, Entry> Entries = new();

    public static async ValueTask<IDisposable> AcquireAsync(string key)
    {
        var entry = Entries.AddOrUpdate(key,
            _ => new Entry { References = 1 },
            (_, existing) => { Interlocked.Increment(ref existing.References); return existing; });
        await entry.Gate.WaitAsync().ConfigureAwait(false);
        return new Releaser(key, entry);
    }

    private sealed class Releaser(string key, Entry entry) : IDisposable
    {
        public void Dispose()
        {
            entry.Gate.Release();
            if (Interlocked.Decrement(ref entry.References) == 0)
                Entries.TryRemove(new KeyValuePair<string, Entry>(key, entry));
        }
    }
}
