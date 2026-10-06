<!-- ephemeral -->

# Create `CustomerOrder` with the generated .NET API

Use this model-aware Assist for API discovery. Do not read generated library
source. Allow-list writable fields; never mass-assign untrusted JSON.

```csharp
using System.Threading.Tasks;
using Generated;
using Generated.Models;
using TeaQL.Core;

public static class CustomerOrderCreateService
{
    public static async Task<CustomerOrder> CreateAsync(UserContext context)
    {
        var entity = Q.CustomerOrders()
            .Comment("initialize an authorized Customer Order")
            .Purpose("create Customer Order")
            .NewEntity(context);

        // entity.UpdatePlatformId(relatedId); // validated relation id
        // entity.UpdateOrderNumber(value); // allow-listed business input
        // entity.UpdateDescription(value); // allow-listed business input

        await entity
            .AuditAs("business reason for creating Customer Order")
            .SaveAsync(context);
        return entity;
    }
}
```

`Purpose(...)` enters the executable stage and `NewEntity` accepts exactly one
context argument, `UserContext`. Add a negative test proving an empty audit
reason cannot write. Output C# that uses the exact generated updater names.
## Compose children before saving the root

- Append a generated `OrderItem` to `entity.OrderItemList` with `entity.OrderItemList.Add(child)`.
- Append a generated `Payment` to `entity.PaymentList` with `entity.PaymentList.Add(child)`.
- Append a generated `Shipment` to `entity.ShipmentList` with `entity.ShipmentList.Add(child)`.

Create each child through its generated Q request and `NewEntity(context)`.
Set the child's allow-listed scalar fields; the graph save fills its parent FK.
Children without AuditAs inherit the parent chain without duplicating a node.
Call `child.AuditAs("local business reason")` only for a distinct child action.
Use a loaded child and `MarkForDeletion()` for deletion in the same graph.
Save only the audited root; do not call a second root SaveAsync inside a save.


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

Capability: `create`.

- Validate and allow-list writable business fields; never mass-assign dynamic JSON.
- Create through the generated request/entity API, attach a non-empty audit reason,
  save with the same UserContext, and return the runtime's native save result.
- Add a negative test proving a missing audit reason cannot write.
