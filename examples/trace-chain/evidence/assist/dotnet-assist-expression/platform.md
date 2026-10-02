<!-- ephemeral -->

# .NET Assist — Expression `Platform`

The generated E facade preserves Value, loaded null, and NotLoaded. `Eval()`
returns a native nullable value for the first two and throws
`TeaQLNotLoadedException` for the third. `OrIfNull` is null-only and never hides
NotLoaded.

```csharp
using Generated;
using Generated.Models;

public static class PlatformExpressionAssist
{
    public static long? ExtractPlatformId(Platform entity) =>
        E.Platform(entity).Id().Eval();

    public static long? ExtractPlatformIdOrIfNull(Platform entity, long? fallback) =>
        E.Platform(entity).Id().OrIfNull(fallback);

    public static string ExtractPlatformName(Platform entity) =>
        E.Platform(entity).Name().Eval();

    public static string ExtractPlatformNameOrIfNull(Platform entity, string fallback) =>
        E.Platform(entity).Name().OrIfNull(fallback);

    public static long? ExtractPlatformVersion(Platform entity) =>
        E.Platform(entity).Version().Eval();

    public static long? ExtractPlatformVersionOrIfNull(Platform entity, long? fallback) =>
        E.Platform(entity).Version().OrIfNull(fallback);

    public static int? AggregatePlatformCustomerOrderListSize(Platform entity) =>
        E.Platform(entity).CustomerOrderList().Size().Eval();

    public static long? FirstPlatformCustomerOrderListId(Platform entity) =>
        E.Platform(entity).CustomerOrderList().First().Id().Eval();

    public static long? GetPlatformCustomerOrderListId(Platform entity, int index) =>
        E.Platform(entity).CustomerOrderList().Get(index).Id().Eval();

}
```

Select every traversed field and relation. Never catch
`TeaQLNotLoadedException` merely to supply a default, and never replace the
generated E facade with null-conditional access.

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
