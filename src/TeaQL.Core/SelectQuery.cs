using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace TeaQL.Core;

public enum SortDirection { Asc, Desc }

public record NamedExpr(string Alias, Expr Expr);

public record OrderBy(string Field, Expr? ExprValue, SortDirection Direction)
{
    public static OrderBy New(string field, SortDirection direction) => new(field, null, direction);
    public static OrderBy Expr(Core.Expr expr, SortDirection direction) => new("", expr, direction);
    
    public static OrderBy Asc(string field) => New(field, SortDirection.Asc);
    public static OrderBy Desc(string field) => New(field, SortDirection.Desc);
    
    public static OrderBy AscExpr(Core.Expr expr) => Expr(expr, SortDirection.Asc);
    public static OrderBy DescExpr(Core.Expr expr) => Expr(expr, SortDirection.Desc);
    
    public static OrderBy AscGbk(string field) => AscExpr(Core.Expr.Gbk(Core.Expr.Column(field)));
    public static OrderBy DescGbk(string field) => DescExpr(Core.Expr.Gbk(Core.Expr.Column(field)));
}

public enum AggregateFunction { Count, Sum, Avg, Min, Max, Stddev, StddevPop, VarSamp, VarPop, BitAnd, BitOr, BitXor }

public record Aggregate(AggregateFunction Function, string Field, string Alias)
{
    public static Aggregate Count(string alias) => new(AggregateFunction.Count, "*", alias);
    public static Aggregate CountField(string field, string alias) => new(AggregateFunction.Count, field, alias);
    public static Aggregate Sum(string field, string alias) => new(AggregateFunction.Sum, field, alias);
    public static Aggregate Avg(string field, string alias) => new(AggregateFunction.Avg, field, alias);
    public static Aggregate Min(string field, string alias) => new(AggregateFunction.Min, field, alias);
    public static Aggregate Max(string field, string alias) => new(AggregateFunction.Max, field, alias);
    public static Aggregate Stddev(string field, string alias) => new(AggregateFunction.Stddev, field, alias);
    public static Aggregate StddevPop(string field, string alias) => new(AggregateFunction.StddevPop, field, alias);
    public static Aggregate VarSamp(string field, string alias) => new(AggregateFunction.VarSamp, field, alias);
    public static Aggregate VarPop(string field, string alias) => new(AggregateFunction.VarPop, field, alias);
    public static Aggregate BitAnd(string field, string alias) => new(AggregateFunction.BitAnd, field, alias);
    public static Aggregate BitOr(string field, string alias) => new(AggregateFunction.BitOr, field, alias);
    public static Aggregate BitXor(string field, string alias) => new(AggregateFunction.BitXor, field, alias);
}

public record Slice(ulong? Limit, ulong Offset);

public record RelationLoad(string Name, SelectQuery? Query = null)
{
    public static RelationLoad WithQuery(string name, SelectQuery query) => new(name, query);
}

public record RelationAggregate(string RelationName, string Alias, SelectQuery Query, bool SingleResult);

public record RawSqlProjection(string PropertyName, string RawSqlSegment);

public record ObjectGroupBy(string PropertyName, string StorageField, SelectQuery Query);

public record AggregationCacheOptions(bool EnabledValue, ulong CacheExpiredMillis, bool PropagateValue, ulong PropagateCacheExpiredMillis)
{
    public static AggregationCacheOptions Enabled(ulong cacheExpiredMillis) =>
        new(true, cacheExpiredMillis, false, 0);

    public AggregationCacheOptions Propagate(ulong cacheExpiredMillis) =>
        this with { PropagateValue = true, PropagateCacheExpiredMillis = cacheExpiredMillis };
}

public record StreamConfig(int ChunkSize = 1000);
public record ContinuousPageFetchOptions(string Namespace, int TtlSeconds);

