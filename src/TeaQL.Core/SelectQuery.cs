using System.Collections.Generic;

namespace TeaQL.Core;

public enum SortDirection { Asc, Desc }

public record NamedExpr(string Alias, Expr Expr);

public record OrderBy(string Field, Expr? Expr, SortDirection Direction);

public enum AggregateFunction { Count, Sum, Avg, Min, Max, Stddev, StddevPop, VarSamp, VarPop, BitAnd, BitOr, BitXor }

public record Aggregate(AggregateFunction Function, string Field, string Alias);

public record Slice(ulong? Limit, ulong Offset);

public record RelationLoad(string Name, SelectQuery? Query = null);

public record RelationAggregate(string RelationName, string Alias, SelectQuery Query, bool SingleResult);

public record RawSqlProjection(string PropertyName, string RawSqlSegment);

public record ObjectGroupBy(string PropertyName, string StorageField, SelectQuery Query);

public record AggregationCacheOptions(bool Enabled, ulong CacheExpiredMillis, bool Propagate, ulong PropagateCacheExpiredMillis);

public record StreamConfig(int ChunkSize = 1000);

public record TraceNode(string EntityType, Value? EntityId, string Comment);

public record SelectQuery
{
    public string Entity { get; init; } = "";
    public List<string> Projection { get; init; } = new();
    public List<NamedExpr> ExprProjection { get; init; } = new();
    public string? SearchWithText { get; init; }
    public Expr? Filter { get; init; }
    public Expr? Having { get; init; }
    public List<OrderBy> OrderBy { get; init; } = new();
    public Slice? Slice { get; init; }
    public List<Aggregate> Aggregates { get; init; } = new();
    public List<string> GroupBy { get; init; } = new();
    public List<RelationLoad> Relations { get; init; } = new();
    public AggregationCacheOptions? AggregationCache { get; init; }
    public string? Comment { get; init; }
    public List<TraceNode> TraceChain { get; init; } = new();
    public string? RawSql { get; init; }
    public List<string> RawSqlSearchCriteria { get; init; } = new();
    public List<RawSqlProjection> DynamicProperties { get; init; } = new();
    public List<RawSqlProjection> RawProjections { get; init; } = new();
    public List<ObjectGroupBy> ObjectGroupBys { get; init; } = new();
    public List<SelectQuery> ChildEnhancements { get; init; } = new();
    public StreamConfig? StreamConfig { get; init; }
}
