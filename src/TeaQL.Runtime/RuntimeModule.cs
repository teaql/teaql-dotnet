using System;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public class RuntimeModule
{
    public InMemoryMetadataStore Metadata { get; } = new();
    public InMemoryEntityRegistry EntityRegistry { get; } = new();
    internal Dictionary<string, IEntityChecker> Checkers { get; } = new();

    public RuntimeModule Checker(string entity, IEntityChecker checker)
    {
        Checkers[entity] = checker;
        return this;
    }

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
        foreach (var checker in Checkers) combined.Checker(checker.Key, checker.Value);
        foreach (var checker in other.Checkers) combined.Checker(checker.Key, checker.Value);
        return combined;
    }

    public void ApplyTo(UserContext context)
    {
        context.Metadata = Metadata;
        context.EntityRegistry = EntityRegistry;
        context.InstallCheckers(Checkers);
    }

    public UserContext IntoContext()
    {
        var context = new UserContext();
        ApplyTo(context);
        return context;
    }
}

public interface IEntityChecker
{
    IReadOnlyList<CheckResult> CheckAndFix(
        UserContext context, MutationRequest mutation, DateTimeOffset now);
}
