# TeaQL .NET SDK

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
