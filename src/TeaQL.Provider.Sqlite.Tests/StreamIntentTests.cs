using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Provider.Sqlite.Tests;

public class StreamIntentTests
{
    private sealed class Sink : IDiagnosticSqlLogSink
    {
        public readonly List<ExecutionMetadata> Entries = new();
        public void Write(ExecutionMetadata entry) => Entries.Add(entry);
    }
    private sealed class ObservedTransport(SqliteTransport inner) : IStreamingSqlTransport
    {
        public int Opened, Closed;
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query) => inner.FetchAllSqlAsync(query);
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public async IAsyncEnumerable<Record> StreamSqlAsync(CompiledQuery query,
            [EnumeratorCancellation] CancellationToken token=default)
        {
            Opened++;
            try { await foreach(var row in inner.StreamSqlAsync(query,token)) yield return row; }
            finally { Closed++; }
        }
    }
    private sealed record Fixture(SqliteConnection Connection, SqlDataServiceExecutor Provider,
        UserContext Context, ObservedTransport Transport, Sink Log) : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => Connection.DisposeAsync();
        public IStreamQueryExecutor Streamer => (IStreamQueryExecutor)Context.RequireResource<IDataService>();
    }
    private static async Task<Fixture> Create()
    {
        var connection=new SqliteConnection("Data Source=:memory:"); await connection.OpenAsync();
        var entity=EntityDescriptor.New("Entry").TableName("entry_data")
            .Property(PropertyDescriptor.New("id",DataType.I64).Id())
            .Property(PropertyDescriptor.New("version",DataType.I64).Version())
            .Property(PropertyDescriptor.New("name",DataType.Text)).AuditMaskFields(new(){"name"})
            .Relation(RelationDescriptor.New("related","Entry").LocalKey("id").ForeignKey("id"));
        var inner=new SqliteTransport(connection);
        var schema=new MetadataSchemaProvider(name=>name=="Entry"?entity:null);
        var setup=new SqlDataServiceExecutor(new SqliteDialect(),inner,schema);
        var context=new RuntimeModule().Entity(entity).IntoContext().WithDataService(setup);
        await context.EnsureSchemaAsync();
        for(long id=1;id<=3;id++) await context.RequireResource<IDataService>().MutateAsync(new InsertMutationRequest(
            new InsertCommand("Entry").Value("id",Value.FromObject(id)).Value("version",Value.FromObject(1L))
                .Value("name",Value.FromObject("STREAM-PRIVATE-CANARY")),"prepare stream fixture"));
        var transport=new ObservedTransport(inner);
        var provider=new SqlDataServiceExecutor(new SqliteDialect(),transport,schema);
        var sink=new Sink(); context.WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        return new(connection,provider,context,transport,sink);
    }
    private static SelectQuery Query()=>new SelectQuery("Entry").Limit(10).OrderAsc("id")
        .Comment("stream STREAM-PRIVATE-CANARY").Purpose("inspect STREAM-PRIVATE-CANARY");
    private static async Task<List<Record>> Read(IAsyncEnumerable<StreamChunk> stream)
    {
        var rows=new List<Record>(); await foreach(var chunk in stream) rows.AddRange(chunk.Rows); return rows;
    }
    private static void AssertLog(Fixture f,string outcome,int count)
    {
        var entry=Assert.Single(f.Log.Entries);
        Assert.Equal(outcome,entry.ExecutionOutcome); Assert.Equal(count,entry.ResultCount);
        Assert.DoesNotContain("STREAM-PRIVATE-CANARY",JsonSerializer.Serialize(entry));
        Assert.Equal(new[]{"Entry","Entry","sqlite","select"},entry.TraceChain.Select(node=>node.Name));
        Assert.Equal(f.Transport.Opened,f.Transport.Closed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CreatingStreamCapturesMutableRequestBeforeFirstPoll(bool wrapped)
    {
        await using var f=await Create();
        var request=new QueryRequest(Query().Filter(Expr.Lte("id",2L)));
        var stream=(wrapped?f.Streamer:f.Provider).QueryStreamAsync(request,1);
        request.Query.Filter(Expr.Eq("id",999L)); request.Query.Projection.Add("missing_field");
        Assert.Equal(0,f.Transport.Opened); Assert.Empty(f.Log.Entries);
        var rows=await Read(stream);
        Assert.Equal(new long?[]{1,2},rows.Select(row=>row["id"].TryI64()));
        Assert.Equal(1,f.Transport.Closed);
    }

    [Fact]
    public async Task GeneratedFilterValuesAreCapturedAndActuallyApplied()
    {
        await using var f=await Create();
        var query=Query();
        query.Filters.Add(new FilterExpression {Operator="eq",Field="id",Expected=2L});
        query.Filters.Add(new FilterExpression {Operator="eq",Field="name",Expected="STREAM-PRIVATE-CANARY"});
        var request=new QueryRequest(query);
        var stream=f.Streamer.QueryStreamAsync(request,2);
        request.Query.Filters.Clear();
        Assert.Equal(2L,Assert.Single(await Read(stream))["id"].TryI64());
        AssertLog(f,"success",1);
    }

    [Theory]
    [InlineData("cancel")]
    [InlineData("break")]
    [InlineData("failure")]
    public async Task TerminalDiagnosticsRetainMaskedIntentAndCloseCursor(string mode)
    {
        await using var f=await Create();
        using var cancellation=new CancellationTokenSource();
        var stream=f.Streamer.QueryStreamAsync(new QueryRequest(Query().Filter(Expr.Eq("name","STREAM-PRIVATE-CANARY"))),1);
        if(mode=="failure")
        {
            await using var ddl=f.Connection.CreateCommand(); ddl.CommandText="DROP TABLE entry_data"; await ddl.ExecuteNonQueryAsync();
            await Assert.ThrowsAnyAsync<Exception>(()=>Read(stream));
            AssertLog(f,"failure",0);
        }
        else
        {
            await using(var cursor=stream.GetAsyncEnumerator(cancellation.Token))
            {
                Assert.True(await cursor.MoveNextAsync()); Assert.Single(cursor.Current.Rows);
                if(mode=="cancel")
                {
                    cancellation.Cancel();
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(async()=>{await cursor.MoveNextAsync();});
                }
            }
            AssertLog(f,"cancelled",1);
        }
    }

    [Fact]
    public async Task CountStreamRetainsRemovedChildPrivacy()
    {
        await using var f=await Create();
        var query=Query().RelationQuery("related",new SelectQuery("Entry").Filter(Expr.Eq("name","STREAM-PRIVATE-CANARY")));
        var rows=await Read(f.Streamer.QueryStreamAsync(new QueryRequest(query.ForExactCount()),1));
        Assert.Equal(3L,Assert.Single(rows)["count"].TryI64());
        AssertLog(f,"success",1);
    }

    [Fact]
    public async Task NeverPolledStreamHasNoSqlFact()
    {
        await using var f=await Create();
        _=f.Streamer.QueryStreamAsync(new QueryRequest(Query()),1);
        Assert.Equal(0,f.Transport.Opened); Assert.Empty(f.Log.Entries);
    }

    [Theory]
    [InlineData("relation")]
    [InlineData("aggregate")]
    [InlineData("facet")]
    [InlineData("enhancement")]
    [InlineData("group")]
    public async Task UnsupportedEnhancementsRejectBeforeOpeningCursor(string shape)
    {
        await using var f=await Create();
        var query=Query(); var child=new SelectQuery("Entry");
        switch(shape)
        {
            case "relation":query.RelationQuery("related",child);break;
            case "aggregate":query.RelationAggregates.Add(new RelationAggregate("related","count",child,true));break;
            case "facet":query.Facets.Add(new FacetRequest("facet","related",child,false));break;
            case "enhancement":query.ChildEnhancements.Add(child);break;
            case "group":query.ObjectGroupBys.Add(new ObjectGroupBy("group","related",child));break;
        }
        await Assert.ThrowsAsync<NotSupportedException>(()=>Read(f.Streamer.QueryStreamAsync(new QueryRequest(query),1)));
        Assert.Equal(0,f.Transport.Opened);Assert.Empty(f.Log.Entries);
    }

    [Fact]
    public async Task HardLimitRejectsBeforeOpeningCursor()
    {
        await using var f=await Create();
        await Assert.ThrowsAsync<InvalidOperationException>(()=>Read(f.Streamer.QueryStreamAsync(new QueryRequest(Query().Limit(10001)),1)));
        Assert.Equal(0,f.Transport.Opened);Assert.Empty(f.Log.Entries);
    }

    [Fact]
    public async Task IndependentAndRepeatedEnumerationsRetainTheirOwnInput()
    {
        await using var f=await Create();
        var request=new QueryRequest(Query().Filter(Expr.And(new List<Expr>{Expr.Lte("id",2L),Expr.Eq("name","STREAM-PRIVATE-CANARY")})));
        var first=f.Streamer.QueryStreamAsync(request,1);
        request.Query.Filter(Expr.And(new List<Expr>{Expr.Eq("id",3L),Expr.Eq("name","STREAM-PRIVATE-CANARY")}));
        var second=f.Streamer.QueryStreamAsync(request,1);
        request.Query.Filter(Expr.Eq("id",999L));
        var groups=await Task.WhenAll(Read(first),Read(second),Read(first));
        Assert.Equal(new long?[]{1,2},groups[0].Select(row=>row["id"].TryI64()));
        Assert.Equal(new long?[]{3},groups[1].Select(row=>row["id"].TryI64()));
        Assert.Equal(new long?[]{1,2},groups[2].Select(row=>row["id"].TryI64()));
        Assert.Equal(3,f.Transport.Opened); Assert.Equal(3,f.Transport.Closed);
        Assert.Equal(3,f.Log.Entries.Count);
        Assert.All(f.Log.Entries,entry=>Assert.Equal("success",entry.ExecutionOutcome));
        Assert.DoesNotContain("STREAM-PRIVATE-CANARY",JsonSerializer.Serialize(f.Log.Entries));
    }
}
