using System;
using TeaQL.Core;

namespace TeaQL.Runtime;

public class RuntimeModule
{
    public InMemoryMetadataStore Metadata { get; } = new();
    public InMemoryEntityRegistry EntityRegistry { get; } = new();
    private IRawAuditEventSink? _auditEventSink;

    public RuntimeModule Entity(EntityDescriptor descriptor)
    {
        EntityRegistry.Register(descriptor.Name);
        Metadata.Register(descriptor);
        return this;
    }

    public RuntimeModule AuditEventSink(IRawAuditEventSink sink)
    {
        _auditEventSink = sink;
        return this;
    }

    public void ApplyTo(UserContext ctx)
    {
        ctx.Metadata = Metadata;
        ctx.EntityRegistry = EntityRegistry;
        if (_auditEventSink is not null) ctx.SetStandardAuditEventSink(_auditEventSink);
    }

    public UserContext IntoContext()
    {
        var ctx = new UserContext();
        ApplyTo(ctx);
        return ctx;
    }
}
