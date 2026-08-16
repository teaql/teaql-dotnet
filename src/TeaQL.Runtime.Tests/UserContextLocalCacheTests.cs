using System;
using System.Threading.Tasks;
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
}
