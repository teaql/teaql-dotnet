# School Management bootstrap example

This generated console example retains the shared `models/school-model.xml`
fixture. `dotnet run` verifies explicit SQLite schema creation, Platform and
SchoolType seed creation, repeated-seed idempotency, and versioned reconciliation
of a changed constant.

The generated workspace currently carries its compact TeaQL runtime support in
`TeaQLCore.cs`; the runtime repository separately retains the same executable
bootstrap contract as an SQLite integration test.
