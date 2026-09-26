# Runtime logging example

This executable references the runtime projects in this repository directly.
It proves the default-on query and mutation logs, structured intent, safe SQL
diagnostics, result metrics, and relation trace without relying on a published
NuGet package or an embedded generated runtime. Parameter values are emitted
only through the explicitly configured sensitive diagnostic sink; see
`../security-foundations` for the complete security example.

```bash
dotnet run --project examples/runtime-logging/runtime-logging.csproj
```
