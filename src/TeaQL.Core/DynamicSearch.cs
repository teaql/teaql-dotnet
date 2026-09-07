using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace TeaQL.Core;

/// <summary>Trusted local UI-search metadata, not client-supplied policy or a TFP schema.</summary>
public sealed record SearchModel(IReadOnlyDictionary<string, string> Fields,
    IReadOnlyDictionary<string, string> Relations);

public sealed record DynamicSearchWarning(
    [property: JsonPropertyName("entity")] string Entity,
    [property: JsonPropertyName("clause")] string Clause,
    [property: JsonPropertyName("fieldPath")] string FieldPath)
{
    [JsonPropertyName("code")]
    public string Code => "DYNAMIC_SEARCH_UNKNOWN_FIELD";
}

public sealed record DynamicSearchFilter(string FieldPath, string Operator, JsonElement Value);
public sealed record DynamicSearchOrder(string FieldPath, string Direction);
public sealed record NormalizedDynamicSearch(IReadOnlyList<DynamicSearchFilter> Filters,
    IReadOnlyList<DynamicSearchOrder> Orders, IReadOnlyList<DynamicSearchWarning> Warnings);
public sealed record DynamicSearchResult(SelectQuery Query, IReadOnlyList<DynamicSearchWarning> Warnings);

/// <summary>
/// Local search normalization. Unknown complete clauses warn; malformed input remains fatal.
/// Native bindings are application-owned and must preserve related-query authorization.
/// This API does not relax the federation protocol's validation.
/// </summary>
public static class DynamicSearch
{
    private static readonly HashSet<string> Operators = new(StringComparer.Ordinal)
        { "$eq", "$ne", "$gt", "$gte", "$lt", "$lte", "$in", "$notIn", "$contains" };

    public static NormalizedDynamicSearch Normalize(string source, string entity,
        IReadOnlyDictionary<string, SearchModel> models, Action<DynamicSearchWarning>? warn = null,
        int maxClauses = 100)
    {
        if (maxClauses < 1 || !models.ContainsKey(entity)) throw Invalid("Invalid trusted search setup");
        JsonDocument document;
        try { document = JsonDocument.Parse(source); }
        catch (JsonException) { throw Invalid("Dynamic search requires valid JSON"); }
        using (document)
        {
            var root = document.RootElement;
            CheckObject(root, "filter", "orderBy");
            var filters = root.TryGetProperty("filter", out var f) ? f : default;
            var orders = root.TryGetProperty("orderBy", out var o) ? o : default;
            if (filters.ValueKind is not (JsonValueKind.Undefined or JsonValueKind.Object)
                || orders.ValueKind is not (JsonValueKind.Undefined or JsonValueKind.Array))
                throw Invalid("Invalid search filter or ordering");
            var count = filters.ValueKind == JsonValueKind.Object ? filters.EnumerateObject().Count() : 0;
            count += orders.ValueKind == JsonValueKind.Array ? orders.GetArrayLength() : 0;
            if (count > maxClauses) throw Invalid("Dynamic search exceeds clause limit");
            var resultFilters = new List<DynamicSearchFilter>();
            var resultOrders = new List<DynamicSearchOrder>();
            var warnings = new List<DynamicSearchWarning>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            if (filters.ValueKind == JsonValueKind.Object)
                foreach (var property in filters.EnumerateObject())
                {
                    if (!seen.Add(property.Name)) throw Invalid("Duplicate search field");
                    var op = "$eq";
                    var value = property.Value;
                    if (value.ValueKind == JsonValueKind.Object)
                    {
                        var parts = value.EnumerateObject().ToArray();
                        if (parts.Length != 1 || !Operators.Contains(parts[0].Name))
                            throw Invalid("Unsupported or malformed dynamic search operator");
                        op = parts[0].Name;
                        value = parts[0].Value;
                    }
                    if (op is "$in" or "$notIn"
                        && (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() > 1000))
                        throw Invalid("Invalid or oversized search value list");
                    var kind = FieldType(property.Name, entity, models);
                    if (kind == null)
                    {
                        warnings.Add(new(entity, "FILTER", property.Name));
                        continue;
                    }
                    if (op == "$contains" && kind != "string")
                        throw Invalid("String operator requires a string field");
                    if (value.ValueKind == JsonValueKind.Array)
                    {
                        if (op is not ("$in" or "$notIn")) throw Invalid("Unexpected search value list");
                        foreach (var item in value.EnumerateArray()) ValidateScalar(item, kind);
                    }
                    else ValidateScalar(value, kind);
                    resultFilters.Add(new(property.Name, op, value.Clone()));
                }
            if (orders.ValueKind == JsonValueKind.Array)
                foreach (var order in orders.EnumerateArray())
                {
                    CheckObject(order, "field", "direction");
                    if (!order.TryGetProperty("field", out var field) || field.ValueKind != JsonValueKind.String
                        || !order.TryGetProperty("direction", out var direction)
                        || direction.ValueKind != JsonValueKind.String || direction.GetString() is not ("asc" or "desc"))
                        throw Invalid("Invalid dynamic search ordering");
                    var path = field.GetString()!;
                    if (FieldType(path, entity, models) == null) warnings.Add(new(entity, "ORDER_BY", path));
                    else resultOrders.Add(new(path, direction.GetString()!));
                }
            Emit(warnings, warn);
            return new(resultFilters.AsReadOnly(), resultOrders.AsReadOnly(), warnings.AsReadOnly());
        }
    }

