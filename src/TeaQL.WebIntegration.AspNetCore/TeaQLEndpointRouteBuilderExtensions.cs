using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;

namespace TeaQL.WebIntegration.AspNetCore;

public class TeaQLRequest
{
    public string Operation { get; set; } = "Query"; // Query, Insert, Update, Delete
    public string? Comment { get; set; }
    public JsonElement Payload { get; set; }
}

internal sealed class QueryPayload
{
    public SelectQuery Query { get; set; } = new();
    public string? Comment { get; set; }
    public string? Purpose { get; set; }
    public System.Collections.Generic.List<TraceNode> TraceChain { get; set; } = new();
}

public static class TeaQLEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapTeaQLEndpoint(this IEndpointRouteBuilder endpoints, string pattern)
    {
        return endpoints.MapPost(pattern, async context =>
        {
            var dataService = context.RequestServices.GetRequiredService<IDataService>();
            
            // Resolve ITeaqlRuntime as it might be required by implementations,
            // or used for context setup in a real scenario.
            var runtime = context.RequestServices.GetService<ITeaqlRuntime>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var req = await JsonSerializer.DeserializeAsync<TeaQLRequest>(context.Request.Body, options);

            if (req == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid request body");
                return;
            }

            object? result = null;

            try
            {
                switch (req.Operation.ToLowerInvariant())
                {
                    case "query":
                        var queryPayload = req.Payload.Deserialize<QueryPayload>(options);
                        var queryReq = queryPayload == null ? null : new QueryRequest(queryPayload.Query,
                            new QueryIntent(queryPayload.Comment, queryPayload.Purpose)) { TraceChain = queryPayload.TraceChain };
                        if (queryReq != null)
                        {
                            result = await dataService.QueryAsync(queryReq);
                        }
                        break;
                    case "insert":
                        var insertCmd = req.Payload.Deserialize<InsertCommand>(options);
                        if (insertCmd != null)
                        {
                            result = await dataService.MutateAsync(new InsertMutationRequest(insertCmd, req.Comment));
                        }
                        break;
                    case "update":
                        var updateCmd = req.Payload.Deserialize<UpdateCommand>(options);
                        if (updateCmd != null)
                        {
                            result = await dataService.MutateAsync(new UpdateMutationRequest(updateCmd, req.Comment));
                        }
                        break;
                    case "delete":
                        var deleteCmd = req.Payload.Deserialize<DeleteCommand>(options);
                        if (deleteCmd != null)
                        {
                            result = await dataService.MutateAsync(new DeleteMutationRequest(deleteCmd, req.Comment));
                        }
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync($"Unknown operation: {req.Operation}");
                        return;
                }
            }
            catch (RequestIntentException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { code = ex.Code, field = ex.Field, requestKind = ex.RequestKind });
                return;
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(ex.Message);
                return;
            }

            if (result == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid payload for the specified operation");
                return;
            }

            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, result, options);
        });
    }
}
