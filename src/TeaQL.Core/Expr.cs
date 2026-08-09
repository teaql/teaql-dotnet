using System.Collections.Generic;
using System.Linq;

namespace TeaQL.Core;

public enum BinaryOp
{
    Eq, Ne, Gt, Gte, Lt, Lte, Like, NotLike, In, NotIn, InLarge, NotInLarge
}

public enum ExprFunction
{
    Soundex, Gbk, Count, Sum, Avg, Min, Max, Stddev, StddevPop, VarSamp, VarPop, BitAnd, BitOr, BitXor
}

public abstract record Expr
{
    private Expr() { }

    public sealed record ColumnExpr(string Name) : Expr;
    public sealed record ValueExpr(Value Value) : Expr;
    public sealed record FunctionExpr(ExprFunction Fn, List<Expr> Args) : Expr;
    public sealed record BinaryExpr(Expr Left, BinaryOp Op, Expr Right) : Expr;
    public sealed record SubQueryExpr(Expr Left, BinaryOp Op, EntityDescriptor Entity, SelectQuery Query) : Expr;
    public sealed record BetweenExpr(Expr Expr1, Expr Lower, Expr Upper) : Expr;
    public sealed record IsNullExpr(Expr Expr1) : Expr;
    public sealed record IsNotNullExpr(Expr Expr1) : Expr;
    public sealed record AndExpr(List<Expr> Parts) : Expr;
    public sealed record OrExpr(List<Expr> Parts) : Expr;
    public sealed record NotExpr(Expr Expr1) : Expr;

    public static Expr Column(string name) => new ColumnExpr(name);

    public static Expr Value(Value value) => new ValueExpr(value);

    public static Expr Function(ExprFunction function, IEnumerable<Expr> args) => 
        new FunctionExpr(function, args.ToList());

    public static Expr Soundex(Expr expr) => Function(ExprFunction.Soundex, new[] { expr });
    public static Expr Gbk(Expr expr) => Function(ExprFunction.Gbk, new[] { expr });
    public static Expr CountAll() => Function(ExprFunction.Count, new Expr[0]);
    public static Expr CountExpr(Expr expr) => Function(ExprFunction.Count, new[] { expr });
    public static Expr SumExpr(Expr expr) => Function(ExprFunction.Sum, new[] { expr });
    public static Expr AvgExpr(Expr expr) => Function(ExprFunction.Avg, new[] { expr });
    public static Expr MinExpr(Expr expr) => Function(ExprFunction.Min, new[] { expr });
    public static Expr MaxExpr(Expr expr) => Function(ExprFunction.Max, new[] { expr });
    public static Expr StddevExpr(Expr expr) => Function(ExprFunction.Stddev, new[] { expr });
    public static Expr StddevPopExpr(Expr expr) => Function(ExprFunction.StddevPop, new[] { expr });
    public static Expr VarSampExpr(Expr expr) => Function(ExprFunction.VarSamp, new[] { expr });
    public static Expr VarPopExpr(Expr expr) => Function(ExprFunction.VarPop, new[] { expr });
    public static Expr BitAndExpr(Expr expr) => Function(ExprFunction.BitAnd, new[] { expr });
    public static Expr BitOrExpr(Expr expr) => Function(ExprFunction.BitOr, new[] { expr });
    public static Expr BitXorExpr(Expr expr) => Function(ExprFunction.BitXor, new[] { expr });

    public static Expr SoundLike(string column, Value value) => 
        Binary(Soundex(Column(column)), BinaryOp.Eq, Soundex(Value(value)));

    public static Expr Eq(string column, Value value) => Binary(Column(column), BinaryOp.Eq, Value(value));
    public static Expr Ne(string column, Value value) => Binary(Column(column), BinaryOp.Ne, Value(value));
    public static Expr Gt(string column, Value value) => Binary(Column(column), BinaryOp.Gt, Value(value));
    public static Expr Gte(string column, Value value) => Binary(Column(column), BinaryOp.Gte, Value(value));
    public static Expr Lt(string column, Value value) => Binary(Column(column), BinaryOp.Lt, Value(value));
    public static Expr Lte(string column, Value value) => Binary(Column(column), BinaryOp.Lte, Value(value));
    
