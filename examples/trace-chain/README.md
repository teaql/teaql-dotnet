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
| Normative six-mutation graph | Root, ordinary item, Payment, inherited PaymentAttempt, Shipment and deleted item retain their own ordered lineage in emitted commands, safe SQL and committed audit |
| Three-level generated Q and E | PaymentAttempt → Payment → CustomerOrder → Platform loads safely; four physical statements retain the same origin and qualified relation frames; unselected E access fails |
| Actual provider failure | SQLite UNIQUE violation retains failed SQL lineage, rolls back the graph and emits no committed audit |
| Readback failure | An injected authoritative fetch failure does not erase the successful write statement; the graph transaction rolls back and emits no committed audit |
| Same Context concurrency | Two overlapping Task-based generated saves are serialized by the transaction gate while retaining independent root and branch reasons |

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

`model.xml`, the evaluated report, current Assist and producer-generated
`AGENTS.md` are retained. Application API discovery uses those instructions and
Assist, not generated source. To regenerate from the producer checkout:

```bash
mvn -B -pl generator -am -Dtest=DotnetTraceChainExampleGenerationTest \
  -Dsurefire.failIfNoSpecifiedTests=false \
  -Dteaql.dotnet.runtime=/absolute/path/to/teaql-dotnet test
```

This is selected producer coverage. Prepared batch/item-index lineage, detached
children, late low-level allocation, every entry point and privacy mode, reentrant
root fail-fast behavior and immutable internal Registry replay remain open.
