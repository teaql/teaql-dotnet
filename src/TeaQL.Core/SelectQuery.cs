using System.Collections.Generic;
using System.Linq;

namespace TeaQL.Core;

public enum SortDirection { Asc, Desc }

public record NamedExpr(string Alias, Expr Expr);

public record OrderBy(string Field, Expr? Expr, SortDirection Direction)
{
    public static OrderBy New(string field, SortDirection direction) => new(field, null, direction);
    public static OrderBy WithExpr(Expr expr, SortDirection direction) => new("", expr, direction);
    
    public static OrderBy Asc(string field) => New(field, SortDirection.Asc);
    public static OrderBy Desc(string field) => New(field, SortDirection.Desc);
    
    public static OrderBy AscExpr(Expr expr) => WithExpr(expr, SortDirection.Asc);
    public static OrderBy DescExpr(Expr expr) => WithExpr(expr, SortDirection.Desc);
    
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

public record RelationLoad(string Name, SelectQuery? Query = null);

public record RelationAggregate(string RelationName, string Alias, SelectQuery Query, bool SingleResult);

public record RawSqlProjection(string PropertyName, string RawSqlSegment);

public record ObjectGroupBy(string PropertyName, string StorageField, SelectQuery Query);

public record AggregationCacheOptions(bool Enabled, ulong CacheExpiredMillis, bool Propagate, ulong PropagateCacheExpiredMillis)
{
    public static AggregationCacheOptions CreateEnabled(ulong cacheExpiredMillis) => 
        new(true, cacheExpiredMillis, false, 0);

    public AggregationCacheOptions WithPropagate(ulong cacheExpiredMillis) =>
        this with { Propagate = true, PropagateCacheExpiredMillis = cacheExpiredMillis };
}

public record StreamConfig(int ChunkSize = 1000);

public record SelectQuery
{
    public string Entity { get; set; } = "";
    public List<string> Projection { get; set; } = new();
    public List<NamedExpr> ExprProjection { get; set; } = new();
    public string? SearchWithText { get; set; }
    public Expr? Filter { get; set; }
    public Expr? Having { get; set; }
    public List<OrderBy> OrderBy { get; set; } = new();
    public Slice? Slice { get; set; }
    public List<Aggregate> Aggregates { get; set; } = new();
    public List<string> GroupBy { get; set; } = new();
    public List<RelationLoad> Relations { get; set; } = new();
    public AggregationCacheOptions? AggregationCache { get; set; }
    public string? Comment { get; set; }
    public List<TraceNode> TraceChain { get; set; } = new();
    public string? RawSql { get; set; }
    public List<string> RawSqlSearchCriteria { get; set; } = new();
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

    public SelectQuery WithSearchWithText(string text)
    {
        SearchWithText = text;
        return this;
    }

    public SelectQuery WithFilter(Expr filter)
    {
        Filter = filter;
        return this;
    }

    public SelectQuery AndFilter(Expr filter)
    {
        Filter = Filter != null ? Filter.AndExprMethod(filter) : filter;
        return this;
    }

    public SelectQuery OrFilter(Expr filter)
    {
        Filter = Filter != null ? Filter.OrExprMethod(filter) : filter;
        return this;
    }

    public SelectQuery WithHaving(Expr having)
    {
        Having = having;
        return this;
    }

    public SelectQuery AndHaving(Expr having)
    {
        Having = Having != null ? Having.AndExprMethod(having) : having;
        return this;
    }

    public SelectQuery OrHaving(Expr having)
    {
        Having = Having != null ? Having.OrExprMethod(having) : having;
        return this;
    }

    public SelectQuery WithOrderBy(OrderBy order)
    {
        OrderBy.Add(order);
        return this;
    }

