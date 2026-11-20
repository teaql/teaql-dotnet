# Natural Checker messages

This example uses an evaluated/generated Catalog and Entry library with the
repository-local runtime, SQLite, typed Q/E APIs and audited graph saves.
The generated module installs its original Checker and presentation metadata;
the application supplies neither replacements nor hand-built model descriptors.
It runs twice against the same database without deleting prior rows.

```sh
node scripts/verify-natural-messages.cjs --full --output /absolute/new/evidence
```

Use `DOTNET_EXE` to select an installed .NET 8 SDK. The default is `dotnet`.
The verifier retains source hashes, commands, test/build logs, rendered wire
violations and the SQLite database. It removes only its own temporary build
tree after its child processes exit. Existing evidence is never overwritten.
Runner lifecycle checks run with `node --test scripts/verify-natural-messages.test.cjs`.

`dotnet-lib-core/` contains 12 unchanged generated files, the validated model,
passive metadata, provenance and six real object/field Assist responses. The
generated project selects repository-local runtime projects through the supported
`TeaQLRuntimeSourceRoot` property. No generated source or dependency manifest is
patched. The verifier records the complete source tree, not only runtime files.

An unchanged parent contains two children selected in descending ID order.
Each child is created through `await request.NewEntityAsync(context)`; its ID
is positive before Save while its version still indicates it is unpersisted.
One sibling has a valid pending edit while the second has an invalid name.
Four rejection cases exercise Required in English and Chinese, the model's
100-unit string limit, and a safe customer presentation renderer. Required uses
deliberate null input, not an invented nonblank constraint. A private value of
101 Unicode scalars occupies 102 UTF-16 code units in .NET:

```text
The name of the 2nd entry must not exceed 100 UTF-16 code units; its current length is 102.
```

The machine pointer remains `/entryList/1/name`. Chinese says
`请填写第2项明细的名称。` for the required case. Masking preserves the original
count, and the customer renderer runs once despite graph relocation. Rejection
makes zero provider calls, including transaction begin, and commits no audit.
An empty audit reason is also rejected without I/O. Reverting both caller edits
produces no Context-only write. Correcting the second child writes and audits
only it; Q/E confirm its value, version and injected-clock update time while
the parent and sibling stay unchanged. A read-only SQLite oracle checks both runs.

Regenerate the current fixture using the generator's
`DotnetNaturalMessagesGenerationTest` on its recorded commit. Graph ancestry
uses `CheckResult.PrefixedBy` so private state and measurements survive relocation.
The previous manually composed `fixture/` stays untouched as historical source.
Complete example inventory and published artifacts remain separate gates.
Neither this example nor a local test run publishes packages.

Tracking: [teaql-dotnet #70](https://github.com/teaql/teaql-dotnet/issues/70).

## Current model and workspace

`model.xml` defines the Catalog and Entry business graph. The exact generated
domain library is read-only in `dotnet-lib-core/`; its current provenance points to
the qualified generator source build, with the prior generation identity retained
separately in its metadata. The application lives in `dotnet-app-console/showcase/Program.cs`.

Run this focused business entry from the SDK repository root:

```bash
node scripts/verify-natural-messages.cjs --example-only --output /new/absolute/evidence
```

The same graph is rejected before writes for an invalid second Entry, then
corrected and saved through generated Q/E and audited Save. Two runs check
actual stored values, precise machine locations, English/Chinese messages and
private-value redaction. Language-specific native length rules remain intact.
The verifier preserves result stores and compact receipts and removes owned
build outputs. Full runtime example qualification remains a separate gate.
