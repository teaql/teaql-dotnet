namespace TeaQL.Core;

public static class EntityExtensions
{
    public static Audited<T> AuditAs<T>(this T entity, string comment) where T : IEntity
    {
        return new Audited<T>(entity, comment);
    }
}
