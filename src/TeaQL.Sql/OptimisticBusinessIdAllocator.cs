using TeaQL.Core;

namespace TeaQL.Sql;

/// <summary>Portable SQL CAS allocator for externally visible Business IDs.</summary>
public sealed class OptimisticBusinessIdAllocator(
    ISqlTransport transport, SqlDialect dialect) : IBusinessIdAllocator
{
    public const string SqliteSchemaSql =
        "CREATE TABLE IF NOT EXISTS teaql_business_id_space (" +
        "scope_key VARCHAR(512) NOT NULL PRIMARY KEY, " +
        "current_value BIGINT NOT NULL, version BIGINT NOT NULL, " +
        "updated_at BIGINT NOT NULL)";

    /// <summary>Explicit schema operation; construction never executes DDL.</summary>
    public async Task EnsureSchemaAsync() => _ = await transport.ExecuteSqlAsync(
        new CompiledQuery(SqliteSchemaSql, new List<Value>())).ConfigureAwait(false);

    public async Task<BusinessIdAllocation> AllocateAsync(BusinessIdPlan plan)
    {
        var scopeKey = plan.Scope.CanonicalKey;
        var updatedAt = checked((ulong)new DateTimeOffset(
            plan.BusinessDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero)
            .ToUnixTimeMilliseconds());
        for (var attempt = 1; attempt <= OptimisticIdSpace.MaxAttempts; attempt++)
        {
            List<Record> rows;
            try
            {
                rows = await transport.FetchAllSqlAsync(new CompiledQuery(
                    $"SELECT current_value, version FROM teaql_business_id_space " +
                    $"WHERE scope_key = {dialect.Placeholder(1)}",
                    new List<Value> { new Value.TextValue(scopeKey) }));
            }
            catch (Exception error)
            {
                throw new SqlExecutorException(
                    "Business ID allocation requires explicit EnsureSchemaAsync", error);
            }

            if (rows.Count == 0)
            {
                try
                {
                    var inserted = await transport.ExecuteSqlAsync(new CompiledQuery(
                        "INSERT INTO teaql_business_id_space" +
                        "(scope_key, current_value, version, updated_at) VALUES " +
                        $"({dialect.Placeholder(1)}, {dialect.Placeholder(2)}, 1, {dialect.Placeholder(3)})",
                        new List<Value> {
                            new Value.TextValue(scopeKey),
                            new Value.U64Value(plan.InitialSequence),
                            new Value.U64Value(updatedAt)
                        }));
                    if (inserted == 1)
                        return new BusinessIdAllocation(plan.Scope, plan.InitialSequence);
                    throw new SqlExecutorException(
                        $"Business ID insert for {scopeKey} changed {inserted} rows");
                }
                catch (Exception insertError)
                {
                    var winner = await transport.FetchAllSqlAsync(new CompiledQuery(
                        $"SELECT current_value, version FROM teaql_business_id_space " +
                        $"WHERE scope_key = {dialect.Placeholder(1)}",
                        new List<Value> { new Value.TextValue(scopeKey) }));
                    if (winner.Count == 0)
                        throw new SqlExecutorException(
                            $"Insert Business ID space for {scopeKey} failed", insertError);
                }
            }
            else
            {
                var current = AsUnsigned(rows[0]["current_value"], "current_value", scopeKey);
                var version = AsUnsigned(rows[0]["version"], "version", scopeKey);
                if (version == 0)
                    throw new SqlExecutorException($"Invalid Business ID version for {scopeKey}");
                if (current >= plan.MaximumSequence)
                    throw new BusinessIdException(BusinessIdErrorCode.BusinessIdRangeExhausted,
                        $"Business ID range exhausted for {scopeKey}");
                var next = current + 1;
                var changed = await transport.ExecuteSqlAsync(new CompiledQuery(
                    $"UPDATE teaql_business_id_space SET current_value = {dialect.Placeholder(1)}, " +
                    $"version = version + 1, updated_at = {dialect.Placeholder(2)} " +
                    $"WHERE scope_key = {dialect.Placeholder(3)} AND version = {dialect.Placeholder(4)} " +
                    $"AND current_value = {dialect.Placeholder(5)}",
                    new List<Value> {
                        new Value.U64Value(next), new Value.U64Value(updatedAt),
                        new Value.TextValue(scopeKey), new Value.U64Value(version),
                        new Value.U64Value(current)
                    }));
                if (changed == 1) return new BusinessIdAllocation(plan.Scope, next);
                if (changed != 0)
                    throw new SqlExecutorException(
                        $"Business ID update for {scopeKey} changed {changed} rows");
            }
            await Task.Delay(1).ConfigureAwait(false);
        }
        throw new BusinessIdException(
            BusinessIdErrorCode.BusinessIdAllocationRetryExhausted,
            $"Business ID allocation did not converge for {scopeKey}");
    }

    private static ulong AsUnsigned(Value value, string field, string scopeKey) => value switch
    {
        Value.U64Value number => number.Value,
        Value.I64Value number when number.Value >= 0 => (ulong)number.Value,
        _ => throw new SqlExecutorException(
            $"Business ID {field} for {scopeKey} is not an unsigned integer")
    };
}
