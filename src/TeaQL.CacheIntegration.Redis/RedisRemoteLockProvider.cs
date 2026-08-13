using System;
using StackExchange.Redis;
using TeaQL.Runtime;

namespace TeaQL.CacheIntegration.Redis;

public class RedisRemoteLockProvider : IRemoteLockProvider
{
    private readonly IDatabase _database;
    private readonly string _token;

    public RedisRemoteLockProvider(IConnectionMultiplexer redis)
    {
        _database = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
        _token = Guid.NewGuid().ToString();
    }

    public bool TryLock(string key, long timeoutMillis, long expireMillis)
    {
        var expiry = TimeSpan.FromMilliseconds(expireMillis);
        var timeout = TimeSpan.FromMilliseconds(timeoutMillis);

        var end = DateTime.UtcNow.Add(timeout);
        do
        {
            if (_database.StringSet(key, _token, expiry, When.NotExists))
            {
                return true;
            }
            if (DateTime.UtcNow >= end)
            {
                break;
            }
            System.Threading.Thread.Sleep(50);
        } while (true);

        return false;
    }

    public void Unlock(string key)
    {
        var val = _database.StringGet(key);
        if (val.HasValue && val == _token)
        {
            _database.KeyDelete(key);
        }
    }
}
