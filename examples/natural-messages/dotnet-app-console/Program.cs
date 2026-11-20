using TeaQL.Core;
using Generated;
using TeaQL.Sql;
using Microsoft.Data.Sqlite;
using TeaQL.Provider.Sqlite;
var module = GeneratedRuntimeModule.Module;
var context = module.IntoContext();
await using var connection = new SqliteConnection("Data Source=runtime-example.sqlite");
await connection.OpenAsync();
var dialect = new SqliteDialect();
var transport = new SqliteTransport(connection);
context.WithDataService(new SqlDataServiceExecutor(dialect, transport, new MetadataSchemaProvider(module.Metadata.GetEntity)));
await context.EnsureSchemaAsync();
Console.WriteLine("checker-natural-message-service TeaQL console ready ({0} entities)",
    module.Metadata.GetAllEntities().Count);
