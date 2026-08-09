using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace TeaQL.Core;

public static class RequestConstants
{
    public const string COUNT_ALIAS = "count";
    public const string TYPE_FIELD = "internal_type";
    public const string TYPE_GROUP_FIELD = "type_group";
}

public enum FieldOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Between,
    In,
    NotIn,
    Contain,
    NotContain,
    BeginWith,
    NotBeginWith,
    EndWith,
    NotEndWith,
    SoundsLike,
    IsNull,
    IsNotNull,
}

public record DateRange<T>(T Start, T End);

public record QuerySelection
{
    public SelectQuery Query { get; set; }
    public List<RelationSelection> RelationSelections { get; set; } = new();
    public List<RelationFilter> RelationFilters { get; set; } = new();
    public List<QuerySelection> ChildEnhancements { get; set; } = new();
    public QueryOptions QueryOptions { get; set; } = new();

    public QuerySelection(SelectQuery query)
    {
        Query = query;
    }

    public SelectQuery IntoQuery()
    {
        var query = RequestHelpers.ApplyRelationSelections(Query, RelationSelections);
        return RequestHelpers.ApplyRuntimeMetadata(query, QueryOptions, ChildEnhancements);
    }
}

public record RelationSelection
{
    public string Name { get; set; }
    public SelectQuery Query { get; set; }
    public List<RelationSelection> RelationSelections { get; set; } = new();
    public List<RelationFilter> RelationFilters { get; set; } = new();
    public List<QuerySelection> ChildEnhancements { get; set; } = new();
    public QueryOptions QueryOptions { get; set; } = new();

    public RelationSelection(string name, QuerySelection selection)
    {
        Name = name;
        Query = selection.Query;
        RelationSelections = selection.RelationSelections;
        RelationFilters = selection.RelationFilters;
        ChildEnhancements = selection.ChildEnhancements;
        QueryOptions = selection.QueryOptions;
    }

    public SelectQuery IntoQuery()
    {
        var query = RequestHelpers.ApplyRelationSelections(Query, RelationSelections);
        return RequestHelpers.ApplyRuntimeMetadata(query, QueryOptions, ChildEnhancements);
    }
}

public record RelationFilter
{
    public string Name { get; set; }
    public SelectQuery Query { get; set; }
    public List<RelationSelection> RelationSelections { get; set; } = new();
    public List<RelationFilter> RelationFilters { get; set; } = new();
    public List<QuerySelection> ChildEnhancements { get; set; } = new();
    public QueryOptions QueryOptions { get; set; } = new();

    public RelationFilter(string name, QuerySelection selection)
    {
        Name = name;
        Query = selection.Query;
        RelationSelections = selection.RelationSelections;
        RelationFilters = selection.RelationFilters;
        ChildEnhancements = selection.ChildEnhancements;
        QueryOptions = selection.QueryOptions;
    }
}

public record QueryOptions
{
    public string? Comment { get; set; }
    public string? RawSql { get; set; }
    public List<string> RawSqlSearchCriteria { get; set; } = new();
    public List<RawDynamicProperty> DynamicProperties { get; set; } = new();
    public List<BuilderRawProjection> RawProjections { get; set; } = new();
    public List<BuilderRelationAggregate> RelationAggregates { get; set; } = new();
    public List<BuilderObjectGroupBy> ObjectGroupBys { get; set; } = new();
    public List<FacetRequest> Facets { get; set; } = new();
}

public record UnsafeRawSqlSegment
{
    public string Sql { get; }
    private UnsafeRawSqlSegment(string sql) => Sql = sql;
    public static UnsafeRawSqlSegment Trusted(string sql) => new(sql);
}

public record RawDynamicProperty(string PropertyName, string RawSqlSegment)
{
    public RawDynamicProperty(string propertyName, UnsafeRawSqlSegment rawSqlSegment)
        : this(propertyName, rawSqlSegment.Sql) { }
}

public record BuilderRawProjection(string PropertyName, string RawSqlSegment)
{
    public BuilderRawProjection(string propertyName, UnsafeRawSqlSegment rawSqlSegment)
        : this(propertyName, rawSqlSegment.Sql) { }
}

