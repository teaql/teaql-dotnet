using Generated;
using System.Text.Json;
using TeaQL.Runtime;

static class LoadingProbe
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static async Task Verify(UserContext context, long orderId)
    {
        var rows = await Q.PurchaseOrders().WithIdIs(orderId)
            .SelectTenantWith(Q.TenantsWithMinimalFields().WithIdIs(2).Limit(1)
                .Comment("Load only a different tenant's detail"))
            .Limit(1).Comment("Keep the selected FK when target detail is filtered out")
            .Purpose("Verify filtered detail is NotLoaded rather than loaded null").ExecuteForListAsync(context);
        Require(rows.Count == 1, "Missing owner");
        var owner = rows[0];
        Require(E.PurchaseOrder(owner).TenantId().Eval() == 1, "Selected FK was lost");
        NotLoadedDiagnostic detail;
        try { _ = E.PurchaseOrder(owner).Tenant().Name().Eval();
            throw new InvalidOperationException("A filtered target must be NotLoaded"); }
        catch (TeaQLNotLoadedException error) {
            detail = error.ToWire();
            Require(detail.AttemptedInstancePath == "/tenant/name", "Lost the full detail path");
        }
        owner.UpdateContactPhone("5550102");
        await owner.AuditAs("Change only the complete owner").SaveAsync(context);
        rows = await Q.PurchaseOrders().WithIdIs(orderId).Limit(1)
            .Comment("Reload the successful owner update")
            .Purpose("Verify untouched partial targets do not prevent saving the owner").ExecuteForListAsync(context);
        Require(rows.Count == 1 && E.PurchaseOrder(rows[0]).ContactPhone().Eval() == "5550102", "Owner update lost");
        var version = E.PurchaseOrder(rows[0]).Version().Eval();
        var partialRows = await Q.PurchaseOrdersWithMinimalFields().WithIdIs(orderId).Limit(1)
            .Comment("Load an identity-only owner")
            .Purpose("Reject changes to an incomplete owner before persistence").ExecuteForListAsync(context);
        Require(partialRows.Count == 1, "Missing partial owner");
        partialRows[0].UpdateContactPhone("5550103");
        try { await partialRows[0].AuditAs("Attempt to change an incomplete owner").SaveAsync(context);
            throw new InvalidOperationException("Incomplete modified owner was not rejected"); }
        catch (CheckException error) {
            Require(error.Violations.Any(v => v.RuleId == "invalid_type"), "Missing structured completeness violation");
        }
        rows = await Q.PurchaseOrders().WithIdIs(orderId).Limit(1)
            .Comment("Reload after the rejected incomplete mutation")
            .Purpose("Verify persisted values and optimistic version remain unchanged").ExecuteForListAsync(context);
        Require(rows.Count == 1 && E.PurchaseOrder(rows[0]).ContactPhone().Eval() == "5550102", "Rejected write changed phone");
        Require(E.PurchaseOrder(rows[0]).Version().Eval() == version, "Rejected write changed version");
        Console.WriteLine("CORE_LOAD_EXAMPLE=" + JsonSerializer.Serialize(new {
            language = "dotnet", passed = true, foreignKeyPreserved = true, filteredDetailNotLoaded = detail,
            untouchedPartialTargetSave = true, modifiedPartialOwnerRejected = true, rejectedVersionUnchanged = true
        }));
    }
}
