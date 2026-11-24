using Generated;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

// App-owned observers never manufacture expected requests, traces or seeds.
static class BootstrapChecks
{
    public static async Task RunOffAsync(string database)
    {
        await using var connection = new SqliteConnection($"Data Source={database}");
        await connection.OpenAsync();
        var module = GeneratedRuntimeModule.Module;
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
            new MetadataSchemaProvider(module.Metadata.GetEntity));
        var sink = new EvidenceSink();
        var context = module.IntoContext().WithDataService(provider)
            .WithDiagnosticSqlLogSink(sink).WithAppAuditEventSink(sink);
        var capture = new CapturingExecutor(provider, sink);
        context.InsertResource<ITransactionExecutor>(capture);
        sink.ActiveTransactions = () => capture.ActiveTransactions;
        await RunAsync(context, capture, sink, database, logging: false);
    }

    public static async Task RunAsync(UserContext context, CapturingExecutor capture,
        EvidenceSink sink, string database, bool logging)
    {
        var originalService = context.RequireResource<IDataService>();
        var originalActor = context.UserIdentifier;
        var queries = new BootstrapQueryObserver(originalService);
        context.InsertResource<IDataService>(queries);
        context.UserIdentifier = "bootstrap-caller";
        if (!logging) context.DisableQuerySqlLog().DisableMutationSqlLog();
        sink.CommittedProbe = async audit => {
            Verify.Equal("Platform", audit["entityType"], "bootstrap audit target type");
            Verify.Equal(1UL, Convert.ToUInt64(audit["entityId"]), "bootstrap assigned ID");
            // Test oracle only: read from another connection inside the sink.
            await using var observer = new SqliteConnection(new SqliteConnectionStringBuilder {
                DataSource=database, Mode=SqliteOpenMode.ReadOnly, DefaultTimeout=1 }.ToString());
            await observer.OpenAsync();
            using var statement = observer.CreateCommand();
            statement.CommandText = "SELECT version FROM platform_data WHERE id = 1";
            Verify.Equal(1L, Convert.ToInt64(await statement.ExecuteScalarAsync()), "bootstrap audit follows durable commit");
        };
        try
        {
            foreach(var assembly in new[]{typeof(Value).Assembly,typeof(QueryRequest).Assembly,
                typeof(UserContext).Assembly,typeof(SqlDataServiceExecutor).Assembly,typeof(SqliteTransport).Assembly}.Distinct())
                Console.WriteLine("BOOTSTRAP RUNTIME LOAD "+System.Text.Json.JsonSerializer.Serialize(new {
                    name=assembly.GetName().Name,location=assembly.Location,
                    sha256=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(assembly.Location))).ToLowerInvariant()
                }));
            await context.EnsureSchemaAsync();
            var firstWrites = capture.Commands.Count;
            Verify.That(firstWrites is 0 or 1, "generated bootstrap creates only the missing root");
            Verify.Equal(firstWrites, capture.MutationResults.Count, "actual bootstrap mutation results");
            Verify.Equal(firstWrites, sink.Audit.Count, "one committed audit per bootstrap write");
            Verify.Equal(1, queries.Facts.Count, "observe actual generated bootstrap lookup");
            var first = queries.Facts.Single();
            var expectedComment = first.Comment;
            Verify.That(!string.IsNullOrWhiteSpace(expectedComment) && !string.IsNullOrWhiteSpace(first.Purpose),
                "bootstrap lookup owns nonblank intent");
            VerifyQuery(first);
            for (var i=0;i<firstWrites;i++)
            {
                var command=capture.Commands[i]; var result=capture.MutationResults[i]; var audit=sink.Audit[i];
                Verify.That(!string.IsNullOrWhiteSpace(command.Comment), "bootstrap mutation request owns nonblank intent");
                Verify.Equal(("Platform",1L,"insert"),(command.Entity,command.Id,command.Operation), "actual bootstrap command identity");
                Verify.Equal("Platform#1:"+command.Comment, Verify.Shape(command.Lineage), "automatically assigned bootstrap lineage");
                foreach(var fact in Leaves(result)) {
                    // The mutation envelope contains both the physical write
                    // and its authoritative SELECT readback, as distinct leaves.
                    VerifyPhysical(fact,fact.Operation==DataServiceOperation.Query?"request":"entity",
                        fact.Operation==DataServiceOperation.Query?"select":"insert");
                    Verify.Equal(command.Comment,fact.AuditReason,"raw SQL retains actual bootstrap request reason");
                    Verify.Equal(Verify.Shape(command.Lineage),Verify.Shape(fact.MutationLineage),"raw readback preserves bootstrap responsibility");
                }
                Verify.Equal("teaql-generated-bootstrap",audit["actor"],"bootstrap actor");
                Verify.Equal("runtime-bootstrap",audit["category"],"bootstrap category");
                Verify.Equal(command.Comment,audit["reason"],"plain-schema bootstrap audit preserves request reason");
                Verify.Equal(Verify.Shape(command.Lineage),Verify.Shape((IEnumerable<TraceNode>)audit["traceChain"]!),"committed typed bootstrap lineage");
            }
            Verify.Equal(logging ? 1+2*firstWrites : 0,sink.Sql.Count,"bootstrap diagnostic switch does not suppress audit");
            if (logging) foreach(var fact in sink.Sql) {
                VerifyPhysical(fact,fact.Operation==DataServiceOperation.Query?"request":"entity",
                    fact.Operation==DataServiceOperation.Query?"select":"insert");
                if (fact.AuditReason!=null) {
                    Verify.Equal(firstWrites,1,"readback accompanies an actual write");
                    var reason=capture.Commands.Single().Comment.Replace("1","[REDACTED]",StringComparison.Ordinal);
                    Verify.Equal(reason,fact.AuditReason,"physical bootstrap readback preserves governed request reason");
                    Verify.Equal("Platform#1:"+reason,Verify.Shape(fact.MutationLineage),"physical readback keeps typed responsibility");
                }
            }
            capture.Clear();sink.Clear();queries.Facts.Clear();
            await context.EnsureSchemaAsync();
            Verify.Equal(0,capture.Commands.Count,"repeated bootstrap emits no mutation request");
            Verify.Equal(0,capture.MutationResults.Count,"repeated bootstrap emits no mutation result");
            Verify.Equal(0,sink.Audit.Count,"repeated bootstrap emits no audit");
            Verify.Equal(1,queries.Facts.Count,"repeated bootstrap retains a lookup");
            var repeat=queries.Facts.Single();
            Verify.Equal(expectedComment,repeat.Comment,"generated bootstrap owns stable lookup comment");
            Verify.Equal(first.Purpose,repeat.Purpose,"generated bootstrap owns stable lookup purpose");
            VerifyQuery(repeat);
            Verify.Equal(logging ? 1 : 0,sink.Sql.Count,"repeated bootstrap diagnostics respect switch");
            Verify.Equal("bootstrap-caller",context.UserIdentifier,"bootstrap restores caller actor");
            var root=await Q.Platforms().WithIdIs(1).Limit(1).Comment("reload bootstrapped root")
                .Purpose("verify generated identity and authoritative version").ExecuteForOneAsync(context)
                ??throw new Exception("generated root missing");
            Verify.Equal(1L,E.Platform(root).Id().Eval(),"generated E sees default root ID");
            Verify.Equal(1L,E.Platform(root).Version().Eval(),"generated E sees committed root version");
            Console.WriteLine($"TC-REQ-09 DOTNET GENERATED BOOTSTRAP PASSED logging={logging.ToString().ToLowerInvariant()} first_writes={firstWrites} repeat_writes=0 comment={first.Comment} purpose={first.Purpose}");
        }
        finally {
            context.InsertResource<IDataService>(originalService);
            context.UserIdentifier=originalActor;
            sink.CommittedProbe=null;
            capture.Clear();sink.Clear();
        }
    }

    private static IEnumerable<ExecutionMetadata> Leaves(ExecutionMetadata metadata) => metadata.Statements.Count==0
        ? new[]{metadata} : metadata.Statements.SelectMany(Leaves);
    private static void VerifyQuery(BootstrapQueryFact fact) {
        foreach(var physical in Leaves(fact.Metadata)) {
            Verify.Equal(fact.Comment,physical.Comment,"lookup physical SQL retains request-owned comment");
            Verify.Equal(fact.Purpose,physical.Purpose,"lookup physical SQL retains request-owned purpose");
            VerifyPhysical(physical,"request","select");
        }
    }
    private static void VerifyPhysical(ExecutionMetadata fact,string middle,string sqlKind) {
        Verify.That(fact.TraceChain.Select(node=>node.Kind).SequenceEqual(new[]{"operation",middle,"provider","sql"}),
            "canonical bootstrap SQL route: got "+string.Join(",",fact.TraceChain.Select(node=>node.Kind))+" expected operation,"+middle+",provider,sql");
        Verify.Equal("sqlite",fact.TraceChain[2].Name,"actual bootstrap provider");
        Verify.Equal(sqlKind,fact.TraceChain[3].Name,"actual bootstrap SQL kind");
        Verify.Equal("success",fact.ExecutionOutcome,"actual bootstrap execution outcome");
    }
}

sealed record BootstrapQueryFact(string Comment,string Purpose,ExecutionMetadata Metadata);
sealed class BootstrapQueryObserver(IDataService inner) : IDataService
{
    public List<BootstrapQueryFact> Facts { get; } = new();
    public DataServiceCapabilities Capabilities=>inner.Capabilities;
    public async Task<QueryResult> QueryAsync(QueryRequest request) {
        var comment=request.Comment;var purpose=request.Purpose;
        var result=await inner.QueryAsync(request);
        Facts.Add(new(comment,purpose,result.Metadata));return result;
    }
    public Task<MutationResult> MutateAsync(MutationRequest request)=>inner.MutateAsync(request);
}
