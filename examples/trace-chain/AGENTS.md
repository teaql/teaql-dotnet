# trace-chain-service application instructions

Implement only application-owned code. Generated Models, Requests, Expressions,
Q, E and RuntimeModule are read-only. Do not read or search generated library
source for API discovery, including verification-only discovery.

Use current model-aware Assist for the next operation, not every operation:

```text
cargo teaql --input models/ dotnet-assist-query/ksml_entity
cargo teaql --input models/ dotnet-assist-query/ksml_entity.ksml_field
cargo teaql --input models/ dotnet-assist-create/ksml_entity
cargo teaql --input models/ dotnet-assist-update/ksml_entity
cargo teaql --input models/ dotnet-assist-delete/ksml_entity
cargo teaql --input models/ dotnet-assist-expression/ksml_entity
```

Replace `models/` with the actual model file or directory (for example, `model.xml`).
Replace entity and field placeholders with KSML snake_case names. If Assist
cannot supply the required operation, report MISSING_ASSIST; do not guess or
open generated source. Compiler diagnostics and executable tests validate usage.

Every query is bounded and has non-blank Comment and Purpose. Use generated Q
for loading and E for loaded traversal. Load all scalar fields before mutation;
mark deletions and SaveAsync the composed graph with a non-blank AuditAs reason.
Child AuditAs reasons remain local; children without a reason inherit their parent's lineage.
Generated public execution methods accept only UserContext. Schema changes are
explicit through context.EnsureSchemaAsync(), with generated bootstrap intact.

Verify the application twice on one SQLite file without database cleanup. Use
unique test labels. Keep generated library hashes unchanged and retain exact
commands, exit statuses and any skipped or unverified checks. Never claim a
published version from local source evidence.