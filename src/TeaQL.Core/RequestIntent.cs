namespace TeaQL.Core;

/// <summary>A value-free, portable request validation failure.</summary>
public sealed class RequestIntentException : ArgumentException
{
    public string Code { get; }
    public string Field { get; }
    public string RequestKind { get; }

    public RequestIntentException(string code, string field, string requestKind)
        : base($"{code}: {requestKind} request requires {field}")
    {
        Code = code;
        Field = field;
        RequestKind = requestKind;
    }
}

internal static class RequestIntentValidation
{
    // Match Rust's Unicode White_Space exactly, not language-specific trimming.
    internal static string Required(string? text, string field, string kind)
    {
        if (text == null || text.Length == 0 || text.All(IsWhiteSpace))
            throw new RequestIntentException(field == "purpose"
                ? "QUERY_PURPOSE_REQUIRED" : "REQUEST_COMMENT_REQUIRED", field, kind);
        return text; // Preserve valid caller text verbatim.
    }

    private static bool IsWhiteSpace(char value) => value is >= '\u0009' and <= '\u000d'
        or '\u0020' or '\u0085' or '\u00a0' or '\u1680'
        or >= '\u2000' and <= '\u200a'
        or '\u2028' or '\u2029' or '\u202f' or '\u205f' or '\u3000';
}

/// <summary>Immutable caller intent, independent of the mutable query builder.</summary>
public sealed class QueryIntent
{
    public string Comment { get; }
    public string Purpose { get; }

    public QueryIntent(string? comment, string? purpose)
    {
        Comment = RequestIntentValidation.Required(comment, "comment", "query");
        Purpose = RequirePurpose(purpose);
    }

    public static string RequirePurpose(string? purpose) =>
        RequestIntentValidation.Required(purpose, "purpose", "query");
}

/// <summary>One required mutation comment; auditReason is its semantic alias.</summary>
public sealed class MutationIntent
{
    public string Comment { get; }

    public MutationIntent(string? comment) =>
        Comment = RequestIntentValidation.Required(comment, "comment", "mutation");

    public QueryIntent ReadbackIntent() =>
        new(Comment, "verify the persisted mutation result");
}
