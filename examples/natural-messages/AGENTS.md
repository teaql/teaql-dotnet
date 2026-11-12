# Natural Checker messages

Run `node scripts/verify-natural-messages.cjs --full --output /new/evidence`
from the repository root. Use `DOTNET_EXE` for an installed .NET 8 SDK.

Discover APIs through only the required `automatic-fixture/assist/` object,
action and field help. Do not inspect generated implementation to discover
APIs or edit it to make a test pass. The current fixture comes from
`DotnetNaturalMessagesGenerationTest`; its hashes and source provenance are
checked before and after execution. The historical `fixture/` is unchanged.

Use the original generated module/Checker and automatic metadata. Do not add
hand-built descriptors, reflection into private registries or a replacement
Checker. Customer message rendering is a separate presentation seam.
All Q requests are bounded with nonempty comment and purpose; writes use the
generated audited save. Keep Required and Context Fix semantics unchanged.

The verifier uses repository-local runtime projects, runs twice on one SQLite
database and preserves compact logs, hashes, wire responses and the database.
Only its own temporary build tree is removed after owned children exit.
