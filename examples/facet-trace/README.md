# Generated Facet Trace Chain acceptance

Run `bash examples/facet-trace/verify.sh`. This local-source example builds
against repository `src`, runs twice on one retained fresh SQLite file without
cleanup, and fingerprints all 11 generated files. It is included in the
all-examples gate. Logs and the database are retained in the printed directory.

The three-object School model isolates `TC-SQL-09`: root Facets, nested Facets,
and nested Facets within an already-loaded reverse relation. Twenty-four
combinations exercise include-all/matched-only, empty results, logging on/off,
limit-one display with full matching counts, and typed empty carriers. Real SQL
counts, ordered logical ancestor routes, original request intent, future-binding
masking and independent next-query NotLoaded state are asserted. No-op parent
saves must emit zero writes/audits; a final real child change saved through the
root must persist its version and exactly one audit.

`FACET_OBSERVED` records actual results and diagnostics. `lib/` is read-only
generated output retained from producer `190205adec4e3d6a01c0036b0b8e48f9fb25babe`
(runtime Trace Chain/Facet issues and producer #251). Do not inspect generated
source for API discovery. Use current action/entity/field Assist with `model.xml`
for new operations; report `MISSING_ASSIST` rather than guessing.

Downloaded-package replay belongs in a separate workspace: pass a pinned
`TeaQLRuntimeVersion`, leave `TeaQLRuntimeSourceRoot` empty, and verify the actual
loaded assemblies against the fixed NuGet artifact hashes. This example does
not prove all Trace Chain cases, external providers or public-release parity.
