using Generated;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;
using TeaQL.TfpEndpoint;

var database = Environment.GetEnvironmentVariable("TEAQL_FEDERATION_DATABASE")
    ?? throw new InvalidOperationException("Set TEAQL_FEDERATION_DATABASE for the local order store");
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = database, Pooling = false,
}.ToString());
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
    new MetadataSchemaProvider(module.Metadata.GetEntity));
var context = module.IntoContext().WithDataService(provider);
await context.EnsureSchemaAsync();

var primary = await Q.MerchantsWithMinimalFields().WithIdIs(1).Limit(1)
    .Comment("Read the model's primary merchant").Purpose("Resolve the order operator's merchant")
    .ExecuteForOneAsync(context)
    ?? throw new InvalidOperationException("Model did not provision a primary merchant");
var primaryId = primary.Id ?? throw new InvalidOperationException("Primary merchant has no identity");
var previous = await Q.FederatedOrdersWithMinimalFields().WithIdIs(1).Limit(1)
    .Comment("Check for an existing delivery order").Purpose("Keep the local order data across restarts")
    .ExecuteForOneAsync(context);
if (previous is null)
{
    var other = await Q.Merchants().Comment("Prepare an independent merchant")
        .Purpose("Prove another merchant's order remains private").NewEntityAsync(context);
    other.UpdateName("Independent Retailer");
    await other.AuditAs("Register the second merchant").SaveAsync(context);
    await CreateOrder(primaryId, "FEDERATION-042");
    await CreateOrder(other.Id ?? throw new InvalidOperationException("Second merchant has no identity"), "FEDERATION-OTHER");
}

const string entity = "FederatedOrder";
var trusted = new TrustedFederalContext
{
    TenantField = "merchant",
    TenantId = new Value.I64Value(primaryId),
    AuthenticatedUser = "dispatch-desk",
    ApprovedPurpose = "review-dispatch",
    AllowedEntities = new HashSet<string> { entity },
    ReadableFields = new Dictionary<string, IReadOnlyDictionary<string, string>>
    {
        [entity] = new Dictionary<string, string>
        {
            ["id"] = "id", ["orderNumber"] = "order_number",
            ["fulfillmentStatus"] = "fulfillment_status", ["version"] = "version",
        },
    },
    WritableFields = new Dictionary<string, IReadOnlyDictionary<string, string>>
    {
        [entity] = new Dictionary<string, string> { ["fulfillmentStatus"] = "fulfillment_status" },
    },
    AllowedActions = new Dictionary<string, ISet<string>> { [entity] = new HashSet<string> { "Update" } },
    MaxPageSize = 10,
    MaxOffset = 20,
};
var endpoint = new TfpEndpointHandler(provider);
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:0");
var app = builder.Build();
app.MapPost("/query", (HttpRequest request) => Handle(request, endpoint.HandleQueryAsync));
app.MapPost("/mutate", (HttpRequest request) => Handle(request, endpoint.HandleMutationAsync));
await app.StartAsync();
Console.WriteLine("FEDERATION_READY=" + app.Urls.Single());
await app.WaitForShutdownAsync();

async Task CreateOrder(long merchantId, string reference)
{
    var order = await Q.FederatedOrders().Comment("Prepare an order for dispatch")
        .Purpose("Create controlled dispatch data for the merchant").NewEntityAsync(context);
    order.UpdateMerchant(merchantId).UpdateOrderNumber(reference).UpdateFulfillmentStatus("ready");
    await order.AuditAs("Register a merchant dispatch order").SaveAsync(context);
}

async Task<IResult> Handle(HttpRequest request, Func<TrustedFederalContext, string, Task<Dictionary<string, object>>> execute)
{
    if (request.ContentLength is null or > 8192)
        return Results.Json(new { code = "TFP_INVALID_REQUEST" }, statusCode: 400);
    using var reader = new StreamReader(request.Body);
    try
    {
        return Results.Json(await execute(trusted, await reader.ReadToEndAsync()));
    }
    catch (TfpEndpointException error)
    {
        var status = error.Code switch
        {
            "TFP_FORBIDDEN_ENTITY" or "TFP_FORBIDDEN_FIELD" or "TFP_POLICY_VIOLATION" => 403,
            "TFP_UNAVAILABLE" => 409,
            "TFP_EXECUTION_FAILED" => 500,
            _ => 400,
        };
        return Results.Json(new { code = error.Code }, statusCode: status);
    }
}
