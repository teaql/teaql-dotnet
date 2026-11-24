# Register a school through a custom schema executor

The district installs an application-owned Schema decorator separately from
the SQLite query and mutation provider. The decorator receives both modeled
entities on each of two ensures. The application registers a school through
generated Q and audited save, then reads it back. An injected School schema
failure is observed at the decorator while the saved school remains readable.

- `model.xml` owns the domain model.
- `dotnet-lib-core/` holds the generated domain library.
- `dotnet-app-console/Program.cs` runs the business scenario.
- `dotnet-app-console/calibration/` retains the original runtime probe.

The SDK inventory runs both entry points. The focused Conformance gate runs
the console twice against one fresh SQLite database and independently checks
the persisted records.
