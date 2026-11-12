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

`automatic-fixture/` contains 12 unchanged generated files, the validated model,
passive metadata, provenance and six real object/field Assist responses. The
generated project selects repository-local runtime projects through the supported
`TeaQLRuntimeSourceRoot` property. No generated source or dependency manifest is
patched. The verifier records the complete source tree, not only runtime files.

An unchanged parent contains two children selected in descending ID order.
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
