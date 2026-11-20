#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
evidence="${TEAQL_SHIPPING_LOG_EVIDENCE_DIR:-}"
if [[ -z "$evidence" ]]; then
  evidence="$(mktemp -d -t teaql-dotnet-shipping-logs.XXXXXXXX)"
else
  mkdir "$evidence"
fi
build="$(mktemp -d -t teaql-dotnet-shipping-build.XXXXXXXX)"
cleanup() {
  status=$?
  trap - EXIT
  rm -rf -- "$build" || { [[ "$status" != 0 ]] || status=1; }
  exit "$status"
}
trap cleanup EXIT
trap 'exit 130' INT
trap 'exit 143' TERM

options=(-p:UseArtifactsOutput=true "-p:ArtifactsPath=$build"
  "-p:TeaQLRuntimeSourceRoot=$repo" -p:UseSharedCompilation=false)
app="$example/dotnet-app-console"
workspace="$app/shipping-order-logging-console.csproj"
calibration="$app/calibration/runtime-logging.csproj"
for project in "$workspace" "$calibration"; do
  dotnet build --disable-build-servers -m:2 "${options[@]}" "$project"
done
binary="$(dotnet msbuild "$workspace" -nologo -getProperty:TargetPath "${options[@]}")"
legacy="$(dotnet msbuild "$calibration" -nologo -getProperty:TargetPath "${options[@]}")"
[[ "$binary" == "$build"/* && "$legacy" == "$build"/* ]]
cp "${binary%.dll}.deps.json" "$evidence/workspace.deps.json"
unset TEAQL_ALLOW_SENSITIVE_PLAINTEXT_LOGS TEAQL_SQL_DEBUG_ENDPOINT TEAQL_AUDIT_DEBUG_ENDPOINT
export TEAQL_DOMAIN=shipping-order-logging TEAQL_TELEMETRY_ENABLED=false
for format in json human; do
  directory="$evidence/$format"
  mkdir "$directory"
  TEAQL_LOG_FORMAT="$format" TEAQL_LOG_ENDPOINT="$directory/ordinary.log" \
    dotnet "$binary" "$directory/shipment.sqlite" "$directory/safe-audit.json" | tee "$directory/application.log"
done
TMPDIR="$build" TEAQL_LOG_FORMAT=json TEAQL_LOG_ENDPOINT="$evidence/calibration-runtime.log" \
  dotnet "$legacy" | tee "$evidence/calibration.log"
echo "PASS Shipping order logging and original privacy calibration. Evidence at $evidence"
