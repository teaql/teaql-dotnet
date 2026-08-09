using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using TeaQL.DataService;

namespace TeaQL.CacheIntegration.Redis;

public class RedisDataServiceDecorator : IDataService
{
    private readonly IDataService _innerService;
    private readonly IDatabase _redisDatabase;
    private readonly TimeSpan _cacheDuration;

    public RedisDataServiceDecorator(IDataService innerService, IConnectionMultiplexer redisConnection, TimeSpan cacheDuration)
    {
        _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
        _redisDatabase = redisConnection?.GetDatabase() ?? throw new ArgumentNullException(nameof(redisConnection));
        _cacheDuration = cacheDuration;
    }

    public DataServiceCapabilities Capabilities => _innerService.Capabilities;

    public async Task<QueryResult> QueryAsync(QueryRequest request)
    {
        var requestJson = JsonSerializer.Serialize(request);
        var cacheKey = $"teaql:cache:{GenerateHash(requestJson)}";

        var cachedValue = await _redisDatabase.StringGetAsync(cacheKey);
        if (cachedValue.HasValue)
        {
            try
            {
                var result = JsonSerializer.Deserialize<QueryResult>(cachedValue.ToString());
                if (result != null)
                {
                    return result;
                }
            }
            catch (JsonException)
            {
                // Ignore deserialization errors and fetch from database
            }
        }

        var queryResult = await _innerService.QueryAsync(request);

        var serializedResult = JsonSerializer.Serialize(queryResult);
        await _redisDatabase.StringSetAsync(cacheKey, serializedResult, _cacheDuration);

        return queryResult;
    }

    public async Task<MutationResult> MutateAsync(MutationRequest request)
    {
        return await _innerService.MutateAsync(request);
    }

    private static string GenerateHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
