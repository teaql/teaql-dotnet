namespace TeaQL.Core;

public enum MutationAuditKind { Created, Updated, Deleted, Recovered }

public sealed record AuditFieldChange(string Field, Value? OldValue, Value? NewValue);

public sealed record RawAuditEvent(
    MutationAuditKind Kind,
    string Entity,
    Value? EntityId,
    IReadOnlyList<AuditFieldChange> Changes,
    IReadOnlyList<TraceNode> TraceChain);

public sealed record SafeAuditField(string Field, string? Value, bool Masked, bool Truncated);

public sealed record SafeAuditEvent(
    MutationAuditKind Kind,
    string Entity,
    string? EntityId,
    IReadOnlyList<SafeAuditField> Fields,
    IReadOnlyList<TraceNode> TraceChain);
