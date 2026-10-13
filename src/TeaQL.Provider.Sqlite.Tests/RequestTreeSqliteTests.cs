using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class RequestTreeSqliteTests
{
    private sealed class Sink : IDiagnosticSqlLogSink
    {
        public List<ExecutionMetadata> Entries { get; } = new();
        public void Write(ExecutionMetadata metadata) => Entries.Add(metadata);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IdSetCollectionOwnsHelperAndCacheHitsDoNotInventSql(bool logging)
    {
        using var connection = new SqliteConnection("Data Source=:memory:"); await connection.OpenAsync();
        var descriptor = EntityDescriptor.New("School").TableName("id_tree_schools")
            .Property(PropertyDescriptor.New("id",DataType.I64).Id())
            .Property(PropertyDescriptor.New("version",DataType.I64).Version())
            .Property(PropertyDescriptor.New("name",DataType.Text));
        var schema = new MetadataSchemaProvider(name=>name=="School"?descriptor:null);
        var provider = new SqlDataServiceExecutor(new SqliteDialect(),new SqliteTransport(connection),schema);
        var sink = new Sink();
        var context = new RuntimeModule().Entity(descriptor).IntoContext().WithDataService(provider)
            .WithIdSetStore(new InMemoryIdSetStore()).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        foreach (var id in new[]{1L,2L,3L})
            await service.MutateAsync(new InsertMutationRequest(new InsertCommand("School")
                .Value("id",Value.FromObject(id)).Value("version",Value.FromObject(1L))
                .Value("name",Value.FromObject("School "+id)),"seed helper test"));
        context.EnableQuerySqlLog(logging); sink.Entries.Clear();
        var totals = await service.QueryAsync(new QueryRequest(new SelectQuery("School").Count("schoolCount").Sum("id","idTotal")
            .Comment("calculate school totals").Purpose("render totals")));
        Assert.Equal(3L,Assert.Single(totals.Rows)["schoolCount"].TryI64());
        Assert.Equal(6L,totals.Rows.Single()["idTotal"].TryI64());
        Assert.Contains("COUNT(",totals.Metadata!.ParameterizedQuery!.ToUpperInvariant());
        Assert.Contains("SUM(",totals.Metadata.ParameterizedQuery!.ToUpperInvariant());
        Assert.Equal(new[]{"render totals","calculate school totals"},totals.Metadata.RequestTraceChain.Select(node=>node.Comment));
        Assert.Equal(logging?1:0,sink.Entries.Count);
        Console.WriteLine($"RTC_COM_07_AGGREGATE_PASS logging={logging.ToString().ToLowerInvariant()} expressions=2 statements=1");
        sink.Entries.Clear();
        QueryRequest Page(int offset) => new(new SelectQuery("School").Project("id").Project("name")
            .Comment("load school page").Purpose("render school page").OrderDesc("id").Offset(offset).Limit(1)
            .OptimizePaginationWithIdSet("request-tree-schools",60,10));
        var first = await RuntimeDataService.FetchPageAsync(service,context,Page(1));
        Assert.Equal(2L,first.Single()["id"].TryI64()); Assert.Equal(3L,first.TotalCount);
        Assert.Equal("ID_SET_BUILD",context.IdSetPlan); Assert.Equal(3UL,context.IdSetCount);
        Assert.Equal(logging?2:0,sink.Entries.Count);
        if (logging)
        {
            Assert.Equal(new[]{"render school page","load school page","collect School IDs for pagination"},
                sink.Entries[0].RequestTraceChain.Select(node=>node.Comment));
            Assert.Equal(new[]{"render school page","load school page"},sink.Entries[1].RequestTraceChain.Select(node=>node.Comment));
        }
        sink.Entries.Clear();
        var second = await RuntimeDataService.FetchPageAsync(service,context,Page(0));
        Assert.Equal(3L,second.Single()["id"].TryI64()); Assert.Equal(3L,second.TotalCount);
        Assert.Equal("ID_SET_HIT",context.IdSetPlan); Assert.Equal(3UL,context.IdSetCount);
        Assert.Equal(logging?1:0,sink.Entries.Count);
        if (logging)
        {
            Assert.Equal(new[]{"render school page","load school page"},sink.Entries[0].RequestTraceChain.Select(node=>node.Comment));
            Assert.DoesNotContain(sink.Entries[0].TraceChain,node=>node.Kind=="relation");
            Assert.DoesNotContain("COUNT(",sink.Entries[0].ParameterizedQuery!.ToUpperInvariant());
        }
        Console.WriteLine($"RTC_COM_07_CACHE_PASS logging={logging.ToString().ToLowerInvariant()} accuracy=EXACT hit_statements=1");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PrefixesAndEscapedFacetAliasesPreserveBindingsUnderPrivacyProjection(bool logging)
    {
        using var connection = new SqliteConnection("Data Source=:memory:"); await connection.OpenAsync();
        var school = EntityDescriptor.New("School").TableName("prefix_schools")
            .Property(PropertyDescriptor.New("id",DataType.I64).Id())
            .Property(PropertyDescriptor.New("version",DataType.I64).Version())
            .Property(PropertyDescriptor.New("name",DataType.Text)).AuditMaskFields(new List<string>{"name"})
            .Property(PropertyDescriptor.New("school_type",DataType.I64))
            .Relation(RelationDescriptor.New("schoolType","SchoolType").LocalKey("school_type").ForeignKey("id"));
        var type = EntityDescriptor.New("SchoolType").TableName("prefix_types")
            .Property(PropertyDescriptor.New("id",DataType.I64).Id()).Property(PropertyDescriptor.New("version",DataType.I64).Version());
        var provider = new SqlDataServiceExecutor(new SqliteDialect(),new SqliteTransport(connection),
            new MetadataSchemaProvider(name=>name=="School"?school:name=="SchoolType"?type:null));
        var sink = new Sink();
        var context = new RuntimeModule().Entity(school).Entity(type).IntoContext().WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        const string secret = "TRACE-PRIVATE-CANARY";
        await service.MutateAsync(new InsertMutationRequest(new InsertCommand("SchoolType").Value("id",Value.FromObject(1001L)).Value("version",Value.FromObject(1L)),"seed type"));
        foreach (var (id,name) in new[]{(1L,secret),(2L,"Other school")})
            await service.MutateAsync(new InsertMutationRequest(new InsertCommand("School").Value("id",Value.FromObject(id)).Value("version",Value.FromObject(1L))
                .Value("name",Value.FromObject(name)).Value("school_type",Value.FromObject(1001L)),"seed privacy fixture"));
        context.EnableQuerySqlLog(logging);
        foreach (var privateInput in new[]{false,true})
        {
            sink.Entries.Clear();
            var alias = "choices\"\\\n" + (privateInput ? secret : "");
            var nativeComment = "FacetBySchoolTypeAs(\"choices\\\"\\\\\\n" + (privateInput ? secret : "") + "\")";
            var comment = privateInput ? "what: load "+secret : "what: load schools";
            var purpose = privateInput ? "why: inspect "+secret : "why: inspect schools";
            var child = new SelectQuery("SchoolType").Limit(10).Count("schoolCount").Comment(privateInput ? "load "+secret+" facets" : "load type facets");
            var query = new SelectQuery("School").Limit(10).Comment(comment).Purpose(purpose);
            if (privateInput) query.Filter(Expr.Eq("name",secret));
            var operation = new QueryOperation("facet","FacetBySchoolTypeAs",alias,"school_type");
            Assert.Equal(nativeComment,operation.Render());
            query.Facets.Add(new(alias,"schoolType",child,false) { Operation = operation });
            var before = JsonSerializer.Serialize(query);
            var request = new QueryRequest(query);
            var result = await service.QueryAsync(request);
            Assert.Equal(privateInput?1:2,result.Rows.Count);
            Assert.Equal(privateInput?1L:2L,Assert.Single(result.Facets[alias])["schoolCount"].TryI64());
            if (privateInput) Assert.Equal(secret,result.Rows.Single()["name"].TryText());
            Assert.Equal(comment,request.Comment); Assert.Equal(purpose,request.Purpose);
            Assert.Equal(before,JsonSerializer.Serialize(query));
            Assert.Equal(logging?3:0,sink.Entries.Count);
            if (logging)
            {
                var safeComment = privateInput ? "what: load [REDACTED]" : comment;
                var safePurpose = privateInput ? "why: inspect [REDACTED]" : purpose;
                var chains = sink.Entries.Select(entry=>entry.RequestTraceChain.Select(node=>node.Comment).ToArray()).ToList();
                Assert.Contains(chains,chain=>chain.SequenceEqual(new[]{safePurpose,safeComment}));
                foreach (var local in new[]{"aggregate school_type",privateInput?"load [REDACTED] facets":"load type facets"})
                    Assert.Contains(chains,chain=>chain.SequenceEqual(new[]{safePurpose,safeComment,nativeComment.Replace(secret,"[REDACTED]"),local}));
                if (privateInput) Assert.DoesNotContain(secret,JsonSerializer.Serialize(sink.Entries));
            }
        }
        Console.WriteLine($"RTC_COM_09_12_PASS logging={logging.ToString().ToLowerInvariant()} plain=true private=true escaped=true bindings=true");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NamedAndUnnamedPageCountRetainIntentWithoutFakeRelations(bool logging)
    {
        using var connection = new SqliteConnection("Data Source=:memory:"); await connection.OpenAsync();
        var school = EntityDescriptor.New("School").TableName("count_schools")
            .Property(PropertyDescriptor.New("id",DataType.I64).Id())
            .Property(PropertyDescriptor.New("version",DataType.I64).Version());
        var provider = new SqlDataServiceExecutor(new SqliteDialect(),new SqliteTransport(connection),
            new MetadataSchemaProvider(name=>name=="School"?school:null));
        var sink = new Sink();
        var context = new RuntimeModule().Entity(school).IntoContext().WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        await service.MutateAsync(new InsertMutationRequest(new InsertCommand("School")
            .Value("id",Value.FromObject(1L)).Value("version",Value.FromObject(1L)),"seed count fixture"));
        context.EnableQuerySqlLog(logging);
        foreach (var alias in new string?[]{null,"totalSchools'\\tail"})
        {
            sink.Entries.Clear();
            var query = new SelectQuery("School").Project("id").Limit(1)
                .Comment("load schools").Purpose("render school page");
            if (alias != null) query.PageCountOperation = new("count","CountAs",alias);
            var page = await RuntimeDataService.FetchPageAsync(service,context,new QueryRequest(query));
            Assert.Single(page); Assert.Equal(1L,page.TotalCount);
            if (alias != null) Assert.Equal(1L,page.Aggregations[alias].TryI64());
            else Assert.Empty(page.Aggregations);
            Assert.Equal(logging?2:0,sink.Entries.Count);
            if (logging)
            {
                var local = alias == null ? "count matching rows" : "CountAs(\"totalSchools'\\\\tail\")";
                Assert.Equal(new[]{"render school page","load schools",local},sink.Entries[0].RequestTraceChain.Select(node=>node.Comment));
                Assert.DoesNotContain(sink.Entries[0].TraceChain,node=>node.Kind=="relation");
            }
        }
        Console.WriteLine($"RTC_COM_05_PASS logging={logging.ToString().ToLowerInvariant()} named=true unnamed=true");
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task RealRelationAndFacetStatementsOwnNativeAndLocalComments(bool explicitComment, bool relationLogging)
    {
        using var connection = new SqliteConnection("Data Source=:memory:"); await connection.OpenAsync();
        var school = EntityDescriptor.New("School").TableName("school_data")
            .Property(PropertyDescriptor.New("id",DataType.I64).Id())
            .Property(PropertyDescriptor.New("version",DataType.I64).Version())
            .Property(PropertyDescriptor.New("name",DataType.Text))
            .Property(PropertyDescriptor.New("school_type",DataType.I64))
            .Property(PropertyDescriptor.New("capacity",DataType.I64))
            .AuditMaskFields(new List<string>{"name"})
            .Relation(RelationDescriptor.New("schoolType","SchoolType").LocalKey("school_type").ForeignKey("id"));
        var type = EntityDescriptor.New("SchoolType").TableName("school_type_data")
            .Property(PropertyDescriptor.New("id",DataType.I64).Id())
            .Property(PropertyDescriptor.New("version",DataType.I64).Version())
            .Property(PropertyDescriptor.New("name",DataType.Text))
            .Relation(RelationDescriptor.New("schools","School").LocalKey("id").ForeignKey("school_type").Many());
        var module = new RuntimeModule().Entity(school).Entity(type);
        var schema = new MetadataSchemaProvider(name=>name=="School"?school:name=="SchoolType"?type:null);
        var provider = new SqlDataServiceExecutor(new SqliteDialect(),new SqliteTransport(connection),schema);
        var sink = new Sink(); var context = module.IntoContext().WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        await service.MutateAsync(new InsertMutationRequest(new InsertCommand("SchoolType")
            .Value("id",Value.FromObject(1001L)).Value("version",Value.FromObject(1L)).Value("name",Value.FromObject("Primary")),"seed school type"));
        await service.MutateAsync(new InsertMutationRequest(new InsertCommand("School")
            .Value("id",Value.FromObject(1L)).Value("version",Value.FromObject(1L)).Value("name",Value.FromObject("PRIVATE-SCHOOL"))
            .Value("school_type",Value.FromObject(1001L)).Value("capacity",Value.FromObject(50L)),"seed school"));
        context.EnableQuerySqlLog(relationLogging); sink.Entries.Clear();
        var template = new SelectQuery("SchoolType").Project("id").Project("version").Project("name");
        var local = explicitComment?"load visible school types":"load SchoolType";
        if (explicitComment) template.Comment(local);
        var query = new SelectQuery("School").Comment("load PRIVATE-SCHOOL").Purpose("render school list")
            .Filter(Expr.Eq("name","PRIVATE-SCHOOL"));
        query.RelationLoads.Add(new("schoolType",template) {
            Operation = new("relation","SelectSchoolTypeWith",Relation:"school_type") });
        var facetValues = template.Copy().Count("schoolCount").Comment("load type facets");
        query.Facets.Add(new("types","schoolType",facetValues,true) {
            Operation = new("facet","FacetBySchoolTypeAs","types","school_type") });
        var result = await service.QueryAsync(new QueryRequest(query));
        Assert.Single(result.Rows);
        Assert.Single(result.Facets["types"]);
        Assert.Equal(1L,result.Facets["types"].Single()["schoolCount"].TryI64());
        Assert.Equal(relationLogging?4:0,sink.Entries.Count);
        if (relationLogging)
        {
        var relation = sink.Entries[1];
        Assert.Equal(new[]{"render school list","load [REDACTED]","SelectSchoolTypeWith(...)",local},relation.RequestTraceChain.Select(node=>node.Comment));
        Assert.Equal(local,relation.Comment);
        var membership = sink.Entries[2]; var materialization = sink.Entries[3];
        Assert.Contains("schoolCount",membership.ParameterizedQuery);
        Assert.Equal("FacetBySchoolTypeAs(\"types\")",membership.RequestTraceChain[2].Comment);
        Assert.Equal("FacetBySchoolTypeAs(\"types\")",materialization.RequestTraceChain[2].Comment);
        Assert.Equal("aggregate school_type",membership.Comment);
        Assert.Equal("load type facets",materialization.Comment);
        Assert.All(sink.Entries,entry=> {
            Assert.Equal(entry.RequestTraceChain.Last().Comment,entry.Comment);
            Assert.Equal("render school list",entry.Purpose);
            Assert.DoesNotContain("PRIVATE-SCHOOL",JsonSerializer.Serialize(entry));
            Assert.Contains(entry.TraceChain,node=>node.Kind=="provider");
        });
        }
        Assert.Equal(explicitComment?local:null,template.CommentText);
        Assert.DoesNotContain("RequestScope",JsonSerializer.Serialize(template));
        Console.WriteLine($"RTC_COM_02_PASS logging={relationLogging.ToString().ToLowerInvariant()} explicit={explicitComment.ToString().ToLowerInvariant()}");
        foreach (var blank in new[]{"", " \t", "\u0085"})
        {
            sink.Entries.Clear();
            var invalid = new SelectQuery("School").Comment("load schools").Purpose("validate nested input");
            invalid.RelationLoads.Add(new("schoolType",template.Copy().Comment(blank)) {
                Operation = new("relation","SelectSchoolTypeWith",Relation:"school_type") });
            Assert.Equal("comment",Assert.Throws<RequestIntentException>(() => new QueryRequest(invalid)).Field);
            Assert.Empty(sink.Entries);
            Console.WriteLine($"RTC_COM_04_RELATION_PASS logging={relationLogging.ToString().ToLowerInvariant()} invalid_child_provider=0");
            var invalidFacet = new SelectQuery("School").Comment("load schools").Purpose("validate facet input");
            invalidFacet.Facets.Add(new("types","schoolType",template.Copy().Count("schoolCount").Comment(blank),true) {
                Operation = new("facet","FacetBySchoolTypeAs","types","school_type") });
            Assert.Equal("comment",Assert.Throws<RequestIntentException>(() => new QueryRequest(invalidFacet)).Field);
            Assert.Empty(sink.Entries);
            Console.WriteLine($"RTC_COM_04_FACET_PASS logging={relationLogging.ToString().ToLowerInvariant()} invalid_value_provider=0");
        }
        foreach (var logging in new[]{true,false})
        {
            context.EnableQuerySqlLog(logging);
            foreach (var comment in new[]{"sum visible capacity",""," \t","\u0085"})
            {
                sink.Entries.Clear();
                var sum = new SelectQuery("School").Sum("capacity","sumCapacity").Comment(comment);
                var values = template.Copy().Count("schoolCount").Comment("load type facets");
                values.RelationAggregates.Add(new("schools","sumCapacity",sum,true) {
                    Operation = new("aggregate","SumCapacityOfSchoolsAs","sumCapacity","schools") });
                var withAggregate = new SelectQuery("School").Comment("load schools").Purpose("render schools");
                withAggregate.Facets.Add(new("types","schoolType",values,true) {
                    Operation = new("facet","FacetBySchoolTypeAs","types","school_type") });
                if (comment == "sum visible capacity")
                {
                    var loaded = await service.QueryAsync(new QueryRequest(withAggregate));
                    Assert.Equal(50L,loaded.Facets["types"].Single()["sumCapacity"].TryI64());
                    Assert.Equal(logging?4:0,sink.Entries.Count);
                    if (logging)
                    {
                        var fact = Assert.Single(sink.Entries.Where(entry => entry.ParameterizedQuery!.Contains("SUM(")));
                        Assert.Equal(new[]{"render schools","load schools","FacetBySchoolTypeAs(\"types\")",
                            "aggregate school_type",comment},fact.RequestTraceChain.Select(node => node.Comment));
                        Assert.Equal(comment,fact.Comment);
                    }
                }
                else
                {
                    Assert.Equal("comment",Assert.Throws<RequestIntentException>(() => new QueryRequest(withAggregate)).Field);
                    Assert.Empty(sink.Entries);
                }
            }
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task StreamedNamedCountPreservesRequestOwnerAndOnePhysicalStatement(bool logging)
    {
        using var connection = new SqliteConnection("Data Source=:memory:"); await connection.OpenAsync();
        var descriptor = EntityDescriptor.New("School").TableName("stream_tree_schools")
            .Property(PropertyDescriptor.New("id",DataType.I64).Id())
            .Property(PropertyDescriptor.New("version",DataType.I64).Version());
        var schema = new MetadataSchemaProvider(name=>name=="School"?descriptor:null);
        var provider = new SqlDataServiceExecutor(new SqliteDialect(),new SqliteTransport(connection),schema);
        var sink = new Sink();
        var context = new RuntimeModule().Entity(descriptor).IntoContext().WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        foreach (var id in new[]{1L,2L}) await service.MutateAsync(new InsertMutationRequest(new InsertCommand("School")
            .Value("id",Value.FromObject(id)).Value("version",Value.FromObject(1L)),"seed cursor test"));
        context.EnableQuerySqlLog(logging); sink.Entries.Clear();
        var root = new QueryRequest(new SelectQuery("School").Comment("load school totals").Purpose("render totals"));
        var countQuery = root.Query.ForExactCount();
        countQuery.AggregateItems.Clear(); countQuery.Count("totalSchools");
        var counted = root.DeriveAt(countQuery,
            root.Scope.DeriveOperation(new("count","CountAs","totalSchools")));
        var chunks = new List<StreamChunk>();
        await foreach (var chunk in ((IStreamQueryExecutor)service).QueryStreamAsync(counted,1)) chunks.Add(chunk);
        Assert.Equal(2L,Assert.Single(Assert.Single(chunks).Rows)["totalSchools"].TryI64());
        Assert.Equal(logging?1:0,sink.Entries.Count);
        if (logging)
        {
            var entry = Assert.Single(sink.Entries);
            Assert.Equal(new[]{"render totals","load school totals","CountAs(\"totalSchools\")"},entry.RequestTraceChain.Select(node=>node.Comment));
            Assert.Equal("success",entry.ExecutionOutcome);
            Assert.DoesNotContain(entry.TraceChain,node=>node.Kind=="relation");
        }
        Assert.Equal("load school totals",root.Scope.Comment);
    }
}
