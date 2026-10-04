# Generated Trace Chain SQLite example

This example verifies the branch-local Trace Chain contract against the local
.NET runtime and an unchanged generated six-entity library. It is local-source
evidence, not a claim about the published 0.2.9 packages.

Run from the repository root:

```bash
bash scripts/verify-trace-chain-example.sh
```

The script runs twice on `examples/.local/trace-chain.sqlite`, never deletes the
database, uses unique run labels, and checks all generated C# and project hashes
after each run. `TEAQL_TRACE_CHAIN_DB` selects another persistent file. All runtime
dependencies resolve to this repository by default; `TeaQLRuntimeSourceRoot`
selects another local checkout.

| Scenario | Observed evidence |
| --- | --- |
| Required request intent | Blank/missing reasons fail before transactions, even with SQL logging off |
| Normative six-mutation graph | Root, ordinary item, Payment, inherited PaymentAttempt, Shipment and deleted item retain their own ordered lineage in emitted commands, safe SQL and committed audit. Each boundary must cover exactly the six `(type, ID)` identities, without duplicates. SQL identities are bound through actual observed commands, not fabricated path IDs |
| Identity guard controls | Duplicate, unknown replacement and equal-ID type collapse each fail, even when six observations remain |
| Three-level generated Q and E | PaymentAttempt → Payment → CustomerOrder → Platform loads safely; four physical statements retain the same origin and qualified relation frames; unselected E access fails |
| Actual provider failure | SQLite UNIQUE violation retains failed SQL lineage, rolls back the graph and emits no committed audit |
| Readback failure | An injected authoritative fetch failure does not erase the successful write statement; the graph transaction rolls back and emits no committed audit |
| Same Context concurrency | Two overlapping Task-based generated saves are serialized by the transaction gate while retaining independent root and branch reasons |
| Accepted/rejected generated Checker overlap | Two public saves overlap on the same Context; its transaction gate serializes actual generated checking. Missing OrderItem name causes a genuine required-field rejection and rollback, while only the valid graph emits commands, physical SQL and committed audit. Both orderings and logging modes retain independent ledgers and one shared read-only Platform record; the failed wrappers repair and save successfully |
| Shared read-only provider record | Two queries reuse the exact Platform record; generated wrappers and ledgers are separate. Overlapping root-only saves use versions 2 and 1, without updating children or Platform |
| Reached child import | Adopting one changed child imports only that typed key. Its foreign root and unselected sibling stay pending and unsaved |
| Clean ancestor | A changed child inherits the parent's reason, but the clean parent emits no SQL or audit and keeps its version |
| Conflicting loaded versions | Two actual loaded versions of one entity fail before business SQL; both pending values survive |
| Scoped page and COUNT | Four seeded graphs, one excluded by Context policy, total three, offset one and two returned roots; policy prepares once, COUNT retains removed-child masking without a fictional relation edge |
| Page/list graph ownership | Each returned root owns a ledger and its children; saving one graph leaves the other graph pending and unchanged in SQLite until explicitly saved |
| Captured scalar streams | Context scope and request intent freeze before enumeration; two real SQLite cursors overlap with separate filters, safe terminal SQL and per-root ledgers, including two roots delivered in one chunk |

Data is written through generated Mutation APIs. `context.EnsureSchemaAsync()`
provisions the root; there is no manual INSERT or seed workaround. Test-only
DDL adds a UNIQUE index. Independent allocator floors are aligned through the
trusted provider so CustomerOrder and Payment share an assigned numeric ID on
every replay; identity assertions still use the entity type and ID together.
Their original versions deliberately differ (root 2, Payment 1), and the same
save must advance them independently to 3 and 2.

The provider decorator only observes requests. It reads the runtime-owned,
immutable `MutationRequest.MutationLineage` snapshot and never injects expected
scopes or trace frames. Audit assertions run both before and after actual commit.
The transport decorator injects a readback failure, not a fabricated successful
write. Physical statement success does not imply transaction commit.

The shared-record fixture reuses the actual provider-returned Record and verifies
that its contents remain unchanged. Record is not a structurally immutable type;
the fixture deliberately uses it read-only. This does not prove arbitrary
mutable reference sharing. Ownership reflection is observation only; business
reads and writes still use generated Q/E/Mutation APIs. Emitted commands, physical
SQL metadata and committed audit are printed as `OWNERSHIP EVIDENCE` JSON.
`TEAQL_TRACE_CHAIN_SCENARIO=checker-overlap` selects the four accepted/rejected
Checker cases. Installed generated checkers are not replaced, wrapped or given
fabricated results. `CHECKER VIOLATIONS` records the actual required-field
location; `CHECKER OVERLAP EVIDENCE` separates trusted raw physical bindings from
safe SQL and committed audit. The latter never contain the private child name.
This is overlapping public calls, not simultaneous Checker callbacks. The full
verifier requires this scenario twice with unchanged generated-library hashes.
The full verifier ignores `TEAQL_TRACE_CHAIN_SCENARIO` and requires both the six
normative checks and four ownership checks, twice without database cleanup.
It also requires `PageChecks`: four physical SELECTs (root, two child probes,
COUNT), no child canary in safe SQL/comment/purpose, independent root/child
saves and persisted version checks. The model marks `order_item.name` as masked.
`TEAQL_TRACE_CHAIN_SCENARIO=page` runs this focused acceptance only; the full
verifier ignores that selector. Direct `dotnet run --project
examples/trace-chain/trace-chain.csproj -- --database /absolute/path.sqlite`
also propagates its local runtime path to the generated project reference.

