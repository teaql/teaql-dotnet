# Context-derived fields and business intent

This small two-entity SQLite example uses an unedited generated domain library,
typed Q/E APIs and audited graph saves against this repository's runtime source.
It is a local source qualification, not a published-package or clean-room test.

```sh
node examples/context-derived/verify-local.cjs
```

The verifier checks the complete generated fixture inventory and hashes, copies
it into an owned disposable compiler workspace, proves project dependencies from
the built `.deps.json`, and runs the same binary twice on one SQLite database.
It reaps its command groups and removes compiler output, retaining compact logs,
receipt and the database. Shared NuGet caches and services are never removed.
`TEAQL_CONTEXT_FIX_EVIDENCE_DIR` can select a new absolute evidence directory.
Set `TEAQL_CONTEXT_FIX_UNIT_TESTS=1` to run all solution test projects in the
same owned compiler workspace before building and executing the consumer.

The example proves:

- Changing the Business Clock, IP or actor alone causes no provider call, version
  bump or audit. Restoring a failed business edit remains a no-op.
- When business Fix normalizes the last edit to its known persisted value,
  automatic Context values are withdrawn without repeating Fix. The native
  entity and ledger agree, including mixed graphs with a genuinely dirty sibling.
- Create defaults and update defaults use the captured Business Clock; create
  fields are preserved on update. Generated `uniStr` runs once, not per phase.
- Provider errors have a safe field location without raw exception causes.
- Later-child validation yields `/entryList/1/name`, performs no writes, and
  removes earlier siblings' automatic ledger changes without losing caller edits.
- A corrected retry changes only the dirty child, not the parent or sibling.
- An independent read-only SQLite oracle agrees with typed E observations.

`born_at`, `touched_at` and `guarded_token` are generated Context metadata.
The IP/actor strings demonstrate application-owned metadata/Checker SPI, not new
KSML built-in types. `RuntimeModule.ContextField` declares those fields;
`ContextFix.Apply` performs synchronous strongly typed assignments through a
trusted `IContextFixResolver`. Prepare HTTP/async results before invoking save.
Do not invent eligibility flags in JSON or store a mutation ledger on Context.

The application decorates the actual generated Checker through an observation
seam; it neither edits generated source nor substitutes manual mutation SQL.
This fixture reflects the candidate generator's API surface; it does not claim
additional managed-factory APIs from other generator branches.

## Current business model and workspace

`model.xml` defines Catalog and Entry. Trusted Context supplies collection/edit
times, IP and actor fields; the example shows why saving an unchanged entry
must leave those business values untouched. `dotnet-lib-core/` is the exact, read-only
current generated domain library. The application is in `dotnet-app-console/showcase/Program.cs`.
The historical `fixture/` remains byte-identical and retains all of its original
fingerprint checks; a separate guard verifies the current library before and
after business execution.

Run the focused source qualifier entry from the SDK repository root:

```bash
node examples/context-derived/verify-local.cjs
```

The original assertions run twice on retained stores: no-op saves produce no
updates, genuine edits consume the trusted Context, failed validation/provider
work preserves edits, and a later invalid child rolls back staged graph fields.
Current managed creation results are explicitly awaited or checked. No generated
implementation is hand-edited. Compact receipts and result stores are retained;
owned build output is removed. Full runtime inventories remain separate gates.
