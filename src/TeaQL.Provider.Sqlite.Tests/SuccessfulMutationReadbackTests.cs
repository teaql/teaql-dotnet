using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class SuccessfulMutationReadbackTests
{
    private sealed class Sink : IDiagnosticSqlLogSink,IAppAuditEventSink
    {
        public readonly List<ExecutionMetadata> Sql = new();
        public readonly List<IReadOnlyDictionary<string,object?>> Audit = new();
        public void Write(ExecutionMetadata metadata)=>Sql.Add(metadata);
        public Task RecordAsync(IReadOnlyDictionary<string,object?> item,CancellationToken token=default)
        { Audit.Add(item);return Task.CompletedTask; }
    }

    [Theory]
    [InlineData(false,false)][InlineData(false,true)]
    [InlineData(true,false)][InlineData(true,true)]
    public async Task GraphReadbacksPreservePhysicalOrderRootAndSiblingPrivacy(bool updating,bool disableQueries)
    {
        const string secret="PRIVATE-READBACK-CHILD-CANARY";
        await using var db=new SqliteConnection("Data Source=:memory:");await db.OpenAsync();
        var descriptors=new[]{"Parent","Child"}.Select(name=>EntityDescriptor.New(name).TableName(name.ToLowerInvariant()+"_data")
            .Property(PropertyDescriptor.New("id",DataType.I64).Id())
            .Property(PropertyDescriptor.New("version",DataType.I64).Version())
            .Property(PropertyDescriptor.New("name",DataType.Text)).AuditMaskFields(new(){"name"})).ToArray();
        var module=new RuntimeModule();foreach(var entity in descriptors)module.Entity(entity);
        var provider=new SqlDataServiceExecutor(new SqliteDialect(),new SqliteTransport(db),new MetadataSchemaProvider(name=>descriptors.Single(e=>e.Name==name)));
        var sink=new Sink();var context=module.IntoContext().WithDataService(provider).WithDiagnosticSqlLogSink(sink).WithAppAuditEventSink(sink);
        await context.EnsureSchemaAsync();
        if(updating)foreach(var entity in descriptors)await context.RequireResource<IDataService>().MutateAsync(
            new InsertMutationRequest(new InsertCommand(entity.Name).Value("id",1L).Value("name","initial"),"seed readback fixture"));
        sink.Sql.Clear();sink.Audit.Clear();
        if(disableQueries)context.DisableQuerySqlLog();
        var outputs=new List<MutationResult>();
        await context.ExecuteGraphSaveAsync("save "+secret,async graph=>
        {
            MutationRequest Make(string entity,string name)=>updating
                ?new UpdateMutationRequest(new UpdateCommand(entity,Value.FromObject(1L)).ExpectedVersion(1L).Value("name",name),"prepared child")
                :new InsertMutationRequest(new InsertCommand(entity).Value("id",1L).Value("name",name),"prepared child");
            var parent=Make("Parent","ordinary parent");var child=Make("Child",secret);
            graph.Preflight(parent);graph.Preflight(child);
            var root=graph.Scope("Parent",1,null);
            outputs.Add(await graph.MutateAsync(parent,root));
            outputs.Add(await graph.MutateAsync(child,graph.Scope("Child",1,"authorize child",root)));
            Assert.Empty(sink.Audit);
            return true;
        });
        Assert.Equal(2,sink.Audit.Count);Assert.Equal(disableQueries?2:4,sink.Sql.Count);
        Assert.DoesNotContain(secret,JsonSerializer.Serialize(sink.Sql));
        Assert.DoesNotContain(secret,JsonSerializer.Serialize(sink.Audit));
        foreach(var result in outputs)
        {
            Assert.Equal(1UL,result.AffectedRows);Assert.Equal(updating?DataServiceOperation.Update:DataServiceOperation.Insert,result.Metadata.Operation);
            Assert.Equal(2,result.Metadata.Statements.Count);
            var write=result.Metadata.Statements[0];var read=result.Metadata.Statements[1];
            Assert.Equal(DataServiceOperation.Query,read.Operation);Assert.Equal(1,read.ResultCount);Assert.Null(read.AffectedRows);
            Assert.Equal("Parent",read.TraceChain[0].Name);
            Assert.Equal(write.MutationLineage,read.MutationLineage);
            Assert.Equal("select",read.TraceChain.Last().Name);
            Assert.Equal("save "+secret,read.Comment); // Trusted intent is not modified by log projection.
            Assert.True(write.EndedAt<=read.StartedAt);
            Assert.Equal(updating?2L:1L,result.PersistedRecord!["version"].TryI64());
            _=JsonSerializer.Serialize(result.Metadata); // No self-referential statement/provenance graph.
        }
        if(!disableQueries)Assert.Equal(new[]{outputs[0].Metadata.Operation,DataServiceOperation.Query,outputs[1].Metadata.Operation,DataServiceOperation.Query},sink.Sql.Select(e=>e.Operation));
        sink.Sql.Clear();context.EnableQuerySqlLog();
        await context.RequireResource<IDataService>().QueryAsync(new QueryRequest(new SelectQuery("Parent").Limit(1).Comment(secret).Purpose("independent query")));
        Assert.Equal(secret,Assert.Single(sink.Sql).Comment);
    }
}
