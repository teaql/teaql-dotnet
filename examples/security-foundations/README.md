# Security foundations example

This executable references the runtime projects in this repository directly
and fails closed if any of these backend security contracts regress:

1. the ordinary default-on SQL log is parameterized and value-free, while an
   explicitly installed sensitive sink receives copy-paste SQL;
2. the trusted TFP endpoint rejects tenant/raw-SQL injection and carries the
   trusted tenant, ID, and optimistic version into one mutation statement; and
3. a `UserContext` opaque entity reference matches the shared Go/.NET
   AES-256-GCM golden vector and rejects a wrong purpose; and
4. a fixed, context-owned Business Clock drives both business time and business
   date without changing any process-global clock; and
5. Query Policy governs a cloned root-and-relation query graph exactly once,
   leaving the caller-owned request reusable; and
6. a default opaque-reference codec and the local cache keep operational time
   separate from future, unreadable and backdated Business Clocks. The cache
   actually expires, a loaded empty array remains a hit, and a business-time
   rewind cannot revive it. The structured clock marker contains no token,
   internal identity, key material or cache key.

Run it from the repository root:

```bash
dotnet run --project examples/security-foundations/security-foundations.csproj
```

This example does not enable the development-only raw-ID escape hatch and does
not replace authentication or application authorization.
