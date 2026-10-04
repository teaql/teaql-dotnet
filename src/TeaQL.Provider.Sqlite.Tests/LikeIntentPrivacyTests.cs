using System.Text.Json;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;
using Xunit.Abstractions;
using Record = TeaQL.Core.Record;

namespace TeaQL.Provider.Sqlite.Tests;

public class LikeIntentPrivacyTests(ITestOutputHelper output)
{
    private const string Secret = "PRIVATE-LIKE-OPERAND";
    private static readonly string[] Operators = { "contains", "notContains", "starts", "notStarts", "ends", "notEnds" };

    private sealed class Sink : IDiagnosticSqlLogSink
    {
        public readonly List<ExecutionMetadata> Entries = new();
        public readonly StringWriter Text = new();
        public void Write(ExecutionMetadata entry)
        { Entries.Add(entry); new TextDiagnosticSqlLogSink(Text).Write(entry); }
    }

    private sealed class Policy : IRequestPolicy
    {
        public readonly List<SelectQuery> Queries = new();
        public SelectQuery Apply(SelectQuery query) { Queries.Add(query.CloneForExecution()); return query; }
    }

    // Observe actual physical bindings in both connection and transaction paths;
    // never replace requests, results or runtime-produced trace frames.
    private sealed class Transport(ISqlTransport inner) : ISqlTransport, ISqlTransactionTransport
    {
        public readonly List<CompiledQuery> Reads = new();
        public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
        public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
        { Reads.Add(query); return inner.FetchAllSqlAsync(query); }
        public async Task<ISqlTransaction> BeginSqlAsync() =>
            new Transaction(this, await ((ISqlTransactionTransport)inner).BeginSqlAsync());
        private sealed class Transaction(Transport owner, ISqlTransaction inner) : ISqlTransaction
        {
            public Task<ulong> ExecuteSqlAsync(CompiledQuery query) => inner.ExecuteSqlAsync(query);
            public Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
            { owner.Reads.Add(query); return inner.FetchAllSqlAsync(query); }
            public Task CommitSqlAsync() => inner.CommitSqlAsync();
            public Task RollbackSqlAsync() => inner.RollbackSqlAsync();
            public void Dispose() => inner.Dispose();
        }
    }

