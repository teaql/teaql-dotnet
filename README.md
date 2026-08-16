# TeaQL .NET SDK

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
*   **ASP.NET Core Web Endpoint**: Integrates instantly with `Microsoft.AspNetCore.Builder`, exposing underlying abstract data services as RESTful endpoints with just a few lines of code.
*   **Redis Cache Decorator**: The `RedisDataServiceDecorator` enables transparent, underlying distributed caching for data interactions out-of-the-box.

## Quick Start

The solution is natively built for .NET 8. You can build and test using the .NET CLI:
```bash
dotnet build TeaQL.sln
dotnet test src/TeaQL.Core.Tests/TeaQL.Core.Tests.csproj
```
