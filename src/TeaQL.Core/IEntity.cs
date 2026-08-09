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

    Value? Dynamic(string key) => Base.DynamicValue(key);
}

public interface IIdentifiableEntity : IEntity
{
    Value IdValue();
}

public interface IVersionedEntity : IEntity
{
    long Version();
}
