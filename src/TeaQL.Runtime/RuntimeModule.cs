using System;
using System.ComponentModel;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public class RuntimeModule
{
    public InMemoryMetadataStore Metadata { get; } = new();
    public InMemoryEntityRegistry EntityRegistry { get; } = new();
    internal Dictionary<string, IEntityChecker> Checkers { get; } = new();
    internal Func<UserContext, Task>? GeneratedBootstrapCallback { get; private set; }

    public RuntimeModule() { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public RuntimeModule(
        IEnumerable<string> entities,
        IReadOnlyDictionary<string, IEntityChecker> checkers,
        IReadOnlyDictionary<string, Record> schemaSamples,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, bool>> requiredFields)
    {
        foreach (var entity in entities)
        {
            var descriptor = EntityDescriptor.New(entity);
            if (schemaSamples.TryGetValue(entity, out var sample))
            {
                foreach (var (field, value) in sample)
                {
                    var property = TeaQL.Core.PropertyDescriptor.New(field, DataTypeFor(value)).ColumnName(field);
                    if (requiredFields.TryGetValue(entity, out var required)
                        && required.TryGetValue(field, out var isRequired) && isRequired)
                        property = property.NotNull();
                    if (string.Equals(field, "id", StringComparison.OrdinalIgnoreCase)) property = property.Id();
                    if (string.Equals(field, "version", StringComparison.OrdinalIgnoreCase)) property = property.Version();
                    descriptor.Property(property);
                }
            }
            Entity(descriptor);
        }
        foreach (var (entity, checker) in checkers) Checker(entity, checker);
    }

    private static DataType DataTypeFor(Value value) => value switch
    {
        Value.BoolValue => DataType.Bool,
        Value.I64Value => DataType.I64,
        Value.U64Value => DataType.U64,
        Value.F64Value => DataType.F64,
        Value.DecimalValue => DataType.Decimal,
        Value.JsonValue => DataType.Json,
        Value.DateValue => DataType.Date,
        Value.TimestampValue or Value.DateTimeValue => DataType.Timestamp,
        _ => DataType.Text
    };

    /// <summary>
    /// Generator integration point. Applications should install the generated RuntimeModule and call
    /// UserContext.EnsureSchemaAsync rather than invoking this callback directly.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public RuntimeModule GeneratedBootstrap(Func<UserContext, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        GeneratedBootstrapCallback = GeneratedBootstrapCallback == null
            ? callback
            : Compose(GeneratedBootstrapCallback, callback);
        return this;
    }

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
        if (GeneratedBootstrapCallback != null) combined.GeneratedBootstrap(GeneratedBootstrapCallback);
        if (other.GeneratedBootstrapCallback != null) combined.GeneratedBootstrap(other.GeneratedBootstrapCallback);
        return combined;
    }

    private static Func<UserContext, Task> Compose(
        Func<UserContext, Task> first, Func<UserContext, Task> second) => async context =>
    {
        await first(context).ConfigureAwait(false);
        await second(context).ConfigureAwait(false);
    };

    public void ApplyTo(UserContext context)
    {
        context.Metadata = Metadata;
        context.EntityRegistry = EntityRegistry;
        context.InstallCheckers(Checkers);
        context.InstallGeneratedBootstrap(GeneratedBootstrapCallback);
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
