# .NET runtime conformance example

This retained SQLite example is generated from `model.xml` and verifies explicit
`EnsureSchemaAsync`, generated typed root bootstrap, Create, Update, Delete,
typed Q/SmartList, E loaded/null/not-loaded semantics, and Checker rejection
before SQL. The provider owns DDL only; generated bootstrap mutations use the
same Checker/Fix, audit, mutation-ledger and Save path as application entities.
It also verifies that a merged mutation ledger keeps optimistic versions
isolated for different entity types that share the same numeric ID.

```bash
dotnet build
dotnet run
```
