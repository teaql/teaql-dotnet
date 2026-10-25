using Generated;
using System.Text.Json;
using TeaQL.Runtime;

static class OwnershipProbe
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static async Task Verify(UserContext context)
    {
        var before = await Q.PurchaseOrders().Limit(20).Comment("Inspect the initial ownership fixture")
            .Purpose("Verify independent roots never share pending mutations").ExecuteForListAsync(context);
        var first = Q.PurchaseOrders().Comment("Prepare independent new owner A")
            .Purpose("Verify Context does not own mutation ledgers").NewEntity(context);
        var second = Q.PurchaseOrders().Comment("Prepare independent new owner B")
            .Purpose("Verify Context does not own mutation ledgers").NewEntity(context);
        first.UpdateContactPhone("5550300"); second.UpdateContactPhone("5550400");
        var savedA = await first.AuditAs("Save only independent new owner A").SaveAsync(context);
        var initial = await Q.PurchaseOrders().Limit(20).Comment("Inspect A's save before saving B")
            .Purpose("Verify the unsaved new owner did not leak into persistence").ExecuteForListAsync(context);
        Require(initial.Count == before.Count + 1, "Saving new A leaked new B");
        var savedB = await second.AuditAs("Explicitly save independent new owner B").SaveAsync(context);
        var batch = await Q.PurchaseOrders().Limit(20).Comment("Load independent owners in one batch")
            .Purpose("Verify list roots retain independent mutation boundaries").ExecuteForListAsync(context);
        Require(batch.Count == before.Count + 2, "Missing independent roots");
        var ownerA = batch.Single(row => row.Id == savedA.Id);
        var ownerB = batch.Single(row => row.Id == savedB.Id);
        ownerA.UpdateContactPhone("5550301"); ownerB.UpdateContactPhone("5550401");
        await ownerA.AuditAs("Save only A from the multi-root query result").SaveAsync(context);
        var unchanged = await Q.PurchaseOrders().WithIdIs(savedB.Id!.Value).Limit(1)
            .Comment("Inspect B after saving A").Purpose("Verify A neither persists nor commits B").ExecuteForListAsync(context);
        Require(unchanged.Count == 1 && E.PurchaseOrder(unchanged[0]).ContactPhone().Eval() == "5550400", "A persisted B's pending change");
        Require(E.PurchaseOrder(unchanged[0]).Version().Eval() == 1, "A changed B's version");
        await ownerB.AuditAs("Save B after A without discarding its pending mutation").SaveAsync(context);
        var changed = await Q.PurchaseOrders().WithIdIs(savedB.Id!.Value).Limit(1)
            .Comment("Inspect the later independent B save").Purpose("Verify B's pending mutation survived A's save").ExecuteForListAsync(context);
        Require(changed.Count == 1 && E.PurchaseOrder(changed[0]).ContactPhone().Eval() == "5550401", "A discarded B's pending mutation");
        Require(E.PurchaseOrder(changed[0]).Version().Eval() == 2, "B was not saved exactly once");
        await PageStreamOwnershipProbe.Verify(context, savedA.Id!.Value, savedB.Id!.Value);
        Console.WriteLine("CORE_MUTATION_OWNERSHIP=" + JsonSerializer.Serialize(new {
            language = "dotnet", passed = true, independentNewRoots = true,
            batchRootIsolation = true, siblingVersionUnchanged = true, siblingPendingMutationRetained = true
        }));
    }
}
