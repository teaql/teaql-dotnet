# School Management bootstrap example

This generated console example retains the shared `models/school-model.xml`
fixture. `dotnet run` verifies explicit SQLite schema creation, Platform and
SchoolType seed creation, repeated-seed idempotency, and versioned reconciliation
of a changed constant.

Bootstrap data is created and reconciled by generated typed entities through
`AuditAs(...).SaveAsync(context)`. The schema provider performs DDL only; it does
not interpret constants or issue bootstrap INSERT/UPDATE statements itself.

The application-owned `BootstrapEvidence` observes actual generated bootstrap,
without adding expected TraceNodes to requests. It verifies required lookup and
readback intent, canonical SQL paths, matching committed audit lineage, fixed
root/constant identities, actor/category restoration, and audited correction of
a modified constant. SQL logging can be disabled without suppressing audits.
At audit delivery, a separate read-only SQLite connection must already see the
reported version. This checks commit timing rather than treating a callback as
proof of durability. SQL and audit prose are checked under their respective
privacy projections; their typed entity identities and lineage must agree.

Run the same database twice, independently for logging on/off:

```sh
bash scripts/verify-school-bootstrap-example.sh
```

The script uses repository-local runtime projects, retains both SQLite files
and logs, and checks generated-library hashes. The ordinary full School example
still uses a fresh isolated database and exercises Q/relations/mutation as before.
