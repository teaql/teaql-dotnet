using System;

namespace TeaQL.Core;

public class Audited<T> where T : IEntity
{
    public T Entity { get; }
    public string Comment { get; }

    public Audited(T entity, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new ArgumentException("audit comment must not be empty", nameof(comment));
        }
        Entity = entity;
        Comment = comment;
    }

    public T IntoEntity()
    {
        Entity.SetComment(Comment);
        return Entity;
    }
}
