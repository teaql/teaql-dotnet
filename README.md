# TeaQL .NET SDK

## Trace Chain on the feature branch

`feature/request-trace-chain` requires a request-owned non-blank Query Comment
and Purpose, and a non-blank Mutation Comment before policy/provider access.
Logging switches do not relax validation. This is a local-source checkpoint,
not a newly published package capability.

The [School bootstrap example](examples/school-management/README.md) also
checks generated root/constant saves without injected trace nodes: physical
SQL routes, assigned audit identities, idempotence, audited reconciliation and
caller restoration. Its script runs twice on each retained SQLite database with
SQL logs on/off. At audit delivery, an independent read-only connection must
already observe the committed version. See `scripts/verify-school-bootstrap-example.sh`.

The SQL path uses the Rust-baseline canonical algorithm: an operation names
the originating entity, relation names are local, qualified properties belong
in `TraceNode.Detail`, and provider/SQL nodes occur once. Intent stays in its
structured metadata fields, not duplicated in every physical frame. Query
snapshots privately preserve their origin across relation and aggregate
derivation; caller-supplied diagnostic TraceChain frames cannot forge provenance.

Related aggregation consumes scalar membership keys before forward-reference
hydration. Nested attachment captures original scalar keys before hydration,
including targets keyed by `code` rather than `id`. A reference filtered to null
does not detach its child from the parent list or prevent another sibling from
using that FK. These per-load keys never become result fields or ledger entries. Native
SQLite regression cases cover root/nested count results, original trace ancestry,
descendant-binding privacy, logging disabled and aggregate failure recovery.
Generated models now retain related-count aliases in a runtime-owned
`QueryProjectionSnapshot`: `QueryProjection(alias)` returns an isolated `Value`,
`HasQueryProjection(alias)` distinguishes presence, and missing throws
`KeyNotFoundException`. Projections are not modeled fields or ledger mutations.
Regenerate libraries to acquire the API. The trace example exercises generated
root/nested Q/E/save, private descendant intent, logging on/off and count failures.
Broader aggregate/Facet, provider and immutable-artifact acceptance remain open.

`ExecutionMetadata.MutationLineage` separately preserves existing root/item
reasons, including partial batch failures and mutation readback. Default masking
projects both that lineage and physical paths before sinks; execution values
and original command trace lists are not changed. Frozen shared vectors and
real SQLite root/three-level failure tests are separate evidence.

Graph saves explicitly receive an operation-owned `GraphMutationSession`.
Persistent immutable parent scopes retain sibling and deletion reasons; typed
ledger traces are complete per-entity replacements. The generated adapter
allocates IDs through the transaction before constructing a scope. Context does
not hold a trace stack or implicitly join nested saves. Untagged active-graph
mutations, expired capabilities and borrowed parent scopes fail closed.

The graph transaction queues safe application audit until commit and discards it
on rollback. Post-commit errors are `GraphCommittedException` (`Committed=true`);
remaining cleanup/audits still run and the committed transaction is not rolled
back. Whole-graph preflight supplies invocation-local sibling redaction values.
The native SQLite tests exercise allocation, deletion, typed ledger replacement,
real UNIQUE rollback and two concurrent requests serialized on one Context.
Shared helper vectors remain separate from that execution proof.

Regenerate dependent libraries: the adapter callback is now
`ExecuteGraphSaveAsync(comment, async graph => ...)`, with `graph.Preflight`,
`graph.MutateAsync` and graph-owned completion callbacks. Public generated
`.AuditAs(...).SaveAsync(context)` is unchanged. School, Conformance and Order
libraries are regenerated from their retained models, not patched manually.

The [generated six-entity example](examples/trace-chain) observes actual provider
commands, safe SQL and committed audit through generated Q/E/Mutation APIs.
Run `bash scripts/verify-trace-chain-example.sh` for two executions on the same
database without cleanup and an unchanged library. It covers branch/deletion
reasons, assigned typed IDs, a three-level query, real UNIQUE rollback,
successful-write/readback-failure separation and concurrent generated saves.
The read-only provider SPI `MutationRequest.MutationLineage` is not a wire field.
Successful relation SQL is now logged at each physical execution boundary;
parent/descendant binding provenance protects intent before parent logs are
emitted, and root statements are not reported twice.

Prepared batch/item-index lineage, detached-child and late low-level allocation, complete
entry-point/privacy coverage and internal Registry replay remain open. A direct
legacy transaction wrapper is not proof of commit-bound audit. Run
`dotnet test TeaQL.sln`, the separate `TeaQL.Core.Tests` project and
`bash scripts/verify-examples.sh` against this checkout before promoting an
internal candidate.

## Sensitive log data

