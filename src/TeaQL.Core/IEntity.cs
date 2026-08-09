using System.Collections.Generic;

namespace TeaQL.Core;

public interface ITeaqlEntity
{
    static abstract string EntityName { get; }
    static abstract EntityDescriptor EntityDescriptor();
}

public interface IEntity : ITeaqlEntity
{
    Record IntoRecord();
    ISet<string>? DirtyFields() => null;
    bool IsMarkedAsDelete() => false;
    bool IsNew() => false;
    void MarkAsNew() {}
    string? GetComment() => null;
    void SetComment(string comment) {}
    
    
    
    System.Text.Json.Nodes.JsonNode? IntoJson() => IntoRecord().ToJsonValue();
    
    IDictionary<string, Value>? OriginalValues() => null;
    void OnLoaded(object context) {}
}

public interface IBaseEntity : IEntity
{
    BaseEntityData Base { get; }
    BaseEntityData BaseMut { get; }

    ulong Id => Base.Id;
    void SetId(ulong id) { BaseMut.Id = id; }
    
    long VersionValue => Base.Version;
    void SetVersion(long version) { BaseMut.Version = version; }

    Value? Dynamic(string key) => Base.Dynamic(key);
    long? DynamicI64(string key) => Base.DynamicI64(key);
    ulong? DynamicU64(string key) => Base.DynamicU64(key);
    decimal? DynamicDecimal(string key) => Base.DynamicDecimal(key);
    double? DynamicF64(string key) => Base.DynamicF64(key);
    string? DynamicText(string key) => Base.DynamicText(key);
    bool? DynamicBool(string key) => Base.DynamicBool(key);

    Value? PutDynamic(string key, Value value) => BaseMut.PutDynamic(key, value);
}

public interface IIdentifiableEntity : IEntity
{
    Value IdValue();
}

public interface IVersionedEntity : IEntity
{
    long Version();
}
