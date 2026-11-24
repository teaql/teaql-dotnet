using Generated;
using Generated.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Text.Json;
using TeaQL.Core;
using TeaQL.Provider.Sqlite;
using TeaQL.Runtime;
using TeaQL.Sql;

const decimal preciseAmount = 123456789012.3456789m;
const long shipment = 9_007_199_254_740_993L;
var deliveryDate = new DateTime(2026, 10, 31);
var pickup = new TimeSpan(9, 30, 15).Add(TimeSpan.FromTicks(1_234_560));
var quoted = new DateTimeOffset(2026, 10, 31, 1, 45, 0, TimeSpan.FromHours(8)).AddMilliseconds(123).UtcDateTime;
if (args.Length != 2 || args[1] is not ("first" or "restart"))
    throw new ArgumentException("Usage: freight-quote-settlement DATABASE {first|restart}");
var phase = args[1];
await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = args[0], Pooling = false,
}.ToString());
await connection.OpenAsync();
var module = GeneratedRuntimeModule.Module;
var caller = module.IntoContext()
    .WithDataService(new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
        new MetadataSchemaProvider(module.Metadata.GetEntity)))
    .DisableMutationSqlLog().DisableQuerySqlLog();
await caller.EnsureSchemaAsync();

async Task<FreightQuote> ReadAsync(string reference)
{
    return await Q.FreightQuotes().WithReferenceIs(reference).Limit(1)
        .Comment("Read the selected freight quote").Purpose("Verify the exact delivery commitment")
        .ExecuteForOneAsync(caller) ?? throw new InvalidOperationException("The freight quote is missing");
}

if (phase == "first")
{
    var seeds = new (string Reference, decimal Amount, long Number, long Count, string? Instructions)[]
    {
        ("SEA-001", 0.10m, shipment, 2, null),
        ("SEA-002", 0.20m, shipment + 1, 3, ""),
        ("SEA-003", 3.50m, shipment + 2, int.MaxValue, "Hold for inspection"),
    };
    foreach (var seed in seeds)
    {
        var quote = await Q.FreightQuotes().Comment("Prepare a freight quote")
            .Purpose("Price the shipment for its delivery window").NewEntityAsync(caller);
        quote.UpdatePlatform(1).UpdateReference(seed.Reference).UpdateAmount(seed.Amount)
            .UpdateShipmentNumber(seed.Number).UpdatePackageCount(seed.Count).UpdateDeliveryDate(deliveryDate)
            .UpdatePickupTime(pickup).UpdateQuotedAt(quoted);
        if (seed.Instructions is not null)
            quote.UpdateInstructions(seed.Instructions);
        await quote.AuditAs("Register a freight quote").SaveAsync(caller);
    }
    var revised = await ReadAsync("SEA-003");
    Require(E.FreightQuote(revised).Amount().Eval()?.ToString(CultureInfo.InvariantCulture) == "3.50",
        "The initial offshore quote lost its Decimal scale");
    revised.UpdateAmount(preciseAmount);
    await revised.AuditAs("Accept the revised offshore freight rate").SaveAsync(caller);
}

var rows = await Q.FreightQuotes().OrderByAmountAscending().Limit(3).Comment("Compare open freight amounts")
    .Purpose("Rank freight bids numerically").ExecuteForListAsync(caller);
Require(rows.Select(row => row.Reference).SequenceEqual(new[] { "SEA-001", "SEA-002", "SEA-003" }),
    "The numeric sort or quote count changed");
Require(rows.Select(row => E.FreightQuote(row).Amount().Eval()?.ToString(CultureInfo.InvariantCulture))
    .SequenceEqual(new[] { "0.10", "0.20", "123456789012.3456789" }), "Stored Decimal digits or scale changed");
Require(rows.Select(row => E.FreightQuote(row).Instructions().Eval())
    .SequenceEqual(new string?[] { null, "", "Hold for inspection" }), "Null and empty instructions were conflated");
Require(rows.Select(row => E.FreightQuote(row).ShipmentNumber().Eval())
    .SequenceEqual(new long?[] { shipment, shipment + 1, shipment + 2 }), "The shipment identifiers lost precision");
var high = rows[2];
var match = await Q.FreightQuotes().WithAmountIs(preciseAmount).Limit(1).Comment("Find the matching freight rate")
    .Purpose("Use exact Decimal equality").ExecuteForOneAsync(caller);
Require(match is not null && match.Id == high.Id && high.Version == 2, "The precise rate filter lost its quote");
var summary = await Q.FreightQuotes().SumAmountAs("total").Limit(1).Comment("Sum the accepted freight quotes")
    .Purpose("Preserve base-ten precision").ExecuteForRowsAsync(caller);
var expectedTotal = preciseAmount + 0.30m;
Require(summary.Rows.Single()["total"] is Value.DecimalValue total && total.Value == expectedTotal,
    "The exact freight SUM changed");

var wire = high.ToJson();
using (var document = JsonDocument.Parse(wire))
{
    var data = document.RootElement;
    Require(data.GetProperty("amount").GetString() == "123456789012.3456789"
        && data.GetProperty("shipmentNumber").GetString() == (shipment + 2).ToString(CultureInfo.InvariantCulture)
        && data.GetProperty("packageCount").GetInt32() == int.MaxValue, "Typed JSON lost Decimal, Long or Integer width");
}
var restored = E.FreightQuote(FreightQuote.FromJson(wire));
Require(restored.Amount().Eval() == preciseAmount && restored.ShipmentNumber().Eval() == shipment + 2,
    "Generated E changed the decoded quote");
Require(restored.DeliveryDate().Eval() == deliveryDate && restored.PickupTime().Eval() == pickup
    && restored.QuotedAt().Eval() == quoted, "The delivery date, wall clock or UTC timestamp changed");

var id = high.Id ?? throw new InvalidOperationException("The quote has no identity");
var sparse = await Q.FreightQuotesWithMinimalFields().WithIdIs(id).SelectReference().Limit(1)
    .Comment("Read only the quote reference").Purpose("Keep unselected delivery instructions distinct")
    .ExecuteForOneAsync(caller) ?? throw new InvalidOperationException("The sparse freight quote is missing");
var sparseWire = sparse.ToJson();
using (var document = JsonDocument.Parse(sparseWire))
    Require(!document.RootElement.TryGetProperty("instructions", out _), "Sparse JSON included unselected instructions");
try
{
    E.FreightQuote(FreightQuote.FromJson(sparseWire)).Instructions().Eval();
    throw new InvalidOperationException("Unselected instructions became a value");
}
catch (TeaQLNotLoadedException) { }

high.UpdateAmount(null);
try
{
    await high.AuditAs("Reject a quote without its required amount").SaveAsync(caller);
    throw new InvalidOperationException("A missing freight amount was saved");
}
catch (CheckException error) when (error.Violations.Any(item => item.RuleId == "required" && item.ModelPath == "amount")) { }
var retained = await ReadAsync("SEA-003");
Require(retained.Version == 2 && retained.Amount == preciseAmount, "Rejected quote modified persisted state");
Console.WriteLine("FREIGHT_QUOTE=" + JsonSerializer.Serialize(new
{
    phase, quotes = rows.Count, total = expectedTotal.ToString(CultureInfo.InvariantCulture),
    precise = retained.Amount?.ToString(CultureInfo.InvariantCulture), version = retained.Version,
    invalidAmountRejected = true, sparseInstructionsNotLoaded = true,
}));

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
