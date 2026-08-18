using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.Sqlite;

public class SqliteTransport : IStreamingSqlTransport, IAutomaticMutationTransactionTransport
{
    private readonly SqliteConnection _connection;

    public SqliteTransport(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
    {
        var records = new List<Record>();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = query.Sql;
        
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
        cmd.CommandText = query.Sql;
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
        cmd.CommandText = query.Sql;
        
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
            command.CommandText = query.Sql;
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
                p.Value = $"__teaql_decimal__:{d.Value}";
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
            return new Value.I64Value(v);
        }
        else if (fieldType == typeof(double))
        {
            return new Value.F64Value(reader.GetDouble(i));
        }
        else if (fieldType == typeof(string))
        {
            var str = reader.GetString(i);
            if (str.StartsWith("__teaql_decimal__:"))
            {
                if (decimal.TryParse(str.Substring("__teaql_decimal__:".Length), out var dec))
                    return new Value.DecimalValue(dec);
            }
            if (typeName == "NUMERIC" || typeName == "DECIMAL")
            {
                if (decimal.TryParse(str, out var dec))
                    return new Value.DecimalValue(dec);
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
