using System;
using System.Text.Json;
using StackExchange.Redis;
using TeaQL.Runtime;

namespace TeaQL.CacheIntegration.Redis;

public class RedisRemoteCacheProvider : IRemoteCacheProvider
{
    private readonly IDatabase _database;

    public RedisRemoteCacheProvider(IConnectionMultiplexer redis)
    {
        _database = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
    }

    public void Put(string key, object value, int? timeToLiveInSeconds = null)
    {
        var serialized = JsonSerializer.Serialize(value);
        if (timeToLiveInSeconds.HasValue)
        {
            _database.StringSet(key, serialized, TimeSpan.FromSeconds(timeToLiveInSeconds.Value));
        }
        else
        {
            _database.StringSet(key, serialized);
        }
    }

    public T? Get<T>(string key)
    {
        var cached = _database.StringGet(key);
        if (cached.HasValue)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(cached.ToString()!);
            }
            catch (JsonException)
            {
                return default;
            }
        }
        return default;
    }

    public void Remove(string key)
    {
        _database.KeyDelete(key);
    }
}
