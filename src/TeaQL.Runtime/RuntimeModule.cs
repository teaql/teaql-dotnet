using System;
using TeaQL.Core;

namespace TeaQL.Runtime;

public class RuntimeModule
{
    public InMemoryMetadataStore Metadata { get; } = new();
    public InMemoryEntityRegistry EntityRegistry { get; } = new();

    public RuntimeModule Entity(EntityDescriptor descriptor)
    {
        EntityRegistry.Register(descriptor.Name);
        Metadata.Register(descriptor);
        return this;
    }

    public void ApplyTo(UserContext ctx)
    {
        ctx.Metadata = Metadata;
        ctx.EntityRegistry = EntityRegistry;
    }

    public UserContext IntoContext()
    {
        var ctx = new UserContext();
        ApplyTo(ctx);
        return ctx;
    }
}
