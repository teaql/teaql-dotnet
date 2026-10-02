using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

// Projection-only state; no raw source, bindings or context are retained.
// ConditionalWeakTable does not extend the lifetime of a caller's debug record.
internal static class SqlLogProjectionState
{
    private sealed record State(byte[] Fingerprint, ExecutionMetadata Safe);
    private static readonly ConditionalWeakTable<ExecutionMetadata, State> States = new();

    private static object Describe(Value value) => value switch
    {
        Value.ListValue list => new { Type = value.GetType().Name, Items = list.Values.Select(Describe).ToArray() },
        Value.ObjectValue obj => new { Type = value.GetType().Name,
            Items = obj.Value.Select(pair => new { pair.Key, Value = Describe(pair.Value) }).ToArray() },
        _ => new { Type = value.GetType().Name, Native = JsonSerializer.Serialize(value, value.GetType()) }
    };

    private static object BindingTypes(ExecutionMetadata metadata) => new
    {
        Values = metadata.Parameters.Select(Describe).ToArray(),
        Children = metadata.Statements.Select(BindingTypes).ToArray()
    };

    private static byte[]? Fingerprint(ExecutionMetadata metadata)
    {
        try
        {
            // Public metadata plus native value types: JSON alone conflates numeric types.
            return SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(new
            {
                Metadata = metadata, Bindings = BindingTypes(metadata)
            }));
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException or ArgumentException or InvalidOperationException)
        {
            // Unsupported diagnostics never acquire trusted projection provenance.
            return null;
        }
    }

    internal static void Remember(ExecutionMetadata debug, ExecutionMetadata safe)
    {
        var fingerprint = Fingerprint(debug);
        if (fingerprint != null) States.Add(debug, new State(fingerprint, Clone(safe)));
    }

    internal static ExecutionMetadata? Restore(ExecutionMetadata debug)
    {
        if (!States.TryGetValue(debug, out var state)) return null;
        var current = Fingerprint(debug);
        return current != null && CryptographicOperations.FixedTimeEquals(current, state.Fingerprint)
            ? Clone(state.Safe) : null;
    }

    private static ExecutionMetadata Clone(ExecutionMetadata source) => new()
    {
        Backend = source.Backend, Operation = source.Operation, ExecutionOutcome = source.ExecutionOutcome,
        StartedAt = source.StartedAt, EndedAt = source.EndedAt,
        AffectedRows = source.AffectedRows, ResultCount = source.ResultCount,
        TraceChain = source.TraceChain.Select(node => node with { }).ToList(),
        MutationLineage = Array.AsReadOnly(source.MutationLineage.Select(node => node with { }).ToArray()),
        Comment = source.Comment, Purpose = source.Purpose, AuditReason = source.AuditReason,
        BackendRequestId = source.BackendRequestId, ParameterizedQuery = source.ParameterizedQuery,
        Parameters = source.Parameters.Select(LogPrivacy.Copy).ToArray(), ParameterCount = source.ParameterCount,
        DebugQuery = source.DebugQuery, ParameterLogPolicies = source.ParameterLogPolicies.ToArray(),
        MaskedParameters = source.MaskedParameters.ToArray(), GeneratedSql = source.GeneratedSql,
        LogMode = source.LogMode, SqlOmissionReason = source.SqlOmissionReason,
        Statements = source.Statements.Select(Clone).ToArray()
    };
}
