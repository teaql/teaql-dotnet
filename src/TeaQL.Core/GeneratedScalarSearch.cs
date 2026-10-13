using System.Globalization;
using System.Text.Json;

namespace TeaQL.Core;

/// <summary>Generated trusted aliases and native keys. Never read from search input.</summary>
public sealed record GeneratedSearchModel(SearchModel Model, IReadOnlyDictionary<string, string> QueryFields);

public static partial class DynamicSearch
{
    /// <summary>
    /// Bind scalar JSON search through generated metadata and the existing local boundary.
    /// Known related paths require explicit trusted bindings through Merge; this method
    /// never infers related authorization or replaces an existing native predicate.
    /// </summary>
    public static DynamicSearchResult MergeGeneratedScalar(SelectQuery basis, string source,
        IReadOnlyDictionary<string, GeneratedSearchModel> models, Action<DynamicSearchWarning>? warn = null)
    {
        var searchModels = models.ToDictionary(pair => pair.Key, pair => pair.Value.Model, StringComparer.Ordinal);
        string Column(string path)
        {
            if (path.Contains('.')) throw Invalid("Related paths require a trusted relation binding");
            if (!models.TryGetValue(basis.Entity, out var model)
                || !model.QueryFields.TryGetValue(path, out var column) || string.IsNullOrEmpty(column))
                throw Invalid("Missing trusted field binding");
            return column;
        }
        Expr Filter(DynamicSearchFilter filter)
        {
            var column = Column(filter.FieldPath);
            var kind = models[basis.Entity].Model.Fields[filter.FieldPath];
            var raw = filter.Value;
            if (filter.Operator == "$contains")
            {
                if (raw.ValueKind != JsonValueKind.String) throw Invalid("Contains requires a string");
                return Expr.Contain(column, raw.GetString()!);
            }
            if (filter.Operator is "$in" or "$notIn")
            {
                var values = raw.EnumerateArray().Select(item => GeneratedSearchValue(item, kind)).ToArray();
                return filter.Operator == "$in" ? Expr.InList(column, values) : Expr.NotInList(column, values);
            }
            if (raw.ValueKind == JsonValueKind.Null)
                return filter.Operator switch
                {
                    "$eq" => Expr.IsNull(column), "$ne" => Expr.IsNotNull(column),
                    _ => throw Invalid("Null supports only equality predicates")
                };
            var value = GeneratedSearchValue(raw, kind);
            return filter.Operator switch
            {
                "$eq" => Expr.Eq(column, value), "$ne" => Expr.Ne(column, value),
                "$gt" => Expr.Gt(column, value), "$gte" => Expr.Gte(column, value),
                "$lt" => Expr.Lt(column, value), "$lte" => Expr.Lte(column, value),
                _ => throw Invalid("Unsupported operator")
            };
        }
        return Merge(basis, source, searchModels, Filter,
            order => order.Direction == "asc" ? OrderBy.Asc(Column(order.FieldPath)) : OrderBy.Desc(Column(order.FieldPath)), warn);
    }

    private static Value GeneratedSearchValue(JsonElement raw, string kind)
    {
        if (raw.ValueKind == JsonValueKind.Null) return new Value.NullValue();
        switch (kind)
        {
            case "string": return new Value.TextValue(raw.GetString()!);
            case "boolean": return new Value.BoolValue(raw.GetBoolean());
            case "integer": return new Value.I64Value((long)raw.GetDouble());
            case "timestamp": return new Value.TimestampValue((long)raw.GetDouble());
            case "number": return new Value.F64Value(raw.GetDouble());
            case "decimal":
                var text = raw.ValueKind == JsonValueKind.String ? raw.GetString() : raw.GetRawText();
                if (!decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    throw Invalid("Decimal is outside the native range");
                return new Value.DecimalValue(value);
            case "date":
                return new Value.DateValue(DateTime.SpecifyKind(DateTime.ParseExact(raw.GetString()!,
                    "yyyy-MM-dd", CultureInfo.InvariantCulture), DateTimeKind.Utc));
            default: throw Invalid("Unsupported known-field type");
        }
    }
}
