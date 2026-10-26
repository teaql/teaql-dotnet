# Generated scalar fixture: exact SQLite values and typed entity JSON

Run from the repository root:

```bash
bash examples/scalar-type-probe/verify.sh
```

The example uses this checkout's runtime projects by default, not a published
TeaQL package. It runs twice on one unique SQLite file without deleting it.
Both runs ensure schema twice, retain one generated root, exercise the generated
Q/E APIs, verify checker rejection before SQL, and perform an audited mutation.
The three persisted numbers `2`, `10`, and `123456789012.3456789` test numeric
ordering, exact equality, SUM/MIN/MAX and cursor hydration. An optional Decimal
alternates between `0.10` and null while its version advances. The small fixture
also carries the existing scalar types, including nullable typed Time.

The 10 files under `generated/` are byte-exact evaluated output, scope `dotnet-lib-core` and
canonical `scalar_type_probe.xml`. The model and generated-file hashes are checked before
and after every run. No generator service is needed to run the example; change
the model and regenerate if that library needs to change. `Program.cs` and the
outer project are application-owned. The retained generated project is evidence;
the outer project explicitly compiles the generated sources against this repo.
`generated-provenance.json` records the exact producer source, receipt and model
hashes. Regeneration is mechanical; application code does not edit the library.
The latest refresh changes only `GeneratedRuntimeModule.cs`: modeled Integer fields
carry explicit 32-bit range metadata, without narrowing Long, ID, version or FK.

## Exact integral admission and corrupt-read defense

`IntegralRoundtrip.cs` runs forty cases per round through the generated Q/E and
audited mutation APIs on a separate `<scalar database>.integer.sqlite` file.
Thirty-two invalid create/update Fix outputs and four native Integer overflows
must fail before query, mutation, transaction or ID allocation. Four deliberate
legacy-row corruptions must fail during Q readback with a field-specific,
value-free error; the fixture restores those fields in `finally`.

Four endpoint records retain exact Integer/Long values, loaded null and version
one through Q, E and typed JSON, including `9007199254740993`. Each run also saves
one fresh calibration entity through the generated API, so all provider counters
are proven live even on the second run. Two runs therefore leave six rows, not
four, in this dedicated database. This does not reset or alter the Decimal or
typed-JSON databases. Native SQLite tests additionally cover stream rejection,
empty aggregates, sparse projections and exact scaled-Decimal integer bindings.
`DOTNET_INTEGRAL` records the complete case list and calibrated observations.

## Typed entity JSON

`JsonRoundtrip.cs` uses generated `ToJson()` / `FromJson(payload)` and typed
Q/E/Mutation APIs, not a handwritten DTO or reflection. The runtime owns the
codec; the generated library supplies selected-field access and immutable
model-specific type/name metadata. It runs twice on a separate retained
`<scalar database>.json.sqlite` file so it cannot change the three-row Decimal
example's assertions.

Each run checks all 21 business scalar fields, ID/version, FK and managed
timestamps in camelCase, snake_case and PascalCase. Long/ID/version/Decimal use
exact JSON strings; Integer remains a number. Decimal scale is culture-neutral,
Time preserves .NET ticks within a day, and DateTime uses UTC milliseconds.
Unsupported precision, nonfinite numbers, unknown/private fields and ambiguous
duplicates are rejected rather than rounded or silently coerced.

Omitted fields remain NotLoaded, explicit null remains loaded null, and nested
forward/reverse trees retain sparse projections without provider calls. A known
FK survives a filtered target detail, whose missing name remains NotLoaded.
A null FK is a known null target. Cycles fail explicitly instead of overflowing
or performing implicit database reads.

A modified partial JSON entity is rejected before query, mutation, transaction
begin or ID allocation. Two decoded full copies retain independent ledgers:
the first audited save succeeds; the second stale save is rejected and cannot
clear pending changes or overwrite committed state. The generic stale-save
diagnostic is not claimed as a complete conflict-error taxonomy.

Compact `entity-json-*.report.json` files retain the assertions and calibrated
provider counters. Entity JSON does not include query/facet envelope metadata
and does not replace permissions or the context-bound reference adapter at an
external boundary. Public runtime/package versions are unchanged.

SQLite Decimal columns use `TEAQL_DECIMAL_TEXT COLLATE TEAQL_DECIMAL` plus a
connection-local validation CHECK. SQLite comparisons and ordering remain numeric.
SUM uses an exact integer coefficient, permits intermediate cancellation, and
rejects an unrepresentable final result rather than rounding it. Named aggregate
results use compiler metadata; an ordinary string is never decoded by a prefix.
AVG/statistical Decimal aggregates require an explicit precision policy and fail
closed. Raw SQL remains caller-owned.

Existing NUMERIC/REAL or incomplete Decimal TEXT columns require explicit
migration before bootstrap. Previously rounded values cannot be reconstructed
automatically. The example proves SQLite behavior, not all database providers or
wire formats. No public version is changed or released by this verification.

The verifier removes its disposable build outputs on exit. It retains compact
logs and the database in the printed evidence directory. Set `TEAQL_SCALAR_DB`
to an explicit, initially unused path to retain that database elsewhere.
`TeaQLUseLocalPackages=true` is an opt-in for an isolated package-only
consumer supplied with a local NuGet configuration; ordinary example runs do
not use this mode.
