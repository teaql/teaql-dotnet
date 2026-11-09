# Generated Task Board example

This four-object SQLite example uses generated Q, E and audited Mutation APIs.
`context.EnsureSchemaAsync()` creates the schema and provisions Platform plus
four TaskStatus constants. The application creates and reloads a task, verifies
its planned status with E, renames it with optimistic versioning and saves a
linked TaskExecutionLog. A missing audit comment is rejected before saving.

## Run locally

From the runtime repository root:

```bash
dotnet run --project examples/task_board/TaskBoardExample/TaskBoardExample.csproj
```

The default database is in memory. Set `TEAQL_TASK_BOARD_DB` to an absolute SQLite
file path to retain data; its parent directory must already exist. Repeated runs
keep the same root and constants and add one task and one history record per run.
The example never deletes that database. Success ends with
`PASS .NET generated Task Board` and a structured `DOTNET_TASK_BOARD` result.

Nullable diagnostics are compiler errors for this project. The complete
`bash scripts/verify-examples.sh` gate enforces that rule across all examples.

## Model and generated source

[model.xml](model.xml) is a local SQLite variant of the generator's robot-kanban
fixture. Its execution log uses SQLite rather than the original Meilisearch
route. [fixture-manifest.json](TaskBoardExample/Generated/fixture-manifest.json)
binds all fourteen library files, the model hash and the evaluated producer.
`node scripts/verify-generated-fixtures.cjs` verifies those hashes along with
the other retained generated libraries. Change the model and regenerate the
library; do not edit generated C# to satisfy the application.

The application contains no handwritten DDL or data insertion SQL. It depends
on this repository's runtime projects, not a published runtime package. The
complete current fixture lives under `TaskBoardExample/Generated`; it replaces
the earlier four untracked-provenance DTO definitions.
