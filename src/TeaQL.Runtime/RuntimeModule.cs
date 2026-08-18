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

    public RuntimeModule And(RuntimeModule other)
    {
        ArgumentNullException.ThrowIfNull(other);
        var combined = new RuntimeModule();
        foreach (var descriptor in Metadata.GetAllEntities()) combined.Entity(descriptor);
        foreach (var descriptor in other.Metadata.GetAllEntities()) combined.Entity(descriptor);
        return combined;
    }

    public void ApplyTo(UserContext context)
    {
        context.Metadata = Metadata;
        context.EntityRegistry = EntityRegistry;
    }

    public UserContext IntoContext()
    {
        var context = new UserContext();
        ApplyTo(context);
        return context;
    }
}