    public static DynamicSearchResult Merge(SelectQuery basis, string source,
        IReadOnlyDictionary<string, SearchModel> models,
        Func<DynamicSearchFilter, Expr> filterBinding, Func<DynamicSearchOrder, OrderBy> orderBinding,
        Action<DynamicSearchWarning>? warn = null)
    {
        var normalized = Normalize(source, basis.Entity, models, _ => { });
        // Compile everything before mutating a clone or emitting any warnings.
        var filters = normalized.Filters.Select(filterBinding).ToArray();
        var orders = normalized.Orders.Select(orderBinding).ToArray();
        if (filters.Any(x => x == null) || orders.Any(x => x == null)) throw Invalid("Invalid trusted search binding");
        var query = basis.CloneForExecution().NormalizeGeneratedFilters();
        foreach (var filter in filters) query.AndFilter(filter);
        query.OrderByItems.AddRange(orders);
        Emit(normalized.Warnings, warn);
        return new(query, normalized.Warnings);
    }

    private static string? FieldType(string path, string entity, IReadOnlyDictionary<string, SearchModel> models)
    {
        var parts = path.Split('.');
        if (parts.Length > 16 || parts.Any(p => p.Length == 0 || p.StartsWith('$')
                || p is "__proto__" or "prototype" or "constructor")) throw Invalid("Invalid search field path");
        var model = models[entity];
        foreach (var part in parts[..^1])
        {
            if (!model.Relations.TryGetValue(part, out var target)) return null;
            if (!models.TryGetValue(target, out model)) throw Invalid("Invalid trusted search relation metadata");
        }
        return model.Fields.TryGetValue(parts[^1], out var kind) ? kind : null;
    }

    private static void ValidateScalar(JsonElement value, string kind)
    {
        if (value.ValueKind == JsonValueKind.Null) return;
        bool number = value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var n) && double.IsFinite(n);
        var text = value.ValueKind == JsonValueKind.String ? value.GetString() : null;
        bool valid = kind switch
        {
            "integer" or "timestamp" => number && Math.Abs(value.GetDouble()) <= 9007199254740991d
                && Math.Truncate(value.GetDouble()) == value.GetDouble(),
            "number" => number,
            "string" => text != null,
            "boolean" => value.ValueKind is JsonValueKind.True or JsonValueKind.False,
            "decimal" => number || text != null && Regex.IsMatch(text, @"\A[+-]?[0-9]+(?:\.[0-9]+)?\z"),
            "date" => text != null && DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date) && date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == text,
            _ => false
        };
        if (!valid) throw Invalid("Invalid value for known search field");
    }

    private static void CheckObject(JsonElement value, params string[] keys)
    {
        if (value.ValueKind != JsonValueKind.Object) throw Invalid("Expected search object");
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var property in value.EnumerateObject())
            if (!keys.Contains(property.Name, StringComparer.Ordinal) || !seen.Add(property.Name))
                throw Invalid("Unsupported or duplicate dynamic search control");
    }

    private static ArgumentException Invalid(string message) => new(message);
    private static void Emit(IEnumerable<DynamicSearchWarning> warnings, Action<DynamicSearchWarning>? warn)
    {
        foreach (var warning in warnings)
            if (warn != null) warn(warning);
            else Console.Error.WriteLine(JsonSerializer.Serialize(warning));
    }
}
