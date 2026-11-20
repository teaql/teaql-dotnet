using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

if (args.Length != 4 || args[0] is not ("1" or "2"))
    throw new ArgumentException("Usage: order-privacy ROUND DATABASE ORDINARY_SQL DEDICATED_SQL");
var round = args[0];
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = args[1], Pooling = false,
}.ToString());
await connection.OpenAsync();
await using var ordinary = new StreamWriter(new FileStream(args[2], FileMode.CreateNew, FileAccess.Write));
await using var dedicated = new StreamWriter(new FileStream(args[3], FileMode.CreateNew, FileAccess.Write));
var module = GeneratedRuntimeModule.Module;
// This key and both merchant accounts belong to a disposable example.
var codec = new AeadEntityReferenceCodec(1, new Dictionary<uint, byte[]>
{
    [1] = Enumerable.Repeat((byte)0x27, 32).ToArray(),
});
var context = module.IntoContext()
    .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
        new MetadataSchemaProvider(module.Metadata.GetEntity)))
    .WithEntityReferenceCodec(codec)
    .WithDiagnosticSqlLogSink(new TextDiagnosticSqlLogSink(ordinary))
    .WithSensitiveDiagnosticSqlLogSink(new SensitiveDiagnosticSqlLogSink(dedicated))
    .EnableQuerySqlLog().EnableMutationSqlLog();
await context.EnsureSchemaAsync();

var primary = await Q.MerchantsWithMinimalFields().WithIdIs(1).Limit(1)
    .Comment("Read the merchant provisioned by the model")
    .Purpose("Set the trusted dispatch scope").ExecuteForOneAsync(context)
    ?? throw new InvalidOperationException("The primary merchant was not provisioned");
var secondary = await Q.Merchants().Comment("Prepare an independent merchant")
    .Purpose("Provide a second merchant with a dispatch order").NewEntityAsync(context);
secondary.UpdateName("Independent retailer " + round);
await secondary.AuditAs("Register a second merchant").SaveAsync(context);

var merchantId = primary.Id ?? throw new InvalidOperationException("The primary merchant has no ID");
var foreignMerchantId = secondary.Id ?? throw new InvalidOperationException("The second merchant has no ID");
var reference = "SHIP-PRIVATE-" + round;
var email = $"sealed-{round}@example.test";
var foreignReference = "SHIP-OTHER-" + round;
var foreignEmail = $"foreign-{round}@example.test";
var own = await PlaceOrder(merchantId, reference, email);
var foreign = await PlaceOrder(foreignMerchantId, foreignReference, foreignEmail);
var purpose = $"view-dispatch:{merchantId}";
var token = context.EncodeEntityReference("CustomerOrder", own.Id, own.Version, purpose, TimeSpan.FromMinutes(5));
RejectReference("edit-dispatch");
RejectReference($"view-dispatch:{foreignMerchantId}");
var claims = context.DecodeEntityReference(token, "CustomerOrder", purpose);

var loaded = await Lookup(merchantId, claims.Id, reference, email)
    ?? throw new InvalidOperationException("The authorized dispatch order was not returned");
var profile = E.CustomerOrder(loaded).CustomerProfile().Eval()
    ?? throw new InvalidOperationException("The selected customer profile was not loaded");
if (E.CustomerProfile(profile).Email().Eval() != email || loaded.Id != own.Id || loaded.Version != claims.Version)
    throw new InvalidOperationException("The scoped dispatch data did not round-trip");
if (await Lookup(merchantId, foreign.Id, foreignReference, foreignEmail) is not null)
    throw new InvalidOperationException("The primary merchant read a second merchant's order");
Console.WriteLine("ORDER_PRIVACY=" + JsonSerializer.Serialize(new
{
    round, authorizedOrder = loaded.Id, foreignOrder = foreign.Id,
    merchant = merchantId, foreignMerchant = foreignMerchantId, version = loaded.Version,
    purposeDenied = true, foreignDenied = true,
}));

async Task<CustomerOrder> PlaceOrder(long merchant, string number, string deliveryEmail)
{
    var profile = await Q.CustomerProfiles().Comment("Prepare a delivery profile")
        .Purpose("Associate a customer with one merchant").NewEntityAsync(context);
    profile.UpdateMerchant(merchant).UpdateEmail(deliveryEmail);
    await profile.AuditAs("Record the customer's delivery profile").SaveAsync(context);
    var shipment = await Q.CustomerOrders().Comment("Prepare an order for dispatch")
        .Purpose("Record the merchant's order and delivery profile").NewEntityAsync(context);
    shipment.UpdateMerchant(merchant).UpdateCustomerProfile(profile.Id)
        .UpdateOrderNumber(number).UpdateFulfillmentStatus("ready");
    await shipment.AuditAs("Queue a merchant order for dispatch").SaveAsync(context);
    return shipment;
}

async Task<CustomerOrder?> Lookup(long merchant, ulong id, string number, string deliveryEmail)
{
    return await Q.CustomerOrders().WithIdIs(id).WithOrderNumberIs(number)
        .WithMerchantMatching(Q.MerchantsWithMinimalFields().WithIdIs(checked((ulong)merchant)))
        .WithCustomerProfileMatching(Q.CustomerProfilesWithMinimalFields().WithEmailIs(deliveryEmail))
        .SelectCustomerProfile().Limit(1).Comment("Load the scoped order and delivery profile")
        .Purpose("Review a dispatch order for the authenticated merchant").ExecuteForOneAsync(context);
}

void RejectReference(string deniedPurpose)
{
    try
    {
        context.DecodeEntityReference(token, "CustomerOrder", deniedPurpose);
    }
    catch (EntityReferenceTokenException)
    {
        return;
    }
    throw new InvalidOperationException("An unauthorized dispatch reference was accepted");
}
