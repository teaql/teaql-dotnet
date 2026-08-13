using System.Collections.Concurrent;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public sealed class InMemoryContinuousPageCursorStore : IContinuousPageCursorStore
{
    private readonly ConcurrentDictionary<string, ContinuousPageCursor> _cursors = new();
    private readonly int _maxEntries;

    public InMemoryContinuousPageCursorStore(int maxEntries = 4096)
    {
        if (maxEntries <= 0) throw new ArgumentOutOfRangeException(nameof(maxEntries));
        _maxEntries = maxEntries;
    }

    public Task<ContinuousPageCursor?> GetAsync(string queryKey, ulong targetOffset)
    {
        var key = Key(queryKey, targetOffset);
        if (!_cursors.TryGetValue(key, out var cursor)) return Task.FromResult<ContinuousPageCursor?>(null);
        if (cursor.ExpiresAt > DateTimeOffset.UtcNow) return Task.FromResult<ContinuousPageCursor?>(cursor);
        _cursors.TryRemove(key, out _);
        return Task.FromResult<ContinuousPageCursor?>(null);
    }

    public Task PutAsync(ContinuousPageCursor cursor)
    {
        if (_cursors.Count >= _maxEntries)
        {
            var oldest = _cursors.OrderBy(pair => pair.Value.ExpiresAt).FirstOrDefault();
            if (!string.IsNullOrEmpty(oldest.Key)) _cursors.TryRemove(oldest.Key, out _);
        }
        _cursors[Key(cursor.QueryKey, cursor.NextOffset)] = cursor;
        return Task.CompletedTask;
    }

    public Task InvalidateAsync(string queryKey)
    {
        foreach (var key in _cursors.Keys.Where(key => key.StartsWith(queryKey + ":", StringComparison.Ordinal)))
            _cursors.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    private static string Key(string queryKey, ulong offset) => $"{queryKey}:{offset}";
}
