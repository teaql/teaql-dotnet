#!/usr/bin/env bash
set -euo pipefail
repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
example="$repo/examples/school-management"
if [[ -n "${TEAQL_SCHOOL_BOOTSTRAP_EVIDENCE_DIR:-}" ]]; then
  evidence="$TEAQL_SCHOOL_BOOTSTRAP_EVIDENCE_DIR"
  mkdir "$evidence"
else
  evidence="$(mktemp -d -t teaql-dotnet-bootstrap.XXXXXXXX)"
fi
owned_artifacts=''
cleanup() {
  status=$?
  trap - EXIT
  if [[ -n "$owned_artifacts" ]]; then rm -rf -- "$owned_artifacts"; fi
  exit "$status"
}
trap cleanup EXIT
if [[ -z "${TEAQL_SCHOOL_ARTIFACTS_DIR:-}" ]]; then
  owned_artifacts="$(mktemp -d -t teaql-dotnet-bootstrap-build.XXXXXXXX)"
  export TEAQL_SCHOOL_ARTIFACTS_DIR="$owned_artifacts/artifacts"
fi
properties=(--property:TeaQLRuntimeSourceRoot="${TeaQLRuntimeSourceRoot:-$repo}"
  --property:UseArtifactsOutput=true --property:ArtifactsPath="$TEAQL_SCHOOL_ARTIFACTS_DIR")
fingerprint() {
  { rg --files "$example/dotnet-lib-core/Models" "$example/dotnet-lib-core/Requests"; printf '%s\n' "$example/dotnet-lib-core/Q.cs" "$example/dotnet-lib-core/E.cs" "$example/dotnet-lib-core/GeneratedRuntimeModule.cs"; } |
    LC_ALL=C sort | xargs -d '\n' sha256sum
}
fingerprint > "$evidence/library-before.sha256"
dotnet build "${properties[@]}" \
  "$example/dotnet-app-console/school-management-service-console.csproj"
for logging in on off; do
  for round in 1 2; do
    TEAQL_SCHOOL_BOOTSTRAP_DB="$evidence/$logging.sqlite" TEAQL_SCHOOL_BOOTSTRAP_LOGGING="$logging" \
      dotnet run --no-build --no-restore "${properties[@]}" \
      --project "$example/dotnet-app-console/school-management-service-console.csproj" | tee "$evidence/$logging-$round.log"
    expected_logging=True; [[ "$logging" == off ]] && expected_logging=False
    fresh=True; version=1
    if [[ "$round" == 2 ]]; then fresh=False; version=3; fi
    rg -Fq "PASS .NET generated bootstrap trace logging=$expected_logging fresh=$fresh originalVersion=$version" "$evidence/$logging-$round.log"
    fingerprint > "$evidence/library-after.sha256"
    cmp "$evidence/library-before.sha256" "$evidence/library-after.sha256"
  done
done
echo "PASS .NET generated bootstrap twice per logging mode; retained evidence: $evidence"
