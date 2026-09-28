using System.Runtime.CompilerServices;
using TeaQL.Core;

namespace TeaQL.Sql;

/// <summary>Per-compilation provenance, never dialect-instance or thread-local state.
/// Existing custom dialect List&lt;Value&gt; signatures remain usable; unclassified
/// additions fail closed instead of silently inheriting a neighboring policy.</summary>
public static class SqlLogBindings
{
    private sealed class State
    {
        public readonly Dictionary<int, SqlParameterLogPolicy> Policies = new();
        public SqlParameterLogPolicy? Scope;
        public bool Generated;
    }
    private static readonly ConditionalWeakTable<List<Value>, State> States = new();
    public static List<Value> Create()
    {
        var values = new List<Value>();
        States.Add(values, new State { Generated = true });
        return values;
    }
    public static void MarkUntrusted(List<Value> values) => States.GetOrCreateValue(values).Generated = false;
    public static bool IsGenerated(List<Value> values) => States.TryGetValue(values, out var state) && state.Generated;
    public static IReadOnlyList<SqlParameterLogPolicy> Policies(List<Value> values) =>
        Enumerable.Range(0, values.Count).Select(index => States.TryGetValue(values, out var state)
            && state.Policies.TryGetValue(index, out var policy) ? policy : SqlParameterLogPolicy.Unknown).ToArray();

    public static void Add(List<Value> values, Value value, SqlParameterLogPolicy? policy = null)
    {
        var state = States.GetOrCreateValue(values);
        state.Policies[values.Count] = policy ?? state.Scope ?? SqlParameterLogPolicy.Unknown;
        values.Add(value);
    }
    public static void AddField(List<Value> values, EntityDescriptor entity, PropertyDescriptor property, Value value) =>
        Add(values, value, Field(entity, property.Name));

    public static SqlParameterLogPolicy Field(EntityDescriptor entity, string name)
    {
        var property = entity.PropertyByName(name);
        if (property == null) return SqlParameterLogPolicy.Unknown;
        if (SensitiveLogNames.IsCredential(property.Name) || SensitiveLogNames.IsCredential(property.ColumnNameString))
            return SqlParameterLogPolicy.Credential;
        if (!entity.HasExplicitSqlLogPolicyMetadata)
            return SqlParameterLogPolicy.Unknown;
        return entity.AuditMaskFieldList.Contains(property.Name) ? SqlParameterLogPolicy.Masked : SqlParameterLogPolicy.Plain;
    }
    private static SqlParameterLogPolicy? Combine(SqlParameterLogPolicy? left, SqlParameterLogPolicy? right)
    {
        if (left == null) return right;
        if (right == null) return left;
        foreach (var priority in new[] { SqlParameterLogPolicy.Credential, SqlParameterLogPolicy.Unknown,
            SqlParameterLogPolicy.Masked })
            if (left == priority || right == priority) return priority;
        return SqlParameterLogPolicy.Plain;
    }
    private static SqlParameterLogPolicy? Expression(EntityDescriptor entity, Expr expr) => expr switch
    {
        Expr.ColumnExpr e => Field(entity, e.Name),
        Expr.BinaryExpr e => Combine(Expression(entity, e.Left), Expression(entity, e.Right)),
        Expr.FunctionExpr e => e.Args.Aggregate((SqlParameterLogPolicy?)null, (p, child) => Combine(p, Expression(entity, child))),
        Expr.BetweenExpr e => Combine(Expression(entity, e.Expr1), Combine(Expression(entity, e.Lower), Expression(entity, e.Upper))),
        Expr.IsNullExpr e => Expression(entity, e.Expr1),
        Expr.IsNotNullExpr e => Expression(entity, e.Expr1),
        Expr.SubQueryExpr e => Expression(entity, e.Left),
        _ => null
    };
    public static IDisposable EnterExpression(List<Value> values, EntityDescriptor entity, Expr expr)
    {
        var state = States.GetOrCreateValue(values);
        return new Scope(state, Expression(entity, expr) ?? state.Scope);
    }
    public static IDisposable EnterSubquery(List<Value> values) => new Scope(States.GetOrCreateValue(values), null);
    private sealed class Scope : IDisposable
    {
        private readonly State _state;
        private readonly SqlParameterLogPolicy? _old;
        public Scope(State state, SqlParameterLogPolicy? policy) { _state = state; _old = state.Scope; state.Scope = policy; }
        public void Dispose() => _state.Scope = _old;
    }
}
