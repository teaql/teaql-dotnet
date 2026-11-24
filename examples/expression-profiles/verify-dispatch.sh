#!/usr/bin/env bash
set -euo pipefail
example="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo="$(cd "$example/../.." && pwd)"
if [[ -n "${TEAQL_DISPATCH_PROFILE_EVIDENCE_DIR:-}" ]]; then
  evidence="$TEAQL_DISPATCH_PROFILE_EVIDENCE_DIR"
  mkdir "$evidence"
else
  evidence="$(mktemp -d -t teaql-dotnet-dispatch-profile.XXXXXXXX)"
fi
evidence="$(cd "$evidence" && pwd)"
scratch="$(mktemp -d -t teaql-dotnet-dispatch-build.XXXXXXXX)"
cleanup() {
  status=$?
  trap - EXIT
  rm -rf -- "$scratch" || { [[ "$status" != 0 ]] || status=1; }
  exit "$status"
}
trap cleanup EXIT
trap 'exit 130' INT
trap 'exit 143' TERM

for profile in camelCase snake_case PascalCase; do
  source="$example"
  if [[ "$profile" != camelCase ]]; then source="$example/profiles/$profile"; fi
  folder="$evidence/$profile"
  mkdir "$folder"
  project="$source/dotnet-app-console/dispatch-document-console.csproj"
  flags=(-p:UseArtifactsOutput=true "-p:ArtifactsPath=$scratch/$profile" "-p:TeaQLRuntimeSourceRoot=$repo" -p:UseSharedCompilation=false)
  dotnet build --disable-build-servers -m:2 "${flags[@]}" "$project" > "$folder/build.log" 2>&1 || {
    tail -80 "$folder/build.log"
    exit 1
  }
  binary="$(dotnet msbuild "$project" -nologo -getProperty:TargetPath "${flags[@]}")"
  [[ "$binary" == "$scratch/$profile/"* && -f "$binary" ]]
  cp "${binary%.dll}.deps.json" "$folder/workspace.deps.json"
  for phase in first restart; do
    dotnet "$binary" "$folder/dispatch.sqlite" "$profile" "$phase" | tee "$folder/$phase.log"
  done
done
TEAQL_GENERATED_E_PROFILE_EVIDENCE_DIR="${TEAQL_GENERATED_E_PROFILE_EVIDENCE_DIR:-$evidence/calibration}" node "$example/verify-local.cjs" | tee "$evidence/calibration.log"
