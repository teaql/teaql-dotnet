#!/usr/bin/env bash
set -euo pipefail
umask 077
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
project="$example/dotnet-app-console/merchant-order-privacy-service-console.csproj"
library="$example/dotnet-lib-core/merchant-order-privacy-service-lib.csproj"
scratch="$(mktemp -d -t teaql-dotnet-order-privacy.XXXXXXXX)"
cleanup() {
  status=$?
  trap - EXIT
  python3 -B -c 'import pathlib,shutil,sys; p=pathlib.Path(sys.argv[1]); assert p.name.startswith("teaql-dotnet-order-privacy."); shutil.rmtree(p)' "$scratch"
  exit "$status"
}
trap cleanup EXIT
evidence="${TEAQL_ORDER_PRIVACY_EVIDENCE_DIR:-$scratch/evidence}"
[[ "$evidence" == /* ]]
mkdir -- "$evidence"
unset TEAQL_SQL_DEBUG_ENDPOINT TEAQL_AUDIT_DEBUG_ENDPOINT TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS
export TEAQL_LOG_FORMAT=json TEAQL_TELEMETRY_ENABLED=false
artifacts="$scratch/artifacts"
dotnet build --property:TeaQLRuntimeSourceRoot="$repo" --property:UseArtifactsOutput=true \
  --property:ArtifactsPath="$artifacts" --property:UseSharedCompilation=false "$project" >"$evidence/build.log" 2>&1 ||
  { tail -55 "$evidence/build.log" >&2; exit 1; }
python3 -B - "$artifacts" "$project" "$library" "$repo" >"$evidence/dependency-origins.json" <<'PY'
import json, pathlib, sys
artifacts, app, library, root = map(pathlib.Path, sys.argv[1:])
sources = ['TeaQL.Core', 'TeaQL.DataService', 'TeaQL.Runtime', 'TeaQL.Sql', 'TeaQL.Provider.Sqlite']
assets = [json.loads(p.read_text()) for p in artifacts.rglob('project.assets.json')]
by_path = {pathlib.Path(a['project']['restore']['projectPath']): a for a in assets}
def references(asset):
    return set(asset['project']['restore']['frameworks']['net8.0']['projectReferences'])
runtime_paths = {str(root / 'src' / name / (name + '.csproj')) for name in sources}
assert references(by_path[app]) == {str(library)}
assert references(by_path[library]) == runtime_paths
for name in sources:
    selected = [a for key, a in by_path[app]['libraries'].items() if key.split('/')[0] == name]
    assert len(selected) == 1 and selected[0]['type'] == 'project', name
print(json.dumps({'passed': True, 'domainProject': str(library), 'runtimeProjects': sorted(runtime_paths)}))
PY
assembly="$artifacts/bin/merchant-order-privacy-service-console/debug/merchant-order-privacy-service-console.dll"
[[ -f "$assembly" ]]
for round in 1 2; do
  if [[ "$round" == 2 ]]; then
    export TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS=I_UNDERSTAND_SENSITIVE_DATA_MAY_BE_WRITTEN_TO_DISK
  fi
  TEAQL_LOG_ENDPOINT="$evidence/runtime-$round.log" dotnet "$assembly" "$round" "$evidence/orders.sqlite" \
    "$evidence/ordinary-$round.log" "$evidence/debug-$round.log" >"$evidence/round-$round.log" 2>&1 ||
    { tail -30 "$evidence/round-$round.log" | sed -E 's/[[:alnum:]._-]+@example\.test/[REDACTED]/g' >&2; exit 1; }
  rg '^ORDER_PRIVACY=' "$evidence/round-$round.log"
done
