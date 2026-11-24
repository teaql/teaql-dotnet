using System.Globalization;
using System.Text.Json;
using Generated;
using Microsoft.Data.Sqlite;

using var plan = JsonDocument.Parse(Console.In.ReadToEnd());
var root = plan.RootElement;
var module = GeneratedRuntimeModule.Module;
var connectionString = new SqliteConnectionStringBuilder { DataSource = Text(root, "database") };
await using var connection = new SqliteConnection(connectionString.ToString());
await connection.OpenAsync();
var provider = new SqlDataServiceExecutor(new SqliteDialect(), new SqliteTransport(connection),
    new MetadataSchemaProvider(module.Metadata.GetEntity));
UserContext CreateContext() => module.IntoContext().WithDataService(provider);
var contexts = new Dictionary<string, UserContext>
{
    ["main"] = CreateContext(),
    ["independent"] = CreateContext()
};
await contexts["main"].EnsureSchemaAsync();
var warehouseId = root.GetProperty("warehouse").GetProperty("id").GetInt64();

foreach (var step in root.GetProperty("steps").EnumerateArray())
{
    var context = contexts[Text(step, "context")];
    switch (Text(step, "operation"))
    {
        case "select":
            var code = Text(step, "locale");
            try
            {
                context.SetLocaleCode(code);
                Emit(step, context, "locale", new() { ["requestedLocale"] = code, ["accepted"] = true });
            }
            catch (UnsupportedLocaleException failure)
            {
                Emit(step, context, "locale", new()
                {
                    ["requestedLocale"] = failure.LocaleCode,
                    ["accepted"] = false,
                    ["nativeError"] = typeof(UnsupportedLocaleException).Name
                });
            }
            break;
        case "validate":
            await VerifyFailure(step, context, warehouseId);
            break;
        case "install-catalog":
            using (var catalog = File.OpenRead(Text(step, "path")))
                context.InstallI18nCatalog(I18nCatalog.FromJson(catalog, I18nCatalog.Builtin));
            Emit(step, context, "catalog-installed", new());
            break;
        case "verify-unchanged":
            var rows = await Q.SuppliersWithMinimalFields().Limit(1)
                .Comment("Check the rejected registrations").Purpose("Verify no invalid supplier was stored")
                .ExecuteForListAsync(context);
            Emit(step, context, "storage", new() { ["supplierCount"] = rows.Count });
            break;
        default:
            throw new InvalidOperationException("Unknown example operation");
    }
}

static string Text(JsonElement item, string field) =>
    item.GetProperty(field).GetString() ?? throw new InvalidOperationException("Expected " + field);

static void Emit(JsonElement step, UserContext context, string type, Dictionary<string, object?> fields)
{
    fields["type"] = type;
    fields["context"] = Text(step, "context");
    fields["effectiveLocale"] = context.Locale.Code();
    Console.WriteLine("TEAQL_I18N_SHOWCASE " + JsonSerializer.Serialize(fields));
}

static async Task VerifyFailure(JsonElement step, UserContext context, long warehouseId)
{
    var values = step.GetProperty("input");
    var supplier = await Q.Suppliers().Comment("Prepare supplier registration")
        .Purpose("Register a supplier for warehouse purchasing").NewEntityAsync(context);
    supplier.UpdateWarehouse(warehouseId);
    if (values.GetProperty("name").GetString() is { } name)
        supplier.UpdateName(name);
    supplier.UpdateMinimumOrderQuantity(values.GetProperty("minimum_order_quantity").GetInt32());
    try
    {
        await supplier.AuditAs("Register the supplier for purchasing").SaveAsync(context);
    }
    catch (CheckException failure) when (failure.Violations.Count == 1)
    {
        var violation = failure.Violations[0];
        var rule = violation.RuleId.ToLowerInvariant();
        var wire = violation.ToWire();
        var arguments = new Dictionary<string, int>();
        if (rule == "min")
        {
            if (wire.SystemValue is null) throw;
            arguments["minimum"] = Convert.ToInt32(wire.SystemValue, CultureInfo.InvariantCulture);
        }
        else if (rule != "required") throw;
        Emit(step, context, "validation", new()
        {
            ["case"] = Text(step, "case"), ["messageKey"] = $"checker.{rule}",
            ["nativeRule"] = violation.RuleId, ["location"] = violation.ModelPath,
            ["arguments"] = arguments, ["message"] = wire.Message,
            ["nativeArguments"] = new { inputValue = wire.InputValue, systemValue = wire.SystemValue }
        });
        return;
    }
    throw new InvalidOperationException("Invalid supplier registration was stored");
}
