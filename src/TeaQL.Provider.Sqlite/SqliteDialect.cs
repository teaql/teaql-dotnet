using System;
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
            DataType.Decimal => "TEAQL_DECIMAL_TEXT COLLATE TEAQL_DECIMAL",
            DataType.Text => "VARCHAR(255)",
            DataType.LargeText => "TEXT",
            DataType.Json => "JSON",
            DataType.Date => "DATE",
            DataType.Time => "TEAQL_TIME_TEXT",
            DataType.Timestamp => "TIMESTAMP",
            _ => throw SqlCompileException.UnsupportedSchemaType(dataType)
        };
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
            AggregateFunction.Sum => $"teaql_decimal_sum({field})",
            AggregateFunction.Min => $"teaql_decimal_result(MIN({field}))",
            AggregateFunction.Max => $"teaql_decimal_result(MAX({field}))",
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
                "SUM" => $"teaql_decimal_sum({field})",
                "MIN" => $"teaql_decimal_result(MIN({field}))",
                "MAX" => $"teaql_decimal_result(MAX({field}))",
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
