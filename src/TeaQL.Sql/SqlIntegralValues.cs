using TeaQL.Core;

namespace TeaQL.Sql;

// The logical range belongs to model metadata, never to the spelling of a SQL alias.
internal static class SqlIntegralValues
{
    internal static Value Bind(PropertyDescriptor property, Value value)
    {
        if (value is not Value.DecimalValue || property.DataType is not (DataType.I64 or DataType.U64)) return value;
        if (property.DataType == DataType.U64 && value.TryU64() is { } unsigned) return new Value.U64Value(unsigned);
        if (property.DataType == DataType.I64 && value.TryI64() is { } signed
            && (!property.IsInt32 || value.TryI32().HasValue)) return new Value.I64Value(signed);
        throw new SqlCompileException("Invalid integral binding for " + property.Name);
    }

    internal static Action<Record> ResultValidator(EntityDescriptor entity, SelectQuery query)
    {
        var fields = entity.Properties.Where(p => p.DataType is DataType.I64 or DataType.U64)
            .ToDictionary(p => p.Name, p => p, StringComparer.Ordinal);
        foreach (var projection in query.ExprProjection) fields.Remove(projection.Alias);
        foreach (var projection in query.RawProjections.Concat(query.DynamicProperties)) fields.Remove(projection.PropertyName);
        foreach (var aggregate in query.AggregateItems) fields.Remove(aggregate.Alias);
        foreach (var projection in query.ExprProjection)
            if (projection.Expr is Expr.ColumnExpr column && entity.PropertyByName(column.Name) is { } property
                && property.DataType is DataType.I64 or DataType.U64) fields[projection.Alias] = property;
        foreach (var aggregate in query.AggregateItems)
        {
            if (aggregate.Function == AggregateFunction.Count)
                fields[aggregate.Alias] = PropertyDescriptor.New(aggregate.Alias, DataType.I64);
            else if (aggregate.Function is AggregateFunction.Min or AggregateFunction.Max or AggregateFunction.Sum
                && entity.PropertyByName(aggregate.Field) is { } property && property.DataType is DataType.I64 or DataType.U64)
                fields[aggregate.Alias] = aggregate.Function == AggregateFunction.Sum
                    ? PropertyDescriptor.New(aggregate.Alias, property.DataType) : property;
        }
        // One validation plan per request, shared by all rows and stream chunks.
        return row => {
            foreach (var (alias, property) in fields)
            {
                if (!row.TryGetValue(alias, out var value) || value is Value.NullValue or Value.TypedNullValue) continue;
                var valid = property.DataType == DataType.U64 ? value.TryU64().HasValue
                    : property.IsInt32 ? value.TryI32().HasValue : value.TryI64().HasValue;
                if (!valid) throw new SqlExecutorException("Invalid integral result " + entity.Name + "." + alias
                    + ": expected an exact integer in the declared model range");
            }
        };
    }
}
