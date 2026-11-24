# School startup and query showcase

`models/school-model.xml` owns the exact generated `dotnet-lib-core` and
`dotnet-app-console` scopes. The workspace retains the original request-intent,
School creation/query and bootstrap evidence consumers. `showcase.json` is the
current provenance declaration; the root fixture snapshot remains historical.
Run `bash verify.sh` from this scene. It builds against current source projects
in disposable artifact storage, runs the full business application, checks its
actual SQLite rows, and runs bootstrap twice per logging mode on the same files.
A retained full-business database is never silently overwritten or deleted.

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
