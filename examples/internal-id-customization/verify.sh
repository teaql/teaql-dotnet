#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
evidence="${TEAQL_INTERNAL_ID_EVIDENCE_DIR:-}"
if [[ -z "$evidence" ]]; then
  evidence="$(mktemp -d -t teaql-dotnet-warehouse-ids.XXXXXXXX)"
else
  mkdir "$evidence"
fi
build="$(mktemp -d -t teaql-dotnet-warehouse-build.XXXXXXXX)"
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
application="$app/warehouse-order-allocation-console.csproj"
calibration="$app/calibration/internal-id-customization.csproj"
for project in "$application" "$calibration"; do
  dotnet build --disable-build-servers -m:2 "${options[@]}" "$project"
done
binary="$(dotnet msbuild "$application" -nologo -getProperty:TargetPath "${options[@]}")"
legacy="$(dotnet msbuild "$calibration" -nologo -getProperty:TargetPath "${options[@]}")"
[[ "$binary" == "$build"/* && "$legacy" == "$build"/* ]]
cp "${binary%.dll}.deps.json" "$evidence/workspace.deps.json"
for phase in first restart; do
  dotnet "$binary" "$evidence/warehouse.sqlite" "$phase" | tee "$evidence/$phase.log"
done
dotnet "$legacy" | tee "$evidence/calibration.log"
echo "PASS Warehouse orders and internal ID calibration. Evidence at $evidence"
