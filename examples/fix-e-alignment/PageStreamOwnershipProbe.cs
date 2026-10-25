using Generated;
using System.Text.Json;
using TeaQL.Runtime;

static class PageStreamOwnershipProbe
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException(message); }

    public static async Task Verify(UserContext context, long idA, long idB)
    {
        var before = await Q.PurchaseOrders().Limit(20).Comment("Inspect the page ownership fixture")
            .Purpose("Establish the bounded exact total").ExecuteForListAsync(context);
        var sentinel = Q.PurchaseOrders().Comment("Prepare a page-offset sentinel")
            .Purpose("Exercise a genuine nonzero page offset").NewEntity(context);
        sentinel.UpdateContactPhone("5550500");
        await sentinel.AuditAs("Save the synthetic page sentinel").SaveAsync(context);
        async Task Exercise(IEnumerable<Generated.Models.PurchaseOrder> rows, string label, string previous, string next, long version)
        {
            var a = rows.Single(row => row.Id == idA); var b = rows.Single(row => row.Id == idB);
            a.UpdateContactPhone("55503" + next[^2..]); b.UpdateContactPhone(next);
            await Q.TenantsWithMinimalFields().Limit(1).Comment("Run an unrelated query before deferred saves")
                .Purpose("Verify deferred ownership survives Context reuse").ExecuteForListAsync(context);
            await a.AuditAs("Save only A from " + label).SaveAsync(context);
            var persisted = await Q.PurchaseOrders().WithIdIs(idB).Limit(1).Comment("Inspect B after deferred A save")
                .Purpose("Verify unrelated root value and version remain untouched").ExecuteForListAsync(context);
            Require(persisted.Count == 1 && E.PurchaseOrder(persisted[0]).ContactPhone().Eval() == previous, "A persisted B's pending mutation");
            Require(E.PurchaseOrder(persisted[0]).Version().Eval() == version, "A changed B's version");
            await b.AuditAs("Save B later from " + label).SaveAsync(context);
            persisted = await Q.PurchaseOrders().WithIdIs(idB).Limit(1).Comment("Inspect the later deferred B save")
                .Purpose("Verify B's pending mutation survived A's commit").ExecuteForListAsync(context);
            Require(persisted.Count == 1 && E.PurchaseOrder(persisted[0]).ContactPhone().Eval() == next, "A discarded B's pending mutation");
            Require(E.PurchaseOrder(persisted[0]).Version().Eval() == version + 1, "B was not saved exactly once");
        }
        var page = await Q.PurchaseOrders().OrderByIdDescending().Offset(1).Limit(2).Count()
            .Comment("Load a nonzero-offset page of independent roots").Purpose("Verify exact count and page ownership").ExecuteForListAsync(context);
        Require(page.Count == 2 && page.TotalCount == before.Count + 1, "Incorrect page or exact total");
        await Exercise(page, "a nonzero-offset page", "5550401", "5550402", 2);
        var streamed = new List<Generated.Models.PurchaseOrder>();
        await foreach (var row in Q.PurchaseOrders().OrderByIdAscending().Limit(20)
            .Comment("Stream independent roots across single-row chunks").Purpose("Verify cross-chunk deferred-save ownership")
            .ExecuteForStreamAsync(context, chunkSize: 1)) streamed.Add(row);
        Require(streamed.Count == before.Count + 1, "Stream lost roots");
        await Exercise(streamed, "a fully consumed scalar stream", "5550402", "5550403", 3);
        Console.WriteLine("CORE_MUTATION_PAGING=" + JsonSerializer.Serialize(new {
            language = "dotnet", passed = true, pageOffset = 1, pageSize = 2, exactCount = before.Count + 1,
            pageRootIsolation = true, deferredSaveAfterContextReuse = true, streamProfile = "scalar-typed",
            streamChunkSize = 1, streamRootIsolation = true, siblingPendingMutationRetained = true
        }));
    }
}