public record BuilderRelationAggregate(string RelationName, string Alias, QuerySelection Query, bool SingleResult);

public record FacetRequest(string FacetName, string RelationName, QuerySelection Query, bool IncludeAllFacets);

public record BuilderObjectGroupBy(string PropertyName, string StorageField, QuerySelection Query);

public interface IEntityReference
{
    Value EntityIdValue();
}

public static class EntityReferenceExtensions
{
    public static Value EntityIdValue(this Value val) => val;
    public static Value EntityIdValue(this ulong val) => new Value.U64Value(val);
}

public static class RequestHelpers
{
    public static SelectQuery ApplyRelationSelections(SelectQuery query, IEnumerable<RelationSelection> relationSelections)
    {
        var currentQuery = query;
        foreach (var selection in relationSelections)
        {
            var relations = new List<RelationLoad>(currentQuery.Relations);
            relations.Add(new RelationLoad(selection.Name, selection.IntoQuery()));
            currentQuery = currentQuery with { Relations = relations };
        }
        return currentQuery;
    }

    public static SelectQuery ApplyRuntimeMetadata(SelectQuery query, QueryOptions options, IEnumerable<QuerySelection> childEnhancements)
    {
        var q = query;
        if (options.Comment != null)
        {
            q = q with { Comment = options.Comment };
        }
        q = q with
        {
            RawSql = options.RawSql,
            RawSqlSearchCriteria = options.RawSqlSearchCriteria.ToList(),
            DynamicProperties = options.DynamicProperties.Select(p => new RawSqlProjection(p.PropertyName, p.RawSqlSegment)).ToList(),
            RawProjections = options.RawProjections.Select(p => new RawSqlProjection(p.PropertyName, p.RawSqlSegment)).ToList(),
            ObjectGroupBys = options.ObjectGroupBys.Select(g => new ObjectGroupBy(g.PropertyName, g.StorageField, g.Query.IntoQuery())).ToList(),
            ChildEnhancements = childEnhancements.Select(c => c.IntoQuery()).ToList()
        };
        return q;
    }

    public static List<RelationAggregate> RuntimeRelationAggregates(QueryOptions options)
    {
        return options.RelationAggregates.Select(a => new RelationAggregate(
            a.RelationName,
            a.Alias,
            a.Query.IntoQuery(),
            a.SingleResult
        )).ToList();
    }

    public static void MergeOuterFilterIntoFacetAggregates(QuerySelection selection, SelectQuery outerQuery)
    {
        if (outerQuery.Filter == null) return;

        for (int i = 0; i < selection.QueryOptions.RelationAggregates.Count; i++)
        {
            var aggregate = selection.QueryOptions.RelationAggregates[i];
            if (aggregate.Query.Query.Entity == outerQuery.Entity)
            {
                var newFilter = aggregate.Query.Query.Filter == null
                    ? outerQuery.Filter
                    : new Expr.AndExpr(new List<Expr> { aggregate.Query.Query.Filter, outerQuery.Filter });
                
                var newQuery = aggregate.Query.Query with { Filter = newFilter };
                aggregate.Query.Query = newQuery;
            }
        }
    }

    public static void AttachFacets<T>(SmartList<T> rows, Dictionary<string, SmartList<Record>> facets)
    {
        foreach (var kvp in facets)
        {
            rows.Facets[kvp.Key] = kvp.Value;
        }
    }