Runtime diagnostic logs redact payload values by default, before delivery to
file, console, buffers, or custom logging sinks. Selecting a diagnostic sink
alone does not authorize plaintext. For controlled troubleshooting only:

```bash
export TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS=I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK
```

Only this exact value enables plaintext permission; empty values, `true`, and
whitespace variants do not. Enabling it emits a warning. Credential-classified
fields remain redacted. The flag does not force every sink to expose values.
SQL without reliable field/literal provenance may be suppressed and marked
`NOT REPLAYABLE`. Execution parameters and persisted business data are unchanged.

Do not put sensitive data in free-text comments or purpose declarations.
TeaQL cannot govern arbitrary application prints or independent driver loggers;
configure those separately. This setting does not erase older plaintext files.
Restrict access and retention when using plaintext diagnostics, then unset the
variable and restart processes when troubleshooting is complete.

TeaQL .NET SDK is the C#/.NET implementation of the TeaQL framework, designed to bring the highly abstract, efficient, and robust data modeling and SQL execution engine to the modern .NET ecosystem.

## Recommended Agent Harness

When building database-backed applications with the TeaQL .NET runtime, we
recommend using it together with the [TeaQL Agent Kit](https://github.com/teaql/teaql-agent-kit).
The Agent Kit is TeaQL's continuously evolving **Harness Engineering** method.
It gives coding agents a model-mediated, executable workflow for domain
modeling, deterministic evaluation and repair, code generation, implementation,
and evidence-based verification as the generator and runtimes evolve.

## 1. Minimum Version Requirements

*   **.NET SDK**: .NET 8.0+ (C# 12+)
*   *(Optional)* **Third-party Services**: Redis (For CacheIntegration Module), Sqlite/PostgreSQL/MySQL (For Data Providers)

## 2. Tests Performed

This project incorporates a comprehensive unit testing suite, having successfully completed the following verifications:
*   ✅ **`TeaQL.Core` Tests**: Includes `ValueTests` (base types and nullability evaluation), `EntityGraphTests` (creation and deletion of nodes and relationships), `SelectQueryTests` (query conditions and AST composition), and `SafeExpressionTests` & `EvalTests` (expression evaluation and safe execution).
*   ✅ **Entity Abstraction & Metadata Tests**: Validated metadata behaviors via `DescriptorsTests` and `TimestampTests`.
*   ✅ **Fundamental Data Structure Tests**: Tested boundary conditions for custom data structures like `SmartListTests` and `TrimmedStringConverterTests`.
*   ✅ **Cross-Language API Parity**: API signatures are heavily inspired by best practices from the Rust, Golang, and Python equivalents, specifically adapted and aligned for .NET features.

## 3. Available Modules

To ensure high extensibility and dependency isolation, this project adopts a multi-project architecture:
*   **`TeaQL.Core`**: Foundational core structures (Entity Metadata, `Value`, AST node abstractions, etc.).
*   **`TeaQL.DataService`**: Platform-agnostic data service contract abstraction layer (e.g., `QueryRequest`, `QueryResult`).
*   **`TeaQL.Sql`**: SQL compilation and execution engine (`SqlDialect`, `SqlDataServiceExecutor`).
*   **`TeaQL.Runtime`**: Application runtime context handling (`UserContext`, `RuntimeModule`).
*   **`TeaQL.Provider.*`**: Physical transport implementation modules for various relational databases (`Sqlite`, `PostgreSql`, `MySql`).
*   **`TeaQL.CacheIntegration.Redis`**: Transparent distributed cache provider extension.
*   **`TeaQL.WebIntegration.AspNetCore`**: Seamless web interface integration and endpoint mounting middleware tailored for ASP.NET Core environments.

## 4. Features

*   **Core Architecture**: Provides a strong-typing system mapping mechanism based on the `Value` wrapper type, completely eliminating boxing/unboxing overheads and cross-database NULL handling issues, alongside a robust Entity Descriptor modeling system.
*   **SQL Dialect Generator**: Highly secure SQL AST construction that dynamically translates into native parameterized SQL queries/commands for Sqlite, Postgres, and MySQL, inherently preventing SQL injection.
*   **Unified Runtime Context**: A centralized `UserContext` runtime that natively supports chained storage propagation and dependency injection, ensuring environment variables seamlessly pass through various services alongside the request.
*   **Governed Mutation Policy**: Customer policy reviews one immutable whole-graph mutation plan before the first provider write. Exact policy approval, warning codes, operation summaries, and policy identity are retained in application-audit evidence; policy denial is fail-closed while warning delivery is fail-open.
*   **Governed Business ID Lifecycle**: Core model contracts, a context-owned profile/key/service boundary, retry-safe assignment, an in-memory allocator, and explicit-schema durable SQLite allocation extend the portable `daily-permuted-v1` encoder without exposing its internal sequence.
*   **ASP.NET Core Web Endpoint**: Integrates instantly with `Microsoft.AspNetCore.Builder`, exposing underlying abstract data services as RESTful endpoints with just a few lines of code.
*   **Redis Cache Decorator**: The `RedisDataServiceDecorator` enables transparent, underlying distributed caching for data interactions out-of-the-box.

## Security Foundations

TeaQL .NET provides the backend security profile used by generated services:

- ordinary Query and Mutation logs retain intent, trace, parameterized SQL,
  timing, and outcome without values;
- value-bearing/copy-paste SQL requires an explicitly selected sensitive sink;
- the TFP endpoint applies bounded requests, trusted tenant policy,
  writable-field rules, and optimistic version at the provider boundary;
- `UserContext` issues short-lived opaque entity references rather than
  exposing raw internal ID/version pairs.

```csharp
var codec = new AeadEntityReferenceCodec(2, new Dictionary<uint, byte[]>
{
    [2] = activeKeyFromSecretManager,
});
var context = new UserContext().WithEntityReferenceCodec(codec);
var token = context.EncodeEntityReference(
    "OrderItem", 42, 7, "edit-order", TimeSpan.FromMinutes(15));
var claims = context.DecodeEntityReference(token, "OrderItem", "edit-order");
```

The AES-256-GCM envelope supports rotation, expiry, entity-type binding, and
purpose binding. Verification uses stable non-disclosing errors and missing key
infrastructure fails closed. The shared Java/Rust/Go/.NET golden vector and the
exact development-only raw-reference acknowledgement are defined in the
canonical [opaque entity reference contract](https://github.com/teaql/teaql-conformance/blob/main/design/opaque-entity-references.md).
Opaque references do not replace tenant, ownership, role, or optimistic-lock
policy.

## Business ID V1 Foundation

The runtime exposes the deterministic six-character permutation used by the
default volume-obscuring Business ID profile:

```csharp
var scope = new BusinessIdScope(
    "tenant-a", "commerce_order", "order_number", "20260925");
var key = new BusinessIdEncodingKey(1, keyFromSecretManager);
var code = BusinessIdPermutationV1.Encode(0, scope, key);
```

The retained [`examples/business-id`](examples/business-id) flow proves
explicit schema installation, durable concurrent allocation infrastructure and
command-retry reuse. Generated strongly typed fields and external lookup remain
a separate generator capability. Keys belong to an application-owned secret
provider and never to KSML or generated source.

## Quick Start

### Local dynamic-search schema drift

`TeaQL.Core.DynamicSearch.Normalize` validates a local UI search envelope such as
`{"filter":{"name":{"$contains":"Campus"}},"orderBy":[{"field":"id","direction":"desc"}]}`
against application-owned `SearchModel` metadata. Unknown fields or relation
paths remove the **whole clause** and return `DYNAMIC_SEARCH_UNKNOWN_FIELD`
warnings (entity, clause and field path, never the submitted value). Warnings
also go to stderr by default; pass a callback to integrate structured logging.

`DynamicSearch.Merge` accepts an already-scoped `SelectQuery` and trusted
filter/order bindings that produce native `Expr`/`OrderBy` objects. It clones the
query, ANDs filters and appends ordering, retaining existing filters, hard limit,
pagination and intent. Bindings must enforce related-query authorization too.
Warnings are emitted only after all validation and bindings succeed.

Supported scalar metadata types are `string`, `integer`, `number`, `boolean`,
`date` (`yyyy-MM-dd`), `timestamp` (integer epoch milliseconds), and `decimal`
(use a string for exact decimal digits). Operators are `$eq`, `$ne`, `$gt`,
`$gte`, `$lt`, `$lte`, `$in`, `$notIn`, and string `$contains`. The default limit
is 100 filter/order clauses, 16 path segments and 1,000 IN-list values.
Invalid operators/types, malformed JSON and client-supplied trusted controls
remain errors. TFP validation is unchanged. Generated automatic bindings are
not supplied by this adapter; do not use it as an authorization policy.

Regression evidence lives in `DynamicSearchTests` and
`SqliteTransportTests.DynamicSearchKeepsOuterAndNestedTenantScopesWithUnknownClauses`.

### Build and test

The solution is natively built for .NET 8. You can build and test using the .NET CLI:
```bash
dotnet build TeaQL.sln
dotnet test src/TeaQL.Core.Tests/TeaQL.Core.Tests.csproj
```

Run the focused Mutation Policy example:

```bash
dotnet run --project examples/mutation-policy/mutation-policy.csproj
```
