using System;
using System.Collections.Frozen;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.Sqlite;

public class SqliteDialect : SqlDialect
{
    public override DatabaseKind Kind => DatabaseKind.Sqlite;
    public override string RelationTopNPolicy => "always_probe";

    public override string QuoteIdent(string ident)
    {
        return DialectUtils.QuoteIdentifierIfNeeded(ident, '"');
    }

    public override string Placeholder(int index)
    {
        return $"@p{index - 1}";
    }

    public override string[] SchemaSetupSqls() =>
        [OptimisticBusinessIdAllocator.SqliteSchemaSql];

    public override string SchemaTypeSql(DataType dataType, PropertyDescriptor property)
    {
        return dataType switch
        {
            DataType.Bool => "INTEGER",
            DataType.I64 or DataType.U64 when property.IsId => "INTEGER",
            DataType.I64 or DataType.U64 => "INTEGER",
            DataType.F64 => "REAL",
            // TEXT affinity preserves every modeled base-10 digit; the column
            // collation keeps SQL range predicates and ORDER BY numeric.
            DataType.Decimal => ExactDecimalColumn(property),
            DataType.Text => "VARCHAR(255)",
            DataType.LargeText => "TEXT",
            DataType.Json => "JSON",
            DataType.Date => "DATE",
            DataType.Time => "TEAQL_TIME_TEXT",
            DataType.Timestamp => "TIMESTAMP",
            _ => throw SqlCompileException.UnsupportedSchemaType(dataType)
        };
    }

    private string ExactDecimalColumn(PropertyDescriptor property)
    {
        var column = QuoteIdent(property.ColumnNameString);
        return $"TEAQL_DECIMAL_TEXT COLLATE TEAQL_DECIMAL CHECK ({column} IS NULL OR " +
            $"(typeof({column}) = 'text' AND teaql_decimal_valid({column})))";
    }

    protected override IReadOnlySet<string> ExactDecimalResultAliases(EntityDescriptor entity, SelectQuery query)
    {
        if (!string.IsNullOrEmpty(query.RawSqlText)
            || (query.Aggregates.Count == 0 && query.ExprProjection.Count == 0))
            return FrozenSet<string>.Empty;
        HashSet<string>? aliases = null;
        foreach (var aggregate in query.Aggregates)
            if (aggregate.Function is AggregateFunction.Sum or AggregateFunction.Min or AggregateFunction.Max
                && entity.PropertyByName(aggregate.Field)?.DataType == DataType.Decimal)
                (aliases ??= new(StringComparer.Ordinal)).Add(aggregate.Alias);
        foreach (var projection in query.ExprProjection)
        {
            var argument = projection.Expr switch
            {
                Expr.ColumnExpr column => column,
                Expr.FunctionExpr { Fn: ExprFunction.Sum or ExprFunction.Min or ExprFunction.Max, Args.Count: 1 } function
                    => function.Args[0] as Expr.ColumnExpr,
                _ => null
            };
            if (argument is not null && entity.PropertyByName(argument.Name)?.DataType == DataType.Decimal)
                (aliases ??= new(StringComparer.Ordinal)).Add(projection.Alias);
        }
        return aliases is null ? FrozenSet<string>.Empty : aliases.ToFrozenSet(StringComparer.Ordinal);
    }

    public override string CompileAddColumn(EntityDescriptor entity, PropertyDescriptor property)
    {
        var def = ColumnDefinitionSql(property);
        var defWithoutNotNull = def.Replace(" NOT NULL", "");
        return $"ALTER TABLE {QuoteIdent(entity.TableNameValue)} ADD COLUMN {defWithoutNotNull}";
    }

    public override string AggregateCallSql(EntityDescriptor entity, Aggregate aggregate, string field)
    {
        if (entity.PropertyByName(aggregate.Field)?.DataType != DataType.Decimal)
            return base.AggregateCallSql(entity, aggregate, field);

        return aggregate.Function switch
        {
            AggregateFunction.Sum => $"(teaql_decimal_sum({field}) COLLATE TEAQL_DECIMAL)",
            AggregateFunction.Min => $"(MIN({field}) COLLATE TEAQL_DECIMAL)",
            AggregateFunction.Max => $"(MAX({field}) COLLATE TEAQL_DECIMAL)",
            AggregateFunction.Avg => throw new SqlCompileException(
                "SQLite Decimal AVG requires an explicit rounding policy; it cannot use floating-point AVG."),
            AggregateFunction.Stddev or AggregateFunction.StddevPop or
                AggregateFunction.VarSamp or AggregateFunction.VarPop => throw new SqlCompileException(
                    $"SQLite Decimal {aggregate.Function} requires an explicit precision policy."),
            _ => base.AggregateCallSql(entity, aggregate, field)
        };
    }

    public override string CompileSingleArgFunction(EntityDescriptor entity, string function,
        System.Collections.Generic.List<Expr> args, System.Collections.Generic.List<Value> paramsList)
    {
        if (args.Count == 1 && ContainsDecimalOperand(entity, args[0]))
        {
            if (args[0] is not Expr.ColumnExpr column ||
                entity.PropertyByName(column.Name)?.DataType != DataType.Decimal)
                throw new SqlCompileException($"SQLite Decimal {function} expressions require an explicit precision policy.");
            var field = ColumnSql(entity, column.Name);
            return function switch
            {
                "SUM" => $"(teaql_decimal_sum({field}) COLLATE TEAQL_DECIMAL)",
                "MIN" => $"(MIN({field}) COLLATE TEAQL_DECIMAL)",
                "MAX" => $"(MAX({field}) COLLATE TEAQL_DECIMAL)",
                "AVG" or "STDDEV" or "STDDEV_POP" or "VAR_SAMP" or "VAR_POP" =>
                    throw new SqlCompileException($"SQLite Decimal {function} requires an explicit precision policy."),
                _ => base.CompileSingleArgFunction(entity, function, args, paramsList)
            };
        }

        return base.CompileSingleArgFunction(entity, function, args, paramsList);
    }

    private static bool ContainsDecimalOperand(EntityDescriptor entity, Expr expression) => expression switch
    {
        Expr.ColumnExpr column => entity.PropertyByName(column.Name)?.DataType == DataType.Decimal,
        Expr.ValueExpr value => value.NodeValue is Value.DecimalValue,
        Expr.FunctionExpr function => function.Args.Exists(arg => ContainsDecimalOperand(entity, arg)),
        Expr.BinaryExpr binary => ContainsDecimalOperand(entity, binary.Left) || ContainsDecimalOperand(entity, binary.Right),
        Expr.BetweenExpr between => ContainsDecimalOperand(entity, between.Expr1) ||
            ContainsDecimalOperand(entity, between.Lower) || ContainsDecimalOperand(entity, between.Upper),
        Expr.IsNullExpr isNull => ContainsDecimalOperand(entity, isNull.Expr1),
        Expr.IsNotNullExpr isNotNull => ContainsDecimalOperand(entity, isNotNull.Expr1),
        Expr.AndExpr and => and.Parts.Exists(part => ContainsDecimalOperand(entity, part)),
        Expr.OrExpr or => or.Parts.Exists(part => ContainsDecimalOperand(entity, part)),
        Expr.NotExpr not => ContainsDecimalOperand(entity, not.Expr1),
        _ => false
    };
}
