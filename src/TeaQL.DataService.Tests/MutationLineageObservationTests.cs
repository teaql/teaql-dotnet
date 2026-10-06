using System.Text.Json;
using TeaQL.Core;
using TeaQL.DataService;
using Xunit;

namespace TeaQL.DataService.Tests;

public class MutationLineageObservationTests
{
    [Fact]
    public void ProviderObservationIsAnOwnedImmutableSnapshotAndNotAWireField()
    {
        var ledger = new EntityRoot();
        var key = new EntityKey("Payment", 9);
        ledger.SetTraceChain(key, new MutationTraceScope("Payment", 9, "special approval").Recover());
        var request = new InsertMutationRequest(new InsertCommand("Payment"), "save graph")
            { LedgerKey = key, LedgerRoot = ledger };
        var observed = request.MutationLineage;
        ledger.SetTraceChain(key, new MutationTraceScope("Payment", 9, "later approval").Recover());
        Assert.Equal("special approval", Assert.Single(observed).Detail);
        Assert.Equal("later approval", Assert.Single(request.MutationLineage).Detail);
        Assert.Throws<NotSupportedException>(() => ((IList<TraceNode>)observed).Clear());
        Assert.DoesNotContain("MutationLineage", JsonSerializer.Serialize(request));
    }
}
