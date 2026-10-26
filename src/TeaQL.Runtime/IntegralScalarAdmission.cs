using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

// Representation admission after customer Fix. Business ranges remain custom.
internal static class IntegralScalarAdmission
{
    internal sealed record DeferredReference(Value Original, EntityRoot Ledger, EntityKey Target);

    internal static Dictionary<string, DeferredReference> CaptureDeferred(
        EntityDescriptor? descriptor, MutationRequest request, bool preparing)
    {
        var result = new Dictionary<string, DeferredReference>();
        if (!preparing || descriptor == null || request.LedgerRoot is not { } ledger
            || request.LedgerKey is not { } key || key.EntityType != descriptor.Name || !ledger.HasPending(key)) return result;
        var values = Values(request);
        if (values == null) return result;
        foreach (var relation in descriptor.Relations.Where(r => !r.IsMany && r.ForeignKeyValue == "id"))
            if (values.TryGetValue(relation.LocalKeyValue, out var value) && value is Value.I64Value { Value: < 0 })
            {
                var target = new EntityKey(relation.TargetEntity, value);
                if (ledger.IsNew(target)) result[relation.LocalKeyValue] = new(value, ledger, target);
            }
        return result;
    }

    internal static IEnumerable<CheckResult> Check(EntityDescriptor? descriptor, MutationRequest request,
        IReadOnlyDictionary<string, DeferredReference> deferred)
    {
        var values = Values(request);
        if (descriptor == null || values == null) yield break;
        var references = descriptor.Relations.Where(r => !r.IsMany && r.ForeignKeyValue == "id")
            .Select(r => r.LocalKeyValue).ToHashSet(StringComparer.Ordinal);
        foreach (var property in descriptor.Properties)
        {
            if (property.DataType is not (DataType.I64 or DataType.U64)
                || !values.TryGetValue(property.Name, out var value) || value is Value.NullValue or Value.TypedNullValue) continue;
            if (deferred.TryGetValue(property.Name, out var pending) && Equals(value, pending.Original)
                && pending.Ledger.IsNew(pending.Target)) continue;
            var valid = property.DataType == DataType.U64 ? value.TryU64().HasValue
                : property.IsInt32 ? value.TryI32().HasValue : value.TryI64().HasValue;
            if ((property.IsId || references.Contains(property.Name)) && value is Value.I64Value { Value: < 0 }) valid = false;
            if ((property.IsId || references.Contains(property.Name)) && value is Value.DecimalValue { Value: < 0 }) valid = false;
            if (!valid) yield return new CheckResult { RuleId = "invalid_type", EntityType = descriptor.Name,
                Location = ObjectLocation.Property(property.Name), Message = "Expected an exact integer in the declared model range" };
        }
    }

    private static Record? Values(MutationRequest request) => request switch
    {
        InsertMutationRequest insert => insert.Command.Values,
        UpdateMutationRequest update => update.Command.Values,
        _ => null
    };
}
