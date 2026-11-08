using Xunit;

namespace TeaQL.Runtime.Tests;

public class UserContextEntityCreationTests
{
    [Fact]
    public void ValidatesAfterAllInitializersAndBeforeRegistration()
    {
        var calls = new List<string>();
        var context = new UserContext()
            .RegisterEntityInitializer("*", (_, entity) => { calls.Add("global"); ((SpecialOrder)entity).Tenant = "global"; })
            .RegisterEntityInitializer("SpecialOrder", (_, entity) => { calls.Add("entity"); ((SpecialOrder)entity).Tenant += "/entity"; });
        var value = context.InitializeEntity("SpecialOrder", new SpecialOrder(), entity => {
            calls.Add("admission"); Assert.Empty(context.ManagedEntities); Assert.Equal("global/entity", entity.Tenant);
        });
        Assert.Equal(new[] { "global", "entity", "admission" }, calls);
        Assert.Same(value, Assert.Single(context.ManagedEntities));
    }

    [Fact]
    public void FailedFinalAdmissionIsNotRegisteredOrReplaced()
    {
        var context = new UserContext(); var failure = new InvalidOperationException("Controlled final admission failure");
        var caught = Assert.Throws<InvalidOperationException>(() => context.InitializeEntity(
            "SpecialOrder", new SpecialOrder(), _ => throw failure));
        Assert.Same(failure, caught); Assert.Empty(context.ManagedEntities);
        var next = context.InitializeEntity("SpecialOrder", new SpecialOrder());
        Assert.Same(next, Assert.Single(context.ManagedEntities));
    }

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
