using System;

namespace TeaQL.Core;

public class EntityError : Exception
{
    public string Entity { get; }
    public string ErrorMessage { get; }

    public EntityError(string entity, string message) : base($"{entity}: {message}")
    {
        Entity = entity;
        ErrorMessage = message;
    }
}