public record SelectQuery
{
    public const ulong DefaultHardLimit = 10_000;
    [JsonIgnore]
    public ulong HardLimitValue { get; set; } = DefaultHardLimit;
    [JsonIgnore]
    public ContinuousPageFetchOptions? ContinuousPageFetch { get; set; }
    public string Entity { get; set; } = "";
    public List<string> Projection { get; set; } = new();
    public List<NamedExpr> ExprProjection { get; set; } = new();
    public string? SearchText { get; set; }
    public Expr? FilterCondition { get; set; }
    public Expr? HavingCondition { get; set; }
    public List<OrderBy> OrderByItems { get; set; } = new();
    public Slice? Slice { get; set; }
    public string? PartitionBy { get; set; }
    public List<Aggregate> AggregateItems { get; set; } = new();
    public List<string> GroupByItems { get; set; } = new();
    public List<RelationLoad> RelationLoads { get; set; } = new();
    public AggregationCacheOptions? AggregationCache { get; set; }
    public string? CommentText { get; set; }
    public List<TraceNode> TraceChain { get; set; } = new();
    public string? RawSqlText { get; set; }
    public List<string> RawSqlSearchCriteriaItems { get; set; } = new();
    public List<RawSqlProjection> DynamicProperties { get; set; } = new();
    public List<RawSqlProjection> RawProjections { get; set; } = new();
    public List<ObjectGroupBy> ObjectGroupBys { get; set; } = new();
    public List<SelectQuery> ChildEnhancements { get; set; } = new();
    public StreamConfig? StreamConfig { get; set; }

    public SelectQuery() { }

    public SelectQuery(string entity)
    {
        Entity = entity;
    }

    public SelectQuery Project(string field)
    {
        Projection.Add(field);
        return this;
    }

    public SelectQuery Projects(IEnumerable<string> fields)
    {
        Projection.AddRange(fields);
        return this;
    }

    public SelectQuery ProjectExpr(string alias, Expr expr)
    {
        ExprProjection.Add(new NamedExpr(alias, expr));
        return this;
    }

    public SelectQuery ProjectRaw(string alias, string rawSqlSegment)
    {
        RawProjections.Add(new RawSqlProjection(alias, rawSqlSegment));
        return this;
    }

    public SelectQuery DynamicPropertyRaw(string alias, string rawSqlSegment)
    {
        DynamicProperties.Add(new RawSqlProjection(alias, rawSqlSegment));
        return this;
    }

    public SelectQuery SearchWithText(string text)
    {
        SearchText = text;
        return this;
    }

    public SelectQuery Filter(Expr filter)
    {
        FilterCondition = filter;
        return this;
    }

    public SelectQuery AndFilter(Expr filter)
    {
        FilterCondition = FilterCondition != null ? FilterCondition.And(filter) : filter;
        return this;
    }

    public SelectQuery OrFilter(Expr filter)
    {
        FilterCondition = FilterCondition != null ? FilterCondition.Or(filter) : filter;
        return this;
    }

    public SelectQuery Having(Expr having)
    {
        HavingCondition = having;
        return this;
    }

    public SelectQuery AndHaving(Expr having)
    {
        HavingCondition = HavingCondition != null ? HavingCondition.And(having) : having;
        return this;
    }

    public SelectQuery OrHaving(Expr having)
    {
        HavingCondition = HavingCondition != null ? HavingCondition.Or(having) : having;
        return this;
    }

    public SelectQuery OrderBy(OrderBy order)
    {
        OrderByItems.Add(order);
        return this;
    }

    public SelectQuery OrderAsc(string field) => OrderBy(Core.OrderBy.Asc(field));
    public SelectQuery OrderDesc(string field) => OrderBy(Core.OrderBy.Desc(field));
    public SelectQuery OrderExprAsc(Expr expr) => OrderBy(Core.OrderBy.AscExpr(expr));
    public SelectQuery OrderExprDesc(Expr expr) => OrderBy(Core.OrderBy.DescExpr(expr));
    public SelectQuery OrderGbkAsc(string field) => OrderBy(Core.OrderBy.AscGbk(field));
    public SelectQuery OrderGbkDesc(string field) => OrderBy(Core.OrderBy.DescGbk(field));

    public SelectQuery GroupBy(string field)
    {
        GroupByItems.Add(field);
        return this;
    }

    public SelectQuery Aggregate(Aggregate aggregate)
    {
        AggregateItems.Add(aggregate);
        return this;
    }

    public SelectQuery Count(string alias) => Aggregate(Core.Aggregate.Count(alias));
    public SelectQuery CountField(string field, string alias) => Aggregate(Core.Aggregate.CountField(field, alias));
    public SelectQuery Sum(string field, string alias) => Aggregate(Core.Aggregate.Sum(field, alias));
    public SelectQuery Avg(string field, string alias) => Aggregate(Core.Aggregate.Avg(field, alias));
    public SelectQuery Min(string field, string alias) => Aggregate(Core.Aggregate.Min(field, alias));
    public SelectQuery Max(string field, string alias) => Aggregate(Core.Aggregate.Max(field, alias));
    public SelectQuery Stddev(string field, string alias) => Aggregate(Core.Aggregate.Stddev(field, alias));
    public SelectQuery StddevPop(string field, string alias) => Aggregate(Core.Aggregate.StddevPop(field, alias));
    public SelectQuery VarSamp(string field, string alias) => Aggregate(Core.Aggregate.VarSamp(field, alias));
    public SelectQuery VarPop(string field, string alias) => Aggregate(Core.Aggregate.VarPop(field, alias));
    public SelectQuery BitAnd(string field, string alias) => Aggregate(Core.Aggregate.BitAnd(field, alias));
    public SelectQuery BitOr(string field, string alias) => Aggregate(Core.Aggregate.BitOr(field, alias));
    public SelectQuery BitXor(string field, string alias) => Aggregate(Core.Aggregate.BitXor(field, alias));

