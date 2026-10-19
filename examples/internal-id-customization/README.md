# Context-owned internal ID allocation

This focused SQLite example installs an application-owned `IInternalIdAllocator`
through `UserContext.WithInternalIdAllocator`. It checks direct inserts, batch
inserts, explicit-ID floor reservation, and generated-graph allocation. The
query/mutation provider remains independent of the ID allocator.

```bash
dotnet run --project examples/internal-id-customization/internal-id-customization.csproj
```

The in-memory allocator is intentionally only a demonstration. A production
implementation must allocate unique IDs and persist the per-entity floor
across processes. It may leave gaps after failures or rollback. The companion
`InternalIdAllocatorSqliteTests` also verifies that a custom allocator failure
does not silently fall back to the provider's ID space.

This proves local-source behavior; it does not claim a public NuGet release.
