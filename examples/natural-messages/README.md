# Natural Checker messages

This example uses an evaluated/generated Catalog and Entry library with the
repository-local runtime, SQLite, typed Q/E APIs and audited graph saves.
It runs twice against the same database without deleting prior rows.

```sh
node scripts/verify-natural-messages.cjs --full --output /absolute/new/evidence
```

Use `DOTNET_EXE` to select an installed .NET 8 SDK. The default is `dotnet`.
The verifier retains source hashes, commands, test/build logs, rendered wire
violations and the SQLite database. It removes only its own temporary build
tree after its child processes exit. Existing evidence is never overwritten.
Runner lifecycle checks run with `node --test scripts/verify-natural-messages.test.cjs`.

The application attaches immutable presentation metadata through
`RuntimeModule.CheckerPresentation`. Automatic metadata generation is a separate
gate. `AppChecker` decorates the generated Checker and adds an eight-UTF-16-unit
constraint; it does not replace the generated validation rules. Context-derived
IP/actor fields use the same Context Fix customization as the existing example.

An unchanged parent contains two loaded children. Only the second is edited.
Its rejected private value produces this default English message:

```text
The private token of the 2nd entry must not exceed 8 UTF-16 code units; its current length is 10.
```

The machine pointer remains `/entryList/1/guardedToken`. Chinese rendering uses
the same structured violation and retains the original measurement after
masking. Rejection makes zero provider/audit calls. Correcting the value writes
and audits only that child; Q and E then verify the stored value and versions.

`fixture/` is generated source, not an editing surface. Its manifest seals all
11 generated files. Graph ancestry uses `CheckResult.PrefixedBy` so private
presentation state and measurements survive path relocation. Regenerate from
the generator's `DotnetContextFixGenerationTest` on its matching feature branch.
Neither this example nor a local test run publishes packages.
