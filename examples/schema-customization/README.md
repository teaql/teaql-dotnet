# Context-owned Schema SPI

This focused example replaces schema execution with a typed decorator while
keeping SQLite query/mutation execution as a separate data service. The public
application entry point remains `context.EnsureSchemaAsync()`; no generated
domain-library source or direct provider schema call is needed.

```bash
dotnet run --project examples/schema-customization/schema-customization.csproj
```

It runs `EnsureSchemaAsync` twice without deleting the in-memory database, then
executes a bounded query through the independently installed data service.
The companion `SchemaCompositionTests` cover both installation orders,
unsupported-provider replacement, and failure before generated bootstrap.
This proves the local-source extension cut, not a public NuGet release.
