using System.Reflection;
using System.Text.Json;
using Generated;
using Generated.Models;
using TeaQL.Core;
using TeaQL.Runtime;

static class StreamChecks
{
    private sealed class Scope(long boundary) : IRequestPolicy
    {
        public bool LastOnly;
        public int Calls;
        public SelectQuery Apply(SelectQuery query)
        {
            if (query.Entity == "OrderItem")
            {
                Calls++;
                query.AndFilter(LastOnly ? Expr.Gt("id",boundary) : Expr.Lte("id",boundary));
            }
            return query;
        }
    }
    private sealed class NoScope : IRequestPolicy { public SelectQuery Apply(SelectQuery query)=>query; }
    private static EntityRoot Ledger(object entity)=>(EntityRoot)(entity.GetType()
        .GetField("_entityRoot",BindingFlags.NonPublic|BindingFlags.Instance)?.GetValue(entity)
        ?? throw new Exception("missing runtime ledger"));

    public static async Task RunAsync(UserContext context,CapturingExecutor capture,EvidenceSink sink,FaultTransport transport)
    {
        var nonce=Guid.NewGuid().ToString("N");
        var secret="STREAM-GENERATED-PRIVATE-"+nonce;
        var parent=Q.CustomerOrders().Comment("prepare stream parent").Purpose("own test items").NewEntity(context)
            .UpdatePlatformId(1).UpdateOrderNumber("stream-"+nonce).UpdateDescription("stream fixture");
        for(var i=0;i<8;i++) parent.OrderItemList.Add(Q.OrderItems().Comment("prepare streamed entity")
            .Purpose("verify independent stream mutation").NewEntity(context).UpdateName(secret));
        await parent.AuditAs("seed stream graph").SaveAsync(context);
        var ids=parent.OrderItemList.Select(item=>E.OrderItem(item).Id().Eval()!.Value).Order().ToArray();
        var policy=new Scope(ids[4]);
        var previous=context.GetResource<IRequestPolicy>();
        IAsyncEnumerable<OrderItem> first,second;
        sink.Clear(); var startOpens=transport.StreamOpens; var startCloses=transport.StreamCloses;
        try
        {
            context.WithRequestPolicy(policy);
            var builder=Q.OrderItems().WithNameIs(secret).OrderByIdAscending().Limit(10)
                .Comment("first stream "+secret);
            first=builder.Purpose("first purpose "+secret).ExecuteForStreamAsync(context,chunkSize:2);
            Verify.Equal(1,policy.Calls,"first stream prepares Context immediately");
            builder.WithNameIs("changed after capture").Comment("changed builder comment");
            policy.LastOnly=true;
            second=Q.OrderItems().WithNameIs(secret).OrderByIdAscending().Limit(10)
                .Comment("second stream "+secret).Purpose("second purpose "+secret)
                .ExecuteForStreamAsync(context,chunkSize:1);
            Verify.Equal(2,policy.Calls,"second stream has independent Context preparation");
        }
        finally { context.WithRequestPolicy(previous??new NoScope()); }
        Verify.Equal(startOpens,transport.StreamOpens,"no cursor before enumeration"); Verify.Equal(0,sink.Sql.Count,"no premature SQL fact");
        var firstRows=new List<OrderItem>();var secondRows=new List<OrderItem>();
        await using(var left=first.GetAsyncEnumerator())
        await using(var right=second.GetAsyncEnumerator())
        {
            Verify.That(await left.MoveNextAsync(),"first stream has first entity");firstRows.Add(left.Current);
            Verify.That(await right.MoveNextAsync(),"second stream has its entity");secondRows.Add(right.Current);
            Verify.Equal(2,(transport.StreamOpens-startOpens)-(transport.StreamCloses-startCloses),"two actual cursors overlap");
            while(await left.MoveNextAsync()) firstRows.Add(left.Current);
            while(await right.MoveNextAsync()) secondRows.Add(right.Current);
        }
        Verify.That(firstRows.Select(item=>E.OrderItem(item).Id().Eval()!.Value).SequenceEqual(ids.Take(5)),"first captured filter");
        Verify.That(secondRows.Select(item=>E.OrderItem(item).Id().Eval()!.Value).SequenceEqual(ids.Skip(5)),"second captured filter");
        Verify.Equal(2,transport.StreamCloses-startCloses,"both cursors closed");
        Verify.Equal(2,sink.Sql.Count,"one terminal SQL fact per stream");
        Verify.That(!JsonSerializer.Serialize(sink.Sql).Contains(secret),"captured stream intent is masked");
        Verify.That(sink.Sql[0].Comment!.StartsWith("first stream"),"first immutable comment");
        Verify.That(sink.Sql[1].Purpose!.StartsWith("second purpose"),"second immutable purpose");
        foreach(var entry in sink.Sql)
        {
            Verify.Equal("success",entry.ExecutionOutcome,"stream terminal success");
            Verify.That(entry.TraceChain.Select(node=>node.Name).SequenceEqual(new[]{"OrderItem","OrderItem","sqlite","select"}),"canonical stream path");
        }
        Verify.Equal(5,sink.Sql[0].ResultCount,"first delivered rows");Verify.Equal(3,sink.Sql[1].ResultCount,"second delivered rows");
        Console.WriteLine("STREAM QUERY EVIDENCE "+JsonSerializer.Serialize(new{ids,sql=sink.Sql}));
        Verify.That(!ReferenceEquals(Ledger(firstRows[0]),Ledger(firstRows[1])),"same chunk has separate root ledgers");
        Verify.That(!ReferenceEquals(Ledger(firstRows[1]),Ledger(secondRows[0])),"separate streams own separate ledgers");

        sink.Clear();var abandonedOpens=transport.StreamOpens;
        context.DisableQuerySqlLog();
        try
        {
            var invalid=Q.OrderItems().Limit(1).Purpose("missing stream comment must reject");
            await Verify.Throws<RequestIntentException>(()=>Task.FromResult(invalid.ExecuteForStreamAsync(context)),
                "missing comment rejects at stream creation even with logging disabled");
            Verify.Equal(abandonedOpens,transport.StreamOpens,"invalid intent opens no cursor");
        }
        finally { context.EnableQuerySqlLog(); }
        _=Q.OrderItems().WithNameIs(secret).Limit(10).Comment("unused stream").Purpose("no SQL before polling").ExecuteForStreamAsync(context);
        Verify.Equal(abandonedOpens,transport.StreamOpens,"abandoned stream opens nothing");Verify.Equal(0,sink.Sql.Count,"abandoned stream logs nothing");
        using(var cancellation=new CancellationTokenSource())
        {
            var cancellable=Q.OrderItems().WithNameIs(secret).OrderByIdAscending().Limit(10)
                .Comment("cancel stream "+secret).Purpose("cancel purpose "+secret).ExecuteForStreamAsync(context,chunkSize:1);
            await using var cursor=cancellable.GetAsyncEnumerator(cancellation.Token);
            Verify.That(await cursor.MoveNextAsync(),"cancel test gets one row");cancellation.Cancel();
            await Verify.Throws<OperationCanceledException>(async()=>{await cursor.MoveNextAsync();},"enumeration token reaches transport");
        }
        var cancelled=sink.Sql.Single();Verify.Equal("cancelled",cancelled.ExecutionOutcome,"cancellation fact");
        Verify.Equal(1,cancelled.ResultCount,"delivered count excludes prefetched rows");
        Verify.That(!JsonSerializer.Serialize(cancelled).Contains(secret),"cancel intent remains masked");
        Console.WriteLine("STREAM TERMINAL EVIDENCE "+JsonSerializer.Serialize(cancelled));

        firstRows[0].UpdateName("stream first saved "+nonce);firstRows[1].UpdateName("stream second pending "+nonce);
        capture.Clear();sink.Clear();await firstRows[0].AuditAs("save first streamed entity").SaveAsync(context);
        Verify.Equal(1,capture.Commands.Count,"only one streamed root is written");
        Verify.That(!Ledger(firstRows[1]).IsEmpty,"other streamed root remains pending");
        var before=await Q.OrderItems().WithIdIs(ids[1]).Limit(1).Comment("check pending streamed row")
            .Purpose("prove it was not saved").ExecuteForOneAsync(context)??throw new Exception("missing streamed row");
        Verify.Equal(secret,E.OrderItem(before).Name().Eval(),"unsaved value unchanged");Verify.Equal(1L,E.OrderItem(before).Version().Eval(),"unsaved version unchanged");
        await firstRows[1].AuditAs("save second streamed entity").SaveAsync(context);
        for(var i=0;i<2;i++)
        {
            var actual=await Q.OrderItems().WithIdIs(ids[i]).Limit(1).Comment("reload independently saved stream row")
                .Purpose("verify persisted result").ExecuteForOneAsync(context)??throw new Exception("missing saved row");
            Verify.Equal(i==0?"stream first saved "+nonce:"stream second pending "+nonce,E.OrderItem(actual).Name().Eval(),"saved name");
            Verify.Equal(2L,E.OrderItem(actual).Version().Eval(),"saved version");
        }
        Verify.Equal(1L,E.OrderItem(secondRows[0]).Version().Eval(),"untouched third row version");
        Console.WriteLine("PASS: .NET generated stream capture, overlapping cursors, privacy and independent saves");
    }
}