    public SelectQuery EnableAggregationCache() => EnableAggregationCacheFor(0);

    public SelectQuery EnableAggregationCacheFor(ulong cacheExpiredMillis)
    {
        AggregationCache = AggregationCacheOptions.Enabled(cacheExpiredMillis);
        return this;
    }

    public SelectQuery PropagateAggregationCache(ulong cacheExpiredMillis)
    {
        AggregationCache = (AggregationCache ?? AggregationCacheOptions.Enabled(0)).Propagate(cacheExpiredMillis);
        return this;
    }

    public SelectQuery Comment(string comment)
    {
        CommentText = comment;
        TraceChain.Add(new TraceNode(Entity, null, comment));
        return this;
    }

    public SelectQuery RawSql(string rawSql)
    {
        RawSqlText = rawSql;
        return this;
    }

    public SelectQuery RawSqlSearchCriteria(string rawSql)
    {
        RawSqlSearchCriteriaItems.Add(rawSql);
        return this;
    }

    public SelectQuery ObjectGroupBy(string propertyName, string storageField, SelectQuery query)
    {
        ObjectGroupBys.Add(new ObjectGroupBy(propertyName, storageField, query));
        return this;
    }

    public SelectQuery ChildEnhancement(SelectQuery query)
    {
        ChildEnhancements.Add(query);
        return this;
    }

    public SelectQuery Relation(string name)
    {
        RelationLoads.Add(new RelationLoad(name));
        return this;
    }

    public SelectQuery RelationQuery(string name, SelectQuery query)
    {
        RelationLoads.Add(new RelationLoad(name, query));
        return this;
    }

    public SelectQuery Limit(ulong limit)
    {
        Slice = Slice ?? new Slice(null, 0);
        Slice = Slice with { Limit = limit };
        return this;
    }

    /// Override the outer materialized-list ceiling. Most callers should keep 10,000.
    public SelectQuery HardLimit(ulong hardLimit)
    {
        if (hardLimit == 0) throw new ArgumentOutOfRangeException(nameof(hardLimit));
        HardLimitValue = hardLimit;
        return this;
    }

    public SelectQuery PrepareForList()
    {
        ApplyListLimit(HardLimitValue);
        return this;
    }

    private void ApplyListLimit(ulong ceiling)
    {
        if (Slice?.Limit is ulong requested && requested > ceiling)
            throw new InvalidOperationException($"QUERY_HARD_LIMIT_EXCEEDED: requested limit {requested} exceeds hard limit {ceiling}");
        Slice = Slice == null ? new Slice(ceiling, 0) : Slice with { Limit = Slice.Limit ?? ceiling };
        foreach (var relation in RelationLoads) relation.Query?.ApplyListLimit(DefaultHardLimit);
        foreach (var child in ChildEnhancements) child.ApplyListLimit(DefaultHardLimit);
    }

    public SelectQuery Offset(ulong offset)
    {
        Slice = Slice ?? new Slice(null, 0);
        Slice = Slice with { Offset = offset };
        return this;
    }

    public SelectQuery Page(ulong offset, ulong limit) => Offset(offset).Limit(limit);

    public SelectQuery OptimizeForContinuousPageFetch() =>
        OptimizeForContinuousPageFetchWith("default", 600);

    public SelectQuery OptimizeForContinuousPageFetchWith(string @namespace, int ttlSeconds)
    {
        if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentException("namespace must not be empty", nameof(@namespace));
        if (ttlSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(ttlSeconds));
        ContinuousPageFetch = new ContinuousPageFetchOptions(@namespace, ttlSeconds);
        return this;
    }

    public SelectQuery PartitionByField(string field)
    {
        PartitionBy = field;
        return this;
    }

    public SelectQuery Stream(int chunkSize)
    {
        StreamConfig = new StreamConfig(chunkSize);
        return this;
    }

    public SelectQuery StreamDefault()
    {
        StreamConfig = new StreamConfig();
        return this;
    }
}
