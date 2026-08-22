using TeaQL.Core;

namespace TeaQL.Sql;

internal static class OptimisticIdSpace
{
    internal const int MaxAttempts = 100;

    internal static ulong Floor(Value value, string entity) => value switch
    {
        Value.U64Value id => id.Value,
        Value.I64Value id when id.Value >= 0 => (ulong)id.Value,
        _ => throw new SqlExecutorException($"Explicit ID for {entity} must be a non-negative integer")
    };

    internal static async Task<ulong> NextIdAsync(
        ISqlTransport transport, SqlDialect dialect, string entity)
    {
        if (string.IsNullOrWhiteSpace(entity))
            throw new SqlExecutorException("ID space type name must not be empty");
        await transport.ExecuteSqlAsync(new CompiledQuery(
            CreateTableSql(dialect),
            new List<Value>()));
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var rows = await transport.FetchAllSqlAsync(new CompiledQuery(
                $"SELECT current_level FROM teaql_id_space WHERE type_name = {dialect.Placeholder(1)}",
                new List<Value> { new Value.TextValue(entity) }));
            if (rows.Count == 0)
            {
                try
                {
                    var inserted = await transport.ExecuteSqlAsync(new CompiledQuery(
                        $"INSERT INTO teaql_id_space(type_name, current_level) VALUES ({dialect.Placeholder(1)}, 1)",
                        new List<Value> { new Value.TextValue(entity) }));
                    if (inserted == 1) return 1;
                    throw new SqlExecutorException(
                        $"ID space insert for {entity} changed {inserted} rows");
                }
                catch (Exception insertError)
                {
                    var winner = await transport.FetchAllSqlAsync(new CompiledQuery(
                        $"SELECT current_level FROM teaql_id_space WHERE type_name = {dialect.Placeholder(1)}",
                        new List<Value> { new Value.TextValue(entity) }));
                    if (winner.Count == 0)
                        throw new SqlExecutorException($"Insert ID space for {entity} failed", insertError);
                    continue;
                }
            }
            var current = rows[0]["current_level"] switch
            {
                Value.I64Value value when value.Value >= 0 => (ulong)value.Value,
                Value.U64Value value => value.Value,
                _ => throw new SqlExecutorException(
                    $"ID space current_level for {entity} is not an unsigned integer")
            };
            if (current >= long.MaxValue)
                throw new SqlExecutorException($"ID space overflow for {entity}");
            var next = current + 1;
            var changed = await transport.ExecuteSqlAsync(new CompiledQuery(
                $"UPDATE teaql_id_space SET current_level = {dialect.Placeholder(1)} " +
                $"WHERE type_name = {dialect.Placeholder(2)} AND current_level = {dialect.Placeholder(3)}",
                new List<Value> {
                    new Value.U64Value(next), new Value.TextValue(entity),
                    new Value.U64Value(current)
                }));
            if (changed == 1) return next;
            if (changed != 0)
                throw new SqlExecutorException(
                    $"ID space update for {entity} changed {changed} rows on attempt {attempt}");
        }
        throw new SqlExecutorException(
            $"Unable to allocate ID for {entity} after {MaxAttempts} optimistic-lock attempts");
    }

    internal static async Task EnsureFloorAsync(
        ISqlTransport transport, SqlDialect dialect, string entity, ulong floor)
    {
        if (floor > long.MaxValue)
            throw new SqlExecutorException($"ID space floor {floor} for {entity} exceeds BIGINT");
        await transport.ExecuteSqlAsync(new CompiledQuery(
            CreateTableSql(dialect),
            new List<Value>()));
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var rows = await transport.FetchAllSqlAsync(new CompiledQuery(
                $"SELECT current_level FROM teaql_id_space WHERE type_name = {dialect.Placeholder(1)}",
                new List<Value> { new Value.TextValue(entity) }));
            if (rows.Count == 0)
            {
                try
                {
                    var inserted = await transport.ExecuteSqlAsync(new CompiledQuery(
                        $"INSERT INTO teaql_id_space(type_name, current_level) VALUES ({dialect.Placeholder(1)}, {dialect.Placeholder(2)})",
                        new List<Value> { new Value.TextValue(entity), new Value.U64Value(floor) }));
                    if (inserted == 1) return;
                }
                catch
                {
                    var winner = await transport.FetchAllSqlAsync(new CompiledQuery(
                        $"SELECT current_level FROM teaql_id_space WHERE type_name = {dialect.Placeholder(1)}",
                        new List<Value> { new Value.TextValue(entity) }));
                    if (winner.Count == 0) throw;
                }
                continue;
            }
            var current = rows[0]["current_level"] switch
            {
                Value.I64Value value when value.Value >= 0 => (ulong)value.Value,
                Value.U64Value value => value.Value,
                _ => throw new SqlExecutorException($"Invalid ID space level for {entity}")
            };
            if (current >= floor) return;
            var changed = await transport.ExecuteSqlAsync(new CompiledQuery(
                $"UPDATE teaql_id_space SET current_level = {dialect.Placeholder(1)} " +
                $"WHERE type_name = {dialect.Placeholder(2)} AND current_level = {dialect.Placeholder(3)}",
                new List<Value> {
                    new Value.U64Value(floor), new Value.TextValue(entity),
                    new Value.U64Value(current)
                }));
            if (changed == 1) return;
            if (changed != 0)
                throw new SqlExecutorException(
                    $"ID space floor update for {entity} changed {changed} rows on attempt {attempt}");
        }
        throw new SqlExecutorException(
            $"Unable to synchronize ID space floor for {entity} after {MaxAttempts} optimistic-lock attempts");
    }

    private static string CreateTableSql(SqlDialect dialect) => dialect.Kind switch
    {
        DatabaseKind.SqlServer =>
            "IF OBJECT_ID(N'teaql_id_space', N'U') IS NULL " +
            "CREATE TABLE teaql_id_space (type_name NVARCHAR(255) NOT NULL PRIMARY KEY, current_level BIGINT NOT NULL)",
        _ => "CREATE TABLE IF NOT EXISTS teaql_id_space " +
             "(type_name VARCHAR(255) NOT NULL PRIMARY KEY, current_level BIGINT NOT NULL)"
    };
}
