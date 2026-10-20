# Generated scalar fixture: exact SQLite Decimal

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

The 10 files under `generated/` are unchanged output from local generator commit
`8ecff7241999b665234216ae336a4cc8254f8936`, scope `dotnet-lib-core` and canonical
`scalar_type_probe.xml`. The model and generated-file hashes are checked before
and after every run. No generator service is needed to run the example; change
the model and regenerate if that library needs to change. `Program.cs` and the
outer project are application-owned. The retained generated project is evidence;
the outer project explicitly compiles the generated sources against this repo.

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

To retain a particular database, run the project with `TEAQL_SCALAR_DB` set to an
explicit path. The script prints the directory containing both rounds and the
database. `TeaQLUseLocalPackages=true` is an opt-in for an isolated package-only
consumer supplied with a local NuGet configuration; ordinary example runs do
not use this mode.
