using Npgsql;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.MySql;
using TeaQL.Provider.PostgreSql;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Streaming.Tests;

public class ProviderMaskingTests
{
    [Fact]
    public async Task PostgreSqlMasksLiveQueryAndMutationWhenConfigured()
    {
        var url = LiveUrl("TEAQL_TEST_POSTGRES_URL");
        if (url is null) return;
        await using var dataSource = NpgsqlDataSource.Create(url);
        await VerifyAsync(new PostgreSqlDialect(), new PostgreSqlTransport(dataSource));
    }

    [Fact]
    public async Task MySqlMasksLiveQueryAndMutationWhenConfigured()
    {
        var url = LiveUrl("TEAQL_TEST_MYSQL_URL");
        if (url is null) return;
        using var transport = new MySqlTransport(url);
        await VerifyAsync(new MySqlDialect(), transport);
    }

    [Fact]
    public async Task MySqlEnsureSchemaRejectsSameNameWrongShapeIndexWhenConfigured()
    {
        var url = LiveUrl("TEAQL_TEST_MYSQL_URL");
        if (url is null) return;
        using var transport = new MySqlTransport(url);
        var dialect = new MySqlDialect();
        var table = "teaql_mask_" + Guid.NewGuid().ToString("N")[..12];
        var entity = EntityDescriptor.New("DriftProbe").TableName(table)
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("display_name", DataType.Text));
        try
        {
            await transport.ExecuteSqlAsync(new CompiledQuery(dialect.CompileCreateTable(entity), new List<Value>()));
            var indexName = "PK_" + table.ToUpperInvariant() + "_ID_VERSION";
            await transport.ExecuteSqlAsync(new CompiledQuery(
                $"CREATE UNIQUE INDEX {dialect.QuoteIdent(indexName)} ON {dialect.QuoteIdent(table)} " +
                "(`id`, `display_name`)", new List<Value>()));
            var context = new UserContext()
                .WithMetadata(new InMemoryMetadataStore().WithEntity(entity))
                .WithDataService(new SqlDataServiceExecutor(dialect, transport,
                    new MetadataSchemaProvider(name => name == entity.Name ? entity : null)));
            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => context.EnsureSchemaAsync());
            Assert.Contains("incompatible shape", error.Message);
        }
        finally
        {
            await transport.ExecuteSqlAsync(new CompiledQuery(
                "DROP TABLE IF EXISTS " + dialect.QuoteIdent(table), new List<Value>()));
        }
    }

    private static string? LiveUrl(string variable)
    {
        var url = Environment.GetEnvironmentVariable(variable);
        if (!string.IsNullOrWhiteSpace(url)) return url;
        if (string.Equals(Environment.GetEnvironmentVariable("TEAQL_REQUIRE_LIVE_DB"),
                "true", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"{variable} is required for live provider tests");
        return null;
    }

    private static async Task VerifyAsync(SqlDialect dialect, ISqlTransport transport)
    {
        var table = "teaql_mask_" + Guid.NewGuid().ToString("N")[..12];
        var entity = EntityDescriptor.New("Customer").TableName(table)
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Property(PropertyDescriptor.New("display_name", DataType.Text))
            .Property(PropertyDescriptor.New("public_address", DataType.Text))
            .Property(PropertyDescriptor.New("password_hash", DataType.Text))
            .AuditMaskFields(new() { "display_name", "password_hash" });
        var service = new SqlDataServiceExecutor(dialect, transport,
            new MetadataSchemaProvider(name => name == entity.Name ? entity : null));
        using var output = new StringWriter();
        var context = new UserContext()
            .WithMetadata(new InMemoryMetadataStore().WithEntity(entity))
            .WithDiagnosticSqlLogSink(new TextDiagnosticSqlLogSink(output))
            .WithDataService(service);
        try
        {
            await context.EnsureSchemaAsync();
            await context.EnsureSchemaAsync(); // index installation must be idempotent
            var command = new InsertCommand("Customer")
                .Value("id", 1L).Value("version", 1L)
                .Value("display_name", "Riverside")
                .Value("public_address", "1 Runtime Road")
                .Value("password_hash", "PASSWORD-CANARY");
            command.TraceChain.Add(new TraceNode("Customer", null, "what: create masked customer"));
            var runtimeService = new RuntimeDataService(service, context);
            await runtimeService.MutateAsync(new InsertMutationRequest(command));
            var query = new SelectQuery("Customer")
                .Filter(Expr.And(new List<Expr> {
                    Expr.Eq("display_name", "Riverside"),
                    Expr.Eq("public_address", "1 Runtime Road")
                })).Limit(1);
            var result = await runtimeService.QueryAsync(new QueryRequest(query) {
                Comment = "what: read masked customer",
                Purpose = "why: verify live-provider SQL masking"
            });
            Assert.Single(result.Rows);
            Assert.Equal("Riverside", result.Rows[0]["display_name"].TryText());
            Assert.Equal("1 Runtime Road", result.Rows[0]["public_address"].TryText());
            var logged = output.ToString();
            Assert.Contains("INSERT", logged);
            Assert.Contains("SELECT", logged);
            Assert.Contains("Ri*****de", logged);
            Assert.Contains("1 Runtime Road", logged);
            Assert.Contains("what: read masked customer", logged);
            Assert.Contains("why: verify live-provider SQL masking", logged);
            Assert.DoesNotContain("Riverside", logged);
            Assert.DoesNotContain("PASSWORD-CANARY", logged);
        }
        finally
        {
            await transport.ExecuteSqlAsync(new CompiledQuery(
                "DROP TABLE IF EXISTS " + dialect.QuoteIdent(table), new List<Value>()));
        }
    }
}
