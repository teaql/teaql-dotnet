#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
evidence="${TEAQL_FREIGHT_QUOTE_EVIDENCE_DIR:-}"
if [[ -z "$evidence" ]]; then
  evidence="$(mktemp -d -t teaql-dotnet-freight.XXXXXXXX)"
else
  mkdir "$evidence"
fi
evidence="$(cd "$evidence" && pwd)"
build="$(mktemp -d -t teaql-dotnet-freight-build.XXXXXXXX)"
cleanup() {
  status=$?
  trap - EXIT
  rm -rf -- "$build" || { [[ "$status" != 0 ]] || status=1; }
  exit "$status"
}
trap cleanup EXIT
trap 'exit 130' INT
trap 'exit 143' TERM

project="$example/dotnet-app-console/freight-quote-settlement-console.csproj"
flags=(-p:UseArtifactsOutput=true "-p:ArtifactsPath=$build"
  "-p:TeaQLRuntimeSourceRoot=$repo" -p:UseSharedCompilation=false)
dotnet build --disable-build-servers -m:2 "${flags[@]}" "$project"
binary="$(dotnet msbuild "$project" -nologo -getProperty:TargetPath "${flags[@]}")"
[[ "$binary" == "$build/"* && -f "$binary" ]]
cp "${binary%.dll}.deps.json" "$evidence/workspace.deps.json"
for phase in first restart; do
  dotnet "$binary" "$evidence/freight.sqlite" "$phase" | tee "$evidence/$phase.log"
done
mkdir "$evidence/calibration"
TEAQL_SCALAR_EVIDENCE_DIR="$evidence/calibration/scalar" TEAQL_SCALAR_DB="$evidence/calibration/scalar/scalar.sqlite" \
  TeaQLRuntimeSourceRoot="$repo" bash "$example/verify.sh" | tee "$evidence/calibration/scalar.log"
TEAQL_EMPTY_EVIDENCE_DIR="$evidence/calibration/empty" TEAQL_EMPTY_DB="$evidence/calibration/empty/empty-aggregate.sqlite" \
  TeaQLRuntimeSourceRoot="$repo" bash "$example/verify-empty.sh" | tee "$evidence/calibration/empty.log"
echo "PASS Freight quotes and scalar calibrations. Evidence at $evidence"
