# Order Management — .NET + SQLite

No database server, fixture database, model input, or generator installation is needed.

```bash
cd examples/order-management/dotnet-app-console
dotnet run -p:UseSharedCompilation=false
```

The first run creates `../.local/order.db`, grows its schema from generated metadata, seeds through generated entities, runs a governed query, and saves an audited preset. The second run is idempotent.

Read `dotnet-app-console/Program.cs` first (handwritten), then `dotnet-lib-core/Q.cs`, `Requests/CustomerOrderRequest.cs`, and `Models/CustomerOrder.cs` (generated). Runtime dependencies live in the trusted `UserContext`; execute methods accept only that context.

## Verify the first result

Expect `WEB-2026-001`, `2026-08-12`, and decimal `129.95`. The first run reports four immutable and four application-safe audit events; the second reports no new mutations.

## Customize it

Change the generated filter or ordering in `Program.cs`, then rebuild with shared Roslyn compilation disabled as shown above. Read request signatures rather than guessing. Keep custom policy and audit sinks in `dotnet-app-console`; regenerate `dotnet-lib-core`. The shared model is generation provenance, not a runtime dependency.
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

### Optional continuous browsing optimization

For a browse-only screen ordered by the unique `id`, trusted application code can opt in:

```csharp
var orders = await Q.CustomerOrders()
    .OrderByIdDescending()
    .Offset(page * pageSize)
    .Limit(pageSize)
    .OptimizeForContinuousPageFetchWith("recent-orders", 60)
    .Purpose("browse recent orders")
    .Comment("order browser")
    .ExecuteForListAsync(ctx);
```

The runtime remembers a bounded, expiring cursor in `UserContext`. A matching next page
transparently uses an `id` seek instead of a deep offset; a cache miss, unsupported query
shape, or unavailable store retains correct offset behavior. The selected plan and cursor
ID remain observable for diagnosis.

**Caution:** this is an explicitly approximate optimization for continuous browsing, not
business logic, reconciliation, export, or a stable snapshot. Browse screens normally do
not need an exact count. The option is local runtime metadata excluded from JSON, so the
federation protocol cannot enable or modify it.
