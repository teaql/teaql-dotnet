using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class TraceChainSqliteTests
{
    private sealed class Sink : IDiagnosticSqlLogSink
    {
        public readonly List<ExecutionMetadata> Entries = new();
        public void Write(ExecutionMetadata metadata) => Entries.Add(metadata);
    }

    private static EntityDescriptor Entity(string name) => EntityDescriptor.New(name)
        .TableName(name.ToLowerInvariant() + "_data")
        .Property(PropertyDescriptor.New("id", DataType.I64).Id())
        .Property(PropertyDescriptor.New("version", DataType.I64).Version());

    private static async Task<(SqliteConnection Connection, SqlDataServiceExecutor Provider,
        UserContext Context, Sink Log)> Fixture()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var region = Entity("Region");
        var organization = Entity("Organization")
            .Property(PropertyDescriptor.New("regionId", DataType.I64))
            .Relation(RelationDescriptor.New("region", "Region").LocalKey("regionId").ForeignKey("id"));
        var platform = Entity("Platform")
            .Property(PropertyDescriptor.New("organizationId", DataType.I64))
            .Relation(RelationDescriptor.New("organization", "Organization").LocalKey("organizationId").ForeignKey("id"));
        var school = Entity("School")
            .Property(PropertyDescriptor.New("platformId", DataType.I64))
            .Relation(RelationDescriptor.New("platform", "Platform").LocalKey("platformId").ForeignKey("id"));
        var module = new RuntimeModule().Entity(region).Entity(organization).Entity(platform).Entity(school);
        var descriptors = new[] { region, organization, platform, school };
        var schemas = new MetadataSchemaProvider(name => descriptors.FirstOrDefault(entity => entity.Name == name));
        var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection), schemas);
        var sink = new Sink();
        var context = module.IntoContext().WithDataService(provider).WithDiagnosticSqlLogSink(sink);
        await context.EnsureSchemaAsync();
        var service = context.RequireResource<IDataService>();
        foreach (var (entity, foreign) in new[] { ("Region", ""), ("Organization", "regionId"),
            ("Platform", "organizationId"), ("School", "platformId") })
        {
            var command = new InsertCommand(entity).Value("id", Value.FromObject(1L))
                .Value("version", Value.FromObject(1L));
            if (foreign.Length > 0) command.Value(foreign, Value.FromObject(1L));
            await service.MutateAsync(new InsertMutationRequest(command, "prepare trace fixture"));
        }
        sink.Entries.Clear();
        return (connection, provider, context, sink);
    }

    [Fact]
    public async Task PhysicalRootPathNamesTheEntityWithoutDuplicatingIntent()
    {
        var fixture = await Fixture();
        await using var connection = fixture.Connection;
        var result = await fixture.Provider.QueryAsync(new QueryRequest(new SelectQuery("School").Limit(1)
            .Comment("load school").Purpose("verify physical path")));
        Assert.Single(result.Rows);
        Assert.Equal(new[] { "School", "School", "sqlite", "select" },
            result.Metadata.TraceChain.Select(node => node.Name));
        Assert.All(result.Metadata.TraceChain, node => Assert.Equal("", node.Comment));
        Assert.Equal("load school", result.Metadata.Comment);
        Assert.Equal("verify physical path", result.Metadata.Purpose);
    }

    [Fact]
    public async Task RealThreeLevelFailureKeepsTheOriginatingRootAndLocalRelationNames()
    {
        var fixture = await Fixture();
        await using var connection = fixture.Connection;
        // Fault injection changes schema only; data was created through Mutation API.
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "DROP TABLE region_data";
            await command.ExecuteNonQueryAsync();
        }
        var query = new SelectQuery("School").Limit(1)
            .RelationQuery("platform", new SelectQuery("Platform").Project("organizationId").Limit(1)
                .RelationQuery("organization", new SelectQuery("Organization").Project("regionId").Limit(1)
                    .RelationQuery("region", new SelectQuery("Region").Limit(1))))
            .Comment("load school graph").Purpose("verify inherited failure route");
        await Assert.ThrowsAsync<SqlExecutorException>(() => fixture.Context.RequireResource<IDataService>()
            .QueryAsync(new QueryRequest(query)));
        var failed = Assert.Single(fixture.Log.Entries);
        Assert.Equal("failure", failed.ExecutionOutcome);
        Assert.Equal(new[] { "School", "School", "platform", "organization", "region", "sqlite", "select" },
            failed.TraceChain.Select(node => node.Name));
        Assert.All(failed.TraceChain, node => Assert.Equal("", node.Comment));
        Assert.Equal(new[] { "School.platform", "Platform.organization", "Organization.region" },
            failed.TraceChain.Where(node => node.Kind == "relation").Select(node => node.Detail));
        Assert.Equal("load school graph", failed.Comment);
        Assert.Equal("verify inherited failure route", failed.Purpose);
    }
}
