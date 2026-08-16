using Xunit;

namespace TeaQL.Runtime.Tests;

public class UserContextEntityCreationTests
{
    private sealed class SpecialOrder
    {
        public string? Tenant { get; set; }
        public string DomainMethod() => "special-order";
    }

    [Fact]
    public void InitializesAndTracksConcreteEntity()
    {
        var context = new UserContext()
            .RegisterEntityInitializer("SpecialOrder", (_, entity) =>
                ((SpecialOrder)entity).Tenant = "trusted");

        var order = context.InitializeEntity("SpecialOrder", new SpecialOrder());

        Assert.IsType<SpecialOrder>(order);
        Assert.Equal("special-order", order.DomainMethod());
        Assert.Equal("trusted", order.Tenant);
        Assert.Contains(order, context.ManagedEntities);
    }
}
