using System.Text.Json;
using Generated;
using TeaQL.DataService;
using TeaQL.Runtime;

static class ReadbackChecks
{
    public static async Task RunAsync(UserContext context,CapturingExecutor capture,EvidenceSink sink)
    {
        var nonce=Guid.NewGuid().ToString("N");
        var root=Q.CustomerOrders().Comment("prepare readback parent").Purpose("verify graph provenance").NewEntity(context)
            .UpdatePlatformId(1).UpdateOrderNumber("readback-"+nonce).UpdateDescription("new parent");
        var child=Q.OrderItems().Comment("prepare private child").Purpose("verify future sibling masking").NewEntity(context).UpdateName("pending");
        root.OrderItemList.Add(child);
        var payment=Q.Payments().Comment("prepare sibling").Purpose("verify sibling readback").NewEntity(context).UpdateReferenceCode("readback-payment-"+nonce);
        root.PaymentList.Add(payment);
        for(var round=0;round<2;round++)
        {
            var secret="PRIVATE-GRAPH-READBACK-"+nonce+"-"+round;
            root.UpdateDescription("saved parent "+round);
            child.UpdateName(secret).AuditAs("edit private item");
            payment.UpdateReferenceCode("readback-payment-"+nonce+"-"+round).AuditAs("authorize payment");
            capture.Clear();sink.Clear();
            await root.AuditAs("save graph "+secret).SaveAsync(context);
            Verify.Equal(3,capture.Commands.Count,"readbacks do not add mutation commands");
            Verify.Equal(3,sink.Audit.Count,"readbacks do not add committed audits");
            Verify.Equal(6,sink.Sql.Count,"three writes and three authoritative reads");
            Verify.That(!JsonSerializer.Serialize(sink.Sql).Contains(secret),"root and sibling readbacks mask future child secret");
            Verify.That(!JsonSerializer.Serialize(sink.Audit).Contains(secret),"committed audit masks child secret");
            for(var i=0;i<6;i+=2)
            {
                var write=sink.Sql[i];var read=sink.Sql[i+1];
                Verify.Equal(round==0?DataServiceOperation.Insert:DataServiceOperation.Update,write.Operation,"actual physical write");
                Verify.Equal(DataServiceOperation.Query,read.Operation,"actual readback SELECT");
                Verify.Equal("success",read.ExecutionOutcome,"successful physical readback");
                Verify.Equal(1,read.ResultCount,"one persisted row");
                Verify.That(read.AffectedRows is null,"readback does not count as write");
                Verify.Equal("CustomerOrder",read.TraceChain[0].Name,"readback keeps aggregate operation root");
                Verify.Equal("select",read.TraceChain.Last().Name,"readback physical SQL kind");
                Verify.Equal(Verify.Shape(write.MutationLineage),Verify.Shape(read.MutationLineage),"readback keeps branch reason");
                Verify.Equal(write.AuditReason,read.AuditReason,"readback keeps root intent");
                Verify.That(write.EndedAt<=read.StartedAt,"physical write precedes readback");
            }
            Console.WriteLine("READBACK EVIDENCE "+JsonSerializer.Serialize(new{round,rootId=E.CustomerOrder(root).Id().Eval(),childId=E.OrderItem(child).Id().Eval(),paymentId=E.Payment(payment).Id().Eval(),sql=sink.Sql,audits=sink.Audit.Count}));
            var loaded=await Q.OrderItems().WithIdIs(E.OrderItem(child).Id().Eval()!.Value).Limit(1)
                .Comment("reload private item").Purpose("check authoritative value and version").ExecuteForOneAsync(context)
                ??throw new Exception("missing item after save");
            Verify.Equal(secret,E.OrderItem(loaded).Name().Eval(),"masking does not alter business values");
            Verify.Equal((long)round+1,E.OrderItem(loaded).Version().Eval(),"readback version is authoritative");
            sink.Clear();
            _=await Q.CustomerOrders().WithIdIs(E.CustomerOrder(root).Id().Eval()!.Value).Limit(1)
                .Comment(secret).Purpose("prove graph redaction does not escape its request").ExecuteForOneAsync(context);
            Verify.Equal(secret,sink.Sql.Single().Comment,"independent query has independent privacy source");
        }
        Console.WriteLine("PASS: .NET generated successful readbacks, sibling privacy and independent request intent");
    }
}
