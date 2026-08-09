# TeaQL .NET SDK

TeaQL .NET SDK is the C#/.NET implementation of the TeaQL framework, designed to bring the highly abstract, efficient, and robust data modeling and SQL execution engine to the modern .NET ecosystem.

## 1. 最小的版本需求 (Minimum Requirements)

*   **.NET SDK**: .NET 8.0+ (C# 12+)
*   *(Optional)* **Third-party Services**: Redis (For CacheIntegration Module), Sqlite/PostgreSQL/MySQL (For Data Providers)

## 2. 我们已经做过哪些测试 (Tests Performed)

本项目包含完整的单元测试验证机制，主要已完成以下测试：
*   ✅ **`TeaQL.Core` 核心测试**：包括 `ValueTests`（基础类型及空值判别）、`EntityGraphTests`（节点和关系的建立与删除）、`SelectQueryTests`（查询条件及组合 AST）、`SafeExpressionTests` 和 `EvalTests`（表达式计算与安全求值）。
*   ✅ **实体抽象及元数据测试**：如 `DescriptorsTests` 及 `TimestampTests`。
*   ✅ **基础数据结构测试**：包括 `SmartListTests` 和 `TrimmedStringConverterTests` 等自定义数据结构的边界条件。
*   ✅ **API 跨语言对比对齐**：API 签名设计参考了 Rust/Golang/Python 版的最佳实践并针对 .NET 特性进行了深度适配和对齐。

## 3. 有哪些模块 (Available Modules)

为了保证扩展性与依赖隔离，本项目采用多工程的架构：
*   **`TeaQL.Core`**: 基础核心结构（实体元数据、`Value`、AST 节点抽象等）。
*   **`TeaQL.DataService`**: 平台无关的数据服务契约抽象层 (`QueryRequest`, `QueryResult` 等)。
*   **`TeaQL.Sql`**: SQL 编译与执行引擎 (`SqlDialect`, `SqlDataServiceExecutor`)。
*   **`TeaQL.Runtime`**: 运行时应用上下文处理 (`UserContext`, `RuntimeModule`)。
*   **`TeaQL.Provider.*`**: 多家关系型数据库的物理传输实现模块（`Sqlite`, `PostgreSql`, `MySql`）。
*   **`TeaQL.CacheIntegration.Redis`**: 分布式透明缓存提供者扩展。
*   **`TeaQL.WebIntegration.AspNetCore`**: 面向 ASP.NET Core 环境的无缝 Web 接口集成及端点挂载中间件。

## 4. 里面有什么功能 (Features)

*   **Core Architecture**: 提供基于 `Value` 包装类型的强类型系统映射机制，彻底解决装箱拆箱及跨库 NULL 值处理的困扰，并提供了完整的 Entity Descriptor 建模机制。
*   **SQL Dialect Generator**: 高度安全的 SQL 抽象语法树构建，能动态翻译为针对 Sqlite、Postgres 和 MySQL 的原生带参 SQL 查询/变更命令，防止 SQL 注入。
*   **Unified Runtime Context**: 一站式的 `UserContext` 运行时，天然支持链式存储传递和依赖注入，保障环境参数随请求在各个服务间无损透传。
*   **ASP.NET Core Web Endpoint**: 可极速对接 `Microsoft.AspNetCore.Builder`，仅用数行代码便可将底层抽象数据服务暴露为 RESTful 端点。
*   **Redis Cache Decorator**: `RedisDataServiceDecorator` 允许一键开启底层数据交互的透明分布式缓存能力。

## Quick Start

The solution is natively built for .NET 8. You can build and test using the .NET CLI:
```bash
dotnet build TeaQL.sln
dotnet test src/TeaQL.Core.Tests/TeaQL.Core.Tests.csproj
```