    public static Expr FieldOperatorExpr(string field, FieldOperator op, List<Value> values)
    {
        return op switch
        {
            FieldOperator.Equal => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.Eq, new Expr.ValueExpr(RequiredValue(op, values, 0))),
            FieldOperator.NotEqual => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.Ne, new Expr.ValueExpr(RequiredValue(op, values, 0))),
            FieldOperator.GreaterThan => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.Gt, new Expr.ValueExpr(RequiredValue(op, values, 0))),
            FieldOperator.GreaterThanOrEqual => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.Gte, new Expr.ValueExpr(RequiredValue(op, values, 0))),
            FieldOperator.LessThan => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.Lt, new Expr.ValueExpr(RequiredValue(op, values, 0))),
            FieldOperator.LessThanOrEqual => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.Lte, new Expr.ValueExpr(RequiredValue(op, values, 0))),
            FieldOperator.Between => new Expr.BetweenExpr(new Expr.ColumnExpr(field), new Expr.ValueExpr(RequiredValue(op, values, 0)), new Expr.ValueExpr(RequiredValue(op, values, 1))),
            FieldOperator.In => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.In, new Expr.ValueExpr(new Value.ListValue(values))),
            FieldOperator.NotIn => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.NotIn, new Expr.ValueExpr(new Value.ListValue(values))),
            FieldOperator.Contain => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.Like, new Expr.ValueExpr(new Value.TextValue($"%{RequiredText(op, values, 0)}%"))),
            FieldOperator.NotContain => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.NotLike, new Expr.ValueExpr(new Value.TextValue($"%{RequiredText(op, values, 0)}%"))),
            FieldOperator.BeginWith => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.Like, new Expr.ValueExpr(new Value.TextValue($"{RequiredText(op, values, 0)}%"))),
            FieldOperator.NotBeginWith => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.NotLike, new Expr.ValueExpr(new Value.TextValue($"{RequiredText(op, values, 0)}%"))),
            FieldOperator.EndWith => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.Like, new Expr.ValueExpr(new Value.TextValue($"%{RequiredText(op, values, 0)}"))),
            FieldOperator.NotEndWith => new Expr.BinaryExpr(new Expr.ColumnExpr(field), BinaryOp.NotLike, new Expr.ValueExpr(new Value.TextValue($"%{RequiredText(op, values, 0)}"))),
            FieldOperator.SoundsLike => new Expr.BinaryExpr(new Expr.FunctionExpr(ExprFunction.Soundex, new List<Expr> { new Expr.ColumnExpr(field) }), BinaryOp.Eq, new Expr.FunctionExpr(ExprFunction.Soundex, new List<Expr> { new Expr.ValueExpr(RequiredValue(op, values, 0)) })),
            FieldOperator.IsNull => new Expr.IsNullExpr(new Expr.ColumnExpr(field)),
            FieldOperator.IsNotNull => new Expr.IsNotNullExpr(new Expr.ColumnExpr(field)),
            _ => throw new NotImplementedException()
        };
    }

    public static Expr FieldOperatorColumnExpr(string field, FieldOperator op, string otherField)
    {
        var binaryOp = op switch
        {
            FieldOperator.Equal => BinaryOp.Eq,
            FieldOperator.NotEqual => BinaryOp.Ne,
            FieldOperator.GreaterThan => BinaryOp.Gt,
            FieldOperator.GreaterThanOrEqual => BinaryOp.Gte,
            FieldOperator.LessThan => BinaryOp.Lt,
            FieldOperator.LessThanOrEqual => BinaryOp.Lte,
            FieldOperator.Contain => BinaryOp.Like,
            FieldOperator.NotContain => BinaryOp.NotLike,
            FieldOperator.BeginWith => BinaryOp.Like,
            FieldOperator.NotBeginWith => BinaryOp.NotLike,
            FieldOperator.EndWith => BinaryOp.Like,
            FieldOperator.NotEndWith => BinaryOp.NotLike,
            _ => throw new ArgumentException($"{op} is not supported for property-to-property filters")
        };
        return new Expr.BinaryExpr(new Expr.ColumnExpr(field), binaryOp, new Expr.ColumnExpr(otherField));
    }

    public static Value RequiredValue(FieldOperator op, List<Value> values, int index)
    {
        if (index < 0 || index >= values.Count)
        {
            throw new ArgumentException($"{op} requires value at index {index}");
        }
        return values[index];
    }

    public static string RequiredText(FieldOperator op, List<Value> values, int index)
    {
        var val = RequiredValue(op, values, index);
        if (val is Value.TextValue textVal)
        {
            return textVal.Value;
        }
        throw new ArgumentException($"{op} requires text value, got {val}");
    }

    public static Expr? RemoveDefaultLiveFilter(Expr? filter)
    {
        if (filter == null) return null;
        var defaultFilter = new Expr.BinaryExpr(new Expr.ColumnExpr("version"), BinaryOp.Gt, new Expr.ValueExpr(new Value.I64Value(0)));
        return RemoveFilterExpr(filter, defaultFilter);
    }

    public static Expr? RemoveFilterExpr(Expr filter, Expr target)
    {
        if (filter.Equals(target)) return null;

        if (filter is Expr.AndExpr andExpr)
        {
            var retained = andExpr.Parts.Select(p => RemoveFilterExpr(p, target)).Where(p => p != null).Select(p => p!).ToList();
            if (retained.Count == 0) return null;
            if (retained.Count == 1) return retained[0];
            return new Expr.AndExpr(retained);
        }

        return filter;
    }

    public static Value DynamicJsonValueToTeaqlValue(JsonNode? value)
    {
        if (value == null) return new Value.NullValue();

        if (value is JsonValue jsonValue)
        {
            if (jsonValue.TryGetValue<bool>(out var b)) return new Value.BoolValue(b);
            if (jsonValue.TryGetValue<long>(out var l)) return new Value.I64Value(l);
            if (jsonValue.TryGetValue<ulong>(out var ul)) return new Value.U64Value(ul);
            if (jsonValue.TryGetValue<double>(out var d)) return new Value.F64Value(d);
            if (jsonValue.TryGetValue<string>(out var s)) return new Value.TextValue(s.Trim());
            return new Value.NullValue();
        }
        
        if (value is JsonArray jsonArray)
        {
            var list = jsonArray.Select(DynamicJsonValueToTeaqlValue).ToList();
            return new Value.ListValue(list);
        }

        if (value is JsonObject jsonObject)
        {
            if (jsonObject.TryGetPropertyValue("id", out var idNode))
            {
                return DynamicJsonValueToTeaqlValue(idNode);
            }
            return new Value.NullValue();
        }

        return new Value.NullValue();
    }

    public static List<Value> DynamicJsonValues(JsonNode? value)
    {
        if (value is JsonArray jsonArray)
        {
            return jsonArray.Select(DynamicJsonValueToTeaqlValue).ToList();
        }
        return new List<Value> { DynamicJsonValueToTeaqlValue(value) };
    }

    public static FieldOperator DynamicJsonOperator(JsonNode? value)
    {
        if (value is JsonValue jsonValue)
        {
            if (jsonValue.TryGetValue<string>(out var s))
            {
                if (string.Equals(s, "__is_null__", StringComparison.OrdinalIgnoreCase)) return FieldOperator.IsNull;
                if (string.Equals(s, "__is_not_null__", StringComparison.OrdinalIgnoreCase)) return FieldOperator.IsNotNull;
                return FieldOperator.Contain;
            }
            if (jsonValue.TryGetValue<double>(out _) || jsonValue.TryGetValue<bool>(out _))
            {
                return FieldOperator.Equal;
            }
        }
        if (value is JsonArray jsonArray)
        {
            if (jsonArray.Count > 0 && jsonArray[0] is JsonValue firstVal && firstVal.TryGetValue<string>(out _)) return FieldOperator.In;
            if (jsonArray.Count > 0 && jsonArray[0] is JsonObject) return FieldOperator.In;
            if (jsonArray.Count == 2) return FieldOperator.Between;
        }

        return FieldOperator.Equal;
    }

    public static Expr DynamicJsonFilterExpr(string field, JsonNode? value)
    {
        var op = DynamicJsonOperator(value);
        return FieldOperatorExpr(field, op, DynamicJsonValues(value));
    }

    public static ulong? DynamicJsonU64Field(JsonObject obj, string field)
    {
        if (obj.TryGetPropertyValue(field, out var val))
        {
            if (val is JsonValue jsonVal)
            {
                if (jsonVal.TryGetValue<ulong>(out var u)) return u;
                if (jsonVal.TryGetValue<long>(out var l) && l >= 0) return (ulong)l;
            }
        }
        return null;
    }
}
