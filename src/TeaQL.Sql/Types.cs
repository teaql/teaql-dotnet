using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using TeaQL.Core;

namespace TeaQL.Sql;

public enum DatabaseKind
{
    PostgreSql,
    Sqlite,
    MySql,
    SqlServer
}

public class CompiledQuery
{
    public string Sql { get; }
    public List<Value> Params { get; }
    public string? Comment { get; }

    public CompiledQuery(string sql, List<Value> @params, string? comment = null)
    {
        Sql = sql;
        Params = @params;
        Comment = comment;
    }

    public string SqlWithComment()
    {
        if (!string.IsNullOrEmpty(Comment))
        {
            var escaped = Comment.Replace("*/", "* /");
            return $"/* {escaped} */ {Sql}";
        }
        return Sql;
    }

    public string DebugSql(DatabaseKind kind)
    {
        var sql = SqlWithComment();
        return kind switch
        {
            DatabaseKind.PostgreSql => ReplacePostgresPlaceholders(sql, Params),
            DatabaseKind.Sqlite => ReplacePositionalPlaceholders(sql, Params, DatabaseKind.Sqlite),
            DatabaseKind.MySql => ReplacePositionalPlaceholders(sql, Params, DatabaseKind.MySql),
            DatabaseKind.SqlServer => ReplaceSqlServerPlaceholders(sql, Params),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }

    private static void HandleSqlQuote(IEnumerator<char> chars, ref string output, ref bool inString)
    {
        output += '\'';
        // In C# IEnumerator, we can't easily peek without writing a wrapper.
        // We'll implement a simple peekable reader in the caller.
    }

    private static string ReplacePostgresPlaceholders(string sql, List<Value> parameters)
    {
        var output = new System.Text.StringBuilder(sql.Length);
        bool inString = false;
        
        for (int i = 0; i < sql.Length; i++)
        {
            char ch = sql[i];
            if (ch == '\'')
            {
                output.Append('\'');
                if (inString && i + 1 < sql.Length && sql[i + 1] == '\'')
                {
                    output.Append('\'');
                    i++;
                }
                else
                {
                    inString = !inString;
                }
                continue;
            }
            if (!inString && ch == '$' && i + 1 < sql.Length && char.IsDigit(sql[i + 1]))
            {
                var indexStr = new System.Text.StringBuilder();
                int j = i + 1;
                while (j < sql.Length && char.IsDigit(sql[j]))
                {
                    indexStr.Append(sql[j]);
                    j++;
                }
                if (int.TryParse(indexStr.ToString(), out int index) && index > 0 && index <= parameters.Count)
                {
                    output.Append(SqlLiteral(parameters[index - 1], DatabaseKind.PostgreSql));
                    i = j - 1;
                    continue;
                }
                output.Append('$');
                output.Append(indexStr);
                i = j - 1;
                continue;
            }
            output.Append(ch);
        }
        return output.ToString();
    }

    private static string ReplaceSqlServerPlaceholders(string sql, List<Value> parameters)
    {
        var output = new System.Text.StringBuilder(sql.Length);
        bool inString = false;
        
        for (int i = 0; i < sql.Length; i++)
        {
            char ch = sql[i];
            if (ch == '\'')
            {
                output.Append('\'');
                if (inString && i + 1 < sql.Length && sql[i + 1] == '\'')
                {
                    output.Append('\'');
                    i++;
                }
                else
                {
                    inString = !inString;
                }
                continue;
            }
            if (!inString && ch == '@' && i + 1 < sql.Length && sql[i + 1] == 'p' && i + 2 < sql.Length && char.IsDigit(sql[i + 2]))
            {
                var indexStr = new System.Text.StringBuilder();
                int j = i + 2;
                while (j < sql.Length && char.IsDigit(sql[j]))
                {
                    indexStr.Append(sql[j]);
                    j++;
                }
                if (int.TryParse(indexStr.ToString(), out int index) && index > 0 && index <= parameters.Count)
                {
                    output.Append(SqlLiteral(parameters[index - 1], DatabaseKind.SqlServer));
                    i = j - 1;
                    continue;
                }
                output.Append("@p");
                output.Append(indexStr);
                i = j - 1;
                continue;
            }
            output.Append(ch);
        }
        return output.ToString();
    }

    private static string ReplacePositionalPlaceholders(string sql, List<Value> parameters, DatabaseKind kind)
    {
        var output = new System.Text.StringBuilder(sql.Length);
        var state = SqlScanState.Sql;
        int paramIndex = 0;

        for (int i = 0; i < sql.Length; i++)
        {
            char ch = sql[i];
            if (state == SqlScanState.Sql && ch == '\'')
            {
                output.Append('\'');
                state = SqlScanState.SingleQuote;
                continue;
            }
            if (state == SqlScanState.Sql && ch == '"')
            {
                output.Append(ch);
                state = SqlScanState.DoubleQuote;
                continue;
            }
            if (state == SqlScanState.Sql && ch == '-' && i + 1 < sql.Length && sql[i + 1] == '-')
            {
                output.Append("--");
                i++;
                state = SqlScanState.LineComment;
                continue;
            }
            if (state == SqlScanState.Sql && ch == '/' && i + 1 < sql.Length && sql[i + 1] == '*')
            {
                output.Append("/*");
                i++;
                state = SqlScanState.BlockComment;
                continue;
            }
            if (state == SqlScanState.SingleQuote)
            {
                output.Append(ch);
                if (ch == '\'' && i + 1 < sql.Length && sql[i + 1] == '\'')
                {
                    output.Append('\'');
                    i++;
                }
                else if (ch == '\'')
                {
                    state = SqlScanState.Sql;
                }
                continue;
            }
            if (state == SqlScanState.DoubleQuote)
            {
                output.Append(ch);
                if (ch == '"' && i + 1 < sql.Length && sql[i + 1] == '"')
                {
                    output.Append('"');
                    i++;
                }
                else if (ch == '"')
                {
                    state = SqlScanState.Sql;
                }
                continue;
            }
            if (state == SqlScanState.LineComment)
            {
                output.Append(ch);
                if (ch == '\r' || ch == '\n') state = SqlScanState.Sql;
                continue;
            }
            if (state == SqlScanState.BlockComment)
            {
                output.Append(ch);
                if (ch == '*' && i + 1 < sql.Length && sql[i + 1] == '/')
                {
                    output.Append('/');
                    i++;
                    state = SqlScanState.Sql;
                }
                continue;
            }
            if (ch == '?')
            {
                if (paramIndex < parameters.Count)
                {
                    output.Append(SqlLiteral(parameters[paramIndex], kind));
                    paramIndex++;
                }
                else
                {
                    output.Append(ch);
                }
                continue;
            }
            if (ch == '@' && i + 2 < sql.Length && sql[i + 1] == 'p'
                && char.IsDigit(sql[i + 2]))
            {
                var end = i + 2;
                while (end < sql.Length && char.IsDigit(sql[end])) end++;
                var parameterIndex = int.Parse(sql[(i + 2)..end]);
                output.Append(parameterIndex < parameters.Count
                    ? SqlLiteral(parameters[parameterIndex], kind)
                    : sql[i..end]);
                i = end - 1;
                continue;
            }
            output.Append(ch);
        }
        return output.ToString();
    }

    private enum SqlScanState { Sql, SingleQuote, DoubleQuote, LineComment, BlockComment }

    private static string SqlBoolLiteral(bool value, DatabaseKind kind) => kind == DatabaseKind.SqlServer
        ? (value ? "1" : "0")
        : (value ? "TRUE" : "FALSE");

    private static string SqlLiteral(Value value, DatabaseKind kind)
    {
        return value switch
        {
            Value.NullValue => "NULL",
            Value.BoolValue b => SqlBoolLiteral(b.Value, kind),
            Value.I64Value i => i.Value.ToString(),
            Value.U64Value u => u.Value.ToString(),
            Value.F64Value f => f.Value.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Value.DecimalValue d => d.Value.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Value.TextValue t => QuotedSqlString(t.Value),
            Value.JsonValue j => QuotedSqlString(j.Value?.ToJsonString() ?? "null"),
            Value.DateValue d => QuotedSqlString(d.Value.ToString("yyyy-MM-dd")),
            Value.TimestampValue t => t.Milliseconds.ToString(),
            Value.ObjectValue o => QuotedSqlString(System.Text.Json.JsonSerializer.Serialize(o.Value)),
            Value.ListValue l => FormatList(l.Values, kind),
            Value.TypedNullValue => "NULL",
            _ => throw new NotSupportedException($"Unsupported value type: {value.GetType()}")
        };
    }

    private static string FormatList(List<Value> values, DatabaseKind kind)
    {
        var formatted = string.Join(", ", values.Select(v => SqlLiteral(v, kind)));
        return kind == DatabaseKind.PostgreSql ? $"ARRAY[{formatted}]" : $"({formatted})";
    }

    private static string QuotedSqlString(string value)
    {
        return $"'{value.Replace("'", "''")}'";
    }
}

public class SqlCompileException : Exception
{
    public SqlCompileException(string message) : base(message) { }
    public SqlCompileException(string message, Exception innerException) : base(message, innerException) { }

    public static SqlCompileException UnknownEntity(string entity) => new($"unknown entity: {entity}");
    public static SqlCompileException UnknownField(string field) => new($"unknown field: {field}");
    public static SqlCompileException EmptyInList() => new("IN requires at least one value");
    public static SqlCompileException MissingIdProperty(string entity) => new($"entity {entity} has no id property");
    public static SqlCompileException MissingVersionProperty(string entity) => new($"entity {entity} has no version property");
    public static SqlCompileException EmptyMutation(string kind) => new($"{kind} requires at least one writable field");
    public static SqlCompileException InvalidRecoverVersion(long version) => new($"recover requires a negative version, got {version}");
    public static SqlCompileException UnsupportedSchemaType(DataType dataType) => new($"unsupported schema type: {dataType}");
    public static SqlCompileException InvalidFunctionArguments(string message) => new(message);
    public static SqlCompileException InvalidSubQueryOperator(string op) => new($"subquery does not support operator: {op}");
}

public interface ISchemaProvider
{
    EntityDescriptor? GetEntity(string name);
}
