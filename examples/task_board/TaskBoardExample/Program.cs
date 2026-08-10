using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using Record = TeaQL.Core.Record;

namespace TaskBoardExample;

public class SchemaProviderWrapper : ISchemaProvider
{
    private readonly IMetadataStore _store;
    public SchemaProviderWrapper(IMetadataStore store) => _store = store;
    public EntityDescriptor? GetEntity(string name) => _store.GetEntity(name);
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Initializing TaskBoard Schema...");

        var platform = EntityDescriptor.New("Platform")
            .TableName("platform")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("name", DataType.Text))
            .Property(PropertyDescriptor.New("founded", DataType.I64)) 
            .Property(PropertyDescriptor.New("user_email", DataType.Text))
            .Property(PropertyDescriptor.New("version", DataType.I64).Version());

        var taskStatus = EntityDescriptor.New("TaskStatus")
            .TableName("task_status")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("name", DataType.Text))
            .Property(PropertyDescriptor.New("code", DataType.Text))
            .Property(PropertyDescriptor.New("color", DataType.Text))
            .Property(PropertyDescriptor.New("display_order", DataType.I64))
            .Property(PropertyDescriptor.New("progress", DataType.I64))
            .Property(PropertyDescriptor.New("platform", DataType.I64))
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Relation(RelationDescriptor.New("platform_ref", "Platform").LocalKey("platform"));

        var task = EntityDescriptor.New("Task")
            .TableName("task")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("name", DataType.Text))
            .Property(PropertyDescriptor.New("status", DataType.I64))
            .Property(PropertyDescriptor.New("platform", DataType.I64))
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Relation(RelationDescriptor.New("status_ref", "TaskStatus").LocalKey("status"))
            .Relation(RelationDescriptor.New("platform_ref", "Platform").LocalKey("platform"));

        var taskExecutionLog = EntityDescriptor.New("TaskExecutionLog")
            .TableName("task_execution_log")
            .Property(PropertyDescriptor.New("id", DataType.I64).Id())
            .Property(PropertyDescriptor.New("task", DataType.I64))
            .Property(PropertyDescriptor.New("action", DataType.Text))
            .Property(PropertyDescriptor.New("detail", DataType.Text))
            .Property(PropertyDescriptor.New("version", DataType.I64).Version())
            .Relation(RelationDescriptor.New("task_ref", "Task").LocalKey("task"));

        var store = new InMemoryMetadataStore()
            .WithEntity(platform)
            .WithEntity(taskStatus)
            .WithEntity(task)
            .WithEntity(taskExecutionLog);

        var userContext = new UserContext().WithMetadata(store);
        var runtime = new TeaqlRuntime(userContext);

        Console.WriteLine("Initializing SQLite Provider...");
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var transport = new SqliteTransport(connection);
        var dialect = new SqliteDialect();
        var schemaProvider = new SchemaProviderWrapper(store);
        var executor = new SqlDataServiceExecutor(dialect, transport, schemaProvider);

        // Create tables manually for this in-memory SQLite demo
        await transport.ExecuteSqlAsync(new CompiledQuery("CREATE TABLE platform (id INTEGER PRIMARY KEY, name TEXT, founded INTEGER, user_email TEXT, version INTEGER)", new List<Value>()));
        await transport.ExecuteSqlAsync(new CompiledQuery("CREATE TABLE task_status (id INTEGER PRIMARY KEY, name TEXT, code TEXT, color TEXT, display_order INTEGER, progress INTEGER, platform INTEGER, version INTEGER)", new List<Value>()));
        await transport.ExecuteSqlAsync(new CompiledQuery("CREATE TABLE task (id INTEGER PRIMARY KEY, name TEXT, status INTEGER, platform INTEGER, version INTEGER)", new List<Value>()));
        await transport.ExecuteSqlAsync(new CompiledQuery("CREATE TABLE task_execution_log (id INTEGER PRIMARY KEY, task INTEGER, action TEXT, detail TEXT, version INTEGER)", new List<Value>()));

        Console.WriteLine("Inserting Platform and TaskStatus...");
        var pId = new Value.I64Value(1);
        await executor.MutateAsync(new InsertMutationRequest(
            new InsertCommand { 
                Entity = "Platform", 
                Values = new Record { 
                    ["id"] = pId, 
                    ["name"] = new Value.TextValue("Robot System"), 
                    ["founded"] = new Value.I64Value(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()), 
                    ["user_email"] = new Value.TextValue("admin@robots.com"), 
                    ["version"] = new Value.I64Value(1) 
                }
            }
        ));

        var tsId = new Value.I64Value(1001);
        await executor.MutateAsync(new InsertMutationRequest(
            new InsertCommand { 
                Entity = "TaskStatus", 
                Values = new Record {
                    ["id"] = tsId,
                    ["name"] = new Value.TextValue("Planned"),
                    ["code"] = new Value.TextValue("PLANNED"),
                    ["color"] = new Value.TextValue("#94A3B8"),
                    ["display_order"] = new Value.I64Value(10),
                    ["progress"] = new Value.I64Value(0),
                    ["platform"] = pId,
                    ["version"] = new Value.I64Value(1)
                }
            }
        ));

        Console.WriteLine("Creating Task...");
        var tId = new Value.I64Value(1);
        await executor.MutateAsync(new InsertMutationRequest(
            new InsertCommand { 
                Entity = "Task", 
                Values = new Record {
                    ["id"] = tId,
                    ["name"] = new Value.TextValue("Initial Robot Task"),
                    ["status"] = tsId,
                    ["platform"] = pId,
                    ["version"] = new Value.I64Value(1)
                }
            }
        ));

        Console.WriteLine("Updating Task...");
        await executor.MutateAsync(new UpdateMutationRequest(
            new UpdateCommand { 
                Entity = "Task", 
                Id = tId,
                Values = new Record {
                    ["name"] = new Value.TextValue("Updated Robot Task"),
                    ["version"] = new Value.I64Value(2)
                }
            }
        ));

        Console.WriteLine("Inserting TaskExecutionLog...");
        await executor.MutateAsync(new InsertMutationRequest(
            new InsertCommand { 
                Entity = "TaskExecutionLog", 
                Values = new Record {
                    ["id"] = new Value.I64Value(1),
                    ["task"] = tId,
                    ["action"] = new Value.TextValue("Update Name"),
                    ["detail"] = new Value.TextValue("Changed name to 'Updated Robot Task'"),
                    ["version"] = new Value.I64Value(1)
                }
            }
        ));

        Console.WriteLine("Querying Tasks...");
        var taskQuery = new SelectQuery("Task");
        var taskResults = await executor.QueryAsync(new QueryRequest { Query = taskQuery });

        foreach (var row in taskResults.Rows)
        {
            Console.WriteLine($"Task ID: {((Value.I64Value)row["id"]).Value}, Name: {((Value.TextValue)row["name"]).Value}");
        }

        Console.WriteLine("Querying TaskExecutionLogs...");
        var logQuery = new SelectQuery("TaskExecutionLog");
        var logResults = await executor.QueryAsync(new QueryRequest { Query = logQuery });

        foreach (var row in logResults.Rows)
        {
            Console.WriteLine($"Log ID: {((Value.I64Value)row["id"]).Value}, Action: {((Value.TextValue)row["action"]).Value}, Detail: {((Value.TextValue)row["detail"]).Value}");
        }

        Console.WriteLine("Example finished successfully!");
    }
}
