using System;
using System.Collections.Generic;
using System.Linq;
using TeaQL.Core;

namespace TeaQL.Sql;

public static class DialectUtils
{
    private static readonly string[] SqlKeywords = {
        "all", "alter", "and", "as", "asc", "between", "by", "case", "create", "delete", "desc",
        "distinct", "drop", "exists", "false", "from", "group", "having", "in", "insert", "into", "is",
        "join", "like", "limit", "not", "null", "offset", "on", "or", "order", "select", "set",
        "table", "true", "type", "union", "update", "values", "where"
    };

    static DialectUtils()
    {
        Array.Sort(SqlKeywords);
    }

    public static string QuoteIdentifierIfNeeded(string ident, char quote)
    {
        if (IsWrappedIdentifier(ident)) return ident;
        if (NeedsQuotedIdentifier(ident))
        {
            var quoteString = quote.ToString();
            var escaped = ident.Replace(quoteString, quoteString + quoteString);
            return $"{quote}{escaped}{quote}";
        }
        return ident;
    }

    private static bool IsWrappedIdentifier(string ident)
    {
        return (ident.StartsWith("\"") && ident.EndsWith("\""))
            || (ident.StartsWith("`") && ident.EndsWith("`"))
            || (ident.StartsWith("[") && ident.EndsWith("]"));
    }

    private static bool NeedsQuotedIdentifier(string ident)
    {
        if (string.IsNullOrEmpty(ident) || Array.BinarySearch(SqlKeywords, ident.ToLowerInvariant()) >= 0)
            return true;
        char first = ident[0];
        if (first != '_' && !char.IsLetter(first)) return true;
        return ident.Any(ch => ch != '_' && !char.IsLetterOrDigit(ch));
    }
}

public abstract class SqlDialect
{
    public abstract DatabaseKind Kind { get; }
    public abstract string QuoteIdent(string ident);
    public abstract string Placeholder(int index);

    public virtual string[] SchemaSetupSqls() => Array.Empty<string>();

    public virtual string SchemaTypeSql(DataType dataType, PropertyDescriptor property)
    {
        return dataType switch
        {
            DataType.Bool => "BOOLEAN",
            DataType.I64 or DataType.U64 => "INTEGER",
            DataType.F64 => "REAL",
            DataType.Decimal => "NUMERIC",
            DataType.Text => "VARCHAR(255)",
            DataType.LargeText or DataType.Json or DataType.Date or DataType.Timestamp => "TEXT",
            _ => throw SqlCompileException.UnsupportedSchemaType(dataType)
        };
    }

    public virtual string ColumnDefinitionSql(PropertyDescriptor property)
    {
        var parts = new List<string> { QuoteIdent(property.ColumnName), SchemaTypeSql(property.DataType, property) };
        if (property.IsId) parts.Add("PRIMARY KEY");
        if (property.IsId || !property.Nullable) parts.Add("NOT NULL");
        return string.Join(" ", parts);
    }

    public virtual string CompileCreateTable(EntityDescriptor entity)
    {
        var columns = string.Join(", ", entity.Properties.Select(ColumnDefinitionSql));
        return $"CREATE TABLE IF NOT EXISTS {QuoteIdent(entity.TableName)} ({columns})";
    }

    public virtual List<string> SchemaIndexesSqls(EntityDescriptor entity)
    {
        var sqls = new List<string>();
        var tableNameUpper = entity.TableName.ToUpperInvariant();
        var quotedTable = QuoteIdent(entity.TableName);

        var versionCol = entity.Properties.FirstOrDefault(p => p.IsVersion);
        if (versionCol != null)
        {
            var idCol = entity.Properties.FirstOrDefault(p => p.IsId)?.ColumnName ?? "id";
            var idxName = $"PK_{tableNameUpper}_ID_VERSION";
            sqls.Add($"CREATE UNIQUE INDEX IF NOT EXISTS {QuoteIdent(idxName)} ON {quotedTable} ({QuoteIdent(idCol)}, {QuoteIdent(versionCol.ColumnName)})");
        }

        foreach (var p in entity.Properties)
        {
            if (p.Name.EndsWith("Id") || p.Name.EndsWith("Time") || p.Name.EndsWith("_time") ||
                p.Name == "create_time" || p.Name == "update_time")
            {
                var idxName = $"IDX_{tableNameUpper}_{p.ColumnName.ToUpperInvariant()}";
                sqls.Add($"CREATE INDEX IF NOT EXISTS {QuoteIdent(idxName)} ON {quotedTable} ({QuoteIdent(p.ColumnName)})");
            }
        }
        return sqls;
    }

