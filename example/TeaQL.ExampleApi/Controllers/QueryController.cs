using Microsoft.AspNetCore.Mvc;
using TeaQL.Core;
using TeaQL.Provider.PostgreSql;
using TeaQL.Sql;
using System.Collections.Generic;

namespace TeaQL.ExampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueryController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] System.Text.Json.JsonElement json)
    {
        string entityName = json.TryGetProperty("entity", out var entityProp) ? entityProp.GetString() ?? "Unknown" : "Unknown";
        
        // Mock the AST based on the TS client request:
        // filterCondition: { "status": { "$eq": "active" } }
        // limitValue: 10
        // orderItems: [{ field: "createdAt", direction: "Desc" }]
        var query = new SelectQuery(entityName)
        {
            FilterCondition = Expr.Eq("status", new Value.TextValue("active")),
            Slice = new Slice(10, 0),
            OrderByItems = new List<OrderBy> { OrderBy.Desc("createdAt") }
        };
        
        // We need a mock EntityDescriptor since we don't have a full schema registry setup
        var entityDesc = new EntityDescriptor
        {
            Name = entityName,
            TableNameValue = entityName.ToLower() + "s", // mock "User" -> "users"
            Properties = new List<PropertyDescriptor>
            {
                new PropertyDescriptor("status", DataType.Text, false, "status_col", true, false),
                new PropertyDescriptor("createdAt", DataType.Timestamp, false, "created_at", true, false)
            }
        };
        
        // Instantiate the Dialect and compile
        var dialect = new PostgreSqlDialect();
        var compiled = dialect.CompileSelect(entityDesc, query);
        
        // Print to backend console
        System.Console.WriteLine($"\n=== TEAQL GENERATED SQL ===");
        System.Console.WriteLine($"SQL:\n{compiled.Sql}");
        System.Console.WriteLine($"Params:\n" + string.Join(", ", compiled.Params.ConvertAll(p => p.ToJsonValue()?.ToString() ?? "null")));
        System.Console.WriteLine($"===========================\n");
        
        return Ok(new[] { new { id = 1, status = "active", test_success = true, parsedEntity = entityName, generatedSql = compiled.Sql } });
    }
}
