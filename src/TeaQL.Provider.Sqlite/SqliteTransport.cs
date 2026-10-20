using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.Sqlite;

public class SqliteTransport : IStreamingSqlTransport, IAutomaticMutationTransactionTransport,
    ISchemaConnectionInitializer, ISchemaCompatibilityVerifier
{
    private readonly SqliteConnection _connection;

    public SqliteTransport(SqliteConnection connection)
    {
        _connection = connection;
        _connection.CreateCollation("TEAQL_DECIMAL", CompareExactDecimal);
        _connection.CreateFunction<string?, string?>("teaql_decimal_result", value =>
            value is null ? null : "__teaql_decimal__:" + ParseExactDecimal(value).ToString(CultureInfo.InvariantCulture),
            isDeterministic: true);
        _connection.CreateAggregate(
            "teaql_decimal_sum",
            (HasValue: false, Sum: 0m),
            ((bool HasValue, decimal Sum) state, string? value) =>
                value is null ? state : (true, checked(state.Sum + ParseExactDecimal(value))),
            state => state.HasValue
                ? "__teaql_decimal__:" + state.Sum.ToString(CultureInfo.InvariantCulture)
                : null);
    }

    private static decimal ParseExactDecimal(string value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        return decimal.Parse(value, styles, CultureInfo.InvariantCulture);
    }

    private static int CompareExactDecimal(string? left, string? right)
    {
        if (left is null) return right is null ? 0 : -1;
        if (right is null) return 1;
        return ParseExactDecimal(left).CompareTo(ParseExactDecimal(right));
    }

    public Task EnsureSchemaFunctionsAsync()
    {
        _connection.CreateFunction<string?, string>("soundex", Soundex, isDeterministic: true);
        return Task.CompletedTask;
    }

    public async Task VerifySchemaCompatibilityAsync(SqlDialect dialect, EntityDescriptor entity)
    {
        var decimals = entity.Properties.Where(property => property.DataType == DataType.Decimal).ToArray();
        if (decimals.Length == 0) return;
        using var command = _connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({dialect.QuoteIdent(entity.TableNameValue)})";
        using var reader = await command.ExecuteReaderAsync();
        var types = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        while (await reader.ReadAsync())
            types[reader.GetString(1)] = reader.GetString(2);
        foreach (var property in decimals)
        {
            if (!types.TryGetValue(property.ColumnNameString, out var actual)
                || !string.Equals(actual, "TEAQL_DECIMAL_TEXT", StringComparison.OrdinalIgnoreCase))
                throw new SqlExecutorException($"SQLite Decimal column {entity.TableNameValue}.{property.ColumnNameString} " +
                    $"has type {actual ?? "<missing>"}; migrate it to TEAQL_DECIMAL_TEXT before continuing. " +
                    "Previously rounded NUMERIC/REAL values cannot be recovered automatically.");
        }
    }

    private static string Soundex(string? value)
    {
        if (value is null) return "?000";
        var letters = value.ToUpperInvariant().Where(c => c is >= 'A' and <= 'Z').ToArray();
        if (letters.Length == 0) return "?000";
        static char Code(char c) => c switch
        {
            'B' or 'F' or 'P' or 'V' => '1',
            'C' or 'G' or 'J' or 'K' or 'Q' or 'S' or 'X' or 'Z' => '2',
            'D' or 'T' => '3', 'L' => '4', 'M' or 'N' => '5', 'R' => '6', _ => '0'
        };
        var result = new List<char> { letters[0] };
        var previous = Code(letters[0]);
        foreach (var letter in letters.Skip(1))
        {
            var current = Code(letter);
            if (current != '0' && current != previous) result.Add(current);
            if (result.Count == 4) break;
            previous = current;
        }
        while (result.Count < 4) result.Add('0');
        return new string(result.ToArray());
    }

    public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
    {
        var records = new List<Record>();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = query.SqlWithComment();
        
        for (int i = 0; i < query.Params.Count; i++)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = $"@p{i}";
            BindValue(p, query.Params[i]);
            cmd.Parameters.Add(p);
        }

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var record = new Record();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = ReadValue(reader, i);
                record[name] = value;
            }
            records.Add(record);
        }

        return records;
    }

    public async IAsyncEnumerable<Record> StreamSqlAsync(
        CompiledQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = query.SqlWithComment();
        for (var i = 0; i < query.Params.Count; i++)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = $"@p{i}";
            BindValue(p, query.Params[i]);
            cmd.Parameters.Add(p);
        }
        using var reader = await cmd.ExecuteReaderAsync(
            System.Data.CommandBehavior.SequentialAccess,
            cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var record = new Record();
            for (var i = 0; i < reader.FieldCount; i++)
                record[reader.GetName(i)] = ReadValue(reader, i);
            yield return record;
        }
    }

    public async Task<ulong> ExecuteSqlAsync(CompiledQuery query)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = query.SqlWithComment();
        
        for (int i = 0; i < query.Params.Count; i++)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = $"@p{i}";
            BindValue(p, query.Params[i]);
            cmd.Parameters.Add(p);
        }

        var rows = await cmd.ExecuteNonQueryAsync();
        return (ulong)(rows > 0 ? rows : 0);
    }

    public async Task<ISqlTransaction> BeginSqlAsync()
    {
        var transaction = await _connection.BeginTransactionAsync();
        return new Transaction(_connection, (SqliteTransaction)transaction, this);
    }

    private sealed class Transaction : ISqlTransaction
    {
        private readonly SqliteConnection _connection;
        private readonly SqliteTransaction _transaction;
        private readonly SqliteTransport _owner;

        public Transaction(SqliteConnection connection, SqliteTransaction transaction, SqliteTransport owner)
        {
            _connection = connection;
            _transaction = transaction;
            _owner = owner;
        }

        public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
        {
            var records = new List<Record>();
            using var command = CreateCommand(query);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var record = new Record();
                for (var index = 0; index < reader.FieldCount; index++)
                    record[reader.GetName(index)] = _owner.ReadValue(reader, index);
                records.Add(record);
            }
            return records;
        }

        public async Task<ulong> ExecuteSqlAsync(CompiledQuery query)
        {
            using var command = CreateCommand(query);
            var rows = await command.ExecuteNonQueryAsync();
            return (ulong)(rows > 0 ? rows : 0);
        }

        private SqliteCommand CreateCommand(CompiledQuery query)
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            command.CommandText = query.SqlWithComment();
            for (var index = 0; index < query.Params.Count; index++)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@p{index}";
                _owner.BindValue(parameter, query.Params[index]);
                command.Parameters.Add(parameter);
            }
            return command;
        }

        public Task CommitSqlAsync() => _transaction.CommitAsync();
        public Task RollbackSqlAsync() => _transaction.RollbackAsync();
        public void Dispose() => _transaction.Dispose();
    }

    private void BindValue(SqliteParameter p, Value value)
    {
        switch (value)
        {
            case Value.NullValue or Value.TypedNullValue:
                p.Value = System.DBNull.Value;
                break;
            case Value.BoolValue b:
                p.Value = b.Value ? 1 : 0;
                break;
            case Value.I64Value i:
                p.Value = i.Value;
                break;
            case Value.U64Value u:
                p.Value = (long)u.Value; // Sqlite doesn't support ulong directly
                break;
            case Value.F64Value f:
                p.Value = f.Value;
                break;
            case Value.DecimalValue d:
                p.Value = d.Value.ToString(CultureInfo.InvariantCulture);
                break;
            case Value.TimeValue t:
                if (t.Value < TimeSpan.Zero || t.Value >= TimeSpan.FromDays(1))
                    throw new ArgumentOutOfRangeException(nameof(value), "Time must be a wall-clock value in [00:00, 24:00).");
                p.Value = t.Value.ToString("c", CultureInfo.InvariantCulture);
                break;
            case Value.TextValue t:
                p.Value = t.Value;
                break;
            case Value.JsonValue j:
                p.Value = j.Value?.ToJsonString() ?? "null";
                break;
            case Value.DateValue d:
                p.Value = d.Value.ToString("yyyy-MM-dd");
                break;
            case Value.TimestampValue t:
                p.Value = t.Milliseconds;
                break;
            default:
                throw new System.NotSupportedException($"Unsupported value type: {value.GetType()}");
        }
    }

    private Value ReadValue(DbDataReader reader, int i)
    {
        if (reader.IsDBNull(i))
        {
            return new Value.NullValue();
        }

        var typeName = reader.GetDataTypeName(i).ToUpperInvariant();
        var fieldType = reader.GetFieldType(i);

        if (fieldType == typeof(long))
        {
            long v = reader.GetInt64(i);
            if (typeName == "BOOLEAN" || typeName == "BOOL") return new Value.BoolValue(v != 0);
            if (typeName == "TIMESTAMP" || typeName == "DATETIME") return new Value.TimestampValue(v);
            if (typeName == "NUMERIC" || typeName == "DECIMAL")
                throw new InvalidOperationException("Legacy SQLite NUMERIC Decimal data may have lost precision; migrate the column to TEAQL_DECIMAL_TEXT.");
            return new Value.I64Value(v);
        }
        else if (fieldType == typeof(double))
        {
            if (typeName == "NUMERIC" || typeName == "DECIMAL")
                throw new InvalidOperationException("Legacy SQLite NUMERIC Decimal data may have lost precision; migrate the column to TEAQL_DECIMAL_TEXT.");
            return new Value.F64Value(reader.GetDouble(i));
        }
        else if (fieldType == typeof(string))
        {
            var str = reader.GetString(i);
            if (str.StartsWith("__teaql_decimal__:", StringComparison.Ordinal))
            {
                return new Value.DecimalValue(ParseExactDecimal(str.Substring("__teaql_decimal__:".Length)));
            }
            if (typeName == "TEAQL_DECIMAL_TEXT")
            {
                const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
                if (decimal.TryParse(str, styles, CultureInfo.InvariantCulture, out var dec))
                    return new Value.DecimalValue(dec);
                throw new FormatException($"Invalid exact SQLite Decimal in column {reader.GetName(i)}.");
            }
            if (typeName == "TIME" || typeName == "TEAQL_TIME_TEXT")
            {
                if (TimeSpan.TryParseExact(str, "c", CultureInfo.InvariantCulture, out var wallClock)
                    && wallClock >= TimeSpan.Zero && wallClock < TimeSpan.FromDays(1))
                    return new Value.TimeValue(wallClock);
                throw new FormatException($"Invalid SQLite wall-clock Time in column {reader.GetName(i)}.");
            }
            if (typeName == "NUMERIC" || typeName == "DECIMAL")
            {
                throw new InvalidOperationException("Legacy SQLite NUMERIC Decimal data may have lost precision; migrate the column to TEAQL_DECIMAL_TEXT.");
            }
            else if (typeName == "JSON")
            {
                return new Value.JsonValue(System.Text.Json.Nodes.JsonNode.Parse(str));
            }
            else if (typeName == "DATE")
            {
                if (System.DateTime.TryParse(str, out var dt))
                    return new Value.DateValue(dt);
            }
            else if (typeName == "TIMESTAMP" || typeName == "DATETIME")
            {
                if (long.TryParse(str, out var ms))
                    return new Value.TimestampValue(ms);
                if (System.DateTime.TryParse(str, out var dt))
                {
                    var ms2 = new System.DateTimeOffset(dt).ToUnixTimeMilliseconds();
                    return new Value.TimestampValue(ms2);
                }
            }
            
            // Try to infer
            if (System.DateTime.TryParseExact(str, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var inferredDate))
            {
                return new Value.DateValue(inferredDate);
            }
            
            return new Value.TextValue(str);
        }
        
        var obj = reader.GetValue(i);
        if (obj is int intVal) return new Value.I64Value(intVal);
        if (obj is long longVal) return new Value.I64Value(longVal);
        if (obj is double doubleVal) return new Value.F64Value(doubleVal);
        if (obj is string strVal) return new Value.TextValue(strVal);

        return new Value.TextValue(obj.ToString() ?? "");
    }
}