    public SelectQuery OrderAsc(string field) => WithOrderBy(Core.OrderBy.Asc(field));
    public SelectQuery OrderDesc(string field) => WithOrderBy(Core.OrderBy.Desc(field));
    public SelectQuery OrderExprAsc(Expr expr) => WithOrderBy(Core.OrderBy.AscExpr(expr));
    public SelectQuery OrderExprDesc(Expr expr) => WithOrderBy(Core.OrderBy.DescExpr(expr));
    public SelectQuery OrderGbkAsc(string field) => WithOrderBy(Core.OrderBy.AscGbk(field));
    public SelectQuery OrderGbkDesc(string field) => WithOrderBy(Core.OrderBy.DescGbk(field));

    public SelectQuery WithGroupBy(string field)
    {
        GroupBy.Add(field);
        return this;
    }

    public SelectQuery WithAggregate(Aggregate aggregate)
    {
        Aggregates.Add(aggregate);
        return this;
    }

    public SelectQuery Count(string alias) => WithAggregate(Aggregate.Count(alias));
    public SelectQuery CountField(string field, string alias) => WithAggregate(Aggregate.CountField(field, alias));
    public SelectQuery Sum(string field, string alias) => WithAggregate(Aggregate.Sum(field, alias));
    public SelectQuery Avg(string field, string alias) => WithAggregate(Aggregate.Avg(field, alias));
    public SelectQuery Min(string field, string alias) => WithAggregate(Aggregate.Min(field, alias));
    public SelectQuery Max(string field, string alias) => WithAggregate(Aggregate.Max(field, alias));
    public SelectQuery Stddev(string field, string alias) => WithAggregate(Aggregate.Stddev(field, alias));
    public SelectQuery StddevPop(string field, string alias) => WithAggregate(Aggregate.StddevPop(field, alias));
    public SelectQuery VarSamp(string field, string alias) => WithAggregate(Aggregate.VarSamp(field, alias));
    public SelectQuery VarPop(string field, string alias) => WithAggregate(Aggregate.VarPop(field, alias));
    public SelectQuery BitAnd(string field, string alias) => WithAggregate(Aggregate.BitAnd(field, alias));
    public SelectQuery BitOr(string field, string alias) => WithAggregate(Aggregate.BitOr(field, alias));
    public SelectQuery BitXor(string field, string alias) => WithAggregate(Aggregate.BitXor(field, alias));

    public SelectQuery EnableAggregationCache() => EnableAggregationCacheFor(0);

    public SelectQuery EnableAggregationCacheFor(ulong cacheExpiredMillis)
    {
        AggregationCache = AggregationCacheOptions.CreateEnabled(cacheExpiredMillis);
        return this;
    }

    public SelectQuery PropagateAggregationCache(ulong cacheExpiredMillis)
    {
        AggregationCache = (AggregationCache ?? AggregationCacheOptions.CreateEnabled(0)).WithPropagate(cacheExpiredMillis);
        return this;
    }

    public SelectQuery WithComment(string comment)
    {
        Comment = comment;
        TraceChain.Add(new TraceNode(Entity, null, comment));
        return this;
    }

    public SelectQuery WithRawSql(string rawSql)
    {
        RawSql = rawSql;
        return this;
    }

    public SelectQuery WithRawSqlSearchCriteria(string rawSql)
    {
        RawSqlSearchCriteria.Add(rawSql);
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
        Relations.Add(new RelationLoad(name));
        return this;
    }

    public SelectQuery RelationQuery(string name, SelectQuery query)
    {
        Relations.Add(new RelationLoad(name, query));
        return this;
    }

    public SelectQuery Limit(ulong limit)
    {
        Slice = Slice ?? new Slice(null, 0);
        Slice = Slice with { Limit = limit };
        return this;
    }

    public SelectQuery Offset(ulong offset)
    {
        Slice = Slice ?? new Slice(null, 0);
        Slice = Slice with { Offset = offset };
        return this;
    }

    public SelectQuery Page(ulong offset, ulong limit) => Offset(offset).Limit(limit);

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
