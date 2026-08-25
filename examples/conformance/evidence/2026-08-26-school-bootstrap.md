# School bootstrap and repeated-bootstrap evidence

Date: 2026-08-26  
Runtime revision: `7ed0e72`  
Generator endpoint: `https://api.teaql.io/latest/generate`  
Generator target: `dotnet-lib-core`  
Database: SQLite  
Tracking: [teaql-dotnet#5](https://github.com/teaql/teaql-dotnet/issues/5)

## Contract under test

Using [school-bootstrap-model.xml](../school-bootstrap-model.xml):

1. The first explicit `EnsureSchemaAsync` creates the schema, Platform id `1`, and SchoolType constants `1001` and `1002`.
2. The second explicit `EnsureSchemaAsync` is idempotent: it retains exactly one root and two constants.
3. Bootstrap reconciliation advances the SchoolType ID floor beyond `1002`.

Installing the Runtime Module must remain passive.

## Execution

The generated workspace was created with:

```bash
cargo teaql --input school-bootstrap-model.xml \
  --output /tmp/teaql-school-dotnet dotnet-lib-core
```

A fresh SQLite database was used. `GeneratedRuntimeModule.Module` was installed, `context.EnsureSchemaAsync()` was called twice, and both tables were counted after each call.

## Actual result

```text
first platform=0 constants=0
second platform=0 constants=0
```

The process exited successfully: both DDL passes completed, but no bootstrap records were written.

## Result

| Assertion | Result |
| --- | :---: |
| First explicit schema reconciliation succeeds | PASS |
| Second identical reconciliation succeeds | PASS |
| Platform id 1 is seeded | FAIL |
| SchoolType 1001/1002 are seeded | FAIL |
| Repeated bootstrap retains 1 root / 2 constants | FAIL |
| Bootstrap advances constant ID floor | NOT REACHED |

Overall: **GAP**. This is not equivalent to the separately passing explicit-ID/ID-floor unit test. Generated root and constant metadata must be added to the Runtime Module and reconciled explicitly by `EnsureSchemaAsync`.

