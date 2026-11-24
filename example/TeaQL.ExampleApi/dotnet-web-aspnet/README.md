# order-federation-service ASP.NET workspace

Run `dotnet run --project order-federation-service-web.csproj`, then request `/health`.
Initialize database, audit, identity and authorization resources in trusted middleware.
Never bind those values from dynamic request JSON.

## Optional standard telemetry

See [STANDARD_TELEMETRY.md](STANDARD_TELEMETRY.md) for the selected runtime's optional module, trusted activation variables and safe startup diagnostics.
