using TeaQL.Core;
using Xunit;
using Record = TeaQL.Core.Record;

namespace TeaQL.Sql.Tests;

public class SqlMaskingBindingsTests
{
    private readonly TestSqlDialect _dialect = new();
    private static EntityDescriptor Entity() => EntityDescriptor.New("Customer")
        .Property(PropertyDescriptor.New("id", DataType.I64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version())
        .Property(PropertyDescriptor.New("display_name", DataType.Text).ColumnName("displayName"))
        .Property(PropertyDescriptor.New("active", DataType.Bool))
        .Property(PropertyDescriptor.New("password", DataType.Text))
        .AuditMaskFields(new() { "display_name" });

    [Fact]
    public void CompilerPropagatesCanonicalFieldPolicyThroughPredicates()
    {
        var entity = Entity();
        var query = new SelectQuery("Customer").Filter(Expr.And(new List<Expr> {
            Expr.SoundLike("display_name", "Riverside"),
            Expr.InList("display_name", new[] { Value.FromObject("Ada"), Value.FromObject("Bert") }),
            Expr.Eq("active", true), Expr.Eq("password", "PASSWORD-CANARY")
        })).Limit(10);
        var compiled = _dialect.CompileSelect(entity, query);
        Assert.True(compiled.GeneratedSql);
        Assert.Equal(new[] { SqlParameterLogPolicy.Masked, SqlParameterLogPolicy.Masked,
            SqlParameterLogPolicy.Masked, SqlParameterLogPolicy.Plain, SqlParameterLogPolicy.Credential }, compiled.ParameterLogPolicies);
        Assert.Equal("Riverside", compiled.Params[0].TryText());
    }

    [Fact]
    public void CrudAndGuardsAttachFieldPoliciesWithoutChangingValues()
    {
        var entity = Entity();
        var insert = _dialect.CompileInsert(entity, new InsertCommand("Customer")
            .Value("id", 1L).Value("version", 1L).Value("display_name", "Riverside").Value("active", true));
        Assert.Equal(new[] { SqlParameterLogPolicy.Plain, SqlParameterLogPolicy.Plain,
            SqlParameterLogPolicy.Masked, SqlParameterLogPolicy.Plain }, insert.ParameterLogPolicies);
        var update = new UpdateCommand("Customer", Value.FromObject(1L)).ExpectedVersion(1).Value("display_name", "Lakeside");
        update.Guards["active"] = Value.FromObject(true);
        var compiled = _dialect.CompileUpdate(entity, update);
        Assert.Equal(SqlParameterLogPolicy.Masked, compiled.ParameterLogPolicies[0]);
        Assert.All(compiled.ParameterLogPolicies.Skip(1), p => Assert.Equal(SqlParameterLogPolicy.Plain, p));
        Assert.True(compiled.GeneratedSql);
        var delete = _dialect.CompileDelete(entity, new DeleteCommand("Customer", Value.FromObject(1L)).ExpectedVersion(2));
        Assert.All(delete.ParameterLogPolicies, p => Assert.Equal(SqlParameterLogPolicy.Plain, p));
    }

    [Fact]
    public void ScopeRestoresOnErrorAndUnclassifiedCustomAddsStayUnknown()
    {
        var values = SqlLogBindings.Create();
        var entity = Entity();
        Assert.Throws<SqlCompileException>(() => _dialect.CompileExpr(entity,
            Expr.Binary(Expr.Column("display_name"), BinaryOp.Eq, Expr.Column("missing")), values));
        _dialect.CompileExpr(entity, Expr.Value(Value.FromObject("UNCLASSIFIED")), values);
        values.Add(Value.FromObject("CUSTOM-DIALECT-BIND"));
        _dialect.CompileExpr(entity, Expr.Eq("active", true), values);
        Assert.Equal(new[] { SqlParameterLogPolicy.Unknown, SqlParameterLogPolicy.Unknown,
            SqlParameterLogPolicy.Plain }, SqlLogBindings.Policies(values));
    }

    [Fact]
    public void NestedEntityScopeCannotInheritOuterMask()
    {
        var inner = EntityDescriptor.New("Status").Property(PropertyDescriptor.New("name", DataType.Text));
        var expr = new Expr.SubQueryExpr(Expr.Column("display_name"), BinaryOp.In, inner,
            new SelectQuery("Status").Filter(Expr.Eq("name", "PUBLIC-STATUS")).Limit(1));
        var compiled = _dialect.CompileSelect(Entity(), new SelectQuery("Customer").Filter(expr).Limit(10));
        Assert.Equal(new[] { SqlParameterLogPolicy.Plain }, compiled.ParameterLogPolicies);
    }

    [Fact]
    public void EveryRawEscapeHatchIncludingNestedQueriesMarksSqlUntrusted()
    {
        var entity = Entity();
        var queries = new[] {
            new SelectQuery("Customer").RawSql("SELECT 'INLINE-CANARY'"),
            new SelectQuery("Customer").RawSqlSearchCriteria("displayName = 'INLINE-CANARY'"),
            new SelectQuery("Customer") { RawProjections = new() { new RawSqlProjection("extra", "'INLINE-CANARY'") } },
            new SelectQuery("Customer").DynamicPropertyRaw("extra", "'INLINE-CANARY'")
        };
        foreach (var query in queries)
        {
            Assert.False(_dialect.CompileSelect(entity, query).GeneratedSql);
            var nested = new Expr.SubQueryExpr(Expr.Column("display_name"), BinaryOp.In, entity, query);
            Assert.False(_dialect.CompileSelect(entity, new SelectQuery("Customer").Filter(nested).Limit(1)).GeneratedSql);
        }
    }

    [Fact]
    public void BatchCaseBindingsDistinguishIdsVersionsAndBusinessValues()
    {
        var entity = Entity();
        var batch = new BatchUpdateCommand {
            Entity = "Customer", BatchIds = new() { Value.FromObject(1L), Value.FromObject(2L) },
            UpdateFields = new() { "display_name" }, BatchExpectedVersions = new() { 1L, 2L },
            BatchValues = new() {
                new Record { ["display_name"] = Value.FromObject("Riverside") },
                new Record { ["display_name"] = Value.FromObject("Lakeside") }
            }
        };
        var compiled = _dialect.CompileBatchUpdate(entity, batch);
        Assert.True(compiled.GeneratedSql);
        Assert.Equal(compiled.Params.Count, compiled.ParameterLogPolicies.Count);
        for (int i = 0; i < compiled.Params.Count; i++)
            Assert.Equal(compiled.Params[i] is Value.TextValue ? SqlParameterLogPolicy.Masked : SqlParameterLogPolicy.Plain,
                compiled.ParameterLogPolicies[i]);
        var inserted = _dialect.CompileBatchInsert(entity, new BatchInsertCommand { Entity = "Customer", BatchValues = batch.BatchValues });
        Assert.Equal(new[] { SqlParameterLogPolicy.Masked, SqlParameterLogPolicy.Masked }, inserted.ParameterLogPolicies);
    }

    [Fact]
    public async Task CompilersDoNotShareScopeAcrossConcurrentQueries()
    {
        await Task.WhenAll(Enumerable.Range(0, 64).Select(i => Task.Run(() => {
            var field = i % 2 == 0 ? "display_name" : "active";
            var compiled = _dialect.CompileSelect(Entity(), new SelectQuery("Customer").Filter(Expr.Eq(field, "value")).Limit(1));
            Assert.Equal(i % 2 == 0 ? SqlParameterLogPolicy.Masked : SqlParameterLogPolicy.Plain,
                Assert.Single(compiled.ParameterLogPolicies));
        })));
    }
}
