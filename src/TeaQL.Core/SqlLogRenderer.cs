using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TeaQL.Core;

/// <summary>Strict diagnostic rendering, shared by compiled debug SQL and safe logs.
/// The caller supplies already-projected values; this never edits driver bindings.</summary>
public static class SqlLogRenderer
{
    private static string Kind(string backend) => backend.ToLowerInvariant() switch
    {
        "" or "sqlite" => "sqlite",
        "postgres" or "postgresql" => "postgresql",
        "mysql" => "mysql",
        "sqlserver" => "sqlserver",
        _ => throw new ArgumentOutOfRangeException(nameof(backend), "unknown_database_dialect")
    };

    public static string Render(string sql, int count, Func<int, string> literal, string backend)
    {
        var kind = Kind(backend);
        var output = new StringBuilder(sql.Length);
        var used = new bool[count];
        int positional = 0;
        string Binding(int index)
        {
            if (index < 0 || index >= count) throw new ArgumentException("binding_mismatch");
            used[index] = true;
            return literal(index);
        }
        for (int i = 0; i < sql.Length;)
        {
            int start = i;
            char ch = sql[i];
            if (ch is '\'' or '"' or '`' || (ch == '[' && kind is "sqlite" or "sqlserver"))
            {
                char close = ch == '[' ? ']' : ch;
                if (kind == "postgresql" && ch == '\'' && i > 0 && sql[i - 1] is 'e' or 'E')
                    throw new ArgumentException("unsupported_escape_string");
                i++;
                bool closed = false;
                while (i < sql.Length)
                {
                    if (sql[i] == '\\' && kind == "mysql") throw new ArgumentException("ambiguous_mysql_escape");
                    if (sql[i++] != close) continue;
                    if (i < sql.Length && sql[i] == close) { i++; continue; }
                    closed = true; break;
                }
                if (!closed) throw new ArgumentException("unterminated_quote");
                output.Append(sql.AsSpan(start, i - start)); continue;
            }
            if (ch == '-' && i + 1 < sql.Length && sql[i + 1] == '-')
            {
                i += 2;
                if (kind == "mysql" && i < sql.Length && !char.IsWhiteSpace(sql[i]))
                    throw new ArgumentException("ambiguous_mysql_comment");
                while (i < sql.Length && sql[i] is not '\n' and not '\r') i++;
                output.Append(sql.AsSpan(start, i - start)); continue;
            }
            if (ch == '/' && i + 1 < sql.Length && sql[i + 1] == '*')
            {
                i += 2; int depth = 1;
                if (kind == "mysql" && i < sql.Length && sql[i] is '!' or '+')
                    throw new ArgumentException("executable_mysql_comment");
                while (i < sql.Length && depth > 0)
                {
                    if (i + 1 < sql.Length && sql[i] == '/' && sql[i + 1] == '*') { depth++; i += 2; }
                    else if (i + 1 < sql.Length && sql[i] == '*' && sql[i + 1] == '/') { depth--; i += 2; }
                    else i++;
                }
                if (depth != 0) throw new ArgumentException("unterminated_comment");
                output.Append(sql.AsSpan(start, i - start)); continue;
            }
            if (ch == '#' && kind == "mysql") throw new ArgumentException("unsupported_mysql_comment");
            if (ch == '$' && kind == "postgresql")
            {
                var tag = Regex.Match(sql[i..], @"^\$(?:[A-Za-z_][A-Za-z_0-9]*)?\$");
                if (tag.Success)
                {
                    int end = sql.IndexOf(tag.Value, i + tag.Length, StringComparison.Ordinal);
                    if (end < 0) throw new ArgumentException("unterminated_dollar_quote");
                    i = end + tag.Length;
                    output.Append(sql.AsSpan(start, i - start)); continue;
                }
            }
            int digits = -1, offset = 0;
            if (ch == '$' && kind == "postgresql") { digits = i + 1; offset = 1; }
            else if (ch == '@' && i + 2 < sql.Length && sql[i + 1] == 'p')
            { digits = i + 2; offset = kind == "sqlserver" ? 1 : 0; }
            if (digits >= 0)
            {
                int end = digits;
                while (end < sql.Length && char.IsAsciiDigit(sql[end])) end++;
                if (end == digits || !int.TryParse(sql.AsSpan(digits, end - digits), out int number))
                    throw new ArgumentException("unsupported_binding");
                if (end < sql.Length && (char.IsLetterOrDigit(sql[end]) || sql[end] == '_'))
                    throw new ArgumentException("unsupported_binding");
                output.Append(Binding(number - offset)); i = end; continue;
            }
            if (ch == '?')
            {
                if (i + 1 < sql.Length && char.IsAsciiDigit(sql[i + 1])) throw new ArgumentException("unsupported_binding");
                output.Append(Binding(positional++)); i++; continue;
            }
            if (ch is '@' or '$' || (ch == ':' && i + 1 < sql.Length && sql[i + 1] != ':'
                && (i == 0 || sql[i - 1] != ':') && (char.IsLetter(sql[i + 1]) || sql[i + 1] == '_')))
                throw new ArgumentException("unsupported_binding");
            output.Append(ch); i++;
        }
        if (used.Any(value => !value)) throw new ArgumentException("unused_binding");
        return output.ToString();
    }

    // The previous CompiledQuery literal rules live here so both log paths agree.
    public static string Literal(Value value, string backend)
    {
        var kind = Kind(backend);
        string Quote(string text)
        {
            if (text.Contains('\0') || (kind == "mysql" && text.Contains('\\')))
                throw new ArgumentException("unsupported_literal");
            return "'" + text.Replace("'", "''") + "'";
        }
        string Number<T>(T n) where T : IFormattable => n.ToString(null, CultureInfo.InvariantCulture);
        switch (value)
        {
            case Value.NullValue or Value.TypedNullValue: return "NULL";
            case Value.BoolValue b: return kind == "sqlserver" ? (b.Value ? "1" : "0") : (b.Value ? "TRUE" : "FALSE");
            case Value.I64Value v: return Number(v.Value);
            case Value.U64Value v: return Number(v.Value);
            case Value.DecimalValue v: return Number(v.Value);
            case Value.F64Value v when double.IsFinite(v.Value): return Number(v.Value);
            case Value.TextValue v: return Quote(v.Value);
            case Value.JsonValue v: return Quote(v.Value?.ToJsonString() ?? "null");
            case Value.ObjectValue v: return Quote(System.Text.Json.JsonSerializer.Serialize(v.Value));
            case Value.ListValue v:
                var values = string.Join(", ", v.Values.Select(item => Literal(item, kind)));
                return kind == "postgresql" ? $"ARRAY[{values}]" : $"({values})";
            case Value.DateValue v:
                var date = Quote(v.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                return kind == "postgresql" ? $"DATE {date}" : kind == "sqlite" ? date : $"CAST({date} AS DATE)";
            case Value.TimestampValue v:
                if (kind == "sqlite") return Number(v.Milliseconds);
                var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(v.Milliseconds).UtcDateTime
                    .ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                return kind switch {
                    "postgresql" => $"TIMESTAMPTZ '{timestamp}Z'",
                    "mysql" => $"CAST('{timestamp}' AS DATETIME(3))",
                    _ => $"CAST('{timestamp}+00:00' AS DATETIMEOFFSET(3))"
                };
            default: throw new ArgumentException("unsupported_literal");
        }
    }
}
