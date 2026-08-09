using System;
using System.Collections.Generic;

namespace TeaQL.Core;

public abstract record LoadState
{
    public sealed record NotLoadedState : LoadState;
    public sealed record PartialState(HashSet<string> Fields) : LoadState;
    public sealed record FullyLoadedState : LoadState;

    public static readonly LoadState NotLoaded = new NotLoadedState();
    public static readonly LoadState FullyLoaded = new FullyLoadedState();
    
    public static LoadState Partial(HashSet<string> fields) => new PartialState(fields);

    public bool IsLoaded(string fieldOrRelation)
    {
        return this switch
        {
            NotLoadedState => false,
            FullyLoadedState => true,
            PartialState partial => partial.Fields.Contains(fieldOrRelation),
            _ => false
        };
    }
}

public abstract record EvalResult<T>
{
    public sealed record ValueResult(T ValueItem) : EvalResult<T>;
    public sealed record NullResult : EvalResult<T>;
    public sealed record NotLoadedResult(string FailedNode, string AttemptedPath) : EvalResult<T>;

    public static EvalResult<T> Value(T value) => new ValueResult(value);
    public static readonly EvalResult<T> Null = new NullResult();
    public static EvalResult<T> NotLoaded(string failedNode, string attemptedPath) => new NotLoadedResult(failedNode, attemptedPath);

    public EvalResult<U> AndThen<U>(string fieldName, Func<T, EvalResult<U>> f)
    {
        return this switch
        {
            ValueResult val => f(val.ValueItem) switch
            {
                EvalResult<U>.NotLoadedResult notLoaded => 
                    new EvalResult<U>.NotLoadedResult(
                        notLoaded.FailedNode,
                        (notLoaded.AttemptedPath == fieldName, string.IsNullOrEmpty(notLoaded.AttemptedPath)) switch
                        {
                            (true, _) => notLoaded.AttemptedPath,
                            (_, true) => fieldName,
                            _ => $"{fieldName}.{notLoaded.AttemptedPath}"
                        }
                    ),
                var other => other
            },
            NullResult => EvalResult<U>.Null,
            NotLoadedResult notLoaded => EvalResult<U>.NotLoaded(notLoaded.FailedNode, notLoaded.AttemptedPath),
            _ => throw new InvalidOperationException()
        };
    }

    public EvalResult<U> Map<U>(Func<T, U> f)
    {
        return this switch
        {
            ValueResult val => EvalResult<U>.Value(f(val.ValueItem)),
            NullResult => EvalResult<U>.Null,
            NotLoadedResult notLoaded => EvalResult<U>.NotLoaded(notLoaded.FailedNode, notLoaded.AttemptedPath),
            _ => throw new InvalidOperationException()
        };
    }
}
