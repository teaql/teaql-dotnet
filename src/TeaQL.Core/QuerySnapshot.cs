using System.Collections;
using System.Text.Json.Nodes;

namespace TeaQL.Core;

/// <summary>Execution-local copies of mutable AST/data nodes, never Context state.</summary>
internal sealed class QuerySnapshot
{
    private readonly Dictionary<object,object> copies=new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<object> active=new(ReferenceEqualityComparer.Instance);

    private T Capture<T>(T source, Func<T> copy) where T:class
    {
        if (active.Contains(source)) throw new ArgumentException("QUERY_SNAPSHOT_CYCLE: cyclic query/expression/value input is not executable");
        if (copies.TryGetValue(source,out var previous)) return (T)previous;
        active.Add(source);
        try {var result=copy();copies.Add(source,result);return result;}
        finally {active.Remove(source);}
    }

    internal SelectQuery Query(SelectQuery source) => Capture(source,()=> source with {
        Projection=new(source.Projection),
        ExprProjection=source.ExprProjection.Select(item=>item with {Expr=Expression(item.Expr)}).ToList(),
        FilterCondition=OptionalExpression(source.FilterCondition),
        HavingCondition=OptionalExpression(source.HavingCondition),
        OrderByItems=source.OrderByItems.Select(item=>item with {ExprValue=OptionalExpression(item.ExprValue)}).ToList(),
        AggregateItems=new(source.AggregateItems), GroupByItems=new(source.GroupByItems),
        RelationLoads=source.RelationLoads.Select(item=>item with {Query=item.Query is null?null:Query(item.Query)}).ToList(),
        RelationAggregates=source.RelationAggregates.Select(item=>item with {Query=Query(item.Query)}).ToList(),
        TraceChain=new(source.TraceChain), RawSqlSearchCriteriaItems=new(source.RawSqlSearchCriteriaItems),
        DynamicProperties=new(source.DynamicProperties), RawProjections=new(source.RawProjections),
        ObjectGroupBys=source.ObjectGroupBys.Select(item=>item with {Query=Query(item.Query)}).ToList(),
        ChildEnhancements=source.ChildEnhancements.Select(Query).ToList(),
        Filters=source.Filters.Select(item=>new FilterExpression {
            Operator=item.Operator,Field=item.Field,Expected=Expected(item.Expected)}).ToList(),
        Facets=source.Facets.Select(item=>item with {Query=Query(item.Query)}).ToList()
    });

    private Expr? OptionalExpression(Expr? source)=>source is null?null:Expression(source);
    private Expr Expression(Expr source)=>Capture(source,()=>source switch {
        Expr.ColumnExpr => source,
        Expr.ValueExpr item=>item with {NodeValue=Data(item.NodeValue)},
        Expr.FunctionExpr item=>item with {Args=item.Args.Select(Expression).ToList()},
        Expr.BinaryExpr item=>item with {Left=Expression(item.Left),Right=Expression(item.Right)},
        Expr.SubQueryExpr item=>item with {Left=Expression(item.Left),Query=Query(item.Query)},
        Expr.BetweenExpr item=>item with {Expr1=Expression(item.Expr1),Lower=Expression(item.Lower),Upper=Expression(item.Upper)},
        Expr.IsNullExpr item=>item with {Expr1=Expression(item.Expr1)},
        Expr.IsNotNullExpr item=>item with {Expr1=Expression(item.Expr1)},
        Expr.AndExpr item=>item with {Parts=item.Parts.Select(Expression).ToList()},
        Expr.OrExpr item=>item with {Parts=item.Parts.Select(Expression).ToList()},
        Expr.NotExpr item=>item with {Expr1=Expression(item.Expr1)},
        _=>throw new NotSupportedException($"Unsupported query expression {source.GetType().Name}")
    });

    private Value Data(Value source)=>Capture(source,()=>source switch {
        Value.ListValue item=>item with {Values=item.Values.Select(Data).ToList()},
        Value.ObjectValue item=>item with {Value=Record(item.Value)},
        Value.JsonValue item=>item with {Value=item.Value?.DeepClone()},
        _=>source // scalar Value variants are immutable records of scalar values
    });
    private Record Record(Record source)=>Capture(source,()=>new Record(source.ToDictionary(item=>item.Key,item=>Data(item.Value))));
    private object? Expected(object? source)=>source switch {
        null=>null, Value value=>Data(value), Record record=>Record(record), JsonNode json=>json.DeepClone(),
        string or bool or byte or short or int or long or uint or ulong or float or double or decimal
            or DateTime or DateTimeOffset or TimeSpan=>source,
        IEnumerable items=>Capture<object>(items,()=>items.Cast<object?>().Select(Expected).ToArray()),
        _=>throw new ArgumentException($"Unsupported generated query value {source.GetType().Name}")
    };
}
