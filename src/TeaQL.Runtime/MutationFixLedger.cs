using TeaQL.Core;
using TeaQL.DataService;

namespace TeaQL.Runtime;

// A checked request is a detached snapshot. Successful Fix changes must also
// remain in its original graph ledger if a later provider/policy step fails.
internal static class MutationFixLedger
{
    internal static Record? Capture(MutationRequest request)
    {
        if (Values(request) is not { } values || request.LedgerRoot == null
            || request.LedgerKey is not { } key || key.EntityType != Entity(request)) return null;
        return new LoadedScalarSnapshot(values).CopyValues();
    }

    internal static void WriteBack(MutationRequest request, Record? before)
    {
        if (before == null || Values(request) is not { } values || request.LedgerRoot is not { } ledger
            || request.LedgerKey is not { } key || key.EntityType != Entity(request)) return;
        foreach (var (field, value) in values)
            if (field is not "id" and not "version"
                && (!before.TryGetValue(field, out var original) || !LoadedScalarSnapshot.SameValue(original, value)))
                ledger.Set(key, field, LoadedScalarSnapshot.CopyValue(value));
    }

    private static Record? Values(MutationRequest request) => request switch
    {
        InsertMutationRequest insert => insert.Command.Values,
        UpdateMutationRequest update => update.Command.Values,
        _ => null
    };
    private static string Entity(MutationRequest request) => request switch
    {
        InsertMutationRequest insert => insert.Command.Entity,
        UpdateMutationRequest update => update.Command.Entity,
        _ => ""
    };
}
