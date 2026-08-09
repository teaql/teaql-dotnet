using System;
using System.Collections;
using System.Text.Json.Nodes;

namespace TeaQL.Core;

public static class TeaqlEmptyHelper
{
    public static bool IsEmpty<T>(T? value)
    {
        if (value == null) return true;
        if (value is string s) return string.IsNullOrEmpty(s);
        if (value is ICollection c) return c.Count == 0;
        if (value is Value v)
        {
            return v switch
            {
                Value.NullValue => true,
                Value.TextValue text => string.IsNullOrEmpty(text.Value),
                Value.ListValue list => list.Values.Count == 0,
                Value.ObjectValue obj => obj.Value.Count == 0,
                Value.JsonValue json => IsJsonEmpty(json.Value),
                _ => false
            };
        }
        return false;
    }

    private static bool IsJsonEmpty(JsonNode? node)
    {
        if (node == null) return true;
        if (node is JsonArray arr) return arr.Count == 0;
        if (node is JsonObject obj) return obj.Count == 0;
        if (node is JsonValue val && val.TryGetValue<string>(out var str)) return string.IsNullOrEmpty(str);
        return false;
    }
}

public class SafeExpression<R, T>
{
    private readonly R _root;
    private readonly Func<R, (bool HasValue, T Value)> _evaluator;

    public SafeExpression(R root, Func<R, (bool HasValue, T Value)> evaluator)
    {
        _root = root;
        _evaluator = evaluator;
    }

    public R Root() => _root;

    public (bool HasValue, T Value) Eval()
    {
        return _evaluator(_root);
    }

    public (bool HasValue, T Value) EvalWith(R root)
    {
        return _evaluator(root);
    }

    public SafeExpression<R, U> Apply<U>(Func<T, U> mapper)
    {
        return ApplyOptional(value => (true, mapper(value)));
    }

    public SafeExpression<R, U> ApplyOptional<U>(Func<T, (bool HasValue, U Value)> mapper)
    {
        var evaluator = _evaluator;
        return new SafeExpression<R, U>(
            _root,
            root => 
            {
                var val = evaluator(root);
                if (!val.HasValue) return (false, default(U)!);
                return mapper(val.Value);
            }
        );
    }

    public T OrElse(T defaultValue)
    {
        var val = Eval();
        return val.HasValue ? val.Value : defaultValue;
    }

    public T OrElseWith(Func<T> defaultValue)
    {
        var val = Eval();
        return val.HasValue ? val.Value : defaultValue();
    }

    public T OrElseThrow<E>(Func<E> error) where E : Exception
    {
        var val = Eval();
        if (!val.HasValue) throw error();
        return val.Value;
    }

    public bool IsNull()
    {
        return !Eval().HasValue;
    }

    public bool IsNotNull()
    {
        return Eval().HasValue;
    }

    public bool IsEmpty()
    {
        var val = Eval();
        if (!val.HasValue) return true;
        return TeaqlEmptyHelper.IsEmpty(val.Value);
    }

    public bool IsNotEmpty()
    {
        return !IsEmpty();
    }

    public void WhenIsNull(Action function)
    {
        if (IsNull())
        {
            function();
        }
    }

    public void WhenIsNotNull(Action<T> consumer)
    {
        var val = Eval();
        if (val.HasValue)
        {
            consumer(val.Value);
        }
    }

    public void WhenIsEmpty(Action function)
    {
        if (IsEmpty())
        {
            function();
        }
    }

    public void WhenNotEmpty(Action<T> consumer)
    {
        var val = Eval();
        if (val.HasValue && !TeaqlEmptyHelper.IsEmpty(val.Value))
        {
            consumer(val.Value);
        }
    }
}

public static class SafeExpression
{
    public static SafeExpression<R, R> Value<R>(R root)
    {
        return new SafeExpression<R, R>(root, r => (true, r));
    }

    public static SafeExpression<R, ulong> EntityId<R, E>(this SafeExpression<R, E> expr) where E : IBaseEntity
    {
        return expr.Apply(entity => entity.Id);
    }

    public static SafeExpression<R, long> EntityVersion<R, E>(this SafeExpression<R, E> expr) where E : IBaseEntity
    {
        return expr.Apply(entity => entity.VersionValue);
    }

    public static SafeExpression<R, E> UpdateEntityId<R, E>(this SafeExpression<R, E> expr, ulong id) where E : IBaseEntity
    {
        return expr.Apply(entity => 
        {
            entity.SetId(id);
            return entity;
        });
    }

    public static SafeExpression<R, int> Size<R, T>(this SafeExpression<R, SmartList<T>> expr)
    {
        return expr.Apply(list => list.Count);
    }

    public static SafeExpression<R, T> First<R, T>(this SafeExpression<R, SmartList<T>> expr)
    {
        return expr.ApplyOptional(list => list.Count > 0 ? (true, list[0]) : (false, default(T)!));
    }

    public static SafeExpression<R, T> Get<R, T>(this SafeExpression<R, SmartList<T>> expr, int index)
    {
        return expr.ApplyOptional(list => (index >= 0 && index < list.Count) ? (true, list[index]) : (false, default(T)!));
    }
}
