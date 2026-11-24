# order-federation-service ASP.NET agent instructions

## TeaQL workspace rules

- Work in application-owned files. Keep the generated domain library read-only.
- Discover API names and signatures through model-aware Assist only.
  Do not read or search generated library source for API discovery. Use the
  original evaluated KSML file or directory and its exact entity and field names.
- Ask for the current entity and action first. Request field-level query help
  only for the fields you use. If Assist lacks the operation, report
  `MISSING_ASSIST` with the entity, action, and relevant field or diagnostic.

```text
cargo teaql --input [model] dotnet-assist-[action]/[entity]
cargo teaql --input [model] dotnet-assist-query/[entity].[field]
```

Replace the bracketed values with the actual model path, action, entity, and
field. Actions include `query`, `list-page`, `create`, `update`, `delete`, and
`expression`. Validate API usage by compiling and testing the application.

- Use the trusted `UserContext` as the only runtime argument for generated query
  execution and saves. Resolve tenant, actor, authorization, and provider
  resources from trusted application setup.
- Keep queries bounded and provide a non-empty purpose and comment. Supply a
  non-empty audit reason for writes through the API shown by Assist.

## Application entry and context

Read `README.md` for startup and the `/health` check. Define endpoints in `Program.cs`.

Initialize the request's `UserContext` with server-side database, audit, identity,
and authorization resources in middleware. Handlers obtain it from
`HttpContext.Items["TeaQL.UserContext"]`. Allowlist business fields in request JSON.

## Optional standard telemetry

See [STANDARD_TELEMETRY.md](STANDARD_TELEMETRY.md) for the selected runtime's optional module, trusted activation variables and safe startup diagnostics.
