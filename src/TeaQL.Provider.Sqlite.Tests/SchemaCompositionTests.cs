using Microsoft.Data.Sqlite;
using Moq;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Xunit;

namespace TeaQL.Provider.Sqlite.Tests;

public class SchemaCompositionTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CustomSchemaComposesWithSqlDataServiceInEitherInstallationOrder(bool schemaFirst)
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var executor = new SqlDataServiceExecutor(new SqliteDialect(),
            new SqliteTransport(connection), new SingleSchemaProvider(School()));
        var bootstrapCalls = 0;
        var context = new RuntimeModule().Entity(School())
            .GeneratedBootstrap(_ => { bootstrapCalls++; return Task.CompletedTask; })
            .IntoContext();
        var schema = new RecordingSchemaExecutor();
        if (schemaFirst)
            context.WithSchemaExecutor(schema).WithDataService(executor);
        else
            context.WithDataService(executor).WithSchemaExecutor(schema);

        await context.EnsureSchemaAsync();
        await context.EnsureSchemaAsync();

        Assert.Equal(new[] { "School", "School" }, schema.Entities);
        Assert.Equal(2, bootstrapCalls);
        Assert.Null(typeof(SqlDataServiceExecutor).GetMethod("EnsureSchemaAsync"));
        using var table = connection.CreateCommand();
        table.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'school_data'";
        Assert.Equal(0L, (long)(await table.ExecuteScalarAsync())!);
    }

    [Fact]
    public async Task ReplacingDataServiceWithoutSchemaDoesNotRetainThePreviousSchemaExecutor()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var context = new RuntimeModule().Entity(School()).IntoContext();
        context.WithDataService(new SqlDataServiceExecutor(new SqliteDialect(),
            new SqliteTransport(connection), new SingleSchemaProvider(School())));
        context.WithDataService(new Mock<IDataService>().Object);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(context.EnsureSchemaAsync);
        Assert.Contains("ISchemaExecutor", error.Message);
    }

    [Fact]
    public async Task SchemaFailureStopsGeneratedBootstrap()
    {
        var bootstrapCalls = 0;
        var context = new RuntimeModule().Entity(School())
            .GeneratedBootstrap(_ => { bootstrapCalls++; return Task.CompletedTask; })
            .IntoContext();
        var schema = new RecordingSchemaExecutor { Fail = true };
        context.WithDataService(new Mock<IDataService>().Object).WithSchemaExecutor(schema);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(context.EnsureSchemaAsync);
        Assert.Equal("schema failure", error.Message);
        Assert.Equal(new[] { "School" }, schema.Entities);
        Assert.Equal(0, bootstrapCalls);
    }

    private static EntityDescriptor School() => EntityDescriptor.New("School")
        .TableName("school_data")
        .Property(PropertyDescriptor.New("id", DataType.I64).Id());

    private sealed class SingleSchemaProvider(EntityDescriptor entity) : ISchemaProvider
    {
        public EntityDescriptor? GetEntity(string name) => name == entity.Name ? entity : null;
    }

    private sealed class RecordingSchemaExecutor : ISchemaExecutor
    {
        public List<string> Entities { get; } = new();
        public bool Fail { get; init; }

        public Task<SchemaResult> EnsureSchemaAsync(SchemaRequest request)
        {
            Entities.Add(request.EntityName);
            if (Fail) throw new InvalidOperationException("schema failure");
            return Task.FromResult(new SchemaResult { Changed = false });
        }
    }
}
