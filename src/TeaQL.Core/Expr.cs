using System.Collections.Generic;

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
    public sealed record FunctionExpr(ExprFunction Function, List<Expr> Args) : Expr;
    public sealed record BinaryExpr(Expr Left, BinaryOp Op, Expr Right) : Expr;
    public sealed record SubQueryExpr(Expr Left, BinaryOp Op, EntityDescriptor Entity, SelectQuery Query) : Expr;
    public sealed record BetweenExpr(Expr Expr, Expr Lower, Expr Upper) : Expr;
    public sealed record IsNullExpr(Expr Expr) : Expr;
    public sealed record IsNotNullExpr(Expr Expr) : Expr;
    public sealed record AndExpr(List<Expr> Parts) : Expr;
    public sealed record OrExpr(List<Expr> Parts) : Expr;
    public sealed record NotExpr(Expr Expr) : Expr;
}
