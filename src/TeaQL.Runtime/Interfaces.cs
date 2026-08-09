using System;
using System.Collections.Generic;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

public interface IMetadataStore
{
    EntityDescriptor? GetEntity(string name);
    IReadOnlyList<EntityDescriptor> GetAllEntities();
    void RecordMetadataLog(ExecutionMetadata metadata) { }
}

public interface IEntityRegistry
{
    bool Contains(string entity);
}

public interface ITeaqlRuntime
{
    UserContext UserContext { get; }
}
