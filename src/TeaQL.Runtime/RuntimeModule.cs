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
    // Shared immutable model declarations, not per-record state or a mutation ledger.
    internal Dictionary<string, IReadOnlyDictionary<string, string>> ContextRootFields { get; } = new();
    internal Func<UserContext, Task>? GeneratedBootstrapCallback { get; private set; }
    public Dictionary<string, WireEntityMetadata> WireMetadata { get; } = new();

    public RuntimeModule() { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public RuntimeModule(
        IEnumerable<string> entities,
        IReadOnlyDictionary<string, IEntityChecker> checkers,
        IReadOnlyDictionary<string, Record> schemaSamples,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, bool>> requiredFields,
        IReadOnlyDictionary<string, IReadOnlyList<RelationDescriptor>>? relations = null,
        IReadOnlyDictionary<string, string>? tableNames = null,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? integer32Fields = null,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>? immutableUpdateFields = null,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>? contextRootFields = null)
    {
        foreach (var entity in entities)
        {
            var descriptor = EntityDescriptor.New(entity);
            if (tableNames != null && tableNames.TryGetValue(entity, out var tableName))
                descriptor = descriptor.TableName(tableName);
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
                    if (integer32Fields != null && integer32Fields.TryGetValue(entity, out var integerFields)
                        && integerFields.Contains(field))
                    {
                        if (property.DataType != DataType.I64 || property.IsId || property.IsVersion
                            || relations != null && relations.TryGetValue(entity, out var forwardRelations)
                                && forwardRelations.Any(relation => !relation.IsMany && relation.LocalKeyValue == field))
                            throw new ArgumentException("Integer32 metadata requires a modeled scalar integer field", nameof(integer32Fields));
                        property = property.Int32();
                    }
                    if (immutableUpdateFields != null && immutableUpdateFields.TryGetValue(entity, out var immutable)
                        && immutable.TryGetValue(field, out var ruleId)) property = property.ImmutableAfterCreate(ruleId);
                    descriptor.Property(property);
                }
            }
            if (relations != null && relations.TryGetValue(entity, out var entityRelations))
                foreach (var relation in entityRelations) descriptor.Relation(relation);
            if (immutableUpdateFields != null && immutableUpdateFields.TryGetValue(entity, out var declaredImmutable)
                && declaredImmutable.Keys.Any(field => descriptor.PropertyByName(field) == null))
                throw new ArgumentException("Immutable metadata requires a canonical modeled field", nameof(immutableUpdateFields));
            Entity(descriptor);
        }
        foreach (var (entity, checker) in checkers) Checker(entity, checker);
        if (contextRootFields != null)
            foreach (var (entity, fields) in contextRootFields) AddContextRootFields(entity, fields);
    }

    private void AddContextRootFields(string entity, IReadOnlyDictionary<string, string> fields)
    {
        if (fields.Count == 0) return;
        var descriptor = Metadata.GetEntity(entity) ?? throw new ArgumentException("Context-root metadata requires a modeled entity");
        var merged = ContextRootFields.TryGetValue(entity, out var previous)
            ? previous.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal)
            : new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (field, target) in fields)
        {
            var property = descriptor.PropertyByName(field);
            if (string.IsNullOrWhiteSpace(target) || property == null || property.DataType != DataType.I64 || property.IsId || property.IsVersion
                || !descriptor.Relations.Any(r => !r.IsMany && r.LocalKeyValue == field && r.ForeignKeyValue == "id" && r.TargetEntity == target))
                throw new ArgumentException("Context-root metadata requires a canonical modeled forward reference");
            if (merged.TryGetValue(field, out var existing) && existing != target)
                throw new ArgumentException("Conflicting context-root metadata");
            merged[field] = target;
        }
        ContextRootFields[entity] = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(merged);
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
        Value.TimeValue => DataType.Time,
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

    public RuntimeModule WireEntity(WireEntityMetadata metadata) { WireMetadata[metadata.EntityType] = metadata; return this; }

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
        foreach (var metadata in WireMetadata.Values) combined.WireEntity(metadata);
        foreach (var metadata in other.WireMetadata.Values) combined.WireEntity(metadata);
        foreach (var checker in Checkers) combined.Checker(checker.Key, checker.Value);
        foreach (var checker in other.Checkers) combined.Checker(checker.Key, checker.Value);
        foreach (var (entity, fields) in ContextRootFields) combined.AddContextRootFields(entity, fields);
        foreach (var (entity, fields) in other.ContextRootFields) combined.AddContextRootFields(entity, fields);
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
        context.InstallContextRootFields(ContextRootFields);
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
