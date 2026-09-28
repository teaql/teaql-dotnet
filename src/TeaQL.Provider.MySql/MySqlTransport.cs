using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using MySqlConnector;
using TeaQL.Core;
using TeaQL.Sql;

namespace TeaQL.Provider.MySql;

public class MySqlTransport : IStreamingSqlTransport, ISchemaIndexInstaller, IDisposable
{
    private readonly MySqlConnection _connection;
    private readonly bool _ownsConnection;

    public MySqlTransport(string connectionString)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString)
        {
            DateTimeKind = MySqlDateTimeKind.Utc
        };
        _connection = new MySqlConnection(builder.ConnectionString);
        _ownsConnection = true;
    }

    public MySqlTransport(MySqlConnection connection, bool ownsConnection = false)
    {
        _connection = connection;
        _ownsConnection = ownsConnection;
    }

    private async Task EnsureConnectionOpenAsync()
    {
        if (_connection.State == ConnectionState.Closed)
        {
            await _connection.OpenAsync();
        }
    }

    public async Task<List<Record>> FetchAllSqlAsync(CompiledQuery query)
    {
        await EnsureConnectionOpenAsync();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = query.SqlWithComment();
        
        foreach (var param in query.Params)
        {
            var p = cmd.CreateParameter();
            p.Value = ConvertValue(param);
            cmd.Parameters.Add(p);
        }

        using var reader = await cmd.ExecuteReaderAsync();
        var results = new List<Record>();
        
        while (await reader.ReadAsync())
        {
            var record = new Record();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var val = reader.GetValue(i);
                record[name] = ConvertFromDbValue(val);
            }
            results.Add(record);
        }

        return results;
    }

    public async IAsyncEnumerable<Record> StreamSqlAsync(
        CompiledQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await EnsureConnectionOpenAsync();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = query.SqlWithComment();
        foreach (var param in query.Params)
        {
            var p = cmd.CreateParameter();
            p.Value = ConvertValue(param);
            cmd.Parameters.Add(p);
        }
        using var reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess, cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var record = new Record();
            for (var i = 0; i < reader.FieldCount; i++)
                record[reader.GetName(i)] = ConvertFromDbValue(reader.GetValue(i));
            yield return record;
        }
    }

    public async Task<ulong> ExecuteSqlAsync(CompiledQuery query)
    {
        await EnsureConnectionOpenAsync();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = query.SqlWithComment();
        
        foreach (var param in query.Params)
        {
            var p = cmd.CreateParameter();
            p.Value = ConvertValue(param);
            cmd.Parameters.Add(p);
        }

        var affectedRows = await cmd.ExecuteNonQueryAsync();
        return (ulong)Math.Max(0, affectedRows);
    }

    public async Task EnsureSchemaIndexesAsync(SqlDialect dialect, EntityDescriptor entity)
    {
        if (dialect.Kind != DatabaseKind.MySql)
            throw new ArgumentException("MySQL index installer requires a MySQL dialect", nameof(dialect));

        var specs = dialect.SchemaIndexSpecs(entity);
        // MySQL has no CREATE INDEX IF NOT EXISTS. Check every existing shape
        // before making any non-transactional DDL change.
        var missing = new List<SchemaIndexSpec>();
        foreach (var spec in specs)
        {
            var existing = await IndexShapeAsync(spec);
            if (existing == null) missing.Add(spec);
            else if (!existing.Value)
                throw new InvalidOperationException($"Existing MySQL index {spec.Name} on {spec.Table} has an incompatible shape");
        }

        foreach (var spec in missing)
        {
            var columns = string.Join(", ", spec.Columns.Select(column =>
                dialect.QuoteIdent(column.Name) + (column.Descending ? " DESC" : "")));
            var sql = $"CREATE {(spec.Unique ? "UNIQUE " : "")}INDEX {dialect.QuoteIdent(spec.Name)} " +
                $"ON {dialect.QuoteIdent(spec.Table)} ({columns})";
            try
            {
                await ExecuteSqlAsync(new CompiledQuery(sql, new List<Value>()));
            }
            catch (MySqlException ex) when (ex.Number == 1061)
            {
                if (await IndexShapeAsync(spec) != true)
                    throw new InvalidOperationException($"Concurrent MySQL index {spec.Name} has an incompatible shape", ex);
            }
        }
    }

    // null = missing; false = same name but incompatible columns/uniqueness/order.
    private async Task<bool?> IndexShapeAsync(SchemaIndexSpec spec)
    {
        await EnsureConnectionOpenAsync();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT NON_UNIQUE, COLUMN_NAME, COLLATION, SUB_PART, INDEX_TYPE " +
            "FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() " +
            "AND TABLE_NAME = @table AND INDEX_NAME = @index ORDER BY SEQ_IN_INDEX";
        cmd.Parameters.AddWithValue("@table", spec.Table);
        cmd.Parameters.AddWithValue("@index", spec.Name);
        using var reader = await cmd.ExecuteReaderAsync();
        var position = 0;
        var matches = true;
        while (await reader.ReadAsync())
        {
            if (position >= spec.Columns.Count) { matches = false; position++; continue; }
            var column = spec.Columns[position++];
            matches &= Convert.ToInt32(reader.GetValue(0)) == (spec.Unique ? 0 : 1)
                && !reader.IsDBNull(1) && string.Equals(reader.GetString(1), column.Name, StringComparison.OrdinalIgnoreCase)
                && !reader.IsDBNull(2) && reader.GetString(2) == (column.Descending ? "D" : "A")
                && reader.IsDBNull(3)
                && string.Equals(reader.GetString(4), "BTREE", StringComparison.OrdinalIgnoreCase);
        }
        return position == 0 ? null : matches && position == spec.Columns.Count;
    }

    private object ConvertValue(Value value)
    {
        return value switch
        {
            Value.NullValue => DBNull.Value,
            Value.TypedNullValue => DBNull.Value,
            Value.BoolValue b => b.Value,
            Value.I64Value i => i.Value,
            Value.U64Value u => u.Value,
            Value.F64Value f => f.Value,
            Value.DecimalValue d => d.Value,
            Value.TextValue t => t.Value,
            Value.JsonValue j => j.Value?.ToJsonString() ?? (object)DBNull.Value,
            Value.DateValue d => d.Value,
            Value.TimestampValue t => DateTimeOffset.FromUnixTimeMilliseconds(t.Milliseconds).UtcDateTime,
            _ => throw new NotSupportedException($"Unsupported value type: {value.GetType()}")
        };
    }

    private Value ConvertFromDbValue(object dbValue)
    {
        if (dbValue == null || dbValue == DBNull.Value)
        {
            return new Value.NullValue();
        }

        return dbValue switch
        {
            bool b => new Value.BoolValue(b),
            sbyte b => new Value.I64Value(b),
            byte b => new Value.U64Value(b),
            short s => new Value.I64Value(s),
            ushort us => new Value.U64Value(us),
            int i => new Value.I64Value(i),
            uint ui => new Value.U64Value(ui),
            long l => new Value.I64Value(l),
            ulong ul => new Value.U64Value(ul),
            float f => new Value.F64Value(f),
            double d => new Value.F64Value(d),
            decimal dec => new Value.DecimalValue(dec),
            string s => new Value.TextValue(s),
            DateTime dt => new Value.TimestampValue(
                new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc)).ToUnixTimeMilliseconds()),
            DateTimeOffset dto => new Value.TimestampValue(dto.ToUnixTimeMilliseconds()),
            byte[] bytes => new Value.TextValue(Convert.ToBase64String(bytes)),
            _ => new Value.TextValue(dbValue.ToString()!)
        };
    }

    public void Dispose()
    {
        if (_ownsConnection)
        {
            _connection.Dispose();
        }
    }
}
