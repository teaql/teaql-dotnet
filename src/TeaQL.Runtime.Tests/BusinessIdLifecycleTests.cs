using TeaQL.Core;
using Xunit;

namespace TeaQL.Runtime.Tests;

public class BusinessIdLifecycleTests
{
    private sealed class Slot(bool isNew, string? value = null) : IBusinessIdSlot
    {
        public string? CurrentValue { get; private set; } = value;
        public bool IsNewAggregate { get; } = isNew;
        public void AssignCanonicalValue(string assigned) => CurrentValue = assigned;
    }

    private static UserContext Context(IBusinessIdAllocator allocator)
    {
        var key = new BusinessIdEncodingKey(1,
            Enumerable.Range(0, 32).Select(value => (byte)value).ToArray());
        return new UserContext()
            .WithBusinessClock(new FixedBusinessClock(
                new DateTimeOffset(2026, 10, 1, 8, 30, 0, TimeSpan.Zero)))
            .WithBusinessIdKeyProvider(new StaticBusinessIdKeyProvider(key))
            .WithBusinessIdProfileFactory(new DefaultBusinessIdProfileFactory())
            .WithBusinessIdService(new DefaultBusinessIdService(allocator));
    }

    [Fact]
    public async Task ContextOwnedLifecycleIsRetrySafeAndUsesBusinessDate()
    {
        var context = Context(new InMemoryBusinessIdAllocator());
        var definition = BusinessIdDefinition.DailyPermuted(
            "order_number", "ORD", "order_number");
        var slot = new Slot(true);

        var first = await context.EnsureBusinessIdAsync(
            definition, "commerce", "commerce_order", slot);
        var retry = await context.EnsureBusinessIdAsync(
            definition, "commerce", "commerce_order", slot);
        var following = await context.EnsureBusinessIdAsync(
            definition, "commerce", "commerce_order", new Slot(true));

        Assert.Equal(first, retry);
        Assert.Equal(first.Value, slot.CurrentValue);
        Assert.StartsWith("ORD-20261001-", first.Value);
        Assert.NotEqual(first.Value, following.Value);
    }

    [Fact]
    public async Task EstablishedAggregateCannotAcquireMissingBusinessId()
    {
        var context = Context(new InMemoryBusinessIdAllocator());
        var error = await Assert.ThrowsAsync<BusinessIdException>(() =>
            context.EnsureBusinessIdAsync(
                BusinessIdDefinition.DailyPermuted(
                    "order_number", "ORD", "order_number"),
                "commerce", "commerce_order", new Slot(false)));
        Assert.Equal(BusinessIdErrorCode.BusinessIdImmutable, error.Code);
    }
}