    public virtual string FallbackDefaultValueSql(DataType dataType)
    {
        return dataType switch
        {
            DataType.Bool => "FALSE",
            DataType.I64 or DataType.U64 or DataType.F64 or DataType.Decimal => "0",
            DataType.Text or DataType.LargeText => "''",
            DataType.Json => "'{}'",
            DataType.Date => "'1970-01-01'",
            DataType.Timestamp => "'1970-01-01 00:00:00Z'",
            _ => throw SqlCompileException.UnsupportedSchemaType(dataType)
        };
    }

    public virtual string CompileAddColumn(EntityDescriptor entity, PropertyDescriptor property)
    {
        var def = ColumnDefinitionSql(property);
        if (!property.Nullable && !property.IsId)
        {
            def += $" DEFAULT {FallbackDefaultValueSql(property.DataType)}";
        }
        return $"ALTER TABLE {QuoteIdent(entity.TableName)} ADD COLUMN {def}";
    }

    public virtual CompiledQuery CompileSelect(EntityDescriptor entity, SelectQuery query)
    {
        var paramsList = new List<Value>();
        var sql = CompileSelectSql(entity, query, paramsList);
        return new CompiledQuery(sql, paramsList, query.Comment);
    }

    public virtual string CompileSelectSql(EntityDescriptor entity, SelectQuery query, List<Value> paramsList)
    {
        if (!string.IsNullOrEmpty(query.RawSql))
        {
            return query.RawSql;
        }
        var projection = CompileProjection(entity, query, paramsList);
        var partitioned = query.PartitionBy != null && query.Slice != null;
        if (partitioned)
        {
            var partitionColumn = ColumnSql(entity, query.PartitionBy!);
            var windowOrder = query.OrderBy.Count == 0
                ? ""
                : $" ORDER BY {string.Join(", ", query.OrderBy.Select(order => OrderBySql(entity, order, paramsList)))}";
            projection += $", ROW_NUMBER() OVER (PARTITION BY {partitionColumn}{windowOrder}) AS {QuoteIdent("__teaql_partition_rank")}";
        }

        var sql = $"SELECT {projection} FROM {QuoteIdent(entity.TableName)}";
        var whereParts = new List<string>();

        if (query.Filter != null)
        {
            whereParts.Add(CompileExpr(entity, query.Filter, paramsList));
        }

        if (!string.IsNullOrEmpty(query.SearchWithText))
        {
            var orParts = new List<string>();
            var likeValue = $"%{query.SearchWithText}%";
            foreach (var property in entity.Properties)
            {
                if (property.DataType == DataType.Text || property.DataType == DataType.LargeText)
                {
                    paramsList.Add(new Value.TextValue(likeValue));
                    orParts.Add($"{QuoteIdent(property.ColumnName)} LIKE {Placeholder(paramsList.Count)}");
                }
            }
            if (orParts.Count > 0)
            {
                whereParts.Add($"({string.Join(" OR ", orParts)})");
            }
        }

        if (query.RawSqlSearchCriteria != null && query.RawSqlSearchCriteria.Count > 0)
        {
            whereParts.AddRange(query.RawSqlSearchCriteria);
        }

        if (whereParts.Count > 0)
        {
            sql += $" WHERE {string.Join(" AND ", whereParts)}";
        }

        if (partitioned)
        {
            var rank = QuoteIdent("__teaql_partition_rank");
            var predicates = new List<string> { $"{rank} > {query.Slice!.Offset}" };
            if (query.Slice.Limit.HasValue)
            {
                predicates.Add($"{rank} <= {query.Slice.Offset + query.Slice.Limit.Value}");
            }
            return $"SELECT * FROM ({sql}) AS {QuoteIdent("__teaql_partitioned")} WHERE {string.Join(" AND ", predicates)} ORDER BY {rank}";
        }

        if (query.GroupBy != null && query.GroupBy.Count > 0)
        {
            var groupBy = string.Join(", ", query.GroupBy.Select(field => ColumnSql(entity, field)));
            sql += $" GROUP BY {groupBy}";
        }

        if (query.Having != null)
        {
            var havingSql = CompileExpr(entity, query.Having, paramsList);
            sql += $" HAVING {havingSql}";
        }

        if (query.OrderBy != null && query.OrderBy.Count > 0)
        {
            var orderBy = string.Join(", ", query.OrderBy.Select(order => OrderBySql(entity, order, paramsList)));
            sql += $" ORDER BY {orderBy}";
        }

        if (query.Slice != null)
        {
            if (query.Slice.Limit.HasValue)
            {
                sql += $" LIMIT {query.Slice.Limit.Value}";
            }
            if (query.Slice.Offset > 0)
            {
                sql += $" OFFSET {query.Slice.Offset}";
            }
        }

        return sql;
    }