    private static EntityDescriptor Entity(string name) => EntityDescriptor.New(name)
        .TableName("like_" + name.ToLowerInvariant())
        .Property(PropertyDescriptor.New("id", DataType.I64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version())
        .Property(PropertyDescriptor.New("parent_id", DataType.I64))
        .Property(PropertyDescriptor.New("name", DataType.Text))
        .Property(PropertyDescriptor.New("public_name", DataType.Text))
        .AuditMaskFields(new() { "name" });

    private static async Task<(SqliteConnection Connection, SqlDataServiceExecutor Provider,
        UserContext Context, Transport Transport, Sink Sink, Policy Policy)> Fixture(string operand = Secret)
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var child = Entity("Child");
        var parent = Entity("Parent").Relation(RelationDescriptor.New("children", "Child")
            .LocalKey("id").ForeignKey("parent_id").Many());
        var entities = new[] { parent, child };
        var transport = new Transport(new SqliteTransport(connection));
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), transport,
            new MetadataSchemaProvider(name => entities.SingleOrDefault(entity => entity.Name == name)));
        var sink = new Sink(); var policy = new Policy();
        var context = new RuntimeModule().Entity(parent).Entity(child).IntoContext()
            .WithDataService(provider).WithDiagnosticSqlLogSink(sink).WithRequestPolicy(policy)
            .EnableMutationSqlLog(false);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        foreach (var entity in entities)
            foreach (var (value, index) in new[] { operand + "-tail", "head-" + operand, "unrelated" }.Select((value, index) => (value, index)))
                await service.MutateAsync(new InsertMutationRequest(new InsertCommand(entity.Name)
                    .Value("id", Value.FromObject((long)index + 1)).Value("parent_id", Value.FromObject(1L))
                    .Value("name", Value.FromObject(value)).Value("public_name", Value.FromObject(value)), "seed LIKE privacy"));
        transport.Reads.Clear(); sink.Entries.Clear(); sink.Text.GetStringBuilder().Clear();
        return (connection, provider, context, transport, sink, policy);
    }

    private static Expr Predicate(string operation, string field, string operand) => operation switch
    {
        "contains" => Expr.Contain(field, operand), "notContains" => Expr.NotContain(field, operand),
        "starts" => Expr.BeginWith(field, operand), "notStarts" => Expr.NotBeginWith(field, operand),
        "ends" => Expr.EndWith(field, operand), "notEnds" => Expr.NotEndWith(field, operand),
        _ => throw new ArgumentOutOfRangeException(nameof(operation))
    };

    public static IEnumerable<object[]> Cases() =>
        from operation in Operators from field in new[] { "name", "public_name" }
        from transaction in new[] { false, true } from logging in new[] { false, true }
        select new object[] { operation, field, transaction, logging };

    [Theory, MemberData(nameof(Cases))]
    public async Task OriginalOperandRetainsFieldPolicyAndPhysicalBindings(string operation, string field, bool transaction, bool logging)
    {
        var f = await Fixture(); await using var connection = f.Connection;
        f.Context.EnableQuerySqlLog(logging);
        using var tx = transaction ? await f.Provider.BeginTransactionAsync() : null;
        var execution = tx is null ? f.Context.RequireResource<IDataService>() : new RuntimeDataService(tx, f.Context);
        var query = new SelectQuery("Parent").Filter(Predicate(operation, field, Secret)).OrderAsc("id").Limit(10)
            .Comment("load " + Secret).Purpose("inspect " + Secret);
        var before = JsonSerializer.Serialize(query);
        var request = f.Context.PrepareQueryRequest(new QueryRequest(query));
        var result = await execution.QueryAsync(request);
        var expected = operation switch {
            "contains" => new long?[] { 1, 2 }, "notContains" => new long?[] { 3 },
            "starts" => new long?[] { 1 }, "notStarts" => new long?[] { 2, 3 },
            "ends" => new long?[] { 2 }, _ => new long?[] { 1, 3 }
        };
        Assert.Equal(expected, result.Rows.Select(row => row["id"].TryI64()));
        var pattern = (operation.EndsWith("Starts", StringComparison.OrdinalIgnoreCase) ? "" : "%")
            + Secret + (operation.EndsWith("Ends", StringComparison.OrdinalIgnoreCase) ? "" : "%");
        var physical = Assert.Single(f.Transport.Reads);
        Assert.Equal(new[] { Value.FromObject(pattern) }, physical.Params);
        Assert.Equal(new[] { field == "name" ? SqlParameterLogPolicy.Masked : SqlParameterLogPolicy.Plain }, physical.ParameterLogPolicies);
        Assert.Equal(physical.Params, result.Metadata.Parameters);
        Assert.Equal(query.CommentText, result.Metadata.Comment); Assert.Equal(query.PurposeText, result.Metadata.Purpose);
        Assert.Equal(query.CommentText, Assert.Single(f.Policy.Queries).CommentText);
        Assert.Equal(query.PurposeText, f.Policy.Queries[0].PurposeText);
        Assert.Equal(query.FilterCondition, f.Policy.Queries[0].FilterCondition);
        Assert.Equal(before, JsonSerializer.Serialize(query));
        output.WriteLine(JsonSerializer.Serialize(new { operation, field, transaction, logging, input = Secret,
            bindings = physical.Params.Select(value => value.Raw), policies = physical.ParameterLogPolicies, resultCount = result.Rows.Count }));
        if (logging)
        {
            var entry = Assert.Single(f.Sink.Entries);
            var visible = field == "name" ? "[REDACTED]" : Secret;
            Assert.Equal("load " + visible, entry.Comment); Assert.Equal("inspect " + visible, entry.Purpose);
            Assert.Equal(new[] { "Parent", "Parent", "sqlite", "select" }, entry.TraceChain.Select(node => node.Name));
            Assert.Equal(result.Rows.Count, entry.ResultCount);
            if (field == "name") {
                Assert.DoesNotContain(Secret, JsonSerializer.Serialize(entry));
                Assert.DoesNotContain(Secret, f.Sink.Text.ToString());
            } else Assert.Contains(Secret, f.Sink.Text.ToString());
        }
        else { Assert.Empty(f.Sink.Entries); Assert.Equal("", f.Sink.Text.ToString()); }
        var independent = new SelectQuery("Parent").Limit(1).Comment("independent " + Secret).Purpose("no inherited operand");
        Assert.Single((await execution.QueryAsync(f.Context.PrepareQueryRequest(new QueryRequest(independent)))).Rows);
        if (logging) Assert.Equal(independent.CommentText, f.Sink.Entries.Last().Comment);
        if (tx is not null) await tx.CommitAsync();
    }

    [Theory]
    [InlineData(false, false)] [InlineData(false, true)]
    [InlineData(true, false)] [InlineData(true, true)]
    public async Task FutureChildOperandIsHiddenBeforeFirstRootSql(bool transaction, bool logging)
    {
        var f = await Fixture(); await using var connection = f.Connection;
        f.Context.EnableQuerySqlLog(logging);
        using var tx = transaction ? await f.Provider.BeginTransactionAsync() : null;
        var execution = tx is null ? f.Context.RequireResource<IDataService>() : new RuntimeDataService(tx, f.Context);
        var child = new SelectQuery("Child").Projects(new[] { "id", "name", "parent_id" })
            .Filter(Expr.BeginWith("name", Secret)).Limit(10)
            .Comment("load future " + Secret).Purpose("inspect future " + Secret);
        var query = new SelectQuery("Parent").Filter(Expr.Eq("id", 1L)).Limit(1).RelationQuery("children", child)
            .Comment(child.CommentText!).Purpose(child.PurposeText!);
        var before = JsonSerializer.Serialize(query);
        var result = await execution.QueryAsync(f.Context.PrepareQueryRequest(new QueryRequest(query)));
        var rows = Assert.IsType<Value.ListValue>(Assert.Single(result.Rows)["children"]).Values;
        Assert.Equal(Secret + "-tail", Assert.IsType<Value.ObjectValue>(Assert.Single(rows)).Value["name"].TryText());
        Assert.Equal(2, f.Transport.Reads.Count);
        Assert.DoesNotContain(f.Transport.Reads[0].Params, value => value.TryText() == Secret + "%");
        Assert.Contains(Value.FromObject(Secret + "%"), f.Transport.Reads[1].Params);
        Assert.Equal(SqlParameterLogPolicy.Masked, f.Transport.Reads[1].ParameterLogPolicies[0]);
        Assert.All(f.Policy.Queries, prepared => {
            Assert.Equal(query.CommentText, prepared.CommentText); Assert.Equal(query.PurposeText, prepared.PurposeText);
        });
        Assert.Equal(before, JsonSerializer.Serialize(query));
        if (logging)
        {
            Assert.Equal(2, f.Sink.Entries.Count);
            Assert.Equal(new[] { "", "children" }, f.Sink.Entries.Select(entry =>
                string.Join("/", entry.TraceChain.Where(node => node.Kind == "relation").Select(node => node.Name))));
            Assert.All(f.Sink.Entries, entry => {
                Assert.Equal("load future [REDACTED]", entry.Comment);
                Assert.Equal("inspect future [REDACTED]", entry.Purpose);
                Assert.Equal("Parent", entry.TraceChain[0].Name);
                Assert.DoesNotContain(Secret, JsonSerializer.Serialize(entry));
            });
            Assert.DoesNotContain(Secret, f.Sink.Text.ToString());
        }
        else Assert.Empty(f.Sink.Entries);
        var independent = new SelectQuery("Parent").Limit(1).Comment("independent " + Secret).Purpose("isolation");
        Assert.Single((await execution.QueryAsync(f.Context.PrepareQueryRequest(new QueryRequest(independent)))).Rows);
        if (logging) Assert.Equal(independent.CommentText, f.Sink.Entries.Last().Comment);
        if (tx is not null) await tx.CommitAsync();
    }

    [Theory]
    [InlineData(false, false)] [InlineData(false, true)]
    [InlineData(true, false)] [InlineData(true, true)]
    public async Task LiteralWildcardsAreNotStrippedAndRawLikeIsNotReinterpreted(bool transaction, bool logging)
    {
        const string literal = "%LITERAL_WILDCARD%\\";
        var f = await Fixture(literal); await using var connection = f.Connection;
        f.Context.EnableQuerySqlLog(logging);
        using var tx = transaction ? await f.Provider.BeginTransactionAsync() : null;
        var execution = tx is null ? f.Context.RequireResource<IDataService>() : new RuntimeDataService(tx, f.Context);
        foreach (var shape in new[] { "equal", "rawLike", "prefix" })
        {
            var operand = shape == "equal" ? literal + "-tail" : literal;
            var predicate = shape switch { "equal" => Expr.Eq("name", operand), "rawLike" => Expr.Like("name", operand),
                _ => Expr.BeginWith("name", operand) };
            var query = new SelectQuery("Parent").Filter(predicate).Limit(10)
                .Comment("private " + operand + "; public LITERAL_WILDCARD").Purpose("preserve exact operand");
            var before = JsonSerializer.Serialize(query);
            var result = await execution.QueryAsync(f.Context.PrepareQueryRequest(new QueryRequest(query)));
            Assert.Equal(shape == "prefix" ? 2 : 1, result.Rows.Count);
            Assert.Equal(new[] { Value.FromObject(shape == "prefix" ? operand + "%" : operand) }, f.Transport.Reads.Last().Params);
            Assert.Equal(before, JsonSerializer.Serialize(query));
            if (logging) {
                Assert.Equal("private [REDACTED]; public LITERAL_WILDCARD", f.Sink.Entries.Last().Comment);
                Assert.DoesNotContain(operand, f.Sink.Text.ToString());
                Assert.Contains("public LITERAL_WILDCARD", f.Sink.Text.ToString());
            }
        }
        Assert.Equal(logging ? 3 : 0, f.Sink.Entries.Count);
        if (tx is not null) await tx.CommitAsync();
    }

    [Theory]
    [InlineData(false, false)] [InlineData(false, true)]
    [InlineData(true, false)] [InlineData(true, true)]
    public async Task RewrittenAstDoesNotRetainStaleLikeProvenance(bool transaction, bool logging)
    {
        var f = await Fixture(); await using var connection = f.Connection;
        f.Context.EnableQuerySqlLog(logging);
        using var tx = transaction ? await f.Provider.BeginTransactionAsync() : null;
        var execution = tx is null ? f.Context.RequireResource<IDataService>() : new RuntimeDataService(tx, f.Context);
        var original = Assert.IsType<Expr.BinaryExpr>(Expr.BeginWith("name", Secret));
        var value = Assert.IsType<Expr.ValueExpr>(original.Right);
        Assert.DoesNotContain("LikeOperand", JsonSerializer.Serialize(value));
        foreach (var rewriteBinding in new[] { false, true })
        {
            var rewritten = rewriteBinding
                ? original with { Right = value with { NodeValue = Value.FromObject("unrelated%") } }
                : original with { Op = BinaryOp.Eq };
            var query = new SelectQuery("Parent").Filter(rewritten).Limit(10)
                .Comment("ordinary " + Secret).Purpose("do not infer a discarded operand");
            var result = await execution.QueryAsync(f.Context.PrepareQueryRequest(new QueryRequest(query)));
            Assert.Equal(rewriteBinding ? 1 : 0, result.Rows.Count);
            Assert.Equal(new[] { Value.FromObject(rewriteBinding ? "unrelated%" : Secret + "%") }, f.Transport.Reads.Last().Params);
            if (logging) Assert.Equal(query.CommentText, f.Sink.Entries.Last().Comment);
        }
        Assert.Equal(logging ? 2 : 0, f.Sink.Entries.Count);
        if (tx is not null) await tx.CommitAsync();
    }
}
