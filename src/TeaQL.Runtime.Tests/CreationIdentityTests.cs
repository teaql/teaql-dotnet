using TeaQL.DataService;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class CreationIdentityTests
{
    private class Provider : IDataService
    {
        public DataServiceCapabilities Capabilities => new();
        public Task<QueryResult> QueryAsync(QueryRequest request) => throw new Exception("Unexpected business query");
        public Task<MutationResult> MutateAsync(MutationRequest request) => throw new Exception("Unexpected business mutation");
    }

    private sealed class Allocator(Func<Task<ulong>> allocate) : Provider, IInternalIdAllocator
    {
        public List<string> Calls { get; } = new();
        public Task<ulong> NextIdAsync(string entity) { Calls.Add(entity); return allocate(); }
        public Task EnsureIdFloorAsync(string entity, ulong floor) => throw new Exception("Reservation must not synchronize a floor");
    }

    [Fact]
    public async Task ExplicitAllocatorWorksWithoutABusinessProvider()
    {
        var allocator = new Allocator(() => Task.FromResult(701UL));
        var context = new UserContext().WithInternalIdAllocator(allocator);
        Assert.Equal(701UL, await context.NextIdAsync("PurchaseOrder"));
        Assert.Equal(new[] { "PurchaseOrder" }, allocator.Calls);
        Assert.Empty(context.ManagedEntities);
    }

    [Fact]
    public async Task DefaultAllocatorIsResolvedBehindTheRuntimeDecorator()
    {
        var allocator = new Allocator(() => Task.FromResult(ulong.MaxValue));
        var context = new UserContext().WithDataService(allocator);
        Assert.Equal(ulong.MaxValue, await context.NextIdAsync("OrderItem"));
        Assert.Equal(new[] { "OrderItem" }, allocator.Calls);
    }

    [Fact]
    public async Task AllocationFailureIsNotReplacedAndRecoveryUsesTheSameAllocator()
    {
        var failure = new InvalidOperationException("Controlled allocation failure");
        var count = 0;
        var allocator = new Allocator(() => ++count == 1 ? Task.FromException<ulong>(failure) : Task.FromResult(702UL));
        var fallback = new Allocator(() => Task.FromResult(999UL));
        var context = new UserContext().WithInternalIdAllocator(allocator).WithDataService(fallback);
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => context.NextIdAsync("PurchaseOrder")));
        Assert.Empty(context.ManagedEntities);
        Assert.Equal(702UL, await context.NextIdAsync("PurchaseOrder"));
        Assert.Empty(fallback.Calls);
        Assert.Equal(2, allocator.Calls.Count);
    }

    [Fact]
    public async Task ZeroDoesNotReturnAnIdentityOrUseTheFallback()
    {
        var allocator = new Allocator(() => Task.FromResult(0UL));
        var fallback = new Allocator(() => Task.FromResult(999UL));
        var context = new UserContext().WithDataService(fallback).WithInternalIdAllocator(allocator);
        Assert.Contains("zero", (await Assert.ThrowsAsync<InvalidOperationException>(() => context.NextIdAsync("PurchaseOrder"))).Message);
        Assert.Empty(fallback.Calls);
        Assert.Empty(context.ManagedEntities);
    }

    [Fact]
    public async Task PendingAllocationRemainsAsynchronous()
    {
        var completion = new TaskCompletionSource<ulong>(TaskCreationOptions.RunContinuationsAsynchronously);
        var context = new UserContext().WithInternalIdAllocator(new Allocator(() => completion.Task));
        var pending = context.NextIdAsync("PurchaseOrder");
        Assert.False(pending.IsCompleted);
        Assert.Empty(context.ManagedEntities);
        completion.SetResult(703UL);
        Assert.Equal(703UL, await pending);
    }

    [Fact]
    public async Task CancellationDoesNotFallBack()
    {
        var cancellation = new OperationCanceledException("Controlled cancellation");
        var fallback = new Allocator(() => Task.FromResult(999UL));
        var context = new UserContext().WithInternalIdAllocator(new Allocator(() => Task.FromException<ulong>(cancellation))).WithDataService(fallback);
        Assert.Same(cancellation, await Assert.ThrowsAsync<OperationCanceledException>(() => context.NextIdAsync("PurchaseOrder")));
        Assert.Empty(fallback.Calls);
        Assert.Empty(context.ManagedEntities);
    }

    [Fact]
    public async Task InvalidNamesAreRejectedBeforeAllocatorEntry()
    {
        var allocator = new Allocator(() => Task.FromResult(701UL));
        var context = new UserContext().WithInternalIdAllocator(allocator);
        foreach (var name in new string?[] { null, "", " " })
            await Assert.ThrowsAsync<ArgumentException>(() => context.NextIdAsync(name!));
        Assert.Empty(allocator.Calls);
    }

    [Fact]
    public async Task MissingAllocationCapabilityFailsClosed()
    {
        var context = new UserContext().WithDataService(new Provider());
        await Assert.ThrowsAsync<NotSupportedException>(() => context.NextIdAsync("PurchaseOrder"));
        Assert.Empty(context.ManagedEntities);
    }
}
