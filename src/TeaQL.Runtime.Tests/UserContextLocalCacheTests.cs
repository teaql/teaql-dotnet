using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TeaQL.Runtime;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class UserContextLocalCacheTests
{
    [Fact]
    public async Task LocalCacheIsProcessSharedAndHonorsTtl()
    {
        var first = new UserContext();
        var second = new UserContext();
        var key = $"local-cache-{Guid.NewGuid()}";

        first.PutToLocalCache(key, "value");
        Assert.Equal("value", second.GetFromLocalCache<string>(key));
        second.RemoveFromLocalCache(key);
        Assert.Null(first.GetFromLocalCache<string>(key));

        first.PutToLocalCache(key, "temporary", 1);
        await Task.Delay(1100);
        Assert.Null(second.GetFromLocalCache<string>(key));
    }

    [Fact]
    public async Task LocalLockEnforcesOwnershipTimeoutAndLeaseExpiry()
    {
        var first = new UserContext();
        var second = new UserContext();
        var key = $"local-lock-{Guid.NewGuid()}";

        Assert.True(first.TryLocalLock(key, 0, 50));
        Assert.False(second.TryLocalLock(key, 0, 50));
        second.UnlockLocal(key);
        Assert.False(second.TryLocalLock(key, 0, 50));
        await Task.Delay(60);
        Assert.True(second.TryLocalLock(key, 0, 50));
        second.UnlockLocal(key);
        Assert.True(first.TryLocalLock(key, 0, 50));
        first.UnlockLocal(key);
    }


    [Fact]
    public void RemoteCacheEmitsHitMissAndRethrowsOriginalProviderFailure()
    {
        var telemetry = new CacheRecordingTelemetry();
        var provider = new StubRemoteCacheProvider();
        var context = new UserContext().WithRuntimeTelemetry(telemetry);
        context.InsertResource<IRemoteCacheProvider>(provider);

        context.PutToRemoteCache("sensitive-key", "value");
        Assert.Equal("value", context.GetFromRemoteCache<string>("sensitive-key"));
        context.RemoveFromRemoteCache("sensitive-key");
        Assert.Null(context.GetFromRemoteCache<string>("sensitive-key"));
        Assert.Equal(new[] { "stored", "hit", "removed", "miss" }, telemetry.Results);
        Assert.All(telemetry.Operations, operation =>
        {
            Assert.Equal("cache", operation.Family);
            Assert.DoesNotContain(operation.Attributes,
                pair => Equals(pair.Value, "sensitive-key"));
        });

        var original = new InvalidOperationException("remote cache unavailable");
        provider.Failure = original;
        Assert.Same(original, Assert.Throws<InvalidOperationException>(
            () => context.GetFromRemoteCache<string>("key")));
        Assert.Same(original, telemetry.LastFailure);
    }

    private sealed class StubRemoteCacheProvider : IRemoteCacheProvider
    {
        private object? _value;
        public Exception? Failure { get; set; }
        public void Put(string key, object value, int? timeToLiveInSeconds = null) => _value = value;
        public T? Get<T>(string key)
        {
            if (Failure is not null) throw Failure;
            return _value is T value ? value : default;
        }
        public void Remove(string key) => _value = null;
    }

    private sealed class CacheRecordingTelemetry : IRuntimeTelemetry
    {
        public List<RuntimeOperation> Operations { get; } = new();
        public List<string> Results { get; } = new();
        public Exception? LastFailure { get; private set; }
        public IRuntimeTelemetryScope Start(RuntimeOperation operation)
        {
            Operations.Add(operation);
            return new Scope(this);
        }

        private sealed class Scope(CacheRecordingTelemetry owner) : IRuntimeTelemetryScope
        {
            public void Success(IReadOnlyDictionary<string, object>? attributes = null)
            {
                if (attributes?.TryGetValue("teaql.cache.result", out var result) == true)
                    owner.Results.Add((string)result);
            }
            public void Failure(Exception error) => owner.LastFailure = error;
        }
    }
}
