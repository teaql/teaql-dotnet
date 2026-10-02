<!-- ephemeral -->

# Update `PaymentAttempt` with optimistic locking

Load the current entity through its generated request. Never reconstruct a
versioned entity from an untrusted DTO: `Id` and `Version` must come from the
tenant-scoped database read.

An entity update is not a partial DTO patch. Load every scalar field before
modification so checker rules can validate the complete business state. Never
use a minimal request or reduced projection for an entity that will be saved.

```csharp
using System;
using System.Threading.Tasks;
using Generated;
using Generated.Models;
using TeaQL.Core;

public static class PaymentAttemptUpdateService
{
    public static async Task<PaymentAttempt?> UpdateAsync(
        UserContext context,
        long id,
        string newReferenceCode,
        bool returnNullWhenMissing = true)
    {
        var current = await Q.PaymentAttempts()
            .WithIdIs(id)
            .Comment("load current Payment Attempt and version")
            .Purpose("authorize Payment Attempt update")
            .ExecuteForOneAsync(context);
        if (current is null)
        {
            if (returnNullWhenMissing) return null;
            throw new InvalidOperationException("Payment Attempt not found");
        }

        current.UpdateReferenceCode(newReferenceCode);

        await current
            .AuditAs("business reason for updating Payment Attempt")
            .SaveAsync(context);
        return current;
    }
}
```

The generated source returns null for not-found by default and can throw a
distinct not-found exception when requested. Add a stale-version test and prove it is rejected. Execution and save each
accept exactly one `UserContext` argument.


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

Capability: `update`.

- Load the tenant-scoped current entity first so its original version participates
  in optimistic locking; do not reconstruct versioned state from untrusted JSON.
- Allow-list writable fields, attach the generated audit-reason API, and save with
  the same UserContext. Add a stale-version rejection test.
