# Runtime logging example

This executable references the runtime projects in this repository directly.
It proves the default-on query and mutation logs, structured intent, safe SQL
diagnostics, result metrics, and relation trace without relying on a published
NuGet package or an embedded generated runtime. SQL stays expanded: known
ordinary fields remain visible, `School.name` uses the shared mask algorithm,
and unknown bindings / credentials stay hidden even under debug opt-in.
The test verifies actual SQLite CRUD, rejected duplicate inserts, and independent
batch statement bindings; stored values remain unchanged. The explicit sensitive
sink requires the exact acknowledgement and labels each debug record; see
`../security-foundations` for the broader security example.

It also consumes a stream through the context-bound service, breaks early,
checks one masked `cancelled` diagnostic with the delivered row count, and
queries again using the same SQLite connection. The duplicate-insert path must
produce a `failure` diagnostic. These outcomes describe the SQL statement or
cursor, not transaction commit or overall business success.

A second batch deliberately inserts a new row and then collides with an existing
primary key. The log must retain the first statement's success and the second
statement's failure in order, hide the password, and omit the unexecuted third
statement. A subsequent query proves the automatic SQLite transaction rolled
back the first row: statement success is not a claim that data was committed.

A test-only SQLite trigger removes an inserted row before authoritative readback.
The audited mutation fails, but the log correctly reports write success/1 and
query success/0. Its inherited audit reason is still scrubbed using the write's
sensitive bindings even though the readback binds only an ID. The trigger is
removed in `finally`; subsequent queries verify no failed mutation was committed.
This probe temporarily enables the exact plaintext acknowledgement and captures
debug records only in memory. After revocation it re-emits those records to a
separate file and checks that inherited sensitive intent is gone while the SQL
and safe audit explanation remain. The prior environment and sink are restored
in `finally`. Private projection state retains only a fingerprint and an already
masked alternative, never the original write bindings.

A relation fault probe temporarily renames the child table, then loads a bounded
School/Student graph. The root's masked name appears in its comment, but the child
SQL binds only the foreign key. File and custom logs must retain the failed child
SQL and safe intent without leaking the root name. `finally` restores the table,
and a subsequent graph query verifies that no fixture rows were lost. Binding
provenance is invocation-local and is removed by the safe output projection.

```bash
dotnet run --project examples/runtime-logging/runtime-logging.csproj
```
