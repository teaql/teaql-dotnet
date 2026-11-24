<!-- ephemeral -->

# .NET Assist — Expression `Entry`

The generated E facade preserves Value, loaded null, and NotLoaded. `Eval()`
returns a native nullable value for the first two and throws
`TeaQLNotLoadedException` for the third. `OrIfNull` is null-only and never hides
NotLoaded.

The runtime exception's `ToWire()` separates the first missing `location` from
the full `attemptedLocation`, preserving numeric list indices and the model's
JSON pointer naming. `expressionWithBrokenPoint` marks one `<broken>` in the
native chain. Use `suggestedFix.location` with Q Assist; do not parse messages
or invent preload methods.

```csharp
using Generated;
using Generated.Models;

public static class EntryExpressionAssist
{
    // Query-derived aliases are readonly, outside the model-field namespace.
    public static bool HasEntryDynamicProperty(Entry entity, string name) =>
        entity.DynamicProperties.Contains(name);

    public static TeaQL.Core.Value? ReadEntryDynamicProperty(Entry entity, string name) =>
        entity.DynamicProperties.Get(name);

    public static long? ExtractEntryId(Entry entity) =>
        E.Entry(entity).Id().Eval();

    public static long? ExtractEntryIdOrIfNull(Entry entity, long? fallback) =>
        E.Entry(entity).Id().OrIfNull(fallback);

    public static string ExtractEntryName(Entry entity) =>
        E.Entry(entity).Name().Eval();

    public static string ExtractEntryNameOrIfNull(Entry entity, string fallback) =>
        E.Entry(entity).Name().OrIfNull(fallback);

    public static System.DateTime? ExtractEntryCreateTime(Entry entity) =>
        E.Entry(entity).CreateTime().Eval();

    public static System.DateTime? ExtractEntryCreateTimeOrIfNull(Entry entity, System.DateTime? fallback) =>
        E.Entry(entity).CreateTime().OrIfNull(fallback);

    public static System.DateTime? ExtractEntryUpdateTime(Entry entity) =>
        E.Entry(entity).UpdateTime().Eval();

    public static System.DateTime? ExtractEntryUpdateTimeOrIfNull(Entry entity, System.DateTime? fallback) =>
        E.Entry(entity).UpdateTime().OrIfNull(fallback);

    public static long? ExtractEntryVersion(Entry entity) =>
        E.Entry(entity).Version().Eval();

    public static long? ExtractEntryVersionOrIfNull(Entry entity, long? fallback) =>
        E.Entry(entity).Version().OrIfNull(fallback);

    public static long? ExtractEntryCatalogId(Entry entity) =>
        E.Entry(entity).CatalogId().Eval();

    public static Catalog TraverseEntryCatalog(Entry entity) =>
        E.Entry(entity).Catalog().Eval();

}
```

Select every traversed field and relation. Never catch
`TeaQLNotLoadedException` merely to supply a default, and never replace the
generated E facade with null-conditional access.

## Readonly query-derived aliases

Use `entity.DynamicProperties.Contains(alias)` to test whether an aggregate or
other query-derived alias is present. `entity.DynamicProperties.Get(alias)`
returns `TeaQL.Core.Value?`: an absent property is C# null, while a selected SQL
null is `Value.NullValue` and remains present. A selected count of zero is a
present numeric value. Missing dynamic properties do not throw NotLoaded or
become zero; fixed model fields still use the strict E behavior above.

Derived aliases do not become entity members, do not shadow methods such as
`SaveAsync`, and must not enter the mutation payload. Use field Query Assist for
the aggregate request; use List-page Assist for `Count()` plus `Offset`/`Limit`
followed by `ExecuteForListAsync(context)`, not a separate page execution API.

## Typed entity JSON

```csharp
string payload = entity.ToJson();
Entry restored = Entry.FromJson(payload);
string snakePayload = entity.ToJson(TeaQL.Runtime.JsonFieldNamingProfile.SnakeCase);
Entry snakeRestored = Entry.FromJson(
    snakePayload, TeaQL.Runtime.JsonFieldNamingProfile.SnakeCase);
```

The model's naming profile is used by default (camelCase unless configured).
ID/version/Long and exact Decimal use JSON strings; Integer remains a number.
Date is an ISO calendar date; Time retains .NET ticks within one day; DateTime
requires UTC/offset-aware input with millisecond precision. Unsupported precision,
nonfinite numbers and invalid types are rejected, not silently rounded/coerced.
Decimal retains supported scale and is independent of the machine's culture.

An omitted field stays NotLoaded; explicit null stays loaded null. Selected
forward/reverse trees keep typed values and sparse nested projections. The codec
does not fetch data, serialize mutation ledgers or bypass the normal audited save,
Checker, full modified-object loading and policy. Each reconstruction owns a fresh
mutation boundary. Cycles require an explicit reference representation.
`EntityJsonException` provides canonical `Location`, actual `InstancePath` and
`Code`, without echoing values. Entity JSON excludes query/facet envelope metadata
and is not authorization; external editing still needs trusted context permissions
and the context-bound reference adapter.

---

## TeaQL seven-language assist contract

Apply the verified Rust semantic ceiling while using only the exact DOTNET generated and
runtime APIs. Discover APIs through the generated application AGENTS.md and progressive
model-aware Assist. Do not inspect generated domain-library source.

- Do not create plurals by appending `s` or `es`; use the centralized generated plural.
- Human and non-human entities use different generated predicate vocabularies. Preserve
  forms such as “who are active” and “whose email is”; never infer them from English.
- Configure filters, projection, paging, and other query options before `purpose(...)`.
  Comment may appear anywhere in the chain. Purpose enters the executable stage; execution
  requires both values, but comment does not have to immediately precede purpose.
- Every execute/list/stream and every save accepts exactly one context argument:
  `UserContext`. Name that argument `context`, never `runtime`; data services and global
  policy are injected when the context is built. Reserve `runtime` for process-level
  runtime ownership, provider/pool setup, and module assembly.
- Tenant, merchant, identity, permissions, request policy, purpose policy, hard limit,
  and continuous-page cursor policy come only from trusted context, never dynamic JSON or TFP.
- If the required operation is absent after current entity/action and required field
  Assist, stop that path and report MISSING_ASSIST. Do not guess an API or search the
  generated library as a fallback.
- Create each application-owned source file once. After its first compile attempt,
  repair only the smallest block identified by the exact compiler or test diagnostic.
  Preserve unrelated code; do not rewrite the complete file as an error-recovery loop.
- Before a repair that would replace more than 25% of an existing application file,
  stop and report LARGE_REWRITE_REQUEST with the file, exact diagnostic, reason, and
  estimated scope. Initial creation and model-driven regeneration are not repairs.

Capability: `expression`.

- Distinguish a loaded null from a field or relation that was not projected. A
  NotLoaded/coding error must remain visible; do not turn it into an ordinary null.
- Select every traversed relation first and use the generated E/expression API for
  scalar, object, and list traversal. Do not translate Java accessor names by guess.