Hydration records original versions without leaving temporary new keys. Graph
composition imports each explicitly reached key, not its entire foreign ledger.
Only changed nodes are checked and written; clean ancestors still create an
audit scope for changed descendants. Only post-commit cleanup may advance the
ledger's authoritative version with `AcceptCommittedVersion`; generated libraries
using the old post-commit `SetOriginalVersion` call must be regenerated.

`model.xml`, the evaluated report, current Assist and producer-generated
`AGENTS.md` are retained. Application API discovery uses those instructions and
Assist, not generated source. To regenerate from the producer checkout:

```bash
mvn -B -pl generator -am -Dtest=DotnetTraceChainExampleGenerationTest \
  -Dsurefire.failIfNoSpecifiedTests=false \
  -Dteaql.dotnet.runtime=/absolute/path/to/teaql-dotnet test
```

`AggregateChecks` additionally uses current field Assist to count related items
at the root and through a loaded Payment→CustomerOrder relation. Four root/nested
and logging-on/off combinations assert results, actual physical reads, original
ancestry and safe intent. Two injected count failures restore the next request.
The generated `QueryProjection(alias)` API returns owned query-only values;
missing throws, null/zero are present, and the snapshot is not a live calculation.
Saving a loaded model changes only description, not aggregate aliases or clean
children. `TEAQL_TRACE_CHAIN_SCENARIO=aggregate` selects this slice; the full
verifier requires it twice on the retained database with stable library hashes.

This is selected producer coverage. Prepared batch/item-index lineage, detached
children, late low-level allocation, every entry point and privacy mode, reentrant
root fail-fast behavior and immutable internal Registry replay remain open.
`StreamChecks` creates eight items through generated mutation APIs, captures two
streams with different Context scopes, changes the caller builder/policy, then
holds both cursors open before completing them. Five and three rows retain their
captured scope. Saving one streamed item leaves another pending and unchanged
until explicitly saved. Missing comment rejects at stream creation even with
logging disabled; never-polled streams produce no SQL. Cancellation through the
enumerator token closes the cursor and retains masked terminal intent (the
generated cancellation probe uses chunk size one). Run only this acceptance with
`TEAQL_TRACE_CHAIN_SCENARIO=stream`; the full script always includes it.

Local streaming supports scalar rows, not relation-graph hydration: requesting
relations, relation aggregates, facets or graph enhancements rejects explicitly.
This does not establish TFP streaming or live external-provider conformance.

`ReadbackChecks` creates and then updates a three-object graph through generated
Q/Mutation APIs. Each save emits three physical writes and three authoritative
SELECTs, but still only three mutation commands and three committed audit
events. Reads retain the aggregate request root, branch lineage and a derived
purpose. A secret in a later child's value is masked from the root and sibling
write/readback intent; trusted values and subsequent independent requests remain
unchanged. Generated E and reloads verify values and versions. Run this case
alone with `TEAQL_TRACE_CHAIN_SCENARIO=readback`; full verification requires it.

Successful readbacks travel in ordered result metadata, not a duplicate observer
notification. Failed/cancelled reads retain the existing failure observer path.
Hard deletes and zero affected rows do not invent SELECT facts. Query-log and
mutation-log switches independently control their physical statement categories.

`LoadedPrivacyChecks` loads a complete parent/item graph, changes the same item
twice and then marks it for deletion. Prior private values are not SQL parameters
in those updates/deletes, but still must be removed from parent/sibling intent
and committed audit. A runtime-owned scalar snapshot advances only after commit;
it is neither a write payload nor shared Context state. Known public values
(including version numbers in ordinary prose) are not indiscriminately redacted.
Independent subsequent queries keep their own intent. The full verifier requires
this check; `TEAQL_TRACE_CHAIN_SCENARIO=loaded-privacy` selects it alone.

## Loaded graph rollback privacy

`LoadedPrivacyRollbackChecks` loads a complete parent/child graph, changes both
objects, and rejects the child's authoritative readback after two actual UPDATEs.
It requires safe failure diagnostics, no committed audit, unchanged stored data
and restored wrapper versions. The same wrappers then retry without reloading;
pending values must commit once, with old/new secrets still masked. A later
independent query must retain its own intent. The controlled script requires
this scenario on both starts of its retained database.

Run the focused probe with `TEAQL_TRACE_CHAIN_SCENARIO=loaded-privacy-rollback`.
Use a fresh SQLite path as well as retained-database starts. The private markers
are alphabetic so numeric ID redaction cannot accidentally hide a missing
old-value snapshot. Existing target-ID intent protection remains unchanged;
this probe does not authorize a change to that policy.