    public virtual CompiledQuery CompileInsert(EntityDescriptor entity, InsertCommand command)
    {
        var columns = new List<string>();
        var placeholders = new List<string>();
        var paramsList = new List<Value>();

        foreach (var property in entity.Properties)
        {
            if (command.Values.TryGetValue(property.Name, out var value))
            {
                columns.Add(QuoteIdent(property.ColumnName));
                if (value is Value.NullValue)
                {
                    value = new Value.TypedNullValue(property.DataType);
                }
                paramsList.Add(value);
                placeholders.Add(Placeholder(paramsList.Count));
            }
        }

        if (columns.Count == 0)
            throw SqlCompileException.EmptyMutation("insert");

        var sql = $"INSERT INTO {QuoteIdent(entity.TableName)} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", placeholders)})";
        return new CompiledQuery(sql, paramsList, null);
    }

    public virtual CompiledQuery CompileBatchInsert(EntityDescriptor entity, BatchInsertCommand command)
    {
        if (command.BatchValues == null || command.BatchValues.Count == 0)
            throw SqlCompileException.EmptyMutation("batch_insert");

        var columns = new List<PropertyDescriptor>();
        var firstRecord = command.BatchValues[0];

        foreach (var property in entity.Properties)
        {
            if (firstRecord.ContainsKey(property.Name))
            {
                columns.Add(property);
            }
        }

        if (columns.Count == 0)
            throw SqlCompileException.EmptyMutation("batch_insert");

        var columnNames = columns.Select(p => QuoteIdent(p.ColumnName)).ToList();
        var paramsList = new List<Value>();
        var valuesClauses = new List<string>();

        foreach (var record in command.BatchValues)
        {
            var rowPlaceholders = new List<string>();
            foreach (var property in columns)
            {
                if (!record.TryGetValue(property.Name, out var value))
                {
                    value = new Value.NullValue();
                }
                if (value is Value.NullValue)
                {
                    value = new Value.TypedNullValue(property.DataType);
                }
                paramsList.Add(value);
                rowPlaceholders.Add(Placeholder(paramsList.Count));
            }
            valuesClauses.Add($"({string.Join(", ", rowPlaceholders)})");
        }

        var sql = $"INSERT INTO {QuoteIdent(entity.TableName)} ({string.Join(", ", columnNames)}) VALUES {string.Join(", ", valuesClauses)}";
        return new CompiledQuery(sql, paramsList, null);
    }

    public virtual CompiledQuery CompileUpdate(EntityDescriptor entity, UpdateCommand command)
    {
        var idProperty = entity.Properties.FirstOrDefault(p => p.IsId) 
            ?? throw SqlCompileException.MissingIdProperty(entity.Name);

        var assignments = new List<string>();
        var paramsList = new List<Value>();

        foreach (var property in entity.Properties)
        {
            if (property.IsId) continue;
            if (property.IsVersion && command.ExpectedVersion.HasValue) continue;

            if (command.Values.TryGetValue(property.Name, out var value))
            {
                if (value is Value.NullValue)
                {
                    value = new Value.TypedNullValue(property.DataType);
                }
                paramsList.Add(value);
                assignments.Add($"{QuoteIdent(property.ColumnName)} = {Placeholder(paramsList.Count)}");
            }
        }

        if (command.ExpectedVersion.HasValue)
        {
            var versionProperty = entity.Properties.FirstOrDefault(p => p.IsVersion)
                ?? throw SqlCompileException.MissingVersionProperty(entity.Name);
            paramsList.Add(new Value.I64Value(command.ExpectedVersion.Value + 1));
            assignments.Add($"{QuoteIdent(versionProperty.ColumnName)} = {Placeholder(paramsList.Count)}");
        }

        if (assignments.Count == 0)
            throw SqlCompileException.EmptyMutation("update");

        paramsList.Add(command.Id);
        var predicates = new List<string> { $"{QuoteIdent(idProperty.ColumnName)} = {Placeholder(paramsList.Count)}" };

        if (command.ExpectedVersion.HasValue)
        {
            var versionProperty = entity.Properties.FirstOrDefault(p => p.IsVersion)
                ?? throw SqlCompileException.MissingVersionProperty(entity.Name);
            paramsList.Add(new Value.I64Value(command.ExpectedVersion.Value));
            predicates.Add($"{QuoteIdent(versionProperty.ColumnName)} = {Placeholder(paramsList.Count)}");
        }

        var sql = $"UPDATE {QuoteIdent(entity.TableName)} SET {string.Join(", ", assignments)} WHERE {string.Join(" AND ", predicates)}";
        return new CompiledQuery(sql, paramsList, null);
    }

    public virtual CompiledQuery CompileBatchUpdate(EntityDescriptor entity, BatchUpdateCommand command)
    {
        if (command.BatchValues == null || command.BatchValues.Count == 0)
            throw SqlCompileException.EmptyMutation("batch_update");

        var idProperty = entity.Properties.FirstOrDefault(p => p.IsId)
            ?? throw SqlCompileException.MissingIdProperty(entity.Name);

        var paramsList = new List<Value>();
        var setClauses = new List<string>();

        foreach (var fieldName in command.UpdateFields)
        {
            var property = entity.Properties.FirstOrDefault(p => p.Name == fieldName)
                ?? throw SqlCompileException.UnknownField(fieldName);

            var caseParts = new List<string> { $"CASE {QuoteIdent(idProperty.ColumnName)}" };

            for (int i = 0; i < command.BatchValues.Count; i++)
            {
                var id = command.BatchIds[i];
                if (!command.BatchValues[i].TryGetValue(fieldName, out var val))
                    val = new Value.NullValue();
                if (val is Value.NullValue)
                    val = new Value.TypedNullValue(property.DataType);

                paramsList.Add(id);
                var idPh = Placeholder(paramsList.Count);

                paramsList.Add(val);
                var valPh = Placeholder(paramsList.Count);

                caseParts.Add($"WHEN {idPh} THEN {valPh}");
            }

            caseParts.Add($"ELSE {QuoteIdent(property.ColumnName)} END");
            setClauses.Add($"{QuoteIdent(property.ColumnName)} = {string.Join(" ", caseParts)}");
        }

        bool hasVersions = false;
        var versionProperty = entity.Properties.FirstOrDefault(p => p.IsVersion);
        if (versionProperty != null)
        {
            var caseParts = new List<string> { $"CASE {QuoteIdent(idProperty.ColumnName)}" };

            for (int i = 0; i < command.BatchExpectedVersions.Count; i++)
            {
                var expVerOpt = command.BatchExpectedVersions[i];
                if (expVerOpt.HasValue)
                {
                    hasVersions = true;
                    var id = command.BatchIds[i];

                    paramsList.Add(id);
                    var idPh = Placeholder(paramsList.Count);

                    paramsList.Add(new Value.I64Value(expVerOpt.Value + 1));
                    var valPh = Placeholder(paramsList.Count);

                    caseParts.Add($"WHEN {idPh} THEN {valPh}");
                }
            }

            if (hasVersions)
            {
                caseParts.Add($"ELSE {QuoteIdent(versionProperty.ColumnName)} END");
                setClauses.Add($"{QuoteIdent(versionProperty.ColumnName)} = {string.Join(" ", caseParts)}");
            }
        }

        if (setClauses.Count == 0)
            throw SqlCompileException.EmptyMutation("batch_update");

        var inPlaceholders = new List<string>();
        foreach (var id in command.BatchIds)
        {
            paramsList.Add(id);
            inPlaceholders.Add(Placeholder(paramsList.Count));
        }

        var predicates = new List<string> { $"{QuoteIdent(idProperty.ColumnName)} IN ({string.Join(", ", inPlaceholders)})" };

        if (hasVersions && versionProperty != null)
        {
            var caseParts = new List<string> { $"CASE {QuoteIdent(idProperty.ColumnName)}" };

            for (int i = 0; i < command.BatchExpectedVersions.Count; i++)
            {
                var expVerOpt = command.BatchExpectedVersions[i];
                if (expVerOpt.HasValue)
                {
                    var id = command.BatchIds[i];

                    paramsList.Add(id);
                    var idPh = Placeholder(paramsList.Count);

                    paramsList.Add(new Value.I64Value(expVerOpt.Value));
                    var valPh = Placeholder(paramsList.Count);

                    caseParts.Add($"WHEN {idPh} THEN {valPh}");
                }
            }
            caseParts.Add($"ELSE {QuoteIdent(versionProperty.ColumnName)} END");

            predicates.Add($"{QuoteIdent(versionProperty.ColumnName)} = {string.Join(" ", caseParts)}");
        }

        var sql = $"UPDATE {QuoteIdent(entity.TableName)} SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", predicates)}";
        return new CompiledQuery(sql, paramsList, null);
    }

    public virtual CompiledQuery CompileDelete(EntityDescriptor entity, DeleteCommand command)
    {
        var idProperty = entity.Properties.FirstOrDefault(p => p.IsId)
            ?? throw SqlCompileException.MissingIdProperty(entity.Name);
        var paramsList = new List<Value>();

        if (command.SoftDelete)
        {
            var versionProperty = entity.Properties.FirstOrDefault(p => p.IsVersion)
                ?? throw SqlCompileException.MissingVersionProperty(entity.Name);

            paramsList.Add(new Value.I64Value(command.ExpectedVersion.HasValue ? -(command.ExpectedVersion.Value + 1) : -1));
            paramsList.Add(command.Id);

            var predicates = new List<string> { $"{QuoteIdent(idProperty.ColumnName)} = {Placeholder(paramsList.Count)}" };

            if (command.ExpectedVersion.HasValue)
            {
                paramsList.Add(new Value.I64Value(command.ExpectedVersion.Value));
                predicates.Add($"{QuoteIdent(versionProperty.ColumnName)} = {Placeholder(paramsList.Count)}");
            }

            var sqlSoft = $"UPDATE {QuoteIdent(entity.TableName)} SET {QuoteIdent(versionProperty.ColumnName)} = {Placeholder(1)} WHERE {string.Join(" AND ", predicates)}";
            return new CompiledQuery(sqlSoft, paramsList, null);
        }

        paramsList.Add(command.Id);
        var preds = new List<string> { $"{QuoteIdent(idProperty.ColumnName)} = {Placeholder(paramsList.Count)}" };

        if (command.ExpectedVersion.HasValue)
        {
            var versionProperty = entity.Properties.FirstOrDefault(p => p.IsVersion)
                ?? throw SqlCompileException.MissingVersionProperty(entity.Name);
            paramsList.Add(new Value.I64Value(command.ExpectedVersion.Value));
            preds.Add($"{QuoteIdent(versionProperty.ColumnName)} = {Placeholder(paramsList.Count)}");
        }

        var sqlHard = $"DELETE FROM {QuoteIdent(entity.TableName)} WHERE {string.Join(" AND ", preds)}";
        return new CompiledQuery(sqlHard, paramsList, null);
    }

    public virtual CompiledQuery CompileRecover(EntityDescriptor entity, RecoverCommand command)
    {
        if (command.ExpectedVersion >= 0)
            throw SqlCompileException.InvalidRecoverVersion(command.ExpectedVersion);

        var idProperty = entity.Properties.FirstOrDefault(p => p.IsId)
            ?? throw SqlCompileException.MissingIdProperty(entity.Name);
        var versionProperty = entity.Properties.FirstOrDefault(p => p.IsVersion)
            ?? throw SqlCompileException.MissingVersionProperty(entity.Name);

        var paramsList = new List<Value>
        {
            new Value.I64Value(-command.ExpectedVersion + 1),
            command.Id,
            new Value.I64Value(command.ExpectedVersion)
        };

        var sql = $"UPDATE {QuoteIdent(entity.TableName)} SET {QuoteIdent(versionProperty.ColumnName)} = {Placeholder(1)} " +
                  $"WHERE {QuoteIdent(idProperty.ColumnName)} = {Placeholder(2)} AND {QuoteIdent(versionProperty.ColumnName)} = {Placeholder(3)}";
        
        return new CompiledQuery(sql, paramsList, null);
    }

    public virtual string ColumnSql(EntityDescriptor entity, string field)
    {
        var property = entity.Properties.FirstOrDefault(p => p.Name == field)
            ?? throw SqlCompileException.UnknownField(field);
        return QuoteIdent(property.ColumnName);
    }

    public virtual string OrderBySql(EntityDescriptor entity, OrderBy orderBy, List<Value> paramsList)
    {
        var field = ResolveOrderField(entity, orderBy, paramsList);
        var direction = orderBy.Direction == SortDirection.Asc ? "ASC" : "DESC";
        return $"{field} {direction}";
    }

    public virtual string SelectProjection(EntityDescriptor entity, SelectQuery query, List<Value> paramsList)
    {
        string PropertyProjection(PropertyDescriptor property) => ColumnWithAlias(property);

        if ((query.Projection == null || query.Projection.Count == 0) &&
            (query.ExprProjection == null || query.ExprProjection.Count == 0) &&
            (query.RawProjections == null || query.RawProjections.Count == 0) &&
            (query.DynamicProperties == null || query.DynamicProperties.Count == 0))
        {
            return string.Join(", ", entity.Properties.Select(PropertyProjection));
        }

        var parts = new List<string>();
        if (query.Projection != null)
        {
            foreach (var field in query.Projection)
            {
                var property = entity.Properties.FirstOrDefault(p => p.Name == field)
                    ?? throw SqlCompileException.UnknownField(field);
                parts.Add(PropertyProjection(property));
            }
        }

        if (query.ExprProjection != null)
        {
            foreach (var projection in query.ExprProjection)
            {
                var expr = CompileExpr(entity, projection.Expr, paramsList);
                parts.Add($"{expr} AS {QuoteIdent(projection.Alias)}");
            }
        }

        var rawAndDyn = (query.RawProjections ?? Enumerable.Empty<RawSqlProjection>())
            .Concat(query.DynamicProperties ?? Enumerable.Empty<RawSqlProjection>());
        foreach (var projection in rawAndDyn)
        {
            parts.Add($"{projection.RawSqlSegment} AS {QuoteIdent(projection.PropertyName)}");
        }

        return string.Join(", ", parts);
    }

    public virtual string AggregateProjection(EntityDescriptor entity, SelectQuery query, List<Value> paramsList)
    {
        var parts = new List<string>();
        
        var groupsAndProj = (query.GroupBy ?? Enumerable.Empty<string>())
            .Concat(query.Projection ?? Enumerable.Empty<string>());
            
        foreach (var field in groupsAndProj)
        {
            var column = ColumnSql(entity, field);
            if (!parts.Contains(column)) parts.Add(column);
        }

        if (query.ExprProjection != null)
        {
            foreach (var projection in query.ExprProjection)
            {
                var expr = CompileExpr(entity, projection.Expr, paramsList);
                var aliased = $"{expr} AS {QuoteIdent(projection.Alias)}";
                if (!parts.Contains(aliased)) parts.Add(aliased);
            }
        }

        var rawAndDyn = (query.RawProjections ?? Enumerable.Empty<RawSqlProjection>())
            .Concat(query.DynamicProperties ?? Enumerable.Empty<RawSqlProjection>());
        foreach (var projection in rawAndDyn)
        {
            var aliased = $"{projection.RawSqlSegment} AS {QuoteIdent(projection.PropertyName)}";
            if (!parts.Contains(aliased)) parts.Add(aliased);
        }

        if (query.Aggregates != null)
        {
            foreach (var aggregate in query.Aggregates)
            {
                var field = ResolveAggregateField(entity, aggregate);
                var call = AggregateCallSql(aggregate.Function, field);
                parts.Add($"{call} AS {QuoteIdent(aggregate.Alias)}");
            }
        }

        return string.Join(", ", parts);
    }

    public virtual string AggregateCallSql(AggregateFunction function, string field)
    {
        var functionSql = AggregateFunctionSql(function);
        return $"{functionSql}({field})";
    }

    public virtual string AggregateFunctionSql(AggregateFunction function)
    {
        return function switch
        {
            AggregateFunction.Count => "COUNT",
            AggregateFunction.Sum => "SUM",
            AggregateFunction.Avg => "AVG",
            AggregateFunction.Min => "MIN",
            AggregateFunction.Max => "MAX",
            AggregateFunction.Stddev => "STDDEV",
            AggregateFunction.StddevPop => "STDDEV_POP",
            AggregateFunction.VarSamp => "VAR_SAMP",
            AggregateFunction.VarPop => "VAR_POP",
            AggregateFunction.BitAnd => "BIT_AND",
            AggregateFunction.BitOr => "BIT_OR",
            AggregateFunction.BitXor => "BIT_XOR",
            _ => throw new NotSupportedException($"Unknown aggregate function: {function}")
        };
    }

    public virtual string CompileExpr(EntityDescriptor entity, Expr expr, List<Value> paramsList)
    {
        switch (expr)
        {
            case Expr.ColumnExpr col:
                return ColumnSql(entity, col.Name);
            case Expr.ValueExpr val:
                paramsList.Add(val.Value);
                return Placeholder(paramsList.Count);
            case Expr.FunctionExpr func:
                return CompileFunction(entity, func.Fn, func.Args, paramsList);
            case Expr.BinaryExpr bin:
                return CompileBinaryExpr(entity, bin, paramsList);
            case Expr.SubQueryExpr sub:
                return CompileSubQuery(entity, sub.Left, sub.Op, sub.Entity, sub.Query, paramsList);
            case Expr.BetweenExpr bet:
                return CompileBetween(entity, bet, paramsList);
            case Expr.IsNullExpr isn:
                return $"({CompileExpr(entity, isn.Expr1, paramsList)} IS NULL)";
            case Expr.IsNotNullExpr isnn:
                return $"({CompileExpr(entity, isnn.Expr1, paramsList)} IS NOT NULL)";
            case Expr.AndExpr and:
                return CompileJoined(entity, and.Parts, "AND", paramsList);
            case Expr.OrExpr or:
                return CompileJoined(entity, or.Parts, "OR", paramsList);
            case Expr.NotExpr not:
                return $"(NOT {CompileExpr(entity, not.Expr1, paramsList)})";
            default:
                throw new NotSupportedException($"Unsupported expression: {expr.GetType()}");
        }
    }

    private string CompileBinaryExpr(EntityDescriptor entity, Expr.BinaryExpr bin, List<Value> paramsList)
    {
        if (bin.Op is BinaryOp.In or BinaryOp.NotIn or BinaryOp.InLarge or BinaryOp.NotInLarge)
        {
            return CompileIn(entity, bin.Left, bin.Op, bin.Right, paramsList);
        }
        var lhs = CompileExpr(entity, bin.Left, paramsList);
        var rhs = CompileExpr(entity, bin.Right, paramsList);
        var op = bin.Op switch
        {
            BinaryOp.Eq => "=",
            BinaryOp.Ne => "!=",
            BinaryOp.Gt => ">",
            BinaryOp.Gte => ">=",
            BinaryOp.Lt => "<",
            BinaryOp.Lte => "<=",
            BinaryOp.Like => "LIKE",
            BinaryOp.NotLike => "NOT LIKE",
            _ => throw new InvalidOperationException("Unreachable")
        };
        return $"({lhs} {op} {rhs})";
    }

    private string CompileBetween(EntityDescriptor entity, Expr.BetweenExpr bet, List<Value> paramsList)
    {
        var expr = CompileExpr(entity, bet.Expr1, paramsList);
        var lower = CompileExpr(entity, bet.Lower, paramsList);
        var upper = CompileExpr(entity, bet.Upper, paramsList);
        return $"({expr} BETWEEN {lower} AND {upper})";
    }

    public virtual string CompileFunction(EntityDescriptor entity, ExprFunction function, List<Expr> args, List<Value> paramsList)
    {
        switch (function)
        {
            case ExprFunction.Soundex:
                if (args.Count != 1) throw SqlCompileException.InvalidFunctionArguments("SOUNDEX expects exactly one argument");
                var argSoundex = CompileExpr(entity, args[0], paramsList);
                return $"SOUNDEX({argSoundex})";
            case ExprFunction.Gbk:
                return CompileGbkFunction(entity, args, paramsList);
            case ExprFunction.Count when args.Count == 0:
                return "COUNT(*)";
            case ExprFunction.Count: return CompileSingleArgFunction(entity, "COUNT", args, paramsList);
            case ExprFunction.Sum: return CompileSingleArgFunction(entity, "SUM", args, paramsList);
            case ExprFunction.Avg: return CompileSingleArgFunction(entity, "AVG", args, paramsList);
            case ExprFunction.Min: return CompileSingleArgFunction(entity, "MIN", args, paramsList);
            case ExprFunction.Max: return CompileSingleArgFunction(entity, "MAX", args, paramsList);
            case ExprFunction.Stddev: return CompileSingleArgFunction(entity, "STDDEV", args, paramsList);
            case ExprFunction.StddevPop: return CompileSingleArgFunction(entity, "STDDEV_POP", args, paramsList);
            case ExprFunction.VarSamp: return CompileSingleArgFunction(entity, "VAR_SAMP", args, paramsList);
            case ExprFunction.VarPop: return CompileSingleArgFunction(entity, "VAR_POP", args, paramsList);
            case ExprFunction.BitAnd: return CompileSingleArgFunction(entity, "BIT_AND", args, paramsList);
            case ExprFunction.BitOr: return CompileSingleArgFunction(entity, "BIT_OR", args, paramsList);
            case ExprFunction.BitXor: return CompileSingleArgFunction(entity, "BIT_XOR", args, paramsList);
            default:
                throw new NotSupportedException($"Unknown function: {function}");
        }
    }

    public virtual string CompileSingleArgFunction(EntityDescriptor entity, string function, List<Expr> args, List<Value> paramsList)
    {
        if (args.Count != 1)
            throw SqlCompileException.InvalidFunctionArguments($"{function} expects exactly one argument");
        var arg = CompileExpr(entity, args[0], paramsList);
        return $"{function}({arg})";
    }

    public virtual string CompileGbkFunction(EntityDescriptor entity, List<Expr> args, List<Value> paramsList)
    {
        if (args.Count != 1)
            throw SqlCompileException.InvalidFunctionArguments("GBK expects exactly one argument");
        return CompileExpr(entity, args[0], paramsList);
    }

    public virtual string CompileSubQuery(EntityDescriptor entity, Expr left, BinaryOp op, EntityDescriptor subEntity, SelectQuery query, List<Value> paramsList)
    {
        var lhs = CompileExpr(entity, left, paramsList);
        var operatorSql = op switch
        {
            BinaryOp.In or BinaryOp.InLarge => "IN",
            BinaryOp.NotIn or BinaryOp.NotInLarge => "NOT IN",
            _ => throw SqlCompileException.InvalidSubQueryOperator(op.ToString())
        };
        var subquery = CompileSelectSql(subEntity, query, paramsList);
        return $"({lhs} {operatorSql} ({subquery}))";
    }

    public virtual string CompileJoined(EntityDescriptor entity, List<Expr> parts, string joiner, List<Value> paramsList)
    {
        var compiled = parts.Select(part => CompileExpr(entity, part, paramsList)).ToList();
        return $"({string.Join($" {joiner} ", compiled)})";
    }

    public virtual string CompileIn(EntityDescriptor entity, Expr left, BinaryOp op, Expr right, List<Value> paramsList)
    {
        var lhs = CompileExpr(entity, left, paramsList);
        var operatorSql = op switch
        {
            BinaryOp.In or BinaryOp.InLarge => "IN",
            BinaryOp.NotIn or BinaryOp.NotInLarge => "NOT IN",
            _ => throw new InvalidOperationException("Unreachable")
        };

        if (right is Expr.ValueExpr valExpr && valExpr.Value is Value.ListValue listVal)
        {
            if (listVal.Values.Count == 0)
            {
                throw SqlCompileException.EmptyInList();
            }
            var placeholders = new List<string>(listVal.Values.Count);
            foreach (var value in listVal.Values)
            {
                paramsList.Add(value);
                placeholders.Add(Placeholder(paramsList.Count));
            }
            return $"({lhs} {operatorSql} ({string.Join(", ", placeholders)}))";
        }
        else
        {
            var rhs = CompileExpr(entity, right, paramsList);
            return $"({lhs} {operatorSql} ({rhs}))";
        }
    }

    public virtual string CompileProjection(EntityDescriptor entity, SelectQuery query, List<Value> paramsList)
    {
        if (query.Aggregates == null || query.Aggregates.Count == 0)
        {
            return SelectProjection(entity, query, paramsList);
        }
        return AggregateProjection(entity, query, paramsList);
    }

    public virtual string ResolveOrderField(EntityDescriptor entity, OrderBy orderBy, List<Value> paramsList)
    {
        if (orderBy.Expr != null)
        {
            return CompileExpr(entity, orderBy.Expr, paramsList);
        }
        return ColumnSql(entity, orderBy.Field);
    }

    public virtual string ColumnWithAlias(PropertyDescriptor property)
    {
        var column = QuoteIdent(property.ColumnName);
        if (property.ColumnName == property.Name)
            return column;
        return $"{column} AS {QuoteIdent(property.Name)}";
    }

    public virtual string ResolveAggregateField(EntityDescriptor entity, Aggregate aggregate)
    {
        if (aggregate.Function == AggregateFunction.Count && aggregate.Field == "*")
            return "*";
        return ColumnSql(entity, aggregate.Field);
    }
}
