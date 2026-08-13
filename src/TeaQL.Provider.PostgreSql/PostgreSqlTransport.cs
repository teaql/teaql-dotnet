using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using Npgsql;
using NpgsqlTypes;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.PostgreSql;

public class PostgreSqlTransport : IStreamingSqlTransport
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgreSqlTransport(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = query.Sql;
        
        SetParameters(cmd, query.Params);

        var records = new List<Record>();
        await using var reader = await cmd.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            var record = new Record();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = reader.IsDBNull(i) ? new Value.NullValue() : MapToValue(reader.GetValue(i));
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
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = query.Sql;
        SetParameters(cmd, query.Params);
        await using var reader = await cmd.ExecuteReaderAsync(
            System.Data.CommandBehavior.SequentialAccess,
            cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var record = new Record();
            for (var i = 0; i < reader.FieldCount; i++)
                record[reader.GetName(i)] = reader.IsDBNull(i)
                    ? new Value.NullValue()
                    : MapToValue(reader.GetValue(i));
            yield return record;
        }
    }

    public async Task<ulong> ExecuteSqlAsync(CompiledQuery query)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = query.Sql;
        
        SetParameters(cmd, query.Params);
        
        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return (ulong)Math.Max(0, rowsAffected);
    }

    private void SetParameters(NpgsqlCommand cmd, List<Value> parameters)
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            var val = parameters[i];
            var p = new NpgsqlParameter();
            var valueObj = MapFromValue(val, out var dbType);
            
            if (dbType.HasValue)
            {
                p.NpgsqlDbType = dbType.Value;
            }
            p.Value = valueObj ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }

    private object? MapFromValue(Value val, out NpgsqlDbType? type)
    {
        type = null;
        return val switch
        {
            Value.NullValue => null,
            Value.BoolValue b => b.Value,
            Value.I64Value i => i.Value,
            Value.U64Value u => (long)u.Value,
            Value.F64Value f => f.Value,
            Value.DecimalValue d => d.Value,
            Value.TextValue t => t.Value,
            Value.JsonValue j => GetJsonValue(j, out type),
            Value.DateValue d => d.Value,
            Value.TimestampValue t => DateTimeOffset.FromUnixTimeMilliseconds(t.Milliseconds),
            Value.ObjectValue o => GetObjectValue(o, out type),
            Value.ListValue l => GetListValue(l, out type),
            Value.TypedNullValue => null,
            _ => throw new NotSupportedException($"Unsupported value type: {val?.GetType()}")
        };
    }

    private object? GetJsonValue(Value.JsonValue j, out NpgsqlDbType? type)
    {
        type = NpgsqlDbType.Jsonb;
        return j.Value?.ToJsonString() ?? "null";
    }

    private object? GetObjectValue(Value.ObjectValue o, out NpgsqlDbType? type)
    {
        type = NpgsqlDbType.Jsonb;
        return JsonSerializer.Serialize(o.Value);
    }
    
    private object? GetListValue(Value.ListValue l, out NpgsqlDbType? type)
    {
        type = null;
        if (l.Values.Count == 0)
        {
            return Array.Empty<object>();
        }
        var mapped = l.Values.Select(v => MapFromValue(v, out _)).ToArray();
        return mapped;
    }

    private Value MapToValue(object dbValue)
    {
        return dbValue switch
        {
            bool b => new Value.BoolValue(b),
            short s => new Value.I64Value(s),
            int i => new Value.I64Value(i),
            long l => new Value.I64Value(l),
            float f => new Value.F64Value(f),
            double d => new Value.F64Value(d),
            decimal dec => new Value.DecimalValue(dec),
            string s => new Value.TextValue(s),
            DateTime dt => new Value.TimestampValue(new DateTimeOffset(dt).ToUnixTimeMilliseconds()),
            DateTimeOffset dto => new Value.TimestampValue(dto.ToUnixTimeMilliseconds()),
            JsonDocument jdoc => new Value.JsonValue(JsonNode.Parse(jdoc.RootElement.GetRawText())),
            Array arr => new Value.ListValue(arr.Cast<object>().Select(MapToValue).ToList()),
            _ => new Value.TextValue(dbValue.ToString() ?? "")
        };
    }
}
