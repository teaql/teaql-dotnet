using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Sql;
using TeaQL.Provider.Sqlite;
using TeaQL.TfpEndpoint;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<IDataService>(sp =>
{
    var connection = new SqliteConnection("Data Source=:memory:");
    connection.Open();

    // Create schema
    var command = connection.CreateCommand();
    command.CommandText = @"
        CREATE TABLE User (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT,
            email TEXT,
            version INTEGER
        );
        INSERT INTO User (username, email, version) VALUES ('admin', 'admin@teaql.com', 1);
        INSERT INTO User (username, email, version) VALUES ('johndoe', 'john@example.com', 1);
    ";
    command.ExecuteNonQuery();

    var transport = new SqliteTransport(connection);
    var dialect = new SqliteDialect();
    var schemaProvider = new DummySchemaProvider();
    
    return new SqlDataServiceExecutor(dialect, transport, schemaProvider);
});

builder.Services.AddSingleton<TfpEndpointHandler>();

var app = builder.Build();

app.MapPost("/tfp/query", async (HttpContext context, TfpEndpointHandler handler) => {
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var result = await handler.HandleQueryAsync(body);
    return Results.Json(result);
});

app.MapPost("/tfp/mutate", async (HttpContext context, TfpEndpointHandler handler) => {
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var result = await handler.HandleMutationAsync(body);
    return Results.Json(result);
});

app.Run();

class DummySchemaProvider : ISchemaProvider
{
    public EntityDescriptor GetEntity(string name)
    {
        var entity = new EntityDescriptor { Name = name }.TableName(name);
        entity.Properties.Add(PropertyDescriptor.New("id", DataType.I64).Id());
        entity.Properties.Add(PropertyDescriptor.New("username", DataType.Text));
        entity.Properties.Add(PropertyDescriptor.New("email", DataType.Text));
        entity.Properties.Add(PropertyDescriptor.New("version", DataType.I64).Version());
        return entity;
    }
}