    public static Expr Like(string column, string pattern) => 
        Binary(Column(column), BinaryOp.Like, Value(new Core.Value.TextValue(pattern)));
        
    public static Expr NotLike(string column, string pattern) => 
        Binary(Column(column), BinaryOp.NotLike, Value(new Core.Value.TextValue(pattern)));

    public static Expr Contain(string column, string value) => Like(column, $"%{value}%");
    public static Expr NotContain(string column, string value) => NotLike(column, $"%{value}%");
    public static Expr BeginWith(string column, string value) => Like(column, $"{value}%");
    public static Expr NotBeginWith(string column, string value) => NotLike(column, $"{value}%");
    public static Expr EndWith(string column, string value) => Like(column, $"%{value}");
    public static Expr NotEndWith(string column, string value) => NotLike(column, $"%{value}");

    public static Expr Binary(Expr left, BinaryOp op, Expr right) => new BinaryExpr(left, op, right);

    public static Expr CompareColumns(string leftColumn, BinaryOp op, string rightColumn) => 
        Binary(Column(leftColumn), op, Column(rightColumn));

    public static Expr InList(string column, IEnumerable<Value> values) => 
        Binary(Column(column), BinaryOp.In, Value(new Core.Value.ListValue(values.ToList())));

    public static Expr NotInList(string column, IEnumerable<Value> values) => 
        Binary(Column(column), BinaryOp.NotIn, Value(new Core.Value.ListValue(values.ToList())));

    public static Expr InLarge(string column, IEnumerable<Value> values) => 
        Binary(Column(column), BinaryOp.InLarge, Value(new Core.Value.ListValue(values.ToList())));

    public static Expr NotInLarge(string column, IEnumerable<Value> values) => 
        Binary(Column(column), BinaryOp.NotInLarge, Value(new Core.Value.ListValue(values.ToList())));

    public static Expr InSubquery(string column, EntityDescriptor entity, SelectQuery query, string field) => 
        Subquery(Column(column), BinaryOp.In, entity, query, field);

    public static Expr NotInSubquery(string column, EntityDescriptor entity, SelectQuery query, string field) => 
        Subquery(Column(column), BinaryOp.NotIn, entity, query, field);

    public static Expr Subquery(Expr left, BinaryOp op, EntityDescriptor entity, SelectQuery query, string field)
    {
        query.Projection = new List<string> { field };
        return new SubQueryExpr(left, op, entity, query);
    }

    public static Expr Between(string column, Value lower, Value upper) => 
        new BetweenExpr(Column(column), Value(lower), Value(upper));

    public static Expr IsNull(string column) => new IsNullExpr(Column(column));
    public static Expr IsNotNull(string column) => new IsNotNullExpr(Column(column));

    public static Expr And(IEnumerable<Expr> parts)
    {
        var unique = new List<Expr>();
        foreach (var part in parts)
        {
            if (!unique.Contains(part))
            {
                unique.Add(part);
            }
        }
        return new AndExpr(unique);
    }

    public static Expr Or(IEnumerable<Expr> parts)
    {
        var unique = new List<Expr>();
        foreach (var part in parts)
        {
            if (!unique.Contains(part))
            {
                unique.Add(part);
            }
        }
        return new OrExpr(unique);
    }

    public static Expr Negate(Expr expr) => new NotExpr(expr);

    public Expr AndExpr(Expr other)
    {
        if (this == other) return this;
        
        if (this is AndExpr andExpr)
        {
            var parts = new List<Expr>(andExpr.Parts);
            if (!parts.Contains(other))
            {
                parts.Add(other);
            }
            return new AndExpr(parts);
        }
        
        return new AndExpr(new List<Expr> { this, other });
    }

    public Expr OrExpr(Expr other)
    {
        if (this == other) return this;
        
        if (this is OrExpr orExpr)
        {
            var parts = new List<Expr>(orExpr.Parts);
            if (!parts.Contains(other))
            {
                parts.Add(other);
            }
            return new OrExpr(parts);
        }
        
        return new OrExpr(new List<Expr> { this, other });
    }
}
