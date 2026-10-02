<!-- ephemeral -->

# .NET Assist — Expression `CustomerOrder`

The generated E facade preserves Value, loaded null, and NotLoaded. `Eval()`
returns a native nullable value for the first two and throws
`TeaQLNotLoadedException` for the third. `OrIfNull` is null-only and never hides
NotLoaded.

```csharp
using Generated;
using Generated.Models;

public static class CustomerOrderExpressionAssist
{
    public static long? ExtractCustomerOrderId(CustomerOrder entity) =>
        E.CustomerOrder(entity).Id().Eval();

    public static long? ExtractCustomerOrderIdOrIfNull(CustomerOrder entity, long? fallback) =>
        E.CustomerOrder(entity).Id().OrIfNull(fallback);

    public static string ExtractCustomerOrderOrderNumber(CustomerOrder entity) =>
        E.CustomerOrder(entity).OrderNumber().Eval();

    public static string ExtractCustomerOrderOrderNumberOrIfNull(CustomerOrder entity, string fallback) =>
        E.CustomerOrder(entity).OrderNumber().OrIfNull(fallback);

    public static string ExtractCustomerOrderDescription(CustomerOrder entity) =>
        E.CustomerOrder(entity).Description().Eval();

    public static string ExtractCustomerOrderDescriptionOrIfNull(CustomerOrder entity, string fallback) =>
        E.CustomerOrder(entity).Description().OrIfNull(fallback);

    public static long? ExtractCustomerOrderVersion(CustomerOrder entity) =>
        E.CustomerOrder(entity).Version().Eval();

    public static long? ExtractCustomerOrderVersionOrIfNull(CustomerOrder entity, long? fallback) =>
        E.CustomerOrder(entity).Version().OrIfNull(fallback);

    public static long? ExtractCustomerOrderPlatformId(CustomerOrder entity) =>
        E.CustomerOrder(entity).PlatformId().Eval();

    public static Platform TraverseCustomerOrderPlatform(CustomerOrder entity) =>
        E.CustomerOrder(entity).Platform().Eval();

    public static int? AggregateCustomerOrderOrderItemListSize(CustomerOrder entity) =>
        E.CustomerOrder(entity).OrderItemList().Size().Eval();

    public static long? FirstCustomerOrderOrderItemListId(CustomerOrder entity) =>
        E.CustomerOrder(entity).OrderItemList().First().Id().Eval();

    public static long? GetCustomerOrderOrderItemListId(CustomerOrder entity, int index) =>
        E.CustomerOrder(entity).OrderItemList().Get(index).Id().Eval();

    public static int? AggregateCustomerOrderPaymentListSize(CustomerOrder entity) =>
        E.CustomerOrder(entity).PaymentList().Size().Eval();

    public static long? FirstCustomerOrderPaymentListId(CustomerOrder entity) =>
        E.CustomerOrder(entity).PaymentList().First().Id().Eval();

    public static long? GetCustomerOrderPaymentListId(CustomerOrder entity, int index) =>
        E.CustomerOrder(entity).PaymentList().Get(index).Id().Eval();

    public static int? AggregateCustomerOrderShipmentListSize(CustomerOrder entity) =>
        E.CustomerOrder(entity).ShipmentList().Size().Eval();

    public static long? FirstCustomerOrderShipmentListId(CustomerOrder entity) =>
        E.CustomerOrder(entity).ShipmentList().First().Id().Eval();

    public static long? GetCustomerOrderShipmentListId(CustomerOrder entity, int index) =>
        E.CustomerOrder(entity).ShipmentList().Get(index).Id().Eval();

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
