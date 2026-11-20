# Order Management — .NET + SQLite

`model.xml` owns the exact `dotnet-lib-core` and `dotnet-app-console` scopes.
The original business program is byte-identical and consumes the named sibling
library. Its trusted business clock, generated queries and audited saves run
inside the workspace. `showcase.json` records current provenance. Run
`bash verify.sh` from this scene for two starts on one retained SQLite file,
source-project binding checks and independent stored-order calibration. Owned
compilation scratch is removed after compact evidence is recorded.

No database server, fixture database, model input, or generator installation is needed.

```bash
cd examples/order-management/dotnet-app-console
dotnet run -p:UseSharedCompilation=false
```

The first run creates `../.local/order.db`, grows its schema from generated metadata, seeds through generated entities, runs a governed query, and saves an audited preset. The second run is idempotent.

Read `dotnet-app-console/Program.cs` first. Discover generated APIs through current model-aware Assist. Runtime dependencies live in the trusted `UserContext`; execute methods accept only that context.

## Verify the first result

Expect `WEB-2026-001`, `2026-08-12`, and decimal `129.95`. Both runs execute the generated governance path; the second reuses the existing order and preset.

## Customize it

Change the generated filter or ordering in `Program.cs`, then rebuild with shared Roslyn compilation disabled as shown above. Request current model-aware Assist for signatures. Keep custom policy and audit sinks in `dotnet-app-console`; regenerate `dotnet-lib-core`. The shared model is generation provenance, not a runtime dependency.
### Materialized-list hard limit

`ExecuteForListAsync` protects the service by applying a default hard limit of 10,000 rows. A requested page size above that ceiling fails explicitly. Trusted application code can call `HardLimit(...)` to override the outer-query ceiling. **Caution:** most applications should not override it; do so only for a reviewed, exceptional requirement. This setting does not describe streaming execution.

### Streaming large root queries

`ExecuteForStreamAsync` returns generated entities through `IAsyncEnumerable<T>` and propagates cancellation to `DbDataReader.ReadAsync`:

```csharp
await foreach (var order in request.Comment("export orders").Purpose("reviewed export")
    .ExecuteForStreamAsync(ctx, 500, cancellationToken))
{
    await WriteOrderAsync(order, cancellationToken);
}
```

The chunk size is the provider fetch bound. **Caution:** normally keep the default 1,000. Streaming relation or aggregate enhancement is rejected; use a root query or `ExecuteForListAsync`. Ordinary federation requires a dedicated streaming protocol.
