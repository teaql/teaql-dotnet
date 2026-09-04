# School Management bootstrap example

This generated console example retains the shared `models/school-model.xml`
fixture. `dotnet run` verifies explicit SQLite schema creation, Platform and
SchoolType seed creation, repeated-seed idempotency, and versioned reconciliation
of a changed constant.

Bootstrap data is created and reconciled by generated typed entities through
`AuditAs(...).SaveAsync(context)`. The schema provider performs DDL only; it does
not interpret constants or issue bootstrap INSERT/UPDATE statements itself.
