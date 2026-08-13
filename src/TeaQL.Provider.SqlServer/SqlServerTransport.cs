using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.SqlServer;

public class SqlServerTransport : ISqlTransport
{
    private readonly string _connectionString;

    public SqlServerTransport(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
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
                var value = await reader.IsDBNullAsync(i) ? new Value.NullValue() : MapToValue(reader.GetValue(i));
                record[name] = value;
            }
            records.Add(record);
        }
        
        return records;
    }

    public async Task<ulong> ExecuteSqlAsync(CompiledQuery query)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = query.Sql;
        
        SetParameters(cmd, query.Params);
        
        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return (ulong)Math.Max(0, rowsAffected);
    }

    private void SetParameters(SqlCommand cmd, List<Value> parameters)
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            var val = parameters[i];
            var p = new SqlParameter();
            p.ParameterName = $"@p{i + 1}";
            var valueObj = MapFromValue(val, out var dbType);
            
            if (dbType.HasValue)
            {
                p.SqlDbType = dbType.Value;
            }
            p.Value = valueObj ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }

    private object? MapFromValue(Value val, out System.Data.SqlDbType? type)
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

    private object? GetJsonValue(Value.JsonValue j, out System.Data.SqlDbType? type)
    {
        type = System.Data.SqlDbType.NVarChar;
        return j.Value?.ToJsonString() ?? "null";
    }

    private object? GetObjectValue(Value.ObjectValue o, out System.Data.SqlDbType? type)
    {
        type = System.Data.SqlDbType.NVarChar;
        return JsonSerializer.Serialize(o.Value);
    }
    
    private object? GetListValue(Value.ListValue l, out System.Data.SqlDbType? type)
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
