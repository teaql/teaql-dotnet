using System;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public class RuntimeModule
{
    public InMemoryMetadataStore Metadata { get; } = new();
    public InMemoryEntityRegistry EntityRegistry { get; } = new();
    internal Dictionary<string, IEntityChecker> Checkers { get; } = new();
    internal List<BootstrapEntity> RootEntities { get; } = new();
    internal List<BootstrapEntity> ConstantEntities { get; } = new();

    public RuntimeModule RootEntity(BootstrapEntity entity) { RootEntities.Add(entity); return this; }
    public RuntimeModule ConstantEntity(BootstrapEntity entity) { ConstantEntities.Add(entity); return this; }

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
        combined.RootEntities.AddRange(RootEntities);
        combined.RootEntities.AddRange(other.RootEntities);
        combined.ConstantEntities.AddRange(ConstantEntities);
        combined.ConstantEntities.AddRange(other.ConstantEntities);
        return combined;
    }

    public void ApplyTo(UserContext context)
    {
        context.Metadata = Metadata;
        context.EntityRegistry = EntityRegistry;
        context.InstallCheckers(Checkers);
        context.InstallBootstrapEntities(RootEntities, ConstantEntities);
    }

    public UserContext IntoContext()
    {
        var context = new UserContext();
        ApplyTo(context);
        return context;
    }
}

public sealed record BootstrapEntity(string Entity, long Id, Record Values);

public interface IEntityChecker
{
    IReadOnlyList<CheckResult> CheckAndFix(
        UserContext context, MutationRequest mutation, DateTimeOffset now);
}
